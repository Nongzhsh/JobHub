using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobHub.JobSearch.Contracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Volo.Abp.Caching;

namespace JobHub
{
    public class JobSearchAppService : JobHubAppService, IJobSearchAppService
    {
        private readonly IDictionary<string, string> _cacheKeyDict = new ConcurrentDictionary<string, string>();
        private readonly IDistributedCache<JobListResultDto> _jobListCache;
        private readonly JobSearchOptions _jobSearchOptions;
        public JobSearchAppService(IDistributedCache<JobListResultDto> jobListCache,
            IOptions<JobSearchOptions> options)
        {
            _jobListCache = jobListCache;
            _jobSearchOptions = options.Value;
        }

        public async Task ClearCacheAsync(string cacheKey)
        {
            var prefix = cacheKey.Left(cacheKey.LastIndexOf('@'));
            var keys = _cacheKeyDict.Keys.Where(x => x.StartsWith(prefix)).ToList();
            foreach (var key in keys)
            {
                await _jobListCache.RemoveAsync(key);
                _cacheKeyDict.Remove(key);
            }
        }

        public async Task<JobListResultDto> GetJobsAsync(GetJobsInput input)
        {
            var cacheKey = $"input@{input.JobSource}@{input.City}@{input.Keyword}@{input.PageIndex}";
            var output = await _jobListCache.GetOrAddAsync(
                cacheKey,
                factory: async () =>
                {
                    if (input.JobSource.IsNullOrEmpty() || !_jobSearchOptions.JobSearchers.TryGetValue(input.JobSource, out var jobSearcherType))
                    {
                        return new JobListResultDto();
                    }

                    var jobSearcher = (IJobSearcher)ServiceProvider.GetService(jobSearcherType);
                    var jobList = await jobSearcher.SearchAsync(input);
                    jobList.CacheKey = cacheKey;

                    _cacheKeyDict[cacheKey] = cacheKey;

                    return jobList;
                },
                optionsFactory: () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _jobSearchOptions.CacheAbsoluteExpirationRelativeToNow,
                    SlidingExpiration = _jobSearchOptions.CacheSlidingExpiration
                }
            );

            return output;
        }
    }
}

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
        protected static IDictionary<string, string> CacheKeyDict = new ConcurrentDictionary<string, string>();

        private readonly IDistributedCache<JobListResultDto> _jobListCache;
        private readonly JobSearchOptions _jobSearchOptions;
        public JobSearchAppService(IDistributedCache<JobListResultDto> jobListCache,
            IOptions<JobSearchOptions> options)
        {
            _jobListCache = jobListCache;
            _jobSearchOptions = options.Value;
        }

        public async Task ClearAllCacheAsync()
        {
            var keys = CacheKeyDict.Keys.ToList();
            foreach (var key in keys)
            {
                await _jobListCache.RemoveAsync(key);
                CacheKeyDict.Remove(key);
            }
        }

        public async Task ClearCacheAsync(string cacheKey)
        {
            if (CacheKeyDict.ContainsKey(cacheKey))
            {
                await _jobListCache.RemoveAsync(cacheKey);
                CacheKeyDict.Remove(cacheKey);
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

                    CacheKeyDict[cacheKey] = cacheKey;

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

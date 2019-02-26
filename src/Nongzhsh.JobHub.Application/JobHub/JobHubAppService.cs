using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;

namespace Nongzhsh.JobHub.JobHub
{
    public class JobHubAppService : ApplicationService
    {
        protected IDistributedCache<JobListResultDto> JobListCache { get; }

        private readonly LiepinJobSearcher _liepinJobSearcher;
        private readonly ZhilianJobSearcher _zhilianJobSearcher;
        private readonly QianchengJobSearcher _qianchengJobSearcher;
        private readonly BossJobSearcher _bossJobSearcher;
        private readonly LagouJobSearcher _lagouJobSearcher;

        public JobHubAppService(
            IDistributedCache<JobListResultDto> jobCache,
            LiepinJobSearcher liepinJobSearcher,
            ZhilianJobSearcher zhilianJobSearcher,
            QianchengJobSearcher qianchengJobSearcher,
            BossJobSearcher bossJobSearcher,
            LagouJobSearcher lagouJobSearcher)
        {
            JobListCache = jobCache;
            _liepinJobSearcher = liepinJobSearcher;
            _zhilianJobSearcher = zhilianJobSearcher;
            _qianchengJobSearcher = qianchengJobSearcher;
            _bossJobSearcher = bossJobSearcher;
            _lagouJobSearcher = lagouJobSearcher;
        }

        public async Task ClearCacheAsync(ClearCacheInput input)
        {
            await JobListCache.RemoveAsync(input.CacheKey);
        }

        public async Task<JobListResultDto> GetJobsAsync(GetJobsInput input)
        {
            var cacheKey = $"Job@{input.Keyword?.Trim().ToLower()}@{input.City}@{input.JobSource}@{input.PageIndex}@{input.PageSize}";

            var output = await JobListCache.GetOrAddAsync(
                cacheKey,
                factory: async () =>
                {
                    JobListResultDto jobList;

                    switch(input.JobSource)
                    {
                        case JobSource.智联招聘:
                            jobList = await _zhilianJobSearcher.SearchAsync(input);
                            break;
                        case JobSource.前程无忧:
                            jobList = await _qianchengJobSearcher.SearchAsync(input);
                            break;
                        case JobSource.猎聘招聘:
                            jobList = await _liepinJobSearcher.SearchAsync(input);
                            break;
                        case JobSource.Boss招聘:
                            jobList = await _bossJobSearcher.SearchAsync(input);
                            break;
                        case JobSource.拉钩招聘:
                            jobList = await _lagouJobSearcher.SearchAsync(input);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    jobList.CacheKey = cacheKey;
                    return jobList;
                },
                optionsFactory: () => new DistributedCacheEntryOptions
                {
                    //TODO: 应该是可配置的
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2),
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                }
            );

            return output;
        }
    }
}
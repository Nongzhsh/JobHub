using System.Threading.Tasks;
using JobHub.JobSearch.Contracts;

namespace JobHub
{
    public interface IJobSearchAppService
    {
        Task ClearAllCacheAsync();
        Task ClearCacheAsync(string cacheKey);
        Task<JobListResultDto> GetJobsAsync(GetJobsInput input);
    }
}

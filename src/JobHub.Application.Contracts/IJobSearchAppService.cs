using System.Threading.Tasks;
using JobHub.JobSearch.Contracts;

namespace JobHub
{
    public interface IJobSearchAppService
    {
        Task ClearCacheAsync(string cacheKey);

        Task<JobListResultDto> GetJobsAsync(GetJobsInput input);
    }
}

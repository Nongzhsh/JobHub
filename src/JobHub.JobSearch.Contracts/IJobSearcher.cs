using System.Threading.Tasks;

namespace JobHub.JobSearch.Contracts
{
    public interface IJobSearcher
    {
        Task<JobListResultDto> SearchAsync(GetJobsInput input);
    }
}

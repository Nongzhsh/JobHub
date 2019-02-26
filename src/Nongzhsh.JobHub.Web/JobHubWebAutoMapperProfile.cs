using AutoMapper;
using Nongzhsh.JobHub.JobHub;
using Nongzhsh.JobHub.Pages.JobHub;

namespace Nongzhsh.JobHub
{
    public class JobHubWebAutoMapperProfile : Profile
    {
        public JobHubWebAutoMapperProfile()
        {
            //Configure your AutoMapper mapping configuration here...
             CreateMap<IndexModel.SearchFormModalView, GetJobsInput>();
        }
    }
}

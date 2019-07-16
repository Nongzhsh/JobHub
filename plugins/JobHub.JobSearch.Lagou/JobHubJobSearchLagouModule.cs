using JobHub.JobSearch.Contracts;
using Volo.Abp.Json;
using Volo.Abp.Modularity;

namespace JobHub.JobSearch.Lagou
{
    [DependsOn(typeof(AbpJsonModule))]
    [DependsOn(typeof(JobHubJobSearchContractsModule))]
    public class JobHubJobSearchLagouModule : AbpModule
    {
    }
}

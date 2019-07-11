using JobHub.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace JobHub
{
    [DependsOn(
        typeof(JobHubEntityFrameworkCoreTestModule)
        )]
    public class JobHubDomainTestModule : AbpModule
    {

    }
}
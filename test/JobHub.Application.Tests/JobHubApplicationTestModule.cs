using Volo.Abp.Modularity;

namespace JobHub
{
    [DependsOn(
        typeof(JobHubApplicationModule),
        typeof(JobHubDomainTestModule)
        )]
    public class JobHubApplicationTestModule : AbpModule
    {

    }
}
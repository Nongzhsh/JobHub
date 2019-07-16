using JobHub.JobSearch.Contracts;
using Volo.Abp.Json;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace JobHub.JobSearch.Boss
{
    [DependsOn(typeof(AbpJsonModule))]
    [DependsOn(typeof(AbpVirtualFileSystemModule))]
    [DependsOn(typeof(JobHubJobSearchContractsModule))]
    public class JobHubJobSearchBossModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<VirtualFileSystemOptions>(options =>
            {
                // https://docs.abp.io/en/abp/latest/Virtual-File-System
                options.FileSets.AddEmbedded<JobHubJobSearchBossModule>();
            });
        }
    }
}

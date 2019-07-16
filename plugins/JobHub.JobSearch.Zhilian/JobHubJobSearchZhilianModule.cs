using JobHub.JobSearch.Contracts;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

namespace JobHub.JobSearch.Zhilian
{
    [DependsOn(typeof(AbpCachingModule))]
    [DependsOn(typeof(AbpVirtualFileSystemModule))]
    [DependsOn(typeof(JobHubJobSearchContractsModule))]
    public class JobHubJobSearchZhilianModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<VirtualFileSystemOptions>(options =>
            {
                // https://docs.abp.io/en/abp/latest/Virtual-File-System
                options.FileSets.AddEmbedded<JobHubJobSearchZhilianModule>();
            });
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace JobHub.EntityFrameworkCore
{
    [DependsOn(
        typeof(JobHubEntityFrameworkCoreModule)
        )]
    public class JobHubEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<JobHubMigrationsDbContext>();
        }
    }
}

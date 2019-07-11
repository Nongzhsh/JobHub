using JobHub.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace JobHub.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(JobHubEntityFrameworkCoreDbMigrationsModule),
        typeof(JobHubApplicationContractsModule)
        )]
    public class JobHubDbMigratorModule : AbpModule
    {
        
    }
}

using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace JobHub.HttpApi.Client.ConsoleTestApp
{
    [DependsOn(
        typeof(JobHubHttpApiClientModule),
        typeof(AbpHttpClientIdentityModelModule)
        )]
    public class JobHubConsoleApiClientModule : AbpModule
    {
        
    }
}

using Volo.Abp;

namespace Nongzhsh.JobHub
{
    public abstract class JobHubApplicationTestBase : AbpIntegratedTest<JobHubApplicationTestModule>
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}

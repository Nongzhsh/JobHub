using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace JobHub.Data
{
    /* This is used if database provider does't define
     * IJobHubDbSchemaMigrator implementation.
     */
    public class NullJobHubDbSchemaMigrator : IJobHubDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}
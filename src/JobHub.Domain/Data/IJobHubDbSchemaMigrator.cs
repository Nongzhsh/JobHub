using System.Threading.Tasks;

namespace JobHub.Data
{
    public interface IJobHubDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}

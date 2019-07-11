using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JobHub.Data;
using Volo.Abp.DependencyInjection;

namespace JobHub.EntityFrameworkCore
{
    [Dependency(ReplaceServices = true)]
    public class EntityFrameworkCoreJobHubDbSchemaMigrator 
        : IJobHubDbSchemaMigrator, ITransientDependency
    {
        private readonly JobHubMigrationsDbContext _dbContext;

        public EntityFrameworkCoreJobHubDbSchemaMigrator(JobHubMigrationsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task MigrateAsync()
        {
            await _dbContext.Database.MigrateAsync();
        }
    }
}
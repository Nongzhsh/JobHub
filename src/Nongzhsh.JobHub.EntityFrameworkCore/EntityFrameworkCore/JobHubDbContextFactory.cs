using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nongzhsh.JobHub.EntityFrameworkCore
{
    public class JobHubDbContextFactory : IDesignTimeDbContextFactory<JobHubDbContext>
    {
        public JobHubDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();

            var builder = new DbContextOptionsBuilder<JobHubDbContext>()
                .UseSqlServer(configuration.GetConnectionString("Default"));

            return new JobHubDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Nongzhsh.JobHub.Web/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace JobHub.JobSearch.Contracts
{
    public class JobHubJobSearchContractsModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AddJobSearchers(context.Services);
        }

        private static void AddJobSearchers(IServiceCollection services)
        {
            var types = new List<Type>();

            services.OnRegistred(context =>
            {
                if (typeof(IJobSearcher).IsAssignableFrom(context.ImplementationType))
                {
                    types.Add(context.ImplementationType);
                }
            });

            services.Configure<JobSearchOptions>(options =>
            {
                foreach (var type in types)
                {
                    var filterName = JobSearcherNameAttribute.GetName(type);
                    options.JobSearchers[filterName] = type;
                }
            });
        }
    }
}

using System;
using System.Collections.Generic;

namespace JobHub.JobSearch.Contracts
{
    public class JobSearchOptions
    {
        public TimeSpan CacheAbsoluteExpirationRelativeToNow { get; set; } = TimeSpan.FromHours(2);
        public TimeSpan CacheSlidingExpiration { get; set; } = TimeSpan.FromMinutes(30);
        public Dictionary<string, Type> JobSearchers { get; } = new Dictionary<string, Type>();
    }
}

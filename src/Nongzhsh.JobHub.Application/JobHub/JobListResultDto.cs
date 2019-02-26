using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Nongzhsh.JobHub.JobHub
{
    [Serializable]
    public class JobListResultDto : PagedResultDto<JobDto>
    {
        public string CacheKey { get; set; }

        public string NextRequestExtendedData { get; set; }

        public JobListResultDto(IReadOnlyList<JobDto> items, long totalCount = 1000) : base(totalCount, items)
        {
        }

        public JobListResultDto() { }
    }
}

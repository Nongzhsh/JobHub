using System;
using System.Collections.Generic;

namespace JobHub.JobSearch.Contracts
{
    [Serializable]
    public class JobListResultDto
    {
        public IReadOnlyList<JobDto> Items
        {
            get => _items ?? (_items = new List<JobDto>());
            set => _items = value;
        }
        private IReadOnlyList<JobDto> _items;

        public long TotalCount { get; set; }

        public string CacheKey { get; set; }

        public JobListResultDto(IReadOnlyList<JobDto> items, long totalCount = 1000)
        {
            Items = items;
            TotalCount = totalCount;
        }

        public JobListResultDto() { }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nongzhsh.JobHub.JobHub;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;

namespace Nongzhsh.JobHub.Pages.JobHub
{
    public class IndexModel : JobHubPageModelBase
    {
        private readonly JobHubAppService _jobHubAppService;

        public JobListResultDto Jobs { get; set; }

        [BindProperty(SupportsGet = true)]
        public SearchFormModalView Search { get; set; }

        public PagerModel PagerModel { get; set; }

        public IndexModel(JobHubAppService jobHubAppService)
        {
            _jobHubAppService = jobHubAppService;
        }

        public async Task OnGetAsync(int currentPage = 1)
        {
            var input = ObjectMapper.Map<SearchFormModalView, GetJobsInput>(Search);
            input.PageIndex = currentPage;
            input.PageSize = 15; //TODO：应该是可配置的
            input.ExtendedData = TempData.Peek("ExtendedData")?.ToString();

            Jobs = await _jobHubAppService.GetJobsAsync(input);
            TempData["ExtendedData"] = Jobs.NextRequestExtendedData;
            PagerModel = new PagerModel(Jobs.TotalCount, 0, input.PageIndex, input.PageSize, "/JobHub/Index");
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("Index", Search);
        }

        public async Task<IActionResult> OnPostClearCacheAsync(string cacheKey)
        {
            await _jobHubAppService.ClearCacheAsync(new ClearCacheInput { CacheKey = cacheKey });

            return RedirectToPage("Index", Search);
        }

        public class SearchFormModalView
        {
            [Required]
            public string Keyword { get; set; } = ".Net";

            public string City { get; set; }

            /// <summary>
            /// 月薪范围，如：1001,2000 
            /// </summary>
            public string Salary { get; set; } //TODO：年薪-月薪计算标准？

            public JobSource JobSource { get; set; }
        }
    }
}

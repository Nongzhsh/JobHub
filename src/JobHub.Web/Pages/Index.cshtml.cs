using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JobHub.JobSearch.Contracts;
using JobHub.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Pagination;

namespace JobHub.Web.Pages
{
    public class IndexModel : JobHubPageModel
    {
        private readonly IJobSearchAppService _jobSearchAppService;

        public JobListResultDto Jobs { get; set; }

        [Required]
        [BindProperty(SupportsGet = true)]
        public string Keyword { get; set; } = ".Net";

        [BindProperty(SupportsGet = true)]
        public string City { get; set; } = "广州";

        [BindProperty(SupportsGet = true)]
        public string JobSource { get; set; }

        public PagerModel PagerModel { get; set; }

        public List<SelectListItem> JobSourceItems { get; }

        public IndexModel(IJobSearchAppService jobSearchAppService, IOptions<JobSearchOptions> options)
        {
            _jobSearchAppService = jobSearchAppService;

            JobSourceItems = options.Value.JobSearchers.Select(x => new SelectListItem(x.Key, x.Key)).ToList();
        }

        public async Task OnGetAsync(int currentPage = 1)
        {
            JobSource = JobSource ?? JobSourceItems.FirstOrDefault()?.Value;

            var input = new GetJobsInput
            {
                Keyword = Keyword,
                JobSource = JobSource,
                City = City,
                PageIndex = currentPage,
                PageSize = 15
            };

            if (TempData.ContainsKey("ExtraProperties"))
            {
                var extraProperties = TempData.Peek<IDictionary<string, object>>("ExtraProperties");
                foreach (var (key, value) in extraProperties)
                {
                    input.ExtraProperties[key] = value;
                }
            }

            Jobs = await _jobSearchAppService.GetJobsAsync(input);

            TempData.Put("ExtraProperties", input.ExtraProperties);

            PagerModel = new PagerModel(Jobs.TotalCount, 0, input.PageIndex, input.PageSize, "/Index");
        }

        public async Task<IActionResult> OnPostClearCacheAsync(string cacheKey)
        {
            await _jobSearchAppService.ClearCacheAsync(cacheKey);
            return RedirectToPage("Index", new { JobSource, Keyword, City });
        }
    }
}

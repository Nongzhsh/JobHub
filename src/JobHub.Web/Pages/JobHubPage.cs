using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using JobHub.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace JobHub.Web.Pages
{
    /* Inherit your UI Pages from this class. To do that, add this line to your Pages (.cshtml files under the Page folder):
     * @inherits JobHub.Web.Pages.JobHubPage
     */
    public abstract class JobHubPage : AbpPage
    {
        [RazorInject]
        public IHtmlLocalizer<JobHubResource> L { get; set; }
    }
}

using JobHub.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace JobHub.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class JobHubPageModel : AbpPageModel
    {
        protected JobHubPageModel()
        {
            LocalizationResourceType = typeof(JobHubResource);
        }
    }
}
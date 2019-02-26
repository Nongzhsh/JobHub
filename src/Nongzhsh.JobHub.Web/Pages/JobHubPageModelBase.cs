using Nongzhsh.JobHub.Localization.JobHub;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Nongzhsh.JobHub.Pages
{
    public abstract class JobHubPageModelBase : AbpPageModel
    {
        protected JobHubPageModelBase()
        {
            LocalizationResourceType = typeof(JobHubResource);
        }
    }
}
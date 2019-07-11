using JobHub.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace JobHub.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class JobHubController : AbpController
    {
        protected JobHubController()
        {
            LocalizationResource = typeof(JobHubResource);
        }
    }
}
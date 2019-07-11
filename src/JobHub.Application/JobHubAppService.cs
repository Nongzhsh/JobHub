using System;
using System.Collections.Generic;
using System.Text;
using JobHub.Localization;
using Volo.Abp.Application.Services;

namespace JobHub
{
    /* Inherit your application services from this class.
     */
    public abstract class JobHubAppService : ApplicationService
    {
        protected JobHubAppService()
        {
            LocalizationResource = typeof(JobHubResource);
        }
    }
}

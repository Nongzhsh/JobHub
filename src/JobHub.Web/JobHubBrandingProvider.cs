using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace JobHub.Web
{
    [Dependency(ReplaceServices = true)]
    public class JobHubBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "JobHub";
    }
}

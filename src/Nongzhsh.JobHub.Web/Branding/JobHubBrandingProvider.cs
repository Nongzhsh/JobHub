using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Components;
using Volo.Abp.DependencyInjection;

namespace Nongzhsh.JobHub.Branding
{
    [Dependency(ReplaceServices = true)]
    public class JobHubBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "JobHub";
    }
}

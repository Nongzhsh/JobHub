using JobHub.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace JobHub.Permissions
{
    public class JobHubPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(JobHubPermissions.GroupName);

            //Define your own permissions here. Example:
            //myGroup.AddPermission(JobHubPermissions.MyPermission1, L("Permission:MyPermission1"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<JobHubResource>(name);
        }
    }
}

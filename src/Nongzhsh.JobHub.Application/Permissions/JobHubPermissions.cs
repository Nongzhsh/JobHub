using System;

namespace Nongzhsh.JobHub.Permissions
{
    public static class JobHubPermissions
    {
        public const string GroupName = "JobHub";

        //Add your own permission names. Example:
        //public const string MyPermission1 = GroupName + ".MyPermission1";

        public static string[] GetAll()
        {
            //Return an array of all permissions
            return Array.Empty<string>();
        }
    }
}
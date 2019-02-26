using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nongzhsh.JobHub.Localization.JobHub;
using Volo.Abp.UI.Navigation;
using Volo.Blogging;

namespace Nongzhsh.JobHub.Menus
{
    public class JobHubMenuContributor : IMenuContributor
    {
        public async Task ConfigureMenuAsync(MenuConfigurationContext context)
        {
            if(context.Menu.Name == StandardMenus.Main)
            {
                await ConfigureMainMenuAsync(context);
            }
        }

        private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
        {
            var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<JobHubResource>>();

            context.Menu.Items.Insert(0, new ApplicationMenuItem("JobHub.Home", l["Menu:Home"], "/"));

            //context.Menu.AddItem(new ApplicationMenuItem("JobHub.JobHub", l["Menu:JobHub"], "/JobHub"));

            var authorizationService = context.ServiceProvider.GetRequiredService<IAuthorizationService>();
            if(!await authorizationService.IsGrantedAsync(BloggingPermissions.Blogs.Default))
            {
                //允许未登录用户查看博客文章
                var blogMenuItem = new ApplicationMenuItem("Blogs", l["Menu:Blogs"], "/Blog");
                context.Menu.AddItem(blogMenuItem);

                var rootMenuItem = new ApplicationMenuItem("Account", l["Login"], "/Account/Login");
                context.Menu.AddItem(rootMenuItem);
            }
        }
    }
}

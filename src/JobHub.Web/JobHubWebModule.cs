using System.IO;
using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JobHub.EntityFrameworkCore;
using JobHub.JobSearch.Boss;
using JobHub.JobSearch.Liepin;
using JobHub.JobSearch.Qiancheng;
using JobHub.JobSearch.Zhilian;
using JobHub.Localization;
using JobHub.MultiTenancy;
using JobHub.Web.Menus;
using Swashbuckle.AspNetCore.Swagger;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity.Web;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Modularity.PlugIns;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.Ui.Navigation.Urls;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace JobHub.Web
{
    [DependsOn(
        typeof(JobHubJobSearchBossModule),
        typeof(JobHubJobSearchZhilianModule),
        typeof(JobHubJobSearchLiepinModule),
        typeof(JobHubJobSearchQianchengModule),
        typeof(JobHubHttpApiModule),
        typeof(JobHubApplicationModule),
        typeof(JobHubEntityFrameworkCoreDbMigrationsModule),
        typeof(AbpAutofacModule),
        typeof(AbpIdentityWebModule),
        typeof(AbpAccountWebIdentityServerModule),
        typeof(AbpAspNetCoreMvcUiBasicThemeModule),
        typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
        typeof(AbpTenantManagementWebModule)
        )]
    public class JobHubWebModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(
                    typeof(JobHubResource),
                    typeof(JobHubDomainModule).Assembly,
                    typeof(JobHubDomainSharedModule).Assembly,
                    typeof(JobHubApplicationModule).Assembly,
                    typeof(JobHubApplicationContractsModule).Assembly,
                    typeof(JobHubWebModule).Assembly
                );
            });
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            var configuration = context.Services.GetConfiguration();

            ConfigureUrls(configuration);
            ConfigureAuthentication(context, configuration);
            ConfigureAutoMapper();
            ConfigureVirtualFileSystem(hostingEnvironment);
            ConfigureLocalizationServices();
            ConfigureNavigationServices();
            ConfigureAutoApiControllers();
            ConfigureSwaggerServices(context.Services);

            // TODO: abp 的插件机制好像没有完善，它无法加载插件所依赖的包，先暂停插件模式
            // ConfigurePlugIns(context);
        }

        private void ConfigureUrls(IConfigurationRoot configuration)
        {
            Configure<AppUrlOptions>(options =>
            {
                options.Applications["MVC"].RootUrl = configuration["AppSelfUrl"];
            });
        }

        private void ConfigureAuthentication(ServiceConfigurationContext context, IConfigurationRoot configuration)
        {
            context.Services.AddAuthentication()
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = configuration["AuthServer:Authority"];
                    options.RequireHttpsMetadata = false;
                    options.ApiName = "JobHub";
                });
        }

        private void ConfigureAutoMapper()
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                /* use `true` for the `validate` parameter if you want to
                 * validate the profile on application startup.
                 * See http://docs.automapper.org/en/stable/Configuration-validation.html for more
                 * about configuration validation.
                 */
                options.AddProfile<JobHubWebAutoMapperProfile>();
            });
        }

        private void ConfigureVirtualFileSystem(IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                Configure<VirtualFileSystemOptions>(options =>
                {
                    options.FileSets.ReplaceEmbeddedByPhysical<JobHubDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}JobHub.Domain.Shared", Path.DirectorySeparatorChar)));
                    options.FileSets.ReplaceEmbeddedByPhysical<JobHubDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}JobHub.Domain", Path.DirectorySeparatorChar)));
                    options.FileSets.ReplaceEmbeddedByPhysical<JobHubApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}JobHub.Application.Contracts", Path.DirectorySeparatorChar)));
                    options.FileSets.ReplaceEmbeddedByPhysical<JobHubApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, string.Format("..{0}JobHub.Application", Path.DirectorySeparatorChar)));
                    options.FileSets.ReplaceEmbeddedByPhysical<JobHubWebModule>(hostingEnvironment.ContentRootPath);
                });
            }
        }

        private void ConfigureLocalizationServices()
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<JobHubResource>()
                    .AddBaseTypes(
                        typeof(AbpUiResource)
                    );

                options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
                options.Languages.Add(new LanguageInfo("en", "en", "English"));
                options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
                options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
                options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            });
        }

        private void ConfigureNavigationServices()
        {
            Configure<NavigationOptions>(options =>
            {
                options.MenuContributors.Add(new JobHubMenuContributor());
            });
        }

        private void ConfigureAutoApiControllers()
        {
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(JobHubApplicationModule).Assembly);
            });
        }

        private void ConfigureSwaggerServices(IServiceCollection services)
        {
            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new Info { Title = "JobHub API", Version = "v1" });
                    options.DocInclusionPredicate((docName, description) => true);
                    options.CustomSchemaIds(type => type.FullName);
                }
            );
        }

        private void ConfigurePlugIns(ServiceConfigurationContext context)
        {
            var hostingEnvironment = context.Services.GetHostingEnvironment();
            context.Services.AddApplication<JobHubWebModule>(options =>
            {
                options.PlugInSources.AddFolder(Path.Combine(hostingEnvironment.ContentRootPath, "PlugIns"));
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseErrorPage();
            }

            app.UseVirtualFiles();
            app.UseAuthentication();
            app.UseJwtTokenMiddleware();

            if (MultiTenancyConsts.IsEnabled)
            {
                app.UseMultiTenancy();
            }

            app.UseIdentityServer();
            app.UseAbpRequestLocalization();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "JobHub API");
            });
            app.UseAuditing();
            app.UseMvcWithDefaultRouteAndArea();
        }
    }
}

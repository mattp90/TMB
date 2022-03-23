using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Reflection;
using System.Threading.Tasks;

namespace Nop.Plugin.MTB
{
    public class MTB : Nopalm.Nopalm
    {
        IPermissionService _permissionService;
        ILocalizationService _localizationService;
        IWorkContext _workContext;

        public MTB(ISettingService settingService, IPermissionService permissionService, IWorkContext workContext,
            IStoreContext storeContext, IWebHelper webHelper, ILocalizationService localizationService, ILanguageService languageService)
            : base(settingService, permissionService, workContext, storeContext, webHelper, localizationService, languageService)
        {
            _permissionService = permissionService;
            _localizationService = localizationService;
            _workContext = workContext;
        }

        public const string PLUGIN_NAME_SPACE = "Nop.Plugin.MTB";

        public override async System.Threading.Tasks.Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            rootNode.ChildNodes.Add(new SiteMapNode()
            {
                Title = (await _localizationService.GetLocaleStringResourceByNameAsync(
                            $"{PLUGIN_NAME_SPACE}.InvoiceRequest.MenuItem",
                            _workContext.GetWorkingLanguageAsync().Result.Id))?.ResourceValue ?? "Richieste Fattura",
                ControllerName = "InvoiceRequest",
                SystemName = "InvoiceRequest",
                ActionName = "List",
                IconClass = "fas fa-cubes",
                Visible = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts),
            });
            await base.ManageSiteMapAsync(rootNode);
        }
    }

    public class MTBStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services
                .Configure<RazorViewEngineOptions>(options =>
                {
                    options.ViewLocationExpanders.Add(new ViewLocationExpander());
                });

            services
                .AddControllersWithViews()
                .AddRazorRuntimeCompilation(options =>
                {
                    options.FileProviders.Add(new EmbeddedFileProvider(
                        typeof(MTB).GetTypeInfo().Assembly,
                        MTB.PLUGIN_NAME_SPACE
                    ));
                });
        }

        public void Configure(IApplicationBuilder application)
        {

        }

        public int Order => 14;
    }
}

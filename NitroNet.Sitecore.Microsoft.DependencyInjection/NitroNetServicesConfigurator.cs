using System.Web;
using System.Web.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NitroNet.Mvc;
using NitroNet.Sitecore.Caching;
using NitroNet.Sitecore.Rendering;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.Grid;
using NitroNet.ViewEngine.ViewEngines;
using Sitecore.DependencyInjection;
using Sitecore.Mvc.Common;
using Veil;
using Veil.Compiler;
using Veil.Helper;
using SitecoreReferences = Sitecore;

namespace NitroNet.Sitecore.Microsoft.DependencyInjection
{
    public class NitroNetServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            RegisterNitroNet(serviceCollection);
            RegisterNitroNetSitecore(serviceCollection);
        }

        private static void RegisterNitroNetSitecore(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<SitecoreNitroNetViewEngine>();
            serviceCollection.AddTransient<ISitecoreRenderingRepository, SitecoreRenderingRepository>();
            serviceCollection.AddTransient(x => GridContext.GetFromRenderingContext(ContextService.Get().GetCurrent<RenderingContext>()));
            serviceCollection.AddSingleton<ISitecoreCacheManager, SitecoreCacheManager>();
            serviceCollection.AddSingleton<INitroTemplateHandlerFactory, SitecoreMvcNitroTemplateHandlerFactory>();
            serviceCollection.AddTransient(x => SitecoreReferences.Context.Database);
        }

        private static void RegisterNitroNet(IServiceCollection serviceCollection)
        {
            var rootPath = HostingEnvironment.MapPath("~/");

            var config = ConfigurationLoader.LoadNitroConfiguration(rootPath);
            serviceCollection.AddSingleton(config);
            serviceCollection.AddSingleton<IFileSystem>(new FileSystem(rootPath, config));

            serviceCollection.AddSingleton(new AsyncLocal<HttpContext>());

            serviceCollection.AddSingleton<IHelperHandlerFactory, DefaultRenderingHelperHandlerFactory>();
            serviceCollection.AddTransient<IMemberLocator, MemberLocatorFromNamingRule>();
            serviceCollection.AddTransient<INamingRule, NamingRule>();
            serviceCollection.AddTransient<IModelTypeProvider, DefaultModelTypeProvider>();
            serviceCollection.AddTransient<ViewEngine.IViewEngine, VeilViewEngine>();
            serviceCollection.AddTransient<ICacheProvider, MemoryCacheProvider>();
            serviceCollection.AddSingleton<IComponentRepository, DefaultComponentRepository>();
            serviceCollection.AddSingleton<ITemplateRepository, NitroTemplateRepository>();
            serviceCollection.AddSingleton<INitroTemplateHandlerFactory, MvcNitroTemplateHandlerFactory>();
        }
    }
}

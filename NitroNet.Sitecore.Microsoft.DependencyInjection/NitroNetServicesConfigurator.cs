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
using Sitecore;
using Sitecore.Configuration;
using Sitecore.DependencyInjection;
using Sitecore.Mvc.Common;
using Veil;
using Veil.Compiler;
using Veil.Helper;

namespace NitroNet.Sitecore.Microsoft.DependencyInjection
{
    [UsedImplicitly]
    public class NitroNetServicesConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            RegisterNitroNet(serviceCollection);
            RegisterNitroNetSitecore(serviceCollection);
        }

        protected virtual void RegisterNitroNetSitecore(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<SitecoreNitroNetViewEngine>();
            serviceCollection.AddTransient<ISitecoreRenderingRepository, SitecoreRenderingRepository>();
            serviceCollection.AddTransient(x => GridContext.GetFromRenderingContext(ContextService.Get().GetCurrent<RenderingContext>()));
            serviceCollection.AddSingleton<ISitecoreCacheManager, SitecoreCacheManager>();
            serviceCollection.AddSingleton<INitroTemplateHandlerFactory, SitecoreMvcNitroTemplateHandlerFactory>();
        }

        protected virtual void RegisterNitroNet(IServiceCollection serviceCollection)
        {
            var basePath = this.GetNitroNetBasePath();

            var config = ConfigurationLoader.LoadNitroConfiguration(basePath);
            serviceCollection.AddSingleton(config);
            serviceCollection.AddSingleton<IFileSystem>(new FileSystem(basePath, config));

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

        protected virtual string GetNitroNetBasePath()
        {
            var rootPath = HostingEnvironment.MapPath("~/");
            var basePath = PathInfo.Combine(PathInfo.Create(rootPath), PathInfo.Create(Settings.GetAppSetting("NitroNet.BasePath")));

            return basePath.ToString();
        }
    }
}

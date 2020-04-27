## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNetSitecore/releases)
- [Known Issues](known-issues.md)

## Installation

### Preconditions
You need a handlebars frontend with a specific file structure (learn more about that here) or a [Nitro](https://github.com/namics/generator-nitro/) project as a precondition of this installation manual.

### Step 1 - Install Sitecore
Please install Sitecore on your local machine and create a Visual Studio project (a clean ASP.Net MVC solution) around of this. As a Sitecore developer you will know what to do ;-) .

### Step 2 - Install NitroNet for Sitecore
There are several ways to install NitroNet for Sitecore. The easiest way is to use it together with Microsoft.DependencyInjection, Unity or CastleWindsor.

Please choose between variant
* **A** with Microsoft.DependencyInjection, Unity or CastleWindsor
* **B** with another IoC Framework.

**Important information**: Because there can be vital changes between different Sitecore versions, we have to make sure that a specific NitroNet.Sitecore Nuget works with a specific Sitecore version. Therefore we created the NuGets with a Sitecore suffix. So you can be sure that they are working with the declared Sitecore version.
So for each supported Sitecore version you will find the corresponding NuGets (e.g. all NuGets with Sitecore 8.2 compatibility end with the suffix `.Sitecore82`). If there is no matching version then choose the next lower version (e.g. for Sitecore 9.1 use packages which end with the suffix `.Sitecore90`).

#### (A) With Microsoft.DependencyInjection, Unity or CastleWindsor

##### NuGet Package Installation

Execute following the line in your NuGet Package Manager to install NitroNet for Sitecore with your preferred IoC framework:

###### Microsoft.DependencyInjection

Starting with Sitecore 8.2, this IoC container is supported out of the box.

`PM >` `Install-Package NitroNet.Sitecore.Microsoft.DependencyInjection.Sitecore82`

###### Unity

`PM >` `Install-Package NitroNet.Sitecore.UnityModules.Sitecore82`

Optionally, we recommend to install the [Unity.Mvc](https://www.nuget.org/packages/Unity.Mvc/) which is a lightweight Unity bootstrapper for MVC applications:

`PM >` `Install-Package Unity.Mvc`

###### CastleWindsor

`PM >` `Install-Package NitroNet.Sitecore.CastleWindsorModules.Sitecore82`

##### Register the IoC containers

###### Microsoft.DependencyInjection
When you are using NitroNet for Sitecore with Microsoft.DependencyInjection then there is nothing to do to active the IoC framework.
The */App_Config/Include/NitroNet/DependencyInjection.config* is automatically installed with this NuGet and sets up the activation.

###### Unity
To activate NitroNet for Sitecore with Unity, please add these lines to */App_Start/UnityConfig.cs* in your application ([Gist](https://gist.github.com/hombreDelPez/a268d69a0b03d5e117d0707f0b3132d9)):

```csharp
public static void RegisterTypes(IUnityContainer container)
{
	var rootPath = HostingEnvironment.MapPath("~/");
	var basePath = PathInfo.Combine(PathInfo.Create(rootPath), PathInfo.Create(ConfigurationManager.AppSettings["NitroNet.BasePath"])).ToString();

	new DefaultUnityModule(basePath).Configure(container);
	new SitecoreUnityModule().Configure(container);
}
```

###### CastleWindsor
To activate NitroNet for Sitecore with CastleWindsor, please add these lines to your application:

```csharp
public static void RegisterTypes(IWindsorContainer container)
{
	var rootPath = HostingEnvironment.MapPath("~/");
	var basePath = PathInfo.Combine(PathInfo.Create(rootPath), PathInfo.Create(ConfigurationManager.AppSettings["NitroNet.BasePath"])).ToString();

	new DefaultCastleWindsorModule(basePath).Configure(container);
	new SitecoreCastleWindsorModule().Configure(container);
}
```


#### (B) With another IoC Framework
You don't like Unity and you design your application with an other IoC framework? No Problem.

In this case, you can install NitroNet only with our base package:

`PM >` `Install-Package NitroNet.Sitecore.Sitecore82 `

##### Extend your Global.asax
*Please extend your Global.asax(.cs) in the same way as in scenario A*

##### Register NitroNet for Sitecore with your own IoC Framework
Actually, we only made a Microsoft.DependencyInjection, Unity or CastleWindsor integration with NitroNet for Sitecore. But it's easy to use another IoC Framework.
Please follow our Unity sample as a template for you ([Gist](https://gist.github.com/daniiiol/036be44e535768fac2df5eec0aff9180)):

###### DefaultUnityModule.cs

```csharp
using Microsoft.Practices.Unity;
using NitroNet.Mvc;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.ViewEngines;
using Veil.Compiler;
using Veil.Helper;

namespace NitroNet.UnityModules
{
    public class DefaultUnityModule : IUnityModule
    {
        private readonly string _basePath;

        public DefaultUnityModule(string basePath)
        {
            _basePath = basePath;
        }

        public void Configure(IUnityContainer container)
        {
            RegisterConfiguration(container);
            RegisterApplication(container);
        }

        protected virtual void RegisterConfiguration(IUnityContainer container)
        {
            var config = ConfigurationLoader.LoadNitroConfiguration(_basePath);
            container.RegisterInstance(config);

            container.RegisterInstance<IFileSystem>(new FileSystem(_basePath, config));
        }

        protected virtual void RegisterApplication(IUnityContainer container)
        {
            container.RegisterType<IHelperHandlerFactory, DefaultRenderingHelperHandlerFactory>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<IMemberLocator, MemberLocatorFromNamingRule>();
            container.RegisterType<INamingRule, NamingRule>();
            container.RegisterType<IModelTypeProvider, DefaultModelTypeProvider>();
            container.RegisterType<IViewEngine, VeilViewEngine>();
            container.RegisterType<ICacheProvider, MemoryCacheProvider>();
            container.RegisterType<IComponentRepository, DefaultComponentRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITemplateRepository, NitroTemplateRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<INitroTemplateHandlerFactory, MvcNitroTemplateHandlerFactory>(
                new ContainerControlledLifetimeManager());
        }
    }
}
```

###### SitecoreUnityModule.cs

```csharp
using Microsoft.Practices.Unity;
using NitroNet.Sitecore.Caching;
using NitroNet.Sitecore.Rendering;
using NitroNet.UnityModules;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.Grid;
using Sitecore;
using Sitecore.Data;
using Sitecore.Mvc.Common;
using Veil;

namespace NitroNet.Sitecore.UnityModules
{
    public class SitecoreUnityModule : IUnityModule
    {
        public void Configure(IUnityContainer container)
        {
            container.RegisterType<GridContext>(
                new InjectionFactory(u => GridContext.GetFromRenderingContext(ContextService.Get().GetCurrent<RenderingContext>())));
            container.RegisterType<ISitecoreRenderingRepository, SitecoreRenderingRepository>();
            container.RegisterType<ISitecoreCacheManager, SitecoreCacheManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<INitroTemplateHandlerFactory, SitecoreMvcNitroTemplateHandlerFactory>(
                new ContainerControlledLifetimeManager());
            container.RegisterType<Database>(new InjectionFactory(u => Context.Database));
        }
    }
}
```
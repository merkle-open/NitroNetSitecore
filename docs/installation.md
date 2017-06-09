## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Known Issues](known-issues.md)

## Installation

### Preconditions
You need your own Nitro project as a precondition of this installation manual.
Please follow the beautiful guide of Nitro: [Link](https://github.com/namics/generator-nitro/)

### Step 1 - Install Sitecore
Please install Sitecore on your local machine and create a Visual Studio project (a clean ASP.Net MVC solution) around of this. As a Sitecore developer you will know what I mean ;-) .

### Step 2 - Install NitroNet for Sitecore
There are several ways to install NitroNet for Sitecore. The easiest way is to use NitroNet together with Unity or CastleWindsor.

Please choose between variant
* **A** with Unity, CastleWindsor or Microsoft.DependencyInjection
* **B** with another IoC Framework.

#### (A) With Unity, CastleWindsor or Microsoft.DependencyInjection

##### NuGet Package Installation
TODO: Text welche Sitecore82 erklärt

Execute following line in your NuGet Package Manager to install NitroNet for Sitecore with **Unity**:

`PM >` `Install-Package NitroNet.Sitecore.UnityModules.Sitecore82`

Optionally, we recommend to install the [Unity.Mvc](https://www.nuget.org/packages/Unity.Mvc/) which is a lightweight Unity bootstrapper for MVC applications:

`PM >` `Install-Package Unity.Mvc`

or to install NitroNet for Sitecore with **CastleWindsor**:

`PM >` `Install-Package NitroNet.Sitecore.CastleWindsorModules.Sitecore82`

or to install NitroNet for Sitecore with **Microsoft.DependencyInjection**:

`PM >` `Install-Package NitroNet.Sitecore.Microsoft.DependencyInjection.Sitecore82`


##### Extend your Global.asax(.cs)
To activate NitroNet it's important to add/register the new view engine in your application. You can do this, with these lines of code ([Gist](https://gist.github.com/hombreDelPez/e5ad065572fdab7145dd72847d8aabd2)):

```csharp
protected void Application_Start()
{
	AreaRegistration.RegisterAllAreas();
	FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
	RouteConfig.RegisterRoutes(RouteTable.Routes);
	BundleConfig.RegisterBundles(BundleTable.Bundles);

	ViewEngines.Engines.Add(DependencyResolver.Current.GetService<SitecoreNitroNetViewEngine>());
}
```

##### Register the IoC containers
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

###### Microsoft.DependencyInjection
When you are using NitroNet for Sitecore with Microsoft.DependencyInjection then there is nothing to do to active the IoC framework.
The */App_Config/Include/NitroNet/DependencyInjection.config* is automatically installed with this NuGet and sets up the activation.


#### (B) With another IoC Framework
You don't like Unity and you design your application with an other IoC framework? No Problem.
In this case, you can install NitroNet only with our base package:

TODO: Text welche Sitecore82 erklärt

`PM >` `Install-Package NitroNet.Sitecore.Sitecore82 `

##### Extend your Global.asax
*Please extend your Global.asax(.cs) in the same way as in scenario A*

##### Register NitroNet for Sitecore with your own IoC Framework
Actually, we only made a Unity,CastleWindsor and Microsoft.DependencyInjection integration with NitroNet for Sitecore. But it's easy to use another IoC Framework.
Please follow our Unity sample as a template for you ([Gist](https://gist.github.com/daniiiol/036be44e535768fac2df5eec0aff9180)):

###### DefaultUnityModule.cs

```csharp
using Microsoft.Practices.Unity;
using NitroNet;
using NitroNet.Mvc;
using NitroNet.ViewEngine;
using NitroNet.ViewEngine.Cache;
using NitroNet.ViewEngine.Config;
using NitroNet.ViewEngine.IO;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.ViewEngines;
using System.Web;
using Veil.Compiler;
using Veil.Helper;

namespace NitroNet.UnityModules
{
  public class DefaultUnityModule : IUnityModule
  {
    private readonly string _basePath;

    public DefaultUnityModule(string basePath)
    {
      this._basePath = basePath;
    }

    public void Configure(IUnityContainer container)
    {
      this.RegisterConfiguration(container);
      this.RegisterApplication(container);
    }

    protected virtual void RegisterConfiguration(IUnityContainer container)
    {
      INitroNetConfig nitroNetConfig = ConfigurationLoader.LoadNitroConfiguration(this._basePath);
      UnityContainerExtensions.RegisterInstance<INitroNetConfig>(container, nitroNetConfig);
      UnityContainerExtensions.RegisterInstance<IFileSystem>(container, (IFileSystem) new FileSystem(this._basePath, nitroNetConfig));
    }

    protected virtual void RegisterApplication(IUnityContainer container)
    {
      UnityContainerExtensions.RegisterInstance<AsyncLocal<HttpContext>>(container, new AsyncLocal<HttpContext>(), (LifetimeManager) new ContainerControlledLifetimeManager());
      UnityContainerExtensions.RegisterType<IHelperHandlerFactory, DefaultRenderingHelperHandlerFactory>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
      UnityContainerExtensions.RegisterType<IMemberLocator, MemberLocatorFromNamingRule>(container);
      UnityContainerExtensions.RegisterType<INamingRule, NamingRule>(container);
      UnityContainerExtensions.RegisterType<IModelTypeProvider, DefaultModelTypeProvider>(container);
      UnityContainerExtensions.RegisterType<IViewEngine, VeilViewEngine>(container);
      UnityContainerExtensions.RegisterType<ICacheProvider, MemoryCacheProvider>(container);
      UnityContainerExtensions.RegisterType<IComponentRepository, DefaultComponentRepository>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
      UnityContainerExtensions.RegisterType<ITemplateRepository, NitroTemplateRepository>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
      UnityContainerExtensions.RegisterType<INitroTemplateHandlerFactory, MvcNitroTemplateHandlerFactory>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
    }
  }
}
```

###### SitecoreUnityModule.cs

```csharp
using Microsoft.Practices.Unity;
using NitroNet.Sitecore;
using NitroNet.Sitecore.Caching;
using NitroNet.Sitecore.Rendering;
using NitroNet.UnityModules;
using NitroNet.ViewEngine.TemplateHandler;
using NitroNet.ViewEngine.TemplateHandler.Grid;
using Sitecore.Mvc.Common;
using System;
using Veil;

namespace NitroNet.Sitecore.UnityModules
{
  public class SitecoreUnityModule : IUnityModule
  {
    public void Configure(IUnityContainer container)
    {
      UnityContainerExtensions.RegisterType<GridContext>(container, new InjectionMember[1]
      {
        (InjectionMember) new InjectionFactory((Func<IUnityContainer, object>) (u => (object) GridContext.GetFromRenderingContext(ContextService.Get().GetCurrent<RenderingContext>())))
      });
      UnityContainerExtensions.RegisterType<ISitecoreRenderingRepository, SitecoreRenderingRepository>(container);
      UnityContainerExtensions.RegisterType<ISitecoreCacheManager, SitecoreCacheManager>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
      UnityContainerExtensions.RegisterType<INitroTemplateHandlerFactory, SitecoreMvcNitroTemplateHandlerFactory>(container, (LifetimeManager) new ContainerControlledLifetimeManager(), new InjectionMember[0]);
    }
  }
}
```
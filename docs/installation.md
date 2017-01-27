## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Known Issues](known-issues.md)

## Installation

### Preconditions
Its important to have a Nitro project as precondition of this installation manual. Please follow the beautiful guide of Nitro: [Link](https://github.com/namics/generator-nitro/)

### Step 1 - Install Sitecore
Please install a Sitecore system on your local machine and create a Visual Studio project (a clear ASP.Net MVC solution) around of this. As a Sitecore developer you know what I mean. :)

### Step 2 - Install NitroNet for Sitecore

Please choose between variant **A** with Unity/CastleWindsor or **B** with another IoC Framework.

#### (A) Directly with Unity or CastleWindsor IoC Container

##### NuGet Package Installation
There are several ways to install NitroNet into Sitecore. The easiest way is to use NitroNet together with Unity or CastleWindsor. Execute following line in your NuGet Package Manager to install NitroNet for Sitecore:

`PM >` `Install-Package NitroNet.Sitecore.UnityModules`

Optionally, we recommend to install the [Unity.Mvc](https://www.nuget.org/packages/Unity.Mvc/) which is a lightweight Unity bootstrapper for MVC applications.

*or*

`PM >` `Install-Package NitroNet.Sitecore.CastleWindsorModules` 

##### Extend your Global.asax
To activate NitroNet it's important to add/register the new view engine in your application. You can do this, with these lines of code ([Gist](https://gist.github.com/daniiiol/216b161462db3dc2f7a3f43745bbfad0)):

	<%@Application Language='C#' Inherits="Sitecore.Web.Application" %>
	<%@ Import Namespace="NitroNet.Sitecore" %>
	<script RunAt="server">
	    
	    public void Application_Start()
	    {
	        ViewEngines.Engines.Add(DependencyResolver.Current.GetService<SitecoreNitroNetViewEngine>());
	    }
	</Script>

##### Register the IoC containers
You got all necessary code classes to configure and register NitroNet with Unity. To activate NitroNet add these lines to your application ([Gist](https://gist.github.com/daniiiol/90b63503bfe0665c642f862f3ec2553f))

	public static void RegisterTypes(IUnityContainer container)
    {
        var rootPath = HostingEnvironment.MapPath("~/");
        var basePath = PathInfo.Combine(PathInfo.Create(rootPath), PathInfo.Create(ConfigurationManager.AppSettings["NitroNet.BasePath"])).ToString();
        
        new DefaultUnityModule(basePath).Configure(container);
        new SitecoreUnityModule().Configure(container);
    }

Also you got all necessary code classes to configure and register NitroNet with CastleWindsor. To activate NitroNet add these lines to your application ([Gist](https://gist.github.com/daniiiol/f8c994c7ebb2c255f8e2b185c5499eaf))

	public static void RegisterTypes(IWindsorContainer container)
    {
        var rootPath = HostingEnvironment.MapPath("~/");
        var basePath = PathInfo.Combine(PathInfo.Create(rootPath), PathInfo.Create(ConfigurationManager.AppSettings["NitroNet.BasePath"])).ToString();
        
        new DefaultCastleWindsorModule(basePath).Configure(container);
        new SitecoreCastleWindsorModule().Configure(container);
    }

#### (B) Directly without the Unity IoC framework
You don't like Unity and you design your application with an other IoC framework? No problem. In this case, you can install NitroNet only with our base package:

`PM >` `Install-Package NitroNet.Sitecore`

##### Extend your Global.asax
*Please extend your Global.asax in the same way as in scenario (A)* 

##### Register NitroNet with your own IoC Framework
Actually, we have made only a Unity integration with NitroNet. But it's easy to use an other IoC Framework. Please use our Unity sample as a template for you ([Gist](https://gist.github.com/daniiiol/036be44e535768fac2df5eec0aff9180)):

###### DefaultUnityModule

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

###### SitecoreUnityModule


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


### Step 3
*Oh sorry... there's no Step 3 to work with NitroNet :)*

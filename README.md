# NitroNet for Sitecore

### What's Nitro?

[Nitro](https://github.com/namics/generator-nitro/) is a Node.js application for simple and complex frontend development with a tiny footprint.  
It provides a proven but flexible structure to develop your frontend code, even in a large team.  
Keep track of your code with a modularized frontend. This app and the suggested [atomic design](http://bradfrost.com/blog/post/atomic-web-design/) and [BEM](https://en.bem.info/method/definitions/) concepts could help.  
Nitro is simple, fast and flexible. It works on OSX, Windows and Linux. Use this app for all your frontend work.

### What's NitroNet ?

NitroNet is a full integration of Nitro-Frontends into ASP.NET. Nitro itself based on the Template-Engine handlebars.js. NitroNet using the same Template-Engine as Nitro with the parsing framework [Veil](https://github.com/csainty/Veil/tree/master/Src/Veil.Handlebars) of [Chris Sainty](https://github.com/csainty) and combine the best of both worlds. In summary, NitroNet is a completely new and simple View-Engine for ASP.NET MVC Web Applications.

### ...and NitroNet for Sitecore ?

The name says it all: NitroNet for Sitecore is a special View-Engine, based on ASP.NET MVC for the Content Management System [Sitecore](http://www.sitecore.net). It handles all possible presentation scenarios to integrate a Nitro-Frontend into Sitecore without functional loss (Sublayouting, Placeholders, Experience-Editor Full-Support, Personalization, ...). In addition it's possible to get a hybrid integration with Nitro-Frontends (based on simple and logic-less View-Pages) and own Razor-Views (*.cshtml). NitroNet is created by [Fabian Geiger](https://github.com/fgeiger).



## Installation

### Preconditions
You need a own Nitro Project as precondition of this installation manuel. Please follow the beautiful guide of Nitro: [Link](https://github.com/namics/generator-nitro/)

### Step 1 - Install Sitecore
Please install a basicly Sitecore version on your local machine and create a Visual Studio Project (a clear ASP.Net MVC Solution) around of this. As a Sitecore Developer you know what I mean. :)

### Step 2 - Install NitroNet for Sitecore

#### (A) Directly with Unity IoC Container

##### NuGet Package Installation
There are several ways to install NitroNet into Sitecore. The easiest way is to use NitroNet together with Unity. Execute following Line in your NuGet Package Manager or search the Package in your NuGet Browser:

`PM >` `Install-Package NitroNet.Sitecore.UnityModules` 

##### Extend your Global.asax
To activate NitroNet it's important to add/register the new View-Engine in your Application. You can do this, with these lines of code ([Gist](https://gist.github.com/daniiiol/216b161462db3dc2f7a3f43745bbfad0)):

	<%@Application Language='C#' Inherits="Sitecore.Web.Application" %>
	<%@ Import Namespace="NitroNet.Sitecore" %>
	<script RunAt="server">
	    
	    public void Application_Start()
	    {
	        ViewEngines.Engines.Add(DependencyResolver.Current.GetService<SitecoreNitroNetViewEngine>());
	    }
	</Script>

##### Register the Unity IoC Containers
In this NuGet Package, you got all necessary code classes to configure and register NitroNet with Unity. To Activate NitroNet, please add these lines to your UnityConfig.cs ([Gist](https://gist.github.com/daniiiol/90b63503bfe0665c642f862f3ec2553f))

	public static void RegisterTypes(IUnityContainer container)
    {
        var rootPath = HostingEnvironment.MapPath("~/");
        var basePath = PathInfo.Combine(PathInfo.Create(rootPath), PathInfo.Create(ConfigurationManager.AppSettings["NitroNet.BasePath"])).ToString();
        
        new DefaultUnityModule(basePath).Configure(container);
        new SitecoreUnityModule().Configure(container);
    }

#### (B) Directly without the Unity IoC Framework
You don't like Unity and you design your application with an other IoC Framework? No Problem. In this case, you can install NitroNet only with our Base-Package:

`PM >` `Install-Package NitroNet.Sitecore`

##### Extend your Global.asax
*Please extend your Global.asax in the same way as in scenario (A)* 

##### Register NitroNet with your own IoC Framework
Actually, we only made a Unity-Integration with NitroNet. But it's easy to use an other IoC Framework. Following our Unity-Sample as a template for you ([Gist](https://gist.github.com/daniiiol/036be44e535768fac2df5eec0aff9180)):


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


## Configuration
... comming very, very soon.
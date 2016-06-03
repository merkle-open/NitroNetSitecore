# NitroNet for Sitecore

### What's Nitro?

[Nitro](https://github.com/namics/generator-nitro/) is a Node.js application for simple and complex frontend development with a tiny footprint.  
It provides a proven but flexible structure to develop your frontend code, even in a large team.  
Keep track of your code with a modularized frontend. This app and the suggested [atomic design](http://bradfrost.com/blog/post/atomic-web-design/) and [BEM](https://en.bem.info/method/definitions/) concepts could help.  
Nitro is simple, fast and flexible. It works on OSX, Windows and Linux. Use this app for all your frontend work.

### What's NitroNet ?

[NitroNet](https://github.com/namics/NitroNet) is a full integration of Nitro-Frontends into ASP.NET. Nitro itself based on the Template-Engine handlebars.js. NitroNet using the same Template-Engine as Nitro with the parsing framework [Veil](https://github.com/csainty/Veil/tree/master/Src/Veil.Handlebars) of [Chris Sainty](https://github.com/csainty) and combine the best of both worlds. In summary, NitroNet is a completely new and simple View-Engine for ASP.NET MVC Web Applications. You can get more informations about NitroNet on our seperate [Git-Hub Project Page of NitroNet](https://github.com/namics/NitroNet). NitroNet is created by [Fabian Geiger](https://github.com/fgeiger).

### ...and NitroNet for Sitecore ?

The name says it all: NitroNet for Sitecore is a special View-Engine, based on ASP.NET MVC for the Content Management System [Sitecore](http://www.sitecore.net). It handles all possible presentation scenarios to integrate a Nitro-Frontend into Sitecore without functional loss (Sublayouting, Placeholders, Experience-Editor Full-Support, Personalization, ...). In addition it's possible to get a hybrid integration with Nitro-Frontends (based on simple and logic-less View-Pages) and own Razor-Views (*.cshtml). 

## Installation

### Preconditions
You need a own Nitro Project as precondition of this installation manuel. Please follow the beautiful guide of Nitro: [Link](https://github.com/namics/generator-nitro/)

### Step 1 - Install Sitecore
Please install a basicly Sitecore version on your local machine and create a Visual Studio Project (a clear ASP.Net MVC Solution) around of this. As a Sitecore Developer you know what I mean. :)

### Step 2 - Install NitroNet for Sitecore

Please choose between variant **A** with Unity or **B** with another IoC Framework.

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


## Configuration

### Connect the Nitro Frontend to your Application
The location of your Nitro-Application could be configure very flexible.

As default-value it would be taken the website-root-folder itself. If you like to change it, you can add an AppSetting with the Key-Value `NitroNet.BasePath` in your *web.config*.

	<appSettings>
	    <add key="NitroNet.BasePath" value ="Nitro/Sample" />
	</appSettings>

In addition, you got a new `config.json`-File in the root-directory of the website-project after installation of NitroNet:

	{
	  "viewPaths": [
	    "frontend/views"
	  ],
	  "partialPaths": [
	    "frontend/views/_partials"
	  ],
	  "componentPaths": [
	    "frontend/components/atoms",
	    "frontend/components/molecules",
	    "frontend/components/organisms"
	  ]
	}

That `config.json` File defines your *view*-, *partial*- and *components*-paths, starting on your `NitroNet.BasePath`.

###### Conclusion
Back to my sample configuration, the *atoms* would be located under `~/Nitro/Sample/frontend/components/atoms/`. That's all about view-logic resolving.

### Getting started with NitroNet

#### Create a Controller

To use a Nitro based component in Sitecore, you can create a normal `System.Web.Mvc.Controller` of ASP.NET MVC:

	public class TeaserController : System.Web.Mvc.Controller
	    {
	        // GET: Teaser
	        public ActionResult Index()
	        {
	            var model = new TeaserModel
	            {
	                Headline = "Headline first-line",
	                Abstract = "Praesent ac massa at ligula laoreet iaculis. Cras id dui. Integer ante arcu, accumsan a, consectetuer eget, posuere ut, mauris. Fusce fermentum odio nec arcu.",
	                Richtext = "<p>Sed lectus. Suspendisse nisl elit, rhoncus eget, <a href='#'>elementum ac</a>, condimentum eget, diam. Curabitur turpis. Ut non enim eleifend felis pretium feugiat. Vivamus aliquet elit ac nisl.</p><ul><li>Primis in faucibus orci luctus et ultrices</li><li>Dignissim dolor, <a href='#'>pretium</a> mi sem ut ipsum.</li><li>Etiam ut purus mattis mauris sodales aliquam.</li></ul>",
	                ButtonText = "Button Text"
	            };
	
	            return View("teaser", model);
	        }
	    }

The whole magic would be execute on the returning line `return View("teaser", model);`

The string `"teaser"` must be fit with the name of a Nitro component. The guidelines of a model would be explained under chapter "Create a Model".

#### Create a Model

The Model definition is very simple and smart. In the Nitro Folder of your selected component (e.g. `.../frontend/components/molecules/teaser`) it's possible to see all needed information of your model definition. This is the contract with the Frontend. You can see the contract under `./_data/<molecule-name>.json`:

	{
		"headline" : "Headline first-line",
		"abstract" : "Praesent ac massa at ligula laoreet iaculis. Cras id dui. Integer ante arcu, accumsan a, consectetuer eget, posuere ut, mauris. Fusce fermentum odio nec arcu.",
		"richtext" : "<p>Sed lectus. Suspendisse nisl elit, rhoncus eget, <a href='#'>elementum ac</a>, condimentum eget, diam. Curabitur turpis. Ut non enim eleifend felis pretium feugiat. Vivamus aliquet elit ac nisl.</p><ul><li>Primis in faucibus orci luctus et ultrices</li><li>Dignissim dolor, <a href='#'>pretium</a> mi sem ut ipsum.</li><li>Etiam ut purus mattis mauris sodales aliquam.</li></ul>",
		"bubbleCenter":false
	}

In this case, you can create an equivalent .Net Class with the same properties:

	public class TeaserModel
    {
        public string Headline { get; set; }
        public string Abstract { get; set; }
        public string Richtext { get; set; }
        public string ButtonText { get; set; }
    }

##### Supported Types

	public class FooModel
	{
	    public string Text { get; set; }
		public int Numeric { get; set; }
	    public bool Abstract { get; set; }
	    public SpecialClassModel Bar { get; set; }
	    public IEnumerable<SpecialClassModel> Items { get; set; }
		...
	}

### View Types

#### The easy view sample

	<div class="m-teaser">
		<div class="m-teaser__wrapper-left">
	
			<h2 class="font-page-title m-teaser__headline">
				{{#if headline}}
					<span class="m-teaser__headline-text">{{headline}}</span>
				{{/if}}
			</h2>
		</div>
	
		<div class="l-tile__content">
			{{#if abstract}}
				<h3 class="font-big-title m-teaser__abstract">{{{abstract}}}</h3>
			{{/if}}
			{{#if richtext}}
				<div class="font-copy-text m-teaser__rte">
					{{{richtext}}}
				</div>
			{{/if}}
		</div>
		<a href="#" class="a-button a-button--primary m-teaser__button">{{buttonText}}</a>
	</div>

This case shows a simple Nitro Html View-File. This Sample can be execute with the Controller and Model of above sample snippets.

#### A view with sub-components

###### View

	<div class="m-location" data-t-name="Location">
		<a href="#" class="a-link m-location__location-link js-m-location__location-link">{{selectedLocation}}</a>
		<div class="m-location__location js-m-location__location l-overlay-container">
			{{component name="Bubble" data="bubbleLocation"}}
			<ul class="m-location__list">
				{{#each locations}}
					<li class="m-location__location-item font-title"><a class="font-region-title a-link{{#if active}} a-link--active{{/if}} m-location__link js-m-location__link" data-location-key="{{locationKey}}" href="{{target}}">{{name}}</a></li>
				{{/each}}
			</ul>
			<a href="/" class="icon l-overlay-container__close js-m-location__location-link"></a>
		</div>
	</div>

###### Model
	public class LocationModel
	{
		public string SelectedLocation { get; set; }
	    public IEnumerable<LocationModel> Locations { get; set; }
		public BubbleLocationModel BubbleLocation { get; set; }
	}
	
	public class LocationModel
	{
	    public bool Active { get; set; }
		public string LocationKey { get; set; }
		public string Target { get; set; }
		public string Name { get; set; }
	}

#### A view with repeating sub-elements

###### View

	<ul class="m-link-list">
		{{#each links}}
			<li class="m-link-list__item font-meta-navi"><a class="a-link" href="{{target}}">{{linkText}}</a></li>
		{{/each}}
	</ul>

###### Model
In this case, we need a Model-Class with a Enumerable-Property called `links`. The main-model self, hasn't any other properties:

	public class LinkListModel
	{
	    public IEnumerable<LinkModel> Links { get; set; }
	}
	
	public class LinkModel
	{
	    public string Target { get; set; }
		public string LinkText { get; set; }
	}


### Actually not implemented

*TBD*
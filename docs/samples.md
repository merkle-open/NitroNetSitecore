## Table of contents
- [What's NitroNet](https://github.com/namics/NitroNetSitecore)
- [Installation](https://github.com/namics/NitroNetSitecore/blob/master/docs/installation.md)
- [Configuration](https://github.com/namics/NitroNetSitecore/blob/master/docs/configuration.md)
- [Getting started](https://github.com/namics/NitroNetSitecore/blob/master/docs/getting-started.md)
- [Samples](https://github.com/namics/NitroNetSitecore/blob/master/docs/samples.md)
- [Known Issues](https://github.com/namics/NitroNetSitecore/blob/master/docs/known-issues.md)

## Templating
Please visit the [Nitro Example Documentation](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md) informations and samples about Nitro.

## Layout view

### The easy layout view example 
In Sitecore you can add only partials, placeholders or static components (which have no datasource) to a layout file.

	<!DOCTYPE html>
	<html>
	<head>
		{{partial name="head"}}
	</head>
	<body>
	<header>
		{{placeholder name="HeaderArea"}}
	</header>
	<div>
		{{component name="Breadcrumb"}}
		{{placeholder name="ContentArea"}}
	</div>
	<footer>
		{{placeholder name="FooterArea"}}
	</footer>
	{{partial name="foot"}}
	</body>
	</html>

In order to get this layout view to work you need to create a layout item, a controller rendering called `Breadcrumb` and a controller `BreadcrumbController`. Please follow the [Getting started](https://github.com/namics/NitroNetSitecore/blob/master/docs/getting-started.md) page for this simple case.

### Model on layout view level ###

In the above example you can only see partials, placeholders and components without data. But what if you need to add direct value, e.g. a simple string indicating the language?
You can make use of the out of the box functionality of Sitecore where you can define a model on your layout. First change your layout to this:

	<!DOCTYPE html>
	<html lang="{{htmlLanguage}}">
		...
	</html>

Create a model inheriting from the Sitecore.Mvc.Presentation.IRenderingModel and implement the Initialize method where you fill your properties.

	public class DefaultRenderingModel : IRenderingModel
 	{
		public string HtmlLanguage { get; set; }

		public void Initialize(Rendering rendering)
		{
			HtmlLanguage = Sitecore.Context.Language.Name;
		}
	}

In the Sitecore backend create a new model item under /sitecore/layout/Models. On the newly created item in the field Model Type enter the fully qualified name of the DefaultRenderingModel you've created above.

Finally you need to link your layout to the model item in the Model field. Reload your page and voila, the value is there.

### The specific component template example

*tbd*

## Handlebars helpers

### Partial
*Remark: The handlebars helper for partials is typically known as `>`. This helper is currently not supported by NitroNet. As a workaround you might want to use `partial` instead.*

Partials are simple includes into a base file and could be used for reducing the complexity of large view-files.
NitroNet supports partials with the keyword `partial`:

	{{partial name="head"}}

### Placeholder

Placeholders doesn't need a special interaction in user code. In Sitecore you need to create a placeholder setting with the same key as the `name` attribute of the placeholder. The `template` parameter is ignored by NitroNet completely.

	<div class="m-accordion" data-t-name="Accordion">
		{{placeholder name="AccordionArea" template="accordion-area"}}
	</div>

#### Placeholders on the same hierarchical level

You might run into situations where you want to have multiple placeholders with the same key on the same hierarchical level. Therefore you need to use the `index` or `indexProp` attribute.

##### index attribute

	<div class="m-accordion" data-t-name="Accordion">
		{{{placeholder name="AccordionArea" index="1"}}}
		{{{placeholder name="AccordionArea" index="2"}}}
	</div>

##### indexProp attribute

View snippet

	<div class="m-accordion" data-t-name="Accordion">
		{{#each items}}
			{{placeholder name="AccordionArea" indexProp="Id"}}
		{{/each}}
	</div>

Model snippet

	public class AccordionModel
	{
		public IEnumerable<AccordionItemModel> Items { get; set; }
	}

	public class AccordionItemModel
	{
		public string Id { get; set; }
	}

### Component

#### The easy view sample
This sample shows a simple Nitro Html View.

##### View
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

##### Model
	public class TeaserModel
    {
        public string Headline { get; set; }
        public string Abstract { get; set; }
        public string Richtext { get; set; }
        public string ButtonText { get; set; }
    }

#### A view with repeating subentities

##### View

	<ul class="m-link-list">
		{{#each links}}
			<li class="m-link-list__item font-meta-navi"><a class="a-link" href="{{target}}">{{linkText}}</a></li>
		{{/each}}
	</ul>

##### Model

In this case, we need a model class with an enumerable property called `links`. The main model itself hasn't any other properties:

	public class LinkListModel
	{
	    public IEnumerable<LinkModel> Links { get; set; }
	}
	
	public class LinkModel
	{
	    public string Target { get; set; }
		public string LinkText { get; set; }
	}

#### A view with sub-components
Nested components (e.g. a molecule that consists of one or more atoms) are handled by one controller action and doesn't invoke a new controller for each sub component. In the following case, the LocationController has the responsibility to create all parts of the LocationModel which also includes data of the sub component `Bubble`.

##### View

	<div class="m-location" data-t-name="Location">
		<a href="#">{{selectedLocation}}</a>
		<div>
			{{component name="Bubble" data="bubbleLocation"}}
			<ul>
				{{#each locations}}
					<li>
						<a data-location-key="{{locationKey}}" href="{{target}}">{{name}}</a>
					</li>
				{{/each}}
			</ul>
		</div>
	</div>

##### Model

	public class LocationModel
	{
		public string SelectedLocation { get; set; }
		public IEnumerable<LocationModel> Locations { get; set; }
		public BubbleLocationModel BubbleLocation { get; set; }
	}
	
	public class BubbleLocationModel
	{
		public string LocationKey { get; set; }
		public string Target { get; set; }
		public string Name { get; set; }
	}

You need to make sure that there is always a property defined in the model for each sub component. It holds the data which is then passed to the sub component. Make sure that it either matches the `name` or `data` attribute of the corresponding component helper. You don't need to worry about case sensitivity.

###### Situation A - Component with name attribute
View snippet

	{{component name="Bubble"}}

Model snippet (maps the `name` attribute)

	public BubbleLocationModel Bubble { get; set; }

###### Situation B - Component with name and data
View snippet

	{{component name="Bubble" data="bubbleLocation"}}

Model snippet (maps the `data` attribute)

	public BubbleLocationModel BubbleLocation { get; set; }

#### A view with sub-components and dataVariation ####

There is a special case where the model does not need to have a property for a specific sub-component. If you do not provide a model for the sub-component you need to ensure that at least a controller for this sub-component exists. You can also create a rendering for it inside Sitecore if you need to set special caching settings for this component because it is run through the rendering pipeline. NitroNetSitecore internally invokes the controller respectively the rendering pipeline and passes the `data` string as parameter to the action method call. If no data is present, the component name will be passed. 

View snippet

	<div class="o-footerContainer">
		{{title}}
		<div class="o-footerContainer__column"> 
			{{component name="footer-link-list"}}
		</div>
		<div class="o-footerContainer__column">
			{{component name="footer-link-list" data="social"}}
		</div>
	</div>

Model

	public class FooterContainerModel
	{
		public string Title { get; set; }
	}

Controller for the `footer-link-list`

	public class FooterLinkListController : Controller
	{
		private readonly IFooterLinkService _service;

		public FooterLinkListController(IFooterLinkService service)
		{
			_service = service;
		}

		public ActionResult Index(string dataVariation)
		{
			FooterLinkListViewModel model;
			if (dataVariation.Equals("social", StringComparison.InvariantCultureIgnoreCase))
			{
				model = _service.CreateSocialLinks()
			}
			else
			{
				model = _service.CreateFooterLinks();
			}

			return View("path/to/your/template/footer-link-list", model);
		}
	}

For direct values like `title` there needs to be property, for the `footer-link-list` component not.

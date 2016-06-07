## Table of contents
- [What's NitroNet](https://github.com/namics/NitroNetSitecore)
- [Installation](https://github.com/namics/NitroNetSitecore/blob/master/docs/installation.md)
- [Configuration](https://github.com/namics/NitroNetSitecore/blob/master/docs/configuration.md)
- [Getting started](https://github.com/namics/NitroNetSitecore/blob/master/docs/getting-started.md)
- [Samples](https://github.com/namics/NitroNetSitecore/blob/master/docs/samples.md)
- [Not yet implemented](https://github.com/namics/NitroNetSitecore/blob/master/docs/not-implemented.md)

## Templating
Please visit the [Nitro Example Documentation](https://github.com/namics/generator-nitro/blob/master/app/templates/project/docs/nitro.md) informations and samples about Nitro.

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
## Table of contents
- [What's NitroNet](https://github.com/namics/NitroNetSitecore)
- [Installation](https://github.com/namics/NitroNetSitecore/blob/master/docs/installation.md)
- [Configuration](https://github.com/namics/NitroNetSitecore/blob/master/docs/configuration.md)
- [Getting started](https://github.com/namics/NitroNetSitecore/blob/master/docs/getting-started.md)
- [Samples](https://github.com/namics/NitroNetSitecore/blob/master/docs/samples.md)
- [Not yet implemented](https://github.com/namics/NitroNetSitecore/blob/master/docs/not-implemented.md)

## View Samples

#### The easy view sample
This sample shows a simple Nitro Html View.

###### View
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

###### Model
	public class TeaserModel
    {
        public string Headline { get; set; }
        public string Abstract { get; set; }
        public string Richtext { get; set; }
        public string ButtonText { get; set; }
    }

#### A view with repeating subentities

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

#### A view with sub-components
Nested components (a molecule has internally atom components) would be created into the same controller and doesn't invoke a new controller. In the following case, the LocationController has the responsibility to create all parts of the LocationModel.

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

####### Situation A - Component with name attribute
View snippet

	{{component name="Bubble"}}

Model snippet

	public BubbleLocationModel Bubble { get; set; }

####### Situation B - Component with name and data
View snippet

	{{component name="Bubble" data="bubbleLocation"}}

Model snippet

	public BubbleLocationModel BubbleLocation { get; set; }

*or*

	public BubbleLocationModel Bubble { get; set; }


#### A view with placeholders

Placeholders doesn't need a special interaction in user code. In Sitecore you need to create a placeholder setting with the same key as the `name` attribute of the placeholder.

	<div class="m-accordion" data-t-name="Accordion">
		{{{placeholder name="AccordionArea" template="accordion-area"}}}
	</div>

### A Layout view
In Sitecore you can add only components without a datasource or placeholders to a layout-file.

	<!DOCTYPE html>
	<html>
	<head>
		{{partial name="head"}}
	</head>
	<body ontouchstart="">
	<header class="l-header">
		{{{placeholder name="HeaderArea" template="header"}}}
	</header>
	<div class="l-container l-container--tile l-container--tile-home">
		<div class="l-1of1">
			{{component name="Breadcrumb"}}
		</div>
		<div class="l-tile l-tile--home-movie l-1of1">
			<div class="l-tile__content l-tile__content--full">
				{{placeholder name="StageArea" template="stage"}}
			</div>
		</div>
		{{placeholder name="ContentArea" template="home"}}
	</div>
	<footer class="l-footer">
		{{{placeholder name="FooterArea" template="footer"}}}
	</footer>
	{{partial name="foot" }}
	</body>
	</html>

##### Partials
Partials are simple includes into a base file and could be used for reducing the complexity of large view-files.
NitroNet supports partials with the keyword `partial`:

	{{partial name="head"}}


##### Components
Components in Layout-Files have an other execution behaviour than in normal components and revoke all time a MVC Controller with the same name.

	{{component name="Breadcrumb"}}

This component calls a Controller with classname `BreadcrumbController` in our website project.
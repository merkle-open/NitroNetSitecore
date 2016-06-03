## Table of contents
- [What's NitroNet](https://github.com/namics/NitroNetSitecore)
- [Installation](https://github.com/namics/NitroNetSitecore/blob/master/docs/installation.md)
- [Configuration](https://github.com/namics/NitroNetSitecore/blob/master/docs/configuration.md)
- [Getting started](https://github.com/namics/NitroNetSitecore/blob/master/docs/getting-started.md)
- [Samples](https://github.com/namics/NitroNetSitecore/blob/master/docs/samples.md)
- [Not yet implemented](https://github.com/namics/NitroNetSitecore/blob/master/docs/not-implemented.md)

## View Samples

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
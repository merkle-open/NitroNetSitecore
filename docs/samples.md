## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Known Issues](known-issues.md)

## Templating
Please visit the [Nitro Documentation](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md) about informations and samples for working with Nitro.

## Layout view

### An easy example
In Sitecore you can add only partials, placeholders or static components (which have no Sitecore datasource) to a layout file:

```html
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
```

In order to get this layout view to work you need to create the following things:
- A Sitecore placeholder item for each placeholder (3x in this example)
- A Sitecore layout item
- A Sitecore controller rendering called `Breadcrumb`
- A C# controller `BreadcrumbController`.

Please follow the [Getting started](getting-started.md) page for this simple case.

### Model on layout level

In the above example you can only see partials, placeholders and components in the markup without data properties. But what if you need to add direct value, e.g. a simple string indicating the language?

You can make use of the out of the box functionality of Sitecore where you can define a model on your layout. First change your layout to this:

```html
<!DOCTYPE html>
<html lang="{{htmlLanguage}}">
    ...
</html>
```

Create a model inheriting from the Sitecore.Mvc.Presentation.IRenderingModel and implement the `Initialize()` method where you set your properties:

```csharp
public class DefaultRenderingModel : IRenderingModel
{
    public string HtmlLanguage { get; set; }

    public void Initialize(Rendering rendering)
    {
        HtmlLanguage = Sitecore.Context.Language.Name;
    }
}
```

In the Sitecore backend create a new model item under `/sitecore/layout/Models`. On the newly created item in the field `Model Type` enter the fully qualified name of the DefaultRenderingModel you've created above (e.g. `MyProject.Internet.Model.DefaultRenderingModel, MyProject.Internet.Model`).

Finally you need to link your layout to the model item. Do this in the `Model` field of the layout item. Reload your page and voila, the value is there.

### Controller invocation with template and data variation

With NitroNet it is possible to determine with which template and data variation a Controller was called.
All you have to do is to add the two parameters `string template` `string data` to the `Index()` method of the Controller.

Let's look at the following example:

View snippet:

```html
<div class="o-footerContainer">
    {{title}}
    <div class="o-footerContainer__column"> 
        {{component name="footer-link-list" template="templateB"}}
    </div>
    <div class="o-footerContainer__column">
        {{component name="footer-link-list" data="social"}}
    </div>
</div>
```

`Index()` method:

```csharp
public ActionResult Index(string data, string template)
{
    FooterLinkListViewModel model;
    if (dataVariation.Equals("social", StringComparison.InvariantCultureIgnoreCase))
    {
        model = _service.CreateSocialLinks()
    }
   	else if (template.Equals("templateB", StringComparison.InvariantCultureIgnoreCase))
    {
        model = _service.CreateBLinks()
    }
    else
    {
        model = _service.CreateFooterLinks();
    }

    return View("[path to your component folder]/footer-link-list", model);
}
```

For direct values like `title` there needs to be property, for the `footer-link-list` component not.

## Standard Handlebars helpers

### Partials

Partials are simple includes of static markup into a base file and could be used for reducing the complexity of large view files.

**TODO: Rework**
NitroNet supports partials with the keyword `partial`:

```
{{partial name="head"}}
```

### Placeholders

Placeholders don't need a special interaction in user code. In Sitecore you need to create a placeholder setting item with the same key value as the `name` attribute of the placeholder. The `template` parameter is completely ignored by NitroNet.

```html
<div class="m-accordion" data-t-name="Accordion">
    {{placeholder name="AccordionArea" template="accordion-area"}}
</div>
```

#### Identical placeholders on the same level

You might run into situations where you want to have multiple placeholders with the same key on the same hierarchical level. Therefore you need to use the `index` or `indexProp` attribute.

##### index attribute

```html
<div class="m-accordion" data-t-name="Accordion">
    {{{placeholder name="AccordionArea" index="1"}}}
    {{{placeholder name="AccordionArea" index="2"}}}
</div>
```

##### indexProp attribute

View snippet:

```html
<div class="m-accordion" data-t-name="Accordion">
    {{#each items}}
        {{placeholder name="AccordionArea" indexProp="Id"}}
    {{/each}}
</div>
```

Model snippet:

```csharp
public class AccordionModel
{
    public IEnumerable<AccordionItemModel> Items { get; set; }
}

public class AccordionItemModel
{
    public string Id { get; set; }
}
```

### Components

#### An easy example
This sample shows a simple Nitro HTML view (handlebars markup) of a *Teaser* component.

##### Handlebars markup

```html
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
```

##### C# Model
The corresponding C# model is the following:

```csharp
public class TeaserModel
{
    public string Headline { get; set; }
    public string Abstract { get; set; }
    public string Richtext { get; set; }
    public string ButtonText { get; set; }
}
```

#### A component with repeating subentities

##### Handlebars markup

```html
<ul class="m-link-list">
    {{#each links}}
        <li class="m-link-list__item font-meta-navi"><a class="a-link" href="{{target}}">{{linkText}}</a></li>
    {{/each}}
</ul>
```

##### C# Models

In this case, we need a model class with an `IEnumerable<>` property called `links`. The parent model itself doesn't have any other properties:

```csharp
public class LinkListModel
{
    public IEnumerable<LinkModel> Links { get; set; }
}

public class LinkModel
{
    public string Target { get; set; }
    public string LinkText { get; set; }
}
```

#### A component with subcomponents
There are two ways to deal with a component who contains subcomponents (e.g. a molecule that consists of one or more atoms):

##### 1) Create a Controller for each or selected subcomponents
Sometimes it is necessary to controll the caching of the individual subcomponents (e.g. header component) and therefore necessary to create Sitecore controller renderings and C# Controllers for these subcomponents. If you do not provide a model of the subcomponent, NitroNet tries to invoke a Controller for this subcomponent. You can also create a Sitecore controller rendering for it if you need to set special caching settings for this component because it is run through the rendering pipeline. NitroNet for Sitecore internally invokes the controller respectively the rendering pipeline.

In the following example we will look more detailed into that:

View snippet:

```html
<div class="o-footerContainer">
    {{title}}
    <div class="o-footerContainer__column"> 
        {{component name="footer-link-list"}}
    </div>
    <div class="o-footerContainer__column">
        {{component name="footer-link-list" data="social"}}
    </div>
</div>
```

C# Model:

```csharp
public class FooterContainerModel
{
    public string Title { get; set; }
}
```

C# Controller for the `footer-link-list`:

```csharp
public class FooterLinkListController : Controller
{
    private readonly IFooterLinkService _service;

    public FooterLinkListController(IFooterLinkService service)
    {
        _service = service;
    }

    public ActionResult Index(string data)
    {
        FooterLinkListViewModel model;
        if (data.Equals("social", StringComparison.InvariantCultureIgnoreCase))
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
```

For direct values like `title` there needs to be property, for the `footer-link-list` component not.

##### 2) Create only one Controller for the parent component
Nested components are handled by one Controller action and don't invoke a new Controller action for each subcomponent. But it is necessary that you provide a model of the subcomponent.

In the following example we will look more detailed into that:
The `LocationController` has the responsibility to create all parts of the `LocationModel` which also includes data of the sub component `Bubble`.

##### Handlebars markup

```html
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

<div class="a-bubble" data-t-name="Bubble">
    {{description}}
	<a data-location-key="{{key}}" href="{{target}}">{{name}}</a>
</div>
```

##### C# Models

```csharp
public class LocationModel
{
    public string SelectedLocation { get; set; }
    public BubbleLocationModel BubbleLocation { get; set; }
    public IEnumerable<LocationModel> Locations { get; set; }
}

public class BubbleModel
{
	public string Description { get; set; }
    public string Key { get; set; }
    public string Target { get; set; }
    public string Name { get; set; }
}
```

You need to make sure that there is always a property defined in the model for each subcomponent. It holds the data which is then passed to the subcomponent. Make sure that it either matches the `name` or `data` (but only when the `data` attribute is present) attribute of the corresponding component helper. You don't need to worry about case sensitivity and hyphens.

###### Situation A - Component with name
View snippet:

```
{{component name="Bubble"}}
```

Model snippet (maps the `name` attribute)

```csharp
public BubbleModel Bubble { get; set; }
```

###### Situation B - Component with name and data
View snippet:
```
{{component name="Bubble" data="bubbleLocation"}}
```

Model snippet (maps the `data` attribute)

```csharp
public BubbleModel BubbleLocation { get; set; }
```

## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Known Issues](known-issues.md)

## Getting started with NitroNet

### Create a Controller

To use a Nitro based component in Sitecore, you have to create a Controller which inherits the normal `System.Web.Mvc.Controller` of ASP.NET MVC:

```csharp
public class TeaserController : System.Web.Mvc.Controller
{
	// GET: Teaser
	public ActionResult Index()
	{
		var model = new TeaserModel
		{
			Headline = "Lorem ipsum",
			Abstract = "Praesent ac massa at ligula laoreet iaculis. Cras id dui.",
			Richtext = "<p>Sed lectus. Suspendisse nisl elit, rhoncus eget, <a href='#'>elementum ac</a>, condimentum eget, diam. Curabitur turpis. Ut non enim eleifend felis pretium feugiat. Vivamus aliquet elit ac nisl.</p>",
			ButtonText = "Button text"
		};

		return View("teaser", model);
	}
}
```

The whole magic of NitroNet happens on the returning line `return View("teaser", model);`. The string `"teaser"` must fit the directory path of a Nitro component relative to your `NitroNet.BasePath`.

The guidelines about how to create a corresponding C# model for the selected Nitro component get explained in the next section.

### Create a C# Model
It is quite easy to create a C# model based on the Nitro component.

Let's take a look at a specific example (teaser component from the previous section). In the Nitro folder of your component you find all needed information for the model definition.
In the handlebars file (mostly `.hbs` or `.html`) you will see all the properties of the component:

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

To determine the data types of the individual properties you have to look at the *data.json* of the component. It is located under `./_data/<molecule-name>.json`. Oftentimes there is more than one *data.json* for the different states the component can have.

```json
{
	"headline" : "Lorem ipsum",
	"abstract" : "Praesent ac massa at ligula laoreet iaculis. Cras id dui.",
	"richtext" : "<p>Sed lectus. Suspendisse nisl elit, rhoncus eget, <a href='#'>elementum ac</a>, condimentum eget, diam. Curabitur turpis. Ut non enim eleifend felis pretium feugiat. Vivamus aliquet elit ac nisl.</p>",
	"ButtonText" : "Button text"
}
```

In this case every property is of type `string`.

Now let's create the corresponding C# model class. Please make sure that the properties have the same name. The only thing you don't need to worry about is case sensitivity. Possible hyphens are also ignored:

```csharp
public class TeaserModel
{
    public string Headline { get; set; }
    public string Abstract { get; set; }
    public string Richtext { get; set; }
    public string ButtonText { get; set; }
}
```

#### Supported Types

Every possible data type is supported if it corresponds with the Nitro component data structure (as mentioned before the *data.json* is crucial here).

```csharp
public class FooModel
{
    public string Text { get; set; }
    public int Numeric { get; set; }
    public bool Abstract { get; set; }
    public SpecialClassModel Bar { get; set; }
    public IEnumerable<SpecialClassModel> Items { get; set; }
    ...
}
```

### Create a Layout in Sitecore

Create a layout item below `/sitecore/layout/Layouts` and set the `Path` field relative to path you configured in the setting `NitroNet.BasePath`. Please make sure that you set the file name without the file extension.

#### Example

Preconditions:
- View file path is `/Nitro/Sample/frontend/views/layout.html`
- `NitroNet.BasePath` setting is `Nitro/Sample`

The resulting `Path` field value is `frontend/views/layout`

### Create a Controller rendering in Sitecore

You only need to create Controller renderings for static components or renderings you want to place on a placeholder. In addition a big advantage when creating a Sitecore Controller rendering for a Nitro component is that you can configure the Sitecore caching for this component.
For more information on this matter please follow the [Samples](samples.md) page.

Create a Controller rendering item below `/sitecore/layout/Renderings` and set the item name accoording to the components directory name (hyphens and case sensitivity can be ignored). Also set the `Controller` field to the controller you have created as shown in the example above.
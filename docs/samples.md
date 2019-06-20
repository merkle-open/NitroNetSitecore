## Table of contents
- [Installation](installation.md)
- [Configuration](configuration.md)
- [Getting started](getting-started.md)
- [Samples](samples.md)
- [Release Notes](https://github.com/namics/NitroNetSitecore/releases)
- [Known Issues](known-issues.md)


Please see the [samples of NitroNet](https://github.com/namics/NitroNet/blob/master/docs/getting-started.md) to see some code samples for the basic MVC features of NitroNet and informations and samples for working with plain handlebars and/or Nitro.

Some helpers mentioned below are custom handlebars helpers from Nitro such as the component or the placeholder helper.

## Layout view

### An easy example
When working with Sitecore you can add only partials, placeholders or static components (which have no Sitecore datasource) to a layout file:

```hbs
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

In the above example you can only see partials, placeholders and components in the markup without data properties. But what if you need to add direct values to the first level of your handlebars hierarchy, e.g. a simple string indicating the language?

You can make use of the out of the box functionality of Sitecore where you can define a model on your layout. First change your layout to this:

```hbs
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

### Controller invocation with template and data attribute

With NitroNet for Sitecore it is possible to determine with which template and data attribute a Controller was called.
All you have to do is to add the two parameters `string template` `string data` to the `Index()` method of the Controller.

Let's look at the following example:

View snippet:

```hbs
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
    if (data.Equals("social", StringComparison.InvariantCultureIgnoreCase))
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

For direct values like `title` there needs to be a property, for the `footer-link-list` component not.

### Use of Sitecore edit frames

An important feature when developing with Sitecore is the use of edit frames for the Experience Editor.  
The following example shows you how you can achieve this when using *handlebars* and *NitroNet.Sitecore*.

#### 1.) Create a view model
In the first step you have to create C# view model for your edit frame:

```csharp
public class EditFrame
{
    public string EditFrameStart { get; set; }
    public string EditFrameStop { get; set; }
}
```

#### 2.) Write a method to populate the view model
In the second step you have to write a method for populating the edit frame view model. You can copy the following method or use it as a basis for your own implementation:

```csharp
public EditFrame GetEditFrame(string path, string buttons, string title, string tooltip, string css, object parameters)
{
    var result = new EditFrame();
    if (!Sitecore.Context.PageMode.IsExperienceEditorEditing)
        return result;

    var output = new StringWriter();
    var writer = new HtmlTextWriter(output);
    var editFrame = new Sitecore.Mvc.Common.EditFrame(path,buttons, title, tooltip, css, parameters);
    editFrame.RenderFirstPart(writer);
    result.EditFrameStart = output.ToString();

    output = new StringWriter();
    writer = new HtmlTextWriter(output);
    editFrame.RenderLastPart(writer);
    result.EditFrameStop = output.ToString();

    return result;
}
```

#### 3.) Use the edit frame in your views
And last but not least you need this handlebars markup to activate the edit frame.

```html
{{{editframe.editframeStart}}}
	<!--
    	Markup you want to enclose with the edit frame
    -->
{{{editframe.editframeStop}}}
```

Please keep in mind that the context object needs to have a property with the following type and name:

```csharp
public EditFrame EditFrame { get; set; }
```

## Nitro helpers for Sitecore

### Placeholders

Placeholders don't need a special interaction in user code. In Sitecore you need to create a placeholder setting item with the same key value as the `name` attribute of the placeholder. The `template` parameter is completely ignored by NitroNet.

```html
<div class="m-accordion" data-t-name="Accordion">
    {{placeholder name="AccordionArea" template="accordion-area"}}
</div>
```

#### Identical placeholders on the same level

You might run into situations where you want to have multiple placeholders with the same key on the same hierarchical level. Therefore you need to use the `index` or `indexprop` attribute. NitroNet uses Dynamic Placeholders by default.

##### index attribute

```html
<div class="m-accordion" data-t-name="Accordion">
    {{placeholder name="AccordionArea" index="1"}}
    {{placeholder name="AccordionArea" index="2"}}
</div>
```

##### indexprop attribute

View snippet:

```html
<div class="m-accordion" data-t-name="Accordion">
    {{#each items}}
        {{placeholder name="AccordionArea" indexprop="Id"}}
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


### A component with subcomponents
There are two ways to deal with a component who contains subcomponents (e.g. a molecule that consists of one or more atoms):

#### 1) Create a Controller for each or selected subcomponents
Sometimes it is necessary to control the caching of the individual subcomponents (e.g. header component) and therefore necessary to create Sitecore controller renderings and C# Controllers for these subcomponents. If you do not provide a model of the subcomponent, NitroNet tries to invoke a Controller for this subcomponent. You can also create a Sitecore controller rendering for it if you need to set special caching settings for this component because it is run through the rendering pipeline. NitroNet for Sitecore internally invokes the controller respectively the rendering pipeline.

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

For direct values like `title` there needs to be property, for the `footer-link-list` component not. At this point the controller gets invoked directly.

If you want to cache the `footer-link-list` component specifically you need to create a Controller Rendering called *FooterLinkList* (hyphens and case sensitivity can be ignored) in Sitecore. Now, the rendering pipeline gets invoked and you can set all the caching configurations for the component. Note that the `data` attribute is considered if you choose *Vary By Data*.


#### 2) Create only one Controller for the parent component
You can find the code example and explanations for this case [here](https://github.com/namics/NitroNet/blob/master/docs/samples.md#a-component-with-subcomponents).

## Additional Arguments

With the introduction of the [Additional Arguments](https://github.com/namics/NitroNet/blob/master/docs/additional-arguments.md) feature, you must pay attention when you fully enable it.

Your frontend Handlebars templates may have additional arguments as this feature has already been supported by Nitro for quite some time now. Up to now those parameters were ignored.

### LiteralParsingMode

With **LiteralParsingMode** enabled (either **StaticLiteralsOnly** or **Full**) the parameters get resolved and are applied to the model. To show the impact we look at an example and show how it looks in the end with the different modes.

**Example Setup**

```csharp
// The model
public class ParentComponentModel {
    public string Text { get; set; }
    public string Modifier { get; set; }
    public SubComponentModel SubComponent { get; set;}
}

public class SubComponentModel {
    public string Title { get; set; }
    public string Content { get; set; }
    public string Modifier { get; set; }
    public string Decorator { get; set; }
}

// Controller snippet
public ActionResult Index() {
    var subComponent = new SubComponentModel {
        Title = "Sub title",
        Content = "SubComponent content",
        Decorator = "subcomponent-decorator"
    };

    return View("parentComponent", new ParentComponentModel {
        Text = "Parent"
        Modifier = "parent-modifier"
        SubComponent = subComponent
    });
}
```

```hbs
{{! The ParentComponent calls the subcomponent }}
<div class="molecule-parent{{#if modifier}} {{modifier}}{{/if}}">
    <p>{{text}}</p>
    {{ pattern name="subcomponent" modifier=modifier decorator="overwrite-decorator"}}
</div>

{{! The SubComponent }}
<div class="atom-subcomponent{{#if modifier}} {{modifier}}{{/if}}{{#if decorator}} {{decorator}}{{/if}}">
    <p>{{title}}</p>
    <div>{{content}}</div>
</div>
```

**Result LiteralParsingMode:None**

Additional Arguments are ignored.
```html
<div class="molecule-parent parent-modifier">
    <p>Parent</p>
    <div class="atom-subcomponent subcomponent-decorator">
        <p>Sub title</p>
        <div>SubComponent content</div>
    </div>
</div>
```

**Result LiteralParsingMode:StaticLiteralsOnly**

Only static literals are parsed and overwrites the value set in the controller.
```html
<div class="molecule-parent parent-modifier">
    <p>Parent</p>
    <div class="atom-subcomponent overwrite-decorator">
        <p>Sub title</p>
        <div>SubComponent content</div>
    </div>
</div>
```

**Result LiteralParsingMode:Full**

All values are resolved. As you can see, the modifier on from the parent model is recognized, resolved from the parent model and passed to the sub component.
```html
<div class="molecule-parent parent-modifier">
    <p>Parent</p>
    <div class="atom-subcomponent parent-modifier overwrite-decorator">
        <p>Sub title</p>
        <div>SubComponent content</div>
    </div>
</div>
```
### AdditionalArgumentsOnlyComponents
There is also an additional settings which enables components to be rendered without a name-matching model on the current context or without explicitly specifying `data`. In our example from before this would mean you need no `SubComponentModel` on the `ParentComponentModel`, you can just pass all values needed via additional arguments. Lets have a look at an example and compare the behaviour of **AdditionalArgumentsOnlyComponents** enabled vs disabled.

**Example**

```csharp
// The model
public class ParentComponentModel {
    public string Text { get; set; }
    public string Modifier { get; set; }

    public string Title { get; set; }
    public string Content { get; set; }
}

// Controller snippet
public ActionResult Index() {
    var subComponent = new SubComponentModel {
        
        
        Decorator = "subcomponent-decorator"
    };

    return View("parentComponent", new ParentComponentModel {
        Text = "Parent"
        Modifier = "parent-modifier"
        Title = "Sub title",
        Content = "SubComponent content",
    });
}
```
```hbs
{{! The ParentComponent calls the subcomponent }}
<div class="molecule-parent{{#if modifier}} {{modifier}}{{/if}}">
    <p>{{text}}</p>
    {{ pattern name="subcomponent" title=title content=content modifier=false decorator="overwrite-decorator"}}
</div>

{{! The SubComponent }}
<div class="atom-subcomponent{{#if modifier}} {{modifier}}{{/if}}{{#if decorator}} {{decorator}}{{/if}}">
    <p>{{title}}</p>
    <div>{{content}}</div>
</div>
```

**Result AdditionalArgumentsOnlyComponents:false**

This is the behaviour as you know it. If no matching sub model is found and no `data` attribute is specified, NitroNet tries to find a matching rendering or controller (see [here](#1-create-a-controller-for-each-or-selected-subcomponents)). 


**Result AdditionalArgumentsOnlyComponents:true**

Remember, this setting only has an effect if **LiteralParsingMode** is enabled (see [here](https://github.com/namics/NitroNet/blob/master/docs/additional-arguments.md)).

As you can see no sub model is available on `ParentComponentModel`. But there are several arguments in the sub component call in the Handlebars. NitroNet collects and resolves the values and if this setting is true, creates a dictionary which is passed to sub component without calling a controller.

The result looks the following:
```html
<div class="molecule-parent parent-modifier">
    <p>Parent</p>
    <div class="atom-subcomponent overwrite-decorator">
        <p>Sub title</p>
        <div>SubComponent content</div>
    </div>
</div>
```

### forceController
In some cases you may want to overwrite the above behaviour and call a controller even if additional arguments are available. Therefore the parameter `forceController` was introduced.

Adding this parameter and set it to "true" forces NitroNet to call a controller even if the component could have been rendered from either the model or the additional arguments alone.

**Example**
```hbs
{{! The ParentComponent calls a controller }}
<div class="molecule-parent{{#if modifier}} {{modifier}}{{/if}}">
    <p>{{text}}</p>
    {{ pattern name="subcomponent" title=title content=content forceController="true"}}
</div>
```

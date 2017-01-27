## Table of contents
- [What's NitroNet](https://github.com/namics/NitroNetSitecore)
- [Installation](https://github.com/namics/NitroNetSitecore/blob/master/docs/installation.md)
- [Configuration](https://github.com/namics/NitroNetSitecore/blob/master/docs/configuration.md)
- [Getting started](https://github.com/namics/NitroNetSitecore/blob/master/docs/getting-started.md)
- [Samples](https://github.com/namics/NitroNetSitecore/blob/master/docs/samples.md)
- [Known Issues](https://github.com/namics/NitroNetSitecore/blob/master/docs/known-issues.md)

### Limitations ###

#### Caching of components called by Html.Sitecore().Rendering(pathOrId)

The versions Sitecore 8.1 and 8.2 have a bug that the above mentioned method doesn't read the cache settings correctly. As NitroNetSitecore uses this internally to render components where a Sitecore rendering exists for, those components won't be cached correctly.

Sitecore provides two support DLLs for these issues (387950 and 414987) which are now included in the solution.

As explained in the [samples](https://github.com/namics/NitroNetSitecore/blob/master/docs/samples.md#a-view-with-sub-components-and-datavariation) there is the possibility to statically add sub-components to another component and those sub-components are rendered via the Sitecore rendering pipeline. NitroNetSitecore needs to add this dataVariation parameter to the cache key in order for the components to be cached correctly. Therefore we made some further adjustments to the GenerateCacheKey processor of the mvc.renderRendering pipeline. This is also automatically included in the solution.

**Important:** If you already have included these fixes or customized the SetCacheability or the GenerateCacheKey processor in your solution please pay attention!

### Actually not implemented

This guide explains all exclusions of NitroNet and talks about features of Nitro which aren't implemented in NitroNet yet.

###### The Nitro documentation

First of all: You can find under this [link](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md) the full documentation of Nitro.

#### Components without attributes
Nitro knows two syntaxes for components:

##### Variant A:

    {{component 'Example' exampleContent}}

##### Variant B:

	{{component name='Example' data='example-variant'}}

with the extended variant:

	{{component name='Example' data='example-variant' template='example-2'}}

NitroNet supports only the absolut declerative type of **Variant B**.

#### Translation handlebars helper
Nitro ships with a Handlebars helper called `t`. This is not supported in NitroNet as there is no need for such a helper in ASP.NET. You can simply use a string expressions in its place and fill the according model property dynamically with data from any source.

#### Flexible attribute on component helper ####
You will find the Nitro documentation for the flexible attribute [here](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md#render-patterns).

In the current version NitroNet does not support this feature. You need to pass a model with a modifier property.

#### Render patterns with children ####

You will find the Nitro documentation for patterns with children [here](https://github.com/namics/generator-nitro/blob/master/generators/app/templates/project/docs/nitro.md#render-patterns-with-children)

In the current version NitroNet does not support this feature. Please use the placeholder feature for this.
## Table of contents
- [What's NitroNet](https://github.com/namics/NitroNetSitecore)
- [Installation](https://github.com/namics/NitroNetSitecore/blob/master/docs/installation.md)
- [Configuration](https://github.com/namics/NitroNetSitecore/blob/master/docs/configuration.md)
- [Getting started](https://github.com/namics/NitroNetSitecore/blob/master/docs/getting-started.md)
- [Samples](https://github.com/namics/NitroNetSitecore/blob/master/docs/samples.md)
- [Not yet implemented](https://github.com/namics/NitroNetSitecore/blob/master/docs/not-implemented.md)


### Actually not implemented

This guide explains all exclusions of NitroNet and talks about features of Nitro which aren't implemented in NitroNet yet.

###### The Nitro documentation

First of all: You can find under this [link](https://github.com/namics/generator-nitro/blob/master/app/templates/project/docs/nitro.md) the full documentation of Nitro.

#### Partials
The handlebars helper for partials is typically known as `>`. This helper is currently not supported by NitroNet. As a workaround you might want to use `partial` instead:

##### In Nitro:

    {{> head}}

##### In NitroNet:

	{{partial name="head" }}

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
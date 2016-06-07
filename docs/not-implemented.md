## Table of contents
- [What's NitroNet](https://github.com/namics/NitroNetSitecore)
- [Installation](https://github.com/namics/NitroNetSitecore/blob/master/docs/installation.md)
- [Configuration](https://github.com/namics/NitroNetSitecore/blob/master/docs/configuration.md)
- [Getting started](https://github.com/namics/NitroNetSitecore/blob/master/docs/getting-started.md)
- [Samples](https://github.com/namics/NitroNetSitecore/blob/master/docs/samples.md)
- [Not yet implemented](https://github.com/namics/NitroNetSitecore/blob/master/docs/not-implemented.md)


### Actually not implemented

This guide explains all excludions of NitroNet and talks about the not implemented features of Nitro in NitroNet.

###### The Nitro Documentation

First of all: You can find under this [link](https://github.com/namics/generator-nitro/blob/master/app/templates/project/docs/nitro.md) the full documentation of Nitro.

#### Partials
Nitro knows two syntaxes for partials:

##### Variant A:

    {{> head}}

##### Variant B:

	{{partial name="head" }}

NitroNet supports (at the moment) only the absolut declerative type of **Variant B**.

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
The translation handlebar helper is actually not implemented in NitroNet for Sitecore. We will implement this feature in the near future and connect this helper to the Sitecore Dictionary feature.

#### Using another Template Engine
NitroNet supports at the moment only handlebars.js Templates or normal *.cshtml Views.
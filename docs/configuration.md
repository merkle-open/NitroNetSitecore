## Table of contents
- [What's NitroNet](https://github.com/namics/NitroNetSitecore)
- [Installation](https://github.com/namics/NitroNetSitecore/blob/master/docs/installation.md)
- [Configuration](https://github.com/namics/NitroNetSitecore/blob/master/docs/configuration.md)
- [Getting started](https://github.com/namics/NitroNetSitecore/blob/master/docs/getting-started.md)
- [Samples](https://github.com/namics/NitroNetSitecore/blob/master/docs/samples.md)
- [Not yet implemented](https://github.com/namics/NitroNetSitecore/blob/master/docs/not-implemented.md)

## Configuration

### Connect the Nitro Frontend to your Application
The location of your Nitro-Application could be configure very flexible.

As default-value it would be taken the website-root-folder itself. If you like to change it, you can add an AppSetting with the Key-Value `NitroNet.BasePath` in your *web.config*.

	<appSettings>
	    <add key="NitroNet.BasePath" value ="Nitro/Sample" />
	</appSettings>

In addition, you got a new `config.json`-File in the root-directory of the website-project after installation of NitroNet:

	{
	  "viewPaths": [
	    "frontend/views"
	  ],
	  "partialPaths": [
	    "frontend/views/_partials"
	  ],
	  "componentPaths": [
	    "frontend/components/atoms",
	    "frontend/components/molecules",
	    "frontend/components/organisms"
	  ]
	}

That `config.json` File defines your *view*-, *partial*- and *components*-paths, starting on your `NitroNet.BasePath`.

###### Conclusion
Back to my sample configuration, the *atoms* would be located under `~/Nitro/Sample/frontend/components/atoms/`. That's all about view-logic resolving.
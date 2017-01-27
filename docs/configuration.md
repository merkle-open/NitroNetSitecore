## Table of contents
- [What's NitroNet](https://github.com/namics/NitroNetSitecore)
- [Installation](https://github.com/namics/NitroNetSitecore/blob/master/docs/installation.md)
- [Configuration](https://github.com/namics/NitroNetSitecore/blob/master/docs/configuration.md)
- [Getting started](https://github.com/namics/NitroNetSitecore/blob/master/docs/getting-started.md)
- [Samples](https://github.com/namics/NitroNetSitecore/blob/master/docs/samples.md)
- [Known Issues](https://github.com/namics/NitroNetSitecore/blob/master/docs/known-issues.md)

## Configuration

### Connect the Nitro frontend to your application
The location of your Nitro application could be configured very flexible.

As default it is set to be located at the root folder of your web application. If you like to change it, you can add an AppSetting with the Key-Value `NitroNet.BasePath` in your *web.config*. 

Sample:

	<appSettings>
	    <add key="NitroNet.BasePath" value ="Nitro/Sample" />
	</appSettings>

In addition, you got a new `config.json`-File in the root directory of the website project after installation of NitroNet:

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

The `config.json` file defines *view*-, *partial*- and *components*-paths, starting at your `NitroNet.BasePath`.
In this case, the *atoms* would be located under `~/Nitro/Sample/frontend/components/atoms/`. That's all about view logic resolving of Nitro.
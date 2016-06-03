## Table of contents
- [What's NitroNet](https://github.com/namics/NitroNetSitecore)
- [Installation](https://github.com/namics/NitroNetSitecore/blob/master/docs/installation.md)
- [Configuration](https://github.com/namics/NitroNetSitecore/blob/master/docs/configuration.md)
- [Getting started](https://github.com/namics/NitroNetSitecore/blob/master/docs/getting-started.md)
- [Samples](https://github.com/namics/NitroNetSitecore/blob/master/docs/samples.md)
- [Not yet implemented](https://github.com/namics/NitroNetSitecore/blob/master/docs/not-implemented.md)

## Getting started with NitroNet

#### Create a Controller

To use a Nitro based component in Sitecore, you can create a normal `System.Web.Mvc.Controller` of ASP.NET MVC:

	public class TeaserController : System.Web.Mvc.Controller
	    {
	        // GET: Teaser
	        public ActionResult Index()
	        {
	            var model = new TeaserModel
	            {
	                Headline = "Headline first-line",
	                Abstract = "Praesent ac massa at ligula laoreet iaculis. Cras id dui. Integer ante arcu, accumsan a, consectetuer eget, posuere ut, mauris. Fusce fermentum odio nec arcu.",
	                Richtext = "<p>Sed lectus. Suspendisse nisl elit, rhoncus eget, <a href='#'>elementum ac</a>, condimentum eget, diam. Curabitur turpis. Ut non enim eleifend felis pretium feugiat. Vivamus aliquet elit ac nisl.</p><ul><li>Primis in faucibus orci luctus et ultrices</li><li>Dignissim dolor, <a href='#'>pretium</a> mi sem ut ipsum.</li><li>Etiam ut purus mattis mauris sodales aliquam.</li></ul>",
	                ButtonText = "Button Text"
	            };
	
	            return View("teaser", model);
	        }
	    }

The whole magic would be execute on the returning line `return View("teaser", model);`

The string `"teaser"` must be fit with the name of a Nitro component. The guidelines of a model would be explained under chapter *"Create a Model"*.

#### Create a Model

The Model definition is very simple and smart. In the Nitro Folder of your selected component (e.g. `.../frontend/components/molecules/teaser`) it's possible to see all needed information of your model definition. This is the contract with the Frontend. You can find it under `./_data/<molecule-name>.json`:

	{
		"headline" : "Headline first-line",
		"abstract" : "Praesent ac massa at ligula laoreet iaculis. Cras id dui. Integer ante arcu, accumsan a, consectetuer eget, posuere ut, mauris. Fusce fermentum odio nec arcu.",
		"richtext" : "<p>Sed lectus. Suspendisse nisl elit, rhoncus eget, <a href='#'>elementum ac</a>, condimentum eget, diam. Curabitur turpis. Ut non enim eleifend felis pretium feugiat. Vivamus aliquet elit ac nisl.</p><ul><li>Primis in faucibus orci luctus et ultrices</li><li>Dignissim dolor, <a href='#'>pretium</a> mi sem ut ipsum.</li><li>Etiam ut purus mattis mauris sodales aliquam.</li></ul>",
		"ButtonText" : "Next"
	}

In this case, create an equivalent .Net Class with same properties:

	public class TeaserModel
    {
        public string Headline { get; set; }
        public string Abstract { get; set; }
        public string Richtext { get; set; }
        public string ButtonText { get; set; }
    }

##### Supported Types

	public class FooModel
	{
	    public string Text { get; set; }
		public int Numeric { get; set; }
	    public bool Abstract { get; set; }
	    public SpecialClassModel Bar { get; set; }
	    public IEnumerable<SpecialClassModel> Items { get; set; }
		...
	}

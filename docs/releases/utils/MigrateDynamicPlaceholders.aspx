<%@ Page Language="C#" AutoEventWireup="true" Debug="true" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore" %>
<%@ Import Namespace="Sitecore.Configuration" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Sitecore.Data.Fields" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Sitecore.Layouts" %>

<script language="C#" runat="server">

    /// <summary>
    /// GET Parameters
    ///
    /// database: The Sitecore database. Example: web
    /// 
    /// itemId: The dynamic placeholder migration is only processed on this start item. Example: {456B38B8-1C42-48AF-858E-FC58A2FC1491}
    /// 
    /// enableRecursion: This option enables recursion for the start item. Example: true
    /// </summary>

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Sitecore.Context.User.IsAdministrator)
        {
            Response.Write("You have no permission to run this script.");
            return;
        }

        var databaseName = Context.Request.QueryString["database"];
        if (string.IsNullOrEmpty(databaseName))
        {
            databaseName = "master";
        }

        var database = Factory.GetDatabase(databaseName);

        ID startItemId = null;
        Item startItem = null;
        var startItemIdString = Context.Request.QueryString["itemId"];

        if (string.IsNullOrEmpty(startItemIdString))
        {
            Response.Write("No Item ID provided. Please use GET parameter 'itemId'.");
            return;
        }

        if (!string.IsNullOrEmpty(startItemIdString) && !Sitecore.Data.ID.TryParse(startItemIdString, out startItemId))
        {
            Response.Write("Invalid Item ID.");
            return;
        }

        if (!string.IsNullOrEmpty(startItemIdString))
        {
            startItem = database.GetItem(startItemId);
        }

        if (!string.IsNullOrEmpty(startItemIdString) && startItem == null)
        {
            Response.Write(string.Format("No item found for ID {0}.", startItemIdString));
            return;
        }

        bool isRecursionEnabled;
        bool.TryParse(Context.Request.QueryString["enableRecursion"], out isRecursionEnabled);

        var fixRenderings = new UpgradeDynamicPlaceholderHelper(database);

        Sitecore.Diagnostics.Log.Info("Dynamic Placeholder Migration: Started", this);

        var result = fixRenderings.Iterate(startItem, isRecursionEnabled);

        OutputResult(result);

        Sitecore.Diagnostics.Log.Info("Dynamic Placeholder Migration: Finished", this);
    }

    private void OutputResult(Dictionary<Item, List<KeyValuePair<string, string>>> result)
    {
        Response.ContentType = "text/html";

        Response.Write(string.Format("<h1>{0} items processed</h1>", result.Count));
        foreach (var pair in result)
        {
            Response.Write(string.Format("<h3>{0} [{1}]</h3>", pair.Key.Paths.FullPath, pair.Key.Language.Name));

            foreach (var kvp in pair.Value)
            {
                if (kvp.Key != kvp.Value)
                {
                    Response.Write(string.Format("<div>{0} ==> {1}</div>", kvp.Key, kvp.Value));
                }
            }
        }
    }

    public class UpgradeDynamicPlaceholderHelper
    {
        private const string ItemsWithPresentationDetailsQuery = "{0}//*[@__Renderings != '' or @__Final Renderings != '']";
        private const string DefaultDeviceId = "{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}";
        private static Regex PlaceholderRegex = new Regex(@"^(.+?)(?:_([^_]*))?_([\d\w]{8}\-([\d\w]{4}\-){3}[\d\w]{12})$", RegexOptions.Compiled);

        private readonly Database _database;

        public UpgradeDynamicPlaceholderHelper(Database database)
        {
            _database = database;
        }

        public Dictionary<Item, List<KeyValuePair<string, string>>> Iterate(Item startItem, bool isRecursionEnabled)
        {
            var result = new Dictionary<Item, List<KeyValuePair<string, string>>>();

            var items = new List<Item>();

            if (startItem != null)
            {
                if (isRecursionEnabled)
                {
                    items.AddRange(_database.SelectItems(string.Format(ItemsWithPresentationDetailsQuery, startItem.Paths.FullPath)));
                }
                else
                {
                    items.Add(startItem);
                }
            }
            else
            {
                items.AddRange(_database.SelectItems(string.Format(ItemsWithPresentationDetailsQuery, "/sitecore/content")));
            }

            var layoutFieldIds = new[] {FieldIDs.LayoutField, FieldIDs.FinalLayoutField};

            foreach (var itemInDefaultLanguage in items)
            {
                foreach (var itemLanguage in itemInDefaultLanguage.Languages)
                {
                    var item = itemInDefaultLanguage.Database.GetItem(itemInDefaultLanguage.ID, itemLanguage);
                    if (item.Versions.Count > 0)
                    {
                        foreach (var layoutFieldId in layoutFieldIds)
                        {
                            var layoutField = item.Fields[layoutFieldId];

                            // Don't convert standard values!
                            if (layoutField.ContainsStandardValue)
                            {
                                continue;
                            }

                            var changeResult = ChangeLayoutFieldForItem(item, layoutField);

                            if (changeResult.Any())
                            {
                                if (!result.ContainsKey(item))
                                {
                                    result.Add(item, changeResult);
                                }
                                else
                                {
                                    result[item].AddRange(changeResult);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private List<KeyValuePair<string, string>> ChangeLayoutFieldForItem(Item currentItem, Field field)
        {
            var result = new List<KeyValuePair<string, string>>();

            string xml = LayoutField.GetFieldValue(field);

            if (!string.IsNullOrWhiteSpace(xml))
            {
                var details = LayoutDefinition.Parse(xml);

                var device = details.GetDevice(DefaultDeviceId);

                if (device != null && device.Renderings != null)
                {
                    bool requiresUpdate = false;

                    foreach (RenderingDefinition rendering in device.Renderings)
                    {
                        if (!string.IsNullOrWhiteSpace(rendering.Placeholder))
                        {
                            var placeholderPartsNew = new List<string>();
                            var placeholderParts = rendering.Placeholder.Split('/');
                            var oldPlaceholder = rendering.Placeholder;

                            foreach (var placeholderPart in placeholderParts)
                            {
                                Match match = PlaceholderRegex.Match(placeholderPart);
                                if (!match.Success)
                                {
                                    placeholderPartsNew.Add(placeholderPart);
                                    continue;
                                }

                                var placeholderKey = match.Groups[1].Value;
                                var indexSuffix = match.Groups[2].Value;
                                ID indexSuffixId;
                                var isIndexSuffixId = Sitecore.Data.ID.TryParse(match.Groups[2].Value, out indexSuffixId);
                                ID uniqueRenderingId;
                                if (!Sitecore.Data.ID.TryParse(match.Groups[3].Value, out uniqueRenderingId))
                                {
                                    placeholderPartsNew.Add(placeholderPart);
                                    continue;
                                }

                                placeholderPartsNew.Add(string.Format("{0}-{1}-{2}", placeholderKey, uniqueRenderingId, isIndexSuffixId ? indexSuffixId.ToString() : indexSuffix));

                                var newPlaceholder = string.Join("/", placeholderPartsNew);

                                rendering.Placeholder = newPlaceholder;

                                result.Add(new KeyValuePair<string, string>(field.Name + ": " + oldPlaceholder, newPlaceholder));

                                requiresUpdate = true;
                            }
                        }
                    }

                    if (requiresUpdate)
                    {
                        var newXml = details.ToXml();

                        using (new EditContext(currentItem))
                        {
                            LayoutField.SetFieldValue(field, newXml);
                        }
                    }

                }
            }

            return result;
        }
    }


</script>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
<form id="form1" runat="server">
    <div>
    </div>
</form>
</body>
</html>
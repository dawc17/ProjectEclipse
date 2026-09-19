// unity command eval_file --file Tools/AuditDE128MissingWeaponAssets.cs --json
// Read-only archive inventory. The mod never loads these XML documents.
var core = new System.Xml.XmlDocument(); core.Load("Assets/vanillaXml/list.xml");
var archive = new System.Xml.XmlDocument(); archive.Load("Assets/DExml/list.xml");
var provider = new Eclipse.Modding.CoreAssetProvider();
var rows = new System.Collections.Generic.List<object>();
foreach (System.Xml.XmlElement item in archive.SelectNodes("/List/Items/Item[@Type='Weapon']"))
{
    string name = item.GetAttribute("Name");
    if (core.SelectSingleNode("/List/Items/Item[@Name='" + name + "']") != null) continue;
    string modelText;
    UnityEngine.Sprite icon;
    var model = Eclipse.Modding.AssetId.Parse("core:gamedata/models/" + item.GetAttribute("Model"));
    var image = Eclipse.Modding.AssetId.Parse("core:ui/items/" + item.GetAttribute("Image"));
    Eclipse.Modding.AssetMetadata modelMetadata, iconMetadata;
    bool modelDescribed = provider.TryDescribe(model, out modelMetadata);
    bool iconDescribed = provider.TryDescribe(image, out iconMetadata);
    bool modelLoaded = provider.TryLoadModelText(model, out modelText);
    bool iconLoaded = provider.TryLoadUnityAsset<UnityEngine.Sprite>(image, out icon);
    rows.Add(new { name, model = model.ToString(), icon = image.ToString(), modelDescribed, iconDescribed,
        modelLoaded, modelChars = modelText?.Length ?? 0, iconLoaded, vertices = icon == null ? 0 : icon.vertices.Length });
}
return rows;

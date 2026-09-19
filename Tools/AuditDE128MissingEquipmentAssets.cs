// Read-only editor eval; archive XML is evidence, not shipped mod content.
var core = new System.Xml.XmlDocument(); core.Load("Assets/vanillaXml/list.xml");
var archive = new System.Xml.XmlDocument(); archive.Load("Assets/DExml/list.xml");
var provider = new Eclipse.Modding.CoreAssetProvider();
var rows = new System.Collections.Generic.List<object>();
foreach (System.Xml.XmlElement item in archive.SelectNodes("/List/Items/Item"))
{
    string name = item.GetAttribute("Name"), type = item.GetAttribute("Type");
    if (type != "Armor" && type != "Helm" && type != "Ranged" && type != "Magic") continue;
    if (item.GetAttribute("ShopHide") == "1" || core.SelectSingleNode("/List/Items/Item[@Name='" + name + "']") != null) continue;
    string text; UnityEngine.Sprite icon;
    var model = Eclipse.Modding.AssetId.Parse("core:gamedata/models/" + item.GetAttribute("Model"));
    var image = Eclipse.Modding.AssetId.Parse("core:ui/items/" + item.GetAttribute("Image"));
    bool modelLoaded = provider.TryLoadModelText(model, out text);
    bool iconLoaded = provider.TryLoadUnityAsset<UnityEngine.Sprite>(image, out icon);
    rows.Add(new { name, type, modelLoaded, modelChars = text?.Length ?? 0,
        iconLoaded, vertices = icon == null ? 0 : icon.vertices.Length });
}
return rows;

// Run with: unity command eval_file --file Tools/VerifyDE128ShopAssets.cs --json
// Read-only native import check; does not instantiate fighters or touch saves.
var document = new System.Xml.XmlDocument();
document.Load("Assets/vanillaXml/list.xml");
var provider = new Eclipse.Modding.CoreAssetProvider();
var rows = new System.Collections.Generic.List<object>();
foreach(System.Xml.XmlElement item in document.SelectNodes("/List/Items/Item")) {
 string name=item.GetAttribute("Name");
 if(!System.Text.RegularExpressions.Regex.IsMatch(name,"_BP_S[1-5]_"))continue;
 var model=Eclipse.Modding.AssetId.Parse("core:gamedata/models/"+item.GetAttribute("Model"));
 var icon=Eclipse.Modding.AssetId.Parse("core:ui/items/"+item.GetAttribute("Image"));
 string modelText;
 UnityEngine.Sprite sprite;
 bool loadedModel=provider.TryLoadModelText(model,out modelText);
 bool loadedIcon=provider.TryLoadUnityAsset<UnityEngine.Sprite>(icon,out sprite);
 if(!loadedModel || string.IsNullOrEmpty(modelText) || !loadedIcon || sprite.vertices.Length < 3)
     throw new Exception("Missing model text or imported sprite: " + name);
 rows.Add(new {name,model=loadedModel,modelChars=modelText==null?0:modelText.Length,icon=loadedIcon,vertices=sprite==null?0:sprite.vertices.Length});
}
if(rows.Count != 25) throw new Exception("Expected 25 archived battle-pass items; got " + rows.Count);
return rows;

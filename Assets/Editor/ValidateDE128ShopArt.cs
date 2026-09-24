using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Eclipse.Content;
using UnityEditor;
using UnityEngine;

// Editor-only archive acceptance: DE128 itself ships generated Lua, never XML.
public static class ValidateDE128ShopArt
{
    [MenuItem("SF2/Validate DE128 Shop Art")]
    public static void RunBatch()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        var vanilla = Read(Path.Combine(root, "Assets/vanillaXml/list.xml"));
        var archive = Read(Path.Combine(root, "Assets/DExml/list.xml"));
        var originals = new Dictionary<string, XmlElement>(StringComparer.Ordinal);
        foreach (XmlElement item in vanilla.SelectNodes("/List/Items/Item"))
        {
            string name = item.GetAttribute("Name");
            if (!originals.ContainsKey(name)) originals.Add(name, item);
        }
        var seen = new HashSet<string>(StringComparer.Ordinal);
        int icons = 0, models = 0;
        foreach (XmlElement item in archive.SelectNodes("/List/Items/Item"))
        {
            string name = item.GetAttribute("Name");
            if (!seen.Add(name) || !originals.TryGetValue(name, out var source) ||
                source.GetAttribute("ShopHide") != "1" || item.GetAttribute("ShopHide") == "1") continue;
            if (source.GetAttribute("Image") != item.GetAttribute("Image"))
            {
                string address = "UI/Items/" + item.GetAttribute("Image");
                if (!PackagedArtCatalog.ContainsSprite(address) || PackagedArtCatalog.Load<Sprite>(address) == null)
                    throw new InvalidDataException("DE128 shop sprite unavailable: " + name + " / " + address);
                icons++;
            }
            if (source.GetAttribute("Model") != item.GetAttribute("Model"))
            {
                string address = "gamedata/models/" + item.GetAttribute("Model");
                string text = PackagedArtCatalog.LoadModelText(address);
                if (string.IsNullOrEmpty(text))
                    throw new InvalidDataException("DE128 shop model unavailable: " + name + " / " + address);
                var document = new XmlDocument { XmlResolver = null };
                document.LoadXml(text);
                if (document["Scene"]?["Figures"] == null)
                    throw new InvalidDataException("DE128 shop model has no Scene/Figures: " + name + " / " + address);
                models++;
            }
        }
        if (icons != 20 || models != 3)
            throw new InvalidDataException("DE128 shop art cohort changed: " + icons + " icons, " + models + " models.");
        Debug.Log("PASS DE128 shop art: " + icons + " packaged sprites and " + models + " packaged models.");
    }

    private static XmlDocument Read(string path)
    {
        var document = new XmlDocument { XmlResolver = null };
        using (var reader = XmlReader.Create(path, new XmlReaderSettings {
            DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null })) document.Load(reader);
        return document;
    }
}

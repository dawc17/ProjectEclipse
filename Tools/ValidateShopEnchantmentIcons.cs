// Isolated native Unity fixture using the real resolver and imported shop PNGs.
// Simulate research bundles offering a competing framed card for every lookup.
using System;
using System.IO;
using System.Xml;
using Nekki.SF2.GUI;
using UnityEditor;
using UnityEngine;

public static class ResourcesAndBundles
{
    public static Sprite Card;
    public static T Load<T>(string path) where T : UnityEngine.Object { return Card as T; }
}
public static class AtlasCache
{
    public static Sprite GetSpriteFromAtlas(string path, string name) { return ResourcesAndBundles.Card; }
}

public static class ValidateShopEnchantmentIcons
{
    private static int checks;
    private static void Require(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
        checks++;
    }

    public static void Run()
    {
        try
        {
            string[] args = Environment.GetCommandLineArgs();
            string root = args[Array.IndexOf(args, "-sourceProject") + 1];
            var texture = new Texture2D(2, 2);
            ResourcesAndBundles.Card = Sprite.Create(texture, new Rect(0, 0, 2, 2), Vector2.one * 0.5f);
            Sprite[] glyphs = Resources.LoadAll<Sprite>("ui/Enchantments");
            Require(glyphs.Length >= 38, "Shop icon fixtures missing");
            foreach (Sprite glyph in glyphs)
            {
                Require(glyph.vertices.Length >= 3 && glyph.rect.width > 0, "Invalid native sprite: " + glyph.name);
                foreach (string prefix in new[] { "Enchantments.", "Enchantments.SkillsEnch02.", "Enchantments.Enchantments." })
                {
                    Require(ResolutionImage.GetSprite(null, prefix + glyph.name) == glyph, "Card replaced glyph: " + prefix + glyph.name);
                    Require(ResolutionImage.GetSprite("UI/Atlases/", prefix + glyph.name) == glyph, "Atlas path replaced glyph: " + glyph.name);
                }
                Require(ResolutionImage.GetSprite(null, "UI/Atlases/Enchantments." + glyph.name) == glyph, "Full path replaced glyph: " + glyph.name);
            }
            var perks = new XmlDocument();
            perks.Load(Path.Combine(root, "Assets/vanillaXml/perks.xml"));
            int xmlIcons = 0;
            foreach (XmlElement perk in perks.SelectNodes("/Perks/Perk[@Image]"))
            {
                string name = perk.GetAttribute("Image");
                string member = name.Substring(name.LastIndexOf('.') + 1);
                Sprite glyph = Resources.Load<Sprite>("UI/Enchantments/" + member);
                if (glyph == null) continue;
                Require(ResolutionImage.GetSprite(null, "Enchantments." + name) == glyph, "XML icon resolved to card: " + name);
                xmlIcons++;
            }
            Require(xmlIcons > 20, "Canonical XML coverage missing");
            Require(ResolutionImage.GetSprite("UI/Skills/", "EnchantmentBleeding") == ResourcesAndBundles.Card, "Non-shop cards changed");
            Require(ResolutionImage.GetSprite(null, "SkillsEnch01.EnchantmentBleeding_Red") == ResourcesAndBundles.Card, "Combat icon lookup changed");
            Require(ResolutionImage.GetSprite(null, null) == null, "Empty lookup changed");
            Debug.Log("[ShopEnchantmentIcons] PASS: " + checks + " checks; " + glyphs.Length + " native glyphs; " + xmlIcons + " XML perk references.");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogError("[ShopEnchantmentIcons] " + exception);
            EditorApplication.Exit(1);
        }
    }
}

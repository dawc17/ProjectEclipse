// Isolated Unity fixture: copies the actual ResolutionImage/layout sources and
// reads atlas descriptors as text; never imports malformed recovered Sprite YAML.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Eclipse.UI;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Shop;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class ResourcesAndBundles
{
    public static readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        Sprite sprite;
        return Sprites.TryGetValue(path, out sprite) ? sprite as T : null;
    }
}
public static class AtlasCache { public static Sprite GetSpriteFromAtlas(string path, string name) { return null; } }
namespace Nekki.SF2.GUI.Shop { public class SectionButton : Button { } }

public static class ValidateProfileBottomBar
{
    private static int checks;
    private static void Require(bool value, string message)
    {
        if (!value) throw new InvalidOperationException(message);
        checks++;
    }
    private static float Number(string text, string name)
    {
        return float.Parse(Regex.Match(text, @"\b" + name + @": ([-0-9.eE+]+)").Groups[1].Value, CultureInfo.InvariantCulture);
    }
    public static void Run()
    {
        try
        {
            string[] args = Environment.GetCommandLineArgs();
            string root = args[Array.IndexOf(args, "-sourceProject") + 1];
            string atlas = Path.Combine(root, "Assets/Resources/ui/atlases");
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.LoadImage(File.ReadAllBytes(Path.Combine(atlas, "ProfileButtons.png")));
            foreach (string file in Directory.GetFiles(atlas, "ProfileButtons.*.asset"))
            {
                string data = File.ReadAllText(file);
                string crop = Regex.Match(data, @"(?s)m_Rect:(.*?)m_Offset:").Groups[1].Value;
                string pivotText = Regex.Match(data, @"m_Pivot: \{([^}]+)").Groups[1].Value;
                var sprite = Sprite.Create(texture, new Rect(Number(crop, "x"), Number(crop, "y"),
                    Number(crop, "width"), Number(crop, "height")),
                    new Vector2(Number(pivotText, "x"), Number(pivotText, "y")), 100, 0, SpriteMeshType.FullRect);
                sprite.name = Path.GetFileNameWithoutExtension(file);
                ResourcesAndBundles.Sprites.Add("UI/Atlases/" + sprite.name, sprite);
            }
            MethodInfo populate = typeof(ResolutionImage).GetMethod("OnPopulateMesh", BindingFlags.Instance | BindingFlags.NonPublic,
                null, new[] { typeof(VertexHelper) }, null);
            string[] icons = { "Progress", "Strikes", "Achiev", "Seal" };
            Vector2[] oldSizes = { new Vector2(194.66223f,185.2431f), new Vector2(189.04037f,179.89325f),
                new Vector2(110.42621f,172.70837f), new Vector2(166.94812f,167.99496f) };
            for (int i = 0; i < icons.Length; i++)
            {
                var go = new GameObject(icons[i], typeof(RectTransform), typeof(ResolutionImage), typeof(SectionButton));
                var image = go.GetComponent<ResolutionImage>();
                var button = go.GetComponent<SectionButton>();
                button.targetGraphic = image;
                var rect = (RectTransform)go.transform;
                rect.sizeDelta = oldSizes[i];
                rect.anchoredPosition = new Vector2(-410 + i * 275, 0);
                var position = rect.anchoredPosition;
                var badgeObject = new GameObject("badge", typeof(RectTransform));
                var badge = (RectTransform)badgeObject.transform;
                badge.SetParent(rect, false);
                badge.anchoredPosition = new Vector2(70, -50);
                image.set_TexturePath("UI/Atlases/");
                image.set_SpriteName("ProfileButtons.Inactive_" + icons[i]);
                ProfileBottomBarLayout.Configure(button);
                foreach (string state in new[] { "Inactive", "Active", "Pushed" })
                {
                    image.overrideSprite = ResourcesAndBundles.Sprites["UI/Atlases/ProfileButtons." + state + "_" + icons[i]];
                    using (var mesh = new VertexHelper())
                    {
                        populate.Invoke(image, new object[] { mesh });
                        Require(mesh.currentVertCount == 4, "Expected native quad");
                        Vector2 min = Vector2.positiveInfinity, max = Vector2.negativeInfinity;
                        UIVertex vertex = default(UIVertex);
                        for (int j = 0; j < mesh.currentVertCount; j++)
                        {
                            mesh.PopulateUIVertex(ref vertex,j);
                            min = Vector2.Min(min, vertex.position); max = Vector2.Max(max, vertex.position);
                        }
                        Require(Vector2.Distance(max-min, image.overrideSprite.rect.size) < 0.01f, "Artwork is stretched: " + image.overrideSprite.name);
                        Require(((min+max)/2-rect.rect.center).sqrMagnitude < 0.001f, "Artwork off center");
                        Require(rect.sizeDelta == new Vector2(248,236), "Hit area changed on state swap");
                        Require(rect.anchoredPosition == position && badge.anchoredPosition == new Vector2(70,-50), "Tab/badge moved");
                    }
                }
                UnityEngine.Object.DestroyImmediate(go);
            }
            ProfileBottomBarLayout.Configure(null, null);
            Debug.Log("[ProfileBottomBar] PASS " + checks + " native assertions: all 12 atlas states, proportions, centering, hit areas and badge positions.");
            EditorApplication.Exit(0);
        }
        catch (Exception error) { Debug.LogException(error); EditorApplication.Exit(1); }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class DevSpriteCrashValidator
{
    [Serializable] private class Entry { public string assetPath, name; public float x, y, width, height; }
    [Serializable] private class Manifest { public Entry[] entries; }

    [MenuItem("SF2/Validate Rebuilt Raid Sprites")]
    public static void Run()
    {
        try
        {
            Manifest manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText("Temp/raid-sprite-rebuild.json"));
            MethodInfo getUvs = typeof(Editor).Assembly.GetType("UnityEditor.Sprites.SpriteUtility")
                .GetMethod("GetSpriteUVs", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (Entry entry in manifest.entries)
            {
                string path = "Assets/" + entry.assetPath.Replace('\\', '/').Split(new[] { "/Assets/" }, StringSplitOptions.None)[1];
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (sprite == null || sprite.texture == null || sprite.name != entry.name)
                    throw new InvalidDataException("Invalid sprite or texture/name: " + path);
                foreach (bool atlas in sprite.packed ? new[] { false, true } : new[] { false })
                {
                    // This is the same native call that crashed SpriteInspector.
                    Vector2[] uv = (Vector2[])getUvs.Invoke(null, new object[] { sprite, atlas });
                    if (uv.Length != 4 || uv.Any(v => float.IsNaN(v.x) || float.IsNaN(v.y)) ||
                        Math.Abs(uv.Min(v => v.x) - entry.x / sprite.texture.width) > 0.00001f ||
                        Math.Abs(uv.Max(v => v.x) - (entry.x + entry.width) / sprite.texture.width) > 0.00001f ||
                        Math.Abs(uv.Min(v => v.y) - entry.y / sprite.texture.height) > 0.00001f ||
                        Math.Abs(uv.Max(v => v.y) - (entry.y + entry.height) / sprite.texture.height) > 0.00001f)
                        throw new InvalidDataException("Wrong native UV region (atlas=" + atlas + "): " + path);
                }
                if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
                {
                    Editor inspector = Editor.CreateEditor(sprite);
                    Texture2D preview = inspector.RenderStaticPreview(path, new UnityEngine.Object[0], 64, 64);
                    if (preview == null)
                        throw new InvalidDataException("SpriteInspector couldn't render a thumbnail: " + path);
                    UnityEngine.Object.DestroyImmediate(preview);
                    UnityEngine.Object.DestroyImmediate(inspector);
                }
            }
            Debug.Log("[SpriteCrashValidation] PASS: " + manifest.entries.Length +
                " installed sprites, original names/textures, native UVs and exact atlas regions. Thumbnail rendering: " +
                (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null));
            if (Application.isBatchMode) EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            if (Application.isBatchMode) EditorApplication.Exit(1);
            else throw;
        }
    }
}

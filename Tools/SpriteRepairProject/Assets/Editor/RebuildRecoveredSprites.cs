using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// Isolated Unity 2019 project. Never imports the malformed source .assets;
// reads only their JSON descriptors and their original PNG pixel data.
public static class RebuildRecoveredSprites
{
    [Serializable] public class Entry
    {
        public string assetPath, texturePath, name, outputPath, generatedTextureGuid;
        public float x, y, width, height, pixelsPerUnit;
    }
    [Serializable] public class Manifest { public Entry[] entries; }

    public static void Run()
    {
        try
        {
            string[] args = Environment.GetCommandLineArgs();
            string manifestPath = args[Array.IndexOf(args, "-repairManifest") + 1];
            Manifest manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(manifestPath));
            EditorSettings.serializationMode = SerializationMode.ForceText;
            Directory.CreateDirectory("Assets/Generated");
            AssetDatabase.Refresh();
            var textures = new Dictionary<string, Texture2D>();
            MethodInfo nativeUvs = typeof(Editor).Assembly.GetType("UnityEditor.Sprites.SpriteUtility")
                .GetMethod("GetSpriteUVs", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            int index = 0;
            foreach (Entry entry in manifest.entries)
            {
                Texture2D texture;
                if (!textures.TryGetValue(entry.texturePath, out texture))
                {
                    texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (!ImageConversion.LoadImage(texture, File.ReadAllBytes(entry.texturePath), false))
                        throw new InvalidDataException("Cannot decode " + entry.texturePath);
                    texture.name = Path.GetFileNameWithoutExtension(entry.texturePath);
                    texture.filterMode = FilterMode.Bilinear;
                    texture.wrapMode = TextureWrapMode.Clamp;
                    AssetDatabase.CreateAsset(texture, "Assets/Generated/texture_" + textures.Count + ".asset");
                    textures.Add(entry.texturePath, texture);
                }
                Rect rect = new Rect(entry.x, entry.y, entry.width, entry.height);
                if (rect.xMin < 0 || rect.yMin < 0 || rect.xMax > texture.width || rect.yMax > texture.height)
                    throw new InvalidDataException("Sprite rectangle is outside texture: " + entry.assetPath);
                Sprite sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), entry.pixelsPerUnit, 0, SpriteMeshType.FullRect);
                sprite.name = entry.name;
                string output = "Assets/Generated/sprite_" + index++ + ".asset";
                AssetDatabase.CreateAsset(sprite, output);
                sprite.name = entry.name;
                EditorUtility.SetDirty(sprite);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(output, ImportAssetOptions.ForceUpdate);
                Sprite reloaded = AssetDatabase.LoadAssetAtPath<Sprite>(output);
                Vector2[] uv = (Vector2[])nativeUvs.Invoke(null, new object[] { reloaded, false });
                if (uv.Length != 4 || reloaded.vertices.Length != 4 || reloaded.triangles.Length != 6 ||
                    uv.Any(v => float.IsNaN(v.x) || float.IsNaN(v.y)))
                    throw new InvalidDataException("Native sprite validation failed: " + entry.assetPath);
                if (Math.Abs(uv.Min(v => v.x) - rect.xMin / texture.width) > 0.00001f ||
                    Math.Abs(uv.Max(v => v.x) - rect.xMax / texture.width) > 0.00001f ||
                    Math.Abs(uv.Min(v => v.y) - rect.yMin / texture.height) > 0.00001f ||
                    Math.Abs(uv.Max(v => v.y) - rect.yMax / texture.height) > 0.00001f)
                    throw new InvalidDataException("Native UVs don't cover the intended atlas region: " + entry.assetPath);
                entry.outputPath = Path.GetFullPath(output);
                entry.generatedTextureGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(texture));
                if (Array.IndexOf(args, "-renderPreviews") >= 0)
                {
                    Editor inspector = Editor.CreateEditor(reloaded);
                    Texture2D preview = inspector.RenderStaticPreview(output, new UnityEngine.Object[0], 128, 128);
                    if (preview == null)
                        throw new InvalidDataException("Native preview failed: " + entry.assetPath);
                    File.WriteAllBytes(manifestPath + "." + entry.name + ".png", preview.EncodeToPNG());
                    UnityEngine.Object.DestroyImmediate(preview);
                    UnityEngine.Object.DestroyImmediate(inspector);
                }
            }
            File.WriteAllText(manifestPath + ".rebuilt.json", JsonUtility.ToJson(manifest, true));
            Debug.Log("[SpriteRebuild] PASS " + index + " sprites serialized, reimported and native UVs checked.");
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            EditorApplication.Exit(1);
        }
    }
}

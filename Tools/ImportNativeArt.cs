// Runs only in an isolated Unity 2022.3 project. Sprite data is written through
// Unity's native APIs and verified again after serialization/reimport.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Eclipse.Content;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;

[Serializable] public sealed class NativeArtInput { public NativeArtGroup[] bundles; }
[Serializable] public sealed class NativeArtGroup { public string name, source; public NativeArtSource[] sources; public string[] addresses; }
[Serializable] public sealed class NativeArtSource
{
    public string address, kind, path, name;
    public int filter, aniso, wrapU, wrapV;
    public bool mipmaps;
}

public sealed class NativeArtSourceImporter : AssetPostprocessor
{
    private static Dictionary<string, NativeArtSource> sources;
    private void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith("Assets/Resources/SF2Content/Art/", StringComparison.Ordinal)) return;
        if (sources == null)
        {
            var input = JsonUtility.FromJson<NativeArtInput>(File.ReadAllText("native-art-input.json"));
            sources = input.bundles.SelectMany(x => x.sources).ToDictionary(x => x.path);
        }
        NativeArtSource source;
        if (!sources.TryGetValue(assetPath, out source)) return;
        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Default;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = source.mipmaps;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.maxTextureSize = 8192;
        importer.filterMode = (FilterMode)source.filter;
        importer.wrapModeU = (TextureWrapMode)source.wrapU;
        importer.wrapModeV = (TextureWrapMode)source.wrapV;
        importer.anisoLevel = source.aniso;
        importer.textureCompression = TextureImporterCompression.CompressedHQ;
    }
}

public static class ImportNativeArt
{
    private const string Root = "Assets/Resources/SF2Content/Art";
    private static int checks, spriteCount;
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
        checks++;
    }
    private static string Resource(string assetPath)
    {
        string path = assetPath.Substring("Assets/Resources/".Length);
        return path.Substring(0, path.Length - Path.GetExtension(path).Length);
    }
    private static string Mapped(Dictionary<UnityEngine.Object, string> map, UnityEngine.Object key)
    {
        string result;
        return key != null && map.TryGetValue(key, out result) ? result : null;
    }

    public static void Run()
    {
        try
        {
            var input = JsonUtility.FromJson<NativeArtInput>(File.ReadAllText("native-art-input.json"));
            var groups = new List<PackagedArtCatalog.BundleRecord>();
            foreach (NativeArtGroup group in input.bundles)
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(group.source);
                Require(bundle != null, "Cannot inspect source " + group.name);
                var map = new Dictionary<UnityEngine.Object, string>();
                var textures = new Dictionary<Texture2D, Texture2D>();
                var checksByPath = new Dictionary<string, Sprite[]>();
                var output = new List<PackagedArtCatalog.ArtRecord>();
                try
                {
                    foreach (NativeArtSource source in group.sources)
                    {
                        Type type = source.kind == "Texture2D" ? typeof(Texture2D) : source.kind == "Font" ? typeof(Font) : typeof(AudioClip);
                        UnityEngine.Object original = bundle.LoadAsset(source.address, type);
                        UnityEngine.Object imported = AssetDatabase.LoadAssetAtPath(source.path, type);
                        Require(original != null && imported != null, "Missing imported source " + source.path);
                        map[original] = Resource(source.path);
                        if (original is Texture2D)
                        {
                            var before = (Texture2D)original; var after = (Texture2D)imported;
                            Require(before.width == after.width && before.height == after.height, "Texture resized: " + source.path);
                            textures[before] = after;
                        }
                        if (original is Font) map[((Font)original).material] = Resource(source.path);
                        if (original is AudioClip)
                        {
                            var before = (AudioClip)original; var after = (AudioClip)imported;
                            Require(before.channels == after.channels && Math.Abs(before.length - after.length) < 0.1f, "Audio differs: " + source.path);
                        }
                    }
                    int number = 0;
                    AssetDatabase.StartAssetEditing();
                    try
                    {
                        foreach (string address in group.addresses)
                        {
                            var record = new PackagedArtCatalog.ArtRecord { address = address,
                                texture = Mapped(map, bundle.LoadAsset<Texture2D>(address)),
                                audio = Mapped(map, bundle.LoadAsset<AudioClip>(address)),
                                font = Mapped(map, bundle.LoadAsset<Font>(address)) };
                            Sprite[] originals = bundle.LoadAssetWithSubAssets<Sprite>(address);
                            if (originals != null && originals.Length > 0)
                            {
                                string name = Regex.Replace(Path.GetFileName(address), "[^a-zA-Z0-9._-]", "_");
                                if (name.Length > 70) name = name.Substring(0, 70);
                                string path = Root + "/" + group.name + "/Sprites/" + name + "_" + number.ToString("D4") + ".asset";
                                Directory.CreateDirectory(Path.GetDirectoryName(path));
                                var atlas = ScriptableObject.CreateInstance<NativeSpriteAtlas>();
                                AssetDatabase.CreateAsset(atlas, path);
                                var members = new List<Sprite>();
                                foreach (Sprite original in originals)
                                {
                                    Texture2D texture;
                                    Require(textures.TryGetValue(original.texture, out texture), "Unmapped sprite texture: " + group.name + "/" + original.name);
                                    Rect rect = original.rect;
                                    var pivot = new Vector2(original.pivot.x / rect.width, original.pivot.y / rect.height);
                                    Sprite sprite = Sprite.Create(texture, rect, pivot, original.pixelsPerUnit, 0, SpriteMeshType.FullRect, original.border);
                                    Require(sprite != null, "Cannot reconstruct " + original.name);
                                    sprite.name = original.name;
                                    sprite.SetVertexCount(original.vertices.Length);
                                    using (var vertices = new NativeArray<Vector3>(original.vertices.Select(x => new Vector3(x.x, x.y, 0)).ToArray(), Allocator.Temp))
                                        sprite.SetVertexAttribute(VertexAttribute.Position, vertices);
                                    using (var indices = new NativeArray<ushort>(original.triangles, Allocator.Temp))
                                        sprite.SetIndices(indices);
                                    using (var uv = new NativeArray<Vector2>(original.uv, Allocator.Temp))
                                        sprite.SetVertexAttribute(VertexAttribute.TexCoord0, uv);
                                    AssetDatabase.AddObjectToAsset(sprite, atlas);
                                    sprite.name = original.name;
                                    EditorUtility.SetDirty(sprite);
                                    members.Add(sprite);
                                    spriteCount++;
                                }
                                atlas.sprites = members.ToArray();
                                EditorUtility.SetDirty(atlas);
                                record.sprites = Resource(path);
                                checksByPath.Add(path, originals);
                            }
                            if (record.texture != null || record.audio != null || record.font != null || record.sprites != null)
                                output.Add(record);
                            number++;
                        }
                    }
                    finally { AssetDatabase.StopAssetEditing(); }
                    AssetDatabase.SaveAssets();
                    foreach (var pair in checksByPath)
                    {
                        AssetDatabase.ImportAsset(pair.Key, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
                        Sprite[] restored = AssetDatabase.LoadAllAssetsAtPath(pair.Key).OfType<Sprite>().ToArray();
                        Require(restored.Length == pair.Value.Length, "Sprite count changed: " + pair.Key);
                        foreach (Sprite original in pair.Value)
                        {
                            Sprite actual = restored.SingleOrDefault(x => x.name == original.name);
                            Require(actual != null, "Missing member '" + original.name + "' in " + pair.Key + "; imported: " + string.Join(",", restored.Select(x => x.name)));
                            Require(actual.rect == original.rect && actual.pivot == original.pivot && actual.border == original.border &&
                                actual.pixelsPerUnit == original.pixelsPerUnit, "Sprite layout changed: " + original.name);
                            float vertexError = actual.vertices.Length == original.vertices.Length ? actual.vertices.Zip(original.vertices, (a, b) => Vector2.Distance(a, b)).Max() : float.PositiveInfinity;
                            float uvError = actual.uv.Length == original.uv.Length ? actual.uv.Zip(original.uv, (a, b) => Vector2.Distance(a, b)).Max() : float.PositiveInfinity;
                            Require(vertexError < 0.00001f && actual.triangles.SequenceEqual(original.triangles), "Sprite geometry changed: " + original.name + "; error=" + vertexError + "; counts=" + actual.vertices.Length + "/" + original.vertices.Length + "; file=" + pair.Key);
                            Require(uvError < 0.000001f, "Sprite UV changed after native serialization: " + original.name + "; error=" + uvError);
                        }
                    }
                    groups.Add(new PackagedArtCatalog.BundleRecord { name = group.name, assets = output.ToArray() });
                    Debug.Log("[NativeArt] " + group.name + ": " + output.Count + " native addresses verified.");
                }
                finally { bundle.Unload(true); }
            }
            AssetDatabase.SaveAssets();
            var files = new List<PackagedArtCatalog.FileRecord>();
            foreach (string file in Directory.GetFiles(Root, "*", SearchOption.AllDirectories).OrderBy(x => x, StringComparer.Ordinal))
            {
                if (file.EndsWith(".meta") || Path.GetFileName(file) == "catalog.json") continue;
                string hash;
                using (var stream = File.OpenRead(file))
                using (var sha = SHA256.Create()) hash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                files.Add(new PackagedArtCatalog.FileRecord { path = file.Substring(Root.Length + 1).Replace('\\', '/'), size = new FileInfo(file).Length, sha256 = hash });
            }
            File.WriteAllText(Root + "/catalog.json", JsonUtility.ToJson(new PackagedArtCatalog.Catalog { version = 2, bundles = groups.ToArray(), files = files.ToArray() }, true));
            AssetDatabase.ImportAsset(Root + "/catalog.json", ImportAssetOptions.ForceSynchronousImport);
            Debug.Log("[NativeArt] PASS: " + spriteCount + " sprites; " + checks + " native round-trip checks; " + files.Count + " files.");
            EditorApplication.Exit(0);
        }
        catch (Exception exception) { Debug.LogError("[NativeArt] " + exception); EditorApplication.Exit(1); }
    }
}

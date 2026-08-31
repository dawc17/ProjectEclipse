using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

// Runs only inside the disposable migration fixture created by
// MigrateCoreLocationsToTar.py. Unity is used here deliberately so the
// exported AssetRipper Sprite objects are the authority for geometry/UVs.
public static class CoreLocationExporter
{
    private sealed class Descriptor
    {
        public string Text;
        public int Priority;
    }

    private static readonly Dictionary<string, string> TextureEntries =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, Descriptor> Descriptors =
        new Dictionary<string, Descriptor>(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<string> Addresses =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    private static string OutputRoot;
    private static float MaxUvError;
    private static int StandaloneSprites;
    private static int ImportedSprites;
    private static int EmptySpriteTextures;

    public static void Run()
    {
        try
        {
            OutputRoot = Environment.GetEnvironmentVariable("SF2DE_CORE_LOCATION_OUTPUT");
            if (string.IsNullOrWhiteSpace(OutputRoot))
                throw new InvalidOperationException("SF2DE_CORE_LOCATION_OUTPUT is not set.");

            OutputRoot = Path.GetFullPath(OutputRoot);
            Directory.CreateDirectory(OutputRoot);
            Directory.CreateDirectory(Path.Combine(OutputRoot, "textures"));
            Directory.CreateDirectory(Path.Combine(OutputRoot, "assets"));

            string[] roots =
            {
                "Assets/Resources/Textures/Locations",
                "Assets/Resources/Textures/Location_effects/atlases",
            };

            List<string> pngs = roots
                .Where(AssetDatabase.IsValidFolder)
                .SelectMany(root => Directory.GetFiles(root, "*.png", SearchOption.AllDirectories))
                .Select(NormalizeAssetPath)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToList();
            if (pngs.Count == 0)
                throw new InvalidDataException("Core-location migration fixture contains no PNG files.");

            for (int i = 0; i < pngs.Count; i++)
            {
                string source = pngs[i];
                string entry = "textures/" + i.ToString("D4", CultureInfo.InvariantCulture) + "_" +
                    SafeName(Path.GetFileName(source));
                string target = Path.Combine(OutputRoot, entry.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(Path.GetFullPath(source), target, true);
                TextureEntries.Add(source, entry);
            }

            // Prefer the standalone Sprite assets exported by AssetRipper. They preserve the
            // original tight meshes and are what the recovered location fallback used directly.
            List<string> spriteAssets = roots
                .Where(AssetDatabase.IsValidFolder)
                .SelectMany(root => Directory.GetFiles(root, "*.asset", SearchOption.AllDirectories))
                .Select(NormalizeAssetPath)
                .OrderBy(path => path, StringComparer.Ordinal)
                .ToList();
            foreach (string assetPath in spriteAssets)
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (sprite == null)
                    throw new InvalidDataException("Expected exported Sprite asset: " + assetPath);
                ExportSprite(sprite, ToResourceAddress(AssetDatabase.GetAssetPath(sprite.texture)),
                    ToResourceAddress(assetPath), 2);
                StandaloneSprites++;
            }

            // Fill textures which are represented only as TextureImporter sprite sub-assets.
            // Existing standalone (address,name) pairs win over importer-generated geometry.
            foreach (string pngPath in pngs)
            {
                string atlasAddress = ToResourceAddress(pngPath);
                Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(pngPath)
                    .OfType<Sprite>()
                    .OrderBy(sprite => sprite.name, StringComparer.Ordinal)
                    .ToArray();
                TextureImporter importer = AssetImporter.GetAtPath(pngPath) as TextureImporter;
                if (importer == null || importer.textureType != TextureImporterType.Sprite)
                    throw new InvalidDataException("Location PNG is not a Sprite texture: " + pngPath);
                if (sprites.Length == 0)
                {
                    // A handful of recovered *_low textures are marked Multiple but have an
                    // explicitly empty spriteSheet. Resources.Load/LoadAll could not return a
                    // Sprite for them in the former loose setup either. Preserve the PNG payload
                    // in TAR, but do not manufacture a logical Sprite that never existed.
                    EmptySpriteTextures++;
                    continue;
                }

                foreach (Sprite sprite in sprites)
                {
                    // Resources.LoadAll(atlasAddress) is the canonical route for importer
                    // sub-assets. Do not invent parent/member aliases here: two different
                    // atlases in one folder may legitimately reuse a frame name. Standalone
                    // exported .asset Sprites above provide the historical direct aliases.
                    ExportSprite(sprite, atlasAddress, atlasAddress, 1);
                    ImportedSprites++;
                }
            }

            int descriptorIndex = 0;
            foreach (KeyValuePair<string, Descriptor> pair in Descriptors.OrderBy(x => x.Key, StringComparer.Ordinal))
            {
                string path = Path.Combine(OutputRoot, "assets",
                    descriptorIndex.ToString("D6", CultureInfo.InvariantCulture) + ".meta");
                File.WriteAllText(path, pair.Value.Text, new UTF8Encoding(false));
                descriptorIndex++;
            }
            File.WriteAllLines(Path.Combine(OutputRoot, "addresses.txt"),
                Addresses.OrderBy(x => x, StringComparer.Ordinal), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(OutputRoot, "stats.txt"),
                "textures=" + TextureEntries.Count + "\n" +
                "standaloneSprites=" + StandaloneSprites + "\n" +
                "importedSprites=" + ImportedSprites + "\n" +
                "emptySpriteTextures=" + EmptySpriteTextures + "\n" +
                "descriptors=" + Descriptors.Count + "\n" +
                "addresses=" + Addresses.Count + "\n" +
                "maxUvError=" + F(MaxUvError) + "\n",
                new UTF8Encoding(false));

            Debug.Log("[CoreLocationExport] PASS: " + TextureEntries.Count + " textures, " +
                Descriptors.Count + " descriptors, " + Addresses.Count + " addresses, maxUvError=" +
                MaxUvError.ToString("R", CultureInfo.InvariantCulture));
            EditorApplication.Exit(0);
        }
        catch (Exception exception)
        {
            Debug.LogError("[CoreLocationExport] " + exception);
            EditorApplication.Exit(1);
        }
    }

    private static void ExportSprite(Sprite sprite, string atlasAddress, string directAddress, int priority)
    {
        if (sprite == null || sprite.texture == null)
            throw new InvalidDataException("Location Sprite has no texture.");
        string texturePath = NormalizeAssetPath(AssetDatabase.GetAssetPath(sprite.texture));
        string textureEntry;
        if (!TextureEntries.TryGetValue(texturePath, out textureEntry))
            throw new InvalidDataException("Sprite texture is outside the migrated location roots: " + texturePath);

        TextureImporter importer = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        if (importer == null)
            throw new InvalidDataException("Missing TextureImporter: " + texturePath);

        Rect rect = sprite.rect;
        if (rect.width <= 0f || rect.height <= 0f || sprite.pixelsPerUnit <= 0f)
            throw new InvalidDataException("Invalid Sprite dimensions: " + directAddress + "/" + sprite.name);
        Vector2 pivot = new Vector2(sprite.pivot.x / rect.width, sprite.pivot.y / rect.height);
        Vector2[] vertices = sprite.vertices;
        Vector2[] uv = sprite.uv;
        ushort[] triangles = sprite.triangles;
        if (vertices.Length < 3 || uv.Length != vertices.Length || triangles.Length < 3 || triangles.Length % 3 != 0)
            throw new InvalidDataException("Invalid Sprite mesh: " + directAddress + "/" + sprite.name);

        // Some recovered Sprite rects are a fraction of a pixel smaller than their own tight
        // mesh because the old importer serialized a rounded float rect. Imported Unity Sprites
        // tolerate that, but Sprite.OverrideGeometry rejects any point outside the new rect.
        // Expand only as far as the actual geometry requires and move the normalized pivot so
        // its absolute texture-space position remains identical. This preserves local vertices
        // and exact UVs while making the runtime-created Sprite legal.
        CanonicalizeRectForGeometry(sprite, vertices, ref rect, ref pivot);

        float uvError = 0f;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 expected = new Vector2(
                (rect.x + pivot.x * rect.width + vertices[i].x * sprite.pixelsPerUnit) / sprite.texture.width,
                (rect.y + pivot.y * rect.height + vertices[i].y * sprite.pixelsPerUnit) / sprite.texture.height);
            uvError = Mathf.Max(uvError, Vector2.Distance(expected, uv[i]));
        }
        MaxUvError = Mathf.Max(MaxUvError, uvError);
        if (uvError > 0.00001f)
            throw new InvalidDataException("Sprite UVs cannot be reproduced by TAR OverrideGeometry: " +
                directAddress + "/" + sprite.name + "; error=" + uvError.ToString("R", CultureInfo.InvariantCulture));

        string text = Serialize(sprite, rect, pivot, vertices, triangles, uv, importer, textureEntry, atlasAddress);
        AddDescriptor(atlasAddress, sprite.name, text, priority);
        if (!string.Equals(directAddress, atlasAddress, StringComparison.OrdinalIgnoreCase))
        {
            string directText = Serialize(sprite, rect, pivot, vertices, triangles, uv, importer, textureEntry, directAddress);
            AddDescriptor(directAddress, sprite.name, directText, priority);
        }
    }

    private static void CanonicalizeRectForGeometry(Sprite sprite, Vector2[] vertices,
        ref Rect rect, ref Vector2 pivot)
    {
        float ppu = sprite.pixelsPerUnit;
        float pivotAbsoluteX = rect.x + pivot.x * rect.width;
        float pivotAbsoluteY = rect.y + pivot.y * rect.height;
        float geometryMinX = float.PositiveInfinity;
        float geometryMaxX = float.NegativeInfinity;
        float geometryMinY = float.PositiveInfinity;
        float geometryMaxY = float.NegativeInfinity;
        foreach (Vector2 vertex in vertices)
        {
            float x = pivotAbsoluteX + vertex.x * ppu;
            float y = pivotAbsoluteY + vertex.y * ppu;
            geometryMinX = Mathf.Min(geometryMinX, x);
            geometryMaxX = Mathf.Max(geometryMaxX, x);
            geometryMinY = Mathf.Min(geometryMinY, y);
            geometryMaxY = Mathf.Max(geometryMaxY, y);
        }

        float left = Mathf.Min(rect.xMin, geometryMinX);
        float right = Mathf.Max(rect.xMax, geometryMaxX);
        float bottom = Mathf.Min(rect.yMin, geometryMinY);
        float top = Mathf.Max(rect.yMax, geometryMaxY);
        if (left == rect.xMin && right == rect.xMax && bottom == rect.yMin && top == rect.yMax)
            return;

        const float maximumRecoveryExpansion = 0.5f;
        float expansion = Mathf.Max(
            Mathf.Max(rect.xMin - left, right - rect.xMax),
            Mathf.Max(rect.yMin - bottom, top - rect.yMax));
        if (expansion > maximumRecoveryExpansion)
            throw new InvalidDataException("Recovered Sprite mesh exceeds its rect by " +
                expansion.ToString("R", CultureInfo.InvariantCulture) + " px: " + sprite.name);
        Vector4 border = sprite.border;
        if (border.sqrMagnitude != 0f)
            throw new InvalidDataException("Cannot expand a bordered recovered Sprite rect: " + sprite.name);

        rect = Rect.MinMaxRect(left, bottom, right, top);
        pivot = new Vector2(
            (pivotAbsoluteX - rect.x) / rect.width,
            (pivotAbsoluteY - rect.y) / rect.height);
    }

    private static void AddDescriptor(string address, string name, string text, int priority)
    {
        string key = address + "\n" + name;
        Descriptor existing;
        if (Descriptors.TryGetValue(key, out existing))
        {
            if (priority < existing.Priority)
                return;
            if (priority == existing.Priority)
            {
                if (!string.Equals(existing.Text, text, StringComparison.Ordinal))
                    throw new InvalidDataException("Conflicting duplicate location Sprite: " + address + "/" + name);
                return;
            }
        }
        Descriptors[key] = new Descriptor { Text = text, Priority = priority };
        Addresses.Add(address);
    }

    private static string Serialize(Sprite sprite, Rect rect, Vector2 pivot, Vector2[] vertices,
        ushort[] triangles, Vector2[] uv, TextureImporter importer, string textureEntry, string address)
    {
        Vector4 border = sprite.border;
        return "type=sprite\n" +
            "namespace=core\n" +
            "address=" + Escape(address) + "\n" +
            "name=" + Escape(sprite.name) + "\n" +
            "texture=" + Escape(textureEntry) + "\n" +
            "rect=" + F(rect.x) + "," + F(rect.y) + "," + F(rect.width) + "," + F(rect.height) + "\n" +
            "pivot=" + F(pivot.x) + "," + F(pivot.y) + "\n" +
            "border=" + F(border.x) + "," + F(border.y) + "," + F(border.z) + "," + F(border.w) + "\n" +
            "pixels_per_unit=" + F(sprite.pixelsPerUnit) + "\n" +
            "filter=" + (int)importer.filterMode + "\n" +
            "aniso=" + importer.anisoLevel + "\n" +
            "wrap_u=" + (int)importer.wrapModeU + "\n" +
            "wrap_v=" + (int)importer.wrapModeV + "\n" +
            "mipmaps=" + (importer.mipmapEnabled ? "true" : "false") + "\n" +
            "vertices=" + string.Join(";", vertices.Select(v => F(v.x) + "," + F(v.y)).ToArray()) + "\n" +
            "triangles=" + string.Join(",", triangles.Select(v => v.ToString(CultureInfo.InvariantCulture)).ToArray()) + "\n" +
            "uv=" + string.Join(";", uv.Select(v => F(v.x) + "," + F(v.y)).ToArray()) + "\n";
    }

    private static string NormalizeAssetPath(string path)
    {
        return path.Replace('\\', '/');
    }

    private static string ToResourceAddress(string assetPath)
    {
        string path = NormalizeAssetPath(assetPath);
        const string prefix = "Assets/Resources/";
        if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Asset is outside Resources: " + assetPath);
        path = path.Substring(prefix.Length);
        string extension = Path.GetExtension(path);
        if (!string.IsNullOrEmpty(extension))
            path = path.Substring(0, path.Length - extension.Length);
        return path;
    }

    private static string SafeName(string value)
    {
        char[] invalid = Path.GetInvalidFileNameChars();
        var output = new StringBuilder(value.Length);
        foreach (char c in value)
            output.Append(invalid.Contains(c) || c == ':' ? '_' : c);
        return output.ToString();
    }

    private static string Escape(string value)
    {
        return (value ?? string.Empty).Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n");
    }

    private static string F(float value)
    {
        return value.ToString("R", CultureInfo.InvariantCulture);
    }
}

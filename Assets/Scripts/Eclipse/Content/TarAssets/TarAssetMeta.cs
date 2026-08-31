using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Eclipse.Content.TarAssets
{
    public sealed class TarAssetMeta
    {
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        public string MetaPath;
        public string Type;
        public string Namespace;
        public string Address;
        public string Name;
        public string File;
        public string Texture;
        public Rect Rect;
        public Vector2 Pivot;
        public Vector4 Border;
        public float PixelsPerUnit;
        public FilterMode Filter;
        public int Aniso;
        public TextureWrapMode WrapU;
        public TextureWrapMode WrapV;
        public bool Mipmaps;
        public Vector2[] Vertices;
        public ushort[] Triangles;
        public Vector2[] Uv;

        public static TarAssetMeta Parse(string metaPath, string text)
        {
            Dictionary<string, string> values = ParseValues(text);
            string type = Required(values, "type").Trim().ToLowerInvariant();
            if (type == "sound" || type == "music") type = "audio";
            if (type != "sprite" && type != "audio" && type != "model" && type != "atlas")
                throw new InvalidDataException("Unsupported TAR asset type '" + type + "' in " + metaPath);

            var result = new TarAssetMeta
            {
                MetaPath = TarArchive.NormalizeEntryPath(metaPath),
                Type = type,
                Namespace = Optional(values, "namespace", "core").Trim(),
                Address = NormalizeAddress(Required(values, "address")),
                Name = Optional(values, "name", string.Empty),
                File = Optional(values, "file", string.Empty),
                Texture = Optional(values, "texture", string.Empty),
                PixelsPerUnit = ParseFloat(Optional(values, "pixels_per_unit", "100"), "pixels_per_unit", metaPath),
                Filter = (FilterMode)ParseInt(Optional(values, "filter", ((int)FilterMode.Bilinear).ToString(Invariant)), "filter", metaPath),
                Aniso = ParseInt(Optional(values, "aniso", "1"), "aniso", metaPath),
                WrapU = (TextureWrapMode)ParseInt(Optional(values, "wrap_u", ((int)TextureWrapMode.Clamp).ToString(Invariant)), "wrap_u", metaPath),
                WrapV = (TextureWrapMode)ParseInt(Optional(values, "wrap_v", ((int)TextureWrapMode.Clamp).ToString(Invariant)), "wrap_v", metaPath),
                Mipmaps = ParseBool(Optional(values, "mipmaps", "false"), "mipmaps", metaPath),
            };

            if (string.IsNullOrEmpty(result.Namespace) || result.Namespace.IndexOfAny(new[] { '/', '\\', ':' }) >= 0)
                throw new InvalidDataException("Invalid namespace in " + metaPath + ": " + result.Namespace);

            if (type == "sprite")
            {
                result.Texture = TarArchive.NormalizeEntryPath(Required(values, "texture"));
                result.Rect = ParseRect(Required(values, "rect"), metaPath);
                result.Pivot = ParseVector2(Optional(values, "pivot", "0.5,0.5"), "pivot", metaPath);
                result.Border = ParseVector4(Optional(values, "border", "0,0,0,0"), "border", metaPath);
                result.Vertices = ParseVector2Array(Optional(values, "vertices", string.Empty), "vertices", metaPath);
                result.Triangles = ParseUShortArray(Optional(values, "triangles", string.Empty), "triangles", metaPath);
                result.Uv = ParseVector2Array(Optional(values, "uv", string.Empty), "uv", metaPath);
                if (string.IsNullOrEmpty(result.Name))
                    throw new InvalidDataException("Sprite name is missing in " + metaPath);
                bool hasGeometry = result.Vertices.Length != 0 || result.Triangles.Length != 0 || result.Uv.Length != 0;
                if (hasGeometry && (result.Vertices.Length == 0 || result.Triangles.Length == 0 || result.Uv.Length != result.Vertices.Length))
                    throw new InvalidDataException("Incomplete sprite geometry in " + metaPath);
            }
            else
            {
                result.File = TarArchive.NormalizeEntryPath(Required(values, "file"));
                if (string.IsNullOrEmpty(result.Name))
                    result.Name = Path.GetFileNameWithoutExtension(result.File);
            }

            return result;
        }

        public static string SerializeSprite(
            string namespaceId, string address, string name, string texture, Sprite sprite,
            FilterMode filter, int aniso, TextureWrapMode wrapU, TextureWrapMode wrapV, bool mipmaps)
        {
            var builder = new StringBuilder(2048);
            Add(builder, "type", "sprite");
            Add(builder, "namespace", namespaceId);
            Add(builder, "address", address);
            Add(builder, "name", name);
            Add(builder, "texture", texture);
            Add(builder, "rect", Format(sprite.rect.x, sprite.rect.y, sprite.rect.width, sprite.rect.height));
            Vector2 normalizedPivot = new Vector2(sprite.pivot.x / sprite.rect.width, sprite.pivot.y / sprite.rect.height);
            Add(builder, "pivot", Format(normalizedPivot.x, normalizedPivot.y));
            Add(builder, "border", Format(sprite.border.x, sprite.border.y, sprite.border.z, sprite.border.w));
            Add(builder, "pixels_per_unit", sprite.pixelsPerUnit.ToString("R", Invariant));
            Add(builder, "filter", ((int)filter).ToString(Invariant));
            Add(builder, "aniso", aniso.ToString(Invariant));
            Add(builder, "wrap_u", ((int)wrapU).ToString(Invariant));
            Add(builder, "wrap_v", ((int)wrapV).ToString(Invariant));
            Add(builder, "mipmaps", mipmaps ? "true" : "false");
            Add(builder, "vertices", Format(sprite.vertices));
            Add(builder, "triangles", string.Join(",", sprite.triangles.Select(x => x.ToString(Invariant)).ToArray()));
            Add(builder, "uv", Format(sprite.uv));
            return builder.ToString();
        }

        public static string SerializeAudio(string namespaceId, string address, string name, string file)
        {
            var builder = new StringBuilder(256);
            Add(builder, "type", "audio");
            Add(builder, "namespace", namespaceId);
            Add(builder, "address", address);
            Add(builder, "name", name);
            Add(builder, "file", file);
            return builder.ToString();
        }

        public static string SerializeModel(string namespaceId, string address, string name, string file)
        {
            var builder = new StringBuilder(256);
            Add(builder, "type", "model");
            Add(builder, "namespace", namespaceId);
            Add(builder, "address", address);
            Add(builder, "name", name);
            Add(builder, "file", file);
            return builder.ToString();
        }

        public static string SerializeAtlas(string namespaceId, string address, string name, string file)
        {
            var builder = new StringBuilder(256);
            Add(builder, "type", "atlas");
            Add(builder, "namespace", namespaceId);
            Add(builder, "address", address);
            Add(builder, "name", name);
            Add(builder, "file", file);
            return builder.ToString();
        }

        public static Dictionary<string, string> ParseValues(string text)
        {
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            using (var reader = new StringReader(text ?? string.Empty))
            {
                string line;
                int number = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    number++;
                    line = line.Trim();
                    if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal)) continue;
                    int equals = line.IndexOf('=');
                    if (equals <= 0)
                        throw new InvalidDataException("Malformed metadata line " + number + ": " + line);
                    string key = line.Substring(0, equals).Trim();
                    string value = line.Substring(equals + 1).Trim();
                    if (values.ContainsKey(key))
                        throw new InvalidDataException("Duplicate metadata key: " + key);
                    values.Add(key, Unescape(value));
                }
            }
            return values;
        }

        public static string NormalizeAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new InvalidDataException("Empty asset address.");
            string value = address.Replace('\\', '/').Trim().TrimStart('/');
            if (value.Contains(":")) throw new InvalidDataException("Drive-qualified asset address: " + address);
            foreach (string part in value.Split('/'))
                if (string.IsNullOrEmpty(part) || part == "." || part == "..")
                    throw new InvalidDataException("Unsafe asset address: " + address);
            return value;
        }

        private static void Add(StringBuilder builder, string key, string value)
        {
            builder.Append(key).Append('=').Append(Escape(value ?? string.Empty)).Append('\n');
        }

        private static string Escape(string value)
        {
            return value.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n");
        }

        private static string Unescape(string value)
        {
            var output = new StringBuilder(value.Length);
            bool escaped = false;
            foreach (char c in value)
            {
                if (!escaped)
                {
                    if (c == '\\') escaped = true;
                    else output.Append(c);
                    continue;
                }
                if (c == 'n') output.Append('\n');
                else if (c == 'r') output.Append('\r');
                else output.Append(c);
                escaped = false;
            }
            if (escaped) output.Append('\\');
            return output.ToString();
        }

        private static string Required(Dictionary<string, string> values, string key)
        {
            string value;
            if (!values.TryGetValue(key, out value) || string.IsNullOrWhiteSpace(value))
                throw new InvalidDataException("Required metadata key is missing: " + key);
            return value;
        }

        private static string Optional(Dictionary<string, string> values, string key, string fallback)
        {
            string value;
            return values.TryGetValue(key, out value) ? value : fallback;
        }

        private static int ParseInt(string value, string field, string path)
        {
            int result;
            if (!int.TryParse(value, NumberStyles.Integer, Invariant, out result))
                throw new InvalidDataException("Invalid " + field + " in " + path + ": " + value);
            return result;
        }

        private static float ParseFloat(string value, string field, string path)
        {
            float result;
            if (!float.TryParse(value, NumberStyles.Float, Invariant, out result))
                throw new InvalidDataException("Invalid " + field + " in " + path + ": " + value);
            return result;
        }

        private static bool ParseBool(string value, string field, string path)
        {
            bool result;
            if (!bool.TryParse(value, out result))
                throw new InvalidDataException("Invalid " + field + " in " + path + ": " + value);
            return result;
        }

        private static Rect ParseRect(string value, string path)
        {
            float[] v = ParseFloatArray(value, 4, "rect", path);
            return new Rect(v[0], v[1], v[2], v[3]);
        }

        private static Vector2 ParseVector2(string value, string field, string path)
        {
            float[] v = ParseFloatArray(value, 2, field, path);
            return new Vector2(v[0], v[1]);
        }

        private static Vector4 ParseVector4(string value, string field, string path)
        {
            float[] v = ParseFloatArray(value, 4, field, path);
            return new Vector4(v[0], v[1], v[2], v[3]);
        }

        private static float[] ParseFloatArray(string value, int expected, string field, string path)
        {
            string[] parts = value.Split(',');
            if (parts.Length != expected)
                throw new InvalidDataException("Invalid " + field + " in " + path + ": " + value);
            var result = new float[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                result[i] = ParseFloat(parts[i], field, path);
            return result;
        }

        private static Vector2[] ParseVector2Array(string value, string field, string path)
        {
            if (string.IsNullOrWhiteSpace(value)) return Array.Empty<Vector2>();
            string[] pairs = value.Split(';');
            var result = new Vector2[pairs.Length];
            for (int i = 0; i < pairs.Length; i++)
                result[i] = ParseVector2(pairs[i], field, path);
            return result;
        }

        private static ushort[] ParseUShortArray(string value, string field, string path)
        {
            if (string.IsNullOrWhiteSpace(value)) return Array.Empty<ushort>();
            string[] parts = value.Split(',');
            var result = new ushort[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                ushort item;
                if (!ushort.TryParse(parts[i], NumberStyles.Integer, Invariant, out item))
                    throw new InvalidDataException("Invalid " + field + " in " + path + ": " + parts[i]);
                result[i] = item;
            }
            return result;
        }

        private static string Format(params float[] values)
        {
            return string.Join(",", values.Select(x => x.ToString("R", Invariant)).ToArray());
        }

        private static string Format(Vector2[] values)
        {
            if (values == null || values.Length == 0) return string.Empty;
            return string.Join(";", values.Select(x => x.x.ToString("R", Invariant) + "," + x.y.ToString("R", Invariant)).ToArray());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Eclipse.Modding
{
    internal sealed class SpriteAssetDescriptor
    {
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        public bool HasRect { get; private set; }
        public Rect Rect { get; private set; }
        public Vector2 Pivot { get; private set; } = new Vector2(0.5f, 0.5f);
        public Vector4 Border { get; private set; } = Vector4.zero;
        public float PixelsPerUnit { get; private set; } = 100f;
        public FilterMode Filter { get; private set; } = FilterMode.Bilinear;
        public TextureWrapMode Wrap { get; private set; } = TextureWrapMode.Clamp;
        public bool Mipmaps { get; private set; }

        public static SpriteAssetDescriptor Parse(string text, string source)
        {
            var result = new SpriteAssetDescriptor();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            string[] lines = (text ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = StripComment(lines[i]).Trim();
                if (line.Length == 0) continue;
                int equals = line.IndexOf('=');
                if (equals <= 0) throw Error(source, i + 1, "Expected key = value.");
                string key = line.Substring(0, equals).Trim();
                string value = line.Substring(equals + 1).Trim();
                if (!seen.Add(key)) throw Error(source, i + 1, "Duplicate field '" + key + "'.");

                switch (key)
                {
                    case "rect":
                        float[] rect = ParseArray(value, 4, source, i + 1);
                        result.Rect = new Rect(rect[0], rect[1], rect[2], rect[3]);
                        result.HasRect = true;
                        break;
                    case "pivot":
                        float[] pivot = ParseArray(value, 2, source, i + 1);
                        result.Pivot = new Vector2(pivot[0], pivot[1]);
                        break;
                    case "border":
                        float[] border = ParseArray(value, 4, source, i + 1);
                        result.Border = new Vector4(border[0], border[1], border[2], border[3]);
                        break;
                    case "pixels_per_unit":
                        result.PixelsPerUnit = ParseFloat(value, source, i + 1);
                        break;
                    case "filter":
                        result.Filter = ParseFilter(ParseString(value, source, i + 1), source, i + 1);
                        break;
                    case "wrap":
                        result.Wrap = ParseWrap(ParseString(value, source, i + 1), source, i + 1);
                        break;
                    case "mipmaps":
                        result.Mipmaps = ParseBool(value, source, i + 1);
                        break;
                    default:
                        throw Error(source, i + 1, "Unknown sprite field '" + key + "'.");
                }
            }

            if (result.PixelsPerUnit <= 0f || float.IsNaN(result.PixelsPerUnit) || float.IsInfinity(result.PixelsPerUnit))
                throw Error(source, 0, "pixels_per_unit must be finite and greater than zero.");
            if (result.Pivot.x < 0f || result.Pivot.x > 1f || result.Pivot.y < 0f || result.Pivot.y > 1f)
                throw Error(source, 0, "pivot components must be within 0..1.");
            return result;
        }

        private static string StripComment(string value)
        {
            bool quoted = false;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '"') quoted = !quoted;
                else if (!quoted && value[i] == '#') return value.Substring(0, i);
            }
            return value;
        }

        private static float[] ParseArray(string value, int count, string source, int line)
        {
            if (value.Length < 2 || value[0] != '[' || value[value.Length - 1] != ']')
                throw Error(source, line, "Expected numeric array.");
            string[] parts = value.Substring(1, value.Length - 2).Split(',');
            if (parts.Length != count) throw Error(source, line, "Expected " + count + " numeric values.");
            var result = new float[count];
            for (int i = 0; i < count; i++) result[i] = ParseFloat(parts[i].Trim(), source, line);
            return result;
        }

        private static float ParseFloat(string value, string source, int line)
        {
            float result;
            if (!float.TryParse(value, NumberStyles.Float, Invariant, out result) || float.IsNaN(result) || float.IsInfinity(result))
                throw Error(source, line, "Invalid finite number '" + value + "'.");
            return result;
        }

        private static bool ParseBool(string value, string source, int line)
        {
            if (value == "true") return true;
            if (value == "false") return false;
            throw Error(source, line, "Expected true or false.");
        }

        private static string ParseString(string value, string source, int line)
        {
            if (value.Length < 2 || value[0] != '"' || value[value.Length - 1] != '"')
                throw Error(source, line, "Expected quoted string.");
            return value.Substring(1, value.Length - 2);
        }

        private static FilterMode ParseFilter(string value, string source, int line)
        {
            if (value == "point") return FilterMode.Point;
            if (value == "bilinear") return FilterMode.Bilinear;
            if (value == "trilinear") return FilterMode.Trilinear;
            throw Error(source, line, "Unknown filter '" + value + "'.");
        }

        private static TextureWrapMode ParseWrap(string value, string source, int line)
        {
            if (value == "clamp") return TextureWrapMode.Clamp;
            if (value == "repeat") return TextureWrapMode.Repeat;
            if (value == "mirror") return TextureWrapMode.Mirror;
            if (value == "mirror_once") return TextureWrapMode.MirrorOnce;
            throw Error(source, line, "Unknown wrap mode '" + value + "'.");
        }

        private static InvalidDataException Error(string source, int line, string message)
        {
            return new InvalidDataException((line > 0 ? source + ":" + line : source) + ": " + message);
        }
    }
}

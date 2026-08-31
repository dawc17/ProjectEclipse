using System;
using System.Collections.Generic;
using System.IO;

namespace Eclipse.Modding
{
    public enum AssetKind
    {
        Unknown = 0,
        Sprite = 1,
        Model = 2,
        Audio = 3,
        Text = 4,
        Binary = 5,
        Texture = 6
    }

    public enum AssetSourceKind
    {
        LooseMod = 0,
        Core = 1
    }

    public sealed class AssetMetadata
    {
        public AssetId Id { get; }
        public AssetKind Kind { get; }
        public AssetSourceKind SourceKind { get; }
        public string Format { get; }
        public long Size { get; }
        public string DiagnosticSource { get; }

        public AssetMetadata(AssetId id, AssetKind kind, AssetSourceKind sourceKind,
            string format, long size, string diagnosticSource)
        {
            Id = id;
            Kind = kind;
            SourceKind = sourceKind;
            Format = format ?? string.Empty;
            Size = size;
            DiagnosticSource = diagnosticSource ?? string.Empty;
        }
    }

    public sealed class AssetBytes
    {
        public AssetMetadata Metadata { get; }
        public byte[] Data { get; }

        public AssetBytes(AssetMetadata metadata, byte[] data)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
    }

    public interface IAssetProvider
    {
        ModId Namespace { get; }
        bool TryDescribe(AssetId id, out AssetMetadata metadata);
    }

    public interface IAssetByteProvider : IAssetProvider
    {
        bool TryRead(AssetId id, out AssetBytes bytes);
    }

    public interface IAssetEnumerableProvider : IAssetProvider
    {
        IReadOnlyList<AssetMetadata> Assets { get; }
    }

    // Shared descriptor syntax for discovery (no Unity dependency) and typed loading.
    public static class AssetDescriptor
    {
        public static Dictionary<string, string> ParseFields(string text, string source)
        {
            var fields = new Dictionary<string, string>(StringComparer.Ordinal);
            string[] lines = (text ?? string.Empty).TrimStart('\uFEFF').Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                bool quoted = false;
                for (int j = 0; j < line.Length; j++)
                {
                    if (line[j] == '"') quoted = !quoted;
                    else if (!quoted && line[j] == '#') { line = line.Substring(0, j); break; }
                }
                line = line.Trim();
                if (line.Length == 0) continue;
                int equals = line.IndexOf('=');
                if (quoted || equals <= 0)
                    throw new InvalidDataException(source + ":" + (i + 1) + ": Expected key = value with balanced quotes.");
                string key = line.Substring(0, equals).Trim();
                if (fields.ContainsKey(key))
                    throw new InvalidDataException(source + ":" + (i + 1) + ": Duplicate field '" + key + "'.");
                fields.Add(key, line.Substring(equals + 1).Trim());
            }
            return fields;
        }

        public static string ReadString(string value, string source)
        {
            if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
                value = value.Substring(1, value.Length - 2);
            if (string.IsNullOrWhiteSpace(value) || value.IndexOf('"') >= 0)
                throw new InvalidDataException(source + ": Expected a nonempty string without embedded quotes.");
            return value;
        }

        public static AssetKind GetKind(Dictionary<string, string> fields, string source)
        {
            string type;
            if (!fields.TryGetValue("type", out type))
                throw new InvalidDataException(source + ": Required field 'type' is missing.");
            type = ReadString(type, source);
            if (type == "sprite") return AssetKind.Sprite;
            throw new InvalidDataException(source + ": Unsupported asset type '" + type + "'.");
        }
    }
}

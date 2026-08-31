using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public enum AssetKind
    {
        Unknown = 0,
        Sprite = 1,
        Model = 2,
        Audio = 3,
        Text = 4,
        Binary = 5
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
}

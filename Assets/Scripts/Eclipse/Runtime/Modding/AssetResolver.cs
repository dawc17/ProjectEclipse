using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public sealed class AssetResolver
    {
        private readonly Dictionary<ModId, IAssetProvider> _providers;

        public AssetResolver(IEnumerable<IAssetProvider> providers)
        {
            if (providers == null) throw new ArgumentNullException(nameof(providers));
            _providers = new Dictionary<ModId, IAssetProvider>();
            foreach (IAssetProvider provider in providers)
            {
                if (provider == null) throw new ArgumentException("Asset provider list contains null.", nameof(providers));
                if (_providers.ContainsKey(provider.Namespace))
                    throw new InvalidOperationException("Multiple asset providers claim namespace '" + provider.Namespace + "'.");
                _providers.Add(provider.Namespace, provider);
            }
        }

        public bool TryDescribe(AssetId id, out AssetMetadata metadata)
        {
            metadata = null;
            IAssetProvider provider;
            return _providers.TryGetValue(id.Namespace, out provider) && provider.TryDescribe(id, out metadata);
        }

        public bool TryRead(AssetId id, out AssetBytes bytes)
        {
            bytes = null;
            IAssetProvider provider;
            if (!_providers.TryGetValue(id.Namespace, out provider)) return false;
            IAssetByteProvider byteProvider = provider as IAssetByteProvider;
            return byteProvider != null && byteProvider.TryRead(id, out bytes);
        }

        public bool TryGetProvider(ModId namespaceId, out IAssetProvider provider)
        {
            return _providers.TryGetValue(namespaceId, out provider);
        }

        public AssetId Qualify(ModId caller, string reference)
        {
            if (string.IsNullOrEmpty(reference)) throw new FormatException("Asset reference must not be empty.");
            return reference.IndexOf(':') >= 0 ? AssetId.Parse(reference) : AssetId.Parse(caller.Value + ":" + reference);
        }
    }
}

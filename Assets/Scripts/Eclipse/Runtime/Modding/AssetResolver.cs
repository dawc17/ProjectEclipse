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

        private Dictionary<AssetId, AssetId> _redirects = new Dictionary<AssetId, AssetId>();
        public void SetReplacements(IEnumerable<ModAssetReplacement> replacements)
        {
            var redirects = new Dictionary<AssetId, AssetId>();
            if (replacements != null) foreach (var value in replacements) redirects.Add(value.Target, value.Replacement);
            foreach (var start in redirects.Keys)
            {
                var seen = new HashSet<AssetId>();
                var current = start;
                while (redirects.TryGetValue(current, out var next))
                {
                    if (!seen.Add(current)) throw new InvalidOperationException("Asset redirect cycle.");
                    current = next;
                }
            }
            _redirects = redirects;
        }
        public AssetId Resolve(AssetId id)
        {
            var seen = new HashSet<AssetId>();
            while(_redirects.TryGetValue(id,out var next))
            { if(!seen.Add(id)) throw new InvalidOperationException("Asset redirect cycle."); id=next; }
            return id;
        }
        public bool TryDescribe(AssetId id, out AssetMetadata metadata)
        {
            id = Resolve(id);
            metadata = null;
            IAssetProvider provider;
            return _providers.TryGetValue(id.Namespace, out provider) && provider.TryDescribe(id, out metadata);
        }

        public bool TryRead(AssetId id, out AssetBytes bytes)
        {
            id = Resolve(id);
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

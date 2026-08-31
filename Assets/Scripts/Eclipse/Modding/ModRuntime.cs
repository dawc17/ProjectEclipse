using System;
using UnityEngine;

namespace Eclipse.Modding
{
    public static class ModRuntime
    {
        private static ModHost _host;

        public static bool IsInitialized => _host != null;
        public static ModHost Host => _host ?? InitializeDefault();

        public static ModHost InitializeDefault()
        {
            return Initialize(ModHost.GetDefaultModsRoot());
        }

        public static ModHost Initialize(string modsRoot)
        {
            Shutdown();
            _host = ModHost.Build(modsRoot);
            Debug.Log("[ModHost] " + _host.EnabledMods.Count + " mod(s) enabled; " +
                _host.Diagnostics.Count + " diagnostic(s). Root: " + _host.ModsRoot);
            return _host;
        }

        public static bool TryLoadQualified<T>(string reference, out T asset) where T : UnityEngine.Object
        {
            asset = null;
            AssetId id;
            if (!TryParseQualified(reference, out id)) return false;
            asset = Host.TypedAssets.LoadUnityAsset<T>(id);
            return true;
        }

        public static bool TryLoadQualifiedWithSubAssets<T>(string reference, out T[] assets)
            where T : UnityEngine.Object
        {
            assets = null;
            AssetId id;
            if (!TryParseQualified(reference, out id)) return false;
            assets = Host.TypedAssets.LoadUnityAssets<T>(id);
            return true;
        }

        public static string LoadQualifiedModelText(string reference)
        {
            AssetId id;
            return TryParseQualified(reference, out id) ? Host.TypedAssets.LoadModelText(id) : null;
        }

        public static void Shutdown()
        {
            if (_host == null) return;
            _host.Dispose();
            _host = null;
        }

        private static bool TryParseQualified(string reference, out AssetId id)
        {
            id = default;
            if (string.IsNullOrEmpty(reference)) return false;
            int colon = reference.IndexOf(':');
            if (colon <= 0) return false;
            ModId namespaceId;
            if (!ModId.TryParse(reference.Substring(0, colon), out namespaceId)) return false;
            id = AssetId.Parse(reference);
            return true;
        }
    }
}

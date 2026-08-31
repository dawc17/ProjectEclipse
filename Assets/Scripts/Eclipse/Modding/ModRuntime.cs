using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Eclipse.Content;
using UnityEngine;

namespace Eclipse.Modding
{
    public static class ModRuntime
    {
        private static ModHost _host;
        private static ModScriptSession _scripts;
        private static LegacyContentAdapter _legacyContent;

        public static bool IsInitialized => _host != null;
        public static ModHost Host => _host ?? InitializeDefault();
        public static ModScriptSession Scripts => _scripts;

        public static ModScriptSession StartScripts()
        {
            _legacyContent?.Dispose();
            _legacyContent = null;
            _scripts?.Dispose();
            _scripts = Host.StartScripts(new MoonSharpScriptRuntime(), LogScript, ImportCoreItems);
            Debug.Log("[ModScripts] " + _scripts.RuntimeName + "; " + _scripts.ActiveMods.Count +
                " mod(s) active; " + _scripts.Diagnostics.Count + " diagnostic(s).");
            return _scripts;
        }

        public static void StartGameContent()
        {
            StartGameContent(ModHost.GetDefaultModsRoot());
        }

        public static void StartGameContent(string modsRoot)
        {
            try
            {
                Initialize(modsRoot);
                ModScriptSession scripts = StartScripts();
                _legacyContent = new LegacyContentAdapter(scripts.Content);
                _legacyContent.ApplyItems(ListSF.DJBOFEEKJMP());
                Debug.Log("[ModContent] Catalog contains " + scripts.Content.Weapons.Count +
                    " core/mod weapon definition(s); applied " + scripts.Content.ShopListings.Count + " external shop listing(s).");
            }
            catch (Exception exception)
            {
                Debug.LogError("[ModContent] Failed to apply mod items; continuing without external mods. " + exception);
                Shutdown();
            }
        }

        public static void ApplyLegacyLocalization()
        {
            if (_legacyContent == null) return;
            try
            {
                _legacyContent.ApplyLocalization();
            }
            catch (Exception exception)
            {
                Debug.LogError("[ModContent] Failed to apply mod localization; vanilla localization remains active. " +
                    exception);
            }
        }

        public static void RecordSaveContext(System.Xml.XmlNode warrior)
        {
            // Do not overwrite provenance if mod initialization itself was unavailable.
            if (_scripts == null) return;
            if (!ModSaveData.RecordContext(warrior, _scripts.ActiveMods, _scripts.Content))
                Debug.LogWarning("[ModSave] Unrecognized save metadata schema; leaving it unchanged.");
        }

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
            if (!string.IsNullOrEmpty(reference) && reference.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                reference = reference.Substring(0, reference.Length - 4);
            AssetId id;
            return TryParseQualified(reference, out id) ? Host.TypedAssets.LoadModelText(id) : null;
        }

        public static void Shutdown()
        {
            _legacyContent?.Dispose();
            _legacyContent = null;
            _scripts?.Dispose();
            _scripts = null;
            if (_host == null) return;
            _host.Dispose();
            _host = null;
        }

        private static void LogScript(ModLogEntry entry)
        {
            string message = "[Mod:" + entry.ModId + "] " + entry.Message;
            if (entry.Level == ModLogLevel.Error) Debug.LogError(message);
            else if (entry.Level == ModLogLevel.Warning) Debug.LogWarning(message);
            else Debug.Log(message);
        }

        private static void ImportCoreItems(ModContentCatalog content)
        {
            var nodes = new List<XmlNode>();
            foreach (ItemInfo item in ListSF.DJBOFEEKJMP().HCDLKHKBEPF())
                if (item.Name.IndexOf(':') < 0 && item.NodeXML != null) nodes.Add(item.NodeXML);
            if (nodes.Count == 0) return;
            var languages = CoreContentImporter.ReadLocalizations(
                Path.Combine(GameplayContentArchive.GetXmlRoot(), "localizations"));
            int weapons = CoreContentImporter.ImportWeapons(content, nodes, languages);
            int armors = CoreContentImporter.ImportArmors(content, nodes, languages);
            int helms = CoreContentImporter.ImportHelms(content, nodes, languages);
            int ranged = CoreContentImporter.ImportRanged(content, nodes, languages);
            int magic = CoreContentImporter.ImportMagic(content, nodes, languages);
            Debug.Log("[ModContent] Imported core equipment: " + weapons + " weapons, " + armors +
                " armors, " + helms + " helms, " + ranged + " ranged, " + magic + " magic.");
        }

        private static bool TryParseQualified(string reference, out AssetId id)
        {
            id = default;
            if (string.IsNullOrEmpty(reference)) return false;
            int colon = reference.IndexOf(':');
            if (colon <= 0) return false;
            ModId namespaceId;
            if (!ModId.TryParse(reference.Substring(0, colon), out namespaceId)) return false;
            return AssetId.TryParse(reference, out id);
        }
    }
}

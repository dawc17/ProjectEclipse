using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Eclipse.Content;
using UnityEngine;

namespace Eclipse.Modding
{
    public static partial class ModRuntime
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
            ModModeRuntime.Clear();
            ModModeRuntime.Warning = message => Debug.LogWarning(message);
            ModPolicies.Content = null;
            _scripts = Host.StartScripts(new MoonSharpScriptRuntime(), LogScript, ImportCoreContent);
            ModPolicies.Content = _scripts.Content;
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
                _legacyContent.ApplyPerksAndEnchantments(GameUtils.FDEJIIDIPBI, ForgeManager.ELEBLBJKDBI());
                ApplyP1DContent();
                Debug.Log("[ModContent] Catalog equipment: " + scripts.Content.Weapons.Count + " weapons, " +
                    scripts.Content.Armors.Count + " armor, " + scripts.Content.Helms.Count + " helms, " +
                    scripts.Content.Ranged.Count + " ranged, " + scripts.Content.Magic.Count + " magic; applied " +
                    scripts.Content.ShopListings.Count + " external shop listing(s), " + scripts.Content.Perks.Count +
                    " perks, " + scripts.Content.Enchantments.Count + " external enchantment(s).");
            }
            catch (Exception exception)
            {
                Debug.LogError("[ModContent] Failed to apply mod content; continuing without external mods. " + exception);
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

        public static void ApplyStageContent()
        {
            if (_legacyContent == null) return;
            try
            {
                _legacyContent.ApplyStages(ListSF.ELEBLBJKDBI());
                _legacyContent.ApplyP3Content();
                Debug.Log("[ModContent] Applied stage graph: " + Scripts.Content.Zones.Count + " zones, " +
                    Scripts.Content.Battles.Count + " battles, " + Scripts.Content.Fights.Count + " fights.");
            }
            catch (Exception exception)
            {
                Debug.LogError("[ModContent] Failed to apply mod stage content; continuing without external mods. " + exception);
                // Let the base parse finish. Throwing here makes ParseModule retry the entire
                // non-idempotent item/zone parse on its next Update, duplicating vanilla content.
                Shutdown();
            }
        }

        public static void ApplyQuestContent()
        {
            if (_legacyContent == null) return;
            try
            {
                _legacyContent.ApplyQuests(ListSF.ELEBLBJKDBI());
                Debug.Log("[ModContent] Applied quest graph: " + Scripts.Content.Quests.Count + " external quest(s).");
            }
            catch (Exception exception)
            {
                Debug.LogError("[ModContent] Failed to apply mod quest content; continuing without external mods. " + exception);
                Shutdown();
            }
        }

        public static bool TryGetExternalEffectPresentation(string runtimeName, out string displayName,
            out string description)
        {
            displayName = string.Empty;
            description = string.Empty;
            if (_scripts == null || string.IsNullOrEmpty(runtimeName)) return false;

            DefinitionId id;
            if (!DefinitionId.TryParse(runtimeName, out id) || id.Namespace.Value == "core") return false;

            if (id.Category == "enchantments")
            {
                EnchantmentDefinition enchantment;
                if (!_scripts.Content.TryGetEnchantment(id, out enchantment)) return false;
                displayName = enchantment.DisplayName.ToString();
                description = enchantment.Description.ToString();
                return !string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(description);
            }

            if (id.Category == "perks")
            {
                PerkDefinition perk;
                if (!_scripts.Content.TryGetPerk(id, out perk)) return false;
                displayName = perk.DisplayName.ToString();
                description = perk.Description.ToString();
                return !string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(description);
            }

            return false;
        }

        public static void RecordSaveContext(System.Xml.XmlNode warrior)
        {
            // Do not overwrite provenance if mod initialization itself was unavailable.
            if (_scripts == null) return;
            if (!ModSaveData.RecordContext(warrior, _scripts.ActiveMods, _scripts.Content, _scripts.State))
            {
                Debug.LogWarning("[ModSave] Unrecognized save metadata schema; leaving it unchanged.");
                return;
            }
            ModModeRuntime.Bind(warrior);
            IReadOnlyList<ModDiagnostic> stateDiagnostics = _scripts.BindState(warrior);
            for (int i = 0; i < stateDiagnostics.Count; i++)
                Debug.LogWarning("[ModSave] " + stateDiagnostics[i]);
        }

        public static bool TryReadSavedEnchantment(XmlNode perkNode, out EnchantmentDefinition enchantment,
            out ModEffectInstance instance, out string error)
        {
            enchantment = null;
            instance = null;
            error = string.Empty;
            if (_scripts == null || perkNode == null)
            {
                error = "Mod scripts are not active or the saved enchantment node is missing.";
                return false;
            }
            string savedId = perkNode.Attributes?[PerkStruct.EclipseEnchantmentAttribute]?.Value;
            DefinitionId id;
            if (!DefinitionId.TryParse(savedId, out id) || id.Category != "enchantments" ||
                id.Namespace.Value == "core")
            {
                error = "Saved enchantment has no valid EclipseEnchantment identity.";
                return false;
            }
            if (!_scripts.Content.TryGetEnchantment(id, out enchantment) || !enchantment.HasBehavior)
            {
                error = "Saved scripted enchantment is unavailable: '" + id + "'.";
                enchantment = null;
                return false;
            }
            ModBehaviorDefinition behavior;
            if (!_scripts.Content.TryGetBehavior(enchantment.Behavior, out behavior))
            {
                error = "Saved enchantment behavior is unavailable: '" + enchantment.Behavior + "'.";
                enchantment = null;
                return false;
            }

            if (perkNode[ModEffectSaveData.NodeName] == null)
            {
                try
                {
                    Dictionary<string, ModParameterValue> values =
                        behavior.Parameters.ResolveValues(enchantment.InitialParameters);
                    instance = new ModEffectInstance(enchantment.Id, values);
                    return true;
                }
                catch (ModContentException exception)
                {
                    error = exception.Message;
                    return false;
                }
            }
            return ModEffectSaveData.TryRead(perkNode, enchantment.Id, behavior.Parameters, out instance, out error);
        }

        public static bool TryInvokeSavedEnchantmentFightBegin(XmlNode perkNode,
            IReadOnlyDictionary<string, string> fighterContext, out string error)
        {
            return TryInvokeSavedEnchantmentFightBegin(perkNode, fighterContext, null, out error);
        }

        public static bool TryInvokeSavedEnchantmentFightBegin(XmlNode perkNode,
            IReadOnlyDictionary<string, string> fighterContext, IModFighterOperations fighter, out string error,
            ModEffectEvent effectEvent = ModEffectEvent.FightBegin)
        {
            EnchantmentDefinition enchantment;
            ModEffectInstance instance;
            if (!TryReadSavedEnchantment(perkNode, out enchantment, out instance, out error)) return false;
            if (_scripts == null)
            {
                error = "Mod scripts are not active.";
                return false;
            }
            return _scripts.TryInvokeBehavior(enchantment.Behavior, effectEvent,
                instance.Values, fighterContext, new ModInstanceFighter(fighter, perkNode), out error);
        }

        public static bool TryInvokePerkFightBegin(DefinitionId perkId,
            IReadOnlyDictionary<string, string> fighterContext, IModFighterOperations fighter, out string error)
        {
            error = string.Empty;
            if (_scripts == null)
            {
                error = "Mod scripts are not active.";
                return false;
            }
            if (perkId.Category != "perks" || perkId.Namespace.Value == "core")
            {
                error = "Scripted perk has no valid external perk identity: '" + perkId + "'.";
                return false;
            }
            PerkDefinition perk;
            if (!_scripts.Content.TryGetPerk(perkId, out perk) || !perk.HasBehavior)
            {
                error = "Scripted perk is unavailable: '" + perkId + "'.";
                return false;
            }
            ModBehaviorDefinition behavior;
            if (!_scripts.Content.TryGetBehavior(perk.Behavior, out behavior))
            {
                error = "Scripted perk behavior is unavailable: '" + perk.Behavior + "'.";
                return false;
            }
            try
            {
                Dictionary<string, ModParameterValue> values = behavior.Parameters.ResolveValues(perk.InitialParameters);
                return _scripts.TryInvokeBehavior(perk.Behavior, ModEffectEvent.FightBegin,
                    values, fighterContext, fighter, out error);
            }
            catch (ModContentException exception)
            {
                error = exception.Message;
                return false;
            }
        }

        public static bool TryReadSavedPerk(XmlNode perkNode, out PerkDefinition perk,
            out ModEffectInstance instance, out string error)
        {
            perk = null;
            instance = null;
            error = string.Empty;
            if (_scripts == null || perkNode == null)
            {
                error = "Mod scripts are not active or the saved perk node is missing.";
                return false;
            }
            DefinitionId id;
            string savedId = perkNode.Attributes?["Name"]?.Value;
            if (!DefinitionId.TryParse(savedId, out id) || id.Category != "perks" || id.Namespace.Value == "core")
            {
                error = "Saved perk has no valid external perk identity.";
                return false;
            }
            if (!_scripts.Content.TryGetPerk(id, out perk) || !perk.HasBehavior)
            {
                error = "Saved scripted perk is unavailable: '" + id + "'.";
                perk = null;
                return false;
            }
            ModBehaviorDefinition behavior;
            if (!_scripts.Content.TryGetBehavior(perk.Behavior, out behavior))
            {
                error = "Saved perk behavior is unavailable: '" + perk.Behavior + "'.";
                perk = null;
                return false;
            }
            if (perkNode[ModEffectSaveData.NodeName] == null)
            {
                try
                {
                    Dictionary<string, ModParameterValue> values = behavior.Parameters.ResolveValues(perk.InitialParameters);
                    instance = new ModEffectInstance(perk.Id, values);
                    return true;
                }
                catch (ModContentException exception)
                {
                    error = exception.Message;
                    return false;
                }
            }
            return ModEffectSaveData.TryRead(perkNode, perk.Id, behavior.Parameters, out instance, out error);
        }

        public static bool TryInvokeSavedPerkFightBegin(XmlNode perkNode,
            IReadOnlyDictionary<string, string> fighterContext, IModFighterOperations fighter, out string error,
            ModEffectEvent effectEvent = ModEffectEvent.FightBegin)
        {
            PerkDefinition perk;
            ModEffectInstance instance;
            if (!TryReadSavedPerk(perkNode, out perk, out instance, out error)) return false;
            if (_scripts == null)
            {
                error = "Mod scripts are not active.";
                return false;
            }
            return _scripts.TryInvokeBehavior(perk.Behavior, effectEvent,
                instance.Values, fighterContext, new ModInstanceFighter(fighter, perkNode), out error);
        }

        public static bool TryInitializeSavedPerkParameters(XmlNode perkNode, out string error)
        {
            error = string.Empty;
            if (_scripts == null || perkNode == null || perkNode[ModEffectSaveData.NodeName] != null) return true;
            DefinitionId id;
            string savedId = perkNode.Attributes?["Name"]?.Value;
            if (!DefinitionId.TryParse(savedId, out id) || id.Category != "perks" || id.Namespace.Value == "core") return true;
            PerkDefinition perk;
            if (!_scripts.Content.TryGetPerk(id, out perk) || !perk.HasBehavior) return true;
            ModBehaviorDefinition behavior;
            if (!_scripts.Content.TryGetBehavior(perk.Behavior, out behavior))
            {
                error = "Scripted perk behavior is unavailable: '" + perk.Behavior + "'.";
                return false;
            }
            XmlElement element = perkNode as XmlElement;
            if (element == null)
            {
                error = "Saved scripted perk is not an XML element.";
                return false;
            }
            try
            {
                ModEffectSaveData.Write(element, perk.Id, behavior.Parameters, perk.InitialParameters);
                return true;
            }
            catch (Exception exception) when (exception is ModContentException || exception is ArgumentException)
            {
                error = exception.Message;
                return false;
            }
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

        public static bool TryLoadCoreSpriteReplacement(string atlas, string member, out Sprite sprite)
        {
            sprite = null;
            if (string.IsNullOrEmpty(atlas) || string.IsNullOrEmpty(member)) return false;
            string path = atlas.Replace('\\', '/').TrimEnd('/');
            string leaf = path.Substring(path.LastIndexOf('/') + 1);
            string address = path + "." + (member.StartsWith(leaf + ".", StringComparison.OrdinalIgnoreCase) ? member.Substring(leaf.Length + 1) : member);
            if (!TryResolveCoreReplacement(address, out var replacement)) return false;
            sprite = Host.TypedAssets.LoadReplacementMember(replacement, member);
            return true;
        }

        public static string LoadCoreModelReplacement(string reference)
        {
            if (string.IsNullOrEmpty(reference)) return null;
            string path = reference.Replace('\\', '/').TrimStart('/');
            if (!path.StartsWith("gamedata/models/", StringComparison.OrdinalIgnoreCase)) return null;
            if (path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) path = path.Substring(0, path.Length - 4);
            return TryResolveCoreReplacement(path, out var replacement) ? Host.TypedAssets.LoadModelText(replacement) : null;
        }

        public static bool TryResolveCoreReplacement(string reference, out AssetId replacement)
        {
            replacement=default;
            if(_host==null || string.IsNullOrEmpty(reference)) return false;
            if(!AssetId.TryParse("core:"+reference.Replace('\\','/').TrimStart('/'),out var id)) return false;
            replacement=_host.Assets.Resolve(id);
            return replacement!=id;
        }

        public static bool TryLoadCore<T>(string reference, out T asset) where T : UnityEngine.Object
        {
            asset = null;
            AssetId id;
            if (!TryQualifyCore(reference, out id)) return false;
            IAssetProvider provider;
            IRuntimeAssetProvider runtimeProvider;
            if (!Host.Assets.TryGetProvider(id.Namespace, out provider) ||
                (runtimeProvider = provider as IRuntimeAssetProvider) == null) return false;
            return runtimeProvider.TryLoadUnityAsset(id, out asset);
        }

        public static bool TryLoadCoreWithSubAssets<T>(string reference, out T[] assets)
            where T : UnityEngine.Object
        {
            assets = null;
            AssetId id;
            if (!TryQualifyCore(reference, out id)) return false;
            IAssetProvider provider;
            IRuntimeAssetProvider runtimeProvider;
            if (!Host.Assets.TryGetProvider(id.Namespace, out provider) ||
                (runtimeProvider = provider as IRuntimeAssetProvider) == null) return false;
            return runtimeProvider.TryLoadUnityAssets(id, out assets);
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
            ModModeRuntime.Clear();
            ModProgressionAccess.Clear();
            ModPolicies.Content = null;
            _legacyContent?.Dispose();
            _legacyContent = null;
            _scripts?.Dispose();
            _scripts = null;
            if (_host == null) return;
            _host.Dispose();
            _host = null;
            AtlasCache.Clear();
            LocationSpriteCache.Clear();
        }

        private static void LogScript(ModLogEntry entry)
        {
            string message = "[Mod:" + entry.ModId + "] " + entry.Message;
            if (entry.Level == ModLogLevel.Error) Debug.LogError(message);
            else if (entry.Level == ModLogLevel.Warning) Debug.LogWarning(message);
            else Debug.Log(message);
        }

        private static void ImportCoreContent(ModContentCatalog content)
        {
            var nodes = new List<XmlNode>();
            foreach (ItemInfo item in ListSF.DJBOFEEKJMP().HCDLKHKBEPF())
                if (item.Name.IndexOf(':') < 0 && item.NodeXML != null) nodes.Add(item.NodeXML);
            var languages = CoreContentImporter.ReadLocalizations(
                Path.Combine(GameplayContentArchive.GetXmlRoot(), "localizations"));
            int weapons = nodes.Count == 0 ? 0 : CoreContentImporter.ImportWeapons(content, nodes, languages);
            int armors = nodes.Count == 0 ? 0 : CoreContentImporter.ImportArmors(content, nodes, languages);
            int helms = nodes.Count == 0 ? 0 : CoreContentImporter.ImportHelms(content, nodes, languages);
            int ranged = nodes.Count == 0 ? 0 : CoreContentImporter.ImportRanged(content, nodes, languages);
            int magic = nodes.Count == 0 ? 0 : CoreContentImporter.ImportMagic(content, nodes, languages);
            int nonEquipment = nodes.Count == 0 ? 0 : CoreContentImporter.ImportNonEquipment(content, nodes);
            var forgeProfileNames = new List<string>();
            ForgeManager forge = ForgeManager.ELEBLBJKDBI();
            if (forge != null)
            {
                foreach (Recipe recipe in forge.Recipes)
                {
                    if (recipe == null || string.IsNullOrEmpty(recipe.Name)) continue;
                    forgeProfileNames.Add(recipe.Name);
                }
            }
            int forgeProfiles = CoreContentImporter.ImportForgeEconomicProfiles(content, forgeProfileNames);
            int perks = 0;
            string perksPath = Path.Combine(GameplayContentArchive.GetXmlRoot(), "perks.xml");
            var perksDocument = new XmlDocument { XmlResolver = null };
            using (XmlReader reader = XmlReader.Create(perksPath, new XmlReaderSettings
                { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null })) perksDocument.Load(reader);
            XmlNode perksRoot = perksDocument["Perks"];
            if (perksRoot != null) perks = CoreContentImporter.ImportPerks(content, EnumerateChildren(perksRoot));
            int fights = 0;
            string stagesPath = Path.Combine(GameplayContentArchive.GetXmlRoot(), "stages.xml");
            var stagesDocument = new XmlDocument { XmlResolver = null };
            using (XmlReader reader = XmlReader.Create(stagesPath, new XmlReaderSettings
                { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null })) stagesDocument.Load(reader);
            XmlNode zonesRoot = stagesDocument["Stages"]?["Zones"];
            if (zonesRoot != null) fights = CoreContentImporter.ImportStages(content, zonesRoot);
            int warriorTemplates = CoreContentImporter.ImportWarriorTemplates(content,
                stagesDocument["Stages"]?["Warriors"]?["Templates"]);
            Debug.Log("[ModContent] Imported core items: " + weapons + " weapons, " + armors +
                " armors, " + helms + " helms, " + ranged + " ranged, " + magic + " magic, " + nonEquipment +
                " non-equipment; " + perks + " perks; " + forgeProfiles + " immutable forge economic profiles.");
            Debug.Log("[ModContent] Imported core stage graph: " + content.Zones.Count + " zones, " +
                content.Battles.Count + " battles, " + fights + " fights, " + warriorTemplates +
                " warrior templates.");
        }

        private static IEnumerable<XmlNode> EnumerateChildren(XmlNode parent)
        {
            foreach (XmlNode child in parent.ChildNodes) yield return child;
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

        private static bool TryQualifyCore(string reference, out AssetId id)
        {
            id = default;
            if (string.IsNullOrEmpty(reference) || reference.IndexOf(':') >= 0) return false;
            try
            {
                id = AssetId.Parse("core:" + reference);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}

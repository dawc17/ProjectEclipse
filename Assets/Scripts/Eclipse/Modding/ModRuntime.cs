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
        private static readonly ModDojoSelection DojoSelection = new ModDojoSelection();
        internal static readonly ModStoryEvents StoryEvents = new ModStoryEvents(
            (owner, message) => Debug.LogWarning("[ModStory] " + owner + ": " + message));
        private static Roster _profileRoster;
        private static bool _sceneNavigationInProgress;
        public static string ResolveDojoLocation(string fallback) => DojoSelection.Resolve(fallback);

        public static bool IsInitialized => _host != null;
        public static ModHost Host => _host ?? InitializeDefault();
        public static ModScriptSession Scripts => _scripts;

        public static ModScriptSession StartScripts()
        {
            StoryEvents.Clear();
            _profileRoster = null;
            ModProfileAccess.Clear();
            DojoSelection.Clear();
            _legacyContent?.Dispose();
            _legacyContent = null;
            _scripts?.Dispose();
            ModModeRuntime.Clear();
            ModModeRuntime.SelectNext = null;
            ModModeRuntime.Prepare = null; ModModeRuntime.BuildEncounter = null; ModModeRuntime.SchedulePreparation = null;
            ModModeRuntime.Warning = message => Debug.LogWarning(message);
            ModPolicies.Content = null;
            _scripts = Host.StartScripts(new MoonSharpScriptRuntime(Eclipse.UI.Modding.ModUiGameBridge.Attach,
                () => LocalizationManager.ILAJKOBCHFH == null ? LocalizationManager.POIPGLLCCKC : LocalizationManager.ILAJKOBCHFH.name, DojoSelection, StoryEvents), LogScript, ImportCoreContent);
            var dojoChoices = new List<DefinitionId>();
            foreach (var location in _scripts.Content.Locations)
                if (location.IsDojo) dojoChoices.Add(location.Id);
            DojoSelection.SetChoices(dojoChoices);
            ModProfileAccess.Level = ReadProfileLevel;
            ModProfileAccess.Item = ReadProfileItem;
            ModPolicies.Content = _scripts.Content;
            ModModeRuntime.SchedulePreparation = (request,ready,cancel) =>
                new GameObject("Mod encounter preparation").AddComponent<ModPendingEncounter>().Configure(request,ready,cancel);
            ModModeRuntime.Prepare = (mode,step,completions,request) => {
                if (!_scripts.TryPrepareMode(mode,step,completions,request,out var error)) throw new ModContentException(error);
            };
            ModModeRuntime.BuildEncounter = (mode,step,plan) => {
                if (_legacyContent == null || !_scripts.Content.TryGetFight(mode.Fights[step],out var definition))
                    throw new ModContentException("Generated encounter content is unavailable.");
                var original = ListSF.CHMCKGCDGCM(new FightIDS(_scripts.Content.RuntimeFightId(definition.Id)));
                if (original == null) throw new ModContentException("Generated encounter blueprint is unavailable.");
                var node = _legacyContent.BuildEncounterNode(definition,plan);
                var result = new FightList();
                ListSF.ELEBLBJKDBI().FOKCPLOMLOK(result,node,original.get_Type(),original.JKMJHIIMHPG,original.NPPIFKKLNCN,original.CNAOMDMIGLJ);
                result.BCKFACGMOKC = new FightIDS(original.BCKFACGMOKC.ToString());
                result.CNAOMDMIGLJ = original.CNAOMDMIGLJ; result.Index = original.Index;
                return result;
            };
            ModModeRuntime.SelectNext = (mode,won,step,completions) => {
                if (_scripts.TryChooseModeNext(mode,won,step,completions,out var selected,out var error)) return selected;
                Debug.LogWarning("[ModMode] Result callback failed; using default progression. "+error);
                return null;
            };
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

        public static void RecordSaveContext(System.Xml.XmlNode warrior, Roster roster = null)
        {
            StoryEvents.UnbindProfile();
            _profileRoster = null;
            DojoSelection.Unbind();
            // Do not overwrite provenance if mod initialization itself was unavailable.
            if (_scripts == null) return;
            if (!ModSaveData.RecordContext(warrior, _scripts.ActiveMods, _scripts.Content, _scripts.State))
            {
                Debug.LogWarning("[ModSave] Unrecognized save metadata schema; leaving it unchanged.");
                return;
            }
            ModModeRuntime.Bind(warrior);
            try { DojoSelection.Bind(warrior); }
            catch (ModContentException error) { Debug.LogWarning("[ModDojo] " + error.Message); }
            IReadOnlyList<ModDiagnostic> stateDiagnostics = _scripts.BindState(warrior);
            for (int i = 0; i < stateDiagnostics.Count; i++)
                Debug.LogWarning("[ModSave] " + stateDiagnostics[i]);
            _profileRoster = roster;
            if (roster != null) StoryEvents.BindProfile();
        }

        public static void UnbindProfile()
        {
            StoryEvents.UnbindProfile();
            _profileRoster = null;
            DojoSelection.Unbind();
            ModModeRuntime.Clear();
            _scripts?.State.Unbind();
        }

        private static int? ReadProfileLevel() => _profileRoster == null ? (int?)null : _profileRoster.Level;

        internal static bool TryNavigateScene(string destination)
        {
            ScreenType target;
            switch (destination)
            {
                case "map": target = ScreenType.ModuleMap; break;
                case "shop": target = ScreenType.ModuleShop; break;
                case "profile": target = ScreenType.ModuleProfile; break;
                case "dojo": target = ScreenType.ModuleDojo; break;
                default: throw new ModContentException("Unsupported menu destination: " + destination);
            }
            if (_profileRoster == null || _sceneNavigationInProgress || ModModeRuntime.HasPendingPreparation ||
                Eclipse.UI.Modding.ModUiGameBridge.NativeInputBlocked) return false;
            var lockScreen = Nekki.SF2.GUI.LockScreen.get_Instance();
            if (lockScreen != null && lockScreen.gameObject.activeInHierarchy) return false;
            var module = Module.ELEBLBJKDBI();
            var current = SceneManagerSF.EKFBDMBCDMB();
            // A combat exit must go through the native surrender/result workflow.
            if (current != ScreenType.ModuleMap && current != ScreenType.ModuleShop &&
                current != ScreenType.ModuleProfile && current != ScreenType.ModuleDojo) return false;
            if (module.BOHBCFMJPCA() == null || module.NMCNDOPKFJD() != current ||
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != (int)current) return false;
            if (current == target) return true;
            _sceneNavigationInProgress = true;
            try
            {
                // Keep the native quest/tab gates enabled. False can mean a native
                // quest consumed the request; do not force a second transition.
                return Module.DLOKJOHNDID(target);
            }
            finally { _sceneNavigationInProgress = false; }
        }

        internal static void PublishSceneEntry(string scene, int profileGeneration)
        {
            if (_profileRoster == null || StoryEvents.ProfileGeneration != profileGeneration ||
                !StoryEvents.HasSubscribers(ModStoryEventKind.SceneEnter)) return;
            StoryEvents.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter, null, scene: scene));
        }

        internal static void PublishLevelUp(Roster roster, int previousLevel, int profileGeneration)
        {
            if (roster == null || !ReferenceEquals(roster, _profileRoster) || previousLevel < 1 ||
                StoryEvents.ProfileGeneration != profileGeneration || !StoryEvents.HasSubscribers(ModStoryEventKind.LevelUp)) return;
            int level = roster.Level;
            if (level > previousLevel)
                StoryEvents.Publish(new ModStoryEvent(ModStoryEventKind.LevelUp, null, null, previousLevel, level));
        }

        internal static ModStoryEvent CaptureStoryEvent(QuestEvent.PMDPDMFLCIJ kind, QuestParameters parameters)
        {
            if (_profileRoster == null || _scripts == null || parameters == null) return null;
            ModStoryEventKind eventKind;
            if (kind == QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_PURCHASE) eventKind = ModStoryEventKind.Purchase;
            else if (kind == QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_ENCHANTMENT) eventKind = ModStoryEventKind.Enchantment;
            else return null;
            if (!StoryEvents.HasSubscribers(eventKind)) return null;
            try
            {
                string name = eventKind == ModStoryEventKind.Purchase
                    ? parameters.DLKPBAJDHBO?.Name : parameters.DPLEGFCHOCE?.OHCGEEEKEJH;
                string xml = eventKind == ModStoryEventKind.Purchase ? parameters.DLKPBAJDHBO?.NodeXML?.OuterXml : null;
                DefinitionId? item = _scripts.Content.TryResolveRuntimeItem(name, xml, out var itemId) ? itemId : (DefinitionId?)null;
                DefinitionId? recipe = null;
                if (eventKind == ModStoryEventKind.Enchantment)
                {
                    string recipeName = parameters.DPLEGFCHOCE?.FHELNNCGCGC;
                    if (DefinitionId.TryParse(recipeName, out var recipeId) && _scripts.Content.TryGetForgeRecipeFamily(recipeId, out var family))
                        recipe = family.Id;
                    else
                        foreach (var profile in _scripts.Content.ForgeEconomicProfiles)
                            if (string.Equals(profile.RuntimeRecipeName, recipeName, StringComparison.Ordinal)) { recipe = profile.Id; break; }
                }
                return new ModStoryEvent(eventKind, item, recipe);
            }
            catch (Exception error)
            {
                Debug.LogWarning("[ModStory] Unable to capture native notification: " + error.Message);
                return null;
            }
        }

        internal static void PublishStoryEvent(ModStoryEvent notification, int profileGeneration)
        {
            if (notification != null && StoryEvents.ProfileGeneration == profileGeneration)
                StoryEvents.Publish(notification);
        }

        private static ModProfileItemSnapshot ReadProfileItem(DefinitionId id)
        {
            if (_profileRoster == null || _scripts == null) return null;
            if (!_scripts.Content.TryResolveItem(id, out var definition))
                throw new ModContentException("Profile query references unavailable item: " + id);
            string name = definition.IsCore && !string.IsNullOrEmpty(definition.LegacyName)
                ? definition.LegacyName : definition.Id.ToString();
            var item = _profileRoster.KHCNHPCPFII().CMGOCLGHNLH(name);
            return item == null ? new ModProfileItemSnapshot(false, 0, false, null)
                : new ModProfileItemSnapshot(true, item.Count, item.EFMFGEPDAOP(), item.DHNNCAEEMLL());
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

        public static void DispatchBattleRules(ModBattleRuleInstances instances, string runtimeFightId,
            bool player, int round, bool eclipse, string fightInstanceId, string playerResult,
            ModEffectEvent effectEvent, IModFighterOperations fighter)
        {
            if (_scripts == null || fighter == null) return;
            foreach (var rule in instances.Applicable(_scripts.Content, runtimeFightId, player, round, eclipse))
            {
                if (!_scripts.HasBehaviorHandler(rule.Behavior, effectEvent)) continue;
                try
                {
                    var context = new Dictionary<string, string>
                    {
                        { "side", player ? "player" : "opponent" }, { "source", "rule" },
                        { "rule_id", rule.Id.ToString() }, { "fight_id", fightInstanceId },
                        { "round", round.ToString(System.Globalization.CultureInfo.InvariantCulture) },
                        { "player_result", playerResult ?? string.Empty }
                    };
                    var instanceFighter = new ModInstanceFighter(fighter, instances.Instance(rule.Id, player));
                    if (!_scripts.TryInvokeBehavior(rule.Behavior, effectEvent, rule.InitialParameters, context, instanceFighter, out var error))
                        UnityEngine.Debug.LogWarning("[ModCombat] " + effectEvent + " failed for rule " + rule.Id + ": " + error);
                }
                catch (Exception exception)
                {
                    UnityEngine.Debug.LogWarning("[ModCombat] Rule " + rule.Id + " failed: " + exception.Message);
                }
            }
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
            try
            {
                var values = perk.ResolveSavedUpgradeParameters(perkNode, instance.Values);
                return _scripts.TryInvokeBehavior(perk.Behavior, effectEvent,
                    values, fighterContext, new ModInstanceFighter(fighter, perkNode), out error);
            }
            catch (ModContentException exception) { error = exception.Message; return false; }
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
            StoryEvents.Clear();
            _profileRoster = null;
            ModProfileAccess.Clear();
            DojoSelection.Clear();
            ModModeRuntime.Clear();
            ModModeRuntime.SelectNext = null;
            ModProgressionAccess.Clear();
            ModModeRuntime.Prepare = null; ModModeRuntime.BuildEncounter = null; ModModeRuntime.SchedulePreparation = null;
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
            CoreContentImporter.ImportQuestSources(content, GameplayContentArchive.GetXmlRoot());
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

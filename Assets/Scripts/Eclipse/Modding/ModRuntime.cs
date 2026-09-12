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
        private static XmlNode _lotteryProfileNode;
        private static int _lotterySaveState; // 0 idle, 1 settling, 2 failed: reload before saving.
        private static ModQuestLotteryAction _battleLotteryPresentation;
        internal static bool HasPendingLottery => _lotterySaveState != 0 || _lotteryProfileNode?["EclipseLotteryClaim"] is XmlElement saved &&
            (saved.GetAttribute("State") == "prepared" || (saved["BattleEnd"] != null && saved["BattleEnd"].GetAttribute("Dispatched") != "1"));

        internal static bool DeferProfileSave()
        {
            if (_lotterySaveState == 2)
                throw new InvalidOperationException("Lottery settlement failed. Reload the profile before saving further changes.");
            return _lotterySaveState == 1;
        }

        internal static bool IsProfileSnapshotPath(string path)
        {
            var comparison = Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            string full = Path.GetFullPath(path);
            string directory = SF2Paths.APHDBIBDMDG();
            return string.Equals(full, Path.GetFullPath(Path.Combine(directory, Constants.OJMIJINKBPJ)), comparison) ||
                string.Equals(full, Path.GetFullPath(Path.Combine(directory, Constants.GHKPPHAAMBL)), comparison);
        }

        internal static bool TryWriteProfileSnapshot(XmlDocument document, string path)
        {
            if (!IsProfileSnapshotPath(path)) return false;
            byte[] snapshot;
            using (var stream = new MemoryStream())
            {
                document.Save(stream);
                snapshot = stream.ToArray();
            }
            byte[] hash = GameSettings.HCAJHNKLLGB() ? System.Text.Encoding.UTF8.GetBytes(
                MD5Utils.INPENHNJBGJ(document.OuterXml, UserDataValidator.GHEHGBBNMNO())) : null;
            ModProfileWriteJournal.Write(path, snapshot, hash, ValidateProfileSnapshot);
            return true;
        }

        internal static void RecoverProfileSnapshot(string path)
        {
            if (IsProfileSnapshotPath(path) && File.Exists(path + ".eclipse-write"))
                ModProfileWriteJournal.Recover(path, ValidateProfileSnapshot);
        }

        private static void ValidateProfileSnapshot(byte[] snapshot, byte[] hash)
        {
            var document = new XmlDocument { XmlResolver = null };
            using (var stream = new MemoryStream(snapshot, false)) document.Load(stream);
            UserDataValidator.CheckSnapshotHash(document,
                hash == null ? null : System.Text.Encoding.UTF8.GetString(hash), "profile write journal");
        }
        public static string ResolveDojoLocation(string fallback) => DojoSelection.Resolve(fallback);

        public static bool IsInitialized => _host != null;
        public static ModHost Host => _host ?? InitializeDefault();
        public static ModScriptSession Scripts => _scripts;

        public static ModScriptSession StartScripts()
        {
            StoryEvents.Clear();
            _profileRoster = null;
            ModProfileAccess.Clear();
            ModSceneAccess.Clear();
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
            ModProfileAccess.Perk = ReadProfilePerk;
            ModProfileAccess.Equipment = ReadProfileEquipment;
            ModSceneAccess.Open = TryNavigateScene;
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
            if (_lotterySaveState == 1) throw new InvalidOperationException("Cannot replace the profile during lottery settlement.");
            _battleLotteryPresentation?.Dispose();
            _battleLotteryPresentation = null;
            _lotteryProfileNode = warrior;
            _lotterySaveState = 0;
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
            if (_lotterySaveState == 1) throw new InvalidOperationException("Cannot unload the profile during lottery settlement.");
            _battleLotteryPresentation?.Dispose();
            _battleLotteryPresentation = null;
            _lotteryProfileNode = null;
            // Keep a failed settlement's save gate closed until a new profile binds.
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

        // Host-only selection. Eligibility policy and random sampling belong to
        // the caller; this neither evaluates unselected rewards nor grants loot.
        internal static bool TrySelectLotterySlot(RewardLottery lottery, int level, double sample,
            Func<MANJCIGJPMK, bool> eligible, out MANJCIGJPMK selected)
        {
            if(lottery==null)throw new ArgumentNullException(nameof(lottery));
            if(level<1)throw new ArgumentOutOfRangeException(nameof(level));
            if(double.IsNaN(sample)||sample<0||sample>=1)throw new ArgumentOutOfRangeException(nameof(sample));
            selected=default(MANJCIGJPMK);
            var candidates=new List<MANJCIGJPMK>();
            double total=0;
            foreach(var slot in lottery.EDCOGMLOEHE.ToArray())
            {
                if(float.IsNaN(slot.Weight)||float.IsInfinity(slot.Weight)||slot.Weight<0)
                    throw new InvalidOperationException("Lottery slot weights must be finite and nonnegative.");
                if(slot.Weight==0||!slot.IsAvailableAtLevel(level)||(eligible!=null&&!eligible(slot)))continue;
                candidates.Add(slot);total+=slot.Weight;
            }
            if(candidates.Count==0)return false;
            double cursor=sample*total;
            foreach(var slot in candidates)
            {
                if(cursor<slot.Weight){selected=slot;return true;}
                cursor-=slot.Weight;
            }
            // Multiplication/subtraction can round at the upper boundary.
            selected=candidates[candidates.Count-1];return true;
        }

        internal sealed class LotteryClaim
        {
            internal string PreviewImage => _saved?.GetAttribute("Image");
            internal string PreviewItem => _saved?["Prize"]?["Item"]?.GetAttribute("Name");
            internal bool IsCurrent => _owner != null && ReferenceEquals(_owner, _profileRoster) &&
                _generation == StoryEvents.ProfileGeneration;
            internal string PreviewText
            {
                get
                {
                    var prize = _saved?["Prize"];
                    if (prize == null) return LocalizationManager.GetStringOrDefault("ClanRewardTxt", "Reward");
                    var lines = new List<string>();
                    foreach (string attributeName in new[] { "Money", "Bonus", "Experience" })
                    {
                        string value = prize.GetAttribute(attributeName);
                        string label = attributeName == "Money" ? "Coins" : attributeName == "Bonus" ? "Gems" : "Experience";
                        if (!string.IsNullOrEmpty(value) && value != "0") lines.Add(
                            LocalizationManager.GetStringOrDefault("eclipse.lottery." + attributeName, label) + ": " + value);
                    }
                    foreach (XmlNode child in prize.ChildNodes)
                    {
                        if (!(child is XmlElement item)) continue;
                        string name = item.GetAttribute("Name");
                        string displayName = LocalizationManager.GetStringOrDefault(name, name);
                        if (item.Name == "Item") lines.Add(displayName + LocalizationManager.GetStringOrDefault(
                            "StoryMenuLevel", " (lvl " + item.GetAttribute("Level") + ")", item.GetAttribute("Level")));
                        else if (item.Name == "Currency") lines.Add(displayName + " × " + item.GetAttribute("Count"));
                    }
                    return lines.Count == 0 ? LocalizationManager.GetStringOrDefault("eclipse.lottery.empty", "No additional items") : string.Join("\n", lines);
                }
            }
            private readonly Roster _owner;
            private readonly int _generation;
            private readonly FightResult.ResultPrizeStruct _prize;
            private bool _consumed;
            private readonly XmlElement _saved;
            internal LotteryClaim(Roster owner, int generation, FightResult.ResultPrizeStruct prize, XmlElement saved = null)
            { _owner=owner; _generation=generation; _prize=prize ?? throw new ArgumentNullException(nameof(prize)); _saved=saved; }

            internal bool TryClaim()
            {
                if(_consumed || _owner==null || !ReferenceEquals(_owner,_profileRoster) ||
                    _generation!=StoryEvents.ProfileGeneration)return false;
                if(_prize.FAPDEKOMOGH!=null)
                    throw new InvalidOperationException("Nested lottery rewards must be resolved before claiming.");
                if (_prize.KBMDJACLAOH.Count != 0)
                    throw new NotSupportedException("Native resistance reward granting is not implemented.");
                if (_saved != null && (_saved.ParentNode != _lotteryProfileNode || _saved.GetAttribute("State") != "prepared")) return false;
                if (_lotterySaveState != 0) throw new InvalidOperationException("Another lottery settlement is active or requires profile reload.");
                var acknowledge = ResolveLotteryAcknowledgement(_saved);
                // Consume before callbacks. Exceptions may follow partial native
                // mutation, so this live claim must never be retried automatically.
                _consumed=true;
                StoryEvents.RunDeferred(()=>{
                    _lotterySaveState = 1;
                    try
                    {
                        ListSF.ELEBLBJKDBI().IMDGMNFHFCN(_prize);
                        // IMDGMNFHFCN's return value indicates level-up, not grant success.
                        if(!ReferenceEquals(_owner,_profileRoster) || _generation!=StoryEvents.ProfileGeneration)
                            throw new InvalidOperationException("Profile changed during lottery grant; the consumed claim cannot be retried.");
                        _owner.GGGEHAGCLGC(true);
                        acknowledge?.Invoke();
                        if (_saved != null) _saved.SetAttribute("State", "claimed");
                        _lotterySaveState = 0;
                        if (_saved != null) ListSF.ELEBLBJKDBI().OnAuthenticate(true);
                    }
                    catch
                    {
                        // Disk retains the prepared snapshot or a recoverable committed
                        // snapshot. Do not later autosave partial in-memory mutations.
                        _lotterySaveState = 2;
                        throw;
                    }
                });
                return true;
            }
        }

        internal static void PrepareBattleLottery(RewardLottery lottery, QuestParameters context, bool raid, string encounterId)
        {
            if (_lotteryProfileNode == null || _profileRoster == null) throw new InvalidOperationException("Battle lottery requires an active profile.");
            var previous = _lotteryProfileNode["EclipseLotteryClaim"]?["BattleEnd"];
            if (previous != null && previous.GetAttribute("Encounter") == encounterId) return;
            if (HasPendingLottery) throw new InvalidOperationException("Claim the outstanding reward before another battle lottery.");
            var continuation = _lotteryProfileNode.OwnerDocument.CreateElement("BattleEnd");
            continuation.SetAttribute("Format", "1");
            continuation.SetAttribute("Encounter", encounterId ?? Guid.NewGuid().ToString("N"));
            continuation.SetAttribute("Fight", context.JLGLBLDPAAF.ToString());
            continuation.SetAttribute("Raid", raid ? "1" : "0");
            continuation.SetAttribute("RaidId", context.OHPHPJBMNLH ?? string.Empty);
            continuation.SetAttribute("LevelUp", context.BJIDALJIKNC.ToString(System.Globalization.CultureInfo.InvariantCulture));
            continuation.SetAttribute("AverageFps", context.fightAvgFps.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
            continuation.SetAttribute("Dispatched", "0");
            var claim = PrepareLotteryClaim(lottery, Math.Min(UnityEngine.Random.value, 0.9999999999999999), null, battleEnd: continuation);
            if (claim == null) throw new InvalidOperationException("Awarded lottery has no eligible reward.");
        }

        internal static void ShowPendingBattleLottery()
        {
            var saved = _lotteryProfileNode?["EclipseLotteryClaim"];
            if (_profileRoster == null || saved?["BattleEnd"] == null || saved["BattleEnd"].GetAttribute("Dispatched") == "1") return;
            if (Module.ELEBLBJKDBI().NMCNDOPKFJD() == ScreenType.ModuleFight) return;
            try
            {
                if (saved.GetAttribute("State") == "claimed") { CompleteBattleLottery(); return; }
                if (_battleLotteryPresentation == null)
                    _battleLotteryPresentation = new ModQuestLotteryAction(ResumeLotteryClaim(), CompleteBattleLottery);
                _battleLotteryPresentation.Show();
            }
            catch (Exception error) { Debug.LogException(error); }
        }

        internal static void CompleteBattleLottery()
        {
            var saved = _lotteryProfileNode?["EclipseLotteryClaim"];
            var continuation = saved?["BattleEnd"];
            if (continuation == null || continuation.GetAttribute("Dispatched") == "1") return;
            if (_profileRoster == null || _lotterySaveState != 0 || saved.GetAttribute("State") != "claimed")
                throw new InvalidOperationException("Battle lottery must be claimed before its fight-end event.");
            if (continuation.GetAttribute("Format") != "1" ||
                (continuation.GetAttribute("Raid") != "0" && continuation.GetAttribute("Raid") != "1") ||
                !int.TryParse(continuation.GetAttribute("LevelUp"), out int levelUp) ||
                !float.TryParse(continuation.GetAttribute("AverageFps"), System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out float averageFps) || float.IsNaN(averageFps) || float.IsInfinity(averageFps))
                throw new InvalidDataException("Invalid saved battle lottery continuation.");
            var id = new FightIDS(); id.SetFightIDSByString(continuation.GetAttribute("Fight"));
            var context = new QuestParameters { JLGLBLDPAAF = id, HEIADONEACH = "Win", BJIDALJIKNC = levelUp,
                fightAvgFps = averageFps, OHPHPJBMNLH = continuation.GetAttribute("RaidId"), inLottery = true,
                FOODLENBJGI = saved["Prize"]?["Item"]?.GetAttribute("Name") ?? string.Empty };
            bool queued = false;
            StoryEvents.RunDeferred(() => {
                _lotterySaveState = 1;
                try
                {
                    queued = ListSF.ELEBLBJKDBI().QueueLotteryFightEnd(context, continuation.GetAttribute("Raid") == "1");
                    continuation.SetAttribute("Dispatched", "1");
                    _lotterySaveState = 0;
                    ListSF.ELEBLBJKDBI().OnAuthenticate(true);
                }
                catch { _lotterySaveState = 2; throw; }
            });
            ListSF.ELEBLBJKDBI().HAOHNNFLOGK = new QuestParameters();
            _battleLotteryPresentation?.Dispose();
            _battleLotteryPresentation = null;
            if (queued) ListSF.ELEBLBJKDBI().MHHNIPBJNAD();
        }

        internal static void SaveQuestLotteryContext(ParametersQuest saved, QuestParameters context)
        {
            var parent = saved.Node;
            var previous = parent["EclipseLotteryContext"];
            if (!context.inLottery && context.EGAPDJLHHNJ == 0 && string.IsNullOrEmpty(context.FOODLENBJGI) && string.IsNullOrEmpty(context.OHPHPJBMNLH))
            {
                if (previous != null) parent.RemoveChild(previous);
                return;
            }
            var node = parent.OwnerDocument.CreateElement("EclipseLotteryContext");
            node.SetAttribute("Format", "1");
            node.SetAttribute("InLottery", context.inLottery ? "1" : "0");
            node.SetAttribute("Spin", context.EGAPDJLHHNJ.ToString(System.Globalization.CultureInfo.InvariantCulture));
            node.SetAttribute("Item", context.FOODLENBJGI ?? string.Empty);
            node.SetAttribute("Raid", context.OHPHPJBMNLH ?? string.Empty);
            if (previous != null) parent.ReplaceChild(node, previous);
            else parent.AppendChild(node);
        }

        internal static void RestoreQuestLotteryContext(ParametersQuest saved, QuestParameters context)
        {
            var node = saved.Node["EclipseLotteryContext"];
            if (node == null) return;
            string inLottery = node.GetAttribute("InLottery");
            if (node.GetAttribute("Format") != "1" || (inLottery != "0" && inLottery != "1") ||
                !int.TryParse(node.GetAttribute("Spin"), System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.InvariantCulture, out int spin))
                throw new InvalidDataException("Invalid saved quest lottery context.");
            context.inLottery = inLottery == "1";
            context.EGAPDJLHHNJ = spin;
            context.FOODLENBJGI = node.GetAttribute("Item");
            context.OHPHPJBMNLH = node.GetAttribute("Raid");
        }

        internal static AssetId? ResolveLotteryArtwork(LotteryClaim claim)
        {
            if (claim == null || !IsInitialized) return null;
            foreach (string name in new[] { claim.PreviewImage, claim.PreviewItem })
            {
                if (string.IsNullOrEmpty(name)) continue;
                string reference = name;
                if (name.IndexOf(':') < 0)
                {
                    var item = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(name);
                    reference = item?.FileName ?? name;
                    if (reference.IndexOf(':') < 0)
                        reference = "core:" + (item?.Type == "Seal" ? SF2Paths.BHCPOOOJAAK() : SF2Paths.LFIIMPEAMFG()) + reference;
                }
                if (!AssetId.TryParse(reference, out var id)) continue;
                try
                {
                    if (Host.TypedAssets.LoadSprite(id) != null) return id;
                }
                catch (Exception error) when (error is System.IO.InvalidDataException || error is System.IO.IOException || error is UnauthorizedAccessException)
                {
                    Debug.LogWarning("[ModLottery] Reward artwork unavailable: " + error.Message);
                }
            }
            // Artwork is optional; a missing icon must not make a saved reward
            // impossible to claim. Keep its readable reward summary instead.
            return null;
        }

        internal static LotteryClaim PrepareQuestLotteryClaim(QuestStage stage, int actionIndex, string fightName, double sample)
        {
            var ledger = GetQuestLotteryInvocation(stage);
            if (ledger.IsCompleted(actionIndex)) return null;
            if (ResumeLotteryClaim() != null)
                return PrepareLotteryClaim(null, sample, null, ledger, actionIndex);
            var id = new FightIDS();
            id.SetFightIDSByString(fightName);
            var fight = ListSF.CHMCKGCDGCM(id);
            if (fight == null) throw new InvalidOperationException("Lottery fight was not found: " + fightName);
            RewardLottery lottery = null;
            foreach (var reward in fight.APKPCGDBMEP())
            {
                var candidate = reward.KOBOIFJNPMO(_profileRoster.PINDEKDNCNL()).FAPDEKOMOGH;
                if (candidate == null) continue;
                if (lottery != null) throw new InvalidOperationException("Lottery action requires exactly one lottery reward.");
                lottery = candidate;
            }
            if (lottery == null) throw new InvalidOperationException("Fight has no lottery reward: " + fightName);
            return PrepareLotteryClaim(lottery, sample, null, ledger, actionIndex)
                ?? throw new InvalidOperationException("Lottery has no eligible reward at this level.");
        }

        internal static ModQuestInvocationLedger GetQuestLotteryInvocation(QuestStage stage)
        {
            if (stage == null) throw new NotSupportedException("Lottery actions require a top-level quest stage.");
            if (_profileRoster == null || _lotteryProfileNode == null || _lotterySaveState != 0)
                throw new InvalidOperationException("An active, writable profile is required for quest rewards.");
            if (stage.allowDoubles) throw new NotSupportedException("Concurrent runs of a lottery quest require separate invocation identities.");
            if (stage.EclipseLotteryInvocations != null)
            {
                if (!stage.EclipseLotteryInvocations.BelongsTo(_lotteryProfileNode))
                    throw new InvalidOperationException("Quest lottery belongs to another profile.");
                return stage.EclipseLotteryInvocations;
            }
            var parent = _lotteryProfileNode["EclipseQuestClaims"];
            if (parent == null)
            {
                parent = _lotteryProfileNode.OwnerDocument.CreateElement("EclipseQuestClaims");
                _lotteryProfileNode.AppendChild(parent);
            }
            XmlElement quest = null;
            foreach (XmlNode child in parent.ChildNodes)
            {
                if (!(child is XmlElement entry)) continue;
                if (entry.Name != "Quest") throw new InvalidDataException("Invalid quest claim entry.");
                if (entry.GetAttribute("File") != stage.FileName || entry.GetAttribute("Name") != stage.get_Name()) continue;
                if (quest != null) throw new InvalidDataException("Duplicate quest claim identity.");
                quest = entry;
            }
            string definition;
            using (var hash = System.Security.Cryptography.SHA256.Create())
                definition = Convert.ToBase64String(hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(stage.EclipseActionsDefinition)));
            if (quest == null)
            {
                quest = parent.OwnerDocument.CreateElement("Quest");
                quest.SetAttribute("File", stage.FileName);
                quest.SetAttribute("Name", stage.get_Name());
                parent.AppendChild(quest);
            }
            string state = quest.GetAttribute("State");
            if (state != string.Empty && state != "active" && state != "completed")
                throw new InvalidDataException("Invalid quest claim run state.");
            bool resume = stage.EclipseResumeActions || state == "active";
            if (resume && quest["EclipseInvocations"] != null && quest.GetAttribute("Definition") != definition)
                throw new InvalidOperationException("The pending quest definition changed; restore it before resuming its lottery.");
            quest.SetAttribute("Definition", definition);
            quest.SetAttribute("State", "active");
            return stage.EclipseLotteryInvocations = new ModQuestInvocationLedger(quest, resume);
        }

        internal static bool CompleteQuestLotteryRun(QuestStage stage)
        {
            var ledger = stage.EclipseLotteryInvocations;
            if (ledger == null) return false;
            if (_lotterySaveState != 0 || !ledger.BelongsTo(_lotteryProfileNode))
                throw new InvalidOperationException("Quest lottery cannot finish in this profile state.");
            ledger.CloseRun();
            return true;
        }

        internal static LotteryClaim PrepareLotteryClaim(RewardLottery lottery, double sample,
            Func<MANJCIGJPMK,bool> eligible, ModQuestInvocationLedger invocation = null, int actionIndex = 0, XmlElement battleEnd = null)
        {
            var owner=_profileRoster;
            if(owner==null)throw new InvalidOperationException("No active game profile is available.");
            if (_lotteryProfileNode == null) throw new InvalidOperationException("No profile save node is available.");
            if (_lotterySaveState != 0) throw new InvalidOperationException("Reload the profile before preparing another lottery.");
            if (invocation != null && !invocation.BelongsTo(_lotteryProfileNode))
                throw new InvalidOperationException("Quest invocation belongs to another profile.");
            if (invocation != null && invocation.IsCompleted(actionIndex)) return null;
            string invocationKey = invocation?.Operation(actionIndex) ?? string.Empty;
            if (_lotteryProfileNode["EclipseLotteryClaim"] is XmlElement prior && prior.GetAttribute("State") == "claimed" &&
                prior["BattleEnd"] != null && prior["BattleEnd"].GetAttribute("Dispatched") != "1")
                throw new InvalidOperationException("Finish the deferred battle event before another draw.");
            var pending = ResumeLotteryClaim();
            if (pending != null)
            {
                if (_lotteryProfileNode["EclipseLotteryClaim"].GetAttribute("Invocation") != invocationKey ||
                    _lotteryProfileNode["EclipseLotteryClaim"]["BattleEnd"]?.GetAttribute("Encounter") != battleEnd?.GetAttribute("Encounter"))
                    throw new InvalidOperationException("Finish the pending lottery before starting another quest claim.");
                ListSF.ELEBLBJKDBI().OnAuthenticate(true);
                return pending;
            }
            int generation=StoryEvents.ProfileGeneration;
            int level=owner.PINDEKDNCNL();
            if(!TrySelectLotterySlot(lottery,level,sample,eligible,out var selected))return null;
            var prize=BuildLotteryPrize(selected,level);
            if (prize.KBMDJACLAOH.Count != 0)
                throw new NotSupportedException("Native resistance reward granting is not implemented.");
            if(!ReferenceEquals(owner,_profileRoster)||generation!=StoryEvents.ProfileGeneration)
                throw new InvalidOperationException("Profile changed while preparing lottery rewards.");
            var saved = _lotteryProfileNode.OwnerDocument.CreateElement("EclipseLotteryClaim");
            saved.SetAttribute("Format", "1");
            saved.SetAttribute("State", "prepared");
            saved.SetAttribute("Id", Guid.NewGuid().ToString("N"));
            saved.SetAttribute("Invocation", invocationKey);
            saved.SetAttribute("Image", selected.Image ?? string.Empty);
            saved.SetAttribute("ViewType", selected.ViewType ?? string.Empty);
            saved.AppendChild(ModLotteryPrizeCodec.Write(saved.OwnerDocument, prize));
            if (battleEnd != null) saved.AppendChild(saved.OwnerDocument.ImportNode(battleEnd, true));
            invocation?.BindClaim(actionIndex, saved.GetAttribute("Id"));
            var previous = _lotteryProfileNode["EclipseLotteryClaim"];
            if (previous != null) _lotteryProfileNode.ReplaceChild(saved, previous);
            else _lotteryProfileNode.AppendChild(saved);
            ListSF.ELEBLBJKDBI().OnAuthenticate(true);
            return new LotteryClaim(owner,generation,prize,saved);
        }

        private static Action ResolveLotteryAcknowledgement(XmlElement saved)
        {
            string operation = saved?.GetAttribute("Invocation");
            if (string.IsNullOrEmpty(operation)) return null;
            string[] parts = operation.Split('/');
            if (parts.Length != 2 || !Guid.TryParseExact(parts[0], "N", out _) ||
                !int.TryParse(parts[1], out int index) || index < 0)
                throw new InvalidDataException("Invalid lottery quest invocation.");
            foreach (XmlNode node in _lotteryProfileNode.SelectNodes(".//EclipseInvocations"))
            {
                if (node.Attributes?["Id"]?.Value != parts[0]) continue;
                var ledger = new ModQuestInvocationLedger((XmlElement)node.ParentNode, true);
                if (ledger.IsCompleted(index)) throw new InvalidDataException("Lottery invocation was already acknowledged.");
                ledger.BindClaim(index, saved.GetAttribute("Id"));
                return () => ledger.Complete(index, saved.GetAttribute("Id"));
            }
            throw new InvalidDataException("Lottery quest invocation is unavailable.");
        }

        internal static LotteryClaim ResumeLotteryClaim()
        {
            var saved = _lotteryProfileNode?["EclipseLotteryClaim"];
            if (saved == null) return null;
            if (_profileRoster == null || _lotterySaveState != 0) throw new InvalidOperationException("Profile is unavailable for lottery recovery.");
            if (saved.GetAttribute("Format") != "1") throw new InvalidDataException("Unknown saved lottery claim format.");
            if (saved.GetAttribute("State") == "claimed") return null;
            if (saved.GetAttribute("State") != "prepared") throw new InvalidDataException("Invalid saved lottery claim state.");
            var prize = ModLotteryPrizeCodec.Read(saved["Prize"], (name, level, upgrade) => {
                var item = ListSF.DJBOFEEKJMP().KCCDBEEKBCG(name);
                if (item == null) return null;
                return item.MHGODOLNDLE == level && item.OBJDGBBFJOO == upgrade ? item : item.HIOBANJPMKF(upgrade);
            }, name => GameUtils.AJDKHINLIDI.ICFINJLNCPM(name), name => GameUtils.JNIMKHKGPHE.NDMEGBEFBPJ(name));
            return new LotteryClaim(_profileRoster, StoryEvents.ProfileGeneration, prize, saved);
        }

        internal static FightResult.ResultPrizeStruct BuildLotteryPrize(MANJCIGJPMK slot, int level)
        {
            if(!slot.TryEvaluateAtLevel(level,out var prize))
                throw new InvalidOperationException("Lottery slot is unavailable at the selected level.");
            var result=new FightResult.ResultPrizeStruct {
                GBGNFPNCGED=(long)prize.GBGNFPNCGED,
                PNDAIFALIKF=(long)prize.PNDAIFALIKF,
                exp=(uint)prize.exp
            };
            foreach(var item in prize.MDJFGLELOBA)result.KFJABAMAKOD(item);
            foreach(var item in prize.KIMJGOHCCPO)result.KFJABAMAKOD(item);
            foreach(var item in prize.KBMDJACLAOH)result.KFJABAMAKOD(item);
            if(prize.FAPDEKOMOGH!=null)result.KFJABAMAKOD(prize.FAPDEKOMOGH);
            foreach(var item in prize.HELFDCAIJNE)result.KFJABAMAKOD(item);
            foreach(var choice in prize.PNFMKMLLFHK)result.KFJABAMAKOD(choice.OOOBLJIHBEP());
            return result;
        }

        internal static ModStoryEvent CaptureBattleResult(Roster roster, FightList fight, GameOverTypes outcome,
            ModelParameters first, ModelParameters second)
        {
            if (roster == null || !ReferenceEquals(roster, _profileRoster) || _scripts == null ||
                !StoryEvents.HasSubscribers(ModStoryEventKind.BattleResult)) return null;
            string result;
            switch(outcome)
            {
                case GameOverTypes.GAME_OVER_WIN: result="win"; break;
                case GameOverTypes.GAME_OVER_LOSS: result="loss"; break;
                case GameOverTypes.GAME_OVER_SURRENDER: result="surrender"; break;
                case GameOverTypes.GAME_OVER_RAID_TIMEOUT: result="raid_timeout"; break;
                case GameOverTypes.GAME_OVER_RAID_ROUND_TIMEOUT: result="raid_round_timeout"; break;
                default: return null;
            }
            DefinitionId? id=null;
            foreach(var definition in _scripts.Content.Fights)
                if(_scripts.Content.RuntimeFightId(definition.Id)==fight.BCKFACGMOKC.ToString()) { id=definition.Id; break; }
            var player=first!=null&&first.IsPlayer?first:second!=null&&second.IsPlayer?second:null;
            List<ModBattleEquipmentSnapshot> equipment=null;
            if(player!=null)
            {
                equipment=new List<ModBattleEquipmentSnapshot>();
                foreach(var item in player.PJNJIJIODHE())
                {
                    DefinitionId? itemId=_scripts.Content.TryResolveRuntimeItem(item.Name,item.NodeXML?.OuterXml,out var resolved)?resolved:(DefinitionId?)null;
                    equipment.Add(new ModBattleEquipmentSnapshot(itemId,item.Type,item.MDPPNGIEJGD));
                }
            }
            return new ModStoryEvent(ModStoryEventKind.BattleResult,null,
                battle:new ModBattleResultSnapshot(id,result,roster.JPMPIDFGCJL(),equipment));
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

        internal static void PublishItemAcquired(Roster roster, ItemInfo item, int previousCount, int count, int generation)
        {
            if (roster == null || !ReferenceEquals(roster, _profileRoster) || _scripts == null || item == null ||
                previousCount < 0 || count <= previousCount || generation != StoryEvents.ProfileGeneration ||
                !StoryEvents.HasSubscribers(ModStoryEventKind.ItemAcquired)) return;
            DefinitionId? id = _scripts.Content.TryResolveRuntimeItem(item.Name, item.NodeXML?.OuterXml, out var resolved)
                ? resolved : (DefinitionId?)null;
            StoryEvents.Publish(new ModStoryEvent(ModStoryEventKind.ItemAcquired, id, previousCount: previousCount, count: count));
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

        private static ModProfilePerkSnapshot ReadProfilePerk(DefinitionId id)
        {
            if (_profileRoster == null || _scripts == null) return null;
            if (!_scripts.Content.TryGetPerk(id, out var definition))
                throw new ModContentException("Profile query references unavailable perk: " + id);
            string name = definition.IsCore && !string.IsNullOrEmpty(definition.LegacyName)
                ? definition.LegacyName : definition.Id.ToString();
            foreach (var perk in _profileRoster.JLBDOBLHHAF().KEHFPLBNDHI())
                if (string.Equals(perk.get_Name(), name, StringComparison.Ordinal))
                    return new ModProfilePerkSnapshot(true, perk.DHNNCAEEMLL());
            return new ModProfilePerkSnapshot(false, null);
        }

        private static IReadOnlyList<ModProfileEquipmentSnapshot> ReadProfileEquipment()
        {
            if (_profileRoster == null || _scripts == null) return null;
            var result = new List<ModProfileEquipmentSnapshot>();
            foreach (var item in _profileRoster.KHCNHPCPFII().JCMOHPFKPBO())
            {
                var metadata = item.BHKHOJPANHE();
                DefinitionId? id = _scripts.Content.TryResolveRuntimeItem(item.get_Name(), metadata?.NodeXML?.OuterXml, out var resolved)
                    ? resolved : (DefinitionId?)null;
                result.Add(new ModProfileEquipmentSnapshot(id,
                    new ModProfileItemSnapshot(true, item.Count, true, item.DHNNCAEEMLL(), metadata?.Type, metadata?.MDPPNGIEJGD)));
            }
            return result.AsReadOnly();
        }

        private static ModProfileItemSnapshot ReadProfileItem(DefinitionId id)
        {
            if (_profileRoster == null || _scripts == null) return null;
            if (!_scripts.Content.TryResolveItem(id, out var definition))
                throw new ModContentException("Profile query references unavailable item: " + id);
            string name = definition.IsCore && !string.IsNullOrEmpty(definition.LegacyName)
                ? definition.LegacyName : definition.Id.ToString();
            var item = _profileRoster.KHCNHPCPFII().CMGOCLGHNLH(name);
            var metadata = ListSF.DJBOFEEKJMP()?.KCCDBEEKBCG(name);
            return item == null ? new ModProfileItemSnapshot(false, 0, false, null, metadata?.Type, metadata?.MDPPNGIEJGD)
                : new ModProfileItemSnapshot(true, item.Count, item.EFMFGEPDAOP(), item.DHNNCAEEMLL(), metadata?.Type, metadata?.MDPPNGIEJGD);
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
            ModSceneAccess.Clear();
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

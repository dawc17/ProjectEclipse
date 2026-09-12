using System;
using System.Xml;

namespace Eclipse.Modding
{
    public static class ModModeRuntime
    {
        public static Action<string> Warning;
        public static Func<ModModeDefinition,bool,int,int,int?> SelectNext;
        public static Action<ModModeDefinition,int,int,ModModeRequest> Prepare;
        public static Func<ModModeDefinition,int,ModEncounterPlan,FightList> BuildEncounter;
        public static Action<ModModeRequest,Action,Action> SchedulePreparation;
        private static ModModeRequest _pending;
        internal static bool HasPendingPreparation => _pending != null;
        private static XmlNode _warrior;
        private static string _activeFight;
        private static bool _newReservation;
        private static bool _completedMode;
        private static bool _resetMode;
        private static FightResult _raidResult;
        private static bool _raidResultShown;
        public static void SetRaidResult(FightResult result) { _raidResult = result; _raidResultShown = false; }
        public static bool ShowRaidResult()
        {
            if (_raidResult == null || _raidResultShown) return false;
            var fight = Fight.OHNKFOHIAKG();
            if (fight == null) return false;
            _raidResultShown = true; fight.BCFBHJOLGNL(_raidResult); return true;
        }
        public static void Raise(QuestEvent.PMDPDMFLCIJ kind)
        {
            var list = ListSF.ELEBLBJKDBI();
            if (list != null && list.FFBAJNGHGGD(kind)) list.MHHNIPBJNAD();
        }
        public static void Bind(XmlNode warrior) { if (_warrior != warrior) Clear(); _warrior = warrior; }
        public static void Clear() { _pending?.Invalidate(); _pending = null; _warrior = null; _activeFight = null; _raidResult = null; _raidResultShown = false; _newReservation = false; _completedMode = false; _resetMode = false; }
        public static bool TryFind(string runtimeId, out ModModeDefinition mode)
        {
            var content = ModPolicies.Content;
            if (content != null) foreach (var candidate in content.Modes)
                foreach (var id in candidate.Fights)
                    if (content.RuntimeFightId(id) == runtimeId) { mode = candidate; return true; }
            mode = null; return false;
        }
        public static bool IsRaid(FightList fight) => fight != null && TryFind(fight.BCKFACGMOKC.ToString(), out var mode) && mode.Raid;
        public static bool IsRaid(string runtimeId) => TryFind(runtimeId, out var mode) && mode.Raid;
        public static bool HasCustomRouting(FightList fight) => fight != null && TryFind(fight.BCKFACGMOKC.ToString(),out var mode) && mode.UsesResultCallback;

        public static bool OwnsBattle(Battle battle)
        {
            var content = ModPolicies.Content;
            if (battle == null || content == null) return false;
            foreach (var mode in content.Modes) foreach (var id in mode.Fights)
                if (content.TryGetFight(id, out var fight) && content.TryGetBattle(fight.Battle, out var definition) && definition.LegacyName == battle.get_Name()) return true;
            return false;
        }
        public static bool TryCurrent(Battle battle, out FightList fight)
        {
            fight = null;
            var content = ModPolicies.Content;
            if (battle == null || content == null) return false;
            foreach (var mode in content.Modes)
                foreach (var id in mode.Fights)
                    if (content.TryGetFight(id, out var definition) && content.TryGetBattle(definition.Battle, out var entry) && entry.LegacyName == battle.get_Name())
                    {
                        try
                        {
                            var state = new ModModeProgress(_warrior, mode);
                            if (state.Step < mode.Fights.Count && mode.IsAvailable(ListSF.CCDKHLAMKKO().PINDEKDNCNL(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()))
                            {
                                fight = ListSF.CHMCKGCDGCM(new FightIDS(content.RuntimeFightId(mode.Fights[state.Step])));
                                var plan = state.ReadPlan();
                                if (plan != null) fight = BuildEncounter?.Invoke(mode,state.Step,plan) ?? throw new ModContentException("Saved encounter construction is unavailable.");
                            }
                        }
                        catch (Exception exception) { Reject(exception.Message); }
                        return true;
                    }
            return false;
        }

        public static string EntryStatus(FightList fight)
        {
            if (fight == null || !TryFind(fight.BCKFACGMOKC.ToString(), out var mode)) return "Unavailable";
            try
            {
                var state = new ModModeProgress(_warrior, mode);
                if (mode.HasEntryItem && !state.Entered)
                {
                    var item = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(mode.EntryItem.ToString());
                    int owned = item?.Count ?? 0;
                    if (owned < mode.EntryCount)
                    {
                        ModPolicies.Content.TryGetItem(mode.EntryItem, out var definition);
                        string name = definition == null ? "entry items" : LocalizationManager.GetString(definition.DisplayName.ToString());
                        return "Requires " + mode.EntryCount + " " + name + " (owned: " + owned + ")";
                    }
                }
                return "";
            }
            catch (Exception exception) { return exception.Message; }
        }
        public static string ProgressLabel(FightList fight)
        {
            if (HasCustomRouting(fight)) return "";
            if (fight == null || !TryFind(fight.BCKFACGMOKC.ToString(), out var mode)) return "";
            var progress = new ModModeProgress(_warrior, mode);
            return " (" + (progress.Step + 1) + "/" + mode.Fights.Count + ")";
        }

        public static bool TryProgress(FightList fight, out int completed, out int total)
        {
            completed = 0; total = 0;
            if (fight == null || !TryFind(fight.BCKFACGMOKC.ToString(), out var mode)) return false;
            var progress = new ModModeProgress(_warrior, mode);
            completed = progress.Step; total = mode.Fights.Count;
            return true;
        }

        // Every owned map battle is a semantic entry point into the mode's current step.
        public static bool PrepareEntry(FightList fight, Action resume)
        {
            if (fight == null || !TryFind(fight.BCKFACGMOKC.ToString(),out var mode) || !mode.UsesPrepareCallback) return true;
            try
            {
                if (_pending != null) return false;
                var progress = new ModModeProgress(_warrior,mode);
                if (progress.ReadPlan() != null || progress.Entered) return true;
                if (progress.Step >= mode.Fights.Count || !mode.IsAvailable(ListSF.CCDKHLAMKKO().PINDEKDNCNL(),DateTimeOffset.UtcNow.ToUnixTimeSeconds()))
                    return Reject("This mode is complete or unavailable.");
                if (Prepare == null || BuildEncounter == null || SchedulePreparation == null) return Reject("Mode preparation is unavailable.");
                int step = progress.Step, completions = progress.Completions;
                var save = _warrior;
                var request = new ModModeRequest();
                _pending = request;
                SchedulePreparation(request, () => {
                    _pending = null;
                    if (_warrior != save) throw new ModContentException("The profile changed during encounter preparation.");
                    var current = new ModModeProgress(_warrior,mode);
                    if (current.Step != step || current.Completions != completions || current.Entered)
                        throw new ModContentException("Mode progress changed during preparation.");
                    // Validate native construction before committing the saved plan or charging entry.
                    if (BuildEncounter(mode,step,request.Plan) == null) throw new ModContentException("Encounter construction failed.");
                    current.SavePlan(request.Plan);
                    ListSF.CCDKHLAMKKO().GGGEHAGCLGC(true);
                    request.Invalidate();
                    resume();
                }, () => { if (_pending == request) _pending = null; });
                Prepare(mode,step,completions,request);
                return false;
            }
            catch (Exception exception) { _pending?.Invalidate(); _pending=null; return Reject(exception.Message); }
        }

        public static bool ResolveEntry(ref FightList fight)
        {
            if (fight == null || !TryFind(fight.BCKFACGMOKC.ToString(), out var mode)) return true;
            try
            {
                var progress = new ModModeProgress(_warrior, mode);
                if (!mode.IsAvailable(ListSF.CCDKHLAMKKO().PINDEKDNCNL(), DateTimeOffset.UtcNow.ToUnixTimeSeconds()))
                    return Reject("This event is outside its schedule or level requirement.");
                if (progress.Step >= mode.Fights.Count) return Reject("This mode is complete.");
                var selected = ListSF.CHMCKGCDGCM(new FightIDS(ModPolicies.Content.RuntimeFightId(mode.Fights[progress.Step])));
                if (selected == null) return Reject("The mode's next fight is unavailable.");
                var plan = progress.ReadPlan();
                if (plan != null) selected = BuildEncounter?.Invoke(mode,progress.Step,plan) ?? throw new ModContentException("Saved encounter construction is unavailable.");
                if (mode.HasEntryItem && !progress.Entered)
                {
                    var item = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(mode.EntryItem.ToString());
                    if (item == null || item.Count < mode.EntryCount) return Reject("This mode requires " + mode.EntryCount + " entry item(s): " + mode.EntryItem);
                }
                fight = selected; return true;
            }
            catch (Exception exception) { return Reject(exception.Message); }
        }
        public static bool Begin(FightList fight)
        {
            if (fight == null || !TryFind(fight.BCKFACGMOKC.ToString(), out var mode)) return true;
            try
            {
                var progress = new ModModeProgress(_warrior, mode);
                if (!mode.IsAvailable(ListSF.CCDKHLAMKKO().PINDEKDNCNL(), DateTimeOffset.UtcNow.ToUnixTimeSeconds())) return false;
                if (_activeFight != null && _activeFight != fight.BCKFACGMOKC.ToString()) return false;
                _newReservation = _activeFight == fight.BCKFACGMOKC.ToString() ? _newReservation : !progress.Entered;
                if (progress.Step >= mode.Fights.Count || ModPolicies.Content.RuntimeFightId(mode.Fights[progress.Step]) != fight.BCKFACGMOKC.ToString()) return false;
                if (!progress.Entered)
                {
                    if (mode.HasEntryItem)
                    {
                        var item = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(mode.EntryItem.ToString());
                        if (item == null || item.Count < mode.EntryCount) return false;
                        item.Count -= mode.EntryCount;
                    }
                    progress.Enter();
                    ListSF.CCDKHLAMKKO().GGGEHAGCLGC(true);
                }
                _activeFight = fight.BCKFACGMOKC.ToString(); return true;
            }
            catch (Exception exception) { return Reject(exception.Message); }
        }
        public static void CancelLaunch(FightList fight)
        {
            if (fight == null || !TryFind(fight.BCKFACGMOKC.ToString(), out var mode)) return;
            if (_newReservation)
            {
                var progress = new ModModeProgress(_warrior, mode);
                if (progress.Entered)
                {
                    if (mode.HasEntryItem)
                    {
                        var item = ListSF.CCDKHLAMKKO().KHCNHPCPFII().CMGOCLGHNLH(mode.EntryItem.ToString());
                        if (item != null) item.Count += mode.EntryCount;
                    }
                    progress.CancelEnter(); ListSF.CCDKHLAMKKO().GGGEHAGCLGC(true);
                }
            }
            _activeFight = null; _newReservation = false;
        }
        public static void NotifyEntry(FightList fight)
        {
            if (_newReservation && IsRaid(fight)) Raise(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_ENTER);
            _newReservation = false;
        }
        public static void NotifyResult(FightList fight)
        {
            if (fight == null || !TryFind(fight.BCKFACGMOKC.ToString(), out var mode)) return;
            var list = ListSF.ELEBLBJKDBI();
            if (_completedMode && mode.Raid) list.FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RAID_END);
            if (_resetMode) list.FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_RESET_ASCENSION);
            if (mode.Raid) list.FFBAJNGHGGD(QuestEvent.PMDPDMFLCIJ.QUEST_EVENT_SHOW_RAID_LOOT);
            _completedMode = false; _resetMode = false;
        }
        public static bool CanResolve(FightList fight) => fight == null || !TryFind(fight.BCKFACGMOKC.ToString(), out var ignored) || _activeFight == fight.BCKFACGMOKC.ToString();
        public static void Complete(FightList fight, bool won)
        {
            if (fight == null || _activeFight != fight.BCKFACGMOKC.ToString() || !TryFind(_activeFight, out var mode)) return;
            _activeFight = null;
            try
            {
                var progress = new ModModeProgress(_warrior, mode);
                int before=progress.Completions;
                int? selected=null;
                try { selected=SelectNext?.Invoke(mode,won,progress.Step,progress.Completions); }
                catch (Exception exception) { Reject("Mode result callback failed; using default progression. "+exception.Message); }
                if (selected.HasValue && (selected < 0 || selected > mode.Fights.Count))
                {
                    Reject("Mode result callback selected an invalid step; using default progression.");
                    selected=null;
                }
                progress.Complete(mode, won, selected);
                _completedMode = progress.Completions > before;
                _resetMode = !_completedMode && !won && progress.Step == 0 && (selected.HasValue || mode.ResetOnLoss);
                ListSF.CCDKHLAMKKO().GGGEHAGCLGC(true);
            }
            catch (Exception exception) { Reject(exception.Message); }
        }
        private static bool Reject(string reason) { Warning?.Invoke("[ModMode] " + reason); return false; }
    }
    public sealed class OfflineRaidQuestAction : QuestAction
    {
        private readonly string _operation;
        private int _index;
        private string _zone;
        public OfflineRaidQuestAction(string operation) { _operation = operation; }
        public override void Parse(XmlNode node)
        {
            base.Parse(node);
            int.TryParse(node.Attributes?["Index"]?.Value, out _index);
            _zone = node.Attributes?["Name"]?.Value;
        }
        public override void DEJMHFMLKIC(QuestParameters parameters)
        {
            base.DEJMHFMLKIC(parameters);
            var map = Nekki.SF2.GUI.Scene<Nekki.SF2.GUI.Map.MapScene>.get_Current();
            if (_operation == "loot") ModModeRuntime.ShowRaidResult();
            else if (map != null)
            {
                map.SetRaidToggleVisible(true);
                if (_operation == "open")
                {
                    var zones = ListSF.FHAIJEAPFEA().FindAll(Eclipse.Underworld.UnderworldZonePolicy.IsRaidZone);
                    string name = _zone;
                    if (string.IsNullOrEmpty(name) && _index >= 0 && _index < zones.Count) name = zones[_index].get_Name();
                    if (!string.IsNullOrEmpty(name)) { map.SwitchToRaidMap(); map.GotoZoneByName(name); }
                }
            }
            OGIJONMKABB();
        }
    }

}

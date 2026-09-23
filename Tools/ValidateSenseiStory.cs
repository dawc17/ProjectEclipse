using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

// Story orchestration test, not an opponent/art or native combat acceptance test.
sealed class StoryPortraits : IAssetByteProvider
{
    readonly LooseModProvider scripts;
    internal StoryPortraits(ModDescriptor mod) { scripts = new LooseModProvider(mod); }
    public ModId Namespace => ModId.Parse("fixture.story");
    public bool TryRead(AssetId id, out AssetBytes bytes) => scripts.TryRead(id, out bytes);
    public bool TryDescribe(AssetId id, out AssetMetadata metadata)
    {
        metadata = null;
        if (id.Namespace != Namespace || !id.Path.StartsWith("portraits/")) return scripts.TryDescribe(id, out metadata);
        metadata = new AssetMetadata(id, AssetKind.Sprite, AssetSourceKind.Core, "", -1, "controlled portrait metadata");
        return true;
    }
}
static class Program
{
    sealed class Lease : IDisposable
    {
        Action cancel;
        internal Lease(Action action) { cancel = action; }
        public void Dispose() { var action = cancel; cancel = null; action?.Invoke(); }
    }
    static int checks;
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static XmlDocument Read(string path) { var doc = new XmlDocument(); doc.Load(path); return doc; }
    static XmlDocument Save(string xml = null)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml ?? "<Warrior><EclipseMods schema='1'><Mod id='fixture.story'/></EclipseMods></Warrior>");
        return doc;
    }
    static void Main(string[] args)
    {
        string package = Path.Combine(args[0], "fixture.story");
        File.WriteAllText(Path.Combine(package, "mod.toml"), """
schema = 1
id = "fixture.story"
name = "Sensei combined story fixture"
version = "1.0.0"
authors = ["Eclipse tests"]
entrypoint = "scripts/main.lua"
capabilities = ["content.register", "story.events", "story.progression", "profile.read", "state.read", "state.write", "ui.create", "combat.effects"]
[[dependencies]]
id = "core"
version = ">=1.0 <2.0"
""");
        File.WriteAllText(Path.Combine(package, "scripts/main.lua"), """
local sf2=require("sf2")
-- Deliberate fixture warriors and availability reader. They do not satisfy the
-- production guard/prince identity, combat loadout or perk-state requirements.
local opponents,portraits={},{}
for act=1,6 do
 opponents[act]={normal={},eclipse={}}
 for _,mode in ipairs{"normal","eclipse"} do
  for index=1,(act==6 and 2 or 3) do
   opponents[act][mode][index]=sf2.warriors.register{id="dummy_"..act.."_"..mode.."_"..index}
  end
 end
end
local function portrait(name)
 if name then portraits[name]=sf2.assets.sprite("fixture.story:portraits/"..name) end
end
portrait("character_sensei")
for _,sequence in ipairs(require("content.sensei_entry_data")) do
 for _,card in ipairs(sequence.cards) do portrait(card.portrait) end
end
for _,sequence in ipairs(require("content.sensei_victory_data")) do
 for _,card in ipairs(sequence) do portrait(card.portrait) end
end
local factory=require("content.sensei_story")
local graph=factory.install(opponents,function() return false end,portraits)
assert(#graph.final_ids==6 and #graph.prior_final_ids==5 and #graph.finals==5)
for act=1,6 do
 assert(graph.final_ids[act]=="fixture.story:fights/sensei_act_"..act.."_normal_"..#graph.normal[act])
 if act<6 then assert(graph.prior_final_ids[act]==graph.final_ids[act]) end
end
""");
        var mod = ModDiscovery.DiscoverLoose(args[0]).Mods.Single();
        var catalog = new ModContentCatalog();
        CoreContentImporter.ImportStages(catalog, Read(Path.Combine(args[1], "Assets/vanillaXml/stages.xml")).SelectSingleNode("Stages/Zones"));
        var items = Read(Path.Combine(args[1], "Assets/vanillaXml/list.xml")).SelectNodes("/List/Items/Item").Cast<XmlNode>().ToArray();
        CoreContentImporter.ImportWeapons(catalog, items, null);
        CoreContentImporter.ImportArmors(catalog, items, null);
        CoreContentImporter.ImportHelms(catalog, items, null);
        CoreContentImporter.ImportMagic(catalog, items, null);
        CoreContentImporter.ImportRanged(catalog, items, null);
        CoreContentImporter.ImportPerks(catalog, Read(Path.Combine(args[1], "Assets/vanillaXml/perks.xml")).DocumentElement.ChildNodes.Cast<XmlNode>());
        var errors = new List<string>();
        var bus = new ModStoryEvents((owner, message) => errors.Add(message));
        var state = new ModStateRuntime();
        var dialogs = new FakeDialogHost(catalog);
        var views = dialogs.Views;
        var wins = new Dictionary<string, int>();
        var mapActions = new List<string>();
        bool mapReady = true;
        ModProfileAccess.Fight = id => new ModProfileFightSnapshot(true, wins.TryGetValue(id.ToString(), out var value) ? value : 0, 0);
        ModProfileAccess.SetEclipseMode = value => { mapActions.Add("eclipse:" + value); return mapReady; };
        ModBattleAccess.Reveal = (id, locked) => { mapActions.Add("reveal:" + id + ":" + locked); return mapReady; };
        ModBattleAccess.SetLocked = (id, locked) => { mapActions.Add("lock:" + id + ":" + locked); return mapReady; };
        ModBattleAccess.Focus = id => { mapActions.Add("focus:" + id); return mapReady; };
        Action<bool> finish = null;
        IDisposable screen = null;
        int timed = 0, entryCards = 0, victoryCards = 0, notifications = 0, launches = 0, defeats = 0;
        ModActScreenAccess.Open = (lines, done) =>
        {
            Check(finish == null, "Two timed screens overlapped");
            Check(views.All(view => view.IsClosed), "Timed screen overlapped a modal");
            timed++; finish = done; return screen = new Lease(() => done(false));
        };
        dialogs.Opened = view => Check(finish == null, "Native dialog overlapped a timed screen");
        using (var tx = catalog.BeginRegistration(mod))
        using (var context = new MoonSharpScriptRuntime(null, null, null, bus).CreateContext(mod, new ModApiFacade(mod,
            new AssetResolver(new IAssetProvider[] { new StoryPortraits(mod) }), tx, state, entry => errors.Add(entry.Message))))
        {
            context.ExecuteEntrypoint(); tx.Commit();
            Check(catalog.Fights.Count(value => value.Id.Namespace == mod.Id) == 23, "Not all actual encounters registered");
            Check(catalog.Rewards.Count(value => value.Id.Namespace == mod.Id) == 57, "Not all actual rewards registered");
            foreach (var fight in catalog.Fights.Where(value => value.Id.Namespace == mod.Id))
                Check(bus.FightEntries.Contains(fight.Id) == fight.Id.LocalId.Contains("_normal_"), "Entry handler attached to wrong mode");
            var save = Save();
            void Bind(XmlDocument doc)
            {
                Check(state.Bind(doc.DocumentElement, new[] { context }).Count == 0, "State bind failed");
                bus.BindProfile();
            }
            bool Flag(string name) => state.TryGetValue(mod.Id, "sensei_" + name, out var value) && value.Boolean;
            FakeDialog Live() => views.SingleOrDefault(view => !view.IsClosed);
            void Scene(string scene)
            {
                if (scene != "map")
                {
                    foreach (var view in views.ToArray()) view.Close(ModUiCloseReason.Scene);
                    finish = null; screen?.Dispose(); screen = null;
                    bus.FightEntries.CancelPending();
                }
                bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter, null, scene: scene));
            }
            void Result(string id, string outcome)
            {
                if (outcome == "win") wins[id] = wins.TryGetValue(id, out var count) ? count + 1 : 1;
                bus.Publish(new ModStoryEvent(ModStoryEventKind.BattleResult, null,
                    battle: new ModBattleResultSnapshot(DefinitionId.Parse(id), outcome, false)));
            }
            string Fight(int act, int index) => "fixture.story:fights/sensei_act_" + act + "_normal_" + index;
            void Timed()
            {
                var done = finish; Check(done != null, "Timed screen missing"); finish = null; screen = null;
                done(true); done(true);
            }
            void Acknowledge(string expected)
            {
                var view = Live(); Check(view?.Id == expected, "Expected " + expected + ", got " + view?.Id);
                Check(view.Read("portrait").Sprite?.Namespace == mod.Id, "A module ignored the supplied portrait namespace");
                Check(view.TryClick("continue") && view.IsClosed, "Acknowledgement did not close card");
            }
            void DrainEntry()
            {
                int guard = 0;
                while (finish != null || Live() != null)
                {
                    Check(++guard < 20, "Entry stalled");
                    if (finish != null) Timed();
                    else { Acknowledge("sensei_entry"); entryCards++; }
                }
            }
            Bind(save); Scene("map");
            Result("core:fights/zone_1/tournament/3", "loss");
            Check(Live() == null && !Flag("pending_1"), "Loss unlocked act one");
            Result("core:fights/zone_1/tournament/3", "win");
            Check(Live()?.Id == "sensei_notification" && !Flag("opened_1"), "First unlock not queued");
            mapReady = false; var blocked = Live(); blocked.TryClick("continue");
            Check(blocked.IsClosed && Flag("pending_1") && !Flag("opened_1"), "Failed map mutation acknowledged notification");
            Scene("map"); Check(Live()?.Id == "sensei_notification", "Refused notification was not shown again on the next map wake");
            mapReady = true; mapActions.Clear(); Acknowledge("sensei_notification"); notifications++;
            Check(mapActions.Count == 9 && mapActions[0] == "eclipse:False" && Flag("opened_1"), "First map unlock ordering differs");
            for (int act = 1; act <= 6; act++)
            {
                int count = act == 6 ? 2 : 3;
                // Next tournament is already complete: only the final Sensei win
                // may unlock the next act, and its dialogue must finish first.
                if (act < 6) wins["core:fights/zone_" + (act + 1) + "/tournament/3"] = 1;
                for (int index = 1; index <= count; index++)
                {
                    int before = launches;
                    Check(bus.FightEntries.Begin(DefinitionId.Parse(Fight(act, index)), () => { launches++; return true; }, () => true, () => true)
                        == ModFightEntryDecision.Deferred, "First normal entry did not defer");
                    DrainEntry();
                    Check(launches == before + 1 && Flag("entered_" + act + "_" + index), "Entry completion/launch missing");
                    Scene("fight");
                    if (index == 1)
                    {
                        Result(Fight(act, index), "loss");
                        Check(Live() == null && Flag("defeat_pending"), "Defeat dialogue opened in fight or was not saved");
                        Scene("map");
                        Check(Live()?.Id == "sensei_defeat" && finish == null && !Flag("dialogue_pending_" + act), "Loss did not queue only the defeat dialogue");
                        Check(!string.IsNullOrEmpty(Live().Read("body").Text), "Defeat dialogue has no line");
                        Acknowledge("sensei_defeat"); defeats++;
                        Check(Live() == null && !Flag("defeat_pending"), "Defeat acknowledgement left presentation or pending state");
                        Check(bus.FightEntries.Begin(DefinitionId.Parse(Fight(act, index)), () => throw new Exception("Replay deferred"), () => true, () => true)
                            == ModFightEntryDecision.Continue, "Completed introduction replayed after loss");
                        Scene("fight");
                    }
                    Result(Fight(act, index), "win");
                    Check(Live() == null && finish == null, "Presentation opened in fight");
                    Scene("map");
                    if (index < count)
                        Check(Live() == null && !Flag("dialogue_pending_" + act) && (act == 6 || !Flag("opened_" + (act + 1))), "Non-final win advanced story");
                }
                Check(Flag("dialogue_pending_" + act) && !Flag("complete_" + act), "Final ID did not queue victory");
                if (act < 6) Check(Flag("pending_" + (act + 1)) && !Flag("opened_" + (act + 1)), "Next act prerequisite uses wrong final");
                if (act == 3)
                {
                    Acknowledge("sensei_victory"); victoryCards++;
                    string body = Live().Read("body").Text, stored = save.OuterXml;
                    var savedWins = new Dictionary<string, int>(wins); wins.Clear();
                    Scene("shop"); bus.UnbindProfile(); state.Unbind(); Bind(Save()); Scene("map");
                    Check(Live() == null && !Flag("opened_1"), "Fresh profile inherited queued story");
                    foreach (var pair in savedWins) wins[pair.Key] = pair.Value;
                    bus.UnbindProfile(); state.Unbind(); save = Save(stored); Bind(save); Scene("map");
                    Check(Live()?.Read("body").Text == body, "Combined story lost saved victory cursor");
                }
                while (Live()?.Id == "sensei_victory") { Acknowledge("sensei_victory"); victoryCards++; }
                if (act == 6)
                {
                    Check(finish != null && !Flag("complete_6"), "Last act skipped timed outro");
                    var stale = finish; Scene("dojo"); stale(true);
                    Check(!Flag("complete_6"), "Cancelled outro completed campaign");
                    Scene("map"); Timed();
                }
                Check(Flag("complete_" + act), "Victory never completed");
                if (act < 6)
                {
                    Acknowledge("sensei_notification"); notifications++;
                    Check(Flag("opened_" + (act + 1)), "Next act notification did not unlock");
                }
                Check(Live() == null && finish == null, "Act left stale presentation");
            }
            Check(launches == 17 && entryCards == 39 && victoryCards == 23 && notifications == 6 && timed == 8 && defeats == 6,
                "Combined campaign coverage changed: " + launches + "/" + entryCards + "/" + victoryCards + "/" + notifications + "/" + timed + "/" + defeats);
            int shown = views.Count;
            for (int act = 1; act <= 6; act++)
            {
                Scene("fight"); Result("fixture.story:fights/sensei_act_" + act + "_eclipse_1", "loss"); Scene("map");
                Check(Live() == null && !Flag("defeat_pending"), "Eclipse loss queued the normal defeat dialogue");
                Scene("fight"); Result("fixture.story:fights/sensei_act_" + act + "_eclipse_1", "win"); Scene("map");
                Result(Fight(act, act == 6 ? 2 : 3), "win");
            }
            Check(views.Count == shown && finish == null, "Completed or eclipse fights replayed narrative");
            context.Dispose(); Check(!bus.FightEntries.HasHandlers && Live() == null, "Combined teardown leaked handlers/views");
            Check(errors.Count == 0, string.Join("\n", errors));
        }
        ModProfileAccess.Clear(); ModBattleAccess.Clear(); ModActScreenAccess.Clear();
        Console.WriteLine("PASS: " + checks + " combined Sensei story checks; actual encounter graph, 17 entries/39 cards, 23 victory cards, six ordered unlocks, archive-independent Lua execution, namespaced portraits, six defeat dialogues, loss/retry, save/profile separation and cancelled outro. Controlled opponents, availability and presentation host; no native campaign acceptance.");
    }
}

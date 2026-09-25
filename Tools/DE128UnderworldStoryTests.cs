using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;

// Plays the actual DE128 Underworld story (underworld_story.lua) through the
// production MoonSharp bindings with controlled profile, map and dialog hosts, and
// compares every card it presents with the archived quests.xml read directly
// here (RaidIntro, RaidIntro2, RaidEnter/RaidWinAfter/RaidLoseAfter<Boss>).
// Native StoryDialog rendering and a real campaign are not exercised.
internal static class DE128UnderworldStoryTests
{
    private sealed class Lease : IDisposable
    {
        private Action _cancel;
        public Lease(Action cancel) { _cancel = cancel; }
        public void Dispose() { var cancel = _cancel; _cancel = null; cancel?.Invoke(); }
    }

    private sealed class Card
    {
        public string Title, Text, Button, Portrait, Screen;
        public bool Launch;
        public override string ToString() => Title + " | " + Text + " | " + Button + " | " + Portrait;
    }

    private const string Lynx = "core:fights/zone_1/boss_lynx/2";
    // Users variables with exactly one assignment in quests.xml (asserted below).
    private static readonly Dictionary<string, string> Constants = new Dictionary<string, string>
    {
        { "_Title_Assistant", "characterMay" }, { "_Avatar_Assistant_1", "character_may_1" },
        { "_Title_Assistant_2", "characterSensei" }, { "_Avatar_Assistant_6", "character_sensei" },
    };
    private static readonly Dictionary<string, string> OwnedPortraits = new Dictionary<string, string>
    {
        { "character_may_1", "de128:sprites/underworld/character_may_1" },
        { "character_may_4", "de128:sprites/underworld/character_may_4" },
        { "character_sensei", "de128:sprites/sensei/character_sensei" },
    };

    private static Action<bool, string> _check;
    private static int _checks;
    private static void Check(bool value, string message) { _checks++; _check(value, message); }

    public static int Run(ModDescriptor mod, string repository,
        Func<ModDescriptor, ModContentCatalog, ModStoryEvents, ModStateRuntime, IModScriptContext> load, Action<bool, string> check)
    {
        _check = check; _checks = 0;
        var quests = new XmlDocument { XmlResolver = null };
        quests.Load(Path.Combine(repository, "Assets/DExml/quests.xml"));
        var english = new Dictionary<string, string>();
        var eng = new XmlDocument { XmlResolver = null };
        eng.Load(Path.Combine(repository, "Assets/DExml/localizations/eng.xml"));
        foreach (XmlElement word in eng.SelectNodes("//Word"))
            if (!english.ContainsKey(word.GetAttribute("Title"))) english[word.GetAttribute("Title")] = word.InnerText;
        foreach (var constant in Constants)
        {
            var values = quests.SelectNodes("//SetVariable[@Scope='Users' and @Name='" + constant.Key.Substring(1) + "']")
                .Cast<XmlElement>().Select(e => e.GetAttribute("Value")).Distinct().ToArray();
            Check(values.Length == 1 && values[0] == constant.Value, "Archive story constant changed: " + constant.Key);
        }
        var byName = quests.SelectNodes("//Quest").Cast<XmlElement>().ToDictionary(q => q.GetAttribute("Name"));
        var variables = byName.Where(q => q.Key.StartsWith("Raid")).SelectMany(q => q.Value.SelectNodes(".//Dialog").Cast<XmlElement>())
            .SelectMany(d => new[] { d.GetAttribute("Title"), d.GetAttribute("Image") }).Where(v => v.StartsWith("_")).Distinct();
        Check(variables.All(Constants.ContainsKey), "Archive story uses an unlisted variable");
        string Resolve(string value) => value != null && Constants.TryGetValue(value, out var constant) ? constant : value;
        string Text(string key) => key.Length == 0 ? string.Empty : english.TryGetValue(key, out var value) ? value : "<missing " + key + ">";
        List<Card> Cards(XmlElement quest) => quest.SelectNodes("Actions/Dialog|Actions/ActScreen").Cast<XmlElement>().Select(dialog =>
        {
            if (dialog.LocalName == "ActScreen")
            {
                // Text form: EnterScreen.prefab hideTime 5 s = 300 frames.
                var lines = dialog.SelectNodes("Line").Cast<XmlElement>().Select(l => Text(l.GetAttribute("Text")) + "@" + l.GetAttribute("Frames")).ToArray();
                return new Card { Screen = lines.Length > 0 ? string.Join("\n", lines) : Text(dialog.GetAttribute("Text")) + "@300" };
            }
            var button = (XmlElement)dialog.SelectSingleNode("Button");
            return new Card
            {
                Title = Text(Resolve(dialog.GetAttribute("Title"))).Trim().Length == 0 ? string.Empty : Text(Resolve(dialog.GetAttribute("Title"))),
                Text = Text(((XmlElement)dialog.SelectSingleNode("Line")).GetAttribute("Text")),
                Button = Text(button.GetAttribute("Text")),
                Portrait = dialog.HasAttribute("Image") ? Resolve(dialog.GetAttribute("Image")) : null,
                Launch = button.SelectSingleNode("Fight") != null,
            };
        }).ToList();
        var bosses = new SortedDictionary<string, Dictionary<string, List<Card>>>(StringComparer.Ordinal);
        foreach (var quest in byName)
        {
            string kind = quest.Key.StartsWith("RaidEnter") ? "enter" : quest.Key.StartsWith("RaidWinAfter") ? "win" : quest.Key.StartsWith("RaidLoseAfter") ? "loss" : null;
            if (kind == null) continue;
            string fight = quest.Value.SelectNodes("Conditions//Equal[@Value1='_$Fight']").Cast<XmlElement>().Single().GetAttribute("Value2");
            string boss = fight.Split('|')[1];
            if (!bosses.TryGetValue(boss, out var entry)) bosses[boss] = entry = new Dictionary<string, List<Card>>();
            entry[kind] = Cards(quest.Value);
        }
        Check(bosses.Count == 32 && bosses.Values.All(b => b.Count == 3), "Archive Underworld boss dialogue set changed");

        var catalog = new ModContentCatalog();
        var errors = new List<string>();
        var bus = new ModStoryEvents((owner, message) => errors.Add(message));
        var state = new ModStateRuntime();
        var dialogs = new FakeDialogHost(catalog);
        var wins = new Dictionary<string, int>();
        var toggles = new List<bool>();
        var focus = new List<string>();
        var scenes = new List<string>();
        Action<bool> finish = null;
        var screens = new List<string>();
        ModProfileAccess.Fight = id => new ModProfileFightSnapshot(true, wins.TryGetValue(id.ToString(), out var value) ? value : 0, 0);
        ModUnderworldAccess.SetToggleVisible = visible => { toggles.Add(visible); return true; };
        ModUnderworldAccess.SetFocus = id => { focus.Add(id.ToString()); return true; };
        ModSceneAccess.Open = scene => { scenes.Add(scene); return true; };
        ModActScreenAccess.Open = (lines, done) =>
        {
            Check(finish == null && dialogs.Live == null, "Act screen overlapped presentation");
            screens.Add(string.Join("\n", lines.Select(line => dialogs.Resolve(line.Text) + "@" + line.Frames)));
            finish = done;
            return new Lease(() => done(false));
        };
        try
        {
            using (var context = load(mod, catalog, bus, state))
            {
                var save = new XmlDocument();
                save.LoadXml("<Warrior><EclipseMods schema='1'><Mod id='de128'/></EclipseMods></Warrior>");
                Check(state.Bind(save.DocumentElement, new[] { context }).Count == 0, "Underworld story state did not bind");
                bus.BindProfile();
                long Intro() => state.TryGetValue(mod.Id, "uw_intro", out var value) ? value.Integer : -1;
                string Set(string name) => state.TryGetValue(mod.Id, "uw_" + name, out var value) ? value.String : null;
                void Scene(string scene)
                {
                    if (scene != "map")
                    {
                        foreach (var view in dialogs.Views.ToArray()) view.Close(ModUiCloseReason.Scene);
                        var pending = finish; finish = null; pending?.Invoke(false);
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
                void Expect(Card card, string where)
                {
                    var view = dialogs.Live;
                    Check(view != null, where + ": expected card " + card);
                    Check(view.Read("speaker").Text == card.Title && view.Read("body").Text == card.Text && view.Read("continue").Text == card.Button,
                        where + ": card text differs. Expected " + card + ", got " + view.Read("speaker").Text + " | " + view.Read("body").Text + " | " + view.Read("continue").Text);
                    string portrait = card.Portrait == null ? string.Empty
                        : OwnedPortraits.TryGetValue(card.Portrait, out var owned) ? owned : "core:ui/users/" + card.Portrait.ToLowerInvariant();
                    Check(view.Request.Portrait == portrait && !view.Request.Mirrored, where + ": portrait differs: " + view.Request.Portrait + " vs " + portrait);
                }
                void Play(List<Card> cards, string where)
                {
                    for (int index = 0; index < cards.Count; index++)
                    {
                        if (cards[index].Screen != null)
                        {
                            Check(finish != null && dialogs.Live == null && screens.Last() == cards[index].Screen,
                                where + " card " + (index + 1) + ": act screen differs: " + screens.LastOrDefault());
                            var done = finish; finish = null; done(true);
                            continue;
                        }
                        Expect(cards[index], where + " card " + (index + 1));
                        Check(dialogs.Live.TryClick("continue"), where + ": acknowledgement refused");
                    }
                }
                string FightId(string boss) => "de128:fights/uw_" + boss.ToLowerInvariant() + "_1";

                // Before Lynx 2: the Underworld toggle stays hidden and nothing is shown.
                Scene("map");
                Check(toggles.SequenceEqual(new[] { false }) && dialogs.Live == null && finish == null && Intro() == 0,
                    "Locked Underworld was not hidden quietly");
                Result(Lynx, "loss"); Scene("fight"); Scene("map");
                Check(dialogs.Live == null && finish == null && toggles.Count(value => !value) == 2, "A Lynx 2 loss started the Underworld intro");

                // RaidIntro after a Lynx 2 win (FightEnd, map), interrupted once by leaving the map.
                Scene("fight"); Result(Lynx, "win"); Check(finish == null, "Intro played outside the map");
                toggles.Clear(); Scene("map");
                Check(toggles.SequenceEqual(new[] { false }) && finish != null, "Intro act screen did not start on the map");
                var intro = byName["RaidIntro"];
                var act = (XmlElement)intro.SelectSingleNode("Actions/ActScreen/Line");
                Check(screens.Last() == Text(act.GetAttribute("Text")) + "@" + act.GetAttribute("Frames"), "Intro act screen differs");
                var introCards = Cards(intro).Where(card => card.Screen == null).ToList();
                finish(true); finish = null;
                Check(toggles.SequenceEqual(new[] { false, true }) && focus.SequenceEqual(new[] { "de128:battles/uw_boss_1" }),
                    "ShowRaidsGag/SetMapFocus did not precede the intro dialogs: " + string.Join(",", focus));
                Expect(introCards[0], "Intro"); Check(dialogs.Live.TryClick("continue"), "Intro card refused");
                Scene("shop");
                Check(Intro() == 0, "Interrupted intro was recorded as shown");
                toggles.Clear(); focus.Clear(); Scene("map");
                Check(toggles.First() == false && finish == null, "Interrupted intro replayed its completed act screen");
                Play(introCards.Skip(1).ToList(), "Resumed intro");
                Check(Intro() == 1, "Intro completion was not saved");
                Play(Cards(byName["RaidIntro2"]), "Followup");
                Check(Intro() == 2 && scenes.SequenceEqual(new[] { "dojo" }) && dialogs.Live == null, "Followup did not end in the dojo");
                toggles.Clear(); Scene("dojo"); Scene("map");
                Check(toggles.Count == 0 && dialogs.Live == null && finish == null, "Completed intro hid the toggle or replayed");

                // Every boss: entry held until its Fight button, archived win/loss dialogues once each.
                int cards = 0, screensSeen = 0;
                foreach (var boss in bosses)
                {
                    var id = DefinitionId.Parse(FightId(boss.Key));
                    int launches = 0;
                    Check(bus.FightEntries.Begin(id, () => { launches++; return true; }, () => true, () => true) == ModFightEntryDecision.Deferred,
                        boss.Key + ": first entry was not held");
                    var enter = boss.Value["enter"];
                    Check(enter.Last().Launch && enter.Take(enter.Count - 1).All(card => !card.Launch), boss.Key + ": archive entry shape changed");
                    screensSeen += enter.Count(card => card.Screen != null);
                    Play(enter, boss.Key + " entry");
                    Check(launches == 1 && Set("entered").Contains("," + boss.Key.ToLowerInvariant() + ","), boss.Key + ": entry did not launch once");
                    Check(bus.FightEntries.Begin(id, () => throw new Exception("replayed"), () => true, () => true) == ModFightEntryDecision.Continue,
                        boss.Key + ": greeting replayed");
                    Scene("fight"); Result(FightId(boss.Key), "surrender"); Scene("map");
                    Check(dialogs.Live == null, boss.Key + ": surrender showed the loss dialogue");
                    Scene("fight"); Result(FightId(boss.Key), "loss");
                    Check(dialogs.Live == null, boss.Key + ": result dialogue opened in the fight scene");
                    Scene("map"); Play(boss.Value["loss"], boss.Key + " loss");
                    Scene("fight"); Result(FightId(boss.Key), "loss"); Scene("map");
                    Check(dialogs.Live == null, boss.Key + ": loss dialogue repeated");
                    Scene("fight"); Result(FightId(boss.Key), "win"); Scene("map"); Play(boss.Value["win"], boss.Key + " win");
                    Scene("fight"); Result(FightId(boss.Key), "win"); Scene("map");
                    Check(dialogs.Live == null, boss.Key + ": win dialogue repeated");
                    cards += enter.Count(card => card.Screen == null) + boss.Value["loss"].Count + boss.Value["win"].Count;
                }
                Check(Set("win_pending") == "," && Set("loss_pending") == ",", "Result dialogues left pending state");

                // Hardmode and other fights never trigger the archived dialogues; a saved profile keeps completion.
                int shown = dialogs.Views.Count;
                Scene("fight"); Result("de128:fights/uw_boss_1_hardmode_1", "win"); Scene("map");
                Check(dialogs.Views.Count == shown, "A hardmode fight triggered story dialogue");
                string stored = save.OuterXml;
                bus.UnbindProfile(); state.Unbind();
                var fresh = new XmlDocument(); fresh.LoadXml("<Warrior><EclipseMods schema='1'><Mod id='de128'/></EclipseMods></Warrior>");
                wins.Clear(); Check(state.Bind(fresh.DocumentElement, new[] { context }).Count == 0, "Fresh profile did not bind"); bus.BindProfile();
                toggles.Clear(); Scene("map");
                Check(Intro() == 0 && toggles.SequenceEqual(new[] { false }), "Fresh profile inherited Underworld story state");
                bus.UnbindProfile(); state.Unbind();
                var restored = new XmlDocument(); restored.LoadXml(stored);
                Check(state.Bind(restored.DocumentElement, new[] { context }).Count == 0, "Saved profile did not bind"); bus.BindProfile();
                toggles.Clear(); Scene("map");
                Check(Intro() == 2 && toggles.Count == 0 && dialogs.Live == null, "Saved Underworld story progress was lost");

                // A save that beat Lynx 2 before DE128 existed is caught up on its first map entry.
                bus.UnbindProfile(); state.Unbind();
                var veteran = new XmlDocument(); veteran.LoadXml("<Warrior><EclipseMods schema='1'><Mod id='de128'/></EclipseMods></Warrior>");
                wins[Lynx] = 3; Check(state.Bind(veteran.DocumentElement, new[] { context }).Count == 0, "Veteran profile did not bind"); bus.BindProfile();
                Scene("map");
                Check(finish != null && Intro() == 0, "Veteran save was not caught up with the intro");
                finish(false); finish = null;
                Check(cards == 234 - introCards.Count - Cards(byName["RaidIntro2"]).Count && screensSeen == 1,
                    "Archived card coverage changed: " + cards + "/" + screensSeen);
            }
            Check(!bus.FightEntries.HasHandlers && dialogs.Live == null, "Underworld story teardown leaked handlers or dialogs");
            Check(errors.Count == 0, "Underworld story errors: " + string.Join("\n", errors));
        }
        finally
        {
            ModProfileAccess.Clear(); ModUnderworldAccess.Clear(); ModSceneAccess.Clear(); ModActScreenAccess.Clear(); ModStoryDialogAccess.Clear();
        }
        return _checks;
    }
}

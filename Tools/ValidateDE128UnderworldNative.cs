using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Eclipse.Modding;
using Eclipse.Underworld;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Dialogs;
using Nekki.SF2.GUI.Map;
using Nekki.SF2.GUI.Menu;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Runs only in the independently copied project made by TestDE128UnderworldNative.py.
[InitializeOnLoad]
public static class ValidateDE128UnderworldNative
{
    const string Active = "Eclipse.DE128UnderworldNative.Active";
    const string Prefix = "[DE128UnderworldNative] ";
    const string FightId = "de128:fights/uw_boss_1_1";
    static readonly BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static double started, lastReadinessReport, lastDialogPress;
    static bool campaign, mapRequested, raidPrepared, catalogAudited, entryRequested, surrenderRequested;
    static int cards, campaignCards;
    static readonly HashSet<ModStoryDialogPresenter> Acknowledged = new HashSet<ModStoryDialogPresenter>();

    static ValidateDE128UnderworldNative()
    {
        if (!SessionState.GetBool(Active, false)) return;
        started = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
    }

    public static void RunEditor()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "de128-underworld-fixture.marker")))
            throw new InvalidOperationException("Underworld acceptance requires an isolated project copy.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT", Path.Combine(root, "Mods"));
        PlayerSettings.companyName = "EclipseAcceptance";
        string profileTag = Environment.GetEnvironmentVariable("ECLIPSE_DE128_UNDERWORLD_PROFILE_TAG");
        PlayerSettings.productName = Path.GetFileName(root) +
            (string.IsNullOrEmpty(profileTag) ? string.Empty : "-" + profileTag);
        SessionState.SetBool(Active, true);
        EditorSceneManager.OpenScene("Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        EditorApplication.EnterPlaymode();
    }

    static void Update()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            if (EditorApplication.timeSinceStartup - started > 300)
                throw new Exception("Timed out: campaign=" + campaign + " map=" + mapRequested +
                    " entry=" + entryRequested + " cards=" + cards + " fight=" + (Fight.GetCurrentFight() != null));
            if (!campaign && Eclipse.UI.TitleScreen.IsOpen)
            {
                var title = UnityEngine.Object.FindObjectOfType<Eclipse.UI.TitleScreen>();
                if (title != null)
                {
                    typeof(Eclipse.UI.TitleScreen).GetMethod("BeginCampaign", Hidden).Invoke(title, null);
                    campaign = true;
                }
                return;
            }
            var scripts = ModRuntime.Scripts;
            var roster = ListSF.CCDKHLAMKKO();
            var module = Module.GetInstance();
            if (scripts == null || roster == null || module == null) return;
            if (scripts.Diagnostics.Count != 0 || scripts.StateDiagnostics.Count != 0)
                throw new Exception("DE128 diagnostics: " + string.Join("; ", scripts.Diagnostics) +
                    "; state=" + string.Join("; ", scripts.StateDiagnostics));
            if (!entryRequested)
            {
                if (module.NMCNDOPKFJD() == ScreenType.ModuleDojo && !mapRequested)
                {
                    var menu = MainMenu.get_Instance();
                    if (menu == null) return;
                    // Use the game's own skip flow so a fresh profile does not leave
                    // a tutorial quest awaiting a fight after navigation to Map.
                    scripts.State.SetValues(ModId.Parse("de128"), new Dictionary<string, ModParameterValue> {
                        { "uw_intro", ModParameterValue.FromInteger(2) }
                    });
                    roster.Level = 4;
                    menu.SkipTutorial();
                    mapRequested = true;
                    return;
                }
                if (module.NMCNDOPKFJD() != ScreenType.ModuleMap) return;
                var scene = UnityEngine.Object.FindObjectOfType<MapScene>();
                if (scene == null) return;
                if (!raidPrepared)
                {
                    scene.SwitchToRaidMap();
                    var container = (MapContainer)typeof(MapScene).GetField("_storyContainer", Hidden).GetValue(scene);
                    if (container.GetZonesCount() != 8)
                        throw new Exception("The real Underworld map did not load all eight tiers: " + container.GetZonesCount());
                    raidPrepared = true;
                    return;
                }
                if (typeof(ModRuntime).GetMethod("ReadyProgressionMap", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, null) == null)
                {
                    var flags = BindingFlags.Static | BindingFlags.NonPublic;
                    var activeDialog = typeof(DialogsManager).GetField("OALIPPPOHCL", flags).GetValue(null) as BaseDialog;
                    if (activeDialog is StoryDialog story && story.IsQuestDialog &&
                        EditorApplication.timeSinceStartup - lastDialogPress > 0.2)
                    {
                        if (++campaignCards > 40 || story.get_ButtonOK() == null)
                            throw new Exception("Native campaign story did not present a usable confirmation button.");
                        lastDialogPress = EditorApplication.timeSinceStartup;
                        var title = typeof(StoryDialog).GetField("AKNJEGGNNBJ", Hidden).GetValue(story);
                        PressStoryButton(story);
                        Debug.Log(Prefix + "Acknowledged campaign quest card " + campaignCards + ": " + title);
                        return;
                    }
                    if (EditorApplication.timeSinceStartup - lastReadinessReport > 10)
                    {
                        lastReadinessReport = EditorApplication.timeSinceStartup;
                        var blocker = typeof(Eclipse.UI.Modding.ModUiGameBridge)
                            .GetProperty("NativeInputBlocked", flags);
                        var lockScreen = LockScreen.get_Instance();
                        var uiBridge = typeof(Eclipse.UI.Modding.ModUiGameBridge);
                        var blocks = uiBridge.GetField("presentationBlocks", flags).GetValue(null);
                        Debug.Log(Prefix + "Waiting for map readiness: scene=" +
                            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex +
                            " mapCurrent=" + (Scene<MapScene>.get_Current() != null) + " lock=" +
                            (lockScreen != null && lockScreen.gameObject.activeInHierarchy) +
                            " input=" + blocker?.GetValue(null) +
                            " nativeBlocked=" + uiBridge.GetField("nativeBlocked", flags).GetValue(null) +
                            " presentationBlocks=" + blocks.GetType().GetProperty("Count").GetValue(blocks) +
                            " title=" + Eclipse.UI.TitleScreen.IsOpen +
                            " dialog=" + (activeDialog == null ? "<none>" : activeDialog.GetType().Name + "/" + activeDialog.IsQuestDialog) +
                            " navigation=" + typeof(ModRuntime).GetField("_sceneNavigationInProgress", flags).GetValue(null) +
                            " mutation=" + typeof(ModRuntime).GetField("_profileMutationState", flags).GetValue(null) +
                            " preparation=" + typeof(ModModeRuntime).GetProperty("HasPendingPreparation", flags).GetValue(null));
                    }
                    return;
                }
                var definition = scripts.Content.Fights.FirstOrDefault(value => value.Id.ToString() == FightId);
                if (definition == null) throw new Exception("Volcano fight missing from the live mod catalog.");
                if (!catalogAudited)
                {
                    AuditUnderworldCatalog(scripts.Content);
                    catalogAudited = true;
                }
                var encounter = ListSF.CHMCKGCDGCM(new FightIDS(scripts.Content.RuntimeFightId(definition.Id)));
                if (encounter == null || !UnderworldZonePolicy.IsRaidZone(encounter.Battle.OAEIILGHJMG))
                    throw new Exception("The resolved Volcano encounter is not an Underworld fight.");
                bool immediate = GameUtils.StartFight(encounter, false, null, true, false);
                entryRequested = true;
                bool preparing = (bool)typeof(ModModeRuntime).GetProperty("HasPendingPreparation",
                    BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                if (!immediate && !preparing && !StoryBus.FightEntries.HasPending)
                    throw new Exception("Volcano entry was refused without native preparation or story hold.");
                if (Fight.GetCurrentFight() != null)
                    throw new Exception("Volcano launched without its native story-entry hold.");
                Debug.Log(Prefix + "Entered eight-tier raid map; Volcano preparation=" + preparing +
                    " story pending=" + StoryBus.FightEntries.HasPending);
                return;
            }
            if (surrenderRequested)
            {
                if (module.NMCNDOPKFJD() != ScreenType.ModuleMap) return;
                var returnedMap = UnityEngine.Object.FindObjectOfType<MapScene>();
                if (returnedMap == null) return;
                if (returnedMap.GetCurrentState() != MapScene.NMFLNANKNOJ.RaidMode)
                    throw new Exception("Surrender returned to the story map instead of Underworld.");
                Debug.Log(Prefix + "PASS: eight real raid map tiers, three native Volcano entry cards, resumed fight, " +
                    "60 live frames, rendered arena art and fighter rigs, 15 shield bars, nine alignment rows, " +
                    "and native surrender returning to Underworld.");
                Finish(0);
                return;
            }
            var fight = Fight.GetCurrentFight();
            if (fight == null)
            {
                var presenters = UnityEngine.Object.FindObjectsOfType<ModStoryDialogPresenter>();
                foreach (var presenter in presenters)
                {
                    if (presenter == null || !presenter.gameObject.activeInHierarchy ||
                        !Acknowledged.Add(presenter)) continue;
                    var dialog = typeof(ModStoryDialogPresenter).GetField("dialog", Hidden).GetValue(presenter) as StoryDialog;
                    if (dialog == null || dialog.get_ButtonOK() == null)
                        throw new Exception("The Volcano card did not use a native StoryDialog with a Fight/More button.");
                    cards++;
                    if (cards > 3 || !StoryBus.FightEntries.HasPending)
                        throw new Exception("Volcano entry card order or pending fight state changed.");
                    PressStoryButton(dialog);
                    Debug.Log(Prefix + "Acknowledged native Volcano story card " + cards);
                    break;
                }
                return;
            }
            if (cards != 3 || StoryBus.FightEntries.HasPending)
                throw new Exception("Volcano fight began before all three story cards completed.");
            var enemy = (Model)typeof(Fight).GetField("CKNCPOABFBO", Hidden).GetValue(fight);
            var player = (Model)typeof(Fight).GetField("_playerModel", Hidden).GetValue(fight);
            if (enemy == null || player == null || fight.get_FightTimeInFrames() < 60) return;
            var live = fight.OGNINOBBHIG();
            if (live?.FightId?.ToString() != new FightIDS(scripts.Content.RuntimeFightId(
                    scripts.Content.Fights.First(value => value.Id.ToString() == FightId).Id)).ToString() ||
                !UnderworldZonePolicy.IsRaidZone(live.Battle.OAEIILGHJMG) ||
                live.Location != "vulcan_raid")
                throw new Exception("Volcano fight identity, raid zone or arena changed: " + live?.Location);
            if (enemy.Parameters.ShieldTotal != 15 || enemy.Parameters.AttributeAlignments.Count != 9 ||
                enemy.CLDMEJKGLBA() == null || player.CLDMEJKGLBA() == null)
                throw new Exception("Volcano native fighter/shield/alignment setup changed: bars=" +
                    enemy.Parameters.ShieldTotal + " alignments=" + enemy.Parameters.AttributeAlignments.Count);
            var location = (Location)typeof(Fight).GetField("_location", Hidden).GetValue(fight);
            int renderedSprites = location?.layers?.Sum(layer => layer.ICDCIANNAAI == null ? 0 :
                layer.ICDCIANNAAI.GetComponentsInChildren<SpriteRenderer>(true).Length) ?? 0;
            if (location?.name != "vulcan_raid" || renderedSprites == 0)
                throw new Exception("Volcano arena did not render its packaged sprites: " + location?.name +
                    " sprites=" + renderedSprites);
            var weapon = new List<global::Pair<string, float>> {
                new global::Pair<string, float>("WeaponDamage", 0f)
            };
            float playerStrike = GameUtils.GetAttributesHitMultiplier(true, player.Parameters,
                enemy.Parameters, weapon, "BodyDefense");
            float bossStrike = GameUtils.GetAttributesHitMultiplier(false, enemy.Parameters,
                player.Parameters, weapon, "BodyDefense");
            if (float.IsNaN(playerStrike) || float.IsInfinity(playerStrike) ||
                float.IsNaN(bossStrike) || float.IsInfinity(bossStrike) ||
                playerStrike < 0.01f || playerStrike > 100f || bossStrike < 0.01f || bossStrike > 100f)
                throw new Exception("Volcano weapon/body alignment escaped the expected combat scale: " +
                    playerStrike + " / " + bossStrike);
            Debug.Log(Prefix + "Volcano weapon/body alignment multipliers: player=" + playerStrike +
                " boss=" + bossStrike);
            Debug.Log(Prefix + "Volcano arena rendered " + renderedSprites + " native sprites; surrendering.");
            typeof(Fight).GetMethod("SurrenderButtonCallback", Hidden).Invoke(fight, new object[] { null });
            surrenderRequested = true;
        }
        catch (Exception error) { Debug.LogError(Prefix + "FAIL " + error); Finish(1); }
    }

    static ModStoryEvents StoryBus => (ModStoryEvents)typeof(ModRuntime)
        .GetField("StoryEvents", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

    static void AuditUnderworldCatalog(ModContentCatalog content)
    {
        var templates = content.WarriorTemplates.Where(value =>
            value.Id.ToString().StartsWith("de128:warrior-templates/uw_", StringComparison.Ordinal)).ToArray();
        if (templates.Length != 66)
            throw new Exception("Expected 66 registered Underworld boss templates, found " + templates.Length);
        var expected = new Dictionary<DefinitionId, int>();
        int ExpectedRows(DefinitionId id)
        {
            if (expected.TryGetValue(id, out int known)) return known;
            if (!content.TryGetWarriorTemplate(id, out var definition))
                throw new Exception("Missing warrior template in live catalog: " + id);
            int count;
            if (definition.IsCore)
            {
                var core = ListSF.GetInstance().CNFBCBDPKCI(definition.LegacyName);
                if (core?.KEJDJHAGBMK == null)
                    throw new Exception("Missing native core warrior template: " + definition.LegacyName);
                count = core.KEJDJHAGBMK.AttributeAlignments.Count;
            }
            else
            {
                var body = definition.Body;
                count = (body.HasTemplate ? ExpectedRows(body.Template) : 0) + body.AttributeAlignments.Count;
                var native = ListSF.GetInstance().CNFBCBDPKCI(definition.LegacyName);
                int actual = native?.KEJDJHAGBMK?.AttributeAlignments.Count ?? -1;
                if (actual != count)
                    throw new Exception("Native template alignment inheritance differs: " + definition.Id +
                        " expected=" + count + " actual=" + actual);
            }
            expected.Add(id, count);
            return count;
        }
        foreach (var template in templates) ExpectedRows(template.Id);
        var fights = content.Fights.Where(value =>
            value.Id.ToString().StartsWith("de128:fights/uw_", StringComparison.Ordinal)).ToArray();
        if (fights.Length != 76)
            throw new Exception("Expected 76 registered Underworld fights, found " + fights.Length);
        int opponents = 0;
        foreach (var fight in fights)
        {
            var native = ListSF.CHMCKGCDGCM(new FightIDS(content.RuntimeFightId(fight.Id)));
            if (native == null || !UnderworldZonePolicy.IsRaidZone(native.Battle?.OAEIILGHJMG) ||
                native.OFKJMHPMCCD().Count != fight.Warriors.Count)
                throw new Exception("Native Underworld fight or opponent roster differs: " + fight.Id);
            if (!string.IsNullOrEmpty(fight.Location) && native.Location != fight.Location)
                throw new Exception("Native Underworld arena differs: " + fight.Id + " at " + native.Location);
            for (int i = 0; i < fight.Warriors.Count; i++)
            {
                if (!content.TryGetWarrior(fight.Warriors[i], out var warrior))
                    throw new Exception("Missing Underworld opponent definition: " + fight.Warriors[i]);
                int count = (warrior.HasTemplate ? ExpectedRows(warrior.Template) : 0) +
                    warrior.AttributeAlignments.Count;
                var model = native.OFKJMHPMCCD()[i];
                if (model == null || model.AttributeAlignments.Count != count ||
                    (warrior.HealthBars > 0 && model.ShieldTotal != warrior.HealthBars))
                    throw new Exception("Native Underworld opponent inheritance differs: " + warrior.Id +
                        " expected rows=" + count + " actual=" + model?.AttributeAlignments.Count);
                opponents++;
            }
        }
        if (opponents != 104)
            throw new Exception("Expected 104 native Underworld opponent slots, found " + opponents);
        Debug.Log(Prefix + "Audited 66 native boss templates, 76 raid fights and 104 opponent slots.");
    }

    static void PressStoryButton(StoryDialog dialog)
    {
        typeof(StoryDialog).GetMethod("GPEKKGLDKDF", Hidden).Invoke(dialog, new object[] { null });
    }

    static void Finish(int code)
    {
        SessionState.SetBool(Active, false);
        EditorApplication.update -= Update;
        EditorApplication.Exit(code);
    }
}

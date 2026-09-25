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
public static class ValidateDE128TierBossesNative
{
    const string Active = "Eclipse.DE128TierBossesNative.Active";
    const string Prefix = "[DE128TierBossesNative] ";
    static readonly BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static readonly BindingFlags HiddenStatic = BindingFlags.Static | BindingFlags.NonPublic;
    static double started, lastPress, lastReport;
    static bool campaign, mapRequested, raidPrepared, entryRequested, surrenderRequested;
    static int targetIndex, storyPresses, storyIntros;
    static List<FightDefinition> targets;
    static string combatException;

    static ValidateDE128TierBossesNative()
    {
        if (!SessionState.GetBool(Active, false)) return;
        started = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
        Application.logMessageReceived += CaptureCombatException;
    }

    public static void RunEditor()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "de128-underworld-fixture.marker")))
            throw new InvalidOperationException("Tier boss acceptance requires an isolated project copy.");
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
            if (combatException != null) throw new Exception("Native boss combat exception: " + combatException);
            if (EditorApplication.timeSinceStartup - started > 180)
                throw new Exception("Timed out on boss " + targetIndex + " of " + (targets?.Count ?? 0) +
                    ": entry=" + entryRequested + " cards=" + storyPresses +
                    " fight=" + (Fight.GetCurrentFight() != null));
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
            if (!mapRequested && module.NMCNDOPKFJD() == ScreenType.ModuleDojo)
            {
                var menu = MainMenu.get_Instance();
                if (menu == null) return;
                scripts.State.SetValues(ModId.Parse("de128"), new Dictionary<string, ModParameterValue> {
                    { "uw_intro", ModParameterValue.FromInteger(2) }
                });
                roster.Level = 4;
                menu.SkipTutorial();
                mapRequested = true;
                return;
            }
            if (surrenderRequested)
            {
                if (module.NMCNDOPKFJD() != ScreenType.ModuleMap) return;
                var returned = UnityEngine.Object.FindObjectOfType<MapScene>();
                if (returned == null) return;
                if (returned.GetCurrentState() != MapScene.NMFLNANKNOJ.RaidMode)
                    throw new Exception("Boss " + Target.Id + " surrendered to the story map.");
                Debug.Log(Prefix + "Returned from " + Target.Id + " to the Underworld map.");
                if (storyPresses > 0) storyIntros++;
                targetIndex++;
                started = EditorApplication.timeSinceStartup;
                storyPresses = 0;
                entryRequested = surrenderRequested = false;
                combatException = null;
                if (targetIndex == targets.Count)
                {
                    bool storyMatrix = Environment.GetEnvironmentVariable(
                        "ECLIPSE_DE128_UNDERWORLD_MATRIX") == "story";
                    if (storyMatrix && storyIntros < 30)
                        throw new Exception("Only " + storyIntros + " of 32 archived boss intros appeared on this profile.");
                    Debug.Log(Prefix + "PASS: " + targets.Count + " boss fights rendered arenas and fighter rigs, " +
                        "ran 30 combat frames each, and returned to Underworld; " + storyIntros +
                        " native story intros appeared on this profile.");
                    Finish(0);
                }
                return;
            }
            if (!entryRequested)
            {
                if (module.NMCNDOPKFJD() != ScreenType.ModuleMap) return;
                var scene = UnityEngine.Object.FindObjectOfType<MapScene>();
                if (scene == null) return;
                if (!raidPrepared)
                {
                    scene.SwitchToRaidMap();
                    var container = (MapContainer)typeof(MapScene).GetField("_storyContainer", Hidden).GetValue(scene);
                    if (container.GetZonesCount() != 8)
                        throw new Exception("Native raid map did not load eight tiers.");
                    targets = SelectTargets(scripts.Content);
                    raidPrepared = true;
                    return;
                }
                if (typeof(ModRuntime).GetMethod("ReadyProgressionMap", HiddenStatic).Invoke(null, null) == null)
                {
                    PressBlockingStory();
                    if (EditorApplication.timeSinceStartup - lastReport > 15)
                    {
                        lastReport = EditorApplication.timeSinceStartup;
                        Debug.Log(Prefix + "Waiting for map readiness at boss " + targetIndex + ".");
                    }
                    return;
                }
                var encounter = ListSF.CHMCKGCDGCM(new FightIDS(scripts.Content.RuntimeFightId(Target.Id)));
                if (encounter == null || !UnderworldZonePolicy.IsRaidZone(encounter.Battle?.OAEIILGHJMG))
                    throw new Exception("Tier boss has no native raid encounter: " + Target.Id);
                bool immediate = GameUtils.StartFight(encounter, false, null, true, false);
                entryRequested = true;
                bool preparing = (bool)typeof(ModModeRuntime).GetProperty("HasPendingPreparation", HiddenStatic).GetValue(null);
                if (!immediate && !preparing && !StoryBus.FightEntries.HasPending)
                    throw new Exception("Tier boss entry was refused: " + Target.Id);
                Debug.Log(Prefix + "Requested boss " + (targetIndex + 1) + "/" + targets.Count + ": " + Target.Id);
                return;
            }
            var fight = Fight.GetCurrentFight();
            if (fight == null)
            {
                if (EditorApplication.timeSinceStartup - lastPress < 0.2) return;
                var presenter = UnityEngine.Object.FindObjectsOfType<ModStoryDialogPresenter>()
                    .FirstOrDefault(value => value != null && value.gameObject.activeInHierarchy);
                if (presenter == null) return;
                var dialog = typeof(ModStoryDialogPresenter).GetField("dialog", Hidden).GetValue(presenter) as StoryDialog;
                if (dialog == null || dialog.get_ButtonOK() == null || ++storyPresses > 24)
                    throw new Exception("Tier boss story has no usable native card: " + Target.Id);
                lastPress = EditorApplication.timeSinceStartup;
                PressStoryButton(dialog);
                return;
            }
            if (StoryBus.FightEntries.HasPending)
                throw new Exception("Tier boss retained its native story hold: " + Target.Id);
            var enemy = (Model)typeof(Fight).GetField("CKNCPOABFBO", Hidden).GetValue(fight);
            var player = (Model)typeof(Fight).GetField("_playerModel", Hidden).GetValue(fight);
            if (enemy == null || player == null) return;
            player.Parameters.set_IsImmortalityEnabled(true);
            if (fight.get_FightTimeInFrames() < 30) return;
            var live = fight.OGNINOBBHIG();
            if (live?.FightId?.ToString() != new FightIDS(scripts.Content.RuntimeFightId(Target.Id)).ToString() ||
                enemy.CLDMEJKGLBA() == null || player.CLDMEJKGLBA() == null)
                throw new Exception("Tier boss fight or native fighter rig differs: " + Target.Id);
            if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_TITAN_EQUIPMENT") == "1")
            {
                string[] names = { "de128:items/weapon/titans_desolator", "de128:items/armor/titans_form",
                    "de128:items/helm/titans_helm", "de128:items/ranged/titans_harpoon",
                    "de128:items/magic/titans_mind_throw" };
                var equipped = player.Parameters.PJNJIJIODHE().Select(item => item.Name).ToArray();
                if (names.Any(name => !equipped.Contains(name)))
                    throw new Exception("The native fighter lost saved Titan equipment: " + string.Join(",", equipped));
                var macros = player.CLDMEJKGLBA().BLFJJAEFKKP();
                int bodyNodes = macros.Count(node => node.GetName().StartsWith("TitanBody_", StringComparison.Ordinal));
                int headNodes = macros.Count(node => node.GetName().StartsWith("TitanHead_", StringComparison.Ordinal));
                if (bodyNodes != 8 || headNodes != 12)
                    throw new Exception("Titan's archived body or helm geometry fell back: body=" +
                        bodyNodes + " helm=" + headNodes);
                Debug.Log(Prefix + "Five saved Titan items loaded; 8 body and 12 helm macro nodes in the native rig.");
            }
            var location = (Location)typeof(Fight).GetField("_location", Hidden).GetValue(fight);
            int sprites = location?.layers?.Sum(layer => layer.ICDCIANNAAI == null ? 0 :
                layer.ICDCIANNAAI.GetComponentsInChildren<SpriteRenderer>(true).Length) ?? 0;
            if (location?.name != live.Location || sprites == 0)
                throw new Exception("Tier boss arena did not render: " + Target.Id +
                    " location=" + location?.name + " sprites=" + sprites);
            if (Target.Id.ToString() == "de128:fights/uw_boss_berstuuk_1")
            {
                var armor = enemy.Parameters.Armor;
                var helm = enemy.Parameters.Helm;
                if (armor?.Name != "de128:items/armor/berstuuk_form" ||
                    armor.ModelFileName != "de128:models/underworld/mdl_body_berstuuk_early" ||
                    helm?.Name != "de128:items/helm/berstuuk_mask" ||
                    helm.ModelFileName != "de128:models/underworld/mdl_head_berstuuk")
                    throw new Exception("Berstuuk's native body or mask model was not equipped.");
                var macros = enemy.CLDMEJKGLBA().BLFJJAEFKKP();
                int bodyNodes = macros.Count(node => node.GetName().StartsWith("MacroBerstuukBody-", StringComparison.Ordinal));
                int headNodes = macros.Count(node => node.GetName().StartsWith("MacroBerstuukHead-", StringComparison.Ordinal));
                if (bodyNodes == 0 || headNodes == 0)
                    throw new Exception("Berstuuk model geometry fell back to a generic rig: body=" +
                        bodyNodes + " head=" + headNodes);
                Debug.Log(Prefix + "Berstuuk's archived body and mask geometry loaded: " +
                    bodyNodes + " body nodes, " + headNodes + " mask nodes.");
            }
            Debug.Log(Prefix + "Boss " + Target.Id + " ran 30 frames at " + location.name +
                " with " + sprites + " arena sprites and " + storyPresses + " story presses.");
            typeof(Fight).GetMethod("SurrenderButtonCallback", Hidden).Invoke(fight, new object[] { null });
            surrenderRequested = true;
        }
        catch (Exception error) { Debug.LogError(Prefix + "FAIL " + error); Finish(1); }
    }

    static FightDefinition Target => targets[targetIndex];

    static List<FightDefinition> SelectTargets(ModContentCatalog content)
    {
        string requested = Environment.GetEnvironmentVariable("ECLIPSE_DE128_UNDERWORLD_TARGETS");
        if (!string.IsNullOrEmpty(requested))
        {
            var ids = requested.Split(',');
            bool single = Environment.GetEnvironmentVariable("ECLIPSE_DE128_UNDERWORLD_MATRIX") == "single";
            if (ids.Length != (single ? 1 : 32) || ids.Distinct(StringComparer.Ordinal).Count() != ids.Length)
                throw new Exception("The requested encounter matrix has an invalid fight list.");
            var storyFights = new List<FightDefinition>();
            foreach (string id in ids)
            {
                var fight = content.Fights.FirstOrDefault(value => value.Id.ToString() == id);
                if (fight == null) throw new Exception("Archived boss story has no live fight: " + id);
                storyFights.Add(fight);
                Debug.Log(Prefix + "Selected story boss " + fight.Id);
            }
            return storyFights;
        }
        var zones = content.Zones.Where(value => value.Underworld && value.Id.Namespace.Value == "de128")
            .OrderBy(value => value.LegacyName, StringComparer.Ordinal).ToArray();
        if (zones.Length != 8) throw new Exception("Expected eight registered Underworld zones.");
        var result = new List<FightDefinition>();
        if (Environment.GetEnvironmentVariable("ECLIPSE_DE128_UNDERWORLD_MATRIX") == "all")
        {
            foreach (var zone in zones)
                foreach (var battleId in zone.Battles)
                {
                    if (!content.TryGetBattle(battleId, out var battle))
                        throw new Exception("Underworld zone has no live battle: " + battleId);
                    foreach (var fightId in battle.Fights)
                    {
                        if (!content.TryGetFight(fightId, out var fight))
                            throw new Exception("Underworld battle has no live fight: " + fightId);
                        result.Add(fight);
                        Debug.Log(Prefix + "Selected " + zone.Id + " -> " + fight.Id);
                    }
                }
            if (result.Count != 76 || result.Select(value => value.Id).Distinct().Count() != 76)
                throw new Exception("Expected 76 distinct native Underworld fights, got " + result.Count);
            return result;
        }
        foreach (var zone in zones)
        {
            BattleDefinition boss = null;
            foreach (var id in zone.Battles)
            {
                if (!content.TryGetBattle(id, out var candidate) || candidate.Kind != ModBattleKind.Final ||
                    candidate.PowerMode == ModPowerMode.Power || candidate.Fights.Count == 0) continue;
                boss = candidate;
                break;
            }
            if (boss == null || !content.TryGetFight(boss.Fights[0], out var fight))
                throw new Exception("Underworld tier has no normal-mode boss fight: " + zone.Id);
            result.Add(fight);
            Debug.Log(Prefix + "Selected " + zone.Id + " -> " + fight.Id);
        }
        return result;
    }

    static void PressBlockingStory()
    {
        if (EditorApplication.timeSinceStartup - lastPress < 0.2) return;
        var active = typeof(DialogsManager).GetField("OALIPPPOHCL", HiddenStatic).GetValue(null) as StoryDialog;
        if (active == null || !active.IsQuestDialog || active.get_ButtonOK() == null) return;
        lastPress = EditorApplication.timeSinceStartup;
        PressStoryButton(active);
    }

    static void PressStoryButton(StoryDialog dialog)
    {
        typeof(StoryDialog).GetMethod("GPEKKGLDKDF", Hidden).Invoke(dialog, new object[] { null });
    }

    static ModStoryEvents StoryBus => (ModStoryEvents)typeof(ModRuntime)
        .GetField("StoryEvents", HiddenStatic).GetValue(null);

    static void CaptureCombatException(string message, string stack, LogType type)
    {
        if (type != LogType.Exception || Fight.GetCurrentFight() == null || combatException != null ||
            string.IsNullOrEmpty(stack)) return;
        if (stack.Contains("Fight.RenderFight") || stack.Contains("FightScene.FixedUpdate") ||
            stack.Contains("Model.") || stack.Contains("ModelAi.") || stack.Contains("Location."))
            combatException = message + "\n" + stack;
    }

    static void Finish(int code)
    {
        SessionState.SetBool(Active, false);
        EditorApplication.update -= Update;
        Application.logMessageReceived -= CaptureCombatException;
        EditorApplication.Exit(code);
    }
}

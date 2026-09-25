using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Eclipse.Modding;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Dialogs;
using Nekki.SF2.GUI.Fight;
using Nekki.SF2.GUI.Map;
using Nekki.SF2.GUI.Menu;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Runs only in the isolated project made by TestDE128UnderworldNative.py --win.
[InitializeOnLoad]
public static class ValidateDE128UnderworldWinNative
{
    const string Active = "Eclipse.DE128UnderworldWinNative.Active";
    const string Prefix = "[DE128UnderworldWinNative] ";
    static readonly BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static readonly BindingFlags HiddenStatic = BindingFlags.Static | BindingFlags.NonPublic;
    static double started, lastPress, lastWin;
    static bool campaign, mapRequested, raidPrepared, entryRequested, resultSeen, continued;
    static int entryCards, resultCards, winCalls;
    static string targetId, bossKey;
    static long gemsBefore;

    static ValidateDE128UnderworldWinNative()
    {
        if (!SessionState.GetBool(Active, false)) return;
        SetTarget();
        started = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
    }

    public static void RunEditor()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "de128-underworld-fixture.marker")))
            throw new Exception("Underworld win acceptance requires an isolated project copy.");
        SetTarget();
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT", Path.Combine(root, "Mods"));
        PlayerSettings.companyName = "EclipseAcceptance";
        string profileTag = Environment.GetEnvironmentVariable("ECLIPSE_DE128_UNDERWORLD_PROFILE_TAG");
        PlayerSettings.productName = Path.GetFileName(root) + "-win-" +
            (string.IsNullOrEmpty(profileTag) ? bossKey : profileTag);
        SessionState.SetBool(Active, true);
        EditorSceneManager.OpenScene("Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        EditorApplication.EnterPlaymode();
    }

    static void SetTarget()
    {
        targetId = Environment.GetEnvironmentVariable("ECLIPSE_DE128_UNDERWORLD_TARGETS");
        if (string.IsNullOrEmpty(targetId) || !targetId.StartsWith("de128:fights/uw_boss_", StringComparison.Ordinal) ||
            targetId.Contains(",") || !targetId.EndsWith("_1", StringComparison.Ordinal))
            throw new Exception("Underworld win acceptance requires one boss fight ID.");
        bossKey = targetId.Substring("de128:fights/uw_".Length);
        bossKey = bossKey.Substring(0, bossKey.Length - 2);
    }

    static void Update()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            if (EditorApplication.timeSinceStartup - started > 240)
                throw new Exception("Timed out: entry=" + entryRequested + " result=" + resultSeen +
                    " continued=" + continued + " wins=" + winCalls + " cards=" + resultCards +
                    " screen=" + Module.GetInstance()?.NMCNDOPKFJD());
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
            if (!entryRequested && module.NMCNDOPKFJD() == ScreenType.ModuleMap)
            {
                var scene = UnityEngine.Object.FindObjectOfType<MapScene>();
                if (scene == null) return;
                if (!raidPrepared)
                {
                    scene.SwitchToRaidMap();
                    var container = (MapContainer)typeof(MapScene).GetField("_storyContainer", Hidden).GetValue(scene);
                    if (container.GetZonesCount() != 8) throw new Exception("Eight Underworld tiers did not load.");
                    raidPrepared = true;
                    return;
                }
                if (typeof(ModRuntime).GetMethod("ReadyProgressionMap", HiddenStatic).Invoke(null, null) == null)
                {
                    PressBlockingStory();
                    return;
                }
                var target = DefinitionId.Parse(targetId);
                if (ModProfileAccess.Fight(target).Wins != 0)
                    throw new Exception("Native win fixture profile has already won " + targetId);
                var encounter = ListSF.CHMCKGCDGCM(new FightIDS(scripts.Content.RuntimeFightId(target)));
                if (encounter == null) throw new Exception("Underworld encounter missing: " + targetId);
                gemsBefore = roster.EHFJHFDACMP();
                bool immediate = GameUtils.StartFight(encounter, false, null, true, false);
                entryRequested = true;
                bool preparing = (bool)typeof(ModModeRuntime).GetProperty("HasPendingPreparation", HiddenStatic).GetValue(null);
                if (!immediate && !preparing && !StoryBus.FightEntries.HasPending)
                    throw new Exception("Underworld entry refused: " + targetId);
                Debug.Log(Prefix + "Requested " + targetId + " with " + gemsBefore + " gems.");
                return;
            }
            if (!entryRequested) return;
            var resultScreen = UnityEngine.Object.FindObjectOfType<EndFightScreen>();
            if (resultScreen != null && !resultSeen)
            {
                var result = (FightResult)typeof(EndFightScreen).GetField("MPFLHOFEOGI", Hidden).GetValue(resultScreen);
                if (result == null || !result.IsWinner()) throw new Exception("Underworld victory did not reach a winning result screen.");
                resultSeen = true;
                Debug.Log(Prefix + "Native victory screen: coins=" + result.PMIHPJFAJIO.GBGNFPNCGED +
                    " gems=" + result.PMIHPJFAJIO.PNDAIFALIKF + " experience=" + result.PMIHPJFAJIO.exp +
                    " items=" + result.PMIHPJFAJIO.HELFDCAIJNE.Count);
            }
            if (resultSeen && !continued)
            {
                resultScreen.OnBackKeyClicked(null);
                continued = true;
                return;
            }
            if (continued)
            {
                if (module.NMCNDOPKFJD() != ScreenType.ModuleMap) return;
                var scene = UnityEngine.Object.FindObjectOfType<MapScene>();
                if (scene == null || scene.GetCurrentState() != MapScene.NMFLNANKNOJ.RaidMode) return;
                if (ModProfileAccess.Fight(DefinitionId.Parse(targetId)).Wins != 1)
                    throw new Exception("Native Underworld victory was not saved as one win.");
                if (EditorApplication.timeSinceStartup - lastPress >= 0.2 && PressResultStory())
                {
                    lastPress = EditorApplication.timeSinceStartup;
                    resultCards++;
                    return;
                }
                var state = scripts.State.Snapshot(ModId.Parse("de128"));
                if (!state.TryGetValue("uw_win_shown", out var shown) ||
                    !shown.String.Contains("," + bossKey + ",")) return;
                if (state.TryGetValue("uw_win_pending", out var pending) &&
                    pending.String.Contains("," + bossKey + ","))
                    throw new Exception("Underworld victory story remained pending after acknowledgement.");
                if (resultCards == 0) throw new Exception("Underworld victory story did not appear natively.");
                Debug.Log(Prefix + "PASS: native win, reward screen, saved fight win, " + resultCards +
                    " result cards, Underworld map return; gems=" + gemsBefore + "->" + roster.EHFJHFDACMP() + ".");
                Finish(0);
                return;
            }
            var fight = Fight.GetCurrentFight();
            if (fight == null)
            {
                if (StoryBus.FightEntries.HasPending && EditorApplication.timeSinceStartup - lastPress >= 0.2 &&
                    PressEntryStory())
                {
                    lastPress = EditorApplication.timeSinceStartup;
                    entryCards++;
                }
                return;
            }
            var player = (Model)typeof(Fight).GetField("_playerModel", Hidden).GetValue(fight);
            if (player == null) return;
            player.Parameters.set_IsImmortalityEnabled(true);
            if (fight.get_FightTimeInFrames() < 30 || EditorApplication.timeSinceStartup - lastWin < 1) return;
            lastWin = EditorApplication.timeSinceStartup;
            if (fight.DebugDefeatOpponent()) Debug.Log(Prefix + "Native opponent defeat " + (++winCalls));
        }
        catch (Exception error) { Debug.LogError(Prefix + "FAIL " + error); Finish(1); }
    }

    static bool PressEntryStory()
    {
        var presenter = UnityEngine.Object.FindObjectsOfType<ModStoryDialogPresenter>()
            .FirstOrDefault(value => value != null && value.gameObject.activeInHierarchy);
        var dialog = presenter == null ? null : typeof(ModStoryDialogPresenter)
            .GetField("dialog", Hidden).GetValue(presenter) as StoryDialog;
        if (dialog == null || dialog.get_ButtonOK() == null || entryCards > 24) return false;
        Press(dialog);
        return true;
    }

    static bool PressResultStory()
    {
        var presenter = UnityEngine.Object.FindObjectsOfType<ModStoryDialogPresenter>()
            .FirstOrDefault(value => value != null && value.gameObject.activeInHierarchy);
        var dialog = presenter == null ? null : typeof(ModStoryDialogPresenter)
            .GetField("dialog", Hidden).GetValue(presenter) as StoryDialog;
        if (dialog == null || dialog.get_ButtonOK() == null || resultCards > 24) return false;
        Press(dialog);
        return true;
    }

    static void PressBlockingStory()
    {
        if (EditorApplication.timeSinceStartup - lastPress < 0.2) return;
        var dialog = typeof(DialogsManager).GetField("OALIPPPOHCL", HiddenStatic).GetValue(null) as StoryDialog;
        if (dialog == null || !dialog.IsQuestDialog || dialog.get_ButtonOK() == null) return;
        lastPress = EditorApplication.timeSinceStartup;
        Press(dialog);
    }

    static void Press(StoryDialog dialog) =>
        typeof(StoryDialog).GetMethod("GPEKKGLDKDF", Hidden).Invoke(dialog, new object[] { null });

    static ModStoryEvents StoryBus => (ModStoryEvents)typeof(ModRuntime)
        .GetField("StoryEvents", HiddenStatic).GetValue(null);

    static void Finish(int code)
    {
        SessionState.SetBool(Active, false);
        EditorApplication.update -= Update;
        EditorApplication.Exit(code);
    }
}

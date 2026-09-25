using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Eclipse.Modding;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Fight;
using Nekki.SF2.GUI.Menu;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class ValidateDE128TitanWinNative
{
    const string Active = "Eclipse.DE128TitanWinNative.Active";
    const string Prefix = "[DE128TitanWinNative] ";
    static readonly BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    static readonly string[] Names = {
        "de128:items/weapon/titans_desolator", "de128:items/armor/titans_form",
        "de128:items/helm/titans_helm", "de128:items/ranged/titans_harpoon",
        "de128:items/magic/titans_mind_throw"
    };
    static double started, lastWin;
    static bool campaign, mapRequested, fightRequested, resultSeen, resultContinued;
    static int winCalls;

    static ValidateDE128TitanWinNative()
    {
        if (!SessionState.GetBool(Active, false)) return;
        started = EditorApplication.timeSinceStartup;
        EditorApplication.update += Update;
    }

    public static void RunEditor()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "de128-titan-reward-fixture.marker")))
            throw new Exception("Titan win acceptance requires an isolated project copy.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT", Path.Combine(root, "Mods"));
        PlayerSettings.companyName = "EclipseAcceptance";
        string profileTag = Environment.GetEnvironmentVariable("ECLIPSE_DE128_TITAN_PROFILE_TAG");
        PlayerSettings.productName = Path.GetFileName(root) + "-" +
            (string.IsNullOrEmpty(profileTag) ? "titanwin" : profileTag);
        SessionState.SetBool(Active, true);
        EditorSceneManager.OpenScene("Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        EditorApplication.EnterPlaymode();
    }

    static void Update()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            if (EditorApplication.timeSinceStartup - started > 240)
                throw new Exception("Timed out: fight=" + fightRequested + " result=" + resultSeen +
                    " calls=" + winCalls + " screen=" + Module.GetInstance()?.NMCNDOPKFJD());
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
                throw new Exception("DE128 diagnostics: " + string.Join("; ", scripts.Diagnostics));
            if (!mapRequested && module.NMCNDOPKFJD() == ScreenType.ModuleDojo)
            {
                var menu = MainMenu.get_Instance();
                if (menu == null) return;
                roster.Level = 52;
                roster.SetEclipseMode(true);
                menu.SkipTutorial();
                mapRequested = true;
                Debug.Log(Prefix + "Advanced profile entered Eclipse mode at level 52.");
                return;
            }
            if (!fightRequested && module.NMCNDOPKFJD() == ScreenType.ModuleMap)
            {
                if (Names.Any(name => roster.KHCNHPCPFII().CMGOCLGHNLH(name) != null))
                    throw new Exception("Titan victory acceptance needs a profile without prior reward items.");
                var id = DefinitionId.Parse("core:fights/zone_7/c3_boss_titan_eclipsemode/6");
                var encounter = ListSF.CHMCKGCDGCM(new FightIDS(scripts.Content.RuntimeFightId(id)));
                if (encounter == null) throw new Exception("Final Eclipse Titan fight missing.");
                Debug.Log(Prefix + "Starting " + id + ", Eclipse=" + roster.IsEclipseMode());
                GameUtils.StartFight(encounter, false, null, true, false);
                fightRequested = true;
                return;
            }
            if (!fightRequested) return;
            var resultScreen = UnityEngine.Object.FindObjectOfType<EndFightScreen>();
            if (resultScreen != null && !resultSeen)
            {
                var result = (FightResult)typeof(EndFightScreen).GetField("MPFLHOFEOGI", Hidden).GetValue(resultScreen);
                var grants = result.PMIHPJFAJIO.HELFDCAIJNE;
                if (!result.IsWinner() || grants.Count != 5 ||
                    !grants.Select(value => value.NAIEGGHELIH.Name).SequenceEqual(Names))
                    throw new Exception("Victory screen has wrong five-item prize: " +
                        string.Join(",", grants.Select(value => value.NAIEGGHELIH.Name)));
                resultSeen = true;
                Debug.Log(Prefix + "Actual victory screen projects five ordered Titan items.");
            }
            if (resultSeen && !resultContinued)
            {
                resultScreen.OnBackKeyClicked(null);
                resultContinued = true;
                Debug.Log(Prefix + "Continued the native victory screen.");
                return;
            }
            if (resultContinued)
            {
                if (module.NMCNDOPKFJD() != ScreenType.ModuleMap &&
                    module.NMCNDOPKFJD() != ScreenType.ModuleDojo) return;
                var inventory = roster.KHCNHPCPFII();
                if (Names.Any(name => inventory.CMGOCLGHNLH(name) == null))
                    throw new Exception("Actual victory did not settle all Titan items.");
                Debug.Log(Prefix + "PASS: native Eclipse Titan victory screen, five grants and inventory settlement.");
                Finish(0);
                return;
            }
            var fight = Fight.GetCurrentFight();
            if (fight == null || EditorApplication.timeSinceStartup - lastWin < 1.0 ||
                fight.get_FightTimeInFrames() < 30) return;
            lastWin = EditorApplication.timeSinceStartup;
            if (fight.DebugDefeatOpponent())
                Debug.Log(Prefix + "Triggered native win transition " + (++winCalls));
        }
        catch (Exception error) { Debug.LogError(Prefix + "FAIL " + error); Finish(1); }
    }

    static void Finish(int code)
    {
        SessionState.SetBool(Active, false);
        EditorApplication.update -= Update;
        EditorApplication.Exit(code);
    }
}

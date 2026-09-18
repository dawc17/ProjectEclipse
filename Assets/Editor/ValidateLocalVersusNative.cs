using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml;
using Eclipse.Multiplayer;
using Nekki.SF2.Core.Fights.Controller;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

// Invoke Prepare(), enter Play Mode, then Run() through the connected Editor.
// This Editor-only validator never ships. Hardware polling is tested separately; the native run
// supplies control events directly and temporarily disables disconnect monitoring.
public static class ValidateLocalVersusNative
{
    const BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    const string PreviousStartScene = "Eclipse.LocalValidation.PreviousStartScene";
    const string Prepared = "Eclipse.LocalValidation.Prepared";
    static int step, round, checks;
    static int captureFrame, movementFrame;
    static float firstX, secondX;
    static double deadline, lastReport;
    static string failure, baseline;
    static Model firstPlayer;
    static Fight firstFight;
    static float previousTimeScale;
    static SaveStamp[] saveStamps;
    static ListSF localProfileOwner;
    static double returnedAt;
    static string Folder => Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Temp", "LocalVersusNative");

    public static object Prepare()
    {
        if (EditorApplication.isPlaying || EditorApplication.isCompiling)
            throw new InvalidOperationException("The Editor must be idle before validation.");
        Directory.CreateDirectory(Folder);
        if (!SessionState.GetBool(Prepared, false))
            SessionState.SetString(PreviousStartScene, EditorSceneManager.playModeStartScene == null ? "" :
                AssetDatabase.GetAssetPath(EditorSceneManager.playModeStartScene));
        SessionState.SetBool(Prepared, true);
        EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(
            "Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        return new { prepared = true, authoringScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path };
    }

    public static object Run()
    {
        if (!EditorApplication.isPlaying || !SessionState.GetBool(Prepared, false))
            throw new InvalidOperationException("Prepare, enter Play Mode, then call Run.");
        step = checks = 0;
        failure = baseline = null;
        previousTimeScale = Time.timeScale;
        deadline = EditorApplication.timeSinceStartup + 240;
        File.WriteAllText(Path.Combine(Folder, "result.txt"), "RUNNING\n");
        Application.logMessageReceived += CaptureException;
        EditorApplication.update += Tick;
        var gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        if (gameViewType != null) EditorWindow.GetWindow(gameViewType).Focus();
        return new { running = true, report = Path.Combine(Folder, "result.txt") };
    }

    static void Tick()
    {
        if (!EditorApplication.isPlaying) { Detach(); Restore(); return; }
        try
        {
            if (failure != null) throw new Exception(failure);
            if (EditorApplication.timeSinceStartup > deadline) throw new TimeoutException("Native validation timed out at step " + step);
            if (EditorApplication.timeSinceStartup - lastReport > 10)
            {
                lastReport = EditorApplication.timeSinceStartup;
                File.AppendAllText(Path.Combine(Folder, "result.txt"), "Waiting at step " + step + "\n");
            }
            var fight = Fight.GetCurrentFight();
            switch (step)
            {
                case 0:
                    if (!Eclipse.UI.TitleScreen.IsOpen) return;
                    var title = UnityEngine.Object.FindFirstObjectByType<Eclipse.UI.TitleScreen>();
                    var multiplayer = title.GetComponentsInChildren<UnityEngine.UI.Button>().First(button =>
                        button.GetComponentInChildren<UnityEngine.UI.Text>().text == "MULTIPLAYER");
                    Check(multiplayer.interactable, "Multiplayer title entry enabled");
                    // Resolve the same paths as the loader before it parses the
                    // profile. Initializing these paths only creates directories.
                    SF2Paths.Init();
                    saveStamps = new[] { new SaveStamp(ListSF.PFMBKJMEDEF()), new SaveStamp(ListSF.IDIFECNLMKO()) };
                    multiplayer.onClick.Invoke();
                    step = 1;
                    break;
                case 1:
                    if (!LocalVersusSession.IsReady) return;
                    Check(LocalVersusSession.IsActive && LocalVersusMenu.Ensure().IsShowing, "Local boot reached lobby");
                    Check(Fight.GetCurrentFight() == null, "Local boot did not enter a campaign fight");
                    CheckSaves("Local startup");
                    localProfileOwner = ListSF.GetInstance();
                    localProfileOwner.EJANJEEGOOE();
                    localProfileOwner.OnAuthenticate(true);
                    CheckSaves("Forced save during local play");
                    baseline = ProfileXml();
                    File.WriteAllText(Path.Combine(Folder, "profile-before.xml"), baseline);
                    UnityEngine.Object.FindFirstObjectByType<LocalVersusSession>().enabled = false;
                    Capture("lobby");
                    step = 10;
                    break;
                case 10:
                    if (Time.frameCount <= captureFrame + 2) return;
                    Time.timeScale = 4f;
                    Launch("WEAPON_KNIVES", "WEAPON_STAFF", "dojo", 2);
                    step = 2;
                    break;
                case 2:
                    if (!Ready(fight)) return;
                    firstFight = fight;
                    firstPlayer = fight.GetPlayerModel();
                    CheckFighters(fight);
                    Check(UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Length == 1,
                        "Exactly one fight EventSystem");
                    Check(ProfileXml() == baseline, "Constructing native local fighters preserves campaign XML");
                    ExerciseRouting(fight);
                    firstX = fight.GetPlayerModel().PLBNCDCFPML().GetX();
                    secondX = fight.GetEnemyModel().PLBNCDCFPML().GetX();
                    SendControl(0, FightCID.QuadrantForward, true);
                    SendControl(1, FightCID.QuadrantBack, true);
                    movementFrame = fight.get_FightTimeInFrames();
                    step = 11;
                    break;
                case 11:
                    if (!Ready(fight) || fight.get_FightTimeInFrames() < movementFrame + 60) return;
                    SendControl(0, FightCID.QuadrantForward, false);
                    SendControl(1, FightCID.QuadrantBack, false);
                    Check(fight.GetPlayerModel().PLBNCDCFPML().GetX() > firstX &&
                        fight.GetEnemyModel().PLBNCDCFPML().GetX() < secondX,
                        "Both native fighters move toward each other from their own controls");
                    Capture("hud");
                    step = 12;
                    break;
                case 12:
                    if (Time.frameCount <= captureFrame + 2) return;
                    LocalVersusSession.Pause("Native validation pause");
                    Check(fight.IsPaused() && LocalVersusMenu.Ensure().IsShowing, "Native pause opens overlay");
                    LocalVersusMenu.Ensure().ShowLobby();
                    LocalVersusMenu.Ensure().ShowPause("Native validation repeated overlay");
                    Check(LocalVersusMenu.Ensure().GetComponentsInChildren<UnityEngine.UI.Image>(true)
                        .All(image => image.GetComponents<UnityEngine.UI.Image>().Length == 1), "Repeated overlays keep one Image per object");
                    LocalVersusMenu.Ensure().Hide();
                    fight.SetPaused(false);
                    round = fight.get_RoundNumber();
                    step = 3;
                    break;
                case 3:
                    if (!Ready(fight)) return;
                    if (!Kill(fight, true)) return;
                    step = 4;
                    break;
                case 4:
                    if (!Ready(fight) || fight.get_RoundNumber() <= round) return;
                    Check(fight.GetPlayerModel().Parameters.RoundsWon == 0 &&
                        fight.GetEnemyModel().Parameters.RoundsWon == 1, "Player two wins a native knockout round");
                    CheckFighters(fight);
                    if (!Kill(fight, true)) return;
                    step = 5;
                    break;
                case 5:
                    if (!LocalVersusSession.HasResult) return;
                    Check(fight.GetEnemyModel().Parameters.RoundsWon == 2, "First-to-two match completes with player two winning");
                    Check(LocalVersusMenu.Ensure().GetComponentsInChildren<UnityEngine.UI.Text>()
                        .Any(label => label.text == "PLAYER 2 WINS"), "Player two result text displayed");
                    Check(ProfileXml() == baseline, "Local result preserves campaign XML");
                    Capture("result");
                    step = 13;
                    break;
                case 13:
                    if (Time.frameCount <= captureFrame + 2) return;
                    Launch("WEAPON_KATANA", "Fists", "autumn", 1);
                    step = 6;
                    break;
                case 6:
                    if (!Ready(fight) || fight == firstFight) return;
                    Check(fight.GetPlayerModel() != firstPlayer, "Rematch uses fresh native models");
                    Check(fight.GetPlayerModel().Parameters.RoundsWon == 0 &&
                        fight.GetEnemyModel().Parameters.RoundsWon == 0, "Rematch resets both scores");
                    Check(fight.GetPlayerModel().Parameters.Weapon.Name == "WEAPON_KATANA", "Changed weapon applied on rematch");
                    CheckFighters(fight);
                    round = fight.get_RoundNumber();
                    // Force the real timer boundary with equal health, then let RenderRound decide.
                    SetTimerExpired(fight);
                    step = 7;
                    break;
                case 7:
                    if (!Ready(fight) || fight.get_RoundNumber() <= round) return;
                    Check(!LocalVersusSession.HasResult && fight.GetPlayerModel().Parameters.RoundsWon == 0 &&
                        fight.GetEnemyModel().Parameters.RoundsWon == 0, "Equal timeout replays with neither player awarded a round");
                    fight.GetEnemyModel().GFNCMLFKBGP(fight.GetEnemyModel().Parameters.CIDCNCDFONA * .5f);
                    SetTimerExpired(fight);
                    step = 8;
                    break;
                case 8:
                    if (!LocalVersusSession.HasResult) return;
                    Check(fight.GetPlayerModel().Parameters.RoundsWon == 1 &&
                        fight.GetEnemyModel().Parameters.RoundsWon == 0, "Higher-health player one wins native timeout");
                    Check(ProfileXml() == baseline, "Rounds and rematch preserve campaign XML");
                    Launch("Fists", "WEAPON_KNIVES", "bamboo_grove", 1);
                    step = 14;
                    break;
                case 14:
                    if (!Ready(fight) || LocalVersusSession.HasResult) return;
                    round = fight.get_RoundNumber();
                    Check(Kill(fight, true) && Kill(fight, false), "Both native fighters enter knockout in the same simulation frame");
                    step = 15;
                    break;
                case 15:
                    if (!Ready(fight) || fight.get_RoundNumber() <= round) return;
                    Check(fight.GetPlayerModel().Parameters.RoundsWon == 0 &&
                        fight.GetEnemyModel().Parameters.RoundsWon == 0 && !LocalVersusSession.HasResult,
                        "Double knockout replays without awarding a round");
                    fight.GetPlayerModel().GFNCMLFKBGP(fight.GetPlayerModel().Parameters.CIDCNCDFONA * .5f);
                    SetTimerExpired(fight);
                    step = 16;
                    break;
                case 16:
                    if (!LocalVersusSession.HasResult) return;
                    Check(fight.GetPlayerModel().Parameters.RoundsWon == 0 &&
                        fight.GetEnemyModel().Parameters.RoundsWon == 1,
                        "Higher-health player two wins native timeout in Bamboo grove");
                    Check(ProfileXml() == baseline, "All local match outcomes preserve campaign XML");
                    CheckSaves("All local match outcomes");
                    File.WriteAllText(Path.Combine(Folder, "profile-after.xml"), ProfileXml());
                    LocalVersusSession.ReturnToTitle();
                    step = 9;
                    break;
                case 9:
                    if (!Eclipse.UI.TitleScreen.IsOpen || LocalVersusSession.IsActive) return;
                    Check(Fight.GetCurrentFight() == null, "Return to title disposes native fight");
                    Check(UnityEngine.Object.FindFirstObjectByType<LocalVersusSession>() == null, "Return to title clears local session host");
                    returnedAt = EditorApplication.timeSinceStartup;
                    step = 17;
                    break;
                case 17:
                    if (EditorApplication.timeSinceStartup - returnedAt < 2) return;
                    // Stale references must retain transient ownership after the
                    // global local-mode flag has been reset at the title screen.
                    localProfileOwner.OnAuthenticate(true);
                    CheckSaves("Return to title and delayed save callback");
                    File.AppendAllText(Path.Combine(Folder, "result.txt"), "PASS: " + checks + " native local versus checks. Hardware polling was simulated separately.\n");
                    Debug.Log("[LocalVersusNative] PASS: " + checks + " checks.");
                    Time.timeScale = previousTimeScale;
                    Detach();
                    Restore();
                    break;
            }
        }
        catch (Exception exception)
        {
            failure = exception.ToString();
            File.AppendAllText(Path.Combine(Folder, "result.txt"), "FAIL at step " + step + ": " + failure + "\n");
            Detach();
            Time.timeScale = previousTimeScale;
            Restore();
            EditorApplication.isPaused = true;
            Debug.LogError("[LocalVersusNative] " + failure);
        }
    }

    static void Launch(string one, string two, string arena, int wins)
    {
        var settings = new LocalVersusSettings(one, two, arena, true, wins);
        var match = new LocalVersusMatch(settings);
        typeof(LocalVersusSession).GetProperty("Settings").GetSetMethod(true).Invoke(null, new object[] { settings });
        LocalVersusMenu.Ensure().Hide();
        typeof(Module).GetMethod("OpenLocalVersus", Hidden).Invoke(Module.GetInstance(), new object[] { match });
    }

    static bool Ready(Fight fight) => fight != null && fight.IsLocalVersus && fight.CONGPMFCIJM() &&
        !fight.IsPaused() && fight.get_FightTimeInFrames() > 10;

    static void CheckFighters(Fight fight)
    {
        var one = fight.GetPlayerModel();
        var two = fight.GetEnemyModel();
        Check(one.Parameters.IsPlayer && !two.Parameters.IsPlayer, "Fighter sides retain native identities");
        Check(one.Parameters.UserControlled && two.Parameters.UserControlled &&
            !one.Parameters.AiControlled && !two.Parameters.AiControlled, "Both fighters are controlled without AI");
        Check(one.FHBLLPCEAHG() != null && two.FHBLLPCEAHG() != null, "Both native fighters have animations");
        Check(!ReferenceEquals(one.Parameters, two.Parameters) &&
            !ReferenceEquals(one.Parameters.Armor, two.Parameters.Armor), "Fighters own separate parameters and equipment");
    }

    static void ExerciseRouting(Fight fight)
    {
        var controller = GameController.get_Current();
        var one = fight.GetPlayerModel().DEGJJOMLJGM();
        var two = fight.GetEnemyModel().DEGJJOMLJGM();
        one.Reset(); two.Reset();
        controller.CallEvent(0, new CBBEIGACPPD { Index = 1, KMOPCKPBHIA = FightCID.QuadrantDown });
        Check(two.FONEJOKEIEN.IGEEOAGOMEM.Contains((int)FightCID.QuadrantDown) && one.FONEJOKEIEN.IGEEOAGOMEM.Count == 0,
            "Player two control reaches only player two's native combo buffer");
        controller.CallEvent(1, new CBBEIGACPPD { Index = 1, KMOPCKPBHIA = FightCID.QuadrantDown });
        controller.CallEvent(0, new CBBEIGACPPD { Index = 0, KMOPCKPBHIA = FightCID.Punch });
        Check(one.FONEJOKEIEN.IGEEOAGOMEM.Contains((int)FightCID.Punch) &&
            !two.FONEJOKEIEN.IGEEOAGOMEM.Contains((int)FightCID.Punch), "Player one control remains independent");
        controller.CallEvent(1, new CBBEIGACPPD { Index = 0, KMOPCKPBHIA = FightCID.Punch });
        one.Reset(); two.Reset();
    }

    static bool Kill(Fight fight, bool playerOne) => (bool)typeof(Fight).GetMethod("KillModel", Hidden)
        .Invoke(fight, new object[] { playerOne, false });

    static void SendControl(int player, FightCID control, bool pressed) =>
        GameController.get_Current().CallEvent(pressed ? 0 : 1, new CBBEIGACPPD { Index = player, KMOPCKPBHIA = control });

    static void Capture(string name)
    {
        captureFrame = Time.frameCount;
        ScreenCapture.CaptureScreenshot(Path.Combine(Folder, name + ".png"));
    }

    static void SetTimerExpired(Fight fight)
    {
        var viewer = fight.preFight.get_ViewerFight();
        var field = typeof(Nekki.SF2.GUI.Fight.ViewerFight).GetField("ENKHHGEMJCK", Hidden);
        if (field == null) throw new InvalidOperationException("The native frame-count timer field is missing.");
        field.SetValue(viewer, (CodeStage.AntiCheat.ObscuredTypes.ObscuredInt)0);
        typeof(Nekki.SF2.GUI.Fight.ViewerFight).GetField("NFEMKPCLDDB", Hidden)
            .SetValue(viewer, (CodeStage.AntiCheat.ObscuredTypes.ObscuredInt)0);
    }

    static string ProfileXml() => ((XmlNode)typeof(MELBIBHDPCE).GetField("_node", Hidden)
        .GetValue(ListSF.CCDKHLAMKKO())).OuterXml;

    sealed class SaveStamp
    {
        readonly string path;
        readonly bool existed;
        readonly DateTime lastWrite;
        readonly byte[] hash;

        internal SaveStamp(string path)
        {
            this.path = path;
            existed = File.Exists(path);
            lastWrite = File.GetLastWriteTimeUtc(path);
            hash = Hash();
        }

        byte[] Hash()
        {
            if (!File.Exists(path)) return Array.Empty<byte>();
            using (var sha = SHA256.Create())
            using (var stream = File.OpenRead(path)) return sha.ComputeHash(stream);
        }

        internal void Verify(string phase)
        {
            Check(File.Exists(path) == existed && File.GetLastWriteTimeUtc(path) == lastWrite && hash.SequenceEqual(Hash()),
                phase + " leaves " + Path.GetFileName(path) + " contents and write time unchanged");
        }
    }

    static void CheckSaves(string phase)
    {
        foreach (var save in saveStamps) save.Verify(phase);
    }

    static void Check(bool condition, string description)
    {
        if (!condition) throw new Exception(description);
        checks++;
        File.AppendAllText(Path.Combine(Folder, "result.txt"), "PASS " + description + "\n");
    }

    static void CaptureException(string message, string trace, LogType kind)
    {
        if (kind == LogType.Exception && failure == null) failure = message + "\n" + trace;
    }

    static void Detach()
    {
        EditorApplication.update -= Tick;
        Application.logMessageReceived -= CaptureException;
    }

    public static object Restore()
    {
        if (SessionState.GetBool(Prepared, false))
        {
            string oldPath = SessionState.GetString(PreviousStartScene, "");
            EditorSceneManager.playModeStartScene = string.IsNullOrEmpty(oldPath) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(oldPath);
            SessionState.SetBool(Prepared, false);
        }
        return new { restored = true };
    }
}

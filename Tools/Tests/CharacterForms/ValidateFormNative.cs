using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Eclipse.Modding;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class ValidateFormNative
{
    const string Active = "Eclipse.FormNative.Active";
    static double started, lastReport;
    static bool campaign, entered;
    static Model original;
    static string combatException;
    static float originalRatio;
    static string originalWeapon;
    static bool originalPlayer, originalControlled, originalAi;
    static int previousFrame;
    const string Counter = "__EclipseFormNativeCounter";
    const string Phase = "__EclipseFormNativePhase";
    static int switchedAt = -1;
    static readonly BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;

    static ValidateFormNative()
    {
        if (SessionState.GetBool(Active, false))
        {
            started = EditorApplication.timeSinceStartup;
            EditorApplication.update += Update;
            Application.logMessageReceived += CaptureCombatException;
        }
    }

    public static void RunEditor()
    {
        string root = Directory.GetParent(Application.dataPath).FullName;
        if (!File.Exists(Path.Combine(root, "form-native-fixture.marker")))
            throw new InvalidOperationException("Native form validation requires an isolated fixture.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT", Path.Combine(root, "Mods"));
        PlayerSettings.companyName = "EclipseAcceptance";
        PlayerSettings.productName = Path.GetFileName(root);
        SessionState.SetBool(Active, true);
        EditorSceneManager.OpenScene("Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        EditorApplication.EnterPlaymode();
    }

    static void Update()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            if (combatException != null)
                throw new Exception("Native combat threw after initial body readiness:\n" + combatException);
            if (EditorApplication.timeSinceStartup - started > 240)
                throw new Exception("Timed out during " + (entered ? "native fight" : "game boot") + ".");
            if (EditorApplication.timeSinceStartup - lastReport > 15)
            {
                lastReport = EditorApplication.timeSinceStartup;
                Debug.Log("[FormNative] Waiting: campaign=" + campaign + " entered=" + entered + " scripts=" + (ModRuntime.Scripts != null));
            }
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
            if (!entered)
            {
                var scripts = ModRuntime.Scripts;
                if (scripts == null || ListSF.CCDKHLAMKKO() == null || Module.GetInstance() == null) return;
                var screen = Module.GetInstance().GetCurrentScreenType();
                if (screen != ScreenType.ModuleDojo && screen != ScreenType.ModuleMap) return;
                var definition = scripts.Content.Fights.FirstOrDefault(f => f.Id.ToString() == "example.shifting-guardian:fights/guardian");
                if (definition == null) throw new Exception("Shifting Guardian did not register.");
                var encounter = ListSF.CHMCKGCDGCM(new FightIDS(scripts.Content.RuntimeFightId(definition.Id)));
                if (encounter == null) throw new Exception("Native encounter projection is missing.");
                entered = GameUtils.StartFight(encounter, false, null, true, false);
                Debug.Log("[FormNative] StartFight=" + entered);
                return;
            }
            var fight = Fight.GetCurrentFight();
            if (fight == null) return;
            var enemy = (Model)typeof(Fight).GetField("CKNCPOABFBO", Hidden).GetValue(fight);
            var player = (Model)typeof(Fight).GetField("_playerModel", Hidden).GetValue(fight);
            if (enemy == null || player == null || fight.get_FightTimeInFrames() < 1) return;
            int frame = fight.get_FightTimeInFrames();
            if (original != null && frame < previousFrame)
                throw new Exception("Fight timer restarted during form replacement.");
            previousFrame = frame;
            player.Parameters.set_IsImmortalityEnabled(true);
            if (original == null)
            {
                if (enemy.Parameters.EclipseCharacterId != "example.shifting-guardian:warriors/staff")
                    throw new Exception("Wrong initial native character: " + enemy.Parameters.EclipseCharacterId);
                original = enemy;
                originalWeapon = WeaponName(enemy);
                if (string.IsNullOrEmpty(originalWeapon)) throw new Exception("Initial fighter has no weapon item.");
                originalPlayer = enemy.Parameters.IsPlayer;
                originalControlled = enemy.Parameters.UserControlled;
                originalAi = enemy.Parameters.AiControlled;
                enemy.EBABHGHPLFK().PerkVariables[Counter] = 17;
                enemy.EBABHGHPLFK().PerkStringVariables[Phase] = "shifting";
                enemy.GFNCMLFKBGP(enemy.Parameters.MaxLife * .75f);
                originalRatio = enemy.KKMCHCNOHMB() / enemy.Parameters.MaxLife;
                Debug.Log("[FormNative] Initial body ready; health ratio=" + originalRatio);
            }
            if (enemy != original && switchedAt < 0)
            {
                if (enemy.Parameters.EclipseCharacterId != "example.shifting-guardian:warriors/baton")
                    throw new Exception("Unexpected replacement character.");
                string weapon = WeaponName(enemy);
                if (string.IsNullOrEmpty(weapon) || weapon == originalWeapon)
                    throw new Exception("Native weapon equipment did not change.");
                if (enemy.Parameters.IsPlayer != originalPlayer ||
                    enemy.Parameters.UserControlled != originalControlled ||
                    enemy.Parameters.AiControlled != originalAi)
                    throw new Exception("Form replacement changed participant input/AI eligibility.");
                CheckVariables(enemy);
                float ratio = enemy.KKMCHCNOHMB() / enemy.Parameters.MaxLife;
                if (Math.Abs(ratio - originalRatio) > .001f) throw new Exception("Health ratio changed across form swap: " + ratio);
                switchedAt = fight.get_FightTimeInFrames();
                Debug.Log("[FormNative] Native body replaced at frame " + switchedAt + "; weapon=" +
                    originalWeapon + " -> " + weapon + "; health ratio=" + ratio);
            }
            if (switchedAt >= 0 && fight.get_FightTimeInFrames() >= switchedAt + 120)
            {
                if (enemy.CLDMEJKGLBA() == null)
                    throw new Exception("Replacement lost its native model object.");
                var animation = enemy.OCPMJKIEPIG();
                if (animation == null)
                    throw new Exception("Replacement lost its animation controller.");
                if (animation.NNMAFFCCMHC() == null)
                    throw new Exception("Replacement has no selected animation at frame " + frame + ".");
                var surface = enemy.MJNPBMOAFML();
                if (surface == null)
                    throw new Exception("Replacement lost its Unity object.");
                if (!surface.activeSelf || !surface.activeInHierarchy)
                    throw new Exception("Replacement is inactive at frame " + frame + ": activeSelf=" +
                        surface.activeSelf + ", activeInHierarchy=" + surface.activeInHierarchy + ".");
                CheckVariables(enemy);
                string feedback = ReadFormFeedback();
                if (!feedback.StartsWith("BATON FORM | Applied:", StringComparison.Ordinal))
                    throw new Exception("Lua receipt/HUD did not report application: " + feedback);
                Debug.Log("[FormNative] PASS: real game boot, registered encounter, Lua-requested native body and weapon replacement, health and variable continuity, unchanged input/AI eligibility, applied HUD and 120 subsequent combat frames without timer reset.");
                Finish(0);
            }
            else if (switchedAt < 0 && fight.get_FightTimeInFrames() > 420)
            {
                throw new Exception("Form did not apply: " + ReadFormFeedback());
            }
        }
        catch (Exception error)
        {
            Debug.LogError("[FormNative] FAIL: " + error);
            Finish(1);
        }
    }

    static void CaptureCombatException(string message, string stackTrace, LogType type)
    {
        if (original == null || combatException != null || type != LogType.Exception || string.IsNullOrEmpty(stackTrace))
            return;
        if (stackTrace.IndexOf("Fight.RenderFight", StringComparison.Ordinal) < 0 &&
            stackTrace.IndexOf("FightScene.FixedUpdate", StringComparison.Ordinal) < 0 &&
            stackTrace.IndexOf("Model.", StringComparison.Ordinal) < 0 &&
            stackTrace.IndexOf("ModelAi.", StringComparison.Ordinal) < 0)
            return;
        // Store only. Logging or exiting inside this callback can recurse; the
        // next editor update reports the original failure and shuts down safely.
        combatException = message + "\n" + stackTrace;
    }

    static string WeaponName(Model model)
    {
        return model.Parameters.DGMDEDKLGMB().FirstOrDefault(item => item != null && item.Type == "Weapon")?.Name;
    }

    static void CheckVariables(Model model)
    {
        var conditions = model.EBABHGHPLFK();
        if (conditions == null || !conditions.PerkVariables.TryGetValue(Counter, out float count) || count != 17 ||
            !conditions.PerkStringVariables.TryGetValue(Phase, out string phase) || phase != "shifting")
            throw new Exception("Numeric/text perk variables were lost across native body retirement.");
    }

    // Read the Lua-owned state as well as rendered text: an inactive/missing HUD
    // must not hide the original request failure from native acceptance logs.
    static string ReadFormFeedback()
    {
        var contexts = (System.Collections.IEnumerable)typeof(ModScriptSession)
            .GetField("_contexts", Hidden).GetValue(ModRuntime.Scripts);
        foreach (var context in contexts)
        {
            var property = context.GetType().GetProperty("UiScope");
            var scope = property?.GetValue(context) as ModUiScope;
            if (scope == null || scope.Owner.Value != "example.shifting-guardian") continue;
            var surfaces = (System.Collections.IDictionary)typeof(ModUiScope)
                .GetField("surfaces", Hidden).GetValue(scope);
            foreach (ModUiSurface surface in surfaces.Values)
                if (surface.Id == "shift")
                    return surface.Read("phase").Text + " | " + surface.Read("result").Text;
        }
        return "Shifting Guardian HUD state is absent.";
    }

    static void Finish(int code)
    {
        SessionState.SetBool(Active, false);
        EditorApplication.update -= Update;
        Application.logMessageReceived -= CaptureCombatException;
        EditorApplication.Exit(code);
    }
}

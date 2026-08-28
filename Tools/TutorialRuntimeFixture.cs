// Test-only Unity/UI fakes. Runtime method bodies are injected by TestTutorialRuntime.ps1.
using System;
using System.Collections.Generic;
using UnityEngine;
using Nekki.SF2.GUI;
using Nekki.SF2.Core.Tutorials;

namespace UnityEngine { public static class Debug { public static void LogWarning(string text) {} } }
namespace Nekki.SF2.GUI { public class Scene<T> { public static T Current; public static T get_Current() { return Current; } } }
namespace Nekki.SF2.Core.Tutorials {
    public class TutorialCanvas {
        public static TutorialCanvas Instance = new TutorialCanvas();
        public bool Blocked;
        public static TutorialCanvas get_Instance() { return Instance; }
        public void set_BlockOn(bool value) { Blocked = value; }
    }
}
public class QuestParameters {}
public class QuestAction {
    public int Completions, Cancellations;
    public virtual void DEJMHFMLKIC(QuestParameters p) {}
    public virtual void GKFMJKAAJCA() {}
    public void OGIJONMKABB() { Completions++; }
    public void PJGEOIKPGFH() { Cancellations++; }
}
public class TutorialComponent { public bool IsActive; }
public class ClickEvent {
    private event Action Clicked;
    public int Count { get { return Clicked == null ? 0 : Clicked.GetInvocationList().Length; } }
    public void AddListener(Action action) { Clicked += action; }
    public void RemoveListener(Action action) { Clicked -= action; }
    public void Invoke() { if (Clicked != null) Clicked(); }
}
public class LabelButton {
    public ClickEvent onClick = new ClickEvent();
    public TutorialComponent Component = new TutorialComponent();
    public bool Flashing;
    public void set_IsFlashing(bool value) { Flashing = value; }
    public T GetComponent<T>() where T : class { return Component as T; }
}
public enum SliderType { SliderTricks }
public enum SceneTypes { SceneFight, SceneProfile }
public class InfoAnimation { public string Name; }
public class Trick { public string Name; public InfoAnimation KJHMOGGECBN; }
public static class GameUtils {
    public static List<Trick> Tricks;
    public static SceneTypes RequestedScene;
    public static bool InputLocked;
    public static int Unlocks;
    public static List<Trick> KLLGJKHALGH(SceneTypes scene = SceneTypes.SceneFight) { RequestedScene = scene; return Tricks; }
    public static void FMICOICLCNL(bool visible) { InputLocked = true; }
    public static void KKNGFGMJKHG() { InputLocked = false; Unlocks++; }
}
public static class SubItem { public static bool Enabled = true; public static void EnableAnimation(bool value) { Enabled = value; } }
public static class Constants { public const int GFBLKELEBEH = 0; }
public class FakeObject { public void SetActive(bool value) {} }
public class FakePanel { public FakeObject gameObject = new FakeObject(); }
public class FakeGroup { public bool blocksRaycasts = true; public float alpha = 1; }
public class FakeBackground { public int color; }
public class Model {
    public class EventModel { public object Data; }
    public void PlayAnimationDelay(InfoAnimation animation) {}
}
public static class AnimationData {
    public static bool Available = true;
    public static InfoAnimation BCIFKBJAFEC(string name) { return Available ? new InfoAnimation { Name = name } : null; }
}
public class ModelContainer {
    private Model _playerModel = new Model();
    public int Resets;
    public void ResetModel() { Resets++; }
    /* TRY_PLAY_METHOD */
}
public class ProfileScene {
    public event Action<object> TrickPreviewCompleted;
    public event Action<object> ProfileClosing;
    public LabelButton Button = new LabelButton();
    public string Selected;
    public ModelContainer ModelContainer = new ModelContainer();
    private InfoAnimation MKGONDJABAH;
    private bool IBADMKPHOOJ, EIDKMLIOKOD, _trickPreviewActive;
    private bool OHDPMGDBCCF = true;
    private string _previewAnimationName;
    private float BKACEHDPGKC = 1;
    private FakePanel _leftPanel = new FakePanel();
    private FakeBackground _backgroundLeft = new FakeBackground(), _backgroundRight = new FakeBackground();
    private FakeGroup _profileUIGroup = new FakeGroup(), _bottomUIGroup = new FakeGroup();
    private List<object> JHFCFBIPGPF = new List<object>();
    public bool Clickable { get { return _profileUIGroup.blocksRaycasts && _bottomUIGroup.blocksRaycasts; } }
    public void ScrollToItemByName(SliderType type, string name) { Selected = name; }
    public LabelButton GetBtnStrikeShow() { return Button; }
    public void Show() { MKGONDJABAH = new InfoAnimation { Name = "HighBlockProfile" }; CGFOHBFAJBL(); Button.onClick.Invoke(); }
    public void Tick(int count = 40) { for (int i = 0; i < count; i++) HPHAOJDPNND(); }
    public void EndAnimation(string name) { LDFKBJAHGII(new Model.EventModel { Data = new InfoAnimation { Name = name } }); }
    public void Close() { ReleaseTrickPreviewInput(); if (ProfileClosing != null) ProfileClosing(null); }
    /* PROFILE_METHODS */
}
/* TUTORIAL_ACTION */

public static class TutorialRegression {
    private static int checks;
    private static void Assert(bool condition, string message) { if (!condition) throw new Exception(message); checks++; }
    private static ProfileScene Setup() {
        GameUtils.InputLocked = false;
        GameUtils.Unlocks = 0;
        AnimationData.Available = true;
        TutorialCanvas.Instance = new TutorialCanvas();
        GameUtils.Tricks = new List<Trick> {
            new Trick { Name = "WrongFirstMove", KJHMOGGECBN = new InfoAnimation { Name = "WrongFirstMove" } },
            new Trick { Name = "HighBlockProfile", KJHMOGGECBN = new InfoAnimation { Name = "HighBlockProfile" } }
        };
        var profile = new ProfileScene();
        Scene<ProfileScene>.Current = profile;
        return profile;
    }
    public static string Run() {
        var profile = Setup();
        var action = new QuestActionStoryTutorialShowBlock();
        action.DEJMHFMLKIC(new QuestParameters());
        Assert(GameUtils.RequestedScene == SceneTypes.SceneProfile && profile.Selected == "HighBlockProfile", "Must select the block preview, not the first fight move");
        Assert(TutorialCanvas.Instance.Blocked && profile.Button.Flashing && profile.Button.Component.IsActive, "Show must be the active tutorial control");
        profile.Show();
        Assert(!TutorialCanvas.Instance.Blocked && !profile.Button.Flashing && !profile.Button.Component.IsActive, "Show must release the persistent tutorial blocker");
        Assert(GameUtils.InputLocked && action.Completions == 0, "Preview alone owns input while playing");
        profile.Tick();
        profile.EndAnimation("Idle");
        profile.Tick();
        Assert(GameUtils.InputLocked && profile.ModelContainer.Resets == 0 && action.Completions == 0, "Idle animation must not finish the block demonstration");
        profile.EndAnimation("HighBlockProfile");
        profile.Tick();
        Assert(!GameUtils.InputLocked && profile.Clickable && SubItem.Enabled, "Completed preview must restore controls");
        Assert(action.Completions == 1 && profile.Button.onClick.Count == 0, "Tutorial must complete once and detach button listener");
        profile.EndAnimation("HighBlockProfile"); profile.Tick(); profile.Close();
        Assert(action.Completions == 1 && action.Cancellations == 0 && GameUtils.Unlocks == 1, "Duplicate end/close must not complete or unlock twice");

        foreach (int ticks in new[] { -1, 0, 40 }) {
            profile = Setup(); action = new QuestActionStoryTutorialShowBlock(); action.DEJMHFMLKIC(new QuestParameters());
            if (ticks >= 0) { profile.Show(); profile.Tick(ticks); }
            profile.Close();
            Assert(!TutorialCanvas.Instance.Blocked && !GameUtils.InputLocked, "Leaving Profile must release both locks, including during fade");
            Assert(action.Completions == 0 && action.Cancellations == 1, "Leaving must cancel without advancing tutorial on the dying scene");
            Assert(profile.Button.onClick.Count == 0 && !profile.Button.Component.IsActive, "Leaving must remove tutorial controls");
            profile.Close(); Assert(action.Cancellations == 1, "Close cleanup must be idempotent");
        }
        profile = Setup(); action = new QuestActionStoryTutorialShowBlock(); action.DEJMHFMLKIC(new QuestParameters());
        AnimationData.Available = false; profile.Show(); profile.Tick(100);
        Assert(!GameUtils.InputLocked && !TutorialCanvas.Instance.Blocked && profile.Clickable && action.Completions == 1, "Missing animation must restore UI without Escape");

        profile = Setup(); action = new QuestActionStoryTutorialShowBlock(); action.DEJMHFMLKIC(new QuestParameters());
        action.GKFMJKAAJCA();
        Assert(!TutorialCanvas.Instance.Blocked && profile.Button.onClick.Count == 0, "Reset must release lock and callbacks");
        action.DEJMHFMLKIC(new QuestParameters());
        Assert(profile.Button.onClick.Count == 1, "Restart must not accumulate callbacks");
        profile.Show(); profile.Tick(); profile.EndAnimation("HighBlockProfile"); profile.Tick();
        Assert(action.Completions == 1, "Reset action must be reusable");

        foreach (string missing in new[] { "profile", "animation", "button", "component" }) {
            profile = Setup();
            if (missing == "profile") Scene<ProfileScene>.Current = null;
            if (missing == "animation") GameUtils.Tricks.Clear();
            if (missing == "button") profile.Button = null;
            if (missing == "component") profile.Button.Component = null;
            action = new QuestActionStoryTutorialShowBlock(); action.DEJMHFMLKIC(new QuestParameters());
            Assert(!TutorialCanvas.Instance.Blocked && action.Completions == 1, "Missing " + missing + " must not leave an input lock");
        }
        return "PASS: " + checks + " tutorial/preview regression assertions (headless UI fakes; native playtest still required).";
    }
}

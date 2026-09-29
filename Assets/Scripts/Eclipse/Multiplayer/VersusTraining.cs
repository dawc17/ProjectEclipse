using System;
using System.Collections.Generic;
using Eclipse.Multiplayer.Online;
using Eclipse.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Multiplayer
{
    public enum DummyStance { Stand, Crouch, Jump, WalkIn, WalkAway }
    public enum DummyBlock { Never, Always, AfterFirstHit, Random }
    public enum DummyControl { Script, Cpu, Record, Playback }
    public enum TrainingHealth { Infinite, RefillAfterCombo, Normal }

    /// <summary>
    /// Training mode: the dummy's behaviour, the rules (health, clock), and what the
    /// readouts show. Settings are static so they survive restarts with a new loadout.
    /// </summary>
    public static class VersusTraining
    {
        public static bool Active { get; internal set; }
        /// <summary>A replay is showing the training readouts (no rules or dummy apply).</summary>
        public static bool Replay { get; internal set; }
        /// <summary>Hits, combos, frame advantage and move lengths are being measured.</summary>
        public static bool Observing => Active || Replay;

        public static DummyStance Stance = DummyStance.Stand;
        public static DummyBlock Block = DummyBlock.Never;
        public static DummyControl Control = DummyControl.Script;
        public static TrainingHealth Health = TrainingHealth.Infinite;
        public static int CpuLevel = 1;
        public static bool ShowInputs = true;
        public static bool ShowReadouts = true;
        public static int SpeedIndex = 2;

        public static readonly string[] CpuTactics = { "Beginner", "Standard", "Aggressive", "Sensei" };
        public static readonly string[] CpuNames = { "Beginner", "Standard", "Aggressive", "Master" };
        public static readonly float[] Speeds = { .25f, .5f, 1f };
        public const int MaxRecordingTicks = 600;
        public const int ComboGapTicks = 45;

        // Readouts. Damage is a percentage of the victim's full health.
        public static float LastDamage;
        public static bool LastBlocked, LastCritical;
        public static string LastMove = string.Empty;
        public static int ComboHits, BestCombo;
        public static float ComboDamage, BestComboDamage;
        public static int FrameAdvantage = int.MinValue;
        public static readonly List<string> Log = new List<string>();
        /// <summary>Per side (0 left, 1 right): the move now playing and how many ticks it has run.</summary>
        public static readonly string[] MoveName = { string.Empty, string.Empty };
        public static readonly int[] MoveTicks = new int[2];
        /// <summary>Per side: the last finished action (idle and walking loops are skipped) and its length in ticks.</summary>
        public static readonly string[] LastActionName = { string.Empty, string.Empty };
        public static readonly int[] LastActionTicks = new int[2];
        private static readonly int[] _moveFrame = new int[2];
        private static readonly bool[] _moveWasBusy = new bool[2];
        private static Model _comboAttacker;

        internal static readonly List<byte> Recording = new List<byte>();
        /// <summary>Random block: re-rolled every 40 ticks.</summary>
        internal static bool RandomGuard;
        /// <summary>After a hit, how long "block after first hit" keeps its guard up.</summary>
        public const int GuardAfterHitTicks = 90;
        internal static int LastDummyHitTick = -10000;
        internal static int LastHitTick = -10000;
        private static Model _measureAttacker, _measureVictim;
        private static int _measureStart = -1, _attackerFree = -1, _victimFree = -1;

        public static string CpuTactic => CpuTactics[Mathf.Clamp(CpuLevel, 0, CpuTactics.Length - 1)];

        internal static void ResetReadouts()
        {
            LastDamage = 0; LastBlocked = LastCritical = false; LastMove = string.Empty;
            ComboHits = 0; ComboDamage = 0; BestCombo = 0; BestComboDamage = 0;
            FrameAdvantage = int.MinValue; Log.Clear();
            LastDummyHitTick = LastHitTick = -10000;
            _measureStart = -1;
            _comboAttacker = null;
            for (int side = 0; side < 2; side++)
            {
                MoveName[side] = LastActionName[side] = string.Empty;
                MoveTicks[side] = LastActionTicks[side] = _moveFrame[side] = 0;
                _moveWasBusy[side] = false;
            }
        }

        /// <summary>Replays show the same readouts as training; the rules never apply.</summary>
        internal static void BeginReplay()
        {
            Replay = true;
            ResetReadouts();
            TrainingHud.Begin(true);
        }

        /// <summary>Who a readout means: "You" and "Dummy" in training, the players' names in a replay.</summary>
        internal static string SideName(int side)
        {
            if (!Replay) return side == 0 ? "You" : "Dummy";
            var settings = LocalVersusSession.Settings;
            return settings == null ? (side == 0 ? "P1" : "P2") : side == 0 ? settings.PlayerOneName : settings.PlayerTwoName;
        }

        /// <summary>Called from the fight when a hit lands (training and replays).</summary>
        public static void OnHit(Model attacker, Model victim, float damage, bool blocked, bool critical, string move)
        {
            var fight = Fight.GetCurrentFight();
            if (fight == null) return;
            // The victim is whoever loses the health. The event's attacker is not trusted:
            // paired grabs (throws) retarget it, which credited throws to the wrong side.
            attacker = victim == fight.GetPlayerModel() ? fight.GetEnemyModel() : fight.GetPlayerModel();
            int tick = VersusTickDriver.Tick;
            float full = victim != null && victim.Parameters != null ? victim.Parameters.CIDCNCDFONA : 0f;
            float percent = full > 0f ? damage * 100f / full : 0f;
            if (victim == fight.GetEnemyModel()) LastDummyHitTick = tick;
            // A combo is one attacker's unblocked hits, each within ComboGapTicks of the last.
            if (attacker != _comboAttacker || tick - LastHitTick > ComboGapTicks || blocked) { ComboHits = 0; ComboDamage = 0; }
            _comboAttacker = attacker;
            if (!blocked)
            {
                ComboHits++;
                ComboDamage += percent;
                if (ComboHits > BestCombo) BestCombo = ComboHits;
                if (ComboDamage > BestComboDamage) BestComboDamage = ComboDamage;
            }
            LastHitTick = tick;
            LastDamage = percent; LastBlocked = blocked; LastCritical = critical; LastMove = Pretty(move);
            Log.Insert(0, SideName(attacker == fight.GetPlayerModel() ? 0 : 1) + "   " + LastMove + "   " + Percent(percent) +
                (blocked ? "  blocked" : critical ? "  critical" : ""));
            if (Log.Count > 10) Log.RemoveAt(Log.Count - 1);
            // Frame advantage: who can act first once the exchange settles.
            _measureAttacker = attacker; _measureVictim = victim;
            _measureStart = tick; _attackerFree = _victimFree = -1;
        }

        /// <summary>
        /// Whether the scripted dummy should take this hit unguarded. Idle stances block on
        /// their own, so "never" and "after first hit" work by dropping the guard as the hit
        /// lands (as unblockable attacks do), not by input.
        /// </summary>
        public static bool ShouldDropGuard(Model victim)
        {
            var fight = Fight.GetCurrentFight();
            if (fight == null || victim != fight.GetEnemyModel() || Control != DummyControl.Script) return false;
            switch (Block)
            {
                case DummyBlock.Never: return true;
                case DummyBlock.AfterFirstHit: return VersusTickDriver.Tick - LastDummyHitTick >= GuardAfterHitTicks;
                case DummyBlock.Random: return !RandomGuard;
                default: return false;
            }
        }

        /// <summary>Per tick: the readouts update, health refills, the clock stays full.</summary>
        internal static void Tick(Fight fight, int tick)
        {
            if (fight == null) return;
            TickReadouts(fight, tick);
            fight.TrainingRefillTime();
            var dummy = fight.GetEnemyModel();
            var player = fight.GetPlayerModel();
            switch (Health)
            {
                case TrainingHealth.Infinite:
                    Refill(fight, dummy); Refill(fight, player);
                    break;
                case TrainingHealth.RefillAfterCombo:
                    if (tick - LastHitTick == ComboGapTicks + 20) { Refill(fight, dummy); Refill(fight, player); }
                    break;
            }
        }

        /// <summary>The measurements alone, shared with replays: move lengths and frame advantage.</summary>
        internal static void TickReadouts(Fight fight, int tick)
        {
            if (fight == null) return;
            TrackMove(0, fight.GetPlayerModel());
            TrackMove(1, fight.GetEnemyModel());
            if (_measureStart >= 0)
            {
                if (_attackerFree < 0 && !Busy(_measureAttacker)) _attackerFree = tick;
                if (_victimFree < 0 && !Busy(_measureVictim)) _victimFree = tick;
                if (_attackerFree >= 0 && _victimFree >= 0)
                {
                    // Positive: the player who hit can act first.
                    int advantage = _victimFree - _attackerFree;
                    FrameAdvantage = _measureAttacker == fight.GetPlayerModel() ? advantage : -advantage;
                    _measureStart = -1;
                }
                // A knockdown can lie in physics for 180 frames before its get-up.
                else if (tick - _measureStart > MeasureTimeoutTicks) _measureStart = -1;
            }
        }

        /// <summary>
        /// Counts the ticks each move runs. A move ends when another starts or the same one
        /// restarts; it becomes the last action only if it was ever uninterruptible (an attack,
        /// a dodge, a hit reaction), so idle and walking loops never replace it.
        /// </summary>
        private static void TrackMove(int side, Model model)
        {
            var animation = model?.OCPMJKIEPIG();
            // With no move playing (between two moves, or a ragdoll) the finished move's name
            // lingers with frame 0; that must not read as the move restarting.
            bool playing = animation != null && animation.NMEEPBDJHMG();
            string name = playing ? model.GetCurrentAnimation()?.Name ?? string.Empty : PhysicsState;
            int frame = playing ? animation.GetFrameInMove() : 0;
            if (name != MoveName[side] || frame < _moveFrame[side])
            {
                if (_moveWasBusy[side] && MoveTicks[side] > 0) { LastActionName[side] = MoveName[side]; LastActionTicks[side] = MoveTicks[side]; }
                MoveName[side] = name;
                MoveTicks[side] = 0;
                _moveWasBusy[side] = false;
            }
            MoveTicks[side]++;
            _moveFrame[side] = frame;
            if (playing && Busy(model)) _moveWasBusy[side] = true;
        }

        /// <summary>Shown while no move plays: the gap between moves, or a throw or knockdown ragdoll.</summary>
        private const string PhysicsState = "physics";

        internal static string Percent(float value) => value.ToString(value < 10f ? "0.0" : "0") + "%";

        private static void Refill(Fight fight, Model model)
        {
            if (model == null || model.Parameters == null) return;
            if (model.KKMCHCNOHMB() < model.Parameters.CIDCNCDFONA) fight.SetLife(model, model.Parameters.CIDCNCDFONA);
        }

        private const int MeasureTimeoutTicks = 400;

        /// <summary>
        /// Whether the fighter cannot act yet: inside an uninterruptible stretch of a move, or
        /// with no move playing at all, which is the ragdoll after a throw or knockdown.
        /// </summary>
        private static bool Busy(Model model)
        {
            var animation = model?.OCPMJKIEPIG();
            if (animation == null) return false;
            return !animation.NMEEPBDJHMG() || animation.HDJBHPOGKNJ(IntervalAnimation.NGAJJDIEDGF.INTERVAL_UNINTERRUPT) != null;
        }

        /// <summary>"punch_hand_right_2" as "Punch hand right 2".</summary>
        internal static string Pretty(string move)
        {
            if (string.IsNullOrEmpty(move)) return "?";
            string text = move.Replace('_', ' ').Trim();
            return text.Length == 0 ? "?" : char.ToUpperInvariant(text[0]) + text.Substring(1).ToLowerInvariant();
        }

        /// <summary>Numpad notation for a screen-space input: 5 neutral, 6 right, 2 down...; then the buttons.</summary>
        internal static string Notation(byte input)
        {
            string[] numpad = { "5", "8", "9", "6", "3", "2", "1", "4", "7" };
            int direction = NetInput.Direction(input);
            var text = new System.Text.StringBuilder(direction >= 0 && direction < numpad.Length ? numpad[direction] : "5");
            if ((input & NetInput.Punch) != 0) text.Append(" P");
            if ((input & NetInput.Kick) != 0) text.Append(" K");
            if ((input & NetInput.Ranged) != 0) text.Append(" R");
            if ((input & NetInput.Magic) != 0) text.Append(" M");
            return text.ToString();
        }
    }

    /// <summary>Player one from any device; the dummy scripted, recorded, played back or left to the AI.</summary>
    public sealed class TrainingInputSource : IVersusInputSource
    {
        // Screen-space quadrants (see NetInput): 1 up, 3 right, 5 down, 7 left.
        private const int Up = 1, Right = 3, Down = 5, Left = 7;
        private readonly VersusInputSampler _player = new VersusInputSampler(GamePad.Player.One, true, true, true);
        private readonly System.Random _random = new System.Random();
        private int _playback;
        public byte LastPlayerInput { get; private set; }
        public byte LastDummyInput { get; private set; }

        public TrainingInputSource()
        {
            VersusTraining.Active = true;
            VersusTraining.ResetReadouts();
            if (VersusTraining.Control == DummyControl.Cpu) ModelAi.set_AiOn(true);
        }

        public void Pump() { }
        public int StepsWanted => 1;

        public bool TryGetTick(int tick, out byte left, out byte right)
        {
            bool enabled = Application.isFocused && !LocalVersusMenu.BlocksFightInput;
            byte input = _player.SampleWithTouch(enabled);
            var fight = Fight.GetCurrentFight();
            switch (VersusTraining.Control)
            {
                case DummyControl.Record:
                    // You play the dummy; your own fighter stands still.
                    left = NetInput.Neutral;
                    right = input;
                    if (VersusTraining.Recording.Count < VersusTraining.MaxRecordingTicks) VersusTraining.Recording.Add(input);
                    break;
                case DummyControl.Playback:
                    left = input;
                    var recording = VersusTraining.Recording;
                    right = recording.Count == 0 ? NetInput.Neutral : recording[_playback++ % recording.Count];
                    break;
                case DummyControl.Cpu:
                    left = input;
                    right = NetInput.Neutral;
                    if (tick == 120) LogCpuState(fight);
                    break;
                default:
                    left = input;
                    right = Script(fight, tick);
                    break;
            }
            LastPlayerInput = VersusTraining.Control == DummyControl.Record ? right : left;
            LastDummyInput = right;
            return true;
        }

        private byte Script(Fight fight, int tick)
        {
            var dummy = fight?.GetEnemyModel();
            var player = fight?.GetPlayerModel();
            var dummyAt = dummy?.PLBNCDCFPML();
            var playerAt = player?.PLBNCDCFPML();
            bool dummyOnRight = dummyAt == null || playerAt == null || dummyAt.GetX() >= playerAt.GetX();
            int away = dummyOnRight ? Right : Left, toward = dummyOnRight ? Left : Right;
            if (tick % 40 == 0) VersusTraining.RandomGuard = _random.Next(2) == 0;
            // Guarding is the stance itself (idle and crouch block by themselves); whether a
            // hit is blocked is settled as it lands (VersusTraining.ShouldDropGuard).
            switch (VersusTraining.Stance)
            {
                case DummyStance.Crouch: return Crouch(dummy, tick);
                case DummyStance.Jump: return tick % 50 < 6 ? NetInput.WithDirection(NetInput.Neutral, Up) : NetInput.Neutral;
                case DummyStance.WalkIn: return NetInput.WithDirection(NetInput.Neutral, toward);
                case DummyStance.WalkAway: return NetInput.WithDirection(NetInput.Neutral, away);
                default: return NetInput.Neutral;
            }
        }

        /// <summary>What the game AI needs to drive the dummy, logged once so an idle CPU dummy can be traced.</summary>
        private static void LogCpuState(Fight fight)
        {
            var dummy = fight?.GetEnemyModel();
            var tactic = dummy?.Parameters?.HBFMBOHLKPJ;
            Debug.Log("[Training] CPU dummy: aiControlled=" + (dummy?.Parameters?.AiControlled) + " tactic=" + (tactic?.get_Name() ?? "none") +
                " aiOn=" + ModelAi.get_AiOn() + " move=" + (dummy?.GetCurrentAnimation()?.Name ?? "none"));
        }

        /// <summary>
        /// A duck is one tapped move (moves.xml "Duck", tap Down), not a held stance, so
        /// holding Down ducks once. Once a duck has had time to start and the dummy is no
        /// longer in it, Down is released for a tick so the next tick taps it again.
        /// </summary>
        private byte Crouch(Model dummy, int tick)
        {
            byte down = NetInput.WithDirection(NetInput.Neutral, Down);
            if (NetInput.Direction(LastDummyInput) != Down) { _downSince = tick; return down; }
            bool ducking = dummy?.GetCurrentAnimation()?.Name == DuckMove;
            return !ducking && tick - _downSince > DuckStartTicks ? NetInput.Neutral : down;
        }

        private const string DuckMove = "Duck";
        /// <summary>How long a tap may take to become a duck before it is tapped again.</summary>
        private const int DuckStartTicks = 6;
        private int _downSince;

        public void OnTickSimulated(int tick, byte left, byte right, uint? hash)
        {
            VersusTraining.Tick(Fight.GetCurrentFight(), tick);
            TrainingHud.Record(LastPlayerInput, LastDummyInput);
        }

        public void OnMatchEnded(int finalTick, int winner, int leftRounds, int rightRounds) { }

        public void Stop()
        {
            VersusTraining.Active = false;
            Eclipse.Diagnostics.EclipseFightDebugMenu.ShowCollisionShapes = false;
            Time.timeScale = 1f;
        }
    }

    /// <summary>Training readouts (damage, combo, frame advantage, moves), the hit log and the input history.</summary>
    public sealed class TrainingHud : MonoBehaviour
    {
        private static readonly Color Paper = new Color32(223, 207, 177, 255);
        private static readonly Color Ink = new Color32(14, 10, 8, 255);
        private static TrainingHud _instance;
        private const int HistoryRows = 16;

        private struct Held { public byte Input; public int Frames; }
        private readonly List<Held> _player = new List<Held>(), _dummy = new List<Held>();
        private Font _font;
        private Text _readouts, _log, _inputs, _dummyInputs, _hints;
        private CanvasGroup _group;
        /// <summary>Watching a replay: readouts only, no training keys; the replay bar owns the bottom.</summary>
        private bool _replay;

        public static void Begin(bool replay = false)
        {
            if (_instance != null) Destroy(_instance.gameObject);
            var host = new GameObject(replay ? "Eclipse Replay Readouts" : "Eclipse Training HUD");
            DontDestroyOnLoad(host);
            _instance = host.AddComponent<TrainingHud>();
            _instance._replay = replay;
            _instance.Build();
        }

        internal static void Record(byte player, byte dummy)
        {
            if (_instance == null) return;
            Push(_instance._player, player);
            Push(_instance._dummy, dummy);
        }

        private static void Push(List<Held> history, byte input)
        {
            if (history.Count > 0 && history[0].Input == input) { var top = history[0]; top.Frames++; history[0] = top; return; }
            history.Insert(0, new Held { Input = input, Frames = 1 });
            if (history.Count > HistoryRows) history.RemoveAt(history.Count - 1);
        }

        private void Build()
        {
            _font = Resources.Load<Font>("ui/fonts/AGOpusBold") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var canvas = gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 32742;
            var scaler = gameObject.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            var root = Node(transform, "Training", new Vector2(0, 0), Vector2.zero, Vector2.zero, true);
            _group = root.gameObject.AddComponent<CanvasGroup>();
            _group.blocksRaycasts = false;
            var panel = Node(root, "Readouts", new Vector2(0, 1), new Vector2(16, -120), new Vector2(330, 300));
            panel.gameObject.AddComponent<Image>().color = new Color(Ink.r, Ink.g, Ink.b, .62f);
            _readouts = Label(panel, 16, TextAnchor.UpperLeft, 12, 12);
            var logPanel = Node(root, "Log", new Vector2(0, 1), new Vector2(16, -428), new Vector2(330, 196));
            logPanel.gameObject.AddComponent<Image>().color = new Color(Ink.r, Ink.g, Ink.b, .5f);
            _log = Label(logPanel, 13, TextAnchor.UpperLeft, 12, 12);
            var inputs = Node(root, "Inputs", new Vector2(1, 1), new Vector2(-16, -120), new Vector2(210, 360));
            inputs.gameObject.AddComponent<Image>().color = new Color(Ink.r, Ink.g, Ink.b, .55f);
            _inputs = Label(inputs, 15, TextAnchor.UpperLeft, 12, 106);
            _dummyInputs = Label(inputs, 15, TextAnchor.UpperLeft, 110, 8);
            // In a replay the transport bar sits at the bottom; the hints go above it.
            _hints = Label(Node(root, "Hints", new Vector2(.5f, 0), new Vector2(0, _replay ? 96 : 12), new Vector2(1100, 30)), 14, TextAnchor.MiddleCenter, 0, 0);
            _hints.text = _replay
                ? "<color=#D6AA4E>F8</color> readouts     <color=#D6AA4E>F11</color> hitboxes     <color=#D6AA4E>F12</color> inputs"
                : "<color=#D6AA4E>Esc</color> training menu     <color=#D6AA4E>F9</color> reset     <color=#D6AA4E>F10</color> swap sides     " +
                  "<color=#D6AA4E>F11</color> hitboxes     <color=#D6AA4E>F12</color> inputs     <color=#D6AA4E>PgDn</color> speed     <color=#D6AA4E>PgUp</color> record / play";
        }

        private void Update()
        {
            var fight = Fight.GetCurrentFight();
            bool alive = _replay ? VersusTraining.Replay && LocalVersusSession.IsReplay : VersusTraining.Active;
            if (!alive || fight == null || !fight.IsLocalVersus) { if (!LocalVersusSession.IsStarting) { Destroy(gameObject); _instance = null; } return; }
            bool menu = LocalVersusMenu.Ensure().IsShowing;
            _group.alpha = Mathf.MoveTowards(_group.alpha, menu ? 0f : 1f, Time.unscaledDeltaTime * 4f);
            if (!menu && _replay)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.F8)) VersusTraining.ShowReadouts = !VersusTraining.ShowReadouts;
                if (UnityEngine.Input.GetKeyDown(KeyCode.F11)) Eclipse.Diagnostics.EclipseFightDebugMenu.ShowCollisionShapes = !Eclipse.Diagnostics.EclipseFightDebugMenu.ShowCollisionShapes;
                if (UnityEngine.Input.GetKeyDown(KeyCode.F12)) VersusTraining.ShowInputs = !VersusTraining.ShowInputs;
            }
            else if (!menu)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.F9)) { fight.TrainingPlaceFighters(false); VersusTraining.ResetReadouts(); EclipseUiAudio.Play(UiSound.Tick); }
                if (UnityEngine.Input.GetKeyDown(KeyCode.F10)) { _swapped = !_swapped; fight.TrainingPlaceFighters(_swapped); EclipseUiAudio.Play(UiSound.Tick); }
                if (UnityEngine.Input.GetKeyDown(KeyCode.F11)) Eclipse.Diagnostics.EclipseFightDebugMenu.ShowCollisionShapes = !Eclipse.Diagnostics.EclipseFightDebugMenu.ShowCollisionShapes;
                if (UnityEngine.Input.GetKeyDown(KeyCode.F12)) VersusTraining.ShowInputs = !VersusTraining.ShowInputs;
                if (UnityEngine.Input.GetKeyDown(KeyCode.PageDown)) { VersusTraining.SpeedIndex = (VersusTraining.SpeedIndex + VersusTraining.Speeds.Length - 1) % VersusTraining.Speeds.Length; }
                if (UnityEngine.Input.GetKeyDown(KeyCode.PageUp)) CycleRecording();
                Time.timeScale = fight.IsPaused() ? Time.timeScale : VersusTraining.Speeds[Mathf.Clamp(VersusTraining.SpeedIndex, 0, VersusTraining.Speeds.Length - 1)];
            }
            string advantage = VersusTraining.FrameAdvantage == int.MinValue ? "-" :
                (VersusTraining.FrameAdvantage > 0 ? "<color=#7FC97A>+" : VersusTraining.FrameAdvantage < 0 ? "<color=#E07A5F>" : "<color=#DFCFB1>") + VersusTraining.FrameAdvantage + "</color>";
            bool readouts = !_replay || VersusTraining.ShowReadouts;
            _readouts.transform.parent.gameObject.SetActive(readouts);
            _log.transform.parent.gameObject.SetActive(readouts);
            if (readouts)
            {
                _readouts.text =
                    (_replay ? "<color=#D6AA4E>REPLAY</color>" : "<color=#D6AA4E>TRAINING</color>   " + ControlName()) + "\n" +
                    "Last hit   <b>" + VersusTraining.Percent(VersusTraining.LastDamage) + "</b>" + (VersusTraining.LastBlocked ? "  blocked" : VersusTraining.LastCritical ? "  critical" : "") + "\n" +
                    "Combo   <b>" + VersusTraining.ComboHits + " hits  " + VersusTraining.Percent(VersusTraining.ComboDamage) + "</b>\n" +
                    "Best   " + VersusTraining.BestCombo + " hits  " + VersusTraining.Percent(VersusTraining.BestComboDamage) + "\n" +
                    // Always from the left side's view: + means they recover first.
                    "Frame advantage (" + VersusTraining.SideName(0) + ")   " + advantage + "\n" +
                    Move(0) + Move(1) +
                    (_replay ? "" : "Speed   " + VersusTraining.Speeds[Mathf.Clamp(VersusTraining.SpeedIndex, 0, VersusTraining.Speeds.Length - 1)] + "x") +
                    (Eclipse.Diagnostics.EclipseFightDebugMenu.ShowCollisionShapes ? (_replay ? "" : "   ·   ") + "hitboxes" : "");
                _log.text = "<color=#D6AA4E>HITS</color>   <color=#A69C8C>damage as % of full health</color>\n" + string.Join("\n", VersusTraining.Log);
            }
            _inputs.transform.parent.gameObject.SetActive(VersusTraining.ShowInputs);
            if (VersusTraining.ShowInputs)
            {
                _inputs.text = "<color=#D6AA4E>" + VersusTraining.SideName(0).ToUpperInvariant() + "</color>\n" + History(_player);
                _dummyInputs.text = "<color=#D6AA4E>" + VersusTraining.SideName(1).ToUpperInvariant() + "</color>\n" + History(_dummy);
            }
        }

        /// <summary>
        /// One side's move and how many frames (fixed ticks) it has run, then the last
        /// finished action and its full length.
        /// </summary>
        private static string Move(int side)
        {
            string text = VersusTraining.SideName(side) + "   " + VersusTraining.Pretty(VersusTraining.MoveName[side]) +
                "   <b>" + VersusTraining.MoveTicks[side] + "</b>f\n";
            if (VersusTraining.LastActionTicks[side] > 0)
                text += "<color=#A69C8C>      last   " + VersusTraining.Pretty(VersusTraining.LastActionName[side]) + "   " + VersusTraining.LastActionTicks[side] + "f</color>\n";
            return text;
        }

        private bool _swapped;

        private static string ControlName()
        {
            switch (VersusTraining.Control)
            {
                case DummyControl.Cpu: return "CPU " + VersusTraining.CpuNames[Mathf.Clamp(VersusTraining.CpuLevel, 0, VersusTraining.CpuNames.Length - 1)];
                case DummyControl.Record: return "<color=#E07A5F>RECORDING " + (VersusTraining.Recording.Count / 60f).ToString("0.0") + "s</color>";
                case DummyControl.Playback: return "PLAYBACK " + (VersusTraining.Recording.Count / 60f).ToString("0.0") + "s";
                default: return VersusTraining.Stance + (VersusTraining.Block != DummyBlock.Never ? ", block " + VersusTraining.Block : "");
            }
        }

        /// <summary>Page Up: start recording the dummy, then play it back, then return to the script.</summary>
        private static void CycleRecording()
        {
            switch (VersusTraining.Control)
            {
                case DummyControl.Record: VersusTraining.Control = DummyControl.Playback; break;
                case DummyControl.Playback: VersusTraining.Control = DummyControl.Script; break;
                case DummyControl.Cpu: return;
                default: VersusTraining.Recording.Clear(); VersusTraining.Control = DummyControl.Record; break;
            }
            EclipseUiAudio.Play(UiSound.Toggle);
        }

        private static string History(List<Held> history)
        {
            var text = new System.Text.StringBuilder();
            foreach (var held in history)
                text.Append(VersusTraining.Notation(held.Input)).Append("  <color=#A69C8C>").Append(Math.Min(held.Frames, 999)).Append("</color>\n");
            return text.ToString();
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        private static RectTransform Node(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size, bool stretch = false)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            if (stretch) { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; return rect; }
            rect.anchorMin = rect.anchorMax = rect.pivot = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        private Text Label(RectTransform parent, int size, TextAnchor alignment, float leftPad, float rightPad)
        {
            var rect = Node(parent, "Text", Vector2.zero, Vector2.zero, Vector2.zero, true);
            rect.offsetMin = new Vector2(leftPad, 8); rect.offsetMax = new Vector2(-rightPad, -8);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = _font; text.fontSize = size; text.alignment = alignment; text.color = Paper; text.supportRichText = true; text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Wrap; text.verticalOverflow = VerticalWrapMode.Truncate;
            var shadow = rect.gameObject.AddComponent<Shadow>(); shadow.effectColor = new Color(0, 0, 0, .7f); shadow.effectDistance = new Vector2(1f, -1.5f);
            return text;
        }
    }
}

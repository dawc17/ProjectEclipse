using System;
using System.Collections;
using Eclipse.Multiplayer.Online;
using Eclipse.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Multiplayer
{
    /// <summary>
    /// Transport controls while a replay plays: pause, speed, frame step and a seekable
    /// timeline with round markers. Replays are deterministic, so seeking is exact:
    /// forward runs ticks at high speed with sound muted; backward restarts the replay
    /// and runs forward to the target.
    /// </summary>
    public sealed class VersusReplayPlayer : MonoBehaviour
    {
        private static readonly float[] Speeds = { .25f, .5f, 1f, 2f, 4f };
        private static readonly Color Ink = new Color32(14, 10, 8, 255);
        private static readonly Color Paper = new Color32(223, 207, 177, 255);
        private static readonly Color Red = new Color32(147, 39, 31, 255);
        private static readonly Color Gold = new Color32(214, 170, 78, 255);
        private const float SeekSpeed = 24f;

        private static VersusReplayPlayer _instance;
        /// <summary>Carried across a restart when seeking backward.</summary>
        private static int _pendingSeek = -1;
        private static int _speedIndex = 2;

        private VersusReplay _replay;
        private Font _font;
        private RectTransform _bar, _track, _fill, _head;
        private Text _time, _speedLabel, _playLabel;
        private CanvasGroup _group;
        private int _seekTarget = -1;
        private float _savedVolume = 1f;
        private bool _hidden, _stepping;
        private float _shownUntil;

        public static bool IsSeeking => _instance != null && _instance._seekTarget >= 0;

        /// <summary>Attaches the controls to the replay now starting.</summary>
        public static void Begin(VersusReplay replay)
        {
            if (_instance != null) Destroy(_instance.gameObject);
            var host = new GameObject("Eclipse Replay Controls");
            DontDestroyOnLoad(host);
            _instance = host.AddComponent<VersusReplayPlayer>();
            _instance._replay = replay;
            _instance.Build();
        }

        public static void End()
        {
            if (_instance != null) Destroy(_instance.gameObject);
            _instance = null;
        }

        private void Build()
        {
            _font = Resources.Load<Font>("ui/fonts/AGOpusBold") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var canvas = gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 32740;
            var scaler = gameObject.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            gameObject.AddComponent<GraphicRaycaster>();
            // Two rows: key hints along the top, then the buttons, timeline and clock on one line.
            _bar = Node(transform, "Bar", new Vector2(.5f, 0), new Vector2(0, 14), new Vector2(980, 78));
            _group = _bar.gameObject.AddComponent<CanvasGroup>();
            var back = _bar.gameObject.AddComponent<Image>(); back.color = new Color(Ink.r, Ink.g, Ink.b, .82f);
            var rule = Node(_bar, "Rule", new Vector2(.5f, 1), Vector2.zero, new Vector2(980, 2));
            rule.gameObject.AddComponent<Image>().color = Red;
            _playLabel = Button(_bar, new Vector2(16, 8), 52, "II", TogglePause);
            Button(_bar, new Vector2(76, 8), 48, "|>", Step);
            _speedLabel = Button(_bar, new Vector2(132, 8), 80, "1x", () => SetSpeed((_speedIndex + 1) % Speeds.Length));
            Button(_bar, new Vector2(220, 8), 48, "<<", () => Seek(0));
            // The timeline: click anywhere to jump there.
            _track = Node(_bar, "Track", new Vector2(0, 0), new Vector2(292, 28), new Vector2(560, 10));
            _track.pivot = new Vector2(0, .5f);
            _track.gameObject.AddComponent<Image>().color = new Color(Paper.r, Paper.g, Paper.b, .22f);
            var hit = Node(_track, "Hit", new Vector2(.5f, .5f), Vector2.zero, new Vector2(560, 40));
            var hitImage = hit.gameObject.AddComponent<Image>(); hitImage.color = new Color(0, 0, 0, 0);
            hit.gameObject.AddComponent<TimelineClick>().Clicked = fraction => Seek(Mathf.RoundToInt(fraction * _replay.TickCount));
            _fill = Node(_track, "Fill", new Vector2(0, .5f), Vector2.zero, new Vector2(0, 10));
            _fill.pivot = new Vector2(0, .5f);
            _fill.gameObject.AddComponent<Image>().color = Red;
            foreach (int end in _replay.RoundEnds)
            {
                if (_replay.TickCount <= 0) break;
                var marker = Node(_track, "Round", new Vector2(0, .5f), new Vector2(560f * end / _replay.TickCount, 0), new Vector2(3, 22));
                marker.gameObject.AddComponent<Image>().color = Gold;
            }
            _head = Node(_track, "Head", new Vector2(0, .5f), Vector2.zero, new Vector2(16, 16));
            var disc = _head.gameObject.AddComponent<UiDisc>(); disc.color = Paper; disc.raycastTarget = false;
            _time = Text(_bar, "", 16, Paper, TextAnchor.MiddleRight, new Vector2(-16, 13), new Vector2(110, 30), new Vector2(1, 0));
            Text(_bar, "<color=#D6AA4E>Space</color> pause   <color=#D6AA4E>.</color> step   <color=#D6AA4E>- +</color> speed   <color=#D6AA4E>H</color> hide", 13,
                new Color(Paper.r, Paper.g, Paper.b, .6f), TextAnchor.MiddleCenter, new Vector2(0, -6), new Vector2(700, 20), new Vector2(.5f, 1)).supportRichText = true;
            SetSpeed(_speedIndex);
            _shownUntil = Time.unscaledTime + 4f;
        }

        private void Start()
        {
            // A backward seek restarted the replay; continue to where the player asked.
            if (_pendingSeek >= 0) { int target = _pendingSeek; _pendingSeek = -1; Seek(target); }
        }

        private void Update()
        {
            var fight = Fight.GetCurrentFight();
            // While a replay (re)loads there is no fight yet; wait for it.
            if (LocalVersusSession.IsStarting) { _group.alpha = 0f; _group.blocksRaycasts = false; return; }
            if (!LocalVersusSession.IsReplay || !(VersusTickDriver.Source is ReplayInputSource) || fight == null || !fight.IsLocalVersus) { Finish(); return; }
            if (_seekTarget >= 0)
            {
                if (VersusTickDriver.Tick >= _seekTarget || VersusTickDriver.Tick >= _replay.TickCount - 1 || LocalVersusSession.HasResult) StopSeeking();
            }
            else if (!LocalVersusMenu.Ensure().IsShowing)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.Space)) TogglePause();
                if (UnityEngine.Input.GetKeyDown(KeyCode.Period)) Step();
                if (UnityEngine.Input.GetKeyDown(KeyCode.Minus) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadMinus)) SetSpeed(Math.Max(0, _speedIndex - 1));
                if (UnityEngine.Input.GetKeyDown(KeyCode.Equals) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadPlus)) SetSpeed(Math.Min(Speeds.Length - 1, _speedIndex + 1));
                if (UnityEngine.Input.GetKeyDown(KeyCode.H)) _hidden = !_hidden;
            }
            if (UnityEngine.Input.GetAxisRaw("Mouse X") != 0 || UnityEngine.Input.GetAxisRaw("Mouse Y") != 0) _shownUntil = Time.unscaledTime + 3f;
            bool paused = fight != null && fight.IsPaused();
            float visible = _hidden || LocalVersusMenu.Ensure().IsShowing ? 0f : paused || _seekTarget >= 0 || Time.unscaledTime < _shownUntil ? 1f : .35f;
            _group.alpha = Mathf.MoveTowards(_group.alpha, visible, Time.unscaledDeltaTime * 3f);
            _group.blocksRaycasts = _group.alpha > .5f;
            int tick = Mathf.Clamp(VersusTickDriver.Tick, 0, Math.Max(1, _replay.TickCount));
            float fraction = _replay.TickCount > 0 ? (float)tick / _replay.TickCount : 0f;
            _fill.sizeDelta = new Vector2(560f * fraction, 10);
            _head.anchoredPosition = new Vector2(560f * fraction, 0);
            _time.text = _seekTarget >= 0 ? "SEEKING..." : Clock(tick) + " / " + Clock(_replay.TickCount);
            _playLabel.text = paused ? ">" : "II";
        }

        private static string Clock(int ticks)
        {
            int seconds = ticks / 60;
            return (seconds / 60) + ":" + (seconds % 60).ToString("00");
        }

        private void TogglePause()
        {
            var fight = Fight.GetCurrentFight();
            if (fight == null || _seekTarget >= 0) return;
            fight.SetPaused(!fight.IsPaused());
            EclipseUiAudio.Play(UiSound.Toggle);
        }

        /// <summary>Runs exactly one tick while paused.</summary>
        private void Step()
        {
            var fight = Fight.GetCurrentFight();
            if (fight == null || _stepping || _seekTarget >= 0) return;
            if (!fight.IsPaused()) { fight.SetPaused(true); return; }
            StartCoroutine(StepOnce(fight));
        }

        private IEnumerator StepOnce(Fight fight)
        {
            _stepping = true;
            int from = VersusTickDriver.Tick;
            float scale = Time.timeScale;
            Time.timeScale = 1f;
            fight.SetPaused(false);
            while (fight != null && VersusTickDriver.Tick == from) yield return new WaitForFixedUpdate();
            if (fight != null) fight.SetPaused(true);
            Time.timeScale = scale;
            _stepping = false;
        }

        private void SetSpeed(int index)
        {
            _speedIndex = Mathf.Clamp(index, 0, Speeds.Length - 1);
            if (_seekTarget < 0) Time.timeScale = Speeds[_speedIndex];
            float speed = Speeds[_speedIndex];
            _speedLabel.text = (speed < 1f ? speed.ToString("0.##") : speed.ToString("0")) + "x";
        }

        private void Seek(int tick)
        {
            tick = Mathf.Clamp(tick, 0, Math.Max(0, _replay.TickCount - 1));
            if (tick < VersusTickDriver.Tick)
            {
                // Back in time: replay from the start, then run forward.
                _pendingSeek = tick;
                try { LocalVersusSession.StartReplay(_replay); }
                catch (Exception exception) { _pendingSeek = -1; Debug.LogWarning("[Versus] Could not restart the replay: " + exception.Message); }
                return;
            }
            var fight = Fight.GetCurrentFight();
            if (fight != null && fight.IsPaused()) fight.SetPaused(false);
            _seekTarget = tick;
            _savedVolume = AudioListener.volume;
            AudioListener.volume = 0f;
            Time.timeScale = SeekSpeed;
        }

        private void StopSeeking()
        {
            _seekTarget = -1;
            AudioListener.volume = _savedVolume;
            Time.timeScale = Speeds[_speedIndex];
            var fight = Fight.GetCurrentFight();
            fight?.SetPaused(true);
            _shownUntil = Time.unscaledTime + 3f;
        }

        private void Finish()
        {
            if (_seekTarget >= 0) AudioListener.volume = _savedVolume;
            Time.timeScale = 1f;
            if (_instance == this) _instance = null;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_seekTarget >= 0) AudioListener.volume = _savedVolume;
            if (_pendingSeek < 0) Time.timeScale = 1f;
        }

        private Text Button(RectTransform parent, Vector2 position, float width, string text, Action click)
        {
            var rect = Node(parent, text, new Vector2(0, 0), position, new Vector2(width, 40));
            var stroke = rect.gameObject.AddComponent<InkStroke>(); stroke.color = new Color(.2f, .16f, .13f, 1f); stroke.Seed = text.GetHashCode() & 0xffff;
            var button = rect.gameObject.AddComponent<UnityEngine.UI.Button>(); button.targetGraphic = stroke;
            var label = Text(rect, text, 18, Paper, TextAnchor.MiddleCenter, Vector2.zero, new Vector2(width, 40), new Vector2(.5f, .5f));
            EclipseUiButton.Attach(button, stroke, label, new Color(.2f, .16f, .13f, 1f), Red, Paper, Paper, 0f, .05f, .05f);
            button.onClick.AddListener(() => click());
            return label;
        }

        private static RectTransform Node(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        private Text Text(RectTransform parent, string text, int size, Color color, TextAnchor alignment, Vector2 position, Vector2 box, Vector2 anchor)
        {
            var rect = Node(parent, "Label", anchor, position, box);
            var label = rect.gameObject.AddComponent<UnityEngine.UI.Text>();
            label.font = _font; label.fontSize = size; label.text = text; label.color = color; label.alignment = alignment; label.raycastTarget = false;
            return label;
        }

        /// <summary>Reports where along its width the timeline was clicked (0..1).</summary>
        private sealed class TimelineClick : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler
        {
            public Action<float> Clicked;

            public void OnPointerDown(UnityEngine.EventSystems.PointerEventData data)
            {
                var rect = (RectTransform)transform;
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, data.position, data.pressEventCamera, out var local)) return;
                Clicked?.Invoke(Mathf.Clamp01((local.x - rect.rect.xMin) / rect.rect.width));
            }
        }
    }
}

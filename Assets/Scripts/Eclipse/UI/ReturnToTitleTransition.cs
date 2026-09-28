using System;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Return to title as one continuous shot: the game sinks into ink while its sound fades,
    // the session restarts behind the black, and the title rises out of it once it is open.
    public sealed class ReturnToTitleTransition : MonoBehaviour
    {
        // The title replays its intro entrance (an ink veil lifting off the scene) on return, so
        // this overlay only hands over to that veil once the title is open.
        private const float FadeOut = .9f, Hold = .1f, FadeIn = .35f, Timeout = 60f;
        private static ReturnToTitleTransition current;
        private CanvasGroup group;
        private Action<string> failed;
        private float startedAt, restartedAt = -1f, arrivedAt = -1f, volume;

        public static bool Running => current != null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession() { current = null; }

        public static void Begin(Action<string> failed)
        {
            if (current != null || GameSessionRestart.IsRestarting) return;
            var host = new GameObject("Eclipse Return to Title", typeof(RectTransform));
            DontDestroyOnLoad(host);
            current = host.AddComponent<ReturnToTitleTransition>();
            current.failed = failed;
        }

        private void Awake()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32766;
            gameObject.AddComponent<GraphicRaycaster>();
            group = gameObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = true;
            var ink = new GameObject("Ink", typeof(RectTransform)).GetComponent<RectTransform>();
            ink.SetParent(transform, false);
            ink.anchorMin = Vector2.zero; ink.anchorMax = Vector2.one;
            ink.offsetMin = ink.offsetMax = Vector2.zero;
            ink.gameObject.AddComponent<Image>().color = new Color32(30, 25, 22, 255); // TitleScreen.Ink
            startedAt = Time.unscaledTime;
            volume = AudioListener.volume;
        }

        private void Update()
        {
            float now = Time.unscaledTime;
            if (restartedAt < 0f)
            {
                // Ease into black, taking the game's sound down with it.
                float t = Mathf.Clamp01((now - startedAt) / FadeOut);
                float eased = t * t * (3f - 2f * t);
                group.alpha = eased;
                AudioListener.volume = volume * (1f - eased);
                if (t < 1f) return;
                restartedAt = now;
                string error;
                bool restarted = GameSessionRestart.TryRestart(null, out error);
                // The restart has stopped every channel; the title brings its own music.
                AudioListener.volume = volume;
                if (!restarted)
                {
                    if (error != null) failed?.Invoke(error);
                    Leave(now);
                }
                return;
            }
            if (arrivedAt < 0f)
            {
                bool arrived = TitleScreen.IsOpen && !GameSessionRestart.IsRestarting;
                if (arrived) arrivedAt = now;
                else if (now - restartedAt > Timeout) Leave(now);
                return;
            }
            // Let the title's first frames settle, then hand over to its own intro veil.
            float lift = Mathf.Clamp01((now - arrivedAt - Hold) / FadeIn);
            group.alpha = 1f - lift * lift * (3f - 2f * lift);
            group.blocksRaycasts = lift < .5f;
            if (lift >= 1f) Destroy(gameObject);
        }

        private void Leave(float now)
        {
            arrivedAt = now - Hold;
        }

        private void OnDestroy()
        {
            if (current == this) current = null;
            if (restartedAt < 0f) AudioListener.volume = volume;
        }
    }
}

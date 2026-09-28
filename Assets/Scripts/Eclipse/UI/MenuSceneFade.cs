using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Short fade through black between the menu modules (dojo, shop, profile, map), including
    // a dojo reload for a new location or the bag/disciple toggle, and into and out of fights. The native flow swaps the
    // scene in two hard cuts (current -> Loader -> destination); this overlay freezes the last
    // rendered frame, dims it before loading, reveals the loader and fades out once the
    // destination has actually arrived (in the dojo: once the fighters are posed).
    public sealed class MenuSceneFade : MonoBehaviour
    {
        private const float FadeIn = .22f, FadeOut = .35f, Settle = .12f, Timeout = 20f;
        private static MenuSceneFade current;
        private CanvasGroup group;
        private RawImage frozen;
        private Image black;
        private Texture2D snapshot;
        private ScreenType target;
        private System.Action load;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession() { current = null; }

        private static bool IsMenu(ScreenType screen)
        {
            return screen == ScreenType.ModuleDojo || screen == ScreenType.ModuleShop ||
                   screen == ScreenType.ModuleProfile || screen == ScreenType.ModuleMap ||
                   screen == ScreenType.ModuleFight;
        }

        // Called by Module right before it starts a module scene load.
        public static bool Begin(ScreenType from, ScreenType to, System.Action load)
        {
            if (Application.isBatchMode || !IsMenu(from) || !IsMenu(to)) return false;
            if (TitleScreen.IsOpen || GameSessionRestart.IsRestarting) return false;
            if (current != null) { current.target = to; return false; }
            var host = new GameObject("Eclipse Scene Fade", typeof(RectTransform));
            DontDestroyOnLoad(host);
            current = host.AddComponent<MenuSceneFade>();
            current.target = to;
            current.load = load;
            return true;
        }

        private void Awake()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32760;
            gameObject.AddComponent<GraphicRaycaster>();
            group = gameObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = true;
            frozen = Stretch("Frozen Frame").gameObject.AddComponent<RawImage>();
            frozen.raycastTarget = true;
            frozen.enabled = false;
            black = Stretch("Black").gameObject.AddComponent<Image>();
            black.color = new Color(0f, 0f, 0f, 0f);
            black.raycastTarget = true;
            StartCoroutine(Run());
        }

        private RectTransform Stretch(string name)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(transform, false);
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero; rect.offsetMax = Vector2.zero;
            return rect;
        }

        private IEnumerator Run()
        {
            // Module waits for this capture/fade before unloading the outgoing scene.
            yield return new WaitForEndOfFrame();
            // Entering a fight already passes through the map's full-screen EnterScreen and the
            // loader; dimming that to black first would flash. Only the arrival fades.
            bool dimOut = target != ScreenType.ModuleFight;
            if (dimOut)
            {
                try { snapshot = ScreenCapture.CaptureScreenshotAsTexture(); }
                catch (System.Exception) { snapshot = null; }
                if (snapshot != null) { frozen.texture = snapshot; frozen.enabled = true; }
                group.alpha = 1f;
            }

            float start = Time.unscaledTime;
            for (float t = dimOut ? 0f : 1f; t < 1f; )
            {
                t = Mathf.Clamp01((Time.unscaledTime - start) / FadeIn);
                black.color = new Color(0f, 0f, 0f, snapshot != null ? Smooth(t) : 1f);
                if (snapshot == null) group.alpha = Smooth(t);
                yield return null;
            }
            black.color = Color.black;
            frozen.enabled = false;
            if (snapshot != null) { Destroy(snapshot); snapshot = null; }
            var beginLoad = load;
            load = null;
            beginLoad?.Invoke();

            float readySince = -1f;
            // Without the dim-out, stay clear until the loader has appeared: the outgoing scene
            // (the EnterScreen) is still showing for the frames before it.
            bool loaderSeen = dimOut;
            while (Time.unscaledTime - start < Timeout)
            {
                // Let the native loader (including mod splash replacements) remain visible.
                bool loading = Nekki.SF2.GUI.Scenes.LoaderScene.get_Current() != null;
                loaderSeen |= loading;
                group.alpha = loading || !loaderSeen ? 0f : 1f;
                if (Arrived()) { if (readySince < 0f) readySince = Time.unscaledTime; }
                else readySince = -1f;
                if (readySince >= 0f && Time.unscaledTime - readySince >= Settle) break;
                yield return null;
            }

            group.blocksRaycasts = false;
            group.alpha = 1f;
            float leave = Time.unscaledTime;
            for (float t = 0f; t < 1f; )
            {
                t = Mathf.Clamp01((Time.unscaledTime - leave) / FadeOut);
                group.alpha = 1f - Smooth(t);
                yield return null;
            }
            Destroy(gameObject);
        }

        private static float Smooth(float t) { return t * t * (3f - 2f * t); }

        private bool Arrived()
        {
            try
            {
                if (Nekki.SF2.GUI.Scenes.LoaderScene.get_Current() != null) return false;
                var active = SceneManager.GetActiveScene();
                if (!active.isLoaded || active.buildIndex != (int)target) return false;
                var module = Module.GetInstance();
                if (module == null || module.GetCurrentScreenType() != target) return false;
                // The dojo reveals once both fighters are posed. A fight reveals as soon as its
                // scene is active: its VS screen covers the fighters while they load.
                if (target != ScreenType.ModuleDojo) return true;
                var fight = Fight.GetCurrentFight();
                if (fight == null) return false;
                var player = fight.GetPlayerModel();
                if (player == null || player.GetCurrentAnimation() == null) return false;
                var enemy = fight.GetEnemyModel();
                return enemy == null || enemy.GetCurrentAnimation() != null;
            }
            catch (System.Exception) { return false; }
        }

        private void OnDestroy()
        {
            if (snapshot != null) Destroy(snapshot);
            if (current == this) current = null;
        }
    }
}

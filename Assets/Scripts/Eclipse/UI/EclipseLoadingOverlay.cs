using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // "Entering the shadows..." kept on screen from the title's Campaign/Multiplayer press
    // until the game has actually arrived: past the native loading splash, and in the dojo or
    // a fight only once both fighters are posed (or the local versus lobby opened).
    public sealed class EclipseLoadingOverlay : MonoBehaviour
    {
        private const float FadeIn = .45f, FadeOut = .7f, MinimumShown = 1.2f, Timeout = 90f;
        private static EclipseLoadingOverlay current;
        private CanvasGroup group;
        private Text caption;
        private float shownAt, arrivedAt = -1f, leaveAt = -1f;
        // How long the destination must stay ready before revealing it: long enough for the
        // fighters to settle into their intro poses.
        private const float Settle = 1f;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession() { current = null; }

        public static void Show()
        {
            if (current != null) return;
            var host = new GameObject("Eclipse Loading", typeof(RectTransform));
            DontDestroyOnLoad(host);
            current = host.AddComponent<EclipseLoadingOverlay>();
        }

        private void Awake()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 32767;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            gameObject.AddComponent<GraphicRaycaster>();
            group = gameObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = true;

            var backdrop = Child("Backdrop", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero).gameObject.AddComponent<Image>();
            backdrop.color = new Color32(14, 10, 8, 255);

            var font = Resources.Load<Font>("ui/fonts/AGOpusBold") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            caption = Child("Caption", new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(0, -28), new Vector2(1000, 70))
                .gameObject.AddComponent<Text>();
            caption.font = font;
            caption.fontSize = 40;
            caption.alignment = TextAnchor.MiddleCenter;
            caption.color = new Color32(223, 207, 177, 255);
            caption.text = "Entering the shadows...";
            caption.raycastTarget = false;

            // A total eclipse above the words: a paper corona ring around the dark moon. It is
            // deliberately still. The campaign loads in long blocking steps on the main thread,
            // so anything animated here would visibly freeze mid-motion.
            var corona = Child("Corona", new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(0, 48), new Vector2(58, 58))
                .gameObject.AddComponent<RawImage>();
            corona.texture = Halo();
            corona.color = new Color32(223, 207, 177, 150);
            corona.raycastTarget = false;
            var sun = Child("Sun", new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(0, 48), new Vector2(36, 36))
                .gameObject.AddComponent<UiDisc>();
            sun.color = new Color32(223, 207, 177, 255);
            sun.raycastTarget = false;
            var moonDisc = Child("Moon", new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(0, 48), new Vector2(32, 32))
                .gameObject.AddComponent<UiDisc>();
            moonDisc.color = backdrop.color;
            moonDisc.raycastTarget = false;
            shownAt = Time.unscaledTime;
        }

        private RectTransform Child(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(transform, false);
            rect.anchorMin = anchorMin; rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = position; rect.sizeDelta = size;
            return rect;
        }

        private static bool Arrived()
        {
            if (TitleScreen.IsOpen || GameSessionRestart.IsRestarting) return false;
            if (Eclipse.Multiplayer.LocalVersusMenu.LobbyVisible) return true;
            try
            {
                // The native "Special Edition" loading splash is its own scene; wait it out.
                if (Nekki.SF2.GUI.Scenes.LoaderScene.get_Current() != null) return false;
                var module = Module.GetInstance();
                if (module == null || !SceneManager.GetActiveScene().isLoaded) return false;
                var screen = module.NMCNDOPKFJD();
                if (screen == ScreenType.ModuleMap || screen == ScreenType.ModuleShop || screen == ScreenType.ModuleProfile) return true;
                if (screen != ScreenType.ModuleDojo && screen != ScreenType.ModuleFight) return false;
                return FightersPosed(Fight.GetCurrentFight());
            }
            catch (System.Exception) { return false; }
        }

        // Both fighters exist and are playing an animation (the dojo has no opponent until one loads).
        private static bool FightersPosed(Fight fight)
        {
            if (fight == null) return false;
            var player = fight.GetPlayerModel();
            if (player == null || player.FHBLLPCEAHG() == null) return false;
            var enemy = fight.GetEnemyModel();
            return enemy == null || enemy.FHBLLPCEAHG() != null;
        }

        private void Update()
        {
            float now = Time.unscaledTime;
            float shown = now - shownAt;
            if (leaveAt < 0f)
            {
                group.alpha = Mathf.Clamp01(shown / FadeIn);
                // Ready must hold continuously; a loader or model swap in between restarts the wait.
                if (shown >= MinimumShown && Arrived()) { if (arrivedAt < 0f) arrivedAt = now; }
                else arrivedAt = -1f;
                if ((arrivedAt >= 0f && now - arrivedAt >= Settle) || shown >= Timeout) { leaveAt = now; group.blocksRaycasts = false; }
                return;
            }
            float t = Mathf.Clamp01((now - leaveAt) / FadeOut);
            group.alpha = 1f - t * t * (3f - 2f * t);
            if (t >= 1f) Destroy(gameObject);
        }

        private void OnDestroy() { if (current == this) current = null; }

        private static Texture2D halo;

        // Soft ring for the corona, generated once.
        private static Texture2D Halo()
        {
            if (halo != null) return halo;
            const int Size = 64;
            halo = new Texture2D(Size, Size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[Size * Size];
            for (int y = 0; y < Size; y++)
                for (int x = 0; x < Size; x++)
                {
                    float dx = (x + .5f) / Size * 2f - 1f, dy = (y + .5f) / Size * 2f - 1f;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Exp(-Mathf.Pow((d - .62f) / .16f, 2f));
                    pixels[y * Size + x] = new Color(1f, 1f, 1f, a);
                }
            halo.SetPixels32(pixels);
            halo.Apply();
            return halo;
        }
    }
}

using Nekki.SF2.GUI.Menu;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // A menu-only overlay avoids modifying the recovered scroll/prefab identities. A door icon
    // fades in with the open menu scroll; hovering lights it and names it.
    public sealed class ReturnToTitleButton : MonoBehaviour
    {
        private const float FadeSeconds = .25f;
        private MainMenu menu;
        private GameObject panel;
        private GameObject canvasObject;
        private CanvasGroup group;
        private RawImage icon;
        private Texture2D normal, active;
        private Text label;
        private float shown, hover;
        private bool hovered, failed;

        public static void Attach(MainMenu owner)
        {
            if (owner.GetComponent<ReturnToTitleButton>() == null)
                owner.gameObject.AddComponent<ReturnToTitleButton>().Build(owner);
        }

        private void Build(MainMenu owner)
        {
            menu = owner;
            // Must be a root canvas: nesting under the recovered menu inherits its
            // canvas scale and coordinate space, even with ScreenSpaceOverlay set.
            canvasObject = new GameObject("Return to Title", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 31000;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);
            normal = Resources.Load<Texture2D>("EclipseTitle/door_normal");
            active = Resources.Load<Texture2D>("EclipseTitle/door_active");

            panel = new GameObject("Return to Title button", typeof(RectTransform), typeof(RawImage), typeof(Button));
            panel.transform.SetParent(canvasObject.transform, false);
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(.5f, .5f);
            rect.anchoredPosition = new Vector2(330, -122);
            rect.sizeDelta = new Vector2(112, 112);
            icon = panel.GetComponent<RawImage>();
            icon.texture = normal;
            group = panel.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            var button = panel.GetComponent<Button>();
            button.transition = Selectable.Transition.None;
            var trigger = panel.AddComponent<EventTrigger>();
            AddTrigger(trigger, EventTriggerType.PointerEnter, () => hovered = true);
            AddTrigger(trigger, EventTriggerType.PointerExit, () => hovered = false);

            var textObject = new GameObject("Label", typeof(RectTransform), typeof(Text), typeof(Shadow));
            textObject.transform.SetParent(panel.transform, false);
            var textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = textRect.anchorMax = new Vector2(1, .5f);
            textRect.pivot = new Vector2(0, .5f);
            textRect.anchoredPosition = new Vector2(-10, 0);
            textRect.sizeDelta = new Vector2(320, 40);
            label = textObject.GetComponent<Text>();
            label.font = Resources.Load<Font>("ui/fonts/AGOpusBold") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 22; label.alignment = TextAnchor.MiddleLeft;
            label.color = new Color32(223, 207, 177, 0); label.raycastTarget = false;
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.text = "RETURN TO TITLE";
            textObject.GetComponent<Shadow>().effectColor = new Color(0f, 0f, 0f, .75f);
            button.onClick.AddListener(() =>
            {
                if (shown < .9f || GameSessionRestart.IsRestarting || ReturnToTitleTransition.Running) return;
                EclipseUiAudio.Play(UiSound.Back);
                ReturnToTitleTransition.Begin(error =>
                {
                    failed = true;
                    label.text = "SAVE FAILED — TRY AGAIN";
                    Debug.LogError(error);
                });
            });
            panel.SetActive(false);
        }

        private static void AddTrigger(EventTrigger trigger, EventTriggerType type, System.Action action)
        {
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(_ => action());
            trigger.triggers.Add(entry);
        }

        private void Update()
        {
            if (panel == null) return;
            bool open = menu != null && menu.Scroll != null && menu.Scroll.CurScrollState == MenuScroll.ANJKEGGALAG.ScrollOpen &&
                !TitleScreen.IsOpen && !GameSessionRestart.IsRestarting && !ReturnToTitleTransition.Running;
            float dt = Time.unscaledDeltaTime;
            shown = Mathf.MoveTowards(shown, open ? 1f : 0f, dt / FadeSeconds);
            if (open && !panel.activeSelf) panel.SetActive(true);
            if (!open && shown <= 0f && panel.activeSelf) { panel.SetActive(false); hovered = false; }
            if (!panel.activeSelf) return;
            hover = Mathf.MoveTowards(hover, hovered && open ? 1f : 0f, dt / .12f);
            float s = shown * shown * (3f - 2f * shown);
            float h = hover * hover * (3f - 2f * hover);
            group.alpha = s;
            group.interactable = group.blocksRaycasts = open;
            icon.texture = h > .5f && active != null ? active : normal;
            float scale = Mathf.Lerp(.85f, 1f, s) * (1f + .06f * h);
            panel.transform.localScale = new Vector3(scale, scale, 1f);
            var color = label.color;
            color.a = failed ? s : h * s;
            label.color = color;
        }

        private void OnDestroy()
        {
            if (canvasObject != null) Destroy(canvasObject);
        }
    }
}

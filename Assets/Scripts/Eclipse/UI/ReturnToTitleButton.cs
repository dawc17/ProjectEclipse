using Nekki.SF2.GUI.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // A menu-only overlay avoids modifying the recovered scroll/prefab identities.
    public sealed class ReturnToTitleButton : MonoBehaviour
    {
        private MainMenu menu;
        private GameObject panel;
        private GameObject canvasObject;
        private Text label;
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
            panel = new GameObject("Return to Title button", typeof(RectTransform), typeof(Image), typeof(Button));
            panel.transform.SetParent(canvasObject.transform, false);
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(280, -96);
            rect.sizeDelta = new Vector2(280, 52);
            panel.GetComponent<Image>().color = new Color32(73, 43, 29, 255);
            var textObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(panel.transform, false);
            var textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8, 4); textRect.offsetMax = new Vector2(-8, -4);
            label = textObject.GetComponent<Text>();
            label.font = Resources.Load<Font>("ui/fonts/AGOpusBold") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 22; label.alignment = TextAnchor.MiddleCenter;
            label.color = new Color32(223, 207, 177, 255); label.raycastTarget = false;
            label.text = "RETURN TO TITLE";
            panel.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (!GameSessionRestart.TryRestart(null, out var error) && error != null)
                { label.text = "SAVE FAILED — TRY AGAIN"; Debug.LogError(error); }
            });
            panel.SetActive(false);
        }
        private void Update()
        {
            if (panel != null) panel.SetActive(menu != null && menu.Scroll != null && menu.Scroll.CurScrollState == MenuScroll.ANJKEGGALAG.ScrollOpen &&
                !TitleScreen.IsOpen && !GameSessionRestart.IsRestarting);
        }
        private void OnDestroy()
        {
            if (canvasObject != null) Destroy(canvasObject);
        }
    }
}

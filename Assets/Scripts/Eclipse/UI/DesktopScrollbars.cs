using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    public static class DesktopScrollbars
    {
        public static void Attach(SFScrollRect scroll, UnityEngine.Events.UnityAction cancelMotion)
        {
            if (Application.isMobilePlatform || scroll.get_horizontalScrollbar() != null || scroll.get_verticalScrollbar() != null) return;
            bool horizontal = scroll.get_horizontal();
            var track = new GameObject("Desktop item scrollbar", typeof(RectTransform), typeof(Image), typeof(Scrollbar));
            var rect = (RectTransform)track.transform;
            rect.SetParent(scroll.transform, false);
            rect.anchorMin = horizontal ? Vector2.zero : new Vector2(1, 0);
            rect.anchorMax = horizontal ? new Vector2(1, 0) : Vector2.one;
            rect.pivot = horizontal ? new Vector2(.5f, 0) : new Vector2(1, .5f);
            rect.sizeDelta = horizontal ? new Vector2(-16, 14) : new Vector2(14, -16);
            rect.anchoredPosition = horizontal ? new Vector2(0, 3) : new Vector2(-3, 0);
            track.GetComponent<Image>().color = new Color(.1f, .07f, .04f, .7f);
            var handle = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            var handleRect = (RectTransform)handle.transform;
            handleRect.SetParent(rect, false);
            handleRect.anchorMin = Vector2.zero; handleRect.anchorMax = Vector2.one;
            handleRect.offsetMin = handleRect.offsetMax = Vector2.zero;
            var image = handle.GetComponent<Image>(); image.color = new Color(.8f, .64f, .35f, 1);
            var bar = track.GetComponent<Scrollbar>();
            bar.handleRect = handleRect; bar.targetGraphic = image;
            bar.direction = horizontal ? Scrollbar.Direction.LeftToRight : Scrollbar.Direction.BottomToTop;
            if (cancelMotion != null) bar.onValueChanged.AddListener(_ => cancelMotion());
            if (horizontal) { scroll.set_horizontalScrollbar(bar); scroll.set_horizontalScrollbarVisibility(SFScrollRect.JJDKHMPDLNC.AutoHide); }
            else { scroll.set_verticalScrollbar(bar); scroll.set_verticalScrollbarVisibility(SFScrollRect.JJDKHMPDLNC.AutoHide); }
        }
    }
}

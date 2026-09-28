using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // One half of a two-panel fullscreen picture (loading screens) whose art was replaced by a
    // mod. The recovered panels have fixed authored boxes that stretch the art; this sizes the
    // pair to cover the whole screen at the art's own aspect ratio, centred, cropping the
    // overflow instead of squashing it on narrower (or wider) displays.
    [DisallowMultipleComponent]
    public sealed class CoverSplitHalf : MonoBehaviour
    {
        private Image image;
        private bool left;

        public static void Attach(Image image, bool left)
        {
            var cover = image.GetComponent<CoverSplitHalf>() ?? image.gameObject.AddComponent<CoverSplitHalf>();
            cover.image = image;
            cover.left = left;
            cover.enabled = true;
            image.preserveAspect = false;
            cover.Layout();
        }

        public static void Detach(Image image)
        {
            var cover = image.GetComponent<CoverSplitHalf>();
            if (cover != null) cover.enabled = false;
        }

        private void LateUpdate() { Layout(); }

        private void Layout()
        {
            if (image == null || image.sprite == null || image.canvas == null) return;
            var rect = image.rectTransform;
            var parent = rect.parent as RectTransform;
            var screen = image.canvas.rootCanvas.transform as RectTransform;
            if (parent == null || screen == null || parent.lossyScale.x == 0f || parent.lossyScale.y == 0f) return;
            Vector2 view = screen.rect.size;
            view = new Vector2(view.x * screen.lossyScale.x / parent.lossyScale.x, view.y * screen.lossyScale.y / parent.lossyScale.y);
            if (view.x <= 0f || view.y <= 0f) return;
            // Both halves together form one picture twice as wide as a half.
            float aspect = 2f * image.sprite.rect.width / image.sprite.rect.height;
            Vector2 size = view.x / view.y > aspect ? new Vector2(view.x, view.x / aspect) : new Vector2(view.y * aspect, view.y);
            Vector2 centre = (Vector2)parent.InverseTransformPoint(screen.TransformPoint(screen.rect.center)) - parent.rect.center;
            rect.anchorMin = rect.anchorMax = new Vector2(.5f, .5f);
            rect.pivot = new Vector2(left ? 1f : 0f, .5f);
            // One unit of overlap hides the seam, as the recovered layout did.
            rect.sizeDelta = new Vector2(size.x * .5f + 1f, size.y);
            rect.anchoredPosition = centre + new Vector2(left ? 1f : -1f, 0f);
        }
    }
}

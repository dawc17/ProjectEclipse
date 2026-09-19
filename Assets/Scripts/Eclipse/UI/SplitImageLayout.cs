using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Restore the authored canvas around trimmed logo halves through UI layout.
    public sealed class SplitImageLayout : MonoBehaviour
    {
        private Vector2 originalSize, originalPosition;
        private RectTransform rect;
        public static void Apply(Image image, string name)
        {
            if (name != "Logo.left" && name != "Logo.right" &&
                name != "FightPause.PauseLeft" && name != "FightPause.PauseRight") return;
            if (image.sprite == null) return;
            var layout = image.GetComponent<SplitImageLayout>();
            if (layout == null)
            {
                layout = image.gameObject.AddComponent<SplitImageLayout>();
                layout.rect = image.rectTransform;
                layout.originalSize = layout.rect.sizeDelta;
                layout.originalPosition = layout.rect.anchoredPosition;
            }
            var sprite = image.sprite;
            Vector2 canvas = name.StartsWith("Logo.")
                ? new Vector2(name == "Logo.left" ? 842 : 826, 494)
                : new Vector2(400, 192);
            Vector2 scale = new Vector2(layout.originalSize.x / canvas.x, layout.originalSize.y / canvas.y);
            Vector2 size = Vector2.Scale(sprite.rect.size, scale);
            Vector2 centerOffset = Vector2.Scale((Vector2)sprite.bounds.center * sprite.pixelsPerUnit, scale);
            // The recovered right tile sits four authored pixels below the left seam.
            if (name == "Logo.right") centerOffset.y += 4f * scale.y;
            // Account for a non-centred RectTransform pivot when resizing.
            layout.rect.sizeDelta = size;
            layout.rect.anchoredPosition = layout.originalPosition + centerOffset +
                Vector2.Scale(size - layout.originalSize, layout.rect.pivot - new Vector2(.5f, .5f));
        }
    }
}

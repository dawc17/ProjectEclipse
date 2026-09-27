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
            if (name == "Stripe.left" || name == "Stripe.right")
            {
                ProceduralVsStripe.Attach(image, name);
                return;
            }
            if (name == "VS_Fon_left.img" || name == "VS_Fon_right.img")
            {
                // The recovered panels were fixed at 1600 canvas units each.
                // Stretch each half to the viewport so wide displays cannot
                // expose the fight behind the VS/enemies screen at the edges.
                RectTransform panel = image.rectTransform;
                bool left = name == "VS_Fon_left.img";
                panel.anchorMin = new Vector2(left ? 0f : 0.5f, 0f);
                panel.anchorMax = new Vector2(left ? 0.5f : 1f, 1f);
                panel.offsetMin = new Vector2(-1f, 0f);
                panel.offsetMax = new Vector2(1f, 0f);
                return;
            }
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

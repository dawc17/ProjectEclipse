using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    public static class ProfileBottomBarLayout
    {
        public static void Configure(params SectionButton[] buttons)
        {
            foreach (SectionButton button in buttons)
            {
                if (button == null) continue;
                ResolutionImage image = button.targetGraphic as ResolutionImage;
                if (image == null || !(image.get_SpriteName() ?? string.Empty).StartsWith("ProfileButtons.")) continue;

                // BottomPanel/ProfileBottomBtn author all four slots at 248x236.
                // The scene has per-icon sizes baked into these hit areas. Restore
                // the common slot; ResolutionImage sizes the state artwork alone.
                RectTransform rect = button.transform as RectTransform;
                if (rect == null) continue;
                rect.sizeDelta = new Vector2(248f, 236f);
                image.type = Image.Type.Simple;
                image.preserveAspect = false;
                image.useSpriteMesh = false;
            }
        }
    }
}

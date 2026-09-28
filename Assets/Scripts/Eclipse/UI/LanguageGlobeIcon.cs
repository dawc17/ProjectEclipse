using Nekki.SF2.GUI;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // The settings language button shows a globe instead of the current language's flag.
    // The label beside it still names the language.
    public static class LanguageGlobeIcon
    {
        private static Sprite normal, selected;

        public static void Apply(ResolutionButton button)
        {
            if (button == null) return;
            if (normal == null) normal = Load("EclipseUI/language");
            if (selected == null) selected = Load("EclipseUI/language_selected");
            if (normal == null) return;
            var image = button.targetGraphic as Image;
            if (image != null) image.sprite = normal;
            if (selected != null) button.SetPressedSprite(selected);
        }

        private static Sprite Load(string path)
        {
            var texture = Resources.Load<Texture2D>(path);
            if (texture == null) { Debug.LogWarning("[Settings] Missing language icon: " + path); return null; }
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f), 100f);
        }
    }
}

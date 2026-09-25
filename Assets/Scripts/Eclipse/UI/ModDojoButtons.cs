using System;
using Eclipse.Modding;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Hosts mod-registered dojo buttons in each scene's main menu. They stack
    // downward from the disciple toggle's slot, share its top-right anchor and
    // show whenever the dojo is the current screen, whether or not the
    // disciple has been unlocked yet.
    public static class ModDojoButtons
    {
        const string HostName = "Eclipse dojo buttons";
        const float Gap = 12f;

        public static void Update(RectTransform disciple, bool inDojo)
        {
            if (disciple == null || disciple.parent == null) return;
            var parent = (RectTransform)disciple.parent;
            var host = parent.Find(HostName) as RectTransform;
            if (host == null)
            {
                if (ModRuntime.DojoButtons.Count == 0) return;
                host = Build(parent, disciple);
            }
            host.gameObject.SetActive(inDojo);
        }

        static RectTransform Build(RectTransform parent, RectTransform disciple)
        {
            var host = new GameObject(HostName, typeof(RectTransform)).GetComponent<RectTransform>();
            host.SetParent(parent, false);
            host.SetSiblingIndex(disciple.GetSiblingIndex() + 1);
            host.anchorMin = disciple.anchorMin; host.anchorMax = disciple.anchorMax;
            host.pivot = new Vector2(.5f, 1f);
            host.sizeDelta = new Vector2(disciple.rect.width, 0f);
            // Top of the host is the disciple's lower edge.
            host.anchoredPosition = disciple.anchoredPosition +
                new Vector2((.5f - disciple.pivot.x) * disciple.rect.width, -disciple.pivot.y * disciple.rect.height);
            float top = -Gap;
            foreach (ModDojoButton definition in ModRuntime.DojoButtons)
            {
                Sprite sprite;
                try { sprite = ModRuntime.Host.TypedAssets.LoadSprite(definition.Image); }
                catch (Exception error)
                {
                    Debug.LogWarning("[ModUI] Dojo button '" + definition.Name + "' image failed: " + error.Message);
                    continue;
                }
                if (sprite == null) continue;
                var item = new GameObject(definition.Name, typeof(RectTransform), typeof(Image), typeof(Button));
                var rect = item.GetComponent<RectTransform>();
                rect.SetParent(host, false);
                var image = item.GetComponent<Image>();
                image.sprite = sprite; image.preserveAspect = true;
                image.SetNativeSize();
                // Never wider or taller than the disciple toggle.
                Vector2 size = rect.sizeDelta;
                float scale = Mathf.Min(1f, disciple.rect.width / Mathf.Max(1f, size.x), disciple.rect.height / Mathf.Max(1f, size.y));
                rect.sizeDelta = size * scale;
                rect.anchorMin = rect.anchorMax = new Vector2(.5f, 1f);
                rect.pivot = new Vector2(.5f, 1f);
                rect.anchoredPosition = new Vector2(0f, top);
                top -= rect.sizeDelta.y + Gap;
                string name = definition.Name;
                item.GetComponent<Button>().onClick.AddListener(() => ModRuntime.PublishDojoButton(name));
            }
            host.sizeDelta = new Vector2(host.sizeDelta.x, -top);
            return host;
        }
    }
}

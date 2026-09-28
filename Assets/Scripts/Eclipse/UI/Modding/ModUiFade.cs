using UnityEngine;

namespace Eclipse.UI.Modding
{
    // Fades a mod UI layer in when it mounts and out when it closes (then destroys it), on
    // unscaled time. A closing layer stops taking input at once.
    [DisallowMultipleComponent]
    public sealed class ModUiFade : MonoBehaviour
    {
        private const float InSeconds = .2f, OutSeconds = .18f;
        private CanvasGroup group;
        private float startedAt, from, to;
        private bool closing;

        public static void In(GameObject target) { Begin(target, false); }
        public static void Out(GameObject target) { Begin(target, true); }

        private static void Begin(GameObject target, bool close)
        {
            if (target == null) return;
            if (!Application.isPlaying || !target.activeInHierarchy) { if (close) Destroy(target); return; }
            var fade = Eclipse.UI.ComponentUtility.Ensure<ModUiFade>(target);
            if (fade.closing) return;
            fade.group = Eclipse.UI.ComponentUtility.Ensure<CanvasGroup>(target);
            fade.closing = close;
            fade.from = close ? fade.group.alpha : 0f;
            fade.to = close ? 0f : 1f;
            fade.startedAt = Time.unscaledTime;
            fade.group.alpha = fade.from;
            if (close) { fade.group.interactable = false; fade.group.blocksRaycasts = false; }
            fade.enabled = true;
        }

        private void Update()
        {
            float t = Mathf.Clamp01((Time.unscaledTime - startedAt) / (closing ? OutSeconds : InSeconds));
            float eased = t * t * (3f - 2f * t);
            group.alpha = Mathf.Lerp(from, to, eased);
            if (t < 1f) return;
            if (closing) Destroy(gameObject);
            else enabled = false;
        }
    }
}

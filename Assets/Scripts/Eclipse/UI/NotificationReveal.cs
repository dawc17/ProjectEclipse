using UnityEngine;

namespace Eclipse.UI
{
    // The in-game notification scroll unrolls over half a second; its avatar and text were
    // shown at full strength from the first frame. They now follow the unroll: the avatar
    // pops in with a small overshoot and the text fades up once the paper is mostly open.
    public sealed class NotificationReveal : MonoBehaviour
    {
        private const float Delay = .22f, Seconds = .32f;
        private Transform avatar;
        private CanvasGroup text;
        private Vector3 avatarScale = Vector3.one;
        private float startedAt;

        public static void Play(GameObject host, GameObject avatarObject, GameObject label)
        {
            if (host == null) return;
            var reveal = Eclipse.UI.ComponentUtility.Ensure<NotificationReveal>(host);
            if (reveal.avatar != null) reveal.avatar.localScale = reveal.avatarScale;
            reveal.avatar = avatarObject != null ? avatarObject.transform : null;
            if (reveal.avatar != null) reveal.avatarScale = reveal.avatar.localScale;
            reveal.text = label == null ? null : Eclipse.UI.ComponentUtility.Ensure<CanvasGroup>(label);
            reveal.startedAt = Time.unscaledTime;
            reveal.enabled = true;
            reveal.Apply(0f);
        }

        private void Update()
        {
            float t = Mathf.Clamp01((Time.unscaledTime - startedAt - Delay) / Seconds);
            Apply(t);
            if (t >= 1f) enabled = false;
        }

        private void Apply(float t)
        {
            if (avatar != null)
            {
                const float s = 1.7f;
                float u = t - 1f;
                avatar.localScale = avatarScale * Mathf.Max(0f, u * u * ((s + 1f) * u + s) + 1f);
            }
            if (text != null) text.alpha = t * t * (3f - 2f * t);
        }

        private void OnDisable()
        {
            if (avatar != null) avatar.localScale = avatarScale;
            if (text != null) text.alpha = 1f;
        }
    }
}

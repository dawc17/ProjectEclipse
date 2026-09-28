using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Entrance for the recovered dialogs (settings, buy/upgrade, confirmations and the rest
    // of BaseDialog): the whole dialog fades in while its content panel settles from slightly
    // smaller with a small overshoot, and every button gets the press bounce. Unscaled time,
    // so it also plays while a fight is paused.
    public sealed class DialogCinematic : MonoBehaviour
    {
        private const float Seconds = .28f;
        private CanvasGroup group;
        private Transform content;
        private Vector3 contentScale;
        private float startedAt;

        public static void Play(GameObject dialog, GameObject panel)
        {
            if (dialog == null || dialog.GetComponent<DialogCinematic>() != null) return;
            var cinematic = dialog.AddComponent<DialogCinematic>();
            cinematic.content = panel != null ? panel.transform : null;
            if (cinematic.content != null) cinematic.contentScale = cinematic.content.localScale;
            foreach (var button in dialog.GetComponentsInChildren<Button>(true)) PressBounce.Attach(button.gameObject);
        }

        private void Start()
        {
            group = Eclipse.UI.ComponentUtility.Ensure<CanvasGroup>(gameObject);
            startedAt = Time.unscaledTime;
            Apply(0f);
        }

        private void Update()
        {
            float t = Mathf.Clamp01((Time.unscaledTime - startedAt) / Seconds);
            Apply(t);
            if (t >= 1f) enabled = false;
        }

        private void Apply(float t)
        {
            if (group != null) group.alpha = Mathf.Clamp01(t * 1.6f);
            if (content == null) return;
            const float s = 1.5f;
            float u = t - 1f;
            float settle = u * u * ((s + 1f) * u + s) + 1f;
            content.localScale = contentScale * Mathf.Lerp(.9f, 1f, settle);
        }

        private void OnDisable()
        {
            // A dialog hidden mid-entrance (e.g. settings opening Options) returns complete.
            if (group != null) group.alpha = 1f;
            if (content != null) content.localScale = contentScale;
        }
    }
}

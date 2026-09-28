using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Entrance and exit for the recovered pause screen (prefabs/fight/display/screens/
    // PauseScreen). The backdrop fades in, the two halves of the PAUSE title slide together
    // from the sides and settle with a small overshoot, then the button row and the rule text
    // rise in. Resuming fades it out over the fight instead of cutting. Unscaled time: the
    // fight is frozen underneath.
    public sealed class FightPauseCinematic : MonoBehaviour
    {
        private const float Duration = .5f, CloseSeconds = .16f, Slide = 520f;
        private RectTransform left, right;
        private Vector2 leftHome, rightHome;
        private CanvasGroup whole, backdrop, buttons, rules;
        private Transform buttonRow;
        private float startedAt, closingAt = -1f;

        public static void Play(GameObject screen)
        {
            if (screen == null || screen.GetComponent<FightPauseCinematic>() != null) return;
            screen.AddComponent<FightPauseCinematic>();
        }

        // Fades the screen out and destroys it; it stops taking input at once.
        public static void Close(GameObject screen)
        {
            if (screen == null) return;
            var cinematic = screen.GetComponent<FightPauseCinematic>();
            if (cinematic == null || !screen.activeInHierarchy) { screen.SetActive(false); Destroy(screen); return; }
            cinematic.BeginClose();
        }

        private void Awake()
        {
            whole = Eclipse.UI.ComponentUtility.Ensure<CanvasGroup>(gameObject);
            backdrop = Group(transform.Find("Background"));
            var content = transform.Find("Content");
            left = Find(content, "PauseImage/PauseImageLeft");
            right = Find(content, "PauseImage/PauseImageRight");
            buttonRow = content == null ? null : content.Find("Buttons/Panel");
            buttons = Group(buttonRow);
            rules = Group(content == null ? null : content.Find("RulesLabel"));
            if (buttonRow != null)
                foreach (var button in buttonRow.GetComponentsInChildren<Selectable>(true)) PressBounce.Attach(button.gameObject);
            startedAt = Time.unscaledTime;
        }

        private void Start()
        {
            // SplitImageLayout places the halves when their sprites resolve; animate around that.
            if (left != null) leftHome = left.anchoredPosition;
            if (right != null) rightHome = right.anchoredPosition;
            Apply(0f);
        }

        private static RectTransform Find(Transform parent, string path) => parent == null ? null : parent.Find(path) as RectTransform;

        private static CanvasGroup Group(Transform target)
        {
            if (target == null) return null;
            var group = Eclipse.UI.ComponentUtility.Ensure<CanvasGroup>(target.gameObject);
            return group;
        }

        private void BeginClose()
        {
            if (closingAt >= 0f) return;
            closingAt = Time.unscaledTime;
            whole.interactable = false;
            whole.blocksRaycasts = false;
        }

        private void Update()
        {
            if (closingAt >= 0f)
            {
                float t = Mathf.Clamp01((Time.unscaledTime - closingAt) / CloseSeconds);
                whole.alpha = 1f - t;
                if (t >= 1f) Destroy(gameObject);
                return;
            }
            float elapsed = Time.unscaledTime - startedAt;
            Apply(elapsed);
            if (elapsed > Duration + .3f) enabled = false;
        }

        private void Apply(float elapsed)
        {
            if (backdrop != null) backdrop.alpha = Smooth(Mathf.Clamp01(elapsed / .22f));
            float title = Mathf.Clamp01((elapsed - .04f) / .34f);
            float slide = 1f - Back(title);
            if (left != null) left.anchoredPosition = leftHome + new Vector2(-Slide * slide, 0f);
            if (right != null) right.anchoredPosition = rightHome + new Vector2(Slide * slide, 0f);
            SetAlpha(left, Smooth(title));
            SetAlpha(right, Smooth(title));
            float row = Mathf.Clamp01((elapsed - .2f) / .3f);
            if (buttons != null)
            {
                buttons.alpha = Smooth(row);
                buttonRow.localScale = Vector3.one * Mathf.Lerp(.88f, 1f, Back(row));
            }
            if (rules != null) rules.alpha = Smooth(Mathf.Clamp01((elapsed - .3f) / .3f));
        }

        private static void SetAlpha(RectTransform target, float alpha)
        {
            if (target == null) return;
            var graphic = target.GetComponent<Graphic>();
            if (graphic != null) graphic.canvasRenderer.SetAlpha(alpha);
        }

        private static float Smooth(float t) { return t * t * (3f - 2f * t); }

        private static float Back(float t)
        {
            const float s = 1.5f;
            float u = t - 1f;
            return u * u * ((s + 1f) * u + s) + 1f;
        }
    }
}

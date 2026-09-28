using UnityEngine;

namespace Eclipse.UI
{
    // Presentation-only pop-in for the fight banner sprites (round, fight, perfect, great,
    // time's up, ring out, you win / you lose). ScreenFight keeps its own countdown; this
    // is evaluated from that countdown's elapsed time, so pause and gameplay timing are
    // unchanged. The banner slams in from large with an overshoot, drifts in slowly while
    // held, then swells and fades out as the countdown ends.
    public sealed class FightPopupCinematic
    {
        private const float SlamSeconds = .2f;
        private const float SettleSeconds = .14f;
        private const float OutSeconds = .18f;
        private const float StartScale = 2.6f;
        private const float Undershoot = .93f;
        private const float HoldDrift = .04f;
        private const float OutScale = 1.16f;

        private readonly RectTransform panel;
        private readonly CanvasGroup group;
        private readonly Vector3 baseScale;
        private float duration;
        private bool playing;

        public FightPopupCinematic(RectTransform panel)
        {
            this.panel = panel;
            if (panel == null) return;
            baseScale = panel.localScale;
            group = panel.GetComponent<CanvasGroup>();
            if (group == null) group = panel.gameObject.AddComponent<CanvasGroup>();
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        public void Play(float seconds)
        {
            duration = Mathf.Max(seconds, .01f);
            playing = panel != null;
            Evaluate(0f);
        }

        public void Evaluate(float elapsed)
        {
            if (!playing) return;
            float scale;
            float alpha;
            // Very short banners only fade: there is no room for the slam.
            float outStart = Mathf.Max(duration - OutSeconds, SlamSeconds + SettleSeconds);
            if (duration < SlamSeconds + SettleSeconds + OutSeconds)
            {
                scale = 1f;
                alpha = Mathf.Clamp01(Mathf.Min(elapsed, duration - elapsed) / Mathf.Min(.08f, duration * .25f));
            }
            else if (elapsed < SlamSeconds)
            {
                float t = elapsed / SlamSeconds;
                float eased = 1f - (1f - t) * (1f - t) * (1f - t);
                scale = Mathf.LerpUnclamped(StartScale, Undershoot, eased);
                alpha = Mathf.Clamp01(t * 2.5f);
            }
            else if (elapsed < SlamSeconds + SettleSeconds)
            {
                float t = (elapsed - SlamSeconds) / SettleSeconds;
                scale = Mathf.Lerp(Undershoot, 1f, Mathf.SmoothStep(0f, 1f, t));
                alpha = 1f;
            }
            else if (elapsed < outStart)
            {
                float span = Mathf.Max(outStart - SlamSeconds - SettleSeconds, .01f);
                scale = 1f + HoldDrift * Mathf.Clamp01((elapsed - SlamSeconds - SettleSeconds) / span);
                alpha = 1f;
            }
            else
            {
                float span = Mathf.Max(duration - outStart, .01f);
                float t = Mathf.Clamp01((elapsed - outStart) / span);
                scale = Mathf.Lerp(1f + HoldDrift, OutScale, t * t);
                alpha = 1f - t * t;
            }
            Apply(scale, alpha);
        }

        public void Reset()
        {
            playing = false;
            Apply(1f, 1f);
        }

        private void Apply(float scale, float alpha)
        {
            if (panel == null) return;
            panel.localScale = Vector3.Scale(baseScale, new Vector3(scale, scale, 1f));
            if (group != null) group.alpha = alpha;
        }
    }
}

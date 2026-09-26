using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    public static class BattleTouchControls
    {
        private const string Preference = "Eclipse.BattleTouchControls";
        private struct Appearance
        {
            public float Alpha;
            public bool Interactable;
            public bool BlocksRaycasts;
        }
        private static readonly Dictionary<CanvasGroup, Appearance> Groups = new Dictionary<CanvasGroup, Appearance>();

        public static bool Visible
        {
            get { return PlayerPrefs.GetInt(Preference,
                !Application.isEditor && Application.platform == RuntimePlatform.Android ? 1 : 0) != 0; }
        }

        // Touches this far outside a control's art (fraction of its smaller side) still hit it.
        public const float TouchLeniency = 0.15f;
        private const float LayoutAspect = 16f / 9f;
        private static readonly Dictionary<RectTransform, Vector2> AuthoredPositions = new Dictionary<RectTransform, Vector2>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() { Groups.Clear(); AuthoredPositions.Clear(); }

        // Grows every raycast target under the control so near-misses still register.
        public static void ApplyTouchLeniency(GameObject target)
        {
            if (target == null) return;
            foreach (var graphic in target.GetComponentsInChildren<Graphic>(true))
            {
                if (!graphic.raycastTarget) continue;
                Rect rect = graphic.rectTransform.rect;
                float pad = Mathf.Min(Mathf.Abs(rect.width), Mathf.Abs(rect.height)) * TouchLeniency;
                graphic.raycastPadding = new Vector4(-pad, -pad, -pad, -pad);
            }
        }

        // Past 16:9 the canvas widens (up to WideScreenController's 21:9 cap). Keep the
        // left and right control groups where a 16:9 screen places them.
        public static void KeepSixteenByNinePositions(RectTransform container, RectTransform left, RectTransform right)
        {
            if (container == null) return;
            Rect rect = container.rect;
            float extra = rect.height > 0f ? Mathf.Max(0f, (rect.width - rect.height * LayoutAspect) * .5f) : 0f;
            Place(left, extra);
            Place(right, -extra);
        }

        private static void Place(RectTransform target, float shift)
        {
            if (target == null) return;
            Vector2 authored;
            if (!AuthoredPositions.TryGetValue(target, out authored))
            {
                authored = target.anchoredPosition;
                AuthoredPositions.Add(target, authored);
            }
            Vector2 wanted = authored + new Vector2(shift, 0f);
            if (target.anchoredPosition != wanted) target.anchoredPosition = wanted;
        }

        public static void Toggle()
        {
            PlayerPrefs.SetInt(Preference, Visible ? 0 : 1);
            PlayerPrefs.Save();
            foreach (var group in new List<CanvasGroup>(Groups.Keys))
            {
                if (group == null) Groups.Remove(group);
                else Apply(group, Groups[group]);
            }
        }

        public static void ApplyPlatformVisibility(GameObject target)
        {
            if (target == null) return;

            // Keep cooldown updates and combat state alive while suppressing touch presentation.
            // A parent group also survives the children's round/tutorial visibility changes.
            var group = target.GetComponent<CanvasGroup>();
            if (group == null) group = target.AddComponent<CanvasGroup>();
            if (!Groups.ContainsKey(group))
                Groups.Add(group, new Appearance { Alpha = group.alpha, Interactable = group.interactable, BlocksRaycasts = group.blocksRaycasts });
            Apply(group, Groups[group]);
        }

        private static void Apply(CanvasGroup group, Appearance original)
        {
            group.alpha = Visible ? original.Alpha : 0f;
            group.interactable = Visible && original.Interactable;
            group.blocksRaycasts = Visible && original.BlocksRaycasts;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() { Groups.Clear(); }

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

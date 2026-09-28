using UnityEngine;

namespace Eclipse.UI
{
    public static class ComponentUtility
    {
        // Returns the component, adding it when missing. Never use `GetComponent<T>() ?? Add`:
        // Unity returns a fake-null object for a missing component (in the Editor), which
        // C#'s ?? treats as present, so nothing would be added.
        public static T Ensure<T>(GameObject target) where T : Component
        {
            var component = target.GetComponent<T>();
            return component != null ? component : target.AddComponent<T>();
        }
    }
}

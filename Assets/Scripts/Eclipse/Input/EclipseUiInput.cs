using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace Eclipse.Input
{
    /// <summary>Upgrades recovered EventSystems without changing prefab/script GUIDs.</summary>
    public static class EclipseUiInput
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Boot()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneLoaded += SceneLoaded;
        }

        private static void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (var events in Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include))
                Ensure(events);
        }

        public static void Ensure(EventSystem events)
        {
            if (events == null) return;
            foreach (var legacy in events.GetComponents<StandaloneInputModule>()) legacy.enabled = false;
            if (events.GetComponent<InputSystemUIInputModule>() == null)
                events.gameObject.AddComponent<InputSystemUIInputModule>();
        }
    }
}

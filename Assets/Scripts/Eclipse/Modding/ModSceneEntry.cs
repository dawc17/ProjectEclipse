using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Modding
{
    // Attached only after the native scene's Init and module/layout setup finish.
    // A scene-owned coroutine cannot outlive an unloaded destination scene.
    public sealed class ModSceneEntry : MonoBehaviour
    {
        private ScreenType _screen;
        private int _profileGeneration;
        private bool _configured;

        public static void Schedule(Component owner, ScreenType screen, int profileGeneration)
        {
            if (owner == null || !owner.gameObject.activeInHierarchy || Name(screen) == null ||
                !ModRuntime.StoryEvents.HasSubscribers(ModStoryEventKind.SceneEnter)) return;
            var entry = owner.gameObject.AddComponent<ModSceneEntry>();
            entry._screen = screen;
            entry._profileGeneration = profileGeneration;
            entry._configured = true;
        }

        internal static string Name(ScreenType screen)
        {
            switch (screen)
            {
                case ScreenType.ModuleMap: return "map";
                case ScreenType.ModuleShop: return "shop";
                case ScreenType.ModuleProfile: return "profile";
                case ScreenType.ModuleDojo: return "dojo";
                case ScreenType.ModuleFight: return "fight";
                default: return null;
            }
        }

        private IEnumerator Start()
        {
            yield return null;
            try
            {
                if (!_configured || !gameObject.activeInHierarchy ||
                    gameObject.scene != SceneManager.GetActiveScene() ||
                    Module.ELEBLBJKDBI().NMCNDOPKFJD() != _screen) yield break;
                ModRuntime.PublishSceneEntry(Name(_screen), _profileGeneration);
            }
            finally { _configured = false; Destroy(this); }
        }

        private void OnDisable()
        {
            if (!_configured) return;
            _configured = false;
            Destroy(this);
        }
    }
}

using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Fight;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Eclipse.Input
{
    // Enter / keypad Enter opens the fight pause menu (Escape keeps its toggle through
    // BackKeyManager). It only opens: once a menu, dialog or result screen is up, Enter
    // belongs to that UI's submit. The pause opens on release of a press that started
    // while the fight was running, so the same press cannot also submit the pause menu's
    // focused button. Return/KeypadEnter are never valid fight key bindings.
    public static class FightPauseKey
    {
        private static bool armed;

        public static void Tick(int backControllerCount)
        {
            bool down = Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.Return) || Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.KeypadEnter);
            bool up = Eclipse.Input.EclipseInput.GetKeyUp(KeyCode.Return) || Eclipse.Input.EclipseInput.GetKeyUp(KeyCode.KeypadEnter);
            if (down) armed = CanOpen(backControllerCount) && !AltHeld();
            if (!up) return;
            bool open = armed && CanOpen(backControllerCount);
            armed = false;
            if (!open) return;
            var fight = Fight.GetCurrentFight();
            if (fight != null) fight.TogglePauseMenu(true);
        }

        private static bool AltHeld()
        {
            return Eclipse.Input.EclipseInput.GetKey(KeyCode.LeftAlt) || Eclipse.Input.EclipseInput.GetKey(KeyCode.RightAlt);
        }

        private static bool CanOpen(int backControllerCount)
        {
            if (backControllerCount > 0 || Module.GetInstance().GetCurrentScreenType() != ScreenType.ModuleFight) return false;
            if (Eclipse.UI.Modding.ModUiGameBridge.BlocksGameplayInput) return false;
            FightScene scene = Scene<FightScene>.get_Current();
            var fight = Fight.GetCurrentFight();
            if (scene == null || scene.Fight == null || fight == null || fight != scene.Fight) return false;
            // TogglePauseMenu itself ignores BattleType.FightNone (training) fights.
            if (fight.IsPaused()) return false;
            var selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
            return selected == null || selected.GetComponent<InputField>() == null;
        }
    }
}

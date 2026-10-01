using System;
using Eclipse.Input;
using Nekki.SF2.Core.Fights.Controller;
using UnityEngine;

namespace Eclipse.Multiplayer
{
    /// <summary>Owns the lifetime of local play, including boot, pause and results.</summary>
    public sealed class LocalVersusSession : MonoBehaviour
    {
        public static bool IsActive { get; private set; }
        public static bool IsReady { get; private set; }
        public static LocalVersusSettings Settings { get; private set; }
        public static bool HasResult { get; private set; }
        public static bool IsOnline => Settings != null && Settings.Mode == VersusMode.Online;
        public static bool IsReplay => Settings != null && Settings.Mode == VersusMode.Replay;
        public static bool IsSpectating => Settings != null && Settings.Mode == VersusMode.Spectator;
        /// <summary>A match is set up and its fight scene has not finished loading.</summary>
        public static bool IsStarting => _starting;
        /// <summary>The replay being watched, kept for "watch again".</summary>
        public static Online.VersusReplay CurrentReplay { get; private set; }
        private static Func<IVersusInputSource> _sourceFactory;
        private static LocalVersusSession _instance;
        private static bool _starting;
        private static bool _returning;
        private static float _startedAt;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession()
        {
            IsActive = IsReady = HasResult = _starting = _returning = false;
            Settings = null;
            CurrentReplay = null;
            _sourceFactory = null;
            _instance = null;
        }

        public static void RequestEntry()
        {
            IsActive = true;
            IsReady = HasResult = false;
        }

        public static void DataReady()
        {
            if (!IsActive) return;
            IsReady = true;
            if (_instance == null)
            {
                _instance = new GameObject("Eclipse Local Versus Session").AddComponent<LocalVersusSession>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            LocalVersusMenu.Ensure().ShowModeSelect();
        }

        public static bool DevicesReady(LocalVersusSettings settings)
        {
            return settings.SharedKeyboard || DevicesReady(settings.KeyboardPlayerOne);
        }

        public static bool DevicesReady(bool keyboardPlayerOne)
        {
            return FightGamepadInput.IsConnected(GamePad.Player.One) &&
                (keyboardPlayerOne || FightGamepadInput.IsConnected(GamePad.Player.Two));
        }

        /// <param name="sourceFactory">Creates the fight's input source; null plays locally.</param>
        public static void StartMatch(LocalVersusSettings settings, Func<IVersusInputSource> sourceFactory = null)
        {
            if (!IsActive || !IsReady || _starting || _returning)
                throw new InvalidOperationException("Local versus is not ready to start.");
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (settings.Mode == VersusMode.Local && !DevicesReady(settings))
                throw new InvalidOperationException(settings.KeyboardPlayerOne ? "Connect a gamepad for player two." : "Connect two gamepads.");
            if (settings.Mode != VersusMode.Local && settings.Mode != VersusMode.Training && sourceFactory == null)
                throw new ArgumentException("Online and replay matches need an input source.", nameof(sourceFactory));
            var current = Fight.GetCurrentFight();
            if (current != null && !current.IsLocalVersus)
                throw new InvalidOperationException("Leave the current fight before starting local versus.");
            // Validate and prepare both independent loadouts before leaving the lobby.
            var match = new LocalVersusMatch(settings);
            // Replay speed controls never carry into a real match.
            if (settings.Mode != VersusMode.Replay) { VersusReplayPlayer.End(); Time.timeScale = 1f; }
            current?.SetPaused(true);
            VersusTickDriver.Stop();
            VersusTraining.Replay = false;
            VersusDeterminism.Seed(settings.Seed);
            Settings = settings;
            _sourceFactory = sourceFactory;
            if (settings.Mode != VersusMode.Replay) CurrentReplay = null;
            HasResult = false;
            _starting = true;
            _startedAt = Time.realtimeSinceStartup;
            // Fighters are introduced before local and online fights; replays and training go straight in.
            if (settings.Mode == VersusMode.Local || settings.Mode == VersusMode.Online)
                LocalVersusMenu.Ensure().PlayVersusSplash(settings, () =>
                {
                    try { OpenFight(match, settings); }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                        LocalVersusMenu.Ensure().ShowPause("The match could not start: " + exception.Message);
                    }
                });
            else OpenFight(match, settings);
        }

        private static void OpenFight(LocalVersusMatch match, LocalVersusSettings settings)
        {
            if (!_starting) return;
            _startedAt = Time.realtimeSinceStartup;
            LocalVersusMenu.Ensure().Hide();
            try { Module.GetInstance().OpenLocalVersus(match); }
            catch
            {
                _starting = false;
                if (settings.Mode == VersusMode.Local) LocalVersusMenu.Ensure().ShowLobby();
                else LocalVersusMenu.Ensure().ShowModeSelect();
                throw;
            }
        }

        /// <param name="rollbackTest">Force a rollback every tick and report any state it fails to restore.</param>
        public static void StartReplay(Online.VersusReplay replay, bool rollbackTest = false)
        {
            if (replay == null) throw new ArgumentNullException(nameof(replay));
            if (OnlineVersusSession.IsActive) throw new InvalidOperationException("Leave the online session before watching a replay.");
            var settings = new LocalVersusSettings(VersusLoadout.FromIds(replay.LeftLoadout), VersusLoadout.FromIds(replay.RightLoadout), replay.Arena, true, replay.WinsRequired,
                replay.RoundTimeSeconds, VersusMode.Replay, replay.LeftName, replay.RightName, replay.Seed);
            StartMatch(settings, rollbackTest ? (Func<IVersusInputSource>)(() => new Rollback.RollbackSelfTest(replay)) : () => new ReplayInputSource(replay));
            CurrentReplay = replay;
            if (!rollbackTest)
            {
                VersusReplayPlayer.Begin(replay);
                VersusTraining.BeginReplay();
            }
        }

        internal static IVersusInputSource CreateInputSource()
        {
            return _sourceFactory != null ? _sourceFactory() : new LocalInputSource(Settings);
        }

        internal static void FightReady(Fight fight)
        {
            if (fight == null || !fight.IsLocalVersus) throw new ArgumentException("Expected a local fight.");
            _starting = false;
            HasResult = false;
            OnlineVersusSession.Current?.OnFightReady(fight);
        }

        public static void Pause(string reason)
        {
            var fight = Fight.GetCurrentFight();
            if (!IsActive || _starting || HasResult || fight == null || !fight.IsLocalVersus) return;
            // An online match cannot pause for one player; the menu only covers the screen.
            if (!IsOnline) fight.SetPaused(true);
            LocalVersusMenu.Ensure().ShowPause(reason);
        }

        public static void Resume()
        {
            var fight = Fight.GetCurrentFight();
            if (!IsActive || HasResult || _starting || fight == null || !fight.IsLocalVersus) return;
            if (Settings.Mode == VersusMode.Local && !DevicesReady(Settings))
            {
                LocalVersusMenu.Ensure().ShowPause("Reconnect the assigned controllers to continue.");
                return;
            }
            LocalVersusMenu.Ensure().Hide();
            fight.SetPaused(false);
        }

        public static void ShowLobby()
        {
            if (!LeaveFightForMenu()) return;
            if (RoomSession.IsActive && RoomSession.Current.Room != null) LocalVersusMenu.Ensure().ShowRoom();
            else if (OnlineVersusSession.IsActive) LocalVersusMenu.Ensure().ShowOnlineLobby();
            else LocalVersusMenu.Ensure().ShowLobby();
        }

        /// <summary>Back to the multiplayer page that chooses local, online or replays (from a replay, for one).</summary>
        public static void ShowMultiplayerHome()
        {
            if (!LeaveFightForMenu()) return;
            if (RoomSession.IsActive && RoomSession.Current.Room != null) LocalVersusMenu.Ensure().ShowRoom();
            else if (OnlineVersusSession.IsActive) LocalVersusMenu.Ensure().ShowOnlineLobby();
            else LocalVersusMenu.Ensure().ShowModeSelect();
        }

        /// <summary>Pauses and stops driving a versus fight that a menu is replacing; false when no menu may open now.</summary>
        private static bool LeaveFightForMenu()
        {
            if (!IsActive || !IsReady || _starting || _returning) return false;
            var fight = Fight.GetCurrentFight();
            if (fight != null && fight.IsLocalVersus)
            {
                fight.SetPaused(true);
                // The menu plays its own music; the arena's track and loops must not carry on under it.
                Sound.StopMusic();
                Sound.StopLoopedSounds();
            }
            if (fight != null && fight.IsLocalVersus && (!HasResult || VersusTraining.Active)) VersusTickDriver.Stop();
            VersusTraining.Replay = false;
            return true;
        }

        internal static void Complete(Fight fight, bool abandoned = false)
        {
            if (HasResult || fight == null || !fight.IsLocalVersus) return;
            // The result only ever lands on a confirmed tick.
            if (VersusTickDriver.Barrier()) return;
            int one = fight.GetPlayerModel().Parameters.RoundsWon;
            int two = fight.GetEnemyModel().Parameters.RoundsWon;
            int winner = abandoned ? -1 : (one > two ? 0 : 1);
            // Report before pausing so the source sees the tick the result happened on.
            VersusTickDriver.MatchEnded(fight, winner, one, two);
            HasResult = true;
            fight.SetPaused(true);
            LocalVersusMenu.Ensure().ShowResult(winner, one, two);
        }

        /// <summary>Ends an online match early (forfeit, desync or lost connection).</summary>
        internal static void CompleteOnline(Fight fight, int winner, string message)
        {
            if (HasResult || fight == null || !fight.IsLocalVersus) return;
            HasResult = true;
            fight.SetPaused(true);
            VersusTickDriver.Stop();
            int one = fight.GetPlayerModel().Parameters.RoundsWon;
            int two = fight.GetEnemyModel().Parameters.RoundsWon;
            LocalVersusMenu.Ensure().ShowResult(winner, one, two, message);
        }

        public static void ReturnToTitle()
        {
            if (_returning) return;
            _returning = true;
            Fight.GetCurrentFight()?.SetPaused(true);
            VersusTickDriver.Stop();
            OnlineVersusSession.Shutdown("Returned to the title screen.");
            RoomSession.Shutdown("Returned to the title screen.");
            GameController.get_Current()?.StopController();
            LocalVersusMenu.Ensure().Hide();
            Eclipse.UI.TitleScreen.PrepareForRestart();
            Sound.StopMusic();
            Sound.StopLoopedSounds();
            Time.timeScale = 1f;
            // Keep isolation active until the outgoing scene has completed teardown.
            SceneManagerSF.Load(ScreenType.ModulePreloader);
        }

        internal static void ArrivedAtTitle()
        {
            if (!_returning) return;
            var menu = UnityEngine.Object.FindFirstObjectByType<LocalVersusMenu>();
            if (menu != null) Destroy(menu.gameObject);
            if (_instance != null) Destroy(_instance.gameObject);
            ResetSession();
        }

        // Closing the game mid-match still keeps that match's replay.
        private void OnApplicationQuit() => VersusTickDriver.Stop();

        private void OnApplicationFocus(bool focused)
        {
            if (!focused && Settings != null && Settings.Mode == VersusMode.Local) Pause("The game lost focus. Resume when both players are ready.");
        }

        private void Update()
        {
            if (!IsActive || _returning) return;
            if (_starting)
            {
                if (Time.realtimeSinceStartup - _startedAt > 45f)
                {
                    _starting = false;
                    Debug.LogError("[Local Versus] The fight scene did not finish loading.");
                    LocalVersusMenu.Ensure().ShowPause("The match could not load. Return to the title screen and try again.");
                }
                return;
            }
            var fight = Fight.GetCurrentFight();
            // A replay that stops short of a result (a disconnect, say) ends where its inputs end.
            if (IsReplay && fight != null && fight.IsLocalVersus && !HasResult &&
                (VersusTickDriver.Source is ReplayInputSource replay && replay.ReachedEnd ||
                 VersusTickDriver.Source is Rollback.RollbackSelfTest test && test.ReachedEnd))
                Complete(fight, true);
            if (Settings != null && Settings.Mode == VersusMode.Local && fight != null && fight.IsLocalVersus && !HasResult && !fight.IsPaused() &&
                !DevicesReady(Settings)) Pause("A controller disconnected. Reconnect it, then resume.");
        }
    }
}

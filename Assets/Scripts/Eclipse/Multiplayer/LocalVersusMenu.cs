using System;
using Eclipse.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Eclipse.Multiplayer
{
    public sealed partial class LocalVersusMenu : MonoBehaviour
    {
        private static readonly Color Ink = new Color32(30, 25, 22, 255);
        private static readonly Color Paper = new Color32(223, 207, 177, 255);
        private static readonly Color Red = new Color32(147, 39, 31, 255);
        private static LocalVersusMenu instance;
        private Font font;
        private RectTransform panel;
        private UnityEngine.UI.Text status;
        private EventSystem ownedEventSystem;
        private EventSystem navigationEventSystem;
        private bool previousNavigation;
        private Vector2Int heldNavigation;
        private float repeatNavigationAt;
        private VersusLoadout p1Loadout, p2Loadout;
        private string arena;
        private bool keyboardPlayerOne = true;
        private bool sharedKeyboard;
        private int winsRequired = 2;
        private int roundTime = 99;
        private float nextDeviceCheck;
        private bool padsWereReady;
        private int inputFrame;
        private Page page;
        public bool IsShowing { get; private set; }
        // Lets the loading overlay step aside once the local versus lobby is up.
        public static bool LobbyVisible => instance != null && instance.IsShowing && instance.IsBackdropPage;
        internal static bool VersusSplashVisible => instance != null && instance.IsShowing && instance.page == Page.Splash;

        internal static void FinishVersusSplash()
        {
            // A disconnect or loading failure may have replaced the splash with a menu.
            if (VersusSplashVisible) instance.Hide();
        }
        /// <summary>While any versus menu covers the fight, local devices send neutral input.</summary>
        public static bool BlocksFightInput => instance != null && instance.IsShowing;
        private readonly System.Collections.Generic.List<(UnityEngine.UI.Text label, Func<string> value)> liveLabels =
            new System.Collections.Generic.List<(UnityEngine.UI.Text, Func<string>)>();
        private OnlinePhase builtPhase;
        private UnityEngine.UI.InputField nameField, addressField, portField;

        private enum Page { Hidden, Lobby, Pause, Result, OnlineSetup, OnlineLobby, OnlineHome, RoomBrowser, RoomCreate, Room, ModeSelect, Armory, Replays, Training, Splash }

        /// <summary>Menu pages drawn over the living arena backdrop rather than over a fight.</summary>
        private bool IsBackdropPage => page == Page.Lobby || page == Page.OnlineSetup || page == Page.OnlineLobby ||
            page == Page.OnlineHome || page == Page.RoomBrowser || page == Page.RoomCreate || page == Page.Room || page == Page.ModeSelect ||
            page == Page.Armory || page == Page.Replays || page == Page.Training || page == Page.Splash;

        public static LocalVersusMenu Ensure()
        {
            if (instance != null) return instance;
            instance = FindFirstObjectByType<LocalVersusMenu>();
            if (instance != null) return instance;
            var host = new GameObject("Eclipse Local Versus Menu", typeof(RectTransform));
            instance = host.AddComponent<LocalVersusMenu>();
            DontDestroyOnLoad(host);
            return instance;
        }

        private void Awake()
        {
            if (instance != null && instance != this) { Destroy(gameObject); return; }
            instance = this;
            DontDestroyOnLoad(gameObject);
            font = Resources.Load<Font>("ui/fonts/AGOpusBold") ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var canvas = gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 32755;
            var scaler = gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1280, 720);
            scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
            gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            // Behind every page: the living arena (only on pages that have no fight behind them).
            backdrop = VersusBackdrop.Create(transform);
            panel = Rect(transform, "Local Versus Overlay"); Stretch(panel);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Hide();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            RestoreNavigation();
            if (ownedEventSystem != null) Destroy(ownedEventSystem.gameObject);
            if (instance == this) instance = null;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (IsShowing) EnsureEventSystem();
        }

        private void Update()
        {
            if (!LocalVersusSession.IsActive) return;
            if (IsShowing)
                foreach (var (label, value) in liveLabels)
                    if (label != null) label.text = value();
            // Pause keys belong to the fight and its pause menu. A menu page that handled
            // Escape itself (backing out to the title, say) must not also reopen the pause menu.
            bool overFight = !IsShowing || page == Page.Pause;
            if (IsShowing)
            {
                UpdateScreens();
                if (IsShowing && inputFrame != Time.frameCount && !VersusStagePicker.IsOpen && page != Page.Splash) UpdateNavigation();
            }
            if (page == Page.Lobby && Time.unscaledTime >= nextDeviceCheck)
            {
                nextDeviceCheck = Time.unscaledTime + .5f;
                if (SchemeReady(keyboardPlayerOne, sharedKeyboard) != padsWereReady) RefreshDeviceStatus();
            }
            var fight = Fight.GetCurrentFight();
            if (fight == null || !fight.IsLocalVersus || !overFight) return;
            bool pausePressed = Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.Escape);
            if (!pausePressed && (GamePad.GetButtonDown(GamePad.Button.Start, GamePad.Player.One) ||
                (!CurrentKeyboardPlayerOne() && GamePad.GetButtonDown(GamePad.Button.Start, GamePad.Player.Two))))
            {
                pausePressed = true;
                // A custom combat binding owns its button. Escape and the HUD's
                // pause button remain available when Start is assigned to combat.
                for (int action = 0; action < FightControllerBindings.Names.Length; action++)
                    if (FightControllerBindings.Get(action) == (int)GamePad.Button.Start) pausePressed = false;
            }
            if (!pausePressed) return;
            // Training: Escape (or Start) opens the training menu and closes it again.
            if (LocalVersusSession.Settings != null && LocalVersusSession.Settings.Mode == VersusMode.Training)
            {
                if (IsShowing && page == Page.Pause) ResumeTraining();
                else if (!IsShowing) Pause("Paused");
                return;
            }
            if (LocalVersusSession.IsOnline)
            {
                if (IsShowing && page == Page.Pause) Resume();
                else if (!IsShowing && !LocalVersusSession.HasResult) Pause("The match keeps running while this menu is open.");
                return;
            }
            if (IsShowing)
            {
                if (page == Page.Pause && fight.IsPaused() && (LocalVersusSession.IsReplay || CurrentSchemeReady())) Resume();
            }
            else if (!fight.IsPaused()) Pause(LocalVersusSession.IsReplay ? "Replay paused." : "Match paused.");
        }

        /// <summary>A small line under a button explaining it.</summary>
        private void AddCaption(RectTransform parent, string text)
        {
            var box = Rect(parent, "Caption");
            box.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredHeight = 24;
            Label(box, text, 17, new Color(Ink.r, Ink.g, Ink.b, .7f), TextAnchor.UpperCenter);
        }

        public void ShowResult(int winner, int playerOneWins, int playerTwoWins, string message = null)
        {
            EnsureEventSystem();
            page = Page.Result;
            if (LocalVersusSession.IsSpectating) { ShowSpectatorMenu(message ?? "The fight ended."); return; }
            if (LocalVersusSession.Settings != null && LocalVersusSession.Settings.Mode == VersusMode.Training) { ShowTrainingMenu("Round over"); return; }
            if (LocalVersusSession.IsOnline) { ShowOnlineResult(winner, playerOneWins, playerTwoWins, message); return; }
            if (LocalVersusSession.IsReplay) { ShowReplayResult(winner, playerOneWins, playerTwoWins); return; }
            Rebuild(winner == 0 ? "PLAYER 1 WINS" : winner == 1 ? "PLAYER 2 WINS" : "MATCH ENDED",
                "Player 1  " + playerOneWins + "  :  " + playerTwoWins + "  Player 2", body =>
            {
                AddButton(body, "REMATCH", () => { if (LocalVersusSession.Settings == null) SetStatus("No matchup is configured."); else TryStart(LocalVersusSession.Settings.Reseeded()); });
                AddButton(body, "WATCH REPLAY", WatchLastReplay);
                AddButton(body, "CHANGE MATCHUP", LocalVersusSession.ShowLobby);
                AddButton(body, "RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
            });
        }

        public void ShowPause(string reason)
        {
            EnsureEventSystem();
            page = Page.Pause;
            if (LocalVersusSession.IsSpectating) { ShowSpectatorMenu("You are spectating this fight."); return; }
            if (LocalVersusSession.Settings != null && LocalVersusSession.Settings.Mode == VersusMode.Training) { ShowTrainingMenu(reason); return; }
            if (LocalVersusSession.IsOnline)
            {
                Rebuild("MENU", reason, body =>
                {
                    AddButton(body, "RESUME", Resume);
                    AddButton(body, "FORFEIT MATCH", () => OnlineVersusSession.Current?.Forfeit());
                    AddButton(body, "LEAVE AND RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
                });
                return;
            }
            if (LocalVersusSession.IsReplay)
            {
                Rebuild("REPLAY PAUSED", ReplayTitle(), body =>
                {
                    AddButton(body, "RESUME", Resume);
                    AddButton(body, "STOP REPLAY", LocalVersusSession.ShowMultiplayerHome);
                    AddButton(body, "RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
                });
                return;
            }
            Rebuild("PAUSED", string.IsNullOrEmpty(reason) ? "Local versus" : reason, body =>
            {
                AddButton(body, "RESUME", Resume);
                AddButton(body, "CHANGE MATCHUP", LocalVersusSession.ShowLobby);
                AddButton(body, "RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
            });
        }

        public void Hide()
        {
            RestoreNavigation();
            IsShowing = false; page = Page.Hidden;
            LeaveBackdrop();
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null && panel != null &&
                EventSystem.current.currentSelectedGameObject.transform.IsChildOf(panel)) EventSystem.current.SetSelectedGameObject(null);
            if (panel != null) panel.gameObject.SetActive(false);
        }

        // ---- Online and replays ----

        public void ShowOnlineSetup()
        {
            if (OnlineVersusSession.IsActive) { ShowOnlineLobby(); return; }
            EnsureEventSystem();
            page = Page.OnlineSetup;
            Rebuild("DIRECT CONNECT", "Host a match, or join a friend's address", body =>
            {
                nameField = AddTextField(body, "YOUR NAME", OnlineVersusSession.SavedName, 24);
                addressField = AddTextField(body, "HOST ADDRESS", OnlineVersusSession.SavedAddress, 80, "e.g. 192.168.1.20:" + NetProtocolPort());
                portField = AddTextField(body, "PORT TO HOST ON", OnlineVersusSession.SavedPort.ToString(), 5);
                portField.contentType = UnityEngine.UI.InputField.ContentType.IntegerNumber;
                AddButton(body, "HOST GAME", HostOnline);
                AddButton(body, "JOIN GAME", JoinOnline);
                AddButton(body, "BACK", ShowOnlineHome);
            });
            SetStatus("Both players need the same game version and mods. Hosting over the internet needs the UDP port forwarded, or a VPN such as Tailscale.");
        }

        public void ShowOnlineLobby()
        {
            var session = OnlineVersusSession.Current;
            if (session == null) { ShowOnlineSetup(); return; }
            EnsureEventSystem();
            page = Page.OnlineLobby;
            builtPhase = session.Phase;
            switch (session.Phase)
            {
                case OnlinePhase.Hosting:
                    int port = session.Peer.LocalPort;
                    Rebuild("HOSTING", "Waiting for an opponent on UDP port " + port, body =>
                    {
                        foreach (var address in LocalAddresses()) AddInfo(body, "SHARE THIS ADDRESS", () => address + ":" + port);
                        AddButton(body, "CANCEL", LeaveOnline);
                    });
                    SetStatus("On the same network, share the address above. Over the internet, forward UDP port " + port + " or use a VPN.");
                    break;
                case OnlinePhase.Connecting:
                    Rebuild("JOINING", "Connecting to " + session.Peer.RemoteEndPoint + "...", body => AddButton(body, "CANCEL", LeaveOnline));
                    break;
                case OnlinePhase.Closed:
                    Rebuild("DISCONNECTED", "The online session ended", body =>
                    {
                        AddButton(body, "BACK", LeaveOnline);
                        AddButton(body, "RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
                    });
                    SetStatus(session.Notice);
                    break;
                default:
                    Rebuild("ONLINE VERSUS", session.LocalName + "  vs  " + session.RemoteName, body =>
                    {
                        AddChoice(body, "YOUR LOADOUT", () => session.LocalLoadout.WeaponName, () => ShowArmory("Your loadout", session.LocalLoadout, loadout =>
                        {
                            OnlineVersusSession.Current?.SetLocalLoadout(loadout);
                            ShowOnlineLobby();
                        }));
                        AddInfo(body, "OPPONENT", () => session.RemoteLoadout?.WeaponName ?? "Choosing...");
                        if (session.IsHost && session.RoomMatch == null)
                            AddChoice(body, "BALANCE", () => session.Lobby.BalanceName, PvpBalanceProfiles.Cycle);
                        else AddInfo(body, "BALANCE", () => session.Lobby.BalanceName);
                        if (session.IsHost)
                        {
                            AddChoice(body, "ARENA", () => ArenaLabel(session.Lobby.Arena),
                                () => VersusStagePicker.Open(session.Lobby.Arena, true, picked => OnlineVersusSession.Current?.SetArena(picked)));
                            AddChoice(body, "FIRST TO", () => WinsLabel(session.Lobby.WinsRequired), session.CycleWins);
                            AddChoice(body, "NETCODE", () => NetcodeLabel(session), session.CycleNetcode);
                            AddChoice(body, "INPUT DELAY", () => DelayLabel(session), session.CycleDelay);
                            AddButton(body, "START MATCH", () =>
                            {
                                string blocker = session.StartBlocker();
                                if (blocker != null) { SetStatus(blocker); return; }
                                try { session.HostStart(); }
                                catch (Exception exception) { Debug.LogException(exception); SetStatus(exception.Message); }
                            }, -1, Eclipse.UI.UiSound.Begin);
                        }
                        else
                        {
                            AddInfo(body, "ARENA", () => ArenaLabel(session.Lobby.Arena));
                            AddInfo(body, "FIRST TO", () => WinsLabel(session.Lobby.WinsRequired));
                            AddInfo(body, "NETCODE", () => NetcodeLabel(session));
                            AddInfo(body, "INPUT DELAY", () => DelayLabel(session));
                            AddChoice(body, "READY", () => session.LocalReady ? "READY" : "NOT READY", session.ToggleReady);
                        }
                        AddButton(body, "LEAVE", LeaveOnline);
                    });
                    liveLabels.Add((status, () => LobbyStatus(session)));
                    break;
            }
        }

        /// <summary>Called whenever online state changes; rebuilds only when the page layout must change.</summary>
        public void OnOnlineChanged()
        {
            var session = OnlineVersusSession.Current;
            if (session == null) return;
            if (page == Page.OnlineLobby && session.Phase != builtPhase)
            {
                bool lobbyLike(OnlinePhase phase) => phase == OnlinePhase.Lobby || phase == OnlinePhase.Result;
                if (!(lobbyLike(session.Phase) && lobbyLike(builtPhase))) ShowOnlineLobby();
            }
            else if (page == Page.Result && session.Phase == OnlinePhase.Closed && LocalVersusSession.IsOnline)
            {
                SetStatus(session.Notice);
            }
        }

        private void ShowOnlineResult(int winner, int playerOneWins, int playerTwoWins, string message)
        {
            var session = OnlineVersusSession.Current;
            int localSide = session != null && !session.IsHost ? 1 : 0;
            string title = winner < 0 ? "MATCH ENDED" : winner == localSide ? "YOU WIN" : "YOU LOSE";
            var settings = LocalVersusSession.Settings;
            if (session != null && session.RoomMatch != null) { ShowRoomResult(title, playerOneWins, playerTwoWins, message); return; }
            Rebuild(title, settings.PlayerOneName + "  " + playerOneWins + "  :  " + playerTwoWins + "  " + settings.PlayerTwoName, body =>
            {
                if (session != null && session.Phase != OnlinePhase.Closed)
                {
                    AddButton(body, "REMATCH", () => session.RequestRematch(), -1, Eclipse.UI.UiSound.Begin);
                    AddButton(body, "CHANGE MATCHUP", () => session.ReturnToLobby());
                }
                else AddButton(body, "BACK TO ONLINE", LeaveOnline);
                AddButton(body, "LEAVE AND RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
            });
            string fixedMessage = message;
            liveLabels.Add((status, () => ResultStatus(OnlineVersusSession.Current, fixedMessage)));
        }

        private void ShowReplayResult(int winner, int playerOneWins, int playerTwoWins)
        {
            var settings = LocalVersusSession.Settings;
            string title = winner == 0 ? settings.PlayerOneName + " WINS" : winner == 1 ? settings.PlayerTwoName + " WINS" : "REPLAY ENDED";
            Rebuild(title, settings.PlayerOneName + "  " + playerOneWins + "  :  " + playerTwoWins + "  " + settings.PlayerTwoName, body =>
            {
                AddButton(body, "WATCH AGAIN", () =>
                {
                    var replay = LocalVersusSession.CurrentReplay;
                    if (replay == null) { SetStatus("The replay is no longer loaded."); return; }
                    try { LocalVersusSession.StartReplay(replay); }
                    catch (Exception exception) { Debug.LogException(exception); SetStatus(exception.Message); }
                }, -1, Eclipse.UI.UiSound.Begin);
                AddButton(body, "BACK", LocalVersusSession.ShowMultiplayerHome);
                AddButton(body, "RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
            });
            var source = VersusTickDriver.Source;
            if (source is Rollback.RollbackSelfTest selfTest) Debug.Log("[Rollback test] " + selfTest.Summary());
            SetStatus(source is ReplayInputSource replaySource ? replaySource.Summary()
                : source is Rollback.RollbackSelfTest test ? test.Summary() : "Replay finished.");
        }

        private void HostOnline()
        {
            if (!int.TryParse(portField.text, out int port) || port < 1 || port > 65535) { SetStatus("The port must be a number from 1 to 65535."); return; }
            OnlineVersusSession.SavedPort = port;
            try { OnlineVersusSession.Host(nameField.text, port); ShowOnlineLobby(); }
            catch (Exception exception) { Debug.LogWarning(exception.Message); SetStatus(exception.Message); }
        }

        private void JoinOnline()
        {
            OnlineVersusSession.SavedAddress = addressField.text.Trim();
            try { OnlineVersusSession.Join(nameField.text, addressField.text); ShowOnlineLobby(); }
            catch (Exception exception) { Debug.LogWarning(exception.Message); SetStatus(exception.Message); }
        }

        private void LeaveOnline()
        {
            OnlineVersusSession.Shutdown();
            ShowOnlineSetup();
        }

        /// <summary>Replays the last match while forcing a rollback every tick, to prove rollback restores all fight state.</summary>
        private void TestRollbackOnLastReplay()
        {
            if (OnlineVersusSession.IsActive) { SetStatus("The rollback test runs after leaving the online session."); return; }
            if (!VersusReplays.TryLoadLast(out var replay, out var error)) { SetStatus(error); return; }
            try { LocalVersusSession.StartReplay(replay, rollbackTest: true); }
            catch (Exception exception) { Debug.LogException(exception); SetStatus(exception.Message); }
        }

        private void WatchLastReplay()
        {
            if (OnlineVersusSession.IsActive) { SetStatus("Replays can be watched after leaving the online session."); return; }
            if (!VersusReplays.TryLoadLast(out var replay, out var error)) { SetStatus(error); return; }
            try { LocalVersusSession.StartReplay(replay); }
            catch (Exception exception) { Debug.LogException(exception); SetStatus(exception.Message); }
        }

        private string ReplayTitle()
        {
            var settings = LocalVersusSession.Settings;
            return settings == null ? "Replay" : settings.PlayerOneName + " vs " + settings.PlayerTwoName;
        }

        private static string LobbyStatus(OnlineVersusSession session)
        {
            if (session == null || session.Peer == null) return string.Empty;
            string ping = session.Peer.RttMs >= 0 ? "Ping " + session.Peer.RttMs + " ms. " : string.Empty;
            if (session.IsHost) return ping + (session.Lobby.GuestReady ? session.RemoteName + " is ready." : "Waiting for " + session.RemoteName + " to ready up.");
            return ping + (session.LocalReady ? "Waiting for the host to start." : "Choose your weapon, then ready up.");
        }

        private static string ResultStatus(OnlineVersusSession session, string message)
        {
            if (session == null) return message ?? string.Empty;
            if (session.Phase == OnlinePhase.Closed) return session.Notice;
            string text = session.SyncProblem ?? message ?? session.Notice ?? string.Empty;
            if (session.LocalWantsRematch) text = (text.Length > 0 ? text + " " : "") + "Waiting for " + session.RemoteName + " to accept the rematch.";
            else if (session.RemoteWantsRematch) text = (text.Length > 0 ? text + " " : "") + session.RemoteName + " wants a rematch.";
            return text;
        }

        private static string ArenaLabel(string id) => VersusRoster.ArenaName(id);

        /// <summary>The loadout with the next roster weapon.</summary>
        private static VersusLoadout NextWeapon(VersusLoadout loadout)
        {
            var weapons = VersusRoster.Items(LoadoutSlot.Weapon);
            if (weapons.Count == 0) return loadout;
            int index = VersusRoster.IndexOf(LoadoutSlot.Weapon, loadout.Weapon);
            return loadout.With(LoadoutSlot.Weapon, weapons[(index + 1) % weapons.Count].Id);
        }

        /// <summary>The roster arena after <paramref name="current"/>; "random" joins the cycle when allowed.</summary>
        private static string NextArena(string current, bool allowRandom)
        {
            var arenas = VersusRoster.Arenas;
            if (arenas.Count == 0) return "dojo";
            int index = -1;
            for (int i = 0; i < arenas.Count; i++) if (arenas[i].Id == current) index = i;
            if (index + 1 < arenas.Count) return arenas[index + 1].Id;
            return allowRandom ? VersusRoster.RandomArena : arenas[0].Id;
        }

        private static string WinsLabel(int wins) => wins + (wins == 1 ? " WIN" : " WINS");

        private static string NetcodeLabel(OnlineVersusSession session) =>
            session.Lobby.Netcode == Online.NetcodeMode.Rollback ? "ROLLBACK" : "DELAY BASED";

        private static string DelayLabel(OnlineVersusSession session)
        {
            int delay = session.Lobby.InputDelay;
            return delay + (delay == 1 ? " FRAME" : " FRAMES") + (session.Peer != null && session.Peer.RttMs >= 0 ? "  (SUGGESTED " + session.SuggestedDelay + ")" : "");
        }

        private static int NetProtocolPort() => Online.NetProtocol.DefaultPort;

        private static System.Collections.Generic.List<string> LocalAddresses()
        {
            var result = new System.Collections.Generic.List<string>();
            try
            {
                foreach (var network in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (network.OperationalStatus != System.Net.NetworkInformation.OperationalStatus.Up ||
                        network.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Loopback) continue;
                    foreach (var unicast in network.GetIPProperties().UnicastAddresses)
                        if (unicast.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !result.Contains(unicast.Address.ToString()))
                            result.Add(unicast.Address.ToString());
                }
            }
            catch (Exception exception) { Debug.LogWarning("[Online] Could not list network addresses: " + exception.Message); }
            if (result.Count == 0) result.Add("127.0.0.1");
            if (result.Count > 3) result.RemoveRange(3, result.Count - 3);
            return result;
        }

        private void Pause(string reason)
        {
            if (inputFrame == Time.frameCount) return;
            inputFrame = Time.frameCount;
            LocalVersusSession.Pause(reason);
        }

        private void Resume()
        {
            if (inputFrame == Time.frameCount) return;
            bool keyboard = CurrentKeyboardPlayerOne();
            bool local = LocalVersusSession.Settings == null || LocalVersusSession.Settings.Mode == VersusMode.Local;
            if (local && !CurrentSchemeReady()) { SetStatus(PadReason(keyboard)); return; }
            inputFrame = Time.frameCount;
            LocalVersusSession.Resume();
        }

        private void TryStart(LocalVersusSettings settings)
        {
            try { LocalVersusSession.StartMatch(settings); }
            catch (Exception exception) { Debug.LogException(exception); SetStatus(string.IsNullOrEmpty(exception.Message) ? "The match could not start." : exception.Message); panel.gameObject.SetActive(true); IsShowing = true; }
        }

        private void ReadSettings(LocalVersusSettings settings)
        {
            if (p1Loadout == null) p1Loadout = VersusLoadouts.Load(VersusLoadouts.PlayerOne);
            if (p2Loadout == null) p2Loadout = VersusLoadouts.Load(VersusLoadouts.PlayerTwo);
            if (!VersusRoster.IsArena(arena)) arena = VersusRoster.Arenas.Count > 0 ? VersusRoster.Arenas[0].Id : "dojo";
            // Replays and online matches reuse these settings; only a local matchup carries over.
            if (settings == null || settings.Mode != VersusMode.Local) return;
            p1Loadout = settings.PlayerOneLoadout;
            p2Loadout = settings.PlayerTwoLoadout;
            if (VersusRoster.IsArena(settings.Location)) arena = settings.Location;
            keyboardPlayerOne = settings.KeyboardPlayerOne; sharedKeyboard = settings.SharedKeyboard; winsRequired = Mathf.Clamp(settings.WinsRequired, 1, 3); roundTime = settings.RoundTimeSeconds;
        }

        private bool CurrentKeyboardPlayerOne() { return LocalVersusSession.Settings != null ? LocalVersusSession.Settings.KeyboardPlayerOne : keyboardPlayerOne; }
        private string SchemeLabel() { return sharedKeyboard ? "SHARED KEYBOARD" : keyboardPlayerOne ? "KEYBOARD + GAMEPAD" : "2 GAMEPADS"; }
        private static bool SchemeReady(bool keyboard, bool shared) { return shared || PadsReady(keyboard); }
        private bool CurrentSchemeReady()
        {
            var settings = LocalVersusSession.Settings;
            // Only local versus binds devices to players; training takes any device, replays none.
            if (settings != null && settings.Mode != VersusMode.Local) return true;
            return settings != null ? LocalVersusSession.DevicesReady(settings) : SchemeReady(keyboardPlayerOne, sharedKeyboard);
        }
        private static bool PadsReady(bool keyboard) { return FightGamepadInput.IsConnected(GamePad.Player.One) && (keyboard || FightGamepadInput.IsConnected(GamePad.Player.Two)); }
        private static string PadReason(bool keyboard)
        {
            if (!FightGamepadInput.IsConnected(GamePad.Player.One)) return "Connect Gamepad 1 to continue.";
            return !keyboard && !FightGamepadInput.IsConnected(GamePad.Player.Two) ? "Connect Gamepad 2 to continue." : "Ready.";
        }

        private static string InputHint(bool keyboard)
        {
            if (!keyboard) return "Player 1: Gamepad 1     Player 2: Gamepad 2";
            return "P1 move " + FightKeyBindings.Display(FightKeyBindings.Get(KeyCode.W)) + "/" + FightKeyBindings.Display(FightKeyBindings.Get(KeyCode.A)) + "/" +
                FightKeyBindings.Display(FightKeyBindings.Get(KeyCode.S)) + "/" + FightKeyBindings.Display(FightKeyBindings.Get(KeyCode.D)) +
                "  Punch " + FightKeyBindings.Display(FightKeyBindings.Get(KeyCode.O)) + "  Kick " + FightKeyBindings.Display(FightKeyBindings.Get(KeyCode.P)) +
                "     P2: Gamepad 1";
        }

        private void RefreshDeviceStatus()
        {
            padsWereReady = SchemeReady(keyboardPlayerOne, sharedKeyboard);
            SetStatus(sharedKeyboard ? SharedKeyboard.Hint : padsWereReady ? InputHint(keyboardPlayerOne) : PadReason(keyboardPlayerOne));
        }

        private void Rebuild(string title, string subtitle, Action<RectTransform> build, Action back = null, float width = 760)
        {
            panel.gameObject.SetActive(true);
            liveLabels.Clear();
            shortcuts.Clear();
            backAction = back;
            for (int i = panel.childCount - 1; i >= 0; i--) { panel.GetChild(i).gameObject.SetActive(false); Destroy(panel.GetChild(i).gameObject); }
            // The lobby shows the living arena; pause and results keep the fight visible under an ink wash.
            SetImage(panel, IsBackdropPage ? new Color(0, 0, 0, 0) : new Color(20f / 255f, 14f / 255f, 11f / 255f, .7f));
            EnterBackdrop();
            var paper = Rect(panel, "Paper"); paper.anchorMin = paper.anchorMax = paper.pivot = new Vector2(.5f, .5f); paper.sizeDelta = new Vector2(width, 660);
            var card = paper.gameObject.AddComponent<Eclipse.UI.PaperPanel>(); card.color = Paper; card.raycastTarget = true;
            // Title painted on a red brush stroke rather than a flat band.
            var header = Rect(paper, "Header"); header.anchorMin = new Vector2(0, 1); header.anchorMax = new Vector2(1, 1); header.pivot = new Vector2(.5f, 1); header.anchoredPosition = new Vector2(0, -14); header.sizeDelta = new Vector2(-40, 82);
            var stroke = header.gameObject.AddComponent<Eclipse.UI.InkStroke>(); stroke.color = Red; stroke.raycastTarget = false; stroke.Seed = title.Length * 31;
            Label(header, title, 38, Paper, TextAnchor.MiddleCenter);
            Eclipse.UI.UiReveal.Play(paper, 0f, .34f, new Vector2(0, -18), .96f);
            StartCoroutine(PaintStroke(stroke));
            var sub = Label(paper, subtitle, 21, Ink, TextAnchor.MiddleCenter); sub.rectTransform.anchorMin = new Vector2(0, 1); sub.rectTransform.anchorMax = new Vector2(1, 1); sub.rectTransform.pivot = new Vector2(.5f, 1); sub.rectTransform.anchoredPosition = new Vector2(0, -105); sub.rectTransform.sizeDelta = new Vector2(-40, 48);
            var body = Rect(paper, "Options"); body.anchorMin = body.anchorMax = body.pivot = new Vector2(.5f, 1); body.anchoredPosition = new Vector2(0, -150); body.sizeDelta = new Vector2(width - 110, 440);
            var layout = body.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>(); layout.spacing = 9; layout.childControlHeight = layout.childControlWidth = true; layout.childForceExpandHeight = false; layout.childForceExpandWidth = true;
            build(body);
            // The card fits its content, so short pages don't stretch their rows or float in empty paper.
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(body);
            float content = UnityEngine.UI.LayoutUtility.GetPreferredHeight(body);
            body.sizeDelta = new Vector2(width - 110, content);
            paper.sizeDelta = new Vector2(width, Mathf.Clamp(150 + content + 74, 340, 680));
            status = Label(paper, "", 16, Red, TextAnchor.MiddleCenter); status.rectTransform.anchorMin = new Vector2(0, 0); status.rectTransform.anchorMax = new Vector2(1, 0); status.rectTransform.pivot = new Vector2(.5f, 0); status.rectTransform.anchoredPosition = new Vector2(0, 14); status.rectTransform.sizeDelta = new Vector2(-40, 52);
            IsShowing = true; Cursor.visible = true; Cursor.lockState = CursorLockMode.None;
            Eclipse.UI.EclipseUiAudio.SuppressFocusSound();
            Eclipse.UI.EclipseUiAudio.Play(Eclipse.UI.UiSound.Open);
            FocusFirst(body);
        }

        private static System.Collections.IEnumerator PaintStroke(Eclipse.UI.InkStroke stroke)
        {
            float start = Time.unscaledTime + .12f;
            while (stroke != null)
            {
                float t = Mathf.Clamp01((Time.unscaledTime - start) / .32f);
                stroke.Fill = 1f - (1f - t) * (1f - t);
                if (t >= 1f) yield break;
                yield return null;
            }
        }

        private void AddChoice(RectTransform parent, string caption, Func<string> value, Action cycle, float valueWidth = 340)
        {
            var row = Rect(parent, caption); row.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredHeight = 46;
            var horizontal = row.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>(); horizontal.spacing = 12; horizontal.childControlWidth = horizontal.childControlHeight = true; horizontal.childForceExpandWidth = false;
            var left = Label(row, caption, 22, Ink, TextAnchor.MiddleLeft); left.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().flexibleWidth = 1;
            UnityEngine.UI.Text valueLabel = null;
            var button = AddButton(row, value() + "   >", () => { cycle(); valueLabel.text = value() + "   >"; }, valueWidth, Eclipse.UI.UiSound.Toggle);
            valueLabel = button.GetComponentInChildren<UnityEngine.UI.Text>();
            liveLabels.Add((valueLabel, () => value() + "   >"));
        }

        /// <summary>A read-only caption/value row whose value refreshes every frame.</summary>
        private void AddInfo(RectTransform parent, string caption, Func<string> value)
        {
            var row = Rect(parent, caption); row.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredHeight = 46;
            var horizontal = row.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>(); horizontal.spacing = 12; horizontal.childControlWidth = horizontal.childControlHeight = true; horizontal.childForceExpandWidth = false;
            var left = Label(row, caption, 22, Ink, TextAnchor.MiddleLeft); left.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().flexibleWidth = 1;
            var right = Label(row, value(), 22, Red, TextAnchor.MiddleRight); var element = right.gameObject.AddComponent<UnityEngine.UI.LayoutElement>(); element.minWidth = element.preferredWidth = 340;
            liveLabels.Add((right, value));
        }

        private RectTransform AddRow(RectTransform parent)
        {
            var row = Rect(parent, "Row"); row.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredHeight = 46;
            var horizontal = row.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>(); horizontal.spacing = 12; horizontal.childControlWidth = horizontal.childControlHeight = true; horizontal.childForceExpandWidth = true;
            return row;
        }

        private UnityEngine.UI.InputField AddTextField(RectTransform parent, string caption, string value, int limit, string placeholder = null, float width = 340)
        {
            var row = Rect(parent, caption); row.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredHeight = 46;
            var horizontal = row.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>(); horizontal.spacing = 12; horizontal.childControlWidth = horizontal.childControlHeight = true; horizontal.childForceExpandWidth = false;
            var left = Label(row, caption, 22, Ink, TextAnchor.MiddleLeft); left.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().flexibleWidth = 1;
            var box = Rect(row, caption + " Field"); var element = box.gameObject.AddComponent<UnityEngine.UI.LayoutElement>(); element.minWidth = element.preferredWidth = width;
            // Written on the paper: a faint ink wash over a ruled line, darkening while focused.
            var image = box.gameObject.AddComponent<UnityEngine.UI.Image>(); image.color = new Color(Ink.r, Ink.g, Ink.b, .12f); image.raycastTarget = true;
            var rule = Rect(box, "Rule"); rule.anchorMin = Vector2.zero; rule.anchorMax = new Vector2(1, 0); rule.pivot = new Vector2(.5f, 0); rule.sizeDelta = new Vector2(0, 2);
            var ruleImage = rule.gameObject.AddComponent<UnityEngine.UI.Image>(); ruleImage.color = new Color(Ink.r, Ink.g, Ink.b, .7f); ruleImage.raycastTarget = false;
            var text = Label(box, "", 22, Ink, TextAnchor.MiddleLeft); text.supportRichText = false; text.rectTransform.offsetMin = new Vector2(12, 0); text.rectTransform.offsetMax = new Vector2(-12, 0);
            var hint = Label(box, placeholder ?? "", 20, new Color(Ink.r, Ink.g, Ink.b, .45f), TextAnchor.MiddleLeft); hint.fontStyle = FontStyle.Italic; hint.rectTransform.offsetMin = new Vector2(12, 0); hint.rectTransform.offsetMax = new Vector2(-12, 0);
            var field = box.gameObject.AddComponent<UnityEngine.UI.InputField>();
            field.textComponent = text; field.placeholder = hint; field.targetGraphic = image;
            var tint = field.colors; tint.normalColor = new Color(1, 1, 1, .6f); tint.highlightedColor = tint.selectedColor = tint.pressedColor = Color.white; tint.fadeDuration = .08f; field.colors = tint;
            field.customCaretColor = true; field.caretColor = Red; field.caretWidth = 2; field.selectionColor = new Color(Red.r, Red.g, Red.b, .3f);
            field.lineType = UnityEngine.UI.InputField.LineType.SingleLine; field.characterLimit = limit;
            field.text = value ?? string.Empty;
            return field;
        }

        private UnityEngine.UI.Button AddButton(RectTransform parent, string text, Action action, float width = -1,
            Eclipse.UI.UiSound? sound = null)
        {
            var rect = Rect(parent, text); var element = rect.gameObject.AddComponent<UnityEngine.UI.LayoutElement>(); element.preferredHeight = 46; if (width > 0) { element.minWidth = element.preferredWidth = width; element.flexibleWidth = 0; } else if (width == 0) element.flexibleWidth = 1;
            // A brush-stroke plate; the stroke itself is also the hit area.
            var plate = rect.gameObject.AddComponent<Eclipse.UI.InkStroke>(); plate.color = Ink; plate.Seed = text.GetHashCode() & 0xffff;
            var button = rect.gameObject.AddComponent<UnityEngine.UI.Button>(); button.targetGraphic = plate;
            var label = Label(rect, text, 22, Paper, TextAnchor.MiddleCenter);
            var fx = Eclipse.UI.EclipseUiButton.Attach(button, plate, label, Ink, Red, Paper, Paper, 6f, .02f);
            var cue = sound ?? (text == "START MATCH" || text == "REMATCH" ? Eclipse.UI.UiSound.Begin
                : text == "RETURN TO TITLE" || text == "RESUME" || text == "BACK" || text == "LEAVE" || text == "CANCEL" ? Eclipse.UI.UiSound.Back : Eclipse.UI.UiSound.Confirm);
            button.onClick.AddListener(() => { fx.Punch(); Eclipse.UI.EclipseUiAudio.Play(cue); action(); });
            return button;
        }

        private UnityEngine.UI.Text Label(Transform parent, string text, int size, Color color, TextAnchor alignment)
        {
            var rect = Rect(parent, "Label"); Stretch(rect); var label = rect.gameObject.AddComponent<UnityEngine.UI.Text>(); label.font = font; label.fontSize = size; label.text = text; label.color = color; label.alignment = alignment; label.raycastTarget = false; return label;
        }

        private static RectTransform Rect(Transform parent, string name) { var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>(); rect.SetParent(parent, false); return rect; }
        private static void Stretch(RectTransform rect) { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; }
        private static void AddImage(RectTransform rect, Color color) { var image = rect.gameObject.AddComponent<UnityEngine.UI.Image>(); image.color = color; image.raycastTarget = true; }
        private static void SetImage(RectTransform rect, Color color) { var image = Eclipse.UI.ComponentUtility.Ensure<UnityEngine.UI.Image>(rect.gameObject); image.color = color; image.raycastTarget = true; }
        private void SetStatus(string text) { if (status != null) status.text = text ?? string.Empty; }

        private void EnsureEventSystem()
        {
            if (EventSystem.current == null)
                ownedEventSystem = new GameObject("Local Versus Input", typeof(EventSystem), typeof(UnityEngine.InputSystem.UI.InputSystemUIInputModule)).GetComponent<EventSystem>();
            Eclipse.Input.EclipseUiInput.Ensure(EventSystem.current);
            if (navigationEventSystem == EventSystem.current) return;
            RestoreNavigation();
            navigationEventSystem = EventSystem.current;
            previousNavigation = navigationEventSystem.sendNavigationEvents;
            navigationEventSystem.sendNavigationEvents = false;
        }

        private void RestoreNavigation()
        {
            if (navigationEventSystem != null) navigationEventSystem.sendNavigationEvents = previousNavigation;
            navigationEventSystem = null;
            heldNavigation = Vector2Int.zero;
        }

        // Explicit navigation keeps the recovered WASD axes out of text entry,
        // and uses the same numbered D-pad/stick axes as combat.
        private void UpdateNavigation()
        {
            var events = EventSystem.current;
            if (events == null) return;
            var selected = events.currentSelectedGameObject;
            var field = selected == null ? null : selected.GetComponent<UnityEngine.UI.InputField>();
            bool typing = field != null && field.isFocused;
            var pad = GamePad.GetStick(GamePad.Stick.Dpad, GamePad.Player.Any, true);
            var stick = GamePad.GetStick(GamePad.Stick.LeftStick, GamePad.Player.Any, true);
            if (stick.sqrMagnitude > pad.sqrMagnitude) pad = stick;
            var direction = new Vector2Int(Mathf.Abs(pad.x) > .6f ? (pad.x > 0 ? 1 : -1) : 0,
                Mathf.Abs(pad.y) > .6f ? (pad.y > 0 ? 1 : -1) : 0);
            bool keyboardMove = false;
            if (!typing)
            {
                if (Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.DownArrow) || Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.Tab)) { direction = Vector2Int.down; keyboardMove = true; }
                else if (Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.UpArrow)) { direction = Vector2Int.up; keyboardMove = true; }
                else if (Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.LeftArrow)) { direction = Vector2Int.left; keyboardMove = true; }
                else if (Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.RightArrow)) { direction = Vector2Int.right; keyboardMove = true; }
            }
            if (direction != Vector2Int.zero && (keyboardMove || direction != heldNavigation || Time.unscaledTime >= repeatNavigationAt))
            {
                repeatNavigationAt = Time.unscaledTime + (direction != heldNavigation ? .38f : .1f);
                if (typing) field.DeactivateInputField();
                if (selected == null || !selected.activeInHierarchy || !selected.transform.IsChildOf(panel)) FocusFirst(panel);
                else
                {
                    var move = new AxisEventData(events) { moveVector = new Vector2(direction.x, direction.y),
                        moveDir = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? (direction.x > 0 ? MoveDirection.Right : MoveDirection.Left) :
                            direction.y > 0 ? MoveDirection.Up : MoveDirection.Down };
                    ExecuteEvents.Execute(selected, move, ExecuteEvents.moveHandler);
                }
            }
            heldNavigation = keyboardMove ? Vector2Int.zero : direction;
            if (typing) return;
            if (IsBackdropPage && backAction != null && GamePad.GetButtonDown(GamePad.Button.B, GamePad.Player.Any))
            { Eclipse.UI.EclipseUiAudio.Play(Eclipse.UI.UiSound.Back); backAction(); return; }
            if (Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.Return) || Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.KeypadEnter) ||
                Eclipse.Input.EclipseInput.GetKeyDown(KeyCode.Space) || GamePad.GetButtonDown(GamePad.Button.A, GamePad.Player.Any))
            {
                selected = events.currentSelectedGameObject;
                if (selected != null && selected.activeInHierarchy && selected.transform.IsChildOf(panel))
                    ExecuteEvents.Execute(selected, new BaseEventData(events), ExecuteEvents.submitHandler);
            }
        }

        private static void FocusFirst(RectTransform body)
        {
            if (EventSystem.current == null) return;
            var buttons = body.GetComponentsInChildren<UnityEngine.UI.Button>(true); if (buttons.Length > 0) EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        }
    }
}

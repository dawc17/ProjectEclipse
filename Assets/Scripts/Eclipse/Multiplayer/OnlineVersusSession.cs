using System;
using System.Diagnostics;
using System.Linq;
using Eclipse.Multiplayer.Online;
using Eclipse.Multiplayer.Online.Rooms;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Eclipse.Multiplayer
{
    public enum OnlinePhase { None, Hosting, Connecting, Lobby, Starting, InMatch, Result, Closed }

    /// <summary>
    /// The online half of versus: connection, lobby agreement, match start, rematch
    /// and failure handling. The host plays the left fighter and owns the matchup.
    /// </summary>
    public sealed class OnlineVersusSession : MonoBehaviour
    {
        private const string NamePreference = "Eclipse.Online.PlayerName";
        private const string AddressPreference = "Eclipse.Online.LastAddress";
        private const string PortPreference = "Eclipse.Online.Port";
        private const string DelayPreference = "Eclipse.Online.InputDelay";
        private const string NetcodePreference = "Eclipse.Online.Netcode";
        private static readonly Stopwatch Clock = Stopwatch.StartNew();
        public static long NowMs => Clock.ElapsedMilliseconds;

        public static OnlineVersusSession Current { get; private set; }
        public static bool IsActive => Current != null;

        public NetplayPeer Peer { get; private set; }
        public OnlinePhase Phase { get; private set; }
        public bool IsHost => Peer != null && Peer.IsHost;
        public string LocalName { get; private set; }
        public string RemoteName => Peer?.RemoteIdentity?.PlayerName ?? "Opponent";
        public LobbyState Lobby { get; private set; } = new LobbyState();
        /// <summary>Loaded in Awake: PlayerPrefs may not be read from a component's constructor.</summary>
        public VersusLoadout LocalLoadout { get; private set; }
        public bool LocalReady { get; private set; }
        public bool LocalWantsRematch { get; private set; }
        public bool RemoteWantsRematch { get; private set; }
        public string Notice { get; private set; } = string.Empty;
        public MatchStart CurrentMatch { get; private set; }
        /// <summary>Set when the peers' simulations disagreed during the last match.</summary>
        public string SyncProblem { get; private set; }

        private int _nextMatchIndex = 1;
        private OnlineInputSource _source;
        private (int winner, int left, int right, int tick)? _localResult, _remoteResult;
        private bool _remoteLeftToLobby;
        private static bool _savedRunInBackground;
        private static bool _overridingRunInBackground;
        private (int winner, string message)? _pendingEnd;

        /// <summary>
        /// Game version plus scripting runtime. Mono (the editor, Mono players) and IL2CPP
        /// builds round fight math differently and desync within seconds, so they never
        /// match. CPU and OS may differ: IL2CPP x64 and ARM64 stayed in sync in testing.
        /// The per-tick state hash still catches anything else that changes the simulation.
        /// </summary>
        public static string BuildId => Application.version + "/" + Runtime;

        public const string Runtime =
#if ENABLE_IL2CPP
            "IL2CPP";
#else
            "Mono";
#endif

        /// <summary>Enabled mods and versions; peers and replays must agree on gameplay content.</summary>
        public static string ContentFingerprint()
        {
            const string combat = ";combat:" + LocalVersusMatch.CombatBalanceId;
            try
            {
                if (!Eclipse.Modding.ModRuntime.IsInitialized) return "mods:unloaded" + combat;
                var mods = Eclipse.Modding.ModRuntime.Host.EnabledMods.Select(mod => mod.Id + "@" + mod.Version).OrderBy(id => id, StringComparer.Ordinal).ToArray();
                // Loadouts travel as roster indices, so the roster must match too.
                return (mods.Length == 0 ? "mods:none" : "mods:" + string.Join(",", mods)) + ";roster:" + VersusRoster.Fingerprint
                    + combat;
            }
            catch (Exception exception)
            {
                return "mods:error:" + exception.GetType().Name + combat;
            }
        }

        /// <summary>Platform, CPU and scripting backend, for desync reports.</summary>
        public static string Platform =>
            Application.platform + " " + System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture + " " + Runtime;

        public static string SavedName
        {
            get => Eclipse.Runtime.EditorPlayModeContext.Role ?? PlayerPrefs.GetString(NamePreference, "Player");
            set { if (Eclipse.Runtime.EditorPlayModeContext.Role == null) PlayerPrefs.SetString(NamePreference, value ?? "Player"); }
        }
        public static string SavedAddress { get => PlayerPrefs.GetString(AddressPreference, ""); set => PlayerPrefs.SetString(AddressPreference, value ?? ""); }
        public static int SavedPort { get => PlayerPrefs.GetInt(PortPreference, NetProtocol.DefaultPort); set => PlayerPrefs.SetInt(PortPreference, value); }
        public static int SavedDelay { get => Mathf.Clamp(PlayerPrefs.GetInt(DelayPreference, NetProtocol.DefaultInputDelay), 0, NetProtocol.MaxInputDelay); set => PlayerPrefs.SetInt(DelayPreference, value); }
        public static NetcodeMode SavedNetcode
        {
            get => PlayerPrefs.GetInt(NetcodePreference, (int)NetcodeMode.Rollback) == (int)NetcodeMode.Delay ? NetcodeMode.Delay : NetcodeMode.Rollback;
            set => PlayerPrefs.SetInt(NetcodePreference, (int)value);
        }

        public static void Host(string playerName, int port)
        {
            var session = Create(playerName);
            try { session.Peer = NetplayPeer.Host(port, session.Identity(), NowMs); }
            catch (Exception exception)
            {
                Destroy(session.gameObject);
                throw new InvalidOperationException("Could not open port " + port + ": " + exception.Message, exception);
            }
            session.Phase = OnlinePhase.Hosting;
            var netcode = SavedNetcode;
            session.Lobby = new LobbyState
            {
                HostLoadout = session.LocalLoadout.ToCode(),
                BalanceHash = PvpBalanceProfiles.Selected.Hash,
                BalanceName = PvpBalanceProfiles.Selected.Name,
                Arena = SavedArena,
                Netcode = netcode,
                InputDelay = netcode == NetcodeMode.Rollback ? NetProtocol.DefaultRollbackDelay : SavedDelay,
            };
            Debug.Log("[Online] Hosting on UDP port " + session.Peer.LocalPort + " (" + BuildId + ", " + ContentFingerprint() + ").");
        }

        public static void Join(string playerName, string address)
        {
            if (!NetplayPeer.TryParseAddress(address, out var endPoint, out var error)) throw new ArgumentException(error);
            var session = Create(playerName);
            try { session.Peer = NetplayPeer.Join(endPoint, session.Identity(), NowMs); }
            catch (Exception exception)
            {
                Destroy(session.gameObject);
                throw new InvalidOperationException("Could not open a network socket: " + exception.Message, exception);
            }
            session.Phase = OnlinePhase.Connecting;
            Debug.Log("[Online] Joining " + endPoint + ".");
        }

        /// <summary>
        /// One fight arranged by a room: the peer is already routed (punched or relayed),
        /// both sides are ready, and the host starts with the room's settings as soon as
        /// it has a ping sample to pick the input delay from.
        /// </summary>
        public static void StartRoomFight(NetplayPeer peer, RoomPairing pairing, RoomSettings settings, string playerName, VersusLoadout localLoadout)
        {
            var session = Create(playerName);
            session.Peer = peer;
            session.RoomMatch = pairing;
            session._roomStartedMs = NowMs;
            session.LocalLoadout = (localLoadout ?? VersusLoadout.Default).Sanitized();
            session.LocalReady = true;
            session.Phase = peer.IsHost ? OnlinePhase.Hosting : OnlinePhase.Connecting;
            session.Lobby = new LobbyState
            {
                HostLoadout = session.LocalLoadout.ToCode(),
                BalanceHash = PvpBalanceProfiles.Selected.Hash,
                BalanceName = PvpBalanceProfiles.Selected.Name,
                Arena = VersusRoster.ResolveArena(settings.Arena, pairing.Seed),
                WinsRequired = settings.WinsRequired,
                RoundTimeSeconds = settings.RoundTimeSeconds,
                Netcode = NetcodeMode.Rollback,
                InputDelay = NetProtocol.DefaultRollbackDelay,
            };
            Debug.Log("[Online] Room match " + pairing.MatchId + " vs " + pairing.PeerName + " as " + (peer.IsHost ? "host" : "guest") + ".");
        }

        /// <summary>Set when this session is one fight arranged by a room.</summary>
        public RoomPairing RoomMatch { get; private set; }
        /// <summary>Raised once with this side's view of how the room fight ended.</summary>
        public static event Action<RoomPairing, MatchOutcome, string> RoomFightOver;
        private long _roomStartedMs, _roomConnectedMs = -1;
        private bool _roomReported;

        private void ReportRoomFight(MatchOutcome outcome, string reason)
        {
            if (RoomMatch == null || _roomReported) return;
            _roomReported = true;
            Debug.Log("[Online] Room match " + RoomMatch.MatchId + " over: " + outcome + (string.IsNullOrEmpty(reason) ? "" : " (" + reason + ")"));
            RoomFightOver?.Invoke(RoomMatch, outcome, reason ?? "");
        }

        /// <summary>This side's winner (0 left, 1 right, -1 none) as a room outcome.</summary>
        private static MatchOutcome OutcomeFor(int winner) =>
            winner == 0 ? MatchOutcome.LeftWon : winner == 1 ? MatchOutcome.RightWon : MatchOutcome.Draw;

        public const int RoomConnectTimeoutMs = 15000;

        private void UpdateRoomFight()
        {
            if (RoomMatch == null) return;
            if ((Phase == OnlinePhase.Hosting || Phase == OnlinePhase.Connecting) && NowMs - _roomStartedMs > RoomConnectTimeoutMs)
            {
                // The opponent never arrived (crashed, or no path either way).
                Peer.Close("Could not connect to " + RoomMatch.PeerName + ".", false);
                return;
            }
            if (!IsHost || Phase != OnlinePhase.Lobby || !Lobby.GuestReady) return;
            if (_roomConnectedMs < 0) _roomConnectedMs = NowMs;
            // Wait briefly for a ping sample so the delay suits the connection.
            if (Peer.RttMs < 0 && NowMs - _roomConnectedMs < 2500) return;
            if (NowMs - _roomConnectedMs < 600) return;
            Lobby.InputDelay = SuggestedDelay;
            _seedOverride = RoomMatch.Seed;
            try { HostStart(); }
            catch (Exception exception) { Debug.LogException(exception); }
        }

        private int? _seedOverride;

        public static void Shutdown(string reason = "Left the session.")
        {
            var session = Current;
            if (session == null) return;
            // Clear immediately: Destroy is deferred, and menus check IsActive this frame.
            Current = null;
            session.Peer?.Close(reason);
            session.Peer?.Dispose();
            session.Peer = null;
            session._source = null;
            Destroy(session.gameObject);
        }

        private void Awake()
        {
            if (LocalLoadout == null) LocalLoadout = VersusLoadouts.Load(VersusLoadouts.Online);
        }

        private static OnlineVersusSession Create(string playerName)
        {
            Shutdown();
            var session = new GameObject("Eclipse Online Versus").AddComponent<OnlineVersusSession>();
            DontDestroyOnLoad(session.gameObject);
            // A backgrounded peer must keep simulating or its opponent stalls.
            if (!_overridingRunInBackground)
            {
                _savedRunInBackground = Application.runInBackground;
                _overridingRunInBackground = true;
            }
            Application.runInBackground = true;
            session.LocalName = new NetIdentity("", "", playerName).PlayerName;
            SavedName = session.LocalName;
            Current = session;
            return session;
        }

        private NetIdentity Identity() => new NetIdentity(BuildId, ContentFingerprint(), LocalName);

        private void OnDestroy()
        {
            _source = null;
            Peer?.Dispose();
            Peer = null;
            if (Current == this) Current = null;
            // Only the last session restores the player's setting.
            if (Current == null && _overridingRunInBackground)
            {
                Application.runInBackground = _savedRunInBackground;
                _overridingRunInBackground = false;
            }
        }

        private void OnApplicationQuit() => Peer?.Close("Closed the game.");

        private GUIStyle _hudStyle, _waitStyle;
        private long _waitNoticeUntilMs;

        // A minimal in-fight readout: connection quality, and a notice while stalled.
        private void OnGUI()
        {
            if (Peer == null || Phase != OnlinePhase.InMatch || _source == null || LocalVersusMenu.BlocksFightInput) return;
            float scale = Screen.height / 720f;
            if (_hudStyle == null)
            {
                _hudStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter };
                _waitStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            }
            _hudStyle.fontSize = Mathf.RoundToInt(14 * scale);
            _waitStyle.fontSize = Mathf.RoundToInt(22 * scale);
            string ping = Peer.RttMs >= 0 ? Peer.RttMs + " ms" : "-- ms";
            var line = new Rect(0, Screen.height - 26 * scale, Screen.width, 24 * scale);
            GUI.color = new Color(1f, 1f, 1f, .75f);
            var timeline = _source.Timeline;
            string readout = "PING " + ping + "   DELAY " + timeline.Delay + "F";
            if (timeline.IsRollback)
            {
                // How many ticks ran ahead on a guess, and the deepest correction so far.
                int ahead = Mathf.Max(0, timeline.SimulatedTicks - timeline.RemoteFrames);
                readout += "   ROLLBACK " + ahead + "F (MAX " + timeline.LongestRollback + ")";
            }
            if (Peer.LossPercent > 0) readout += "   LOSS " + Peer.LossPercent + "%";
            GUI.Label(line, readout, _hudStyle);
            GUI.color = Color.white;
            // Show after a real stall, and keep it up briefly so short, repeated stalls
            // (an opponent whose game runs slowly) read as one steady notice.
            if (_source.StalledMs > 350) _waitNoticeUntilMs = NowMs + 1500;
            if (NowMs < _waitNoticeUntilMs)
            {
                long silent = NowMs - Peer.LastReceiveMs;
                string text = silent > 1500
                    ? "Waiting for " + RemoteName + "...  " + (silent / 1000) + "s (gives up at " + NetplayPeer.TimeoutMs / 1000 + "s)"
                    : _source.StalledMs > 0 ? "Waiting for " + RemoteName + "..." : RemoteName + "'s game is running slowly";
                GUI.Box(new Rect(Screen.width / 2f - 300 * scale, Screen.height * .22f, 600 * scale, 56 * scale), text, _waitStyle);
            }
        }

        private void Update()
        {
            if (Peer == null) return;
            var before = Peer.State;
            Peer.Update(NowMs);
            if (before != NetplayState.Connected && Peer.State == NetplayState.Connected) OnConnected();
            while (Peer != null && Peer.TryReceiveReliable(out var message)) HandleMessage(message);
            if (Peer == null) return;
            if (_source != null && _source.Timeline.DesyncTick >= 0 && SyncProblem == null) OnDesync(_source.Timeline.DesyncTick);
            if (Peer.State == NetplayState.Closed && Phase != OnlinePhase.Closed) OnClosed(Peer.CloseReason);
            UpdateRoomFight();
        }

        private void OnConnected()
        {
            Phase = OnlinePhase.Lobby;
            Notice = IsHost ? RemoteName + " joined." : "Connected to " + RemoteName + ".";
            Debug.Log("[Online] " + Notice);
            if (IsHost) SendLobby();
            else SendGuestLobby();
            LocalVersusMenu.Ensure().OnOnlineChanged();
        }

        // ---- Lobby ----

        public void SetLocalLoadout(VersusLoadout loadout)
        {
            LocalLoadout = (loadout ?? VersusLoadout.Default).Sanitized();
            VersusLoadouts.Save(VersusLoadouts.Online, LocalLoadout);
            if (IsHost) { Lobby.HostLoadout = LocalLoadout.ToCode(); SendLobby(); }
            else SendGuestLobby();
        }

        /// <summary>The opponent's loadout as the lobby currently shows it, or null before they chose.</summary>
        public VersusLoadout RemoteLoadout
        {
            get
            {
                var code = IsHost ? Lobby.GuestLoadout : Lobby.HostLoadout;
                return code.IsSet && VersusLoadouts.TryFromCode(code, out var loadout) ? loadout : null;
            }
        }

        private const string ArenaPreference = "Eclipse.Online.Arena";
        public static string SavedArena
        {
            get
            {
                string arena = PlayerPrefs.GetString(ArenaPreference, VersusRoster.RandomArena);
                return arena == VersusRoster.RandomArena || VersusRoster.IsArena(arena) ? arena : VersusRoster.RandomArena;
            }
            set => PlayerPrefs.SetString(ArenaPreference, value ?? VersusRoster.RandomArena);
        }

        public void SetArena(string arena)
        {
            if (!IsHost || arena != VersusRoster.RandomArena && !VersusRoster.IsArena(arena)) return;
            Lobby.Arena = arena;
            SavedArena = arena;
            SendLobby();
        }

        public void ToggleReady()
        {
            if (IsHost) return;
            if (!PvpBalanceProfiles.TryFind(Lobby.BalanceHash, out _))
            {
                Notice = "Install the host's balance profile: " + Lobby.BalanceName + ". Its gameplay hash must match.";
                LocalReady = false;
                SendGuestLobby();
                return;
            }
            LocalReady = !LocalReady;
            SendGuestLobby();
        }

        public void SetBalance(Eclipse.Multiplayer.Balance.PvpBalanceSnapshot balance)
        {
            if (!IsHost || RoomMatch != null || balance == null ||
                Phase != OnlinePhase.Hosting && Phase != OnlinePhase.Lobby) return;
            if (Lobby.BalanceHash == balance.Hash) return;
            Lobby.BalanceHash = balance.Hash;
            Lobby.BalanceName = balance.Name;
            Lobby.GuestReady = false;
            LocalWantsRematch = RemoteWantsRematch = false;
            SendLobby();
        }

        public void CycleArena()
        {
            if (!IsHost) return;
            var arenas = VersusRoster.Arenas;
            int index = -1;
            for (int i = 0; i < arenas.Count; i++) if (arenas[i].Id == Lobby.Arena) index = i;
            SetArena(index + 1 >= arenas.Count ? VersusRoster.RandomArena : arenas[index + 1].Id);
        }

        public void CycleWins()
        {
            if (!IsHost) return;
            Lobby.WinsRequired = Lobby.WinsRequired % 3 + 1;
            SendLobby();
        }

        public void CycleDelay()
        {
            if (!IsHost) return;
            Lobby.InputDelay = Lobby.InputDelay >= 8 ? 0 : Lobby.InputDelay + 1;
            if (Lobby.Netcode == NetcodeMode.Delay) SavedDelay = Lobby.InputDelay;
            SendLobby();
        }

        /// <summary>Switches between rollback and delay-based netcode, with the delay each suggests.</summary>
        public void CycleNetcode()
        {
            if (!IsHost) return;
            Lobby.Netcode = Lobby.Netcode == NetcodeMode.Rollback ? NetcodeMode.Delay : NetcodeMode.Rollback;
            SavedNetcode = Lobby.Netcode;
            Lobby.InputDelay = Peer != null && Peer.RttMs >= 0 ? SuggestedDelay
                : Lobby.Netcode == NetcodeMode.Rollback ? NetProtocol.DefaultRollbackDelay : SavedDelay;
            SendLobby();
        }

        /// <summary>
        /// The input delay suited to the measured connection: in delay mode it hides the
        /// whole one-way trip; with rollback it stays small so input feels immediate.
        /// </summary>
        public int SuggestedDelay => NetcodeModes.SuggestedDelay(Lobby.Netcode, Peer != null ? Peer.RttMs : -1, Peer != null ? Peer.JitterMs : 0);

        public string StartBlocker()
        {
            if (!IsHost) return "Only the host can start.";
            if (Phase != OnlinePhase.Lobby && Phase != OnlinePhase.Result) return "Waiting for an opponent.";
            if (!Lobby.GuestReady) return RemoteName + " is not ready yet.";
            if (!PvpBalanceProfiles.TryFind(Lobby.BalanceHash, out _)) return "The selected balance profile is unavailable.";
            return null;
        }

        public void HostStart()
        {
            string blocker = StartBlocker();
            if (blocker != null) throw new InvalidOperationException(blocker);
            var start = new MatchStart
            {
                MatchIndex = _nextMatchIndex & 0xFF,
                HostLoadout = Lobby.HostLoadout,
                GuestLoadout = Lobby.GuestLoadout,
                Arena = Lobby.Arena,
                WinsRequired = Lobby.WinsRequired,
                RoundTimeSeconds = Lobby.RoundTimeSeconds,
                InputDelay = Lobby.InputDelay,
                Seed = _seedOverride ?? new System.Random().Next(),
                RollbackWindow = Lobby.Netcode == NetcodeMode.Rollback ? NetProtocol.DefaultRollbackWindow : 0,
                BalanceHash = Lobby.BalanceHash,
            };
            _seedOverride = null;
            Peer.SendReliable(start.Encode());
            BeginMatch(start);
        }

        private void SendLobby()
        {
            if (IsHost && Peer.State == NetplayState.Connected) Peer.SendReliable(Lobby.Encode());
            LocalVersusMenu.Ensure().OnOnlineChanged();
        }

        private void SendGuestLobby()
        {
            if (Peer.State == NetplayState.Connected) Peer.SendReliable(NetMessages.GuestLobby(LocalLoadout.ToCode(), LocalReady, Lobby.BalanceHash));
            LocalVersusMenu.Ensure().OnOnlineChanged();
        }

        // ---- Match ----

        private void BeginMatch(MatchStart start)
        {
            if (!VersusLoadouts.TryFromCode(start.HostLoadout, out var hostLoadout) || !VersusLoadouts.TryFromCode(start.GuestLoadout, out var guestLoadout) ||
                start.Arena != VersusRoster.RandomArena && !VersusRoster.IsArena(start.Arena) || start.WinsRequired < 1 || start.WinsRequired > 5 ||
                start.RoundTimeSeconds < 30 || start.RoundTimeSeconds > 300)
            {
                Peer.Close("The match settings were not recognised.");
                OnClosed("The host sent a matchup this build does not support.");
                return;
            }
            CurrentMatch = start;
            if (IsHost && RoomMatch != null) RoomSession.Current?.Client.PublishStart(RoomMatch.MatchId, start);
            _source = null;
            _nextMatchIndex = start.MatchIndex + 1;
            LocalWantsRematch = RemoteWantsRematch = false;
            _localResult = _remoteResult = null;
            _remoteLeftToLobby = false;
            _pendingEnd = null;
            SyncProblem = null;
            Notice = string.Empty;
            Phase = OnlinePhase.Starting;
            var timeline = new InputTimeline(start.MatchIndex, start.InputDelay, start.RollbackWindow);
            Peer.Timeline = timeline;
            string hostName = IsHost ? LocalName : RemoteName;
            string guestName = IsHost ? RemoteName : LocalName;
            // Both peers resolve a random arena from the shared seed.
            string arena = VersusRoster.ResolveArena(start.Arena, start.Seed);
            Debug.Log("[Online] Match " + start.MatchIndex + ": " + hostLoadout + " vs " + guestLoadout + " at " + arena +
                ", input delay " + start.InputDelay + ", " + (start.RollbackWindow > 0 ? "rollback window " + start.RollbackWindow : "delay-based") +
                ", seed " + start.Seed + ".");
            try
            {
                if (!PvpBalanceProfiles.TryFind(start.BalanceHash, out var balance))
                    throw new InvalidOperationException("The match's balance profile is unavailable or differs from the host's.");
                var settings = new LocalVersusSettings(hostLoadout, guestLoadout, arena, true,
                    start.WinsRequired, start.RoundTimeSeconds, VersusMode.Online, hostName, guestName, start.Seed, balance: balance);
                LocalVersusSession.StartMatch(settings, () => _source = new OnlineInputSource(this, settings, timeline, IsHost ? 0 : 1));
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                Peer.Close("The match could not load.");
                OnClosed("The match could not start: " + exception.Message);
            }
        }

        internal void OnFightStarted() { if (Phase == OnlinePhase.Starting) Phase = OnlinePhase.InMatch; }

        /// <summary>The fight exists and its tick driver runs; apply an end that arrived while it loaded.</summary>
        internal void OnFightReady(Fight fight)
        {
            if (_pendingEnd == null) return;
            var end = _pendingEnd.Value;
            _pendingEnd = null;
            LocalVersusSession.CompleteOnline(fight, end.winner, end.message);
        }

        /// <summary>Ends the current match now, or as soon as a still-loading fight is ready.</summary>
        private void EndMatchEarly(int winner, string message)
        {
            var fight = Fight.GetCurrentFight();
            if (fight != null && fight.IsLocalVersus && VersusTickDriver.Owns(fight)) LocalVersusSession.CompleteOnline(fight, winner, message);
            else _pendingEnd = (winner, message);
        }

        private bool MatchRunning => Phase == OnlinePhase.InMatch || Phase == OnlinePhase.Starting;

        internal void OnLocalResult(int winner, int leftRounds, int rightRounds, int finalTick)
        {
            if (Phase == OnlinePhase.Closed) return;
            Phase = OnlinePhase.Result;
            _localResult = (winner, leftRounds, rightRounds, finalTick);
            if (CurrentMatch != null) Peer.SendReliable(NetMessages.MatchResult(CurrentMatch.MatchIndex, winner < 0 ? 255 : winner, leftRounds, rightRounds, finalTick));
            CompareResults();
            ReportRoomFight(OutcomeFor(winner), null);
            if (_remoteLeftToLobby) ShowLobbyFromResult();
        }

        public void Forfeit()
        {
            if (CurrentMatch == null || !MatchRunning) return;
            Peer.SendReliable(NetMessages.Simple(NetMessageType.Forfeit, CurrentMatch.MatchIndex));
            Notice = "You forfeited the match.";
            Phase = OnlinePhase.Result;
            ReportRoomFight(OutcomeFor(IsHost ? 1 : 0), "forfeit");
            EndMatchEarly(IsHost ? 1 : 0, Notice);
        }

        public void RequestRematch()
        {
            if (Phase != OnlinePhase.Result || CurrentMatch == null || Peer == null) return;
            LocalWantsRematch = true;
            Peer.SendReliable(NetMessages.Simple(NetMessageType.RequestRematch, CurrentMatch.MatchIndex));
            TryStartRematch();
            LocalVersusMenu.Ensure().OnOnlineChanged();
        }

        public void ReturnToLobby()
        {
            if (Peer.State == NetplayState.Connected) Peer.SendReliable(NetMessages.Simple(NetMessageType.ReturnToLobby, CurrentMatch?.MatchIndex ?? 0));
            ShowLobbyFromResult();
        }

        private void ShowLobbyFromResult()
        {
            if (Phase == OnlinePhase.Closed) return;
            Phase = OnlinePhase.Lobby;
            LocalWantsRematch = RemoteWantsRematch = false;
            _remoteLeftToLobby = false;
            if (!IsHost) { LocalReady = false; SendGuestLobby(); }
            else { Lobby.GuestReady = false; SendLobby(); }
            LocalVersusSession.ShowLobby();
        }

        private void TryStartRematch()
        {
            if (IsHost && LocalWantsRematch && RemoteWantsRematch && Phase == OnlinePhase.Result)
            {
                Lobby.GuestReady = true;
                HostStart();
            }
        }

        private void CompareResults()
        {
            if (_localResult == null || _remoteResult == null) return;
            if (_localResult.Value.Equals(_remoteResult.Value)) return;
            SyncProblem = "The two games disagreed about the result. This match was out of sync.";
            Debug.LogError("[Online] Result mismatch: local " + _localResult + ", remote " + _remoteResult + ".");
            LocalVersusMenu.Ensure().OnOnlineChanged();
        }

        internal void OnDesync(int tick)
        {
            if (SyncProblem != null || Phase == OnlinePhase.Closed) return;
            SyncProblem = "The games fell out of sync at tick " + tick + ". A replay was saved for diagnosis.";
            ReportRoomFight(MatchOutcome.Aborted, "desync at tick " + tick);
            Debug.LogError("[Online] Desync at tick " + tick + ". " + VersusStateHash.Describe(Fight.GetCurrentFight()));
            if (CurrentMatch != null)
            {
                Peer.SendReliable(NetMessages.Desync(CurrentMatch.MatchIndex, tick));
                string mine = _source != null && _source.TryGetSnapshot(tick, out var snapshot) ? VersusStateHash.Format(snapshot) : "(no snapshot for this tick)";
                Debug.LogError("[Online] Local state at tick " + tick + " (" + Platform + "): " + mine);
                Peer.SendReliable(NetMessages.DesyncReport(CurrentMatch.MatchIndex, tick, Platform, mine));
            }
            _source?.SaveDiagnostic("desync");
            if (MatchRunning)
            {
                Phase = OnlinePhase.Result;
                EndMatchEarly(-1, SyncProblem);
            }
            else LocalVersusMenu.Ensure().OnOnlineChanged();
        }

        // ---- Messages ----

        private void HandleMessage(byte[] message)
        {
            try
            {
                var reader = new NetReader(message, 1, message.Length - 1);
                switch ((NetMessageType)message[0])
                {
                    case NetMessageType.GuestLobby when IsHost:
                        var guestLoadout = LoadoutCode.Read(reader);
                        bool ready = reader.Bool();
                        string readyBalanceHash = reader.Str();
                        if (VersusLoadouts.TryFromCode(guestLoadout, out _)) Lobby.GuestLoadout = guestLoadout;
                        Lobby.GuestReady = ready && readyBalanceHash == Lobby.BalanceHash && VersusLoadouts.TryFromCode(Lobby.GuestLoadout, out _);
                        SendLobby();
                        break;
                    case NetMessageType.HostLobby when !IsHost:
                        var nextLobby = LobbyState.Decode(reader);
                        bool balanceChanged = Lobby.BalanceHash != nextLobby.BalanceHash;
                        Lobby = nextLobby;
                        if (balanceChanged)
                        {
                            bool available = PvpBalanceProfiles.TryFind(Lobby.BalanceHash, out _);
                            LocalReady = RoomMatch != null && available;
                            Notice = available ? "Balance profile: " + Lobby.BalanceName + "." : "Install the host's balance profile: " + Lobby.BalanceName + ". Its gameplay hash must match.";
                            SendGuestLobby();
                        }
                        LocalVersusMenu.Ensure().OnOnlineChanged();
                        break;
                    case NetMessageType.StartMatch when !IsHost:
                        BeginMatch(MatchStart.Decode(reader));
                        break;
                    case NetMessageType.RequestRematch:
                        if (CurrentMatch != null && reader.U8() == CurrentMatch.MatchIndex)
                        {
                            RemoteWantsRematch = true;
                            TryStartRematch();
                            LocalVersusMenu.Ensure().OnOnlineChanged();
                        }
                        break;
                    case NetMessageType.ReturnToLobby:
                        if (Phase == OnlinePhase.Result) ShowLobbyFromResult();
                        else if (Phase == OnlinePhase.InMatch) _remoteLeftToLobby = true;
                        break;
                    case NetMessageType.MatchResult:
                        if (CurrentMatch != null && reader.U8() == CurrentMatch.MatchIndex)
                        {
                            int winner = reader.U8();
                            _remoteResult = (winner == 255 ? -1 : winner, reader.U8(), reader.U8(), reader.I32());
                            CompareResults();
                            // The opponent's match ended on a tick we have already simulated past.
                            if (_localResult == null && Phase == OnlinePhase.InMatch && VersusTickDriver.Tick > _remoteResult.Value.tick + 1)
                                OnDesync(_remoteResult.Value.tick);
                        }
                        break;
                    case NetMessageType.Desync:
                        if (CurrentMatch != null && reader.U8() == CurrentMatch.MatchIndex) OnDesync(reader.I32());
                        break;
                    case NetMessageType.DesyncReport:
                        if (CurrentMatch != null && reader.U8() == CurrentMatch.MatchIndex)
                        {
                            int reportTick = reader.I32();
                            string platform = reader.Str();
                            string state = System.Text.Encoding.UTF8.GetString(reader.Bytes(reader.U16()));
                            Debug.LogError("[Online] Remote state at tick " + reportTick + " (" + platform + "): " + state);
                        }
                        break;
                    case NetMessageType.Forfeit:
                        if (CurrentMatch != null && reader.U8() == CurrentMatch.MatchIndex && MatchRunning)
                        {
                            Notice = RemoteName + " forfeited the match.";
                            Phase = OnlinePhase.Result;
                            ReportRoomFight(OutcomeFor(IsHost ? 0 : 1), "opponent forfeit");
                            EndMatchEarly(IsHost ? 0 : 1, Notice);
                        }
                        break;
                }
            }
            catch (NetFormatException exception)
            {
                Debug.LogWarning("[Online] Ignored a malformed message: " + exception.Message);
            }
        }

        private void OnClosed(string reason)
        {
            if (Phase == OnlinePhase.Closed) return;
            var previous = Phase;
            Phase = OnlinePhase.Closed;
            Notice = string.IsNullOrEmpty(reason) ? "The session ended." : reason;
            Debug.Log("[Online] Session closed: " + Notice);
            // An opponent who already reached the result and then left finished the match normally:
            // take their result. One who vanishes mid-fight forfeits. A fight that never started has no result.
            if (previous == OnlinePhase.InMatch && _remoteResult.HasValue)
            {
                int winner = _remoteResult.Value.winner;
                ReportRoomFight(OutcomeFor(winner), "opponent finished first");
                EndMatchEarly(winner, RemoteName + " finished the match and left.");
                return;
            }
            if (previous == OnlinePhase.InMatch) ReportRoomFight(OutcomeFor(IsHost ? 0 : 1), "opponent disconnected");
            else if (previous != OnlinePhase.Result) ReportRoomFight(MatchOutcome.Aborted, Notice);
            _source?.SaveDiagnostic("disconnect");
            if (previous == OnlinePhase.InMatch || previous == OnlinePhase.Starting)
                EndMatchEarly(-1, "Connection lost. " + Notice);
            else
                LocalVersusMenu.Ensure().OnOnlineChanged();
        }
    }

    /// <summary>
    /// The local player's device plus the remote player's inputs. Runs the fixed step
    /// itself through a <see cref="RollbackRunner"/>: in rollback mode it predicts the
    /// opponent and re-simulates when a guess was wrong; in delay mode (no prediction
    /// window) the same runner simply waits for both inputs.
    /// </summary>
    public sealed class OnlineInputSource : RecordingInputSource, IVersusStepRunner
    {
        private readonly OnlineVersusSession _session;
        private readonly InputTimeline _timeline;
        private readonly VersusInputSampler _sampler = new VersusInputSampler(GamePad.Player.One, true, true, true);
        private readonly int _localSide;
        private long _stallStartedMs = -1;
        private long _lastStallFlushMs = long.MinValue / 2;
        private readonly VersusSnapshot[] _history = new VersusSnapshot[256];
        private Eclipse.Multiplayer.Rollback.FightRollback _rollback;
        private RollbackRunner _runner;
        private int _recorded;
        private int _published;
        private bool _started;
        /// <summary>
        /// Fixed steps allowed to simulate per rendered frame. After a slow frame Unity runs
        /// many fixed steps to catch up, and each one saves and re-simulates rollback state,
        /// which makes the next frame slower still. Skipped steps are harmless: this peer
        /// falls behind and the timeline's pacing (catch-up ticks here, waits on the
        /// opponent) evens it out.
        /// </summary>
        private const int MaxStepsPerFrame = 2;
        private int _stepFrame = -1, _stepsThisFrame, _skippedSteps;

        /// <summary>This peer's state after <paramref name="tick"/>, if still in the recent history.</summary>
        public bool TryGetSnapshot(int tick, out VersusSnapshot snapshot)
        {
            snapshot = _history[tick & (_history.Length - 1)];
            return tick >= 0 && snapshot.Tick == tick && (tick != 0 || snapshot.Left.Present);
        }

        public OnlineInputSource(OnlineVersusSession session, LocalVersusSettings settings, InputTimeline timeline, int localSide) : base(settings)
        {
            _session = session;
            _timeline = timeline;
            _localSide = localSide;
        }

        public InputTimeline Timeline => _timeline;
        public RollbackRunner Runner => _runner;
        internal Eclipse.Multiplayer.Rollback.FightRollback FightState => _rollback;
        public int LocalSide => _localSide;
        /// <summary>Milliseconds the simulation has been waiting for the opponent, or 0.</summary>
        public long StalledMs => _stallStartedMs < 0 ? 0 : OnlineVersusSession.NowMs - _stallStartedMs;

        public override void Pump()
        {
            var peer = _session.Peer;
            if (peer == null) return;
            peer.Update(OnlineVersusSession.NowMs);
        }

        public override bool TryGetTick(int tick, out byte left, out byte right)
        {
            // Unused: this source runs the step itself (RunStep).
            left = right = 0;
            return false;
        }

        public void RunStep(Fight fight)
        {
            if (_runner == null)
            {
                _rollback = new Eclipse.Multiplayer.Rollback.FightRollback(fight, _timeline.MaxPrediction);
                _rollback.TickSimulated = OnTick;
                _runner = new RollbackRunner(_timeline, _rollback, _localSide);
            }
            if (Time.frameCount != _stepFrame) { _stepFrame = Time.frameCount; _stepsThisFrame = 0; }
            if (++_stepsThisFrame > MaxStepsPerFrame)
            {
                _skippedSteps++;
                VersusTickDriver.MarkStalled(false);
                return;
            }
            bool advanced = false, waited = false;
            _runner.Resolve();
            if (!VersusTickDriver.Owns(fight)) return;
            if (_runner.ShouldWait()) waited = true;
            else
            {
                int steps = _runner.StepsWanted;
                for (int i = 0; i < steps && _runner.Failure == null; i++)
                {
                    SampleLocal(_runner.Tick);
                    bool ran = _runner.Advance();
                    if (!VersusTickDriver.Owns(fight)) return;
                    if (!ran) break;
                    advanced = true;
                }
            }
            if (_runner.Failure != null)
            {
                Debug.LogError("[Rollback] " + _runner.Failure);
                _session.OnDesync(_timeline.FinalTicks);
                return;
            }
            long now = OnlineVersusSession.NowMs;
            bool stalled = !advanced && !waited;
            VersusTickDriver.MarkStalled(stalled);
            if (stalled)
            {
                if (_stallStartedMs < 0) _stallStartedMs = now;
                // While stalled, resend at most at the tick rate (not every fixed step).
                if (now - _lastStallFlushMs >= 16) { _lastStallFlushMs = now; _session.Peer?.Flush(now); }
            }
            else
            {
                _stallStartedMs = -1;
                _session.Peer?.Flush(now);
            }
            RecordFinal(_timeline.FinalTicks);
            Eclipse.Multiplayer.Rollback.RollbackObjects.Finalized(_timeline.FinalTicks);
            if (_timeline.DesyncTick >= 0) _session.OnDesync(_timeline.DesyncTick);
        }

        private void SampleLocal(int tick)
        {
            if (!_timeline.NeedsLocalInput(tick)) return;
            bool enabled = Application.isFocused && !LocalVersusMenu.BlocksFightInput;
            _timeline.AddLocal(_sampler.SampleWithTouch(enabled));
        }

        private void OnTick(int tick)
        {
            _history[tick & (_history.Length - 1)] = VersusTickDriver.LastSnapshot;
            if (!_started && tick == 0) { _started = true; _session.OnFightStarted(); }
        }

        /// <summary>Replays keep only confirmed ticks, in order.</summary>
        private void RecordFinal(int finalTicks)
        {
            for (; _recorded < finalTicks; _recorded++)
            {
                byte local = _timeline.GetLocal(_recorded), remote = _timeline.GetRemote(_recorded);
                uint? hash = _timeline.TryGetLocalHash(_recorded, out var value) ? value : (uint?)null;
                base.OnTickSimulated(_recorded, _localSide == 0 ? local : remote, _localSide == 0 ? remote : local, hash);
            }
            PublishConfirmed(false);
        }

        private void PublishConfirmed(bool flush)
        {
            if (!_session.IsHost || _session.RoomMatch == null || RoomSession.Current == null) return;
            if (!flush && Replay.TickCount - _published < 20) return;
            while (RoomSession.Current.Client.PublishFrames(_session.RoomMatch.MatchId, Replay, ref _published)) { }
        }

        public override void OnTickSimulated(int tick, byte left, byte right, uint? hash) { }

        // Final ticks are recorded a few ticks after they ran, so the round comes from history.
        protected override bool TryGetRound(int tick, out int round)
        {
            round = 0;
            if (!TryGetSnapshot(tick, out var snapshot)) return false;
            round = snapshot.Round;
            return true;
        }

        public override void OnMatchEnded(int finalTick, int winner, int leftRounds, int rightRounds)
        {
            // The final tick ran on confirmed input, and so did every tick before it; its
            // result is not marked final until the runner finishes it, so record it here.
            RecordFinal(Math.Min(finalTick, _timeline.FinalTicks));
            if (_recorded == finalTick && finalTick < _timeline.LocalFrames && finalTick < _timeline.RemoteFrames)
            {
                byte local = _timeline.GetLocal(finalTick), remote = _timeline.GetRemote(finalTick);
                base.OnTickSimulated(finalTick, _localSide == 0 ? local : remote, _localSide == 0 ? remote : local,
                    VersusStateHash.Hash(VersusTickDriver.LastSnapshot));
                _recorded++;
            }
            base.OnMatchEnded(finalTick, winner, leftRounds, rightRounds);
            PublishConfirmed(true);
            _session.OnLocalResult(winner, leftRounds, rightRounds, finalTick);
        }

        public override void Stop()
        {
            if (_runner != null && _rollback != null)
                Debug.Log("[Rollback] Match stats: " + _runner.Tick + " ticks, " + _rollback.ObjectCount + " objects; " +
                    _rollback.Saves + " saves (avg " + _rollback.AverageSaveMs.ToString("0.00") + " ms, max " + _rollback.MaxSaveMs.ToString("0.00") + " ms), " +
                    _rollback.Loads + " restores (avg " + _rollback.AverageLoadMs.ToString("0.00") + " ms); " +
                    _runner.Rollbacks + " rollbacks re-simulating " + _runner.TotalResimulated + " ticks (max " + _runner.MaxResimulated + "); " +
                    _runner.Waits + " waits, " + _runner.Barriers + " barriers, " + _skippedSteps + " fixed steps skipped after slow frames. " +
                    "Grew since the first snapshot: " + _rollback.GrowthSinceFirstSave() + ".");
            RecordFinal(_timeline.FinalTicks);
            PublishConfirmed(true);
            Eclipse.Multiplayer.Rollback.RollbackObjects.Clear();
            base.Stop();
            // The room/result UI may retain this input source. Release the old fight
            // and all ten full snapshots immediately, rather than at the next match.
            if (_rollback != null) _rollback.TickSimulated = null;
            _rollback = null;
            _runner = null;
        }

        internal void SaveDiagnostic(string suffix) => Save(suffix);
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using Eclipse.Multiplayer.Online;
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
        public string LocalWeapon { get; private set; } = LocalVersusMatch.WeaponIds[0];
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
        /// Peers must run the same game version. Builds are not fingerprinted beyond that
        /// (editor and player, or Windows and Linux, may play together); the per-tick
        /// state hash catches any build difference that actually changes the simulation.
        /// </summary>
        public static string BuildId => Application.version;

        /// <summary>Enabled mods and versions; peers and replays must agree on gameplay content.</summary>
        public static string ContentFingerprint()
        {
            try
            {
                if (!Eclipse.Modding.ModRuntime.IsInitialized) return "mods:unloaded";
                var mods = Eclipse.Modding.ModRuntime.Host.EnabledMods.Select(mod => mod.Id + "@" + mod.Version).OrderBy(id => id, StringComparer.Ordinal).ToArray();
                return mods.Length == 0 ? "mods:none" : "mods:" + string.Join(",", mods);
            }
            catch (Exception exception)
            {
                return "mods:error:" + exception.GetType().Name;
            }
        }

        public static string SavedName { get => PlayerPrefs.GetString(NamePreference, "Player"); set => PlayerPrefs.SetString(NamePreference, value ?? "Player"); }
        public static string SavedAddress { get => PlayerPrefs.GetString(AddressPreference, ""); set => PlayerPrefs.SetString(AddressPreference, value ?? ""); }
        public static int SavedPort { get => PlayerPrefs.GetInt(PortPreference, NetProtocol.DefaultPort); set => PlayerPrefs.SetInt(PortPreference, value); }
        public static int SavedDelay { get => Mathf.Clamp(PlayerPrefs.GetInt(DelayPreference, NetProtocol.DefaultInputDelay), 0, NetProtocol.MaxInputDelay); set => PlayerPrefs.SetInt(DelayPreference, value); }

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
            session.Lobby = new LobbyState
            {
                HostWeapon = session.LocalWeapon,
                Arena = LocalVersusMatch.ArenaIds[0],
                InputDelay = SavedDelay,
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
            GUI.Label(line, "PING " + ping + "   DELAY " + _source.Timeline.Delay + "F", _hudStyle);
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
        }

        private void OnConnected()
        {
            Phase = OnlinePhase.Lobby;
            Notice = IsHost ? RemoteName + " joined." : "Connected to " + RemoteName + ".";
            Debug.Log("[Online] " + Notice);
            if (IsHost) SendLobby();
            else Peer.SendReliable(NetMessages.GuestLobby(LocalWeapon, LocalReady));
            LocalVersusMenu.Ensure().OnOnlineChanged();
        }

        // ---- Lobby ----

        public void CycleLocalWeapon()
        {
            int index = (Array.IndexOf(LocalVersusMatch.WeaponIds, LocalWeapon) + 1) % LocalVersusMatch.WeaponIds.Length;
            LocalWeapon = LocalVersusMatch.WeaponIds[index];
            if (IsHost) { Lobby.HostWeapon = LocalWeapon; SendLobby(); }
            else SendGuestLobby();
        }

        public void ToggleReady()
        {
            if (IsHost) return;
            LocalReady = !LocalReady;
            SendGuestLobby();
        }

        public void CycleArena()
        {
            if (!IsHost) return;
            int index = (Array.IndexOf(LocalVersusMatch.ArenaIds, Lobby.Arena) + 1) % LocalVersusMatch.ArenaIds.Length;
            Lobby.Arena = LocalVersusMatch.ArenaIds[index];
            SendLobby();
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
            SavedDelay = Lobby.InputDelay;
            SendLobby();
        }

        /// <summary>A delay that hides the measured one-way latency at 60 ticks per second.</summary>
        public int SuggestedDelay => Peer == null || Peer.RttMs < 0 ? NetProtocol.DefaultInputDelay
            : Mathf.Clamp(Mathf.CeilToInt(Peer.RttMs / 2f / (1000f / 60f)) + 1, 1, 8);

        public string StartBlocker()
        {
            if (!IsHost) return "Only the host can start.";
            if (Phase != OnlinePhase.Lobby && Phase != OnlinePhase.Result) return "Waiting for an opponent.";
            if (!Lobby.GuestReady) return RemoteName + " is not ready yet.";
            return null;
        }

        public void HostStart()
        {
            string blocker = StartBlocker();
            if (blocker != null) throw new InvalidOperationException(blocker);
            var start = new MatchStart
            {
                MatchIndex = _nextMatchIndex & 0xFF,
                HostWeapon = Lobby.HostWeapon,
                GuestWeapon = Lobby.GuestWeapon,
                Arena = Lobby.Arena,
                WinsRequired = Lobby.WinsRequired,
                RoundTimeSeconds = Lobby.RoundTimeSeconds,
                InputDelay = Lobby.InputDelay,
                Seed = new System.Random().Next(),
            };
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
            if (Peer.State == NetplayState.Connected) Peer.SendReliable(NetMessages.GuestLobby(LocalWeapon, LocalReady));
            LocalVersusMenu.Ensure().OnOnlineChanged();
        }

        // ---- Match ----

        private void BeginMatch(MatchStart start)
        {
            if (Array.IndexOf(LocalVersusMatch.WeaponIds, start.HostWeapon) < 0 || Array.IndexOf(LocalVersusMatch.WeaponIds, start.GuestWeapon) < 0 ||
                Array.IndexOf(LocalVersusMatch.ArenaIds, start.Arena) < 0 || start.WinsRequired < 1 || start.WinsRequired > 5 ||
                start.RoundTimeSeconds < 30 || start.RoundTimeSeconds > 300)
            {
                Peer.Close("The match settings were not recognised.");
                OnClosed("The host sent a matchup this build does not support.");
                return;
            }
            CurrentMatch = start;
            _source = null;
            _nextMatchIndex = start.MatchIndex + 1;
            LocalWantsRematch = RemoteWantsRematch = false;
            _localResult = _remoteResult = null;
            _remoteLeftToLobby = false;
            _pendingEnd = null;
            SyncProblem = null;
            Notice = string.Empty;
            Phase = OnlinePhase.Starting;
            var timeline = new LockstepTimeline(start.MatchIndex, start.InputDelay);
            Peer.Timeline = timeline;
            string hostName = IsHost ? LocalName : RemoteName;
            string guestName = IsHost ? RemoteName : LocalName;
            Debug.Log("[Online] Match " + start.MatchIndex + ": " + start.HostWeapon + " vs " + start.GuestWeapon + " at " + start.Arena +
                ", input delay " + start.InputDelay + ", seed " + start.Seed + ".");
            try
            {
                var settings = new LocalVersusSettings(start.HostWeapon, start.GuestWeapon, start.Arena, true,
                    start.WinsRequired, start.RoundTimeSeconds, VersusMode.Online, hostName, guestName, start.Seed);
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
            if (_remoteLeftToLobby) ShowLobbyFromResult();
        }

        public void Forfeit()
        {
            if (CurrentMatch == null || !MatchRunning) return;
            Peer.SendReliable(NetMessages.Simple(NetMessageType.Forfeit, CurrentMatch.MatchIndex));
            Notice = "You forfeited the match.";
            Phase = OnlinePhase.Result;
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
            Debug.LogError("[Online] Desync at tick " + tick + ". " + VersusStateHash.Describe(Fight.GetCurrentFight()));
            if (CurrentMatch != null) Peer.SendReliable(NetMessages.Desync(CurrentMatch.MatchIndex, tick));
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
                        string weapon = reader.Str();
                        bool ready = reader.Bool();
                        if (Array.IndexOf(LocalVersusMatch.WeaponIds, weapon) >= 0) Lobby.GuestWeapon = weapon;
                        Lobby.GuestReady = ready && Array.IndexOf(LocalVersusMatch.WeaponIds, Lobby.GuestWeapon) >= 0;
                        SendLobby();
                        break;
                    case NetMessageType.HostLobby when !IsHost:
                        Lobby = LobbyState.Decode(reader);
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
                    case NetMessageType.Forfeit:
                        if (CurrentMatch != null && reader.U8() == CurrentMatch.MatchIndex && MatchRunning)
                        {
                            Notice = RemoteName + " forfeited the match.";
                            Phase = OnlinePhase.Result;
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
            _source?.SaveDiagnostic("disconnect");
            if (previous == OnlinePhase.InMatch || previous == OnlinePhase.Starting)
                EndMatchEarly(-1, "Connection lost. " + Notice);
            else
                LocalVersusMenu.Ensure().OnOnlineChanged();
        }
    }

    /// <summary>The local player's device plus the remote player's inputs, in lockstep.</summary>
    public sealed class OnlineInputSource : RecordingInputSource
    {
        private readonly OnlineVersusSession _session;
        private readonly LockstepTimeline _timeline;
        private readonly VersusInputSampler _sampler = new VersusInputSampler(GamePad.Player.One, true, true, true);
        private readonly int _localSide;
        private long _stallStartedMs = -1;

        public OnlineInputSource(OnlineVersusSession session, LocalVersusSettings settings, LockstepTimeline timeline, int localSide) : base(settings)
        {
            _session = session;
            _timeline = timeline;
            _localSide = localSide;
        }

        public LockstepTimeline Timeline => _timeline;
        public int LocalSide => _localSide;
        /// <summary>Milliseconds the simulation has been waiting for the opponent, or 0.</summary>
        public long StalledMs => _stallStartedMs < 0 ? 0 : OnlineVersusSession.NowMs - _stallStartedMs;

        public override void Pump()
        {
            var peer = _session.Peer;
            if (peer == null) return;
            peer.Update(OnlineVersusSession.NowMs);
        }

        /// <summary>Run two ticks when the opponent is more than two ticks ahead of us.</summary>
        public override int StepsWanted => _timeline.RemoteFrames - _timeline.Delay - 1 > VersusTickDriver.Tick + 2 ? 2 : 1;

        public override bool TryGetTick(int tick, out byte left, out byte right)
        {
            if (_timeline.NeedsLocalInput(tick))
            {
                bool enabled = Application.isFocused && !LocalVersusMenu.BlocksFightInput;
                _timeline.AddLocal(_sampler.SampleWithTouch(enabled));
            }
            if (!_timeline.TryGetInputs(tick, out var local, out var remote))
            {
                if (_stallStartedMs < 0) _stallStartedMs = OnlineVersusSession.NowMs;
                _session.Peer?.Flush(OnlineVersusSession.NowMs);
                left = right = 0;
                return false;
            }
            _stallStartedMs = -1;
            left = _localSide == 0 ? local : remote;
            right = _localSide == 0 ? remote : local;
            return true;
        }

        public override void OnTickSimulated(int tick, byte left, byte right, uint? hash)
        {
            base.OnTickSimulated(tick, left, right, hash);
            if (tick == 0) _session.OnFightStarted();
            if (hash.HasValue)
            {
                _timeline.RecordLocalHash(tick, hash.Value);
            }
            _session.Peer?.Flush(OnlineVersusSession.NowMs);
            if (_timeline.DesyncTick >= 0) _session.OnDesync(_timeline.DesyncTick);
        }

        public override void OnMatchEnded(int finalTick, int winner, int leftRounds, int rightRounds)
        {
            base.OnMatchEnded(finalTick, winner, leftRounds, rightRounds);
            _session.OnLocalResult(winner, leftRounds, rightRounds, finalTick);
        }

        internal void SaveDiagnostic(string suffix) => Save(suffix);
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using Eclipse.Multiplayer.Online;
using Eclipse.Multiplayer.Online.Rooms;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Eclipse.Multiplayer
{
    /// <summary>
    /// Online rooms: the connection to the room server, the room the player is in, and
    /// the hand-off from a pairing to a peer-to-peer fight and back.
    /// </summary>
    public sealed class RoomSession : MonoBehaviour
    {
        public static string DefaultServer => Eclipse.Runtime.EditorPlayModeContext.IsRoomScenario ?
            "127.0.0.1:7300" : "rooms.projecteclipse.fyi:7300";
        public const int AutoContinueSeconds = 20;

        public static RoomSession Current { get; private set; }
        public static bool IsActive => Current != null;

        public RoomClient Client { get; private set; }
        public string LocalName { get; private set; }
        public VersusLoadout LocalLoadout { get; private set; }
        /// <summary>Whether this player wants to be in the queue (restored after each fight).</summary>
        public bool WantsQueue { get; private set; }
        public string Notice { get; private set; } = string.Empty;
        /// <summary>Waiting for the path to the paired opponent, before the fight session exists.</summary>
        public RoomPairing PendingPairing { get; private set; }
        public bool InFight { get; private set; }
        /// <summary>When the post-fight result screen continues on its own, or -1.</summary>
        public long AutoContinueAtMs { get; private set; } = -1;
        public MatchOutcome? LastOutcome { get; private set; }

        private Action _whenConnected;
        private long _pairedAtMs, _lastQueueResendMs;

        /// <summary>Set when the room server went away during a fight that is still running.</summary>
        public string ServerLost { get; private set; }

        public RoomState Room => Client?.Room;
        public bool IsHost => Room != null && Room.HostId == Client.ClientId;
        public uint ClientId => Client?.ClientId ?? 0;

        /// <summary>Connects (or reuses the connection) and then runs <paramref name="then"/>.</summary>
        public static void Connect(string playerName, Action then)
        {
            if (!NetplayPeer.TryParseAddress(DefaultServer, RoomProtocol.DefaultPort, out var endPoint, out var error))
                throw new ArgumentException(error.Replace("host's", "room server's"));
            string name = new NetIdentity("", "", playerName).PlayerName;
            OnlineVersusSession.SavedName = name;
            var session = Current;
            if (session != null && session.Client != null && session.Client.State != RoomClientState.Closed &&
                session.Client.Server.Equals(endPoint) && session.LocalName == name)
            {
                session.RunWhenConnected(then);
                return;
            }
            Shutdown();
            session = new GameObject("Eclipse Online Rooms").AddComponent<RoomSession>();
            DontDestroyOnLoad(session.gameObject);
            session.LocalName = name;
            session.LocalLoadout = VersusLoadouts.Load(VersusLoadouts.Online);
            var identity = new NetIdentity(OnlineVersusSession.BuildId, OnlineVersusSession.ContentFingerprint(), name);
            try { session.Client = new RoomClient(endPoint, identity, NetAddresses.LocalIPv4(), OnlineVersusSession.NowMs); }
            catch (Exception exception)
            {
                Destroy(session.gameObject);
                throw new InvalidOperationException("Could not open a network socket: " + exception.Message, exception);
            }
            Current = session;
            session.RunWhenConnected(then);
            Debug.Log("[Rooms] Connecting to " + endPoint + " as " + name + ".");
        }

        public static void Shutdown(string reason = "Left online play.")
        {
            var session = Current;
            if (session == null) return;
            Current = null;
            if (OnlineVersusSession.Current != null && OnlineVersusSession.Current.RoomMatch != null) OnlineVersusSession.Shutdown(reason);
            session.Client?.Dispose();
            session.Client = null;
            Destroy(session.gameObject);
        }

        private void RunWhenConnected(Action then)
        {
            if (then == null) return;
            if (Client.State == RoomClientState.Connected) then();
            else _whenConnected += then;
        }

        private void OnEnable() => OnlineVersusSession.RoomFightOver += OnFightOver;
        private void OnDisable() => OnlineVersusSession.RoomFightOver -= OnFightOver;

        private void OnDestroy()
        {
            Client?.Dispose();
            if (Current == this) Current = null;
        }

        private void OnApplicationQuit() => Client?.Close("Closed the game.");

        // ---- Actions (from the menu) ----

        public void SetLoadout(VersusLoadout loadout)
        {
            LocalLoadout = (loadout ?? VersusLoadout.Default).Sanitized();
            VersusLoadouts.Save(VersusLoadouts.Online, LocalLoadout);
            if (Room != null) Client.SetMember(LocalLoadout.ToCode(), WantsQueue && !InFight);
        }

        /// <summary>Sends a chat line to the room.</summary>
        public void Say(string text)
        {
            if (Room == null) return;
            Client.SendChat(text);
        }

        /// <summary>A member's loadout as the room reports it, or null before they chose one this roster knows.</summary>
        public static VersusLoadout LoadoutOf(RoomMember member) =>
            member != null && VersusLoadouts.TryFromCode(member.Loadout, out var loadout) ? loadout : null;

        public void ToggleQueue()
        {
            if (Room == null || InFight) return;
            WantsQueue = !WantsQueue;
            Client.SetMember(LocalLoadout.ToCode(), WantsQueue);
        }

        public void Spectate(uint matchId)
        {
            if (Room == null || InFight || PendingPairing != null || LocalVersusSession.IsStarting) return;
            WantsQueue = false;
            Notice = "Connecting to the spectator stream...";
            Client.Spectate(matchId);
        }

        public void StopSpectating()
        {
            if (LocalVersusSession.IsStarting) return;
            Client?.Spectate(0);
            Notice = string.Empty;
            LocalVersusSession.ShowMultiplayerHome();
        }

        public void Leave()
        {
            if (LocalVersusSession.IsSpectating) StopSpectating();
            if (InFight) OnlineVersusSession.Shutdown("Left the room.");
            InFight = false;
            PendingPairing = null;
            AutoContinueAtMs = -1;
            WantsQueue = false;
            RematchRequested = false;
            Client?.LeaveRoom();
        }

        /// <summary>This player asked (from the result screen) to fight the same opponent again.</summary>
        public bool RematchRequested { get; private set; }
        /// <summary>Why the server turned the last rematch request down, or null.</summary>
        public string RematchRefusal { get; private set; }

        /// <summary>Whether the opponent of the fight just finished has asked for a rematch.</summary>
        public bool OpponentWantsRematch
        {
            get
            {
                var pairing = OnlineVersusSession.Current?.RoomMatch;
                return pairing != null && Room?.Find(pairing.PeerId)?.WantsRematch == true;
            }
        }

        /// <summary>
        /// From the result screen: ask the room server to pair the same two players again.
        /// The server starts it once both ask and nobody else is waiting in line.
        /// </summary>
        public void RequestRematch()
        {
            uint matchId = OnlineVersusSession.Current?.RoomMatch?.MatchId ?? 0;
            if (!InFight || matchId == 0 || RematchRequested || ServerLost != null || Client.State != RoomClientState.Connected) return;
            RematchRequested = true;
            RematchRefusal = null;
            Client.Rematch(matchId, true);
            // Waiting on the opponent's answer; the screen stays until they accept or it times out.
            AutoContinueAtMs = OnlineVersusSession.NowMs + AutoContinueSeconds * 1000;
        }

        /// <summary>From the result screen: close the fight and return to the room, out of the queue.</summary>
        public void ContinueAfterFight()
        {
            if (!InFight) return;
            // Back to watching; the player queues again themselves when they want another fight.
            WantsQueue = false;
            RematchRequested = false;
            RematchRefusal = null;
            if (ServerLost != null)
            {
                string reason = ServerLost;
                Shutdown(reason);
                LocalVersusMenu.Ensure().OnRoomClosed(reason);
                return;
            }
            InFight = false;
            AutoContinueAtMs = -1;
            uint matchId = OnlineVersusSession.Current?.RoomMatch?.MatchId ?? 0;
            OnlineVersusSession.Shutdown("Returned to the room.");
            if (matchId != 0) Client.ReleaseLink(matchId);
            if (Room != null) Client.SetMember(LocalLoadout.ToCode(), WantsQueue);
            string notice = Notice;
            LocalVersusMenu.Ensure().ShowRoom();
            if (notice.StartsWith("Could not connect")) Notice = notice;
            else Notice = string.Empty;
        }

        // ---- Pump ----

        private void Update()
        {
            if (Client == null) return;
            var before = Client.State;
            Client.Update(OnlineVersusSession.NowMs);
            if (before != RoomClientState.Connected && Client.State == RoomClientState.Connected)
            {
                Debug.Log("[Rooms] Connected as client " + Client.ClientId + " (seen as " + Client.PublicEndPoint + ").");
                var then = _whenConnected;
                _whenConnected = null;
                then?.Invoke();
            }
            while (Client != null && Client.TryGetEvent(out var roomEvent)) Handle(roomEvent);
            if (Client == null) return;
            if (Client.State == RoomClientState.Closed)
            {
                string reason = Client.CloseReason;
                // A fight already under way keeps going on its own link; leave after it.
                if (InFight && ServerLost == null && OnlineVersusSession.Current != null)
                {
                    ServerLost = reason;
                    Debug.Log("[Rooms] " + reason + " The current fight continues.");
                    return;
                }
                if (InFight && ServerLost != null) return;
                Debug.Log("[Rooms] " + reason);
                Shutdown(reason);
                LocalVersusMenu.Ensure().OnRoomClosed(reason);
                return;
            }
            StartFightWhenLinked();
            ExpireStuckPairing();
            ResendQueueIntent();
            if (InFight && AutoContinueAtMs >= 0 && OnlineVersusSession.NowMs >= AutoContinueAtMs &&
                (LocalVersusSession.HasResult || !FightLoaded))
                ContinueAfterFight();
        }

        private void Handle(RoomEvent roomEvent)
        {
            var menu = LocalVersusMenu.Ensure();
            switch (roomEvent.Type)
            {
                case RoomEventType.Error:
                    Notice = roomEvent.Text;
                    // A refused rematch (someone queued, the opponent left) says why; it can be asked for again.
                    if (RematchRequested) RematchRefusal = roomEvent.Text;
                    RematchRequested = false;
                    menu.OnRoomNotice(roomEvent.Text);
                    break;
                case RoomEventType.RoomListUpdated:
                    menu.OnRoomListChanged();
                    break;
                case RoomEventType.RoomChanged:
                    menu.OnRoomChanged();
                    break;
                case RoomEventType.Chat:
                    menu.OnRoomChat(roomEvent.Chat);
                    break;
                case RoomEventType.LeftRoom:
                    Notice = roomEvent.Text;
                    InFight = false;
                    PendingPairing = null;
                    WantsQueue = false;
                    if (OnlineVersusSession.Current != null && OnlineVersusSession.Current.RoomMatch != null) OnlineVersusSession.Shutdown("Left the room.");
                    menu.OnLeftRoom(roomEvent.Text);
                    break;
                case RoomEventType.Paired:
                    PendingPairing = roomEvent.Pairing;
                    _pairedAtMs = OnlineVersusSession.NowMs;
                    LastOutcome = null;
                    // A rematch pairs players still on the result screen; it must not time out to the room.
                    AutoContinueAtMs = -1;
                    RematchRequested = false;
                    RematchRefusal = null;
                    Notice = "Next up: you vs " + roomEvent.Pairing.PeerName + ". Connecting...";
                    Debug.Log("[Rooms] Paired with " + roomEvent.Pairing.PeerName + " for match " + roomEvent.Pairing.MatchId + " as " +
                        (roomEvent.Pairing.Side == 0 ? "left (host)" : "right (guest)") + "; candidates " + string.Join(", ", roomEvent.Pairing.Candidates));
                    menu.OnRoomChanged();
                    break;
                case RoomEventType.Spectating:
                    var stream = Client.Spectating;
                    if (stream == null) break;
                    try
                    {
                        OnlineVersusSession.Shutdown("Started spectating.");
                        var start = stream.Start;
                        if (!VersusLoadouts.TryFromCode(start.HostLoadout, out var left) || !VersusLoadouts.TryFromCode(start.GuestLoadout, out var right))
                            throw new InvalidOperationException("The fighter loadouts are not recognised.");
                        if (!PvpBalanceProfiles.TryFind(start.BalanceHash, out var balance))
                            throw new InvalidOperationException("Install the match's balance profile before spectating.");
                        stream.Replay.BalanceHash = balance.Hash;
                        stream.Replay.BalanceJson = balance.ToJson();
                        var settings = new LocalVersusSettings(left, right,
                            VersusRoster.ResolveArena(start.Arena, start.Seed), true, start.WinsRequired, start.RoundTimeSeconds,
                            VersusMode.Spectator, stream.Replay.LeftName, stream.Replay.RightName, start.Seed, balance: balance);
                        LocalVersusSession.StartMatch(settings, () => new SpectatorInputSource(stream));
                    }
                    catch (Exception exception)
                    {
                        Client.Spectate(0);
                        Notice = "Could not spectate: " + exception.Message;
                        menu.ShowRoom();
                    }
                    break;
            }
        }

        private void StartFightWhenLinked()
        {
            var pairing = PendingPairing;
            var link = Client.Link;
            if (pairing == null || link == null || link.MatchId != pairing.MatchId || !link.IsReady) return;
            if (!LocalVersusSession.IsActive || !LocalVersusSession.IsReady) return;
            PendingPairing = null;
            Debug.Log("[Rooms] Path to " + pairing.PeerName + ": " + link.Path + ".");
            var identity = new NetIdentity(OnlineVersusSession.BuildId, OnlineVersusSession.ContentFingerprint(), LocalName);
            var peer = pairing.Side == 0
                ? NetplayPeer.Host(link, identity, OnlineVersusSession.NowMs)
                : NetplayPeer.Join(link, MatchLink.PeerEndPoint, identity, OnlineVersusSession.NowMs);
            InFight = true;
            AutoContinueAtMs = -1;
            OnlineVersusSession.StartRoomFight(peer, pairing, pairing.Settings, LocalName, LocalLoadout);
            Notice = "Fighting " + pairing.PeerName + (link.Path == LinkPath.Relay ? " (relayed)." : ".");
            LocalVersusMenu.Ensure().OnRoomChanged();
        }

        /// <summary>A pairing that never turns into a fight (no path, or not in the versus screens) gives up.</summary>
        private void ExpireStuckPairing()
        {
            var pairing = PendingPairing;
            if (pairing == null || OnlineVersusSession.NowMs - _pairedAtMs < 20000) return;
            PendingPairing = null;
            Client.ReportMatch(pairing.MatchId, MatchOutcome.Aborted, "could not start");
            Client.ReleaseLink(pairing.MatchId);
            Notice = "Could not start the fight with " + pairing.PeerName + ".";
            if (Room != null) Client.SetMember(LocalLoadout.ToCode(), WantsQueue);
            LocalVersusMenu.Ensure().OnRoomChanged();
        }

        /// <summary>If the server still shows us away while we want to fight, ask again (a request can arrive too early).</summary>
        private void ResendQueueIntent()
        {
            if (!WantsQueue || InFight || PendingPairing != null || Room == null) return;
            var self = Room.Find(ClientId);
            if (self == null || self.Status != MemberStatus.Away) return;
            long now = OnlineVersusSession.NowMs;
            if (now - _lastQueueResendMs < 2000) return;
            _lastQueueResendMs = now;
            Client.SetMember(LocalLoadout.ToCode(), true);
        }

        private void OnFightOver(RoomPairing pairing, MatchOutcome outcome, string reason)
        {
            if (Client == null) return;
            if (Client.State == RoomClientState.Connected) Client.ReportMatch(pairing.MatchId, outcome, reason);
            LastOutcome = outcome;
            if (!FightLoaded)
            {
                // Never got as far as a fight: straight back to the room.
                Notice = "Could not connect to " + pairing.PeerName + ". " + (WantsQueue ? "Back in line." : "");
                AutoContinueAtMs = OnlineVersusSession.NowMs;
            }
            else AutoContinueAtMs = OnlineVersusSession.NowMs + AutoContinueSeconds * 1000;
        }

        /// <summary>The paired fight got as far as loading (a match start was agreed).</summary>
        private static bool FightLoaded => OnlineVersusSession.Current != null && OnlineVersusSession.Current.CurrentMatch != null;

        // ---- Presentation helpers ----

        /// <summary>The roster's status column: fighting, the champion's streak, queue place, or watching.</summary>
        public string MemberStatusLabel(RoomMember member)
        {
            var room = Room;
            switch (member.Status)
            {
                case MemberStatus.InMatch: return "Fighting";
                case MemberStatus.Away: return "Back soon";
                case MemberStatus.Spectating: return "Spectating";
            }
            if (room != null && room.ChampionId == member.Id)
                return room.Streak > 1 ? "Champion x" + room.Streak : "Champion";
            if (member.Status == MemberStatus.Queued)
            {
                int position = room != null ? room.Queue.IndexOf(member.Id) + 1 : 0;
                return position > 0 ? "Next #" + position : "In line";
            }
            return "Watching";
        }

        public string NowPlaying()
        {
            var room = Room;
            if (room == null) return "";
            if (room.Fights.Count > 1) return room.Fights.Count + " fights in progress";
            if (room.MatchId != 0)
            {
                string left = room.Find(room.LeftId)?.Name ?? "?", right = room.Find(room.RightId)?.Name ?? "?";
                return left + "  vs  " + right;
            }
            if (room.ChampionId != 0 && room.Find(room.ChampionId)?.Status == MemberStatus.Away)
                return "Waiting for the winner to continue";
            return room.Queue.Count >= 2 ? "Starting..." : room.Queue.Count == 1 ? "Waiting for a challenger" : "Nobody is in line to fight";
        }

        public static string WeaponLabel(VersusLoadout loadout) => loadout?.WeaponName ?? "-";
    }

    /// <summary>This machine's IPv4 addresses, so players behind the same router can connect directly.</summary>
    public static class NetAddresses
    {
        public static List<IPAddress> LocalIPv4()
        {
            var result = new List<IPAddress>();
            try
            {
                foreach (var network in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (network.OperationalStatus != System.Net.NetworkInformation.OperationalStatus.Up ||
                        network.NetworkInterfaceType == System.Net.NetworkInformation.NetworkInterfaceType.Loopback) continue;
                    foreach (var unicast in network.GetIPProperties().UnicastAddresses)
                        if (unicast.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !result.Contains(unicast.Address))
                            result.Add(unicast.Address);
                }
            }
            catch (Exception exception) { Debug.LogWarning("[Online] Could not list network addresses: " + exception.Message); }
            return result;
        }
    }
}

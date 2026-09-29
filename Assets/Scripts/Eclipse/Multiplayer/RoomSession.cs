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
        private const string ServerPreference = "Eclipse.Online.RoomServer";
        private const string WeaponPreference = "Eclipse.Online.RoomWeapon";
        public const int AutoContinueSeconds = 20;

        public static RoomSession Current { get; private set; }
        public static bool IsActive => Current != null;

        public RoomClient Client { get; private set; }
        public string LocalName { get; private set; }
        public string LocalWeapon { get; private set; }
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

        public static string SavedServer { get => PlayerPrefs.GetString(ServerPreference, ""); set => PlayerPrefs.SetString(ServerPreference, value ?? ""); }

        public RoomState Room => Client?.Room;
        public bool IsHost => Room != null && Room.HostId == Client.ClientId;
        public uint ClientId => Client?.ClientId ?? 0;

        /// <summary>Connects (or reuses the connection) and then runs <paramref name="then"/>.</summary>
        public static void Connect(string server, string playerName, Action then)
        {
            if (!NetplayPeer.TryParseAddress(server, RoomProtocol.DefaultPort, out var endPoint, out var error))
                throw new ArgumentException(error.Replace("host's", "room server's"));
            SavedServer = server.Trim();
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
            session.LocalWeapon = PlayerPrefs.GetString(WeaponPreference, LocalVersusMatch.WeaponIds[0]);
            if (Array.IndexOf(LocalVersusMatch.WeaponIds, session.LocalWeapon) < 0) session.LocalWeapon = LocalVersusMatch.WeaponIds[0];
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

        public void CycleWeapon()
        {
            int index = (Array.IndexOf(LocalVersusMatch.WeaponIds, LocalWeapon) + 1) % LocalVersusMatch.WeaponIds.Length;
            LocalWeapon = LocalVersusMatch.WeaponIds[index];
            PlayerPrefs.SetString(WeaponPreference, LocalWeapon);
            if (Room != null) Client.SetMember(LocalWeapon, WantsQueue && !InFight);
        }

        public void ToggleQueue()
        {
            if (Room == null || InFight) return;
            WantsQueue = !WantsQueue;
            Client.SetMember(LocalWeapon, WantsQueue);
        }

        public void Leave()
        {
            if (InFight) OnlineVersusSession.Shutdown("Left the room.");
            InFight = false;
            PendingPairing = null;
            AutoContinueAtMs = -1;
            WantsQueue = false;
            Client?.LeaveRoom();
        }

        /// <summary>From the result screen: close the fight and return to the room.</summary>
        public void ContinueAfterFight()
        {
            if (!InFight) return;
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
            if (Room != null) Client.SetMember(LocalWeapon, WantsQueue);
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
                    menu.OnRoomNotice(roomEvent.Text);
                    break;
                case RoomEventType.RoomListUpdated:
                    menu.OnRoomListChanged();
                    break;
                case RoomEventType.RoomChanged:
                    menu.OnRoomChanged();
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
                    Notice = "Next up: you vs " + roomEvent.Pairing.PeerName + ". Connecting...";
                    Debug.Log("[Rooms] Paired with " + roomEvent.Pairing.PeerName + " for match " + roomEvent.Pairing.MatchId + " as " +
                        (roomEvent.Pairing.Side == 0 ? "left (host)" : "right (guest)") + "; candidates " + string.Join(", ", roomEvent.Pairing.Candidates));
                    menu.OnRoomChanged();
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
            OnlineVersusSession.StartRoomFight(peer, pairing, Room?.Settings ?? new RoomSettings(), LocalName, LocalWeapon);
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
            if (Room != null) Client.SetMember(LocalWeapon, WantsQueue);
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
            Client.SetMember(LocalWeapon, true);
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
            if (room.MatchId != 0)
            {
                string left = room.Find(room.LeftId)?.Name ?? "?", right = room.Find(room.RightId)?.Name ?? "?";
                return left + "  vs  " + right;
            }
            if (room.ChampionId != 0 && room.Find(room.ChampionId)?.Status == MemberStatus.Away)
                return "Waiting for the winner to continue";
            return room.Queue.Count >= 2 ? "Starting..." : room.Queue.Count == 1 ? "Waiting for a challenger" : "Nobody is in line to fight";
        }

        public static string WeaponLabel(string id)
        {
            int index = Array.IndexOf(LocalVersusMatch.WeaponIds, id);
            return index >= 0 ? LocalVersusMatch.WeaponLabels[index] : "-";
        }
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

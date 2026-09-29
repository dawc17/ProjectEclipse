using System;
using System.Linq;
using Eclipse.Multiplayer.Online.Rooms;
using UnityEngine;

namespace Eclipse.Multiplayer
{
    // Online rooms: server entry, room browser, room creation/settings, the room, and room fight results.
    public sealed partial class LocalVersusMenu
    {
        private static readonly string[] RoomArenaIds = new[] { RoomSettings.RandomArena }.Concat(LocalVersusMatch.ArenaIds).ToArray();
        private static readonly string[] RoomArenaLabels = new[] { "Random" }.Concat(LocalVersusMatch.ArenaLabels).ToArray();
        private UnityEngine.UI.InputField serverField, codeField, passwordField, roomNameField;
        private RoomSettings draft = new RoomSettings();
        private bool editingRoom;
        private int builtMembers = -1;
        private bool builtAsHost;

        public void ShowOnlineHome()
        {
            if (OnlineVersusSession.IsActive && OnlineVersusSession.Current.RoomMatch == null) { ShowOnlineLobby(); return; }
            if (RoomSession.IsActive && RoomSession.Current.Room != null) { ShowRoom(); return; }
            EnsureEventSystem();
            page = Page.OnlineHome;
            Rebuild("ONLINE", "Find a room, make one, or join a friend's code", body =>
            {
                nameField = AddTextField(body, "YOUR NAME", OnlineVersusSession.SavedName, 24);
                serverField = AddTextField(body, "ROOM SERVER", RoomSession.SavedServer, 80, "address:" + RoomProtocol.DefaultPort);
                var rooms = AddRow(body);
                AddButton(rooms, "BROWSE ROOMS", () => WithRooms(session => { session.Client.RefreshRooms(); ShowRoomBrowser(); }), 0);
                AddButton(rooms, "CREATE ROOM", () => WithRooms(_ => ShowRoomCreate(false)), 0);
                codeField = AddTextField(body, "ROOM CODE", "", 8, "ABC123");
                AddButton(body, "JOIN BY CODE", () =>
                {
                    string code = codeField.text.Trim();
                    if (code.Length == 0) { SetStatus("Enter the room code your friend sees in their room."); return; }
                    WithRooms(session => session.Client.JoinByCode(code, ""));
                });
                var other = AddRow(body);
                AddButton(other, "DIRECT CONNECT", ShowOnlineSetup, 0);
                AddButton(other, "BACK", ShowLobby, 0);
            });
            SetStatus(RoomSession.SavedServer.Length == 0
                ? "Enter the room server address once; it is remembered. Players need the same game version and mods."
                : "No port forwarding needed: fights connect peer-to-peer, or through the server if they cannot.");
        }

        /// <summary>Connects to the room server named on the home page, then runs <paramref name="then"/>.</summary>
        private void WithRooms(Action<RoomSession> then)
        {
            string server = serverField != null ? serverField.text : RoomSession.SavedServer;
            string name = nameField != null ? nameField.text : OnlineVersusSession.SavedName;
            if (string.IsNullOrWhiteSpace(server)) { SetStatus("Enter the room server's address first."); return; }
            try
            {
                SetStatus("Connecting to the room server...");
                RoomSession.Connect(server, name, () => { if (RoomSession.Current != null) then(RoomSession.Current); });
            }
            catch (Exception exception) { Debug.LogWarning(exception.Message); SetStatus(exception.Message); }
        }

        public void ShowRoomBrowser()
        {
            var session = RoomSession.Current;
            if (session == null) { ShowOnlineHome(); return; }
            if (session.Room != null) { ShowRoom(); return; }
            EnsureEventSystem();
            page = Page.RoomBrowser;
            var rooms = session.Client.Rooms;
            Rebuild("ROOMS", rooms.Count == 0 ? "No open rooms yet" : rooms.Count + (rooms.Count == 1 ? " room" : " rooms"), body =>
            {
                foreach (var room in rooms.Take(6))
                {
                    var listing = room;
                    string label = listing.Name + "   " + listing.Players + "/" + listing.MaxPlayers + "   FT" + listing.WinsRequired +
                        (listing.Rotation == RoomRotation.Rotate ? "  ROTATE" : "") + (listing.Locked ? "  LOCKED" : "");
                    AddButton(body, label, () => session.Client.JoinRoom(listing.Id, passwordField != null ? passwordField.text : ""));
                }
                if (rooms.Count == 0) AddInfo(body, "BE THE FIRST", () => "Create a room");
                passwordField = AddTextField(body, "PASSWORD", "", 24, "only for locked rooms");
                var row = AddRow(body);
                AddButton(row, "REFRESH", () => session.Client.RefreshRooms(), 0);
                AddButton(row, "CREATE ROOM", () => ShowRoomCreate(false), 0);
                AddButton(row, "BACK", ShowOnlineHome, 0);
            });
            SetStatus(session.Notice);
        }

        public void ShowRoomCreate(bool editing)
        {
            var session = RoomSession.Current;
            if (session == null) { ShowOnlineHome(); return; }
            EnsureEventSystem();
            editingRoom = editing && session.Room != null;
            page = Page.RoomCreate;
            draft = editingRoom ? session.Room.Settings.Copy() : new RoomSettings { Name = session.LocalName + "'s room" };
            Rebuild(editingRoom ? "ROOM SETTINGS" : "CREATE ROOM", editingRoom ? "Changes apply to the next fight" : "Up to 8 players take turns", body =>
            {
                roomNameField = AddTextField(body, "ROOM NAME", draft.Name, 32);
                if (!editingRoom) passwordField = AddTextField(body, "PASSWORD", "", 24, "optional");
                AddChoice(body, "PLAYERS", () => draft.MaxPlayers.ToString(), () => draft.MaxPlayers = draft.MaxPlayers >= RoomProtocol.MaxMembers ? 2 : draft.MaxPlayers + 1);
                AddChoice(body, "FIRST TO", () => WinsLabel(draft.WinsRequired), () => draft.WinsRequired = draft.WinsRequired % 5 + 1);
                AddChoice(body, "ARENA", () => RoomArenaLabels[Math.Max(0, Array.IndexOf(RoomArenaIds, draft.Arena))],
                    () => draft.Arena = RoomArenaIds[(Math.Max(0, Array.IndexOf(RoomArenaIds, draft.Arena)) + 1) % RoomArenaIds.Length]);
                AddChoice(body, "ROTATION", () => draft.Rotation == RoomRotation.WinnerStays ? "WINNER STAYS" : "EVERYONE ROTATES",
                    () => draft.Rotation = draft.Rotation == RoomRotation.WinnerStays ? RoomRotation.Rotate : RoomRotation.WinnerStays);
                var row = AddRow(body);
                AddButton(row, editingRoom ? "SAVE" : "CREATE", () =>
                {
                    draft.Name = roomNameField.text.Trim();
                    string problem = draft.Validate();
                    if (problem != null) { SetStatus(problem); return; }
                    if (editingRoom) { session.Client.UpdateSettings(draft); ShowRoom(); }
                    else { SetStatus("Creating..."); session.Client.CreateRoom(draft, passwordField.text); }
                }, 0, Eclipse.UI.UiSound.Begin);
                AddButton(row, "BACK", () => { if (editingRoom) ShowRoom(); else ShowOnlineHome(); }, 0);
            });
        }

        public void ShowRoom()
        {
            var session = RoomSession.Current;
            if (session == null || session.Room == null) { ShowOnlineHome(); return; }
            EnsureEventSystem();
            page = Page.Room;
            var room = session.Room;
            builtMembers = room.Members.Count;
            builtAsHost = session.IsHost;
            string rotation = room.Settings.Rotation == RoomRotation.WinnerStays ? "winner stays" : "rotation";
            Rebuild(room.Settings.Name.ToUpperInvariant(), "Code " + room.Code + "   -   first to " + room.Settings.WinsRequired + "   -   " + rotation, body =>
            {
                AddInfo(body, "NOW", () => RoomSession.Current?.NowPlaying() ?? "");
                AddTextBlock(body, 26 * RoomProtocol.MaxMembers, () =>
                {
                    var current = RoomSession.Current;
                    if (current?.Room == null) return "";
                    return string.Join("\n", current.Room.Members.Select(current.MemberLine));
                });
                AddChoice(body, "YOUR WEAPON", () => RoomSession.WeaponLabel(RoomSession.Current?.LocalWeapon), () => RoomSession.Current?.CycleWeapon());
                AddChoice(body, "QUEUE", () => RoomSession.Current != null && RoomSession.Current.WantsQueue ? "IN LINE TO FIGHT" : "WATCHING",
                    () => RoomSession.Current?.ToggleQueue());
                var row = AddRow(body);
                if (session.IsHost) AddButton(row, "ROOM SETTINGS", () => ShowRoomCreate(true), 0);
                AddButton(row, "LEAVE ROOM", () => RoomSession.Current?.Leave(), 0);
            });
            liveLabels.Add((status, () => RoomSession.Current?.Notice ?? ""));
        }

        private void ShowRoomResult(string title, int playerOneWins, int playerTwoWins, string message)
        {
            var settings = LocalVersusSession.Settings;
            Rebuild(title, settings.PlayerOneName + "  " + playerOneWins + "  :  " + playerTwoWins + "  " + settings.PlayerTwoName, body =>
            {
                AddButton(body, "CONTINUE", () => RoomSession.Current?.ContinueAfterFight(), -1, Eclipse.UI.UiSound.Begin);
                AddButton(body, "LEAVE ROOM", () => { RoomSession.Current?.Leave(); ShowOnlineHome(); });
                AddButton(body, "LEAVE AND RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
            });
            string fixedMessage = message;
            liveLabels.Add((status, () =>
            {
                var rooms = RoomSession.Current;
                var online = OnlineVersusSession.Current;
                string text = online?.SyncProblem ?? fixedMessage ?? "";
                if (rooms != null && rooms.AutoContinueAtMs >= 0)
                {
                    long seconds = Math.Max(0, (rooms.AutoContinueAtMs - OnlineVersusSession.NowMs + 999) / 1000);
                    text = (text.Length > 0 ? text + " " : "") + "Back to the room in " + seconds + "s.";
                }
                return text;
            }));
        }

        // ---- Events from RoomSession ----

        public void OnRoomChanged()
        {
            var session = RoomSession.Current;
            if (session == null) return;
            if (session.Room != null && (page == Page.RoomBrowser || page == Page.OnlineHome || (page == Page.RoomCreate && !editingRoom)))
            {
                ShowRoom();
                return;
            }
            if (page == Page.Room && session.Room != null &&
                (session.Room.Members.Count != builtMembers || session.IsHost != builtAsHost))
                ShowRoom();
        }

        public void OnRoomListChanged()
        {
            if (page == Page.RoomBrowser) ShowRoomBrowser();
        }

        public void OnRoomNotice(string text)
        {
            if (IsShowing) SetStatus(text);
        }

        public void OnLeftRoom(string reason)
        {
            if (page == Page.Room || page == Page.RoomCreate || page == Page.Hidden || page == Page.Result)
            {
                RoomSession.Current?.Client.RefreshRooms();
                ShowRoomBrowser();
                SetStatus(reason);
            }
        }

        public void OnRoomClosed(string reason)
        {
            if (page == Page.Hidden && Fight.GetCurrentFight() != null && !LocalVersusSession.HasResult) return;
            ShowOnlineHome();
            SetStatus(reason);
        }

        /// <summary>A multi-line, self-refreshing text area.</summary>
        private void AddTextBlock(RectTransform parent, float height, Func<string> value)
        {
            var box = Rect(parent, "Text block");
            box.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredHeight = height;
            var label = Label(box, value(), 19, Ink, TextAnchor.UpperLeft);
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            liveLabels.Add((label, value));
        }
    }
}

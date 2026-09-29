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
                codeField = AddTextField(body, "ROOM CODE", "", 8, "ABC123", 170);
                codeField.characterValidation = UnityEngine.UI.InputField.CharacterValidation.Alphanumeric;
                AddButton((RectTransform)codeField.transform.parent, "JOIN", () =>
                {
                    string code = codeField.text.Trim();
                    if (code.Length == 0) { SetStatus("Enter the room code your friend sees in their room."); return; }
                    WithRooms(session => session.Client.JoinByCode(code, ""));
                }, 156);
                var other = AddRow(body);
                AddButton(other, "DIRECT CONNECT", ShowOnlineSetup, 0);
                AddButton(other, "BACK", ShowLobby, 0);
            });
            SetStatus(RoomSession.SavedServer.Length == 0
                ? "Enter the room server address once; it is remembered. Players need the same game version and mods."
                : "Fights connect peer-to-peer. No port forwarding needed.");
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
            Rebuild("ROOMS", rooms.Count == 0 ? "No open rooms" : rooms.Count == 1 ? "1 open room" : rooms.Count + " open rooms", body =>
            {
                passwordField = null;
                foreach (var room in rooms.Take(6))
                {
                    var listing = room;
                    string details = listing.Players + "/" + listing.MaxPlayers + "  FIRST TO " + listing.WinsRequired +
                        (listing.Rotation == RoomRotation.Rotate ? "  ROTATE" : "") + (listing.Locked ? "  LOCKED" : "");
                    AddListing(body, listing.Name, details, () => session.Client.JoinRoom(listing.Id, passwordField != null ? passwordField.text : ""));
                }
                if (rooms.Count == 0) AddNote(body, "Nobody is hosting right now.\nCreate a room and share its code with friends.");
                if (rooms.Take(6).Any(room => room.Locked)) passwordField = AddTextField(body, "PASSWORD", "", 24, "for locked rooms");
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
            string rotation = room.Settings.Rotation == RoomRotation.WinnerStays ? "Winner stays" : "Everyone rotates";
            Rebuild(room.Settings.Name.ToUpperInvariant(), "Room code " + room.Code + "     First to " + room.Settings.WinsRequired + "     " + rotation, body =>
            {
                var now = AddNote(body, "");
                now.color = Red;
                liveLabels.Add((now, () => RoomSession.Current?.NowPlaying() ?? ""));
                AddRoster(body, room.Members.Count);
                if (room.Members.Count == 1) AddNote(body, "Share the code " + room.Code + " so friends can join.");
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

        /// <summary>Centered body text sized to its lines.</summary>
        private UnityEngine.UI.Text AddNote(RectTransform parent, string text)
        {
            var box = Rect(parent, "Note");
            int lines = 1 + text.Count(c => c == '\n');
            box.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredHeight = 8 + 26 * lines;
            return Label(box, text, 20, Ink, TextAnchor.MiddleCenter);
        }

        /// <summary>A room listing: an ink plate with the name on the left and its settings on the right.</summary>
        private void AddListing(RectTransform parent, string name, string details, Action join)
        {
            var button = AddButton(parent, name, join);
            var title = button.GetComponentInChildren<UnityEngine.UI.Text>();
            title.alignment = TextAnchor.MiddleLeft; title.rectTransform.offsetMin = new Vector2(34, 0); title.rectTransform.offsetMax = new Vector2(-330, 0);
            title.horizontalOverflow = HorizontalWrapMode.Wrap; title.resizeTextForBestFit = true; title.resizeTextMinSize = 14; title.resizeTextMaxSize = 22;
            var info = Label(button.transform, details, 17, new Color(Paper.r, Paper.g, Paper.b, .75f), TextAnchor.MiddleRight);
            info.rectTransform.offsetMin = new Vector2(320, 0); info.rectTransform.offsetMax = new Vector2(-40, 0);
            info.resizeTextForBestFit = true; info.resizeTextMinSize = 12; info.resizeTextMaxSize = 17;
            button.GetComponent<Eclipse.UI.EclipseUiButton>()?.Rehome();
        }

        private static readonly float[] RosterColumns = { -1, 150, 90, 150 };

        /// <summary>Room members in aligned columns; the room page rebuilds when the member count changes.</summary>
        private void AddRoster(RectTransform parent, int count)
        {
            var table = Rect(parent, "Roster");
            var layout = table.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>(); layout.spacing = 0; layout.childControlHeight = layout.childControlWidth = true; layout.childForceExpandHeight = false; layout.childForceExpandWidth = true;
            var faded = new Color(Ink.r, Ink.g, Ink.b, .5f);
            RosterRow(table, 24, 15, new Func<string>[] { () => "PLAYER", () => "WEAPON", () => "RECORD", () => "STATUS" }, _ => faded);
            for (int i = 0; i < count; i++)
            {
                int index = i;
                RoomMember Member() { var room = RoomSession.Current?.Room; return room != null && index < room.Members.Count ? room.Members[index] : null; }
                RosterRow(table, 28, 20, new Func<string>[]
                {
                    () => { var m = Member(); return m == null ? "" : m.Name + (RoomSession.Current.Room.HostId == m.Id ? "  (host)" : ""); },
                    () => { var m = Member(); return m == null ? "" : RoomSession.WeaponLabel(m.Weapon); },
                    () => { var m = Member(); return m == null ? "" : m.Wins + " - " + m.Losses; },
                    () => { var m = Member(); return m == null ? "" : RoomSession.Current.MemberStatusLabel(m); },
                }, column =>
                {
                    var m = Member();
                    return column == 0 && m != null && m.Id == RoomSession.Current.ClientId ? Red : Ink;
                });
            }
        }

        private void RosterRow(RectTransform table, float height, int size, Func<string>[] cells, Func<int, Color> color)
        {
            var row = Rect(table, "Roster Row"); row.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredHeight = height;
            var horizontal = row.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>(); horizontal.spacing = 12; horizontal.childControlWidth = horizontal.childControlHeight = true; horizontal.childForceExpandWidth = false;
            horizontal.padding = new RectOffset(8, 8, 0, 0);
            for (int column = 0; column < cells.Length; column++)
            {
                var label = Label(row, cells[column](), size, color(column), column == 0 ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter);
                var element = label.gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
                if (RosterColumns[column] < 0) element.flexibleWidth = 1; else element.minWidth = element.preferredWidth = RosterColumns[column];
                var cell = cells[column]; int fixedColumn = column;
                liveLabels.Add((label, () => { label.color = color(fixedColumn); return cell(); }));
            }
        }
    }
}

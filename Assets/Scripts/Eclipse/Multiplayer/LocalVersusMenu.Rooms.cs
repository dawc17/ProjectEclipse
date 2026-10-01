using System;
using System.Linq;
using Eclipse.Multiplayer.Online.Rooms;
using UnityEngine;

namespace Eclipse.Multiplayer
{
    // Online rooms: room browser, room creation/settings, the room, and room fight results.
    public sealed partial class LocalVersusMenu
    {
        // private UnityEngine.UI.InputField serverField;
        private UnityEngine.UI.InputField codeField, passwordField, roomNameField;
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
                // serverField = AddTextField(body, "ROOM SERVER", RoomSession.DefaultServer, 80, "address:" + RoomProtocol.DefaultPort);
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
                AddButton(other, "BACK", ShowModeSelect, 0);
            });
            SetStatus("Fights connect peer-to-peer. No port forwarding needed.");
        }

        /// <summary>Connects to the default room server, then runs <paramref name="then"/>.</summary>
        private void WithRooms(Action<RoomSession> then)
        {
            string name = nameField != null ? nameField.text : OnlineVersusSession.SavedName;
            try
            {
                SetStatus("Connecting to the room server...");
                RoomSession.Connect(name, () => { if (RoomSession.Current != null) then(RoomSession.Current); });
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
            RebuildScreen("ROOMS", rooms.Count == 0 ? "No open rooms" : rooms.Count == 1 ? "1 open room" : rooms.Count + " open rooms",
                Hints("F5", "Refresh", "C", "Create", "Esc", "Back"), ShowOnlineHome, content =>
            {
                passwordField = null;
                var scroll = Place(content, "Rooms", new Vector2(.5f, 1), new Vector2(0, 0), new Vector2(1180, 420));
                var grid = BuildScrollGrid(scroll, new Vector2(570, 128), 2);
                grid.GetComponent<UnityEngine.UI.GridLayoutGroup>().spacing = new Vector2(16, 14);
                foreach (var room in rooms)
                {
                    var listing = room;
                    RoomCard(grid, listing, () => session.Client.JoinRoom(listing.Id, passwordField != null ? passwordField.text : ""));
                }
                if (rooms.Count == 0)
                {
                    var empty = Label(content, "Nobody is hosting right now.\nCreate a room and share its code with friends.", 24, Paper, TextAnchor.MiddleCenter);
                    Anchor(empty.rectTransform, new Vector2(.5f, .5f), new Vector2(0, 40), new Vector2(900, 90));
                    Shade(empty);
                }
                var bottom = Place(content, "Bottom", new Vector2(.5f, 0), new Vector2(0, 0), new Vector2(1180, 46));
                if (rooms.Any(room => room.Locked))
                {
                    var passwordRect = Place(bottom, "Password", new Vector2(0, .5f), new Vector2(0, 0), new Vector2(360, 40));
                    passwordRect.pivot = new Vector2(0, .5f);
                    passwordField = AddFilterField(passwordRect);
                    passwordField.placeholder.GetComponent<UnityEngine.UI.Text>().text = "Password for locked rooms";
                    passwordField.contentType = UnityEngine.UI.InputField.ContentType.Password;
                }
                var actions = Place(bottom, "Actions", new Vector2(1, .5f), new Vector2(0, 0), new Vector2(620, 46));
                actions.pivot = new Vector2(1, .5f);
                var row = actions.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>(); row.spacing = 12; row.childControlWidth = row.childControlHeight = true; row.childForceExpandWidth = true;
                AddButton(actions, "REFRESH", () => session.Client.RefreshRooms(), 0, Eclipse.UI.UiSound.Toggle);
                AddButton(actions, "CREATE ROOM", () => ShowRoomCreate(false), 0);
                AddButton(actions, "BACK", ShowOnlineHome, 0);
                shortcuts.Add((KeyCode.F5, () => session.Client.RefreshRooms()));
                shortcuts.Add((KeyCode.C, () => ShowRoomCreate(false)));
            });
            SetStatus(session.Notice);
        }

        /// <summary>A room as a card: its arena, name, host, players and rules.</summary>
        private void RoomCard(RectTransform parent, RoomListing listing, Action join)
        {
            var card = Rect(parent, listing.Name);
            var paper = card.gameObject.AddComponent<Eclipse.UI.PaperPanel>(); paper.color = Paper; paper.raycastTarget = true;
            var thumbRect = Place(card, "Arena", new Vector2(0, .5f), new Vector2(14, 0), new Vector2(172, 100));
            thumbRect.pivot = new Vector2(0, .5f);
            var thumb = thumbRect.gameObject.AddComponent<UnityEngine.UI.RawImage>(); thumb.raycastTarget = false;
            BindArenaPicture(thumb, listing.Arena);
            if (listing.Arena == VersusRoster.RandomArena) Label(thumbRect, "?", 40, Paper, TextAnchor.MiddleCenter);
            var name = Label(card, listing.Name, 24, Ink, TextAnchor.MiddleLeft);
            Anchor(name.rectTransform, new Vector2(0, 1), new Vector2(204, -14), new Vector2(340, 32), new Vector2(0, 1));
            name.horizontalOverflow = HorizontalWrapMode.Wrap; name.resizeTextForBestFit = true; name.resizeTextMinSize = 15; name.resizeTextMaxSize = 24;
            var host = Label(card, "Hosted by " + listing.HostName + "   ·   " + VersusRoster.ArenaName(listing.Arena), 15, new Color(Ink.r, Ink.g, Ink.b, .7f), TextAnchor.MiddleLeft);
            Anchor(host.rectTransform, new Vector2(0, 1), new Vector2(204, -48), new Vector2(340, 22), new Vector2(0, 1));
            string rules = "FIRST TO " + listing.WinsRequired + "   ·   " + RotationLabel(listing.Rotation).ToUpperInvariant();
            var rulesLabel = Label(card, rules, 15, Red, TextAnchor.MiddleLeft);
            Anchor(rulesLabel.rectTransform, new Vector2(0, 0), new Vector2(204, 18), new Vector2(260, 22), new Vector2(0, 0));
            var count = Label(card, listing.Players + " / " + listing.MaxPlayers + (listing.Locked ? "   LOCKED" : ""), 22, listing.Players >= listing.MaxPlayers ? Red : Ink, TextAnchor.MiddleRight);
            Anchor(count.rectTransform, new Vector2(1, 0), new Vector2(-18, 14), new Vector2(200, 30), new Vector2(1, 0));
            var button = card.gameObject.AddComponent<UnityEngine.UI.Button>(); button.targetGraphic = paper;
            var fx = Eclipse.UI.EclipseUiButton.Attach(button, paper, name, Paper, PaperWarm, Ink, Red, 0f, .025f, .03f);
            button.onClick.AddListener(() => { fx.Punch(); Eclipse.UI.EclipseUiAudio.Play(Eclipse.UI.UiSound.Confirm); join(); });
        }

        public void ShowRoomCreate(bool editing)
        {
            var session = RoomSession.Current;
            if (session == null) { ShowOnlineHome(); return; }
            EnsureEventSystem();
            editingRoom = editing && session.Room != null;
            page = Page.RoomCreate;
            draft = editingRoom ? session.Room.Settings.Copy() : new RoomSettings { Name = session.LocalName + "'s room" };
            Rebuild(editingRoom ? "ROOM SETTINGS" : "CREATE ROOM", editingRoom ? "Changes apply to new fights" : "Up to 8 players, up to 4 simultaneous fights", body =>
            {
                roomNameField = AddTextField(body, "ROOM NAME", draft.Name, 32);
                if (!editingRoom) passwordField = AddTextField(body, "PASSWORD", "", 24, "optional");
                AddChoice(body, "PLAYERS", () => draft.MaxPlayers.ToString(), () => draft.MaxPlayers = draft.MaxPlayers >= RoomProtocol.MaxMembers ? 2 : draft.MaxPlayers + 1);
                AddChoice(body, "FIRST TO", () => WinsLabel(draft.WinsRequired), () => draft.WinsRequired = draft.WinsRequired % 5 + 1);
                AddChoice(body, "ARENA", () => VersusRoster.ArenaName(draft.Arena),
                    () => VersusStagePicker.Open(draft.Arena, true, picked => { draft.Arena = picked; SetBackdropArena(picked); }));
                AddChoice(body, "MATCH MODE", () => RotationLabel(draft.Rotation).ToUpperInvariant(),
                    () => draft.Rotation = (RoomRotation)(((int)draft.Rotation + 1) % 3));
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

        private string builtFights;
        private RoomRotation builtRotation;

        private static string RotationLabel(RoomRotation rotation) => rotation == RoomRotation.Simultaneous ? "Simultaneous" :
            rotation == RoomRotation.WinnerStays ? "Winner stays" : "Everyone rotates";
        private RectTransform roomRoster, roomStage;
        private UnityEngine.UI.Text roomChatLog;
        private UnityEngine.UI.InputField roomChatInput;

        public void ShowRoom()
        {
            var session = RoomSession.Current;
            if (session == null || session.Room == null) { ShowOnlineHome(); return; }
            EnsureEventSystem();
            page = Page.Room;
            var room = session.Room;
            builtMembers = room.Members.Count;
            builtAsHost = session.IsHost;
            builtFights = null;
            builtRotation = room.Settings.Rotation;
            string rotation = RotationLabel(room.Settings.Rotation);
            if (room.Settings.Arena != VersusRoster.RandomArena) SetBackdropArena(room.Settings.Arena);
            RebuildScreen(room.Settings.Name.ToUpperInvariant(), "Room code " + room.Code + "   ·   first to " + room.Settings.WinsRequired + "   ·   " + rotation +
                "   ·   " + VersusRoster.ArenaName(room.Settings.Arena), Hints("L", "Loadout", "Q", "Queue", "T", "Chat", "Esc", "Leave"),
                () => RoomSession.Current?.Leave(), content =>
            {
                // Left: who is here.
                var rosterCard = Place(content, "Roster", new Vector2(0, 1), new Vector2(0, 0), new Vector2(590, 446));
                rosterCard.pivot = new Vector2(0, 1);
                var rosterPaper = rosterCard.gameObject.AddComponent<Eclipse.UI.PaperPanel>(); rosterPaper.color = Paper; rosterPaper.raycastTarget = true;
                roomRoster = Place(rosterCard, "Members", new Vector2(.5f, 1), new Vector2(0, -16), new Vector2(560, 414));
                var list = roomRoster.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
                list.spacing = 4; list.childControlHeight = list.childControlWidth = true; list.childForceExpandHeight = false;
                Eclipse.UI.UiReveal.Play(rosterCard, .04f, .34f, new Vector2(-30, 0), .97f);

                // Right top: the fight in progress.
                var stageCard = Place(content, "Now", new Vector2(1, 1), new Vector2(0, 0), new Vector2(570, 212));
                stageCard.pivot = new Vector2(1, 1);
                var stagePaper = stageCard.gameObject.AddComponent<Eclipse.UI.PaperPanel>(); stagePaper.color = Paper; stagePaper.raycastTarget = true;
                roomStage = Place(stageCard, "Stage", new Vector2(.5f, .5f), Vector2.zero, new Vector2(540, 190));
                Eclipse.UI.UiReveal.Play(stageCard, .1f, .34f, new Vector2(30, 0), .97f);

                // Right bottom: chat.
                var chat = Place(content, "Chat", new Vector2(1, 1), new Vector2(0, -226), new Vector2(570, 220));
                chat.pivot = new Vector2(1, 1);
                var glass = chat.gameObject.AddComponent<UnityEngine.UI.Image>(); glass.color = GlassInk; glass.raycastTarget = true;
                roomChatLog = Label(chat, "", 16, Paper, TextAnchor.LowerLeft);
                roomChatLog.supportRichText = true;
                roomChatLog.verticalOverflow = VerticalWrapMode.Truncate;
                roomChatLog.rectTransform.offsetMin = new Vector2(14, 50); roomChatLog.rectTransform.offsetMax = new Vector2(-14, -10);
                var inputRect = Place(chat, "Say", new Vector2(.5f, 0), new Vector2(0, 8), new Vector2(544, 36));
                roomChatInput = AddFilterField(inputRect);
                roomChatInput.characterLimit = ChatLine.MaxChars;
                roomChatInput.placeholder.GetComponent<UnityEngine.UI.Text>().text = "Say something  (T)";
                roomChatInput.onEndEdit.AddListener(SendRoomChat);
                RefreshRoomChat();
                Eclipse.UI.UiReveal.Play(chat, .16f, .34f, new Vector2(30, 0), .97f);

                // Bottom: this player's controls.
                var actions = Place(content, "Actions", new Vector2(.5f, 0), new Vector2(0, 0), new Vector2(1160, 48));
                var row = actions.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>(); row.spacing = 12; row.childControlWidth = row.childControlHeight = true; row.childForceExpandWidth = true;
                AddButton(actions, "YOUR LOADOUT", OpenRoomArmory, 0);
                var queue = AddButton(actions, "JOIN QUEUE", () => RoomSession.Current?.ToggleQueue(), 0, Eclipse.UI.UiSound.Toggle);
                var queueLabel = queue.GetComponentInChildren<UnityEngine.UI.Text>();
                liveLabels.Add((queueLabel, () => RoomSession.Current != null && RoomSession.Current.WantsQueue ? "LEAVE QUEUE" : "JOIN QUEUE"));
                if (session.IsHost) AddButton(actions, "ROOM SETTINGS", () => ShowRoomCreate(true), 0);
                AddButton(actions, "LEAVE ROOM", () => RoomSession.Current?.Leave(), 0, Eclipse.UI.UiSound.Back);
                shortcuts.Add((KeyCode.L, OpenRoomArmory));
                shortcuts.Add((KeyCode.Q, () => RoomSession.Current?.ToggleQueue()));
                shortcuts.Add((KeyCode.T, FocusRoomChat));
            });
            RefreshRoomRoster();
            liveLabels.Add((status, () => RoomSession.Current?.Notice ?? ""));
        }

        private void OpenRoomArmory()
        {
            var session = RoomSession.Current;
            if (session == null) return;
            ShowArmory("Your room loadout", session.LocalLoadout, loadout =>
            {
                RoomSession.Current?.SetLoadout(loadout);
                ShowRoom();
            });
        }

        private void FocusRoomChat()
        {
            if (roomChatInput == null || UnityEngine.EventSystems.EventSystem.current == null) return;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(roomChatInput.gameObject);
            roomChatInput.ActivateInputField();
        }

        private void SendRoomChat(string text)
        {
            if (roomChatInput == null) return;
            bool submitted = UnityEngine.Input.GetKey(KeyCode.Return) || UnityEngine.Input.GetKey(KeyCode.KeypadEnter);
            if (!submitted) return;
            if (!string.IsNullOrWhiteSpace(text)) RoomSession.Current?.Say(text);
            roomChatInput.text = string.Empty;
            // Stay in the chat for the next line.
            roomChatInput.ActivateInputField();
        }

        /// <summary>The chat log: the newest lines that fit, names in gold (your own in red), room lines in grey.</summary>
        private void RefreshRoomChat()
        {
            var session = RoomSession.Current;
            if (roomChatLog == null || session?.Client == null) return;
            var text = new System.Text.StringBuilder();
            var lines = session.Client.Chat;
            for (int i = Math.Max(0, lines.Count - 9); i < lines.Count; i++)
            {
                var line = lines[i];
                if (text.Length > 0) text.Append('\n');
                string body = Plain(line.Text);
                switch (line.Kind)
                {
                    case ChatKind.Player:
                        string color = line.SenderId == session.ClientId ? "#E07A5F" : "#D6AA4E";
                        text.Append("<color=").Append(color).Append('>').Append(Plain(line.SenderName)).Append("</color>  ").Append(body);
                        break;
                    case ChatKind.Notice:
                        text.Append("<color=#E07A5F>").Append(body).Append("</color>");
                        break;
                    default:
                        text.Append("<i><color=#A69C8C>").Append(body).Append("</color></i>");
                        break;
                }
            }
            roomChatLog.text = text.Length == 0 ? "<i><color=#A69C8C>Nobody has said anything yet.</color></i>" : text.ToString();
        }

        /// <summary>Player text shown as typed: angle brackets would otherwise open rich-text tags.</summary>
        private static string Plain(string text) => (text ?? string.Empty).Replace("<", "\u2039").Replace(">", "\u203A");

        /// <summary>One row per member: connection, name and tags, loadout, record and status.</summary>
        private void RefreshRoomRoster()
        {
            var session = RoomSession.Current;
            if (roomRoster == null || session?.Room == null) return;
            for (int i = roomRoster.childCount - 1; i >= 0; i--) Destroy(roomRoster.GetChild(i).gameObject);
            var room = session.Room;
            foreach (var member in room.Members)
            {
                var row = Rect(roomRoster, member.Name);
                row.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredHeight = 48;
                bool self = member.Id == session.ClientId;
                if (self)
                {
                    var mark = row.gameObject.AddComponent<UnityEngine.UI.Image>(); mark.color = new Color(Red.r, Red.g, Red.b, .12f); mark.raycastTarget = false;
                }
                var bars = Place(row, "Signal", new Vector2(0, .5f), new Vector2(10, 6), new Vector2(26, 20));
                bars.pivot = new Vector2(0, .5f);
                var signal = bars.gameObject.AddComponent<SignalBars>(); signal.raycastTarget = false;
                signal.Set(member.PingMs, (member.Link & MemberLink.Stale) != 0);
                var ping = Label(row, member.PingMs < 0 ? "-" : member.PingMs + " ms" + ((member.Link & MemberLink.Relayed) != 0 ? "  relay" : ""), 12,
                    new Color(Ink.r, Ink.g, Ink.b, .6f), TextAnchor.MiddleLeft);
                Anchor(ping.rectTransform, new Vector2(0, .5f), new Vector2(6, -16), new Vector2(80, 16), new Vector2(0, .5f));
                string tags = (room.HostId == member.Id ? "  <color=#D6AA4E>HOST</color>" : "") +
                    (room.ChampionId == member.Id ? "  <color=#93271F>CHAMPION" + (room.Streak > 1 ? " x" + room.Streak : "") + "</color>" : "");
                var name = Label(row, Plain(member.Name) + tags, 19, self ? Red : Ink, TextAnchor.MiddleLeft);
                name.supportRichText = true;
                Anchor(name.rectTransform, new Vector2(0, .5f), new Vector2(92, 9), new Vector2(230, 26), new Vector2(0, .5f));
                var record = Label(row, member.Wins + " - " + member.Losses + "   ·   " + session.MemberStatusLabel(member), 14, new Color(Ink.r, Ink.g, Ink.b, .7f), TextAnchor.MiddleLeft);
                Anchor(record.rectTransform, new Vector2(0, .5f), new Vector2(92, -14), new Vector2(230, 20), new Vector2(0, .5f));
                var strip = Place(row, "Loadout", new Vector2(1, .5f), new Vector2(-8, 0), new Vector2(222, 48));
                strip.pivot = new Vector2(1, .5f);
                AddLoadoutStrip(strip, RoomSession.LoadoutOf(member), 36f, null, onPaper: true);
            }
            if (room.Members.Count == 1)
            {
                var hint = Rect(roomRoster, "Invite");
                hint.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().preferredHeight = 60;
                Label(hint, "Share the code " + room.Code + " so friends can join.", 18, new Color(Ink.r, Ink.g, Ink.b, .65f), TextAnchor.MiddleCenter);
            }
            RefreshRoomStage();
        }

        /// <summary>Every active fight, with a separate spectate action.</summary>
        private void RefreshRoomStage()
        {
            var session = RoomSession.Current;
            if (roomStage == null || session?.Room == null) return;
            var room = session.Room;
            string fights = string.Join(",", room.Fights.Select(fight => fight.MatchId + ":" + fight.CanSpectate));
            if (fights == builtFights) return;
            builtFights = fights;
            for (int i = roomStage.childCount - 1; i >= 0; i--) Destroy(roomStage.GetChild(i).gameObject);
            if (room.Fights.Count == 0)
            {
                var waiting = Label(roomStage, "", 24, Ink, TextAnchor.MiddleCenter);
                liveLabels.Add((waiting, () => (RoomSession.Current?.NowPlaying() ?? "").ToUpperInvariant()));
                return;
            }
            for (int i = 0; i < room.Fights.Count; i++)
            {
                var fight = room.Fights[i];
                var row = Place(roomStage, "Fight " + fight.MatchId, new Vector2(.5f, 1), new Vector2(0, -i * 48), new Vector2(540, 44));
                row.pivot = new Vector2(.5f, 1);
                var layout = row.gameObject.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                layout.spacing = 8; layout.childControlWidth = layout.childControlHeight = true; layout.childForceExpandWidth = false;
                var label = Label(row, Plain(room.Find(fight.LeftId)?.Name ?? "?") + "  vs  " + Plain(room.Find(fight.RightId)?.Name ?? "?"), 18, Ink, TextAnchor.MiddleLeft);
                label.gameObject.AddComponent<UnityEngine.UI.LayoutElement>().flexibleWidth = 1;
                var watch = AddButton(row, fight.CanSpectate ? "SPECTATE" : "STARTING", () => RoomSession.Current?.Spectate(fight.MatchId), 150);
                watch.interactable = fight.CanSpectate && !session.InFight && session.PendingPairing == null;
            }
        }

        private void ShowSpectatorMenu(string message)
        {
            Rebuild("SPECTATING", message, body =>
            {
                if (!LocalVersusSession.HasResult) AddButton(body, "RESUME", LocalVersusSession.Resume);
                AddButton(body, "BACK TO ROOM", () =>
                {
                    if (RoomSession.Current != null) RoomSession.Current.StopSpectating();
                    else LocalVersusSession.ShowMultiplayerHome();
                });
                AddButton(body, "RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
            });
        }

        private void ShowRoomResult(string title, int playerOneWins, int playerTwoWins, string message)
        {
            var settings = LocalVersusSession.Settings;
            Rebuild(title, settings.PlayerOneName + "  " + playerOneWins + "  :  " + playerTwoWins + "  " + settings.PlayerTwoName, body =>
            {
                AddButton(body, "REMATCH", () => RoomSession.Current?.RequestRematch(), -1, Eclipse.UI.UiSound.Begin);
                AddButton(body, "BACK TO ROOM", () => RoomSession.Current?.ContinueAfterFight(), -1, Eclipse.UI.UiSound.Confirm);
                AddButton(body, "LEAVE ROOM", () => { RoomSession.Current?.Leave(); ShowOnlineHome(); });
                AddButton(body, "LEAVE AND RETURN TO TITLE", LocalVersusSession.ReturnToTitle);
            });
            string fixedMessage = message;
            liveLabels.Add((status, () =>
            {
                var rooms = RoomSession.Current;
                var online = OnlineVersusSession.Current;
                string text = online?.SyncProblem ?? fixedMessage ?? "";
                string opponent = online?.RoomMatch?.PeerName ?? "your opponent";
                if (rooms != null && rooms.PendingPairing != null) text = "Rematch accepted. Connecting...";
                else if (rooms != null && rooms.RematchRequested) text = (text.Length > 0 ? text + " " : "") + "Waiting for " + opponent + " to accept the rematch.";
                else if (rooms != null && rooms.OpponentWantsRematch) text = (text.Length > 0 ? text + " " : "") + opponent + " wants a rematch.";
                else if (rooms != null && rooms.RematchRefusal != null) text = rooms.RematchRefusal;
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
            if (page != Page.Room || session.Room == null) return;
            if (session.Room.Members.Count != builtMembers || session.IsHost != builtAsHost || session.Room.Settings.Rotation != builtRotation) { ShowRoom(); return; }
            // Loadouts, pings, records and the fight in progress change without anyone joining.
            RefreshRoomRoster();
        }

        public void OnRoomListChanged()
        {
            if (page == Page.RoomBrowser) ShowRoomBrowser();
        }

        public void OnRoomNotice(string text)
        {
            if (IsShowing) SetStatus(text);
        }

        /// <summary>A chat line arrived for the room the player is in.</summary>
        public void OnRoomChat(ChatLine line)
        {
            if (!IsShowing || line == null) return;
            if (page == Page.Room)
            {
                RefreshRoomChat();
                if (line.Kind == ChatKind.Player && line.SenderId != RoomSession.Current?.ClientId) Eclipse.UI.EclipseUiAudio.Play(Eclipse.UI.UiSound.Tick);
            }
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

    }
}

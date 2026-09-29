using System;
using System.Collections.Generic;
using Eclipse.Multiplayer.Online;
using Eclipse.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Multiplayer
{
    // The replay browser: every saved match as a card, with watch, keep, rename and delete.
    public sealed partial class LocalVersusMenu
    {
        private enum ReplayFilter { All, Online, Local, Kept }
        private static readonly string[] ReplayFilterNames = { "ALL", "ONLINE", "LOCAL", "KEPT" };

        private ReplayFilter replayFilter = ReplayFilter.All;
        private List<VersusReplays.Entry> replayEntries;
        private VersusReplays.Entry replaySelected;
        private string replayDeleteArmed;
        private RectTransform replayGrid, replayActions;
        private Text replayDetail;

        public void ShowReplays()
        {
            EnsureEventSystem();
            page = Page.Replays;
            replayEntries = VersusReplays.List();
            replayDeleteArmed = null;
            if (replaySelected != null && !replayEntries.Exists(entry => entry.Path == replaySelected.Path)) replaySelected = null;
            int count = replayEntries.Count;
            RebuildScreen("REPLAYS", count == 0 ? "No replays yet" : count == 1 ? "1 saved match" : count + " saved matches",
                Hints("Enter", "Watch", "K", "Keep", "Del", "Delete", "Tab", "Filter", "Esc", "Back"), ShowModeSelect, content =>
            {
                var tabs = Place(content, "Filters", new Vector2(0, 1), new Vector2(0, 0), new Vector2(560, 42));
                tabs.pivot = new Vector2(0, 1);
                var row = tabs.gameObject.AddComponent<HorizontalLayoutGroup>(); row.spacing = 8; row.childControlWidth = row.childControlHeight = true; row.childForceExpandWidth = true;
                for (int i = 0; i < ReplayFilterNames.Length; i++)
                {
                    var filter = (ReplayFilter)i;
                    var tab = AddButton(tabs, ReplayFilterNames[i], () => { replayFilter = filter; ShowReplays(); }, 0, UiSound.Tab);
                    if (filter == replayFilter) tab.GetComponent<EclipseUiButton>()?.SetColors(Red, RedBright, Paper, Paper);
                }
                var scroll = Place(content, "List", new Vector2(.5f, 1), new Vector2(0, -52), new Vector2(1180, 340));
                replayGrid = BuildScrollGrid(scroll, new Vector2(572, 150), 2);
                replayGrid.GetComponent<GridLayoutGroup>().spacing = new Vector2(16, 12);
                Button first = null;
                foreach (var entry in replayEntries)
                {
                    if (!Matches(entry)) continue;
                    var button = ReplayCard(replayGrid, entry);
                    if (first == null) first = button;
                }
                if (first == null)
                {
                    var empty = Label(content, count == 0 ? "Finish a local or online match and it is saved here." : "No replays match this filter.", 22, Paper, TextAnchor.MiddleCenter);
                    Anchor(empty.rectTransform, new Vector2(.5f, .5f), new Vector2(0, 20), new Vector2(900, 60));
                    Shade(empty);
                }
                replayDetail = Label(content, "", 17, Paper, TextAnchor.MiddleLeft);
                Anchor(replayDetail.rectTransform, new Vector2(0, 0), new Vector2(0, 60), new Vector2(1180, 26), new Vector2(0, 0));
                replayDetail.supportRichText = true;
                Shade(replayDetail);
                replayActions = Place(content, "Actions", new Vector2(.5f, 0), new Vector2(0, 0), new Vector2(1180, 48));
                BuildReplayActions();
                shortcuts.Add((KeyCode.K, ToggleReplayKept));
                shortcuts.Add((KeyCode.Delete, DeleteReplay));
                shortcuts.Add((KeyCode.Tab, () => { replayFilter = (ReplayFilter)(((int)replayFilter + 1) % ReplayFilterNames.Length); ShowReplays(); }));
                if (first != null && EventSystemAvailable) UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(first.gameObject);
            });
        }

        private bool Matches(VersusReplays.Entry entry)
        {
            var header = entry.Header;
            switch (replayFilter)
            {
                case ReplayFilter.Online: return header != null && header.Online;
                case ReplayFilter.Local: return header != null && !header.Online;
                case ReplayFilter.Kept: return header != null && header.Kept;
                default: return true;
            }
        }

        private Button ReplayCard(RectTransform parent, VersusReplays.Entry entry)
        {
            var header = entry.Header;
            var card = Rect(parent, System.IO.Path.GetFileName(entry.Path));
            var paper = card.gameObject.AddComponent<PaperPanel>(); paper.color = entry.Problem == null ? Paper : PaperDim; paper.raycastTarget = true;
            var thumbRect = Place(card, "Arena", new Vector2(0, .5f), new Vector2(14, 0), new Vector2(150, 120));
            thumbRect.pivot = new Vector2(0, .5f);
            var thumb = thumbRect.gameObject.AddComponent<RawImage>(); thumb.raycastTarget = false;
            BindArenaPicture(thumb, header?.Arena);
            if (header != null && header.Kept) Dot(thumbRect, new Vector2(56, 44), 22f, Gold);
            string title = header == null ? System.IO.Path.GetFileNameWithoutExtension(entry.Path) :
                !string.IsNullOrEmpty(header.Title) ? header.Title : Plain(header.LeftName) + "  vs  " + Plain(header.RightName);
            var name = Label(card, title, 22, Ink, TextAnchor.MiddleLeft);
            Anchor(name.rectTransform, new Vector2(0, 1), new Vector2(180, -12), new Vector2(380, 30), new Vector2(0, 1));
            name.resizeTextForBestFit = true; name.resizeTextMinSize = 14; name.resizeTextMaxSize = 22;
            if (header != null)
            {
                string result = header.Winner == VersusReplay.WinnerUnknown ? "" : header.Winner < 0 ? "Draw   " :
                    (header.Winner == 0 ? Plain(header.LeftName) : Plain(header.RightName)) + " won  " + header.LeftRounds + " : " + header.RightRounds + "   ";
                var info = Label(card, result + (header.Online ? "ONLINE" : "LOCAL") + "   ·   " + VersusRoster.ArenaName(header.Arena) + "   ·   " +
                    ClockTicks(header.TickCount), 14, Red, TextAnchor.MiddleLeft);
                Anchor(info.rectTransform, new Vector2(0, 1), new Vector2(180, -44), new Vector2(380, 20), new Vector2(0, 1));
                var left = Place(card, "Left", new Vector2(0, 0), new Vector2(180, 14), new Vector2(180, 46));
                left.pivot = new Vector2(0, 0);
                AddLoadoutStrip(left, VersusLoadout.FromIds(header.LeftLoadout), 32f, null, onPaper: true);
                var right = Place(card, "Right", new Vector2(0, 0), new Vector2(380, 14), new Vector2(180, 46));
                right.pivot = new Vector2(0, 0);
                AddLoadoutStrip(right, VersusLoadout.FromIds(header.RightLoadout), 32f, null, onPaper: true);
            }
            var date = Label(card, entry.Saved.ToString("d MMM  HH:mm"), 13, new Color(Ink.r, Ink.g, Ink.b, .6f), TextAnchor.UpperRight);
            Anchor(date.rectTransform, new Vector2(1, 1), new Vector2(-14, -12), new Vector2(120, 20), new Vector2(1, 1));
            if (entry.Problem != null)
            {
                var problem = Label(card, "CANNOT PLAY", 13, Red, TextAnchor.UpperRight);
                Anchor(problem.rectTransform, new Vector2(1, 1), new Vector2(-14, -32), new Vector2(160, 20), new Vector2(1, 1));
            }
            var button = card.gameObject.AddComponent<Button>(); button.targetGraphic = paper;
            var fx = EclipseUiButton.Attach(button, paper, name, entry.Problem == null ? Paper : PaperDim, PaperWarm, Ink, Red, 0f, .02f, .03f);
            UiHover.Attach(card.gameObject, () => SelectReplay(entry), null);
            button.onClick.AddListener(() => { fx.Punch(); SelectReplay(entry); WatchReplay(); });
            return button;
        }

        private static string ClockTicks(int ticks)
        {
            int seconds = ticks / 60;
            return (seconds / 60) + ":" + (seconds % 60).ToString("00");
        }

        private void SelectReplay(VersusReplays.Entry entry)
        {
            if (replaySelected == entry) return;
            replaySelected = entry;
            replayDeleteArmed = null;
            if (entry.Header != null && !string.IsNullOrEmpty(entry.Header.Arena) && VersusRoster.IsArena(entry.Header.Arena)) SetBackdropArena(entry.Header.Arena);
            BuildReplayActions();
        }

        private void BuildReplayActions()
        {
            if (replayActions == null) return;
            for (int i = replayActions.childCount - 1; i >= 0; i--) Destroy(replayActions.GetChild(i).gameObject);
            var row = Eclipse.UI.ComponentUtility.Ensure<HorizontalLayoutGroup>(replayActions.gameObject);
            row.spacing = 12; row.childControlWidth = row.childControlHeight = true; row.childForceExpandWidth = true;
            var entry = replaySelected;
            if (replayDetail != null)
                replayDetail.text = entry == null ? "Choose a replay." :
                    entry.Problem != null ? "<color=#E07A5F>" + entry.Problem + "</color>" :
                    System.IO.Path.GetFileName(entry.Path) + (entry.Header != null && !string.IsNullOrEmpty(entry.Header.Tag) ? "   ·   saved after a " + entry.Header.Tag : "");
            if (entry != null)
            {
                AddButton(replayActions, "WATCH", WatchReplay, 0, UiSound.Begin);
                AddButton(replayActions, entry.Header != null && entry.Header.Kept ? "UNKEEP" : "KEEP", ToggleReplayKept, 0, UiSound.Toggle);
                AddButton(replayActions, "RENAME", RenameReplay, 0);
                AddButton(replayActions, replayDeleteArmed == entry.Path ? "REALLY DELETE?" : "DELETE", DeleteReplay, 0, UiSound.Back);
            }
            AddButton(replayActions, "OPEN FOLDER", () => Application.OpenURL("file://" + VersusReplays.Directory.Replace('\\', '/')), 0);
            AddButton(replayActions, "BACK", ShowModeSelect, 0, UiSound.Back);
        }

        private void WatchReplay()
        {
            var entry = replaySelected;
            if (entry == null) return;
            if (OnlineVersusSession.IsActive) { SetStatus("Replays can be watched after leaving the online session."); return; }
            if (!VersusReplays.TryLoad(entry.Path, out var replay, out var error)) { SetStatus(error); return; }
            try { LocalVersusSession.StartReplay(replay); }
            catch (Exception exception) { Debug.LogException(exception); SetStatus(exception.Message); }
        }

        private void ToggleReplayKept()
        {
            var entry = replaySelected;
            if (entry?.Header == null) return;
            if (!VersusReplays.SetKept(entry.Path, !entry.Header.Kept, out var error)) { SetStatus(error); return; }
            SetStatus(entry.Header.Kept ? "No longer kept: it may be removed when the list is full." : "Kept: it is never removed automatically.");
            string path = entry.Path;
            ShowReplays();
            replaySelected = replayEntries.Find(e => e.Path == path);
            BuildReplayActions();
        }

        private void DeleteReplay()
        {
            var entry = replaySelected;
            if (entry == null) return;
            if (replayDeleteArmed != entry.Path) { replayDeleteArmed = entry.Path; BuildReplayActions(); SetStatus("Press delete again to remove this replay for good."); return; }
            if (!VersusReplays.Delete(entry.Path, out var error)) { SetStatus(error); return; }
            replaySelected = null;
            ShowReplays();
            SetStatus("Replay deleted.");
        }

        private void RenameReplay()
        {
            var entry = replaySelected;
            if (entry?.Header == null) return;
            for (int i = replayActions.childCount - 1; i >= 0; i--) Destroy(replayActions.GetChild(i).gameObject);
            var fieldRect = Rect(replayActions, "Title");
            fieldRect.gameObject.AddComponent<LayoutElement>().flexibleWidth = 3;
            var field = AddFilterField(fieldRect);
            field.characterLimit = 40;
            field.text = entry.Header.Title ?? string.Empty;
            field.placeholder.GetComponent<Text>().text = "Name this replay";
            string path = entry.Path;
            Action save = () =>
            {
                if (!VersusReplays.Rename(path, field.text, out var error)) { SetStatus(error); return; }
                ShowReplays();
                replaySelected = replayEntries.Find(e => e.Path == path);
                BuildReplayActions();
            };
            field.onEndEdit.AddListener(_ => { if (UnityEngine.Input.GetKey(KeyCode.Return) || UnityEngine.Input.GetKey(KeyCode.KeypadEnter)) save(); });
            AddButton(replayActions, "SAVE NAME", () => save(), 0, UiSound.Confirm);
            AddButton(replayActions, "CANCEL", BuildReplayActions, 0, UiSound.Back);
            if (EventSystemAvailable) { UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(field.gameObject); field.ActivateInputField(); }
        }
    }
}

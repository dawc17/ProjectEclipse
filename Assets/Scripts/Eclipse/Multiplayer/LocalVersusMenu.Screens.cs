using System;
using System.Collections.Generic;
using Eclipse.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Multiplayer
{
    // Full-screen pages over the living arena: the multiplayer home, the page frame
    // (title ribbon, content, footer with hints and status), music and the backdrop.
    public sealed partial class LocalVersusMenu
    {
        private static readonly Color PaperWarm = new Color32(240, 226, 196, 255);
        private static readonly Color RedBright = new Color32(214, 86, 64, 255);
        private static readonly Color Gold = new Color32(214, 170, 78, 255);
        private static readonly Color FooterInk = new Color(.09f, .066f, .052f, .84f);
        private const string MenuMusic = "gamedata/music/menu";
        /// <summary>Arenas the home page drifts through while nobody has chosen one.</summary>
        private static readonly string[] Showcase = { "sakura", "autumn", "night_bridge", "bamboo_grove", "lamps_on_water", "fuji", "moon", "snowy_peak" };

        private VersusBackdrop backdrop;
        private string backdropArena;
        private float nextShowcaseAt;
        private int showcaseIndex;
        private Action backAction;
        private readonly List<(KeyCode key, Action action)> shortcuts = new List<(KeyCode, Action)>();

        /// <summary>Hides the page (not the backdrop) while a picker covers it.</summary>
        internal void SetPageHidden(bool hidden)
        {
            if (panel == null) return;
            var group = ComponentUtility.Ensure<CanvasGroup>(panel.gameObject);
            group.alpha = hidden ? 0f : 1f;
            group.blocksRaycasts = !hidden;
            group.interactable = !hidden;
        }

        // ---- Backdrop and music ----

        private void EnterBackdrop()
        {
            if (!IsBackdropPage) { LeaveBackdrop(); return; }
            if (string.IsNullOrEmpty(backdropArena)) backdropArena = NextShowcaseArena();
            backdrop.Show(backdropArena);
            EclipseUiAudio.StartTitleMusic(MenuMusic, 1.6f);
        }

        private void LeaveBackdrop()
        {
            if (backdrop != null) backdrop.Hide();
            EclipseUiAudio.StopTitleMusic();
        }

        /// <summary>Shows <paramref name="arena"/> behind the menus (a roster id; "random" keeps the current one).</summary>
        internal void SetBackdropArena(string arena)
        {
            if (string.IsNullOrEmpty(arena) || arena == VersusRoster.RandomArena) return;
            backdropArena = arena;
            nextShowcaseAt = Time.unscaledTime + 40f;
            if (IsShowing && IsBackdropPage) backdrop.Show(arena);
        }

        private string NextShowcaseArena()
        {
            for (int tries = 0; tries < Showcase.Length; tries++)
            {
                string arena = Showcase[showcaseIndex++ % Showcase.Length];
                if (VersusRoster.IsArena(arena)) return arena;
            }
            return VersusRoster.Arenas.Count > 0 ? VersusRoster.Arenas[0].Id : "dojo";
        }

        /// <summary>Per-frame work for the full-screen pages: shortcuts, back, the drifting showcase.</summary>
        private void UpdateScreens()
        {
            if (!IsBackdropPage || VersusStagePicker.IsOpen) return;
            bool typing = UnityEngine.EventSystems.EventSystem.current != null &&
                UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != null &&
                UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null;
            if (!typing)
            {
                if (backAction != null && UnityEngine.Input.GetKeyDown(KeyCode.Escape)) { inputFrame = Time.frameCount; EclipseUiAudio.Play(UiSound.Back); backAction(); return; }
                foreach (var (key, action) in shortcuts)
                    if (UnityEngine.Input.GetKeyDown(key)) { inputFrame = Time.frameCount; EclipseUiAudio.Play(UiSound.Confirm); action(); return; }
            }
            if ((page == Page.ModeSelect || page == Page.OnlineHome || page == Page.RoomBrowser || page == Page.Replays) && Time.unscaledTime >= nextShowcaseAt)
            {
                nextShowcaseAt = Time.unscaledTime + 22f;
                backdrop.Show(backdropArena = NextShowcaseArena());
            }
        }

        // ---- Page frame ----

        /// <summary>
        /// A full-screen page: a red title ribbon top left, the content area, and a footer with
        /// key hints and the status line. <paramref name="back"/> runs on Escape.
        /// </summary>
        private RectTransform RebuildScreen(string title, string subtitle, string hints, Action back, Action<RectTransform> build)
        {
            panel.gameObject.SetActive(true);
            liveLabels.Clear();
            shortcuts.Clear();
            backAction = back;
            for (int i = panel.childCount - 1; i >= 0; i--) { panel.GetChild(i).gameObject.SetActive(false); Destroy(panel.GetChild(i).gameObject); }
            SetImage(panel, new Color(0, 0, 0, 0));
            EnterBackdrop();

            var ribbon = Rect(panel, "Title ribbon");
            ribbon.anchorMin = ribbon.anchorMax = ribbon.pivot = new Vector2(0, 1);
            ribbon.anchoredPosition = new Vector2(34, -26);
            ribbon.sizeDelta = new Vector2(Mathf.Clamp(120 + title.Length * 26, 360, 760), 84);
            var stroke = ribbon.gameObject.AddComponent<InkStroke>(); stroke.color = Red; stroke.raycastTarget = false; stroke.Seed = title.Length * 37 + 11;
            var titleLabel = Label(ribbon, title, 42, Paper, TextAnchor.MiddleLeft);
            titleLabel.rectTransform.offsetMin = new Vector2(40, 0);
            StartCoroutine(PaintStroke(stroke));
            UiReveal.Play(ribbon, 0f, .32f, new Vector2(-24, 0));
            if (!string.IsNullOrEmpty(subtitle))
            {
                var sub = Label(panel, subtitle, 21, Paper, TextAnchor.UpperLeft);
                sub.rectTransform.anchorMin = sub.rectTransform.anchorMax = sub.rectTransform.pivot = new Vector2(0, 1);
                sub.rectTransform.anchoredPosition = new Vector2(78, -114);
                sub.rectTransform.sizeDelta = new Vector2(900, 30);
                Shade(sub);
                UiReveal.Play(sub.rectTransform, .08f, .3f, new Vector2(-16, 0));
            }

            var footer = Rect(panel, "Footer");
            footer.anchorMin = new Vector2(0, 0); footer.anchorMax = new Vector2(1, 0); footer.pivot = new Vector2(.5f, 0);
            footer.sizeDelta = new Vector2(0, 44);
            var footerImage = footer.gameObject.AddComponent<Image>(); footerImage.color = FooterInk; footerImage.raycastTarget = false;
            var rule = Rect(footer, "Rule"); rule.anchorMin = new Vector2(0, 1); rule.anchorMax = new Vector2(1, 1); rule.pivot = new Vector2(.5f, 1); rule.sizeDelta = new Vector2(0, 2);
            var ruleImage = rule.gameObject.AddComponent<Image>(); ruleImage.color = Red; ruleImage.raycastTarget = false;
            var hintLabel = Label(footer, hints ?? string.Empty, 16, new Color(Paper.r, Paper.g, Paper.b, .78f), TextAnchor.MiddleLeft);
            hintLabel.supportRichText = true;
            hintLabel.rectTransform.offsetMin = new Vector2(28, 0); hintLabel.rectTransform.offsetMax = new Vector2(-640, 0);
            status = Label(footer, string.Empty, 16, RedBright, TextAnchor.MiddleRight);
            status.rectTransform.offsetMin = new Vector2(520, 0); status.rectTransform.offsetMax = new Vector2(-28, 0);

            var content = Rect(panel, "Content");
            content.anchorMin = Vector2.zero; content.anchorMax = Vector2.one;
            content.offsetMin = new Vector2(48, 60); content.offsetMax = new Vector2(-48, -150);
            build(content);
            IsShowing = true; Cursor.visible = true; Cursor.lockState = CursorLockMode.None;
            EclipseUiAudio.SuppressFocusSound();
            EclipseUiAudio.Play(UiSound.Open);
            FocusFirst(content);
            return content;
        }

        /// <summary>Key hints in the footer's style: the key in gold, then what it does.</summary>
        private static string Hints(params string[] pairs)
        {
            var text = new System.Text.StringBuilder();
            for (int i = 0; i + 1 < pairs.Length; i += 2)
            {
                if (text.Length > 0) text.Append("      ");
                text.Append("<color=#D6AA4E>").Append(pairs[i]).Append("</color>  ").Append(pairs[i + 1]);
            }
            return text.ToString();
        }

        private static void Shade(Graphic graphic)
        {
            var shadow = graphic.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0, 0, 0, .7f);
            shadow.effectDistance = new Vector2(1.5f, -2f);
        }

        private RectTransform Place(RectTransform parent, string name, Vector2 anchor, Vector2 position, Vector2 size)
        {
            var rect = Rect(parent, name);
            rect.anchorMin = rect.anchorMax = rect.pivot = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        // ---- Multiplayer home ----

        /// <summary>The multiplayer entry: local versus, online, training, and replays.</summary>
        public void ShowModeSelect()
        {
            EnsureEventSystem();
            page = Page.ModeSelect;
            RebuildScreen("MULTIPLAYER", "Choose how to fight", Hints("1 2 3", "Choose", "R", "Replays", "Esc", "Title"), LocalVersusSession.ReturnToTitle, content =>
            {
                var cards = Place(content, "Modes", new Vector2(.5f, .5f), new Vector2(0, 40), new Vector2(1140, 430));
                ModeCard(cards, 0, "LOCAL VERSUS", "Two players, one screen.\nKeyboard and gamepads.", ModeArt.Local, ShowLobby);
                ModeCard(cards, 1, "ONLINE", "Rooms, friends' codes and\ndirect connections.", ModeArt.Online, ShowOnlineHome);
                ModeCard(cards, 2, "TRAINING", "Practise against a dummy.\nReadouts, hitboxes, replays.", ModeArt.Training, ShowTraining);
                shortcuts.Add((KeyCode.Alpha1, ShowLobby));
                shortcuts.Add((KeyCode.Alpha2, ShowOnlineHome));
                shortcuts.Add((KeyCode.Alpha3, ShowTraining));
                shortcuts.Add((KeyCode.R, ShowReplays));

                var links = Place(content, "Links", new Vector2(.5f, 0), new Vector2(0, 0), new Vector2(Debug.isDebugBuild ? 900 : 620, 46));
                var row = links.gameObject.AddComponent<HorizontalLayoutGroup>();
                row.spacing = 16; row.childControlWidth = row.childControlHeight = true; row.childForceExpandWidth = true;
                AddButton(links, "REPLAYS", ShowReplays, 0);
                // A netcode self-test for development; players have no use for it.
                if (Debug.isDebugBuild) AddButton(links, "ROLLBACK TEST", TestRollbackOnLastReplay, 0);
                AddButton(links, "RETURN TO TITLE", LocalVersusSession.ReturnToTitle, 0);
                UiReveal.Play(links, .3f, .3f, new Vector2(0, -14));
            });
        }

        private enum ModeArt { Local, Online, Training }

        /// <summary>A large paper card with an ink illustration; it lifts and warms on focus.</summary>
        private void ModeCard(RectTransform parent, int index, string title, string caption, ModeArt art, Action open)
        {
            var card = Place(parent, title, new Vector2(.5f, .5f), new Vector2((index - 1) * 380, 0), new Vector2(340, 420));
            var paper = card.gameObject.AddComponent<PaperPanel>(); paper.color = Paper; paper.raycastTarget = true;
            var button = card.gameObject.AddComponent<Button>(); button.targetGraphic = paper;
            var artRect = Place(card, "Art", new Vector2(.5f, 1), new Vector2(0, -26), new Vector2(280, 230));
            DrawModeArt(artRect, art, index);
            var name = Label(card, title, 32, Ink, TextAnchor.MiddleCenter);
            name.rectTransform.anchorMin = new Vector2(0, 0); name.rectTransform.anchorMax = new Vector2(1, 0); name.rectTransform.pivot = new Vector2(.5f, 0);
            name.rectTransform.anchoredPosition = new Vector2(0, 110); name.rectTransform.sizeDelta = new Vector2(-30, 44);
            var text = Label(card, caption, 17, new Color(Ink.r, Ink.g, Ink.b, .78f), TextAnchor.UpperCenter);
            text.rectTransform.anchorMin = new Vector2(0, 0); text.rectTransform.anchorMax = new Vector2(1, 0); text.rectTransform.pivot = new Vector2(.5f, 0);
            text.rectTransform.anchoredPosition = new Vector2(0, 52); text.rectTransform.sizeDelta = new Vector2(-40, 54);
            var key = Place(card, "Key", new Vector2(.5f, 0), new Vector2(0, 16), new Vector2(34, 34));
            var keyDisc = key.gameObject.AddComponent<UiDisc>(); keyDisc.color = Ink; keyDisc.raycastTarget = false;
            Label(key, (index + 1).ToString(), 18, Gold, TextAnchor.MiddleCenter);
            var fx = EclipseUiButton.Attach(button, paper, name, Paper, PaperWarm, Ink, Red, 0f, .045f, .04f);
            button.onClick.AddListener(() => { fx.Punch(); EclipseUiAudio.Play(UiSound.Begin); open(); });
            UiReveal.Play(card, .06f + index * .08f, .38f, new Vector2(0, -30), .94f);
        }

        /// <summary>Brush-and-sun motifs: two crossing blades, a signal carried between two suns, a target.</summary>
        private void DrawModeArt(RectTransform art, ModeArt kind, int seed)
        {
            var sun = Place(art, "Sun", new Vector2(.5f, .5f), new Vector2(0, 0), new Vector2(170, 170));
            var disc = sun.gameObject.AddComponent<UiDisc>(); disc.color = new Color(Red.r, Red.g, Red.b, .92f); disc.raycastTarget = false;
            disc.SetRing(new Color(Ink.r, Ink.g, Ink.b, .35f), 3f);
            switch (kind)
            {
                case ModeArt.Local:
                    Brush(art, new Vector2(0, 0), new Vector2(250, 30), 34f, seed * 7 + 1);
                    Brush(art, new Vector2(0, 0), new Vector2(250, 30), -34f, seed * 7 + 2);
                    break;
                case ModeArt.Online:
                    Brush(art, new Vector2(0, 26), new Vector2(240, 18), 0f, seed * 7 + 3);
                    Brush(art, new Vector2(0, -4), new Vector2(210, 18), 0f, seed * 7 + 4);
                    Brush(art, new Vector2(0, -34), new Vector2(180, 18), 0f, seed * 7 + 5);
                    Dot(art, new Vector2(-120, 26), 22f, Ink);
                    Dot(art, new Vector2(120, -34), 22f, Ink);
                    break;
                case ModeArt.Training:
                    var ring = Place(art, "Target", new Vector2(.5f, .5f), Vector2.zero, new Vector2(110, 110));
                    var target = ring.gameObject.AddComponent<UiDisc>(); target.color = new Color(0, 0, 0, 0); target.raycastTarget = false;
                    target.SetRing(Ink, 7f);
                    Dot(art, Vector2.zero, 26f, Ink);
                    Brush(art, new Vector2(10, 0), new Vector2(240, 22), -58f, seed * 7 + 6);
                    break;
            }
        }

        private void Brush(RectTransform parent, Vector2 position, Vector2 size, float angle, int seed)
        {
            var rect = Place(parent, "Brush", new Vector2(.5f, .5f), position, size);
            rect.localRotation = Quaternion.Euler(0, 0, angle);
            var stroke = rect.gameObject.AddComponent<InkStroke>(); stroke.color = Ink; stroke.raycastTarget = false; stroke.Seed = seed;
            StartCoroutine(PaintStroke(stroke));
        }

        private void Dot(RectTransform parent, Vector2 position, float size, Color color)
        {
            var rect = Place(parent, "Dot", new Vector2(.5f, .5f), position, new Vector2(size, size));
            var disc = rect.gameObject.AddComponent<UiDisc>(); disc.color = color; disc.raycastTarget = false;
        }
    }
}

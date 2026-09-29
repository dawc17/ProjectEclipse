using System;
using System.Collections;
using Eclipse.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Multiplayer
{
    // Local versus lobby (two fighters, arena, rules) and the VS splash before a fight.
    public sealed partial class LocalVersusMenu
    {
        private const float SplashSeconds = 2.8f;
        private Text lobbyArenaName;
        private RawImage lobbyArenaThumb;
        private Action splashContinue;
        private float splashEndsAt;

        public void ShowLobby()
        {
            EnsureEventSystem(); ReadSettings(LocalVersusSession.Settings);
            page = Page.Lobby;
            SetBackdropArena(arena);
            RebuildScreen("LOCAL VERSUS", "Two players, one screen", Hints("1 2", "Loadouts", "A", "Arena", "Enter", "Fight", "Esc", "Back"), ShowModeSelect, content =>
            {
                FighterCard(content, "PLAYER 1", p1Loadout, false, () => ShowArmory("Player 1", p1Loadout, loadout =>
                {
                    p1Loadout = loadout; VersusLoadouts.Save(VersusLoadouts.PlayerOne, loadout); ShowLobby();
                }), () => DeviceLabel(0));
                FighterCard(content, "PLAYER 2", p2Loadout, true, () => ShowArmory("Player 2", p2Loadout, loadout =>
                {
                    p2Loadout = loadout; VersusLoadouts.Save(VersusLoadouts.PlayerTwo, loadout); ShowLobby();
                }), () => DeviceLabel(1));

                // Centre: the arena, the rules and the start.
                var arenaCard = Place(content, "Arena", new Vector2(.5f, 1), new Vector2(0, 0), new Vector2(420, 190));
                var paper = arenaCard.gameObject.AddComponent<PaperPanel>(); paper.color = Paper; paper.raycastTarget = true;
                var thumbRect = Place(arenaCard, "Thumb", new Vector2(.5f, 1), new Vector2(0, -14), new Vector2(390, 118));
                lobbyArenaThumb = thumbRect.gameObject.AddComponent<RawImage>(); lobbyArenaThumb.raycastTarget = false;
                lobbyArenaName = Label(arenaCard, "", 24, Ink, TextAnchor.MiddleCenter);
                Anchor(lobbyArenaName.rectTransform, new Vector2(.5f, 0), new Vector2(0, 16), new Vector2(390, 36));
                var pick = arenaCard.gameObject.AddComponent<Button>(); pick.targetGraphic = paper;
                EclipseUiButton.Attach(pick, paper, lobbyArenaName, Paper, PaperWarm, Ink, Red, 0f, .03f, .03f);
                pick.onClick.AddListener(OpenLobbyArenaPicker);
                RefreshLobbyArena();
                UiReveal.Play(arenaCard, .08f, .34f, new Vector2(0, -20), .96f);

                var rules = Place(content, "Rules", new Vector2(.5f, 1), new Vector2(0, -206), new Vector2(420, 128));
                var rulesPaper = rules.gameObject.AddComponent<PaperPanel>(); rulesPaper.color = Paper; rulesPaper.raycastTarget = true;
                var list = Place(rules, "List", new Vector2(.5f, .5f), Vector2.zero, new Vector2(390, 104));
                var layout = list.gameObject.AddComponent<VerticalLayoutGroup>(); layout.spacing = 10; layout.childControlHeight = layout.childControlWidth = true; layout.childForceExpandHeight = false;
                AddChoice(list, "FIRST TO", () => winsRequired + (winsRequired == 1 ? " WIN" : " WINS"), () => winsRequired = winsRequired % 5 + 1, 220);
                AddChoice(list, "CONTROLS", SchemeLabel, () =>
                {
                    // Keyboard + gamepad -> two gamepads -> shared keyboard.
                    if (sharedKeyboard) { sharedKeyboard = false; keyboardPlayerOne = true; }
                    else if (keyboardPlayerOne) keyboardPlayerOne = false;
                    else { sharedKeyboard = true; keyboardPlayerOne = true; }
                    RefreshDeviceStatus();
                }, 220);
                UiReveal.Play(rules, .14f, .34f, new Vector2(0, -20), .96f);

                var start = Place(content, "Start", new Vector2(.5f, 0), new Vector2(0, 24), new Vector2(420, 62));
                var startButton = AddButton(start, "FIGHT", StartLocalMatch, -1, UiSound.Begin);
                StretchChild(start);
                var fx = startButton.GetComponent<EclipseUiButton>(); fx?.SetColors(Red, RedBright, Paper, Paper);
                var startLabel = startButton.GetComponentInChildren<Text>(); if (startLabel != null) startLabel.fontSize = 30;
                UiReveal.Play(start, .22f, .34f, new Vector2(0, -20), .96f);

                shortcuts.Add((KeyCode.Alpha1, () => ShowArmory("Player 1", p1Loadout, loadout => { p1Loadout = loadout; VersusLoadouts.Save(VersusLoadouts.PlayerOne, loadout); ShowLobby(); })));
                shortcuts.Add((KeyCode.Alpha2, () => ShowArmory("Player 2", p2Loadout, loadout => { p2Loadout = loadout; VersusLoadouts.Save(VersusLoadouts.PlayerTwo, loadout); ShowLobby(); })));
                shortcuts.Add((KeyCode.A, OpenLobbyArenaPicker));
                if (EventSystemAvailable) UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(startButton.gameObject);
            });
            RefreshDeviceStatus();
        }

        private static bool EventSystemAvailable => UnityEngine.EventSystems.EventSystem.current != null;

        private void StartLocalMatch()
        {
            if (!SchemeReady(keyboardPlayerOne, sharedKeyboard)) { SetStatus(PadReason(keyboardPlayerOne)); return; }
            TryStart(new LocalVersusSettings(p1Loadout, p2Loadout, arena, keyboardPlayerOne, winsRequired, roundTime, sharedKeyboard: sharedKeyboard));
        }

        private void OpenLobbyArenaPicker()
        {
            VersusStagePicker.Open(arena, true, picked =>
            {
                // "Random" picks now, so both players see where they will fight.
                arena = picked == VersusRoster.RandomArena ? VersusRoster.ResolveArena(picked, new System.Random().Next()) : picked;
                SetBackdropArena(arena);
                RefreshLobbyArena();
                FocusLobbyStart();
            }, FocusLobbyStart);
        }

        private void FocusLobbyStart()
        {
            if (page != Page.Lobby || !EventSystemAvailable) return;
            var start = panel.Find("Content/Start");
            var button = start != null ? start.GetComponentInChildren<Button>() : null;
            if (button != null) UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

        private void RefreshLobbyArena()
        {
            if (lobbyArenaName != null) lobbyArenaName.text = VersusRoster.ArenaName(arena).ToUpperInvariant();
            BindArenaPicture(lobbyArenaThumb, arena);
        }

        /// <summary>Shows <paramref name="arena"/>'s picture on <paramref name="image"/> (ink until it has been drawn; red for Random).</summary>
        internal static void BindArenaPicture(RawImage image, string arena)
        {
            if (image == null) return;
            var ink = new Color(20 / 255f, 15 / 255f, 12 / 255f, .9f);
            image.texture = null;
            image.color = arena == VersusRoster.RandomArena ? (Color)new Color32(147, 39, 31, 255) : ink;
            var texture = ArenaThumbnails.Get(arena, ready =>
            {
                // The card may have been rebuilt meanwhile; only fill the one still asking.
                if (image == null || image.name != "Arena picture " + arena) return;
                image.texture = ready;
                image.color = Color.white;
            });
            image.name = "Arena picture " + arena;
            if (texture == null) return;
            image.texture = texture;
            image.color = Color.white;
        }

        private string DeviceLabel(int side)
        {
            if (sharedKeyboard) return side == 0 ? "KEYBOARD  ·  WASD" : "KEYBOARD  ·  ARROWS";
            if (side == 0) return keyboardPlayerOne ? "KEYBOARD" : "GAMEPAD 1" + (Eclipse.Input.FightGamepadInput.IsConnected(GamePad.Player.One) ? "" : "  (not connected)");
            var pad = keyboardPlayerOne ? GamePad.Player.One : GamePad.Player.Two;
            return "GAMEPAD " + (keyboardPlayerOne ? 1 : 2) + (Eclipse.Input.FightGamepadInput.IsConnected(pad) ? "" : "  (not connected)");
        }

        /// <summary>A fighter on a paper card with their loadout, input device and an edit button.</summary>
        private void FighterCard(RectTransform content, string title, VersusLoadout loadout, bool right, Action edit, Func<string> device)
        {
            var card = Place(content, title, new Vector2(right ? 1 : 0, .5f), new Vector2(0, 0), new Vector2(330, 510));
            var paper = card.gameObject.AddComponent<PaperPanel>(); paper.color = Paper; paper.raycastTarget = true;
            var header = Place(card, "Header", new Vector2(.5f, 1), new Vector2(0, -12), new Vector2(290, 52));
            var stroke = header.gameObject.AddComponent<InkStroke>(); stroke.color = right ? Ink : Red; stroke.raycastTarget = false; stroke.Seed = title.GetHashCode() & 0xffff;
            Label(header, title, 26, Paper, TextAnchor.MiddleCenter);
            StartCoroutine(PaintStroke(stroke));
            var stage = Place(card, "Stage", new Vector2(.5f, 1), new Vector2(0, -70), new Vector2(300, 250));
            VersusFighterPreview.Create(stage, right).Show(loadout, true);
            var weapon = Label(card, (loadout?.WeaponName ?? "-").ToUpperInvariant(), 20, Red, TextAnchor.MiddleCenter);
            Anchor(weapon.rectTransform, new Vector2(.5f, 0), new Vector2(0, 170), new Vector2(300, 28));
            var strip = Place(card, "Strip", new Vector2(.5f, 0), new Vector2(0, 108), new Vector2(300, 60));
            AddLoadoutStrip(strip, loadout, 46f, _ => edit(), onPaper: true);
            var deviceLabel = Label(card, device(), 15, new Color(Ink.r, Ink.g, Ink.b, .7f), TextAnchor.MiddleCenter);
            Anchor(deviceLabel.rectTransform, new Vector2(.5f, 0), new Vector2(0, 76), new Vector2(300, 22));
            liveLabels.Add((deviceLabel, device));
            var buttonRect = Place(card, "Edit", new Vector2(.5f, 0), new Vector2(0, 18), new Vector2(270, 48));
            AddButton(buttonRect, "EDIT LOADOUT", edit);
            StretchChild(buttonRect);
            UiReveal.Play(card, right ? .12f : .04f, .38f, new Vector2(right ? 40 : -40, 0), .96f);
        }

        // ---- VS splash ----

        /// <summary>
        /// Both fighters, their loadouts and the arena, before the fight loads. Fixed length
        /// online (both sides start together); Enter skips it locally.
        /// </summary>
        public void PlayVersusSplash(LocalVersusSettings settings, Action then)
        {
            EnsureEventSystem();
            page = Page.Splash;
            splashContinue = then;
            splashEndsAt = Time.unscaledTime + SplashSeconds;
            SetBackdropArena(settings.Location);
            RebuildScreen("VERSUS", VersusRoster.ArenaName(settings.Location) + "   ·   first to " + settings.WinsRequired, "", null, content =>
            {
                SplashSide(content, settings.PlayerOneName, settings.PlayerOneLoadout, false);
                SplashSide(content, settings.PlayerTwoName, settings.PlayerTwoLoadout, true);
                var stripe = Place(content, "Stripe", new Vector2(.5f, .5f), new Vector2(0, 30), new Vector2(220, 520));
                stripe.localRotation = Quaternion.Euler(0, 0, -8f);
                var ink = stripe.gameObject.AddComponent<InkStroke>(); ink.color = new Color(Ink.r, Ink.g, Ink.b, .92f); ink.raycastTarget = false; ink.Seed = 919;
                stripe.localScale = new Vector3(1f, 1f, 1f);
                var vs = Label(content, "VS", 110, RedBright, TextAnchor.MiddleCenter);
                Anchor(vs.rectTransform, new Vector2(.5f, .5f), new Vector2(0, 40), new Vector2(300, 150));
                Shade(vs);
                UiReveal.Play(vs.rectTransform, .25f, .3f, Vector2.zero, 1.8f);
                UiReveal.Play(stripe, .12f, .3f, new Vector2(0, 60), .8f);
            });
            EclipseUiAudio.Play(UiSound.Begin);
            StartCoroutine(RunSplash(settings.Mode == VersusMode.Local));
        }

        private void SplashSide(RectTransform content, string name, VersusLoadout loadout, bool right)
        {
            var side = Place(content, name, new Vector2(right ? 1 : 0, .5f), new Vector2(right ? -40 : 40, 20), new Vector2(420, 500));
            var stage = Place(side, "Stage", new Vector2(.5f, 1), new Vector2(0, 0), new Vector2(420, 360));
            // Paper-light fighters stand out on the darkened arena.
            VersusFighterPreview.Create(stage, right, new Color32(232, 216, 186, 255)).Show(loadout, true);
            var label = Label(side, name.ToUpperInvariant(), 38, Paper, TextAnchor.MiddleCenter);
            Anchor(label.rectTransform, new Vector2(.5f, 0), new Vector2(0, 92), new Vector2(420, 50));
            Shade(label);
            var weapon = Label(side, (loadout?.WeaponName ?? "").ToUpperInvariant(), 20, Gold, TextAnchor.MiddleCenter);
            Anchor(weapon.rectTransform, new Vector2(.5f, 0), new Vector2(0, 66), new Vector2(420, 28));
            Shade(weapon);
            var strip = Place(side, "Strip", new Vector2(.5f, 0), new Vector2(0, 8), new Vector2(360, 56));
            AddLoadoutStrip(strip, loadout, 44f, null, onPaper: false);
            UiReveal.Play(side, .05f, .42f, new Vector2(right ? 120 : -120, 0), .9f);
        }

        private IEnumerator RunSplash(bool skippable)
        {
            float skippableAt = Time.unscaledTime + .6f;
            while (page == Page.Splash && Time.unscaledTime < splashEndsAt)
            {
                if (skippable && Time.unscaledTime >= skippableAt &&
                    (UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.Space) || UnityEngine.Input.GetKeyDown(KeyCode.Escape)))
                    break;
                yield return null;
            }
            if (page != Page.Splash) yield break;
            var then = splashContinue;
            splashContinue = null;
            then?.Invoke();
        }
    }
}

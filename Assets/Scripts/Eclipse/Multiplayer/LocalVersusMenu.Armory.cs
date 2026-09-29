using System;
using System.Collections.Generic;
using Eclipse.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Multiplayer
{
    // The armory: build a loadout on a live fighter. Also the loadout strip every other
    // page uses to show what someone is wearing.
    public sealed partial class LocalVersusMenu
    {
        private static readonly string[] SlotTitles = { "MELEE", "ARMOR", "HELM", "RANGED", "MAGIC" };
        private static readonly LoadoutSlot[] SlotOrder = { LoadoutSlot.Weapon, LoadoutSlot.Ranged, LoadoutSlot.Magic, LoadoutSlot.Armor, LoadoutSlot.Helm };
        private static readonly Color GlassInk = new Color(.07f, .05f, .04f, .72f);
        private static readonly Color PaperDim = new Color32(196, 178, 146, 255);

        private VersusLoadout armoryLoadout;
        private LoadoutSlot armorySlot = LoadoutSlot.Weapon;
        private Action<VersusLoadout> armoryDone;
        private VersusFighterPreview armoryPreview;
        private RectTransform armoryGrid, armoryStrip, armoryTabs;
        private Text armoryDetail, armoryClass;
        private InputField armoryFilter;
        private RectTransform armoryPresets;
        private readonly System.Random armoryRandom = new System.Random();

        /// <summary>Opens the armory for <paramref name="who"/>; <paramref name="done"/> gets the result (also on Escape).</summary>
        public void ShowArmory(string who, VersusLoadout start, Action<VersusLoadout> done)
        {
            EnsureEventSystem();
            page = Page.Armory;
            armoryLoadout = (start ?? VersusLoadout.Default).Sanitized();
            armoryDone = done;
            armorySlot = LoadoutSlot.Weapon;
            armoryPresets = null;
            RebuildScreen("ARMORY", who, Hints("Tab", "Next slot", "Enter", "Equip", "R", "Random", "P", "Presets", "Esc", "Done"), FinishArmory, content =>
            {
                // Left: the fighter on a paper card.
                var card = Place(content, "Fighter card", new Vector2(0, .5f), new Vector2(0, 10), new Vector2(390, 500));
                var paper = card.gameObject.AddComponent<PaperPanel>(); paper.color = Paper; paper.raycastTarget = true;
                var stage = Place(card, "Stage", new Vector2(.5f, 1), new Vector2(0, -20), new Vector2(350, 330));
                armoryPreview = VersusFighterPreview.Create(stage, false);
                armoryPreview.Show(armoryLoadout, true);
                armoryClass = Label(card, "", 18, Red, TextAnchor.MiddleCenter);
                Anchor(armoryClass.rectTransform, new Vector2(.5f, 0), new Vector2(0, 118), new Vector2(360, 26));
                armoryStrip = Place(card, "Strip", new Vector2(.5f, 0), new Vector2(0, 28), new Vector2(360, 84));
                UiReveal.Play(card, .04f, .36f, new Vector2(-30, 0), .96f);

                // Right: tabs, grid, details and actions on dark glass.
                var shelf = Place(content, "Shelf", new Vector2(1, .5f), new Vector2(0, 10), new Vector2(770, 500));
                var glass = shelf.gameObject.AddComponent<Image>(); glass.color = GlassInk; glass.raycastTarget = true;
                armoryTabs = Place(shelf, "Tabs", new Vector2(.5f, 1), new Vector2(0, -12), new Vector2(740, 44));
                var filterRect = Place(shelf, "Filter", new Vector2(1, 1), new Vector2(-16, -64), new Vector2(250, 34));
                armoryFilter = AddFilterField(filterRect);
                armoryFilter.onValueChanged.AddListener(_ => RefreshArmoryGrid());
                var scroll = Place(shelf, "Items", new Vector2(.5f, 1), new Vector2(0, -106), new Vector2(740, 300));
                armoryGrid = BuildScrollGrid(scroll, new Vector2(112, 128), 6);
                armoryDetail = Label(shelf, "", 20, Paper, TextAnchor.MiddleLeft);
                Anchor(armoryDetail.rectTransform, new Vector2(0, 0), new Vector2(20, 64), new Vector2(730, 30), new Vector2(0, .5f));
                Shade(armoryDetail);
                var actions = Place(shelf, "Actions", new Vector2(.5f, 0), new Vector2(0, 10), new Vector2(740, 46));
                var row = actions.gameObject.AddComponent<HorizontalLayoutGroup>(); row.spacing = 12; row.childControlWidth = row.childControlHeight = true; row.childForceExpandWidth = true;
                AddButton(actions, "RANDOM SLOT", () => SetArmory(VersusLoadout.Random(armoryRandom, armorySlot, armoryLoadout)), 0, UiSound.Toggle);
                AddButton(actions, "RANDOM ALL", () => SetArmory(VersusLoadout.Random(armoryRandom, null, armoryLoadout)), 0, UiSound.Toggle);
                AddButton(actions, "PRESETS", ShowArmoryPresets, 0);
                AddButton(actions, "DONE", FinishArmory, 0, UiSound.Begin);
                UiReveal.Play(shelf, .1f, .36f, new Vector2(30, 0), .97f);

                shortcuts.Add((KeyCode.Tab, () => SelectArmorySlot(NextSlot(armorySlot, 1))));
                shortcuts.Add((KeyCode.E, () => SelectArmorySlot(NextSlot(armorySlot, 1))));
                shortcuts.Add((KeyCode.Q, () => SelectArmorySlot(NextSlot(armorySlot, -1))));
                shortcuts.Add((KeyCode.R, () => SetArmory(VersusLoadout.Random(armoryRandom, armorySlot, armoryLoadout))));
                shortcuts.Add((KeyCode.P, ShowArmoryPresets));
            });
            RefreshArmoryTabs();
            RefreshArmoryGrid();
            RefreshArmoryStrip();
        }

        private void FinishArmory()
        {
            if (armoryPresets != null) { CloseArmoryPresets(); return; }
            var done = armoryDone;
            armoryDone = null;
            done?.Invoke(armoryLoadout);
        }

        private static LoadoutSlot NextSlot(LoadoutSlot slot, int step)
        {
            int index = Array.IndexOf(SlotOrder, slot);
            return SlotOrder[(index + step + SlotOrder.Length) % SlotOrder.Length];
        }

        private void SelectArmorySlot(LoadoutSlot slot)
        {
            armorySlot = slot;
            if (armoryFilter != null) armoryFilter.text = string.Empty;
            RefreshArmoryTabs();
            RefreshArmoryGrid();
        }

        private void SetArmory(VersusLoadout loadout)
        {
            armoryLoadout = loadout.Sanitized();
            armoryPreview?.Show(armoryLoadout);
            RefreshArmoryGrid();
            RefreshArmoryStrip();
        }

        private void RefreshArmoryTabs()
        {
            for (int i = armoryTabs.childCount - 1; i >= 0; i--) Destroy(armoryTabs.GetChild(i).gameObject);
            var row = Eclipse.UI.ComponentUtility.Ensure<HorizontalLayoutGroup>(armoryTabs.gameObject);
            row.spacing = 8; row.childControlWidth = row.childControlHeight = true; row.childForceExpandWidth = true;
            foreach (var slot in SlotOrder)
            {
                var captured = slot;
                var tab = AddButton(armoryTabs, SlotTitles[(int)slot], () => SelectArmorySlot(captured), 0, UiSound.Tab);
                if (slot == armorySlot)
                {
                    var fx = tab.GetComponent<EclipseUiButton>();
                    fx?.SetColors(Red, RedBright, Paper, Paper);
                }
            }
            armoryFilter.gameObject.SetActive(armorySlot == LoadoutSlot.Armor || armorySlot == LoadoutSlot.Helm);
        }

        private void RefreshArmoryGrid()
        {
            if (armoryGrid == null) return;
            for (int i = armoryGrid.childCount - 1; i >= 0; i--) Destroy(armoryGrid.GetChild(i).gameObject);
            string filter = armoryFilter != null && armoryFilter.gameObject.activeInHierarchy ? armoryFilter.text.Trim() : string.Empty;
            var items = VersusRoster.Items(armorySlot);
            string equipped = armoryLoadout[armorySlot];
            Button first = null, current = null;
            foreach (var item in items)
            {
                if (filter.Length > 0 && item.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0 && item.Id.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0) continue;
                var captured = item;
                var tile = ItemTile(armoryGrid, item, item.Id == equipped, () =>
                {
                    EclipseUiAudio.Play(UiSound.Confirm);
                    SetArmory(armoryLoadout.With(captured.Slot, captured.Id));
                });
                UiHover.Attach(tile.gameObject, () =>
                {
                    armoryPreview?.Show(armoryLoadout.With(captured.Slot, captured.Id));
                    ShowArmoryDetail(captured);
                }, () =>
                {
                    armoryPreview?.Show(armoryLoadout);
                    ShowArmoryDetail(VersusRoster.Find(armorySlot, armoryLoadout[armorySlot]));
                });
                if (first == null) first = tile;
                if (item.Id == equipped) current = tile;
            }
            ShowArmoryDetail(VersusRoster.Find(armorySlot, equipped));
            var focus = current ?? first;
            if (focus != null && UnityEngine.EventSystems.EventSystem.current != null && armoryPresets == null)
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(focus.gameObject);
        }

        private void ShowArmoryDetail(VersusItem item)
        {
            if (armoryDetail == null) return;
            if (item == null) { armoryDetail.text = string.Empty; return; }
            string kind = item.Slot == LoadoutSlot.Weapon ? ClassName(item.SubType) : item.Slot == LoadoutSlot.Armor || item.Slot == LoadoutSlot.Helm ? "cosmetic" : ClassName(item.SubType);
            armoryDetail.text = item.Name + "   <color=#D6AA4E>" + kind + "</color>";
            armoryDetail.supportRichText = true;
            var weapon = VersusRoster.Find(LoadoutSlot.Weapon, armoryLoadout.Weapon);
            if (armoryClass != null) armoryClass.text = weapon == null ? string.Empty : (weapon.Name + "  ·  " + ClassName(weapon.SubType)).ToUpperInvariant();
        }

        /// <summary>"TwoHandedBlunt" as "two handed blunt"; the item class is how moves are chosen.</summary>
        private static string ClassName(string subType)
        {
            if (string.IsNullOrEmpty(subType)) return string.Empty;
            var text = new System.Text.StringBuilder();
            for (int i = 0; i < subType.Length; i++)
            {
                char c = subType[i];
                if (i > 0 && char.IsUpper(c) && !char.IsUpper(subType[i - 1])) text.Append(' ');
                text.Append(char.ToLowerInvariant(c));
            }
            return text.ToString();
        }

        private void RefreshArmoryStrip()
        {
            if (armoryStrip == null) return;
            for (int i = armoryStrip.childCount - 1; i >= 0; i--) Destroy(armoryStrip.GetChild(i).gameObject);
            AddLoadoutStrip(armoryStrip, armoryLoadout, 64f, slot => SelectArmorySlot(slot), onPaper: true);
        }

        // ---- Presets ----

        private void ShowArmoryPresets()
        {
            if (armoryPresets != null) { CloseArmoryPresets(); return; }
            armoryPresets = Rect(panel, "Presets");
            Stretch(armoryPresets);
            var scrim = armoryPresets.gameObject.AddComponent<Image>(); scrim.color = new Color(.05f, .035f, .03f, .66f); scrim.raycastTarget = true;
            var card = Place(armoryPresets, "Card", new Vector2(.5f, .5f), Vector2.zero, new Vector2(700, 560));
            var paper = card.gameObject.AddComponent<PaperPanel>(); paper.color = Paper; paper.raycastTarget = true;
            var header = Place(card, "Header", new Vector2(.5f, 1), new Vector2(0, -14), new Vector2(640, 70));
            var stroke = header.gameObject.AddComponent<InkStroke>(); stroke.color = Red; stroke.raycastTarget = false; stroke.Seed = 77;
            Label(header, "PRESETS", 34, Paper, TextAnchor.MiddleCenter);
            StartCoroutine(PaintStroke(stroke));
            var list = Place(card, "List", new Vector2(.5f, 1), new Vector2(0, -96), new Vector2(640, 360));
            var layout = list.gameObject.AddComponent<VerticalLayoutGroup>(); layout.spacing = 6; layout.childControlHeight = layout.childControlWidth = true; layout.childForceExpandHeight = false;
            var presets = VersusLoadouts.Presets();
            if (presets.Count == 0)
            {
                var note = Rect(list, "Empty"); note.gameObject.AddComponent<LayoutElement>().preferredHeight = 60;
                Label(note, "No presets yet. Save the current loadout below.", 19, new Color(Ink.r, Ink.g, Ink.b, .7f), TextAnchor.MiddleCenter);
            }
            foreach (var preset in presets)
            {
                var captured = preset;
                var row = Rect(list, "Preset"); row.gameObject.AddComponent<LayoutElement>().preferredHeight = 54;
                var name = Label(row, preset.Name, 20, Ink, TextAnchor.MiddleLeft);
                Anchor(name.rectTransform, new Vector2(0, .5f), new Vector2(8, 0), new Vector2(170, 50), new Vector2(0, .5f));
                var strip = Place(row, "Strip", new Vector2(0, .5f), new Vector2(180, 0), new Vector2(230, 50));
                strip.pivot = new Vector2(0, .5f);
                AddLoadoutStrip(strip, preset.Loadout, 40f, null, onPaper: true);
                var load = Place(row, "Load", new Vector2(1, .5f), new Vector2(-118, 0), new Vector2(110, 44));
                AddButton(load, "LOAD", () => { SetArmory(captured.Loadout); CloseArmoryPresets(); }).GetComponent<LayoutElement>().ignoreLayout = true;
                StretchChild(load);
                var delete = Place(row, "Delete", new Vector2(1, .5f), new Vector2(0, 0), new Vector2(110, 44));
                AddButton(delete, "DELETE", () => { VersusLoadouts.DeletePreset(captured.Index); CloseArmoryPresets(); ShowArmoryPresets(); }, -1, UiSound.Back)
                    .GetComponent<LayoutElement>().ignoreLayout = true;
                StretchChild(delete);
            }
            var saveRow = Place(card, "Save", new Vector2(.5f, 0), new Vector2(0, 24), new Vector2(640, 46));
            var nameField = AddTextField(saveRow, "NAME", "", 24, "Katana rushdown", 280);
            StretchChild(saveRow);
            var buttons = Place(card, "Buttons", new Vector2(.5f, 0), new Vector2(0, -34), new Vector2(640, 46));
            var buttonRow = buttons.gameObject.AddComponent<HorizontalLayoutGroup>(); buttonRow.spacing = 12; buttonRow.childControlWidth = buttonRow.childControlHeight = true; buttonRow.childForceExpandWidth = true;
            AddButton(buttons, "SAVE CURRENT", () =>
            {
                if (VersusLoadouts.SavePreset(nameField.text, armoryLoadout) < 0) { SetStatus("All " + VersusLoadouts.MaxPresets + " preset slots are used. Delete one first."); return; }
                CloseArmoryPresets();
                ShowArmoryPresets();
            }, 0, UiSound.Begin);
            AddButton(buttons, "CLOSE", CloseArmoryPresets, 0, UiSound.Back);
            UiReveal.Play(card, 0f, .3f, new Vector2(0, -20), .95f);
            FocusFirst(card);
        }

        private void CloseArmoryPresets()
        {
            if (armoryPresets != null) Destroy(armoryPresets.gameObject);
            armoryPresets = null;
            RefreshArmoryGrid();
        }

        // ---- Shared pieces ----

        /// <summary>
        /// The loadout as five icons (melee largest), each on a disc. <paramref name="onSlot"/>
        /// makes them buttons. Used by the armory, lobbies, rooms, the VS splash and replays.
        /// </summary>
        private RectTransform AddLoadoutStrip(RectTransform parent, VersusLoadout loadout, float size, Action<LoadoutSlot> onSlot, bool onPaper)
        {
            var strip = Rect(parent, "Loadout");
            Stretch(strip);
            if (loadout == null) return strip;
            LoadoutSlot[] order = { LoadoutSlot.Weapon, LoadoutSlot.Ranged, LoadoutSlot.Magic, LoadoutSlot.Armor, LoadoutSlot.Helm };
            float x = 0f;
            float total = size * 1.25f + (size + 6f) * 4f;
            for (int i = 0; i < order.Length; i++)
            {
                var slot = order[i];
                float tile = i == 0 ? size * 1.25f : size;
                var item = VersusRoster.Find(slot, loadout[slot]);
                var rect = Rect(strip, SlotTitles[(int)slot]);
                rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(.5f, .5f);
                rect.anchoredPosition = new Vector2(-total / 2f + x + tile / 2f, 0);
                rect.sizeDelta = new Vector2(tile, tile);
                x += tile + 6f;
                var disc = rect.gameObject.AddComponent<UiDisc>();
                disc.color = onPaper ? new Color(Ink.r, Ink.g, Ink.b, .86f) : new Color(Paper.r, Paper.g, Paper.b, .92f);
                disc.raycastTarget = onSlot != null;
                if (i == 0) disc.SetRing(Gold, 2.5f);
                var icon = Rect(rect, "Icon");
                Stretch(icon);
                icon.offsetMin = new Vector2(tile * .12f, tile * .12f); icon.offsetMax = new Vector2(-tile * .12f, -tile * .12f);
                var sprite = item?.Sprite;
                if (sprite != null)
                {
                    var image = icon.gameObject.AddComponent<Image>(); image.sprite = sprite; image.preserveAspect = true; image.raycastTarget = false;
                }
                else
                {
                    // Nothing equipped: a faint dash (unarmed says so), never a stray letter.
                    bool fist = item != null && item.IsNothing && slot == LoadoutSlot.Weapon;
                    var mark = Label(icon, fist ? "FIST" : item == null || item.IsNothing ? "-" : item.Name.Substring(0, 1), Mathf.RoundToInt(tile * (fist ? .26f : .42f)),
                        onPaper ? Paper : Ink, TextAnchor.MiddleCenter);
                    if (item == null || item.IsNothing) mark.color = new Color(mark.color.r, mark.color.g, mark.color.b, .55f);
                }
                if (onSlot != null)
                {
                    var button = rect.gameObject.AddComponent<Button>(); button.targetGraphic = disc;
                    var captured = slot;
                    button.onClick.AddListener(() => { EclipseUiAudio.Play(UiSound.Tab); onSlot(captured); });
                    PressBounce.Attach(rect.gameObject);
                }
            }
            return strip;
        }

        /// <summary>An item on a paper disc with its name; a gold ring marks the equipped one.</summary>
        private Button ItemTile(RectTransform parent, VersusItem item, bool equipped, Action pick)
        {
            var rect = Rect(parent, item.Id);
            var disc = Place(rect, "Disc", new Vector2(.5f, 1), new Vector2(0, -4), new Vector2(88, 88));
            var back = disc.gameObject.AddComponent<UiDisc>(); back.color = PaperDim; back.raycastTarget = true;
            if (equipped) back.SetRing(Gold, 4f);
            var icon = Rect(disc, "Icon"); Stretch(icon); icon.offsetMin = new Vector2(10, 10); icon.offsetMax = new Vector2(-10, -10);
            var sprite = item.Sprite;
            if (sprite != null) { var image = icon.gameObject.AddComponent<Image>(); image.sprite = sprite; image.preserveAspect = true; image.raycastTarget = false; }
            else Label(icon, item.IsNothing ? (item.Slot == LoadoutSlot.Weapon ? "FIST" : "-") : item.Name.Substring(0, 1), item.IsNothing && item.Slot == LoadoutSlot.Weapon ? 20 : 34,
                new Color(Ink.r, Ink.g, Ink.b, item.IsNothing ? .55f : 1f), TextAnchor.MiddleCenter);
            var name = Label(rect, item.Name, 14, equipped ? Gold : Paper, TextAnchor.UpperCenter);
            Anchor(name.rectTransform, new Vector2(.5f, 0), new Vector2(0, 2), new Vector2(110, 34));
            name.horizontalOverflow = HorizontalWrapMode.Wrap;
            name.resizeTextForBestFit = true; name.resizeTextMinSize = 11; name.resizeTextMaxSize = 14;
            var button = rect.gameObject.AddComponent<Button>(); button.targetGraphic = back;
            EclipseUiButton.Attach(button, back, name, PaperDim, PaperWarm, equipped ? Gold : Paper, Gold, 0f, .06f, .05f);
            button.onClick.AddListener(() => pick());
            return button;
        }

        /// <summary>A vertical scroll area holding a fixed-column grid; returns the grid content.</summary>
        private RectTransform BuildScrollGrid(RectTransform area, Vector2 cell, int columns)
        {
            var scroll = area.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 40f;
            var viewport = Rect(area, "Viewport"); Stretch(viewport);
            viewport.gameObject.AddComponent<RectMask2D>();
            var hit = viewport.gameObject.AddComponent<Image>(); hit.color = new Color(0, 0, 0, 0); hit.raycastTarget = true;
            var content = Rect(viewport, "Content");
            content.anchorMin = new Vector2(0, 1); content.anchorMax = new Vector2(1, 1); content.pivot = new Vector2(.5f, 1);
            content.sizeDelta = Vector2.zero;
            var grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = cell;
            grid.spacing = new Vector2(10, 8);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = columns;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.padding = new RectOffset(4, 4, 6, 6);
            var fitter = content.gameObject.AddComponent<ContentSizeFitter>(); fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.viewport = viewport;
            scroll.content = content;
            // Keyboard and pad focus scrolls the grid to keep the focused tile visible.
            area.gameObject.AddComponent<ScrollToSelection>().Scroll = scroll;
            return content;
        }

        private InputField AddFilterField(RectTransform rect)
        {
            var image = rect.gameObject.AddComponent<Image>(); image.color = new Color(Paper.r, Paper.g, Paper.b, .16f); image.raycastTarget = true;
            var text = Label(rect, "", 18, Paper, TextAnchor.MiddleLeft); text.supportRichText = false;
            text.rectTransform.offsetMin = new Vector2(12, 0); text.rectTransform.offsetMax = new Vector2(-12, 0);
            var hint = Label(rect, "Search", 17, new Color(Paper.r, Paper.g, Paper.b, .45f), TextAnchor.MiddleLeft); hint.fontStyle = FontStyle.Italic;
            hint.rectTransform.offsetMin = new Vector2(12, 0); hint.rectTransform.offsetMax = new Vector2(-12, 0);
            var field = rect.gameObject.AddComponent<InputField>();
            field.textComponent = text; field.placeholder = hint; field.targetGraphic = image;
            field.lineType = InputField.LineType.SingleLine; field.characterLimit = 24;
            field.customCaretColor = true; field.caretColor = Gold;
            return field;
        }

        private static void Anchor(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size, Vector2? pivot = null)
        {
            rect.anchorMin = rect.anchorMax = anchor;
            rect.pivot = pivot ?? anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        /// <summary>Makes a layout-built child fill its placed parent instead.</summary>
        private static void StretchChild(RectTransform rect)
        {
            if (rect == null) return;
            for (int i = 0; i < rect.childCount; i++)
            {
                var child = (RectTransform)rect.GetChild(i);
                var element = child.GetComponent<LayoutElement>();
                if (element != null) element.ignoreLayout = true;
                child.anchorMin = Vector2.zero; child.anchorMax = Vector2.one; child.offsetMin = child.offsetMax = Vector2.zero;
            }
        }
    }

    /// <summary>Scrolls a ScrollRect so the selected child stays in view (keyboard and gamepad).</summary>
    public sealed class ScrollToSelection : MonoBehaviour
    {
        public ScrollRect Scroll;
        private GameObject _last;

        private void Update()
        {
            var system = UnityEngine.EventSystems.EventSystem.current;
            if (Scroll == null || system == null) return;
            var selected = system.currentSelectedGameObject;
            if (selected == null || selected == _last || !selected.transform.IsChildOf(Scroll.content)) { _last = selected; return; }
            _last = selected;
            var target = (RectTransform)selected.transform;
            var viewport = Scroll.viewport;
            Vector3[] corners = new Vector3[4];
            target.GetWorldCorners(corners);
            Vector3 top = viewport.InverseTransformPoint(corners[1]), bottom = viewport.InverseTransformPoint(corners[0]);
            float viewTop = viewport.rect.yMax, viewBottom = viewport.rect.yMin;
            var position = Scroll.content.anchoredPosition;
            if (top.y > viewTop) position.y -= top.y - viewTop + 6f;
            else if (bottom.y < viewBottom) position.y += viewBottom - bottom.y + 6f;
            Scroll.content.anchoredPosition = position;
        }
    }
}

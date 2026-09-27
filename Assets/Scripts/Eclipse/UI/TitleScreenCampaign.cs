using System;
using System.Collections.Generic;
using Eclipse.Saves;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    public sealed partial class TitleScreen
    {
        private const int CampaignsPerPage = 4;
        private CampaignSaveStore campaignStore;
        private List<CampaignSaveInfo> campaigns = new List<CampaignSaveInfo>();
        private int campaignPage;
        private string campaignMessage;
        private InputField campaignNameField;
        private Action campaignNameSubmit;

        private void OpenCampaignSaves()
        {
            try
            {
                SF2Paths.Init();
                var store = new CampaignSaveStore(SF2Paths.GetLegacyUserDataDirectory());
                store.Initialize();
                campaignStore = store;
                campaignMessage = null;
                DrawCampaignSaves();
            }
            catch (Exception error)
            {
                Debug.LogException(error);
                campaignStore = null;
                Clear("Saves");
                Label(page, "Saves", 76, 96, 1000, 64, 42, Ink);
                var message = Label(page, "Could not open your saves.\n" + error.Message, 76, 220, 1120, 180, 23, Ink);
                message.supportRichText = false;
                Button(page, "Back", 76, 604, 320, 48, Home, UiSound.Back);
                Button(page, "Try again", 742, 604, 450, 48, OpenCampaignSaves);
                FocusFirst();
            }
        }

        private void DrawCampaignSaves(string focusId = null)
        {
            if (campaignStore == null) { OpenCampaignSaves(); return; }
            try { campaigns = campaignStore.List(); }
            catch (Exception error) { campaignMessage = error.Message; campaigns = new List<CampaignSaveInfo>(); }
            if (focusId != null)
            {
                int index = campaigns.FindIndex(save => save.Id == focusId);
                if (index >= 0) campaignPage = index / CampaignsPerPage;
            }
            int pages = Math.Max(1, (campaigns.Count + CampaignsPerPage - 1) / CampaignsPerPage);
            campaignPage = Math.Max(0, Math.Min(campaignPage, pages - 1));
            Clear("Saves");
            Label(page, "Saves", 76, 96, 900, 64, 42, Ink);
            Label(page, campaigns.Count + (campaigns.Count == 1 ? " save" : " saves"), 990, 108, 200, 40, 20, Ink, TextAnchor.MiddleRight);
            Label(page, "Choose a save to continue, or begin a new one.", 76, 166, 1120, 36, 20, Ink);
            Button focus = null;
            for (int row = 0; row < CampaignsPerPage && campaignPage * CampaignsPerPage + row < campaigns.Count; row++)
            {
                CampaignSaveInfo save = campaigns[campaignPage * CampaignsPerPage + row];
                float y = 218 + row * 72;
                var play = Button(page, save.Name, 76, y, 720, 64, () => LoadCampaign(save), UiSound.Begin);
                var name = play.GetComponentInChildren<Text>();
                name.supportRichText = false; name.fontSize = 23;
                name.resizeTextForBestFit = true; name.resizeTextMinSize = 17; name.resizeTextMaxSize = 23;
                name.rectTransform.sizeDelta = new Vector2(660, 32);
                play.GetComponent<EclipseUiButton>().Rehome();
                string summary = save.Error == null ? CampaignSummary(save) : "Save details could not be read";
                var detail = Label(play.transform, summary, 30, 33, 660, 23, 15,
                    new Color(Paper.r, Paper.g, Paper.b, .85f));
                detail.supportRichText = false;
                play.interactable = save.Error == null;
                var rename = Button(page, "Rename", 818, y + 12, 174, 42, () => CampaignNamePrompt(save), UiSound.Open, Look.Field);
                rename.interactable = save.Error == null;
                Button(page, "Delete", 1010, y + 12, 182, 42, () => DeleteCampaignPrompt(save), UiSound.Open, Look.Field);
                if (save.Id == focusId) focus = play;
            }
            if (campaigns.Count == 0)
            {
                Label(page, "Your journey begins here.", 76, 258, 1120, 68, 32, Ink, TextAnchor.MiddleCenter);
                Label(page, "Create a save to enter the world of Shadow Fight.", 76, 330, 1120, 60, 21, Ink, TextAnchor.MiddleCenter);
            }
            if (!string.IsNullOrEmpty(campaignMessage))
            {
                var status = Label(page, campaignMessage, 76, 510, 1120, 40, 17, Red);
                status.supportRichText = false;
            }
            if (pages > 1)
            {
                Button(page, "Previous", 76, 552, 240, 40, () => { campaignPage = (campaignPage + pages - 1) % pages; DrawCampaignSaves(); }, UiSound.Tab);
                Label(page, (campaignPage + 1) + " / " + pages, 332, 552, 150, 40, 20, Ink);
                Button(page, "Next", 500, 552, 210, 40, () => { campaignPage = (campaignPage + 1) % pages; DrawCampaignSaves(); }, UiSound.Tab);
            }
            Button(page, "Back", 76, 604, 320, 48, Home, UiSound.Back);
            var create = Button(page, "New save", 742, 604, 450, 48, () => CampaignNamePrompt(null), UiSound.Open);
            FocusFirst();
            if (focus != null && focus.interactable) focus.Select();
            else if (campaigns.Count == 0) create.Select();
        }

        private string CampaignSummary(CampaignSaveInfo save)
        {
            try
            {
                string summary = campaignStore.Progress(save.Id);
                if (save.LastPlayedUtc != default)
                    summary += "   ·   " + save.LastPlayedUtc.ToLocalTime().ToString("g");
                return summary;
            }
            catch (Exception error) { Debug.LogWarning("[Campaigns] " + error.Message); return "Progress unavailable"; }
        }

        private void LoadCampaign(CampaignSaveInfo save)
        {
            if (leaving) return;
            try
            {
                CampaignSaveSession.Select(campaignStore, save.Id);
                BeginCampaign();
            }
            catch (Exception error)
            {
                CampaignSaveSession.Clear();
                campaignMessage = "Could not open save: " + error.Message;
                DrawCampaignSaves(save.Id);
            }
        }

        private void CampaignNamePrompt(CampaignSaveInfo save)
        {
            bool creating = save == null;
            Clear(creating ? "New save" : "Rename save");
            Label(page, creating ? "A new journey" : "Rename save", 76, 96, 1120, 64, 42, Ink);
            Label(page, "Give your save a name.", 76, 212, 1120, 44, 24, Ink);
            var fieldRoot = Rect(page, "Save name", 76, 282, 1120, 68);
            var hit = fieldRoot.gameObject.AddComponent<Image>(); hit.color = new Color(Ink.r, Ink.g, Ink.b, .045f);
            var underline = Stroke(fieldRoot, "Ink underline", 0, 60, 1120, 7, "Save name");
            underline.color = Red;
            underline.raycastTarget = false;
            var value = Label(fieldRoot, "", 18, 4, 1084, 52, 28, Ink);
            value.supportRichText = false;
            var input = fieldRoot.gameObject.AddComponent<InputField>();
            input.targetGraphic = hit; input.textComponent = value;
            input.lineType = InputField.LineType.SingleLine;
            input.characterLimit = CampaignSaveStore.MaximumNameLength;
            input.text = creating ? "Save " + (campaigns.Count + 1) : save.Name;
            input.selectionColor = new Color(Red.r, Red.g, Red.b, .25f);
            campaignNameField = input;
            controls.Add(input);
            Label(page, "Up to 48 characters. Each save keeps its own progress.", 76, 376, 1120, 44, 20, Ink);
            var errorText = Label(page, "", 76, 466, 1120, 90, 19, Red); errorText.supportRichText = false;
            Action submit = () =>
            {
                if (leaving) return;
                try
                {
                    if (creating)
                    {
                        CampaignSaveInfo created = campaignStore.Create(input.text);
                        campaignMessage = null;
                        LoadCampaign(created);
                    }
                    else
                    {
                        campaignStore.Rename(save.Id, input.text);
                        campaignMessage = null;
                        DrawCampaignSaves(save.Id);
                    }
                }
                catch (Exception error) { errorText.text = error.Message; }
            };
            campaignNameSubmit = submit;
            // Confirm follows the field in keyboard/controller navigation.
            Button(page, creating ? "Create & play" : "Save name", 742, 604, 450, 48, submit, creating ? UiSound.Begin : UiSound.Confirm);
            Button(page, "Cancel", 76, 604, 320, 48, () => DrawCampaignSaves(save?.Id), UiSound.Back);
            FocusFirst();
        }

        private void DeleteCampaignPrompt(CampaignSaveInfo save)
        {
            Clear("Delete save");
            Label(page, "End this journey?", 76, 96, 1120, 64, 42, Ink);
            var name = Label(page, save.Name, 76, 240, 1120, 72, 32, Ink, TextAnchor.MiddleCenter);
            name.supportRichText = false;
            Label(page, "This deletes the save file and its saved progress.\nThis cannot be undone.",
                76, 330, 1120, 100, 23, Ink, TextAnchor.MiddleCenter);
            Button(page, "Keep save", 76, 604, 450, 48, () => DrawCampaignSaves(save.Id), UiSound.Back);
            Button(page, "Delete save", 742, 604, 450, 48, () =>
            {
                try { campaignStore.Delete(save.Id); campaignMessage = "Save file deleted."; }
                catch (Exception error) { campaignMessage = "Could not delete save: " + error.Message; }
                DrawCampaignSaves();
            });
            FocusFirst(); // Keep is the safe default for keyboard/controller confirmation.
        }
    }
}

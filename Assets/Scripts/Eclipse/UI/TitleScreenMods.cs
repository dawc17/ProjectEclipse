using System;
using System.Collections.Generic;
using System.IO;
using Eclipse.Modding;
using UnityEngine;

namespace Eclipse.UI
{
    public sealed partial class TitleScreen
    {
        private ModSelection modSelection;
        private ModDiscoveryResult modDiscovery;
        private string modSelectionPath;
        private string modMessage;
        private int modPage;
        private string pendingModZip;
        private bool pendingModZipIsTemporary;
        private ModZipPreview pendingModPreview;
        private const int ModsPerPage = 4;

        private void OpenMods()
        {
            modPage = 0;
            modMessage = null;
            try
            {
                string root = ModHost.GetDefaultModsRoot();
                modDiscovery = ModDiscovery.DiscoverLoose(root);
                modSelectionPath = ModHost.GetSelectionPath(root);
                try { modSelection = ModSelection.Load(modSelectionPath); }
                catch (Exception settingsError)
                {
                    modSelection = new ModSelection();
                    modMessage = "Saved selection could not be read. Review the defaults below, then Apply to replace it.";
                    Debug.LogWarning("[Mods] " + settingsError.Message);
                }
                DrawMods();
            }
            catch (Exception error)
            {
                Clear("Mods");
                Label(page, "Unable to read mod settings", 76, 96, 1100, 64, 42, Ink);
                Label(page, error.Message, 76, 190, 1100, 300, 22, Ink);
                Button(page, "Back", 76, 604, 240, 48, Home, UiSound.Back);
                FocusFirst();
            }
        }

        private DependencyResolutionResult ResolveModSelection() => DependencyResolver.Resolve(
            modSelection.Filter(modDiscovery.Mods), ModPlatformVersions.Core);

        private void DrawMods(int focus = 0)
        {
            Clear("Mods");
            Label(page, "Mods", 76, 96, 500, 64, 46, Ink);
            Label(page, "Core " + ModPlatformVersions.Core + "  ·  Always enabled", 730, 108, 465, 40, 20, Ink, TextAnchor.MiddleRight);
            Label(page, "Enable or disable mods, then apply to restart. Dependencies are toggled together.", 76, 166, 1120, 40, 19, Ink);
            var mods = modDiscovery.Mods;
            int pages = Math.Max(1, (mods.Count + ModsPerPage - 1) / ModsPerPage);
            modPage = Math.Min(modPage, pages - 1);
            for (int i = 0; i < ModsPerPage && modPage * ModsPerPage + i < mods.Count; i++)
            {
                var mod = mods[modPage * ModsPerPage + i];
                int row = i;
                float y = 226 + i * 62;
                var name = Label(page, mod.Manifest.Name + "  " + mod.Version, 76, y, 850, 30, 24, Ink);
                name.supportRichText = false;
                name.resizeTextForBestFit = true; name.resizeTextMinSize = 16; name.resizeTextMaxSize = 24;
                var id = Label(page, mod.Id.Value, 76, y + 30, 850, 24, 16, Ink);
                id.supportRichText = false;
                bool enabled = modSelection.IsEnabled(mod.Id);
                var toggle = Button(page, enabled ? "Enabled" : "Disabled", 952, y, 240, 48, () =>
                {
                    modSelection.SetEnabled(mod.Id, !modSelection.IsEnabled(mod.Id), mods);
                    modMessage = "Changes pending. Apply & Restart to use this selection.";
                    DrawMods(row);
                }, UiSound.Toggle);
                // State is the plate colour (red on, faded ink off); focus brightens either one.
                toggle.GetComponent<EclipseUiButton>().SetColors(
                    enabled ? Red : new Color(Ink.r, Ink.g, Ink.b, .38f),
                    enabled ? RedBright : new Color(Ink.r, Ink.g, Ink.b, .72f), Paper, Paper);
            }
            if (mods.Count == 0) Label(page, "No mods found in the Mods folder.", 76, 240, 1000, 70, 26, Ink);
            var resolution = ResolveModSelection();
            var issues = new List<ModDiagnostic>(modDiscovery.Diagnostics);
            issues.AddRange(resolution.Diagnostics);
            var status = Label(page, modMessage ?? (resolution.HasErrors ? "Some enabled mods have unmet requirements. Review details or disable them." : "Selections apply after a restart. Your saved mod progress is kept."),
                76, 500, 1120, 40, 18, resolution.HasErrors ? Red : Ink);
            status.supportRichText = false;
            if (pages > 1)
            {
                Button(page, "Previous", 76, 552, 240, 40, () => { modPage = (modPage + pages - 1) % pages; DrawMods(); }, UiSound.Tab);
                Label(page, (modPage + 1) + " / " + pages, 332, 552, 105, 40, 20, Ink);
                Button(page, "Next", 446, 552, 190, 40, () => { modPage = (modPage + 1) % pages; DrawMods(); }, UiSound.Tab);
            }
            if (issues.Count > 0) Button(page, "Details (" + issues.Count + ")", 742, 552, 450, 40, () => DrawModIssues(issues, 0));
            Button(page, "Back / Cancel", 76, 604, 320, 48, Home, UiSound.Back);
            Button(page, "Install ZIP", 418, 604, 300, 48, PickModZip);
            var apply = Button(page, "Apply & Restart", 742, 604, 450, 48, ApplyMods);
            apply.interactable = !resolution.HasErrors;
            FocusFirst();
            if (focus < controls.Count) controls[focus].Select();
        }

        private void DrawModIssues(List<ModDiagnostic> issues, int index)
        {
            Clear("Mod details");
            Label(page, "Mod diagnostics", 76, 96, 1000, 64, 42, Ink);
            var message = Label(page, issues[index].ToString(), 76, 190, 1120, 310, 23, Ink);
            message.supportRichText = false;
            Label(page, (index + 1) + " / " + issues.Count, 76, 552, 200, 40, 20, Ink);
            Button(page, "Back to Mods", 76, 604, 350, 48, () => DrawMods(), UiSound.Back);
            if (issues.Count > 1) Button(page, "Next issue", 842, 604, 350, 48, () => DrawModIssues(issues, (index + 1) % issues.Count));
            FocusFirst();
        }

        private void ApplyMods()
        {
            if (ResolveModSelection().HasErrors) { DrawMods(); return; }
            if (!GameSessionRestart.TryRestart(() => modSelection.Save(modSelectionPath), out var error))
            {
                modMessage = error ?? "Restart already in progress.";
                DrawMods();
                return;
            }
            Clear("Restarting");
            Label(page, "Restarting with your mod selection...", 76, 270, 1120, 120, 36, Ink, TextAnchor.MiddleCenter);
        }

        private void PickModZip()
        {
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                modMessage = "Choose a mod ZIP in Android's file picker.";
                DrawMods();
                ModZipPicker.PickAndroid(gameObject.name);
#else
                string path = ModZipPicker.PickDesktop();
                if (!string.IsNullOrEmpty(path)) PreviewModZip(path, false);
#endif
            }
            catch (Exception error)
            {
                modMessage = "Could not open file picker: " + error.Message;
                DrawMods();
            }
        }

        // Called by the Android document picker after it copies the selected URI to app cache.
        public void OnModZipPicked(string path)
        {
            if (!string.IsNullOrEmpty(path)) PreviewModZip(path, true);
            else { modMessage = "ZIP selection canceled."; DrawMods(); }
        }

        public void OnModZipPickerError(string error)
        {
            modMessage = "Could not read ZIP: " + error;
            DrawMods();
        }

        private void PreviewModZip(string path, bool temporary)
        {
            ClearPendingModZip();
            pendingModZip = path;
            pendingModZipIsTemporary = temporary;
            try
            {
                if (!string.Equals(Path.GetExtension(path), ".zip", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Choose a .zip archive.");
                pendingModPreview = ModZipInstaller.Inspect(path, ModHost.GetDefaultModsRoot());
                DrawModZipPreview();
            }
            catch (Exception error)
            {
                ClearPendingModZip();
                modMessage = "ZIP cannot be installed: " + error.Message;
                DrawMods();
            }
        }

        private void DrawModZipPreview(string error = null)
        {
            Clear("Mod ZIP");
            Label(page, pendingModPreview.IsUpdate ? "Update mod" : "Install mod", 76, 96, 1050, 64, 42, Ink);
            var name = Label(page, pendingModPreview.Manifest.Name + "  " + pendingModPreview.Manifest.Version,
                76, 205, 1100, 52, 30, Ink);
            name.supportRichText = false;
            var id = Label(page, "ID: " + pendingModPreview.Manifest.Id, 76, 262, 1100, 40, 22, Ink);
            id.supportRichText = false;
            Label(page, pendingModPreview.IsUpdate
                ? "The installed mod folder will be replaced. Your saved mod progress is kept."
                : "The mod will be added to this installation's Mods directory.",
                76, 335, 1100, 85, 21, Ink);
            var status = Label(page, error ?? "Review the mod, then install it. Apply & Restart on the Mods screen to load it.",
                76, 480, 1100, 90, 20, error == null ? Ink : Red);
            status.supportRichText = false;
            Button(page, "Back / Cancel", 76, 604, 320, 48, CancelModZip, UiSound.Back);
            Button(page, pendingModPreview.IsUpdate ? "Replace mod" : "Install mod", 742, 604, 450, 48, InstallModZip);
            FocusFirst();
        }

        private void InstallModZip()
        {
            try
            {
                ModZipPreview result = ModZipInstaller.Install(pendingModZip, ModHost.GetDefaultModsRoot(),
                    pendingModPreview.IsUpdate, pendingModPreview.Manifest.Id);
                ClearPendingModZip();
                modDiscovery = ModDiscovery.DiscoverLoose(ModHost.GetDefaultModsRoot());
                modMessage = (result.IsUpdate ? "Updated " : "Installed ") + result.Manifest.Name +
                    ". Review its toggle, then Apply & Restart.";
                DrawMods();
            }
            catch (Exception error)
            {
                DrawModZipPreview("Install failed: " + error.Message);
            }
        }

        private void CancelModZip()
        {
            ClearPendingModZip();
            modMessage = "ZIP install canceled.";
            DrawMods();
        }

        private void ClearPendingModZip()
        {
            if (pendingModZipIsTemporary && !string.IsNullOrEmpty(pendingModZip))
            {
                try { File.Delete(pendingModZip); }
                catch (Exception error) { Debug.LogWarning("[Mods] Could not remove temporary ZIP: " + error.Message); }
            }
            pendingModZip = null;
            pendingModZipIsTemporary = false;
            pendingModPreview = null;
        }
    }
}

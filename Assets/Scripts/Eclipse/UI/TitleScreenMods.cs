using System;
using System.Collections.Generic;
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
        private const int ModsPerPage = 5;

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
                Label(page, "Unable to read mod settings", 76, 80, 1100, 60, 36, Ink);
                Label(page, error.Message, 76, 180, 1100, 240, 22, Ink);
                Button(page, "Back", 76, 604, 240, 48, Home);
                FocusFirst();
            }
        }

        private DependencyResolutionResult ResolveModSelection() => DependencyResolver.Resolve(
            modSelection.Filter(modDiscovery.Mods), ModPlatformVersions.Api, ModPlatformVersions.Core);

        private void DrawMods(int focus = 0)
        {
            Clear("Mods");
            Label(page, "Mods", 76, 55, 500, 64, 46, Ink);
            Label(page, "Core " + ModPlatformVersions.Core + "  ·  Always enabled", 730, 70, 465, 40, 20, Ink, TextAnchor.MiddleRight);
            Label(page, "Enable or disable mods, then apply to restart. Dependencies are toggled together.", 76, 125, 1120, 40, 19, Ink);
            var mods = modDiscovery.Mods;
            int pages = Math.Max(1, (mods.Count + ModsPerPage - 1) / ModsPerPage);
            modPage = Math.Min(modPage, pages - 1);
            for (int i = 0; i < ModsPerPage && modPage * ModsPerPage + i < mods.Count; i++)
            {
                var mod = mods[modPage * ModsPerPage + i];
                int row = i;
                float y = 185 + i * 65;
                var name = Label(page, mod.Manifest.Name + "  " + mod.Version, 76, y, 850, 30, 24, Ink);
                name.supportRichText = false;
                name.resizeTextForBestFit = true; name.resizeTextMinSize = 16; name.resizeTextMaxSize = 24;
                var id = Label(page, mod.Id.Value, 76, y + 30, 850, 24, 16, Ink);
                id.supportRichText = false;
                Button(page, modSelection.IsEnabled(mod.Id) ? "Enabled" : "Disabled", 952, y, 240, 48, () =>
                {
                    modSelection.SetEnabled(mod.Id, !modSelection.IsEnabled(mod.Id), mods);
                    modMessage = "Changes pending. Apply & Restart to use this selection.";
                    DrawMods(row);
                });
            }
            if (mods.Count == 0) Label(page, "No mods found in the Mods folder.", 76, 200, 1000, 70, 26, Ink);
            var resolution = ResolveModSelection();
            var issues = new List<ModDiagnostic>(modDiscovery.Diagnostics);
            issues.AddRange(resolution.Diagnostics);
            Label(page, modMessage ?? (resolution.HasErrors ? "Some enabled mods have unmet requirements. Review details or disable them." : "Selections apply after a restart. Your saved mod progress is kept."),
                76, 525, 1120, 46, 18, resolution.HasErrors ? Red : Ink);
            if (pages > 1)
            {
                Button(page, "Previous", 76, 575, 240, 40, () => { modPage = (modPage + pages - 1) % pages; DrawMods(); });
                Label(page, (modPage + 1) + " / " + pages, 332, 575, 105, 40, 20, Ink);
                Button(page, "Next", 446, 575, 190, 40, () => { modPage = (modPage + 1) % pages; DrawMods(); });
            }
            if (issues.Count > 0) Button(page, "Details (" + issues.Count + ")", 742, 575, 450, 40, () => DrawModIssues(issues, 0));
            Button(page, "Back / Cancel", 76, 626, 320, 40, Home);
            var apply = Button(page, "Apply & Restart", 742, 626, 450, 40, ApplyMods);
            apply.interactable = !resolution.HasErrors;
            FocusFirst();
            if (focus < controls.Count) controls[focus].Select();
        }

        private void DrawModIssues(List<ModDiagnostic> issues, int index)
        {
            Clear("Mod details");
            Label(page, "Mod diagnostics", 76, 75, 1000, 64, 42, Ink);
            var message = Label(page, issues[index].ToString(), 76, 180, 1120, 330, 23, Ink);
            message.supportRichText = false;
            Label(page, (index + 1) + " / " + issues.Count, 76, 530, 200, 40, 20, Ink);
            Button(page, "Back to Mods", 76, 604, 350, 48, () => DrawMods());
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
    }
}

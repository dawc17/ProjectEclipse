using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eclipse.Multiplayer;
using Eclipse.Multiplayer.Balance;
using UnityEditor;
using UnityEngine;

/// <summary>Typed JSON authoring; numerical edits never alter a running match's snapshot.</summary>
public sealed class PvpBalanceEditor : EditorWindow
{
    private PvpBalanceProfile profile;
    private Vector2 scroll;
    private string message, search = "", overrideId = "", category = "", equipment = "", move = "";
    private int layer;
    private bool dirty;
    private bool saveLocally;
    private string ProjectFolder => Path.Combine(Application.dataPath, "Resources", PvpBalanceProfiles.ResourcePath);

    [MenuItem("SF2/Multiplayer/PvP Balance")]
    public static void Open() => GetWindow<PvpBalanceEditor>("PvP Balance");

    private void OnEnable()
    {
        if (profile == null)
        {
            profile = PvpBalanceProfiles.Selected.Export();
            saveLocally = File.Exists(Path.Combine(PvpBalanceProfiles.LocalDirectory, profile.Id + ".json"));
        }
    }

    private void OnGUI()
    {
        if (profile == null) OnEnable();
        using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
        {
            if (GUILayout.Button("Load preset", EditorStyles.toolbarDropDown)) ShowPresets();
            if (GUILayout.Button("Duplicate", EditorStyles.toolbarButton))
            {
                profile = profile.Compile().Export();
                profile.Id += "-copy"; profile.Name += " Copy"; dirty = true;
            }
            if (GUILayout.Button("Import JSON", EditorStyles.toolbarButton)) Import();
            if (GUILayout.Button("Export JSON", EditorStyles.toolbarButton)) Export();
            if (GUILayout.Button("Save", EditorStyles.toolbarButton)) Save();
        }
        EditorGUILayout.HelpBox("Edits apply to new matches. Online, the host chooses a profile and both players must have identical gameplay values. Category → equipment → move overrides replace each value independently.", MessageType.Info);
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUI.BeginChangeCheck();
        profile.Id = EditorGUILayout.TextField("Profile ID", profile.Id);
        profile.Name = EditorGUILayout.TextField("Name", profile.Name);
        saveLocally = EditorGUILayout.Toggle("Save as local override", saveLocally);
        EditorGUILayout.LabelField("Global damage", EditorStyles.boldLabel);
        profile.HitDamageScale = EditorGUILayout.Slider("Hit damage multiplier", profile.HitDamageScale, 0f, 10f);
        profile.BlockedDamageScale = EditorGUILayout.Slider("Blocked damage multiplier", profile.BlockedDamageScale, 0f, 10f);
        EditorGUILayout.LabelField("Recoverable block damage", EditorStyles.boldLabel);
        profile.RecoverableBlockedFraction = EditorGUILayout.Slider("Recoverable fraction", profile.RecoverableBlockedFraction, 0f, 1f);
        profile.RecoverOnHit = EditorGUILayout.Slider("Recovery / damage on hit", profile.RecoverOnHit, 0f, 10f);
        profile.RecoverOnBlock = EditorGUILayout.Slider("Recovery / damage on block", profile.RecoverOnBlock, 0f, 10f);
        profile.RecoverableLossOnHit = EditorGUILayout.Slider("Grey loss / incoming hit", profile.RecoverableLossOnHit, 0f, 1f);
        profile.MinimumLifeOnBlock = EditorGUILayout.FloatField("Minimum life fraction on block", profile.MinimumLifeOnBlock);
        EditorGUILayout.HelpBox("Recovery restores existing grey health only, based on actual damage inflicted. Blocking cannot kill. The floor is a fraction of maximum life; fighters already below it are never raised by blocking.", MessageType.None);
        if (EditorGUI.EndChangeCheck()) dirty = true;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Damage overrides", EditorStyles.boldLabel);
        layer = GUILayout.Toolbar(layer, new[] { "Categories", "Equipment", "Moves" });
        search = EditorGUILayout.TextField("Search", search);
        using (new EditorGUILayout.HorizontalScope())
        {
            overrideId = EditorGUILayout.TextField("New override ID", overrideId);
            if (GUILayout.Button("Browse", GUILayout.Width(70))) Browse();
            if (GUILayout.Button("Add", GUILayout.Width(50))) AddOverride();
        }
        var entries = Entries();
        foreach (var entry in entries.ToArray())
        {
            if (entry.Id.IndexOf(search, StringComparison.OrdinalIgnoreCase) < 0) continue;
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField(entry.Id, EditorStyles.boldLabel);
                EditorGUI.BeginChangeCheck();
                entry.HitDamageScale = Override("Hit damage", entry.HitDamageScale, profile.HitDamageScale);
                entry.BlockedDamageScale = Override("Blocked damage", entry.BlockedDamageScale, profile.BlockedDamageScale);
                if (EditorGUI.EndChangeCheck()) dirty = true;
                if (GUILayout.Button("Reset / remove override")) { SetEntries(Entries().Where(x => x != entry).ToArray()); dirty = true; }
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Effective values", EditorStyles.boldLabel);
        category = EditorGUILayout.TextField("Category ID", category);
        equipment = EditorGUILayout.TextField("Equipment ID", equipment);
        move = EditorGUILayout.TextField("Move ID", move);
        try
        {
            var snapshot = profile.Compile();
            var effective = snapshot.Resolve(category, equipment, move);
            EditorGUILayout.LabelField("Hit multiplier", effective.HitDamageScale + "  (" + effective.HitSource + ")");
            EditorGUILayout.LabelField("Blocked multiplier", effective.BlockedDamageScale + "  (" + effective.BlockedSource + ")");
            EditorGUILayout.LabelField("Gameplay hash", snapshot.Hash);
        }
        catch (Exception error) { EditorGUILayout.HelpBox(error.Message, MessageType.Error); }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.LabelField(dirty ? "Unsaved changes" : "Saved / loaded profile", EditorStyles.miniLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Save and use for new matches"))
            {
                if (Save()) UseProfile();
            }
            using (new EditorGUI.DisabledScope(!CanRestartTraining()))
                if (GUILayout.Button("Save and restart training")) RestartTraining();
        }
        if (!string.IsNullOrEmpty(message)) EditorGUILayout.HelpBox(message, MessageType.None);
    }

    private static float? Override(string label, float? value, float inherited)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            bool enabled = EditorGUILayout.ToggleLeft(label, value.HasValue, GUILayout.Width(130));
            if (!enabled) return null;
            return EditorGUILayout.Slider(value ?? inherited, 0f, 10f);
        }
    }

    private PvpDamageOverride[] Entries() => layer == 0 ? profile.Categories : layer == 1 ? profile.Equipment : profile.Moves;
    private void SetEntries(PvpDamageOverride[] values)
    { if (layer == 0) profile.Categories = values; else if (layer == 1) profile.Equipment = values; else profile.Moves = values; }

    private void AddOverride()
    {
        if (string.IsNullOrWhiteSpace(overrideId) || Entries().Any(x => x.Id == overrideId)) { message = "Choose a nonempty, unique ID."; return; }
        SetEntries(Entries().Concat(new[] { new PvpDamageOverride { Id = overrideId.Trim(), HitDamageScale = profile.HitDamageScale } }).ToArray());
        dirty = true;
    }

    private void Browse()
    {
        IEnumerable<string> ids = Entries().Select(x => x.Id);
        if (VersusRoster.GameDataLoaded)
        {
            var items = new[] { LoadoutSlot.Weapon, LoadoutSlot.Ranged, LoadoutSlot.Magic }.SelectMany(VersusRoster.Items).ToArray();
            ids = ids.Concat(layer == 0 ? items.Select(x => x.SubType).Concat(new[] { "Unarmed" }) :
                layer == 1 ? items.Select(x => x.Id) : AnimationData.Animations.Select(x => x.Name));
        }
        var menu = new GenericMenu();
        int count = 0;
        foreach (var id in ids.Where(x => !string.IsNullOrEmpty(x) && x.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0).Distinct().OrderBy(x => x, StringComparer.Ordinal))
        { string chosen = id; menu.AddItem(new GUIContent(chosen.Replace('/', '∕')), false, () => { overrideId = chosen; Repaint(); }); count++; }
        if (count == 0) menu.AddDisabledItem(new GUIContent("Enter multiplayer Play Mode to load the catalog, or type an exact ID."));
        menu.ShowAsContext();
    }

    private void ShowPresets()
    {
        PvpBalanceProfiles.Refresh();
        var menu = new GenericMenu();
        foreach (var preset in PvpBalanceProfiles.Available)
        { var chosen = preset; menu.AddItem(new GUIContent(chosen.Name), chosen.Id == profile.Id, () => {
            profile = chosen.Export(); dirty = false;
            saveLocally = File.Exists(Path.Combine(PvpBalanceProfiles.LocalDirectory, profile.Id + ".json")); Repaint();
        }); }
        menu.ShowAsContext();
    }

    private bool Save()
    {
        try
        {
            string json = profile.ToJson();
            string folder = saveLocally ? PvpBalanceProfiles.LocalDirectory : ProjectFolder;
            Directory.CreateDirectory(folder);
            string path = Path.Combine(folder, profile.Id + ".json");
            File.WriteAllText(path, json);
            if (!saveLocally) AssetDatabase.ImportAsset("Assets/Resources/" + PvpBalanceProfiles.ResourcePath + "/" + profile.Id + ".json");
            PvpBalanceProfiles.Refresh();
            dirty = false; message = "Saved " + path;
            return true;
        }
        catch (Exception error) { message = error.Message; return false; }
    }

    private void Import()
    {
        string path = EditorUtility.OpenFilePanel("Import PvP balance JSON", ProjectFolder, "json");
        if (string.IsNullOrEmpty(path)) return;
        try { profile = PvpBalanceProfile.Parse(File.ReadAllText(path)); dirty = true; message = "Imported; save to add it to project presets."; }
        catch (Exception error) { message = error.Message; }
    }

    private void Export()
    {
        try
        {
            string json = profile.ToJson();
            string path = EditorUtility.SaveFilePanel("Export PvP balance JSON", ProjectFolder, profile.Id + ".json", "json");
            if (!string.IsNullOrEmpty(path)) { File.WriteAllText(path, json); message = "Exported " + path; }
        }
        catch (Exception error) { message = error.Message; }
    }

    private static bool CanRestartTraining() => EditorApplication.isPlaying && LocalVersusSession.IsReady && !LocalVersusSession.IsStarting &&
        LocalVersusSession.Settings?.Mode == VersusMode.Training && !OnlineVersusSession.IsActive && !RoomSession.IsActive;

    private bool UseProfile()
    {
        try
        {
            if (PvpBalanceProfiles.Available.First(x => x.Id == profile.Id).Hash != profile.Compile().Hash)
                throw new InvalidOperationException("A local JSON overrides this project preset. Enable 'Save as local override' to use these edits, or remove the local override.");
            PvpBalanceProfiles.Select(profile.Id);
            message = "Selected " + profile.Name + ". Active matches retain their rules.";
            return true;
        }
        catch (Exception error) { message = error.Message; return false; }
    }

    private void RestartTraining()
    {
        if (!CanRestartTraining() || !Save()) return;
        try
        {
            if (!UseProfile()) return;
            var old = LocalVersusSession.Settings;
            var settings = new LocalVersusSettings(old.PlayerOneLoadout, old.PlayerTwoLoadout, old.Location, old.KeyboardPlayerOne,
                old.WinsRequired, old.RoundTimeSeconds, VersusMode.Training, old.PlayerOneName, old.PlayerTwoName,
                sharedKeyboard: old.SharedKeyboard, playerTwoTactic: old.PlayerTwoTactic, balance: PvpBalanceProfiles.Selected);
            LocalVersusSession.StartMatch(settings, () => new TrainingInputSource());
            TrainingHud.Begin();
            message = "Restarting training with " + profile.Name + ".";
        }
        catch (Exception error) { message = error.Message; }
    }
}

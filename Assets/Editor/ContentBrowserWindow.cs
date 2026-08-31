using System;
using System.Collections.Generic;
using System.IO;
using Eclipse.Content;
using UnityEditor;
using UnityEngine;

// One searchable index for the existing GUID-backed assets and packaged runtime content.
public sealed class ContentBrowserWindow : EditorWindow
{
    private sealed class Entry
    {
        public string Kind;
        public string Address;
        public string Path;
        public string Search;
    }

    private readonly List<Entry> entries = new List<Entry>();
    private string query = "";
    private Vector2 scroll;
    private string error;

    [MenuItem("SF2/Content Browser")]
    public static void Open()
    {
        GetWindow<ContentBrowserWindow>("Eclipse Content");
    }

    private void OnEnable() { RefreshIndex(); }

    private void Add(string kind, string address, string path)
    {
        entries.Add(new Entry { Kind = kind, Address = address, Path = path,
            Search = (kind + " " + address + " " + path).ToLowerInvariant() });
    }

    private void RefreshIndex()
    {
        entries.Clear();
        error = null;
        try
        {
            string catalogPath = "Assets/Resources/" + PackagedArtCatalog.CatalogResourcePath + ".json";
            var catalog = PackagedArtCatalog.ReadCatalog(File.ReadAllText(catalogPath));
            foreach (var bundle in catalog.bundles)
                foreach (var asset in bundle.assets)
                {
                    if (catalog.version == 3)
                    {
                        if (!string.IsNullOrEmpty(asset.font))
                            Add("Font", asset.address, "Assets/Resources/" + asset.font + ".ttf");
                        else if (!string.IsNullOrEmpty(bundle.file))
                            Add(asset.address.StartsWith("gamedata/models/", StringComparison.OrdinalIgnoreCase) ? "TAR model" :
                                string.Equals(bundle.name, "LOCATION_DATA", StringComparison.OrdinalIgnoreCase) ? "TAR atlas" : "TAR asset", asset.address, "Assets/StreamingAssets/" +
                                PackagedArtCatalog.TarStreamingRoot + "/" + bundle.file);
                        continue;
                    }

                    string resource = !string.IsNullOrEmpty(asset.sprites) ? asset.sprites :
                        !string.IsNullOrEmpty(asset.texture) ? asset.texture :
                        !string.IsNullOrEmpty(asset.audio) ? asset.audio : asset.font;
                    if (string.IsNullOrEmpty(resource) || catalog.files == null) continue;
                    string prefix = resource.Substring(PackagedArtCatalog.ResourceRoot.Length + 1);
                    var file = Array.Find(catalog.files, x => x.path.StartsWith(prefix + ".", StringComparison.Ordinal));
                    if (file != null) Add("Native art", asset.address, "Assets/Resources/" + PackagedArtCatalog.ResourceRoot + "/" + file.path);
                }
            foreach (string path in AssetDatabase.GetAllAssetPaths())
            {
                if (!path.StartsWith("Assets/", StringComparison.Ordinal) || AssetDatabase.IsValidFolder(path) ||
                    path.StartsWith("Assets/Resources/" + PackagedArtCatalog.ResourceRoot + "/", StringComparison.Ordinal))
                    continue;
                if (path.StartsWith("Assets/Resources/", StringComparison.Ordinal))
                {
                    string address = path.Substring("Assets/Resources/".Length);
                    Add("Resource", address.Substring(0, address.Length - Path.GetExtension(address).Length), path);
                }
                else if (path.StartsWith("Assets/vanillaXml/", StringComparison.Ordinal))
                    Add("Gameplay", path.Substring("Assets/vanillaXml/".Length), path);
                else if (path.StartsWith("Assets/src/", StringComparison.Ordinal) ||
                    path.StartsWith("Assets/GameObject/", StringComparison.Ordinal) ||
                    path.StartsWith("Assets/Shader/", StringComparison.Ordinal))
                    Add("Referenced", Path.GetFileName(path), path);
            }
            entries.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.Address, b.Address));
        }
        catch (Exception exception) { error = exception.Message; }
        Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("Search resource names, gameplay/config files, scenes, or packaged runtime assets. TAR entries select their archive; use Tools/AssetPacker to list, extract, edit, and repack archive contents. Models are runtime asset XML inside TAR, while gameplay and location params stay loose/moddable. ResearchSources and DExml are archival, not runtime sources.", MessageType.Info);
        using (new EditorGUILayout.HorizontalScope())
        {
            query = EditorGUILayout.TextField("Search", query);
            if (GUILayout.Button("Refresh", GUILayout.Width(70))) RefreshIndex();
        }
        if (error != null) EditorGUILayout.HelpBox(error, MessageType.Error);
        string filter = query.Trim().ToLowerInvariant();
        int count = 0;
        using (var view = new EditorGUILayout.ScrollViewScope(scroll))
        {
            scroll = view.scrollPosition;
            foreach (Entry entry in entries)
            {
                if (filter.Length != 0 && !entry.Search.Contains(filter)) continue;
                count++;
                if (count > 250) continue;
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(entry.Kind, GUILayout.Width(85));
                    GUILayout.Label(new GUIContent(entry.Address, entry.Path));
                    if (GUILayout.Button("Select", GUILayout.Width(55)))
                    {
                        UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(entry.Path);
                        Selection.activeObject = asset;
                        EditorGUIUtility.PingObject(asset);
                    }
                }
            }
        }
        EditorGUILayout.LabelField(count + " matches / " + entries.Count + " entries. Showing at most 250; narrow the search for more.");
    }
}

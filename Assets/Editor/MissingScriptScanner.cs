using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MissingScriptScanner
{
    private static int _totalMissing;
    private static readonly List<string> Flagged = new List<string>();

    [MenuItem("Tools/Diagnostics/Scan Project For Missing Scripts")]
    public static void ScanAll()
    {
        _totalMissing = 0;
        Flagged.Clear();

        foreach (var path in Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories))
        {
            ScanPrefab(path);
        }

        foreach (var path in Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories))
        {
            ScanScene(path);
        }

        if (Flagged.Count == 0)
        {
            Debug.Log("[MissingScriptScanner] Clean - no missing scripts in any scene or prefab.");
        }
        else
        {
            foreach (var line in Flagged)
            {
                Debug.Log("[MissingScriptScanner] " + line);
            }
            Debug.LogWarning("[MissingScriptScanner] TOTAL missing script components: " + _totalMissing
                             + " across " + Flagged.Count + " asset(s)");
        }
    }

    private static void ScanPrefab(string path)
    {
        try
        {
            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                Report(path, CountIn(root));
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[MissingScriptScanner] Skipped prefab '" + path + "': " + e.Message);
        }
    }

    private static void ScanScene(string path)
    {
        try
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            try
            {
                int n = 0;
                foreach (var go in scene.GetRootGameObjects())
                {
                    n += CountIn(go);
                }
                Report(path, n);
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[MissingScriptScanner] Skipped scene '" + path + "': " + e.Message);
        }
    }

    private static void Report(string path, int count)
    {
        if (count <= 0)
        {
            return;
        }
        Flagged.Add(count + "  " + path);
        _totalMissing += count;
    }

    private static int CountIn(GameObject go)
    {
        int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
        foreach (Transform child in go.transform)
        {
            count += CountIn(child.gameObject);
        }
        return count;
    }
}

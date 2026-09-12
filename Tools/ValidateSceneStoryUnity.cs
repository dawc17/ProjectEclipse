#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class ValidateSceneStoryUnity
{
    const string Pending="Eclipse.SceneStoryUnity";
    static ValidateSceneStoryUnity()
    {
        EditorApplication.playModeStateChanged += state => {
            if(state==PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(Pending,false))
            {
                SessionState.SetBool(Pending,false);
                EditorApplication.delayCall += () => new GameObject("Scene fixture driver").AddComponent<SceneStoryUnityDriver>();
            }
        };
    }
    public static void RunEditor()
    {
        SessionState.SetBool(Pending,true);
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        EditorApplication.EnterPlaymode();
    }
}
#endif

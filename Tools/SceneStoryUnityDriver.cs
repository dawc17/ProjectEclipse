#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using Eclipse.Modding;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// Controlled native host; the scene helper, event transport and publisher are production source.
public sealed class Module
{
    public static readonly Module Instance=new Module();
    public ScreenType Requested=ScreenType.ModuleMap;
    public static Module ELEBLBJKDBI()=>Instance;
    public ScreenType NMCNDOPKFJD()=>Requested;
}
namespace Eclipse.Modding
{
    public static class ModRuntime
    {
        public static object _profileRoster=new object();
        public static readonly ModStoryEvents StoryEvents=new ModStoryEvents((id,message)=>Debug.LogError(message));
        /* PUBLISH */
    }
}

public sealed class SceneStoryUnityDriver : MonoBehaviour
{
    int checks;
    float deadline;
    readonly List<ModStoryEvent> events=new List<ModStoryEvent>();
    void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    void Update(){if(deadline>0 && Time.realtimeSinceStartup>deadline){Debug.LogError("[SceneStoryUnity] Timeout");EditorApplication.Exit(1);}}
    IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        deadline=Time.realtimeSinceStartup+60;
        var test=Run();
        while(true)
        {
            bool more;
            try {more=test.MoveNext();}
            catch(Exception error){Debug.LogError("[SceneStoryUnity] FAIL: "+error);EditorApplication.Exit(1);yield break;}
            if(!more)break;
            yield return test.Current;
        }
        Debug.Log("[SceneStoryUnity] PASS: "+checks+" real Unity play-mode scene/coroutine lifecycle checks.");
        EditorApplication.Exit(0);
    }
    GameObject Owner(Scene scene)
    {
        var owner=new GameObject("Destination owner");
        SceneManager.MoveGameObjectToScene(owner,scene);
        return owner;
    }
    void Schedule(GameObject owner)=>ModSceneEntry.Schedule(owner.transform,ScreenType.ModuleMap,ModRuntime.StoryEvents.ProfileGeneration);
    IEnumerator Run()
    {
        Check(Application.isPlaying,"Not in play mode");
        var bus=ModRuntime.StoryEvents;
        var scope=bus.CreateScope(ModId.Parse("example.scene-unity"));
        scope.Subscribe(ModStoryEventKind.SceneEnter,events.Add);bus.BindProfile();
        var baseline=SceneManager.GetActiveScene();
        var destination=SceneManager.CreateScene("Destination");SceneManager.SetActiveScene(destination);
        var owner=Owner(destination);Schedule(owner);
        Check(events.Count==0&&owner.GetComponent<ModSceneEntry>()!=null,"Synchronous delivery or missing helper");
        yield return null;yield return null;yield return null;yield return null;
        Check(events.Count==1&&events[0].Scene=="map","Entry not delivered once after frames");
        Check(owner.GetComponent<ModSceneEntry>()==null,"Completed helper retained");
        yield return null;Check(events.Count==1,"Entry repeated");

        SceneManager.SetActiveScene(baseline);
        yield return SceneManager.UnloadSceneAsync(destination);
        Check(owner==null,"Actual unload did not destroy owner");
        destination=SceneManager.CreateScene("Abandoned");SceneManager.SetActiveScene(destination);
        owner=Owner(destination);Schedule(owner);
        SceneManager.SetActiveScene(baseline);
        yield return SceneManager.UnloadSceneAsync(destination);
        yield return null;yield return null;
        Check(events.Count==1&&owner==null,"Unloaded pending scene emitted entry");

        destination=SceneManager.CreateScene("Returning");SceneManager.SetActiveScene(destination);
        owner=Owner(destination);Schedule(owner);bus.BindProfile();
        yield return null;yield return null;yield return null;yield return null;
        Check(events.Count==1&&owner.GetComponent<ModSceneEntry>()==null,"Stale profile delivered or leaked helper");
        Schedule(owner);Module.Instance.Requested=ScreenType.ModuleShop;
        yield return null;yield return null;yield return null;yield return null;
        Check(events.Count==1,"Superseded module delivered");Module.Instance.Requested=ScreenType.ModuleMap;
        Schedule(owner);owner.SetActive(false);
        yield return null;yield return null;yield return null;
        Check(events.Count==1,"Disabled owner delivered");
        owner.SetActive(true);
        yield return null;yield return null;yield return null;yield return null;
        Check(events.Count==1&&owner.GetComponent<ModSceneEntry>()==null,"Reactivated owner revived pending entry");
        Schedule(owner);owner.GetComponent<ModSceneEntry>().enabled=false;
        yield return null;yield return null;
        Check(events.Count==1&&owner.GetComponent<ModSceneEntry>()==null,"Disabled helper retained pending entry");
        owner.SetActive(false);Schedule(owner);
        Check(owner.GetComponent<ModSceneEntry>()==null,"Initially inactive owner scheduled work");
        Destroy(owner);yield return null;
        owner=Owner(destination);Schedule(owner);
        yield return null;yield return null;yield return null;yield return null;
        Check(events.Count==2,"New destination owner failed to deliver");
        scope.Dispose();Schedule(owner);
        Check(owner.GetComponent<ModSceneEntry>()==null,"Unobserved entry scheduled work");
        SceneManager.SetActiveScene(baseline);yield return SceneManager.UnloadSceneAsync(destination);
        bus.Clear();
    }
}
#endif

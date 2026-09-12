using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Eclipse.Modding;
public static class Trace {public static List<string> Steps=new List<string>();}
namespace UnityEngine {
 public class Component {public GameObject gameObject;}
 public class MonoBehaviour:Component {public bool Destroyed;public static void Destroy(MonoBehaviour obj){obj.Destroyed=true;}}
 public class GameObject {
  public bool activeInHierarchy=true;public int scene=4;public ModSceneEntry Entry;
  public T AddComponent<T>() where T:Component,new(){var value=new T{gameObject=this};Entry=value as ModSceneEntry;Trace.Steps.Add("schedule");return value;}
 }
}
namespace UnityEngine.SceneManagement {public static class SceneManager {public static int Active=4;public static int GetActiveScene()=>Active;}}
public static class SceneManagerSF {
 public static bool Allowed=true;
 public static bool Init(ScreenType type)=>Allowed;
 public static void DJKMOGJMHLO(ScreenType type){Trace.Steps.Add("current");}
}
public class Module {
 public class Info {public object Data;}
 public Info DMCJGOMOJEF=new Info();public ScreenType Requested=ScreenType.ModuleMap;
 public static Module Value=new Module();public static Module ELEBLBJKDBI()=>Value;
 public ScreenType NMCNDOPKFJD()=>Requested;
 public void NFEBHLDPHHI(object scene){Trace.Steps.Add("module");}
}
public class OwnerBase:UnityEngine.Component {protected virtual void Awake(){Trace.Steps.Add("base");}}
public class SceneFixture<T>:OwnerBase where T:SceneFixture<T> {
 public class Layout {public void Run(){Trace.Steps.Add("layout");}}
 public Layout _WideScreenController=new Layout();public ScreenType Screen=ScreenType.ModuleMap;public bool Fail;
 public ScreenType get_SceneId()=>Screen;
 public void GAKMJOBBBAD(T value){}
 public void GIHJGHJJJGK(){Trace.Steps.Add("debug");}
 public void Init(object data){Trace.Steps.Add("init");if(Fail)throw new InvalidOperationException("init");}
 public void Run()=>Awake();
 /* AWAKE */
}
public class Destination:SceneFixture<Destination>{}
namespace Eclipse.Modding {
 public static class ModRuntime {
  public static object _profileRoster=new object();
  public static ModStoryEvents StoryEvents=new ModStoryEvents();
  /* PUBLISH */
 }
}
static class Program {
 static int checks;
 static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static IEnumerator Begin(ModSceneEntry entry)=>(IEnumerator)typeof(ModSceneEntry).GetMethod("Start",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(entry,null);
 static Destination Create(ScreenType screen=ScreenType.ModuleMap){
  Trace.Steps.Clear();var d=new Destination{gameObject=new UnityEngine.GameObject(),Screen=screen};d.Run();return d;
 }
 static void Main(){
  var bus=ModRuntime.StoryEvents;var events=new List<ModStoryEvent>();
  var scope=bus.CreateScope(ModId.Parse("example.scene"));scope.Subscribe(ModStoryEventKind.SceneEnter,events.Add);bus.BindProfile();
  var d=Create();Check(string.Join(",",Trace.Steps)=="current,debug,base,init,module,layout,schedule","native initialization order");
  var entry=d.gameObject.Entry;var routine=Begin(entry);
  Check(routine.MoveNext()&&events.Count==0,"callback during scheduling/first yield");
  Check(!routine.MoveNext()&&events.Count==1&&events[0].Scene=="map"&&entry.Destroyed,"deferred entry and component cleanup");
  Check(!routine.MoveNext()&&events.Count==1,"duplicate callback");
  foreach(var screen in new[]{ScreenType.Loader,ScreenType.ModulePreloader,ScreenType.ModuleCreditsScreen,ScreenType.ModuleNone})
   Check(Create(screen).gameObject.Entry==null,"unsupported scene scheduled");
  foreach(var screen in new[]{ScreenType.ModuleShop,ScreenType.ModuleProfile,ScreenType.ModuleDojo,ScreenType.ModuleFight}){
   d=Create(screen);Module.Value.Requested=screen;routine=Begin(d.gameObject.Entry);routine.MoveNext();routine.MoveNext();
   Check(events[events.Count-1].Scene==ModSceneEntry.Name(screen),"destination name");
  }
  Module.Value.Requested=ScreenType.ModuleMap;int before=events.Count;
  d=Create();routine=Begin(d.gameObject.Entry);routine.MoveNext();Module.Value.Requested=ScreenType.ModuleShop;routine.MoveNext();
  Check(events.Count==before&&d.gameObject.Entry.Destroyed,"superseded transition delivered");
  Module.Value.Requested=ScreenType.ModuleMap;
  d=Create();routine=Begin(d.gameObject.Entry);routine.MoveNext();bus.BindProfile();routine.MoveNext();
  Check(events.Count==before,"old profile coroutine delivered");
  d=Create();routine=Begin(d.gameObject.Entry);routine.MoveNext();UnityEngine.SceneManagement.SceneManager.Active=99;routine.MoveNext();
  Check(events.Count==before,"inactive destination scene delivered");UnityEngine.SceneManagement.SceneManager.Active=4;
  d=Create();routine=Begin(d.gameObject.Entry);routine.MoveNext();d.gameObject.activeInHierarchy=false;routine.MoveNext();
  Check(events.Count==before,"inactive destination object delivered");
  d=new Destination{gameObject=new UnityEngine.GameObject(),Fail=true};bool failed=false;try{d.Run();}catch(InvalidOperationException){failed=true;}
  Check(failed&&d.gameObject.Entry==null,"failed native initialization scheduled event");
  SceneManagerSF.Allowed=false;Check(Create().gameObject.Entry==null,"rejected initialization scheduled event");SceneManagerSF.Allowed=true;
  bus.UnbindProfile();Check(Create().gameObject.Entry==null,"unbound profile scheduled event");
  bus.BindProfile();scope.Dispose();Check(Create().gameObject.Entry==null,"unobserved scene scheduled work");
  Console.WriteLine("PASS: "+checks+" production scene hook/coroutine checks with controlled Unity lifecycle. Real Unity scene unloading is a separate acceptance test.");
 }
}

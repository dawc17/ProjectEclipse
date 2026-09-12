using System;
using Eclipse.Modding;
public enum SliderType {None}
public class QuestParameters {public string GAEPENBCCPB,BPPAPLLPBIJ="Map",GMDFCHJBJGO;public object OIKHBNOANPP;}
public class ListSF {public static readonly ListSF Value=new ListSF();public QuestParameters Parameters=new QuestParameters();public static ListSF ELEBLBJKDBI()=>Value;public QuestParameters BNMLDPNCMLB()=>Parameters;}
public static class GameUtils {
 public static bool SceneGate,TabGate,Throw;public static int Gates;
 public static SliderType NAMBCLFLNIN(object value)=>SliderType.None;
 public static bool OIGPBEKELCP(ScreenType value){Gates++;if(Throw)throw new Exception("native failure");return SceneGate;}
 public static bool MKADBAEEMFA(SliderType a,SliderType b){Gates++;return TabGate;}
}
public static class MenuController {public static void BGFJOFOLGDH(bool value){}public static void BEMOBLOBCHN(){} }
public class Module {
 public class ScreenInfo {public ScreenType ScreenType=ScreenType.ModuleMap,HKJFKDEEIDJ;public object Data;public Action<object> Dlg;}
 public static Module Value=new Module();public ScreenInfo DMCJGOMOJEF=new ScreenInfo();public object Holder=new object();
 public int Transitions;public Action OnTransition;
 public static Module ELEBLBJKDBI()=>Value;
 public object BOHBCFMJPCA()=>Holder;
 public ScreenType NMCNDOPKFJD()=>DMCJGOMOJEF.ScreenType;
 public static string INIOOEKJIDI(ScreenType type)=>type.ToString();
 public static SliderType PDLBAGNMFIN(ScreenType type,object data)=>SliderType.None;
 public void OAAFAINKKMI(){Transitions++;OnTransition?.Invoke();}
 public void CallEvent(int id,object data){}
 /* NATIVE */
}
public static class SceneManagerSF {public static ScreenType Current=ScreenType.ModuleMap;public static ScreenType EKFBDMBCDMB()=>Current;}
namespace UnityEngine.SceneManagement {public struct Scene {public int buildIndex;}public static class SceneManager {public static int Index=4;public static Scene GetActiveScene()=>new Scene{buildIndex=Index};}}
namespace Nekki.SF2.GUI {public class LockScreen {public class Object {public bool activeInHierarchy;}public Object gameObject=new Object();public static LockScreen Value;public static LockScreen get_Instance()=>Value;}}
namespace Eclipse.UI.Modding {public static class ModUiGameBridge {public static bool NativeInputBlocked;}}
namespace Eclipse.Modding {
 public class ModContentException:Exception {public ModContentException(string text):base(text){}}
 public static class ModModeRuntime {public static bool HasPendingPreparation;}
 public static class ModRuntime {
  public static object _profileRoster=new object();private static bool _sceneNavigationInProgress;
  /* HOST */
 }
}
static class Program {
 static int checks;
 static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static void Reset(){
  ModRuntime._profileRoster=new object();Module.Value=new Module();SceneManagerSF.Current=ScreenType.ModuleMap;
  UnityEngine.SceneManagement.SceneManager.Index=4;GameUtils.SceneGate=GameUtils.TabGate=GameUtils.Throw=false;GameUtils.Gates=0;
  ModModeRuntime.HasPendingPreparation=false;Eclipse.UI.Modding.ModUiGameBridge.NativeInputBlocked=false;Nekki.SF2.GUI.LockScreen.Value=null;
 }
 static void Reject(Action setup,string name){Reset();setup();Check(!ModRuntime.TryNavigateScene("shop")&&Module.Value.Transitions==0&&GameUtils.Gates==0,name);}
 static void Main(){
  Reject(()=>ModRuntime._profileRoster=null,"no profile");
  Reject(()=>ModModeRuntime.HasPendingPreparation=true,"pending encounter");
  Reject(()=>Eclipse.UI.Modding.ModUiGameBridge.NativeInputBlocked=true,"native input block");
  Reject(()=>Nekki.SF2.GUI.LockScreen.Value=new Nekki.SF2.GUI.LockScreen{gameObject=new Nekki.SF2.GUI.LockScreen.Object{activeInHierarchy=true}},"native lock screen");
  Reject(()=>Module.Value.Holder=null,"scene not initialized");
  Reject(()=>Module.Value.DMCJGOMOJEF.ScreenType=ScreenType.ModuleShop,"transition already requested");
  Reject(()=>UnityEngine.SceneManagement.SceneManager.Index=1,"active loader scene");
  foreach(var screen in new[]{ScreenType.ModuleFight,ScreenType.Loader,ScreenType.ModulePreloader,ScreenType.ModuleCreditsScreen,ScreenType.ModuleNone})
   Reject(()=>{SceneManagerSF.Current=screen;Module.Value.DMCJGOMOJEF.ScreenType=screen;UnityEngine.SceneManagement.SceneManager.Index=(int)screen;},"unsupported source "+screen);
  foreach(var destination in new[]{"map","shop","profile","dojo"}){
   Reset();bool same=destination=="map";
   Check(ModRuntime.TryNavigateScene(destination),"valid destination "+destination);
   Check(Module.Value.Transitions==(same?0:1)&&GameUtils.Gates==(same?0:2),"preserved native transition and both gates "+destination);
  }
  foreach(var source in new[]{ScreenType.ModuleShop,ScreenType.ModuleProfile,ScreenType.ModuleDojo}){
   Reset();SceneManagerSF.Current=source;Module.Value.DMCJGOMOJEF.ScreenType=source;UnityEngine.SceneManagement.SceneManager.Index=(int)source;
   Check(ModRuntime.TryNavigateScene("map")&&Module.Value.Transitions==1,"valid source "+source);
  }
  Reset();GameUtils.SceneGate=true;
  Check(!ModRuntime.TryNavigateScene("shop")&&Module.Value.Transitions==0&&GameUtils.Gates==1,"scene quest gate bypassed");
  Reset();GameUtils.TabGate=true;
  Check(!ModRuntime.TryNavigateScene("shop")&&Module.Value.Transitions==0&&GameUtils.Gates==2,"tab quest gate bypassed");
  Reset();bool nested=true;Module.Value.OnTransition=()=>nested=ModRuntime.TryNavigateScene("profile");
  Check(ModRuntime.TryNavigateScene("shop")&&!nested&&Module.Value.Transitions==1,"nested navigation not rejected");
  Reset();GameUtils.Throw=true;bool threw=false;try{ModRuntime.TryNavigateScene("shop");}catch(Exception){threw=true;}
  GameUtils.Throw=false;Check(threw&&ModRuntime.TryNavigateScene("shop"),"exception retained reentry lock or was masked");
  foreach(var invalid in new[]{"fight","loader","credits","Map","",null}){
   Reset();bool rejected=false;try{ModRuntime.TryNavigateScene(invalid);}catch(ModContentException){rejected=true;}
   Check(rejected&&GameUtils.Gates==0,"invalid destination reached native code");
  }
  Console.WriteLine("PASS: "+checks+" production navigation gate and native transition checks with controlled scene/quest services. No Lua or real scene load claimed.");
 }
}

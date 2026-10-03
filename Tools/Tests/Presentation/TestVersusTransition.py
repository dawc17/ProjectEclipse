"""Check production multiplayer/native presentation ownership with controlled stubs.

Requires Python and .NET 10. This extracts the actual native setup methods and
checks their timing/artwork decisions; it does not render or playtest Unity.
"""
from pathlib import Path
import re, subprocess

root = Path(__file__).resolve().parents[3]
folder = root / 'Temp/VersusPresentationCheck'
folder.mkdir(parents=True, exist_ok=True)
def method(path, signature):
    text = (root / path).read_text(encoding='utf-8-sig')
    found = re.search(r'^\t\t' + signature + r'.*?^\t\t}', text, re.M | re.S)
    if not found: raise RuntimeError(signature)
    return found.group()
screen = 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/Fight/ScreenFight.cs'
loader = 'Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/Scenes/LoaderScene.cs'
code = '''
using System;
using System.Collections.Generic;
using Object = FakeObject;
class Transform { public void SetParent(Transform parent,bool world) {} }
class GameObject { public bool activeSelf=true; public Transform transform=new(); public VsScreen component; public void SetActive(bool v)=>activeSelf=v; public T GetComponent<T>() where T:class=>component as T; }
class FakeObject { public static GameObject Instantiate(GameObject prefab) { var result=new GameObject(); result.component=new VsScreen { gameObject=result }; return result; } }
class VsScreen { public GameObject gameObject; public Transform transform=>gameObject.transform; public ModelParameters Left,Right; float duration; public void Init(ModelParameters left,ModelParameters right) { Left=left;Right=right;duration=2.4f; } public float get_AnimationTime()=>duration; }
class ModelParameters {}
class FightList { public int RoundsToWin=2; public int PNHLGCBPFIG()=>2; public string GJOAJAIJHOE()=>""; }
namespace Eclipse.Multiplayer {
 enum VersusMode { Local,Online,Replay,Training,Spectator }
 class LocalVersusSettings { public VersusMode Mode; }
 class LocalVersusMatch : FightList { public LocalVersusSettings Settings=new(); }
 static class LocalVersusMenu { public static bool VersusSplashVisible; }
}
namespace Eclipse.UI { static class LoaderArt { public static int Calls; public static void ApplyMenuSplash(GameObject target,ScreenType before,ScreenType next)=>Calls++; } }
enum ScreenFightType { TYPE_INFO_VS }
enum ScreenType { ModuleNone,ModulePreloader,ModuleDojo,ModuleFight }
static class AtlasCache { public static int Clears; public static void Clear()=>Clears++; }
class Base { public GameObject gameObject=new(); public Transform transform=>gameObject.transform; public int Coroutines; protected virtual void Init(object data) {} protected void StartCoroutine(object task)=>Coroutines++; }
class Screen : Base {
 struct Data { public ModelParameters KMMJCHDKBDO; public List<ModelParameters> BDANFHBMIOF; public int BLOOLFFMKFI; }
 Data BBFBFDDDJMJ; public ScreenFightType Type; GameObject vsScreenPrefab=new(); public VsScreen vsScreen;
 bool customVersusIntro; int maxRounds; string ruleDesc; public float Timer; public bool Paused;
 public void set_Pause(bool v)=>Paused=v; void ClearPictures() {} void StartScreen(float t)=>Timer=t;
 public void Run(FightList definition,ModelParameters left,ModelParameters right) { PreInit(definition); BBFBFDDDJMJ=new Data { KMMJCHDKBDO=left,BDANFHBMIOF=new(){right} }; StartVS(); }
'''
code += method(screen,r'public void PreInit\(') + '\n' + method(screen,r'private void StartVS\(') + '\n}\n'
code += '''class Loader : Base {
 ScreenType LMGJJNACLFG, previous; public GameObject _LoaderType1=new(), _LoaderType2=new();
 ScreenType get_PrevScene()=>previous; object OBHAPHKNGFE()=>new object();
 public void Run(ScreenType prev,ScreenType next) { previous=prev;LMGJJNACLFG=next;Init(null); }
'''
code += method(loader,r'protected override void Init\(') + '\n}\n'
code += '''class Program {
 static int checks; static void Check(bool result,string reason) { checks++;if(!result)throw new Exception(reason); }
 static void Main() {
  var left=new ModelParameters(); var right=new ModelParameters(); float campaignTimer=0;
  foreach (var mode in new[]{-1,0,1,2,3,4}) {
   FightList definition=mode<0?new FightList():new Eclipse.Multiplayer.LocalVersusMatch { Settings=new(){ Mode=(Eclipse.Multiplayer.VersusMode)mode } };
   var screen=new Screen();screen.Run(definition,left,right);
   if(mode<0)campaignTimer=screen.Timer;
   Check(screen.vsScreen.gameObject.activeSelf!=(mode==0||mode==1),"Native artwork ownership, mode "+mode);
   Check(screen.Timer==campaignTimer&&screen.Timer>0,"VS completion timing changed, mode "+mode);
   Check(screen.Type==ScreenFightType.TYPE_INFO_VS,"Native stop event type, mode "+mode);
   Check(screen.vsScreen.Left==left&&screen.vsScreen.Right==right,"Native presentation initialization, mode "+mode);
  }
  foreach(var previous in new[]{ScreenType.ModulePreloader,ScreenType.ModuleDojo})
   foreach(var next in new[]{ScreenType.ModuleFight,ScreenType.ModuleDojo})
    foreach(var splash in new[]{false,true}) {
     Eclipse.Multiplayer.LocalVersusMenu.VersusSplashVisible=splash;
     int clears=AtlasCache.Clears; var loader=new Loader();loader.Run(previous,next);
     bool owned=splash&&next==ScreenType.ModuleFight;
     Check(loader._LoaderType1.activeSelf==(!owned&&previous==ScreenType.ModulePreloader),"Initial loading artwork ownership");
     Check(loader._LoaderType2.activeSelf==(!owned&&previous!=ScreenType.ModulePreloader),"Menu loading artwork ownership");
     Check(AtlasCache.Clears==clears+1&&loader.Coroutines==1,"Native loading/cache flow changed");
    }
  Console.WriteLine("PASS: "+checks+" production presentation checks (controlled Unity dependencies; no game playtest).");
 }
}
'''
(folder / 'Program.cs').write_text(code,encoding='utf-8')
(folder / 'Check.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup></Project>',encoding='utf-8')
subprocess.run(['dotnet','run','--project',str(folder / 'Check.csproj'),'--verbosity','quiet'],cwd=root,check=True)

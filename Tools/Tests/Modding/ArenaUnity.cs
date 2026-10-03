#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using Eclipse.Modding;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class ArenaUnity
{
    const string Active="Eclipse.ArenaUnity.Active";
    const BindingFlags Hidden=BindingFlags.Instance|BindingFlags.NonPublic;
    static double started,lastReport,phaseAt; static bool campaign,entered;
    static int checks,phase,pausedFrame; static ModUiSurface surface; static string combatException;
    static ModArenaRect region; static float health; static IModFighterRegions sensor; static IModArenaMarker orphan; static ModArenaMarkerScope scope;
    static ArenaUnity(){if(SessionState.GetBool(Active,false)){started=EditorApplication.timeSinceStartup;EditorApplication.update+=Update;Application.logMessageReceived+=Log;}}
    public static void Run()
    {
        var root=Path.GetDirectoryName(Application.dataPath);
        if(!File.Exists(Path.Combine(root,"arena-fixture.marker")))throw new Exception("Requires isolated audio fixture.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT",Path.Combine(root,"Mods"));
        var arguments=Environment.GetCommandLineArgs();int productIndex=Array.IndexOf(arguments,"-arenaAcceptanceProductName");
        if(productIndex<0||productIndex+1>=arguments.Length||!System.Text.RegularExpressions.Regex.IsMatch(arguments[productIndex+1],"^ArenaUnity-[0-9a-f]{32}$"))throw new Exception("Pass the generated acceptance product through the runner.");
        PlayerSettings.companyName="EclipseAcceptance";PlayerSettings.productName=arguments[productIndex+1];
        SessionState.SetBool(Active,true);EditorSceneManager.OpenScene("Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        var view=EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));view.Show();view.Focus();EditorApplication.EnterPlaymode();
    }
    static object Field(object value,string name)=>value.GetType().GetField(name,Hidden|BindingFlags.Public).GetValue(value);
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static UnityEngine.Renderer[] Markers()=>UnityEngine.Object.FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None).Where(r=>r.gameObject.name=="Eclipse arena marker").ToArray();
    static bool Inside(ModArenaRect rect){Check(sensor.TryOverlapRect(rect,out var hit,out var error),error);return hit;}
    static void Next(){phase++;phaseAt=EditorApplication.timeSinceStartup;}
    static void Update()
    {
        if(!EditorApplication.isPlaying)return;
        try{
            if(combatException!=null)throw new Exception(combatException);
            if(EditorApplication.timeSinceStartup-started>300)throw new Exception("Timed out phase "+phase);
            if(EditorApplication.timeSinceStartup-lastReport>20){lastReport=EditorApplication.timeSinceStartup;Debug.Log("[ArenaUnity] Waiting entered="+entered+" phase="+phase);}
            if(!campaign&&Eclipse.UI.TitleScreen.IsOpen){
                var title=UnityEngine.Object.FindFirstObjectByType<Eclipse.UI.TitleScreen>();
                if((bool)Field(title,"splashing")||(string)Field(title,"currentPage")!="Home")return;
                typeof(Eclipse.UI.TitleScreen).GetMethod("BeginCampaign",Hidden).Invoke(title,null);
                var directory=SF2Paths.GetUserDataDirectory();
                Check(Eclipse.Saves.CampaignSaveSession.PreviewDirectory==null&&directory.StartsWith(Application.persistentDataPath,StringComparison.OrdinalIgnoreCase)&&Application.persistentDataPath.Contains("ArenaUnity-"),"Profile not isolated");
                var profile=XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(),"usersDefault.xml",XmlUtils.EBLFEPIOMOL.Normal,true,XmlCryptoUtils.NNLGALNDJCL());
                ((System.Xml.XmlElement)profile.SelectSingleNode("/Root/Warriors/Warrior[@ID='1']")).SetAttribute("Tutorial","END");
                Directory.CreateDirectory(directory);XmlUtils.ONLDJNLKKAL(profile,Path.Combine(directory,Constants.OJMIJINKBPJ).Replace('\\','/'));
                campaign=true;return;
            }
            if(!entered){
                if(ModRuntime.Scripts==null||Module.GetInstance()==null)return;
                var screen=Module.GetInstance().GetCurrentScreenType();if(screen!=ScreenType.ModuleDojo&&screen!=ScreenType.ModuleMap)return;
                Check(!ModRuntime.Host.HasErrors,ModRuntime.Host.FormatReport());Check(ModRuntime.Host.EnabledMods.Any(m=>m.Id.Value=="example.pulse-arena"),"Pulse Arena not enabled");Check(!ModRuntime.Scripts.HasErrors,"Startup mod errors");
                var encounter=ListSF.CHMCKGCDGCM(new FightIDS(ModRuntime.Scripts.Content.RuntimeFightId(DefinitionId.Parse("core:fights/zone_1/tournament/3"))));
                Check(encounter!=null,"Core encounter missing");entered=GameUtils.StartFight(encounter,false,null,true,false);return;
            }
            var fight=Fight.GetCurrentFight();if(fight==null||fight.get_FightTimeInFrames()<10)return;
            var player=fight.GetPlayerModel();var enemy=fight.GetEnemyModel();if(player==null||enemy==null)return;
            player.Parameters.UserControlled=false;enemy.Parameters.AiControlled=false;player.Parameters.set_IsImmortalityEnabled(false);
            double elapsed=EditorApplication.timeSinceStartup-phaseAt;

            switch(phase){
            case 0:
                ModRuntime.Scripts.CallbackDiagnostics.Recording=true;surface=Surface();if(surface==null||Markers().Length!=1)return;
                Check(surface.Read("status").Text=="Warning: leave the column","Shipped warning schedule/HUD");
                var bounds=Markers()[0].GetComponent<MeshFilter>().sharedMesh.vertices;
                region=new ModArenaRect(bounds[0].x,bounds[0].y,bounds[2].x-bounds[0].x,bounds[2].y-bounds[0].y);
                var type=typeof(Fight).GetNestedType("EclipseFighterOperations",BindingFlags.NonPublic);
                sensor=(IModFighterRegions)Activator.CreateInstance(type,new object[]{fight,player,null,null,null,null,false});
                var pos=player.PLBNCDCFPML();player.ShiftModelPosition(new Vector3f((float)(region.X+80)-pos.GetX(),0,0),true);
                var other=enemy.PLBNCDCFPML();enemy.ShiftModelPosition(new Vector3f((float)(region.X+region.Width+350)-other.GetX(),0,0),true);
                Check(Inside(region),"Actual native player capsules did not overlap column");
                Check(!Inside(new ModArenaRect(8000,8000,20,20)),"Far empty region overlapped");
                Check(!Inside(new ModArenaRect(-50,-10000,20,20)),"Wrong Y region overlapped");
                Check(Markers()[0].sharedMaterial.color.g>.7f&&Markers()[0].sharedMaterial.color.a<.5f,"Warning material");
                var transform=Markers()[0].transform;Check(transform.parent==player.GetRenderObject().transform.parent,"Marker not arena-owned");
                Check(Vector3.Distance(transform.TransformPoint(new Vector3((float)region.X,(float)region.Y,-.25f)),player.GetRenderObject().transform.TransformPoint(new Vector3((float)region.X,(float)region.Y,-.25f)))<.001f,"Marker/native coordinate projection mismatch");
                health=player.KKMCHCNOHMB();new GameObject("Pulse capture").AddComponent<ArenaCapture>();Next();break;
            case 1:
                if(surface.Read("status").Text!="Pulse active")return;
                if(surface.Read("contacts").Text=="Contacts: 0")return;
                Check(Markers().Length==1&&Markers()[0].sharedMaterial.color.g<.3f,"Shipped active warning recolor");
                Check(player.KKMCHCNOHMB()<health,"Actual Lua hazard failed native health loss");
                Check(surface.Read("contacts").Text!="Contacts: 0","Actual Lua capsule sensor/contact counter: "+surface.Read("contacts").Text+" failures="+string.Join(";",ModRuntime.Scripts.CallbackDiagnostics.RecentFailures.Select(f=>f.ToString())));
                fight.SetPaused(true);health=player.KKMCHCNOHMB();pausedFrame=fight.get_FightTimeInFrames();Next();break;
            case 2:
                if(elapsed<.5)return;
                Check(fight.get_FightTimeInFrames()==pausedFrame,"Paused simulation advanced hazard clock");
                Check(Math.Abs(player.KKMCHCNOHMB()-health)<.00001,"Paused hazard damaged fighter");
                Check(Markers().Length==1&&Markers()[0].sharedMaterial.color.g<.3f,"Pause lost current marker");
                fight.SetPaused(false);Next();break;
            case 3:
                if(surface.Read("status").Text!="Safe: pulse recovering")return;
                Check(player.KKMCHCNOHMB()<health,"Resumed hazard failed subsequent pulse");
                Check(Markers().Length==0,"Recovery retained native marker");health=player.KKMCHCNOHMB();Next();break;
            case 4:
                if(surface.Read("status").Text!="Warning: leave the column")return;
                Check(Math.Abs(player.KKMCHCNOHMB()-health)<.00001,"Safe phase inflicted health loss");Check(Markers().Length==1,"Next cycle did not create owned marker");
                // Move out of the column. Geometric sampling must follow actual pose.
                var here=player.PLBNCDCFPML();player.ShiftModelPosition(new Vector3f((float)(region.X-350)-here.GetX(),0,0),true);
                Check(!Inside(region),"Displaced native rig still overlaps fixed sensor");
                scope=new ModArenaMarkerScope();Check(scope.TryCreate(sensor,region,new ModUiColor("#33ccff66"),out var owned,out var failure),failure);
                Check(owned.IsActive,"Owned native marker inactive");scope.Dispose();Check(!owned.IsActive,"Scope disposal retained marker");
                var pools=new ModArenaMarkerScope[4];for(int i=0;i<4;i++)pools[i]=new ModArenaMarkerScope();
                for(int i=0;i<63;i++)Check(pools[i/16].TryCreate(sensor,region,new ModUiColor("#ffffff11"),out _,out failure),"Shared native allocation: "+failure);
                Check(!pools[3].TryCreate(sensor,region,new ModUiColor("#ffffff11"),out _,out failure)&&failure.Contains("64"),"Shared 64-marker bound absent");
                Check(Markers().Length==64,"Budget evicted another owner");foreach(var pool in pools)pool.Dispose();Check(Markers().Length==1,"Pool disposal retained native markers");
                // Unclaimed native marker proves automatic cleanup independently of
                // the example's explicit Lua on_round_end remove.
                Check(sensor.TryMarkRect(region,new ModUiColor("#33ccff66"),out orphan,out failure),failure);
                typeof(Fight).GetMethod("HCNDAFDHACI",Hidden).Invoke(fight,new object[]{GameOverTypes.GAME_OVER_SURRENDER});
                Check(surface.IsClosed,"Surrender left shipped HUD");Check(!orphan.IsActive,"Surrender left unclaimed native marker");
                Check(!sensor.TryOverlapRect(region,out _,out _),"Ended round permitted native sensor");
                long before=ModRuntime.Scripts.CallbackDiagnostics.TimedCalls;
                typeof(Fight).GetMethod("DispatchEclipseCombatEvent",Hidden).Invoke(fight,new object[]{ModEffectEvent.Tick,null,null,null,null});
                typeof(Fight).GetMethod("DispatchEclipseOpponent",Hidden).Invoke(fight,new object[]{ModEffectEvent.Tick,null,null,null,null});
                Check(ModRuntime.Scripts.CallbackDiagnostics.TimedCalls==before,"Ended round dispatched script ticks");Next();break;
            case 5:
                if(elapsed<.2)return;Check(Markers().Length==0,"Surrender left native marker objects");
                Check(ModRuntime.Scripts.CallbackDiagnostics.RecentFailures.Count==0,"Shipped callback failures: "+string.Join(";",ModRuntime.Scripts.CallbackDiagnostics.RecentFailures.Select(f=>f.Error)));
                Check(File.Exists(Path.Combine(Path.GetDirectoryName(Application.dataPath),"pulse-arena-native.png")),"Native rendered capture missing");
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath),"arena-result.txt"),"PASS: "+checks+" native full-game arena checks; shipped Lua warning/active/recovery clock, native rectangle projection/recolor and capsule sensor, direct health loss, pose displacement, pause/resume, recurring schedule, 64-marker shared bound, script-scope and surrender cleanup, post-end tick suppression. Controls/profile isolated; no swept/solid physics or hit-reaction claim.");
                Debug.Log("[ArenaUnity] PASS: "+checks+" full-game checks");Finish(0);break;
            }
        }catch(Exception error){Debug.LogError("[ArenaUnity] FAIL: "+error);Finish(1);}
    }
    static ModUiSurface Surface(){foreach(var context in (IEnumerable)Field(ModRuntime.Scripts,"_contexts")){var scope=context.GetType().GetProperty("UiScope").GetValue(context) as ModUiScope;if(scope==null||scope.Owner.Value!="example.pulse-arena")continue;foreach(ModUiSurface s in ((IDictionary)Field(scope,"surfaces")).Values)if(s.Id=="pulse")return s;}return null;}
    static void Log(string message,string stack,LogType type){if(entered&&type==LogType.Exception&&(stack.Contains("Fight.")||stack.Contains("Model.")||stack.Contains("Modding")))combatException=message+"\n"+stack;}
    static void Finish(int code){SessionState.SetBool(Active,false);EditorApplication.update-=Update;Application.logMessageReceived-=Log;EditorApplication.Exit(code);}
}
public sealed class ArenaCapture : MonoBehaviour
{
    IEnumerator Start(){yield return new WaitForEndOfFrame();var texture=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Application.dataPath),"pulse-arena-native.png"),texture.EncodeToPNG());UnityEngine.Object.Destroy(texture);UnityEngine.Object.Destroy(gameObject);}
}
#endif

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
public static class FighterPlaybackUnity
{
    const string Active="Eclipse.FighterPlaybackUnity.Active";
    const BindingFlags Hidden=BindingFlags.Instance|BindingFlags.NonPublic;
    const string Move="example.active-strike:moves/active_strike";
    static double started,lastReport;
    static bool campaign,entered,clicked,played,hit,cooldownStarted,recharged;
    static int checks,castFrame,starts; static double healthBefore;
    static string combatException;
    static FighterPlaybackUnity(){if(SessionState.GetBool(Active,false)){started=EditorApplication.timeSinceStartup;EditorApplication.update+=Update;Application.logMessageReceived+=Log;}}
    public static void Run()
    {
        var root=Path.GetDirectoryName(Application.dataPath);
        if(!File.Exists(Path.Combine(root,"fighter-playback-fixture.marker")))throw new Exception("Requires isolated full-game fixture.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT",Path.Combine(root,"Mods"));
        PlayerSettings.companyName="EclipseAcceptance";PlayerSettings.productName=Path.GetFileName(root)+"-"+Guid.NewGuid().ToString("N").Substring(0,8);
        SessionState.SetBool(Active,true);EditorSceneManager.OpenScene("Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        var view=EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));view.Show();view.Focus();EditorApplication.EnterPlaymode();
    }
    static object Field(object value,string name)=>value.GetType().GetField(name,Hidden|BindingFlags.Public).GetValue(value);
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static ModCombatSnapshot Snapshot(Fight fight,Model model)=>((IModCombatSnapshotSource)Activator.CreateInstance(typeof(Fight).GetNestedType("EclipseFighterOperations",BindingFlags.NonPublic),Hidden|BindingFlags.Public,null,new object[]{fight,model,null,null,null,null,false},null)).CaptureCombatSnapshot();
    static void Update()
    {
        if(!EditorApplication.isPlaying)return;
        try {
            if(combatException!=null)throw new Exception(combatException);
            if(EditorApplication.timeSinceStartup-started>300)throw new Exception("Timed out: clicked="+clicked+" played="+played+" hit="+hit+" recharged="+recharged);
            if(EditorApplication.timeSinceStartup-lastReport>20){lastReport=EditorApplication.timeSinceStartup;Debug.Log("[FighterPlaybackUnity] Waiting entered="+entered+" clicked="+clicked+" played="+played+" hit="+hit);}
            if(!campaign&&Eclipse.UI.TitleScreen.IsOpen){
                var title=UnityEngine.Object.FindFirstObjectByType<Eclipse.UI.TitleScreen>();
                if((bool)Field(title,"splashing")||(string)Field(title,"currentPage")!="Home")return;
                typeof(Eclipse.UI.TitleScreen).GetMethod("BeginCampaign",Hidden).Invoke(title,null);
                var directory=SF2Paths.GetUserDataDirectory();
                Check(Eclipse.Saves.CampaignSaveSession.PreviewDirectory==null&&directory.StartsWith(Application.persistentDataPath,StringComparison.OrdinalIgnoreCase)&&Application.persistentDataPath.Contains(Path.GetFileName(Path.GetDirectoryName(Application.dataPath))),"Profile not isolated");
                var profile=XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(),"usersDefault.xml",XmlUtils.EBLFEPIOMOL.Normal,true,XmlCryptoUtils.NNLGALNDJCL());
                ((System.Xml.XmlElement)profile.SelectSingleNode("/Root/Warriors/Warrior[@ID='1']")).SetAttribute("Tutorial","END");
                Directory.CreateDirectory(directory);XmlUtils.ONLDJNLKKAL(profile,Path.Combine(directory,Constants.OJMIJINKBPJ).Replace('\\','/'));
                campaign=true;return;
            }
            if(!entered){
                if(ModRuntime.Scripts==null||Module.GetInstance()==null)return;
                var screen=Module.GetInstance().GetCurrentScreenType();if(screen!=ScreenType.ModuleDojo&&screen!=ScreenType.ModuleMap)return;
                Check(!ModRuntime.Scripts.HasErrors,"Startup mod errors");
                var encounter=ListSF.CHMCKGCDGCM(new FightIDS(ModRuntime.Scripts.Content.RuntimeFightId(DefinitionId.Parse("core:fights/zone_1/tournament/3"))));
                Check(encounter!=null,"Core encounter missing");entered=GameUtils.StartFight(encounter,false,null,true,false);return;
            }
            var fight=Fight.GetCurrentFight();if(fight==null)return;
            var player=fight.GetPlayerModel();var enemy=fight.GetEnemyModel();if(player==null||enemy==null||fight.get_FightTimeInFrames()<1)return;
            player.Parameters.UserControlled=false;enemy.Parameters.AiControlled=false;player.Parameters.set_IsImmortalityEnabled(true);
            int frame=fight.get_FightTimeInFrames();var surface=Surface();
            if(!clicked&&frame>=100){
                Check(surface!=null&&!surface.IsClosed,"Shipped HUD missing");
                var nativeMove=player.GetAvailableAnimations().SingleOrDefault(m=>m.Name==Move);Check(nativeMove!=null,"Owned move unavailable on real model");
                var observation=Snapshot(fight,player);
                // Controlled spacing gives the authored punch a real contact opportunity.
                enemy.ShiftModelPosition(new Vector3f((float)(observation.Self.X+100-observation.Opponent.X),0,0),true);
                healthBefore=Snapshot(fight,enemy).Self.Health;
                var view=UnityEngine.Object.FindObjectsByType<Eclipse.UI.Modding.ModUiView>(FindObjectsInactive.Include,FindObjectsSortMode.None).Single(v=>ReferenceEquals(Field(v,"surface"),surface));
                var button=(Button)Field(((IDictionary)Field(view,"widgets"))["cast"],"Button");
                Check(button.gameObject.activeInHierarchy&&button.interactable,"Native button blocked");
                castFrame=frame;button.onClick.Invoke();Check(player.GetCurrentAnimation()?.Name!=Move,"Button started move recursively");clicked=true;return;
            }
            if(clicked){
                if(player.GetCurrentAnimation()?.Name==Move)played=true;
                if(Snapshot(fight,enemy).Self.Health<healthBefore)hit=true;
                if(surface.Read("status").Text.Contains("started")&&!surface.Read("cast").Enabled)cooldownStarted=true;
                if(!recharged&&surface.Read("status").Text=="Active Strike ready"&&surface.Read("cast").Enabled&&frame>castFrame){
                    Check(frame>=castFrame+121,"Cooldown recharged before applied receipt plus 120 frames");Check(cooldownStarted,"Applied receipt was never displayed with a disabled cooldown button");
                    Check(played,"Authored native animation never played");Check(starts==1,"Native start callback count: "+starts);Check(hit,"Authored attack made no native damage/contact");
                    Check(ModRuntime.Scripts.CallbackDiagnostics.RecentFailures.Count==0,"Lua callback failure history nonempty");recharged=true;
                    Debug.Log("[FighterPlaybackUnity] Authored move played; enemy health "+healthBefore+" -> "+Snapshot(fight,enemy).Self.Health+"; cooldown frames="+(frame-castFrame));
                    new GameObject("Playback capture").AddComponent<FighterPlaybackCapture>();
                }
            }
        }catch(Exception error){Debug.LogError("[FighterPlaybackUnity] FAIL: "+error);Finish(1);}
    }
    static ModUiSurface Surface()
    {
        foreach(var context in (IEnumerable)Field(ModRuntime.Scripts,"_contexts")){
            var scope=context.GetType().GetProperty("UiScope").GetValue(context) as ModUiScope;if(scope==null||scope.Owner.Value!="example.active-strike")continue;
            foreach(ModUiSurface surface in ((IDictionary)Field(scope,"surfaces")).Values)if(surface.Id=="ability")return surface;
        }return null;
    }
    public static void Captured()
    {
        try {
            var fight=Fight.GetCurrentFight();var surface=Surface();
            var ops=(IModFighterPlayback)Activator.CreateInstance(typeof(Fight).GetNestedType("EclipseFighterOperations",BindingFlags.NonPublic),Hidden|BindingFlags.Public,null,new object[]{fight,fight.GetPlayerModel(),null,null,null,null,false},null);
            bool? applied=null;Check(ops.TryPlayMove(DefinitionId.Parse(Move),(ok,error)=>applied=ok,out var failure),failure);
            fight.SetPaused(true);typeof(Fight).GetMethod("ApplyEclipseFighterPlayback",Hidden,null,Type.EmptyTypes,null).Invoke(fight,null);Check(applied==null,"Pause consumed playback");
            fight.SetPaused(false);typeof(Fight).GetMethod("HCNDAFDHACI",Hidden).Invoke(fight,new object[]{GameOverTypes.GAME_OVER_SURRENDER});
            Check(applied==false&&surface.IsClosed,"Surrender retained queued playback or HUD");
            Check(((IDictionary)Field(fight,"_eclipseFighterPlayback")).Count==0,"Surrender queue nonempty");
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath),"validation-result.txt"),"PASS: "+checks+" full-game native fighter playback checks; shipped authored punch, native button/deferred playback/start callback/contact damage/cooldown, pause retention and surrender cancellation. Idle controls/spacing and isolated post-tutorial profile controlled.");
            Debug.Log("[FighterPlaybackUnity] PASS: "+checks+" full-game checks");Finish(0);
        }catch(Exception error){Debug.LogError("[FighterPlaybackUnity] FAIL: "+error);Finish(1);}
    }
    static void Log(string message,string stack,LogType type){if(message.Contains("Active Strike animation started"))starts++;if(entered&&type==LogType.Exception&&(stack.Contains("Fight.")||stack.Contains("Model.")||stack.Contains("Modding")))combatException=message+"\n"+stack;}
    static void Finish(int code){SessionState.SetBool(Active,false);EditorApplication.update-=Update;Application.logMessageReceived-=Log;EditorApplication.Exit(code);}
}
public sealed class FighterPlaybackCapture : MonoBehaviour
{
    IEnumerator Start(){yield return new WaitForEndOfFrame();var texture=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Application.dataPath),"active-strike-native.png"),texture.EncodeToPNG());UnityEngine.Object.Destroy(texture);FighterPlaybackUnity.Captured();}
}
#endif

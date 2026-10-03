#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Eclipse.Modding;
using Eclipse.Multiplayer.Online;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class FighterMotionUnity
{
    const string Active="Eclipse.FighterMotionUnity.Active";
    const BindingFlags Hidden=BindingFlags.Instance|BindingFlags.NonPublic;
    static double started,lastReport,pauseStarted;
    static bool campaign,entered,rigChecked,paused,clicked,castVerified,recharged;
    static int checks,castFrame;
    static float beforePlayer,beforeEnemy;
    static string combatException;
    static FighterMotionUnity()
    {
        if(SessionState.GetBool(Active,false)) {
            started=EditorApplication.timeSinceStartup;
            EditorApplication.update+=Update;
            Application.logMessageReceived+=CaptureError;
        }
    }
    public static void Run()
    {
        var root=Path.GetDirectoryName(Application.dataPath);
        if(!File.Exists(Path.Combine(root,"fighter-motion-fixture.marker")))throw new Exception("Requires isolated full-game fixture.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT",Path.Combine(root,"Mods"));
        PlayerSettings.companyName="EclipseAcceptance";
        PlayerSettings.productName=Path.GetFileName(root)+"-"+Guid.NewGuid().ToString("N").Substring(0,8);
        SessionState.SetBool(Active,true);
        EditorSceneManager.OpenScene("Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        var view=EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));view.Show();view.Focus();
        EditorApplication.EnterPlaymode();
    }
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static object Field(object target,string name)=>target.GetType().GetField(name,Hidden|BindingFlags.Public).GetValue(target);
    static IModCombatSnapshotSource Observations(Fight fight,Model model)=>
        (IModCombatSnapshotSource)Activator.CreateInstance(typeof(Fight).GetNestedType("EclipseFighterOperations",BindingFlags.NonPublic),Hidden|BindingFlags.Public,null,new object[]{fight,model,null,null,null,null,false},null);
    static float X(Fight fight,Model model)=>(float)Observations(fight,model).CaptureCombatSnapshot().Self.X;
    static bool Queue(Fight fight,Model model,double x)
    {
        var args=new object[]{model,x,0d,0d,null};
        bool accepted=(bool)typeof(Fight).GetMethod("TryQueueEclipseFighterMotion",Hidden).Invoke(fight,args);
        Check(accepted,"Native queue rejected: "+args[4]);return accepted;
    }
    static void Apply(Fight fight)=>typeof(Fight).GetMethod("ApplyEclipseFighterMotion",Hidden,null,Type.EmptyTypes,null).Invoke(fight,null);
    static void Update()
    {
        if(!EditorApplication.isPlaying)return;
        try {
            if(combatException!=null)throw new Exception(combatException);
            if(EditorApplication.timeSinceStartup-started>300)throw new Exception("Timed out during "+(entered?"real fight":"boot"));
            if(EditorApplication.timeSinceStartup-lastReport>15) {
                lastReport=EditorApplication.timeSinceStartup;
                Debug.Log("[FighterMotionUnity] Waiting campaign="+campaign+" entered="+entered+" rig="+rigChecked+" click="+clicked);
            }
            if(!campaign&&Eclipse.UI.TitleScreen.IsOpen) {
                var title=UnityEngine.Object.FindFirstObjectByType<Eclipse.UI.TitleScreen>();
                if((bool)Field(title,"splashing")||(string)Field(title,"currentPage")!="Home")return;
                typeof(Eclipse.UI.TitleScreen).GetMethod("BeginCampaign",Hidden).Invoke(title,null);
                SeedPostTutorialProfile();campaign=true;return;
            }
            if(!entered) {
                if(ModRuntime.Scripts==null||Module.GetInstance()==null)return;
                var screen=Module.GetInstance().GetCurrentScreenType();if(screen!=ScreenType.ModuleDojo&&screen!=ScreenType.ModuleMap)return;
                Check(!ModRuntime.Scripts.HasErrors,"Game session has mod startup errors");
                var encounter=ListSF.CHMCKGCDGCM(new FightIDS(ModRuntime.Scripts.Content.RuntimeFightId(DefinitionId.Parse("core:fights/zone_1/tournament/3"))));
                Check(encounter!=null,"Core encounter missing");entered=GameUtils.StartFight(encounter,false,null,true,false);return;
            }
            var fight=Fight.GetCurrentFight();if(fight==null)return;
            var player=fight.GetPlayerModel();var enemy=fight.GetEnemyModel();
            if(player==null||enemy==null||fight.get_FightTimeInFrames()<1)return;
            player.Parameters.set_IsImmortalityEnabled(true);enemy.Parameters.set_IsImmortalityEnabled(true);
            // Controlled idle participants isolate displacement from input/AI.
            player.Parameters.UserControlled=false;enemy.Parameters.AiControlled=false;
            int frame=fight.get_FightTimeInFrames();
            if(!rigChecked&&frame>=75) {
                var probe=Surface("fixture.motion-probe","probe");
                Check(probe!=null&&probe.Read("result").Text.StartsWith("composed:"),"Independent Lua rules did not report composed native movement");
                VerifyRigTranslation(fight,player);
                beforePlayer=X(fight,player);beforeEnemy=X(fight,enemy);
                Queue(fight,player,7);fight.SetPaused(true);Apply(fight);
                Check(Math.Abs(X(fight,player)-beforePlayer)<.01,"Paused boundary consumed motion");
                paused=true;pauseStarted=EditorApplication.timeSinceStartup;rigChecked=true;return;
            }
            if(paused) {
                if(EditorApplication.timeSinceStartup-pauseStarted<.5)return;
                Check(Math.Abs(X(fight,player)-beforePlayer)<.01,"Native pause moved rig");
                fight.SetPaused(false);Apply(fight);
                Check(Math.Abs(X(fight,player)-beforePlayer-7)<.05,"Resume did not apply once");
                Apply(fight);Check(Math.Abs(X(fight,player)-beforePlayer-7)<.05,"Native motion repeated");paused=false;
            }
            var pulse=Surface("example.repulse","repulse");
            if(!clicked&&frame>=100) {
                Check(pulse!=null&&!pulse.IsClosed,"Shipped Repulse HUD missing");
                var views=UnityEngine.Object.FindObjectsByType<Eclipse.UI.Modding.ModUiView>(FindObjectsInactive.Include,FindObjectsSortMode.None);
                var view=views.SingleOrDefault(v=>ReferenceEquals(Field(v,"surface"),pulse));
                Check(view!=null,"Repulse view absent; mounted="+pulse.IsMounted+" views="+string.Join(",",views.Select(v=>v.name+":"+v.gameObject.activeInHierarchy)));
                var widgets=(IDictionary)Field(view,"widgets");
                var button=(Button)Field(widgets["cast"],"Button");
                Check(button!=null&&button.gameObject.activeInHierarchy&&button.interactable,"Native Repulse button not active: view="+view.gameObject.activeInHierarchy+
                    " nativeBlocked="+typeof(Eclipse.UI.Modding.ModUiGameBridge).GetProperty("NativeInputBlocked",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null)+
                    " dialogLock="+typeof(Eclipse.UI.Modding.ModUiGameBridge).GetField("nativeBlocked",BindingFlags.Static|BindingFlags.NonPublic).GetValue(null)+
                    " title="+Eclipse.UI.TitleScreen.IsOpen+" dialogs="+string.Join(",",UnityEngine.Object.FindObjectsByType<Nekki.SF2.GUI.Dialogs.BaseDialog>(FindObjectsSortMode.None).Select(d=>d.name)));
                beforePlayer=X(fight,player);beforeEnemy=X(fight,enemy);castFrame=frame;
                button.onClick.Invoke();
                Check(Math.Abs(X(fight,player)-beforePlayer)<.01,"UI click moved fighter recursively");
                clicked=true;return;
            }
            if(clicked&&!recharged&&frame>castFrame) {
                if(!castVerified) {
                    Check(frame<=castFrame+3,"Native acceptance missed the initial displacement observation");
                    Check(pulse.Read("status").Text.Contains("queued")&&!pulse.Read("cast").Enabled,"Shipped ability did not queue or disable its cooldown button");
                    float dx=X(fight,player)-beforePlayer,enemyDx=X(fight,enemy)-beforeEnemy;
                    Check(Math.Abs(Math.Abs(dx)-40)<3&&Math.Abs(Math.Abs(enemyDx)-100)<3,"Shipped ability did not displace both native fighters: "+dx+", "+enemyDx);
                    Check(dx*enemyDx<0,"Repulse did not separate participants");
                    Debug.Log("[FighterMotionUnity] Repulse native displacement self="+dx+" opponent="+enemyDx);
                    castVerified=true;
                }
                if(pulse.Read("status").Text=="Repulse ready"&&pulse.Read("cast").Enabled) {
                    Check(frame>=castFrame+180,"Cooldown recharged too soon");recharged=true;
                    Check(player.GetCurrentAnimation()!=null&&enemy.GetCurrentAnimation()!=null,"Running animation lost after movement");
                    Check(ModRuntime.Scripts.CallbackDiagnostics.RecentFailures.Count==0,"Lua callback diagnostics retained a native motion failure");
                    // Capture real rendering before native surrender closes the HUD.
                    new GameObject("Motion capture").AddComponent<FighterMotionCapture>();
                }
            }
        } catch(Exception error) {Debug.LogError("[FighterMotionUnity] FAIL: "+error);Finish(1);}
    }
    static void SeedPostTutorialProfile()
    {
        // BeginCampaign releases the title's throwaway preview before its
        // delayed real load. Seed only this run's isolated native save path.
        // First-time tutorial input is not part of this combat acceptance.
        var root=Path.GetDirectoryName(Application.dataPath);
        var directory=SF2Paths.GetUserDataDirectory();
        Check(Eclipse.Saves.CampaignSaveSession.PreviewDirectory==null,"Title preview was not released");
        Check(directory.StartsWith(Application.persistentDataPath,StringComparison.OrdinalIgnoreCase)&&
            Application.persistentDataPath.Contains(Path.GetFileName(root)),"Profile path was not isolated");
        var profile=XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(),"usersDefault.xml",XmlUtils.EBLFEPIOMOL.Normal,true,XmlCryptoUtils.NNLGALNDJCL());
        Check(profile!=null,"Default native profile missing");
        var warrior=profile.SelectSingleNode("/Root/Warriors/Warrior[@ID='1']") as System.Xml.XmlElement;
        Check(warrior!=null,"Default native warrior missing");
        warrior.SetAttribute("Tutorial","END");
        Directory.CreateDirectory(directory);
        XmlUtils.ONLDJNLKKAL(profile,Path.Combine(directory,Constants.OJMIJINKBPJ).Replace('\\','/'));
        Debug.Log("[FighterMotionUnity] Seeded isolated post-tutorial profile: "+directory);
    }
    static void VerifyRigTranslation(Fight fight,Model player)
    {
        var points=(IEnumerable)Field(Field(player.GetModelObject(),"CEHJGIHMKFF"),"OEAFIMFONDL");
        var rig=new List<(Vector3f vector,float x)>();
        foreach(ModelNode node in points){rig.Add((node.GetStart(),node.GetStart().GetX()));rig.Add((node.GetEnd(),node.GetEnd().GetX()));}
        var animation=Field(player,"_Animation");
        var buffers=(IEnumerable)Field(animation,"AHOBIIMFNEP");
        var vectors=new List<(Vector3f vector,float x)>();
        foreach(IEnumerable buffer in buffers)foreach(Vector3f vector in buffer)vectors.Add((vector,vector.GetX()));
        var frames=Field(animation,"_Frames");
        var frameList=(IList)Field(frames,"DCFPONJAING");
        int first=(bool)Field(frames,"_IsInterruptFramesSeted")?2:0;
        int last=(int)Field(frames,"BOAIMPGFINL");
        for(int i=first;i<last;i++) {
            var item=(KeyFrames.Frame)frameList[i];for(int j=0;j<item.Size;j++)vectors.Add((item.Data[j],item.Data[j].GetX()));
        }
        Check(rig.Count>20&&vectors.Count>20,"Native rig/buffer/keyframe data missing");
        // Compare all rig coordinates against the native path from identical
        // state. Its constraint/friction solver can correct current points;
        // expecting a perfect +7 on every current point is not native behavior.
        // Limit the baseline to rig/physics/animation. A whole versus snapshot
        // is not a supported campaign Lua/UI rollback mechanism.
        var snapshotter=new ObjectGraphSnapshotter(new MotionRigPolicy());
        var baseline=new StateSnapshot();
        snapshotter.Capture(baseline,0,new object[]{player.GetModelObject(),Field(player,"_Physics"),animation});
        player.ShiftModelPosition(new Vector3f(7,0,0),true);
        var expected=rig.Select(point=>new Vector3f(point.vector)).ToArray();
        foreach(var point in vectors)Check(Math.Abs(point.vector.GetX()-point.x-7)<.01,"Native animation data did not translate");
        snapshotter.Restore(baseline);
        foreach(var point in rig)Check(Math.Abs(point.vector.GetX()-point.x)<.01,"Rig baseline was not restored");
        foreach(var point in vectors)Check(Math.Abs(point.vector.GetX()-point.x)<.01,"Animation baseline was not restored");
        float start=X(fight,player);Queue(fight,player,9);Queue(fight,player,-2);
        Check(Math.Abs(X(fight,player)-start)<.01,"Native queue applied inline");Apply(fight);
        for(int i=0;i<rig.Count;i++) {
            var actual=rig[i].vector;var wanted=expected[i];
            Check(Math.Abs(actual.GetX()-wanted.GetX())<.01&&Math.Abs(actual.GetY()-wanted.GetY())<.01&&Math.Abs(actual.GetZ()-wanted.GetZ())<.01,"Queued rig translation diverged from native constraints at point "+i);
        }
        foreach(var point in vectors)Check(Math.Abs(point.vector.GetX()-point.x-7)<.01,"Running animation data did not translate");
        Debug.Log("[FighterMotionUnity] Verified "+rig.Count+" real rig coordinates and "+vectors.Count+" buffer/keyframe coordinates; additive translation +7.");
    }
    sealed class MotionRigPolicy : ISnapshotPolicy
    {
        public bool IsOpaque(Type type)=>typeof(Model).IsAssignableFrom(type)||typeof(UnityEngine.Object).IsAssignableFrom(type)||
            typeof(InfoAnimation).IsAssignableFrom(type)||typeof(Delegate).IsAssignableFrom(type);
        public bool Captures(FieldInfo field)=>true;
        public bool NeedsFieldCopy(Type type)=>false;
        public SnapshotCodec CodecFor(Type type)=>null;
        public PropertyInfo[] ExtraProperties(Type type)=>null;
    }
    static ModUiSurface Surface(string owner,string id)
    {
        foreach(var context in (IEnumerable)Field(ModRuntime.Scripts,"_contexts")) {
            var scope=context.GetType().GetProperty("UiScope").GetValue(context) as ModUiScope;
            if(scope==null||scope.Owner.Value!=owner)continue;
            foreach(ModUiSurface surface in ((IDictionary)Field(scope,"surfaces")).Values)if(surface.Id==id)return surface;
        }
        return null;
    }
    public static void Captured()
    {
        try {
            var fight=Fight.GetCurrentFight();var pulse=Surface("example.repulse","repulse");
            Queue(fight,fight.GetPlayerModel(),50);
            typeof(Fight).GetMethod("HCNDAFDHACI",Hidden).Invoke(fight,new object[]{GameOverTypes.GAME_OVER_SURRENDER});
            Check(pulse.IsClosed,"Native surrender left shipped HUD open");
            Check(((IDictionary)Field(fight,"_eclipseFighterMotion")).Count==0,"Native surrender retained queued displacement");
            var root=Path.GetDirectoryName(Application.dataPath);
            File.WriteAllText(Path.Combine(root,"validation-result.txt"),"PASS: "+checks+" full-game native fighter motion checks; real boot/core encounter, independent Lua rule composition, rig/animation data, pause/resume, shipped native HUD button/cooldown, continued combat and surrender cleanup. Input/AI participants controlled; isolated post-tutorial saved profile. First-time onboarding not validated.");
            Debug.Log("[FighterMotionUnity] PASS: "+checks+" full-game checks");Finish(0);
        } catch(Exception error){Debug.LogError("[FighterMotionUnity] FAIL: "+error);Finish(1);}
    }
    static void CaptureError(string message,string stack,LogType type)
    {
        if(!entered||combatException!=null)return;
        if(type==LogType.Exception&&(stack.Contains("Fight.")||stack.Contains("Model.")||stack.Contains("Modding")))combatException=message+"\n"+stack;
        if(type==LogType.Warning&&message.Contains("[ModMotion]"))combatException=message;
    }
    static void Finish(int code)
    {
        SessionState.SetBool(Active,false);EditorApplication.update-=Update;Application.logMessageReceived-=CaptureError;
        EditorApplication.Exit(code);
    }
}
public sealed class FighterMotionCapture : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        var texture=ScreenCapture.CaptureScreenshotAsTexture();
        File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Application.dataPath),"repulse-native.png"),texture.EncodeToPNG());
        UnityEngine.Object.Destroy(texture);FighterMotionUnity.Captured();
    }
}
#endif

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
public static class AudioUnity
{
    const string Active="Eclipse.AudioUnity.Active";
    const BindingFlags Hidden=BindingFlags.Instance|BindingFlags.NonPublic;
    static double started,lastReport,phaseAt; static bool campaign,entered;
    static int checks,phase,sample; static ModUiSurface surface; static AudioSource source;
    static ModAudioScope isolated; static ModAudioInstance oneShot; static string combatException;
    static AudioUnity(){if(SessionState.GetBool(Active,false)){started=EditorApplication.timeSinceStartup;EditorApplication.update+=Update;Application.logMessageReceived+=Log;}}
    public static void Run()
    {
        var root=Path.GetDirectoryName(Application.dataPath);
        if(!File.Exists(Path.Combine(root,"audio-fixture.marker")))throw new Exception("Requires isolated audio fixture.");
        Environment.SetEnvironmentVariable("ECLIPSE_MODS_ROOT",Path.Combine(root,"Mods"));
        var arguments=Environment.GetCommandLineArgs();int productIndex=Array.IndexOf(arguments,"-audioAcceptanceProductName");
        if(productIndex<0||productIndex+1>=arguments.Length||!System.Text.RegularExpressions.Regex.IsMatch(arguments[productIndex+1],"^AudioUnity-[0-9a-f]{32}$"))throw new Exception("Pass the generated acceptance product through the runner.");
        PlayerSettings.companyName="EclipseAcceptance";PlayerSettings.productName=arguments[productIndex+1];
        SessionState.SetBool(Active,true);EditorSceneManager.OpenScene("Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity");
        var view=EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));view.Show();view.Focus();EditorApplication.EnterPlaymode();
    }
    static object Field(object value,string name)=>value.GetType().GetField(name,Hidden|BindingFlags.Public).GetValue(value);
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static AudioSource[] Sources()=>UnityEngine.Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None).Where(s=>s.gameObject.name=="Eclipse mod audio"&&s.clip!=null).ToArray();
    static void Click(string id){var view=UnityEngine.Object.FindObjectsByType<Eclipse.UI.Modding.ModUiView>(FindObjectsInactive.Include,FindObjectsSortMode.None).Single(v=>ReferenceEquals(Field(v,"surface"),surface));var button=(Button)Field(((IDictionary)Field(view,"widgets"))[id],"Button");Check(button.gameObject.activeInHierarchy&&button.interactable,"Button unavailable: "+id);button.onClick.Invoke();}
    static void Next(){phase++;phaseAt=EditorApplication.timeSinceStartup;}
    static void Update()
    {
        if(!EditorApplication.isPlaying)return;
        try{
            if(combatException!=null)throw new Exception(combatException);
            if(EditorApplication.timeSinceStartup-started>300)throw new Exception("Timed out phase "+phase);
            if(EditorApplication.timeSinceStartup-lastReport>20){lastReport=EditorApplication.timeSinceStartup;Debug.Log("[AudioUnity] Waiting entered="+entered+" phase="+phase);}
            if(!campaign&&Eclipse.UI.TitleScreen.IsOpen){
                var title=UnityEngine.Object.FindFirstObjectByType<Eclipse.UI.TitleScreen>();
                if((bool)Field(title,"splashing")||(string)Field(title,"currentPage")!="Home")return;
                typeof(Eclipse.UI.TitleScreen).GetMethod("BeginCampaign",Hidden).Invoke(title,null);
                var directory=SF2Paths.GetUserDataDirectory();
                Check(Eclipse.Saves.CampaignSaveSession.PreviewDirectory==null&&directory.StartsWith(Application.persistentDataPath,StringComparison.OrdinalIgnoreCase)&&Application.persistentDataPath.Contains("AudioUnity-"),"Profile not isolated");
                var profile=XmlUtils.OpenXMLDocument(SF2Paths.KKIDGPBOBNI(),"usersDefault.xml",XmlUtils.EBLFEPIOMOL.Normal,true,XmlCryptoUtils.NNLGALNDJCL());
                ((System.Xml.XmlElement)profile.SelectSingleNode("/Root/Warriors/Warrior[@ID='1']")).SetAttribute("Tutorial","END");
                Directory.CreateDirectory(directory);XmlUtils.ONLDJNLKKAL(profile,Path.Combine(directory,Constants.OJMIJINKBPJ).Replace('\\','/'));
                campaign=true;return;
            }
            if(!entered){
                if(ModRuntime.Scripts==null||Module.GetInstance()==null)return;
                var screen=Module.GetInstance().GetCurrentScreenType();if(screen!=ScreenType.ModuleDojo&&screen!=ScreenType.ModuleMap)return;
                Check(!ModRuntime.Host.HasErrors,ModRuntime.Host.FormatReport());Check(ModRuntime.Host.EnabledMods.Any(m=>m.Id.Value=="example.audio-lab"),"Audio Lab not enabled");Check(!ModRuntime.Scripts.HasErrors,"Startup mod errors");
                var encounter=ListSF.CHMCKGCDGCM(new FightIDS(ModRuntime.Scripts.Content.RuntimeFightId(DefinitionId.Parse("core:fights/zone_1/tournament/3"))));
                Check(encounter!=null,"Core encounter missing");entered=GameUtils.StartFight(encounter,false,null,true,false);return;
            }
            var fight=Fight.GetCurrentFight();if(fight==null||fight.get_FightTimeInFrames()<100)return;
            fight.GetPlayerModel().Parameters.UserControlled=false;fight.GetEnemyModel().Parameters.AiControlled=false;fight.GetPlayerModel().Parameters.set_IsImmortalityEnabled(true);
            double elapsed=EditorApplication.timeSinceStartup-phaseAt;
            switch(phase){
            case 0:
                surface=Surface();Check(surface!=null&&!surface.IsClosed,"Audio Lab HUD missing");SoundController.SetSoundVolume(.8f);Click("game");Check(surface.Read("status").Text=="Loop: game","Actual Lua button failed");Next();break;
            case 1:
                if(elapsed<.3)return;source=Sources().Single();Check(source.isPlaying&&source.loop&&!source.ignoreListenerPause,"Game voice not playing");Check(source.clip.samples==22050&&source.clip.frequency==22050,"WAV decode mismatch");var data=new float[22050];Check(source.clip.GetData(data,0)&&data.Any(v=>Math.Abs(v)>.1),"Decoded signal missing");Check(Math.Abs(source.volume-.8)<.001,"Saved sound volume not applied");sample=source.timeSamples;new GameObject("Audio Lab capture").AddComponent<AudioCapture>();Next();break;
            case 2:
                if(elapsed<.2)return;Check(source.timeSamples!=sample,"Native source clock did not advance");Click("quiet");Check(Math.Abs(source.volume-.2)<.001,"Instance volume did not multiply saved volume");fight.SetPaused(true);Next();break;
            case 3:
                if(elapsed<.15)return;Check(!source.isPlaying&&source.clip!=null,"Game clock did not pause/retained clip");sample=source.timeSamples;Next();break;
            case 4:
                if(elapsed<.25)return;Check(source.timeSamples==sample,"Paused source advanced");fight.SetPaused(false);Next();break;
            case 5:
                if(elapsed<.2)return;Check(source.isPlaying&&source.timeSamples!=sample,"Resume failed");AudioListener.pause=true;Next();break;
            case 6:
                if(elapsed<.15)return;Check(source.clip!=null,"Game voice was released on listener pause");sample=source.timeSamples;Next();break;
            case 7:
                if(elapsed<.25)return;Check(source.timeSamples==sample,"Listener pause advanced game source");AudioListener.pause=false;Click("real");Next();break;
            case 8:
                if(elapsed<.2)return;Check(Sources().Length==1,"Replacing owned voice leaked source");source=Sources().Single();Check(source.isPlaying&&source.ignoreListenerPause,"Real clock option not applied");fight.SetPaused(true);sample=source.timeSamples;Next();break;
            case 9:
                if(elapsed<.25)return;Check(source.isPlaying&&source.timeSamples!=sample,"Real voice stopped with native pause");AudioListener.pause=true;sample=source.timeSamples;Next();break;
            case 10:
                if(elapsed<.25)return;Check(source.isPlaying&&source.timeSamples!=sample,"Real voice ignored listener-pause contract");AudioListener.pause=false;fight.SetPaused(false);SoundController.SetSoundVolume(0);Next();break;
            case 11:
                if(elapsed<.1)return;Check(source.volume==0&&source.isPlaying,"Saved mute stopped or failed to silence voice");SoundController.SetSoundVolume(.8f);Click("stop");Check(Sources().Length==0&&surface.Read("status").Text=="Stopped","Explicit stop retained source");
                // Natural completion uses the same production backend, independently
                // of a UI and without disabling the audio device (-noaudio is absent).
                var type=typeof(ModRuntime).Assembly.GetType("Eclipse.Modding.ModAudioBackend");isolated=new ModAudioScope((IModAudioBackend)Activator.CreateInstance(type,true));
                Check(isolated.TryPlay(AssetId.Parse("example.audio-lab:audio/beacon"),new ModAudioOptions(),null,out oneShot,out var error),error);Next();break;
            case 12:
                if(elapsed<1.4)return;Check(!oneShot.IsActive&&Sources().Length==0,"One-shot failed natural completion");
                // Shared session bound: four full per-mod scopes plus the isolated
                // one fail at 64 without stopping anyone else's voices.
                var scopes=new ModAudioScope[4];for(int i=0;i<4;i++){var poolBackendType=typeof(ModRuntime).Assembly.GetType("Eclipse.Modding.ModAudioBackend");scopes[i]=new ModAudioScope((IModAudioBackend)Activator.CreateInstance(poolBackendType,true));for(int j=0;j<16;j++)Check(scopes[i].TryPlay(AssetId.Parse("example.audio-lab:audio/beacon"),new ModAudioOptions(loop:true),null,out _,out _),"Global pool allocation");}
                Check(!isolated.TryPlay(AssetId.Parse("example.audio-lab:audio/beacon"),new ModAudioOptions(loop:true),null,out _,out var budget)&&budget.Contains("64"),"Global voice bound absent");Check(Sources().Length==64,"Global rejection evicted voices");foreach(var scope in scopes)scope.Dispose();Check(Sources().Length==0,"Scope shutdown left native sources");Click("game");Next();break;
            case 13:
                if(elapsed<.2)return;Check(Sources().Length==1,"UI restart voice missing");surface.Close();Check(Sources().Length==0,"UI owner closure failed");
                Check(isolated.TryPlay(AssetId.Parse("example.audio-lab:audio/beacon"),new ModAudioOptions(loop:true),null,out _,out _),"Scene voice failed");UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.CreateScene("Audio acceptance exit"));Check(Sources().Length==0,"Scene change leaked voice");isolated.Dispose();
                Check(ModRuntime.Scripts.CallbackDiagnostics.RecentFailures.Count==0,"Shipped Lua callback failures");Check(File.Exists(Path.Combine(Path.GetDirectoryName(Application.dataPath),"audio-lab-native.png")),"Rendered Audio Lab capture missing");
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(Application.dataPath),"audio-result.txt"),"PASS: "+checks+" native full-game audio checks; shipped Lua/native HUD, actual WAV decode/source progression, per-instance volume and stop, saved volume/mute, game/real/listener pause, resume, natural completion, 64-voice shared bound, UI/scope/scene teardown. Device audibility not inspected; controls/profile isolated.");
                Debug.Log("[AudioUnity] PASS: "+checks+" full-game checks");Finish(0);break;
            }
        }catch(Exception error){Debug.LogError("[AudioUnity] FAIL: "+error);Finish(1);}
    }
    static ModUiSurface Surface(){foreach(var context in (IEnumerable)Field(ModRuntime.Scripts,"_contexts")){var scope=context.GetType().GetProperty("UiScope").GetValue(context) as ModUiScope;if(scope==null||scope.Owner.Value!="example.audio-lab")continue;foreach(ModUiSurface s in ((IDictionary)Field(scope,"surfaces")).Values)if(s.Id=="lab")return s;}return null;}
    static void Log(string message,string stack,LogType type){if(entered&&type==LogType.Exception&&(stack.Contains("Fight.")||stack.Contains("Model.")||stack.Contains("Modding")))combatException=message+"\n"+stack;}
    static void Finish(int code){AudioListener.pause=false;SessionState.SetBool(Active,false);EditorApplication.update-=Update;Application.logMessageReceived-=Log;EditorApplication.Exit(code);}
}
public sealed class AudioCapture : MonoBehaviour
{
    IEnumerator Start(){yield return new WaitForEndOfFrame();var texture=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(Path.GetDirectoryName(Application.dataPath),"audio-lab-native.png"),texture.EncodeToPNG());UnityEngine.Object.Destroy(texture);UnityEngine.Object.Destroy(gameObject);}
}
#endif

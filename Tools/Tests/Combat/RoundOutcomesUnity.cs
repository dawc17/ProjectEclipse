#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;
using Eclipse.UI.Modding;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[InitializeOnLoad]
public static class RoundOutcomesUnity
{
    const string Pending="Eclipse.RoundOutcomesUnity.Pending";
    static int checks;
    static string root;
    static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
    static RoundOutcomesUnity(){if(SessionState.GetBool(Pending,false))EditorApplication.update+=Begin;}
    public static void Run()
    {
        SessionState.SetBool(Pending,true);EditorApplication.update+=Begin;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);EditorApplication.EnterPlaymode();
    }
    static void Begin()
    {
        if(!EditorApplication.isPlaying || EditorApplication.isCompiling)return;
        EditorApplication.update-=Begin;SessionState.SetBool(Pending,false);
        new GameObject("Objective acceptance").AddComponent<RoundOutcomesUnityRunner>();
    }
    static Color32[] Capture(Camera camera,RenderTexture target,string name)
    {
        Canvas.ForceUpdateCanvases();camera.Render();var before=RenderTexture.active;RenderTexture.active=target;
        var pixels=new Texture2D(target.width,target.height,TextureFormat.RGBA32,false);
        pixels.ReadPixels(new Rect(0,0,target.width,target.height),0,0);pixels.Apply();RenderTexture.active=before;
        var data=pixels.GetPixels32();Check(data.Count(pixel=>pixel.r>64 || pixel.g>64 || pixel.b>64)>30,"HUD rendered no readable text pixels");
        File.WriteAllBytes(Path.Combine(root,name+".png"),pixels.EncodeToPNG());UnityEngine.Object.Destroy(pixels);return data;
    }
    public static IEnumerator Steps()
    {
        root=Path.GetDirectoryName(Application.dataPath);
        Check(Application.isPlaying && SystemInfo.graphicsDeviceType!=GraphicsDeviceType.Null,"Play Mode with graphics required");
        var discovery=ModDiscovery.DiscoverLoose(Path.Combine(root,"Mods"));Check(discovery.Diagnostics.Count==0,"Shipped example discovery failed");
        var host=new ModHost(discovery.Mods);var logs=new List<ModLogEntry>();var views=new List<ModUiView>();var surfaces=new List<ModUiSurface>();
        var canvasObject=new GameObject("Objective canvas",typeof(RectTransform),typeof(Canvas));var canvas=canvasObject.GetComponent<Canvas>();
        canvas.renderMode=RenderMode.WorldSpace;var mount=canvasObject.GetComponent<RectTransform>();mount.sizeDelta=new Vector2(500,100);mount.localScale=Vector3.one*.01f;
        var camera=new GameObject("Objective camera").AddComponent<Camera>();camera.transform.position=new Vector3(0,0,-10);
        camera.orthographic=true;camera.orthographicSize=.5f;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;canvas.worldCamera=camera;
        var target=new RenderTexture(1000,200,24);target.Create();camera.targetTexture=target;
        var runtime=new MoonSharpScriptRuntime(surface=>{surfaces.Add(surface);views.Add(ModUiView.Attach(surface,mount));});
        using(var session=ModScriptSession.Start(host,runtime,logs.Add,content=>{
            var document=new XmlDocument();document.Load(Path.Combine(root,"FixtureData/stages.xml"));CoreContentImporter.ImportStages(content,document.SelectSingleNode("Stages/Zones"));
        }))
        {
            Check(!session.HasErrors,session.FormatReport());var rule=session.Content.FightRules.Single();
            ModRuntime.Scripts=new FixtureScripts{Content=session.Content};
            var fight=new Fight();fight.FightDefinition.FightId=session.Content.RuntimeFightId(DefinitionId.Parse("core:fights/zone_1/tournament/3"));
            var xml=new XmlDocument();xml.LoadXml("<BattleRuleInstance/>");var fighter=new ModInstanceFighter(fight,xml.DocumentElement,rule);
            void Invoke(ModEffectEvent kind)
            {
                Check(session.TryInvokeBehavior(rule.Behavior,kind,null,new Dictionary<string,string>{{"source","rule"},{"round",fight.round.round.ToString()},{"side","player"},{"fight_id",fight.FightDefinition.FightId}},fighter,out var error),error);
            }
            Invoke(ModEffectEvent.RoundBegin);Check(views.Count==1 && surfaces[0].Mount==ModUiMount.CombatHud,"Objective did not mount HUD");
            var text=views[0].GetComponentInChildren<Text>();Check(text.font!=null && text.font.name=="AGOpusBold","Recovered font unavailable");
            Check(text.text.EndsWith("0/3"),"Initial objective text incorrect");yield return null;
            var initial=Capture(camera,target,"objective-initial");
            fight.DamageEvent=new ModDamageEvent(1,1,.9,true,false);Invoke(ModEffectEvent.DamageDealt);
            Check(text.text.EndsWith("0/3"),"Blocked hit advanced goal");
            for(int i=1;i<=3;i++){fight.DamageEvent=new ModDamageEvent(1,1,.9,false,false);Invoke(ModEffectEvent.DamageDealt);Check(text.text.EndsWith(i+"/3"),"Hit HUD did not update");}
            yield return null;Check(!initial.SequenceEqual(Capture(camera,target,"objective-complete")),"Completed goal did not change rendered pixels");
            Invoke(ModEffectEvent.Tick);Check(fight.Player.RoundsWon==0,"Callback settled round recursively");fight.Step();
            Check(fight.Player.RoundsWon==1 && fight.Winner()==fight.Player && fight.Player.Health==1 && fight.Enemy.Health==1,"Nonlethal win ignored");
            fight.Step();Check(fight.Player.RoundsWon==1,"Repeated boundary scored twice");
            Invoke(ModEffectEvent.RoundEnd);Check(surfaces[0].IsClosed,"Round end retained HUD");
            yield return new WaitForSecondsRealtime(.3f);Check(views[0]==null,"Native fade did not destroy ended HUD");
            fight.ResetForNextRound();Invoke(ModEffectEvent.RoundBegin);text=views[1].GetComponentInChildren<Text>();Check(text.text.EndsWith("0/3"),"Next round did not reset state");
            for(int i=0;i<599;i++){fight.Clock++;Invoke(ModEffectEvent.Tick);}
            Check(!fight._eclipseRoundOutcomes.PendingPlayerWins.HasValue,"Goal timeout ended too early");fight.Clock++;Invoke(ModEffectEvent.Tick);fight.Step();
            Check(fight.Enemy.RoundsWon==1 && fight.Winner()==fight.Enemy && fight.Player.Health==1,"Nonlethal objective loss ignored");
            Invoke(ModEffectEvent.FightEnd);Check(surfaces[1].IsClosed,"Fight end retained HUD");
            // Requests are transient. A new fight starts with no objective result.
            var next=new Fight();Check(!next._eclipseRoundOutcomes.PendingPlayerWins.HasValue && !next._eclipseRoundOutcomes.ResolvedPlayerWins.HasValue,"New fight retained result");
        }
        yield return new WaitForSecondsRealtime(.3f);Check(views.All(view=>view==null),"Session teardown retained native views");
        Check(!logs.Any(log=>log.Level==ModLogLevel.Error),"Lua reported an error");
        camera.targetTexture=null;target.Release();UnityEngine.Object.Destroy(target);UnityEngine.Object.Destroy(canvasObject);UnityEngine.Object.Destroy(camera.gameObject);
        var result="[RoundOutcomesUnity] PASS: "+checks+" checks: shipped objective Lua, native font/HUD pixels/fades, extracted round score and winner. Controlled contacts, models, clock, presentation and settlement; not a full-game playtest.";
        File.WriteAllText(Path.Combine(root,"validation-result.txt"),result);Debug.Log(result);EditorApplication.Exit(0);
    }
}
public sealed class RoundOutcomesUnityRunner : MonoBehaviour
{
    IEnumerator Start()
    {
        var steps=RoundOutcomesUnity.Steps();while(true)
        {
            object current;try{if(!steps.MoveNext())yield break;current=steps.Current;}
            catch(Exception error){Debug.LogException(error);EditorApplication.Exit(1);yield break;}
            yield return current;
        }
    }
}
#endif

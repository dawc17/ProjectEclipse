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
public static class ModExtensionsUnity
{
    const string Pending="Eclipse.ModExtensionsUnity.Pending";
    static int checks;
    static string root;
    static void Check(bool value,string message) { checks++;if(!value)throw new Exception(message); }
    static ModExtensionsUnity() { if(SessionState.GetBool(Pending,false))EditorApplication.update+=Begin; }
    public static void Run()
    {
        SessionState.SetBool(Pending,true);EditorApplication.update+=Begin;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        EditorApplication.EnterPlaymode();
    }
    static void Begin()
    {
        if(!EditorApplication.isPlaying || EditorApplication.isCompiling)return;
        EditorApplication.update-=Begin;SessionState.SetBool(Pending,false);
        new GameObject("Framework acceptance").AddComponent<ModExtensionsUnityRunner>();
    }
    sealed class Fighter : IModFighterOperations,IModIncomingHitSource
    {
        public double Damage=.1;
        public bool Blocked;
        public ModIncomingHit IncomingHit=>new ModIncomingHit(()=>Damage,value=>Damage=value,Blocked);
        public bool TryChangeHealth(double value,out string error){error="";return true;}
        public bool TryAddMagicCharge(double value,out string error){error="";return true;}
    }
    static void Import(ModContentCatalog content)
    {
        var xml=new XmlDocument();xml.Load(Path.Combine(root,"FixtureData/stages.xml"));
        CoreContentImporter.ImportStages(content,xml.SelectSingleNode("Stages/Zones"));
    }
    static bool Gone(ModUiView view)=>view==null || !view.gameObject.activeSelf ||
        (view.GetComponent<CanvasGroup>()!=null && !view.GetComponent<CanvasGroup>().blocksRaycasts);
    static void Invoke(ModScriptSession session,ModEffectEvent kind,Fighter fighter)
    {
        Check(session.TryInvokeBehavior(DefinitionId.Parse("example.focus-addon:behaviors/focus"),kind,null,
            new Dictionary<string,string>{{"round","1"},{"source","rule"}},fighter,out var error),error);
    }
    static Color32[] Capture(Camera camera,RenderTexture target,string name)
    {
        Canvas.ForceUpdateCanvases();camera.Render();
        var before=RenderTexture.active;RenderTexture.active=target;
        var pixels=new Texture2D(target.width,target.height,TextureFormat.RGBA32,false);
        pixels.ReadPixels(new Rect(0,0,target.width,target.height),0,0);pixels.Apply();RenderTexture.active=before;
        var data=pixels.GetPixels32();
        Check(data.Count(pixel=>pixel.r>64 || pixel.g>64 || pixel.b>64)>30,"HUD rendered no readable text pixels");
        File.WriteAllBytes(Path.Combine(root,name+".png"),pixels.EncodeToPNG());
        UnityEngine.Object.Destroy(pixels);return data;
    }
    public static IEnumerator Steps()
    {
        root=Path.GetDirectoryName(Application.dataPath);
        Check(Application.isPlaying,"Native acceptance must run in Play Mode");
        Check(SystemInfo.graphicsDeviceType!=GraphicsDeviceType.Null,"A real graphics device is required");
        var discovery=ModDiscovery.DiscoverLoose(Path.Combine(root,"Mods"));
        Check(discovery.Diagnostics.Count==0,"Shipped pair discovery failed");
        var host=new ModHost(discovery.Mods);
        var canvasObject=new GameObject("Framework canvas",typeof(RectTransform),typeof(Canvas));
        var canvas=canvasObject.GetComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;
        var mount=canvasObject.GetComponent<RectTransform>();mount.sizeDelta=new Vector2(400,100);mount.localScale=Vector3.one*.01f;
        var camera=new GameObject("Framework camera").AddComponent<Camera>();
        camera.transform.position=new Vector3(0,0,-10);camera.orthographic=true;camera.orthographicSize=.5f;
        camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;canvas.worldCamera=camera;
        var target=new RenderTexture(800,200,24);target.Create();camera.targetTexture=target;
        var views=new List<ModUiView>();
        var surfaces=new List<ModUiSurface>();
        var logs=new List<ModLogEntry>();
        var runtime=new MoonSharpScriptRuntime(surface=> {surfaces.Add(surface);views.Add(ModUiView.Attach(surface,mount));});
        var profile=new XmlDocument();profile.LoadXml("<Warrior/>");
        var fighter=new Fighter();
        using(var session=ModScriptSession.Start(host,runtime,logs.Add,Import))
        {
            Check(!session.HasErrors,session.FormatReport());
            Check(session.ActiveMods.Count==2 && session.Content.Extensions.Count==2,"Framework composition was not active");
            ModSaveData.RecordContext(profile.DocumentElement,session.ActiveMods,session.Content,session.State);
            var initialDiagnostics=session.BindState(profile.DocumentElement);
            Check(initialDiagnostics.Count==0,"Initial state bind failed: "+string.Join("; ",initialDiagnostics));
            Invoke(session,ModEffectEvent.RoundBegin,fighter);
            Check(views.Count==1 && surfaces[0].Mount==ModUiMount.CombatHud,"Add-on HUD did not mount");
            var text=views[0].GetComponentInChildren<Text>();
            Check(text.font!=null && text.font.name=="AGOpusBold","HUD did not load the actual recovered game font");
            Check(text.text=="Focus: 0/3","Initial native text differs from Lua");
            yield return null;
            var zero=Capture(camera,target,"focus-zero");
            Invoke(session,ModEffectEvent.DamageDealing,fighter);
            Check(text.text=="Focus: 1/3" && fighter.Damage==.1,"First hit changed the wrong native text/damage");
            yield return null;
            var one=Capture(camera,target,"focus-one");
            Check(!zero.SequenceEqual(one),"First-hit HUD did not change rendered pixels");
            fighter.Blocked=true;Invoke(session,ModEffectEvent.DamageDealing,fighter);
            Check(text.text=="Focus: 1/3","Blocked hit changed native HUD");
            fighter.Blocked=false;Invoke(session,ModEffectEvent.DamageDealing,fighter);
            Check(text.text=="Focus: 2/3","Second-hit native text did not update");
            yield return null;
            Capture(camera,target,"focus-two");
            ModSaveData.RecordContext(profile.DocumentElement,session.ActiveMods,session.Content,session.State);
            profile.Save(Path.Combine(root,"focus-profile.xml"));
            Invoke(session,ModEffectEvent.FightEnd,fighter);
            Check(surfaces[0].IsClosed && Gone(views[0]),"Fight end left HUD input/presentation active");
        }
        yield return new WaitForSecondsRealtime(.3f);
        Check(views.All(view=>view==null),"Disposed native HUD objects were retained after fade");
        // Removing all mods must preserve their serialized state for a future reinstall.
        using(var missing=ModScriptSession.Start(new ModHost(Array.Empty<ModDescriptor>()),runtime,logs.Add,Import))
        {
            missing.BindState(profile.DocumentElement);
            ModSaveData.RecordContext(profile.DocumentElement,missing.ActiveMods,missing.Content,missing.State);
            profile.Save(Path.Combine(root,"focus-missing.xml"));
        }
        var reloaded=new XmlDocument();reloaded.Load(Path.Combine(root,"focus-missing.xml"));
        views.Clear();surfaces.Clear();
        using(var session=ModScriptSession.Start(host,runtime,logs.Add,Import))
        {
            Check(session.BindState(reloaded.DocumentElement).Count==0,"Reinstall state bind failed");
            Invoke(session,ModEffectEvent.RoundBegin,fighter);
            var text=views.Single().GetComponentInChildren<Text>();
            Check(text.text=="Focus: 2/3","Removal/reinstall lost saved framework resource");
            yield return null;
            var saved=Capture(camera,target,"focus-reinstalled");
            fighter.Damage=.1;Invoke(session,ModEffectEvent.DamageDealing,fighter);
            Check(Math.Abs(fighter.Damage-.15)<1e-10 && text.text=="Focus: 0/3","Third-hit framework bonus or native text failed");
            yield return null;
            Check(!saved.SequenceEqual(Capture(camera,target,"focus-bonus")),"Bonus did not change rendered HUD pixels");
        }
        yield return new WaitForSecondsRealtime(.3f);
        Check(surfaces.All(surface=>surface.IsClosed) && views.All(view=>view==null),"Reload teardown retained HUD");
        Check(!logs.Any(log=>log.Level==ModLogLevel.Error),"Lua produced an error: "+string.Join("; ",logs));
        camera.targetTexture=null;target.Release();UnityEngine.Object.Destroy(target);
        UnityEngine.Object.Destroy(canvasObject);UnityEngine.Object.Destroy(camera.gameObject);
        var result="[ModExtensionsUnity] PASS: "+checks+" checks, shipped framework/add-on, native font/HUD pixels, removal/reinstall save preservation and teardown. Contact source and asset host are controlled; this is not a full-game playtest.";
        File.WriteAllText(Path.Combine(root,"validation-result.txt"),result);Debug.Log(result);EditorApplication.Exit(0);
    }
}
public sealed class ModExtensionsUnityRunner : MonoBehaviour
{
    IEnumerator Start()
    {
        var steps=ModExtensionsUnity.Steps();
        while(true)
        {
            object current;
            try {if(!steps.MoveNext())yield break;current=steps.Current;}
            catch(Exception error){Debug.LogException(error);EditorApplication.Exit(1);yield break;}
            yield return current;
        }
    }
}
#endif

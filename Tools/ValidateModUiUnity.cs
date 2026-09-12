#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Eclipse.Modding;
using Eclipse.UI.Modding;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[InitializeOnLoad]
public static class ValidateModUiUnity
{
    const string Pending = "Eclipse.ModUi.Validation";
    static int checks;
    static ValidateModUiUnity()
    {
        EditorApplication.playModeStateChanged += state => {
            if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool(Pending, false))
                EditorApplication.delayCall += Run;
        };
    }
    public static void RunEditor()
    {
        SessionState.SetBool(Pending, true);
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorApplication.EnterPlaymode();
    }
    static void Check(bool value, string message) { checks++; if (!value) throw new Exception(message); }
    static void Run()
    {
        SessionState.SetBool(Pending, false);
        try
        {
            Check(Application.isPlaying, "Fixture must run production view in play mode");
            var events = new GameObject("Events", typeof(EventSystem)).GetComponent<EventSystem>();
            var canvas = new GameObject("Mount", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var prior = new GameObject("Previous selection", typeof(RectTransform), typeof(Button));
            prior.transform.SetParent(canvas.transform, false);
            events.SetSelectedGameObject(prior);
            int clicks = 0;
            var scope = new ModUiScope(ModId.Parse("example.ui"));
            var tree = new ModUiNode("root", ModUiKind.Column, 360, 320, gap: 8, children: new[] {
                new ModUiNode("text",ModUiKind.Text,320,40,text:"<b>Plain text</b>"),
                new ModUiNode("bar",ModUiKind.Progress,320,20,value:.25),
                new ModUiNode("disabled",ModUiKind.Button,160,40,text:"Disabled",enabled:false),
                new ModUiNode("button",ModUiKind.Button,160,40,text:"Activate"),
                new ModUiNode("scroll",ModUiKind.Scroll,320,80,children:new[] {
                    new ModUiNode("content",ModUiKind.Column,300,200,children:new[] {
                        new ModUiNode("long",ModUiKind.Text,300,200,text:"Scrollable content") }) }) });
            var surface = scope.Open("meter",ModUiMount.CombatHud,tree,_=>clicks++);
            var view = ModUiView.Attach(surface,canvas.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
            var text = view.transform.Find("root/text").GetComponent<Text>();
            Check(text.font != null && !text.supportRichText && !text.raycastTarget, "Font/plain text/raycast defaults");
            Check(view.transform.Find("root").GetComponent<VerticalLayoutGroup>() != null, "Column not rendered");
            var button = view.transform.Find("root/button").GetComponent<Button>();
            Check(button.GetComponent<Image>().sprite?.name=="CommonButtons.BtnWhite" && button.GetComponent<Image>().type==Image.Type.Sliced,"Original game button sprite not used");
            Check(text.font.name=="AGOpusBold", "Original game font not loaded");
            Check(view.transform.Find("root/bar/Fill").GetComponent<Image>().sprite?.name=="FightUI.HealthBar_Full","Original bar texture not used");
            Check(button.navigation.mode == Navigation.Mode.None, "View enabled uncontrolled navigation");
            Check(view.MoveFocus(1) && events.currentSelectedGameObject == button.gameObject, "Focus did not skip disabled button");
            Check(view.ActivateSelected() && clicks == 1, "Selected activation not routed");
            button.onClick.Invoke(); Check(clicks == 2, "Pointer callback not routed");
            surface.SetText("text","Changed"); Check(text.text == "Changed", "Live text not rendered");
            surface.SetValue("bar",.75);
            var fill = view.transform.Find("root/bar/Fill").GetComponent<RectTransform>();
            Check(Math.Abs(fill.anchorMax.x-.75f)<.0001, "Live progress not rendered");
            surface.SetVisible("root",false);
            Check(!button.gameObject.activeInHierarchy && !view.ActivateSelected(), "Hidden view accepted input");
            surface.SetVisible("root",true);
            surface.SetEnabled("root",false);
            Check(!view.MoveFocus(1) && !surface.TryClick("button"), "Disabled ancestor accepted input");
            surface.SetEnabled("root",true); view.MoveFocus(1);
            var scroll = view.transform.Find("root/scroll").GetComponent<ScrollRect>();
            Check(scroll.content != null && scroll.viewport.GetComponent<RectMask2D>() != null && !scroll.horizontal,
                "Scroll hierarchy/clipping missing");
            surface.Close();
            Check(!view.gameObject.activeSelf && events.currentSelectedGameObject == prior, "Close did not hide/restore focus immediately");
            Check(!view.ActivateSelected(), "Closed view retained input");
            ModUiCloseReason? destroyedReason=null;
            var second = scope.Open("second",ModUiMount.Menu,tree,_=>clicks++,onClose:reason=>destroyedReason=reason);
            var secondView = ModUiView.Attach(second,canvas.GetComponent<RectTransform>());
            Check(secondView.GetComponent<Image>().sprite?.name=="DialogScroll.Background_Center","Menu did not use game parchment");
            second.SetVisible("root",false);
            Check(!secondView.GetComponent<Image>().enabled,"Hidden root retained its parchment");
            second.SetVisible("root",true);
            Check(secondView.GetComponent<Image>().enabled,"Showing root did not restore parchment");
            var styled=scope.Open("styled",ModUiMount.Menu,new ModUiNode("style",ModUiKind.Text,200,40,text:"Style",style:new ModUiStyle(textColor:"#12345680",fontSize:28,textAlign:"right")));
            var styledView=ModUiView.Attach(styled,canvas.GetComponent<RectTransform>());
            var styledText=styledView.GetComponentInChildren<Text>();
            Check(styledText.fontSize==28 && styledText.alignment==TextAnchor.MiddleRight && ((Color32)styledText.color).Equals(new Color32(18,52,86,128)),"Native text style lost");
            styled.Close();
            UnityEngine.Object.DestroyImmediate(secondView.gameObject);
            Check(second.IsClosed && scope.Count == 0, "Native object teardown retained surface");
            Check(destroyedReason==ModUiCloseReason.Destroyed,"Native destruction did not report its close reason");
            scope.Dispose();
            Placement(tree, canvas.GetComponent<RectTransform>());
            Coordination(tree, events, prior);
            Bridge(tree);
            LuaRoundTrip();
            if (Environment.GetCommandLineArgs().Contains("-uiPreview")) Preview();
            Debug.Log("[ModUiUnity] PASS: " + checks + " production Unity UI hierarchy, update, input and lifetime checks. Full-game integration is not claimed.");
            EditorApplication.Exit(0);
        }
        catch (Exception error)
        {
            Debug.LogError("[ModUiUnity] FAIL: " + error);
            EditorApplication.Exit(1);
        }
    }
    static void Preview()
    {
        var camera = new GameObject("Preview camera",typeof(Camera)).GetComponent<Camera>();
        camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color32(35,19,12,255);
        var target=new RenderTexture(1280,720,24);camera.targetTexture=target;
        var canvas=new GameObject("Preview canvas",typeof(RectTransform),typeof(Canvas)).GetComponent<Canvas>();
        canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;
        using(var scope=new ModUiScope(ModId.Parse("example.preview")))
        {
            var tree=new ModUiNode("root",ModUiKind.Column,480,280,gap:16,children:new[]{
                new ModUiNode("title",ModUiKind.Text,480,64,text:"Custom battle rules",style:new ModUiStyle(fontSize:32)),
                new ModUiNode("description",ModUiKind.Text,480,48,text:"Charge your next strike"),
                new ModUiNode("meter",ModUiKind.Progress,480,24,value:.65),
                new ModUiNode("play",ModUiKind.Button,480,64,text:"FIGHT!") });
            ModUiView.Attach(scope.Open("preview",ModUiMount.Menu,tree),canvas.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();camera.Render();
            var previous=RenderTexture.active;RenderTexture.active=target;
            var pixels=new Texture2D(1280,720,TextureFormat.RGB24,false);
            pixels.ReadPixels(new Rect(0,0,1280,720),0,0);pixels.Apply();RenderTexture.active=previous;
            string path=Path.Combine(Path.GetDirectoryName(Application.dataPath),"ui-theme-preview.png");
            File.WriteAllBytes(path,pixels.EncodeToPNG());Debug.Log("[ModUiUnity] Preview: "+path);
            UnityEngine.Object.Destroy(pixels);
        }
        camera.targetTexture=null;target.Release();UnityEngine.Object.Destroy(target);
        UnityEngine.Object.Destroy(canvas.gameObject);UnityEngine.Object.Destroy(camera.gameObject);
    }
    static void Placement(ModUiNode tree, RectTransform mount)
    {
        using(var scope=new ModUiScope(ModId.Parse("example.placement")))
        {
            string[] anchors={"bottom_left","bottom","bottom_right","left","center","right","top_left","top","top_right"};
            for(int i=0;i<anchors.Length;i++)
            {
                var surface=scope.Open("position",ModUiMount.CombatHud,tree,placement:new ModUiPlacement(anchors[i]));
                var view=ModUiView.Attach(surface,mount);view.FitToSafeArea(1280,720);
                var rect=view.GetComponent<RectTransform>();
                var expected=new Vector2((i%3)*.5f,(i/3)*.5f);
                Check(rect.anchorMin==expected && rect.anchorMax==expected && rect.pivot==expected && rect.anchoredPosition==Vector2.zero,"Wrong anchor: "+anchors[i]);
                surface.Close();
            }
            var offset=scope.Open("offset",ModUiMount.CombatHud,tree,placement:new ModUiPlacement("top_right",-24,104));
            var offsetView=ModUiView.Attach(offset,mount);offsetView.FitToSafeArea(1280,720);
            var offsetRect=offsetView.GetComponent<RectTransform>();
            Check(offsetRect.anchoredPosition==new Vector2(-24,-104) && offsetRect.localScale==Vector3.one,"Reference offsets not applied");
            offsetView.FitToSafeArea(180,160);
            Check(offsetRect.localScale==Vector3.one*.5f && offsetRect.anchoredPosition==Vector2.zero,"Resize failed to scale/clamp entire view");
            offsetView.FitToSafeArea(1280,720);
            Check(offsetRect.anchoredPosition==new Vector2(-24,-104) && offsetRect.localScale==Vector3.one,"Resize lost requested placement");
            offset.Close();
            var extreme=scope.Open("extreme",ModUiMount.CombatHud,tree,placement:new ModUiPlacement("center",8192,-8192));
            var extremeView=ModUiView.Attach(extreme,mount);extremeView.FitToSafeArea(1280,720);
            Check(extremeView.GetComponent<RectTransform>().anchoredPosition==new Vector2(460,200),"Outward offsets escaped safe area");
        }
    }
    sealed class LuaFighter : IModFighterOperations, IModCombatSnapshotSource, IModIncomingHitSource
    {
        public int Frame;
        public ModIncomingHit IncomingHit { get; set; }
        public ModCombatSnapshot CaptureCombatSnapshot() => new ModCombatSnapshot(new ModFighterSnapshot(1,1,1,0,0,0),null,Frame,true);
        public bool TryChangeHealth(double value,out string error) { error=""; return true; }
        public bool TryAddMagicCharge(double value,out string error) { error=""; return true; }
    }
    static void LuaRoundTrip()
    {
        string root=Path.GetFullPath(Path.Combine(Application.dataPath,".."));
        var mod=ModDiscovery.DiscoverLoose(Path.Combine(root,"Mods")).Mods.Single();
        var catalog=new ModContentCatalog();
        var stages=new XmlDocument();stages.Load(Path.Combine(root,"FixtureData/stages.xml"));
        CoreContentImporter.ImportStages(catalog,stages.SelectSingleNode("Stages/Zones"));
        var assets=new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)});
        var views=new List<ModUiSurface>();
        var logs=new List<ModLogEntry>();
        string language="eng";
        var runtime=new MoonSharpScriptRuntime(surface=>{views.Add(surface);ModUiGameBridge.Attach(surface);},()=>language);
        using(var registration=catalog.BeginRegistration(mod))
        using(var context=runtime.CreateContext(mod,new ModApiFacade(mod,assets,registration,new ModStateRuntime(),logs.Add)))
        {
            ModLocalizationLoader.Load(mod,assets,registration);
            context.ExecuteEntrypoint();registration.Commit();
            var behavior=catalog.FightRules.Single().Behavior;
            var fighter=new LuaFighter();
            var fields=new Dictionary<string,string>{{"round","1"},{"source","rule"},{"fight_id","unity-fixture"}};
            var interactive=(IModInteractiveBehaviorScriptContext)context;
            Action<ModEffectEvent> invoke=kind=>{
                if(!interactive.TryInvokeBehavior(behavior,kind,null,fields,fighter,out var error))throw new Exception(error);
            };
            invoke(ModEffectEvent.RoundBegin);
            var surface=views.Single();
            var view=UnityEngine.Object.FindObjectsOfType<ModUiView>().Single();
            Check(view.GetComponent<RectTransform>().anchorMin==Vector2.one && surface.Placement.Anchor=="top_right","Lua HUD placement did not reach Unity");
            var button=view.transform.Find("root/arm").GetComponent<Button>();
            var status=view.transform.Find("root/status").GetComponent<Text>();
            var fill=view.transform.Find("root/meter/Fill").GetComponent<RectTransform>();
            Check(!button.interactable && status.text=="Charge: 0%", "Lua-created HUD did not render initial state");
            for(int frame=1;frame<=300;frame++){fighter.Frame=frame;invoke(ModEffectEvent.Tick);}
            Check(button.interactable && fill.anchorMax.x==1 && status.text=="Charge: 100%", "Combat ticks did not update Unity HUD");
            button.onClick.Invoke();
            Check(!button.interactable && status.text=="Next hit: double damage", "Unity button did not invoke Lua/update its native label");
            double damage=10;
            fighter.IncomingHit=new ModIncomingHit(()=>damage,n=>damage=n,true,false);
            invoke(ModEffectEvent.DamageDealing);
            Check(damage==10 && status.text=="Next hit: double damage", "Blocked hit consumed native HUD ability");
            fighter.IncomingHit=new ModIncomingHit(()=>damage,n=>damage=n);
            invoke(ModEffectEvent.DamageDealing);
            Check(damage==20 && fill.anchorMax.x==0 && status.text=="Charge: 0%", "Lua combat consumption did not reach native UI");
            language="pol";fighter.Frame=306;invoke(ModEffectEvent.Tick);
            Check(status.text.StartsWith("Ładowanie:") && button.GetComponentInChildren<Text>().text=="WZMOCNIJ NASTĘPNY CIOS","Language change did not reach native HUD");
            language="unknown";fighter.Frame=312;invoke(ModEffectEvent.Tick);
            Check(status.text.StartsWith("Charge:") && button.GetComponentInChildren<Text>().text=="ARM NEXT STRIKE","Unknown language did not fall back to English");
            invoke(ModEffectEvent.RoundEnd);
            Check(surface.IsClosed && !view.gameObject.activeSelf, "Lua round-end close retained visible Unity UI");
            button.onClick.Invoke();
            Check(surface.IsClosed, "Stale Unity button reactivated a closed Lua view");
            fields["round"]="2";invoke(ModEffectEvent.RoundBegin);
            var next=views.Last();var nextView=UnityEngine.Object.FindObjectsOfType<ModUiView>().Single();
            for(int frame=313;frame<=612;frame++){fighter.Frame=frame;invoke(ModEffectEvent.Tick);}
            nextView.transform.Find("root/arm").GetComponent<Button>().onClick.Invoke();
            Check(nextView.transform.Find("root/status").GetComponent<Text>().text=="Next hit: double damage","Native second-round bonus was not armed before destruction");
            UnityEngine.Object.DestroyImmediate(nextView.gameObject);
            damage=10;invoke(ModEffectEvent.DamageDealing);
            Check(next.IsClosed && damage==10,"Native HUD destruction retained Lua armed bonus");
            fields["round"]="3";invoke(ModEffectEvent.RoundBegin);
            next=views.Last();nextView=UnityEngine.Object.FindObjectsOfType<ModUiView>().Single();
            context.Dispose();
            Check(next.IsClosed && !nextView.gameObject.activeSelf, "Lua context disposal retained native UI");
            Check(logs.All(entry=>entry.Level!=ModLogLevel.Error), "Lua-to-Unity example logged unexpected errors");
        }
        var bridge=UnityEngine.Object.FindObjectOfType<ModUiGameBridge>();
        if(bridge!=null)UnityEngine.Object.DestroyImmediate(bridge.gameObject);
    }
    static void Bridge(ModUiNode tree)
    {
        using (var scope = new ModUiScope(ModId.Parse("example.bridge")))
        {
            int clicks=0;
            ModUiGameBridge.SetNativeBlocked(true);
            var menu=scope.Open("menu",ModUiMount.Menu,tree,_=>clicks++);
            ModUiGameBridge.Attach(menu);
            Check(!menu.CanClick("button") && !ModUiGameBridge.Route(0,true,false), "New UI bypassed existing native block");
            ModUiGameBridge.SetNativeBlocked(false);
            Check(ModUiGameBridge.BlocksGameplayInput && ModUiGameBridge.Route(0,true,false) && clicks==1,
                "Bridge did not capture/route exclusive UI");
            Eclipse.UI.TitleScreen.IsOpen=true;
            Check(!ModUiGameBridge.Route(0,true,false) && clicks==1, "Title shell did not block mod UI");
            Eclipse.UI.TitleScreen.IsOpen=false; Eclipse.UI.GameSessionRestart.IsRestarting=true;
            Check(!ModUiGameBridge.Route(0,true,false), "Restart did not suspend mod UI");
            Eclipse.UI.GameSessionRestart.IsRestarting=false;
            Check(ModUiGameBridge.Route(0,false,true) && menu.IsClosed, "Bridge Back failed");
            Check(ModUiGameBridge.BlocksGameplayInput && ModUiGameBridge.TryHandleBack(), "Closing-frame input leaked to native game");
            var modal=scope.Open("modal",ModUiMount.Modal,tree,_=>clicks++);
            ModUiGameBridge.Attach(modal);
            Check(ModUiGameBridge.TryHandleBack() && !modal.IsClosed, "Same Back event closed two overlays");
            var bridge=UnityEngine.Object.FindObjectOfType<ModUiGameBridge>();
            UnityEngine.Object.DestroyImmediate(bridge.gameObject);
            Check(modal.IsClosed && !scope.IsClosed, "Game bridge teardown retained UI");
        }
    }
    static void Coordination(ModUiNode tree, EventSystem events, GameObject prior)
    {
        var coordinator = ModUiCoordinator.Create();
        var firstScope = new ModUiScope(ModId.Parse("example.first"));
        var secondScope = new ModUiScope(ModId.Parse("example.second"));
        int firstClicks = 0, secondClicks = 0;
        events.SetSelectedGameObject(prior); events.sendNavigationEvents = true;
        var hud = firstScope.Open("hud",ModUiMount.CombatHud,tree,_=>firstClicks++);
        coordinator.Attach(hud);
        Check(coordinator.Foreground == hud && !coordinator.CapturesInput && events.currentSelectedGameObject == prior,
            "HUD stole keyboard focus");
        var menu = firstScope.Open("menu",ModUiMount.Menu,tree,_=>firstClicks++);
        coordinator.Attach(menu);
        Check(coordinator.CapturesInput && !events.sendNavigationEvents && coordinator.ActivateSelected() && firstClicks == 1,
            "Menu focus/navigation ownership failed");
        var modal = secondScope.Open("modal",ModUiMount.Modal,tree,_=>secondClicks++);
        coordinator.Attach(modal);
        Check(coordinator.Foreground == modal && !menu.TryClick("button") && coordinator.ActivateSelected() && secondClicks == 1,
            "Modal did not exclude lower layer input");
        var modals = coordinator.GetComponentsInChildren<Canvas>();
        int maxOrder = 0;
        foreach (var item in modals) maxOrder = Math.Max(maxOrder,item.sortingOrder);
        var modalRoot = coordinator.transform.Find("example.second:modal");
        Check(modalRoot.GetComponent<Canvas>().sortingOrder == maxOrder && modalRoot.Find("Backdrop").GetComponent<Image>().raycastTarget,
            "Modal sorting/backdrop missing");
        var safe = modalRoot.Find("Safe area").GetComponent<RectTransform>();
        Check(safe.anchorMin.x >= 0 && safe.anchorMax.x <= 1 && safe.anchorMin.y >= 0 && safe.anchorMax.y <= 1,
            "Safe-area anchors invalid");
        // A native dialog may disable raycasters; coordination must not turn them back on.
        var raycaster = modalRoot.GetComponent<GraphicRaycaster>(); raycaster.enabled = false;
        coordinator.SetNativeBlocked(true);
        Check(coordinator.Foreground == null && !coordinator.CapturesInput && !modalRoot.gameObject.activeSelf &&
            events.sendNavigationEvents && !coordinator.Back() && !modal.TryClick("button"), "Native block bypassed");
        coordinator.SetNativeBlocked(false);
        Check(coordinator.Foreground == modal && !raycaster.enabled, "Coordinator overwrote native raycaster state");
        raycaster.enabled = true;
        Check(coordinator.Back() && modal.IsClosed && coordinator.Foreground == menu, "Back did not restore menu");
        Check(coordinator.Back() && menu.IsClosed && coordinator.Foreground == hud && !coordinator.CapturesInput && events.sendNavigationEvents,
            "Menu close retained exclusive input");
        var otherCoordinator = ModUiCoordinator.Create();
        bool rejected = false;
        try { otherCoordinator.Attach(hud); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected && !hud.IsClosed && coordinator.Foreground == hud, "Duplicate mount damaged original owner");
        UnityEngine.Object.DestroyImmediate(otherCoordinator.gameObject);
        UnityEngine.Object.DestroyImmediate(coordinator.gameObject);
        Check(hud.IsClosed && firstScope.Count == 0 && !firstScope.IsClosed && !secondScope.IsClosed,
            "Scene coordinator teardown did not release surfaces independently of script scopes");
        firstScope.Dispose(); secondScope.Dispose();
    }
}
#endif

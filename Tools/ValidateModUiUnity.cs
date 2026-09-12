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
            var artTexture=new Texture2D(32,16);
            var artSprite=Sprite.Create(artTexture,new Rect(0,0,32,16),new Vector2(.5f,.5f));
            ModRuntime.Host.TypedAssets.Sprite=artSprite;
            var artSurface=scope.Open("art",ModUiMount.CombatHud,new ModUiNode("art",ModUiKind.Image,100,100,sprite:AssetId.Parse("example.ui:sprites/art")));
            var artView=ModUiView.Attach(artSurface,canvas.GetComponent<RectTransform>());
            var artImage=artView.transform.Find("art").GetComponent<Image>();
            Check(artImage.sprite==artSprite&&artImage.preserveAspect&&!artImage.raycastTarget,"Image lost sprite/aspect/noninteractive presentation");
            var replacementSprite=Sprite.Create(artTexture,new Rect(0,0,16,16),new Vector2(.5f,.5f));
            ModRuntime.Host.TypedAssets.Sprite=replacementSprite;
            artSurface.SetSprite("art",AssetId.Parse("example.ui:sprites/replacement"));
            Check(artImage.sprite==replacementSprite&&artImage.preserveAspect&&!artImage.raycastTarget,"Live sprite replacement lost presentation");
            Check(artView.transform.Find("art").GetComponent<Image>()==artImage&&artSprite!=null,"Sprite replacement rebuilt widget or destroyed shared sprite");
            ModRuntime.Host.TypedAssets.Sprite=null;
            artSurface.SetVisible("art",false);artSurface.SetVisible("art",true);
            Check(!artSurface.IsClosed&&artImage.sprite==replacementSprite,"Visibility update reloaded unchanged artwork");
            artSurface.SetSprite("art",AssetId.Parse("example.ui:sprites/missing"));
            Check(artSurface.IsClosed,"Missing replacement sprite left a broken open surface");
            artSurface.Close();
            Check(artSprite!=null,"Closing UI destroyed loader-owned sprite");
            UnityEngine.Object.Destroy(artSprite);UnityEngine.Object.Destroy(artTexture);
            UnityEngine.Object.Destroy(replacementSprite);
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
            var cells = new List<ModUiNode>();
            for (int i = 0; i < 6; i++) cells.Add(new ModUiNode("cell" + i, ModUiKind.Button, 0, 0, text: "Item " + i, enabled: i != 1));
            var gridSurface = scope.Open("grid", ModUiMount.Menu,
                new ModUiNode("viewport", ModUiKind.Scroll, 200, 80, children: new[] {
                    new ModUiNode("grid", ModUiKind.Grid, 200, 140, columns: 2, cellWidth: 90, cellHeight: 40, gap: 10, children: cells)
                }), _ => clicks++);
            gridSurface.SetInputAllowed(true);
            var gridView = ModUiView.Attach(gridSurface, canvas.GetComponent<RectTransform>());
            Canvas.ForceUpdateCanvases();
            var gridScroll = gridView.GetComponentInChildren<ScrollRect>();
            var gridLayout = gridView.GetComponentInChildren<GridLayoutGroup>();
            var firstCell = gridLayout.transform.Find("cell0").GetComponent<RectTransform>();
            var secondCell = gridLayout.transform.Find("cell1").GetComponent<RectTransform>();
            var thirdCell = gridLayout.transform.Find("cell2").GetComponent<RectTransform>();
            Check(firstCell.rect.size == new Vector2(90,40) && secondCell.anchoredPosition.x-firstCell.anchoredPosition.x == 100,
                "Grid cell width/gap did not override child sizing");
            Check(firstCell.anchoredPosition.y == secondCell.anchoredPosition.y && firstCell.anchoredPosition.y-thirdCell.anchoredPosition.y == 50,
                "Grid did not wrap after fixed column count");
            Check(firstCell.GetComponent<Image>().sprite?.name == "CommonButtons.BtnWhite" && firstCell.GetComponentInChildren<Text>().font.name == "AGOpusBold",
                "Grid discarded original widget styling");
            Check(gridView.MoveFocus(1) && events.currentSelectedGameObject == firstCell.gameObject,"Grid first focus failed");
            Check(gridView.MoveFocus(1) && events.currentSelectedGameObject == thirdCell.gameObject,"Grid focus did not skip disabled cell");
            gridView.MoveFocus(1); gridView.MoveFocus(1);
            Check(events.currentSelectedGameObject.name == "cell4" && gridScroll.content.anchoredPosition.y > 0,
                "Keyboard focus did not reveal lower grid row");
            Check(gridView.ActivateSelected(),"Grid selected button did not activate");
            Check(gridView.NavigateFocus(1,0) && events.currentSelectedGameObject.name == "cell5", "Grid right did not follow row");
            Check(!gridView.NavigateFocus(1,0) && events.currentSelectedGameObject.name == "cell5", "Grid right wrapped at edge");
            Check(gridView.NavigateFocus(0,-1) && events.currentSelectedGameObject.name == "cell3", "Grid up did not follow column");
            Check(gridView.NavigateFocus(-1,0) && events.currentSelectedGameObject.name == "cell2", "Grid left did not follow row");
            Check(gridView.NavigateFocus(0,-1) && events.currentSelectedGameObject.name == "cell0" && gridScroll.content.anchoredPosition.y == 0,
                "Grid up did not reveal first row");
            Check(!gridView.NavigateFocus(1,0) && events.currentSelectedGameObject.name == "cell0", "Grid entered disabled neighbor or jumped rows");
            gridSurface.SetEnabled("cell1",true);
            Check(gridView.NavigateFocus(1,0) && events.currentSelectedGameObject.name == "cell1", "Enabled grid neighbor remained unreachable");
            Check(gridView.NavigateFocus(0,1) && events.currentSelectedGameObject.name == "cell3", "Grid down advanced in linear order");
            gridSurface.Close();
            Check(events.currentSelectedGameObject == prior && !gridView.gameObject.activeSelf,"Grid close left focus or view behind");
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
            int changes = 0;
            var controls = scope.Open("controls", ModUiMount.Menu, new ModUiNode("root",ModUiKind.Column,400,160,children:new[]{
                new ModUiNode("toggle",ModUiKind.Toggle,400,48,text:"Challenge"),
                new ModUiNode("slider",ModUiKind.Slider,400,48,value:.5)
            }),onChange:(_,__)=>changes++);
            var controlsView = ModUiView.Attach(controls,canvas.GetComponent<RectTransform>());
            var toggle = controlsView.GetComponentInChildren<Toggle>();
            var slider = controlsView.GetComponentInChildren<Slider>();
            Check(toggle.targetGraphic.GetComponent<Image>().sprite?.name=="MiscSprites.checkboxOff" &&
                toggle.graphic.GetComponent<Image>().sprite?.name=="MiscSprites.checkboxOn", "Original checkbox skin missing");
            Check(slider.targetGraphic.GetComponent<Image>().sprite?.name=="SlidersSettings.slider" &&
                slider.fillRect.GetComponent<Image>().sprite?.name=="SlidersSettings.full", "Original settings slider skin missing");
            Check(controlsView.MoveFocus(1) && events.currentSelectedGameObject==toggle.gameObject && controlsView.ActivateSelected() && toggle.isOn,"Toggle focus/submit failed");
            Check(controlsView.MoveFocus(1) && events.currentSelectedGameObject==slider.gameObject && controlsView.AdjustSelected(1) && Math.Abs(slider.value-.55)<.0001,"Slider focus/adjust failed");
            controls.SetValue("slider",.25); controls.SetChecked("toggle",false);
            Check(changes==2 && !toggle.isOn && slider.value==.25,"Setters echoed changes or failed to render");
            toggle.isOn=true; slider.value=.75f;
            Check(changes==4 && controls.Read("toggle").Value==1 && controls.Read("slider").Value==.75,"Pointer value changes did not reach model");
            controls.SetValue("slider",1);
            Check(controlsView.NavigateFocus(1,0) && events.currentSelectedGameObject==slider.gameObject && slider.value==1,
                "Slider endpoint let horizontal input escape to navigation");
            controls.SetValue("slider",.75);
            controls.SetInputAllowed(false); toggle.isOn=false; slider.value=1;
            Check(changes==4 && toggle.isOn && slider.value==.75,"Blocked control did not restore authoritative state");
            controls.Close();
            scope.Dispose();
            Placement(tree, canvas.GetComponent<RectTransform>());
            Coordination(tree, events, prior);
            Bridge(tree);
            LuaRoundTrip();
            SceneMenuRoundTrip();
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
            var tree=new ModUiNode("root",ModUiKind.Column,480,400,gap:16,children:new[]{
                new ModUiNode("title",ModUiKind.Text,480,64,text:"Custom battle rules",style:new ModUiStyle(fontSize:32)),
                new ModUiNode("description",ModUiKind.Text,480,48,text:"Charge your next strike"),
                new ModUiNode("meter",ModUiKind.Progress,480,24,value:.65),
                new ModUiNode("challenge",ModUiKind.Toggle,480,48,text:"Challenge rules",value:1),
                new ModUiNode("intensity",ModUiKind.Slider,480,48,value:.65),
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
    static void SceneMenuRoundTrip()
    {
        string root=Path.GetFullPath(Path.Combine(Application.dataPath,".."));
        var mod=ModDiscovery.DiscoverLoose(Path.Combine(root,"SceneMods")).Mods.Single();
        var catalog=new ModContentCatalog();
        var assets=new AssetResolver(new IAssetProvider[]{new LooseModProvider(mod)});
        var surfaces=new List<ModUiSurface>();
        var errors=new List<string>();
        var bus=new ModStoryEvents((id,error)=>errors.Add(error));
        int requests=0;bool accepted=false;string destination=null;
        ModSceneAccess.Open=name=>{requests++;destination=name;return accepted;};
        var runtime=new MoonSharpScriptRuntime(surface=>{surfaces.Add(surface);ModUiGameBridge.Attach(surface);},null,null,bus);
        var eventSystem=EventSystem.current;
        bool nativeNavigation=eventSystem.sendNavigationEvents;
        try
        {
            using(var registration=catalog.BeginRegistration(mod))
            using(var context=runtime.CreateContext(mod,new ModApiFacade(mod,assets,registration,new ModStateRuntime(),null)))
            {
                context.ExecuteEntrypoint();registration.Commit();bus.BindProfile();
                bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:"map"));
                var surface=surfaces.Single();
                var view=UnityEngine.Object.FindObjectsOfType<ModUiView>().Single();
                Canvas.ForceUpdateCanvases();
                Check(view.GetComponent<Image>().sprite?.name=="DialogScroll.Background_Center","Scene Menu parchment missing");
                var buttons=view.GetComponentsInChildren<Button>();
                Check(buttons.Length==5,"Scene Menu button count");
                Check(buttons.All(button=>button.GetComponent<Image>().sprite?.name=="CommonButtons.BtnWhite"),"Scene Menu button skin differs from game");
                Check(view.GetComponentsInChildren<Text>().All(text=>text.font!=null&&text.font.name=="AGOpusBold"),"Scene Menu font differs from game");
                var parent=view.transform.Find("root").GetComponent<RectTransform>();
                foreach(var button in buttons)
                {
                    var corners=new Vector3[4];button.GetComponent<RectTransform>().GetWorldCorners(corners);
                    Check(corners.All(point=>{
                        var local=parent.InverseTransformPoint(point);var bounds=parent.rect;
                        return local.x>=bounds.xMin-.01f&&local.x<=bounds.xMax+.01f&&
                            local.y>=bounds.yMin-.01f&&local.y<=bounds.yMax+.01f;
                    }),"Scene Menu button escapes root: "+button.name);
                }
                var shop=view.transform.Find("root/shop").GetComponent<Button>();
                shop.onClick.Invoke();
                Check(requests==1&&destination=="shop"&&!surface.IsClosed,"Rejected native navigation closed menu or wrong request");
                Check(view.transform.Find("root/status").GetComponent<Text>().text=="Unavailable right now","Native rejection label did not update");
                ModUiGameBridge.SetNativeBlocked(true);
                shop.onClick.Invoke();
                Check(requests==1&&!surface.IsClosed&&!ModUiGameBridge.Route(0,true,false),"Native dialog block bypassed by menu");
                ModUiGameBridge.SetNativeBlocked(false);accepted=true;
                for(int i=0;i<5&&eventSystem.currentSelectedGameObject!=shop.gameObject;i++)
                    ModUiGameBridge.Route(1,false,false);
                Check(eventSystem.currentSelectedGameObject==shop.gameObject,"Scene Menu directional input could not select SHOP");
                Check(ModUiGameBridge.Route(0,true,false),"Scene Menu submit was not routed");
                Check(requests==2&&surface.IsClosed,"Accepted button did not close Lua/native menu");
                Check(ModUiGameBridge.BlocksGameplayInput,"Navigation closing frame leaked input");

                bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:"shop"));
                var second=surfaces.Last();var bridge=UnityEngine.Object.FindObjectOfType<ModUiGameBridge>();
                UnityEngine.Object.DestroyImmediate(bridge.gameObject);
                Check(second.IsClosed,"Scene coordinator destruction retained menu");
                bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:"profile"));
                Check(surfaces.Count==3&&!surfaces.Last().IsClosed,"Live script could not remount after scene teardown");
                bus.Publish(new ModStoryEvent(ModStoryEventKind.SceneEnter,null,scene:"fight"));
                Check(surfaces.Count==3,"Scene Menu opened during combat");
                Check(errors.Count==0,"Scene Menu callback failure: "+string.Join("; ",errors));
            }
            Check(surfaces.All(surface=>surface.IsClosed),"Unloaded script retained scene menu");
            Check(!bus.HasSubscribers(ModStoryEventKind.SceneEnter),"Unloaded script retained scene listener");
            Check(eventSystem.sendNavigationEvents==nativeNavigation&&!UnityEngine.Object.FindObjectOfType<ModUiCoordinator>().CapturesInput,
                "Scene Menu teardown retained navigation ownership");
        }
        finally
        {
            ModSceneAccess.Clear();bus.Clear();ModUiGameBridge.SetNativeBlocked(false);
            var bridge=UnityEngine.Object.FindObjectOfType<ModUiGameBridge>();
            if(bridge!=null)UnityEngine.Object.DestroyImmediate(bridge.gameObject);
        }
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
            var grid=scope.Open("grid_navigation",ModUiMount.Modal,
                new ModUiNode("grid",ModUiKind.Grid,200,100,columns:2,cellWidth:90,cellHeight:40,gap:10,
                    children:Enumerable.Range(0,4).Select(i=>new ModUiNode("nav"+i,ModUiKind.Button,0,0,text:i.ToString()))),_=>{});
            ModUiGameBridge.Attach(grid);
            Check(EventSystem.current.currentSelectedGameObject.name=="nav0","Bridge grid initial focus missing");
            Check(ModUiGameBridge.Route(1,false,false,sequential:false) && EventSystem.current.currentSelectedGameObject.name=="nav2",
                "Bridge arrow route did not use grid geometry");
            Check(ModUiGameBridge.Route(0,false,false,1) && EventSystem.current.currentSelectedGameObject.name=="nav3",
                "Bridge horizontal route did not navigate grid");
            Check(ModUiGameBridge.Route(-1,false,false) && EventSystem.current.currentSelectedGameObject.name=="nav2",
                "Bridge reverse sequential route did not preserve Tab order");
            grid.Close();
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

// Run with unity command eval_file --file Tools/Tests/Presentation/VerifyPresentationLayouts.cs.
// Requires a loaded campaign. Creates only a temporary result UI; never presses OK
// or applies an enchantment, so it grants no rewards and spends no materials.
if (!Application.isPlaying || ListSF.CCDKHLAMKKO() == null) throw new Exception("Load a campaign first.");
var checks=new System.Collections.Generic.List<string>();
Action<bool,string> check=(ok,name)=>{if(!ok)throw new Exception(name);checks.Add(name);};
var root=new GameObject("Presentation verification",typeof(RectTransform),typeof(Canvas),typeof(UnityEngine.UI.CanvasScaler));
root.hideFlags=HideFlags.DontSave;
try {
 var canvas=root.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;
 var scaler=root.GetComponent<UnityEngine.UI.CanvasScaler>();
 scaler.uiScaleMode=UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(2730,1536);scaler.matchWidthOrHeight=1;
 var screen=UnityEngine.Object.Instantiate(Resources.Load<GameObject>("prefabs/fight/display/screens/EndFightScreen"),root.transform,false).GetComponent<Nekki.SF2.GUI.Fight.EndFightScreen>();
 var result=new FightResult {MHNEKAEGNBO=GameOverTypes.GAME_OVER_WIN,AIOMDIAFHGB=new ComboStatistic(),NJNKGLJNNDH=100};
 result.AIOMDIAFHGB.ECOOCLMNFJM.PDJPOBHLIHA=12345;
 result.AIOMDIAFHGB.ECOOCLMNFJM.POPNFGNAOJD=12345;
 screen.Init(result);
 screen.OnAnimationFinishButton();
 Canvas.ForceUpdateCanvases();
 var hidden=System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic;
 foreach(var reveal in screen.GetComponentsInChildren<Eclipse.UI.UiReveal>()) {
  var rect=(RectTransform)reveal.transform;var placed=rect.anchoredPosition;var scale=rect.localScale;
  typeof(Eclipse.UI.UiReveal).GetMethod("Finish",hidden).Invoke(reveal,null);
  check(rect.anchoredPosition==placed,"Reveal preserves layout placement: "+rect.name);
 }
 foreach(var label in screen.GetComponentsInChildren<Nekki.SF2.GUI.LabelAliasLE>()) {
  check(label.rectTransform.rect.width>0,"Measured counter width: "+label.text);
  check(label.horizontalOverflow==HorizontalWrapMode.Overflow && label.verticalOverflow==VerticalWrapMode.Overflow,"Single-line counter is not clipped: "+label.text);
 }
 foreach(var row in screen.GetComponentsInChildren<Nekki.SF2.GUI.Fight.TextAndMoneyLine>()) {
  var label=row.transform.Find("Text").GetComponent<UnityEngine.UI.Text>();
  check(label.rectTransform.rect.width>=label.preferredWidth,"Localized reward label fits: "+label.text);
 }
 var background=(RectTransform)screen.transform.Find("Background");
 check(background.anchorMin==Vector2.zero && background.anchorMax==Vector2.one && background.sizeDelta==Vector2.zero,"Result shadow fills viewport");
 var ok=(RectTransform)screen.transform.Find("Content/EndFightContent(Clone)/ButtonLine/LabelButton");
 check(ok.anchoredPosition==Vector2.zero,"OK is centered under rewards");
 // Native font geometry, not just the strings: force a graphic rebuild and inspect it.
 foreach(var label in screen.GetComponentsInChildren<Nekki.SF2.GUI.LabelAliasLE>()) {
  label.SetVerticesDirty();label.Rebuild(UnityEngine.UI.CanvasUpdate.PreRender);
  check(label.cachedTextGenerator.vertexCount>0,"Reward counter emits glyphs: "+label.text);
 }
 foreach(var side in new[]{"left","right"}) {
  var sprite=Nekki.SF2.GUI.ResolutionImage.GetSprite("Textures/Logos/","Logo."+side);
  check(sprite!=null,"Loader logo resolves: "+side);
  // This assertion deliberately checks the active DE128 replacement pack.
  check(sprite.rect.width==1 && sprite.texture.GetPixel(0,0).a==0,"Baked-logo splash has no second logo: "+side);
 }
 var shop=Nekki.SF2.GUI.Shop.ShopScene.get_Instance();
 if(shop!=null) {
  var backdrop=shop.GetComponent<Eclipse.Rendering.ShopDojoBackdrop>();
  check(backdrop!=null,"Shop owns native dojo scenery");
  check(Eclipse.Rendering.LocationAtmosphere.CurrentLocationName==Eclipse.Modding.ModRuntime.ResolveDojoLocation(GameUtils.NIPABEEAMHJ),"Shop matches selected dojo");
  check(UnityEngine.Camera.main.GetComponent<EclipseScreenEffects>()!=null,"Shop camera supports mod screen effects");
  var forge=(Eclipse.Forge.ShopForgeController)typeof(Nekki.SF2.GUI.Shop.ShopScene).GetField("_forgeController",hidden).GetValue(shop);
  if(forge.IsOpen) {
   forge.UpdateDrawerPosition();
   var drawer=(RectTransform)shop.transform.Find("ShopUIGroup/ForgePanel");
   var paper=(RectTransform)shop.transform.Find("ShopUIGroup/SidePanels/ItemPropertiesPanel/Panel/InfoPanel");
   var a=UnityEngine.RectTransformUtility.CalculateRelativeRectTransformBounds(drawer.parent,drawer);
   var corners=new Vector3[4];paper.GetWorldCorners(corners);float left=float.PositiveInfinity;
   foreach(var corner in corners)left=Mathf.Min(left,drawer.parent.InverseTransformPoint(corner).x);
   check(drawer.localScale.x>0 && drawer.localScale.y>0,"Drawer is upright beside rotated enchantment panel");
   check(a.max.x<=left+1f,"Recipe drawer does not overlap current enchantments");
  }
 }
 return new {passed=checks.Count,checks};
} finally {UnityEngine.Object.Destroy(root);}


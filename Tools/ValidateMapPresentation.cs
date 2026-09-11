using System;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Map;
using Nekki.SF2.GUI.Menu;

// Isolated native fixture: real presentation components and original imported art.
public class SFMonoBehaviour<T> : MonoBehaviour { }
public class Pair<T,U> { public T First; public U Second; public Pair(T a,U b) { First=a; Second=b; } }
public class LabelAlias : Text { public void SetAlias(string value) { text=value; } }
public class ProgressBar : MonoBehaviour {
    public ResolutionImage Background, Stripe;
    public void SetValueBorders(float a,float b) {} public void SetValue(float value) { Stripe.fillAmount=1; }
}
public static class Constants { public static string[] DNDKOMGCBLC = {"DifficultyBars.very_easy","DifficultyBars.easy","DifficultyBars.middle","DifficultyBars.hard","DifficultyBars.very_hard"}; }
public static class XmlHelpers {
    public static string CIPOICEEIBK(this XmlAttribute a,string fallback) { return a == null ? fallback : a.Value; }
    public static float ParseFloat(this XmlAttribute a) { return float.Parse(a.Value,System.Globalization.CultureInfo.InvariantCulture); }
}
public static class ResourcesAndBundles { public static T Load<T>(string path) where T:UnityEngine.Object { return Resources.Load<T>(path); } }
public static class AtlasCache { public static Sprite GetSpriteFromAtlas(string path,string name) { return null; } }
public class GameCurrency { public string MJBPMLCLMFN="ComboButtons.base_damage"; }
public class Roster { public int GetCurrencyCount(GameCurrency c) { return int.MaxValue; } }
public static class ListSF { public static Roster CCDKHLAMKKO() { return new Roster(); } }
public static class LocalizationManager { public static Font MBPJIKFOEBJ() { return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); } }
public static class ValidateMapPresentation {
    static void Require(bool ok,string message) { if(!ok) throw new Exception(message); }
    static void Field(object target,string name,object value) { target.GetType().GetField(name,BindingFlags.NonPublic|BindingFlags.Instance).SetValue(target,value); }
    public static void Run() {
      try {
        var canvas = new GameObject("Canvas",typeof(Canvas));
        var root = new GameObject("InactiveDifficulty",typeof(RectTransform)); root.transform.SetParent(canvas.transform,false); root.SetActive(false);
        ((RectTransform)root.transform).sizeDelta=new Vector2(501,40);
        var bar=root.AddComponent<ProgressBar>();
        var bg=new GameObject("Background",typeof(RectTransform)); bg.transform.SetParent(root.transform,false); bar.Background=bg.AddComponent<ResolutionImage>(); bar.Background.set_TexturePath("UI/Atlases/");
        var fg=new GameObject("Stripe",typeof(RectTransform)); fg.transform.SetParent(root.transform,false); bar.Stripe=fg.AddComponent<ResolutionImage>(); bar.Stripe.set_TexturePath("UI/Atlases/");
        var panel=root.AddComponent<DifficultyPanel>(); var label=new GameObject("Label",typeof(RectTransform)).AddComponent<LabelAlias>();
        Field(panel,"_difficultyBar",bar); Field(panel,"_difficultyLabel",label);
        DifficultyPanel.get_DifficultyEvaluation().Clear();
        for(int i=0;i<5;i++) DifficultyPanel.get_DifficultyEvaluation().Add(new Pair<string,float>("Level"+i,i));
        float previous=0;
        for(int i=0;i<5;i++) {
          panel.Init(i+0.5f);
          float width=bar.Stripe.rectTransform.anchorMax.x;
          Require(width>previous && width<=1,"Difficulty levels must increase in width, including before Awake: "+i+" width="+width);
          Require(bar.Background.sprite!=null,"Background unavailable before Awake");
          Require(bar.Stripe.sprite.uv.Length>=4,"Native UVs missing"); previous=width;
        }
        root.SetActive(true); Canvas.ForceUpdateCanvases();
        Require(ResolutionImage.GetSprite("UI/Atlases/","ComboButtons.base_damage")!=null,"Damage glyph missing");
        Require(ResolutionImage.GetSprite("UI/Items/","img_video")!=null,"Video button missing");
        foreach(string name in new[]{"shadow_gate","Shadow_fight_ending"}) {
          var clip=Resources.Load<VideoClip>("gamedata/video/"+name);
          Require(clip!=null && clip.frameCount>0 && clip.width>0,"Video import failed: "+name);
          Debug.Log("[MapPresentation] Video "+name+" frames="+clip.frameCount+" audioTracks="+clip.audioTrackCount);
        }
        var cell=new GameObject("Orbs",typeof(RectTransform)); cell.transform.SetParent(canvas.transform,false); ((RectTransform)cell.transform).sizeDelta=new Vector2(250,100);
        var value=new GameObject("Value",typeof(RectTransform)).AddComponent<Text>(); value.transform.SetParent(cell.transform,false);
        var orb=cell.AddComponent<MenuMaterSprite>(); Field(orb,"_valueLbl",value); orb.Init(new GameCurrency());
        Canvas.ForceUpdateCanvases(); orb.SendMessage("LateUpdate");
        Require(value.text==int.MaxValue.ToString(),"Orb count truncated");
        Require(value.preferredWidth<=value.rectTransform.rect.width+1,"Orb count overlaps next cell");
        Debug.Log("[MapPresentation] PASS: inactive difficulty layout, increasing stripe widths, native glyphs, ending imports, ten-digit orb fit."); EditorApplication.Exit(0);
      } catch(Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
    }
}

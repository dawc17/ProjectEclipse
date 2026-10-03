using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Eclipse.Multiplayer;
using Nekki.SF2.GUI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class PvpHealthBarNative
{
    const string Pending="Eclipse.PvpHealthBarNative.Pending";
    static int checks;
    static void Check(bool ok,string label) { checks++; if(!ok)throw new Exception(label); }
    static PvpHealthBarNative() { if(SessionState.GetBool(Pending,false))EditorApplication.update+=Begin; }
    public static void Run()
    {
        SessionState.SetBool(Pending,true);
        EditorApplication.update+=Begin;
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        EditorApplication.EnterPlaymode();
    }
    static void Begin()
    {
        if(!EditorApplication.isPlaying || EditorApplication.isCompiling)return;
        EditorApplication.update-=Begin;SessionState.SetBool(Pending,false);
        new GameObject("Recovery validation").AddComponent<PvpHealthBarNativeRunner>();
    }
    public static IEnumerator Steps()
    {
            var root=new GameObject("Health canvas",typeof(RectTransform),typeof(Canvas));
            var canvas=root.GetComponent<Canvas>(); canvas.renderMode=RenderMode.WorldSpace;
            root.GetComponent<RectTransform>().sizeDelta=new Vector2(400,100); root.transform.localScale=Vector3.one*.01f;
            var camera=new GameObject("Health camera").AddComponent<Camera>(); camera.transform.position=new Vector3(0,0,-10);
            camera.orthographic=true;camera.orthographicSize=.5f;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=Color.black;
            var texture=new RenderTexture(400,100,24);texture.Create();camera.targetTexture=texture;
            canvas.worldCamera=camera;
            var source=new GameObject("Life",typeof(RectTransform)).AddComponent<ResolutionImageSkew>();source.transform.SetParent(root.transform,false);
            source.rectTransform.sizeDelta=new Vector2(320,60);source.type=Image.Type.Filled;source.fillMethod=Image.FillMethod.Horizontal;
            source.fillAmount=.5f;source.color=Color.red;source.set_SkewAngle(12);
            var orange=new Texture2D(8,4);var atlasPixels=new Color[32];for(int i=0;i<atlasPixels.Length;i++)atlasPixels[i]=new Color(1,.3f,.05f);orange.SetPixels(atlasPixels);orange.Apply();
            source.sprite=Sprite.Create(orange,new Rect(2,1,4,2),new Vector2(.5f,.5f),1);
            source.gameObject.AddComponent<Eclipse.Rendering.Interpolation.TickPresentationSmoother>();
            var parameters=new ModelParameters {RecoverableLife=.25f};
            PvpRecoverableBar.Attach(source,parameters);
            Check(root.transform.childCount==1,"Campaign bar unchanged");
            Fight.Current.IsLocalVersus=true;PvpRecoverableBar.Attach(source,parameters);
            Check(root.transform.childCount==2,"PvP adds one recovery bar");
            var grey=root.transform.GetChild(0).GetComponent<ResolutionImageSkew>();
            Check(grey!=source && source.transform.GetSiblingIndex()>grey.transform.GetSiblingIndex(),"Live health renders above grey");
            Check(grey.sprite==source.sprite && grey.get_SkewAngle()==12 && grey.fillMethod==source.fillMethod,"Native skew/sprite/fill preserved");
            Check(grey.material.shader.name=="Eclipse/UI/Recoverable Health" && !ShaderUtil.ShaderHasError(grey.material.shader),"Production grey shader compiles");
            Check(!grey.raycastTarget && !grey.GetComponent<Eclipse.Rendering.Interpolation.TickPresentationSmoother>().enabled,"Recovery presentation ignores input and live smoothing");
            var display=source.GetComponent<PvpRecoverableBar>();
            Action update=()=>typeof(PvpRecoverableBar).GetMethod("LateUpdate",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(display,null);
            update();Check(grey.fillAmount==1 && Math.Abs(grey.material.GetVector("_Segment").y-.75f)<.000001f,"Full native shape clipped to recoverable segment");
            source.color=new Color(1,0,0,.35f);
            update();
            Check(CurrentHealthHasNoGrey(camera,texture),"Transparent current health never exposes grey underneath");
            Check(CountGrey(camera,texture)>1000,"Native rendered grey segment is visible");
            var center=Sample(camera,texture,240,50);
            var rim=Sample(camera,texture,240,78);
            Check(center.r>.15f && center.r<.35f,"Recovery middle is translucent");
            Check(rim.r>center.r*2,"Recovery top outline is clearer than the middle");
            Check(Sample(camera,texture,240,21).r>center.r*2,"Recovery bottom outline is clearer than the middle");
            // The recovered mesh shears the top left by height*tan(angle),
            // so the midpoint boundaries are about x=194 and x=274 here.
            Check(Sample(camera,texture,194,50).r<center.r*.7f,"Recovery start fades into live edge");
            Check(Sample(camera,texture,272,50).r<center.r*.7f,"Recovery end fades into missing health");
            source.fillOrigin=1;update();
            Check(Sample(camera,texture,160,78).r>Sample(camera,texture,160,50).r*2 && Sample(camera,texture,192,50).r<Sample(camera,texture,160,50).r*.7f,"Right-origin recovery retains outline and fade");
            source.fillOrigin=0;update();
            int centeredCount=CountGrey(camera,texture);
            source.rectTransform.anchoredPosition=new Vector2(130,80);
            grey.rectTransform.anchoredPosition=source.rectTransform.anchoredPosition;
            camera.transform.position=new Vector3(1.3f,.8f,-10);
            update();
            int translatedCount=CountGrey(camera,texture);
            Check(Math.Abs(translatedCount-centeredCount)<100,"Translated HUD must retain recoverable pixels: "+translatedCount+" vs "+centeredCount);
            source.rectTransform.anchoredPosition=Vector2.zero;grey.rectTransform.anchoredPosition=Vector2.zero;
            camera.transform.position=new Vector3(0,0,-10);update();
            // Actual ViewerFight layout: 564x43, 25-degree skew, displaced
            // +/-670 pixels horizontally and 585 vertically from the canvas origin.
            source.rectTransform.sizeDelta=grey.rectTransform.sizeDelta=new Vector2(564,43);
            source.set_SkewAngle(25);grey.set_SkewAngle(25);camera.orthographicSize=.8f;
            source.rectTransform.anchoredPosition=grey.rectTransform.anchoredPosition=new Vector2(-670,585);
            camera.transform.position=new Vector3(-6.7f,5.85f,-10);update();
            int hostCount=CountGrey(camera,texture);
            Check(hostCount>1000 && CurrentHealthHasNoGrey(camera,texture),"Actual host HUD placement displays grey beside live health");
            source.rectTransform.anchoredPosition=grey.rectTransform.anchoredPosition=new Vector2(670,585);
            source.fillOrigin=1;camera.transform.position=new Vector3(6.7f,5.85f,-10);update();
            int guestCount=CountGrey(camera,texture);
            Check(guestCount>1000 && Math.Abs(guestCount-hostCount)<100 && CurrentHealthHasNoGrey(camera,texture,true),"Actual guest HUD placement displays the same recoverable amount");
            source.rectTransform.anchoredPosition=grey.rectTransform.anchoredPosition=Vector2.zero;
            source.rectTransform.sizeDelta=grey.rectTransform.sizeDelta=new Vector2(320,60);
            source.set_SkewAngle(12);grey.set_SkewAngle(12);source.fillOrigin=0;
            camera.transform.position=new Vector3(0,0,-10);camera.orthographicSize=.5f;update();
            var maskObject=new GameObject("HUD mask",typeof(RectTransform),typeof(Image),typeof(Mask));
            maskObject.transform.SetParent(root.transform,false);maskObject.GetComponent<RectTransform>().sizeDelta=new Vector2(400,100);
            maskObject.GetComponent<Mask>().showMaskGraphic=false;
            source.transform.SetParent(maskObject.transform,false);grey.transform.SetParent(maskObject.transform,false);update();
            int maskedCount=CountGrey(camera,texture);
            Check(maskedCount>1000,"Stencil-masked recovery segment visible");
            parameters.RecoverableLife=.1f;update();
            int updatedMaskedCount=CountGrey(camera,texture);
            Check(updatedMaskedCount>500 && updatedMaskedCount<maskedCount*.6f,"Cached stencil material follows changing recovery amount");
            parameters.RecoverableLife=.25f;
            source.transform.SetParent(root.transform,false);grey.transform.SetParent(root.transform,false);
            UnityEngine.Object.DestroyImmediate(maskObject);update();


            root.transform.localScale=new Vector3(-.01f,.01f,.01f);
            Check(CountGrey(camera,texture)>1000,"Mirrored native health still shows recovery");
            // Re-init reuses the existing component and redirects its parameters.
            var replacement=new ModelParameters {RecoverableLife=.1f};PvpRecoverableBar.Attach(source,replacement);
            Check(root.transform.childCount==2,"Repeated init does not duplicate bars");
            update();Check(Math.Abs(grey.material.GetVector("_Segment").y-.6f)<.000001f,"Re-init binds current fighter");
            replacement.RecoverableLife=0;update();
            Check(!grey.enabled && CountGrey(camera,texture)==0,"Empty pool renders no grey");
            replacement.RecoverableLife=.2f;replacement.CurrentHealthBarFraction=.9f;source.fillAmount=.9f;update();Check(grey.material.GetVector("_Segment").y==1,"Presentation caps full health");
            root.transform.localScale=Vector3.one*.01f;source.fillOrigin=1;source.fillAmount=.5f;replacement.CurrentHealthBarFraction=.5f;update();
            Check(CountGrey(camera,texture)>1000 && CurrentHealthHasNoGrey(camera,texture,true),"Right-origin fill clips the recovery segment correctly");
            source.fillOrigin=0;source.fillAmount=.8f;replacement.CurrentHealthBarFraction=.5f;replacement.RecoverableLife=.25f;update();
            Check(!grey.enabled,"Native damage animation reveals grey only after the live edge reaches it");
            source.enabled=false;update();Check(!grey.enabled,"Disabled health hides recovery bar");
            var material=grey.material;
            UnityEngine.Object.Destroy(source.gameObject);
            yield return null;
            yield return null;
            Check(root.transform.childCount==0,"Destroying live bar cleans up recovery clone");
            Check(material==null,"Recovery material cleaned up");
            // Destroy uses Unity's delayed lifetime; the bar shares its owning canvas at teardown.
            UnityEngine.Object.DestroyImmediate(root);UnityEngine.Object.DestroyImmediate(camera.gameObject);UnityEngine.Object.DestroyImmediate(texture);
            UnityEngine.Object.DestroyImmediate(orange);
            File.WriteAllText(Path.Combine(Application.dataPath,"../validation-result.txt"),"PASS: "+checks+" native PvP health bar checks (controlled health and atlas base; production skew mesh; no game playtest).");
            EditorApplication.Exit(0);
    }
    static bool CurrentHealthHasNoGrey(Camera camera,RenderTexture texture,bool rightOrigin=false)
    {
        Canvas.ForceUpdateCanvases();camera.Render();
        var pixels=new Texture2D(texture.width,texture.height,TextureFormat.RGBA32,false);
        var before=RenderTexture.active;RenderTexture.active=texture;pixels.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0);pixels.Apply();RenderTexture.active=before;
        // Sample well inside the live segment, including its translucent artwork.
        var color=pixels.GetPixel(rightOrigin ? 290 : 100,50);
        UnityEngine.Object.DestroyImmediate(pixels);
        return color.r>.1f && color.g<.01f && color.b<.01f;
    }
    static int CountGrey(Camera camera,RenderTexture texture)
    {
        Canvas.ForceUpdateCanvases();camera.Render();
        var pixels=new Texture2D(texture.width,texture.height,TextureFormat.RGBA32,false);
        var before=RenderTexture.active;RenderTexture.active=texture;pixels.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0);pixels.Apply();RenderTexture.active=before;
        int result=0;foreach(var c in pixels.GetPixels32())if(c.r>20 && Math.Abs(c.r-c.g)<4 && Math.Abs(c.r-c.b)<4)result++;
        UnityEngine.Object.DestroyImmediate(pixels);return result;
    }
    static Color Sample(Camera camera,RenderTexture texture,int x,int y)
    {
        Canvas.ForceUpdateCanvases();camera.Render();
        var pixels=new Texture2D(texture.width,texture.height,TextureFormat.RGBA32,false);
        var before=RenderTexture.active;RenderTexture.active=texture;
        pixels.ReadPixels(new Rect(0,0,texture.width,texture.height),0,0);pixels.Apply();RenderTexture.active=before;
        var color=pixels.GetPixel(x,y);UnityEngine.Object.DestroyImmediate(pixels);return color;
    }
}

public sealed class PvpHealthBarNativeRunner : MonoBehaviour
{
    IEnumerator Start()
    {
        var steps=PvpHealthBarNative.Steps();
        while(true)
        {
            object current;
            try { if(!steps.MoveNext())yield break;current=steps.Current; }
            catch(Exception error) {Debug.LogException(error);EditorApplication.Exit(1);yield break;}
            yield return current;
        }
    }
}

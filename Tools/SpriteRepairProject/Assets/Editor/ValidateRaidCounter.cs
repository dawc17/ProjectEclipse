using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Nekki.SF2.GUI.Fight;

public static class ValidateRaidCounter
{
    public static void Run()
    {
        try
        {
            var font = AssetDatabase.LoadAssetAtPath<Font>("Assets/NavigationTestRuntime/majallab.ttf");
            if (font == null) throw new InvalidOperationException("Copy the production majallab font and importer first.");
            var canvasObject = new GameObject("Counter test canvas", typeof(Canvas));
            var canvas = canvasObject.GetComponent<Canvas>();
            var cameraObject = new GameObject("Counter test camera", typeof(Camera));
            var camera = cameraObject.GetComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 48;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.1f, 0.05f, 0.02f);
            var target = new RenderTexture(256, 96, 24);
            camera.targetTexture = target;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 100;
            var parent = new GameObject("Enemy bar", typeof(RectTransform));
            parent.layer = 5;
            parent.transform.SetParent(canvas.transform, false);
            ((RectTransform)parent.transform).sizeDelta = new Vector2(564, 43);
            parent.transform.localRotation = Quaternion.Euler(0, 179.36f, 0);
            var model = new ModelParameters { ShieldTotal = 40, RemainingHealthBars = 14 };
            var counter = RaidShieldBar.Attach(parent.transform, model, font);
            var text = counter.GetComponentInChildren<Text>();
            Canvas.ForceUpdateCanvases();
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            int oldVertices = Vertices(text);
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            int newVertices = Vertices(text);
            if (newVertices <= 4) throw new InvalidOperationException("Counter still has no visible glyphs");
            if (text.gameObject.layer != 5) throw new InvalidOperationException("Counter didn't inherit HUD layer");
            foreach (int remaining in new[] { 40, 14, 1, 0 })
            {
                model.RemainingHealthBars = remaining;
                counter.UpdateBar(0);
                if (text.text != "x " + remaining || Vertices(text) <= 4)
                    throw new InvalidOperationException("Missing counter glyphs at " + remaining);
            }
            model.RemainingHealthBars = 14;
            counter.UpdateBar(0);
            // Render a close-up of the actual runtime Text, not a replacement label.
            text.transform.SetParent(canvas.transform, false);
            var rect = text.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(115, 42);
            rect.anchoredPosition = Vector2.zero;
            rect.localRotation = Quaternion.identity;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            var capture = new Texture2D(256, 96, TextureFormat.RGB24, false);
            capture.ReadPixels(new Rect(0, 0, 256, 96), 0, 0);
            capture.Apply();
            string[] args = Environment.GetCommandLineArgs();
            string output = args[Array.IndexOf(args, "-counterPreview") + 1];
            File.WriteAllBytes(output, capture.EncodeToPNG());
            RenderTexture.active = null;
            UnityEngine.Object.DestroyImmediate(capture);
            UnityEngine.Object.DestroyImmediate(canvasObject);
            UnityEngine.Object.DestroyImmediate(cameraObject);
            UnityEngine.Object.DestroyImmediate(target);
            Debug.Log("[RaidCounter] PASS old truncated vertices=" + oldVertices + ", fixed vertices=" + newVertices +
                "; x40/x14/x1/x0 have glyphs and inherit the HUD layer. Rendered " + output);
            EditorApplication.Exit(0);
        }
        catch (Exception error)
        {
            Debug.LogException(error);
            EditorApplication.Exit(1);
        }
    }

    private static int Vertices(Text text)
    {
        text.cachedTextGenerator.Invalidate();
        text.cachedTextGenerator.Populate(text.text, text.GetGenerationSettings(text.rectTransform.rect.size));
        return text.cachedTextGenerator.vertexCount;
    }
}

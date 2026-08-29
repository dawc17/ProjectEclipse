using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Nekki.SF2.GUI;
using Nekki.SF2.GUI.Fight;
using SF2DE.Underworld.UI;

// Exercises the real prefab and runtime widgets without loading/changing a save.
public static class DevRaidHealthBarValidator
{
    [MenuItem("SF2/Validate Raid Health Bar")]
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            throw new InvalidOperationException("Stop Play mode before validating the HUD.");
        Scene scene = EditorSceneManager.NewPreviewScene();
        RenderTexture target = null;
        Texture2D capture = null;
        try
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/prefabs/fight/display/ViewerFight.prefab");
            var template = prefab.GetComponentsInChildren<PlayerLifeBar>(true).Single(b => b.name == "PlayerLifeBar_right");
            var cameraObject = new GameObject("Raid HUD validation camera", typeof(UnityEngine.Camera));
            SceneManager.MoveGameObjectToScene(cameraObject, scene);
            var camera = cameraObject.GetComponent<UnityEngine.Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.12f, 0.08f, 0.05f);
            camera.orthographic = true;
            camera.orthographicSize = 230;
            target = new RenderTexture(900, 460, 24);
            camera.targetTexture = target;
            var canvasObject = new GameObject("Raid HUD validation", typeof(Canvas));
            SceneManager.MoveGameObjectToScene(canvasObject, scene);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            // The exported enemy widget has a slight Y rotation: leave depth
            // room for its far edge instead of clipping it at the near plane.
            canvas.planeDistance = 100;
            Canvas.ForceUpdateCanvases();
            Font font = prefab.GetComponentsInChildren<Text>(true).First(t => t.font != null).font;
            float originalWidth = template.get_rectTransform().rect.width;
            float[] damages = { 0f, 4.35f, 39.5f, 40f };
            int[] counts = { 40, 36, 1, 0 };
            for (int row = 0; row < damages.Length; row++)
            {
                var life = UnityEngine.Object.Instantiate(template, canvas.transform, false);
                life.gameObject.SetActive(true);
                var rect = life.get_rectTransform();
                rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.anchoredPosition = new Vector2(290, 170 - row * 110);
                var parameters = new ModelParameters { CIDCNCDFONA = 1, ShieldTotal = 40 };
                parameters.GFNCMLFKBGP(1);
                life.Init(parameters);
                life.SetRaidStyle(true);
				var counter = UnderworldRaidShieldBar.Attach(rect, parameters, font);
                parameters.GEACPINOAAN(-damages[row]);
                life.Render(); // Includes cross-bar reset, not only direct fill assignment.
                life.SetValBarValue(parameters.CurrentHealthBarFraction);
                life.SetHitBarValue(Mathf.Min(1, parameters.CurrentHealthBarFraction + (row == 1 ? 0.035f : 0)));
                counter.UpdateBar(0);
                counter.SetVisible(true);
                var text = counter.GetComponentInChildren<Text>();
                Require(text.text == "x " + counts[row], "Remaining-bar counter");
                text.cachedTextGenerator.Populate(text.text, text.GetGenerationSettings(text.rectTransform.rect.size));
                Require(text.cachedTextGenerator.vertexCount > 4, "Counter must generate visible glyphs, not just contain text");
                Require(counter.GetComponentsInChildren<Image>().Length == 0, "Counter must not have a shield icon");
                Require(Mathf.Abs(rect.rect.width - originalWidth) < 0.01f, "Fixed bar width");
                Require(Vector3.Dot(text.transform.right, canvas.transform.right) > 0.99f, "Counter must not be mirrored");
                var images = life.GetComponentsInChildren<ResolutionImageSkew>();
                Require(images.Count(i => i.get_SpriteName() == "FightUI.Raid_HealthBar_Full") == 2, "Blue fill and dark blue backing");
                Require(images.All(i => i.sprite != null && i.sprite.vertices.Length >= 4), "Valid native sprite geometry");
                var corners = new Vector3[4];
                rect.GetWorldCorners(corners);
                Vector3 label = canvas.transform.InverseTransformPoint(text.transform.position);
                float left = corners.Min(c => canvas.transform.InverseTransformPoint(c).x);
                float bottom = corners.Min(c => canvas.transform.InverseTransformPoint(c).y);
                Require(label.x > left && label.x < left + 130 && label.y < bottom, "Counter below screen-left end");
                if (row == 3)
                {
                    // Reusing this widget for a story enemy must restore its original skin.
                    life.SetRaidStyle(false);
                    Require(images.All(i => i.get_SpriteName() != "FightUI.Raid_HealthBar_Full"), "Story style restoration");
                    life.SetRaidStyle(true);
                }
            }
            VerifySmoothSegmentHandoff(template, canvas.transform);
            Canvas.ForceUpdateCanvases();
            if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null)
            {
                camera.Render();
                var previous = RenderTexture.active;
                try
                {
                    RenderTexture.active = target;
                    capture = new Texture2D(target.width, target.height, TextureFormat.RGB24, false);
                    capture.ReadPixels(new Rect(0, 0, target.width, target.height), 0, 0);
                    capture.Apply();
                    Directory.CreateDirectory("Temp");
                    File.WriteAllBytes("Temp/raid-healthbar-validation.png", capture.EncodeToPNG());
                }
                finally { RenderTexture.active = previous; }
            }
            Debug.Log("[RaidHealthBarValidation] PASS: fixed width, blue sprites, depletion/carry, smooth segment handoff, x40/x36/x1/x0, counter alignment, story style restoration.");
        }
        catch (Exception error)
        {
            Debug.LogException(error);
            if (Application.isBatchMode) EditorApplication.Exit(1);
            else throw;
            return;
        }
        finally
        {
            EditorSceneManager.ClosePreviewScene(scene);
            if (target != null) UnityEngine.Object.DestroyImmediate(target);
            if (capture != null) UnityEngine.Object.DestroyImmediate(capture);
        }
        if (Application.isBatchMode) EditorApplication.Exit(0);
    }

    private static void Require(bool condition, string label)
    {
        if (!condition) throw new InvalidOperationException("[RaidHealthBarValidation] " + label);
    }

    private static void VerifySmoothSegmentHandoff(PlayerLifeBar template, Transform parent)
    {
        var life = UnityEngine.Object.Instantiate(template, parent, false);
        try
        {
            life.gameObject.SetActive(true);
            var parameters = new ModelParameters { CIDCNCDFONA = 1, ShieldTotal = 2 };
            parameters.GFNCMLFKBGP(1f);
            life.Init(parameters);
            life.SetRaidStyle(true);

            // Cross into the second segment with carry-over damage. The old
            // implementation immediately set the display to 75%; the new one
            // must visibly drain the first segment, fill its replacement, then
            // animate the 25% carry-over damage.
            parameters.GEACPINOAAN(-1.25f);
            float expectedFraction = parameters.CurrentHealthBarFraction;
            life.Render();
            float firstFrameFill = GetLifeLayer(life, "_healthBar").fillAmount;
            Require(firstFrameFill > expectedFraction + 0.05f && firstFrameFill < 1f,
                "First segment must drain before replacement appears");

            bool sawEmptySegment = false;
            bool sawReplacementFilling = false;
            for (int frame = 0; frame < 180; frame++)
            {
                life.Render();
                float fill = GetLifeLayer(life, "_healthBar").fillAmount;
                if (fill <= 0.01f) sawEmptySegment = true;
                if (sawEmptySegment && fill > 0.01f && fill < 0.99f) sawReplacementFilling = true;
            }
            Require(sawEmptySegment, "Exhausted segment must reach empty");
            Require(sawReplacementFilling, "Replacement segment must fill smoothly");
            Require(Mathf.Abs(GetLifeLayer(life, "_healthBar").fillAmount - expectedFraction) < 0.0001f,
                "Carry-over damage after replacement");
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(life.gameObject);
        }
    }

    private static ResolutionImageSkew GetLifeLayer(PlayerLifeBar life, string fieldName)
    {
        FieldInfo field = typeof(PlayerLifeBar).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null) throw new InvalidOperationException("[RaidHealthBarValidation] Life-bar field missing: " + fieldName);
        ResolutionImageSkew layer = field.GetValue(life) as ResolutionImageSkew;
        if (layer == null) throw new InvalidOperationException("[RaidHealthBarValidation] Life-bar layer missing: " + fieldName);
        return layer;
    }
}

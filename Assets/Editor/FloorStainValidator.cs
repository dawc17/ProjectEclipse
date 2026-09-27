using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

// Native GPU regression: foreground stage art must not cover a floor decal,
// and the decal must not cover a fighter silhouette. Run in the matching editor.
public static class FloorStainValidator
{
    [MenuItem("Tools/SF2/Validate Floor Stain Mask")]
    public static void Validate()
    {
        var owned = new List<Object>();
        RenderTexture previous = RenderTexture.active;
        try
        {
            Shader bodyShader = Shader.Find("Mesh/Colored");
            Shader decalShader = Resources.Load<Shader>("shaders/EclipseFloorDecal");
            if (bodyShader == null || decalShader == null || !bodyShader.isSupported || !decalShader.isSupported ||
                ShaderUtil.ShaderHasError(bodyShader) || ShaderUtil.ShaderHasError(decalShader))
                throw new Exception("Floor decal or fighter shader is missing, unsupported or failed compilation.");

            var root = new GameObject("Floor stain validation") { hideFlags = HideFlags.HideAndDontSave };
            owned.Add(root);
            root.transform.position = new Vector3(100000f, 100000f, 0f);
            var cameraObject = new GameObject("Validation camera");
            cameraObject.transform.SetParent(root.transform, false);
            var camera = cameraObject.AddComponent<UnityEngine.Camera>();
            camera.enabled = false;
            camera.transform.localPosition = new Vector3(0f, 0f, -10f);
            camera.orthographic = true; camera.orthographicSize = 2f;
            camera.nearClipPlane = 0.1f; camera.farClipPlane = 20f;
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = Color.blue;
            camera.cullingMask = 1 << 31;
            var target = new RenderTexture(64, 64, 24, RenderTextureFormat.ARGB32);
            owned.Add(target); target.Create(); camera.targetTexture = target;
            var pixels = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            owned.Add(pixels);

            var body = new GameObject("Fighter") { layer = 31 };
            body.transform.SetParent(root.transform, false);
            var mesh = new Mesh(); owned.Add(mesh);
            mesh.vertices = new[] { new Vector3(-0.5f, -1f), new Vector3(0.5f, -1f),
                new Vector3(0.5f, 1f), new Vector3(-0.5f, 1f) };
            mesh.triangles = new[] { 0, 1, 2, 0, 2, 3 }; mesh.RecalculateBounds();
            body.AddComponent<MeshFilter>().sharedMesh = mesh;
            var bodyMaterial = new Material(bodyShader); owned.Add(bodyMaterial);
            bodyMaterial.SetFloat("_FloorDecalMask", 64f);
            bodyMaterial.SetColor("_Color", Color.black);
            body.AddComponent<MeshRenderer>().sharedMaterial = bodyMaterial;

            Sprite sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
            owned.Add(sprite);
            var floor = new GameObject("Foreground floor") { layer = 31 };
            floor.transform.SetParent(root.transform, false);
            floor.transform.localPosition = new Vector3(0, -1f, -2f);
            floor.transform.localScale = new Vector3(4, 2, 1);
            var floorRenderer = floor.AddComponent<SpriteRenderer>();
            floorRenderer.sprite = sprite; floorRenderer.color = Color.blue;

            var decal = new GameObject("Decal") { layer = 31 };
            decal.transform.SetParent(root.transform, false);
            decal.transform.localPosition = new Vector3(0, 0, 0.06f);
            decal.transform.localScale = new Vector3(4, 4, 1);
            var decalRenderer = decal.AddComponent<SpriteRenderer>();
            decalRenderer.sprite = sprite; decalRenderer.color = Color.red; decalRenderer.sortingOrder = 1;
            var decalMaterial = new Material(decalShader); owned.Add(decalMaterial);
            decalRenderer.sharedMaterial = decalMaterial;

            camera.Render();
            RenderTexture.active = target;
            pixels.ReadPixels(new Rect(0, 0, 64, 64), 0, 0); pixels.Apply();
            Require(pixels.GetPixel(50, 16).r > 0.9f, "Foreground floor covered the decal.");
            Require(pixels.GetPixel(32, 40).r < 0.1f, "Decal painted over the fighter.");
            Require(pixels.GetPixel(32, 24).b > 0.9f, "Mask was lost under the foreground floor.");

            body.SetActive(false);
            camera.Render(); pixels.ReadPixels(new Rect(0, 0, 64, 64), 0, 0); pixels.Apply();
            Require(pixels.GetPixel(32, 40).r > 0.9f, "Fighter stencil persisted into the next frame.");
            Debug.Log("PASS: floor decal shader compilation, foreground layering, fighter masking and per-frame mask clearing.");
        }
        finally
        {
            RenderTexture.active = previous;
            // Destroy renderers/camera before their resources.
            if (owned.Count > 0) Object.DestroyImmediate(owned[0]);
            for (int i = owned.Count - 1; i > 0; i--) Object.DestroyImmediate(owned[i]);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Modding
{
    // Arena-local geometry, independent of the fighter's animation and lifetime.
    // The current player render transform supplies the native coordinate mapping,
    // including mirrored arenas and a replacement model after a form change.
    internal sealed class ModArenaMarkerRenderer : MonoBehaviour, IModArenaMarker
    {
        internal const int MaximumMarkers = 64;
        private static readonly HashSet<ModArenaMarkerRenderer> markers = new HashSet<ModArenaMarkerRenderer>();
        private Func<bool> alive;
        private Func<Transform> coordinates;
        private Mesh mesh;
        private Material material;
        private bool closed;

        internal static bool TryCreate(ModArenaRect rect, ModUiColor color, Func<bool> alive, Func<Transform> coordinates,
            out IModArenaMarker result, out string error)
        {
            result = null; error = null;
            foreach (var old in new List<ModArenaMarkerRenderer>(markers))
                if (old == null) markers.Remove(old); else if (!old.IsActive) old.Dispose();
            if (!alive() || coordinates() == null) { error = "An active arena render transform is required."; return false; }
            if (markers.Count >= MaximumMarkers) { error = "The session already has 64 active arena markers."; return false; }
            var shader = Shader.Find("Sprites/Default");
            if (shader == null) { error = "Arena marker shader is unavailable."; return false; }
            var host = new GameObject("Eclipse arena marker");
            var marker = host.AddComponent<ModArenaMarkerRenderer>();
            try
            {
                marker.alive = alive; marker.coordinates = coordinates;
                marker.material = new Material(shader); marker.SetColor(color);
                marker.mesh = new Mesh { name = "Eclipse arena rectangle",
                    vertices = new[] { new Vector3((float)rect.X, (float)rect.Y, -.25f), new Vector3((float)(rect.X+rect.Width), (float)rect.Y, -.25f),
                        new Vector3((float)(rect.X+rect.Width), (float)(rect.Y+rect.Height), -.25f), new Vector3((float)rect.X, (float)(rect.Y+rect.Height), -.25f) },
                    uv = new[] { Vector2.zero, Vector2.right, Vector2.one, Vector2.up },
                    colors = new[] { Color.white, Color.white, Color.white, Color.white },
                    triangles = new[] { 0, 1, 2, 0, 2, 3 } };
                marker.mesh.RecalculateBounds();
                host.AddComponent<MeshFilter>().sharedMesh = marker.mesh;
                host.AddComponent<MeshRenderer>().sharedMaterial = marker.material;
                markers.Add(marker); marker.Refresh(); result = marker; return true;
            }
            catch { marker.Dispose(); throw; }
        }
        public bool IsActive
        {
            get
            {
                if (closed) return false;
                if (alive == null || !alive() || coordinates == null || coordinates() == null) { Dispose(); return false; }
                return true;
            }
        }
        public void SetColor(ModUiColor color)
        {
            if (material != null) material.color = new Color32(color.R, color.G, color.B, color.A);
        }
        private void LateUpdate() { if (IsActive) Refresh(); }
        private void Refresh()
        {
            var source = coordinates();
            transform.SetParent(source.parent, false);
            transform.localPosition = source.localPosition; transform.localRotation = source.localRotation;
            transform.localScale = source.localScale; gameObject.layer = source.gameObject.layer;
        }
        public void Dispose()
        {
            if (closed) return;
            closed = true; markers.Remove(this); alive = null; coordinates = null;
            if (material != null) Destroy(material); if (mesh != null) Destroy(mesh);
            material = null; mesh = null;
            if (gameObject != null) { gameObject.SetActive(false); Destroy(gameObject); }
        }
        private void OnDestroy() { Dispose(); }
    }
}

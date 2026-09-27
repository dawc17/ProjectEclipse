using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Eclipse.Rendering
{
    // A translucent skirt on boundary edges only. Shared triangle edges never
    // receive a fringe, so the recovered triangulation stays invisible.
    public sealed class RimFeather : MonoBehaviour
    {
        private struct Edge { public int A, B, Opposite, Count; }
        private readonly List<Edge> edges = new List<Edge>();
        private Vector3[] positions, projected, normals;
        private Mesh mesh;
        private MeshRenderer target;
        private MaterialPropertyBlock properties;
        private static Material material;

        public static RimFeather Create(Transform parent, int[] triangles, int count)
        {
            var shader = Resources.Load<Shader>("shaders/EclipseRimFeather");
            if (shader == null || !shader.isSupported) return null;
            if (material == null) material = new Material(shader);
            var child = new GameObject("Rim feather");
            child.transform.SetParent(parent, false);
            var feather = child.AddComponent<RimFeather>();
            feather.mesh = new Mesh { name = "Rim boundary feather" };
            feather.mesh.MarkDynamic();
            child.AddComponent<MeshFilter>().sharedMesh = feather.mesh;
            feather.target = child.AddComponent<MeshRenderer>();
            feather.target.sharedMaterial = material;
            feather.target.shadowCastingMode = ShadowCastingMode.Off;
            feather.target.receiveShadows = false;
            feather.target.lightProbeUsage = LightProbeUsage.Off;
            feather.target.reflectionProbeUsage = ReflectionProbeUsage.Off;
            feather.properties = new MaterialPropertyBlock();
            var boundary = new Dictionary<ulong, Edge>();
            for (int i = 0; i < triangles.Length; i += 3)
                for (int j = 0; j < 3; j++)
                {
                    int a = triangles[i + j], b = triangles[i + (j + 1) % 3];
                    ulong key = ((ulong)(uint)Mathf.Min(a, b) << 32) | (uint)Mathf.Max(a, b);
                    if (boundary.TryGetValue(key, out var edge)) { edge.Count++; boundary[key] = edge; }
                    else boundary[key] = new Edge { A = a, B = b, Opposite = triangles[i + (j + 2) % 3], Count = 1 };
                }
            foreach (var edge in boundary.Values) if (edge.Count == 1) feather.edges.Add(edge);
            feather.positions = new Vector3[count * 2];
            feather.projected = new Vector3[count];
            feather.normals = new Vector3[count];
            var colors = new Color[count * 2];
            for (int i = 0; i < count; i++) { colors[i] = Color.white; colors[i + count] = new Color(1, 1, 1, 0); }
            var indices = new List<int>();
            foreach (var edge in feather.edges)
            {
                int a = edge.A, b = edge.B;
                indices.AddRange(new[] { a, b, b + count, a, b + count, a + count });
            }
            feather.mesh.vertices = feather.positions;
            feather.mesh.colors = colors;
            feather.mesh.triangles = indices.ToArray();
            return feather;
        }

        public void Refresh(Vector3[] vertices, float pixels, Color color)
        {
            target.enabled = pixels > 0f;
            if (!target.enabled) return;
            var camera = UnityEngine.Camera.main;
            int count = vertices.Length;
            for (int i = 0; i < count; i++)
            {
                projected[i] = camera != null ? camera.WorldToScreenPoint(transform.TransformPoint(vertices[i])) : vertices[i];
                normals[i] = Vector3.zero;
                positions[i] = vertices[i];
            }
            foreach (var edge in edges)
            {
                Vector3 delta = projected[edge.B] - projected[edge.A];
                var normal = new Vector3(-delta.y, delta.x, 0).normalized;
                // The source mixes triangle winding. Choose the side away from
                // the triangle interior instead of assuming clockwise faces.
                if (Vector3.Dot(normal, projected[edge.Opposite] - projected[edge.A]) > 0) normal = -normal;
                normals[edge.A] += normal; normals[edge.B] += normal;
            }
            for (int i = 0; i < count; i++)
            {
                Vector3 outer = projected[i] + normals[i].normalized * pixels;
                positions[i + count] = camera != null
                    ? transform.InverseTransformPoint(camera.ScreenToWorldPoint(outer)) : outer;
            }
            mesh.vertices = positions;
            mesh.RecalculateBounds();
            properties.SetColor("_Color", color);
            target.SetPropertyBlock(properties);
        }

        private void OnDestroy() { if (mesh != null) Destroy(mesh); }
    }
}

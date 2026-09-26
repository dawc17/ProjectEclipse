using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // A procedural sumi brush stroke: ragged top and bottom edges, a blunt loaded start
    // and a dry, tapering tail. Fill (0..1) draws it left to right for a "painted" reveal.
    // Code-owned geometry; it never touches a recovered sprite mesh.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class InkStroke : MaskableGraphic
    {
        private const int Segments = 36;
        [SerializeField] private float fill = 1f;
        [SerializeField] private int seed = 7;

        public float Fill
        {
            get { return fill; }
            set { value = Mathf.Clamp01(value); if (!Mathf.Approximately(value, fill)) { fill = value; SetVerticesDirty(); } }
        }

        public int Seed { get { return seed; } set { if (seed != value) { seed = value; SetVerticesDirty(); } } }

        private float Noise(float x, float salt)
        {
            float s = seed * 12.9898f + salt * 78.233f;
            return Mathf.Sin(x * 17.1f + s) * .45f + Mathf.Sin(x * 43.7f + s * 1.7f) * .3f + Mathf.Sin(x * 91.3f + s * .3f) * .25f;
        }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            if (fill <= 0f) return;
            Rect r = rectTransform.rect;
            float half = r.height * .5f, mid = r.center.y;
            int count = Mathf.Max(2, Mathf.CeilToInt(Segments * fill));
            for (int i = 0; i <= count; i++)
            {
                float u = Mathf.Min(fill, i / (float)Segments);
                float x = r.xMin + r.width * u;
                // Loaded, rounded head; long tail that thins and dries out.
                float head = Mathf.Sqrt(Mathf.Clamp01(u / .05f));
                float tail = 1f - Mathf.Pow(Mathf.Clamp01((u - .72f) / .28f), 1.6f) * .78f;
                float thickness = half * head * tail;
                float top = mid + thickness * (.9f + .1f * Noise(u, 1f)) + Noise(u, 3f) * 1.6f;
                float bottom = mid - thickness * (.9f + .1f * Noise(u, 2f)) + Noise(u, 4f) * 1.6f;
                var c = color;
                c.a *= Mathf.Lerp(1f, .55f + .25f * Noise(u, 5f), Mathf.Clamp01((u - .6f) / .4f));
                mesh.AddVert(new Vector3(x, top), c, Vector2.zero);
                mesh.AddVert(new Vector3(x, bottom), c, Vector2.zero);
                if (i > 0)
                {
                    int v = i * 2;
                    mesh.AddTriangle(v - 2, v - 1, v + 1);
                    mesh.AddTriangle(v - 2, v + 1, v);
                }
            }
        }
    }
}

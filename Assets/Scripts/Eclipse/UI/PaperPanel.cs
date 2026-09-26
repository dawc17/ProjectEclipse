using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // A parchment card: deckled (slightly torn) edges, a warm vignette toward the border
    // and a soft drop shadow, drawn as one procedural mesh. No texture or sprite involved.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class PaperPanel : MaskableGraphic
    {
        private const float Tear = 3.2f;
        private const float ShadowSoftness = 22f;
        private static readonly Vector2 ShadowOffset = new Vector2(8f, -10f);
        [SerializeField] private Color edge = new Color32(196, 172, 131, 255);
        [SerializeField] private float shadowAlpha = .45f;

        public Color Edge { get { return edge; } set { edge = value; SetVerticesDirty(); } }

        private static float Noise(float t)
        {
            return Mathf.Sin(t * 1.9f) * .5f + Mathf.Sin(t * 5.3f + 1.3f) * .3f + Mathf.Sin(t * 13.7f + 2.1f) * .2f;
        }

        // Perimeter points, clockwise from the top-left corner, with a torn offset.
        private static Vector2[] Outline(Rect r, int perSide, bool torn = true)
        {
            var points = new Vector2[perSide * 4];
            Vector2[] corners = { new Vector2(r.xMin, r.yMax), new Vector2(r.xMax, r.yMax), new Vector2(r.xMax, r.yMin), new Vector2(r.xMin, r.yMin) };
            for (int side = 0; side < 4; side++)
            {
                Vector2 a = corners[side], b = corners[(side + 1) % 4];
                Vector2 normal = new Vector2(b.y - a.y, a.x - b.x).normalized;
                for (int i = 0; i < perSide; i++)
                {
                    float u = i / (float)perSide;
                    float tear = !torn || i == 0 ? 0f : Noise((side * perSide + i) * .9f) * Tear;
                    points[side * perSide + i] = Vector2.Lerp(a, b, u) + normal * tear;
                }
            }
            return points;
        }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect r = rectTransform.rect;
            int perSide = Mathf.Clamp(Mathf.RoundToInt(Mathf.Max(r.width, r.height) / 24f), 8, 64);
            var outline = Outline(r, perSide);
            Vector2 center = r.center;

            // Soft shadow ring: opaque at the offset outline, transparent further out.
            var shade = new Color(0f, 0f, 0f, shadowAlpha * color.a);
            var clear = new Color(0f, 0f, 0f, 0f);
            int start = mesh.currentVertCount;
            for (int i = 0; i < outline.Length; i++)
            {
                Vector2 p = outline[i] + ShadowOffset;
                Vector2 away = (p - center - ShadowOffset).normalized;
                mesh.AddVert(p, shade, Vector2.zero);
                mesh.AddVert(p + away * ShadowSoftness, clear, Vector2.zero);
            }
            for (int i = 0; i < outline.Length; i++)
            {
                int a = start + i * 2, b = start + ((i + 1) % outline.Length) * 2;
                mesh.AddTriangle(a, a + 1, b + 1);
                mesh.AddTriangle(a, b + 1, b);
            }
            int shadowCenter = mesh.currentVertCount;
            mesh.AddVert(center + ShadowOffset, shade, Vector2.zero);
            for (int i = 0; i < outline.Length; i++)
                mesh.AddTriangle(shadowCenter, start + i * 2, start + ((i + 1) % outline.Length) * 2);

            // Paper: a light centre, flat paper through most of the card, then a warmer,
            // darker band toward the torn border (a fixed-width band, not a full radial fade).
            const float Band = 34f;
            int paperCenter = mesh.currentVertCount;
            mesh.AddVert(center, Color.Lerp(color, Color.white, .06f), Vector2.zero);
            var border = edge; border.a *= color.a;
            float band = Mathf.Min(Band, Mathf.Min(r.width, r.height) * .25f);
            var insetOutline = Outline(new Rect(r.xMin + band, r.yMin + band, r.width - band * 2f, r.height - band * 2f), perSide, false);
            for (int i = 0; i < insetOutline.Length; i++) mesh.AddVert(insetOutline[i], color, Vector2.zero);
            int ring = mesh.currentVertCount;
            for (int i = 0; i < outline.Length; i++) mesh.AddVert(outline[i], border, Vector2.zero);
            int n = outline.Length;
            for (int i = 0; i < n; i++)
            {
                int inner = paperCenter + 1 + i, innerNext = paperCenter + 1 + (i + 1) % n;
                mesh.AddTriangle(paperCenter, inner, innerNext);
                mesh.AddTriangle(inner, ring + i, ring + (i + 1) % n);
                mesh.AddTriangle(inner, ring + (i + 1) % n, innerNext);
            }
        }
    }
}

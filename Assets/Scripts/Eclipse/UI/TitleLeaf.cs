using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Seasonal title particles, positioned in the title page's top-left coordinate space.
    // Leaves and petals sway, spin and flutter down (a horizontal squash reads as the
    // leaf turning over), come to rest on the scene's ground line, then fade and respawn.
    // A shared gust pushes all of them.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class TitleLeaf : MaskableGraphic
    {
        public enum Kind { Leaf, Petal }

        private static readonly Color[] LeafColors =
        {
            new Color32(232, 92, 28, 255), new Color32(246, 136, 38, 255), new Color32(201, 54, 27, 255),
            new Color32(250, 171, 62, 255), new Color32(178, 44, 24, 255),
        };
        private static readonly Color[] PetalColors =
        {
            new Color32(255, 196, 214, 255), new Color32(248, 170, 196, 255), new Color32(255, 222, 232, 255),
        };

        // 0 (calm) .. 1 (full gust); driven by the title screen.
        public static float Gust;

        private Kind kind;
        private float minX, maxX, top, bottom, ground;
        private float x, y, fall, sway, swayRate, phase, spin, angle, flutter, rest;
        private Color baseColor;

        public static void Scatter(RectTransform parent, int count, Kind kind, float minX, float maxX, float top,
            float bottom, float ground)
        {
            for (int i = 0; i < count; i++)
            {
                var rect = new GameObject(kind.ToString(), typeof(RectTransform)).GetComponent<RectTransform>();
                rect.SetParent(parent, false);
                rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(.5f, .5f);
                var leaf = rect.gameObject.AddComponent<TitleLeaf>();
                leaf.raycastTarget = false;
                leaf.kind = kind;
                leaf.minX = minX; leaf.maxX = maxX; leaf.top = top; leaf.bottom = bottom; leaf.ground = ground;
                leaf.Respawn(true);
            }
        }

        private void Respawn(bool anywhere)
        {
            bool petal = kind == Kind.Petal;
            float size = petal ? Random.Range(7f, 13f) : Random.Range(9f, 21f);
            baseColor = petal ? PetalColors[Random.Range(0, PetalColors.Length)] : LeafColors[Random.Range(0, LeafColors.Length)];
            rectTransform.sizeDelta = new Vector2(size, size * (petal ? .8f : .62f));
            color = baseColor;
            x = Random.Range(minX, maxX);
            y = anywhere ? Random.Range(top, ground) : top - Random.Range(0f, 60f);
            fall = Random.Range(26f, 58f) * (size / 15f);
            sway = Random.Range(18f, 46f);
            swayRate = Random.Range(.5f, 1.1f);
            phase = Random.Range(0f, Mathf.PI * 2f);
            spin = Random.Range(-70f, 70f);
            angle = Random.Range(0f, 360f);
            flutter = Random.Range(1.4f, 3.2f);
            rest = 0f;
        }

        private void Update()
        {
            float dt = Mathf.Min(Time.unscaledDeltaTime, .1f);
            float t = Time.unscaledTime;
            float gust = Gust;
            float drift = Mathf.Sin(t * swayRate + phase) * sway;
            if (rest > 0f)
            {
                // Resting on the ground: a strong gust carries it on, otherwise it fades out.
                rest += dt;
                x += 70f * gust * gust * dt;
                var c = baseColor; c.a = Mathf.Clamp01((6f - rest) / 1.5f); color = c;
                if (rest >= 6f) Respawn(false);
                rectTransform.anchoredPosition = new Vector2(x, -y);
                return;
            }
            y += fall * (1f - .45f * gust) * dt;
            // A light breeze from the left, much stronger in a gust.
            x += (8f + 10f * Mathf.Sin(t * .23f + phase) + 150f * gust) * dt;
            angle += spin * (1f + 2f * gust) * dt;
            if (x > maxX + 80f) { x = minX; }
            if (y >= ground - (phase * 2.2f))
            {
                // Settle: lie roughly flat, face up.
                rest = .001f;
                angle = Random.Range(-18f, 18f) + (Random.value < .5f ? 0f : 180f);
                rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
                rectTransform.localScale = Vector3.one;
                return;
            }
            rectTransform.anchoredPosition = new Vector2(x + drift, -y);
            rectTransform.localRotation = Quaternion.Euler(0, 0, angle + drift * .8f);
            rectTransform.localScale = new Vector3(Mathf.Lerp(.25f, 1f, Mathf.Abs(Mathf.Cos(t * flutter + phase))), 1f, 1f);
        }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect r = rectTransform.rect;
            // A pointed leaf (petals are rounder): two curved edges plus a darker vein.
            const int Segments = 8;
            var body = color;
            var vein = Color.Lerp(color, new Color(.35f, .1f, .04f, color.a), kind == Kind.Petal ? .15f : .55f);
            float fullness = kind == Kind.Petal ? .62f : .5f;
            mesh.AddVert(new Vector3(r.xMin, r.center.y), body, Vector2.zero);
            for (int i = 1; i < Segments; i++)
            {
                float u = i / (float)Segments;
                float w = Mathf.Pow(Mathf.Sin(u * Mathf.PI), kind == Kind.Petal ? .7f : 1f) * r.height * fullness;
                float px = Mathf.Lerp(r.xMin, r.xMax, u);
                mesh.AddVert(new Vector3(px, r.center.y + w), body, Vector2.zero);
                mesh.AddVert(new Vector3(px, r.center.y - w), body, Vector2.zero);
            }
            int tip = mesh.currentVertCount;
            mesh.AddVert(new Vector3(r.xMax, r.center.y), body, Vector2.zero);
            mesh.AddTriangle(0, 1, 2);
            for (int i = 1; i < Segments - 1; i++)
            {
                int a = 1 + (i - 1) * 2, b = 1 + i * 2;
                mesh.AddTriangle(a, b, b + 1);
                mesh.AddTriangle(a, b + 1, a + 1);
            }
            mesh.AddTriangle(1 + (Segments - 2) * 2, tip, 2 + (Segments - 2) * 2);
            int v = mesh.currentVertCount;
            float thin = Mathf.Max(.6f, r.height * .06f);
            mesh.AddVert(new Vector3(r.xMin, r.center.y + thin), vein, Vector2.zero);
            mesh.AddVert(new Vector3(r.xMax, r.center.y), vein, Vector2.zero);
            mesh.AddVert(new Vector3(r.xMin, r.center.y - thin), vein, Vector2.zero);
            mesh.AddTriangle(v, v + 1, v + 2);
        }
    }
}

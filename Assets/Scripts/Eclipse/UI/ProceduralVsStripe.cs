using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // Draws one recovered VS stripe half with its original sprite. On wide
    // displays only a plain body section of the brush stroke is lengthened, so
    // the pointed end still lies past the screen edge while the chevron, the
    // splatter at the seam and the original gradient keep their proportions.
    // The hidden recovered Image keeps receiving the VsScreen fill tween and
    // supplies the reveal progress.
    public sealed class ProceduralVsStripe : MaskableGraphic
    {
        private const string NodeName = "Procedural VS stripe";

        // Sprite-space (u, v) outline of each pointed end, measured from Stripe.png.
        private static readonly Vector2[] LeftTip =
            { new Vector2(0.401f, 0.963f), new Vector2(0f, 0.566f), new Vector2(0.102f, 0.132f) };
        private static readonly Vector2[] RightTip =
            { new Vector2(0.843f, 1f), new Vector2(1f, 0.402f), new Vector2(0.671f, 0.077f) };

        // Stretchable body sections: between the chevron and the seam splatter,
        // where the texture only has horizontal streaks and a smooth gradient.
        private static readonly float[] LeftColumns = { 0f, 0.42f, 0.62f, 1f };
        private static readonly float[] RightColumns = { 0f, 0.40f, 0.62f, 1f };

        private const float EdgeMargin = 8f;
        private const int OutlineSamples = 8;

        private Image track;
        private Sprite source;
        private bool left;
        private float progress = -1f;
        private Vector2 screenSize;

        public override Texture mainTexture
        {
            get { return source != null ? source.texture : s_WhiteTexture; }
        }

        public static void Attach(Image image, string spriteName)
        {
            Transform existing = image.transform.Find(NodeName);
            ProceduralVsStripe stripe = existing != null ? existing.GetComponent<ProceduralVsStripe>() : null;
            if (stripe == null)
            {
                var node = new GameObject(NodeName, typeof(RectTransform), typeof(CanvasRenderer));
                node.layer = image.gameObject.layer;
                node.transform.SetParent(image.transform, false);
                stripe = node.AddComponent<ProceduralVsStripe>();
                RectTransform rect = stripe.rectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = image.rectTransform.pivot;
                rect.offsetMin = rect.offsetMax = Vector2.zero;
                stripe.raycastTarget = false;
            }
            stripe.track = image;
            stripe.left = spriteName == "Stripe.left";
            stripe.source = image.sprite;
            stripe.color = image.color;
            image.enabled = false;
            stripe.SetMaterialDirty();
            stripe.SetVerticesDirty();
        }

        private void LateUpdate()
        {
            float next = track != null ? Mathf.Clamp01(track.fillAmount) : 0f;
            RectTransform screen = ScreenRect();
            Vector2 size = screen != null ? screen.rect.size : Vector2.zero;
            if (next == progress && size == screenSize) return;
            progress = next;
            screenSize = size;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            if (source == null || progress <= 0f) return;
            RectTransform screen = ScreenRect();
            if (screen == null) return;
            Rect area = rectTransform.rect;
            if (area.width <= 0f || area.height <= 0f) return;

            float extension = Extension(screen, area);
            float[] columns = left ? LeftColumns : RightColumns;
            var xs = new float[columns.Length];
            for (int i = 0; i < columns.Length; i++)
            {
                float shift = left ? (i <= 1 ? -extension : 0f) : (i >= 2 ? extension : 0f);
                xs[i] = area.xMin + columns[i] * area.width + shift;
            }

            // Both recovered halves fill horizontally from their left edge.
            float end = Mathf.Lerp(xs[0], xs[xs.Length - 1], progress);
            Vector4 uv = DataUtility.GetOuterUV(source);
            Color32 ink = color;
            for (int i = 0; i < columns.Length - 1; i++)
            {
                float x0 = xs[i];
                float x1 = Mathf.Min(xs[i + 1], end);
                if (x1 <= x0) break;
                float u0 = columns[i];
                float u1 = Mathf.Lerp(columns[i], columns[i + 1], (x1 - x0) / (xs[i + 1] - x0));
                int first = mesh.currentVertCount;
                float texU0 = Mathf.Lerp(uv.x, uv.z, u0), texU1 = Mathf.Lerp(uv.x, uv.z, u1);
                mesh.AddVert(new Vector3(x0, area.yMin), ink, new Vector2(texU0, uv.y));
                mesh.AddVert(new Vector3(x0, area.yMax), ink, new Vector2(texU0, uv.w));
                mesh.AddVert(new Vector3(x1, area.yMax), ink, new Vector2(texU1, uv.w));
                mesh.AddVert(new Vector3(x1, area.yMin), ink, new Vector2(texU1, uv.y));
                mesh.AddTriangle(first, first + 1, first + 2);
                mesh.AddTriangle(first + 2, first + 3, first);
            }
        }

        // Local-space length to add so every point of the pointed end leaves the screen.
        private float Extension(RectTransform screen, Rect area)
        {
            Vector2 axis = screen.InverseTransformVector(rectTransform.TransformVector(Vector3.right));
            float scale = axis.magnitude;
            if (scale <= 0f) return 0f;
            Vector2 outward = (left ? -axis : axis) / scale;
            Rect bounds = screen.rect;
            bounds.min -= Vector2.one * EdgeMargin;
            bounds.max += Vector2.one * EdgeMargin;
            Vector2[] tip = left ? LeftTip : RightTip;
            float needed = 0f;
            for (int k = 0; k < tip.Length - 1; k++)
            {
                for (int s = 0; s <= OutlineSamples; s++)
                {
                    Vector2 point = Vector2.Lerp(tip[k], tip[k + 1], s / (float)OutlineSamples);
                    Vector2 local = new Vector2(Mathf.Lerp(area.xMin, area.xMax, point.x),
                        Mathf.Lerp(area.yMin, area.yMax, point.y));
                    Vector2 onScreen = screen.InverseTransformPoint(rectTransform.TransformPoint(local));
                    needed = Mathf.Max(needed, ExitDistance(onScreen, outward, bounds));
                }
            }
            return needed / scale;
        }

        // Distance along direction before a point inside bounds crosses their edge.
        private static float ExitDistance(Vector2 point, Vector2 direction, Rect bounds)
        {
            if (!bounds.Contains(point)) return 0f;
            float best = float.MaxValue;
            if (direction.x < -1e-4f) best = Mathf.Min(best, (point.x - bounds.xMin) / -direction.x);
            if (direction.x > 1e-4f) best = Mathf.Min(best, (bounds.xMax - point.x) / direction.x);
            if (direction.y < -1e-4f) best = Mathf.Min(best, (point.y - bounds.yMin) / -direction.y);
            if (direction.y > 1e-4f) best = Mathf.Min(best, (bounds.yMax - point.y) / direction.y);
            return best == float.MaxValue ? 0f : best;
        }

        private RectTransform ScreenRect()
        {
            Canvas owner = canvas;
            return owner != null ? owner.rootCanvas.transform as RectTransform : null;
        }
    }
}

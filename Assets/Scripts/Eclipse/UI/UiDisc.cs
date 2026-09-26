using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // A filled circle with an optional ring, used for slider knobs and markers.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class UiDisc : MaskableGraphic
    {
        private const int Steps = 32;
        [SerializeField] private Color ring = Color.clear;
        [SerializeField] private float ringWidth = 3f;

        public void SetRing(Color value, float width) { ring = value; ringWidth = width; SetVerticesDirty(); }

        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect r = rectTransform.rect;
            float outer = Mathf.Min(r.width, r.height) * .5f;
            float inner = ring.a > 0f ? Mathf.Max(0f, outer - ringWidth) : outer;
            mesh.AddVert(r.center, color, Vector2.zero);
            for (int i = 0; i < Steps; i++)
            {
                float a = i * Mathf.PI * 2f / Steps;
                mesh.AddVert(r.center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * inner, color, Vector2.zero);
            }
            for (int i = 0; i < Steps; i++) mesh.AddTriangle(0, 1 + i, 1 + (i + 1) % Steps);
            if (inner >= outer) return;
            var c = ring; c.a *= color.a;
            int start = mesh.currentVertCount;
            for (int i = 0; i < Steps; i++)
            {
                float a = i * Mathf.PI * 2f / Steps;
                var d = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
                mesh.AddVert(r.center + d * inner, c, Vector2.zero);
                mesh.AddVert(r.center + d * outer, c, Vector2.zero);
            }
            for (int i = 0; i < Steps; i++)
            {
                int a = start + i * 2, b = start + ((i + 1) % Steps) * 2;
                mesh.AddTriangle(a, a + 1, b + 1);
                mesh.AddTriangle(a, b + 1, b);
            }
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.UI
{
    // A code-owned UI shape, never a modification to a recovered sprite mesh.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class TitleRibbon : MaskableGraphic
    {
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            mesh.Clear();
            Rect r = rectTransform.rect;
            mesh.AddVert(new Vector3(r.xMin, r.yMin), color, Vector2.zero);
            mesh.AddVert(new Vector3(r.xMin, r.yMax), color, Vector2.zero);
            mesh.AddVert(new Vector3(r.xMax, r.yMax), color, Vector2.zero);
            mesh.AddVert(new Vector3(r.xMax - 28, r.yMin), color, Vector2.zero);
            mesh.AddTriangle(0, 1, 2); mesh.AddTriangle(0, 2, 3);
        }
    }
}

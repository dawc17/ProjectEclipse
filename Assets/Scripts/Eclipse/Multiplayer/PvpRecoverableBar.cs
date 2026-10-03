using Nekki.SF2.GUI;
using UnityEngine;

namespace Eclipse.Multiplayer
{
    /// <summary>Recoverable health occupies only the segment beyond the displayed live fill.</summary>
    public sealed class PvpRecoverableBar : MonoBehaviour
    {
        private ResolutionImageSkew bar;
        private ResolutionImageSkew liveBar;
        private ModelParameters parameters;
        private Material greyMaterial;
        private static readonly int Segment = Shader.PropertyToID("_Segment");
        private static readonly int SpriteUv = Shader.PropertyToID("_SpriteUV");

        public static void Attach(ResolutionImageSkew source, ModelParameters fighter)
        {
            if (source == null || fighter == null || Fight.GetCurrentFight()?.IsLocalVersus != true) return;
            var existing = source.GetComponent<PvpRecoverableBar>();
            if (existing != null) { existing.parameters = fighter; return; }
            var clone = Object.Instantiate(source, source.transform.parent, false);
            clone.name = "PvP Recoverable Health";
            clone.transform.SetSiblingIndex(source.transform.GetSiblingIndex());
            clone.color = new Color32(210, 210, 210, 255);
            clone.raycastTarget = false;
            clone.fillAmount = 1f;
            clone.enabled = false;
            var smoothing = clone.GetComponent<Eclipse.Rendering.Interpolation.TickPresentationSmoother>();
            if (smoothing != null) smoothing.enabled = false;
            var display = source.gameObject.AddComponent<PvpRecoverableBar>();
            display.liveBar = source;
            display.bar = clone;
            display.parameters = fighter;
            var shader = Resources.Load<Shader>("shaders/EclipseRecoverableHealth");
            if (shader != null)
            {
                display.greyMaterial = new Material(shader) { name = "PvP Recoverable Health" };
                clone.material = display.greyMaterial;
            }
        }

        private void LateUpdate()
        {
            if (bar == null || parameters == null || greyMaterial == null) return;
            float pool = parameters.MaxLife > 0f ? parameters.RecoverableLife / parameters.MaxLife : 0f;
            // Follow the visible, interpolated edge: transparent pixels in the live
            // artwork must never reveal a second health texture underneath it.
            float start = Mathf.Clamp01(liveBar.fillAmount);
            float end = Mathf.Clamp01(parameters.CurrentHealthBarFraction + pool);
            bar.enabled = liveBar.enabled && pool > 0f && end > start;
            bar.sprite = liveBar.sprite;
            bar.overrideSprite = liveBar.overrideSprite;
            bar.fillOrigin = liveBar.fillOrigin;
            var sprite = bar.overrideSprite != null ? bar.overrideSprite : bar.sprite;
            var uv = sprite != null ? UnityEngine.Sprites.DataUtility.GetOuterUV(sprite) : new Vector4(0, 0, 1, 1);
            var segment = new Vector4(start, end, liveBar.fillOrigin == 1 ? 1f : 0f, 0f);
            SetSegment(greyMaterial, segment, uv);
            // UGUI stencil masks cache a separate material. Update the material
            // actually rendered too, rather than leaving it with the initial range.
            var rendering = bar.materialForRendering;
            if (rendering != greyMaterial) SetSegment(rendering, segment, uv);
        }

        private static void SetSegment(Material material, Vector4 segment, Vector4 uv)
        {
            material.SetVector(Segment, segment);
            material.SetVector(SpriteUv, uv);
        }

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                if (bar != null) Destroy(bar.gameObject);
                if (greyMaterial != null) Destroy(greyMaterial);
            }
            else
            {
                if (bar != null) DestroyImmediate(bar.gameObject);
                if (greyMaterial != null) DestroyImmediate(greyMaterial);
            }
        }
    }
}

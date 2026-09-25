using Eclipse.Modding;
using Eclipse.Rendering.Interpolation;
using UnityEngine;

namespace Eclipse.Rendering
{
	// Rim light (sf2.visuals.rim_light): body renderers draw an offset, coloured
	// twin just behind themselves, so a thin lit edge shows on one side of the
	// silhouette. A fight's location supplies the colour; menu previews (shop,
	// profile) use a warm neutral light.
	public static class RimLight
	{
		private const float Depth = 0.05f;

		private static readonly Color PreviewLight = new Color(0.95f, 0.87f, 0.72f, 1f);

		public static Color? SceneColor { get; set; }

		public static bool Active
		{
			get { return ModVisuals.Active(ModVisualEffect.RimLight) != null; }
		}

		// The rim colour now: the fight location's, or the preview light brightened
		// and faded by the mod's lighten/alpha settings.
		public static Color CurrentColor
		{
			get
			{
				if (SceneColor.HasValue) return SceneColor.Value;
				ModVisualDefinition settings = ModVisuals.Active(ModVisualEffect.RimLight);
				Color c = Color.Lerp(PreviewLight, Color.white, settings != null ? settings.Number("lighten") : 0.35f);
				c.a = settings != null ? settings.Number("alpha") : 0.85f;
				return c;
			}
		}

		// Offset toward the upper left of the screen by a constant pixel distance,
		// expressed in the renderer parent's local space, and pushed slightly back.
		public static Vector3 LocalOffset(Transform parent)
		{
			ModVisualDefinition settings = ModVisuals.Active(ModVisualEffect.RimLight);
			float offsetPixels = settings != null ? settings.Number("offset") : 0f;
			UnityEngine.Camera camera = UnityEngine.Camera.main;
			float pixel = camera != null && camera.orthographic && Screen.height > 0
				? 2f * camera.orthographicSize / Screen.height : 1f;
			Vector3 world = new Vector3(-offsetPixels * pixel, offsetPixels * pixel, 0f);
			Vector3 local = parent != null ? parent.InverseTransformVector(world) : world;
			local.z = Depth;
			return local;
		}
	}
}

using Eclipse.Modding;
using Eclipse.Rendering.Interpolation;
using UnityEngine;

namespace Eclipse.Rendering
{
	// Rim light (sf2.visuals.rim_light): body renderers draw an offset, coloured
	// twin just behind themselves, so a thin lit edge shows on one side of the
	// silhouette. The fight's location supplies the colour while a fight runs.
	public static class RimLight
	{
		private const float Depth = 0.05f;

		public static Color? SceneColor { get; set; }

		public static bool Active
		{
			get { return SceneColor.HasValue && FightInterpolation.IsFightActive && ModVisuals.Active(ModVisualEffect.RimLight) != null; }
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

using Eclipse.Rendering.Interpolation;
using UnityEngine;

namespace Eclipse.Rendering
{
	// Experimental rim light: body renderers draw an offset, coloured twin just
	// behind themselves, so a thin lit edge shows on one side of the silhouette.
	// The fight's location supplies the colour while a fight is running.
	public static class RimLight
	{
		private const float OffsetPixels = 2.5f;
		private const float Depth = 0.05f;

		public static Color? SceneColor { get; set; }

		public static bool Active
		{
			get { return ExperimentalVisuals.RimLight && SceneColor.HasValue && FightInterpolation.IsFightActive; }
		}

		// Offset toward the upper left of the screen by a constant pixel distance,
		// expressed in the renderer parent's local space, and pushed slightly back.
		public static Vector3 LocalOffset(Transform parent)
		{
			UnityEngine.Camera camera = UnityEngine.Camera.main;
			float pixel = camera != null && camera.orthographic && Screen.height > 0
				? 2f * camera.orthographicSize / Screen.height : 1f;
			Vector3 world = new Vector3(-OffsetPixels * pixel, OffsetPixels * pixel, 0f);
			Vector3 local = parent != null ? parent.InverseTransformVector(world) : world;
			local.z = Depth;
			return local;
		}
	}
}

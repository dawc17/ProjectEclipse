using Eclipse.Modding;
using Eclipse.Rendering.Interpolation;
using UnityEngine;

namespace Eclipse.Rendering
{
	// Rim light (sf2.visuals.rim_light): body renderers draw an offset, coloured
	// twin just behind themselves, so a thin lit edge shows on one side of the
	// silhouette. A fight's location supplies the colour; menu previews (shop,
	// profile) use a warm neutral light.
	//
	// The same twin carries sf2.fx.light: a fighter near a glowing weapon or a
	// magic cast gets its edge turned toward that light and tinted by it, even
	// when no rim_light preset is configured.
	public static class RimLight
	{
		private const float Depth = 0.05f;
		// Edge width for fighter lighting when the rim_light preset is off.
		private const float LightOffset = 3f;

		private static readonly Color PreviewLight = new Color(0.95f, 0.87f, 0.72f, 1f);
		private static readonly Color DefaultInk = new Color(0.07f, 0.03f, 0.11f, 1f);

		public static Color? SceneColor { get; set; }

		public static bool Active
		{
			get { return ModVisuals.Active(ModVisualEffect.RimLight) != null || ModVisuals.HasActiveFx(ModFxKind.Light); }
		}

		// The rim colour now: the fight location's, or the preview light brightened
		// and faded by the mod's lighten/alpha settings. Without the preset it is
		// invisible, so only fighter lighting shows.
		public static Color CurrentColor
		{
			get
			{
				ModVisualDefinition settings = ModVisuals.Active(ModVisualEffect.RimLight);
				if (settings == null) return new Color(1f, 1f, 1f, 0f);
				if (SceneColor.HasValue) return SceneColor.Value;
				Color c = Color.Lerp(PreviewLight, Color.white, settings.Number("lighten"));
				c.a = settings.Number("alpha");
				return c;
			}
		}

		// The rim colour for one fighter's renderer: CurrentColor, turned toward a
		// nearby light's colour, then toward the ink colour while that fighter casts
		// magic (rim_light ink > 0).
		public static Color ColorFor(Transform renderer)
		{
			Color color = CurrentColor;
			FighterParticles fighter = renderer != null ? renderer.GetComponentInParent<FighterParticles>() : null;
			if (fighter == null) return color;
			float lit = fighter.LightAmount;
			if (lit > 0f)
			{
				Color light = fighter.LightColor;
				float a = Mathf.Max(color.a, lit);
				color = color.a <= 0f ? light : Color.Lerp(color, light, lit);
				color.a = a;
			}
			ModVisualDefinition settings = ModVisuals.Active(ModVisualEffect.RimLight);
			if (settings == null || settings.Number("ink") <= 0f) return color;
			float weight = fighter.InkWeight * settings.Number("ink");
			if (weight <= 0f) return color;
			Color ink = ModVisuals.ColorOf(settings) ?? DefaultInk;
			ink.a = Mathf.Max(color.a, ink.a < 1f ? ink.a : color.a);
			return Color.Lerp(color, ink, weight);
		}

		// Offset toward the upper left of the screen by a constant pixel distance
		// (or toward a nearby light), expressed in the renderer parent's local space,
		// and pushed slightly back.
		public static Vector3 LocalOffset(Transform parent)
		{
			ModVisualDefinition settings = ModVisuals.Active(ModVisualEffect.RimLight);
			float offsetPixels = settings != null ? settings.Number("offset") : LightOffset;
			Vector2 direction = new Vector2(-1f, 1f);
			FighterParticles fighter = parent != null ? parent.GetComponentInParent<FighterParticles>() : null;
			if (fighter != null && fighter.LightAmount > 0f)
			{
				// The lit edge faces the light; with the preset it blends from the default side.
				Vector2 toward = fighter.LightDirection * 1.41421356f;
				direction = settings != null ? Vector2.Lerp(direction, toward, fighter.LightAmount) : toward;
				offsetPixels = Mathf.Max(offsetPixels, LightOffset);
			}
			UnityEngine.Camera camera = UnityEngine.Camera.main;
			float pixel = camera != null && camera.orthographic && Screen.height > 0
				? 2f * camera.orthographicSize / Screen.height : 1f;
			Vector3 world = new Vector3(direction.x * offsetPixels * pixel, direction.y * offsetPixels * pixel, 0f);
			Vector3 local = parent != null ? parent.InverseTransformVector(world) : world;
			local.z = Depth;
			return local;
		}
	}
}

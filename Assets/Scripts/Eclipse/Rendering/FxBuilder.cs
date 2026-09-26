using System;
using System.Collections.Generic;
using Eclipse.Modding;
using UnityEngine;

namespace Eclipse.Rendering
{
	// Shared pieces for sf2.fx renderers: materials per blend mode and texture,
	// mod sprite loading, and a particle emitter configured from a definition.
	public static class FxBuilder
	{
		private static readonly Dictionary<(Texture, ModFxBlend), Material> Materials = new Dictionary<(Texture, ModFxBlend), Material>();
		private static Texture2D _softDot;
		private static Sprite _whiteSprite;
		private static Sprite _shaftSprite, _glowSprite, _shadowSprite;
		private static Shader _additive;
		private static bool _additiveMissing;

		public static Texture2D SoftDot
		{
			get
			{
				if (_softDot != null) return _softDot;
				const int size = 32;
				var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave };
				var pixels = new Color32[size * size];
				for (int y = 0; y < size; y++)
					for (int x = 0; x < size; x++)
					{
						float dx = (x + 0.5f) / size * 2f - 1f, dy = (y + 0.5f) / size * 2f - 1f;
						float a = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
						pixels[y * size + x] = new Color32(255, 255, 255, (byte)(a * a * 255f));
					}
				texture.SetPixels32(pixels);
				texture.Apply(false, true);
				return _softDot = texture;
			}
		}

		public static Sprite WhiteSprite
		{
			get
			{
				if (_whiteSprite != null) return _whiteSprite;
				Texture2D white = Texture2D.whiteTexture;
				return _whiteSprite = Sprite.Create(white, new Rect(0f, 0f, white.width, white.height), new Vector2(0.5f, 0.5f), white.width);
			}
		}

		// Built-in overlay art: a flat fill, a soft vertical beam or a soft round light.
		public static Sprite ShapeSprite(ModFxShape shape)
		{
			if (shape == ModFxShape.Shaft)
				return _shaftSprite != null ? _shaftSprite : _shaftSprite = Generate(32, 128, (u, v) =>
				{
					// Soft sides; brightest near the source (top) and fading out below.
					float across = 1f - Mathf.Abs(u * 2f - 1f);
					float side = across * across * (3f - 2f * across);
					float top = Mathf.Clamp01((1f - v) / 0.12f);
					float along = v * v * (3f - 2f * v) * top * top * (3f - 2f * top);
					return side * along;
				});
			if (shape == ModFxShape.Glow)
				return _glowSprite != null ? _glowSprite : _glowSprite = Generate(64, 64, (u, v) =>
				{
					float dx = u * 2f - 1f, dy = v * 2f - 1f;
					float a = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
					return a * a;
				});
			return WhiteSprite;
		}

		// A soft oval that is densest in the middle, for contact shadows.
		public static Sprite ShadowSprite
		{
			get
			{
				return _shadowSprite != null ? _shadowSprite : _shadowSprite = Generate(64, 32, (u, v) =>
				{
					float dx = u * 2f - 1f, dy = v * 2f - 1f;
					float d = Mathf.Sqrt(dx * dx + dy * dy);
					float a = Mathf.Clamp01(1f - d);
					return a * (0.4f + 0.6f * a);
				});
			}
		}

		// White texture with a generated alpha; v runs from the bottom (0) to the top (1).
		private static Sprite Generate(int width, int height, Func<float, float, float> alpha)
		{
			var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
				{ wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave };
			var pixels = new Color32[width * height];
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
					pixels[y * width + x] = new Color32(255, 255, 255,
						(byte)(Mathf.Clamp01(alpha((x + 0.5f) / width, (y + 0.5f) / height)) * 255f));
			texture.SetPixels32(pixels);
			texture.Apply(false, true);
			return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), width);
		}

		// A one-shot emitter for hit particles: nothing plays until Emit is called.
		// Particles fly outward at speed_min..speed_max (plus the velocity ranges)
		// and fall with gravity, simulated in world space.
		public static ParticleSystem CreateBurst(Transform parent, ModFxDefinition definition)
		{
			var go = new GameObject("Effect " + definition.Name);
			go.transform.SetParent(parent, false);
			var system = go.AddComponent<ParticleSystem>();
			system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			var main = system.main;
			// Kept playing with emission off, so each Emit simulates at once.
			main.loop = true;
			main.playOnAwake = true;
			main.simulationSpace = ParticleSystemSimulationSpace.World;
			main.scalingMode = ParticleSystemScalingMode.Hierarchy;
			main.maxParticles = Mathf.Max(1, Mathf.RoundToInt(definition.Number("count"))) * 4;
			main.startLifetime = new ParticleSystem.MinMaxCurve(definition.Number("lifetime_min"), definition.Number("lifetime_max"));
			main.startSize = new ParticleSystem.MinMaxCurve(definition.Number("size_min"), definition.Number("size_max"));
			main.startSpeed = new ParticleSystem.MinMaxCurve(definition.Number("speed_min"), definition.Number("speed_max"));
			main.startColor = ModVisuals.ToColor(definition.Color, Color.white);
			if (definition.Number("spin") > 0f)
				main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f * definition.Number("spin"));
			var emission = system.emission;
			emission.enabled = false;
			var shape = system.shape;
			shape.shapeType = ParticleSystemShapeType.Circle;
			shape.radius = Mathf.Max(0.01f, definition.Number("radius"));
			var velocity = system.velocityOverLifetime;
			velocity.enabled = true;
			velocity.space = ParticleSystemSimulationSpace.World;
			velocity.x = new ParticleSystem.MinMaxCurve(definition.Number("velocity_x_min"), definition.Number("velocity_x_max"));
			velocity.y = new ParticleSystem.MinMaxCurve(definition.Number("velocity_y_min"), definition.Number("velocity_y_max"));
			velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);
			var force = system.forceOverLifetime;
			force.enabled = definition.Number("gravity") != 0f;
			force.space = ParticleSystemSimulationSpace.World;
			force.x = new ParticleSystem.MinMaxCurve(0f, 0f);
			force.y = new ParticleSystem.MinMaxCurve(-definition.Number("gravity"), -definition.Number("gravity"));
			force.z = new ParticleSystem.MinMaxCurve(0f, 0f);
			float noiseStrength = definition.Number("noise");
			var noise = system.noise;
			noise.enabled = noiseStrength > 0f;
			noise.frequency = 0.5f;
			noise.strength = noiseStrength;
			var colour = system.colorOverLifetime;
			colour.enabled = true;
			Color end = definition.EndColor != null ? ModVisuals.ToColor(definition.EndColor, Color.white) : Color.white;
			if (definition.EndColor != null && definition.Color != null)
			{
				Color c = ModVisuals.ToColor(definition.Color, Color.white);
				end = new Color(end.r / Mathf.Max(c.r, 1e-3f), end.g / Mathf.Max(c.g, 1e-3f), end.b / Mathf.Max(c.b, 1e-3f), end.a / Mathf.Max(c.a, 1e-3f));
			}
			var gradient = new Gradient();
			// Full strength at once (a burst), fading out over the lifetime.
			gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(new Color(Mathf.Clamp01(end.r), Mathf.Clamp01(end.g), Mathf.Clamp01(end.b)), 1f) },
				new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(Mathf.Clamp01(end.a), 0.6f), new GradientAlphaKey(0f, 1f) });
			colour.color = gradient;
			var renderer = go.GetComponent<ParticleSystemRenderer>();
			renderer.renderMode = ParticleSystemRenderMode.Billboard;
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			Sprite sprite = LoadSprite(definition.Sprite, definition.Name);
			if (sprite != null)
			{
				var sheet = system.textureSheetAnimation;
				sheet.enabled = true;
				sheet.mode = ParticleSystemAnimationMode.Sprites;
				sheet.AddSprite(sprite);
				renderer.sharedMaterial = MaterialFor(sprite.texture, definition.Blend);
			}
			else
			{
				renderer.sharedMaterial = MaterialFor(SoftDot, definition.Blend);
			}
			system.Play(true);
			return system;
		}

		// Alpha uses the built-in sprite shader; additive uses Eclipse's own.
		public static Material MaterialFor(Texture texture, ModFxBlend blend)
		{
			if (blend == ModFxBlend.Additive && _additive == null && !_additiveMissing)
			{
				_additive = Resources.Load<Shader>("shaders/EclipseFxAdditive");
				if (_additive == null || !_additive.isSupported)
				{
					_additiveMissing = true;
					Debug.LogWarning("[Eclipse] Additive effect shader is unavailable; using alpha blending.");
				}
			}
			if (blend == ModFxBlend.Additive && _additive == null) blend = ModFxBlend.Alpha;
			var key = (texture, blend);
			if (Materials.TryGetValue(key, out var material) && material != null) return material;
			material = new Material(blend == ModFxBlend.Additive ? _additive : Shader.Find("Sprites/Default")) { hideFlags = HideFlags.HideAndDontSave };
			if (texture != null) material.mainTexture = texture;
			Materials[key] = material;
			return material;
		}

		public static Sprite LoadSprite(AssetId? id, string effect)
		{
			if (!id.HasValue || !ModRuntime.IsInitialized) return null;
			try { return ModRuntime.Host.TypedAssets.LoadSprite(id.Value); }
			catch (Exception error)
			{
				Debug.LogWarning("[Eclipse] Effect '" + effect + "' sprite failed to load: " + error.Message);
				return null;
			}
		}

		// A looping emitter for a Particles definition. `area` is the box size for
		// location emitters; node emitters use the definition's radius instead.
		public static ParticleSystem CreateEmitter(Transform parent, Vector3 localPosition, Vector3 localScale,
			ModFxDefinition definition, Vector2 area, bool worldSpace)
		{
			var go = new GameObject("Effect " + definition.Name);
			go.transform.SetParent(parent, false);
			go.transform.localPosition = localPosition;
			go.transform.localScale = localScale;
			var system = go.AddComponent<ParticleSystem>();
			system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

			var main = system.main;
			main.loop = true;
			main.prewarm = !worldSpace;
			main.playOnAwake = true;
			main.simulationSpace = worldSpace ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;
			main.scalingMode = ParticleSystemScalingMode.Hierarchy;
			main.startSpeed = 0f;
			int count = Mathf.RoundToInt(definition.Number("count"));
			float lifeMin = definition.Number("lifetime_min"), lifeMax = definition.Number("lifetime_max");
			main.maxParticles = Mathf.Max(1, count);
			main.startLifetime = new ParticleSystem.MinMaxCurve(lifeMin, lifeMax);
			main.startSize = new ParticleSystem.MinMaxCurve(definition.Number("size_min"), definition.Number("size_max"));
			main.startColor = ModVisuals.ToColor(definition.Color, Color.white);
			if (definition.Number("spin") > 0f)
				main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f * definition.Number("spin"));

			var shape = system.shape;
			if (definition.Placement == ModFxPlacement.Node)
			{
				shape.shapeType = ParticleSystemShapeType.Circle;
				shape.radius = Mathf.Max(0.01f, definition.Number("radius"));
			}
			else
			{
				shape.shapeType = ParticleSystemShapeType.Box;
				shape.scale = new Vector3(Mathf.Max(area.x, 1f), Mathf.Max(area.y, 1f), 0.1f);
			}

			var velocity = system.velocityOverLifetime;
			velocity.enabled = true;
			velocity.space = worldSpace ? ParticleSystemSimulationSpace.World : ParticleSystemSimulationSpace.Local;
			velocity.x = new ParticleSystem.MinMaxCurve(definition.Number("velocity_x_min"), definition.Number("velocity_x_max"));
			velocity.y = new ParticleSystem.MinMaxCurve(definition.Number("velocity_y_min"), definition.Number("velocity_y_max"));
			velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);

			float noiseStrength = definition.Number("noise");
			var noise = system.noise;
			noise.enabled = noiseStrength > 0f;
			noise.frequency = 0.15f;
			noise.strength = noiseStrength;

			// Fade in and out; blend toward end_color over the lifetime when given.
			Color start = Color.white;
			Color end = definition.EndColor != null ? ModVisuals.ToColor(definition.EndColor, Color.white) : Color.white;
			if (definition.EndColor != null && definition.Color != null)
			{
				Color c = ModVisuals.ToColor(definition.Color, Color.white);
				end = new Color(end.r / Mathf.Max(c.r, 1e-3f), end.g / Mathf.Max(c.g, 1e-3f), end.b / Mathf.Max(c.b, 1e-3f), end.a / Mathf.Max(c.a, 1e-3f));
			}
			var colour = system.colorOverLifetime;
			colour.enabled = true;
			var gradient = new Gradient();
			gradient.SetKeys(new[] { new GradientColorKey(start, 0f), new GradientColorKey(new Color(Mathf.Clamp01(end.r), Mathf.Clamp01(end.g), Mathf.Clamp01(end.b)), 1f) },
				new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.2f), new GradientAlphaKey(Mathf.Clamp01(end.a), 0.8f), new GradientAlphaKey(0f, 1f) });
			colour.color = gradient;

			var emission = system.emission;
			emission.rateOverTime = main.maxParticles / Mathf.Max(lifeMax, 0.05f);

			var renderer = go.GetComponent<ParticleSystemRenderer>();
			renderer.renderMode = ParticleSystemRenderMode.Billboard;
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			Sprite sprite = LoadSprite(definition.Sprite, definition.Name);
			if (sprite != null)
			{
				var sheet = system.textureSheetAnimation;
				sheet.enabled = true;
				sheet.mode = ParticleSystemAnimationMode.Sprites;
				sheet.AddSprite(sprite);
				renderer.sharedMaterial = MaterialFor(sprite.texture, definition.Blend);
			}
			else
			{
				renderer.sharedMaterial = MaterialFor(SoftDot, definition.Blend);
			}
			system.Play(true);
			return system;
		}
	}
}

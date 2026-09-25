using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Rendering
{
	// Experimental location presentation, attached to a fight's render root:
	// depth haze over background layers, the scene colour used by the rim light,
	// and ambient particles on the game layer. Everything it creates is inactive
	// unless its Options > Experimental switch is on.
	public sealed class LocationAtmosphere : MonoBehaviour
	{
		private const float HazeStrength = 0.4f;
		private const float OverlayDepth = -2.9f;   // in front of a layer's own art, behind the next layer

		private Location _location;
		private readonly List<SpriteRenderer> _overlays = new List<SpriteRenderer>();
		private Color? _sceneColor;
		private GameObject _particles;
		private Sprite _whiteSprite;
		private Material _particleMaterial;
		private Texture2D _particleTexture;

		public static void Attach(GameObject root, Location location)
		{
			var atmosphere = root.GetComponent<LocationAtmosphere>() ?? root.AddComponent<LocationAtmosphere>();
			atmosphere._location = location;
		}

		private void LateUpdate()
		{
			if (_location == null || _location.layers == null) return;
			bool haze = ExperimentalVisuals.DepthHaze;
			bool rim = ExperimentalVisuals.RimLight;
			if ((haze || rim) && !_sceneColor.HasValue) _sceneColor = SampleSceneColor();

			RimLight.SceneColor = rim && _sceneColor.HasValue ? RimColor(_sceneColor.Value) : (Color?)null;

			if (haze && _overlays.Count == 0) BuildHaze();
			for (int i = 0; i < _overlays.Count; i++)
				if (_overlays[i] != null && _overlays[i].gameObject.activeSelf != haze) _overlays[i].gameObject.SetActive(haze);

			bool particles = ExperimentalVisuals.AmbientParticles;
			if (particles && _particles == null) BuildParticles();
			if (_particles != null && _particles.activeSelf != particles) _particles.SetActive(particles);
		}

		// Aerial perspective: layer i should show haze h_i = strength * (1 - factor).
		// Overlays in front of each background layer stack onto everything behind
		// them, so solve each overlay's alpha from the nearer layer's haze.
		private void BuildHaze()
		{
			var background = new List<LocationSelector>();
			foreach (LocationSelector layer in _location.layers)
			{
				if (layer.BBELALLBKHH()) break;
				background.Add(layer);
			}
			if (background.Count == 0 || !_sceneColor.HasValue) return;
			if (_whiteSprite == null)
			{
				Texture2D white = Texture2D.whiteTexture;
				_whiteSprite = Sprite.Create(white, new Rect(0f, 0f, white.width, white.height), new Vector2(0.5f, 0.5f), white.width);
			}
			float nearer = 0f;
			for (int i = background.Count - 1; i >= 0; i--)
			{
				float h = HazeStrength * (1f - Mathf.Clamp01(background[i].JLBBJEELMGG()));
				float alpha = h <= nearer ? 0f : 1f - (1f - h) / Mathf.Max(1f - nearer, 1e-3f);
				nearer = Mathf.Max(nearer, h);
				if (alpha <= 0.005f) continue;
				var overlay = new GameObject("Depth haze").AddComponent<SpriteRenderer>();
				overlay.sprite = _whiteSprite;
				overlay.transform.SetParent(background[i].MJNPBMOAFML().transform, false);
				overlay.transform.localPosition = new Vector3(0f, 0f, OverlayDepth);
				overlay.transform.localScale = new Vector3(40000f, 40000f, 1f);
				Color color = _sceneColor.Value; color.a = alpha;
				overlay.color = color;
				_overlays.Add(overlay);
			}
		}

		private static Color RimColor(Color scene)
		{
			// A brightened, slightly desaturated version of the background light.
			Color c = Color.Lerp(scene, Color.white, 0.35f);
			float max = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
			if (max > 0.001f && max < 0.75f) c *= 0.75f / max;
			c.a = 0.85f;
			return c;
		}

		// Average colour of the farthest background layer's art, read once on the GPU.
		private Color SampleSceneColor()
		{
			Color fallback = new Color(0.6f, 0.65f, 0.75f, 1f);
			if (_location.layers.Count == 0) return fallback;
			var renderers = _location.layers[0].MJNPBMOAFML().GetComponentsInChildren<SpriteRenderer>(true);
			if (renderers.Length == 0) return fallback;
			RenderTexture rt = RenderTexture.GetTemporary(8, 8, 0, RenderTextureFormat.ARGB32);
			var readback = new Texture2D(8, 8, TextureFormat.RGBA32, false);
			RenderTexture previous = RenderTexture.active;
			Vector4 sum = Vector4.zero; float weight = 0f;
			try
			{
				int used = 0;
				foreach (SpriteRenderer renderer in renderers)
				{
					Sprite sprite = renderer.sprite;
					if (sprite == null || sprite.texture == null || used++ >= 8) continue;
					Texture texture = sprite.texture;
					Rect rect = sprite.textureRect;
					Vector2 scale = new Vector2(rect.width / texture.width, rect.height / texture.height);
					Vector2 offset = new Vector2(rect.x / texture.width, rect.y / texture.height);
					Graphics.Blit(texture, rt, scale, offset);
					RenderTexture.active = rt;
					readback.ReadPixels(new Rect(0, 0, 8, 8), 0, 0);
					readback.Apply(false);
					foreach (Color32 pixel in readback.GetPixels32())
					{
						float a = pixel.a / 255f;
						sum += new Vector4(pixel.r / 255f, pixel.g / 255f, pixel.b / 255f, 0f) * a;
						weight += a;
					}
				}
			}
			finally
			{
				RenderTexture.active = previous;
				RenderTexture.ReleaseTemporary(rt);
				Destroy(readback);
			}
			if (weight < 0.01f) return fallback;
			return new Color(sum.x / weight, sum.y / weight, sum.z / weight, 1f);
		}

		private enum Preset { Dust, Snow, Embers, Petals }

		private static Preset ChoosePreset(string name)
		{
			var tokens = new HashSet<string>((name ?? string.Empty).ToLowerInvariant()
				.Split(new[] { '_', '-', ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }, System.StringSplitOptions.RemoveEmptyEntries));
			if (Any(tokens, "ny", "xmas", "christmas", "winter", "snow", "ice", "frost", "newyear")) return Preset.Snow;
			if (Any(tokens, "hw", "haloween", "halloween", "volcano", "fire", "burn", "burning", "magma", "lava", "hell", "inferno", "uw", "underworld")) return Preset.Embers;
			if (Any(tokens, "china", "chinese", "sakura", "spring", "cherry", "india", "indian")) return Preset.Petals;
			return Preset.Dust;
		}

		private static bool Any(HashSet<string> tokens, params string[] words)
		{
			foreach (string word in words) if (tokens.Contains(word)) return true;
			return false;
		}

		private void BuildParticles()
		{
			if (_location.gameLayer == null) return;
			float width = Mathf.Max(_location.JMLAKAKDBBL, 1f);
			float height = Mathf.Max(_location.FEIHFIPFNKF, 1f);
			Preset preset = ChoosePreset(_location.name);

			_particles = new GameObject("Ambient particles (" + preset + ")");
			_particles.transform.SetParent(_location.gameLayer.MJNPBMOAFML().transform, false);
			// Behind the game layer's fighters and floor, in front of the next background layer.
			_particles.transform.localPosition = new Vector3(0f, 0f, 1f);
			// The render root is mirrored vertically; flip back so +y is up.
			_particles.transform.localScale = new Vector3(1f, -1f, 1f);

			var system = _particles.AddComponent<ParticleSystem>();
			system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			var main = system.main;
			main.loop = true;
			main.prewarm = true;
			main.playOnAwake = true;
			main.simulationSpace = ParticleSystemSimulationSpace.Local;
			main.scalingMode = ParticleSystemScalingMode.Hierarchy;
			main.startSpeed = 0f;
			var shape = system.shape;
			shape.shapeType = ParticleSystemShapeType.Box;
			shape.scale = new Vector3(width * 1.1f, height * 1.1f, 0.1f);
			var velocity = system.velocityOverLifetime;
			velocity.enabled = true;
			velocity.space = ParticleSystemSimulationSpace.Local;
			var noise = system.noise;
			noise.enabled = true;
			noise.frequency = 0.15f;
			var fade = system.colorOverLifetime;
			fade.enabled = true;
			var gradient = new Gradient();
			gradient.SetKeys(new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
				new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.2f), new GradientAlphaKey(1f, 0.8f), new GradientAlphaKey(0f, 1f) });
			fade.color = gradient;
			var emission = system.emission;

			switch (preset)
			{
			case Preset.Snow:
				Configure(ref main, 220, 10f, 14f, 3f, 8f, new Color(1f, 1f, 1f, 0.75f));
				velocity.x = new ParticleSystem.MinMaxCurve(-15f, 15f);
				velocity.y = new ParticleSystem.MinMaxCurve(-80f, -35f);
				noise.strength = 12f;
				break;
			case Preset.Embers:
				Configure(ref main, 110, 4f, 7f, 2f, 5f, new Color(1f, 0.55f, 0.18f, 0.9f));
				velocity.x = new ParticleSystem.MinMaxCurve(-12f, 12f);
				velocity.y = new ParticleSystem.MinMaxCurve(30f, 75f);
				noise.strength = 25f;
				break;
			case Preset.Petals:
				Configure(ref main, 80, 10f, 14f, 5f, 9f, new Color(1f, 0.74f, 0.82f, 0.8f));
				velocity.x = new ParticleSystem.MinMaxCurve(15f, 40f);
				velocity.y = new ParticleSystem.MinMaxCurve(-40f, -18f);
				noise.strength = 18f;
				main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
				break;
			default:
				Configure(ref main, 120, 8f, 13f, 2.5f, 6f, new Color(1f, 0.95f, 0.85f, 0.28f));
				velocity.x = new ParticleSystem.MinMaxCurve(-8f, 8f);
				velocity.y = new ParticleSystem.MinMaxCurve(-5f, 7f);
				noise.strength = 6f;
				break;
			}
			velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);
			emission.rateOverTime = main.maxParticles / Mathf.Max(main.startLifetime.constantMax, 1f);

			var renderer = _particles.GetComponent<ParticleSystemRenderer>();
			renderer.renderMode = ParticleSystemRenderMode.Billboard;
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			if (_particleTexture == null) _particleTexture = SoftDot();
			if (_particleMaterial == null)
				_particleMaterial = new Material(Shader.Find("Sprites/Default")) { mainTexture = _particleTexture };
			renderer.sharedMaterial = _particleMaterial;
			system.Play(true);
		}

		private static void Configure(ref ParticleSystem.MainModule main, int max, float minLife, float maxLife,
			float minSize, float maxSize, Color color)
		{
			main.maxParticles = max;
			main.startLifetime = new ParticleSystem.MinMaxCurve(minLife, maxLife);
			main.startSize = new ParticleSystem.MinMaxCurve(minSize, maxSize);
			main.startColor = color;
		}

		private static Texture2D SoftDot()
		{
			const int size = 32;
			var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
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
			return texture;
		}

		private void OnDestroy()
		{
			RimLight.SceneColor = null;
			if (_particleMaterial != null) Destroy(_particleMaterial);
			if (_particleTexture != null) Destroy(_particleTexture);
		}
	}
}

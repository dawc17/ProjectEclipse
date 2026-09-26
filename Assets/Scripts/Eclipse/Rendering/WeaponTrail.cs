using System.Collections.Generic;
using System.Text;
using Eclipse.Modding;
using Eclipse.Rendering.Interpolation;
using UnityEngine;

namespace Eclipse.Rendering
{
	// Fighter trails: the sf2.visuals.weapon_trails preset plus any sf2.fx.trail
	// effects, and sf2.fx.glint highlights that run along a still blade. Each trail is a short ribbon between two points of the fighter,
	// sampled every rendered frame from the interpolated pose. Segments fade with
	// age and with how fast the far point moved, so still limbs leave nothing.
	//
	// Weapon art is built from macro nodes: weighted combinations of the
	// skeleton's Weapon-Node1..4_N control points, extending beyond them. So a
	// weapon trail uses the weapon's own geometry: edges named "*-Blade" when
	// the model has them, otherwise, per hand (_1 main, _2 off hand), the two
	// farthest macro nodes driven by that hand.
	public sealed class WeaponTrail : MonoBehaviour
	{
		private const int MaxSamples = 48;
		private const float MinBladeLength = 25f;

		private struct Sample { public Vector3 Grip, Tip; public float Time, Strength; }

		private sealed class Blade
		{
			public ModelNode Grip, Tip;
			public readonly List<Sample> Samples = new List<Sample>();
		}

		private sealed class Stream
		{
			public float Lifetime, MinSpeed, FullSpeed, Alpha, StartAlpha;
			public Color? Color;
			public bool Additive, Weapon;
			public string[] Nodes;
			public readonly List<Blade> Blades = new List<Blade>();
			// Glints: a light that sweeps grip to tip every `Interval` seconds.
			public bool Glint;
			public float Interval, Duration, Size, MaxSpeed, NextTime = -1f, StartTime = -1f;
			public int GlintBlade;
		}

		private Model _model;
		private ModelPresentation _presentation;
		private readonly List<Stream> _streams = new List<Stream>();
		private string _key;
		private int _nodeCount = -1;
		private Mesh _alphaMesh, _additiveMesh, _glintMesh;
		private MeshRenderer _additiveRenderer;
		private readonly List<Vector3> _vertices = new List<Vector3>();
		private readonly List<Color> _colors = new List<Color>();
		private readonly List<int> _triangles = new List<int>();

		public static void Attach(GameObject root, Model model)
		{
			var trail = root.GetComponent<WeaponTrail>() ?? root.AddComponent<WeaponTrail>();
			trail._model = model;
		}

		private void Start()
		{
			_presentation = GetComponent<ModelPresentation>();
			_alphaMesh = CreateLayer("Trails", ModFxBlend.Alpha, out _);
			_additiveMesh = CreateLayer("Trails (additive)", ModFxBlend.Additive, out _additiveRenderer);
			_glintMesh = CreateLayer("Glints", ModFxBlend.Additive, out MeshRenderer glints);
			// Glints sit in front of the body; trails stay behind it.
			glints.transform.localPosition = new Vector3(0f, 0f, -0.05f);
		}

		private Mesh CreateLayer(string name, ModFxBlend blend, out MeshRenderer renderer)
		{
			var child = new GameObject(name);
			child.transform.SetParent(transform, false);
			// Just behind the fighter's own body renderers.
			child.transform.localPosition = new Vector3(0f, 0f, 0.1f);
			var mesh = new Mesh { name = name };
			mesh.MarkDynamic();
			child.AddComponent<MeshFilter>().sharedMesh = mesh;
			renderer = child.AddComponent<MeshRenderer>();
			renderer.sharedMaterial = FxBuilder.MaterialFor(null, blend);
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.receiveShadows = false;
			return mesh;
		}

		private void LateUpdate()
		{
			if (_alphaMesh == null || _model == null) return;
			RefreshStreams();
			float now = Time.unscaledTime;
			float alpha = _presentation != null ? _presentation.Alpha : FightInterpolation.FightAlpha;
			foreach (Stream stream in _streams)
				foreach (Blade blade in stream.Blades)
				{
					Vector3 grip = Sample3(blade.Grip, alpha);
					Vector3 tip = Sample3(blade.Tip, alpha);
					List<Sample> samples = blade.Samples;
					float strength = 0f;
					if (samples.Count > 0)
					{
						Sample last = samples[samples.Count - 1];
						float dt = Mathf.Max(now - last.Time, 1e-4f);
						float speed = (tip - last.Tip).magnitude / dt;
						strength = Mathf.Clamp01((speed - stream.MinSpeed) / (stream.FullSpeed - stream.MinSpeed));
					}
					if (samples.Count == 0 || now > samples[samples.Count - 1].Time)
						samples.Add(new Sample { Grip = grip, Tip = tip, Time = now, Strength = strength });
					while (samples.Count > 0 && (now - samples[0].Time > stream.Lifetime || samples.Count > MaxSamples))
						samples.RemoveAt(0);
				}
			Rebuild(_alphaMesh, false, now);
			Rebuild(_additiveMesh, true, now);
			RebuildGlints(now);
		}

		// Rebuilds the trail list when the active effects or the model's nodes change.
		private void RefreshStreams()
		{
			bool inFight = FightInterpolation.IsFightActive;
			string location = inFight ? LocationAtmosphere.CurrentLocationName : null;
			var key = new StringBuilder();
			ModVisualDefinition preset = ModVisuals.Active(ModVisualEffect.WeaponTrails);
			if (preset != null) key.Append("preset|");
			var custom = new List<ModFxDefinition>();
			foreach (ModFxDefinition definition in ModVisuals.ActiveFx(ModFxKind.Trail))
			{
				if (definition.Scenes == ModFxScenes.Fights && !inFight) continue;
				if (inFight && !definition.MatchesLocation(location)) continue;
				if (!FighterMatches(definition.Fighters)) continue;
				custom.Add(definition);
				key.Append(definition.Name).Append('|');
			}
			var glints = new List<ModFxDefinition>();
			foreach (ModFxDefinition definition in ModVisuals.ActiveFx(ModFxKind.Glint))
			{
				if (definition.Scenes == ModFxScenes.Fights && !inFight) continue;
				if (inFight && !definition.MatchesLocation(location)) continue;
				if (!FighterMatches(definition.Fighters)) continue;
				glints.Add(definition);
				key.Append(definition.Name).Append('|');
			}
			Dictionary<string, ModelNode> nodes = _model.CLDMEJKGLBA()?.HKCFFKKFFFE();
			int nodeCount = nodes != null ? nodes.Count : 0;
			string built = key.ToString();
			if (built == _key && nodeCount == _nodeCount)
			{
				UpdateStreamSettings(preset, custom);
				UpdateGlintSettings(glints);
				return;
			}
			_key = built; _nodeCount = nodeCount;
			_streams.Clear();
			if (preset != null) _streams.Add(new Stream { Weapon = true });
			foreach (ModFxDefinition definition in custom)
				_streams.Add(new Stream { Weapon = definition.Weapon, Nodes = definition.Nodes.Count == 2 ? new[] { definition.Nodes[0], definition.Nodes[1] } : null });
			foreach (ModFxDefinition definition in glints)
				_streams.Add(new Stream { Weapon = true, Glint = true, Lifetime = 0.1f });
			UpdateStreamSettings(preset, custom);
			UpdateGlintSettings(glints);
			foreach (Stream stream in _streams) ResolveBlades(stream);
		}

		private void UpdateStreamSettings(ModVisualDefinition preset, List<ModFxDefinition> custom)
		{
			int index = 0;
			if (preset != null && index < _streams.Count)
			{
				Stream s = _streams[index++];
				s.Lifetime = preset.Number("lifetime"); s.MinSpeed = preset.Number("min_speed"); s.FullSpeed = preset.Number("full_speed");
				s.Alpha = preset.Number("alpha"); s.StartAlpha = 0.35f; s.Color = ModVisuals.ColorOf(preset); s.Additive = false;
			}
			foreach (ModFxDefinition definition in custom)
			{
				if (index >= _streams.Count || _streams[index].Glint) break;
				Stream s = _streams[index++];
				s.Lifetime = definition.Number("lifetime"); s.MinSpeed = definition.Number("min_speed"); s.FullSpeed = definition.Number("full_speed");
				s.Alpha = definition.Number("alpha"); s.StartAlpha = definition.Number("start_alpha");
				s.Color = definition.Color != null ? ModVisuals.ToColor(definition.Color, Color.white) : (Color?)null;
				s.Additive = definition.Blend == ModFxBlend.Additive;
			}
		}

		private void UpdateGlintSettings(List<ModFxDefinition> glints)
		{
			int index = 0;
			foreach (Stream s in _streams)
			{
				if (!s.Glint) continue;
				if (index >= glints.Count) break;
				ModFxDefinition d = glints[index++];
				s.Interval = d.Number("interval"); s.Duration = d.Number("duration"); s.Size = d.Number("size");
				s.MaxSpeed = d.Number("max_speed"); s.Alpha = d.Number("alpha");
				s.Color = d.Color != null ? ModVisuals.ToColor(d.Color, Color.white) : (Color?)null;
			}
		}

		// Player/opponent come from the running fight; menu previews count as the player.
		private bool FighterMatches(ModFxFighters fighters)
		{
			if (fighters == ModFxFighters.Both) return true;
			Fight fight = FightInterpolation.IsFightActive ? Fight.GetCurrentFight() : null;
			bool player = fight == null || fight.GetPlayerModel() == _model;
			bool opponent = fight != null && fight.GetEnemyModel() == _model;
			return fighters == ModFxFighters.Player ? player : opponent;
		}

		private static Vector3 Sample3(ModelNode node, float alpha)
		{
			float x, y, z;
			FightInterpolation.SamplePosition(node, alpha, out x, out y, out z);
			return new Vector3(x, y, 0f);
		}

		private static Vector3 ToVector(Vector3f value)
		{
			return new Vector3(value.GetX(), value.GetY(), value.GetZ());
		}

		private void ResolveBlades(Stream stream)
		{
			ModelObject body = _model.CLDMEJKGLBA();
			Dictionary<string, ModelNode> nodes = body?.HKCFFKKFFFE();
			if (nodes == null) return;
			Vector3 pivot = body.HOFFDCFEBGA() != null ? ToVector(body.HOFFDCFEBGA().GetStart()) : Vector3.zero;

			if (!stream.Weapon)
			{
				// Two named nodes: the first is the inner end, the second the moving end.
				ModelNode inner = body.KLAPIGGACMM(stream.Nodes[0]);
				ModelNode outer = body.KLAPIGGACMM(stream.Nodes[1]);
				if (inner != null && outer != null && inner != outer) stream.Blades.Add(new Blade { Grip = inner, Tip = outer });
				return;
			}

			// 1. Explicit blade edges.
			List<ModelEdge> edges = body.BKAPPJMGPKP();
			if (edges != null)
				foreach (ModelEdge edge in edges)
				{
					string name = edge.get_Name();
					if (name == null || name.IndexOf("-Blade", System.StringComparison.Ordinal) < 0) continue;
					AddBlade(stream, edge.GetStartNode(), edge.GetEndNode(), pivot);
				}
			if (stream.Blades.Count != 0) return;

			// 2. The weapon's macro nodes, grouped by the hand whose control points drive them.
			var hands = new Dictionary<string, List<ModelNode>>();
			foreach (var pair in nodes)
			{
				var macro = pair.Value as ModelMacroNode;
				if (macro == null) continue;
				string hand = WeaponHand(macro);
				if (hand == null) continue;
				if (!hands.TryGetValue(hand, out var list)) hands[hand] = list = new List<ModelNode>();
				list.Add(macro);
			}
			foreach (var list in hands.Values)
			{
				ModelNode a = null, b = null; float best = MinBladeLength;
				for (int i = 0; i < list.Count; i++)
					for (int j = i + 1; j < list.Count; j++)
					{
						float d = Vector3.Distance(ToVector(list[i].GetStart()), ToVector(list[j].GetStart()));
						if (d > best) { best = d; a = list[i]; b = list[j]; }
					}
				if (a != null) AddBlade(stream, a, b, pivot);
			}
		}

		// "_1" or "_2" when every weighted control point is a skeleton weapon node of one hand.
		// The loader binds control-point names to nodes and may release the name list,
		// so read the bound nodes and fall back to the names.
		private static string WeaponHand(ModelMacroNode macro)
		{
			var names = new List<string>();
			List<global::Pair<ModelNode, float>> bound = macro.LDEBJOPLCKO();
			if (bound != null && bound.Count > 0)
			{
				foreach (var child in bound) names.Add(child?.First?.GetName());
			}
			else if (macro.LMPPCKACMNB != null)
			{
				foreach (var child in macro.LMPPCKACMNB) names.Add(child?.First);
			}
			if (names.Count == 0) return null;
			string hand = null;
			foreach (string name in names)
			{
				if (name == null || !name.StartsWith("Weapon-Node", System.StringComparison.Ordinal)) return null;
				int underscore = name.LastIndexOf('_');
				string suffix = underscore >= 0 ? name.Substring(underscore) : string.Empty;
				if (hand != null && hand != suffix) return null;
				hand = suffix;
			}
			return hand;
		}

		// The tip is the end farther from the body, so the ribbon is brightest there.
		private static void AddBlade(Stream stream, ModelNode a, ModelNode b, Vector3 pivot)
		{
			if (a == null || b == null || a == b) return;
			bool aIsTip = Vector3.Distance(ToVector(a.GetStart()), pivot) > Vector3.Distance(ToVector(b.GetStart()), pivot);
			stream.Blades.Add(new Blade { Grip = aIsTip ? b : a, Tip = aIsTip ? a : b });
		}

		private void Rebuild(Mesh mesh, bool additive, float now)
		{
			mesh.Clear();
			_vertices.Clear(); _colors.Clear(); _triangles.Clear();
			Color fighter = _presentation != null && _presentation.Tint.HasValue ? _presentation.Tint.Value : Color.black;
			foreach (Stream stream in _streams)
			{
				if (stream.Additive != additive || stream.Glint) continue;
				// An explicit colour wins; otherwise follow the fighter's own (perk) colour.
				Color tint = stream.Color ?? fighter;
				float colorAlpha = stream.Color.HasValue ? stream.Color.Value.a : 1f;
				foreach (Blade blade in stream.Blades)
				{
					List<Sample> samples = blade.Samples;
					if (samples.Count < 2) continue;
					int first = _vertices.Count;
					for (int i = 0; i < samples.Count; i++)
					{
						Sample s = samples[i];
						float age = Mathf.Clamp01((now - s.Time) / stream.Lifetime);
						float a = stream.Alpha * colorAlpha * (1f - age) * s.Strength;
						Color grip = tint; grip.a = a * stream.StartAlpha;
						Color tip = tint; tip.a = a;
						_vertices.Add(s.Grip); _colors.Add(grip);
						_vertices.Add(s.Tip); _colors.Add(tip);
						if (i > 0)
						{
							int v = first + i * 2;
							_triangles.Add(v - 2); _triangles.Add(v - 1); _triangles.Add(v);
							_triangles.Add(v - 1); _triangles.Add(v + 1); _triangles.Add(v);
						}
					}
				}
			}
			if (_triangles.Count == 0) return;
			mesh.SetVertices(_vertices);
			mesh.SetColors(_colors);
			mesh.SetTriangles(_triangles, 0);
			mesh.RecalculateBounds();
		}

		private static readonly Color GlintColor = new Color(1f, 0.97f, 0.9f, 1f);

		// A four-pointed star that travels from grip to tip and swells then fades.
		// It only starts while the blade is nearly still, so it never fights a trail.
		private void RebuildGlints(float now)
		{
			_glintMesh.Clear();
			_vertices.Clear(); _colors.Clear(); _triangles.Clear();
			foreach (Stream stream in _streams)
			{
				if (!stream.Glint || stream.Blades.Count == 0) continue;
				if (stream.NextTime < 0f) stream.NextTime = now + Random.Range(0.3f, 1f) * stream.Interval;
				if (stream.StartTime < 0f && now >= stream.NextTime)
				{
					int blade = Random.Range(0, stream.Blades.Count);
					if (BladeSpeed(stream.Blades[blade]) <= stream.MaxSpeed) { stream.StartTime = now; stream.GlintBlade = blade; }
				}
				if (stream.StartTime < 0f) continue;
				float t = (now - stream.StartTime) / stream.Duration;
				if (t >= 1f || stream.GlintBlade >= stream.Blades.Count)
				{
					stream.StartTime = -1f;
					stream.NextTime = now + stream.Interval * Random.Range(0.7f, 1.3f);
					continue;
				}
				List<Sample> samples = stream.Blades[stream.GlintBlade].Samples;
				if (samples.Count == 0) continue;
				Sample s = samples[samples.Count - 1];
				float along = Mathf.Lerp(0.2f, 1f, t * t * (3f - 2f * t));
				Vector3 centre = Vector3.Lerp(s.Grip, s.Tip, along);
				float swell = Mathf.Sin(t * Mathf.PI);
				Color color = stream.Color ?? GlintColor;
				color.a *= stream.Alpha * swell;
				float size = stream.Size * (0.4f + 0.6f * swell);
				AddStar(centre, size, color);
			}
			if (_triangles.Count == 0) return;
			_glintMesh.SetVertices(_vertices);
			_glintMesh.SetColors(_colors);
			_glintMesh.SetTriangles(_triangles, 0);
			_glintMesh.RecalculateBounds();
		}

		private static float BladeSpeed(Blade blade)
		{
			List<Sample> samples = blade.Samples;
			if (samples.Count < 2) return 0f;
			Sample a = samples[samples.Count - 2], b = samples[samples.Count - 1];
			return (b.Tip - a.Tip).magnitude / Mathf.Max(b.Time - a.Time, 1e-4f);
		}

		// Two thin diamonds (long across, long up) around a bright centre.
		private void AddStar(Vector3 centre, float size, Color color)
		{
			Color clear = color; clear.a = 0f;
			float thin = size * 0.16f;
			foreach (var arms in new[] { new Vector2(size, thin), new Vector2(thin, size) })
			{
				int c = _vertices.Count;
				_vertices.Add(centre); _colors.Add(color);
				_vertices.Add(centre + new Vector3(arms.x, 0f, 0f)); _colors.Add(clear);
				_vertices.Add(centre + new Vector3(0f, arms.y, 0f)); _colors.Add(clear);
				_vertices.Add(centre + new Vector3(-arms.x, 0f, 0f)); _colors.Add(clear);
				_vertices.Add(centre + new Vector3(0f, -arms.y, 0f)); _colors.Add(clear);
				for (int i = 0; i < 4; i++)
				{
					_triangles.Add(c); _triangles.Add(c + 1 + i); _triangles.Add(c + 1 + (i + 1) % 4);
				}
			}
		}

		private void OnDestroy()
		{
			if (_alphaMesh != null) Destroy(_alphaMesh);
			if (_additiveMesh != null) Destroy(_additiveMesh);
			if (_glintMesh != null) Destroy(_glintMesh);
		}
	}
}

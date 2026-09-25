using System.Collections.Generic;
using Eclipse.Modding;
using Eclipse.Rendering.Interpolation;
using UnityEngine;

namespace Eclipse.Rendering
{
	// Swing trail (sf2.visuals.weapon_trails): a short ribbon along each blade,
	// sampled every rendered frame from the interpolated pose. Segments fade with
	// age and with how fast the tip moved, so idle weapons leave nothing.
	//
	// Weapon art is built from macro nodes: weighted combinations of the
	// skeleton's Weapon-Node1..4_N control points, extending beyond them. So the
	// blade is taken from the weapon's own geometry: an edge named "*-Blade" when
	// the model has one (34 of the 171 shipped weapons), otherwise, per hand
	// (_1 main, _2 off hand), the two farthest macro nodes driven by that hand.
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

		private static Material _material;

		private Model _model;
		private ModelPresentation _presentation;
		private readonly List<Blade> _blades = new List<Blade>();
		private int _nodeCount = -1;
		private float _lifetime = 0.11f;
		private Mesh _mesh;
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
			var child = new GameObject("Weapon trail");
			child.transform.SetParent(transform, false);
			// Just behind the fighter's own body renderers.
			child.transform.localPosition = new Vector3(0f, 0f, 0.1f);
			_mesh = new Mesh { name = "Weapon trail" };
			_mesh.MarkDynamic();
			child.AddComponent<MeshFilter>().sharedMesh = _mesh;
			var renderer = child.AddComponent<MeshRenderer>();
			if (_material == null) _material = new Material(Shader.Find("Sprites/Default")) { hideFlags = HideFlags.HideAndDontSave };
			renderer.sharedMaterial = _material;
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.receiveShadows = false;
		}

		private void LateUpdate()
		{
			if (_mesh == null) return;
			ModVisualDefinition settings = ModVisuals.Active(ModVisualEffect.WeaponTrails);
			if (settings == null || _model == null || !FightInterpolation.IsFightActive)
			{
				if (_blades.Count != 0) { ClearSamples(); _mesh.Clear(); }
				return;
			}
			ResolveBlades();
			_lifetime = settings.Number("lifetime");
			float minSpeed = settings.Number("min_speed"), fullSpeed = settings.Number("full_speed");
			float now = Time.unscaledTime;
			float alpha = _presentation != null ? _presentation.Alpha : FightInterpolation.FightAlpha;
			foreach (Blade blade in _blades)
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
					strength = Mathf.Clamp01((speed - minSpeed) / (fullSpeed - minSpeed));
				}
				if (samples.Count == 0 || now > samples[samples.Count - 1].Time)
					samples.Add(new Sample { Grip = grip, Tip = tip, Time = now, Strength = strength });
				while (samples.Count > 0 && (now - samples[0].Time > _lifetime || samples.Count > MaxSamples))
					samples.RemoveAt(0);
			}
			Rebuild(now, settings);
		}

		private void ClearSamples()
		{
			foreach (Blade blade in _blades) blade.Samples.Clear();
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

		private void ResolveBlades()
		{
			ModelObject body = _model.CLDMEJKGLBA();
			Dictionary<string, ModelNode> nodes = body?.HKCFFKKFFFE();
			int count = nodes != null ? nodes.Count : 0;
			if (count == _nodeCount) return;
			_nodeCount = count;
			_blades.Clear();
			if (nodes == null) return;
			Vector3 pivot = body.HOFFDCFEBGA() != null ? ToVector(body.HOFFDCFEBGA().GetStart()) : Vector3.zero;

			// 1. Explicit blade edges.
			List<ModelEdge> edges = body.BKAPPJMGPKP();
			if (edges != null)
				foreach (ModelEdge edge in edges)
				{
					string name = edge.get_Name();
					if (name == null || name.IndexOf("-Blade", System.StringComparison.Ordinal) < 0) continue;
					AddBlade(edge.GetStartNode(), edge.GetEndNode(), pivot);
				}
			if (_blades.Count != 0) return;

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
				if (a != null) AddBlade(a, b, pivot);
			}
		}

		// "_1" or "_2" when every weighted control point is a skeleton weapon node of one hand.
		private static string WeaponHand(ModelMacroNode macro)
		{
			string hand = null;
			foreach (var child in macro.LMPPCKACMNB)
			{
				string name = child.First;
				if (name == null || !name.StartsWith("Weapon-Node", System.StringComparison.Ordinal)) return null;
				int underscore = name.LastIndexOf('_');
				string suffix = underscore >= 0 ? name.Substring(underscore) : string.Empty;
				if (hand != null && hand != suffix) return null;
				hand = suffix;
			}
			return hand;
		}

		// The tip is the end farther from the body, so the ribbon is brightest there.
		private void AddBlade(ModelNode a, ModelNode b, Vector3 pivot)
		{
			if (a == null || b == null || a == b) return;
			bool aIsTip = Vector3.Distance(ToVector(a.GetStart()), pivot) > Vector3.Distance(ToVector(b.GetStart()), pivot);
			_blades.Add(new Blade { Grip = aIsTip ? b : a, Tip = aIsTip ? a : b });
		}

		private void Rebuild(float now, ModVisualDefinition settings)
		{
			_mesh.Clear();
			_vertices.Clear(); _colors.Clear(); _triangles.Clear();
			float maxAlpha = settings.Number("alpha");
			// An explicit colour wins; otherwise follow the fighter's own (perk) colour.
			Color? explicitColor = ModVisuals.ColorOf(settings);
			Color tint = explicitColor ?? (_presentation != null && _presentation.Tint.HasValue ? _presentation.Tint.Value : Color.black);
			float colorAlpha = explicitColor.HasValue ? explicitColor.Value.a : 1f;
			foreach (Blade blade in _blades)
			{
				List<Sample> samples = blade.Samples;
				if (samples.Count < 2) continue;
				int first = _vertices.Count;
				for (int i = 0; i < samples.Count; i++)
				{
					Sample s = samples[i];
					float age = Mathf.Clamp01((now - s.Time) / _lifetime);
					float a = maxAlpha * colorAlpha * (1f - age) * s.Strength;
					Color grip = tint; grip.a = a * 0.35f;
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
			if (_triangles.Count == 0) return;
			_mesh.SetVertices(_vertices);
			_mesh.SetColors(_colors);
			_mesh.SetTriangles(_triangles, 0);
			_mesh.RecalculateBounds();
		}

		private void OnDestroy()
		{
			if (_mesh != null) Destroy(_mesh);
		}
	}
}

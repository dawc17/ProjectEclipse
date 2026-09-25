using System.Collections.Generic;
using Eclipse.Modding;
using Eclipse.Rendering.Interpolation;
using UnityEngine;

namespace Eclipse.Rendering
{
	// Swing trail (sf2.visuals.weapon_trails): a short ribbon between the weapon's
	// grip and tip nodes, sampled every rendered frame from the interpolated pose.
	// Segments fade with age and with how fast the tip moved, so idle weapons leave nothing.
	public sealed class WeaponTrail : MonoBehaviour
	{
		private const int MaxSamples = 48;
		private const float MinBladeLength = 25f;
		private float Lifetime = 0.11f;

		private struct Sample { public Vector3 Grip, Tip; public float Time, Strength; }

		private static Material _material;

		private Model _model;
		private ModelPresentation _presentation;
		private readonly List<Sample> _samples = new List<Sample>();
		private readonly List<KeyValuePair<ModelNode, ModelNode>> _blades = new List<KeyValuePair<ModelNode, ModelNode>>();
		private int _nodeCount = -1;
		private Mesh _mesh;
		private MeshRenderer _renderer;
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
			_renderer = child.AddComponent<MeshRenderer>();
			if (_material == null) _material = new Material(Shader.Find("Sprites/Default")) { hideFlags = HideFlags.HideAndDontSave };
			_renderer.sharedMaterial = _material;
			_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			_renderer.receiveShadows = false;
		}

		private void LateUpdate()
		{
			if (_mesh == null) return;
			ModVisualDefinition settings = ModVisuals.Active(ModVisualEffect.WeaponTrails);
			if (settings == null || _model == null || !FightInterpolation.IsFightActive)
			{
				if (_samples.Count != 0) { _samples.Clear(); _mesh.Clear(); }
				return;
			}
			ResolveBlades();
			Lifetime = settings.Number("lifetime");
			float minSpeed = settings.Number("min_speed"), fullSpeed = settings.Number("full_speed");
			float now = Time.unscaledTime;
			float alpha = _presentation != null ? _presentation.Alpha : FightInterpolation.FightAlpha;
			// Trail the main-hand blade (the first group found).
			int blade = _blades.Count > 0 ? 0 : -1;
			if (blade >= 0)
			{
				Vector3 grip = Sample3(_blades[blade].Key, alpha);
				Vector3 tip = Sample3(_blades[blade].Value, alpha);
				float strength = 0f;
				if (_samples.Count > 0)
				{
					Sample last = _samples[_samples.Count - 1];
					float dt = Mathf.Max(now - last.Time, 1e-4f);
					float speed = (tip - last.Tip).magnitude / dt;
					strength = Mathf.Clamp01((speed - minSpeed) / (fullSpeed - minSpeed));
				}
				if (_samples.Count == 0 || now > _samples[_samples.Count - 1].Time)
					_samples.Add(new Sample { Grip = grip, Tip = tip, Time = now, Strength = strength });
			}
			while (_samples.Count > 0 && (now - _samples[0].Time > Lifetime || _samples.Count > MaxSamples))
				_samples.RemoveAt(0);
			Rebuild(now, settings);
		}

		private static Vector3 Sample3(ModelNode node, float alpha)
		{
			float x, y, z;
			FightInterpolation.SamplePosition(node, alpha, out x, out y, out z);
			return new Vector3(x, y, 0f);
		}

		// Weapon models attach nodes named Weapon-Node*; the farthest pair within
		// a hand's group (suffix _1 main hand, _2 off hand) spans grip to tip.
		private void ResolveBlades()
		{
			Dictionary<string, ModelNode> nodes = _model.CLDMEJKGLBA()?.HKCFFKKFFFE();
			int count = nodes != null ? nodes.Count : 0;
			if (count == _nodeCount) return;
			_nodeCount = count;
			_blades.Clear();
			_samples.Clear();
			if (nodes == null) return;
			var groups = new Dictionary<string, List<ModelNode>>();
			foreach (var pair in nodes)
			{
				if (!pair.Key.StartsWith("Weapon-Node", System.StringComparison.Ordinal)) continue;
				int underscore = pair.Key.LastIndexOf('_');
				string hand = underscore >= 0 ? pair.Key.Substring(underscore) : string.Empty;
				if (!groups.TryGetValue(hand, out var list)) groups[hand] = list = new List<ModelNode>();
				list.Add(pair.Value);
			}
			foreach (var list in groups.Values)
			{
				ModelNode a = null, b = null; float best = MinBladeLength;
				for (int i = 0; i < list.Count; i++)
					for (int j = i + 1; j < list.Count; j++)
					{
						float d = Vector3.Distance(ToVector(list[i].GetStart()), ToVector(list[j].GetStart()));
						if (d > best) { best = d; a = list[i]; b = list[j]; }
					}
				if (a != null) _blades.Add(new KeyValuePair<ModelNode, ModelNode>(a, b));
			}
		}

		private static Vector3 ToVector(Vector3f value)
		{
			return new Vector3(value.GetX(), value.GetY(), value.GetZ());
		}

		private void Rebuild(float now, ModVisualDefinition settings)
		{
			float maxAlpha = settings.Number("alpha");
			_mesh.Clear();
			if (_samples.Count < 2) return;
			_vertices.Clear(); _colors.Clear(); _triangles.Clear();
			// An explicit colour wins; otherwise follow the fighter's own (perk) colour.
			Color? explicitColor = ModVisuals.ColorOf(settings);
			Color tint = explicitColor ?? (_presentation != null && _presentation.Tint.HasValue ? _presentation.Tint.Value : Color.black);
			float colorAlpha = explicitColor.HasValue ? explicitColor.Value.a : 1f;
			for (int i = 0; i < _samples.Count; i++)
			{
				Sample s = _samples[i];
				float age = Mathf.Clamp01((now - s.Time) / Lifetime);
				float a = maxAlpha * colorAlpha * (1f - age) * s.Strength;
				Color grip = tint; grip.a = a * 0.35f;
				Color tip = tint; tip.a = a;
				_vertices.Add(s.Grip); _colors.Add(grip);
				_vertices.Add(s.Tip); _colors.Add(tip);
				if (i > 0)
				{
					int v = i * 2;
					_triangles.Add(v - 2); _triangles.Add(v - 1); _triangles.Add(v);
					_triangles.Add(v - 1); _triangles.Add(v + 1); _triangles.Add(v);
				}
			}
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

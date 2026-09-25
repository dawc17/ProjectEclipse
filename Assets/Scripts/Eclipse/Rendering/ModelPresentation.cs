using Eclipse.Rendering.Interpolation;
using UnityEngine;

namespace Eclipse.Rendering
{
	// Per-fighter presentation state on the model's root object. Body renderers
	// (mesh, capsules, edges) read it so a perk tint or slow-down applies to one
	// fighter only, instead of the static materials all fighters share.
	public sealed class ModelPresentation : MonoBehaviour
	{
		private int _slowFactor = 1;
		private int _slowFrame;

		private Color? _baseColor;
		private Color? _perkTint;

		// The colour body renderers should show: an active perk tint, else the
		// fighter's base colour. Null keeps each renderer's own default.
		public Color? Tint => _perkTint ?? _baseColor;

		// Incremented on every tint change so renderers can apply it lazily.
		public int TintVersion { get; private set; }

		public static ModelPresentation Attach(GameObject root)
		{
			var presentation = root.GetComponent<ModelPresentation>();
			return presentation != null ? presentation : root.AddComponent<ModelPresentation>();
		}

		public void SetTint(Color? tint)
		{
			if (_perkTint == tint) return;
			_perkTint = tint;
			TintVersion++;
		}

		public void SetBaseColor(Color color)
		{
			if (_baseColor == color) return;
			_baseColor = color;
			TintVersion++;
		}

		// A slowed model advances its simulation once every `factor` fixed ticks;
		// `frame` counts the ticks since that advance. Spread one advance over the
		// whole span so the pose moves smoothly instead of lerping one tick and
		// then holding.
		public void SetSlow(int factor, int frame)
		{
			_slowFactor = Mathf.Max(1, factor);
			_slowFrame = frame;
		}

		public float Alpha
		{
			get
			{
				float alpha = FightInterpolation.FightAlpha;
				if (_slowFactor <= 1 || !FightInterpolation.Enabled) return alpha;
				return Mathf.Clamp01((_slowFrame + alpha) / _slowFactor);
			}
		}

		public static float AlphaFor(ModelPresentation presentation)
		{
			return presentation != null ? presentation.Alpha : FightInterpolation.FightAlpha;
		}
	}
}

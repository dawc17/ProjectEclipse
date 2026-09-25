using UnityEngine;

// Optional presentation effects under Options > Experimental. All are off by
// default so the recovered look stays the baseline; none of them feed back into
// the fight simulation.
public static class ExperimentalVisuals
{
	public enum Feature { WeaponTrails, DepthHaze, RimLight, Bloom, AmbientParticles, ImpactEffects }

	private static readonly string[] Keys =
	{
		"Eclipse.Experimental.WeaponTrails", "Eclipse.Experimental.DepthHaze", "Eclipse.Experimental.RimLight",
		"Eclipse.Experimental.Bloom", "Eclipse.Experimental.AmbientParticles", "Eclipse.Experimental.ImpactEffects"
	};

	private static readonly bool[] Values = new bool[Keys.Length];

	private static bool _loaded;

	private static float _impact;

	private static float _impactStartTime = -1f;

	public const float ImpactDuration = 0.3f;

	public static bool Get(Feature feature)
	{
		Load();
		return Values[(int)feature];
	}

	public static void Set(Feature feature, bool enabled)
	{
		Load();
		Values[(int)feature] = enabled;
		PlayerPrefs.SetInt(Keys[(int)feature], enabled ? 1 : 0);
	}

	public static void Toggle(Feature feature)
	{
		Set(feature, !Get(feature));
	}

	public static bool WeaponTrails => Get(Feature.WeaponTrails);
	public static bool DepthHaze => Get(Feature.DepthHaze);
	public static bool RimLight => Get(Feature.RimLight);
	public static bool Bloom => Get(Feature.Bloom);
	public static bool AmbientParticles => Get(Feature.AmbientParticles);
	public static bool ImpactEffects => Get(Feature.ImpactEffects);

	// Starts a short screen impact (radial blur and colour split). Strength is
	// already scaled by the caller, e.g. by the accessibility shake slider.
	public static void TriggerImpact(float strength)
	{
		if (!ImpactEffects || strength <= 0f) return;
		float remaining = CurrentImpact;
		_impact = Mathf.Max(remaining, Mathf.Clamp01(strength));
		_impactStartTime = Time.unscaledTime;
	}

	// Impact strength now, fading out over ImpactDuration.
	public static float CurrentImpact
	{
		get
		{
			if (_impactStartTime < 0f) return 0f;
			float t = (Time.unscaledTime - _impactStartTime) / ImpactDuration;
			if (t >= 1f) { _impactStartTime = -1f; return 0f; }
			float fade = 1f - t;
			return _impact * fade * fade;
		}
	}

	private static void Load()
	{
		if (_loaded) return;
		for (int i = 0; i < Keys.Length; i++) Values[i] = PlayerPrefs.GetInt(Keys[i], 0) != 0;
		_loaded = true;
	}
}

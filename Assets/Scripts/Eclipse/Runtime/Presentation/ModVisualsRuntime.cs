using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Modding
{
	// Installation-wide values of mod settings (not part of any profile).
	public static class ModSettingsStore
	{
		private static string Key(ModSettingToggle toggle) => "Eclipse.ModSetting." + toggle.Name;

		// Makes sf2.settings.get and the visuals read this store.
		public static void Install() { ModSettingValues.Read = Get; }

		public static bool Get(ModSettingToggle toggle)
		{
			if (toggle == null) return false;
			return PlayerPrefs.GetInt(Key(toggle), toggle.Default ? 1 : 0) != 0;
		}

		public static void Set(ModSettingToggle toggle, bool value)
		{
			if (toggle == null) return;
			PlayerPrefs.SetInt(Key(toggle), value ? 1 : 0);
		}
	}

	// What the renderers query each frame. Bound to the running content catalog.
	public static class ModVisuals
	{
		private static ModContentCatalog _catalog;
		private static float _impact;
		private static float _impactStart = -1f;
		private static float _impactDuration = 0.3f;

		// When each fight event last fired (unscaled seconds), for triggered effects.
		private static readonly float[] _triggerTimes = { -1f, -1f, -1f, -1f, -1f };

		public static void Bind(ModContentCatalog catalog)
		{
			_catalog = catalog;
			_impactStart = -1f;
			ResetTriggers();
			ModSettingsStore.Install();
		}

		// The running fight's location name; screen effects use it for match/exclude.
		public static string CurrentLocation { get; set; }

		public static void ResetTriggers()
		{
			for (int i = 0; i < _triggerTimes.Length; i++) _triggerTimes[i] = -1f;
		}

		// Slow motion from triggered screen grades (time_scale < 1): the game
		// speed follows the grade's strength, so it returns as the grade fades.
		// The value is only written while this code owns it: it starts from normal
		// speed, and if anything else changes the speed (a pause dialog, the debug
		// sprint) it lets go without restoring.
		private static float _writtenTimeScale = -1f;

		public static float CurrentTimeScale()
		{
			float scale = 1f;
			foreach (ModFxDefinition definition in ActiveFx(ModFxKind.Screen))
			{
				if (definition.Trigger == ModFxTrigger.Always || definition.Number("time_scale") >= 1f) continue;
				if (!definition.MatchesLocation(CurrentLocation)) continue;
				float w = TriggerWeight(definition);
				if (w > 0f) scale = Mathf.Min(scale, Mathf.Lerp(1f, definition.Number("time_scale"), w));
			}
			return scale;
		}

		public static void UpdateTimeScale()
		{
			float target = CurrentTimeScale();
			if (_writtenTimeScale < 0f)
			{
				if (target >= 0.999f || Time.timeScale != 1f) return;
				Time.timeScale = _writtenTimeScale = target;
				return;
			}
			if (Time.timeScale != _writtenTimeScale) { _writtenTimeScale = -1f; return; }
			if (target >= 0.999f) { Time.timeScale = 1f; _writtenTimeScale = -1f; return; }
			Time.timeScale = _writtenTimeScale = target;
		}

		public static void ReleaseTimeScale()
		{
			if (_writtenTimeScale >= 0f && Time.timeScale == _writtenTimeScale) Time.timeScale = 1f;
			_writtenTimeScale = -1f;
		}

		// Records a resolved hit for triggered screen effects. A ko also counts as
		// a hit (and a critical ko as a critical).
		public static void NotifyHit(bool critical, bool blocked, bool ko)
		{
			float now = Time.unscaledTime;
			if (blocked) _triggerTimes[(int)ModFxTrigger.Block] = now;
			else
			{
				_triggerTimes[(int)ModFxTrigger.Hit] = now;
				if (critical) _triggerTimes[(int)ModFxTrigger.Critical] = now;
			}
			if (ko) _triggerTimes[(int)ModFxTrigger.Ko] = now;
		}

		// 0..1 strength of a triggered effect now: full for `hold`, then an
		// ease-out fade over `duration`. Always-on effects are 1.
		public static float TriggerWeight(ModFxDefinition definition)
		{
			if (definition.Trigger == ModFxTrigger.Always) return 1f;
			float start = _triggerTimes[(int)definition.Trigger];
			if (start < 0f) return 0f;
			float t = Time.unscaledTime - start - definition.Number("hold");
			if (t <= 0f) return 1f;
			float fade = 1f - t / definition.Number("duration");
			// Smooth at both ends: no sudden drop after the pop-in, no snap at the end.
			return fade <= 0f ? 0f : fade * fade * (3f - 2f * fade);
		}

		// A definition's colour as a Unity colour, when it has one.
		public static Color? ColorOf(ModVisualDefinition definition)
		{
			ModUiColor c = definition?.Color;
			return c == null ? (Color?)null : new Color32(c.R, c.G, c.B, c.A);
		}

		public static IReadOnlyList<ModSettingToggle> Settings =>
			_catalog != null ? _catalog.SettingToggles : (IReadOnlyList<ModSettingToggle>)Array.Empty<ModSettingToggle>();

		// The effect's definition when a mod configured it and its setting (if any) is on.
		public static ModVisualDefinition Active(ModVisualEffect effect)
		{
			if (_catalog == null || !_catalog.Visuals.TryGetValue(effect, out var definition)) return null;
			if (definition.Setting == null) return definition;
			foreach (ModSettingToggle toggle in _catalog.SettingToggles)
				if (toggle.Name == definition.Setting) return ModSettingsStore.Get(toggle) ? definition : null;
			return null;
		}

		private static bool SettingOn(string setting)
		{
			if (setting == null) return true;
			foreach (ModSettingToggle toggle in _catalog.SettingToggles)
				if (toggle.Name == setting) return ModSettingsStore.Get(toggle);
			return false;
		}

		// sf2.fx effects of one kind whose switch (if any) is on, in load order.
		public static List<ModFxDefinition> ActiveFx(ModFxKind kind)
		{
			var result = new List<ModFxDefinition>();
			if (_catalog == null) return result;
			foreach (ModFxDefinition definition in _catalog.Effects)
				if (definition.Kind == kind && SettingOn(definition.Setting)) result.Add(definition);
			return result;
		}

		// Whether any effect of one kind is active, without building a list.
		public static bool HasActiveFx(ModFxKind kind)
		{
			if (_catalog == null) return false;
			foreach (ModFxDefinition definition in _catalog.Effects)
				if (definition.Kind == kind && SettingOn(definition.Setting)) return true;
			return false;
		}

		// A stable key for the active set of one kind, so renderers rebuild only on change.
		public static string ActiveFxKey(ModFxKind kind)
		{
			if (_catalog == null) return string.Empty;
			var key = new System.Text.StringBuilder();
			foreach (ModFxDefinition definition in _catalog.Effects)
				if (definition.Kind == kind && SettingOn(definition.Setting)) key.Append(definition.Name).Append('|');
			return key.ToString();
		}

		public static Color ToColor(ModUiColor color, Color fallback)
		{
			return color == null ? fallback : (Color)new Color32(color.R, color.G, color.B, color.A);
		}

		// All active sf2.fx.screen grades for the current location combined:
		// saturation and contrast multiply, brightness adds, tints layer in load
		// order, vignette, grain and accent take the strongest, halation adds.
		// A triggered grade is scaled by its current strength (0 = no change).
		public struct ScreenGrade
		{
			public bool Active;
			public float Saturation, Contrast, Brightness, TintStrength, Vignette;
			public Vector2 VignetteCenter;
			public float Grain, Halation, HalationThreshold, AccentStrength, AccentWidth;
			public Color Tint, HalationColor, Accent;
		}

		private static readonly Color DefaultHalation = new Color(1f, 0.62f, 0.42f, 1f);

		public static ScreenGrade CurrentGrade()
		{
			var grade = new ScreenGrade { Saturation = 1f, Contrast = 1f, Tint = Color.white, HalationColor = DefaultHalation,
				HalationThreshold = 0.75f, Accent = Color.red, AccentWidth = 0.08f };
			float now = Time.unscaledTime;
			foreach (ModFxDefinition definition in ActiveFx(ModFxKind.Screen))
			{
				if (!definition.MatchesLocation(CurrentLocation)) continue;
				float w = TriggerWeight(definition);
				if (w <= 0f) continue;
				grade.Active = true;
				grade.Saturation *= Mathf.Lerp(1f, definition.Number("saturation"), w);
				grade.Contrast *= Mathf.Lerp(1f, definition.Number("contrast"), w);
				grade.Brightness += definition.Number("brightness") * w;
				float flicker = definition.Number("flicker");
				if (flicker > 0f)
				{
					// Two out-of-step waves read as an irregular flame rather than a pulse.
					float speed = definition.Number("flicker_speed");
					float n = Mathf.PerlinNoise(now * speed * 0.5f, definition.Name.Length * 7.31f) - 0.5f;
					n += (Mathf.PerlinNoise(now * speed * 1.7f, 3.7f) - 0.5f) * 0.5f;
					grade.Brightness += n * flicker * 0.25f * w;
				}
				float strength = definition.Number("tint_strength") * w;
				if (strength > 0f && definition.Color != null)
				{
					Color tint = ToColor(definition.Color, Color.white);
					grade.Tint = grade.TintStrength <= 0f ? tint : Color.Lerp(grade.Tint, tint, strength);
					grade.TintStrength = Mathf.Max(grade.TintStrength, strength);
				}
				float vignette = definition.Number("vignette") * w;
				if (vignette > grade.Vignette)
				{
					grade.Vignette = vignette;
					grade.VignetteCenter = new Vector2(definition.Number("vignette_x"), definition.Number("vignette_y"));
				}
				grade.Grain = Mathf.Max(grade.Grain, definition.Number("grain") * w);
				float halation = definition.Number("halation") * w;
				if (halation > 0f)
				{
					if (grade.Halation <= 0f || definition.HalationColor != null)
						grade.HalationColor = ToColor(definition.HalationColor, DefaultHalation);
					grade.HalationThreshold = grade.Halation <= 0f ? definition.Number("halation_threshold")
						: Mathf.Min(grade.HalationThreshold, definition.Number("halation_threshold"));
					grade.Halation += halation;
				}
				float accent = definition.Number("accent_strength") * w;
				if (accent > grade.AccentStrength && definition.AccentColor != null)
				{
					grade.AccentStrength = accent;
					grade.Accent = ToColor(definition.AccentColor, Color.red);
					grade.AccentWidth = definition.Number("accent_width");
				}
			}
			grade.Brightness = Mathf.Clamp(grade.Brightness, -1f, 1f);
			grade.Halation = Mathf.Min(grade.Halation, 2f);
			return grade;
		}

		// Spreads background layer factors: factor^(1+strength). Never larger than
		// the authored factor, so a layer never travels beyond its art.
		public static float BackgroundLayerFactor(float factor)
		{
			if (factor <= 0f || factor >= 1f) return factor;
			ModVisualDefinition depth = Active(ModVisualEffect.BackgroundDepth);
			return depth == null ? factor : Mathf.Pow(factor, 1f + depth.Number("strength"));
		}

		// Starts a short screen impact for a native hit effect type. `scale` lets
		// the caller apply the accessibility shake slider to critical hits.
		public static void TriggerImpact(string hitType, float scale)
		{
			ModVisualDefinition impact = Active(ModVisualEffect.Impact);
			if (impact == null) return;
			float strength = hitType == "CriticalHit" ? impact.Number("critical") :
				hitType == "HeadHit" ? impact.Number("head") : hitType == "Shock" ? impact.Number("shock") : 0f;
			strength *= scale;
			if (strength <= 0f) return;
			_impact = Mathf.Max(CurrentImpact, Mathf.Clamp01(strength));
			_impactDuration = impact.Number("duration");
			_impactStart = Time.unscaledTime;
		}

		public static float CurrentImpact
		{
			get
			{
				if (_impactStart < 0f) return 0f;
				float t = (Time.unscaledTime - _impactStart) / _impactDuration;
				if (t >= 1f) { _impactStart = -1f; return 0f; }
				float fade = 1f - t;
				return _impact * fade * fade;
			}
		}
	}
}

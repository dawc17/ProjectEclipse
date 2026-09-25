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

		public static void Bind(ModContentCatalog catalog)
		{
			_catalog = catalog;
			_impactStart = -1f;
			ModSettingsStore.Install();
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

		// All active sf2.fx.screen grades combined: saturation and contrast
		// multiply, brightness adds, tints layer in load order, vignette takes the strongest.
		public struct ScreenGrade
		{
			public bool Active;
			public float Saturation, Contrast, Brightness, TintStrength, Vignette;
			public Color Tint;
		}

		public static ScreenGrade CurrentGrade()
		{
			var grade = new ScreenGrade { Saturation = 1f, Contrast = 1f, Tint = Color.white };
			foreach (ModFxDefinition definition in ActiveFx(ModFxKind.Screen))
			{
				grade.Active = true;
				grade.Saturation *= definition.Number("saturation");
				grade.Contrast *= definition.Number("contrast");
				grade.Brightness += definition.Number("brightness");
				float strength = definition.Number("tint_strength");
				if (strength > 0f && definition.Color != null)
				{
					Color tint = ToColor(definition.Color, Color.white);
					grade.Tint = grade.TintStrength <= 0f ? tint : Color.Lerp(grade.Tint, tint, strength);
					grade.TintStrength = Mathf.Max(grade.TintStrength, strength);
				}
				grade.Vignette = Mathf.Max(grade.Vignette, definition.Number("vignette"));
			}
			grade.Brightness = Mathf.Clamp(grade.Brightness, -1f, 1f);
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

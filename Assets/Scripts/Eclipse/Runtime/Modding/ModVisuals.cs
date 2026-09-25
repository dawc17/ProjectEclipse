using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
	// Presentation-only fight visuals that a mod can switch on through
	// sf2.visuals. The engine owns the renderers; a mod supplies typed settings.
	// Nothing here is saved to the profile or included in the content fingerprint.
	public enum ModVisualEffect { BackgroundDepth, WeaponTrails, DepthHaze, RimLight, Bloom, AmbientParticles, Impact }

	public enum ModParticleStyle { None, Dust, Snow, Embers, Petals }

	public sealed class ModParticleRule
	{
		private readonly string[] _match;
		public IReadOnlyList<string> Match => _match;
		public ModParticleStyle Style { get; }
		public ModParticleRule(string[] match, ModParticleStyle style) { _match = (string[])(match ?? Array.Empty<string>()).Clone(); Style = style; }
	}

	public sealed class ModVisualDefinition
	{
		private readonly Dictionary<string, float> _numbers;
		private readonly ModParticleRule[] _rules;
		public ModVisualEffect Effect { get; }
		public ModId Owner { get; }
		public string Setting { get; }
		public ModUiColor Color { get; }
		public ModParticleStyle DefaultStyle { get; }
		public IReadOnlyList<ModParticleRule> Rules => _rules;

		internal ModVisualDefinition(ModVisualEffect effect, ModId owner, string setting, Dictionary<string, float> numbers,
			ModUiColor color, ModParticleStyle defaultStyle, ModParticleRule[] rules)
		{
			Effect = effect; Owner = owner; Setting = setting; _numbers = numbers; Color = color;
			DefaultStyle = defaultStyle; _rules = rules ?? Array.Empty<ModParticleRule>();
		}

		public float Number(string name) => _numbers[name];
	}

	public sealed class ModSettingToggle
	{
		public string Name { get; }
		public ModId Owner { get; }
		public string Label { get; }
		public string Description { get; }
		public bool Default { get; }
		internal ModSettingToggle(string name, ModId owner, string label, string description, bool value)
		{ Name = name; Owner = owner; Label = label; Description = description; Default = value; }
	}

	public static class ModVisualParameters
	{
		// Name, default, minimum, maximum for each effect's numeric settings.
		private static readonly Dictionary<ModVisualEffect, (string Name, float Default, float Min, float Max)[]> Table =
			new Dictionary<ModVisualEffect, (string, float, float, float)[]>
			{
				{ ModVisualEffect.BackgroundDepth, new[] { ("strength", 0.6f, 0f, 2f) } },
				{ ModVisualEffect.WeaponTrails, new[] { ("lifetime", 0.11f, 0.02f, 0.5f), ("min_speed", 900f, 0f, 20000f),
					("full_speed", 2600f, 1f, 40000f), ("alpha", 0.55f, 0f, 1f) } },
				{ ModVisualEffect.DepthHaze, new[] { ("strength", 0.4f, 0f, 1f) } },
				{ ModVisualEffect.RimLight, new[] { ("offset", 2.5f, 0f, 12f), ("alpha", 0.85f, 0f, 1f), ("lighten", 0.35f, 0f, 1f) } },
				{ ModVisualEffect.Bloom, new[] { ("threshold", 0.82f, 0f, 2f), ("knee", 0.12f, 0f, 1f), ("intensity", 0.7f, 0f, 4f) } },
				{ ModVisualEffect.AmbientParticles, new[] { ("density", 1f, 0f, 4f) } },
				{ ModVisualEffect.Impact, new[] { ("critical", 1f, 0f, 1f), ("head", 0.6f, 0f, 1f), ("shock", 0.4f, 0f, 1f),
					("duration", 0.3f, 0.05f, 2f) } },
			};

		public static IReadOnlyList<(string Name, float Default, float Min, float Max)> For(ModVisualEffect effect) => Table[effect];

		public static ModParticleStyle? ParseStyle(string value)
		{
			switch (value)
			{
				case "none": return ModParticleStyle.None;
				case "dust": return ModParticleStyle.Dust;
				case "snow": return ModParticleStyle.Snow;
				case "embers": return ModParticleStyle.Embers;
				case "petals": return ModParticleStyle.Petals;
				default: return null;
			}
		}
	}

	public sealed partial class ModContentCatalog
	{
		private readonly Dictionary<ModVisualEffect, ModVisualDefinition> _visuals = new Dictionary<ModVisualEffect, ModVisualDefinition>();
		private readonly List<ModSettingToggle> _settings = new List<ModSettingToggle>();
		public IReadOnlyDictionary<ModVisualEffect, ModVisualDefinition> Visuals => _visuals;
		public IReadOnlyList<ModSettingToggle> SettingToggles => _settings.AsReadOnly();

		internal void CommitVisuals(IEnumerable<ModVisualDefinition> visuals, IEnumerable<ModSettingToggle> settings)
		{
			foreach (ModVisualDefinition visual in visuals) _visuals.Add(visual.Effect, visual);
			_settings.AddRange(settings);
		}
	}

	public sealed partial class ModRegistrationTransaction
	{
		public const int MaxSettingsPerMod = 16;
		public const int MaxParticleRules = 32;
		private readonly Dictionary<ModVisualEffect, ModVisualDefinition> _pendingVisuals = new Dictionary<ModVisualEffect, ModVisualDefinition>();
		private readonly List<ModSettingToggle> _pendingSettings = new List<ModSettingToggle>();
		private int VisualRegistrationCount => _pendingVisuals.Count + _pendingSettings.Count;

		public ModSettingToggle RegisterSettingToggle(string localId, string label, string description, bool value)
		{
			ThrowIfCompleted();
			ValidateLocalName(localId, "Setting id");
			if (string.IsNullOrWhiteSpace(label) || label.Length > 48)
				throw new ModContentException("Setting label must be 1..48 characters.");
			if (description != null && description.Length > 160)
				throw new ModContentException("Setting description must be at most 160 characters.");
			string name = Mod.Id.Value + "." + localId;
			foreach (ModSettingToggle existing in _pendingSettings)
				if (existing.Name == name) throw new ModContentException("Duplicate setting: '" + name + "'.");
			if (_pendingSettings.Count >= MaxSettingsPerMod)
				throw new ModContentException("A mod may register at most " + MaxSettingsPerMod + " settings.");
			EnsureCapacityForNewRegistration();
			var toggle = new ModSettingToggle(name, Mod.Id, label.Trim(), description?.Trim(), value);
			_pendingSettings.Add(toggle);
			return toggle;
		}

		// `numbers` holds only the fields the mod supplied; the rest use defaults.
		public ModVisualDefinition RegisterVisual(ModVisualEffect effect, IDictionary<string, float> numbers, string setting,
			ModUiColor color, ModParticleStyle defaultStyle, IList<ModParticleRule> rules)
		{
			ThrowIfCompleted();
			if (!Enum.IsDefined(typeof(ModVisualEffect), effect)) throw new ModContentException("Unsupported visual effect.");
			if (_pendingVisuals.ContainsKey(effect)) throw new ModContentException("Duplicate visual effect: " + effect + ".");
			var resolved = new Dictionary<string, float>(StringComparer.Ordinal);
			foreach (var parameter in ModVisualParameters.For(effect)) resolved[parameter.Name] = parameter.Default;
			if (numbers != null)
				foreach (var pair in numbers)
				{
					(string Name, float Default, float Min, float Max)? spec = null;
					foreach (var parameter in ModVisualParameters.For(effect)) if (parameter.Name == pair.Key) spec = parameter;
					if (spec == null) throw new ModContentException("Unknown " + effect + " field '" + pair.Key + "'.");
					if (float.IsNaN(pair.Value) || pair.Value < spec.Value.Min || pair.Value > spec.Value.Max)
						throw new ModContentException(effect + "." + pair.Key + " must be between " + spec.Value.Min + " and " + spec.Value.Max + ".");
					resolved[pair.Key] = pair.Value;
				}
			if (effect == ModVisualEffect.WeaponTrails && resolved["full_speed"] <= resolved["min_speed"])
				throw new ModContentException("WeaponTrails.full_speed must be greater than min_speed.");
			if (setting != null)
			{
				bool found = false;
				foreach (ModSettingToggle toggle in _pendingSettings) if (toggle.Name == setting) found = true;
				if (!found) throw new ModContentException("Visual setting must be a toggle registered by this mod.");
			}
			var copied = new List<ModParticleRule>();
			if (rules != null)
			{
				if (rules.Count > MaxParticleRules) throw new ModContentException("At most " + MaxParticleRules + " particle rules are allowed.");
				foreach (ModParticleRule rule in rules)
				{
					if (rule == null || rule.Match.Count == 0 || rule.Match.Count > 16)
						throw new ModContentException("Each particle rule needs 1..16 match words.");
					foreach (string word in rule.Match) ValidateLocalName(word, "Particle match word");
					copied.Add(rule);
				}
			}
			EnsureCapacityForNewRegistration();
			var definition = new ModVisualDefinition(effect, Mod.Id, setting, resolved, color, defaultStyle, copied.ToArray());
			_pendingVisuals.Add(effect, definition);
			return definition;
		}

		private static void ValidateLocalName(string value, string what)
		{
			if (string.IsNullOrEmpty(value) || value.Length > 64)
				throw new ModContentException(what + " must be 1..64 lowercase ASCII letters, digits, '_' or '-'.");
			foreach (char character in value)
				if (!((character >= 'a' && character <= 'z') || (character >= '0' && character <= '9') ||
					  character == '_' || character == '-'))
					throw new ModContentException(what + " must be lowercase ASCII letters, digits, '_' or '-'.");
		}

		// One mod owns each effect; a second registration from another mod is a conflict.
		private void ValidateVisualsCommit()
		{
			foreach (ModVisualEffect effect in _pendingVisuals.Keys)
				if (_catalog.Visuals.TryGetValue(effect, out var existing))
					throw new ModContentException("Visual effect " + effect + " is already configured by '" + existing.Owner + "'.");
		}

		private void ApplyVisualsCommit() => _catalog.CommitVisuals(_pendingVisuals.Values, _pendingSettings);
		private void ClearVisualsPending() { _pendingVisuals.Clear(); _pendingSettings.Clear(); }
	}

	// How the running game reads a setting's current value. Headless hosts keep
	// the declared default; the game installs an installation-wide store.
	public static class ModSettingValues
	{
		public static Func<ModSettingToggle, bool> Read { get; set; } = toggle => toggle != null && toggle.Default;
	}
}

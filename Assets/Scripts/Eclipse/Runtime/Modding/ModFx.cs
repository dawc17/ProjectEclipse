using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
	// Composable presentation effects (sf2.fx). Unlike the single-owner
	// sf2.visuals presets, any number of mods may add these, each with its own
	// art and numbers. Presentation only: not saved and not fingerprinted.
	public enum ModFxKind { Particles, Overlay, Trail, Screen }

	// Where an effect lives. Background follows a background layer chosen by
	// depth; Behind sits behind the fighters; Front sits in front of them; Node
	// follows a node of each selected fighter.
	public enum ModFxPlacement { Background, Behind, Front, Node }

	public enum ModFxFighters { Both, Player, Opponent }

	public enum ModFxBlend { Alpha, Additive }

	// Fights: fights and the dojo only. Everywhere: also menu fighter previews
	// (fighter-attached effects only; location effects exist only in fights).
	public enum ModFxScenes { Fights, Everywhere }

	public sealed class ModFxDefinition
	{
		private readonly Dictionary<string, float> _numbers;
		private readonly string[] _match;
		private readonly string[] _nodes;

		public string Name { get; }
		public ModId Owner { get; }
		public ModFxKind Kind { get; }
		public string Setting { get; }
		public IReadOnlyList<string> Match => _match;
		public ModFxScenes Scenes { get; }
		public ModFxPlacement Placement { get; }
		public ModFxFighters Fighters { get; }
		public ModFxBlend Blend { get; }
		public AssetId? Sprite { get; }
		public ModUiColor Color { get; }
		public ModUiColor EndColor { get; }
		public bool Weapon { get; }
		public IReadOnlyList<string> Nodes => _nodes;

		internal ModFxDefinition(string name, ModId owner, ModFxKind kind, string setting, string[] match, ModFxScenes scenes,
			ModFxPlacement placement, ModFxFighters fighters, ModFxBlend blend, AssetId? sprite, ModUiColor color,
			ModUiColor endColor, bool weapon, string[] nodes, Dictionary<string, float> numbers)
		{
			Name = name; Owner = owner; Kind = kind; Setting = setting; _match = match ?? Array.Empty<string>();
			Scenes = scenes; Placement = placement; Fighters = fighters; Blend = blend; Sprite = sprite; Color = color;
			EndColor = endColor; Weapon = weapon; _nodes = nodes ?? Array.Empty<string>(); _numbers = numbers;
		}

		public float Number(string name) => _numbers[name];

		// True when no match words were given or one of them is a word of the location name.
		public bool MatchesLocation(string location)
		{
			if (_match.Length == 0) return true;
			if (string.IsNullOrEmpty(location)) return false;
			var words = new HashSet<string>(location.ToLowerInvariant().Split(new[] { '_', '-', ' ', ':', '/', '0', '1', '2', '3',
				'4', '5', '6', '7', '8', '9' }, StringSplitOptions.RemoveEmptyEntries));
			foreach (string word in _match) if (words.Contains(word)) return true;
			return false;
		}
	}

	// The optional values a mod supplies to sf2.fx.* registrations.
	public sealed class ModFxRequest
	{
		public string Setting;
		public string[] Match;
		public ModFxScenes Scenes = ModFxScenes.Fights;
		public ModFxPlacement Placement = ModFxPlacement.Behind;
		public ModFxFighters Fighters = ModFxFighters.Both;
		public ModFxBlend Blend = ModFxBlend.Alpha;
		public AssetId? Sprite;
		public ModUiColor Color;
		public ModUiColor EndColor;
		public bool Weapon;
		public string[] Nodes;
		public Dictionary<string, float> Numbers = new Dictionary<string, float>(StringComparer.Ordinal);
	}

	public static class ModFxParameters
	{
		private static readonly Dictionary<ModFxKind, (string Name, float Default, float Min, float Max)[]> Table =
			new Dictionary<ModFxKind, (string, float, float, float)[]>
			{
				{ ModFxKind.Particles, new[] {
					("count", 100f, 1f, 1000f), ("lifetime_min", 6f, 0.05f, 60f), ("lifetime_max", 10f, 0.05f, 60f),
					("size_min", 3f, 0.1f, 400f), ("size_max", 6f, 0.1f, 400f),
					("velocity_x_min", -10f, -2000f, 2000f), ("velocity_x_max", 10f, -2000f, 2000f),
					("velocity_y_min", -10f, -2000f, 2000f), ("velocity_y_max", 10f, -2000f, 2000f),
					("noise", 5f, 0f, 500f), ("spin", 0f, 0f, 1f), ("depth", 0.3f, 0f, 1f),
					("area_width", 1.1f, 0f, 2f), ("area_height", 1.1f, 0f, 2f), ("radius", 20f, 0f, 500f) } },
				{ ModFxKind.Overlay, new[] {
					("alpha", 1f, 0f, 1f), ("depth", 0.3f, 0f, 1f), ("x", 0f, -8192f, 8192f), ("y", 0f, -8192f, 8192f),
					("width", 0f, 0f, 16384f), ("height", 0f, 0f, 16384f) } },
				{ ModFxKind.Trail, new[] {
					("lifetime", 0.11f, 0.02f, 1f), ("min_speed", 900f, 0f, 20000f), ("full_speed", 2600f, 1f, 40000f),
					("alpha", 0.55f, 0f, 1f), ("start_alpha", 0.35f, 0f, 1f) } },
				{ ModFxKind.Screen, new[] {
					("saturation", 1f, 0f, 2f), ("contrast", 1f, 0f, 2f), ("brightness", 0f, -1f, 1f),
					("tint_strength", 0f, 0f, 1f), ("vignette", 0f, 0f, 1f) } },
			};

		public static IReadOnlyList<(string Name, float Default, float Min, float Max)> For(ModFxKind kind) => Table[kind];
	}

	public sealed partial class ModContentCatalog
	{
		private readonly List<ModFxDefinition> _fx = new List<ModFxDefinition>();
		public IReadOnlyList<ModFxDefinition> Effects => _fx.AsReadOnly();
		internal void CommitFx(IEnumerable<ModFxDefinition> fx) => _fx.AddRange(fx);
	}

	public sealed partial class ModRegistrationTransaction
	{
		public const int MaxEffectsPerMod = 32;
		private readonly List<ModFxDefinition> _pendingFx = new List<ModFxDefinition>();
		private int FxRegistrationCount => _pendingFx.Count;

		public ModFxDefinition RegisterFx(ModFxKind kind, string localId, ModFxRequest request)
		{
			ThrowIfCompleted();
			if (!Enum.IsDefined(typeof(ModFxKind), kind)) throw new ModContentException("Unsupported effect kind.");
			if (request == null) throw new ArgumentNullException(nameof(request));
			ValidateLocalName(localId, "Effect id");
			string name = Mod.Id.Value + "." + localId;
			foreach (ModFxDefinition existing in _pendingFx)
				if (existing.Name == name) throw new ModContentException("Duplicate effect: '" + name + "'.");
			if (_pendingFx.Count >= MaxEffectsPerMod)
				throw new ModContentException("A mod may register at most " + MaxEffectsPerMod + " effects.");

			var numbers = new Dictionary<string, float>(StringComparer.Ordinal);
			foreach (var parameter in ModFxParameters.For(kind)) numbers[parameter.Name] = parameter.Default;
			foreach (var pair in request.Numbers)
			{
				(string Name, float Default, float Min, float Max)? spec = null;
				foreach (var parameter in ModFxParameters.For(kind)) if (parameter.Name == pair.Key) spec = parameter;
				if (spec == null) throw new ModContentException("Unknown " + kind + " field '" + pair.Key + "'.");
				if (float.IsNaN(pair.Value) || pair.Value < spec.Value.Min || pair.Value > spec.Value.Max)
					throw new ModContentException(kind + "." + pair.Key + " must be between " + spec.Value.Min + " and " + spec.Value.Max + ".");
				numbers[pair.Key] = pair.Value;
			}
			RequireOrdered(numbers, kind, "lifetime_min", "lifetime_max");
			RequireOrdered(numbers, kind, "size_min", "size_max");
			RequireOrdered(numbers, kind, "velocity_x_min", "velocity_x_max");
			RequireOrdered(numbers, kind, "velocity_y_min", "velocity_y_max");
			if (kind == ModFxKind.Trail && numbers["full_speed"] <= numbers["min_speed"])
				throw new ModContentException("Trail.full_speed must be greater than min_speed.");

			if (request.Setting != null)
			{
				bool found = false;
				foreach (ModSettingToggle toggle in _pendingSettings) if (toggle.Name == request.Setting) found = true;
				if (!found) throw new ModContentException("Effect setting must be a toggle registered by this mod.");
			}
			string[] match = request.Match ?? Array.Empty<string>();
			if (match.Length > 32) throw new ModContentException("Effects accept at most 32 location match words.");
			foreach (string word in match) ValidateLocalName(word, "Location match word");

			string[] nodes = request.Nodes ?? Array.Empty<string>();
			if (kind == ModFxKind.Trail)
			{
				if (request.Weapon == (nodes.Length != 0))
					throw new ModContentException("A trail needs either weapon = true or exactly two nodes.");
				if (nodes.Length != 0 && nodes.Length != 2) throw new ModContentException("A trail needs exactly two nodes.");
			}
			else if (kind == ModFxKind.Particles && request.Placement == ModFxPlacement.Node)
			{
				if (nodes.Length != 1) throw new ModContentException("Node particles need exactly one node.");
			}
			else if (nodes.Length != 0 || request.Weapon)
				throw new ModContentException(kind + " does not accept nodes or weapon.");
			foreach (string node in nodes)
				if (string.IsNullOrWhiteSpace(node) || node.Length > 128) throw new ModContentException("Node names must be 1..128 characters.");

			if (kind == ModFxKind.Overlay && request.Placement != ModFxPlacement.Background && request.Placement != ModFxPlacement.Front)
				throw new ModContentException("Overlays are placed in the background or front.");
			bool fighterEffect = kind == ModFxKind.Trail || (kind == ModFxKind.Particles && request.Placement == ModFxPlacement.Node);
			if (request.Scenes == ModFxScenes.Everywhere && !fighterEffect)
				throw new ModContentException("Only trails and node particles can run everywhere; location effects exist only in fights.");

			EnsureCapacityForNewRegistration();
			var definition = new ModFxDefinition(name, Mod.Id, kind, request.Setting, (string[])match.Clone(), request.Scenes,
				kind == ModFxKind.Trail ? ModFxPlacement.Node : request.Placement, request.Fighters, request.Blend, request.Sprite,
				request.Color, request.EndColor, request.Weapon, (string[])nodes.Clone(), numbers);
			_pendingFx.Add(definition);
			return definition;
		}

		private static void RequireOrdered(Dictionary<string, float> numbers, ModFxKind kind, string min, string max)
		{
			if (numbers.TryGetValue(min, out float low) && numbers.TryGetValue(max, out float high) && low > high)
				throw new ModContentException(kind + "." + min + " must not exceed " + max + ".");
		}

		private void ApplyFxCommit() => _catalog.CommitFx(_pendingFx);
		private void ClearFxPending() => _pendingFx.Clear();
	}
}

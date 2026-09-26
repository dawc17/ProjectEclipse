using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
	// Composable presentation effects (sf2.fx). Unlike the single-owner
	// sf2.visuals presets, any number of mods may add these, each with its own
	// art and numbers. Presentation only: not saved and not fingerprinted.
	public enum ModFxKind { Particles, Overlay, Trail, Screen, Shadow, Glint, Light, Stain }

	// What carries a light: Weapon is a matching equipped weapon's blade; Magic is
	// a fighter casting magic and every magic projectile in flight.
	public enum ModFxLightSource { Weapon, Magic }

	// Where an effect lives. Background follows a background layer chosen by
	// depth; Behind sits behind the fighters; Front sits in front of them; Node
	// follows a node of each selected fighter; Hit bursts at a hit's contact point.
	public enum ModFxPlacement { Background, Behind, Front, Node, Hit }

	// What starts an effect. Always runs continuously; the others fire once per
	// matching hit: Hit is any damaging unblocked hit, Critical a critical hit,
	// Block a blocked hit and Ko the hit that empties a fighter's health.
	public enum ModFxTrigger { Always, Hit, Critical, Block, Ko }

	// Built-in overlay art when no sprite is given: Rect is a flat fill, Shaft a
	// soft vertical light beam and Glow a soft round light.
	public enum ModFxShape { Rect, Shaft, Glow }

	public enum ModFxFighters { Both, Player, Opponent }

	public enum ModFxBlend { Alpha, Additive }

	// Fights: fights and the dojo only. Everywhere: also menu fighter previews
	// (fighter-attached effects only; location effects exist only in fights).
	public enum ModFxScenes { Fights, Everywhere }

	public sealed class ModFxDefinition
	{
		private readonly Dictionary<string, float> _numbers;
		private readonly string[] _match;
		private readonly string[] _exclude;
		private readonly string[] _nodes;
		private string[] _weapons = Array.Empty<string>();

		public string Name { get; }
		public ModId Owner { get; }
		public ModFxKind Kind { get; }
		public string Setting { get; }
		public IReadOnlyList<string> Match => _match;
		public IReadOnlyList<string> Exclude => _exclude;
		public ModFxScenes Scenes { get; }
		public ModFxPlacement Placement { get; }
		public ModFxFighters Fighters { get; }
		public ModFxBlend Blend { get; }
		public AssetId? Sprite { get; }
		public ModUiColor Color { get; }
		public ModUiColor EndColor { get; }
		public ModUiColor AccentColor { get; }
		public ModUiColor HalationColor { get; }
		public ModFxTrigger Trigger { get; }
		public ModFxShape Shape { get; }
		public ModFxLightSource Source { get; private set; }
		public IReadOnlyList<string> Weapons => _weapons;
		public bool Weapon { get; }
		public IReadOnlyList<string> Nodes => _nodes;

		internal ModFxDefinition(string name, ModId owner, ModFxKind kind, string setting, string[] match, string[] exclude, ModFxScenes scenes,
			ModFxPlacement placement, ModFxFighters fighters, ModFxBlend blend, AssetId? sprite, ModUiColor color,
			ModUiColor endColor, bool weapon, string[] nodes, Dictionary<string, float> numbers,
			ModFxTrigger trigger = ModFxTrigger.Always, ModFxShape shape = ModFxShape.Rect, ModUiColor accentColor = null, ModUiColor halationColor = null)
		{
			Trigger = trigger; Shape = shape; AccentColor = accentColor; HalationColor = halationColor;
			Name = name; Owner = owner; Kind = kind; Setting = setting; _match = match ?? Array.Empty<string>();
			_exclude = exclude ?? Array.Empty<string>();
			Scenes = scenes; Placement = placement; Fighters = fighters; Blend = blend; Sprite = sprite; Color = color;
			EndColor = endColor; Weapon = weapon; _nodes = nodes ?? Array.Empty<string>(); _numbers = numbers;
		}

		public float Number(string name) => _numbers[name];

		internal void SetLightSource(ModFxLightSource source, string[] weapons)
		{
			Source = source;
			_weapons = weapons ?? Array.Empty<string>();
		}

		// True when a weapon's item name or subtype contains one of the weapon words.
		public bool MatchesWeapon(string name, string subtype)
		{
			string text = ((name ?? string.Empty) + " " + (subtype ?? string.Empty)).ToLowerInvariant();
			foreach (string word in _weapons) if (text.Contains(word)) return true;
			return false;
		}

		// True when the location name has one of the match words (or none were
		// given) and none of the exclude words.
		public bool MatchesLocation(string location)
		{
			if (_match.Length == 0 && _exclude.Length == 0) return true;
			var words = new HashSet<string>((location ?? string.Empty).ToLowerInvariant().Split(new[] { '_', '-', ' ', ':', '/', '0', '1', '2', '3',
				'4', '5', '6', '7', '8', '9' }, StringSplitOptions.RemoveEmptyEntries));
			foreach (string word in _exclude) if (words.Contains(word)) return false;
			if (_match.Length == 0) return true;
			foreach (string word in _match) if (words.Contains(word)) return true;
			return false;
		}
	}

	// The optional values a mod supplies to sf2.fx.* registrations.
	public sealed class ModFxRequest
	{
		public string Setting;
		public string[] Match;
		public string[] Exclude;
		public ModFxScenes Scenes = ModFxScenes.Fights;
		public ModFxPlacement Placement = ModFxPlacement.Behind;
		public ModFxFighters Fighters = ModFxFighters.Both;
		public ModFxBlend Blend = ModFxBlend.Alpha;
		public AssetId? Sprite;
		public ModUiColor Color;
		public ModUiColor EndColor;
		public ModUiColor AccentColor;
		public ModUiColor HalationColor;
		public ModFxTrigger Trigger = ModFxTrigger.Always;
		public ModFxShape Shape = ModFxShape.Rect;
		public ModFxLightSource Source = ModFxLightSource.Weapon;
		public string[] Weapons;
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
					("area_width", 1.1f, 0f, 2f), ("area_height", 1.1f, 0f, 2f), ("radius", 20f, 0f, 500f),
					("x", 0f, -8192f, 8192f), ("y", 0f, -8192f, 8192f),
					("speed_min", 0f, 0f, 5000f), ("speed_max", 0f, 0f, 5000f), ("gravity", 0f, -5000f, 5000f) } },
				{ ModFxKind.Overlay, new[] {
					("alpha", 1f, 0f, 1f), ("depth", 0.3f, 0f, 1f), ("x", 0f, -8192f, 8192f), ("y", 0f, -8192f, 8192f),
					("width", 0f, 0f, 16384f), ("height", 0f, 0f, 16384f), ("angle", 0f, -180f, 180f),
					("flicker", 0f, 0f, 1f), ("flicker_speed", 6f, 0.1f, 30f) } },
				{ ModFxKind.Trail, new[] {
					("lifetime", 0.11f, 0.02f, 1f), ("min_speed", 900f, 0f, 20000f), ("full_speed", 2600f, 1f, 40000f),
					("alpha", 0.55f, 0f, 1f), ("start_alpha", 0.35f, 0f, 1f) } },
				{ ModFxKind.Screen, new[] {
					("saturation", 1f, 0f, 2f), ("contrast", 1f, 0f, 2f), ("brightness", 0f, -1f, 1f),
					("tint_strength", 0f, 0f, 1f), ("vignette", 0f, 0f, 1f), ("vignette_x", 0f, -1f, 1f), ("vignette_y", 0f, -1f, 1f),
					("grain", 0f, 0f, 1f), ("halation", 0f, 0f, 2f), ("halation_threshold", 0.75f, 0f, 2f),
					("flicker", 0f, 0f, 1f), ("flicker_speed", 6f, 0.1f, 30f),
					("accent_strength", 0f, 0f, 1f), ("accent_width", 0.08f, 0.01f, 0.5f),
					("duration", 0.25f, 0.02f, 10f), ("hold", 0f, 0f, 10f), ("time_scale", 1f, 0.05f, 1f) } },
				{ ModFxKind.Shadow, new[] {
					("alpha", 0.45f, 0f, 1f), ("width", 150f, 1f, 2000f), ("height", 28f, 1f, 1000f),
					("fade_height", 350f, 1f, 5000f), ("min_scale", 0.35f, 0f, 1f) } },
				{ ModFxKind.Light, new[] {
					("radius", 320f, 10f, 5000f), ("intensity", 1f, 0f, 2f), ("fighter_light", 0.8f, 0f, 1f),
					("glow", 0.3f, 0f, 1f), ("glow_size", 380f, 1f, 5000f),
					("flicker", 0.15f, 0f, 1f), ("flicker_speed", 8f, 0.1f, 30f) } },
				{ ModFxKind.Stain, new[] {
					("count", 3f, 1f, 12f), ("size_min", 10f, 1f, 400f), ("size_max", 26f, 1f, 400f),
					("spread", 40f, 0f, 400f), ("flatten", 0.35f, 0.1f, 1f), ("alpha", 0.8f, 0f, 1f),
					("limit", 60f, 1f, 200f) } },
				{ ModFxKind.Glint, new[] {
					("interval", 3f, 0.2f, 60f), ("duration", 0.35f, 0.05f, 3f), ("size", 22f, 1f, 400f),
					("alpha", 0.9f, 0f, 1f), ("max_speed", 250f, 0f, 20000f) } },
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
			RequireOrdered(numbers, kind, "speed_min", "speed_max");
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
			string[] exclude = request.Exclude ?? Array.Empty<string>();
			if (exclude.Length > 64) throw new ModContentException("Effects accept at most 64 location exclude words.");
			foreach (string word in exclude) ValidateLocalName(word, "Location exclude word");

			string[] nodes = request.Nodes ?? Array.Empty<string>();
			if (kind == ModFxKind.Glint)
			{
				if (nodes.Length != 0) throw new ModContentException("A glint follows the weapon and accepts no nodes.");
			}
			else if (kind == ModFxKind.Trail)
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
			if (request.Placement == ModFxPlacement.Hit && kind != ModFxKind.Particles)
				throw new ModContentException("Only particles can be placed at hits.");
			ModFxTrigger trigger = request.Trigger;
			if (kind == ModFxKind.Particles && request.Placement == ModFxPlacement.Hit)
			{
				if (trigger == ModFxTrigger.Always) trigger = ModFxTrigger.Hit;
			}
			else if (kind == ModFxKind.Stain)
			{
				if (trigger == ModFxTrigger.Always) trigger = ModFxTrigger.Hit;
				if (trigger == ModFxTrigger.Block) throw new ModContentException("Stains trigger on hit, critical or ko.");
			}
			else if (kind == ModFxKind.Screen)
			{
				if (trigger == ModFxTrigger.Block) throw new ModContentException("Screen effects trigger always, on hit, critical or ko.");
				if (trigger == ModFxTrigger.Always && numbers["time_scale"] < 1f)
					throw new ModContentException("Screen.time_scale needs a trigger; a grade that is always on cannot slow the game.");
			}
			else if (trigger != ModFxTrigger.Always)
				throw new ModContentException("Only screen effects and hit particles accept a trigger.");
			if (kind != ModFxKind.Overlay && request.Shape != ModFxShape.Rect)
				throw new ModContentException("Only overlays accept a shape.");
			if (kind != ModFxKind.Screen && (request.AccentColor != null || request.HalationColor != null))
				throw new ModContentException("Only screen effects accept accent and halation colours.");
			string[] weapons = request.Weapons ?? Array.Empty<string>();
			if (kind == ModFxKind.Light)
			{
				if (request.Source == ModFxLightSource.Weapon && weapons.Length == 0)
					throw new ModContentException("A weapon light needs weapons: words found in the glowing weapons' names.");
				if (request.Source == ModFxLightSource.Magic && weapons.Length != 0)
					throw new ModContentException("A magic light does not accept weapons.");
				if (weapons.Length > 32) throw new ModContentException("A light accepts at most 32 weapon words.");
				foreach (string word in weapons) ValidateLocalName(word, "Weapon word");
			}
			else if (weapons.Length != 0)
				throw new ModContentException("Only lights accept weapons.");
			RequireOrdered(numbers, kind, "size_min", "size_max");
			foreach (string node in nodes)
				if (string.IsNullOrWhiteSpace(node) || node.Length > 128) throw new ModContentException("Node names must be 1..128 characters.");

			if (kind == ModFxKind.Overlay && request.Placement != ModFxPlacement.Background && request.Placement != ModFxPlacement.Front)
				throw new ModContentException("Overlays are placed in the background or front.");
			bool fighterEffect = kind == ModFxKind.Trail || kind == ModFxKind.Glint ||
				(kind == ModFxKind.Light && request.Source == ModFxLightSource.Weapon) ||
				(kind == ModFxKind.Particles && request.Placement == ModFxPlacement.Node);
			if (request.Scenes == ModFxScenes.Everywhere && !fighterEffect)
				throw new ModContentException("Only trails, glints and node particles can run everywhere; other effects exist only in fights.");

			EnsureCapacityForNewRegistration();
			var definition = new ModFxDefinition(name, Mod.Id, kind, request.Setting, (string[])match.Clone(), (string[])exclude.Clone(), request.Scenes,
				kind == ModFxKind.Trail || kind == ModFxKind.Glint ? ModFxPlacement.Node : request.Placement, request.Fighters, request.Blend, request.Sprite,
				request.Color, request.EndColor, request.Weapon || kind == ModFxKind.Glint, (string[])nodes.Clone(), numbers,
				trigger, request.Shape, request.AccentColor, request.HalationColor);
			if (kind == ModFxKind.Light) definition.SetLightSource(request.Source, (string[])weapons.Clone());
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

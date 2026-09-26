using System.Collections.Generic;
using System.Text;
using Eclipse.Modding;
using Eclipse.Rendering.Interpolation;
using UnityEngine;

namespace Eclipse.Rendering
{
	// Per-fighter sf2.fx effects:
	// - particles with placement = "node": an emitter that follows one node of a
	//   fighter (a hand, a weapon tip, the pivot), simulated in world space so it
	//   streams behind a moving node;
	// - particles with placement = "hit": a burst at the contact point when this
	//   fighter is struck;
	// - particles with placement = "contact": bursts where the fighter lands, is
	//   knocked down, skids along the floor or plays a wall-hit recoil (see UpdateMotion);
	// - sf2.fx.shadow: a soft oval on the floor under the fighter that shrinks and
	//   fades as the fighter rises;
	// - the rim-light ink weight, which rises while the fighter casts magic;
	// - sf2.fx.light: glowing weapons and magic light the stage behind them and
	//   every fighter in reach (through the rim light, turned toward the source);
	// - sf2.fx.stain: splats left on the floor under hits until the round ends.
	public sealed class FighterParticles : MonoBehaviour
	{
		private sealed class Emitter
		{
			public ModelNode Node;
			public ParticleSystem System;
		}

		private sealed class Burst
		{
			public ModFxDefinition Definition;
			public ParticleSystem System;
		}

		private sealed class Shadow
		{
			public ModFxDefinition Definition;
			public SpriteRenderer Renderer;
		}

		// Shadows sit just behind the fighter's body, in front of the rim-light twin.
		private const float ShadowDepth = 0.045f;

		private static readonly List<FighterParticles> Live = new List<FighterParticles>();
		private static Fight _groundFight;
		// The floor as a height measured upward on screen (see ScreenUp); MaxValue until found.
		private static float _groundY = float.MaxValue;
		private static float _groundSign = 1f;

		// Lights found this frame, and the complete set from the previous frame
		// that fighters are lit by (every fighter sees every light, whatever order
		// the fighters update in).
		private struct LightSample { public Vector3 Position; public Color Color; public float Radius, Strength; }
		private static List<LightSample> _lightsBuilding = new List<LightSample>();
		private static List<LightSample> _lightsReady = new List<LightSample>();
		private static int _lightFrame = -1;
		private static readonly Dictionary<string, Queue<GameObject>> Stains = new Dictionary<string, Queue<GameObject>>();
		private static Sprite[] _stainSprites;

		private readonly List<SpriteRenderer> _glows = new List<SpriteRenderer>();
		private float _lowest = float.MaxValue, _centerX;
		private bool _hasFloorSample;
		private float _flickerSeed;
		// Set once a projectile has struck something: its magic light is out.
		private bool _spent;

		private Model _model;
		private ModelPresentation _presentation;
		private readonly List<Emitter> _emitters = new List<Emitter>();
		private readonly List<Burst> _bursts = new List<Burst>();
		private readonly List<Burst> _contacts = new List<Burst>();

		// Motion triggers, in fighter units (a standing fighter is about 300 tall).
		private const float AirborneHeight = 30f, GroundedHeight = 10f, MinimumFall = 45f;
		private const float KnockdownHeight = 60f, KnockdownSpeed = 120f, KnockdownCooldown = 0.6f;
		private const float SlideSpeed = 420f, SlideInterval = 0.07f;
		// The recoil moves the fighter plays when knocked into the arena wall (moves.xml).
		private static readonly string[] WallHitMoves = { "WallHit", "WallHitFall" };
		// A jump this large in one frame is a reposition (round start), not motion.
		private const float Teleport = 150f;
		private float _pivotHeight, _lowestX, _localCenterX, _minX, _minXY, _maxX, _maxXY;
		private float _previousCenter = float.NaN, _previousPivot, _peak;
		private bool _airborne, _hasPivot;
		private float _nextKnockdown, _nextSlide;
		private bool _inWallHit;
		private readonly List<Shadow> _shadows = new List<Shadow>();
		private string _key;
		private int _nodeCount = -1;
		private string _shadowKey;

		// 0..1: how far this fighter's rim light has turned to ink (see RimLight).
		public float InkWeight { get; private set; }

		// How strongly nearby sf2.fx.light sources light this fighter (0..1), their
		// blended colour, and the world direction from the fighter toward them.
		public float LightAmount { get; private set; }
		public Color LightColor { get; private set; } = Color.white;
		public Vector2 LightDirection { get; private set; } = new Vector2(-1f, 1f).normalized;

		// Removes every stain; the fight calls this when a round begins.
		public static void ClearStains()
		{
			foreach (Queue<GameObject> queue in Stains.Values)
				while (queue.Count > 0) { GameObject stain = queue.Dequeue(); if (stain != null) Destroy(stain); }
			Stains.Clear();
		}

		public static void Attach(GameObject root, Model model)
		{
			var particles = root.GetComponent<FighterParticles>() ?? root.AddComponent<FighterParticles>();
			particles._model = model;
		}

		// Called by the fight once a strike is resolved. `point` is the contact
		// point in fighter coordinates; `ko` is the hit that emptied the health.
		// A projectile attacker has struck, so its light goes out.
		public static void Hit(Model victim, Vector3f point, bool critical, bool blocked, bool ko, Model attacker = null)
		{
			ModVisuals.NotifyHit(critical, blocked, ko);
			if (attacker != null && attacker.NJDJHGDMCIJ() != null)
				foreach (FighterParticles live in Live)
					if (live != null && live._model == attacker) live._spent = true;
			if (victim == null || point == null) return;
			foreach (FighterParticles fighter in Live)
				if (fighter != null && fighter._model == victim)
				{
					fighter.EmitHit(new Vector3(point.GetX(), point.GetY(), 0f), critical, blocked, ko);
					return;
				}
		}

		private void OnEnable() { if (!Live.Contains(this)) Live.Add(this); }
		private void OnDisable() { Live.Remove(this); }

		private void Start()
		{
			_presentation = GetComponent<ModelPresentation>();
			_flickerSeed = Random.Range(0f, 100f);
		}

		private void LateUpdate()
		{
			if (_model == null) return;
			Refresh();
			float alpha = _presentation != null ? _presentation.Alpha : FightInterpolation.FightAlpha;
			foreach (Emitter emitter in _emitters)
			{
				if (emitter.System == null) continue;
				float x, y, z;
				FightInterpolation.SamplePosition(emitter.Node, alpha, out x, out y, out z);
				// In front of the fighter's body; follows the interpolated node.
				emitter.System.transform.localPosition = new Vector3(x, y, -0.1f);
			}
			UpdateGround(alpha);
			UpdateMotion();
			UpdateShadows(alpha);
			UpdateInk();
			UpdateLights(alpha);
			UpdateLighting(alpha);
		}

		private void Refresh()
		{
			bool inFight = FightInterpolation.IsFightActive;
			string location = inFight ? LocationAtmosphere.CurrentLocationName : null;
			var active = new List<ModFxDefinition>();
			var bursts = new List<ModFxDefinition>();
			var contacts = new List<ModFxDefinition>();
			var key = new StringBuilder();
			foreach (ModFxDefinition definition in ModVisuals.ActiveFx(ModFxKind.Particles))
			{
				if (definition.Placement != ModFxPlacement.Node && definition.Placement != ModFxPlacement.Hit &&
					definition.Placement != ModFxPlacement.Contact) continue;
				if (definition.Scenes == ModFxScenes.Fights && !inFight) continue;
				if (inFight && !definition.MatchesLocation(location)) continue;
				if (!FighterMatches(definition.Fighters)) continue;
				(definition.Placement == ModFxPlacement.Hit ? bursts : definition.Placement == ModFxPlacement.Contact ? contacts : active).Add(definition);
				key.Append(definition.Name).Append('|');
			}
			Dictionary<string, ModelNode> nodes = _model.CLDMEJKGLBA()?.HKCFFKKFFFE();
			int nodeCount = nodes != null ? nodes.Count : 0;
			string built = key.ToString();
			if (built == _key && nodeCount == _nodeCount) return;
			_key = built; _nodeCount = nodeCount;
			foreach (Emitter old in _emitters) if (old.System != null) Destroy(old.System.gameObject);
			_emitters.Clear();
			foreach (Burst old in _bursts) if (old.System != null) Destroy(old.System.gameObject);
			_bursts.Clear();
			foreach (ModFxDefinition definition in bursts)
				_bursts.Add(new Burst { Definition = definition, System = FxBuilder.CreateBurst(transform, definition) });
			foreach (Burst old in _contacts) if (old.System != null) Destroy(old.System.gameObject);
			_contacts.Clear();
			foreach (ModFxDefinition definition in contacts)
				_contacts.Add(new Burst { Definition = definition, System = FxBuilder.CreateBurst(transform, definition) });
			ModelObject body = _model.CLDMEJKGLBA();
			if (body == null) return;
			foreach (ModFxDefinition definition in active)
			{
				ModelNode node = body.KLAPIGGACMM(definition.Nodes[0]);
				if (node == null) continue;
				ParticleSystem system = FxBuilder.CreateEmitter(transform, Vector3.zero, Vector3.one, definition, Vector2.zero, true);
				_emitters.Add(new Emitter { Node = node, System = system });
			}
		}

		private void EmitHit(Vector3 point, bool critical, bool blocked, bool ko)
		{
			foreach (Burst burst in _bursts)
			{
				if (burst.System == null) continue;
				ModFxTrigger trigger = burst.Definition.Trigger;
				bool fire = trigger == ModFxTrigger.Hit ? !blocked : trigger == ModFxTrigger.Critical ? critical && !blocked
					: trigger == ModFxTrigger.Block ? blocked : trigger == ModFxTrigger.Ko && ko;
				if (!fire) continue;
				// World-space particles spawn where the emitter is now and stay there.
				burst.System.transform.localPosition = new Vector3(point.x, point.y, -0.2f);
				burst.System.Emit(Mathf.Max(1, Mathf.RoundToInt(burst.Definition.Number("count"))));
			}
			if (blocked || !FightInterpolation.IsFightActive || _groundY == float.MaxValue) return;
			foreach (ModFxDefinition stain in ModVisuals.ActiveFx(ModFxKind.Stain))
			{
				if (!stain.MatchesLocation(LocationAtmosphere.CurrentLocationName) || !FighterMatches(stain.Fighters)) continue;
				bool fire = stain.Trigger == ModFxTrigger.Hit || (stain.Trigger == ModFxTrigger.Critical && critical) ||
					(stain.Trigger == ModFxTrigger.Ko && ko);
				if (fire) SpawnStains(stain, point);
			}
		}

		// Splats on the floor below the hit, parented to the fight's render container
		// so they stay where they landed as the fighters move on.
		private void SpawnStains(ModFxDefinition d, Vector3 point)
		{
			Transform container = transform.parent != null ? transform.parent : transform;
			float unit = Mathf.Max(Mathf.Abs(transform.lossyScale.y), 1e-5f);
			Vector3 hit = transform.TransformPoint(point);
			float depth = transform.TransformPoint(new Vector3(0f, 0f, 0.06f)).z;
			if (!Stains.TryGetValue(d.Name, out var queue)) Stains[d.Name] = queue = new Queue<GameObject>();
			Sprite custom = FxBuilder.LoadSprite(d.Sprite, d.Name);
			int count = Mathf.RoundToInt(d.Number("count"));
			for (int i = 0; i < count; i++)
			{
				var stain = new GameObject("Effect " + d.Name).AddComponent<SpriteRenderer>();
				stain.sprite = custom != null ? custom : StainSprite(Random.Range(0, 3));
				stain.sharedMaterial = FxBuilder.MaterialFor(stain.sprite.texture, d.Blend);
				stain.transform.SetParent(container, true);
				float spread = d.Number("spread") * unit;
				Vector3 place = new Vector3(hit.x + Random.Range(-spread, spread), hit.y, depth);
				place.y = _groundSign * _groundY;
				stain.transform.position = place;
				float size = Random.Range(d.Number("size_min"), d.Number("size_max")) * unit;
				Vector2 bounds = stain.sprite.bounds.size;
				Vector3 parentScale = container.lossyScale;
				float sx = size / Mathf.Max(bounds.x, 1e-3f) / Mathf.Max(Mathf.Abs(parentScale.x), 1e-5f);
				float sy = size * d.Number("flatten") / Mathf.Max(bounds.y, 1e-3f) / Mathf.Max(Mathf.Abs(parentScale.y), 1e-5f);
				stain.transform.localScale = new Vector3(Random.value < 0.5f ? -sx : sx, sy, 1f);
				Color color = ModVisuals.ToColor(d.Color, new Color(0.35f, 0.03f, 0.03f, 1f));
				color.a *= d.Number("alpha") * Random.Range(0.75f, 1f);
				stain.color = color;
				queue.Enqueue(stain.gameObject);
				while (queue.Count > Mathf.RoundToInt(d.Number("limit")))
				{
					GameObject oldest = queue.Dequeue();
					if (oldest != null) Destroy(oldest);
				}
			}
		}

		// Three irregular splats: a core blob with droplets thrown around it.
		private static Sprite StainSprite(int variant)
		{
			if (_stainSprites == null) _stainSprites = new Sprite[3];
			if (_stainSprites[variant] != null) return _stainSprites[variant];
			const int size = 64;
			var random = new System.Random(911 + variant * 37);
			var blobs = new List<Vector3>();
			blobs.Add(new Vector3(0.5f, 0.5f, 0.24f + (float)random.NextDouble() * 0.06f));
			for (int i = 0; i < 5; i++)
			{
				float angle = (float)random.NextDouble() * Mathf.PI * 2f, reach = 0.12f + (float)random.NextDouble() * 0.12f;
				blobs.Add(new Vector3(0.5f + Mathf.Cos(angle) * reach, 0.5f + Mathf.Sin(angle) * reach, 0.1f + (float)random.NextDouble() * 0.08f));
			}
			for (int i = 0; i < 9; i++)
			{
				float angle = (float)random.NextDouble() * Mathf.PI * 2f, reach = 0.3f + (float)random.NextDouble() * 0.16f;
				blobs.Add(new Vector3(0.5f + Mathf.Cos(angle) * reach, 0.5f + Mathf.Sin(angle) * reach, 0.015f + (float)random.NextDouble() * 0.03f));
			}
			var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave };
			var pixels = new Color32[size * size];
			for (int y = 0; y < size; y++)
				for (int x = 0; x < size; x++)
				{
					float u = (x + 0.5f) / size, v = (y + 0.5f) / size, field = 0f;
					foreach (Vector3 blob in blobs)
					{
						float dx = u - blob.x, dy = v - blob.y;
						field += blob.z * blob.z / Mathf.Max(dx * dx + dy * dy, 1e-5f);
					}
					// Metaball edge: solid inside, a one-pixel soft rim.
					float a = Mathf.Clamp01((field - 1f) * 4f);
					pixels[y * size + x] = new Color32(255, 255, 255, (byte)(a * 255f));
				}
			texture.SetPixels32(pixels);
			texture.Apply(false, true);
			return _stainSprites[variant] = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
		}

		// The floor for shadows and stains: the lowest point any fighter's skeleton
		// reached in this fight (the physics keeps nodes above it). Projectiles and
		// other child models are left out.
		private void UpdateGround(float alpha)
		{
			_hasFloorSample = false;
			if (!FightInterpolation.IsFightActive || _model.NJDJHGDMCIJ() != null) return;
			if (!ModVisuals.HasActiveFx(ModFxKind.Shadow) && !ModVisuals.HasActiveFx(ModFxKind.Stain) && !WantsMotion()) return;
			Dictionary<string, ModelNode> nodes = _model.CLDMEJKGLBA()?.HKCFFKKFFFE();
			if (nodes == null) return;
			// Heights are measured upward on screen: the fight camera may show world
			// +y pointing down, so "lowest" follows the camera, not the world axis.
			float sign = ScreenUp();
			// Skeleton nodes are named N*; weapon macro nodes are left out.
			float lowest = float.MaxValue, sumX = 0f, localSum = 0f, lowestX = 0f; int count = 0;
			_minX = float.MaxValue; _maxX = float.MinValue;
			foreach (var pair in nodes)
			{
				if (pair.Value == null || pair.Key == null || pair.Key.Length < 2 || pair.Key[0] != 'N') continue;
				float x, y, z;
				FightInterpolation.SamplePosition(pair.Value, alpha, out x, out y, out z);
				Vector3 world = transform.TransformPoint(new Vector3(x, y, 0f));
				float height = world.y * sign;
				if (height < lowest) { lowest = height; lowestX = world.x; }
				sumX += world.x; localSum += x; count++;
				// The body's extent along the arena, in the same units as the walls.
				if (x < _minX) { _minX = x; _minXY = y; }
				if (x > _maxX) { _maxX = x; _maxXY = y; }
			}
			if (count == 0) return;
			_lowestX = lowestX;
			_localCenterX = localSum / count;
			ModelNode pivot = _model.CLDMEJKGLBA()?.HOFFDCFEBGA();
			_hasPivot = pivot != null;
			if (_hasPivot)
			{
				float px, py, pz;
				FightInterpolation.SamplePosition(pivot, alpha, out px, out py, out pz);
				_pivotHeight = transform.TransformPoint(new Vector3(px, py, 0f)).y * sign;
			}
			Fight fight = Fight.GetCurrentFight();
			if (fight != _groundFight || sign != _groundSign) { _groundFight = fight; _groundY = float.MaxValue; _groundSign = sign; }
			if (lowest < _groundY) _groundY = lowest;
			_lowest = lowest; _centerX = sumX / count; _hasFloorSample = true;
		}

		// Contact particles, or a screen effect waiting on a motion trigger.
		private bool WantsMotion()
		{
			if (_contacts.Count != 0) return true;
			foreach (ModFxDefinition definition in ModVisuals.ActiveFx(ModFxKind.Screen))
				if (ModFxParameters.IsMotionTrigger(definition.Trigger)) return true;
			return false;
		}

		// Reads the fighter's movement against the floor and the arena walls and fires
		// the motion triggers. Presentation only: it never changes the fight. Uses game
		// time, so nothing fires while paused and slow motion slows the checks too.
		private void UpdateMotion()
		{
			if (!_hasFloorSample || _groundY == float.MaxValue || !WantsMotion() || !_hasPivot)
			{
				_previousCenter = float.NaN;
				return;
			}
			float dt = Time.deltaTime;
			if (dt <= 0f) return;
			float unit = Mathf.Max(Mathf.Abs(transform.lossyScale.y), 1e-5f);
			float height = (_lowest - _groundY) / unit;
			float pivot = (_pivotHeight - _groundY) / unit;
			float center = _localCenterX, previousCenter = _previousCenter, previousPivot = _previousPivot;
			_previousCenter = center;
			_previousPivot = pivot;
			UpdateWallHit();
			if (float.IsNaN(previousCenter) || Mathf.Abs(center - previousCenter) > Teleport) { _airborne = false; return; }
			float vx = (center - previousCenter) / dt;
			float vy = (pivot - previousPivot) / dt;
			float now = Time.time;

			// Landing: a real fall (not a hop) that ends with the feet on the floor.
			if (height > AirborneHeight)
			{
				if (!_airborne) { _airborne = true; _peak = height; }
				else if (height > _peak) _peak = height;
			}
			else if (_airborne && height < GroundedHeight)
			{
				_airborne = false;
				if (_peak > MinimumFall) Fire(ModFxTrigger.Land, FloorPoint(_lowestX));
			}
			// Knockdown: the body's centre drops to the floor while falling.
			if (previousPivot >= KnockdownHeight && pivot < KnockdownHeight && vy < -KnockdownSpeed && now >= _nextKnockdown)
			{
				_nextKnockdown = now + KnockdownCooldown;
				Fire(ModFxTrigger.Knockdown, FloorPoint(_centerX));
			}
			// Slide: feet on the floor while the body moves fast along it (skids, pushback, dashes).
			if (height < GroundedHeight && Mathf.Abs(vx) > SlideSpeed && now >= _nextSlide)
			{
				_nextSlide = now + SlideInterval;
				Fire(ModFxTrigger.Slide, FloorPoint(_lowestX));
			}
		}

		// Wall: fires once when the fighter starts a wall-hit recoil (WallHit, or
		// WallHitFall straight from another move), at the edge of the body facing
		// the nearer arena wall.
		private void UpdateWallHit()
		{
			InfoAnimation current = _model.OCPMJKIEPIG()?.NNMAFFCCMHC();
			bool inWallHit = false;
			if (current != null)
				foreach (string move in WallHitMoves)
					if (current.CNPFHBMGDFP(move)) { inWallHit = true; break; }
			bool started = inWallHit && !_inWallHit;
			_inWallHit = inWallHit;
			if (!started || !FightInterpolation.IsFightActive) return;
			float left = GameUtils.CKOPPGCIHPL(), right = GameUtils.FBOGLADLJML();
			bool leftWall = right > left ? _minX - left < right - _maxX : _localCenterX < 0f;
			Fire(ModFxTrigger.Wall, leftWall ? new Vector3(_minX, _minXY, 0f) : new Vector3(_maxX, _maxXY, 0f));
		}

		// A point on the floor under a world x, in this fighter's local space.
		private Vector3 FloorPoint(float worldX)
		{
			Vector3 local = transform.InverseTransformPoint(new Vector3(worldX, _groundSign * _groundY, transform.position.z));
			return new Vector3(local.x, local.y, 0f);
		}

		private void Fire(ModFxTrigger trigger, Vector3 point)
		{
			ModVisuals.NotifyMotion(trigger);
			foreach (Burst contact in _contacts)
			{
				if (contact.System == null || contact.Definition.Trigger != trigger) continue;
				// In front of the fighter's feet, so the dust reads over the body.
				contact.System.transform.localPosition = new Vector3(point.x, point.y, -0.15f);
				contact.System.Emit(Mathf.Max(1, Mathf.RoundToInt(contact.Definition.Number("count"))));
			}
		}

		// +1 when world +y points up on screen, -1 when the camera shows it pointing down.
		private float ScreenUp()
		{
			UnityEngine.Camera camera = UnityEngine.Camera.main;
			if (camera == null) return 1f;
			Vector3 p = transform.position;
			float a = camera.WorldToScreenPoint(p).y, b = camera.WorldToScreenPoint(p + Vector3.up * 10f).y;
			return b >= a ? 1f : -1f;
		}

		// Contact shadows exist in fights only: the floor is the lowest point any
		// fighter's skeleton reached in this fight (the physics keeps nodes above it).
		private void UpdateShadows(float alpha)
		{
			bool inFight = FightInterpolation.IsFightActive;
			var active = new List<ModFxDefinition>();
			var key = new StringBuilder();
			if (inFight)
				foreach (ModFxDefinition definition in ModVisuals.ActiveFx(ModFxKind.Shadow))
				{
					if (!definition.MatchesLocation(LocationAtmosphere.CurrentLocationName) || !FighterMatches(definition.Fighters)) continue;
					active.Add(definition);
					key.Append(definition.Name).Append('|');
				}
			string built = key.ToString();
			if (built != _shadowKey)
			{
				_shadowKey = built;
				foreach (Shadow old in _shadows) if (old.Renderer != null) Destroy(old.Renderer.gameObject);
				_shadows.Clear();
				foreach (ModFxDefinition definition in active)
				{
					var renderer = new GameObject("Effect " + definition.Name).AddComponent<SpriteRenderer>();
					renderer.transform.SetParent(transform, false);
					renderer.sprite = FxBuilder.ShadowSprite;
					renderer.sharedMaterial = FxBuilder.MaterialFor(renderer.sprite.texture, ModFxBlend.Alpha);
					_shadows.Add(new Shadow { Definition = definition, Renderer = renderer });
				}
			}
			if (_shadows.Count == 0) return;
			if (!_hasFloorSample) { SetShadowsVisible(false); return; }

			float unit = Mathf.Max(Mathf.Abs(transform.lossyScale.y), 1e-5f);
			float height = (_lowest - _groundY) / unit;
			Vector3 floor = transform.InverseTransformPoint(new Vector3(_centerX, _groundSign * _groundY, transform.position.z));
			foreach (Shadow shadow in _shadows)
			{
				if (shadow.Renderer == null) continue;
				ModFxDefinition d = shadow.Definition;
				float rise = Mathf.Clamp01(height / d.Number("fade_height"));
				float scale = Mathf.Lerp(1f, d.Number("min_scale"), rise);
				Vector2 size = shadow.Renderer.sprite.bounds.size;
				shadow.Renderer.transform.localPosition = new Vector3(floor.x, floor.y, ShadowDepth);
				shadow.Renderer.transform.localScale = new Vector3(d.Number("width") * scale / Mathf.Max(size.x, 1e-3f),
					d.Number("height") * scale / Mathf.Max(size.y, 1e-3f), 1f);
				Color color = ModVisuals.ToColor(d.Color, Color.black);
				color.a *= d.Number("alpha") * (1f - rise);
				shadow.Renderer.color = color;
				if (!shadow.Renderer.gameObject.activeSelf) shadow.Renderer.gameObject.SetActive(true);
			}
		}

		private void SetShadowsVisible(bool visible)
		{
			foreach (Shadow shadow in _shadows)
				if (shadow.Renderer != null && shadow.Renderer.gameObject.activeSelf != visible) shadow.Renderer.gameObject.SetActive(visible);
		}

		// Rises while the fighter's current move is a magic cast, then eases back.
		private void UpdateInk()
		{
			ModVisualDefinition rim = ModVisuals.Active(ModVisualEffect.RimLight);
			if (rim == null || rim.Number("ink") <= 0f) { InkWeight = 0f; return; }
			InfoAnimation current = _model.OCPMJKIEPIG()?.NNMAFFCCMHC();
			float target = current != null && current.CNPFHBMGDFP("MagicPlayer") ? 1f : 0f;
			InkWeight = Mathf.MoveTowards(InkWeight, target, Time.unscaledDeltaTime * 5f);
		}

		// This model's light sources for the frame: a glow behind each, and a sample
		// other fighters are lit by. A projectile or summoned model follows the
		// fighter filter of the model that owns it.
		private void UpdateLights(float alpha)
		{
			if (Time.frameCount != _lightFrame)
			{
				_lightFrame = Time.frameCount;
				var done = _lightsBuilding; _lightsBuilding = _lightsReady; _lightsReady = done;
				_lightsBuilding.Clear();
			}
			int used = 0;
			if (ModVisuals.HasActiveFx(ModFxKind.Light))
			{
				bool inFight = FightInterpolation.IsFightActive;
				ModelObject body = _model.CLDMEJKGLBA();
				InfoAnimation current = _model.OCPMJKIEPIG()?.NNMAFFCCMHC();
				float unit = Mathf.Max(Mathf.Abs(transform.lossyScale.y), 1e-5f);
				foreach (ModFxDefinition d in ModVisuals.ActiveFx(ModFxKind.Light))
				{
					if (d.Scenes == ModFxScenes.Fights && !inFight) continue;
					if (inFight && !d.MatchesLocation(LocationAtmosphere.CurrentLocationName)) continue;
					if (!OwnerMatches(d.Fighters) || body == null) continue;
					float flicker = 1f - d.Number("flicker") * Mathf.PerlinNoise(Time.unscaledTime * d.Number("flicker_speed"), _flickerSeed);
					float strength = d.Number("intensity") * flicker;
					if (d.Source == ModFxLightSource.Weapon)
					{
						if (!HasMatchingWeapon(d)) continue;
						foreach (string name in new[] { "Weapon-Node2_1", "Weapon-Node2_2" })
						{
							ModelNode node = body.KLAPIGGACMM(name);
							if (node != null) AddLight(d, node, alpha, strength, unit, ref used);
						}
					}
					else if (current != null && current.CNPFHBMGDFP("MagicPlayer"))
					{
						ModelNode node = body.KLAPIGGACMM("Magic-Node2_1") ?? body.KLAPIGGACMM("NKnuckles_1");
						if (node != null) AddLight(d, node, alpha, strength, unit, ref used);
					}
					else if (!_spent && current != null && current.CNPFHBMGDFP("MagicMissile") && !current.CNPFHBMGDFP("MagicMissileEnd") &&
						body.HOFFDCFEBGA() != null)
						AddLight(d, body.HOFFDCFEBGA(), alpha, strength, unit, ref used);
				}
			}
			for (int i = used; i < _glows.Count; i++)
				if (_glows[i] != null && _glows[i].gameObject.activeSelf) _glows[i].gameObject.SetActive(false);
		}

		private void AddLight(ModFxDefinition d, ModelNode node, float alpha, float strength, float unit, ref int used)
		{
			float x, y, z;
			FightInterpolation.SamplePosition(node, alpha, out x, out y, out z);
			Vector3 world = transform.TransformPoint(new Vector3(x, y, 0f));
			Color color = ModVisuals.ToColor(d.Color, new Color(1f, 0.69f, 0.38f, 1f));
			_lightsBuilding.Add(new LightSample { Position = world, Color = color, Radius = d.Number("radius") * unit,
				Strength = strength * d.Number("fighter_light") });
			if (d.Number("glow") <= 0f) return;
			if (used >= _glows.Count)
			{
				var glow = new GameObject("Effect light glow").AddComponent<SpriteRenderer>();
				glow.transform.SetParent(transform, false);
				glow.sprite = FxBuilder.ShapeSprite(ModFxShape.Glow);
				glow.sharedMaterial = FxBuilder.MaterialFor(glow.sprite.texture, ModFxBlend.Additive);
				_glows.Add(glow);
			}
			SpriteRenderer renderer = _glows[used++];
			if (!renderer.gameObject.activeSelf) renderer.gameObject.SetActive(true);
			// Behind the fighter's body, so it lights the stage around the source.
			renderer.transform.localPosition = new Vector3(x, y, 0.06f);
			float size = d.Number("glow_size") / Mathf.Max(renderer.sprite.bounds.size.x, 1e-3f);
			renderer.transform.localScale = new Vector3(size, size, 1f);
			Color tint = color;
			tint.a *= Mathf.Clamp01(d.Number("glow") * strength);
			renderer.color = tint;
		}

		private bool HasMatchingWeapon(ModFxDefinition d)
		{
			List<ItemInfo> items = _model.Parameters?.PJNJIJIODHE();
			if (items == null) return false;
			foreach (ItemInfo item in items)
				if (item != null && item.Type == "Weapon" && d.MatchesWeapon(item.Name, item.SubType)) return true;
			return false;
		}

		// Blends every light in reach of this fighter's body: strength falls off
		// with the square of distance over the light's radius.
		private void UpdateLighting(float alpha)
		{
			float total = 0f;
			Color colour = Color.black;
			Vector2 direction = Vector2.zero;
			ModelNode pivot = _model.CLDMEJKGLBA()?.HOFFDCFEBGA();
			if (pivot != null && _lightsReady.Count != 0)
			{
				float x, y, z;
				FightInterpolation.SamplePosition(pivot, alpha, out x, out y, out z);
				Vector3 centre = transform.TransformPoint(new Vector3(x, y, 0f));
				foreach (LightSample light in _lightsReady)
				{
					Vector2 offset = new Vector2(light.Position.x - centre.x, light.Position.y - centre.y);
					float reach = Mathf.Clamp01(1f - offset.magnitude / Mathf.Max(light.Radius, 1e-3f));
					float f = reach * reach * light.Strength;
					if (f <= 0f) continue;
					total += f;
					colour += light.Color * f;
					direction += (offset.sqrMagnitude > 1e-6f ? offset.normalized : new Vector2(0f, 1f)) * f;
				}
			}
			float target = Mathf.Clamp01(total);
			// Rises smoothly, but drops at once when a source goes out (a projectile hits).
			LightAmount = target < LightAmount ? target : Mathf.MoveTowards(LightAmount, target, Time.unscaledDeltaTime * 8f);
			if (total > 0f)
			{
				LightColor = colour / total;
				if (direction.sqrMagnitude > 1e-6f) LightDirection = direction.normalized;
			}
		}

		// A projectile or summoned model is judged by the fighter that owns it.
		private bool OwnerMatches(ModFxFighters fighters)
		{
			if (fighters == ModFxFighters.Both) return true;
			Model owner = _model;
			for (int i = 0; i < 4 && owner.NJDJHGDMCIJ() != null; i++) owner = owner.NJDJHGDMCIJ();
			Fight fight = FightInterpolation.IsFightActive ? Fight.GetCurrentFight() : null;
			bool player = fight == null || fight.GetPlayerModel() == owner;
			bool opponent = fight != null && fight.GetEnemyModel() == owner;
			return fighters == ModFxFighters.Player ? player : opponent;
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
	}
}

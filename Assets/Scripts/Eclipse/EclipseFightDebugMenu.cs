using System.Collections.Generic;
using Nekki.SF2.GUI.Fight;
using UnityEngine;
using UnityEngine.Rendering;

namespace Eclipse.Diagnostics
{
	/// <summary>
	/// Lightweight, scene-independent fight diagnostics. F1 opens the menu and
	/// F8 invokes the same opponent-defeat path as the menu action. F7 runs
	/// complete fights quickly while preserving their normal progression path.
	/// </summary>
	public sealed class EclipseFightDebugMenu : MonoBehaviour
	{
		private const float PanelWidth = 300f;
		private const float ProgressionSprintTimeScale = 8f;
		private const float AutoDefeatRetrySeconds = 0.2f;
		private static readonly Color HitboxColor = new Color(1f, 0.22f, 0.18f, 0.92f);
		private static readonly Color HurtboxColor = new Color(0.15f, 0.95f, 0.62f, 0.72f);

		private readonly Dictionary<ModelEdge, LineRenderer> _hitboxLines = new Dictionary<ModelEdge, LineRenderer>();
		private readonly Dictionary<ModelEdge, LineRenderer> _hurtboxLines = new Dictionary<ModelEdge, LineRenderer>();
		private readonly HashSet<ModelEdge> _visibleHitboxes = new HashSet<ModelEdge>();
		private readonly HashSet<ModelEdge> _visibleHurtboxes = new HashSet<ModelEdge>();

		private bool _menuOpen;
		private bool _showCollisionShapes;
		private bool _showHitPoints;
		private bool _progressionSprint;
		private bool _ownsTimeScale;
		private float _timeScaleBeforeSprint = 1f;
		private float _nextAutoDefeatAt;
		private EndFightScreen _continuedResultScreen;
		private Material _lineMaterial;
		private Fight _renderedFight;
		private string _actionStatus = string.Empty;
		private float _actionStatusUntil;
		private GUIStyle _titleStyle;
		private GUIStyle _labelStyle;
		private GUIStyle _smallStyle;
		private GUIStyle _healthStyle;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void EnsureInstance()
		{
			if (FindObjectOfType<EclipseFightDebugMenu>() != null)
			{
				return;
			}

			GameObject host = new GameObject("Eclipse Fight Debug Menu");
			DontDestroyOnLoad(host);
			host.AddComponent<EclipseFightDebugMenu>();
		}

		private void Update()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
			{
				_menuOpen = !_menuOpen;
			}

			if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && _menuOpen)
			{
				_menuOpen = false;
			}

			if (UnityEngine.Input.GetKeyDown(KeyCode.F7))
			{
				SetProgressionSprint(!_progressionSprint);
			}

			Fight fight = GetActiveFight();
			if (UnityEngine.Input.GetKeyDown(KeyCode.F8) && fight != null)
			{
				DefeatOpponent();
			}

			UpdateProgressionSprint(fight);
		}

		private void LateUpdate()
		{
			Fight fight = GetActiveFight();
			if (fight != _renderedFight)
			{
				ClearLines();
				_renderedFight = fight;
			}

			if (!_showCollisionShapes || fight == null)
			{
				SetAllLinesVisible(false);
				return;
			}

			RefreshCollisionLines(fight);
		}

		private void OnGUI()
		{
			Fight fight = GetActiveFight();
			if (fight == null)
			{
				return;
			}

			EnsureStyles();
			if (_showHitPoints)
			{
				DrawHitPointLabels(fight);
			}

			Matrix4x4 oldMatrix = GUI.matrix;
			float scale = Mathf.Clamp(Screen.height / 900f, 0.85f, 1.35f);
			GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));

			if (!_menuOpen)
			{
				if (GUI.Button(new Rect(12f, 12f, 116f, 34f), "DEBUG   [F1]"))
				{
					_menuOpen = true;
				}
				GUI.matrix = oldMatrix;
				return;
			}

			DrawMenu(fight);
			GUI.matrix = oldMatrix;
		}

		private void DrawMenu(Fight fight)
		{
			Rect panel = new Rect(12f, 12f, PanelWidth, 304f);
			GUI.DrawTexture(panel, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0f,
				new Color(0.035f, 0.045f, 0.06f, 0.96f), 8f, 12f);
			GUI.Label(new Rect(28f, 24f, 210f, 28f), "FIGHT DEBUG", _titleStyle);
			if (GUI.Button(new Rect(266f, 22f, 30f, 28f), "X"))
			{
				_menuOpen = false;
			}

			GUI.Label(new Rect(28f, 57f, 250f, 20f), "Combat visualization", _smallStyle);
			_showCollisionShapes = DrawToggle(new Rect(28f, 80f, 256f, 38f),
				"Hitboxes + hurtboxes", _showCollisionShapes);
			_showHitPoints = DrawToggle(new Rect(28f, 124f, 256f, 38f),
				"Hit point labels", _showHitPoints);
			bool progressionSprint = DrawToggle(new Rect(28f, 168f, 256f, 38f),
				"Progression sprint [F7]", _progressionSprint);
			if (progressionSprint != _progressionSprint)
			{
				SetProgressionSprint(progressionSprint);
			}

			Color oldColor = GUI.backgroundColor;
			GUI.backgroundColor = new Color(0.95f, 0.28f, 0.22f, 1f);
			if (GUI.Button(new Rect(28f, 218f, 256f, 42f), "DEFEAT OPPONENT   [F8]"))
			{
				DefeatOpponent();
			}
			GUI.backgroundColor = oldColor;

			string footer = Time.unscaledTime < _actionStatusUntil
				? _actionStatus
				: "Sprint: auto-win + 8x + skip results";
			GUI.Label(new Rect(28f, 268f, 256f, 18f), footer, _smallStyle);
		}

		private bool DrawToggle(Rect rect, string text, bool value)
		{
			Color oldColor = GUI.backgroundColor;
			GUI.backgroundColor = value
				? new Color(0.16f, 0.68f, 0.52f, 1f)
				: new Color(0.28f, 0.31f, 0.37f, 1f);
			if (GUI.Button(rect, (value ? "ON     " : "OFF    ") + text))
			{
				value = !value;
			}
			GUI.backgroundColor = oldColor;
			return value;
		}

		private void DefeatOpponent()
		{
			Fight fight = GetActiveFight();
			bool defeated = fight != null && fight.DebugDefeatOpponent();
			_actionStatus = defeated ? "Opponent defeat triggered." : "No vulnerable opponent is active.";
			_actionStatusUntil = Time.unscaledTime + 2.5f;
		}

		private void SetProgressionSprint(bool enabled)
		{
			_progressionSprint = enabled;
			_nextAutoDefeatAt = 0f;
			_continuedResultScreen = null;
			_actionStatus = enabled
				? "Sprint enabled; normal progression is preserved."
				: "Progression sprint disabled.";
			_actionStatusUntil = Time.unscaledTime + 2.5f;
			if (!enabled)
			{
				RestoreTimeScale();
			}
		}

		private void UpdateProgressionSprint(Fight fight)
		{
			if (!_progressionSprint)
			{
				return;
			}

			EndFightScreen resultScreen = FindObjectOfType<EndFightScreen>();
			if (resultScreen != null && resultScreen != _continuedResultScreen)
			{
				_continuedResultScreen = resultScreen;
				resultScreen.OnBackKeyClicked(null);
				_actionStatus = "Fight recorded; returning to map.";
				_actionStatusUntil = Time.unscaledTime + 2.5f;
				return;
			}

			if (fight == null)
			{
				RestoreTimeScale();
				_continuedResultScreen = null;
				return;
			}

			ApplySprintTimeScale();
			if (Time.unscaledTime < _nextAutoDefeatAt)
			{
				return;
			}

			_nextAutoDefeatAt = Time.unscaledTime + AutoDefeatRetrySeconds;
			if (fight.DebugDefeatOpponent())
			{
				_actionStatus = "Round won automatically.";
				_actionStatusUntil = Time.unscaledTime + 1f;
			}
		}

		private void ApplySprintTimeScale()
		{
			if (!_ownsTimeScale)
			{
				_timeScaleBeforeSprint = Time.timeScale;
				_ownsTimeScale = true;
			}
			Time.timeScale = ProgressionSprintTimeScale;
		}

		private void RestoreTimeScale()
		{
			if (!_ownsTimeScale)
			{
				return;
			}
			Time.timeScale = _timeScaleBeforeSprint;
			_ownsTimeScale = false;
		}

		private void RefreshCollisionLines(Fight fight)
		{
			_visibleHitboxes.Clear();
			_visibleHurtboxes.Clear();

			for (int i = 0; i < fight.LNDLFINJHDB.Count; i++)
			{
				Model model = fight.LNDLFINJHDB[i];
				if (!IsFighterModel(model))
				{
					continue;
				}

				ModelObject modelObject = model.KFDGGLKBKEP;
				List<ModelEdge> hurtboxes = modelObject.OOFMOAHJEJF;
				for (int edgeIndex = 0; edgeIndex < hurtboxes.Count; edgeIndex++)
				{
					ModelEdge edge = hurtboxes[edgeIndex];
					_visibleHurtboxes.Add(edge);
					UpdateLine(_hurtboxLines, edge, model.ICDCIANNAAI.transform, HurtboxColor, -1.4f);
				}

				ModelAnimation animation = model.DAPLCAPAPDI;
				List<ModelEdge> hitboxes = animation == null ? null : animation.CPNOFKIMMCK();
				if (hitboxes == null)
				{
					continue;
				}
				for (int edgeIndex = 0; edgeIndex < hitboxes.Count; edgeIndex++)
				{
					ModelEdge edge = hitboxes[edgeIndex];
					_visibleHitboxes.Add(edge);
					UpdateLine(_hitboxLines, edge, model.ICDCIANNAAI.transform, HitboxColor, -1.6f);
				}
			}

			ApplyVisibility(_hurtboxLines, _visibleHurtboxes);
			ApplyVisibility(_hitboxLines, _visibleHitboxes);
		}

		private void UpdateLine(Dictionary<ModelEdge, LineRenderer> cache, ModelEdge edge,
			Transform parent, Color color, float depth)
		{
			LineRenderer line;
			if (!cache.TryGetValue(edge, out line) || line == null)
			{
				GameObject lineObject = new GameObject("Debug " + edge.get_Name());
				lineObject.transform.SetParent(parent, false);
				line = lineObject.AddComponent<LineRenderer>();
				line.sharedMaterial = GetLineMaterial();
				line.useWorldSpace = false;
				line.positionCount = 2;
				line.numCapVertices = 8;
				line.alignment = LineAlignment.TransformZ;
				line.shadowCastingMode = ShadowCastingMode.Off;
				line.receiveShadows = false;
				line.sortingOrder = 32760;
				cache[edge] = line;
			}

			if (line.transform.parent != parent)
			{
				line.transform.SetParent(parent, false);
			}

			Vector3f start = edge.CCMHKFHDFNM;
			Vector3f end = edge.MBLICPBLEFC;
			line.SetPosition(0, new Vector3(start.GILCBJJPKBK(), start.OBIMBNIBEFG(), depth));
			line.SetPosition(1, new Vector3(end.GILCBJJPKBK(), end.OBIMBNIBEFG(), depth));
			float width = Mathf.Max(0.04f, edge.AGODBAOHPJC * 2f);
			line.startWidth = width;
			line.endWidth = width;
			line.startColor = color;
			line.endColor = color;
			line.enabled = true;
		}

		private void DrawHitPointLabels(Fight fight)
		{
			UnityEngine.Camera camera = UnityEngine.Camera.main;
			if (camera == null)
			{
				return;
			}

			for (int i = 0; i < fight.LNDLFINJHDB.Count; i++)
			{
				Model model = fight.LNDLFINJHDB[i];
				if (!IsFighterModel(model))
				{
					continue;
				}

				Vector3f center = model.BPPINEHFOBB;
				Vector3 localCenter = new Vector3(center.GILCBJJPKBK(), center.OBIMBNIBEFG(), center.KMFEKANLCFO());
				Vector3 screen = camera.WorldToScreenPoint(model.ICDCIANNAAI.transform.TransformPoint(localCenter));
				if (screen.z < 0f)
				{
					continue;
				}

				ModelParameters parameters = model.KMMJCHDKBDO;
				float current = parameters.RemainingHealthInDamageUnits;
				float maximum = parameters.CIDCNCDFONA * parameters.HealthBarCount;
				string role = model.IsPlayer ? "PLAYER" : "OPPONENT";
				string text = role + "  " + current.ToString("0.##") + " / " + maximum.ToString("0.##") + " HP";
				Vector2 size = _healthStyle.CalcSize(new GUIContent(text));
				Rect labelRect = new Rect(screen.x - size.x * 0.5f - 8f,
					Screen.height - screen.y - 58f, size.x + 16f, 26f);
				GUI.DrawTexture(labelRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0f,
					new Color(0.02f, 0.025f, 0.035f, 0.88f), 5f, 8f);
				GUI.Label(labelRect, text, _healthStyle);
			}
		}

		private static Fight GetActiveFight()
		{
			Fight fight = Fight.OHNKFOHIAKG();
			return fight != null && fight.LNDLFINJHDB != null && fight.LNDLFINJHDB.Count > 0 ? fight : null;
		}

		private static bool IsFighterModel(Model model)
		{
			return model != null && model.KMMJCHDKBDO != null && model.ICDCIANNAAI != null &&
				model.KFDGGLKBKEP != null && model.HIPJNBEFGHN();
		}

		private Material GetLineMaterial()
		{
			if (_lineMaterial == null)
			{
				Shader shader = Shader.Find("Sprites/Default");
				_lineMaterial = new Material(shader);
				_lineMaterial.name = "Eclipse Fight Debug Overlay";
				_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return _lineMaterial;
		}

		private static void ApplyVisibility(Dictionary<ModelEdge, LineRenderer> cache, HashSet<ModelEdge> visible)
		{
			foreach (KeyValuePair<ModelEdge, LineRenderer> pair in cache)
			{
				if (pair.Value != null)
				{
					pair.Value.enabled = visible.Contains(pair.Key);
				}
			}
		}

		private void SetAllLinesVisible(bool visible)
		{
			SetLinesVisible(_hitboxLines, visible);
			SetLinesVisible(_hurtboxLines, visible);
		}

		private static void SetLinesVisible(Dictionary<ModelEdge, LineRenderer> cache, bool visible)
		{
			foreach (LineRenderer line in cache.Values)
			{
				if (line != null)
				{
					line.enabled = visible;
				}
			}
		}

		private void ClearLines()
		{
			DestroyLines(_hitboxLines);
			DestroyLines(_hurtboxLines);
			_visibleHitboxes.Clear();
			_visibleHurtboxes.Clear();
		}

		private static void DestroyLines(Dictionary<ModelEdge, LineRenderer> cache)
		{
			foreach (LineRenderer line in cache.Values)
			{
				if (line != null)
				{
					Destroy(line.gameObject);
				}
			}
			cache.Clear();
		}

		private void EnsureStyles()
		{
			if (_titleStyle != null)
			{
				return;
			}

			_titleStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 18,
				fontStyle = FontStyle.Bold,
				normal = { textColor = Color.white }
			};
			_labelStyle = new GUIStyle(GUI.skin.label)
			{
				fontSize = 13,
				normal = { textColor = new Color(0.9f, 0.93f, 0.98f) }
			};
			_smallStyle = new GUIStyle(_labelStyle)
			{
				fontSize = 11,
				normal = { textColor = new Color(0.65f, 0.7f, 0.78f) }
			};
			_healthStyle = new GUIStyle(_labelStyle)
			{
				fontSize = 12,
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleCenter,
				normal = { textColor = Color.white }
			};
		}

		private void OnDestroy()
		{
			RestoreTimeScale();
			ClearLines();
			if (_lineMaterial != null)
			{
				Destroy(_lineMaterial);
			}
		}
	}
}

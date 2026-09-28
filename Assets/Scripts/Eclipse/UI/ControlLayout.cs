using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.UI
{
    // A player-arranged layout for the on-screen fight controls (joystick and buttons), edited
    // in ControlLayoutEditor and applied by GameController every frame while it is enabled.
    //
    // Coordinates live in the fight canvas (reference 2048 x 1536, matched on height, so it is
    // always 1536 units tall). Positions are the controls' centres, normalised to a "frame":
    // the centred 16:9 region of the canvas, or the whole canvas when the screen is narrower.
    // This keeps a layout sensible on 4:3, 16:9 and 21:9 alike. Sizes are multipliers of each
    // control's authored size, on top of the Small/Large control size setting.
    public static class ControlLayout
    {
        public const float CanvasHeight = 1536f;
        private const float FrameAspect = 16f / 9f;
        private const string Preference = "Eclipse.ControlLayout.v1";

        public sealed class Control
        {
            public readonly string Id, Name, Node, Sprite;
            public readonly float BaseSize;
            // Authored centre at 16:9 with Large controls, split into the container origin and
            // the offset that the Small/Large container scale multiplies.
            internal readonly Vector2 Origin, Offset;
            internal readonly bool RightSide;

            internal Control(string id, string name, string node, string sprite, float size, Vector2 origin, Vector2 offset, bool right)
            {
                Id = id; Name = name; Node = node; Sprite = sprite; BaseSize = size;
                Origin = origin; Offset = offset; RightSide = right;
            }
        }

        // Values from Assets/src/GUI/Scenes/Fight/Fight.unity (GameController hierarchy).
        public static readonly Control[] Controls =
        {
            new Control("stick", "Joystick", "Stick", "FightButtons.JoystickContainer_norm", 620f, new Vector2(29f, 40f), new Vector2(361f, 350f), false),
            new Control("raid", "Raid charge", "BtnRaidCharge", "FightButtons.btn_charge_normal", 294f, new Vector2(29f, 40f), new Vector2(147f, 869f), false),
            new Control("magic", "Magic", "BtnMagic", "FightButtons.btn_magic_normal", 290f, new Vector2(0f, -.5f), new Vector2(-306f, 758f), true),
            new Control("ranged", "Ranged", "BtnRanged", "FightButtons.btn_throw_normal", 275f, new Vector2(0f, -.5f), new Vector2(-539.3f, 524.9f), true),
            new Control("punch", "Punch", "BtnPunch", "FightButtons.btn_punch_normal", 275f, new Vector2(0f, -.5f), new Vector2(-213.8f, 449.5f), true),
            new Control("kick", "Kick", "BtnKick", "FightButtons.btn_kick_normal", 275f, new Vector2(0f, -.5f), new Vector2(-446f, 215f), true),
        };

        [Serializable]
        public sealed class Placement
        {
            public string id;
            public float x, y;        // centre, normalised to the frame (0..1, y from the bottom)
            public float size = 1f;   // multiplier of the authored size
        }

        [Serializable]
        public sealed class Layout
        {
            public bool enabled;
            public float opacity = 1f;
            public bool snap = true;
            public List<Placement> controls = new List<Placement>();

            public Placement Get(string id)
            {
                foreach (var placement in controls) if (placement.id == id) return placement;
                return null;
            }

            public Layout Clone() { return JsonUtility.FromJson<Layout>(JsonUtility.ToJson(this)); }
        }

        private static Layout current;
        private static readonly Dictionary<Transform, Vector3> AuthoredScales = new Dictionary<Transform, Vector3>();
        private static readonly HashSet<CanvasGroup> Faded = new HashSet<CanvasGroup>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSession() { current = null; AuthoredScales.Clear(); Faded.Clear(); Nodes.Clear(); }

        public static Layout Current
        {
            get
            {
                if (current != null) return current;
                try
                {
                    string json = PlayerPrefs.GetString(Preference, string.Empty);
                    if (!string.IsNullOrEmpty(json)) current = JsonUtility.FromJson<Layout>(json);
                }
                catch (Exception error) { Debug.LogWarning("[Controls] Ignoring an unreadable touch layout: " + error.Message); }
                if (current == null) current = Default();
                Complete(current);
                return current;
            }
        }

        public static void Save(Layout layout)
        {
            current = layout.Clone();
            Complete(current);
            PlayerPrefs.SetString(Preference, JsonUtility.ToJson(current));
            PlayerPrefs.Save();
        }

        public static float ContainerScale => GraphicsController.LargeControlsEnabled() ? 1f : 25f / 33f;

        // The authored arrangement, as it looks at 16:9 with the current control size.
        public static Layout Default()
        {
            var layout = new Layout { enabled = false, opacity = 1f, snap = true };
            float s = ContainerScale, width = CanvasHeight * FrameAspect;
            foreach (var control in Controls)
            {
                Vector2 centre = control.Origin + control.Offset * s;
                if (control.RightSide) centre.x += width;
                layout.controls.Add(new Placement { id = control.Id, x = centre.x / width, y = centre.y / CanvasHeight, size = 1f });
            }
            return layout;
        }

        // Older or hand-edited data: add missing controls at their default spot, clamp values.
        private static void Complete(Layout layout)
        {
            if (layout.controls == null) layout.controls = new List<Placement>();
            var defaults = Default();
            foreach (var control in Controls)
            {
                var placement = layout.Get(control.Id);
                if (placement == null) layout.controls.Add(placement = defaults.Get(control.Id));
                placement.x = Mathf.Clamp01(placement.x);
                placement.y = Mathf.Clamp01(placement.y);
                placement.size = Mathf.Clamp(placement.size, MinSize, MaxSize);
            }
            layout.opacity = Mathf.Clamp(layout.opacity, MinOpacity, 1f);
        }

        public const float MinSize = .5f, MaxSize = 1.6f, MinOpacity = .2f;

        // The frame inside a canvas of the given size: x offset and width.
        public static void Frame(Vector2 canvas, out float left, out float width)
        {
            width = Mathf.Min(canvas.x, canvas.y * FrameAspect);
            left = (canvas.x - width) * .5f;
        }

        public static Vector2 ToCanvas(Placement placement, Vector2 canvas)
        {
            Frame(canvas, out float left, out float width);
            return new Vector2(left + placement.x * width, placement.y * canvas.y);
        }

        public static void FromCanvas(Placement placement, Vector2 point, Vector2 canvas)
        {
            Frame(canvas, out float left, out float width);
            placement.x = Mathf.Clamp01((point.x - left) / width);
            placement.y = Mathf.Clamp01(point.y / canvas.y);
        }

        // Called by GameController after its own layout pass. Places each control's centre and
        // scale from the saved layout; a disabled layout restores the authored scales.
        public static void Apply(RectTransform root)
        {
            if (root == null) return;
            var layout = Current;
            Vector2 canvas = root.rect.size;
            if (canvas.x <= 0f || canvas.y <= 0f) return;
            foreach (var control in Controls)
            {
                var node = Find(root, control.Node);
                if (node == null) continue;
                if (!AuthoredScales.ContainsKey(node)) AuthoredScales[node] = node.localScale;
                var group = node.GetComponent<CanvasGroup>();
                if (!layout.enabled)
                {
                    if (node.localScale != AuthoredScales[node]) node.localScale = AuthoredScales[node];
                    if (group != null && Faded.Remove(group)) group.alpha = 1f;
                    continue;
                }
                var placement = layout.Get(control.Id);
                Vector2 point = ToCanvas(placement, canvas);
                Vector3 local = new Vector3(point.x - canvas.x * root.pivot.x, point.y - canvas.y * root.pivot.y, 0f);
                Vector3 world = root.TransformPoint(local);
                // The node's pivot may not be its centre (stretched containers); move by the
                // difference between its current visual centre and the target.
                var rect = node as RectTransform;
                Vector3 centre = rect != null ? rect.TransformPoint(rect.rect.center) : node.position;
                node.position += world - centre;
                Vector3 scale = AuthoredScales[node] * placement.size;
                if (node.localScale != scale) node.localScale = scale;
                if (group == null) group = node.gameObject.AddComponent<CanvasGroup>();
                group.alpha = layout.opacity;
                Faded.Add(group);
            }
        }

        private static readonly Dictionary<RectTransform, Dictionary<string, Transform>> Nodes = new Dictionary<RectTransform, Dictionary<string, Transform>>();

        private static Transform Find(RectTransform root, string name)
        {
            if (!Nodes.TryGetValue(root, out var map))
            {
                if (Nodes.Count > 8) Nodes.Clear();
                Nodes[root] = map = new Dictionary<string, Transform>();
            }
            if (map.TryGetValue(name, out var found) && found != null) return found;
            foreach (var child in root.GetComponentsInChildren<Transform>(true))
                if (child.name == name) { map[name] = child; return child; }
            return null;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    public enum ModUiCloseReason { Script, Back, Scene, Error, Destroyed, Shutdown }

    public interface IModUiScriptContext
    {
        ModUiScope UiScope { get; }
    }

    // Preserve ordinary input delivery, but release active controls on UI capture
    // and require a physical release before captured controls can press again.
    public sealed class ModUiControlGate<T>
    {
        private readonly List<T> active = new List<T>();
        private readonly HashSet<T> suppressed = new HashSet<T>();
        private bool captured;
        public T[] SetCaptured(bool value)
        {
            if (captured == value) return Array.Empty<T>();
            captured = value;
            if (!value) return Array.Empty<T>();
            var releases = active.ToArray();
            suppressed.UnionWith(active); active.Clear();
            return releases;
        }
        public bool Press(T control)
        {
            if (captured) { suppressed.Add(control); return false; }
            if (suppressed.Contains(control)) return false;
            if (!active.Contains(control)) active.Add(control);
            return true;
        }
        public bool Release(T control)
        {
            active.Remove(control);
            return !suppressed.Remove(control);
        }
    }

    // Engine-independent UI ownership and state, consumed by the Lua binding,
    // Unity renderer, input coordinator and script-context teardown.
    public enum ModUiKind { Stack, Row, Column, Scroll, Text, Button, Progress, Toggle, Slider }
    public enum ModUiMount { Menu, Modal, CombatHud }

    public sealed class ModUiPlacement
    {
        public string Anchor { get; }
        public double X { get; }
        public double Y { get; }
        public double AnchorX { get; }
        public double AnchorY { get; }
        public ModUiPlacement(string anchor = "center", double x = 0, double y = 0)
        {
            switch (anchor)
            {
                case "top_left": AnchorX=0; AnchorY=1; break;
                case "top": AnchorX=.5; AnchorY=1; break;
                case "top_right": AnchorX=1; AnchorY=1; break;
                case "left": AnchorX=0; AnchorY=.5; break;
                case "center": AnchorX=.5; AnchorY=.5; break;
                case "right": AnchorX=1; AnchorY=.5; break;
                case "bottom_left": AnchorX=0; AnchorY=0; break;
                case "bottom": AnchorX=.5; AnchorY=0; break;
                case "bottom_right": AnchorX=1; AnchorY=0; break;
                default: throw new ArgumentException("Unsupported UI anchor.");
            }
            ModUiNode.ValidateNumber(x,-8192,8192,nameof(x));
            ModUiNode.ValidateNumber(y,-8192,8192,nameof(y));
            Anchor=anchor; X=x; Y=y;
        }
    }

    public sealed class ModUiColor
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }
        public ModUiColor(string hex)
        {
            if (hex == null || (hex.Length != 7 && hex.Length != 9) || hex[0] != '#')
                throw new ArgumentException("UI colors require #RRGGBB or #RRGGBBAA.");
            for (int i = 1; i < hex.Length; i++)
                if (!Uri.IsHexDigit(hex[i])) throw new ArgumentException("Invalid UI color.");
            R = Convert.ToByte(hex.Substring(1, 2), 16);
            G = Convert.ToByte(hex.Substring(3, 2), 16);
            B = Convert.ToByte(hex.Substring(5, 2), 16);
            A = hex.Length == 9 ? Convert.ToByte(hex.Substring(7, 2), 16) : (byte)255;
        }
    }

    public sealed class ModUiStyle
    {
        public ModUiColor TextColor { get; }
        public ModUiColor BackgroundColor { get; }
        public ModUiColor FillColor { get; }
        public int? FontSize { get; }
        public string TextAlign { get; }
        public ModUiStyle(string textColor = null, string backgroundColor = null, string fillColor = null,
            int? fontSize = null, string textAlign = null)
        {
            TextColor = textColor == null ? null : new ModUiColor(textColor);
            BackgroundColor = backgroundColor == null ? null : new ModUiColor(backgroundColor);
            FillColor = fillColor == null ? null : new ModUiColor(fillColor);
            if (fontSize.HasValue && (fontSize < 8 || fontSize > 128))
                throw new ArgumentOutOfRangeException(nameof(fontSize), "UI font size must be 8..128.");
            if (textAlign != null && textAlign != "left" && textAlign != "center" && textAlign != "right")
                throw new ArgumentException("UI text alignment must be left, center or right.");
            FontSize = fontSize; TextAlign = textAlign;
        }
        internal void ValidateFor(ModUiKind kind)
        {
            if (kind != ModUiKind.Text && kind != ModUiKind.Button && kind != ModUiKind.Toggle && (TextColor != null || FontSize.HasValue || TextAlign != null))
                throw new ArgumentException("Only text, buttons and toggles accept text styling.");
            if (kind != ModUiKind.Progress && kind != ModUiKind.Slider && FillColor != null)
                throw new ArgumentException("Only progress widgets and sliders accept fill color.");
            if (kind == ModUiKind.Text && BackgroundColor != null)
                throw new ArgumentException("Use a container for a text background.");
        }
    }

    public sealed class ModUiNode
    {
        public string Id { get; }
        public ModUiKind Kind { get; }
        public double Width { get; }
        public double Height { get; }
        public double Gap { get; }
        public string Text { get; }
        public double Value { get; }
        public bool Visible { get; }
        public bool Enabled { get; }
        public IReadOnlyList<ModUiNode> Children { get; }
        public ModUiStyle Style { get; }

        public ModUiNode(string id, ModUiKind kind, double width, double height,
            string text = "", double value = 0, bool visible = true, bool enabled = true,
            double gap = 0, IEnumerable<ModUiNode> children = null, ModUiStyle style = null)
        {
            ValidateId(id);
            if (!Enum.IsDefined(typeof(ModUiKind), kind)) throw new ArgumentOutOfRangeException(nameof(kind));
            ValidateNumber(width, 0, 8192, nameof(width));
            ValidateNumber(height, 0, 8192, nameof(height));
            ValidateNumber(gap, 0, 1024, nameof(gap));
            ValidateText(text);
            ValidateNumber(value, 0, 1, nameof(value));
            var copy = new List<ModUiNode>();
            if (children != null)
                foreach (var child in children)
                {
                    if (child == null) throw new ArgumentException("UI children cannot contain null.");
                    if (copy.Count == 256) throw new ArgumentException("A UI tree permits at most 256 nodes.");
                    copy.Add(child);
                }
            bool container = kind == ModUiKind.Stack || kind == ModUiKind.Row || kind == ModUiKind.Column || kind == ModUiKind.Scroll;
            if (!container && copy.Count != 0) throw new ArgumentException("Leaf widgets cannot have children.");
            if (kind == ModUiKind.Scroll && copy.Count != 1) throw new ArgumentException("Scroll requires one content child.");
            if (kind != ModUiKind.Text && kind != ModUiKind.Button && kind != ModUiKind.Toggle && text.Length != 0)
                throw new ArgumentException("Only text and button widgets have text.");
            if (kind != ModUiKind.Progress && kind != ModUiKind.Slider && kind != ModUiKind.Toggle && value != 0)
                throw new ArgumentException("Only progress, slider and toggle widgets have a value.");
            if (kind == ModUiKind.Toggle && value != 0 && value != 1) throw new ArgumentException("Toggle values are zero or one.");
            if (kind != ModUiKind.Row && kind != ModUiKind.Column && gap != 0)
                throw new ArgumentException("Only rows and columns have a gap.");
            Id = id; Kind = kind; Width = width; Height = height; Gap = gap;
            Text = text; Value = value; Visible = visible; Enabled = enabled;
            Children = copy.AsReadOnly();
            Style = style ?? new ModUiStyle();
            Style.ValidateFor(kind);
        }

        internal static void ValidateId(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length > 64) throw new ArgumentException("UI IDs require 1..64 ASCII letters, digits, underscores or hyphens.");
            foreach (char c in id)
                if (!(c >= 'a' && c <= 'z') && !(c >= 'A' && c <= 'Z') &&
                    !(c >= '0' && c <= '9') && c != '_' && c != '-')
                    throw new ArgumentException("Invalid UI ID: " + id);
        }

        internal static void ValidateText(string text)
        {
            if (text == null || text.Length > 8192) throw new ArgumentException("UI text permits at most 8192 UTF-16 code units.");
        }

        internal static void ValidateNumber(double value, double min, double max, string name)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value < min || value > max)
                throw new ArgumentOutOfRangeException(name);
        }
    }

    // Returned snapshots never expose the mutable widget storage.
    public sealed class ModUiWidgetState
    {
        public string Text { get; }
        public double Value { get; }
        public bool Visible { get; }
        public bool Enabled { get; }
        internal ModUiWidgetState(string text, double value, bool visible, bool enabled)
        { Text = text; Value = value; Visible = visible; Enabled = enabled; }
    }

    public sealed class ModUiScope : IDisposable
    {
        private readonly Dictionary<string, ModUiSurface> surfaces = new Dictionary<string, ModUiSurface>(StringComparer.Ordinal);
        private readonly Action<Exception> report;
        private bool closed;
        public ModId Owner { get; }
        public bool IsClosed => closed;
        public int Count => surfaces.Count;

        public ModUiScope(ModId owner, Action<Exception> report = null)
        { if (string.IsNullOrEmpty(owner.Value)) throw new ArgumentException("UI scope requires a mod owner."); Owner = owner; this.report = report; }

        public ModUiSurface Open(string id, ModUiMount mount, ModUiNode root, Action<string> onClick = null, ModUiPlacement placement = null,
            Action<ModUiCloseReason> onClose = null, Action<string, double> onChange = null)
        {
            if (closed) throw new ObjectDisposedException(nameof(ModUiScope));
            ModUiNode.ValidateId(id);
            if (!Enum.IsDefined(typeof(ModUiMount), mount)) throw new ArgumentOutOfRangeException(nameof(mount));
            if (surfaces.ContainsKey(id)) throw new InvalidOperationException("UI surface is already open: " + id);
            if (surfaces.Count >= 8) throw new InvalidOperationException("A scope permits at most eight open surfaces.");
            var surface = new ModUiSurface(this, id, mount, root, onClick, placement, onClose, onChange);
            surfaces.Add(id, surface);
            return surface;
        }

        internal void Remove(ModUiSurface surface) { surfaces.Remove(surface.Id); }
        internal void Report(Exception error) { try { report?.Invoke(error); } catch { /* Diagnostics must not prevent cleanup. */ } }

        public void Dispose()
        {
            if (closed) return;
            closed = true;
            foreach (var surface in new List<ModUiSurface>(surfaces.Values)) surface.Close(ModUiCloseReason.Shutdown);
            surfaces.Clear();
        }
    }

    public sealed class ModUiSurface : IDisposable
    {
        private sealed class Widget
        {
            public ModUiNode Node;
            public Widget Parent;
            public ModUiWidgetState State;
        }
        private readonly ModUiScope scope;
        private readonly Dictionary<string, Widget> widgets = new Dictionary<string, Widget>(StringComparer.Ordinal);
        private Action<string> click;
        private Action<string, double> change;
        private Action<ModUiCloseReason> close;
        private bool dispatching;
        private bool inputAllowed = true;
        internal ModUiLayerStack LayerOwner { get; set; }
        public bool IsMounted => LayerOwner != null;
        public ModId Owner => scope.Owner;
        public string Id { get; }
        public ModUiMount Mount { get; }
        public ModUiNode Root { get; }
        public ModUiPlacement Placement { get; }
        public bool IsClosed { get; private set; }
        public int WidgetCount => widgets.Count;
        public event Action<string> Changed;
        public event Action Closed;

        internal ModUiSurface(ModUiScope scope, string id, ModUiMount mount, ModUiNode root, Action<string> onClick, ModUiPlacement placement,
            Action<ModUiCloseReason> onClose, Action<string, double> onChange)
        {
            this.scope = scope; Id = id; Mount = mount;
            Root = root ?? throw new ArgumentNullException(nameof(root));
            Placement = placement ?? new ModUiPlacement();
            if (root.Width == 0 || root.Height == 0) throw new ArgumentException("UI root needs positive dimensions.");
            Index(root, null, 1);
            click = onClick;
            close = onClose;
            change = onChange;
        }

        private void Index(ModUiNode node, Widget parent, int depth)
        {
            if (depth > 16 || widgets.Count == 256) throw new ArgumentException("UI trees permit 256 nodes and depth 16.");
            if (widgets.ContainsKey(node.Id)) throw new ArgumentException("Duplicate UI widget ID: " + node.Id);
            var widget = new Widget { Node = node, Parent = parent,
                State = new ModUiWidgetState(node.Text, node.Value, node.Visible, node.Enabled) };
            widgets.Add(node.Id, widget);
            foreach (var child in node.Children) Index(child, widget, depth + 1);
        }

        private Widget Get(string id)
        {
            if (IsClosed) throw new ObjectDisposedException(nameof(ModUiSurface));
            if (id == null || !widgets.TryGetValue(id, out var widget)) throw new ArgumentException("Unknown UI widget: " + id);
            return widget;
        }

        public ModUiWidgetState Read(string id) => Get(id).State;

        // Coordinator gate, independent of the mod-authored enabled/visible values.
        public void SetInputAllowed(bool allowed) { inputAllowed = allowed && !IsClosed; }

        public void SetText(string id, string text)
        {
            var widget = Get(id);
            if (widget.Node.Kind != ModUiKind.Text && widget.Node.Kind != ModUiKind.Button && widget.Node.Kind != ModUiKind.Toggle)
                throw new InvalidOperationException("This widget has no text.");
            ModUiNode.ValidateText(text);
            Update(widget, text, widget.State.Value, widget.State.Visible, widget.State.Enabled);
        }

        public void SetValue(string id, double value)
        {
            var widget = Get(id);
            if (widget.Node.Kind != ModUiKind.Progress && widget.Node.Kind != ModUiKind.Slider) throw new InvalidOperationException("This widget has no numeric value.");
            ModUiNode.ValidateNumber(value, 0, 1, nameof(value));
            Update(widget, widget.State.Text, value, widget.State.Visible, widget.State.Enabled);
        }

        public void SetVisible(string id, bool visible)
        { var w = Get(id); Update(w, w.State.Text, w.State.Value, visible, w.State.Enabled); }
        public void SetEnabled(string id, bool enabled)
        { var w = Get(id); Update(w, w.State.Text, w.State.Value, w.State.Visible, enabled); }

        public void SetChecked(string id, bool value)
        {
            var w = Get(id);
            if (w.Node.Kind != ModUiKind.Toggle) throw new InvalidOperationException("This widget is not a toggle.");
            Update(w, w.State.Text, value ? 1 : 0, w.State.Visible, w.State.Enabled);
        }

        public bool CanInteract(string id)
        {
            if (IsClosed || !inputAllowed || dispatching || id == null || !widgets.TryGetValue(id, out var w)) return false;
            if (w.Node.Kind != ModUiKind.Button && w.Node.Kind != ModUiKind.Toggle && w.Node.Kind != ModUiKind.Slider) return false;
            for (var ancestor = w; ancestor != null; ancestor = ancestor.Parent)
                if (!ancestor.State.Visible || !ancestor.State.Enabled) return false;
            return true;
        }

        // User changes commit before notification. Script setters never echo callbacks.
        public bool TryChange(string id, double value)
        {
            if (!CanInteract(id)) return false;
            var w = Get(id);
            if (w.Node.Kind != ModUiKind.Toggle && w.Node.Kind != ModUiKind.Slider) return false;
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 0 || value > 1 ||
                (w.Node.Kind == ModUiKind.Toggle && value != 0 && value != 1) || value == w.State.Value) return false;
            dispatching = true;
            try
            {
                Update(w, w.State.Text, value, w.State.Visible, w.State.Enabled);
                if (IsClosed) return false;
                change?.Invoke(id, value);
                return true;
            }
            catch (Exception error) { Close(ModUiCloseReason.Error); scope.Report(error); return false; }
            finally { dispatching = false; }
        }

        private void Update(Widget widget, string text, double value, bool visible, bool enabled)
        {
            var old = widget.State;
            if (old.Text == text && old.Value == value && old.Visible == visible && old.Enabled == enabled) return;
            widget.State = new ModUiWidgetState(text, value, visible, enabled);
            try { Changed?.Invoke(widget.Node.Id); }
            catch (Exception error) { Close(ModUiCloseReason.Error); scope.Report(error); }
        }

        public bool CanClick(string id)
        {
            if (IsClosed || !inputAllowed || dispatching || click == null || id == null || !widgets.TryGetValue(id, out var widget) ||
                widget.Node.Kind != ModUiKind.Button) return false;
            for (var ancestor = widget; ancestor != null; ancestor = ancestor.Parent)
                if (!ancestor.State.Visible || !ancestor.State.Enabled) return false;
            return true;
        }

        // Called by the renderer, never by a saved handle or arbitrary widget name.
        public bool TryClick(string id)
        {
            if (!CanClick(id)) return false;
            dispatching = true;
            try { click(id); return true; }
            catch (Exception error) { Close(ModUiCloseReason.Error); scope.Report(error); return false; }
            finally { dispatching = false; }
        }

        public void Close() => Close(ModUiCloseReason.Script);

        public void Close(ModUiCloseReason reason)
        {
            if (IsClosed) return;
            if (!Enum.IsDefined(typeof(ModUiCloseReason), reason)) throw new ArgumentOutOfRangeException(nameof(reason));
            IsClosed = true;
            scope.Remove(this);
            click = null;
            change = null;
            widgets.Clear();
            var listeners = Closed;
            var notification = close;
            close = null;
            Closed = null; Changed = null;
            if (listeners != null)
                foreach (Action listener in listeners.GetInvocationList())
                    try { listener(); } catch (Exception error) { scope.Report(error); }
            // Renderer teardown and input release finish before notifying Lua.
            try { notification?.Invoke(reason); } catch (Exception error) { scope.Report(error); }
        }

        public void Dispose() => Close();
    }

    // One scene's ordered overlays. It borrows scopes: scene exit closes surfaces,
    // while their owning script scope may open fresh surfaces in the next scene.
    public sealed class ModUiLayerStack : IDisposable
    {
        private readonly List<ModUiSurface> surfaces = new List<ModUiSurface>();
        private readonly Dictionary<ModUiSurface, Action<string>> updates = new Dictionary<ModUiSurface, Action<string>>();
        private bool disposed;
        private bool blocked;
        public ModUiSurface Foreground { get; private set; }
        public bool HasExclusiveInput => Foreground != null && Foreground.Mount != ModUiMount.CombatHud;
        public event Action Changed;

        public void Add(ModUiSurface surface)
        {
            if (disposed) throw new ObjectDisposedException(nameof(ModUiLayerStack));
            if (surface == null || surface.IsClosed) throw new ArgumentException("An open UI surface is required.");
            if (surface.LayerOwner != null) throw new InvalidOperationException("UI surface is already mounted.");
            if (surfaces.Count == 64) throw new InvalidOperationException("A scene permits at most 64 mounted UI surfaces.");
            surfaces.Add(surface);
            surface.LayerOwner = this;
            Action<string> update = id => { if (id == surface.Root.Id) Refresh(); };
            updates.Add(surface, update);
            surface.Changed += update;
            surface.Closed += () => Remove(surface);
            Refresh();
        }

        private void Remove(ModUiSurface surface)
        {
            if (updates.TryGetValue(surface, out var update)) surface.Changed -= update;
            updates.Remove(surface); surfaces.Remove(surface);
            surface.LayerOwner = null;
            Refresh();
        }

        public void SetBlocked(bool value)
        {
            if (disposed || blocked == value) return;
            blocked = value; Refresh();
        }

        public static int Priority(ModUiMount mount) => mount == ModUiMount.Modal ? 2 : mount == ModUiMount.Menu ? 1 : 0;

        private void Refresh()
        {
            ModUiSurface next = null;
            if (!blocked && !disposed)
                foreach (var surface in surfaces)
                    if (!surface.IsClosed && surface.Read(surface.Root.Id).Visible &&
                        (next == null || Priority(surface.Mount) >= Priority(next.Mount))) next = surface;
            Foreground = next;
            foreach (var surface in surfaces) surface.SetInputAllowed(surface == next);
            Changed?.Invoke();
        }

        public bool Back()
        {
            if (!HasExclusiveInput) return false;
            Foreground.Close(ModUiCloseReason.Back); return true;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            foreach (var surface in new List<ModUiSurface>(surfaces)) surface.Close(ModUiCloseReason.Scene);
            surfaces.Clear(); updates.Clear(); Foreground = null; Changed = null;
        }
    }
}

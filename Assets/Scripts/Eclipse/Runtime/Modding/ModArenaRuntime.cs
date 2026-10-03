using System;
using System.Collections.Generic;

namespace Eclipse.Modding
{
    // Arena model coordinates, including native positive-down Y. No Unity types.
    public sealed class ModArenaRect
    {
        public double X { get; }
        public double Y { get; }
        public double Width { get; }
        public double Height { get; }
        public ModArenaRect(double x, double y, double width, double height)
        {
            if (!Finite(x) || !Finite(y) || !Finite(width) || !Finite(height) ||
                Math.Abs(x) > 10000 || Math.Abs(y) > 10000 || width <= 0 || height <= 0 || width > 4000 || height > 4000)
                throw new ArgumentException("Arena rectangle requires finite x/y in -10000..10000 and width/height in (0,4000].");
            X = x; Y = y; Width = width; Height = height;
        }
        private static bool Finite(double n) => !double.IsNaN(n) && !double.IsInfinity(n);

        // Exact distance in XY from a segment to a closed axis-aligned rectangle.
        // Rounded capsule corners must not be treated as an expanded square AABB.
        public bool OverlapsCapsule(double ax, double ay, double bx, double by, double radius)
        {
            if (!Finite(ax) || !Finite(ay) || !Finite(bx) || !Finite(by) || !Finite(radius) || radius < 0) return false;
            double low = 0, high = 1, dx = bx - ax, dy = by - ay;
            if (Clip(-dx, ax - X, ref low, ref high) && Clip(dx, X + Width - ax, ref low, ref high) &&
                Clip(-dy, ay - Y, ref low, ref high) && Clip(dy, Y + Height - ay, ref low, ref high)) return true;
            double distance = Math.Min(PointRect(ax, ay), PointRect(bx, by));
            distance = Math.Min(distance, PointSegment(X, Y, ax, ay, bx, by));
            distance = Math.Min(distance, PointSegment(X + Width, Y, ax, ay, bx, by));
            distance = Math.Min(distance, PointSegment(X, Y + Height, ax, ay, bx, by));
            distance = Math.Min(distance, PointSegment(X + Width, Y + Height, ax, ay, bx, by));
            return distance <= radius * radius;
        }
        private static bool Clip(double p, double q, ref double low, ref double high)
        {
            if (p == 0) return q >= 0;
            double t = q / p;
            if (p < 0) { if (t > high) return false; low = Math.Max(low, t); }
            else { if (t < low) return false; high = Math.Min(high, t); }
            return true;
        }
        private double PointRect(double x, double y)
        {
            double dx = Math.Max(Math.Max(X - x, 0), x - X - Width);
            double dy = Math.Max(Math.Max(Y - y, 0), y - Y - Height);
            return dx * dx + dy * dy;
        }
        private static double PointSegment(double x, double y, double ax, double ay, double bx, double by)
        {
            double dx = bx - ax, dy = by - ay, length = dx * dx + dy * dy;
            double t = length == 0 ? 0 : Math.Max(0, Math.Min(1, ((x - ax) * dx + (y - ay) * dy) / length));
            dx = x - ax - t * dx; dy = y - ay - t * dy;
            return dx * dx + dy * dy;
        }
    }
    public interface IModArenaMarker : IDisposable
    {
        bool IsActive { get; }
        void SetColor(ModUiColor color);
    }
    public interface IModFighterRegions
    {
        bool TryOverlapRect(ModArenaRect rect, out bool overlaps, out string error);
        bool TryMarkRect(ModArenaRect rect, ModUiColor color, out IModArenaMarker marker, out string error);
    }
    public sealed class ModArenaMarkerScope : IDisposable
    {
        public const int MaximumMarkers = 16;
        private readonly HashSet<ModArenaMarkerInstance> markers = new HashSet<ModArenaMarkerInstance>();
        private bool closed;
        public bool TryCreate(IModFighterRegions source, ModArenaRect rect, ModUiColor color,
            out ModArenaMarkerInstance result, out string error)
        {
            result = null; error = null;
            if (closed) { error = "Arena marker scope is closed."; return false; }
            if (rect == null || color == null) throw new ArgumentNullException();
            foreach (var old in new List<ModArenaMarkerInstance>(markers)) if (!old.IsActive) old.Remove();
            if (markers.Count >= MaximumMarkers) { error = "This mod already has 16 active arena markers."; return false; }
            if (source == null) { error = "Arena markers are unavailable in this host."; return false; }
            IModArenaMarker native = null;
            try
            {
                if (!source.TryMarkRect(rect, color, out native, out error) || native == null)
                { native?.Dispose(); error = error ?? "Arena marker creation failed."; return false; }
                result = new ModArenaMarkerInstance(native, value => markers.Remove(value));
                markers.Add(result); return true;
            }
            catch (Exception failure) { native?.Dispose(); error = "Arena marker creation failed: " + failure.Message; return false; }
        }
        public void Dispose()
        {
            if (closed) return;
            closed = true;
            foreach (var marker in new List<ModArenaMarkerInstance>(markers)) marker.Remove();
            markers.Clear();
        }
    }
    public sealed class ModArenaMarkerInstance
    {
        private IModArenaMarker native;
        private Action<ModArenaMarkerInstance> remove;
        internal ModArenaMarkerInstance(IModArenaMarker native, Action<ModArenaMarkerInstance> remove) { this.native = native; this.remove = remove; }
        public bool IsActive => native != null && native.IsActive;
        public bool SetColor(ModUiColor color)
        {
            if (color == null) throw new ArgumentNullException(nameof(color));
            if (!IsActive) { Remove(); return false; }
            native.SetColor(color); return true;
        }
        public bool Remove()
        {
            if (native == null) return false;
            bool active = native.IsActive;
            var old = native; native = null;
            var release = remove; remove = null; release?.Invoke(this);
            old.Dispose(); return active;
        }
    }
}

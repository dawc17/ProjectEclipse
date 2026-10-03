namespace Eclipse.Modding
{
    internal static class ModArenaGeometry
    {
        internal static bool TryOverlap(Model model, ModArenaRect rect, out bool overlaps, out string error)
        {
            overlaps = false; error = null;
            var edges = model?.GetModelObject()?.GetCollisionEdges();
            if (edges == null || edges.Count == 0 || edges.Count > 512)
            { error = "A loaded collision rig with 1..512 edges is required."; return false; }
            foreach (var edge in edges)
            {
                // Use current node positions and the native margin/radius values,
                // rather than a pivot, mesh bounding box, or stale cached segment.
                var a = edge?.GetStartNode()?.GetStart(); var b = edge?.GetEndNode()?.GetStart();
                if (a == null || b == null) { error = "Collision rig has an unavailable edge endpoint."; return false; }
                double start = edge.GetStartMargin(), end = 1 - edge.GetEndMargin();
                double dx = b.GetX() - a.GetX(), dy = b.GetY() - a.GetY();
                if (rect.OverlapsCapsule(a.GetX() + start * dx, a.GetY() + start * dy,
                    a.GetX() + end * dx, a.GetY() + end * dy, edge.GetCollisionRadius()))
                { overlaps = true; return true; }
            }
            return true;
        }
    }
}

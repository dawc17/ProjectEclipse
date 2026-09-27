using System;

namespace Eclipse.Rendering
{
    // Engine-independent presentation math. Y points up; all distances are arena units.
    public static class FloorStainMath
    {
        // Solve the crossing analytically so a long frame cannot push a drop
        // through the floor or move its landing point beyond the impact.
        public static bool Advance(ref float x, ref float y, float vx, ref float vy,
            float gravity, float floor, float seconds)
        {
            if (seconds <= 0f) return false;
            double height = Math.Max(0d, y - floor);
            double root = Math.Sqrt(vy * (double)vy + 2d * gravity * height);
            double landing = vy >= 0f ? (vy + root) / gravity : 2d * height / (root - vy);
            double dt = Math.Min(seconds, landing);
            x += (float)(vx * dt);
            y += (float)(vy * dt - 0.5d * gravity * dt * dt);
            vy -= (float)(gravity * dt);
            if (landing > seconds) return false;
            y = floor;
            return true;
        }

        // Area, rather than diameter, accumulates. The cap bounds repeated hits.
        public static float MergeSize(float size, float addedSize, float maximum)
            => (float)Math.Min(maximum, Math.Sqrt(size * (double)size + addedSize * (double)addedSize));

        public static float MergeAlpha(float alpha, float addedAlpha)
            => 1f - (1f - alpha) * (1f - addedAlpha);
    }
}

using System;
using Eclipse.Rendering;

static class Program
{
    static void Near(float actual, float expected, string message)
    {
        if (Math.Abs(actual - expected) > 0.002f) throw new Exception(message + ": " + actual + " != " + expected);
    }
    static void Main()
    {
        // At y=100 with g=200, a horizontal drop lands in one second.
        float x = 0, y = 100, vy = 0;
        if (!FloorStainMath.Advance(ref x, ref y, 30, ref vy, 200, 0, 2)) throw new Exception("Missed floor crossing");
        Near(x, 30, "Overshoot moved the landing point"); Near(y, 0, "Below floor");
        // Different frame rates and a large frame must find the same landing.
        foreach (float step in new[] { 1f / 30, 1f / 60, 1f / 144, 0.7f })
        {
            x = 7; y = 110; vy = 130;
            bool landed = false;
            for (int i = 0; i < 1000 && !landed; i++)
                landed = FloorStainMath.Advance(ref x, ref y, -90, ref vy, 900, 10, step);
            if (!landed) throw new Exception("Never landed");
            float expectedTime = (130f + (float)Math.Sqrt(130 * 130 + 2 * 900 * 100)) / 900;
            Near(x, 7 - 90 * expectedTime, "Frame-dependent trajectory"); Near(y, 10, "Raised floor");
        }
        x = 12; y = 0; vy = 100;
        if (FloorStainMath.Advance(ref x, ref y, 10, ref vy, 200, 0, 0.1f)) throw new Exception("Upward launch landed immediately");
        Near(y, 9, "Lift from floor");
        float before = y;
        FloorStainMath.Advance(ref x, ref y, 10, ref vy, 200, 0, 0);
        Near(y, before, "Paused drop moved");
        x = 0; y = 0; vy = -5;
        if (!FloorStainMath.Advance(ref x, ref y, 100, ref vy, 200, 0, 1)) throw new Exception("Downward floor contact missed");
        Near(x, 0, "Contact drift");
        Near(FloorStainMath.MergeSize(30, 40, 100), 50, "Accumulation must conserve area");
        float size = 10, alpha = 0.2f;
        for (int i = 0; i < 10000; i++)
        {
            size = FloorStainMath.MergeSize(size, 20, 70);
            alpha = FloorStainMath.MergeAlpha(alpha, 0.2f);
            if (size > 70 || alpha > 1 || alpha < 0) throw new Exception("Unbounded accumulation");
        }
        Near(size, 70, "Pool cap"); Near(alpha, 1, "Opacity accumulation");
        Console.WriteLine("PASS: floor crossings, launch/lift, pause, frame-rate independence, area accumulation and caps.");
    }
}

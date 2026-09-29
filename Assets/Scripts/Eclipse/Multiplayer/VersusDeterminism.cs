namespace Eclipse.Multiplayer
{
    /// <summary>
    /// Keeps versus fights reproducible across peers and replays. The combat RNG
    /// (NekkiMath) is re-seeded from the match seed at the start of every tick, so
    /// anything that rolls it between ticks (UI, music, rewards) cannot shift combat
    /// rolls. Gameplay rolls that used UnityEngine.Random, whose global state cosmetic
    /// effects also consume, draw from <see cref="Range(float,float)"/> instead.
    /// Outside versus every call falls back to the original behaviour.
    /// </summary>
    public static class VersusDeterminism
    {
        /// <summary>One simulation tick; matches the project's fixed timestep.</summary>
        public const float TickSeconds = 1f / 60f;
        /// <summary>Critical hit-stop scale used in versus instead of the local accessibility setting.</summary>
        public const float CriticalPause = .8f;

        private static int _seed;
        private static uint _state = 1;

        public static bool Active { get; private set; }

        /// <summary>Called before the versus fight is built, so construction-time rolls are seeded too.</summary>
        public static void Seed(int seed)
        {
            _seed = seed;
            Active = true;
            Reseed(0x5EED0000u);
        }

        public static void End() { Active = false; }

        /// <summary>
        /// A frame counter for short-lived gameplay flags: the simulation tick in versus
        /// (identical on every peer and across rollbacks), the render frame otherwise.
        /// </summary>
        public static int FrameStamp => Active ? VersusTickDriver.Tick : UnityEngine.Time.frameCount;

        internal static void BeginTick(int tick)
        {
            if (Active) Reseed((uint)tick);
        }

        /// <summary>Replaces RulesInspector's clock-based reseed at fight and round starts.</summary>
        public static void ReseedRules(int round)
        {
            if (Active) NekkiMath.KACCBCCEPGB((int)Mix((uint)_seed ^ 0xA5A5A5A5u, 0x40000000u + (uint)round));
            else NekkiMath.KACCBCCEPGB();
        }

        /// <summary>Drop-in for UnityEngine.Random.Range(float, float) in gameplay code.</summary>
        public static float Range(float min, float max)
        {
            if (!Active) return UnityEngine.Random.Range(min, max);
            return min + (float)(Next() / 4294967296.0) * (max - min);
        }

        /// <summary>Drop-in for UnityEngine.Random.Range(int, int) (max exclusive) in gameplay code.</summary>
        public static int Range(int min, int max)
        {
            if (!Active) return UnityEngine.Random.Range(min, max);
            if (max <= min) return min;
            return min + (int)(Next() % (uint)(max - min));
        }

        private static void Reseed(uint salt)
        {
            NekkiMath.KACCBCCEPGB((int)Mix((uint)_seed, salt));
            _state = Mix((uint)_seed ^ 0x9E3779B9u, salt) | 1u;
        }

        private static uint Next()
        {
            // xorshift32
            uint x = _state;
            x ^= x << 13;
            x ^= x >> 17;
            x ^= x << 5;
            _state = x;
            return x;
        }

        private static uint Mix(uint a, uint b)
        {
            unchecked
            {
                uint h = a * 0x85EBCA6Bu ^ b * 0xC2B2AE35u;
                h ^= h >> 16; h *= 0x7FEB352Du;
                h ^= h >> 15; h *= 0x846CA68Bu;
                h ^= h >> 16;
                return h == 0 ? 1u : h;
            }
        }
    }
}

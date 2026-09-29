namespace Eclipse.Multiplayer.Online
{
    /// <summary>
    /// One fighter's controls for one simulation tick, packed into a byte. The low
    /// nibble holds the screen-space direction using the FightCID quadrant values
    /// (0 neutral, 1 up ... 8 up-back); the fight mirrors it by facing as usual.
    /// </summary>
    public static class NetInput
    {
        public const byte Neutral = 0;
        public const byte DirectionMask = 0x0F;
        public const byte Punch = 0x10;
        public const byte Kick = 0x20;
        public const byte Ranged = 0x40;
        public const byte Magic = 0x80;
        public const int MaxDirection = 8;

        public static int Direction(byte input) => input & DirectionMask;

        public static byte WithDirection(byte input, int direction)
        {
            if (direction < 0 || direction > MaxDirection) direction = 0;
            return (byte)((input & ~DirectionMask) | direction);
        }

        public static byte WithButton(byte input, byte button, bool pressed)
        {
            return pressed ? (byte)(input | button) : (byte)(input & ~button);
        }

        /// <summary>Rejects bytes a well-formed peer can never send.</summary>
        public static bool IsValid(byte input) => Direction(input) <= MaxDirection;
    }
}

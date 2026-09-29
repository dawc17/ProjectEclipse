using System;

namespace Eclipse.Multiplayer
{
    public enum VersusMode
    {
        /// <summary>Two players on this machine.</summary>
        Local,
        /// <summary>One player here and one over the network; the host is player one.</summary>
        Online,
        /// <summary>A recorded match played back from its inputs.</summary>
        Replay,
    }

    /// <summary>Immutable configuration for one versus match and its rematches.</summary>
    public sealed class LocalVersusSettings
    {
        public string PlayerOneWeapon { get; }
        public string PlayerTwoWeapon { get; }
        public string Location { get; }
        public bool KeyboardPlayerOne { get; }
        /// <summary>Both players on one keyboard with fixed layouts; no gamepad needed.</summary>
        public bool SharedKeyboard { get; }
        public int WinsRequired { get; }
        public int RoundTimeSeconds { get; }
        public VersusMode Mode { get; }
        public string PlayerOneName { get; }
        public string PlayerTwoName { get; }
        /// <summary>Seeds every gameplay random source, so peers and replays roll identically.</summary>
        public int Seed { get; }

        public LocalVersusSettings(string playerOneWeapon, string playerTwoWeapon, string location,
            bool keyboardPlayerOne, int winsRequired = 2, int roundTimeSeconds = 99,
            VersusMode mode = VersusMode.Local, string playerOneName = null, string playerTwoName = null, int? seed = null,
            bool sharedKeyboard = false)
        {
            if (string.IsNullOrWhiteSpace(playerOneWeapon)) throw new ArgumentException("Choose player one's weapon.", nameof(playerOneWeapon));
            if (string.IsNullOrWhiteSpace(playerTwoWeapon)) throw new ArgumentException("Choose player two's weapon.", nameof(playerTwoWeapon));
            if (string.IsNullOrWhiteSpace(location)) throw new ArgumentException("Choose an arena.", nameof(location));
            if (winsRequired < 1 || winsRequired > 5) throw new ArgumentOutOfRangeException(nameof(winsRequired));
            if (roundTimeSeconds < 30 || roundTimeSeconds > 300) throw new ArgumentOutOfRangeException(nameof(roundTimeSeconds));
            PlayerOneWeapon = playerOneWeapon;
            PlayerTwoWeapon = playerTwoWeapon;
            Location = location;
            SharedKeyboard = sharedKeyboard;
            KeyboardPlayerOne = keyboardPlayerOne || sharedKeyboard;
            WinsRequired = winsRequired;
            RoundTimeSeconds = roundTimeSeconds;
            Mode = mode;
            PlayerOneName = string.IsNullOrWhiteSpace(playerOneName) ? "PLAYER 1" : playerOneName.Trim();
            PlayerTwoName = string.IsNullOrWhiteSpace(playerTwoName) ? "PLAYER 2" : playerTwoName.Trim();
            Seed = seed ?? new Random().Next();
        }

        /// <summary>The same matchup with a fresh seed, for a local rematch.</summary>
        public LocalVersusSettings Reseeded()
        {
            return new LocalVersusSettings(PlayerOneWeapon, PlayerTwoWeapon, Location, KeyboardPlayerOne,
                WinsRequired, RoundTimeSeconds, Mode, PlayerOneName, PlayerTwoName, null, SharedKeyboard);
        }
    }

    public static class LocalVersusRoundRules
    {
        /// <returns>Zero for player one, one for player two, or minus one for a draw.</returns>
        public static int ResolveWinner(float playerOneHealthFraction, float playerTwoHealthFraction)
        {
            if (float.IsNaN(playerOneHealthFraction) || float.IsInfinity(playerOneHealthFraction) ||
                float.IsNaN(playerTwoHealthFraction) || float.IsInfinity(playerTwoHealthFraction))
                throw new ArgumentException("Round health must be finite.");
            float difference = playerOneHealthFraction - playerTwoHealthFraction;
            // Equal timeouts and double knockouts never award a round to either side.
            if (Math.Abs(difference) <= 0.000001f) return -1;
            return difference > 0 ? 0 : 1;
        }
    }
}

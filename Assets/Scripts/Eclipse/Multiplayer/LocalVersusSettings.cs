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
        /// <summary>Player one against a training dummy (scripted, recorded or the game AI).</summary>
        Training,
    }

    /// <summary>Immutable configuration for one versus match and its rematches.</summary>
    public sealed class LocalVersusSettings
    {
        public VersusLoadout PlayerOneLoadout { get; }
        public VersusLoadout PlayerTwoLoadout { get; }
        public string PlayerOneWeapon => PlayerOneLoadout.Weapon;
        public string PlayerTwoWeapon => PlayerTwoLoadout.Weapon;
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
        /// <summary>Player two is driven by the game AI with this tactic (training only), or null.</summary>
        public string PlayerTwoTactic { get; }

        public LocalVersusSettings(VersusLoadout playerOneLoadout, VersusLoadout playerTwoLoadout, string location,
            bool keyboardPlayerOne, int winsRequired = 2, int roundTimeSeconds = 99,
            VersusMode mode = VersusMode.Local, string playerOneName = null, string playerTwoName = null, int? seed = null,
            bool sharedKeyboard = false, string playerTwoTactic = null)
        {
            if (playerOneLoadout == null || string.IsNullOrWhiteSpace(playerOneLoadout.Weapon)) throw new ArgumentException("Choose player one's loadout.", nameof(playerOneLoadout));
            if (playerTwoLoadout == null || string.IsNullOrWhiteSpace(playerTwoLoadout.Weapon)) throw new ArgumentException("Choose player two's loadout.", nameof(playerTwoLoadout));
            if (string.IsNullOrWhiteSpace(location)) throw new ArgumentException("Choose an arena.", nameof(location));
            if (winsRequired < 1 || winsRequired > 5) throw new ArgumentOutOfRangeException(nameof(winsRequired));
            if (roundTimeSeconds < 30 || roundTimeSeconds > 300) throw new ArgumentOutOfRangeException(nameof(roundTimeSeconds));
            PlayerOneLoadout = playerOneLoadout;
            PlayerTwoLoadout = playerTwoLoadout;
            Location = location;
            SharedKeyboard = sharedKeyboard;
            KeyboardPlayerOne = keyboardPlayerOne || sharedKeyboard;
            WinsRequired = winsRequired;
            RoundTimeSeconds = roundTimeSeconds;
            Mode = mode;
            PlayerOneName = string.IsNullOrWhiteSpace(playerOneName) ? "PLAYER 1" : playerOneName.Trim();
            PlayerTwoName = string.IsNullOrWhiteSpace(playerTwoName) ? "PLAYER 2" : playerTwoName.Trim();
            Seed = seed ?? new Random().Next();
            PlayerTwoTactic = mode == VersusMode.Training && !string.IsNullOrEmpty(playerTwoTactic) ? playerTwoTactic : null;
        }

        /// <summary>The same matchup with a fresh seed, for a local rematch.</summary>
        public LocalVersusSettings Reseeded()
        {
            return new LocalVersusSettings(PlayerOneLoadout, PlayerTwoLoadout, Location, KeyboardPlayerOne,
                WinsRequired, RoundTimeSeconds, Mode, PlayerOneName, PlayerTwoName, null, SharedKeyboard, PlayerTwoTactic);
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

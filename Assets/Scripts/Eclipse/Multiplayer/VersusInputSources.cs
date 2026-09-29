using System;
using System.IO;
using Eclipse.Input;
using Eclipse.Multiplayer.Online;
using UnityEngine;

namespace Eclipse.Multiplayer
{
    /// <summary>Records every versus match so it can be replayed and checked for determinism.</summary>
    public abstract class RecordingInputSource : IVersusInputSource
    {
        protected readonly VersusReplay Replay;
        private bool _saved;

        protected RecordingInputSource(LocalVersusSettings settings)
        {
            Replay = VersusReplays.Create(settings);
        }

        public virtual void Pump() { }
        public virtual int StepsWanted => 1;
        public abstract bool TryGetTick(int tick, out byte left, out byte right);

        public virtual void OnTickSimulated(int tick, byte left, byte right, uint? hash)
        {
            Replay.Record(left, right);
            if (hash.HasValue && tick % NetProtocol.ReplayHashInterval == 0) Replay.Hashes[tick] = hash.Value;
        }

        public virtual void OnMatchEnded(int finalTick, int winner, int leftRounds, int rightRounds) => Save(null);

        public virtual void Stop() => Save(null);

        protected string Save(string suffix)
        {
            if (_saved || Replay.TickCount == 0) return null;
            _saved = true;
            return VersusReplays.Save(Replay, suffix);
        }
    }

    /// <summary>Fixed layouts for two players on one keyboard; they never share a key.</summary>
    public static class SharedKeyboard
    {
        public static readonly FightKeyboardLayout Left = new FightKeyboardLayout
        {
            Up = new[] { KeyCode.W }, Left = new[] { KeyCode.A }, Down = new[] { KeyCode.S }, Right = new[] { KeyCode.D },
            Punch = new[] { KeyCode.F }, Kick = new[] { KeyCode.G }, Ranged = new[] { KeyCode.R }, Magic = new[] { KeyCode.T },
        };

        public static readonly FightKeyboardLayout Right = new FightKeyboardLayout
        {
            Up = new[] { KeyCode.UpArrow }, Left = new[] { KeyCode.LeftArrow }, Down = new[] { KeyCode.DownArrow }, Right = new[] { KeyCode.RightArrow },
            Punch = new[] { KeyCode.Period, KeyCode.Keypad1 }, Kick = new[] { KeyCode.Slash, KeyCode.Keypad2 },
            Ranged = new[] { KeyCode.Semicolon, KeyCode.Keypad4 }, Magic = new[] { KeyCode.Quote, KeyCode.Keypad5 },
        };

        public const string Hint = "P1: WASD move, F punch, G kick, R ranged, T magic     " +
            "P2: arrows move, . punch, / kick, ; ranged, ' magic (or numpad 1 2 4 5)";
    }

    /// <summary>Two players on this machine.</summary>
    public sealed class LocalInputSource : RecordingInputSource
    {
        private readonly VersusInputSampler _left, _right;

        public LocalInputSource(LocalVersusSettings settings) : base(settings)
        {
            if (settings.SharedKeyboard)
            {
                _left = new VersusInputSampler(GamePad.Player.One, true, true, false, SharedKeyboard.Left);
                _right = new VersusInputSampler(GamePad.Player.Two, true, true, false, SharedKeyboard.Right);
                return;
            }
            bool keyboard = settings.KeyboardPlayerOne;
            _left = new VersusInputSampler(GamePad.Player.One, keyboard, keyboard, !keyboard);
            _right = new VersusInputSampler(keyboard ? GamePad.Player.One : GamePad.Player.Two, false, false, true);
        }

        public override bool TryGetTick(int tick, out byte left, out byte right)
        {
            bool enabled = Application.isFocused && !LocalVersusMenu.BlocksFightInput;
            left = _left.SampleWithTouch(enabled);
            right = _right.Sample(enabled);
            return true;
        }
    }

    /// <summary>Plays a recorded match back and compares its state hashes.</summary>
    public sealed class ReplayInputSource : IVersusInputSource
    {
        public const int MaxTraceTicks = 3000;
        private readonly VersusReplay _replay;
        private StreamWriter _trace;
        public int CheckedHashes { get; private set; }
        public int DivergedTick { get; private set; } = -1;
        public bool ReachedEnd { get; private set; }

        public ReplayInputSource(VersusReplay replay)
        {
            _replay = replay ?? throw new ArgumentNullException(nameof(replay));
            // Cross-device determinism probe: create Replays/trace.on to log every tick's
            // state, then diff the files produced on two machines from the same replay.
            try
            {
                if (File.Exists(Path.Combine(VersusReplays.Directory, "trace.on")))
                {
                    string platform = Application.platform + "-" + System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture;
                    string path = Path.Combine(VersusReplays.Directory, "trace-" + platform + ".txt");
                    _trace = new StreamWriter(path, false);
                    _trace.WriteLine("# " + platform + " " + Application.version + " seed " + replay.Seed + " ticks " + replay.TickCount);
                    Debug.Log("[Versus Replay] Tracing every tick to " + path);
                }
            }
            catch (Exception exception) { Debug.LogWarning("[Versus Replay] Trace disabled: " + exception.Message); _trace = null; }
        }

        public void Pump() { }
        public int StepsWanted => 1;

        public bool TryGetTick(int tick, out byte left, out byte right)
        {
            if (tick >= _replay.TickCount) { left = right = 0; ReachedEnd = true; return false; }
            left = _replay.Left[tick];
            right = _replay.Right[tick];
            return true;
        }

        public void OnTickSimulated(int tick, byte left, byte right, uint? hash)
        {
            if (_trace != null)
            {
                var fight = Fight.GetCurrentFight();
                _trace.WriteLine(tick + " " + VersusStateHash.Compute(fight, tick).ToString("X8") + " in " + left.ToString("X2") + right.ToString("X2") + " | " + VersusStateHash.Describe(fight));
                if (tick >= MaxTraceTicks) Stop();
            }
            if (!hash.HasValue || !_replay.Hashes.TryGetValue(tick, out var expected)) return;
            CheckedHashes++;
            if (expected != hash.Value && DivergedTick < 0)
            {
                DivergedTick = tick;
                Debug.LogError("[Versus Replay] Simulation diverged from the recording at tick " + tick +
                    " (recorded " + expected.ToString("X8") + ", replayed " + hash.Value.ToString("X8") + "). " + VersusStateHash.Describe(Fight.GetCurrentFight()));
            }
        }

        public void OnMatchEnded(int finalTick, int winner, int leftRounds, int rightRounds) { ReachedEnd = true; Stop(); }

        public void Stop()
        {
            try { _trace?.Dispose(); } catch (Exception) { }
            _trace = null;
        }

        public string Summary()
        {
            if (DivergedTick >= 0) return "Replay diverged at tick " + DivergedTick + ". The simulation is not deterministic here.";
            return "Replay verified: " + CheckedHashes + " state checkpoints matched.";
        }
    }

    public static class VersusReplays
    {
        public static string Directory => Path.Combine(Application.persistentDataPath, "Replays");
        public static string LastPath => Path.Combine(Directory, "last.eclreplay");

        public static VersusReplay Create(LocalVersusSettings settings)
        {
            return new VersusReplay
            {
                Build = OnlineVersusSession.BuildId,
                Content = OnlineVersusSession.ContentFingerprint(),
                LeftName = settings.PlayerOneName,
                RightName = settings.PlayerTwoName,
                LeftWeapon = settings.PlayerOneWeapon,
                RightWeapon = settings.PlayerTwoWeapon,
                Arena = settings.Location,
                WinsRequired = settings.WinsRequired,
                RoundTimeSeconds = settings.RoundTimeSeconds,
                Seed = settings.Seed,
                Online = settings.Mode == VersusMode.Online,
                RecordedUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            };
        }

        /// <returns>The saved path, or null when saving failed.</returns>
        public static string Save(VersusReplay replay, string suffix)
        {
            try
            {
                string stamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                string side = OnlineVersusSession.Current == null ? "" : OnlineVersusSession.Current.IsHost ? "-host" : "-guest";
                string name = (replay.Online ? "online-" : "local-") + stamp + side + (string.IsNullOrEmpty(suffix) ? "" : "-" + suffix) + ".eclreplay";
                string path = Path.Combine(Directory, name);
                if (File.Exists(path)) path = Path.Combine(Directory, Path.GetFileNameWithoutExtension(name) + "-" + Guid.NewGuid().ToString("N").Substring(0, 6) + ".eclreplay");
                replay.Save(path);
                try { File.Copy(path, LastPath, true); }
                catch (IOException) { /* Another instance is writing it; the dated copy is saved. */ }
                Prune(40);
                Debug.Log("[Versus] Replay saved: " + path);
                return path;
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[Versus] Could not save the replay: " + exception.Message);
                return null;
            }
        }

        public static bool TryLoadLast(out VersusReplay replay, out string error)
        {
            replay = null;
            error = null;
            if (!File.Exists(LastPath)) { error = "No replay yet. Finish a versus match first."; return false; }
            try { replay = VersusReplay.Load(LastPath); }
            catch (Exception exception) { error = "The last replay could not be read: " + exception.Message; return false; }
            if (replay.Build != OnlineVersusSession.BuildId) { error = "The last replay was recorded with another game version."; replay = null; return false; }
            if (replay.Content != OnlineVersusSession.ContentFingerprint()) { error = "The last replay was recorded with different mods enabled."; replay = null; return false; }
            return true;
        }

        private static void Prune(int keep)
        {
            var files = new DirectoryInfo(Directory).GetFiles("*-*.eclreplay");
            if (files.Length <= keep) return;
            Array.Sort(files, (a, b) => a.LastWriteTimeUtc.CompareTo(b.LastWriteTimeUtc));
            for (int i = 0; i < files.Length - keep; i++)
            {
                try { files[i].Delete(); } catch (Exception) { }
            }
        }
    }
}

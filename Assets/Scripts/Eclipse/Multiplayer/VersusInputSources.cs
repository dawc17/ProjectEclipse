using System;
using System.Collections.Generic;
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
        private int _lastRound = -1;

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
            // Round changes become timeline markers in the replay player.
            if (TryGetRound(tick, out int round))
            {
                if (_lastRound >= 0 && round != _lastRound) Replay.RoundEnds.Add(tick);
                _lastRound = round;
            }
        }

        /// <summary>The round number after <paramref name="tick"/>, when this source knows it.</summary>
        protected virtual bool TryGetRound(int tick, out int round)
        {
            var snapshot = VersusTickDriver.LastSnapshot;
            round = snapshot.Round;
            return snapshot.Tick == tick;
        }

        public virtual void OnMatchEnded(int finalTick, int winner, int leftRounds, int rightRounds)
        {
            Replay.Winner = winner;
            Replay.LeftRounds = leftRounds;
            Replay.RightRounds = rightRounds;
            Save(null);
        }

        public virtual void Stop() => Save(null);

        protected string Save(string suffix)
        {
            if (_saved || Replay.TickCount == 0) return null;
            _saved = true;
            if (!string.IsNullOrEmpty(suffix)) Replay.Tag = suffix;
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
            if (VersusTraining.Replay)
            {
                VersusTraining.TickReadouts(Fight.GetCurrentFight(), tick);
                TrainingHud.Record(left, right);
            }
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

    /// <summary>Read-only room viewing, using the same deterministic simulation as a replay.</summary>
    public sealed class SpectatorInputSource : IVersusInputSource, IVersusStepRunner
    {
        internal static bool SuppressAudio { get; private set; }
        private readonly Online.Rooms.SpectatorStream _stream;
        private readonly Online.Rooms.SpectatorPlayback _playback = new Online.Rooms.SpectatorPlayback();
        private int _frame = -1;
        public SpectatorInputSource(Online.Rooms.SpectatorStream stream) { _stream = stream; }
        public void Pump() { }
        public int StepsWanted => 1;
        public bool TryGetTick(int tick, out byte left, out byte right)
        {
            left = right = 0;
            if (tick >= _stream.Replay.TickCount) return false;
            left = _stream.Replay.Left[tick]; right = _stream.Replay.Right[tick];
            return true;
        }

        public void RunStep(Fight fight)
        {
            int backlog = _stream.Replay.TickCount - VersusTickDriver.Tick;
            int steps = _playback.StepsWanted(backlog, _stream.Ended);
            bool catchingUp = _playback.CatchingUp;
            // ponytail: replay from tick zero; portable snapshots if late-join catch-up becomes too slow.
            if (steps == 0)
            {
                VersusTickDriver.MarkStalled(true);
                if (_stream.Ended) LocalVersusSession.CompleteOnline(fight, -1, _stream.EndReason);
                return;
            }
            if (catchingUp && _frame == Time.frameCount) return;
            _frame = Time.frameCount;
            var clock = System.Diagnostics.Stopwatch.StartNew();
            // Muting the listener only for this synchronous loop still queues
            // voices that all become audible together once the listener is restored.
            SuppressAudio = catchingUp || steps > 1;
            try
            {
                for (int i = 0; i < steps && !LocalVersusSession.HasResult && VersusTickDriver.Owns(fight); i++)
                {
                    int tick = VersusTickDriver.Tick;
                    if (!TryGetTick(tick, out var left, out var right))
                    {
                        VersusTickDriver.MarkStalled(true);
                        if (_stream.Ended) LocalVersusSession.CompleteOnline(fight, -1, _stream.EndReason);
                        break;
                    }
                    if (!VersusTickDriver.SimulateTick(fight, tick, left, right, TickFlags.None, out uint hash)) break;
                    if (_stream.Replay.Hashes.TryGetValue(tick, out uint expected) && expected != hash)
                    {
                        LocalVersusSession.CompleteOnline(fight, -1, "Spectator playback fell out of sync. Return to the room and try again.");
                        break;
                    }
                    if (clock.ElapsedMilliseconds >= 8) break;
                }
            }
            finally { SuppressAudio = false; }
        }

        public void OnTickSimulated(int tick, byte left, byte right, uint? hash) { }
        public void OnMatchEnded(int finalTick, int winner, int leftRounds, int rightRounds) { }
        public void Stop() { }
    }

    public static class VersusReplays
    {
        public static string Directory => Path.Combine(Eclipse.Runtime.EditorPlayModeContext.PersistentDataPath, "Replays");
        public static string LastPath => Path.Combine(Directory, "last.eclreplay");

        public const int KeepNewest = 40;

        public static VersusReplay Create(LocalVersusSettings settings)
        {
            var replay = new VersusReplay
            {
                Build = OnlineVersusSession.BuildId,
                Content = OnlineVersusSession.ContentFingerprint(),
                BalanceHash = settings.Balance.Hash,
                BalanceJson = settings.Balance.ToJson(),
                LeftName = settings.PlayerOneName,
                RightName = settings.PlayerTwoName,
                Arena = settings.Location,
                WinsRequired = settings.WinsRequired,
                RoundTimeSeconds = settings.RoundTimeSeconds,
                Seed = settings.Seed,
                Online = settings.Mode == VersusMode.Online,
                RecordedUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            };
            var left = settings.PlayerOneLoadout.ToIds();
            var right = settings.PlayerTwoLoadout.ToIds();
            for (int slot = 0; slot < VersusReplay.LoadoutSlots; slot++) { replay.LeftLoadout[slot] = left[slot]; replay.RightLoadout[slot] = right[slot]; }
            return replay;
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
                Prune(KeepNewest);
                Debug.Log("[Versus] Replay saved: " + path);
                return path;
            }
            catch (Exception exception)
            {
                Debug.LogWarning("[Versus] Could not save the replay: " + exception.Message);
                return null;
            }
        }

        public static bool TryLoadLast(out VersusReplay replay, out string error) => TryLoad(LastPath, out replay, out error);

        /// <summary>Loads a replay this build can play back; otherwise a player-facing reason.</summary>
        public static bool TryLoad(string path, out VersusReplay replay, out string error)
        {
            replay = null;
            error = null;
            if (!File.Exists(path)) { error = path == LastPath ? "No replay yet. Finish a versus match first." : "That replay no longer exists."; return false; }
            try { replay = VersusReplay.Load(path); }
            catch (Exception exception) { error = "The replay could not be read: " + exception.Message; return false; }
            error = Incompatibility(replay);
            if (error == null) return true;
            replay = null;
            return false;
        }

        /// <summary>Why this build cannot play the replay back, or null.</summary>
        public static string Incompatibility(VersusReplay replay)
        {
            if (replay.SourceFormat < 3) return "Recorded before adjustable PvP balance; this combat version cannot verify its playback.";
            try
            {
                var balance = Eclipse.Multiplayer.Balance.PvpBalanceProfile.Parse(replay.BalanceJson).Compile();
                if (balance.Hash != replay.BalanceHash) return "The recorded balance profile does not match its gameplay hash.";
            }
            catch (Exception error) { return "The recorded balance profile is invalid: " + error.Message; }
            if (replay.Build != OnlineVersusSession.BuildId)
            {
                NetIdentity.SplitBuild(replay.Build, out var version, out var runtime);
                return version == Application.version && runtime != OnlineVersusSession.Runtime
                    ? "Recorded by a " + (runtime.Length > 0 ? runtime : "different") + " build; it only plays back in the same kind of build."
                    : "Recorded with another game version (" + replay.Build + ").";
            }
            if (replay.Content != OnlineVersusSession.ContentFingerprint()) return "Recorded with different mods, equipment roster, or combat balance.";
            return null;
        }

        /// <summary>One saved replay as the browser lists it.</summary>
        public sealed class Entry
        {
            public string Path;
            public DateTime Saved;
            /// <summary>Header fields only; <see cref="VersusReplay.HeaderOnly"/> is set.</summary>
            public VersusReplay Header;
            /// <summary>Why it cannot be watched in this build, or null.</summary>
            public string Problem;
        }

        /// <summary>Every saved replay (not the "last" copy), kept ones first, then newest first.</summary>
        public static List<Entry> List()
        {
            var entries = new List<Entry>();
            if (!System.IO.Directory.Exists(Directory)) return entries;
            foreach (var file in new DirectoryInfo(Directory).GetFiles("*.eclreplay"))
            {
                if (string.Equals(file.FullName, Path.GetFullPath(LastPath), StringComparison.OrdinalIgnoreCase)) continue;
                var entry = new Entry { Path = file.FullName, Saved = file.LastWriteTime };
                try
                {
                    entry.Header = VersusReplay.LoadHeader(file.FullName);
                    entry.Problem = Incompatibility(entry.Header);
                }
                catch (Exception exception) { entry.Problem = "Unreadable: " + exception.Message; }
                entries.Add(entry);
            }
            entries.Sort((a, b) =>
            {
                bool keptA = a.Header != null && a.Header.Kept, keptB = b.Header != null && b.Header.Kept;
                if (keptA != keptB) return keptA ? -1 : 1;
                return b.Saved.CompareTo(a.Saved);
            });
            return entries;
        }

        /// <summary>Rewrites a replay's header fields (kept flag, title); the inputs are unchanged.</summary>
        public static bool TryUpdate(string path, Action<VersusReplay> change, out string error)
        {
            error = null;
            try
            {
                var replay = VersusReplay.Load(path);
                var saved = File.GetLastWriteTimeUtc(path);
                change(replay);
                replay.Save(path);
                // The browser orders by recording time; editing a flag does not make it "new".
                File.SetLastWriteTimeUtc(path, saved);
                return true;
            }
            catch (Exception exception) { error = "The replay could not be changed: " + exception.Message; return false; }
        }

        public static bool SetKept(string path, bool kept, out string error) => TryUpdate(path, replay => replay.Kept = kept, out error);

        public static bool Rename(string path, string title, out string error)
        {
            title = (title ?? string.Empty).Trim();
            if (title.Length > 40) title = title.Substring(0, 40);
            return TryUpdate(path, replay => replay.Title = title, out error);
        }

        public static bool Delete(string path, out string error)
        {
            error = null;
            try { File.Delete(path); return true; }
            catch (Exception exception) { error = "The replay could not be deleted: " + exception.Message; return false; }
        }

        /// <summary>Keeps the newest <paramref name="keep"/> replays that are not marked kept; kept ones are never removed.</summary>
        private static void Prune(int keep)
        {
            var candidates = new List<FileInfo>();
            foreach (var file in new DirectoryInfo(Directory).GetFiles("*-*.eclreplay"))
            {
                bool kept = false;
                try { kept = VersusReplay.LoadHeader(file.FullName).Kept; } catch (Exception) { }
                if (!kept) candidates.Add(file);
            }
            if (candidates.Count <= keep) return;
            candidates.Sort((a, b) => a.LastWriteTimeUtc.CompareTo(b.LastWriteTimeUtc));
            for (int i = 0; i < candidates.Count - keep; i++)
            {
                try { candidates[i].Delete(); } catch (Exception) { }
            }
        }
    }
}

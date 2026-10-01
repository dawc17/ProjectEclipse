using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Eclipse.Multiplayer;
using Eclipse.Multiplayer.Online;
using Eclipse.Multiplayer.Online.Rooms;
using UnityEditor;
using UnityEngine;

// Prepare the GameLoader start scene, enter Play Mode, then invoke with Unity Pipeline:
// unity command run_script --file Tools/NetplayTests/ValidateRoomSpectatingNative.cs --entry ValidateRoomSpectatingNative.Run
// No imported assets or physical controller are needed. Does not connect to the public room server.
public static class ValidateRoomSpectatingNative
{
    private static readonly string Report = Path.Combine("Temp", "RoomSpectatingNative", "result.txt");
    private static readonly BindingFlags Hidden = BindingFlags.Instance | BindingFlags.NonPublic;
    private static int phase, checks, captureFrame;
    private static double deadline;
    private static double lastReport;
    private static float previousScale;
    private static float liveStartedAt;
    private static int liveStartedTick;
    private static Inputs inputs;
    private static SpectatorStream stream;
    private static RoomClient client;

    public static object Run()
    {
        if (!EditorApplication.isPlaying) throw new InvalidOperationException("Enter Play Mode from GameLoader first.");
        Directory.CreateDirectory(Path.GetDirectoryName(Report));
        File.WriteAllText(Report, "RUNNING\n");
        phase = checks = 0;
        lastReport = EditorApplication.timeSinceStartup;
        deadline = EditorApplication.timeSinceStartup + 300;
        previousScale = Time.timeScale;
        EditorApplication.update += Tick;
        EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView")).Focus();
        return new { running = true, report = Report };
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        checks++;
        File.AppendAllText(Report, "PASS " + message + "\n");
    }

    private static void Tick()
    {
        try
        {
            if (client != null)
            {
                // Native gameplay fixture; the real lease/expiry protocol is checked over UDP in RoomTests.
                // Keep the fixture grant across slow synchronous native asset loads.
                typeof(RoomClient).GetField("_windowReceivedMs", Hidden).SetValue(client, OnlineVersusSession.NowMs + 180000);
                typeof(RoomClient).GetField("_nowMs", Hidden).SetValue(client, OnlineVersusSession.NowMs);
            }
            if (!EditorApplication.isPlaying) throw new Exception("Play Mode ended during validation.");
            if (EditorApplication.timeSinceStartup > deadline) throw new TimeoutException("Timed out at phase " + phase);
            if (EditorApplication.timeSinceStartup - lastReport > 10)
            {
                lastReport = EditorApplication.timeSinceStartup;
                File.AppendAllText(Report, "Waiting phase " + phase + ": tick=" + VersusTickDriver.Tick + ", recorded=" + (inputs?.Replay.TickCount ?? 0) + ", starting=" + LocalVersusSession.IsStarting + ", access=" + RoomSession.CanPlay + "\n");
            }
            switch (phase)
            {
                case 0:
                    if (!Eclipse.UI.TitleScreen.IsOpen) return;
                    var title = UnityEngine.Object.FindAnyObjectByType<Eclipse.UI.TitleScreen>();
                    if ((bool)typeof(Eclipse.UI.TitleScreen).GetField("splashing", Hidden).GetValue(title))
                    {
                        // The unchanged launch movie is outside these menu/fight checks.
                        title.StopAllCoroutines();
                        var splash = title.transform.Find("Splash");
                        if (splash != null) UnityEngine.Object.Destroy(splash.gameObject);
                        typeof(Eclipse.UI.TitleScreen).GetField("splashing", Hidden).SetValue(title, false);
                        typeof(Eclipse.UI.TitleScreen).GetMethod("Home", Hidden).Invoke(title, null);
                    }
                    var multiplayer = title.GetComponentsInChildren<UnityEngine.UI.Button>().FirstOrDefault(button => button.GetComponentInChildren<UnityEngine.UI.Text>()?.text == "JOIN PLAYTEST");
                    if (multiplayer == null) return;
                    Check(!multiplayer.interactable && !RoomSession.CanPlay, "unscheduled title disables playtest entry");
                    Check(!title.GetComponentsInChildren<UnityEngine.UI.Text>().Any(text => text.text == "CAMPAIGN" || text.text == "MODS"), "restricted title offers no campaign or mod entry");
                    RoomSession.Shutdown();
                    BuildAccessFixture();
                    bool refused = false;
                    try { RoomSession.RequireScene(ScreenType.ModuleMap); }
                    catch (InvalidOperationException) { refused = true; }
                    Check(refused, "shared scene loader refuses campaign navigation");
                    multiplayer.onClick.Invoke();
                    phase++;
                    break;
                case 1:
                    if (!LocalVersusSession.IsReady) return;
                    typeof(RoomClient).GetProperty("State").SetValue(client, RoomClientState.Connecting);
                    typeof(LocalVersusSession).GetMethod("Update", Hidden).Invoke(UnityEngine.Object.FindAnyObjectByType<LocalVersusSession>(), null);
                    Check(LocalVersusSession.IsActive && !RoomSession.CanPlay, "lobby can reconnect a renamed player while gameplay stays locked");
                    typeof(RoomClient).GetProperty("State").SetValue(client, RoomClientState.Connected);
                    bool localRefused = false;
                    try { LocalVersusSession.StartMatch(new LocalVersusSettings(VersusLoadout.Default, VersusLoadout.Default, "dojo", true)); }
                    catch (InvalidOperationException) { localRefused = true; }
                    Check(localRefused, "local matches are refused even with playtest access");
                    Check(!LocalVersusMenu.Ensure().GetComponentsInChildren<UnityEngine.UI.Text>().Any(text => text.text == "DIRECT CONNECT" || text.text == "TRAINING" || text.text == "REPLAYS"), "multiplayer home exposes rooms only");
                    var settings = new LocalVersusSettings(VersusLoadout.Default, VersusLoadout.Default, "dojo", true,
                        mode: VersusMode.Online, playerOneName: "Left", playerTwoName: "Right", seed: 123456);
                    inputs = new Inputs();
                    LocalVersusSession.StartMatch(settings, () => inputs);
                    Time.timeScale = 4f;
                    phase++;
                    break;
                case 2:
                    if (inputs.Replay.TickCount < 720) return;
                    Check(!LocalVersusSession.HasResult, "native fight supplies a deterministic input recording");
                    stream = new SpectatorStream { MatchId = 1 };
                    Append(360);
                    var watched = new LocalVersusSettings(VersusLoadout.Default, VersusLoadout.Default, "dojo", true,
                        mode: VersusMode.Spectator, playerOneName: "Left", playerTwoName: "Right", seed: 123456);
                    LocalVersusSession.StartMatch(watched, () => new SpectatorInputSource(stream));
                    phase++;
                    break;
                case 3:
                    if (LocalVersusSession.IsStarting) return;
                    if (LocalVersusSession.HasResult) throw new Exception("Spectator history diverged.");
                    if (VersusTickDriver.Tick < 300) return;
                    Check(VersusTickDriver.Tick <= 360, "late viewer catches up without predicting unavailable inputs");
                    CheckHash();
                    liveStartedAt = Time.fixedTime;
                    liveStartedTick = VersusTickDriver.Tick;
                    phase++;
                    break;
                case 4:
                    if (LocalVersusSession.HasResult) throw new Exception("Live spectator inputs diverged.");
                    // Deliver the same 20-tick batches as the live host, rather than a whole history at once.
                    int elapsedTicks = Mathf.FloorToInt((Time.fixedTime - liveStartedAt) * 60);
                    Append(Math.Min(660, 360 + elapsedTicks / 20 * 20));
                    if (VersusTickDriver.IsStalled) throw new Exception("Live spectator playback ran out of its jitter buffer.");
                    if (Math.Abs(VersusTickDriver.Tick - liveStartedTick - elapsedTicks) > 3)
                        throw new Exception("Live spectator playback changed speed to chase an incoming batch.");
                    if (stream.Replay.TickCount < 660 || VersusTickDriver.Tick < 600) return;
                    Check(true, "native live batches play continuously at normal speed");
                    CheckHash();
                    typeof(RoomClient).GetProperty("Window").SetValue(client, new PlaytestWindow(0, 1));
                    typeof(RoomClient).GetField("_serverUnixMs", Hidden).SetValue(client, 2L);
                    int blockedSteps = (int)typeof(VersusTickDriver).GetMethod("StepsFor", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { Fight.GetCurrentFight() });
                    Check(blockedSteps == 0 && !RoomSession.CanPlay, "expired access blocks native spectator fight ticks before the frame pump");
                    typeof(RoomClient).GetProperty("Window").SetValue(client, new PlaytestWindow(0, PlaytestWindow.MaxUnixMs));
                    typeof(RoomClient).GetField("_serverUnixMs", Hidden).SetValue(client, 0L);
                    stream.Ended = true;
                    phase++;
                    break;
                case 5:
                    if (!LocalVersusSession.HasResult) return;
                    Check(VersusTickDriver.Tick == 660, "spectator consumes the final buffer before ending");
                    var back = LocalVersusMenu.Ensure().GetComponentsInChildren<UnityEngine.UI.Button>().First(button => button.GetComponentInChildren<UnityEngine.UI.Text>().text == "BACK TO ROOM");
                    back.onClick.Invoke();
                    Check(LocalVersusMenu.Ensure().IsShowing, "spectator can leave the result screen");
                    BuildRoom();
                    captureFrame = Time.frameCount;
                    phase++;
                    break;
                case 6:
                    if (Time.frameCount < captureFrame + 5) return;
                    var buttons = LocalVersusMenu.Ensure().GetComponentsInChildren<UnityEngine.UI.Button>()
                        .Where(button => button.GetComponentInChildren<UnityEngine.UI.Text>().text == "SPECTATE").ToArray();
                    Check(buttons.Length == 4 && buttons.All(button => button.interactable), "room renders four enabled spectate buttons");
                    Check(buttons.All(button => ((RectTransform)button.transform).rect.width > 0 && ((RectTransform)button.transform).rect.height > 0), "spectate controls have visible bounds");
                    buttons[1].onClick.Invoke();
                    Check((uint)typeof(RoomClient).GetField("_wantedSpectate", Hidden).GetValue(client) == 2, "spectate button selects its own fight");
                    RoomSession.EndPlaytest("Playtest is over.");
                    client = null;
                    phase++;
                    break;
                case 7:
                    if (!Eclipse.UI.TitleScreen.IsOpen || LocalVersusSession.IsActive) return;
                    var expiredTitle = UnityEngine.Object.FindAnyObjectByType<Eclipse.UI.TitleScreen>();
                    var join = expiredTitle.GetComponentsInChildren<UnityEngine.UI.Button>().FirstOrDefault(button => button.GetComponentInChildren<UnityEngine.UI.Text>()?.text == "JOIN PLAYTEST");
                    if (join == null) return;
                    Check(!join.interactable && expiredTitle.GetComponentsInChildren<UnityEngine.UI.Text>().Any(text => text.text == "Playtest is over."), "expiry returns to the locked title with playtest-over message");
                    ScreenCapture.CaptureScreenshot(Path.Combine("Temp", "RoomSpectatingNative", "expired.png"));
                    Finish(null);
                    break;
            }
        }
        catch (Exception exception) { Finish(exception.ToString()); }
    }

    private static void CheckHash()
    {
        var snapshot = VersusTickDriver.LastSnapshot;
        Check(inputs.Replay.Hashes[snapshot.Tick] == VersusStateHash.Hash(snapshot), "native spectator state matches recording at tick " + snapshot.Tick);
    }

    private static void Append(int until)
    {
        for (int tick = stream.Replay.TickCount; tick < until; tick++)
        {
            stream.Replay.Record(inputs.Replay.Left[tick], inputs.Replay.Right[tick]);
            stream.Replay.Hashes[tick] = inputs.Replay.Hashes[tick];
        }
    }

    private static void BuildRoom()
    {
        RoomSession.Shutdown();
        var session = BuildAccessFixture();
        var room = new RoomState { RoomId = 1, HostId = 99, Code = "ABCDEF" };
        for (uint id = 1; id <= 8; id++) room.Members.Add(new RoomMember { Id = id, Name = "Player " + id, Status = MemberStatus.InMatch });
        for (uint id = 1; id <= 4; id++) room.Fights.Add(new RoomFight { MatchId = id, LeftId = id * 2 - 1, RightId = id * 2, CanSpectate = true });
        typeof(RoomClient).GetProperty("Room").SetValue(client, room);
        LocalVersusMenu.Ensure().ShowRoom();
    }

    private static RoomSession BuildAccessFixture()
    {
        var session = new GameObject("Native room UI validation").AddComponent<RoomSession>();
        UnityEngine.Object.DontDestroyOnLoad(session.gameObject);
        client = new RoomClient(new IPEndPoint(IPAddress.Loopback, 1), new NetIdentity("native", "none", "Viewer"), Array.Empty<IPAddress>(), OnlineVersusSession.NowMs);
        typeof(RoomClient).GetProperty("State").SetValue(client, RoomClientState.Connected);
        typeof(RoomClient).GetProperty("Window").SetValue(client, new PlaytestWindow(0, PlaytestWindow.MaxUnixMs));
        typeof(RoomClient).GetField("_windowReceivedMs", Hidden).SetValue(client, OnlineVersusSession.NowMs + 180000);
        typeof(RoomSession).GetProperty("Client").SetValue(session, client);
        typeof(RoomSession).GetProperty("Current").SetValue(null, session);
        session.enabled = false;
        return session;
    }

    private static void Finish(string error)
    {
        EditorApplication.update -= Tick;
        if (error != null)
        {
            RoomSession.Shutdown("Native validation ended.");
            Time.timeScale = previousScale;
        }
        // On success leave the fixture visible for screenshot inspection; stop Play Mode to restore the scene.
        File.AppendAllText(Report, error == null ? "PASS: " + checks + " native checks\n" : "FAIL: " + error + "\n");
    }

    private sealed class Inputs : IVersusInputSource
    {
        public readonly VersusReplay Replay = new VersusReplay();
        public void Pump() { }
        public int StepsWanted => 1;
        public bool TryGetTick(int tick, out byte left, out byte right)
        {
            left = NetInput.WithDirection(0, tick < 240 ? 4 : 0);
            right = NetInput.WithDirection(0, tick < 240 ? 8 : 0);
            if (tick % 60 < 10) left |= NetInput.Punch;
            return true;
        }
        public void OnTickSimulated(int tick, byte left, byte right, uint? hash)
        {
            Replay.Record(left, right);
            if (hash.HasValue) Replay.Hashes[tick] = hash.Value;
        }
        public void OnMatchEnded(int finalTick, int winner, int leftRounds, int rightRounds) { }
        public void Stop() { }
    }
}

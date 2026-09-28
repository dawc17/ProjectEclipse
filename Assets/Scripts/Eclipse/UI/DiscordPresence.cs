using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Eclipse.UI
{
    // Discord Rich Presence over Discord's local IPC pipe (\\.\pipe\discord-ipc-N), with no
    // native plugin. Shows what the player is doing (menu, dojo, map, shop, profile or a
    // fight with its battle name), their level and Eclipse mode. Windows desktop only; the
    // client is silent when Discord is not running and reconnects when it starts.
    //
    // Needs a Discord application: create one at https://discord.com/developers/applications,
    // put its Application ID in ClientId (or in StreamingAssets/discord_client_id.txt, which
    // wins), and upload Rich Presence art assets named "eclipse" and "eclipse_mode".
    public sealed class DiscordPresence : MonoBehaviour
    {
        public const string ClientId = "1554245978758979654";
        private const string Preference = "Eclipse.DiscordPresence";
        private const float PollSeconds = 2f;

        public static bool Enabled => PlayerPrefs.GetInt(Preference, 1) == 1;
        public static bool Supported =>
            Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor;

        public static void Toggle()
        {
            PlayerPrefs.SetInt(Preference, Enabled ? 0 : 1);
            PlayerPrefs.Save();
            if (instance != null) instance.lastActivity = null;
        }

        private static DiscordPresence instance;
        private Client client;
        private string lastActivity;
        private float nextPoll;
        private long sessionStart, fightStart;
        private object lastFight;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Boot()
        {
            if (instance != null || !Supported || Application.isBatchMode) return;
            string id = ReadClientId();
            if (string.IsNullOrEmpty(id)) { Debug.Log("[Discord] No application ID configured; rich presence is off."); return; }
            var host = new GameObject("Eclipse Discord Presence");
            DontDestroyOnLoad(host);
            host.hideFlags = HideFlags.HideInHierarchy;
            instance = host.AddComponent<DiscordPresence>();
            instance.client = new Client(id);
            instance.sessionStart = Now();
        }

        private static string ReadClientId()
        {
            try
            {
                string path = Path.Combine(Application.streamingAssetsPath, "discord_client_id.txt");
                if (File.Exists(path))
                {
                    string value = File.ReadAllText(path).Trim();
                    if (value.Length > 0) return value;
                }
            }
            catch (Exception) { }
            return ClientId;
        }

        private static long Now() { return DateTimeOffset.UtcNow.ToUnixTimeSeconds(); }

        private void Update()
        {
            if (Time.unscaledTime < nextPoll) return;
            nextPoll = Time.unscaledTime + PollSeconds;
            string activity = Enabled ? BuildActivity() : "null";
            if (activity == lastActivity) return;
            lastActivity = activity;
            client.SetActivity(activity);
        }

        private void OnApplicationQuit() { client?.Dispose(); }
        private void OnDestroy() { client?.Dispose(); if (instance == this) instance = null; }

        // --- What the player is doing ---------------------------------------------------------

        private string BuildActivity()
        {
            string details, state = null, small = null, smallText = null;
            long start = sessionStart;
            try
            {
                var roster = ListSF.CCDKHLAMKKO();
                bool eclipse = roster != null && roster.IsEclipseMode();
                if (roster != null)
                {
                    state = "Level " + roster.Level + (eclipse ? " · Eclipse mode" : string.Empty);
                    if (eclipse) { small = "eclipse_mode"; smallText = "Eclipse mode"; }
                }
                if (TitleScreen.IsOpen || GameSessionRestart.IsRestarting || roster == null)
                {
                    details = "In the main menu";
                    state = null; small = null;
                }
                else
                {
                    var module = Module.GetInstance();
                    var screen = module == null ? ScreenType.ModuleNone : module.GetCurrentScreenType();
                    switch (screen)
                    {
                        case ScreenType.ModuleDojo: details = "Training in the dojo"; break;
                        case ScreenType.ModuleMap: details = "Exploring the map"; break;
                        case ScreenType.ModuleShop: details = "Browsing the shop"; break;
                        case ScreenType.ModuleProfile: details = "Checking their profile"; break;
                        case ScreenType.ModuleFight: details = FightDetails(out start); break;
                        default: details = "Loading"; break;
                    }
                }
            }
            catch (Exception) { details = "Playing"; }

            var json = new StringBuilder("{");
            Field(json, "details", details);
            if (state != null) { json.Append(','); Field(json, "state", state); }
            json.Append(",\"timestamps\":{\"start\":").Append(start).Append('}');
            json.Append(",\"assets\":{");
            Field(json, "large_image", "eclipse");
            json.Append(','); Field(json, "large_text", "Project Eclipse — Shadow Fight 2");
            if (small != null) { json.Append(','); Field(json, "small_image", small); json.Append(','); Field(json, "small_text", smallText); }
            json.Append("}}");
            return json.ToString();
        }

        private string FightDetails(out long start)
        {
            var fight = Fight.GetCurrentFight();
            if (fight != lastFight) { lastFight = fight; fightStart = Now(); }
            start = fightStart;
            if (fight == null) return "In a fight";
            if (fight.IsLocalVersus) return "Local versus";
            var definition = fight.OGNINOBBHIG();
            var battle = definition == null ? null : definition.Battle;
            string title = battle == null ? null : Localized(battle.IGPOHDHPIIL());
            bool raid = definition != null && definition.get_Type() == BattleType.FightRaid;
            if (string.IsNullOrEmpty(title)) return raid ? "Raiding in the Underworld" : "In a fight";
            return (raid ? "Raid: " : "Fighting: ") + title;
        }

        private static string Localized(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            try
            {
                string text = LocalizationManager.GetString(key);
                if (string.IsNullOrEmpty(text) || text == key) return null;
                return System.Text.RegularExpressions.Regex.Replace(text, "<[^>]*>|\\{[^}]*\\}", string.Empty).Trim();
            }
            catch (Exception) { return null; }
        }

        private static void Field(StringBuilder json, string name, string value)
        {
            json.Append('"').Append(name).Append("\":\"");
            if (value.Length > 120) value = value.Substring(0, 120);
            foreach (char c in value)
            {
                if (c == '"' || c == '\\') json.Append('\\').Append(c);
                else if (c < ' ') json.Append("\\u").Append(((int)c).ToString("x4"));
                else json.Append(c);
            }
            json.Append('"');
        }

        // --- IPC --------------------------------------------------------------------------------

        // One background thread owns the pipe: it connects (retrying while Discord is closed),
        // sends the handshake, then sends the latest requested activity and reads each reply.
        // The pipe is opened with Win32 CreateFile: Unity's Mono NamedPipeClientStream is not
        // reliable on Windows, while a FileStream over a pipe handle is.
        private sealed class Client : IDisposable
        {
            private readonly string clientId;
            private readonly int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
            private readonly System.Threading.Thread thread;
            private readonly AutoResetEvent wake = new AutoResetEvent(false);
            private volatile bool running = true;
            private string pending;
            private readonly object gate = new object();

            public Client(string id)
            {
                clientId = id;
                thread = new System.Threading.Thread(Run) { IsBackground = true, Name = "Discord presence" };
                thread.Start();
            }

            public void SetActivity(string activityJson)
            {
                lock (gate) pending = activityJson;
                wake.Set();
            }

            private void Run()
            {
                FileStream pipe = null;
                string sent = null;
                bool reported = false, connectedOnce = false;
                while (running)
                {
                    try
                    {
                        if (pipe == null)
                        {
                            pipe = Connect();
                            if (pipe == null)
                            {
                                if (!reported) { reported = true; Debug.Log("[Discord] Discord is not running; retrying every 15 s."); }
                                wake.WaitOne(15000);
                                continue;
                            }
                            Write(pipe, 0, "{\"v\":1,\"client_id\":\"" + clientId + "\"}");
                            string ready = Read(pipe);
                            if (!ready.Contains("\"READY\"")) throw new IOException("Handshake refused: " + ready);
                            if (!connectedOnce) { connectedOnce = true; Debug.Log("[Discord] Connected; rich presence active."); }
                            sent = null;
                        }
                        string next;
                        lock (gate) next = pending;
                        if (next != null && next != sent)
                        {
                            Write(pipe, 1, "{\"cmd\":\"SET_ACTIVITY\",\"args\":{\"pid\":" + pid + ",\"activity\":" + next +
                                "},\"nonce\":\"" + Guid.NewGuid().ToString("N") + "\"}");
                            string reply = Read(pipe);
                            if (reply.Contains("\"evt\":\"ERROR\"")) Debug.LogWarning("[Discord] Activity rejected: " + reply);
                            sent = next;
                        }
                        wake.WaitOne(5000);
                    }
                    catch (Exception error)
                    {
                        if (running) Debug.LogWarning("[Discord] Connection lost: " + error.Message);
                        try { pipe?.Dispose(); } catch (Exception) { }
                        pipe = null;
                        if (running) wake.WaitOne(15000);
                    }
                }
                try
                {
                    if (pipe != null) Write(pipe, 2, "{}");
                }
                catch (Exception) { }
                pipe?.Dispose();
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            private static extern SafeFileHandle CreateFileW(string name, uint access, uint share, IntPtr security,
                uint creation, uint flags, IntPtr template);

            private static FileStream Connect()
            {
                const uint GenericRead = 0x80000000, GenericWrite = 0x40000000, OpenExisting = 3;
                for (int i = 0; i < 10; i++)
                {
                    var handle = CreateFileW(@"\\.\pipe\discord-ipc-" + i, GenericRead | GenericWrite, 0, IntPtr.Zero, OpenExisting, 0, IntPtr.Zero);
                    if (handle.IsInvalid) { handle.Dispose(); continue; }
                    return new FileStream(handle, FileAccess.ReadWrite, 4096, false);
                }
                return null;
            }

            private static void Write(Stream pipe, int opcode, string json)
            {
                byte[] body = Encoding.UTF8.GetBytes(json);
                byte[] frame = new byte[8 + body.Length];
                BitConverter.GetBytes(opcode).CopyTo(frame, 0);
                BitConverter.GetBytes(body.Length).CopyTo(frame, 4);
                body.CopyTo(frame, 8);
                pipe.Write(frame, 0, frame.Length);
                pipe.Flush();
            }

            // Reads one reply frame; a close frame (opcode 2) drops the connection.
            private static string Read(Stream pipe)
            {
                byte[] header = ReadExactly(pipe, 8);
                int opcode = BitConverter.ToInt32(header, 0), length = BitConverter.ToInt32(header, 4);
                if (length < 0 || length > 1 << 20) throw new IOException("Bad Discord frame.");
                string body = Encoding.UTF8.GetString(ReadExactly(pipe, length));
                if (opcode == 2) throw new IOException("Discord closed the connection: " + body);
                return body;
            }

            private static byte[] ReadExactly(Stream pipe, int count)
            {
                var buffer = new byte[count];
                int offset = 0;
                while (offset < count)
                {
                    int read = pipe.Read(buffer, offset, count - offset);
                    if (read <= 0) throw new IOException("Discord pipe closed.");
                    offset += read;
                }
                return buffer;
            }

            public void Dispose()
            {
                if (!running) return;
                running = false;
                wake.Set();
                thread.Join(500);
            }
        }
    }
}

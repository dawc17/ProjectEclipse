using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eclipse.Launcher
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [STAThread]
        private static void Main(string[] args)
        {
            // Without this Windows bitmap-stretches the window on scaled displays and the text blurs.
            SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            try
            {
                string root = args.Length == 2 && args[0] == "--root" ? Path.GetFullPath(args[1]) : AppDomain.CurrentDomain.BaseDirectory;
                Directory.CreateDirectory(root);
                var state = UpdateCore.ReadState(root);
                string activeLauncher = Path.Combine(UpdateCore.GameDirectory(root, state), "EclipseLauncher.exe");
                // Stable bootstrap forwards to the launcher shipped with the active version.
                if (args.Length == 0 && File.Exists(activeLauncher) &&
                    !Path.GetFullPath(activeLauncher).Equals(Application.ExecutablePath, StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start(new ProcessStartInfo(activeLauncher, "--root \"" + root.TrimEnd('\\') + "\"") { WorkingDirectory = root });
                    return;
                }
                using (var guard = new FileStream(Path.Combine(root, "launcher.lock"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    Application.Run(new LauncherForm(root, state));
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Eclipse Launcher", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
    internal sealed class LauncherForm : Form
    {
        private readonly string root;
        private readonly InstallState state;
        private static readonly int HeaderHeight = Theme.S(56);
        private readonly Label status = new Label { AutoSize = false, AutoEllipsis = true, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, BackColor = Theme.Panel, ForeColor = Theme.Paper, Font = Theme.Body };
        private readonly ThemedButton play = new ThemedButton { Text = "Play", Primary = true };
        private readonly ThemedButton update = new ThemedButton { Text = "Check for updates" };
        private readonly LinkButton rollback = new LinkButton { Text = "Roll back" };
        private readonly LinkButton notes = new LinkButton { Text = "Release notes" };
        private readonly ThemedCheckBox automatic = new ThemedCheckBox { Text = "Check on startup" };
        private readonly ChannelSwitch channel = new ChannelSwitch("stable", "beta");
        private readonly ThemedProgress progress = new ThemedProgress();
        private readonly Image emblem = Theme.Emblem(Theme.S(40));
        private Manifest candidate;
        private bool busy;

        public LauncherForm(string installRoot, InstallState installState)
        {
            root = installRoot; state = installState;
            Text = "Eclipse Launcher"; Icon = Theme.WindowIcon();
            FormBorderStyle = FormBorderStyle.None; MaximizeBox = false; StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(Theme.S(560), Theme.S(214)); BackColor = Theme.Ink; ForeColor = Theme.Paper; Font = Theme.Body; DoubleBuffered = true;
            // Tall enough for two lines: a version plus its release notes, or an error plus the fallback note.
            var card = new Card { Bounds = Theme.S(20, 68, 520, 40), Padding = new Padding(Theme.S(14), Theme.S(2), Theme.S(10), Theme.S(2)) };
            card.Controls.Add(status);
            // Download progress runs along the bottom edge of the status card.
            progress.Bounds = Theme.S(21, 104, 518, 3);
            play.Bounds = Theme.S(20, 120, 254, 38);
            update.Bounds = Theme.S(282, 120, 258, 38);
            // Settings on the left, rarely used actions as links on the right.
            channel.Bounds = Theme.S(92, 172, 120, 24);
            automatic.Bounds = Theme.S(228, 172, 132, 24);
            notes.Bounds = Theme.S(362, 172, 93, 24);
            rollback.Bounds = Theme.S(475, 172, 65, 24);
            var minimize = new CaptionButton(false) { Bounds = Theme.S(484, 2, 37, 30) };
            var close = new CaptionButton(true) { Bounds = Theme.S(521, 2, 37, 30) };
            minimize.Click += (s, e) => WindowState = FormWindowState.Minimized;
            close.Click += (s, e) => Close();
            channel.SelectedItem = state.channel;
            automatic.Checked = state.autoCheck;
            Controls.AddRange(new Control[] { minimize, close, progress, card, play, update, channel, automatic, notes, rollback });
            play.Click += async (s, e) => await Run(Play);
            update.Click += async (s, e) => await Run(candidate == null ? (Func<Task>)Check : Install);
            rollback.Click += async (s, e) => await Run(Rollback);
            notes.Click += (s, e) => Process.Start(UpdateCore.Repository + "/releases");
            automatic.CheckedChanged += (s, e) => { state.autoCheck = automatic.Checked; SaveSettings(); };
            channel.SelectedIndexChanged += (s, e) => { state.channel = (string)channel.SelectedItem; candidate = null; SaveSettings(); RefreshButtons(); };
            FormClosing += (s, e) => { if (busy) e.Cancel = true; };
            Shown += async (s, e) => { if (state.autoCheck) await Run(Check); };
            status.Text = ReadyText();
            RefreshButtons();
        }
        protected override CreateParams CreateParams
        {
            get
            {
                var p = base.CreateParams;
                p.Style |= 0x00020000 | 0x00080000; // WS_MINIMIZEBOX | WS_SYSMENU: taskbar minimize/restore for a borderless window
                p.ClassStyle |= 0x00020000; // CS_DROPSHADOW
                return p;
            }
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            // Dragging the wooden header moves the window (WM_NCHITTEST: HTCLIENT -> HTCAPTION).
            if (m.Msg == 0x84 && (int)m.Result == 1 && PointToClient(new Point((short)((long)m.LParam & 0xFFFF), (short)(((long)m.LParam >> 16) & 0xFFFF))).Y < HeaderHeight)
                m.Result = (IntPtr)2;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            // Header is drawn like the wooden sign on the title screen.
            using (var brush = new SolidBrush(Theme.Wood)) g.FillRectangle(brush, 2, 2, Width - 4, HeaderHeight - 2);
            using (var pen = new Pen(Color.FromArgb(86, 52, 35))) for (int y = Theme.S(20); y < HeaderHeight; y += Theme.S(18)) g.DrawLine(pen, 2, y, Width - 3, y);
            using (var pen = new Pen(Theme.Frame)) { g.DrawRectangle(pen, 1, 1, Width - 3, Height - 3); g.DrawLine(pen, 1, HeaderHeight, Width - 2, HeaderHeight); }
            using (var pen = new Pen(Color.Black)) g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            g.DrawImage(emblem, Theme.S(14, 9, 40, 40));
            Theme.DrawDisplay(g, "PROJECT ECLIPSE", Theme.TitleFont, Theme.S(60, 6, 300, 30), Theme.Paper, StringAlignment.Near);
            TextRenderer.DrawText(g, "LAUNCHER", Theme.Body, new Point(Theme.S(62), Theme.S(33)), Theme.Frame, TextFormatFlags.NoPadding);
            string installed = (string.IsNullOrEmpty(state.current) ? "existing build" : "v" + state.current) + "  ·  " + state.channel;
            TextRenderer.DrawText(g, installed, Theme.Body, Theme.S(300, 32, 248, 18), Theme.Paper, TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            Theme.DrawDisplay(g, "CHANNEL", Theme.SmallFont, Theme.S(20, 172, 70, 24), Theme.Muted, StringAlignment.Near);
            TextRenderer.DrawText(g, "·", Theme.Body, Theme.S(455, 172, 24, 24), Theme.Muted, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
        private bool GameInstalled()
        {
            return File.Exists(Path.Combine(UpdateCore.GameDirectory(root, state), "Eclipse.exe"));
        }
        // The installed version is already shown in the header.
        private string ReadyText()
        {
            return GameInstalled() ? "Ready to play." : "No game build found. Check for updates to install one.";
        }
        private void SaveSettings()
        {
            try { UpdateCore.SaveState(root, state); }
            catch (Exception ex) { status.Text = "Could not save settings: " + ex.Message; }
        }
        private void RefreshButtons()
        {
            play.Enabled = !busy && GameInstalled();
            update.Enabled = rollback.Enabled = channel.Enabled = automatic.Enabled = !busy;
            rollback.Enabled &= !string.IsNullOrEmpty(state.current) &&
                File.Exists(Path.Combine(string.IsNullOrEmpty(state.previous) ? root : UpdateCore.VersionDirectory(root, state.previous), "Eclipse.exe"));
            update.Text = candidate == null ? "Check for updates" : "Update to " + candidate.version;
            update.Accent = candidate != null;
            if (!busy) progress.Value = 0;
            Invalidate(new Rectangle(0, 0, Width, HeaderHeight)); // installed version or channel may have changed
        }
        private async Task Run(Func<Task> action)
        {
            if (busy) return;
            busy = true; RefreshButtons();
            try { await action(); }
            catch (Exception ex) { status.Text = ex.Message + "\nYour installed build is still available."; }
            finally { busy = false; RefreshButtons(); }
        }
        private async Task Check()
        {
            status.Text = "Checking " + state.channel + " releases…";
            string manifestPath = Path.Combine(root, "manifest-" + Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                string url = UpdateCore.Repository + "/releases/" +
                    (state.channel == "stable" ? "latest/download/stable.json" : "download/beta/beta.json");
                await Task.Run(() => UpdateCore.Download(url, manifestPath, 1024 * 1024, null));
                string text = File.ReadAllText(manifestPath);
                var manifest = UpdateCore.Json.Deserialize<Manifest>(text);
                UpdateCore.ValidateManifest(manifest);
                candidate = UpdateCore.IsNewer(manifest.version, state.current) && manifest.version != state.rejected ? manifest : null;
                if (candidate == null) status.Text = GameInstalled() ? "Up to date. Ready to play." : "No newer update available.";
                else status.Text =
                    "Available: " + candidate.version + "\n" + candidate.notes;
            }
            finally { if (File.Exists(manifestPath)) File.Delete(manifestPath); }
        }
        private async Task Install()
        {
            Manifest manifest = candidate;
            string stage = Path.Combine(root, "staging", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(stage);
            string archive = Path.Combine(stage, "game.zip");
            using (var output = File.Create(archive))
            {
                for (int i = 0; i < manifest.parts.Length; i++)
                {
                    var part = manifest.parts[i];
                    string path = Path.Combine(stage, "part-" + i);
                    status.Text = "Downloading part " + (i + 1) + " of " + manifest.parts.Length;
                    IProgress<int> reporting = new Progress<int>(value => progress.Value = value);
                    await Task.Run(() => UpdateCore.Download(part.url, path, part.size,
                        bytes => reporting.Report((int)Math.Min(100, bytes * 100 / part.size))));
                    await Task.Run(() => {
                        UpdateCore.VerifyPart(path, part);
                        using (var input = File.OpenRead(path)) input.CopyTo(output);
                    });
                    File.Delete(path);
                }
            }
            status.Text = "Verifying and extracting update…";
            string extracted = Path.Combine(stage, "game");
            await Task.Run(() => UpdateCore.Extract(archive, extracted, manifest.unpackedSize));
            UpdateCore.Activate(root, state, extracted, manifest.version);
            candidate = null;
            File.Delete(archive);
            status.Text = "Update installed. Click Play. The updated launcher is used on next launch.";
        }
        private async Task Play()
        {
            string directory = UpdateCore.GameDirectory(root, state);
            var start = new ProcessStartInfo(Path.Combine(directory, "Eclipse.exe")) { WorkingDirectory = directory, UseShellExecute = false };
            string mods = Path.Combine(root, "Mods");
            Directory.CreateDirectory(mods);
            start.EnvironmentVariables["ECLIPSE_MODS_ROOT"] = mods;
            Process game;
            try { game = Process.Start(start); }
            catch { Rollback().GetAwaiter().GetResult(); throw; }
            status.Text = "Game running. Close it before changing versions.";
            using (game)
            {
                // Hide while the game runs so only its taskbar icon shows.
                Hide();
                try { await Task.Run(() => game.WaitForExit()); }
                finally { Show(); Activate(); }
                if (game.ExitCode != 0) { await Rollback(); status.Text = "Game exited with an error. Previous build restored when available."; }
                else status.Text = ReadyText();
            }
        }
        private Task Rollback()
        {
            string previous = string.IsNullOrEmpty(state.previous) ? root : UpdateCore.VersionDirectory(root, state.previous);
            if (!string.IsNullOrEmpty(state.current) && File.Exists(Path.Combine(previous, "Eclipse.exe")))
            {
                string rejected = state.current;
                var next = UpdateCore.Json.Deserialize<InstallState>(UpdateCore.Json.Serialize(state));
                next.current = state.previous; next.previous = null; next.rejected = rejected;
                UpdateCore.SaveState(root, next);
                state.current = next.current; state.previous = null; state.rejected = rejected;
                candidate = null;
                status.Text = "Previous build restored. Version " + rejected + " will be skipped.";
            }
            return Task.FromResult(0);
        }
    }
}

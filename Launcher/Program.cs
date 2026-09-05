using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eclipse.Launcher
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
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
        private readonly Label status = new Label { AutoSize = false, Height = 70, Dock = DockStyle.Top };
        private readonly Button play = new Button { Text = "Play", Width = 100 };
        private readonly Button update = new Button { Text = "Check for updates", Width = 150 };
        private readonly Button rollback = new Button { Text = "Roll back", Width = 100 };
        private readonly Button notes = new Button { Text = "Release notes", Width = 110 };
        private readonly CheckBox automatic = new CheckBox { Text = "Check automatically", AutoSize = true };
        private readonly ComboBox channel = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 100 };
        private readonly ProgressBar progress = new ProgressBar { Dock = DockStyle.Bottom };
        private Manifest candidate;
        private bool busy;

        public LauncherForm(string installRoot, InstallState installState)
        {
            root = installRoot; state = installState;
            Text = "Eclipse Launcher"; ClientSize = new Size(610, 220); MinimumSize = Size;
            Font = new Font("Segoe UI", 10); Padding = new Padding(16);
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45 };
            buttons.Controls.AddRange(new Control[] { play, update, rollback, notes });
            var settings = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
            channel.Items.AddRange(new object[] { "stable", "beta" }); channel.SelectedItem = state.channel;
            automatic.Checked = state.autoCheck;
            settings.Controls.AddRange(new Control[] { channel, automatic });
            Controls.Add(settings); Controls.Add(buttons); Controls.Add(status); Controls.Add(progress);
            play.Click += async (s, e) => await Run(Play);
            update.Click += async (s, e) => await Run(candidate == null ? (Func<Task>)Check : Install);
            rollback.Click += async (s, e) => await Run(Rollback);
            notes.Click += (s, e) => Process.Start(UpdateCore.Repository + "/releases");
            automatic.CheckedChanged += (s, e) => { state.autoCheck = automatic.Checked; SaveSettings(); };
            channel.SelectedIndexChanged += (s, e) => { state.channel = (string)channel.SelectedItem; candidate = null; SaveSettings(); RefreshButtons(); };
            FormClosing += (s, e) => { if (busy) e.Cancel = true; };
            Shown += async (s, e) => { if (state.autoCheck) await Run(Check); };
            status.Text = "Installed: " + (state.current ?? "existing build / not installed");
            RefreshButtons();
        }
        private void SaveSettings()
        {
            try { UpdateCore.SaveState(root, state); }
            catch (Exception ex) { status.Text = "Could not save settings: " + ex.Message; }
        }
        private void RefreshButtons()
        {
            play.Enabled = !busy && File.Exists(Path.Combine(UpdateCore.GameDirectory(root, state), "Eclipse.exe"));
            update.Enabled = rollback.Enabled = channel.Enabled = automatic.Enabled = !busy;
            rollback.Enabled &= !string.IsNullOrEmpty(state.current) &&
                File.Exists(Path.Combine(string.IsNullOrEmpty(state.previous) ? root : UpdateCore.VersionDirectory(root, state.previous), "Eclipse.exe"));
            update.Text = candidate == null ? "Check for updates" : "Install " + candidate.version;
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
                status.Text = candidate == null ? "No newer update available." :
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
                await Task.Run(() => game.WaitForExit());
                if (game.ExitCode != 0) { await Rollback(); status.Text = "Game exited with an error. Previous build restored when available."; }
                else status.Text = "Installed: " + (state.current ?? "existing build");
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

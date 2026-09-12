const fs = require('node:fs');
const path = require('node:path');
const { spawn } = require('node:child_process');
const root = path.resolve(__dirname, '..');
const executable = process.argv[2];
if (!executable) throw new Error('Pass the absolute path to Code.exe (or the VS Code executable on your platform).');
const installed = process.argv.includes('--installed')
    ? fs.readdirSync(path.join(root, '.test-runtime/extensions')).find(n => n === 'eclipse-modding.eclipse-modding-preview-0.1.0') : undefined;
if (process.argv.includes('--installed') && !installed) throw new Error('Install the 0.1.0 VSIX in the isolated extensions directory first.');
const workspace = path.join(root, '.test-runtime/vscode-workspace');
// A reused profile can restore unsaved quick-fix edits from the previous run.
// Keep the installed extensions shared, but isolate editor/session state.
fs.mkdirSync(path.join(root, '.test-runtime'), { recursive: true });
const profile = fs.mkdtempSync(path.join(root, '.test-runtime/vscode-profile-'));
fs.mkdirSync(path.join(profile, 'User'), { recursive: true });
fs.writeFileSync(path.join(profile, 'User/settings.json'), JSON.stringify({
    'extensions.autoUpdate': false,
    'extensions.autoCheckUpdates': false,
    'update.mode': 'none',
    'telemetry.telemetryLevel': 'off',
}));
fs.mkdirSync(path.join(workspace, '.vscode'), { recursive: true });
fs.cpSync(path.join(root, 'templates/weapon'), workspace, { recursive: true });
fs.writeFileSync(path.join(workspace, 'probe.lua'), 'local sf2 = require("sf2")\nsf2.items.\n');
fs.writeFileSync(path.join(workspace, '.vscode/settings.json'), JSON.stringify({
    'Lua.workspace.checkThirdParty': false,
    'Lua.runtime.version': 'Lua 5.2',
}));
const child = spawn(executable, [
    workspace,
    '--user-data-dir', profile,
    '--extensions-dir', path.join(root, '.test-runtime/extensions'),
    '--extensionDevelopmentPath', installed ? path.join(root, '.test-runtime/extensions', installed) : root,
    '--extensionTestsPath', path.join(root, 'test/vscode.cjs'),
    '--disable-workspace-trust', '--skip-welcome', '--skip-release-notes', '--disable-updates',
], { windowsHide: true, stdio: 'inherit' });
const timer = setTimeout(() => { child.kill(); console.error('VS Code integration test timed out.'); process.exitCode = 1; }, 90000);
child.on('error', error => { clearTimeout(timer); console.error(error); process.exitCode = 1; });
child.on('exit', code => { clearTimeout(timer); process.exitCode = code ?? 1; });

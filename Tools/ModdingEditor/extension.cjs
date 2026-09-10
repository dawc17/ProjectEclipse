const vscode = require('vscode');
const fs = require('node:fs/promises');
const path = require('node:path');
const jsonc = require('jsonc-parser');
const { createMod } = require('./src/scaffold.cjs');

async function selectFolder() {
    const folders = vscode.workspace.workspaceFolders ?? [];
    if (!folders.length) {
        vscode.window.showInformationMessage('Open your mod folder in VS Code first, then run this command again.');
        return;
    }
    const active = vscode.window.activeTextEditor?.document.uri;
    const current = active && vscode.workspace.getWorkspaceFolder(active);
    return current ?? (folders.length === 1 ? folders[0] : await vscode.window.showWorkspaceFolderPick());
}

async function activate(context) {
    const library = vscode.Uri.joinPath(context.extensionUri, 'library').fsPath;
    const enabled = folder => vscode.workspace.getConfiguration('eclipseModding', folder.uri).get('enabled', true);
    // LuaLS gives .luarc.json precedence over editor settings. Update it when present.
    async function configure(folder, enable) {
        const key = `library:${folder.uri.toString()}`;
        const previous = context.workspaceState.get(key);
        const config = vscode.workspace.getConfiguration('Lua', folder.uri);
        const clean = existing => {
            if (!Array.isArray(existing)) throw new Error('Lua workspace.library must be an array.');
            const next = existing.filter(entry => entry !== library && entry !== previous);
            if (enable) next.push(library);
            return next;
        };
        for (const name of ['.luarc.json', '.luarc.jsonc']) {
            const uri = vscode.Uri.joinPath(folder.uri, name);
            try { await fs.access(uri.fsPath); } catch (e) { if (e.code === 'ENOENT') continue; throw e; }
            const document = await vscode.workspace.openTextDocument(uri);
            const errors = [], text = document.getText(), data = jsonc.parse(text, errors, { allowTrailingComma: true });
            if (errors.length || !data || typeof data !== 'object') throw new Error(`Fix invalid JSON in ${name} before enabling Eclipse Modding.`);
            const property = Object.hasOwn(data, 'workspace.library') ? ['workspace.library'] : ['workspace', 'library'];
            const existing = property.length === 1 ? data['workspace.library'] : data.workspace?.library;
            const edits = jsonc.modify(text, property, clean(existing ?? []), { formattingOptions: { insertSpaces: true, tabSize: 2 } });
            const edit = new vscode.WorkspaceEdit();
            for (const e of edits) edit.replace(uri, new vscode.Range(document.positionAt(e.offset), document.positionAt(e.offset + e.length)), e.content);
            if (!await vscode.workspace.applyEdit(edit)) throw new Error(`Could not update ${name}.`);
            if (!await document.save()) throw new Error(`Could not save ${name}. Resolve its save conflict, then run Enable again.`);
        }
        await config.update('workspace.library', clean(config.get('workspace.library', [])), vscode.ConfigurationTarget.WorkspaceFolder);
        await context.workspaceState.update(key, enable ? library : undefined);
        await vscode.workspace.getConfiguration('eclipseModding', folder.uri).update('enabled', enable, vscode.ConfigurationTarget.WorkspaceFolder);
    }
    for (const enable of [true, false]) {
        context.subscriptions.push(vscode.commands.registerCommand(
            `eclipseModding.${enable ? 'enable' : 'disable'}`,
            async () => {
                const folder = await selectFolder();
                if (!folder) return;
                try { await configure(folder, enable); }
                catch (error) { await vscode.window.showErrorMessage(error.message); return; }
                vscode.window.showInformationMessage(enable
                    ? 'Eclipse Modding enabled. Use local sf2 = require("sf2"), then type sf2. to explore the API.'
                    : 'Eclipse Modding disabled for this folder.');
            }
        ));
    }
    for (const folder of vscode.workspace.workspaceFolders ?? []) {
        const previous = context.workspaceState.get(`library:${folder.uri.toString()}`);
        if (!previous && vscode.workspace.getConfiguration('Lua', folder.uri).get('workspace.library', []).includes(library)) {
            await context.workspaceState.update(`library:${folder.uri.toString()}`, library);
        }
        if (previous && previous !== library && enabled(folder)) {
            try { await configure(folder, true); } catch (e) { vscode.window.showWarningMessage(`Eclipse Modding: ${e.message}`); }
        }
    }
    const providers = require('./src/providers.cjs').register(context, enabled);
    context.subscriptions.push(
        vscode.commands.registerCommand('eclipseModding.validate', async () => { await providers.refresh(); await vscode.commands.executeCommand('workbench.actions.view.problems'); }),
        vscode.commands.registerCommand('eclipseModding.docs', () => vscode.env.openExternal(vscode.Uri.parse('https://dawc17.github.io/ProjectEclipse/'))),
        vscode.commands.registerCommand('eclipseModding.createMod', async () => {
            const id = await vscode.window.showInputBox({ title: 'Create Eclipse Mod', prompt: 'Unique lowercase mod ID (also the new folder name)', placeHolder: 'myname.training-blade', validateInput: value => /^[a-z0-9][a-z0-9_.-]*$/.test(value) && !['core', 'sf2de'].includes(value) ? undefined : 'Use lowercase letters, numbers, dots, underscores, or hyphens.' });
            if (!id) return;
            const name = await vscode.window.showInputBox({ prompt: 'Mod name shown to players', value: 'My Training Blade' }); if (!name?.trim()) return;
            const author = await vscode.window.showInputBox({ prompt: 'Your author name' }); if (!author?.trim()) return;
            const parent = await vscode.window.showOpenDialog({ canSelectFolders: true, canSelectFiles: false, canSelectMany: false, openLabel: 'Create mod here' }); if (!parent?.length) return;
            try {
                const folder = await createMod(path.join(context.extensionUri.fsPath, 'templates', 'weapon'), parent[0].fsPath, id, name, author);
                await fs.mkdir(path.join(folder, '.vscode'));
                await fs.writeFile(path.join(folder, '.vscode', 'settings.json'), JSON.stringify({ 'Lua.runtime.version': 'Lua 5.2', 'Lua.workspace.library': [library], 'eclipseModding.enabled': true }, null, 2) + '\n');
                await vscode.commands.executeCommand('vscode.openFolder', vscode.Uri.file(folder), true);
            } catch (error) { vscode.window.showErrorMessage(`Could not create mod: ${error.message}`); }
        }));
    return providers;
}

module.exports = { activate };

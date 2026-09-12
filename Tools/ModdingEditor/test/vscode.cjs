const vscode = require('vscode');
const assert = require('node:assert/strict');
const path = require('node:path');
const fs = require('node:fs');

exports.run = async function () {
    const root = path.resolve(__dirname, '..');
    const report = path.join(root, '.test-runtime/vscode-result.txt');
    const passed = [];
    try {
        const folder = vscode.workspace.workspaceFolders[0];
        const config = vscode.workspace.getConfiguration('Lua', folder.uri);
        const otherLibrary = path.join(folder.uri.fsPath, 'other-library');
        fs.mkdirSync(otherLibrary, { recursive: true });
        await config.update('workspace.library', [otherLibrary], vscode.ConfigurationTarget.WorkspaceFolder);
        const extension = vscode.extensions.getExtension('eclipse-modding.eclipse-modding-preview');
        assert(extension, 'Preview extension was not loaded');
        const library = path.join(extension.extensionUri.fsPath, 'library');
        await extension.activate();
        await vscode.commands.executeCommand('eclipseModding.enable');
        await vscode.commands.executeCommand('eclipseModding.enable');
        const expected = [otherLibrary, library];
        assert.deepEqual(vscode.workspace.getConfiguration('Lua', folder.uri).get('workspace.library'), expected);
        passed.push('PASS: enable is idempotent and preserves other libraries');

        const document = await vscode.workspace.openTextDocument(vscode.Uri.joinPath(folder.uri, 'probe.lua'));
        await vscode.window.showTextDocument(document);
        let found = false;
        const deadline = Date.now() + 45000;
        while (Date.now() < deadline) {
            const result = await vscode.commands.executeCommand('vscode.executeCompletionItemProvider', document.uri, new vscode.Position(1, 10));
            if (result?.items.some(item => String(typeof item.label === 'string' ? item.label : item.label.label).startsWith('register_weapon'))) {
                found = true;
                break;
            }
            await new Promise(resolve => setTimeout(resolve, 500));
        }
        assert(found, 'VS Code did not offer register_weapon through the installed Lua extension');
        passed.push('PASS: real VS Code Lua extension provides weapon completion after enabling the preview');

        const uri = vscode.Uri.joinPath(folder.uri, 'scripts', 'editor-test.lua');
        fs.writeFileSync(uri.fsPath, 'local sf2 = require("sf2")\nsf2.assets.sprite("sprites/weapon")\nsf2.localization.key("weapon.training_blade")\n');
        const modDoc = await vscode.workspace.openTextDocument(uri);
        await vscode.window.showTextDocument(modDoc);
        const refs = await vscode.commands.executeCommand('vscode.executeCompletionItemProvider', uri, new vscode.Position(1, 22));
        assert(refs.items.some(i => i.label === 'sprites/weapon'), 'Missing real sprite completion');
        const definitions = await vscode.commands.executeCommand('vscode.executeDefinitionProvider', uri, new vscode.Position(1, 24));
        assert(definitions.some(d => (d.uri ?? d.targetUri).fsPath.endsWith('weapon.asset')), 'Sprite definition navigation failed');
        const hover = await vscode.commands.executeCommand('vscode.executeHoverProvider', uri, new vscode.Position(2, 27));
        assert(hover.some(h => h.contents.some(c => (c.value ?? String(c)).replaceAll('&nbsp;', ' ').includes('Training Blade'))), 'Localization hover missing: ' + JSON.stringify(hover.map(h => h.contents.map(c => c.value))));
        passed.push('PASS: local asset completion, go-to-definition, and localization hover');

        const edit = new vscode.WorkspaceEdit();
        edit.insert(uri, new vscode.Position(3, 0), 'sf2.behaviors.register { id="test", on_damage_received=function(params, fighter, event) fighter:change_health(5) end }\n');
        await vscode.workspace.applyEdit(edit);
        await extension.exports.refresh();
        // A debounced document-open/change refresh can supersede the explicit
        // refresh. Wait for the published diagnostic, not one particular run.
        let issues = [];
        const diagnosticDeadline = Date.now() + 10000;
        do {
            issues = vscode.languages.getDiagnostics(uri).filter(d => d.source === 'Eclipse Modding');
            if (issues.some(d => d.code === 'capability:combat.change_life')) break;
            await new Promise(resolve => setTimeout(resolve, 100));
        } while (Date.now() < diagnosticDeadline);
        assert(issues.some(d => d.code === 'capability:combat.change_life'), 'Missing capability diagnostic');
        const fixes = await vscode.commands.executeCommand('vscode.executeCodeActionProvider', uri, issues[0].range);
        const fix = fixes.find(f => f.title === 'Declare combat.change_life in mod.toml');
        assert(fix?.edit, 'Missing capability quick fix');
        await vscode.workspace.applyEdit(fix.edit);
        await extension.exports.refresh();
        assert(!vscode.languages.getDiagnostics(uri).some(d => d.code === 'capability:combat.change_life'));
        passed.push('PASS: missing capability diagnostics and manifest quick fix work with unsaved edits');

        const randomEdit=new vscode.WorkspaceEdit();
        randomEdit.insert(uri,new vscode.Position(modDoc.lineCount,0),'\nsf2.random.integer("route",1,3)\n');
        await vscode.workspace.applyEdit(randomEdit);
        await extension.exports.refresh();
        for(const cap of ['state.read','state.write']) {
            const issue=vscode.languages.getDiagnostics(uri).find(d=>d.code===`capability:${cap}`);
            assert(issue,`Missing independent random capability diagnostic: ${cap}`);
            const actions=await vscode.commands.executeCommand('vscode.executeCodeActionProvider',uri,issue.range);
            const action=actions.find(f=>f.title===`Declare ${cap} in mod.toml`);
            assert(action?.edit,`Missing random capability quick fix: ${cap}`);
            await vscode.workspace.applyEdit(action.edit);
            await extension.exports.refresh();
            assert(!vscode.languages.getDiagnostics(uri).some(d=>d.code===`capability:${cap}`));
        }
        passed.push('PASS: both random stream capabilities have independent working manifest quick fixes');

        const schemaUri = vscode.Uri.joinPath(folder.uri, 'scripts', 'schema-test.lua');
        fs.writeFileSync(schemaUri.fsPath, 'local sf2=require("sf2")\nsf2.behaviors.register { id="hits", state={fields={hits={type="integer",default=0}}}, on_damage_dealt=function(self, fighter, event)\n local value=self.state.\nend }');
        await vscode.workspace.openTextDocument(schemaUri);
        const keys = await vscode.commands.executeCommand('vscode.executeCompletionItemProvider', schemaUri, new vscode.Position(2, 24));
        assert(keys.items.some(i => i.label === 'hits'), 'Declared state key completion missing');
        passed.push('PASS: state schema keys complete inside inline callbacks');

        const luarcUri = vscode.Uri.joinPath(folder.uri, '.luarc.json');
        if (!fs.existsSync(luarcUri.fsPath)) fs.writeFileSync(luarcUri.fsPath, '{}');
        const luarcDoc = await vscode.workspace.openTextDocument(luarcUri);
        const luarcEdit = new vscode.WorkspaceEdit();
        luarcEdit.replace(luarcUri, new vscode.Range(luarcDoc.positionAt(0), luarcDoc.positionAt(luarcDoc.getText().length)), '// Keep this comment\n{ "workspace": { "library": ["other-library"] }, "runtime": { "version": "Lua 5.2" } }\n');
        await vscode.workspace.applyEdit(luarcEdit);
        await luarcDoc.save();
        await vscode.commands.executeCommand('eclipseModding.enable');
        const luarc = fs.readFileSync(path.join(folder.uri.fsPath, '.luarc.json'), 'utf8');
        assert(luarc.includes('// Keep this comment') && luarc.includes('other-library') && luarc.includes('library'));
        assert(require('jsonc-parser').parse(luarc).workspace.library.includes(library));
        passed.push('PASS: .luarc.json settings and comments are preserved');

        await vscode.commands.executeCommand('eclipseModding.disable');
        assert.deepEqual(vscode.workspace.getConfiguration('Lua', folder.uri).get('workspace.library'), [otherLibrary]);
        assert.deepEqual(require('jsonc-parser').parse(fs.readFileSync(path.join(folder.uri.fsPath, '.luarc.json'), 'utf8')).workspace.library, ['other-library']);
        passed.push('PASS: disable removes only the preview library');
        fs.writeFileSync(report, passed.join('\n') + '\n');
        console.log(passed.join('\n'));
    } catch (error) {
        fs.writeFileSync(report, passed.join('\n') + '\nFAIL: ' + error.stack);
        throw error;
    }
};

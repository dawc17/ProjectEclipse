const vscode = require('vscode');
const fs = require('node:fs/promises');
const path = require('node:path');
const project = require('./project.cjs');
const api = require('../data/api.json');

function register(context, isEnabled) {
    const diagnostics = vscode.languages.createDiagnosticCollection('Eclipse Modding');
    const cache = new Map();
    const selector = { language: 'lua', scheme: 'file' };
    let generation = 0, timer;
    const read = async file => vscode.workspace.textDocuments.find(d => d.uri.fsPath.toLowerCase() === file.toLowerCase())?.getText() ?? fs.readFile(file, 'utf8');
    async function getMod(document) {
        const folder = vscode.workspace.getWorkspaceFolder(document.uri);
        if (!folder || !isEnabled(folder)) return;
        let root = path.dirname(document.uri.fsPath);
        while (root === folder.uri.fsPath || root.startsWith(folder.uri.fsPath + path.sep)) {
            try {
                await fs.access(path.join(root, 'mod.toml'));
                if (!cache.has(root)) cache.set(root, project.indexMod(root, read).catch(e => { cache.delete(root); throw e; }));
                return await cache.get(root);
            } catch (e) { if (e.code !== 'ENOENT') throw e; }
            root = path.dirname(root);
        }
    }
    const range = (document, offsets) => new vscode.Range(document.positionAt(offsets[0]), document.positionAt(offsets[1]));
    async function refresh() {
        const current = ++generation;
        cache.clear();
        const results = new Map();
        const mods = new Map();
        for (const document of vscode.workspace.textDocuments) {
            if (document.uri.scheme !== 'file' || !/\.(lua|toml|asset)$/.test(document.fileName)) continue;
            const mod = await getMod(document);
            if (!mod) continue;
            mods.set(mod.root, mod);
            if (document.languageId !== 'lua') continue;
            const list = project.analyze(document.getText(), mod).issues.map(issue => {
                const d = new vscode.Diagnostic(range(document, issue.range), issue.message, vscode.DiagnosticSeverity.Warning);
                d.source = 'Eclipse Modding'; d.code = issue.capability ? `capability:${issue.capability}` : issue.code;
                return d;
            });
            results.set(document.uri.toString(), list);
        }
        for (const mod of mods.values()) for (const issue of mod.issues) {
            const uri = vscode.Uri.file(issue.file);
            const document = /\.(toml|asset)$/.test(issue.file) ? await vscode.workspace.openTextDocument(uri) : undefined;
            const line = document ? Math.min(issue.line ?? 0, document.lineCount - 1) : 0;
            const d = new vscode.Diagnostic(document?.lineAt(line).range ?? new vscode.Range(0, 0, 0, 0), issue.message, vscode.DiagnosticSeverity.Warning);
            d.source = 'Eclipse Modding';
            const list = results.get(uri.toString()) ?? []; list.push(d); results.set(uri.toString(), list);
        }
        if (current !== generation) return;
        diagnostics.clear();
        for (const [uri, list] of results) diagnostics.set(vscode.Uri.parse(uri), list);
    }
    function schedule() {
        ++generation; cache.clear(); clearTimeout(timer);
        timer = setTimeout(() => refresh().catch(e => output.appendLine(`Index: ${e.message}`)), 250);
    }
    const output = vscode.window.createOutputChannel('Eclipse Modding');
    function referenceAt(document, position, mod) {
        const offset = document.offsetAt(position);
        return project.analyze(document.getText(), mod).calls.find(c => api.functions[c.name]?.referenceKind && c.args[0]?.range[0] <= offset && c.args[0]?.range[1] >= offset);
    }
    function target(call, mod) {
        let id = project.literal(call?.args[0]); if (typeof id !== 'string') return;
        if (id.includes(':')) { const [owner, rest] = id.split(':'); if (owner !== mod.data.id) return; id = rest; }
        return api.functions[call.name].referenceKind === 'localization' ? mod.localizations.get(id.replace(/^localization\//, '')) : mod.assets.get(id.toLowerCase());
    }
    context.subscriptions.push(output, diagnostics,
        vscode.languages.registerCompletionItemProvider(selector, {
            async provideCompletionItems(document, position) {
                const mod = await getMod(document); if (!mod) return;
                const text = document.getText(), offset = document.offsetAt(position);
                const at = project.completionContext(text, offset, mod);
                if (at?.kind) {
                    const records = at.kind === 'localization' ? [...mod.localizations.values()] : [...mod.assets.values()].filter(a => a.kind === at.kind);
                    const qualified = at.prefix.includes(':');
                    return records.map(record => {
                        const label = qualified ? `${mod.data.id}:${at.kind === 'localization' ? 'localization/' : ''}${record.id}` : record.id;
                        const item = new vscode.CompletionItem(label, vscode.CompletionItemKind.File);
                        item.range = new vscode.Range(document.positionAt(at.start), position);
                        item.detail = `${at.kind} in ${mod.data.id}`;
                        item.documentation = record.translations?.map(t => `${t.language}: ${t.value}`).join('\n');
                        return item;
                    });
                }
                // Schema-backed parameter/state keys inside inline behavior callbacks.
                const member = /([A-Za-z_][\w]*(?:\.(?:state|params))?)\.([\w]*)$/.exec(text.slice(0, offset));
                if (!member) return;
                const start = offset - member[2].length;
                const repaired = text.slice(0, start) + '__completion' + text.slice(offset);
                const ctx = project.analyze(repaired, mod).contexts.find(c => c.range[0] <= offset && c.range[1] >= offset);
                if (!ctx) return;
                const schema = member[1] === `${ctx.parameters}.state` && ctx.stateful ? ctx.stateSchema
                    : member[1] === (ctx.stateful ? `${ctx.parameters}.params` : ctx.parameters) ? ctx.schema : undefined;
                if (!schema) return;
                return Object.entries(schema).map(([name, node]) => {
                    const item = new vscode.CompletionItem(name, vscode.CompletionItemKind.Field);
                    item.range = new vscode.Range(document.positionAt(start), position);
                    item.detail = project.literal(node) ?? project.literal(project.fields(node).type) ?? 'Declared behavior field';
                    return item;
                });
            }
        }, '"', "'", '/', ':', '.'),
        vscode.languages.registerDefinitionProvider(selector, {
            async provideDefinition(document, position) {
                const mod = await getMod(document); if (!mod) return;
                const found = target(referenceAt(document, position, mod), mod); if (!found) return;
                return (found.translations ?? [found]).map(t => new vscode.Location(vscode.Uri.file(t.file), new vscode.Position(t.line ?? 0, 0)));
            }
        }),
        vscode.languages.registerHoverProvider(selector, {
            async provideHover(document, position) {
                const mod = await getMod(document); if (!mod) return;
                const found = target(referenceAt(document, position, mod), mod); if (!found) return;
                const markdown = new vscode.MarkdownString();
                markdown.appendText(found.translations ? found.translations.map(t => `${t.language}: ${t.value}`).join('\n\n') : `${found.kind}: ${path.relative(mod.root, found.file)}`);
                return new vscode.Hover(markdown);
            }
        }),
        vscode.languages.registerCodeActionsProvider(selector, {
            async provideCodeActions(document, _range, actionContext) {
                const mod = await getMod(document); if (!mod) return;
                const manifestUri = vscode.Uri.file(path.join(mod.root, 'mod.toml'));
                const manifestDocument = await vscode.workspace.openTextDocument(manifestUri);
                const lineNumber = mod.positions.capabilities; if (lineNumber === undefined) return;
                const line = manifestDocument.lineAt(lineNumber);
                const match = /^(\s*capabilities\s*=\s*)(\[[^\r\n]*\])(\s*(?:#.*)?)$/.exec(line.text);
                if (!match || !Array.isArray(mod.data.capabilities)) return;
                return [...new Set(actionContext.diagnostics.filter(d => d.source === 'Eclipse Modding' && String(d.code).startsWith('capability:')).map(d => String(d.code).slice(11)))].map(capability => {
                    const action = new vscode.CodeAction(`Declare ${capability} in mod.toml`, vscode.CodeActionKind.QuickFix);
                    action.edit = new vscode.WorkspaceEdit();
                    action.edit.replace(manifestUri, line.range, match[1] + JSON.stringify([...new Set([...mod.data.capabilities, capability])]) + match[3]);
                    return action;
                });
            }
        }, { providedCodeActionKinds: [vscode.CodeActionKind.QuickFix] }),
        vscode.workspace.onDidOpenTextDocument(schedule), vscode.workspace.onDidCloseTextDocument(schedule),
        vscode.workspace.onDidChangeTextDocument(schedule), vscode.workspace.onDidChangeConfiguration(schedule),
        vscode.workspace.onDidChangeWorkspaceFolders(schedule), { dispose() { clearTimeout(timer); ++generation; } });
    const watcher = vscode.workspace.createFileSystemWatcher('**/{mod.toml,assets/**,localizations/**,scripts/**}');
    context.subscriptions.push(watcher, watcher.onDidChange(schedule), watcher.onDidCreate(schedule), watcher.onDidDelete(schedule));
    schedule();
    return { refresh, getMod };
}
module.exports = { register };

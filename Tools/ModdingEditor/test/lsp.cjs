// Exercise the actual LuaLS completion/hover/signature/diagnostic protocol.
// Usage: npm test -- /absolute/path/to/lua-language-server[.exe]
const { spawn } = require('node:child_process');
const fs = require('node:fs');
const path = require('node:path');
const { pathToFileURL } = require('node:url');
const assert = require('node:assert/strict');

const root = path.resolve(__dirname, '..');
const binary = process.argv[2] || path.join(root, '.test-runtime/luals-3.18.2/bin/lua-language-server.exe');
const workspace = path.join(root, '.test-runtime/workspace');
fs.mkdirSync(workspace, { recursive: true });
const config = {
    runtime: { version: 'Lua 5.2' },
    workspace: { library: [path.join(root, 'library')], checkThirdParty: false },
    diagnostics: { workspaceDelay: 0 },
    telemetry: { enable: false },
};
fs.writeFileSync(path.join(workspace, '.luarc.json'), JSON.stringify(config));
const server = spawn(binary, ['--logpath', path.join(root, '.test-runtime/log')], { windowsHide: true });
const pending = new Map();
const diagnostics = new Map();
let buffer = Buffer.alloc(0);
let sequence = 0;
let stderr = '';
server.stderr.on('data', chunk => { stderr += chunk; });
const send = message => {
    const body = Buffer.from(JSON.stringify({ jsonrpc: '2.0', ...message }));
    server.stdin.write(`Content-Length: ${body.length}\r\n\r\n`);
    server.stdin.write(body);
};
const notify = (method, params) => send({ method, params });
function request(method, params) {
    return new Promise((resolve, reject) => {
        const id = ++sequence;
        const timer = setTimeout(() => { pending.delete(id); reject(new Error(`Timed out: ${method}\n${stderr}`)); }, 20000);
        pending.set(id, { resolve, reject, timer });
        send({ id, method, params });
    });
}
server.stdout.on('data', chunk => {
    buffer = Buffer.concat([buffer, chunk]);
    while (true) {
        const end = buffer.indexOf('\r\n\r\n');
        if (end < 0) break;
        const length = Number(/Content-Length:\s*(\d+)/i.exec(buffer.subarray(0, end).toString())[1]);
        if (buffer.length < end + 4 + length) break;
        const message = JSON.parse(buffer.subarray(end + 4, end + 4 + length));
        buffer = buffer.subarray(end + 4 + length);
        if (message.method && message.id !== undefined) {
            let result = null;
            if (message.method === 'workspace/configuration') {
                result = message.params.items.map(({ section }) =>
                    !section || section === 'Lua' ? config : section.replace(/^Lua\./, '').split('.').reduce((v, k) => v?.[k], config));
            }
            send({ id: message.id, result });
        } else if (message.method === 'textDocument/publishDiagnostics') {
            diagnostics.set(decodeURIComponent(message.params.uri).toLowerCase(), message.params.diagnostics);
        } else if (pending.has(message.id)) {
            const entry = pending.get(message.id);
            pending.delete(message.id);
            clearTimeout(entry.timer);
            message.error ? entry.reject(new Error(JSON.stringify(message.error))) : entry.resolve(message.result);
        }
    }
});
const sleep = ms => new Promise(resolve => setTimeout(resolve, ms));
async function until(check, label) {
    const deadline = Date.now() + 20000;
    while (Date.now() < deadline) {
        const result = await check();
        if (result) return result;
        await sleep(250);
    }
    throw new Error(`Timed out waiting for ${label}`);
}
function open(name, text) {
    const uri = pathToFileURL(path.join(workspace, name)).href;
    fs.writeFileSync(path.join(workspace, name), text);
    notify('textDocument/didOpen', { textDocument: { uri, languageId: 'lua', version: 1, text } });
    return uri;
}
function probe(name, text) {
    const offset = text.indexOf('|');
    assert(offset >= 0);
    const before = text.slice(0, offset).split('\n');
    return { textDocument: { uri: open(name, text.replace('|', '')) }, position: { line: before.length - 1, character: before.at(-1).length } };
}
const labels = result => (result?.items ?? result ?? []).map(item => typeof item.label === 'string' ? item.label : item.label.label);

async function main() {
    const rootUri = pathToFileURL(workspace).href;
    await request('initialize', {
        processId: process.pid, rootUri, workspaceFolders: [{ uri: rootUri, name: 'Eclipse preview test' }],
        capabilities: { workspace: { configuration: true }, textDocument: { completion: { completionItem: { snippetSupport: true } }, hover: { contentFormat: ['markdown', 'plaintext'] } } },
    });
    notify('initialized', {});
    notify('workspace/didChangeConfiguration', { settings: { Lua: config } });

    const moduleProbe = probe('modules.lua', 'local sf2 = require("sf2")\nsf2.|');
    await until(async () => {
        const found = labels(await request('textDocument/completion', moduleProbe));
        return ['items', 'assets', 'localization', 'price', 'shop', 'log'].every(label => found.includes(label));
    }, 'module completion');
    fs.writeFileSync(path.join(root, '.test-runtime/modules.json'), JSON.stringify(await request('textDocument/completion', moduleProbe), null, 2));
    console.log('PASS: require("sf2") resolves and completes API modules');

    const functionProbe = probe('functions.lua', 'local sf2 = require("sf2")\nsf2.items.|');
    await until(async () => {
        const result = await request('textDocument/completion', functionProbe);
        fs.writeFileSync(path.join(root, '.test-runtime/completion.json'), JSON.stringify(result, null, 2));
        fs.writeFileSync(path.join(root, '.test-runtime/hover.json'), JSON.stringify(await request('textDocument/hover', { textDocument: functionProbe.textDocument, position: { line: 1, character: 5 } }), null, 2));
        return labels(result).some(label => label.startsWith('register_weapon'));
    }, 'function completion');
    console.log('PASS: weapon function completion');

    const fieldsProbe = probe('fields.lua', 'local sf2 = require("sf2")\nsf2.items.register_weapon {\n    |\n}');
    await until(async () => {
        const result = await request('textDocument/completion', fieldsProbe);
        fs.writeFileSync(path.join(root, '.test-runtime/fields.json'), JSON.stringify(result, null, 2));
        const fields = labels(result);
        return ['id', 'display_name', 'icon', 'model', 'subtype'].every(field => fields.some(label => label.replace(/\?$/, '') === field || label.startsWith(`${field} `)));
    }, 'weapon field completion');
    console.log('PASS: all five weapon table fields complete without manual type annotations');

    const hoverProbe = probe('hover.lua', 'local sf2 = require("sf2")\nlocal register = sf2.items.register_wea|pon');
    await until(async () => {
        const hover = JSON.stringify(await request('textDocument/hover', hoverProbe));
        return hover?.includes('content.register') && hover.includes('https://dawc17.github.io/ProjectEclipse/');
    }, 'hover documentation');
    console.log('PASS: hover includes capability guidance and a wiki link');

    const signatureProbe = probe('signature.lua', 'local sf2 = require("sf2")\nsf2.price.coins(|)');
    await until(async () => {
        const signature = JSON.stringify(await request('textDocument/signatureHelp', signatureProbe));
        return signature?.includes('amount') && signature.includes('integer');
    }, 'signature help');
    console.log('PASS: price signature help');

    const api = require('../data/api.json');
    const modules = [...new Set(Object.keys(api.functions).map(n => n.split('.')[1]))];
    for (const module of modules) {
        const query = probe(`module-${module}.lua`, `local sf2 = require("sf2")\nsf2.${module}.|`);
        const expected = [...Object.keys(api.functions), ...Object.keys(api.aliases), ...Object.keys(api.constants)]
            .filter(n => n.startsWith(`sf2.${module}.`)).map(n => n.split('.')[2]);
        await until(async () => {
            const found = labels(await request('textDocument/completion', query));
            return expected.every(name => found.some(label => label === name || label.startsWith(name + '(')));
        }, `all ${module} functions, aliases, and constants`);
    }
    console.log(`PASS: every function, alias, and constant completes across ${modules.length} API modules`);
    const ruleFields = probe('rule-fields.lua', 'local sf2=require("sf2")\nsf2.rules.behavior { | }');
    await until(async () => {
        const found = labels(await request('textDocument/completion', ruleFields));
        return ['behavior', 'parameters', 'target', 'rounds', 'mode'].every(key => found.some(value => value.startsWith(key)));
    }, 'battle behavior rule fields');
    const patchFields = probe('fight-patch-fields.lua', 'local sf2=require("sf2")\nsf2.fights.patch { | }');
    await until(async () => {
        const found = labels(await request('textDocument/completion', patchFields));
        return ['rules', 'append_rules', 'location', 'music'].every(key => found.some(value => value.startsWith(key)));
    }, 'fight patch rule and presentation fields');
    const callback = probe('callback.lua', 'local sf2=require("sf2")\nsf2.behaviors.register { id="test", on_damage_resolving=function(params, fighter, event)\n fighter:|\nend }');
    await until(async () => labels(await request('textDocument/completion', callback)).some(n => n.startsWith('scale_incoming_damage')), 'resolving fighter callback inference');
    const event = probe('event.lua', 'local sf2=require("sf2")\nsf2.behaviors.register { id="test", on_damage_received=function(params, fighter, event)\n local value=event.|\nend }');
    await until(async () => labels(await request('textDocument/completion', event)).includes('health_before'), 'damage event inference');
    const stateful = probe('stateful.lua', 'local sf2=require("sf2")\nsf2.behaviors.register { id="test", state={fields={hits={type="integer",default=0}}}, on_damage_received=function(self, fighter, event)\n local value=self.|\nend }');
    await until(async () => labels(await request('textDocument/completion', stateful)).includes('state'), 'stateful callback inference');
    console.log('PASS: inline callbacks infer fighter methods, damage events, and stateful self');
    const snapshot = probe('snapshot.lua', 'local sf2=require("sf2")\nsf2.behaviors.register { id="test", on_round_begin=function(_, fighter)\n local combat=fighter:snapshot()\n if combat then local value=combat.self.| end\nend }');
    await until(async () => {
        const found=labels(await request('textDocument/completion',snapshot));
        return ['health','max_health','health_bars','position'].every(name=>found.includes(name));
    }, 'snapshot return type inference');
    console.log('PASS: combat snapshot return type and fighter fields complete');

    const validText = fs.readFileSync(path.join(root, 'templates/weapon/scripts/main.lua'), 'utf8');
    // LuaLS does not publish an initial empty report. Introduce an error, then
    // fix it, so a cleared report positively confirms that diagnostics ran.
    const validUri = open('valid.lua', validText + '\nsf2.price.coins("temporary test error")\n');
    const validKey = decodeURIComponent(validUri).toLowerCase();
    await until(() => (diagnostics.get(validKey)?.length ?? 0) > 0, 'temporary diagnostic');
    notify('textDocument/didChange', { textDocument: { uri: validUri, version: 2 }, contentChanges: [{ text: validText }] });
    await until(() => diagnostics.get(validKey)?.length === 0, 'cleared sample diagnostics');
    console.log('PASS: complete first-weapon script has no diagnostics');
    const ruleText = fs.readFileSync(path.join(root, 'templates/battle-rules/scripts/main.lua'), 'utf8');
    const ruleUri = open('valid-rule.lua', ruleText + '\nsf2.price.coins("temporary test error")\n');
    const ruleKey = decodeURIComponent(ruleUri).toLowerCase();
    await until(() => (diagnostics.get(ruleKey)?.length ?? 0) > 0, 'temporary rule diagnostic');
    notify('textDocument/didChange', { textDocument: { uri: ruleUri, version: 2 }, contentChanges: [{ text: ruleText }] });
    await until(() => diagnostics.get(ruleKey)?.length === 0, 'cleared battle-rule diagnostics');
    console.log('PASS: complete snapshot-based battle rule has no diagnostics');
    const upgradeFields=probe('upgrade-fields.lua','local sf2=require("sf2")\nsf2.perks.register { upgrades = { { | } } }');
    await until(async()=>{
        const found=labels(await request('textDocument/completion',upgradeFields));
        return ['level','description','parameters'].every(name=>found.some(value=>value.startsWith(name)));
    },'perk upgrade entry fields');
    console.log('PASS: perk upgrade entries complete');
    const outgoing=probe('outgoing.lua','local sf2=require("sf2")\nsf2.behaviors.register { id="test",on_damage_dealing=function(_,fighter,event)\n fighter:|\nend }');
    await until(async()=>labels(await request('textDocument/completion',outgoing)).some(name=>name.startsWith('scale_outgoing_damage')),'outgoing fighter method inference');
    console.log('PASS: outgoing damage callbacks infer the scoped modifier');
    for (const [callback,field] of [['on_combo_changed','last_combo'],['on_style_changed','style_rank'],['on_tick','delta_frames']]) {
        const position=probe(callback+'.lua',`local sf2=require("sf2")\nsf2.behaviors.register { id="test",${callback}=function(_,fighter,event)\n local value=event.|\nend }`);
        await until(async()=>labels(await request('textDocument/completion',position)).includes(field),callback+' event inference');
    }
    console.log('PASS: native combo, style and tick callback fields complete');
    const uiNode=probe('ui-node.lua','local sf2=require("sf2")\nsf2.ui.open { id="menu",mount="menu",root={ | } }');
    await until(async()=>labels(await request('textDocument/completion',uiNode)).includes('kind'),'recursive UI node completion');
    const uiPlacement=probe('ui-placement.lua','local sf2=require("sf2")\nsf2.ui.open { id="hud",mount="hud",placement={ | } }');
    await until(async()=>{
        const result=await request('textDocument/completion',uiPlacement);
        fs.writeFileSync(path.join(root,'.test-runtime/ui-placement.json'),JSON.stringify(result,null,2));
        return labels(result).some(name=>name.startsWith('anchor'));
    },'UI placement completion');
    console.log('PASS: UI layout nodes complete from the open definition');
    const modeResult=probe('mode-result.lua','local sf2=require("sf2")\nsf2.modes.register { id="trial",fights={},on_result=function(result)\n local won=result.|\nend }');
    await until(async()=>labels(await request('textDocument/completion',modeResult)).includes('completions'),'mode result callback completion');
    const uiStyle=probe('ui-style.lua','local sf2=require("sf2")\nsf2.ui.open { id="menu",mount="menu",root={ id="root",kind="text",style={ | } } }');
    await until(async()=>labels(await request('textDocument/completion',uiStyle)).some(name=>name.startsWith('font_size')),'UI style completion');
    const closeDefinition=probe('ui-close.lua','local sf2=require("sf2")\nsf2.ui.open { id="menu",mount="menu", | }');
    await until(async()=>labels(await request('textDocument/completion',closeDefinition)).some(name=>name.startsWith('on_close')),'UI close callback completion');
    await until(async()=>labels(await request('textDocument/completion',closeDefinition)).some(name=>name.startsWith('on_change')),'UI change callback completion');
    const checkedSetter=probe('ui-checked.lua','local sf2=require("sf2")\nsf2.ui.|');
    await until(async()=>labels(await request('textDocument/completion',checkedSetter)).some(name=>name.startsWith('set_checked')),'UI checked setter completion');
    const chargeText=fs.readFileSync(path.join(root,'templates/charge-ui/scripts/main.lua'),'utf8');
    const chargeUri=open('charge-close.lua',chargeText+'\nsf2.ui.is_open("bad handle")');
    const chargeKey=decodeURIComponent(chargeUri).toLowerCase();
    await until(()=>(diagnostics.get(chargeKey)?.length??0)>0,'temporary UI handle diagnostic');
    notify('textDocument/didChange',{textDocument:{uri:chargeUri,version:2},contentChanges:[{text:chargeText}]});
    await until(()=>diagnostics.get(chargeKey)?.length===0,'cleared Charged Strike close callback diagnostics')
        .catch(error=>{throw new Error(error.message+'\n'+JSON.stringify(diagnostics.get(chargeKey)));});
    console.log('PASS: UI close callback completes and Charged Strike has no diagnostics');

    const random=probe('random.lua','local sf2=require("sf2")\nsf2.random.|');
    await until(async()=>{const found=labels(await request('textDocument/completion',random));return ['integer','number'].every(name=>found.some(label=>label.startsWith(name)));},'random stream functions');
    const seededText=fs.readFileSync(path.join(root,'templates/seeded-trial/scripts/main.lua'),'utf8');
    const seededUri=open('seeded.lua',seededText+'\nsf2.random.integer("route","invalid",3)');
    const seededKey=decodeURIComponent(seededUri).toLowerCase();
    await until(()=>(diagnostics.get(seededKey)?.length??0)>0,'random bound type diagnostic');
    notify('textDocument/didChange',{textDocument:{uri:seededUri,version:2},contentChanges:[{text:seededText}]});
    await until(()=>diagnostics.get(seededKey)?.length===0,'cleared seeded mode diagnostics');
    console.log('PASS: random functions complete and seeded trial has no diagnostics');

    for (const name of ['programmable-ai','generated-expedition','animated-arena','dojo-selector']) {
        const source=fs.readFileSync((name==='animated-arena'||name==='dojo-selector')
            ? path.join(root,'../../Mods/example.'+name+'/scripts/main.lua')
            : path.join(root,'templates',name,'scripts/main.lua'),'utf8');
        const uri=open(name+'.lua',source+'\nsf2.price.coins("temporary error")');
        const key=decodeURIComponent(uri).toLowerCase();
        await until(()=>(diagnostics.get(key)?.length??0)>0,name+' temporary diagnostic');
        notify('textDocument/didChange',{textDocument:{uri,version:2},contentChanges:[{text:source}]});
        await until(()=>diagnostics.get(key)?.length===0,name+' cleared diagnostics')
            .catch(error=>{throw new Error(error.message+'\n'+JSON.stringify(diagnostics.get(key)));});
    }
    const aiDecision=probe('ai-decision.lua','local sf2=require("sf2")\nsf2.tactics.register { id="brain",on_decide=function(memory,event)\n local value=event.|\nend }');
    await until(async()=>labels(await request('textDocument/completion',aiDecision)).includes('actions'),'AI decision completion');
    const prepare=probe('prepare.lua','local sf2=require("sf2")\nsf2.modes.register { id="mode",fights={},on_prepare=function(request,event)\n local value=event.|\nend }');
    await until(async()=>labels(await request('textDocument/completion',prepare)).includes('step'),'mode preparation completion');
    const body=probe('body.lua','local sf2=require("sf2")\nsf2.warriors.register { | }');
    await until(async()=>labels(await request('textDocument/completion',body)).some(name=>name.startsWith('body_model')),'character model completion');
    const attack=probe('attack.lua','local sf2=require("sf2")\nsf2.moves.register { intervals={{type="Attack",attack={ | }}} }');
    await until(async()=>labels(await request('textDocument/completion',attack)).some(name=>name.startsWith('edges')),'attack interval completion');
    const profileProbe=probe('profile-query.lua','local sf2=require("sf2")\nlocal item=sf2.items.get("core:items/weapon/weapon_nunchaku")\nlocal snapshot=sf2.profile.item(item)\nlocal value=snapshot.|');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',profileProbe));
        return ['present','owned','count','equipped'].every(field=>result.includes(field));
    },'profile item snapshot fields');
    const storyProbe=probe('story-event.lua','local sf2=require("sf2")\nsf2.story.on("purchase",function(event)\nlocal value=event.|\nend)');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',storyProbe));
        return ['kind','item','recipe','previous_level','level','scene'].every(field=>result.includes(field));
    },'story event callback fields');
    const curveProbe=probe('location-curve.lua','local sf2=require("sf2")\nsf2.locations.register { layers = { { images = { { motion_y = { points = { { | } } } } } } } }');
    await until(async()=>{
        const result=labels(await request('textDocument/completion',curveProbe));
        return ['period','value','ease'].every(field=>result.some(name=>name === field || name === field + '?' || name.startsWith(field + ' ')));
    },'location curve point inference');
    console.log('PASS: procedural workflow and AI examples have no diagnostics; character, attack, location curve and callback fields complete');

    const invalidUri = open('invalid.lua', [
        'local sf2 = require("sf2")',
        'sf2.items.register_weapon {',
        '    id = "bad",',
        '    display_name = sf2.localization.key("weapon.training_blade"),',
        '    icon = sf2.assets.model("wrong_kind"),',
        '    model = sf2.assets.model("core:gamedata/models/mdl_weapon_katana_ritual"),',
        '}',
        'sf2.price.coins("five")',
        'sf2.items.register_weapon { id = "missing_fields" }',
        'sf2.items.register_wepon {}',
    ].join('\n'));
    await until(() => (diagnostics.get(decodeURIComponent(invalidUri).toLowerCase())?.length ?? 0) >= 4, 'invalid sample diagnostics');
    const errors = diagnostics.get(decodeURIComponent(invalidUri).toLowerCase());
    fs.writeFileSync(path.join(root, '.test-runtime/diagnostics.json'), JSON.stringify(errors, null, 2));
    for (const line of [4, 7, 8, 9]) assert(errors.some(d => d.range.start.line === line), `Missing diagnostic at line ${line + 1}: ${JSON.stringify(errors)}`);
    console.log('PASS: wrong handle, wrong scalar type, missing fields, and misspelled function are diagnosed');
    console.log('All LuaLS integration checks passed. This verifies editor behavior, not game execution.');
    await request('shutdown', null);
    notify('exit');
}
server.on('error', error => { console.error(error); process.exitCode = 1; });
main().catch(error => { console.error(error); process.exitCode = 1; }).finally(() => {
    for (const entry of pending.values()) clearTimeout(entry.timer);
    server.kill();
});

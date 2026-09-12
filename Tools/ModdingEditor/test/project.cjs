const { test } = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const p = require('../src/project.cjs');
const { createMod } = require('../src/scaffold.cjs');
const template = path.resolve(__dirname, '../templates/weapon');
const header = 'local sf2 = require("sf2")\n';

for (const name of ['programmable-ai', 'generated-expedition']) test(name+' example and starter share a valid public contract', async () => {
    const directory=path.resolve(__dirname,'../../../Mods/example.'+name);
    const mod=await p.indexMod(directory);
    assert.deepEqual(mod.issues,[]);
    assert.deepEqual(p.analyze(await fs.readFile(path.join(directory,'scripts/main.lua'),'utf8'),mod).issues,[]);
    for (const file of ['scripts/main.lua','mod.toml','localizations/eng.toml'])
        assert.equal(await fs.readFile(path.resolve(__dirname,'../templates',name,file),'utf8'),await fs.readFile(path.join(directory,file),'utf8'));
});

test('clean starter indexes and validates, including table-call syntax', async () => {
    const mod = await p.indexMod(template);
    assert.deepEqual(mod.issues, []);
    assert.equal(mod.assets.get('sprites/weapon').kind, 'sprite');
    assert.equal(mod.localizations.get('weapon.training_blade').translations[0].language, 'eng');
    assert.deepEqual(p.analyze(await fs.readFile(path.join(template, 'scripts/main.lua'), 'utf8'), mod).issues, []);
});
test('references, dependencies, capability requirements, and numeric limits', async () => {
    const mod = await p.indexMod(template);
    const result = p.analyze(header + `
sf2.assets.sprite("missing")
sf2.assets.model("sprites/weapon")
sf2.assets.sprite("another.mod:sprites/weapon")
sf2.price.coins(-1)
sf2.price.coins(1.5)
sf2.price.coins(2147483648)
sf2.behaviors.register { id="x", on_damage_dealt=function(params, fighter, event)
    fighter:change_health(-5)
    fighter.opponent:change_health(-5)
    fighter:scale_incoming_damage(0.5)
end }
`, mod);
    for (const code of ['missing-reference', 'asset-kind', 'dependency', 'range', 'capability', 'callback-timing']) assert(result.issues.some(i => i.code === code), code);
    assert.equal(result.issues.filter(i => i.code === 'range').length, 3);
    assert(result.issues.some(i => i.capability === 'combat.target'));
});
test('aliases resolve; comments and shadowed locals produce no false API warnings', async () => {
    const mod = await p.indexMod(template);
    assert.equal(p.analyze(header + 'local a=sf2.assets\na.sprite("missing")', mod).issues[0].code, 'missing-reference');
    for (const text of ['-- sf2.assets.sprite("missing")', 'local sf2={}\nsf2.assets.sprite("missing")', 'local function f(sf2) sf2.assets.sprite("missing") end']) assert.deepEqual(p.analyze(header + text, mod).issues, []);
});
test('incomplete asset strings preserve AST context', async () => {
    const mod = await p.indexMod(template);
    for (const suffix of ['sf2.assets.sprite("spr', 'local a=sf2.assets\na.sprite("spr']) {
        const text = header + suffix, c = p.completionContext(text, text.length, mod);
        assert.equal(c?.kind, 'sprite'); assert.equal(c.prefix, 'spr');
    }
    assert.equal(p.completionContext(header + '-- sf2.assets.sprite("spr', (header + '-- sf2.assets.sprite("spr').length, mod), null);
});
test('manifest checks unsupported fields, unsafe paths, duplicates, and missing values', () => {
    const result = p.manifest('schema=2\nid="core"\nentrypoint="../main.lua"\nunknown="x"\nid="again"');
    for (const fragment of ['schema must', 'reserved', 'safe path', 'Unknown', 'Duplicate', 'Missing required']) assert(result.issues.some(i => i.message.includes(fragment)), fragment);
    for (const bad of ['../x', '/x', 'C:\\x', 'a/../x', 'a//x']) assert.equal(p.safe(bad), false);
    assert.equal(p.parseValue('["a", "b",]')[1], 'b');
    assert(!p.manifest('api=">=0.7.0-beta.1 <1.0"').issues.some(i => i.message.includes('comparison ranges')));
});
test('scaffold creates a valid mod, refuses overwrite, and prevents path escape', async t => {
    const root = await fs.mkdtemp(path.join(os.tmpdir(), 'eclipse-editor-'));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const destination = await createMod(template, root, 'test.blade', 'Test Blade', 'Test Author');
    const mod = await p.indexMod(destination);
    assert.deepEqual(mod.issues, []); assert.equal(mod.data.id, 'test.blade');
    await assert.rejects(createMod(template, root, 'test.blade', 'Again', 'Author'), { code: 'EEXIST' });
    await assert.rejects(createMod(template, root, '../escape', 'Escape', 'Author'));
    await fs.writeFile(path.join(destination, 'assets', 'bad.ogg'), 'test');
    assert((await p.indexMod(destination)).issues.some(i => i.message.includes('PCM16')));
});


test('battle rule starter is recognized by the authored API contract', async () => {
    const directory = path.resolve(__dirname, '../../../Mods/example.battle-rules');
    const mod = await p.indexMod(directory);
    assert.deepEqual(mod.issues, []);
    const result = p.analyze(await fs.readFile(path.join(directory, 'scripts/main.lua'), 'utf8'), mod);
    assert.deepEqual(result.issues, []);
    const api = require('../data/api.json');
    assert(api.functions['sf2.rules.behavior']);
    assert(api.types.Rule_behavior.fields.behavior);
});

test('core fight patch example validates with registered rule handles', async () => {
    const directory = path.resolve(__dirname, '../../../Mods/example.core-fight');
    const mod = await p.indexMod(directory);
    assert.deepEqual(mod.issues, []);
    assert.deepEqual(p.analyze(await fs.readFile(path.join(directory, 'scripts/main.lua'), 'utf8'), mod).issues, []);
    const api = require('../data/api.json');
    assert(api.types.FightPatch.fields['append_rules?']);
    assert(api.types.FightPatch.fields['warriors?']);
    assert(api.types.FightPatch.fields['reward_drops?']);
    assert(api.types.RewardDropPatch.fields.reward);
});

test('perk upgrade example validates its assets, localization and branch definitions', async () => {
    const directory = path.resolve(__dirname, '../../../Mods/example.perk-upgrades');
    const mod = await p.indexMod(directory);
    assert.deepEqual(mod.issues, []);
    assert.deepEqual(p.analyze(await fs.readFile(path.join(directory, 'scripts/main.lua'), 'utf8'), mod).issues, []);
});

test('outgoing rule example validates its callback and capability', async () => {
    const directory=path.resolve(__dirname,'../../../Mods/example.outgoing-rule');
    const mod=await p.indexMod(directory);
    assert.deepEqual(mod.issues,[]);
    assert.deepEqual(p.analyze(await fs.readFile(path.join(directory,'scripts/main.lua'),'utf8'),mod).issues,[]);
});

test('combo reserve example validates native activity callbacks', async () => {
    const directory=path.resolve(__dirname,'../../../Mods/example.combo-reserve');
    const mod=await p.indexMod(directory);
    assert.deepEqual(mod.issues,[]);
    assert.deepEqual(p.analyze(await fs.readFile(path.join(directory,'scripts/main.lua'),'utf8'),mod).issues,[]);
});

test('charge UI example validates owned handles and callback capability', async () => {
    const directory=path.resolve(__dirname,'../../../Mods/example.charge-ui');
    const mod=await p.indexMod(directory);
    assert.deepEqual(mod.issues,[]);
    assert.deepEqual(p.analyze(await fs.readFile(path.join(directory,'scripts/main.lua'),'utf8'),mod).issues,[]);
});
test('branching mode example validates its callback and roster', async () => {
    const directory=path.resolve(__dirname,'../../../Mods/example.branching-trial');
    const mod=await p.indexMod(directory);
    assert.deepEqual(mod.issues,[]);
    assert.deepEqual(p.analyze(await fs.readFile(path.join(directory,'scripts/main.lua'),'utf8'),mod).issues,[]);
});

test('saved random streams require both capabilities and seeded starter validates', async () => {
    const directory=path.resolve(__dirname,'../../../Mods/example.seeded-trial');
    const mod=await p.indexMod(directory);
    assert.deepEqual(mod.issues,[]);
    const source=await fs.readFile(path.join(directory,'scripts/main.lua'),'utf8');
    assert.deepEqual(p.analyze(source,mod).issues,[]);
    const template=path.resolve(__dirname,'../templates/seeded-trial');
    for(const file of ['scripts/main.lua','mod.toml','localizations/eng.toml'])
        assert.equal(await fs.readFile(path.join(template,file),'utf8'),await fs.readFile(path.join(directory,file),'utf8'));
    for(const held of [[],['state.read'],['state.write'],['state.read','state.write']]) {
        const missing=p.analyze(header+"sf2.random.integer('route',1,3)\nsf2.random.number('route')",{...mod,data:{...mod.data,capabilities:held}}).issues;
        for(const cap of ['state.read','state.write'])
            assert.equal(missing.filter(i=>i.capability===cap).length,held.includes(cap)?0:2);
    }
});


test('quest suppression requires patch capability and example validates', async () => {
    const mod=await p.indexMod(path.resolve(__dirname,'../../../Mods/example.quest-suppression'));
    assert.deepEqual(mod.issues,[]);
    assert.deepEqual(p.analyze(await fs.readFile(path.resolve(__dirname,'../../../Mods/example.quest-suppression/scripts/main.lua'),'utf8'),mod).issues,[]);
    const starter=await p.indexMod(template);
    const issues=p.analyze(header+'sf2.quests.suppress { target="core:quests/quests.xml/example" }',starter).issues;
    assert(issues.some(i=>i.message.includes('content.patch')));
});


test('animated arena validates typed curve authoring', async () => {
 const dir=path.resolve(__dirname,'../../../Mods/example.animated-arena');
 const mod=await p.indexMod(dir);assert.deepEqual(mod.issues,[]);
 assert.deepEqual(p.analyze(await fs.readFile(path.join(dir,'scripts/main.lua'),'utf8'),mod).issues,[]);
});

test('dojo selector validates presentation capability and owned UI', async () => {
 const dir=path.resolve(__dirname,'../../../Mods/example.dojo-selector');
 const mod=await p.indexMod(dir);assert.deepEqual(mod.issues,[]);
 assert.deepEqual(p.analyze(await fs.readFile(path.join(dir,'scripts/main.lua'),'utf8'),mod).issues,[]);
 const starter=await p.indexMod(template);
 assert(p.analyze(header+'sf2.locations.reset_dojo()',starter).issues.some(i=>i.message.includes('presentation.dojo')));
});

test('profile queries require read capability', async () => {
 const mod=await p.indexMod(template);
 for(const call of ['sf2.profile.level()','sf2.profile.item(item)'])
  assert(p.analyze(header+call,mod).issues.some(i=>i.message.includes('profile.read')));
});

test('story subscriptions require event capability', async () => {
 const mod=await p.indexMod(template);
 for(const call of ['sf2.story.on("purchase", function(event) end)','sf2.story.off(subscription)','sf2.story.is_active(subscription)'])
  assert(p.analyze(header+call,mod).issues.some(i=>i.message.includes('story.events')));
});

test('story observer example validates', async () => {
 const dir=path.resolve(__dirname,'../../../Mods/example.story-observer');
 const mod=await p.indexMod(dir);assert.deepEqual(mod.issues,[]);
 assert.deepEqual(p.analyze(await fs.readFile(path.join(dir,'scripts/main.lua'),'utf8'),mod).issues,[]);
});

test('scene navigation capability and example validate', async () => {
 const starter=await p.indexMod(template);
 assert(p.analyze(header+'sf2.scenes.open("shop")',starter).issues.some(i=>i.message.includes('presentation.navigate')));
 const dir=path.resolve(__dirname,'../../../Mods/example.scene-menu');
 const mod=await p.indexMod(dir);assert.deepEqual(mod.issues,[]);
 assert.deepEqual(p.analyze(await fs.readFile(path.join(dir,'scripts/main.lua'),'utf8'),mod).issues,[]);
});


test('Eclipse reward example validates its manifest and reward patch', async () => {
 const dir=path.resolve(__dirname,'../../../Mods/example.eclipse-reward');
 const mod=await p.indexMod(dir);assert.deepEqual(mod.issues,[]);
 assert.deepEqual(p.analyze(await fs.readFile(path.join(dir,'scripts/main.lua'),'utf8'),mod).issues,[]);
});

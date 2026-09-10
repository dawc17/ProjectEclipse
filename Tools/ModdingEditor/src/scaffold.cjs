const fs = require('node:fs/promises');
const path = require('node:path');

async function createMod(template, parent, id, name, author) {
    if (!/^[a-z0-9][a-z0-9_.-]*$/.test(id) || ['core', 'sf2de'].includes(id)) throw new Error('Choose a lowercase mod ID such as myname.training-blade.');
    if (!name?.trim() || !author?.trim()) throw new Error('A mod name and author are required.');
    const target = path.join(parent, id);
    // Exclusive creation: an existing mod must never be overwritten.
    await fs.mkdir(target);
    for (const entry of await fs.readdir(template)) {
        await fs.cp(path.join(template, entry), path.join(target, entry), { recursive: true, force: false, errorOnExist: true });
    }
    const manifest = path.join(target, 'mod.toml');
    let text = await fs.readFile(manifest, 'utf8');
    text = text.replace('id = "tutorial.blade"', `id = ${JSON.stringify(id)}`)
        .replace('name = "Training Blade"', `name = ${JSON.stringify(name)}`)
        .replace('authors = ["Your Name"]', `authors = [${JSON.stringify(author)}]`);
    await fs.writeFile(manifest, text);
    return target;
}
module.exports = { createMod };

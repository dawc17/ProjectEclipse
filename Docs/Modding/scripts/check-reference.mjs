import { readFileSync, readdirSync, writeFileSync } from 'node:fs';
import { fileURLToPath } from 'node:url';
import path from 'node:path';
import GithubSlugger from 'github-slugger';
import { unified } from 'unified';
import remarkParse from 'remark-parse';
import { toString } from 'mdast-util-to-string';
import { pageUrl } from '../site.config.mjs';

const root = fileURLToPath(new URL('../../../', import.meta.url));
const docs = fileURLToPath(new URL('../src/content/docs/', import.meta.url));
const bindings = path.join(root, 'Assets/Scripts/Eclipse/Modding');
const files = readdirSync(bindings).filter((name) => /^MoonSharpScriptRuntime.*\.cs$/.test(name));
const sources = files.map((name) => readFileSync(path.join(bindings, name), 'utf8'));
const tableNames = new Map();
for (const source of sources) {
  for (const [, namespace, variable] of source.matchAll(/root\.Set\("(\w+)"\s*,\s*DynValue\.NewTable\((\w+)\)/g)) {
    tableNames.set(variable, [`sf2.${namespace}`]);
  }
}
// The three mode modules share a registration closure in a foreach loop.
for (const source of sources) {
  for (const [, list] of source.matchAll(/foreach\s*\(string name in new\[\]\s*\{([^}]+)\}/g)) {
    tableNames.set('module', [...list.matchAll(/"(\w+)"/g)].map((match) => `sf2.${match[1]}`));
  }
}
tableNames.set('fighterTable', ['fighter']);
tableNames.set('targetTable', ['fighter.opponent']);
const exported = new Set(['require']);
const separator = (namespace) => namespace.startsWith('fighter') ? ':' : '.';
for (const source of sources) {
  for (const [, variable, name] of source.matchAll(/(\w+)\.Set\("(\w+)"\s*,\s*DynValue\.NewCallback\(/g)) {
    if (variable === 'Globals' && name === 'require') continue;
    if (variable === 'Table') continue; // root.Get(...).Table.Set is handled below.
    const namespaces = tableNames.get(variable);
    if (!namespaces) throw new Error(`Unmapped Lua callback table: ${variable}.${name}. Update the reference audit.`);
    for (const namespace of namespaces) exported.add(`${namespace}${separator(namespace)}${name}`);
  }
  for (const [, namespace, name] of source.matchAll(/root\.Get\("(\w+)"\)\.Table\.Set\("(\w+)"\s*,\s*DynValue\.NewCallback\(/g)) {
    exported.add(`sf2.${namespace}.${name}`);
  }
  for (const [, variable, name] of source.matchAll(/(\w+)\.Set\("(\w+)"\s*,\s*\w+\.Get\("\w+"\)\)/g)) {
    for (const namespace of tableNames.get(variable) ?? []) exported.add(`${namespace}.${name}`);
  }
  for (const [, name] of source.matchAll(/\{\s*"(on_\w+)"\s*,\s*ModEffectEvent\./g)) exported.add(name);
}

if (sources.some(source => /table\.Get\("on_result"\)/.test(source))) exported.add('on_result');
for (const name of ['on_click', 'on_close'])
  if (sources.some(source => source.includes(`table.Get("${name}")`))) exported.add(name);

const parser = unified().use(remarkParse);
const documented = new Map();
const failures = [];
const symbol = /^(?:sf2\.[\w.]+|fighter(?:\.opponent)?:\w+|require|on_\w+)$/;
for (const filename of readdirSync(docs, { recursive: true })) {
  if (!/\.(md|mdx)$/.test(filename) || filename === 'reference.md') continue;
  const text = readFileSync(path.join(docs, filename), 'utf8').replace(/^---\r?\n[\s\S]*?\r?\n---\r?\n/, '');
  const ast = parser.parse(text);
  const slugger = new GithubSlugger();
  // Check reader-visible Markdown, including code. File paths in link targets
  // may retain repository identities, but labels and prose must use feature names.
  function visit(node) {
    if (['text', 'inlineCode', 'code'].includes(node.type) && /\bphase\s*[0-9]|\bP[0-3](?:\.[0-9]|[A-D])?\b/i.test(node.value)) {
      failures.push(`${filename}: internal milestone label in reader-facing content`);
    }
    if (node.type === 'code' && node.lang === 'lua') {
      for (const [, name] of node.value.matchAll(/\b(sf2\.[\w.]+|fighter(?:\.opponent)?:\w+)\s*[({]/g)) {
        if (!exported.has(name)) failures.push(`${filename}: example calls an unknown public function: ${name}`);
      }
    }
    for (const child of node.children ?? []) visit(child);
  }
  visit(ast);
  for (let index = 0; index < ast.children.length; index++) {
    const node = ast.children[index];
    if (node.type !== 'heading') continue;
    const name = toString(node);
    const anchor = slugger.slug(name);
    if (node.depth !== 2 || !symbol.test(name)) continue;
    if (documented.has(name)) failures.push(`Duplicate function section: ${name}`);
    const next = ast.children.slice(index + 1).find((entry) => entry.type === 'heading' && entry.depth <= 2);
    const body = text.slice(node.position.end.offset, next?.position.start.offset ?? text.length);
    for (const label of ['Signature', 'Returns', 'When', 'Requires']) {
      if (!body.includes(`**${label}:**`)) failures.push(`${filename}: ${name} needs ${label}`);
    }
    if (!/```lua\s/.test(body)) failures.push(`${filename}: ${name} needs a Lua example`);
    documented.set(name, `${pageUrl(filename.replaceAll('\\', '/').replace(/\.(md|mdx)$/, ''))}#${anchor}`);
  }
}
for (const name of exported) if (!documented.has(name)) failures.push(`Missing public function/callback section: ${name}`);
for (const name of documented.keys()) if (!exported.has(name)) failures.push(`Documented symbol is not in the Lua bindings: ${name}`);
if (failures.length) throw new Error(`Reference audit failed:\n${[...new Set(failures)].join('\n')}`);

const entries = [...documented].sort(([a], [b]) => a.localeCompare(b));
const content = `---\ntitle: Function index\ndescription: Every public Eclipse Lua function and combat callback, with links to its full reference.\neditUrl: false\n---\n\nUse this index when you know a function's name. Each link opens its dedicated section with arguments, defaults, return value, requirements, and an example.\n\nFunctions beginning with \`sf2.\` come from \`local sf2 = require("sf2")\`. Methods beginning with \`fighter:\` are available inside combat callbacks. Names beginning with \`on_\` are callbacks you supply when registering a behavior.\n\n${entries.map(([name, url]) => `- [\`${name}\`](${url})`).join('\n')}\n`;
writeFileSync(path.join(docs, 'reference.md'), content);
console.log(`Verified dedicated documentation for ${exported.size} public functions, aliases, and callbacks. No internal milestone labels found.`);

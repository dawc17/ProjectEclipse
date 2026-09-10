import { existsSync, readFileSync, readdirSync, statSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { parse } from 'parse5';
import { base, site } from '../site.config.mjs';

const directory = fileURLToPath(new URL('../dist/', import.meta.url));
const pages = new Map();
const errors = [];
let checked = 0;

function walk(node, visit) {
  visit(node);
  for (const child of node.childNodes ?? []) walk(child, visit);
}

for (const filename of readdirSync(directory, { recursive: true })) {
  if (!filename.endsWith('.html')) continue;
  const html = parse(readFileSync(path.join(directory, filename), 'utf8'));
  const ids = new Set();
  walk(html, (node) => {
    const id = node.attrs?.find((attribute) => attribute.name === 'id')?.value;
    if (id) ids.add(id);
  });
  pages.set(path.resolve(directory, filename), { html, ids, filename });
}

for (const page of pages.values()) {
  const relative = page.filename.replaceAll('\\', '/').replace(/index\.html$/, '');
  const current = new URL(`${base}/${relative}`, site);
  walk(page.html, (node) => {
    for (const attribute of node.attrs ?? []) {
      if (!['href', 'src'].includes(attribute.name) || !attribute.value) continue;
      const url = new URL(attribute.value, current);
      if (url.origin !== current.origin) continue;
      checked++;
      const prefix = `${base}/`;
      if (!url.pathname.startsWith(prefix)) {
        errors.push(`${page.filename}: URL escapes Pages base: ${attribute.value}`);
        continue;
      }
      let target = path.resolve(directory, decodeURIComponent(url.pathname.slice(prefix.length)));
      if (existsSync(target) && statSync(target).isDirectory()) target = path.join(target, 'index.html');
      if (!existsSync(target)) {
        errors.push(`${page.filename}: missing target ${attribute.value}`);
        continue;
      }
      const destination = pages.get(target);
      if (destination && url.hash && !destination.ids.has(decodeURIComponent(url.hash.slice(1)))) {
        errors.push(`${page.filename}: missing anchor ${attribute.value}`);
      }
    }
  });
}

if (errors.length) throw new Error(`Broken wiki links:\n${[...new Set(errors)].join('\n')}`);
if (!existsSync(path.join(directory, 'pagefind', 'pagefind.js'))) throw new Error('Search index was not generated.');
console.log(`Checked ${checked} local links/assets across ${pages.size} HTML pages, including fragments and the Pages base. Search index exists.`);

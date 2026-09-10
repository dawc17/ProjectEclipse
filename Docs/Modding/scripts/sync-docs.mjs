import { existsSync, mkdirSync, readFileSync, readdirSync, unlinkSync, writeFileSync } from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import GithubSlugger from 'github-slugger';
import { toString } from 'mdast-util-to-string';
import remarkParse from 'remark-parse';
import { unified } from 'unified';
import { pageUrl, repository, branch } from '../site.config.mjs';

const root = fileURLToPath(new URL('../../../', import.meta.url));
const output = fileURLToPath(new URL('../src/content/docs/', import.meta.url));
const parser = unified().use(remarkParse);

// Exact heading matches make newly added/renamed sections fail visibly until
// they receive a deliberate route. Markdown remains authoritative in Mods/.
const sources = [
  { file: 'Mods/README.md', intro: ['overview', 'API overview'], sections: {
    'Enabling and disabling mods': ['installing-mods', 'Installing and enabling mods'],
    'API design: definitions and behavior': ['definitions-and-behavior', 'Definitions and Lua behavior'],
    'Sprites and textures': ['sprites-and-textures', 'Sprites and textures'],
    'Equipment, shop, and logging API': ['equipment-shop-logging', 'Equipment, shops, and logging'],
    'Targeted content patches (0.4)': ['localization-patches', 'Localization patches'],
    'Mod-owned state and migrations (0.5)': ['mod-state', 'Mod-owned state and migrations'],
    'Phase 1 content graph API (0.5)': ['content-graph', 'Battles, quests, and content definitions'],
    'Perks, enchantments, and reusable behaviors (0.3)': ['perks-and-enchantments', 'Perks, enchantments, and reusable behaviors'],
    'Compatibility': ['legacy-compatibility', 'Legacy API compatibility'],
    'Core ownership and storage': ['core-assets', 'Core assets and ownership'],
    'Save compatibility': ['save-compatibility', 'Save compatibility'],
    'Built-in core equipment registry': ['core-equipment', 'Core equipment registry'],
  } },
  { file: 'Mods/P1C_API.md', intro: ['items-progression-forge', 'Items, sets, progression, and forge'], whole: true },
  { file: 'Mods/P2_API.md', intro: ['combat-overview', 'Combat and mode API status'], sections: {
    'Behavior instances': ['behavior-instances', 'Behavior instances'],
    'Verified combat callbacks and capabilities': ['combat-callbacks', 'Combat callbacks and capabilities'],
    'Timer and service policy': ['timers-and-services', 'Timers and services'],
    'Events, mode sequences and Ascension': ['events-and-modes', 'Events, modes, and Ascension'],
    'Offline raids and quest integration': ['offline-raids', 'Offline raids and quest integration'],
    'Verification': ['combat-verification', 'Combat and mode verification'],
  } },
  { file: 'Mods/P3_API.md', intro: ['progression-overview', 'Achievement and replacement API status'], sections: {
    'Counters and achievements': ['achievements', 'Counters and achievements'],
    'Asset replacement': ['asset-replacement', 'Asset replacement'],
    'Remaining configuration classification': ['configuration-scope', 'Configuration scope and limits'],
    'Verification and playtest': ['progression-verification', 'Achievement and asset verification'],
  } },
  { file: 'Mods/example.phase1/README.md', intro: ['content', 'Content showcase'], whole: true, group: 'showcases' },
  { file: 'Mods/example.phase2/README.md', intro: ['combat-and-modes', 'Combat and modes showcase'], whole: true, group: 'showcases' },
  { file: 'Mods/example.phase3/README.md', intro: ['achievements-and-assets', 'Achievements and assets showcase'], whole: true, group: 'showcases' },
];

function walk(node, visit) {
  visit(node);
  for (const child of node.children ?? []) walk(child, visit);
}

const documents = new Map();
const pages = [];
for (const source of sources) {
  const text = readFileSync(path.join(root, source.file), 'utf8').replace(/\r\n/g, '\n');
  const ast = parser.parse(text);
  const title = ast.children.find((node) => node.type === 'heading' && node.depth === 1);
  if (!title || ast.children[0] !== title) throw new Error(`${source.file}: expected an opening H1`);
  const slugger = new GithubSlugger();
  const headings = ast.children.filter((node) => node.type === 'heading');
  for (const heading of headings) heading.anchor = slugger.slug(toString(heading));
  const boundaries = source.whole ? [] : headings.filter((heading) => heading.depth === 2);
  const found = new Set(boundaries.map(toString));
  for (const heading of found) {
    if (!source.sections[heading]) throw new Error(`${source.file}: add a wiki route for "${heading}"`);
  }
  for (const heading of Object.keys(source.sections ?? {})) {
    if (!found.has(heading)) throw new Error(`${source.file}: wiki section disappeared: "${heading}"`);
  }
  const chunks = [title, ...boundaries].map((heading, index, all) => {
    const [slug, pageTitle] = index === 0 ? source.intro : source.sections[toString(heading)];
    return {
      source, text, ast, title: pageTitle,
      route: `${source.group ?? 'api'}/${slug}`,
      heading, start: heading.position.end.offset,
      end: all[index + 1]?.position.start.offset ?? text.length,
    };
  });
  const anchors = new Map();
  for (const heading of headings) {
    const chunk = chunks.findLast((candidate) => candidate.heading.position.start.offset <= heading.position.start.offset);
    anchors.set(heading.anchor, { route: chunk.route, hash: chunk.heading === heading ? '' : `#${heading.anchor}` });
  }
  documents.set(source.file, { route: chunks[0].route, anchors });
  pages.push(...chunks);
}

function rewriteUrl(url, sourceFile, image = false) {
  if (/^(?:[a-z][a-z\d+.-]*:|\/\/)/i.test(url)) return url;
  const hashIndex = url.indexOf('#');
  const file = hashIndex < 0 ? url : url.slice(0, hashIndex);
  const hash = hashIndex < 0 ? '' : url.slice(hashIndex + 1);
  const target = file ? path.posix.normalize(path.posix.join(path.posix.dirname(sourceFile), decodeURI(file))) : sourceFile;
  if (target.startsWith('../') || !existsSync(path.join(root, target))) {
    throw new Error(`${sourceFile}: missing repository link ${url}`);
  }
  const doc = documents.get(target);
  if (doc) {
    const anchor = hash ? doc.anchors.get(decodeURIComponent(hash)) : undefined;
    if (hash && !anchor) throw new Error(`${sourceFile}: missing heading ${url}`);
    return `${pageUrl(anchor?.route ?? doc.route)}${anchor?.hash ?? ''}`;
  }
  if (image) return `https://raw.githubusercontent.com/dawc17/ProjectEclipse/${branch}/${target}`;
  return `${repository}/blob/${branch}/${target}${hash ? `#${hash}` : ''}`;
}

// Only remove stale Markdown files inside the two explicitly generated folders.
const wanted = new Set(pages.map((page) => `${page.route}.md`));
for (const group of ['api', 'showcases']) {
  const directory = path.join(output, group);
  mkdirSync(directory, { recursive: true });
  for (const entry of readdirSync(directory, { withFileTypes: true })) {
    if (entry.isFile() && entry.name.endsWith('.md') && !wanted.has(`${group}/${entry.name}`)) {
      unlinkSync(path.join(directory, entry.name));
    }
  }
}

for (const page of pages) {
  const replacements = [];
  walk(page.ast, (node) => {
    if (!['link', 'image', 'definition'].includes(node.type)) return;
    const { start, end } = node.position;
    if (start.offset < page.start || end.offset > page.end) return;
    const newUrl = rewriteUrl(node.url, page.source.file, node.type === 'image');
    if (newUrl === node.url) return;
    // Replace only the destination, preserving the author's Markdown formatting.
    const original = page.text.slice(start.offset, end.offset);
    const position = original.lastIndexOf(node.url);
    if (position < 0) throw new Error(`Cannot rewrite ${node.url}`);
    replacements.push({ start: start.offset + position, end: start.offset + position + node.url.length, value: newUrl });
  });
  let body = page.text.slice(page.start, page.end);
  for (const replacement of replacements.sort((a, b) => b.start - a.start)) {
    body = body.slice(0, replacement.start - page.start) + replacement.value + body.slice(replacement.end - page.start);
  }
  const frontmatter = `---\ntitle: ${JSON.stringify(page.title)}\ndescription: ${JSON.stringify(`${page.title} in the Eclipse modding API.`)}\neditUrl: ${repository}/edit/${branch}/${page.source.file}\n---\n`;
  const content = `${frontmatter}\n${body.trim()}\n`;
  const filename = path.join(output, `${page.route}.md`);
  if (!existsSync(filename) || readFileSync(filename, 'utf8') !== content) writeFileSync(filename, content);
}
console.log(`Synchronized ${pages.length} wiki pages from ${sources.length} tracked documents.`);

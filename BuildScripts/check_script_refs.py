import os, re, collections

ROOT = 'Assets'
guid_re = re.compile(r'guid: ([0-9a-f]{32})')
script_guids = {}          # guid -> script path
for dp, dns, fns in os.walk(ROOT):
    for fn in fns:
        if fn.endswith('.cs.meta'):
            p = os.path.join(dp, fn)
            m = re.search(r'guid: ([0-9a-f]{32})', open(p, encoding='utf-8', errors='ignore').read())
            if m:
                script_guids[m.group(1)] = p[:-5]

# collect m_Script references from scenes & prefabs & assets
refs = collections.Counter()          # guid -> count
ref_locations = collections.defaultdict(set)
for dp, dns, fns in os.walk(ROOT):
    for fn in fns:
        if fn.endswith(('.unity', '.prefab', '.asset')):
            p = os.path.join(dp, fn)
            txt = open(p, encoding='utf-8', errors='ignore').read()
            for mm in re.finditer(r'm_Script: \{fileID: \d+, guid: ([0-9a-f]{32})', txt):
                g = mm.group(1)
                refs[g] += 1
                ref_locations[g].add(fn)

orphans = {g: c for g, c in refs.items() if g not in script_guids}
print('distinct m_Script guids referenced:', len(refs))
print('resolved by an existing .cs.meta :', len(refs) - len(orphans))
print('ORPHAN guids (no matching script):', len(orphans))
print('total broken component instances :', sum(orphans.values()))
print()
print('top orphans:')
for g, c in sorted(orphans.items(), key=lambda kv: -kv[1])[:15]:
    loc = next(iter(ref_locations[g])) if ref_locations[g] else '?'
    print(' ', g, 'x', c, '| e.g.', loc)

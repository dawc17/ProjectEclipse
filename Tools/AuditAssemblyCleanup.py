"""Verify the reviewed Assembly-CSharp archive; does not infer dead Unity code."""
from pathlib import Path
import hashlib
import json
import re
import sys

ROOT = Path(__file__).resolve().parents[1]
ARCHIVE = ROOT / 'Tools/LegacyServices/AssemblyCleanup'
TEXT_ASSETS = {'.cs', '.prefab', '.unity', '.asset', '.controller', '.anim',
               '.overridecontroller', '.playable', '.xml', '.json', '.txt', '.uxml'}


def main():
    entries = json.loads((ARCHIVE / 'assembly-cleanup.json').read_text())
    errors = []
    guids = {entry['guid']: entry['source'] for entry in entries}
    # Generic CUDLR names (Console, Server) also exist in unrelated namespaces.
    # Compilation checks C# binding; here we check unique names, reflected strings,
    # fully qualified CUDLR names, and serialized script GUIDs independently.
    names = {name for entry in entries if '/CUDLR/' not in entry['source']
             for name in entry['types']}
    names.update('CUDLR.' + name for entry in entries if '/CUDLR/' in entry['source']
                 for name in entry['types'])
    pattern = re.compile(r'\b(?:' + '|'.join(re.escape(n) for n in sorted(names)) + r')\b')
    for entry in entries:
        for suffix, hash_key in [('', 'sha256'), ('.meta', 'meta_sha256')]:
            relative = entry['source'] + suffix
            if (ROOT / relative).exists():
                errors.append('Archived source still active: ' + relative)
            path = ARCHIVE / relative
            if not path.is_file() or hashlib.sha256(path.read_bytes()).hexdigest() != entry[hash_key]:
                errors.append('Archive bytes missing or changed: ' + relative)
    scanned = 0
    for path in (ROOT / 'Assets').rglob('*'):
        if not path.is_file() or path.suffix.lower() not in TEXT_ASSETS:
            continue
        scanned += 1
        text = path.read_text(encoding='utf-8-sig', errors='replace')
        for guid in set(re.findall(r'\b[0-9a-f]{32}\b', text)) & guids.keys():
            errors.append(f'{path.relative_to(ROOT)} references archived GUID {guid} ({guids[guid]})')
        for match in pattern.finditer(text):
            errors.append(f'{path.relative_to(ROOT)} references archived name {match.group()}')
    for project in ROOT.glob('*.csproj'):
        text = project.read_text(encoding='utf-8-sig').replace('\\', '/')
        for entry in entries:
            if f'Include="{entry["source"]}"' in text:
                errors.append(f'{project.name} still compiles {entry["source"]}')
    if errors:
        print('\n'.join(errors), file=sys.stderr)
        return 1
    print(f'PASS: {len(entries)} archived sources and original metas intact; '
          f'{scanned} text assets scanned, no removed names/GUIDs or project entries remain.')
    return 0


if __name__ == '__main__':
    sys.exit(main())

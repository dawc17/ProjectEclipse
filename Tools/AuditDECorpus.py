"""Read-only inventory and reconciliation evidence for the owner's extracted DE corpus.

Never imports assets, modifies Lua, or interprets archive files as instructions.
Missing/ambiguous/changed content requires review; an inventory is not game parity.
"""
import argparse
from collections import Counter
import hashlib
import json
import os
from pathlib import Path
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[1]


def digest(path):
    value = hashlib.sha256()
    with path.open('rb') as stream:
        for block in iter(lambda: stream.read(1024 * 1024), b''):
            value.update(block)
    return value.hexdigest()


def inventory(root):
    root = root.resolve(strict=True)
    if not root.is_dir():
        raise ValueError('Corpus must be an extracted directory, not an archive or HTML response.')
    rows = []
    for directory, folders, files in os.walk(root, followlinks=False):
        for name in folders + files:
            path = Path(directory) / name
            if path.is_symlink() or getattr(path, 'is_junction', lambda: False)():
                raise ValueError(f'Corpus contains a link/junction: {path}')
            if not path.resolve().is_relative_to(root):
                raise ValueError(f'Corpus entry escapes the source root: {path}')
        for name in files:
            path = Path(directory) / name
            before = path.stat()
            checksum = digest(path)
            after = path.stat()
            if (before.st_size, before.st_mtime_ns) != (after.st_size, after.st_mtime_ns):
                raise ValueError(f'Corpus changed while hashing: {path}')
            rows.append({'path': path.relative_to(root).as_posix(),
                         'bytes': after.st_size, 'sha256': checksum})
    return sorted(rows, key=lambda row: row['path'])


def meaningful(text):
    # Ignore indentation only. Preserve meaningful leading/trailing spaces in text.
    return '' if text is None or not text.strip() else text


def xml_tree(path):
    data = path.read_bytes()
    # No external references or entity-expansion workloads are needed for this audit.
    # Strip NULs for UTF-16/32 declaration detection before ElementTree decodes it.
    declaration = data.replace(b'\x00', b'').upper()
    if b'<!DOCTYPE' in declaration or b'<!ENTITY' in declaration:
        raise ValueError('DTD/entity declarations require manual review.')
    return ET.fromstring(data)


def xml_differences(old, new, path=''):
    """Full positional diff; child ordering and meaningful text remain significant."""
    path = path or '/' + old.tag
    changes = []
    if old.tag != new.tag:
        changes.append({'path': path, 'field': 'tag', 'old': old.tag, 'new': new.tag})
    for key in sorted(set(old.attrib) | set(new.attrib)):
        if old.get(key) != new.get(key):
            changes.append({'path': path, 'field': '@' + key, 'old': old.get(key), 'new': new.get(key)})
    for field in ('text', 'tail'):
        left, right = meaningful(getattr(old, field)), meaningful(getattr(new, field))
        if left != right:
            changes.append({'path': path, 'field': field, 'old': left, 'new': right})
    for index in range(max(len(old), len(new))):
        child_path = path + f'/child[{index + 1}]'
        if index >= len(old):
            changes.append({'path': child_path, 'field': 'added', 'new': ET.tostring(new[index], encoding='unicode')})
        elif index >= len(new):
            changes.append({'path': child_path, 'field': 'removed', 'old': ET.tostring(old[index], encoding='unicode')})
        else:
            changes.extend(xml_differences(old[index], new[index], child_path))
    return changes


def candidates(rows, relative, basename_only=False):
    key = relative.casefold()
    if basename_only:
        key = Path(relative).name.casefold()
        return [row for row in rows if Path(row['path']).name.casefold() == key]
    return [row for row in rows if row['path'].casefold() == key or row['path'].casefold().endswith('/' + key)]


def compare_file(source_root, row, original, xml=False):
    entry = {'reference_sha256': digest(original), 'source': row['path'],
             'source_sha256': row['sha256']}
    if entry['reference_sha256'] == row['sha256']:
        entry['status'] = 'identical'
    elif not xml:
        entry['status'] = 'changed'
    else:
        try:
            changes = xml_differences(xml_tree(original), xml_tree(source_root / row['path']))
            entry.update(status='changed' if changes else 'formatting_only', changes=changes)
        except (ET.ParseError, ValueError, RecursionError) as error:
            entry.update(status='unreadable_xml', error=str(error))
    return entry


def audit(corpus, reference, bundled, xml_root=None):
    corpus = corpus.resolve(strict=True)
    reference = reference.resolve(strict=True)
    bundled = bundled.resolve(strict=True)
    rows = inventory(corpus)
    if not rows:
        raise ValueError('Empty corpus; refusing to report a successful inventory.')
    if xml_root is not None:
        xml_root = xml_root.resolve(strict=True)
        if not xml_root.is_relative_to(corpus) or not xml_root.is_dir():
            raise ValueError('--xml-root must be a directory within the extracted corpus.')
    used = set()
    comparisons = []
    originals = sorted(reference.rglob('*.xml'))
    if not originals:
        raise ValueError('Historical reference contains no XML files.')
    for original in originals:
        relative = original.relative_to(reference).as_posix()
        if xml_root is None:
            found = candidates(rows, relative)
        else:
            exact = (xml_root / relative).relative_to(corpus).as_posix().casefold()
            found = [row for row in rows if row['path'].casefold() == exact]
        record = {'reference': relative, 'candidates': [row['path'] for row in found]}
        used.update(record['candidates'])
        if len(found) != 1:
            record['status'] = 'missing' if not found else 'ambiguous'
        else:
            record.update(compare_file(corpus, found[0], original, xml=True))
        comparisons.append(record)
    binary_comparisons = []
    for original in sorted(bundled.rglob('*')):
        if not original.is_file() or original.suffix.lower() == '.meta':
            continue
        found = candidates(rows, original.name, basename_only=True)
        record = {'bundled': original.relative_to(bundled).as_posix(),
                  'candidates': [row['path'] for row in found]}
        if len(found) != 1:
            record['status'] = 'missing' if not found else 'ambiguous'
        else:
            record.update(compare_file(corpus, found[0], original))
        binary_comparisons.append(record)
    return {
        'schema': 1, 'corpus': str(corpus), 'historical_xml': str(reference), 'bundled_assets': str(bundled),
        'scope': 'Read-only inventory and comparisons; does not prove Lua parity or runtime behavior.',
        'inventory': rows,
        'summary': {'files': len(rows), 'bytes': sum(row['bytes'] for row in rows),
                    'extensions': dict(sorted(Counter(Path(row['path']).suffix.lower() for row in rows).items())),
                    'xml': dict(sorted(Counter(row['status'] for row in comparisons).items())),
                    'bundled': dict(sorted(Counter(row['status'] for row in binary_comparisons).items()))},
        'xml_comparisons': comparisons, 'bundled_comparisons': binary_comparisons,
        'additional_xml': [row['path'] for row in rows if row['path'].lower().endswith('.xml') and row['path'] not in used],
    }


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--corpus', type=Path, required=True)
    parser.add_argument('--xml-root', type=Path, help='Explicit XML subtree when suffix matching is ambiguous.')
    parser.add_argument('--reference', type=Path, default=ROOT / 'Assets/DExml')
    parser.add_argument('--bundled', type=Path, default=ROOT / 'Mods/de128/assets')
    parser.add_argument('--output', type=Path, required=True, help='New JSON report outside the input trees; never overwritten.')
    args = parser.parse_args()
    output = args.output.resolve()
    for source in (args.corpus, args.reference, args.bundled):
        if output.is_relative_to(source.resolve()):
            parser.error('--output must be outside every input tree.')
    if output.exists():
        parser.error('Report already exists; choose a new output path to preserve evidence.')
    result = audit(args.corpus, args.reference, args.bundled, args.xml_root)
    output.parent.mkdir(parents=True, exist_ok=True)
    with output.open('x', encoding='utf-8') as stream:
        json.dump(result, stream, indent=2, ensure_ascii=False)
        stream.write('\n')
    print(json.dumps(result['summary'], indent=2))
    print(f'Report: {output}')


if __name__ == '__main__':
    main()

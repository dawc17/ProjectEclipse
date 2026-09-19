"""Controlled fixtures for corpus reconciliation; never substitutes for the real archive."""
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest

from AuditDECorpus import audit, digest, inventory


class CorpusAuditTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory(prefix='DECorpusAudit-')
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.source = self.root / 'corpus'
        self.old = self.root / 'old'
        self.bundled = self.root / 'bundled'
        for path in (self.source, self.old, self.bundled):
            path.mkdir()
        self.put(self.old, 'animations/moves.xml', '<Moves><Move Name="A" Priority="1"/></Moves>')

    @staticmethod
    def put(root, relative, value):
        path = root / relative
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_bytes(value if isinstance(value, bytes) else value.encode('utf-8'))
        return path

    def run_audit(self, **kwargs):
        return audit(self.source, self.old, self.bundled, **kwargs)

    def test_formatting_attributes_and_file_hashes(self):
        original = self.old / 'animations/moves.xml'
        copied = self.put(self.source, 'Assets/data/animations/moves.xml', original.read_bytes())
        result = self.run_audit()
        self.assertEqual(result['xml_comparisons'][0]['status'], 'identical')
        self.assertEqual(result['inventory'][0]['sha256'], digest(copied))
        copied.write_text('<Moves>\n <Move Priority="1" Name="A"></Move>\n</Moves>', encoding='utf-8')
        result = self.run_audit()
        self.assertEqual(result['xml_comparisons'][0]['status'], 'formatting_only')
        self.assertEqual(result['xml_comparisons'][0]['changes'], [])

    def test_gameplay_changes_order_repetition_and_text_are_retained(self):
        self.put(self.old, 'animations/moves.xml', '<Moves><Move Name="A"><Edge Name="X"/><Edge Name="X"/><Edge Name="Y"/></Move><Text> word </Text></Moves>')
        self.put(self.source, 'animations/moves.xml', '<Moves><Move Name="B"><Edge Name="X"/><Edge Name="Y"/></Move><Text>word</Text></Moves>')
        entry = self.run_audit()['xml_comparisons'][0]
        self.assertEqual(entry['status'], 'changed')
        fields = [change['field'] for change in entry['changes']]
        self.assertIn('@Name', fields)
        self.assertIn('removed', fields)
        self.assertIn('text', fields)
        self.assertEqual(entry['changes'][-1]['old'], ' word ')

    def test_ambiguous_suffix_is_not_silently_resolved_by_hash(self):
        data = (self.old / 'animations/moves.xml').read_bytes()
        self.put(self.source, 'one/animations/moves.xml', data)
        self.put(self.source, 'two/animations/moves.xml', data)
        self.assertEqual(self.run_audit()['xml_comparisons'][0]['status'], 'ambiguous')
        chosen = self.run_audit(xml_root=self.source / 'two')
        self.assertEqual(chosen['xml_comparisons'][0]['status'], 'identical')
        self.assertEqual(chosen['additional_xml'], ['one/animations/moves.xml'])
        with self.assertRaises(ValueError):
            self.run_audit(xml_root=self.old)

    def test_missing_added_and_bad_xml_are_reported(self):
        self.put(self.source, 'new.xml', '<New/>')
        result = self.run_audit()
        self.assertEqual(result['xml_comparisons'][0]['status'], 'missing')
        self.assertEqual(result['additional_xml'], ['new.xml'])
        self.put(self.source, 'animations/moves.xml', '<broken')
        self.assertEqual(self.run_audit()['xml_comparisons'][0]['status'], 'unreadable_xml')
        entity = '<!DOCTYPE x [<!ENTITY x "expanded">]><Moves>&x;</Moves>'
        self.put(self.source, 'animations/moves.xml', entity.encode('utf-16'))
        self.assertIn('DTD/entity', self.run_audit()['xml_comparisons'][0]['error'])

    def test_binary_matches_changes_missing_and_duplicates(self):
        self.put(self.bundled, 'animations/attack.bytes', b'original')
        self.put(self.source, 'Assets/data/attack.bytes', b'original')
        self.assertEqual(self.run_audit()['bundled_comparisons'][0]['status'], 'identical')
        self.put(self.source, 'Assets/data/attack.bytes', b'changed')
        self.assertEqual(self.run_audit()['bundled_comparisons'][0]['status'], 'changed')
        self.put(self.source, 'other/attack.bytes', b'original')
        self.assertEqual(self.run_audit()['bundled_comparisons'][0]['status'], 'ambiguous')
        self.put(self.bundled, 'audio/missing.ogg', b'missing')
        self.assertEqual(self.run_audit()['summary']['bundled']['missing'], 1)

    def test_inventory_is_deterministic_and_preserves_sources(self):
        self.put(self.source, 'b.bin', b'2')
        self.put(self.source, 'a.bin', b'1')
        first = inventory(self.source)
        self.assertEqual([row['path'] for row in first], ['a.bin', 'b.bin'])
        self.run_audit()
        self.assertEqual(first, inventory(self.source))
        self.assertEqual(self.run_audit(), self.run_audit())

    def test_empty_corpus_and_file_inputs_fail(self):
        with self.assertRaises(ValueError):
            self.run_audit()
        html = self.put(self.root, 'Assets.7z', '<html>Quota exceeded</html>')
        with self.assertRaises(ValueError):
            inventory(html)

    def test_cli_preserves_reports_and_refuses_output_inside_inputs(self):
        self.put(self.source, 'animations/moves.xml', '<Moves/>')
        report = self.root / 'report.json'
        command = [sys.executable, str(Path(__file__).with_name('AuditDECorpus.py')),
                   '--corpus', str(self.source), '--reference', str(self.old),
                   '--bundled', str(self.bundled), '--output', str(report)]
        first = subprocess.run(command, capture_output=True, text=True)
        self.assertEqual(first.returncode, 0, first.stderr)
        self.assertEqual(json.loads(report.read_text(encoding='utf-8'))['summary']['files'], 1)
        before = report.read_bytes()
        self.assertNotEqual(subprocess.run(command, capture_output=True).returncode, 0)
        self.assertEqual(report.read_bytes(), before)
        command[-1] = str(self.source / 'report.json')
        self.assertNotEqual(subprocess.run(command, capture_output=True).returncode, 0)
        self.assertFalse((self.source / 'report.json').exists())


if __name__ == '__main__':
    unittest.main(verbosity=2)

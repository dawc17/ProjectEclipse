import json
from pathlib import Path
import tempfile
import unittest
import subprocess
import sys
from unittest.mock import patch

import CharacterPipeline as pipeline
import PackageCharacter as packager


class PackageTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.rig = self.root / 'body.xml'
        self.rig.write_text('<Scene><Nodes><NHeel_1 Type="Node" X="1" Mass="1"/><NTop Type="Node" Y="2" Mass="1"/></Nodes><Edges><Body Type="Edge" End1="NHeel_1" End2="NTop"/></Edges><Figures/></Scene>')
        self.clip = self.root / 'move.bin'
        pipeline.write_animation(self.clip, {'frames': [[[1, 0, 0], [0, 2, 0]], [[1, 0, 0], [0, 3, 0]]]})

    def tearDown(self):
        self.temp.cleanup()

    def test_package_preserves_native_payload_and_declares_timing(self):
        target = self.root / 'test.character'
        packager.package(self.rig, self.clip, [], target, 'test.character', mid_frames=2)
        self.assertEqual((target / 'assets/animations/authored.bytes').read_bytes(), self.clip.read_bytes())
        self.assertEqual((target / 'assets/models/body.xml').read_bytes(), self.rig.read_bytes())
        module = (target / 'scripts/character.lua').read_text()
        self.assertIn('mid_frames = 2', module)
        self.assertIn('end_frame = 1', module)
        self.assertIn('on_decide', module)
        self.assertIn('type = "character"', module)
        self.assertIn('sf2.quests.register', (target / 'scripts/main.lua').read_text())
        report = json.loads((target / 'assets/animations/authored.rig.json').read_text())
        self.assertEqual(report['nodes'], ['NHeel_1', 'NTop'])
        self.assertIn('"fps":20.0', (target / 'preview.html').read_text())
        with self.assertRaisesRegex(ValueError, 'already exists'):
            packager.package(self.rig, self.clip, [], target, 'test.character')
        self.assertEqual((target / 'assets/animations/authored.bytes').read_bytes(), self.clip.read_bytes())

    def test_missing_or_collapsed_bindings_reject_before_creating_mod(self):
        pipeline.write_animation(self.clip, {'frames': [[[1, 0, 0], [0, 0, 0]], [[1, 0, 0], [0, 0, 0]]]})
        target = self.root / 'test.character'
        with self.assertRaisesRegex(ValueError, 'NTop'):
            packager.package(self.rig, self.clip, [], target, 'test.character')
        self.assertFalse(target.exists())

    def test_invalid_identity_and_incompatible_skin(self):
        for name in ['core', '../bad', 'Uppercase']:
            with self.assertRaises(ValueError):
                packager.package(self.rig, self.clip, [], self.root / name, name)
        skin = self.root / 'skin.xml'
        skin.write_text('<Scene><Nodes/><Edges/><Figures><T Type="Triangle" Node1="missing" Node2="NHeel_1" Node3="NTop"/></Figures></Scene>')
        with self.assertRaisesRegex(ValueError, 'unresolved'):
            packager.package(self.rig, self.clip, [skin], self.root / 'test.character', 'test.character')
        self.assertFalse((self.root / 'test.character').exists())

    def test_multiple_clips_keep_independent_payloads_timing_and_controls(self):
        second = self.root / 'kick.bin'
        pipeline.write_animation(second, {'frames': [[[1, 0, 0], [0, y, 0]] for y in (2, 3, 4)]})
        target = self.root / 'test.character'
        packager.package(self.rig, self.clip, [], target, 'test.character', extra_clips=[
            {'name': 'kick', 'path': second, 'key': 'Kick', 'mid_frames': 2}])
        self.assertEqual((target / 'assets/animations/kick.bytes').read_bytes(), second.read_bytes())
        report = json.loads((target / 'assets/animations/kick.rig.json').read_text())
        self.assertEqual((report['frames'], report['mid_frames']), (3, 2))
        self.assertEqual(report['control'], 'Kick')
        self.assertIn('"fps":20.0', (target / 'preview-kick.html').read_text())
        module = (target / 'scripts/character.lua').read_text()
        self.assertIn('moves["kick"]', module)
        self.assertIn('key = "Kick"', module)
        self.assertIn('mid_frames = 2, first_frame = 0, end_frame = 2', module)
        self.assertIn('memory.next_clip', module)
        self.assertIn('moves = moves', module)

    def test_bad_additional_clips_do_not_create_output(self):
        base = {'name': 'kick', 'path': self.clip, 'key': 'Kick', 'mid_frames': 0}
        for patch in ({'name': '../bad'}, {'name': 'authored'}, {'key': 'Punch'}, {'key': 'Left'}, {'mid_frames': 9}, {'mid_frames': True}):
            with self.subTest(patch=patch), self.assertRaises(ValueError):
                packager.package(self.rig, self.clip, [], self.root / 'test.character', 'test.character', extra_clips=[dict(base, **patch)])
            self.assertFalse((self.root / 'test.character').exists())
        for entries in ([base, base], [base, dict(base, name='other')]):
            with self.assertRaises(ValueError):
                packager.package(self.rig, self.clip, [], self.root / 'test.character', 'test.character', extra_clips=entries)
        bad = self.root / 'broken.bin'; bad.write_bytes(b'bad')
        with self.assertRaisesRegex(ValueError, 'Clip kick'):
            packager.package(self.rig, self.clip, [], self.root / 'test.character', 'test.character', extra_clips=[dict(base, path=bad)])
        self.assertFalse((self.root / 'test.character').exists())

    def test_cli_named_clip(self):
        target = self.root / 'test.character'
        subprocess.run([sys.executable, str(Path(packager.__file__)), '--rig', str(self.rig), '--animation', str(self.clip),
                        '--output', str(target), '--mod-id', 'test.character', '--clip', 'kick', 'Kick', '2', str(self.clip)], check=True, capture_output=True)
        self.assertTrue((target / 'assets/animations/kick.rig.json').is_file())

    def test_aggregate_sample_budget(self):
        with patch.object(packager, 'validate_clip', return_value={'frames': range(1_000_001), 'names': ['NHeel_1', 'NTop']}):
            with self.assertRaisesRegex(ValueError, 'two million'):
                packager.package(self.rig, self.clip, [], self.root / 'test.character', 'test.character')
        self.assertFalse((self.root / 'test.character').exists())


if __name__ == '__main__':
    unittest.main()

import json
from pathlib import Path
import tempfile
import unittest

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


if __name__ == '__main__':
    unittest.main()

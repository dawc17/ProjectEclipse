"""Exercise authored point animation and model validation without Blender."""
import copy
import tempfile
import unittest
from pathlib import Path
import xml.etree.ElementTree as ET
import CharacterPipeline as pipeline


class PipelineTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.path = Path(self.temp.name)
        self.rig_file = self.path / 'body.xml'
        self.rig_file.write_text('<Scene><Nodes><A Type="Node" Mass="1"/><B Type="Node" Mass="1"/><M Type="MacroNode" NodesCount="2" ChildNode1="A" ChildNode2="B" LCC1="0.5" LCC2="0.5"/></Nodes><Edges><AB Type="Edge" End1="A" End2="B"/></Edges><Figures><Body Type="Capsule" Edge="AB" Radius1="2" Radius2="3"/></Figures></Scene>')
        self.rig = pipeline.model(self.rig_file)
        self.source = {'names': ['B', 'M', 'A'], 'fps': 30, 'frames': [[[2, 0, 0], [99, 99, 99], [0, 0, 0]], [[4, 0, 0], [99, 99, 99], [2, 0, 0]]]}

    def tearDown(self):
        self.temp.cleanup()

    def test_reorder_resample_helpers_and_binary(self):
        clip = pipeline.bake(self.rig, self.source)
        self.assertEqual(clip['names'], ['A', 'B', 'M'])
        self.assertEqual(len(clip['frames']), 3)
        self.assertEqual(clip['frames'][1], [[1, 0, 0], [3, 0, 0], [2, 0, 0]])
        output = self.path / 'test.bytes'
        pipeline.write_animation(output, clip)
        self.assertEqual(pipeline.read_animation(output, self.rig), clip)
        output.write_bytes(output.read_bytes()[:-1])
        with self.assertRaises(ValueError): pipeline.read_animation(output, self.rig)

    def test_bad_animation_inputs(self):
        for change in [lambda s: s.update(fps=0), lambda s: s.update(names=['A', 'A', 'M']),
                       lambda s: s['frames'][0][0].__setitem__(0, float('nan')),
                       lambda s: s['frames'][0].pop(), lambda s: s.update(frames=[]),
                       lambda s: s.update(fps=float('inf')), lambda s: s.update(fps=240)]:
            source = copy.deepcopy(self.source); change(source)
            with self.assertRaises(ValueError): pipeline.bake(self.rig, source)

    def test_bad_model_references(self):
        for source in ['<Scene><Nodes><A Type="Node"/><A Type="Node"/></Nodes><Figures/></Scene>',
                       '<Scene><Nodes><A Type="Node"/></Nodes><Edges><E Type="Edge" End1="A" End2="missing"/></Edges><Figures/></Scene>',
                       '<Scene><Nodes><A Type="MacroNode" NodesCount="1" ChildNode1="A"/></Nodes><Figures/></Scene>',
                       '<Scene><Nodes><A Type="Node" Mass="nan"/></Nodes><Figures/></Scene>',
                       '<Scene><Nodes><A Type="Node" Mass="-1"/></Nodes><Figures/></Scene>',
                       '<!DOCTYPE Scene><Scene><Figures/></Scene>']:
            self.rig_file.write_text(source)
            with self.assertRaises(ValueError): pipeline.model(self.rig_file)

    def test_skin_binds_to_body(self):
        skin = self.path / 'skin.xml'
        skin.write_text('<Scene><Nodes><S Type="MacroNode" NodesCount="1" ChildNode1="A" LCC1="1"/></Nodes><Edges/><Figures><F Type="Triangle" Node1="A" Node2="B" Node3="S"/></Figures></Scene>')
        self.assertEqual(pipeline.model(skin, self.rig).tag, 'Scene')
        with self.assertRaises(ValueError): pipeline.model(skin)


if __name__ == '__main__':
    unittest.main()

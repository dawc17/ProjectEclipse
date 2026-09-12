"""Package native SF2 model/animation exports as an installable Eclipse preview mod.

Consumes Gymnast exports without changing their units, node order or binary data.
Does not import Blender or vendor third-party authoring code.
"""
import argparse
import hashlib
import json
from pathlib import Path
import re
import shutil
import tempfile
import xml.etree.ElementTree as ET

import CharacterPipeline as pipeline


def character_module(frame_count, skin_count, mid_frames):
    skins = ', '.join('sf2.assets.model("models/skin' + str(i + 1) + '")' for i in range(skin_count))
    return '''local sf2 = require("sf2")
local tactic = sf2.tactics.register {
    id = "authored_preview",
    on_decide = function(memory, event)
        if event.seconds < (memory.ready or 0) then return "wait" end
        for _, action in ipairs(event.actions) do
            if action.name == sf2.mod.id .. ":moves/authored_move" then
                memory.ready = event.seconds + 1.5
                return action
            end
        end
        return nil
    end,
}
local character = sf2.warriors.register {
    id = "authored_character", level = 1,
    template = sf2.warriors.get_template("core:warrior-templates/man_kungfu"),
    first_name = sf2.mod.id .. ":localization/fighter", last_name = "",
    body_model = sf2.assets.model("models/body"),
    skin_models = { SKINS }, tactic = sf2.tactics.name(tactic),
}
local move = sf2.moves.register {
    id = "authored_move", animation = sf2.assets.binary("animations/authored"),
    core_templates = { "Controlled", "NotTitan" }, type = "MOVE", priority = 150,
    mid_frames = SPACING, first_frame = 0, end_frame = LAST, mirror_node = "NHeel_1",
    events = { "key_pressed" },
    conditions = {
        { type = "character", warrior = character },
        { type = "keys", keys = { { key = "Punch", press = "Tap" } } },
        { type = "current_interval", name = "Uninterrupt", ["not"] = true },
    },
    intervals = { { type = "Uninterrupt", start = 0, ["end"] = LAST } },
}
return { warrior = character, move = move }
'''.replace('SKINS', skins).replace('SPACING', str(mid_frames)).replace('LAST', str(frame_count - 1))


PREVIEW_MAIN = '''local sf2 = require("sf2")
local authored = require("character")
local function key(name) return sf2.mod.id .. ":localization/" .. name end
local arena = sf2.locations.register {
    id = "arena", color = "0x1b2230", wall = 200, floor = 80,
    width = 1936, height = 512, min_width = 1936,
    layers = {
        { type = 1, factor = 1, images = {
            { sprite = sf2.assets.sprite("core:Textures/Locations/battlefield/battlefield_bg1.back_1"),
              x = 0, y = 0, width = 1936, height = 1024 },
        } },
        { type = 2, factor = 1, fighters = { player_x = 868, player_y = -94, enemy_x = 1068, enemy_y = -94 } },
    },
}
local zone = sf2.zones.register { id = "preview", file = "Map1.1", start = false }
local location = sf2.locations.name(arena)
local battle = sf2.battles.register {
    id = "preview", zone = zone, type = sf2.battles.STORY, x = 0, y = 0,
    alias = key("title"), title = key("title"), description = key("description"), location = location,
}
local loss = sf2.rewards.register { id = "loss", items = {} }
local win = sf2.rewards.register { id = "win", items = {} }
local fight = sf2.fights.register {
    id = "preview", battle = battle, rounds = 1, round_time = 99,
    location = location, warriors = { authored.warrior }, rewards = { loss, win },
}
sf2.modes.register { id = "preview", fights = { fight }, repeatable = true }
sf2.quests.register {
    id = "entry", place = "map", events = { "session" },
    actions = { { type = "show_battle", battle = battle, locked = false } },
}
'''


def package(rig_path, animation_path, skins, destination, mod_id, title='Character Preview', mid_frames=0):
    destination = Path(destination).resolve()
    if not re.fullmatch(r'[a-z0-9][a-z0-9_.-]{0,127}', mod_id) or mod_id in ('core', 'sf2de'):
        raise ValueError('Choose a non-reserved lowercase mod ID')
    if destination.name != mod_id:
        raise ValueError('Output directory name must equal the mod ID: ' + mod_id)
    if destination.exists():
        raise ValueError('Output already exists; choose a fresh directory to preserve authored files')
    if mid_frames not in range(9) or not title.strip() or len(title) > 80:
        raise ValueError('Use mid_frames 0..8 and a title of 1..80 characters')
    if len(skins) > 16:
        raise ValueError('At most 16 skin overlays are supported')
    rig = pipeline.model(rig_path)
    names = {n.tag for n in rig.find('Nodes')}
    if 'NHeel_1' not in names:
        raise ValueError('The preview move needs an SF2 rig with NHeel_1 for mirroring')
    combined = ET.fromstring(ET.tostring(rig))
    for path in skins:
        skin = pipeline.model(path, combined)
        for section in ('Nodes', 'Edges', 'Figures'):
            target = combined.find(section)
            if target is None:
                target = ET.SubElement(combined, section)
            if skin.find(section) is not None:
                target.extend(list(skin.find(section)))
    clip = pipeline.read_animation(animation_path, rig)
    if len(clip['frames']) < 2:
        raise ValueError('Preview needs at least two animation samples')
    # A common failure of the upstream exporter is a missing object written as
    # zero on every frame. Reject collapsed required bindings with their names.
    collapsed = [name for i, name in enumerate(clip['names'])
                 if any(abs(float(rig.find('Nodes')[i].get(axis, 0))) > 0.001 for axis in 'XYZ')
                 and all(all(value == 0 for value in frame[i]) for frame in clip['frames'])]
    if collapsed:
        raise ValueError('Nodes are zero throughout the animation; check missing Gymnast bindings: ' + ', '.join(collapsed))
    destination.parent.mkdir(parents=True, exist_ok=True)
    staging = Path(tempfile.mkdtemp(prefix='.character-package-', dir=destination.parent))
    try:
        for directory in ('assets/models', 'assets/animations', 'scripts', 'localizations'):
            (staging / directory).mkdir(parents=True)
        shutil.copyfile(rig_path, staging / 'assets/models/body.xml')
        shutil.copyfile(animation_path, staging / 'assets/animations/authored.bytes')
        for i, path in enumerate(skins, 1):
            shutil.copyfile(path, staging / f'assets/models/skin{i}.xml')
        module = character_module(len(clip['frames']), len(skins), mid_frames)
        (staging / 'scripts/character.lua').write_text(module, encoding='utf-8')
        (staging / 'scripts/main.lua').write_text(PREVIEW_MAIN, encoding='utf-8')
        (staging / 'mod.toml').write_text(
            f'schema = 1\nid = {json.dumps(mod_id)}\nname = {json.dumps(title, ensure_ascii=False)}\n'
            'version = "1.0.0"\napi = ">=0.22 <1.0"\nauthors = ["Local author"]\n'
            'entrypoint = "scripts/main.lua"\ncapabilities = ["content.register"]\n'
            '[[dependencies]]\nid = "core"\nversion = ">=1.0 <2.0"\n', encoding='utf-8')
        label = json.dumps(title, ensure_ascii=False)
        (staging / 'localizations/eng.toml').write_text(
            f'title = {label}\nzones/preview = {label}\nfighter = "Authored Fighter"\n'
            'description = "Watch the authored motion. This preview move deals no damage."\n', encoding='utf-8')
        metadata = {'version': 1, 'fps': 60, 'mid_frames': mid_frames, 'frames': len(clip['frames']),
                    'nodes': clip['names'], 'rig_sha256': hashlib.sha256(Path(rig_path).read_bytes()).hexdigest(),
                    'animation_sha256': hashlib.sha256(Path(animation_path).read_bytes()).hexdigest()}
        (staging / 'assets/animations/authored.rig.json').write_text(json.dumps(metadata, indent=2), encoding='utf-8')
        preview_clip = dict(clip, fps=60 / (mid_frames + 1))
        pipeline.preview(staging / 'preview.html', rig, preview_clip)
        (staging / 'README.md').write_text(
            f'# {title}\n\nEnable `{mod_id}` in Eclipse and Apply & Restart. Find **{title}** '
            'using the bottom map-page dots. The opponent selects its authored move when playable. '
            'This movement preview has no attack damage, entry cost or rewards.\n\n'
            'Edit scripts/character.lua to add attack intervals or change controls/tactics. '
            'Keep your source Blender scene separately. Validate facing, deformation, equipment and '
            'contact timing in the game. Check third-party asset permissions before sharing a package.\n', encoding='utf-8')
        staging.rename(destination)
    finally:
        if staging.exists() and staging.resolve().parent == destination.parent and staging.name.startswith('.character-package-'):
            shutil.rmtree(staging)
    return destination


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--rig', type=Path, required=True)
    parser.add_argument('--animation', type=Path, required=True)
    parser.add_argument('--skin', type=Path, action='append', default=[])
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--mod-id', required=True)
    parser.add_argument('--title', default='Character Preview')
    parser.add_argument('--mid-frames', type=int, choices=range(9), default=0)
    args = parser.parse_args()
    print('PACKAGED:', package(args.rig, args.animation, args.skin, args.output, args.mod_id, args.title, args.mid_frames))


if __name__ == '__main__':
    main()

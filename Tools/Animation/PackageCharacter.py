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


def character_module(frame_count, skin_count, mid_frames, extra_clips=()):
    skins = ', '.join('sf2.assets.model("models/skin' + str(i + 1) + '")' for i in range(skin_count))
    module = '''local sf2 = require("sf2")
local preview_moves = { PREVIEW_MOVES }
local tactic = sf2.tactics.register {
    id = "authored_preview",
    on_decide = function(memory, event)
        if event.seconds < (memory.ready or 0) then return "wait" end
        for offset = 0, #preview_moves - 1 do
            local index = ((memory.next_clip or 1) - 1 + offset) % #preview_moves + 1
            for _, action in ipairs(event.actions) do
                if action.name == sf2.mod.id .. ":moves/" .. preview_moves[index] then
                    memory.ready = event.seconds + 1.5
                    memory.next_clip = index % #preview_moves + 1
                    return action
                end
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
    move_names = ['authored_move'] + [clip['name'] for clip in extra_clips]
    module = module.replace('PREVIEW_MOVES', ', '.join(json.dumps(name) for name in move_names))
    marker = 'return { warrior = character, move = move }'
    additions = ['local moves = { authored_move = move }']
    for clip in extra_clips:
        additions.append('''moves[NAME] = sf2.moves.register {
    id = NAME, animation = sf2.assets.binary(ASSET),
    core_templates = { "Controlled", "NotTitan" }, type = "MOVE", priority = 150,
    mid_frames = SPACING, first_frame = 0, end_frame = LAST, mirror_node = "NHeel_1",
    events = { "key_pressed" },
    conditions = {
        { type = "character", warrior = character },
        { type = "keys", keys = { { key = KEY, press = "Tap" } } },
        { type = "current_interval", name = "Uninterrupt", ["not"] = true },
    },
    intervals = { { type = "Uninterrupt", start = 0, ["end"] = LAST } },
}'''.replace('NAME', json.dumps(clip['name'])).replace('ASSET', json.dumps('animations/' + clip['name']))
                         .replace('KEY', json.dumps(clip['key'])).replace('SPACING', str(clip['mid_frames']))
                         .replace('LAST', str(clip['frames'] - 1)))
    additions.append('return { warrior = character, move = move, moves = moves }')
    return module.replace(marker, '\n'.join(additions))


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


def package(rig_path, animation_path, skins, destination, mod_id, title='Character Preview', mid_frames=0, extra_clips=()):
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
    clips = [{'name': 'authored', 'path': Path(animation_path), 'key': 'Punch', 'mid_frames': mid_frames}]
    names_used, keys_used = {'authored', 'authored_move'}, {'Punch'}
    for entry in extra_clips:
        if set(entry) != {'name', 'path', 'key', 'mid_frames'}:
            raise ValueError('Additional clips require name, path, key and mid_frames')
        name, key, spacing = entry['name'], entry['key'], entry['mid_frames']
        if not isinstance(name, str) or not re.fullmatch(r'[a-z][a-z0-9_]{0,47}', name) or name in names_used:
            raise ValueError('Clip names must be unique lowercase identifiers; authored/authored_move are reserved')
        if key not in ('Kick', 'Ranged', 'Magic', 'Up', 'Down', 'Forward', 'Back') or key in keys_used:
            raise ValueError('Additional clips require distinct controls: Kick, Ranged, Magic, Up, Down, Forward or Back')
        if type(spacing) is not int or spacing not in range(9):
            raise ValueError('Clip mid_frames must be an integer from 0 to 8')
        names_used.add(name); keys_used.add(key)
        clips.append(dict(entry, path=Path(entry['path'])))
    samples = 0
    for entry in clips:
        try:
            entry['clip'] = validate_clip(entry['path'], rig)
        except (ValueError, OSError) as error:
            raise ValueError(f"Clip {entry['name']}: {error}") from error
        entry['frames'] = len(entry['clip']['frames'])
        samples += entry['frames'] * len(entry['clip']['names'])
        if samples > 2_000_000:
            raise ValueError('Character package exceeds two million node samples across all clips')
    clip = clips[0]['clip']
    destination.parent.mkdir(parents=True, exist_ok=True)
    staging = Path(tempfile.mkdtemp(prefix='.character-package-', dir=destination.parent))
    try:
        for directory in ('assets/models', 'assets/animations', 'scripts', 'localizations'):
            (staging / directory).mkdir(parents=True)
        shutil.copyfile(rig_path, staging / 'assets/models/body.xml')
        for entry in clips:
            shutil.copyfile(entry['path'], staging / f"assets/animations/{entry['name']}.bytes")
        for i, path in enumerate(skins, 1):
            shutil.copyfile(path, staging / f'assets/models/skin{i}.xml')
        module = character_module(len(clip['frames']), len(skins), mid_frames, clips[1:])
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
        for entry in clips:
            metadata = {'version': 1, 'fps': 60, 'mid_frames': entry['mid_frames'], 'frames': entry['frames'], 'control': entry['key'],
                        'nodes': entry['clip']['names'], 'rig_sha256': hashlib.sha256(Path(rig_path).read_bytes()).hexdigest(),
                        'animation_sha256': hashlib.sha256(entry['path'].read_bytes()).hexdigest()}
            (staging / f"assets/animations/{entry['name']}.rig.json").write_text(json.dumps(metadata, indent=2), encoding='utf-8')
            preview_clip = dict(entry['clip'], fps=60 / (entry['mid_frames'] + 1))
            preview_name = 'preview.html' if entry['name'] == 'authored' else f"preview-{entry['name']}.html"
            pipeline.preview(staging / preview_name, rig, preview_clip)
        (staging / 'README.md').write_text(
            f'# {title}\n\nEnable `{mod_id}` in Eclipse and Apply & Restart. Find **{title}** '
            'using the bottom map-page dots. The opponent cycles through eligible authored clips. '
            'This movement preview has no attack damage, entry cost or rewards.\n\n'
            'Edit scripts/character.lua to add attack intervals or change controls/tactics. '
            'Keep your source Blender scene separately. Validate facing, deformation, equipment and '
            'contact timing in the game. Check third-party asset permissions before sharing a package.\n\n'
            '## Clips\n\n' + '\n'.join(f"- {entry['name']}: {entry['key']}, mid_frames={entry['mid_frames']}, {entry['frames']} samples" for entry in clips) + '\n', encoding='utf-8')
        staging.rename(destination)
    finally:
        if staging.exists() and staging.resolve().parent == destination.parent and staging.name.startswith('.character-package-'):
            shutil.rmtree(staging)
    return destination


def validate_clip(path, rig):
    clip = pipeline.read_animation(path, rig)
    if len(clip['frames']) < 2:
        raise ValueError('Preview needs at least two animation samples')
    # Missing Gymnast objects can otherwise become constant zero coordinates.
    collapsed = [name for i, name in enumerate(clip['names'])
                 if any(abs(float(rig.find('Nodes')[i].get(axis, 0))) > 0.001 for axis in 'XYZ')
                 and all(all(value == 0 for value in frame[i]) for frame in clip['frames'])]
    if collapsed:
        raise ValueError('Nodes are zero throughout the animation; check missing Gymnast bindings: ' + ', '.join(collapsed))
    return clip


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--rig', type=Path, required=True)
    parser.add_argument('--animation', type=Path, required=True)
    parser.add_argument('--skin', type=Path, action='append', default=[])
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--mod-id', required=True)
    parser.add_argument('--title', default='Character Preview')
    parser.add_argument('--mid-frames', type=int, choices=range(9), default=0)
    parser.add_argument('--clip', nargs=4, action='append', default=[], metavar=('NAME', 'KEY', 'MID_FRAMES', 'FILE'),
                        help='Add a named animation with a distinct control and sample spacing; repeat for more clips')
    args = parser.parse_args()
    extra_clips = [{'name': name, 'key': key, 'mid_frames': int(spacing), 'path': Path(path)} for name, key, spacing, path in args.clip]
    print('PACKAGED:', package(args.rig, args.animation, args.skin, args.output, args.mod_id, args.title, args.mid_frames, extra_clips))


if __name__ == '__main__':
    main()

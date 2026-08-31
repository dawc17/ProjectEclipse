"""Install/disable the local ff3 editor experiment without changing vanilla XML."""
import argparse
import copy
from pathlib import Path
import shutil
import struct
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[2]
DEST = ROOT / 'Library/EclipseAnimationPreview'


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--disable', action='store_true')
    parser.add_argument('--animation', type=Path, default=ROOT/'ResearchSources/TekkenRetarget/kazuya_ff3.bytes')
    args = parser.parse_args()
    if args.disable:
        (DEST/'enabled').unlink(missing_ok=True)
        print('Disabled. Restart Unity Play mode to restore the vanilla front kick.')
        return
    data = args.animation.read_bytes()
    assert len(data) == 48544 and struct.unpack_from('<i',data)[0] == 60, 'Expected the 60-frame ff3 prototype'
    vanilla = ET.parse(ROOT/'Assets/vanillaXml/animations/moves.xml').getroot()
    move = copy.deepcopy(vanilla.find("Moves/Move[@Name='FrontKick']"))
    assert move is not None
    move.set('FileName','_eclipse_preview/kazuya_ff3.bytes')
    move.set('FirstFrame','0')
    move.set('EndFrame','59')
    move.set('MidFrames','0')
    # Preserve existing input/selection/templates and ordinary unarmed damage.
    # Only the tested motion, timing, sound cues, and kicking side change.
    for interval in move.find('Intervals'):
        if interval.get('Name') == 'Unstable':
            interval.set('Start','6')
            interval.set('End','38')
        elif interval.get('Name') == 'Uninterrupt':
            interval.set('End','46')
        elif interval.get('Type') == 'Block' or interval.get('Name') == 'Throwable':
            interval.set('Start','47')
        elif interval.get('Type') == 'Attack':
            interval.set('Start','18')
            interval.set('End','22')
            for edge in interval.find('AttackingParts'):
                edge.set('Name',edge.get('Name').removesuffix('_2')+'_1')
    for sound in move.findall('Actions/Sound'):
        sound.set('Frame','16' if sound.get('Name') == 'snd_swish5' else '10')
    DEST.mkdir(parents=True,exist_ok=True)
    ET.indent(move, space='  ')
    ET.ElementTree(move).write(DEST/'Move.xml',encoding='utf-8',xml_declaration=True)
    shutil.copyfile(args.animation,DEST/'kazuya_ff3.bytes')
    (DEST/'enabled').write_text('Local animation preview enabled\n')
    print('Enabled ff3 on forward + kick for the next Unity Play session.')
    print('Hit window: samples 18-22; uninterruptible through 46; ordinary FrontKick damage.')
    print(DEST)


if __name__ == '__main__':
    main()

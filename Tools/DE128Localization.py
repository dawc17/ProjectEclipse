"""Write a generator-owned section of DE128's per-language localization files."""
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
DIRECTORY = ROOT / 'Mods/de128/localizations'


def section_outputs(section, values, prefix):
    """Preserve other authors' sections; keep keys and translation bytes intact."""
    outputs = {}
    begin = '# BEGIN ' + section
    end = '# END ' + section
    languages = set(values) | {p.stem for p in DIRECTORY.glob('*.toml')}
    for language in sorted(languages):
        path = DIRECTORY / (language + '.toml')
        text = path.read_text(encoding='utf-8') if path.exists() else ''
        entries = values.get(language, {})
        block = begin + '\n' + ''.join(
            prefix + key + ' = ' + json.dumps(value, ensure_ascii=False) + '\n'
            for key, value in sorted(entries.items())) + end + '\n'
        if begin in text:
            start = text.index(begin)
            finish = text.index(end, start) + len(end)
            if text[finish:finish + 1] == '\n':
                finish += 1
            text = text[:start] + (block if entries else '') + text[finish:]
        elif entries:
            text += ('\n' if text and not text.endswith('\n\n') else '') + block
        outputs[path] = text
    return outputs


def write_outputs(outputs):
    for path, text in outputs.items():
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(text, encoding='utf-8', newline='\n')


def key_module(keys, prefix, *, native=False):
    lines = ['local sf2 = require("sf2")', '-- Text lives in localizations/<language>.toml.', 'local text = {}']
    # One stable archive-key index; no translations are executable Lua.
    lines.append('for _, key in ipairs({')
    lines.extend('    ' + json.dumps(key, ensure_ascii=False) + ',' for key in sorted(keys))
    lines.append('}) do')
    if native:
        lines.append('    text[key] = (sf2.mod.id .. ":localization/' + prefix + '" .. key):lower()')
    else:
        lines.append('    text[key] = sf2.localization.key("' + prefix + '" .. key)')
    lines.extend(['end', 'return text', ''])
    return '\n'.join(lines)

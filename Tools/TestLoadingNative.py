"""Prepare an isolated Unity fixture for loading and FX regression checks.

Run TestLoadingPerformance.py first. This only prepares files; launch the command
saved in command.json separately. Original assets, editor sessions and saves are untouched.
"""
import argparse
import json
import shutil
import uuid
from pathlib import Path


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--unity-editor", type=Path, required=True)
    args = parser.parse_args()
    root = Path(__file__).resolve().parent.parent
    generated = root / "Temp/LoadingPerformance"
    fixture = root / "Temp" / ("LoadingNative-" + uuid.uuid4().hex)
    editor = fixture / "Assets/Editor"
    editor.mkdir(parents=True)
    sources = sorted((root / "Assets/Scripts/Eclipse/Runtime/Modding").glob("*.cs"))
    sources += [root / "Assets/Scripts/Eclipse/Runtime/Presentation/ModVisualsRuntime.cs"]
    sources += [root / ("Assets/Scripts/Eclipse/Content/TarAssets/" + name + ".cs") for name in ["TarArchive", "TarAssetBundle", "TarAssetMeta", "WaveDecoder"]]
    sources += [root / "Tools/LoadingPerformance.cs", root / "Tools/LoadingNative.cs"]
    sources += [generated / (name + ".cs") for name in ["BaselineQuests", "BaselineMoves", "CurrentMoves", "BaselineTar"]]
    for source in sources:
        shutil.copy2(source, editor / source.name)
    (fixture / "ProjectSettings").mkdir()
    shutil.copy2(root / "ProjectSettings/ProjectVersion.txt", fixture / "ProjectSettings/ProjectVersion.txt")
    # PlayerPrefs, if needed, belong to this fixture's own company/product.
    (fixture / "ProjectSettings/ProjectSettings.asset").write_text("%YAML 1.1\n--- !u!129 &1\nPlayerSettings:\n  m_ObjectHideFlags: 0\n  companyName: EclipseTests\n  productName: " + fixture.name + "\n", encoding="utf-8")
    (fixture / "source-root.txt").write_text(str(root), encoding="utf-8")
    command = [str(args.unity_editor.resolve()), "-batchmode", "-nographics", "-projectPath", str(fixture), "-executeMethod", "LoadingNative.RunEditor", "-logFile", str(fixture / "validation.log")]
    (fixture / "command.json").write_text(json.dumps(command, indent=2), encoding="utf-8")
    print(fixture)


if __name__ == "__main__":
    main()

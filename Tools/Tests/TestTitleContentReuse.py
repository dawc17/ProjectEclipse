"""Exercise production title/parse handoff methods with controlled content and profiles.

Requires .NET 10. Uses an isolated Temp directory; no Unity session or real save is touched.
External parsers/rendering are stubs, so this is a lifecycle check, not a game playtest.
"""
import re
import subprocess
from pathlib import Path


def extract(source, signature):
    start = source.index(signature)
    opening = source.index("{", start)
    depth = 0
    tokens = r'''//[^\n]*|/\*.*?\*/|"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|[{}]'''
    for token in re.finditer(tokens, source[opening:], re.S):
        if token.group() == "{":
            depth += 1
        elif token.group() == "}":
            depth -= 1
            if depth == 0:
                return source[start:opening + token.end()]
    raise RuntimeError("Unclosed production body: " + signature)


def main():
    root = Path(__file__).resolve().parents[2]
    fixture = root / "Temp/TitleContentReuse"
    fixture.mkdir(parents=True, exist_ok=True)

    def read(path):
        return (root / path).read_text(encoding="utf-8-sig")

    title = read("Assets/Scripts/Eclipse/UI/TitleScreenScenery.cs")
    list_sf = read("Assets/Scripts/Assembly-CSharp/ListSF.cs")
    runtime = read("Assets/Scripts/Eclipse/Modding/ModRuntime.cs")
    scene = read("Assets/Scripts/Assembly-CSharp/Nekki/SF2/GUI/Scenes/GameLoaderScene.cs")
    methods = "\n".join(extract(list_sf, signature) for signature in [
        "public static void Reset()", "private static void TimeStep(",
        "internal void LoadTitlePreview()", "internal void DetachTitleProfile()",
        "internal void ResumeTitlePreview()", "public void IIKDNMBIHCM()",
        "private void CompleteGameContentLoad()", "private void PBNNPBEDOOJ()",
        "private int HFPJDOEEDCA()"])
    production = """using System; using System.IO; using System.Xml;
using System.Collections.Generic; using UnityEngine;
namespace Eclipse.UI { public static class TitleScreen {
public static void Load() => TitleGameData.Load();
public static void Enter() => TitleGameData.PrepareForEntry();
public static void Discard() => TitleGameData.Discard();
internal static bool TryResumeGameDataPreview() => TitleGameData.TryResume();
""" + extract(title, "private static class TitleGameData") + "}}\n"
    production += "public partial class ListSF {\n" + methods
    production += "\nprivate bool _titleContentReady; internal static bool CanResumeTitlePreview => _instance != null && _instance._titleContentReady; }\n"
    production += read("Assets/Scripts/Assembly-CSharp/ParseModule.cs")
    production += "\nnamespace Eclipse.Modding { public static partial class ModRuntime {\n"
    production += extract(runtime, "public static void RecordSaveContext(") + "}}\n"
    production += "namespace Nekki.SF2.GUI.Scenes { public static class GameLoaderScene {\n"
    production += "public static bool PIHEPFHMJHJ;\n" + extract(scene, "public static void Stop()")
    production += "\n" + extract(scene, "public static void DiscardTitlePreview()") + "}}\n"
    production += "public partial class UserItem {\n" + extract(read("Assets/Scripts/Assembly-CSharp/UserItem.cs"), "public void KIGHKCOCJFJ(") + "}\n"
    perks = read("Assets/Scripts/Assembly-CSharp/PerkTree.cs")
    production += "public partial class PerkTree {\n" + extract(perks, "public void FAGKACLCCPE()") + "\n" + extract(perks, "public void LJHPGKAOIAE()") + "}\n"
    (fixture / "Production.cs").write_text(production, encoding="utf-8")
    (fixture / "Test.csproj").write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework></PropertyGroup><ItemGroup><Compile Include="../../Tools/Tests/TitleContentReuseTests.cs" /></ItemGroup></Project>', encoding="utf-8")
    subprocess.run(["dotnet", "run", "--project", str(fixture / "Test.csproj"), "--verbosity", "quiet", "--", str(fixture / "userdata")], cwd=root, check=True)


if __name__ == "__main__":
    main()

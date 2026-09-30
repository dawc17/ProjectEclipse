"""Compare startup algorithms with a Git revision, using a managed fixture.

Requires Python, Git and .NET 10. No Unity editor or player data is touched.
Timing results are warm-cache microbenchmarks, not title-screen measurements.
"""
import argparse
import html
import re
import subprocess
from pathlib import Path


def methods(source, pattern):
    found = re.findall(pattern, source, re.M | re.S)
    if not found:
        raise RuntimeError("Production method extraction failed")
    return "\n".join(found)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--baseline", default="HEAD", help="Git revision to compare")
    args = parser.parse_args()
    root = Path(__file__).resolve().parent.parent
    fixture = root / "Temp" / "LoadingPerformance"
    fixture.mkdir(parents=True, exist_ok=True)

    def current(path):
        return (root / path).read_text(encoding="utf-8-sig")

    def baseline(path):
        return subprocess.check_output(["git", "show", f"{args.baseline}:{path}"], cwd=root).decode("utf-8-sig")

    quest_path = "Assets/Scripts/Eclipse/Runtime/Modding/CoreContentImporter.cs"
    quest = methods(baseline(quest_path), r"^        (?:public static int ImportQuestSources|private static bool IsQuestPath)\(.*?^        }")
    (fixture / "BaselineQuests.cs").write_text(
        "using System; using System.Collections.Generic; using System.IO; using System.Xml; using Eclipse.Modding;\n"
        "static class BaselineQuests {\n" + quest + "\n}", encoding="utf-8")
    animation_path = "Assets/Scripts/Assembly-CSharp/AnimationData.cs"
    pattern = r"^\t(?:public|private) static void CreateCapabilityTables?\(.*?^\t}"
    for name, source in [("BaselineMoves", baseline(animation_path)), ("CurrentMoves", current(animation_path))]:
        code = methods(source, pattern)
        (fixture / (name + ".cs")).write_text(
            "using System.Collections.Generic;\nstatic class " + name + " {\n"
            "public static List<InfoAnimation> _Animations = new List<InfoAnimation>();\n" + code + "\n}", encoding="utf-8")
    tar_path = "Assets/Scripts/Eclipse/Content/TarAssets/TarArchive.cs"
    (fixture / "BaselineTar.cs").write_text(baseline(tar_path).replace("TarArchive", "BaselineTarArchive").replace("TarWriter", "BaselineTarWriter"), encoding="utf-8")
    includes = [root / "Tools" / "LoadingPerformance.cs", fixture / "BaselineQuests.cs",
                fixture / "BaselineMoves.cs", fixture / "CurrentMoves.cs", fixture / "BaselineTar.cs", root / tar_path]
    includes += sorted((root / "Assets/Scripts/Eclipse/Runtime/Modding").glob("*.cs"))
    project = '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup>'
    project += "".join('<Compile Include="' + html.escape(str(path), quote=True) + '" />' for path in includes)
    project += "</ItemGroup></Project>"
    (fixture / "Test.csproj").write_text(project, encoding="utf-8")
    subprocess.run(["dotnet", "run", "--project", str(fixture / "Test.csproj"), "-c", "Release", "--verbosity", "quiet", "--", str(root)], cwd=root, check=True)


if __name__ == "__main__":
    main()

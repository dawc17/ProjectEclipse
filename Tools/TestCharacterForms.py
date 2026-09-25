#!/usr/bin/env python3
r"""Run the existing extracted-C# character-form fixtures without PowerShell.

This adapter reads the C# here-strings, production-source extraction expressions,
and template substitutions from the checked-in Test*.ps1 files. Their C# assertions
and controlled dependencies are reused unchanged. Default mode runs no Unity gameplay.

The compiler comes from an installed Unity editor's bundled .NET SDK. Compilation
uses an installed .NET 10 runtime's assemblies directly, so a system .NET SDK is
not required. All generated sources, binaries, response files and logs stay in a
unique ignored Temp directory. The default command does not launch Unity or restore
packages. --native explicitly runs the full-game fixture in an independent copy.

Example:
    python3 Tools/TestCharacterForms.py
    python3 Tools/TestCharacterForms.py --case TestFormModifierTransfer
    python3 Tools/TestCharacterForms.py --unity-editor /path/to/Editor/Unity
    python3 Tools/TestCharacterForms.py --prepare-native
    python3 Tools/TestCharacterForms.py --native
    python3 Tools/TestCharacterForms.py --native --reuse-native Temp/FormNative-EXISTING \
        --sync-native-source Assets/Scripts/Assembly-CSharp/Fight.cs

Reuse accepts only a stopped, marked clone directly under the fixture root.
Set ECLIPSE_NATIVE_FIXTURE_ROOT to use another disk for new and reused clones.
It retains the original logs and records each rerun, including backups and hashes
of explicitly synced inputs, under the clone's NativeRuns directory.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import subprocess
import sys
import tempfile
import time


ROOT = Path(__file__).resolve().parents[1]
CASES: dict[str, tuple[str, ...]] = {
    "TestModelTransitionBoundary": (),
    "TestPreparedFormModel": (),
    "TestAnimationNodeRebind": (),
    "TestFormRenderBindings": (),
    "TestFormParticipant": (),
    "TestFormCombatState": ("ModelController.cs", "ModelConditions.cs", "KeyData.cs", "FightCID.cs"),
    "TestFormPresentation": (),
    "TestFormParameters": (),
    "TestFormParameterCopy": (),
    "TestFormInitialization": (),
    "TestFormAnimationEntry": (),
    "TestFormModifierTransfer": ("ActionType.cs",),
    "TestPerkFlagTransfer": ("ActionType.cs", "PerkActionFlag.cs", "PerkActionModificator.cs",
                             "PerkActionVariable.cs", "PerkConditionModExists.cs"),
    "TestPreparedFormRequest": (),
    "TestFormCommit": (),
}


def match_required(pattern: str, text: str, label: str) -> re.Match[str]:
    match = re.search(pattern, text)
    if match is None:
        raise ValueError(f"Cannot extract {label}; the PowerShell fixture may have changed.")
    return match


def extract_fixture(name: str) -> tuple[str, list[Path]]:
    """Adapt only the small, explicit PowerShell fixture shape used by CASES."""
    script_path = ROOT / "Tools" / f"{name}.ps1"
    script = script_path.read_text(encoding="utf-8-sig")
    extra_sources = [ROOT / "Assets/Scripts/Assembly-CSharp" / item for item in CASES[name]]
    if name == "TestPerkFlagTransfer":
        return extract_flag_fixture(script), extra_sources
    template = match_required(r"(?ms)^\$code\s*=\s*@'\n(.*?)\n'@", script, f"{name} C# template")
    values: dict[str, str] = {}
    # Keep statements in their original order: a regex replacement may refine an
    # earlier extraction and += appends another production method to the same slot.
    reader = re.compile(
        r"^\$(\w+)\s*=\s*Get-Content -Raw(?: -LiteralPath)? "
        r"\(Join-Path \$root '([^']+)'\)\s*$"
    )
    extractor = re.compile(
        r"^\$(\w+)\s*(\+?=)\s*\[regex\]::Match\(\$(\w+),'([^']+)'\)\.Value\s*$"
    )
    transform = re.compile(
        r"^\$(\w+)\s*=\s*\[regex\]::Replace\(\$(\w+),'([^']+)',''\)\s*$"
    )
    for line in script[:template.start()].splitlines():
        if found := reader.fullmatch(line):
            variable, path = found.groups()
            source = (ROOT / path).resolve()
            if not source.is_relative_to(ROOT):
                raise ValueError(f"Fixture source leaves repository: {path}")
            values[variable] = source.read_text(encoding="utf-8-sig")
        elif found := extractor.fullmatch(line):
            variable, operation, source_variable, pattern = found.groups()
            extracted = match_required(pattern, values[source_variable], f"{name} ${variable}").group(0)
            values[variable] = values.get(variable, "") + extracted if operation == "+=" else extracted
        elif found := transform.fullmatch(line):
            variable, source_variable, pattern = found.groups()
            values[variable] = re.sub(pattern, "", values[source_variable])

    # Preserve the additional assertions that the PS runners perform outside C#.
    for variable, pattern in re.findall(r"\$(\w+) -notmatch '([^']+)'", script[:template.start()]):
        if re.search(pattern, values[variable]) is None:
            raise ValueError(f"{name}: production source guard failed: {pattern}")
    if name == "TestFormPresentation":
        adds = sorted(re.findall(r"AddEventListener\((\d+), (\w+)\)", values["attach"]))
        removes = sorted(re.findall(r"RemoveEventListener\((\d+), (\w+)\)", values["detach"]))
        if len(adds) != 13 or adds != removes:
            raise ValueError("Fight listener handover is not symmetric.")

    code = template.group(1)
    substitutions = re.findall(r"\.Replace\('([^']+)',\$(\w+)\)", script[template.end():])
    if not substitutions:
        raise ValueError(f"{name}: no production-source substitutions found")
    for placeholder, variable in substitutions:
        if placeholder not in code or variable not in values:
            raise ValueError(f"{name}: unknown substitution {placeholder} / ${variable}")
        code = code.replace(placeholder, values[variable])
    return code, extra_sources


def extract_flag_fixture(script: str) -> str:
    """Reuse the separate C# template and unique-method checks from its PS runner."""
    template_name = match_required(
        r"(?m)^\$code = Get-Content -Raw -LiteralPath \(Join-Path \$PSScriptRoot '([^']+)'\)",
        script, "flag template path",
    ).group(1)
    code = (ROOT / "Tools" / template_name).read_text(encoding="utf-8-sig")
    substitutions = re.findall(
        r"\.Replace\('([^']+)', \(Extract-Methods \$(\w+) \$(\w+)\)\)", script,
    )
    if len(substitutions) != 2:
        raise ValueError("Flag fixture production substitutions changed")
    for placeholder, source_variable, patterns_variable in substitutions:
        source_name = match_required(
            rf"(?m)^\${source_variable} = Get-Content -Raw -LiteralPath \(Join-Path \$sourceRoot '([^']+)'\)",
            script, f"flag ${source_variable}",
        ).group(1)
        source = (ROOT / "Assets/Scripts/Assembly-CSharp" / source_name).read_text(encoding="utf-8-sig")
        pattern_array = match_required(
            rf"(?ms)^\${patterns_variable} = @\((.*?)^\)", script, f"flag ${patterns_variable}",
        ).group(1)
        patterns = re.findall(r"'([^']+)'", pattern_array)
        if not patterns or placeholder not in code:
            raise ValueError(f"Flag fixture pattern/placeholder missing: {placeholder}")
        methods = []
        for pattern in patterns:
            found = list(re.finditer(pattern, source))
            if len(found) != 1:
                raise ValueError(f"Expected one production method for {pattern}; found {len(found)}")
            methods.append(found[0].group(0))
        code = code.replace(placeholder, "\n".join(methods))
    return code


def installed_editor(requested: Path | None) -> Path:
    if requested is not None:
        editor = requested.expanduser().resolve()
        if not editor.is_file():
            raise FileNotFoundError(f"Unity executable not found: {editor}")
        return editor
    version = match_required(
        r"(?m)^m_EditorVersion: (\S+)",
        (ROOT / "ProjectSettings/ProjectVersion.txt").read_text(encoding="utf-8"),
        "project Unity version",
    ).group(1)
    for base in (Path.home() / "Unity/Hub/Editor", Path("/opt/unity/Editor")):
        for name in ("Unity", "Unity.exe"):
            candidate = base / version / "Editor" / name
            if candidate.is_file():
                return candidate
    raise FileNotFoundError(f"Unity {version} not found; pass --unity-editor /path/to/Editor/Unity")


def version_key(value: str) -> tuple[int, ...]:
    return tuple(int(part) for part in re.findall(r"\d+", value))


def toolchain(editor: Path, requested_dotnet: str | None) -> tuple[list[str], str, str, Path]:
    sdk_root = editor.parent / "Data/DotNetSdk"
    compiler_host = sdk_root / ("dotnet.exe" if os.name == "nt" else "dotnet")
    compilers = sorted(
        (sdk_root / "sdk").glob("*/Roslyn/bincore/csc.dll"),
        key=lambda path: version_key(path.parents[2].name),
    )
    if not compiler_host.is_file() or not compilers:
        raise FileNotFoundError(f"Bundled .NET SDK/Roslyn not found under {sdk_root}")
    runtime_host = requested_dotnet or shutil.which("dotnet")
    if not runtime_host:
        raise FileNotFoundError("No .NET runtime host found; pass --dotnet /path/to/dotnet")
    result = subprocess.run([runtime_host, "--list-runtimes"], check=True, text=True, capture_output=True, timeout=15)
    runtimes = re.findall(r"(?m)^Microsoft.NETCore.App (10\.[^ ]+) \[(.+)\]$", result.stdout)
    if not runtimes:
        raise RuntimeError("The existing fixtures target net10.0; no .NET 10 runtime is available.")
    version, directory = max(runtimes, key=lambda item: version_key(item[0]))
    references = Path(directory) / version
    return [str(compiler_host), str(compilers[-1])], runtime_host, version, references


def response_argument(value: str) -> str:
    if any(character in value for character in ('"', "\n", "\r")):
        raise ValueError(f"Unsupported response-file path: {value!r}")
    return f'"{value}"'


def prepare_native(editor: Path) -> tuple[Path, list[str]]:
    """Copy project inputs independently; never share writable files by hardlink."""
    required = (
        "Assets/src/GUI/Scenes/GameLoaderScene/GameLoader.unity",
        "Packages/manifest.json", "Packages/packages-lock.json",
        "ProjectSettings/ProjectVersion.txt", "Tools/ValidateFormNative.cs",
        "Mods/example.shifting-guardian/mod.toml",
    )
    for name in required:
        if not (ROOT / name).is_file():
            raise FileNotFoundError(f"Native fixture input missing: {name}")
    temporary = native_fixture_root()
    temporary.mkdir(parents=True, exist_ok=True)
    fixture = Path(tempfile.mkdtemp(prefix="FormNative-", dir=temporary))
    print(f"Native fixture: {fixture}", flush=True)
    print("Copying Assets, Packages, ProjectSettings and cached packages independently.", flush=True)
    pairs = [(ROOT / name, fixture / name) for name in ("Assets", "Packages", "ProjectSettings")]
    cache = ROOT / "Library/PackageCache"
    if cache.is_dir():
        pairs.append((cache, fixture / "Library/PackageCache"))
    copier = shutil.which("cp") if sys.platform.startswith("linux") else None
    for source, destination in pairs:
        destination.parent.mkdir(parents=True, exist_ok=True)
        if copier:
            # GNU cp reflinks use separate inodes. -L also prevents copied links
            # from pointing into the user's project. No hardlink option is used.
            subprocess.run([copier, "--reflink=auto", "-R", "-L", "--preserve=mode,timestamps",
                            str(source), str(destination)], check=True, timeout=300)
        else:
            shutil.copytree(source, destination, symlinks=False)
    mods = fixture / "Mods"
    mods.mkdir()
    shutil.copytree(ROOT / "Mods/example.shifting-guardian", mods / "example.shifting-guardian")
    editor_sources = fixture / "Assets/Editor"
    editor_sources.mkdir(exist_ok=True)
    shutil.copy2(ROOT / "Tools/ValidateFormNative.cs", editor_sources / "ValidateFormNative.cs")
    (fixture / "form-native-fixture.marker").write_text("Isolated native form acceptance fixture\n", encoding="utf-8")
    command = [str(editor), "-batchmode", "-projectPath", str(fixture), "-executeMethod",
               "ValidateFormNative.RunEditor", "-logFile", str(fixture / "validation.log")]
    (fixture / "command.json").write_text(json.dumps(command, indent=2) + "\n", encoding="utf-8")
    print("Prepared native command: " + json.dumps(command), flush=True)
    return fixture, command


def native_fixture_root() -> Path:
    configured = os.environ.get("ECLIPSE_NATIVE_FIXTURE_ROOT")
    return Path(configured).expanduser().resolve() if configured else ROOT / "Temp"


def owned_native_fixture(requested: Path) -> Path:
    candidate = requested.expanduser()
    if not candidate.is_absolute():
        candidate = ROOT / candidate
    fixture = candidate.resolve(strict=True)
    if candidate.absolute() != fixture or fixture.parent != native_fixture_root() or not fixture.name.startswith("FormNative-"):
        raise ValueError("Native reuse requires a real FormNative-* directory directly under the configured fixture root.")
    marker = fixture / "form-native-fixture.marker"
    command_file = fixture / "command.json"
    for path in (marker, command_file, fixture / "ProjectSettings/ProjectVersion.txt"):
        if path.resolve() != path or not path.is_file():
            raise ValueError(f"Native clone ownership input is missing or linked: {path}")
    if marker.read_text(encoding="utf-8").strip() != "Isolated native form acceptance fixture":
        raise ValueError("Native clone marker does not match this acceptance fixture.")
    command = json.loads(command_file.read_text(encoding="utf-8"))
    if not isinstance(command, list) or not all(isinstance(value, str) for value in command):
        raise ValueError("Native clone command record is invalid.")
    for option, value in (("-projectPath", str(fixture)), ("-executeMethod", "ValidateFormNative.RunEditor")):
        if command.count(option) != 1 or command[command.index(option) + 1:command.index(option) + 2] != [value]:
            raise ValueError(f"Native clone command ownership does not match {option}.")
    if (fixture / "ProjectSettings/ProjectVersion.txt").read_bytes() != (ROOT / "ProjectSettings/ProjectVersion.txt").read_bytes():
        raise ValueError("Native clone and current project Unity versions differ; prepare a fresh clone.")
    if (fixture / "Temp/UnityLockfile").exists():
        raise ValueError("Native clone has a UnityLockfile; close its editor before reusing it.")
    return fixture


def sync_native_inputs(fixture: Path, evidence: Path, requested: list[str]) -> None:
    pairs: list[tuple[Path, Path]] = []
    for name in dict.fromkeys(requested):
        relative = Path(name)
        if relative.is_absolute() or ".." in relative.parts or not relative.parts:
            raise ValueError(f"Native sync requires a repository-relative file: {name}")
        if relative.parts[0] != "Assets" and relative.parts[:2] != ("Mods", "example.shifting-guardian"):
            raise ValueError(f"Native sync supports Assets or Mods/example.shifting-guardian files: {name}")
        pairs.append((ROOT / relative, relative))
    pairs.append((ROOT / "Tools/ValidateFormNative.cs", Path("Assets/Editor/ValidateFormNative.cs")))
    # Validate every path before writing any destination. Atomic replacement below
    # also avoids writing through an existing hardlink to another project.
    for source, relative in pairs:
        destination = fixture / relative
        if not source.is_file() or not source.resolve().is_relative_to(ROOT):
            raise ValueError(f"Native sync source is missing or outside this repository: {source}")
        if destination.resolve() != destination or (destination.exists() and not destination.is_file()):
            raise ValueError(f"Native sync destination is linked or is not a file: {destination}")
    changes = []
    for source, relative in pairs:
        destination = fixture / relative
        previous = destination.read_bytes() if destination.exists() else None
        incoming = source.read_bytes()
        changes.append({"source": str(source.relative_to(ROOT)), "destination": str(relative),
                        "before_sha256": hashlib.sha256(previous).hexdigest() if previous is not None else None,
                        "after_sha256": hashlib.sha256(incoming).hexdigest(), "changed": previous != incoming})
        if previous == incoming:
            continue
        if previous is not None:
            backup = evidence / "before" / relative
            backup.parent.mkdir(parents=True, exist_ok=True)
            backup.write_bytes(previous)
        destination.parent.mkdir(parents=True, exist_ok=True)
        descriptor, temporary_name = tempfile.mkstemp(prefix=".form-sync-", dir=destination.parent)
        temporary = Path(temporary_name)
        try:
            with os.fdopen(descriptor, "wb") as output:
                output.write(incoming)
            shutil.copystat(source, temporary)
            os.replace(temporary, destination)
        finally:
            temporary.unlink(missing_ok=True)
    (evidence / "inputs.json").write_text(json.dumps(changes, indent=2) + "\n", encoding="utf-8")
    print(f"Synced {sum(item['changed'] for item in changes)} native inputs; original inputs and hashes: {evidence}", flush=True)


def run_native(editor: Path, prepare_only: bool, timeout_seconds: int,
               reuse: Path | None = None, sync_sources: list[str] | None = None) -> int:
    if reuse is None:
        fixture, command = prepare_native(editor)
        evidence = fixture
    else:
        fixture = owned_native_fixture(reuse)
        runs = fixture / "NativeRuns"
        if runs.resolve() != runs:
            raise ValueError("Native clone evidence directory must not be a link.")
        runs.mkdir(exist_ok=True)
        evidence = Path(tempfile.mkdtemp(prefix="Run-", dir=runs))
        command = [str(editor), "-batchmode", "-projectPath", str(fixture), "-executeMethod",
                   "ValidateFormNative.RunEditor", "-logFile", str(evidence / "validation.log")]
        (evidence / "command.json").write_text(json.dumps(command, indent=2) + "\n", encoding="utf-8")
        print(f"Reusing native clone: {fixture.relative_to(ROOT)}\nRerun evidence: {evidence.relative_to(ROOT)}", flush=True)
    lock = fixture / "form-native-run.lock"
    try:
        descriptor = os.open(lock, os.O_WRONLY | os.O_CREAT | os.O_EXCL, 0o600)
    except FileExistsError as error:
        raise ValueError(f"Native clone already has a runner lock: {lock}") from error
    try:
        with os.fdopen(descriptor, "w") as output:
            output.write(str(os.getpid()) + "\n")
        if reuse is not None:
            sync_native_inputs(fixture, evidence, sync_sources or [])
        return execute_native(fixture, command, evidence, prepare_only, timeout_seconds)
    finally:
        lock.unlink(missing_ok=True)


def execute_native(fixture: Path, command: list[str], evidence: Path,
                   prepare_only: bool, timeout_seconds: int) -> int:
    if prepare_only:
        print("Prepared native command: " + json.dumps(command), flush=True)
        print("Native fixture prepared. Unity was not launched.", flush=True)
        return 0
    log = evidence / "validation.log"
    with (evidence / "launcher.log").open("x", encoding="utf-8") as output:
        process = subprocess.Popen(command, cwd=fixture, stdout=output, stderr=subprocess.STDOUT)
        started = time.monotonic()
        offset = 0
        print(f"Native fixture process: {process.pid}", flush=True)
        try:
            while True:
                try:
                    process.wait(timeout=15)
                except subprocess.TimeoutExpired:
                    pass
                if log.is_file():
                    with log.open(encoding="utf-8", errors="replace") as stream:
                        stream.seek(offset)
                        lines = stream.read().splitlines()
                        offset = stream.tell()
                    for line in lines:
                        if "[FormNative]" in line or "error CS" in line:
                            print(line, flush=True)
                if process.returncode is not None:
                    break
                elapsed = int(time.monotonic() - started)
                if elapsed >= timeout_seconds:
                    raise TimeoutError(f"Native fixture exceeded {timeout_seconds} seconds; inspect {log}")
                print(f"Native fixture running ({elapsed}s); log: {log.relative_to(ROOT)}", flush=True)
        except BaseException:
            process.terminate()
            try:
                process.wait(timeout=10)
            except subprocess.TimeoutExpired:
                process.kill()
                process.wait()
            raise
    text = log.read_text(encoding="utf-8", errors="replace") if log.is_file() else ""
    if process.returncode != 0 or "[FormNative] PASS:" not in text or "[FormNative] FAIL:" in text:
        print(f"Native fixture failed (exit {process.returncode}); inspect {log}", file=sys.stderr)
        return 1
    print(f"Native form acceptance passed. Evidence: {log}")
    return 0


def run_case(name: str, output: Path, compiler: list[str], host: str, version: str, references: Path) -> bool:
    directory = output / name
    directory.mkdir()
    code, extra_sources = extract_fixture(name)
    program = directory / "Program.cs"
    program.write_text(code + "\n", encoding="utf-8")
    assembly = directory / "Fixture.dll"
    runtime_config = directory / "Fixture.runtimeconfig.json"
    runtime_config.write_text(json.dumps({"runtimeOptions": {
        "tfm": "net10.0", "framework": {"name": "Microsoft.NETCore.App", "version": version}
    }}, indent=2) + "\n", encoding="utf-8")
    arguments = ["-nologo", "-noconfig", "-nostdlib+", "-target:exe", "-langversion:latest", "-nullable:disable",
                 "-nowarn:0169,0414,0649", "-out:" + response_argument(str(assembly))]
    arguments.extend("-r:" + response_argument(str(path)) for path in sorted(references.glob("*.dll")))
    arguments.extend(response_argument(str(path)) for path in [program, *extra_sources])
    response = directory / "compile.rsp"
    response.write_text("\n".join(arguments) + "\n", encoding="utf-8")
    commands = [compiler + ["@" + str(response)], [host, str(assembly)]]
    (directory / "commands.json").write_text(json.dumps(commands, indent=2) + "\n", encoding="utf-8")
    for phase, command in zip(("compile", "run"), commands):
        result = subprocess.run(command, cwd=ROOT, text=True, stdout=subprocess.PIPE,
                                stderr=subprocess.STDOUT, timeout=60 if phase == "compile" else 30)
        (directory / f"{phase}.log").write_text(result.stdout, encoding="utf-8")
        if result.returncode:
            print(f"FAIL {name}: {phase} exited {result.returncode}\n{result.stdout[-6000:]}", flush=True)
            return False
        if phase == "run":
            print(f"{name}: {result.stdout.strip()}", flush=True)
    return True


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--case", action="append", choices=CASES, help="Run a selected fixture; repeat as needed. Default: all.")
    parser.add_argument("--list", action="store_true", help="List fixtures without locating or launching tools.")
    parser.add_argument("--unity-editor", type=Path, help="Installed Unity executable: locates the compiler, or launches an explicitly requested native fixture.")
    parser.add_argument("--dotnet", help=".NET 10 runtime host. Defaults to dotnet on PATH.")
    native = parser.add_mutually_exclusive_group()
    native.add_argument("--native", action="store_true", help="Run ValidateFormNative in an independent project copy; fresh unless --reuse-native is supplied.")
    native.add_argument("--prepare-native", action="store_true", help="Prepare a native fixture and print its command without launching Unity.")
    parser.add_argument("--reuse-native", type=Path, help="Reuse a stopped owned FormNative-* clone under the configured fixture root; preserve its original logs and imported cache.")
    parser.add_argument("--sync-native-source", action="append", default=[], help="Copy one current repository-relative file into the reused clone, backing up its previous contents. Repeat as needed.")
    parser.add_argument("--native-timeout", type=int, default=900, help="Outer native process timeout in seconds, including import/compile. Default: 900.")
    args = parser.parse_args()
    if args.native_timeout <= 0:
        parser.error("--native-timeout must be positive")
    if (args.native or args.prepare_native) and (args.case or args.list):
        parser.error("Select either native acceptance or extracted fixture cases")
    if args.reuse_native and not (args.native or args.prepare_native):
        parser.error("--reuse-native requires --native or --prepare-native")
    if args.sync_native_source and not args.reuse_native:
        parser.error("--sync-native-source requires --reuse-native")
    if args.list:
        print("\n".join(CASES))
        return 0
    try:
        editor = installed_editor(args.unity_editor)
        if args.native or args.prepare_native:
            return run_native(editor, args.prepare_native, args.native_timeout, args.reuse_native, args.sync_native_source)
        compiler, host, version, references = toolchain(editor, args.dotnet)
        temporary = ROOT / "Temp"
        temporary.mkdir(exist_ok=True)
        output = Path(tempfile.mkdtemp(prefix="CharacterForms-", dir=temporary))
        print(f"Fixture evidence: {output.relative_to(ROOT)}", flush=True)
        print(f"Compiler: {compiler[1]}\nRuntime: {host} ({version})", flush=True)
        cases = list(dict.fromkeys(args.case or CASES))
        passed = 0
        for name in cases:
            try:
                passed += run_case(name, output, compiler, host, version, references)
            except (OSError, ValueError, KeyError, subprocess.SubprocessError) as error:
                print(f"FAIL {name}: {error}", flush=True)
        print(f"{passed}/{len(cases)} extracted production fixtures passed. Unity gameplay was not executed.")
        return 0 if passed == len(cases) else 1
    except (OSError, ValueError, RuntimeError, subprocess.SubprocessError) as error:
        print(f"Character-form validation unavailable: {error}", file=sys.stderr)
        return 2


if __name__ == "__main__":
    raise SystemExit(main())

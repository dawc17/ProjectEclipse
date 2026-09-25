#!/usr/bin/env python3
"""Boot DE128 in an independent Unity project and buy/reload archived shop items.

The project and player profile are isolated from the owner's open editor. The
validator reads archive XML only as test evidence; DE128 itself uses generated Lua.
"""

from __future__ import annotations

import argparse
import os
import shutil
import subprocess
import sys
from pathlib import Path

from TestCharacterForms import ROOT, owned_native_fixture, prepare_native


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    parser.add_argument("--unity-editor", type=Path, default=Path(r"F:\UnityInstalls\6000.6.0f1\Editor\Unity.exe"))
    parser.add_argument("--reuse-native", type=Path, help="Stopped FormNative-* project created by this test or TestCharacterForms.py")
    parser.add_argument("--timeout", type=int, default=1200)
    parser.add_argument("--phases", default="buy,reload,equip_upgrade,reload_equip_upgrade,shop_preview",
                        help="Comma-separated acceptance phases; use equip_upgrade,reload_equip_upgrade after a passing buy/reload run")
    args = parser.parse_args()
    if not args.unity_editor.is_file() or args.timeout < 1:
        parser.error("An installed Unity editor and positive timeout are required.")

    if args.reuse_native:
        fixture = owned_native_fixture(args.reuse_native)
    else:
        fixture, _ = prepare_native(args.unity_editor.resolve())
    if (fixture / "Temp/UnityLockfile").exists():
        raise RuntimeError("The isolated project is already open in Unity.")

    marker = fixture / "de128-shop-fixture.marker"
    marker.write_text("Isolated DE128 shop purchase/save acceptance\n", encoding="utf-8")
    shutil.copytree(ROOT / "Mods/de128", fixture / "Mods/de128", dirs_exist_ok=True)
    validator = fixture / "Assets/Editor/ValidateDE128ShopNative.cs"
    validator.parent.mkdir(parents=True, exist_ok=True)
    shutil.copy2(ROOT / "Tools/ValidateDE128ShopNative.cs", validator)

    phases = args.phases.split(",")
    if not phases or any(phase not in ("buy", "reload", "equip_upgrade", "reload_equip_upgrade", "shop_preview", "inspect", "forge")
                         for phase in phases):
        parser.error("Unknown shop acceptance phase.")
    for phase in phases:
        log = fixture / f"de128-shop-{phase}.log"
        command = [str(args.unity_editor.resolve()), "-batchmode", "-projectPath", str(fixture),
                   "-executeMethod", "ValidateDE128ShopNative.RunEditor", "-logFile", str(log)]
        environment = dict(os.environ, ECLIPSE_DE128_SHOP_PHASE=phase)
        print(f"{phase}: isolated Unity project {fixture}; log {log}", flush=True)
        result = subprocess.run(command, cwd=fixture, env=environment, timeout=args.timeout)
        lines = log.read_text(encoding="utf-8", errors="replace").splitlines() if log.exists() else []
        evidence = [line.strip() for line in lines if "[DE128ShopNative]" in line or "error CS" in line]
        for line in evidence[-35:]:
            print(line, flush=True)
        expected = ("[DE128ShopNative] PASS forge:" if phase == "forge" else
                    "[DE128ShopNative] PASS shop_preview:" if phase == "shop_preview" else
                    f"[DE128ShopNative] PASS {phase}: 221 live catalog rows")
        if result.returncode != 0 or not any(expected in line for line in evidence):
            raise RuntimeError(f"Unity shop {phase} failed (exit {result.returncode}); inspect {log}")
    return 0


if __name__ == "__main__":
    sys.exit(main())

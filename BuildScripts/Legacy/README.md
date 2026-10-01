# Historical reconstruction scripts

These scripts preserve earlier recovery work. They use repository-root relative
paths, so run them from the repository root after reviewing their assumptions.
They are not required by the current Unity 6.6 player build.

| Files | Original purpose |
| --- | --- |
| `decompress_lzma_assets.py` | Decode recovered LZMA payloads in place |
| `fix_apple_ext.py`, `gut_purchaser.py`, `remove_security.py` | Patch recovered purchasing/service source and plugins |
| `fix_selectanim.py` | Repair recovered animation-selection loops |
| `hook_devxml.py`, `offline_patch.py` | Patch earlier development XML and offline behavior |
| `gen_rsp.py`, `roslyn_fp.rsp`, `roslyn_main.rsp` | Manual compiler workflow with Unity 5.6 defines and machine-specific paths |

The patch scripts rewrite source/assets and may depend on names or code that
have since changed. Preserve them as evidence rather than rerunning them as
setup steps. Compiler outputs still use ignored `BuildScripts/out/`.

Use the [current build scripts](../README.md) and
[managed verification commands](../../AGENTS.md#build-and-verification) for current builds.

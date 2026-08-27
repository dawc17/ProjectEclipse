#!/usr/bin/env python3
"""Decrypt SF2 CDN TextAssets recovered from the 2.41.9 Android client.

The Android client derives these values by XORing pairs of embedded blobs.  The
results below are the derived Base64 values, not guesses or legacy SF2 keys.
"""

from __future__ import annotations

import argparse
import base64
import re
from pathlib import Path
from xml.etree import ElementTree

from Crypto.Cipher import AES


KEY = base64.b64decode("KewTRuIg9cTADekkts130la4lpFWqNy/1eFBe7KKzb0=")
IV = base64.b64decode("YHXVuEThtat3xjeX3NAnTg==")
HASHED_NAME = re.compile(r"^-?\d+_(.+)\.bin$", re.IGNORECASE)


def decrypt(path: Path) -> bytes:
    ciphertext = base64.b64decode(path.read_bytes().strip(), validate=True)
    if not ciphertext or len(ciphertext) % AES.block_size:
        raise ValueError("ciphertext length is not a non-empty AES block multiple")
    clear = AES.new(KEY, AES.MODE_CBC, IV).decrypt(ciphertext)
    padding = clear[-1]
    if padding < 1 or padding > AES.block_size or clear[-padding:] != bytes([padding]) * padding:
        raise ValueError("invalid PKCS#7 padding (wrong key or corrupt input)")
    return clear[:-padding]


def output_name(path: Path) -> str:
    match = HASHED_NAME.match(path.name)
    stem = match.group(1) if match else path.stem
    return stem + ".xml"


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("source", type=Path, help="Extracted bundle root or TextAsset directory")
    parser.add_argument("destination", type=Path, help="Directory for plaintext XML")
    parser.add_argument("--name", help="Regular expression used to limit input filenames")
    args = parser.parse_args()

    inputs = sorted(args.source.rglob("*.bin"))
    if args.name:
        name_filter = re.compile(args.name, re.IGNORECASE)
        inputs = [path for path in inputs if name_filter.search(path.name)]
    written = 0
    failures: list[str] = []
    for source in inputs:
        try:
            clear = decrypt(source)
            ElementTree.fromstring(clear)
            relative_parent = source.parent.relative_to(args.source)
            if relative_parent.name.lower() == "textasset":
                relative_parent = relative_parent.parent
            destination = args.destination / relative_parent / output_name(source)
            destination.parent.mkdir(parents=True, exist_ok=True)
            destination.write_bytes(clear)
            written += 1
        except Exception as error:
            failures.append(f"{source}: {error}")

    print(f"Decrypted {written}/{len(inputs)} XML TextAssets into {args.destination}")
    for failure in failures:
        print("FAILED " + failure)
    return 1 if failures else 0


if __name__ == "__main__":
    raise SystemExit(main())

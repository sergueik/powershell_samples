#!/usr/bin/env python3
import argparse
import sys
from fontTools.ttLib import TTFont

def parse_args():
    parser = argparse.ArgumentParser(
        description="Dump glyph names from a TTF/OTF file using fontTools."
    )
    parser.add_argument(
        "--file",
        help="Path to the .ttf/.otf font file"
    )
    parser.add_argument(
        "--noop",
        action="store_true",
        help="Only validate the file and exit (no glyph dump)"
    )
    return parser.parse_args()

def main():
    args = parse_args()

    # Validate font file open
    try:
        font = TTFont(args.file)
    except FileNotFoundError:
        print(f"ERROR: File not found: {args.fontfile}", file=sys.stderr)
        return 1
    except Exception as e:
        print(f"ERROR: Could not open font: {e}", file=sys.stderr)
        return 1

    # If noop: only validation, no further processing
    if args.noop:
        print(f"OK: Font '{args.fontfile}' loaded successfully (noop).")
        return 0

    try:
        glyphs = font.getGlyphOrder()
    except Exception as e:
        print(f"ERROR: Could not read glyph order: {e}", file=sys.stderr)
        return 1

    # Output glyphs
    for index, name in enumerate(glyphs):
        print(f"{index}: {name}")

    return 0


if __name__ == "__main__":
    sys.exit(main())


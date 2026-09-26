# NUX MG-30 Patch Format Findings

This document records Phase 1 findings from the checked-in `MG30AllPatch.mg30patch` reference file and the repository's existing inspection scripts. It intentionally separates observations from interpretations. No serializer changes are made in this phase.

## Evidence

- Reference file: `MG30AllPatch.mg30patch`
- Existing parser scripts: `parse.py`, `parse_params.py`, and `parse_struct.py`
- Existing serializer: `src/backend/NuxToneGenerator.Infrastructure/Services/PatchSerializer.cs`
- Existing tests: `src/backend/NuxToneGenerator.Tests/RealPatchFileTests.cs`
- Model names and documented controls: `mg30effects.pdf`

The reference file is a complete `1,267,456` byte file. `1,267,456 / 128 = 9,902`, and the first four bytes of each 9,902-byte block are sequential little-endian preset indices.

## Verified preset layout

All offsets below are relative to one preset unless stated otherwise.

| Region | Offset | Size | Evidence |
|---|---:|---:|---|
| Preset index | `0` | 4 | `parse_struct.py` reads `<I`; reference indices are 0 through 127 |
| Scene A | `4` | 142 | Scene B starts at `146` in the reference |
| Scene B | `146` | 142 | `4 + 142` |
| Scene C | `288` | 142 | `4 + 2 * 142` |
| Cabinet metadata | `430` | 36 | `4 + 3 * 142`; first preset has `01 00 00 00` followed by a 32-byte name field |
| RIFF/IR data | `466` | 8,236 | `parse.py` finds `RIFF` at absolute `466`; RIFF size field is `8,228`, so chunk size including the 8-byte header is `8,236` |
| Frequency response data | `8,702` | 1,200 | `466 + 8,236`; this reaches the end of the 9,902-byte preset |

The first RIFF of each preset is at `presetBase + 466`. The next preset begins at `presetBase + 9,902`. This repeats for the reference file.

### Cabinet and impulse response

The 36-byte region at `430` is observed as:

```text
00..03  four bytes; first reference preset: 01 00 00 00
04..35  32-byte ASCII/zero-padded cabinet or IR name
```

Therefore the first preset's cabinet name begins at preset offset `434` and is `ML-MEGA-GREEN-MIX-CL`. The reference contains a RIFF/WAV-like chunk beginning at `466`; its RIFF size field is `8228` and its chunk including the header is `8236` bytes. The remaining `1200` bytes are a separate frequency-response region by position and size.

The meaning of each of the four metadata bytes, beyond the observed first-preset value, is not verified. The exact WAV subchunk semantics and frequency-response encoding are also not established by the current evidence.

## Verified scene layout

Within each 142-byte scene:

| Region | Scene offset | Size | Evidence |
|---|---:|---:|---|
| Parameter/header area | `0..93` | 94 | `parse_params.py` inspects scene bytes `12..93`; no field-level mapping is established |
| Signal-chain bytes | `94..105` | 12 | Existing serializer/tests use `0x5E`; reference scene 1 contains `05 00 01 02 03 09 04 0A 06 08 07 0B` here |
| Preset/scene name | `106..121` (`0x6A`) | 16 | Direct byte inspection and `PatchSerializer`'s `NameLength = 16`; reference name is `Ac30 + SteelSing` |
| Remaining scene bytes | `122..141` | 20 | Present after the name in every 142-byte scene; semantics unresolved |

The signal-chain values are verified as bytes at this location, not as semantic block IDs. Their category/order meanings are unresolved. The existing serializer's default chain and the reference chain are not identical, so no new mapping is inferred from their numerical values. `parse_struct.py` currently reads 32 bytes from offset 104, which includes two signal-chain bytes and the following scene fields; that output should not be treated as a verified name-field definition.

## Important implementation discrepancy

`PatchSerializer` currently uses `SceneSize = 140`, while the reference file and the repository scripts demonstrate 142-byte scenes. Consequently, its current read/write boundaries do not match the reference after Scene A. The serializer also writes six cabinet-flag bytes while the reference evidence supports a 4-byte metadata prefix followed by the 32-byte name at offset `434`.

This is documented for Phase 2; it is deliberately not changed in Phase 1.

## Amplifier and effect mappings

### Verified

- `mg30effects.pdf` documents model names and controls.
- The reference scene contains a variable byte area in `0..93`.
- The reference signal chain contains twelve bytes at `94..105`.
- The reference parser output shows values in the parameter area ranging from 0 to 100 for many positions.

### Not verified

The current reference file is a collection of existing presets, not controlled one-variable exports. It cannot prove any of the following mappings:

- amplifier model ID encoding;
- amplifier Gain, Bass, Middle, Treble, Volume, or Presence offsets;
- effect model IDs;
- effect bypass/enable byte or polarity;
- effect parameter offsets or parameter order;
- semantic meaning of the twelve chain values;
- cabinet metadata flag meanings;
- IR microphone/type/position fields.

The byte-range output from `parse_params.py` is useful for locating candidates, but value variation alone does not establish a field mapping. No speculative mapping is recorded here.

## Required controlled exports for Phase 2

For each experiment, export two patches from QuickTone while changing exactly one value and keep every other setting identical:

1. Same preset, amplifier model changed only.
2. Same preset, each amplifier knob changed only, one at a time.
3. Same preset, each effect model changed only.
4. Same preset, each effect bypass state changed only.
5. Same preset, each effect parameter changed only.
6. Same preset, each signal-chain position changed only.
7. Same cabinet with IR disabled/enabled and with microphone/type/position changed individually.

Binary diffs between each pair can then establish exact offsets, encoded IDs, and bypass polarity. The original reference file must remain untouched.

## Phase 2 implementation plan

1. Correct the serializer's structural constants and preserve all 142 scene bytes plus the 4-byte trailing scene fields.
2. Add typed binary read/write helpers for only the mappings proven by controlled diffs.
3. Serialize the AI-generated amplifier/effect model IDs, bypass states, and parameters into those verified offsets.
4. Preserve unknown bytes from a raw reference scene where possible instead of zeroing them.
5. Add pairwise regression tests that assert each verified one-variable change alters only the expected byte range and round-trips through the serializer.
6. Validate generated patches against QuickTone and retain an unchanged golden reference fixture.
# FM21_Tools

Small F# tools to parse Football Manager 2021 player HTML exports and compute role ratings (example: Target Man - Attack).

## Contents
- `FM21_ToolsLib/HTML.fs` — HTML parser that loads players from an exported `all.html` table into `HTML.Player` records.
- `FM21_ToolsLib/ROLE.fs` — role rating utilities (example: `roleRatingTargetManAttack`, `bestTargetMenAttack`).
- `scripts/loadall.fsx` — example script that builds/loads the library and prints a sample ranking using `data/all.html`.

## Prerequisites
- .NET SDK (10.0 or compatible) installed. Verify with:
  - `dotnet --version`

## Build
From repository root:
- Build the library:
  - `dotnet build`
This produces the library under `FM21_ToolsLib/bin/Debug/net10.0/`.

## Run the example script
The example script `scripts/loadall.fsx` references the built DLL and expects the HTML export at `data/all.html`.

Options:
- From repo root (requires `dotnet fsi`):
  - `dotnet fsi scripts/loadall.fsx`
- Or change to `scripts` and run:
  - `cd scripts`
  - `dotnet fsi loadall.fsx`

If the script cannot find the DLL, ensure `dotnet build` completed successfully and the path in `#r` inside `loadall.fsx` matches the build output path.

## Data
Place your exported HTML table at `data/all.html`. The parser expects the table to contain at least 58 columns (the code currently accounts for several inserted columns after `Height`, and parses numeric attributes by index). If your export format differs, update `FM21_ToolsLib/HTML.fs` accordingly.

## Notes
- Parsing is implemented with regular expressions and basic HTML tag stripping—this is lightweight but not robust for arbitrary HTML. For fragile or complex inputs consider using an HTML parser library.
- Numeric parsing strips non-digit characters before converting; empty strings become `None`.

## License
This project is licensed under the MIT License — see the `LICENSE` file for details.

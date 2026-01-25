#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let sctpath = "../data/sct.html"
HTML.loadSctPlayers sctpath
printfn "Loaded %d players from %s" (List.length HTML.SctPlayers) sctpath

let tmas = SCOUT.getSctPlayerNamesForRoleAbove "TMA" 80.0

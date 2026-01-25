#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let sctpath = "../data/sct.html"
HTML.loadSctPlayers sctpath
printfn "Loaded %d players from %s" (List.length HTML.SctPlayers) sctpath

let tmas = SCOUT.getSctPlayersForRoleAbove "TMA" 75.0
let tmanms = SCOUT.getSctPlayerNamesForRoleAbove "TMA" 75.0
let tmanmschp = SCOUT.getSctPlayerNamesForRoleAboveBelowValue "TMA" 75.0 9600
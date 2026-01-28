#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let path = "../data/all.html"
HTML.loadPlayers path
printfn "Loaded %d players from %s" (List.length HTML.AllPlayers) path

let tmstr = TEAM.buildTeam HTML.AllPlayers |> TEAM.teamAsStrings
let scr = TEAM.teamScore (TEAM.buildTeam HTML.AllPlayers)


#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let path = "../data/all.html"
HTML.loadPlayers path
printfn "Loaded %d players from %s" (List.length HTML.AllPlayers) path

let team = TEAM.buildTeam HTML.AllPlayers
team |> TEAM.teamAsStrings |> List.iter (printfn "%s")
printfn "Team score: %f" (TEAM.teamScore team)


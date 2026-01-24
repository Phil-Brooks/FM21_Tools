#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let path = "../data/my.html"
HTML.loadMyPlayers path
printfn "Loaded %d players from %s" (List.length HTML.MyPlayers) path

let team = TEAM.buildTeam HTML.MyPlayers
team |> TEAM.teamAsStrings |> List.iter (printfn "%s")
printfn "Team score: %f" (TEAM.teamScore team)


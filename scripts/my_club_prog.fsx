#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let path = "../data/all.html"
do PROGRESS.loadOldPlayers path

let mypath = "../data/my2.html"
HTML.loadMyPlayers mypath
printfn "Loaded %d players from %s" (List.length HTML.MyPlayers) path

let pclub = 
    HTML.MyPlayers 
    |>List.map ROLE.bestRoleRatedPlayer
    |> List.choose id
    |> List.sortBy (fun rrp -> rrp.Name) 
    |> List.map PROGRESS.progressForRoleRatedPlayer
    |> List.map RRPPtoString
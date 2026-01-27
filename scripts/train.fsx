#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let div = "England (Sky Bet Championship)"
let path = "../data/all.html"
do HTML.loadPlayers path

let mypath = "../data/my2.html"
HTML.loadMyPlayers mypath
printfn "Loaded %d players from %s" (List.length HTML.MyPlayers) path

let brls = HTML.MyPlayers|>List.map (fun p -> p.Name , ROLE.bestRoleForPlayer p )

let rrps = HTML.MyPlayers|>List.map ROLE.bestRoleRatedPlayer

let wkas = 
    rrps
    |>List.map (fun brp -> if brp.IsSome then ROLE.weakestRelevantAttributeForPlayer brp.Value else None)

let wkas2 = 
    rrps
    |>List.map (fun brp -> if brp.IsSome then ROLE.secondWeakestRelevantAttributeForPlayer brp.Value else None)

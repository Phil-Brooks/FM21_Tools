#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let sctpath = "../data/sct.html"
HTML.loadSctPlayers sctpath
printfn "Loaded %d players from %s" (List.length HTML.SctPlayers) sctpath

let tmas = SCOUT.getSctPlayersForRoleAbove "TMA" 75.0
let tmaschp = tmas|>List.filter (SCOUT.roleRatedPlayerValueBelowK 9600)
let tmanmschp = tmaschp|>List.map SCOUT.roleRatedPlayerToNameRating 


let wtmas = SCOUT.getSctPlayersForRoleAbove "TMA" 50.0

let loanListedTmas = wtmas |> List.filter SCOUT.roleRatedPlayerLoanListed
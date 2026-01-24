#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let path = "../data/all.html"
do HTML.loadPlayers path
//let div2 = "England (Sky Bet League Two)"
//let div1 = "England (Sky Bet League One)"
let div = "England (Sky Bet Championship)"

// output
let output() =
    let (bestName, bestTeam, bestScoreOpt) = DIVISION.bestClub div
    printfn "Best club in %s: %s" div bestName
    match bestScoreOpt with
    | Some s -> printfn "Team score: %.2f" s
    | None -> printfn "Team incomplete, computed partial score: %.2f" (TEAM.teamScore bestTeam)

    printfn "\nSelected XI for %s:" bestName
    TEAM.teamAsStrings bestTeam |> List.iter (printfn "%s")
let oup = output()

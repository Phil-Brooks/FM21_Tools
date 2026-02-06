#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let divs = DIVISION.allDivisions()|>List.filter(fun d -> d.StartsWith "France")

let path = "../data/sct6.html"
do HTML.loadPlayers path
//let div2 = "England (Sky Bet League Two)"
//let div1 = "England (Sky Bet League One)"
//let div = "England (Sky Bet Championship)"
//let div = "England (Premier Division)"
//let div = "Holland (Eredivisie)"
let div = "France (Ligue 1 Uber Eats)"

//TEMP
let clbs = DIVISION.clubsInDivision div 
let clbtms = DIVISION.clubTeams div
let clbscr = clbtms |> List.map(fun (n,t,s) -> (n, TEAM.teamScore t, s))

//END
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

let avrol = DIVISION.averageRatingsByRole div
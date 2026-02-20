#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib


let path = "../data/all.html"
do HTML.loadPlayers path
let divs = DIVISION.allDivisions()|>List.filter(fun d -> d.StartsWith "Spain")


//let div2 = "England (Sky Bet League Two)"
//let div = "England (Sky Bet League One)"
//let div = "England (Sky Bet Championship)"
//let div = "England (Premier Division)"
//let div = "Holland (Eredivisie)"
let div = "France (Ligue 1 Uber Eats)"
//let div = "Germany (Bundesliga)"
//let div = "Spain (First Division)"

//TEMP
let clbs = DIVISION.clubsInDivision div 
let clbtms = DIVISION.clubTeamsZAZ div
let clbscr = clbtms |> List.map(fun (n,t,s) -> (n, TEAM.teamScore t, s))

//END
// output
let output() =
    let (bestName, bestTeam, bestScoreOpt) = DIVISION.bestClubZAZ div
    printfn "Best club in %s: %s" div bestName
    match bestScoreOpt with
    | Some s -> printfn "Team score: %.2f" s
    | None -> printfn "Team incomplete, computed partial score: %.2f" (TEAM.teamScore bestTeam)

    printfn "\nSelected XI for %s:" bestName
    TEAM.teamAsStrings bestTeam |> List.iter (printfn "%s")
let oup = output()

let avrol = DIVISION.averageRatingsByRoleZAZ div
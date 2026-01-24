#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let path = "../data/all.html"
let players = HTML.loadPlayers path
let div = "England (Sky Bet League Two)"
let clubs = CLUB.clubsInDivision div players

// helper to read extras (Extras: Map<string,string option>)
let private getExtra key (p: HTML.Player) =
    match Map.tryFind key p.Extras with
    | Some v -> v
    | None -> None

// players for a specific club in the chosen division
let playersInClub (clubName: string) =
    players
    |> List.filter (fun p ->
        match getExtra "Based" p with
        | Some d when d = div ->
            match getExtra "Club" p with
            | Some c when c = clubName -> true
            | _ -> false
        | _ -> false)

// build team + score for each club
let clubTeams =
    clubs
    |> List.map (fun clubName ->
        let pool = playersInClub clubName
        let team = TEAM.buildTeam pool
        let scoreOpt = TEAM.teamScoreOption team
        (clubName, team, scoreOpt))

// pick best club by preferring complete teams (Some score) and using -1.0 for incomplete
let bestClub =
    clubTeams
    |> List.maxBy (fun (_, team, scoreOpt) -> Option.defaultValue -1.0 scoreOpt)

// output
let (bestName, bestTeam, bestScoreOpt) = bestClub
printfn "Best club in %s: %s" div bestName
match bestScoreOpt with
| Some s -> printfn "Team score: %.2f" s
| None -> printfn "Team incomplete, computed partial score: %.2f" (TEAM.teamScore bestTeam)

printfn "\nSelected XI for %s:" bestName
TEAM.teamAsStrings bestTeam |> List.iter (printfn "%s")

namespace FM21_ToolsLib

open System

module DIVISION =

    // helper to extract the "Club" extra (Extras: Map<string,string option>)
    let private getClub (p: HTML.Player) : string option =
        match Map.tryFind "Club" p.Extras with
        | Some v -> v
        | None -> None

    // helper to extract the "Based" extra (Extras: Map<string,string option>)
    let private getBased (p: HTML.Player) : string option =
        match Map.tryFind "Based" p.Extras with
        | Some v -> v
        | None -> None

    /// Return a sorted, distinct list of all divisions (based) present in the player list.
    /// Players without a based value (None or empty) are ignored.
    let allDivisions (players: HTML.Player list) : string list =
        players
        |> List.choose getBased
        |> List.filter (fun s -> not (String.IsNullOrWhiteSpace s))
        |> List.distinct
        |> List.sort

    /// Return a sorted, distinct list of all clubs for the specified division (based).
    /// Players not in the given division or without a club are ignored.
    let clubsInDivision (division: string) (players: HTML.Player list) : string list =
        players
        |> List.filter (fun p ->
            match getBased p with
            | Some d when String.Equals(d, division, StringComparison.Ordinal) -> true
            | _ -> false)
        |> List.choose getClub
        |> List.filter (fun s -> not (String.IsNullOrWhiteSpace s))
        |> List.distinct
        |> List.sort

    // players for a specific club in the chosen division
    let playersInClub div players (clubName: string) =
        let getExtra key (p: HTML.Player) =
            match Map.tryFind key p.Extras with
            | Some v -> v
            | None -> None
        players
        |> List.filter (fun p ->
            match getBased p with
            | Some d when d = div ->
                match getClub p with
                | Some c when c = clubName -> true
                | _ -> false
            | _ -> false)

    // build team + score for each club
    let clubTeams div players =
        let clubs = clubsInDivision div players
        clubs
        |> List.map (fun clubName ->
            let pool = playersInClub div players clubName
            let team = TEAM.buildTeam pool
            let scoreOpt = TEAM.teamScoreOption team
            (clubName, team, scoreOpt))

    // pick best club by preferring complete teams (Some score) and using -1.0 for incomplete
    let bestClub div players =
        (clubTeams div players)
        |> List.maxBy (fun (_, team, scoreOpt) -> Option.defaultValue -1.0 scoreOpt)

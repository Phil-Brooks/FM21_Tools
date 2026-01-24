namespace FM21_ToolsLib

open System

module DIVISION =

    // helper to extract an extra (Extras: Map<string,string>)
    let private getExtra (key: string) (p: HTML.Player) : string option =
        Map.tryFind key p.Extras
        |> Option.filter (fun s -> not (String.IsNullOrWhiteSpace s))

    let private getClub = getExtra "Club"
    let private getBased = getExtra "Based"

    /// Return a sorted, distinct list of all divisions (based) present in the player list.
    /// Players without a based value (None or empty) are ignored.
    let allDivisions () : string list =
        HTML.AllPlayers
        |> List.choose getBased
        |> List.distinct
        |> List.sort

    /// Return a sorted, distinct list of all clubs for the specified division (based).
    /// Players not in the given division or without a club are ignored.
    let clubsInDivision (division: string) : string list =
        HTML.AllPlayers
        |> List.filter (fun p ->
            match getBased p with
            | Some d when String.Equals(d, division, StringComparison.Ordinal) -> true
            | _ -> false)
        |> List.choose getClub
        |> List.distinct
        |> List.sort

    // players for a specific club in the chosen division
    let playersInClub div (clubName: string) : HTML.Player list =
        HTML.AllPlayers
        |> List.filter (fun p ->
            getBased p
            |> Option.exists (fun d -> d = div && (getClub p |> Option.exists ((=) clubName))))

    // build team + score for each club
    let clubTeams div =
        let clubs = clubsInDivision div
        clubs
        |> List.map (fun clubName ->
            let pool = playersInClub div clubName
            let team = TEAM.buildTeam pool
            let scoreOpt = TEAM.teamScoreOption team
            (clubName, team, scoreOpt))

    // pick best club by preferring complete teams (Some score) and using -1.0 for incomplete
    let bestClub div =
        (clubTeams div)
        |> List.maxBy (fun (_, team, scoreOpt) -> Option.defaultValue -1.0 scoreOpt)

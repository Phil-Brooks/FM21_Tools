namespace FM21_ToolsLib

open System

module DIVISION =

    // helper to extract an extra (Extras: Map<string,string>)
    let private getExtra (key: string) (p: Player) : string option =
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
    let playersInClub div (clubName: string) :Player list =
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


    // --- NEW: average rating per role across the clubs' teams in a division ---

    // extract (RoleName, Rating option) for every position in a team
    let private teamRoleRatings (t: Team) : (string * float option) list =
        // helper for single optional role fields
        let fieldPair (opt: RoleRatedPlayer option) canonical =
            match opt with
            | Some r -> (r.RoleName, Some r.Rating)
            | None -> (canonical, None)

        let sweeper = fieldPair t.SweeperKeeper "SKD"
        let iwbR = fieldPair t.InvertedWingBackRight "IWBR"
        let iwbL = fieldPair t.InvertedWingBackLeft "IWBL"
        let bpd1 = fieldPair t.BallPlayingDef1 "BPD1"
        let bpd2 = fieldPair t.BallPlayingDef1 "BPD2"
        let wgr = fieldPair t.WingerAttackRight "WAR"
        let iwL = fieldPair t.InvertedWingerLeft "IWL"
        let bwm = fieldPair t.BallWinningMidfielderSupport "BWM"
        let ap = fieldPair t.AdvancedPlaymakerSupport "AP"
        let afa = fieldPair t.AdvancedForwardAttack "AFA"
        let tma = fieldPair t.TargetManAttack "TMA"

        [ sweeper; iwbR; iwbL; bpd1; bpd2; wgr; iwL; bwm; ap; afa; tma ]

    /// For the given division, compute the average rating for every role across
    /// the teams built for each club in that division.
    /// - Ignores unassigned positions (None ratings) when computing averages.
    /// - Returns a list of (RoleName, averageRating option). If no club provides
    ///   a rating for a role the result for that role is None.
    let averageRatingsByRole (division: string) : (string * float option) list =
        let teams = clubTeams division |> List.map (fun (_, team, _) -> team)
        if List.isEmpty teams then
            []
        else
            // preserve canonical role ordering from the TEAM definition by using first team
            let roleOrder = teamRoleRatings (List.head teams) |> List.map fst

            // aggregate ratings into role -> float list (skip None)
            let agg =
                teams
                |> List.collect teamRoleRatings
                |> List.fold (fun (m: Map<string, float list>) (role, rOpt) ->
                    let existing = defaultArg (Map.tryFind role m) []
                    match rOpt with
                    | Some v -> Map.add role (v :: existing) m
                    | None -> m) Map.empty

            // compute averages in canonical order
            roleOrder
            |> List.map (fun role ->
                match Map.tryFind role agg with
                | None -> (role, None)
                | Some [] -> (role, None)
                | Some vals ->
                    let avg = List.sum vals / float (List.length vals)
                    (role, Some avg))

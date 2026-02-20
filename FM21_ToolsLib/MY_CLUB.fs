namespace FM21_ToolsLib

open System

module MY_CLUB =

    /// Helpers to build teams from a player pool.
    let private buildFromPool pool = TEAM.buildTeam pool
    let private buildFromPoolZAZ pool = TEAM.buildZAZTeam pool
    let getFirstTeam () = buildFromPool HTML.MyPlayers
    let getSecondTeam () =
        let first = getFirstTeam ()
        HTML.MyPlayers
        |> List.filter (fun p -> not (TEAM.teamAsPositionNameOptions first |> List.choose snd |> Set.ofList |> Set.contains p.Name))
        |> buildFromPool

    // Third team: build from players not used in first or second teams.
    let getThirdTeam () =
        let first = getFirstTeam ()
        // build second locally so we can exclude its picks as well
        let second =
            HTML.MyPlayers
            |> List.filter (fun p -> not (TEAM.teamAsPositionNameOptions first |> List.choose snd |> Set.ofList |> Set.contains p.Name))
            |> buildFromPool

        let usedNames =
            (TEAM.teamAsPositionNameOptions first |> List.choose snd)
            @ (TEAM.teamAsPositionNameOptions second |> List.choose snd)
            |> Set.ofList

        HTML.MyPlayers
        |> List.filter (fun p -> not (Set.contains p.Name usedNames))
        |> buildFromPool

    /// Scores / printable views for first team.
    let getFirstTeamScore () = getFirstTeam () |> TEAM.teamScore
    let getFirstTeamScoreOption () = getFirstTeam () |> TEAM.teamScoreOption
    let getFirstTeamAsStrings () = getFirstTeam () |> TEAM.teamAsStrings

    /// ZAZ variants: build from pool using ZAZ formation
    let getFirstZAZTeam () = buildFromPoolZAZ HTML.MyPlayers
    let getSecondZAZTeam () =
        let first = getFirstZAZTeam ()
        HTML.MyPlayers
        |> List.filter (fun p -> not (TEAM.teamAsPositionNameOptions first |> List.choose snd |> Set.ofList |> Set.contains p.Name))
        |> buildFromPoolZAZ

    let getThirdZAZTeam () =
        let first = getFirstZAZTeam ()
        let second =
            HTML.MyPlayers
            |> List.filter (fun p -> not (TEAM.teamAsPositionNameOptions first |> List.choose snd |> Set.ofList |> Set.contains p.Name))
            |> buildFromPoolZAZ

        let usedNames =
            (TEAM.teamAsPositionNameOptions first |> List.choose snd)
            @ (TEAM.teamAsPositionNameOptions second |> List.choose snd)
            |> Set.ofList

        HTML.MyPlayers
        |> List.filter (fun p -> not (Set.contains p.Name usedNames))
        |> buildFromPoolZAZ

    /// Scores / printable views for first ZAZ team.
    let getFirstZAZTeamScore () = getFirstZAZTeam () |> TEAM.teamScore
    let getFirstZAZTeamScoreOption () = getFirstZAZTeam () |> TEAM.teamScoreOption
    let getFirstZAZTeamAsStrings () = getFirstZAZTeam () |> TEAM.teamAsStrings

    /// Scores / printable views for second ZAZ team.
    let getSecondZAZTeamScore () = getSecondZAZTeam () |> TEAM.teamScore
    let getSecondZAZTeamScoreOption () = getSecondZAZTeam () |> TEAM.teamScoreOption
    let getSecondZAZTeamAsStrings () = getSecondZAZTeam () |> TEAM.teamAsStrings

    /// Scores / printable views for third ZAZ team.
    let getThirdZAZTeamScore () = getThirdZAZTeam () |> TEAM.teamScore
    let getThirdZAZTeamScoreOption () = getThirdZAZTeam () |> TEAM.teamScoreOption
    let getThirdZAZTeamAsStrings () = getThirdZAZTeam () |> TEAM.teamAsStrings

    /// Scores / printable views for second team.
    let getSecondTeamScore () = getSecondTeam () |> TEAM.teamScore
    let getSecondTeamScoreOption () = getSecondTeam () |> TEAM.teamScoreOption
    let getSecondTeamAsStrings () = getSecondTeam () |> TEAM.teamAsStrings

    /// Scores / printable views for third team.
    let getThirdTeamScore () = getThirdTeam () |> TEAM.teamScore
    let getThirdTeamScoreOption () = getThirdTeam () |> TEAM.teamScoreOption
    let getThirdTeamAsStrings () = getThirdTeam () |> TEAM.teamAsStrings

    // -- Shared helpers for position lists / weakest attribute / comparisons --

    let posListForTeam (team: Team) : (string * RoleRatedPlayer option) list =
        [ ("SKD", team.SweeperKeeper)
          ("IWBR", team.InvertedWingBackRight)
          ("IWBL", team.InvertedWingBackLeft)
          ("BPD1", team.BallPlayingDef1)
          ("BPD2", team.BallPlayingDef2)
          ("WAR", team.WingerAttackRight)
          ("IWL", team.InvertedWingerLeft)
          ("BWM", team.BallWinningMidfielderSupport)
          ("AP", team.AdvancedPlaymakerSupport)
          ("AFA", team.AdvancedForwardAttack)
          ("TMA", team.TargetManAttack) ]

    /// Position list for ZAZ team mapping (SKS,IWBR,IWBL,BPD1,BPD2,DLP,MR,ML,MCA,AMSS1,AMSS2)
    let posListForZAZTeam (team: Team) : (string * RoleRatedPlayer option) list =
        [ ("SKS", team.SweeperKeeper)
          ("IWBR", team.InvertedWingBackRight)
          ("IWBL", team.InvertedWingBackLeft)
          ("BPD1", team.BallPlayingDef1)
          ("BPD2", team.BallPlayingDef2)
          ("MR", team.WingerAttackRight)
          ("ML", team.InvertedWingerLeft)
          ("DLP", team.BallWinningMidfielderSupport)
          ("MCA", team.AdvancedPlaymakerSupport)
          ("AMSS1", team.AdvancedForwardAttack)
          ("AMSS2", team.TargetManAttack) ]

    let private formatWeakest (roleAbbrev, playerName, attr, value) =
        sprintf "%s: %s -> weakest: %s (%d)" roleAbbrev playerName attr value

    let private getWeakestAttributesForTeam (team: Team) =
        posListForTeam team
        |> List.choose ROLE.weakestRelevantAttributeForPosition
        |> List.map formatWeakest

    let private getWeakestAttributesForZAZTeam (team: Team) =
        posListForZAZTeam team
        |> List.choose ROLE.weakestRelevantAttributeForPosition
        |> List.map formatWeakest

    /// Public: weakest relevant attribute list for first/second/third team.
    let getFirstTeamWeakestAttributes () = getFirstTeam () |> getWeakestAttributesForTeam
    let getSecondTeamWeakestAttributes () = getSecondTeam () |> getWeakestAttributesForTeam
    let getThirdTeamWeakestAttributes () = getThirdTeam () |> getWeakestAttributesForTeam

    let getFirstZAZTeamWeakestAttributes () = getFirstZAZTeam () |> getWeakestAttributesForZAZTeam
    let getSecondZAZTeamWeakestAttributes () = getSecondZAZTeam () |> getWeakestAttributesForZAZTeam
    let getThirdZAZTeamWeakestAttributes () = getThirdZAZTeam () |> getWeakestAttributesForZAZTeam

    /// Generic: find the single assigned player whose role rating is most below the average for that role.
    let private getTeamWeakestRelativeToDivision (team: Team) (division: string) : string option =
        let roleAverages = DIVISION.averageRatingsByRole division |> Map.ofList

        posListForTeam team
        |> List.choose (fun (roleAbbrev, posOpt) ->
            match posOpt with
            | Some r ->
                let rating = r.Rating
                let player = r.Player
                match Map.tryFind r.RoleName roleAverages with
                | Some (Some avg) ->
                    let delta = rating - avg
                    Some (roleAbbrev, r.RoleName, player.Name, rating, avg, delta)
                | _ -> None
            | None -> None)
        |> function
           | [] -> None
           | rels ->
               let (roleAbbrev, _, name, rating, avg, delta) = List.minBy (fun (_,_,_,_,_,d) -> d) rels
               Some (sprintf "%s: %s -> delta: %.2f (player %.2f vs avg %.2f)" roleAbbrev name delta rating avg)

    let getFirstTeamWeakestRelativeToDivision division = getFirstTeam () |> fun t -> getTeamWeakestRelativeToDivision t division
    let getSecondTeamWeakestRelativeToDivision division = getSecondTeam () |> fun t -> getTeamWeakestRelativeToDivision t division
    let getThirdTeamWeakestRelativeToDivision division = getThirdTeam () |> fun t -> getTeamWeakestRelativeToDivision t division

    let private getTeamWeakestRelativeToDivisionZAZ (team: Team) (division: string) : string option =
        let roleAverages = DIVISION.averageRatingsByRole division |> Map.ofList

        posListForZAZTeam team
        |> List.choose (fun (roleAbbrev, posOpt) ->
            match posOpt with
            | Some r ->
                let rating = r.Rating
                let player = r.Player
                match Map.tryFind r.RoleName roleAverages with
                | Some (Some avg) ->
                    let delta = rating - avg
                    Some (roleAbbrev, r.RoleName, player.Name, rating, avg, delta)
                | _ -> None
            | None -> None)
        |> function
           | [] -> None
           | rels ->
               let (roleAbbrev, _, name, rating, avg, delta) = List.minBy (fun (_,_,_,_,_,d) -> d) rels
               Some (sprintf "%s: %s -> delta: %.2f (player %.2f vs avg %.2f)" roleAbbrev name delta rating avg)

    let getFirstZAZTeamWeakestRelativeToDivision division = getFirstZAZTeam () |> fun t -> getTeamWeakestRelativeToDivisionZAZ t division
    let getSecondZAZTeamWeakestRelativeToDivision division = getSecondZAZTeam () |> fun t -> getTeamWeakestRelativeToDivisionZAZ t division
    let getThirdZAZTeamWeakestRelativeToDivision division = getThirdZAZTeam () |> fun t -> getTeamWeakestRelativeToDivisionZAZ t division

    /// Generic: compare a team's per-role ratings against division averages and produce a summary.
    let private getTeamComparisonToDivision (team: Team) (division: string) : string list =
        let roleAverages = DIVISION.averageRatingsByRole division |> Map.ofList

        // Build per-position comparison tuples: (formattedLine, playerRating, divisionAvg)
        let comparisons =
            posListForTeam team
            |> List.choose (fun (roleAbbrev, posOpt) ->
                match posOpt with
                | Some r ->
                    let rating = r.Rating
                    let player = r.Player
                    match Map.tryFind r.RoleName roleAverages with
                    | Some (Some avg) ->
                        let delta = rating - avg
                        let line = sprintf "%s: %s -> player %.2f vs avg %.2f -> delta %.2f" roleAbbrev player.Name rating avg delta
                        Some (line, rating, avg)
                    | _ ->
                        let line = sprintf "%s: %s -> player %.2f (no division avg)" roleAbbrev player.Name rating
                        Some (line, rating, Double.NaN)
                | None -> None)

        // Aggregate numeric comparisons where both player and division avg exist
        let deltas = comparisons |> List.choose (fun (_, r, a) -> if not (Double.IsNaN r) && not (Double.IsNaN a) then Some (r - a) else None)
        let playerRatings = comparisons |> List.choose (fun (_, r, _) -> if not (Double.IsNaN r) then Some r else None)
        let divisionRatings = comparisons |> List.choose (fun (_, _, a) -> if not (Double.IsNaN a) then Some a else None)

        let summaryLines =
            if List.isEmpty deltas then
                [ "No comparable ratings with division averages available." ]
            else
                let playerAvg = List.sum playerRatings / float (List.length playerRatings)
                let divisionAvg = List.sum divisionRatings / float (List.length divisionRatings)
                let avgDelta = List.sum deltas / float (List.length deltas)
                [ sprintf "Team average: %.2f vs Division average: %.2f -> delta %.2f" playerAvg divisionAvg avgDelta ]

        // return per-position lines followed by summary
        (comparisons |> List.map (fun (line, _, _) -> line)) @ summaryLines

    let getFirstTeamComparisonToDivision division = getFirstTeam () |> fun t -> getTeamComparisonToDivision t division
    let getSecondTeamComparisonToDivision division = getSecondTeam () |> fun t -> getTeamComparisonToDivision t division
    let getThirdTeamComparisonToDivision division = getThirdTeam () |> fun t -> getTeamComparisonToDivision t division

    let private getTeamComparisonToDivisionZAZ (team: Team) (division: string) : string list =
        let roleAverages = DIVISION.averageRatingsByRole division |> Map.ofList

        // Build per-position comparison tuples: (formattedLine, playerRating, divisionAvg)
        let comparisons =
            posListForZAZTeam team
            |> List.choose (fun (roleAbbrev, posOpt) ->
                match posOpt with
                | Some r ->
                    let rating = r.Rating
                    let player = r.Player
                    match Map.tryFind r.RoleName roleAverages with
                    | Some (Some avg) ->
                        let delta = rating - avg
                        let line = sprintf "%s: %s -> player %.2f vs avg %.2f -> delta %.2f" roleAbbrev player.Name rating avg delta
                        Some (line, rating, avg)
                    | _ ->
                        let line = sprintf "%s: %s -> player %.2f (no division avg)" roleAbbrev player.Name rating
                        Some (line, rating, Double.NaN)
                | None -> None)

        // Aggregate numeric comparisons where both player and division avg exist
        let deltas = comparisons |> List.choose (fun (_, r, a) -> if not (Double.IsNaN r) && not (Double.IsNaN a) then Some (r - a) else None)
        let playerRatings = comparisons |> List.choose (fun (_, r, _) -> if not (Double.IsNaN r) then Some r else None)
        let divisionRatings = comparisons |> List.choose (fun (_, _, a) -> if not (Double.IsNaN a) then Some a else None)

        let summaryLines =
            if List.isEmpty deltas then
                [ "No comparable ratings with division averages available." ]
            else
                let playerAvg = List.sum playerRatings / float (List.length playerRatings)
                let divisionAvg = List.sum divisionRatings / float (List.length divisionRatings)
                let avgDelta = List.sum deltas / float (List.length deltas)
                [ sprintf "Team average: %.2f vs Division average: %.2f -> delta %.2f" playerAvg divisionAvg avgDelta ]

        // return per-position lines followed by summary
        (comparisons |> List.map (fun (line, _, _) -> line)) @ summaryLines

    let getFirstZAZTeamComparisonToDivision division = getFirstZAZTeam () |> fun t -> getTeamComparisonToDivisionZAZ t division
    let getSecondZAZTeamComparisonToDivision division = getSecondZAZTeam () |> fun t -> getTeamComparisonToDivisionZAZ t division
    let getThirdZAZTeamComparisonToDivision division = getThirdZAZTeam () |> fun t -> getTeamComparisonToDivisionZAZ t division

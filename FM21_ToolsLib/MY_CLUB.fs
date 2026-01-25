namespace FM21_ToolsLib

open System

module MY_CLUB =

    /// Helpers to build teams from a player pool.
    let private buildFromPool pool = TEAM.buildTeam pool
    let getFirstTeam () = buildFromPool HTML.MyPlayers
    let getSecondTeam () =
        let first = getFirstTeam ()
        HTML.MyPlayers
        |> List.filter (fun p -> not (TEAM.teamAsPositionNameOptions first |> List.choose snd |> Set.ofList |> Set.contains p.Name))
        |> buildFromPool

    /// Scores / printable views for first team.
    let getFirstTeamScore () = getFirstTeam () |> TEAM.teamScore
    let getFirstTeamScoreOption () = getFirstTeam () |> TEAM.teamScoreOption
    let getFirstTeamAsStrings () = getFirstTeam () |> TEAM.teamAsStrings

    /// Scores / printable views for second team.
    let getSecondTeamScore () = getSecondTeam () |> TEAM.teamScore
    let getSecondTeamScoreOption () = getSecondTeam () |> TEAM.teamScoreOption
    let getSecondTeamAsStrings () = getSecondTeam () |> TEAM.teamAsStrings

    // -- Weakest relevant attribute per player --

    /// For a TEAM.Position with an assigned Player, return the weakest relevant attribute (roleAbbrev, player, attr, value).
    let private weakestRelevantAttributeForPosition (roleAbbrev: string, pos: TEAM.Position) : (string * string * string * int) option =
        pos.Player
        |> Option.bind (fun player ->
            match ROLE.getRelevantAttributesForRole pos.RoleName with
            | [] -> None
            | relevant ->
                relevant
                |> List.map (fun key -> key, (Map.tryFind key player.Attributes |> Option.defaultValue 0))
                |> List.minBy snd
                |> fun (attr, value) -> Some (roleAbbrev, player.Name, attr, value)
        )

    let private formatWeakest (roleAbbrev, playerName, attr, value) =
        sprintf "%s: %s -> weakest: %s (%d)" roleAbbrev playerName attr value

    let private getWeakestAttributesForTeam (team: TEAM.Team) =
        team
        |> TEAM.teamAsPositionNameOptions
        // Each element is only used to preserve the iteration order; we build the canonical posList and collect results.
        |> List.collect (fun (_rOpt, _pNameOpt) ->
            // reconstruct a lookup of role abbreviation to actual Position to find Player/Attributes
            // We can traverse the Team by building a list of (abbrev, Position) for the team:
            let posList =
                [ ("SKD", team.SweeperKeeper)
                  ("IWBR", team.InvertedWingBackRight)
                  ("IWBL", team.InvertedWingBackLeft) ]
                @ (team.BallPlayingDefs |> List.mapi (fun i p -> (sprintf "BPD%d" (i+1), p)))
                @ [ ("WAR", team.WingerAttackRight); ("IWL", team.InvertedWingerLeft);
                    ("BWM", team.BallWinningMidfielderSupport); ("AP", team.AdvancedPlaymakerSupport);
                    ("AFA", team.AdvancedForwardAttack); ("TMA", team.TargetManAttack) ]

            // for each abbrev/position, compute weakest and format
            posList
            |> List.choose weakestRelevantAttributeForPosition
            |> List.map formatWeakest
        )

    /// Public: weakest relevant attribute list for first/second team.
    let getFirstTeamWeakestAttributes () = getFirstTeam () |> fun t ->
        // reuse posList style above to preserve BPD indices and order
        let posList =
            [ ("SKD", t.SweeperKeeper)
              ("IWBR", t.InvertedWingBackRight)
              ("IWBL", t.InvertedWingBackLeft) ]
            @ (t.BallPlayingDefs |> List.mapi (fun i p -> (sprintf "BPD%d" (i+1), p)))
            @ [ ("WAR", t.WingerAttackRight); ("IWL", t.InvertedWingerLeft);
                ("BWM", t.BallWinningMidfielderSupport); ("AP", t.AdvancedPlaymakerSupport);
                ("AFA", t.AdvancedForwardAttack); ("TMA", t.TargetManAttack) ]
        posList |> List.choose weakestRelevantAttributeForPosition |> List.map formatWeakest

    let getSecondTeamWeakestAttributes () = getSecondTeam () |> fun t ->
        let posList =
            [ ("SKD", t.SweeperKeeper)
              ("IWBR", t.InvertedWingBackRight)
              ("IWBL", t.InvertedWingBackLeft) ]
            @ (t.BallPlayingDefs |> List.mapi (fun i p -> (sprintf "BPD%d" (i+1), p)))
            @ [ ("WAR", t.WingerAttackRight); ("IWL", t.InvertedWingerLeft);
                ("BWM", t.BallWinningMidfielderSupport); ("AP", t.AdvancedPlaymakerSupport);
                ("AFA", t.AdvancedForwardAttack); ("TMA", t.TargetManAttack) ]
        posList |> List.choose weakestRelevantAttributeForPosition |> List.map formatWeakest

    /// For the first team, find the single assigned player whose role rating is most below
    /// the average rating for that role in the specified division.
    let getFirstTeamWeakestRelativeToDivision (division: string) : string option =
        let team = getFirstTeam ()
        let positions =
            [ ("SKD", team.SweeperKeeper)
              ("IWBR", team.InvertedWingBackRight)
              ("IWBL", team.InvertedWingBackLeft) ]
            @ (team.BallPlayingDefs |> List.mapi (fun i p -> (sprintf "BPD%d" (i+1), p)))
            @ [ ("WAR", team.WingerAttackRight); ("IWL", team.InvertedWingerLeft);
                ("BWM", team.BallWinningMidfielderSupport); ("AP", team.AdvancedPlaymakerSupport);
                ("AFA", team.AdvancedForwardAttack); ("TMA", team.TargetManAttack) ]

        let roleAverages = DIVISION.averageRatingsByRole division |> Map.ofList

        positions
        |> List.choose (fun (roleAbbrev, pos) ->
            match pos.Player, pos.Rating with
            | Some player, Some rating ->
                match Map.tryFind pos.RoleName roleAverages with
                | Some (Some avg) ->
                    let delta = rating - avg
                    Some (roleAbbrev, pos.RoleName, player.Name, rating, avg, delta)
                | _ -> None
            | _ -> None)
        |> function
           | [] -> None
           | rels ->
               let (roleAbbrev, _, name, rating, avg, delta) = List.minBy (fun (_,_,_,_,_,d) -> d) rels
               Some (sprintf "%s: %s -> delta: %.2f (player %.2f vs avg %.2f)" roleAbbrev name delta rating avg)

    /// Same as above but for the second team.
    let getSecondTeamWeakestRelativeToDivision (division: string) : string option =
        let team = getSecondTeam ()
        let positions =
            [ ("SKD", team.SweeperKeeper)
              ("IWBR", team.InvertedWingBackRight)
              ("IWBL", team.InvertedWingBackLeft) ]
            @ (team.BallPlayingDefs |> List.mapi (fun i p -> (sprintf "BPD%d" (i+1), p)))
            @ [ ("WAR", team.WingerAttackRight); ("IWL", team.InvertedWingerLeft);
                ("BWM", team.BallWinningMidfielderSupport); ("AP", team.AdvancedPlaymakerSupport);
                ("AFA", team.AdvancedForwardAttack); ("TMA", team.TargetManAttack) ]

        let roleAverages = DIVISION.averageRatingsByRole division |> Map.ofList

        positions
        |> List.choose (fun (roleAbbrev, pos) ->
            match pos.Player, pos.Rating with
            | Some player, Some rating ->
                match Map.tryFind pos.RoleName roleAverages with
                | Some (Some avg) ->
                    let delta = rating - avg
                    Some (roleAbbrev, pos.RoleName, player.Name, rating, avg, delta)
                | _ -> None
            | _ -> None)
        |> function
           | [] -> None
           | rels ->
               let (roleAbbrev, _, name, rating, avg, delta) = List.minBy (fun (_,_,_,_,_,d) -> d) rels
               Some (sprintf "%s: %s -> delta: %.2f (player %.2f vs avg %.2f)" roleAbbrev name delta rating avg)

    /// Compare the first team's per-role ratings against the division averages and produce a summary.
    /// Returns a list of formatted lines: one per position plus a one-line team summary.
    let getFirstTeamComparisonToDivision (division: string) : string list =
        let team = getFirstTeam ()
        let positions =
            [ ("SKD", team.SweeperKeeper)
              ("IWBR", team.InvertedWingBackRight)
              ("IWBL", team.InvertedWingBackLeft) ]
            @ (team.BallPlayingDefs |> List.mapi (fun i p -> (sprintf "BPD%d" (i+1), p)))
            @ [ ("WAR", team.WingerAttackRight); ("IWL", team.InvertedWingerLeft);
                ("BWM", team.BallWinningMidfielderSupport); ("AP", team.AdvancedPlaymakerSupport);
                ("AFA", team.AdvancedForwardAttack); ("TMA", team.TargetManAttack) ]

        let roleAverages = DIVISION.averageRatingsByRole division |> Map.ofList

        // Build per-position comparison tuples: (formattedLine, playerRating, divisionAvg)
        let comparisons =
            positions
            |> List.choose (fun (roleAbbrev, pos) ->
                match pos.Player, pos.Rating with
                | Some player, Some rating ->
                    match Map.tryFind pos.RoleName roleAverages with
                    | Some (Some avg) ->
                        let delta = rating - avg
                        let line = sprintf "%s: %s -> player %.2f vs avg %.2f -> delta %.2f" roleAbbrev player.Name rating avg delta
                        Some (line, rating, avg)
                    | _ ->
                        let line = sprintf "%s: %s -> player %.2f (no division avg)" roleAbbrev player.Name rating
                        Some (line, rating, Double.NaN)
                | Some player, None ->
                    let line = sprintf "%s: %s -> unassigned rating" roleAbbrev player.Name
                    Some (line, Double.NaN, Double.NaN)
                | _ -> None)

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

    /// Compare the second team's per-role ratings against the division averages and produce a summary.
    /// Returns a list of formatted lines: one per position plus a one-line team summary.
    let getSecondTeamComparisonToDivision (division: string) : string list =
        let team = getSecondTeam ()
        let positions =
            [ ("SKD", team.SweeperKeeper)
              ("IWBR", team.InvertedWingBackRight)
              ("IWBL", team.InvertedWingBackLeft) ]
            @ (team.BallPlayingDefs |> List.mapi (fun i p -> (sprintf "BPD%d" (i+1), p)))
            @ [ ("WAR", team.WingerAttackRight); ("IWL", team.InvertedWingerLeft);
                ("BWM", team.BallWinningMidfielderSupport); ("AP", team.AdvancedPlaymakerSupport);
                ("AFA", team.AdvancedForwardAttack); ("TMA", team.TargetManAttack) ]

        let roleAverages = DIVISION.averageRatingsByRole division |> Map.ofList

        // Build per-position comparison tuples: (formattedLine, playerRating, divisionAvg)
        let comparisons =
            positions
            |> List.choose (fun (roleAbbrev, pos) ->
                match pos.Player, pos.Rating with
                | Some player, Some rating ->
                    match Map.tryFind pos.RoleName roleAverages with
                    | Some (Some avg) ->
                        let delta = rating - avg
                        let line = sprintf "%s: %s -> player %.2f vs avg %.2f -> delta %.2f" roleAbbrev player.Name rating avg delta
                        Some (line, rating, avg)
                    | _ ->
                        let line = sprintf "%s: %s -> player %.2f (no division avg)" roleAbbrev player.Name rating
                        Some (line, rating, Double.NaN)
                | Some player, None ->
                    let line = sprintf "%s: %s -> unassigned rating" roleAbbrev player.Name
                    Some (line, Double.NaN, Double.NaN)
                | _ -> None)

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

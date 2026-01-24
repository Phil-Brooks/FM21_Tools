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

    /// Flatten team positions preserving role names.
    let teamPositions (t: TEAM.Team) : TEAM.Position list =
        [ yield t.SweeperKeeper
          yield t.InvertedWingBackRight
          yield t.InvertedWingBackLeft
          yield! t.BallPlayingDefs
          yield t.WingerAttackRight
          yield t.InvertedWingerLeft
          yield t.BallWinningMidfielderSupport
          yield t.AdvancedPlaymakerSupport
          yield t.AdvancedForwardAttack
          yield t.TargetManAttack ]

    /// For a TEAM.Position with an assigned Player, return the weakest relevant attribute (role, player, attr, value).
    let private weakestRelevantAttributeForPosition (pos: TEAM.Position) : (string * string * string * int) option =
        pos.Player
        |> Option.bind (fun player ->
            match ROLE.getRelevantAttributesForRole pos.RoleName with
            | [] -> None
            | relevant ->
                relevant
                |> List.map (fun key -> key, (Map.tryFind key player.Attributes |> Option.defaultValue 0))
                |> List.minBy snd
                |> fun (attr, value) -> Some (pos.RoleName, player.Name, attr, value)
        )

    let private formatWeakest (role, playerName, attr, value) =
        sprintf "%s: %s -> weakest: %s (%d)" role playerName attr value

    let private getWeakestAttributesForTeam (team: TEAM.Team) =
        team
        |> teamPositions
        |> List.choose weakestRelevantAttributeForPosition
        |> List.map formatWeakest

    /// Public: weakest relevant attribute list for first/second team.
    let getFirstTeamWeakestAttributes () = getFirstTeam () |> getWeakestAttributesForTeam
    let getSecondTeamWeakestAttributes () = getSecondTeam () |> getWeakestAttributesForTeam

    /// For the first team, find the single assigned player whose role rating is most below
    /// the average rating for that role in the specified division.
    let getFirstTeamWeakestRelativeToDivision (division: string) : string option =
        let team = getFirstTeam ()
        let positions = teamPositions team
        let roleAverages = DIVISION.averageRatingsByRole division |> Map.ofList

        positions
        |> List.choose (fun pos ->
            match pos.Player, pos.Rating with
            | Some player, Some rating ->
                match Map.tryFind pos.RoleName roleAverages with
                | Some (Some avg) ->
                    let delta = rating - avg
                    Some (pos.RoleName, player.Name, rating, avg, delta)
                | _ -> None
            | _ -> None)
        |> function
           | [] -> None
           | rels ->
               let (role, name, rating, avg, delta) = List.minBy (fun (_,_,_,_,d) -> d) rels
               Some (sprintf "%s: %s -> delta: %.2f (player %.2f vs avg %.2f)" role name delta rating avg)

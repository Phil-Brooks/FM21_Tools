namespace FM21_ToolsLib

open System

module MY_CLUB =

    /// Build the first team from the currently loaded `HTML.MyPlayers`.
    let getFirstTeam () =
        TEAM.buildTeam HTML.MyPlayers

    /// Return the aggregate team score (sum of position ratings, missing ratings treated as 0.0).
    let getFirstTeamScore () =
        getFirstTeam () |> TEAM.teamScore

    /// Return the aggregate team score if the team is complete (all positions have ratings), otherwise None.
    let getFirstTeamScoreOption () =
        getFirstTeam () |> TEAM.teamScoreOption

    /// Return the first team as printable strings: "Role: PlayerName" list.
    let getFirstTeamAsStrings () =
        getFirstTeam () |> TEAM.teamAsStrings

    /// Remove players assigned in `team` from `pool` (matching by player name).
    let private removeAssignedPlayers (team: TEAM.Team) (pool: HTML.Player list) =
        let assignedNames =
            team
            |> TEAM.teamAsPositionNameOptions
            |> List.choose snd
            |> Set.ofList
        pool |> List.filter (fun p -> not (Set.contains p.Name assignedNames))

    /// Build the second team from the currently loaded `HTML.MyPlayers`.
    /// The second team is constructed from the remaining players after the first team selections are removed.
    let getSecondTeam () =
        let first = getFirstTeam ()
        let remainingPool = removeAssignedPlayers first HTML.MyPlayers
        TEAM.buildTeam remainingPool

    /// Aggregate score for second team.
    let getSecondTeamScore () =
        getSecondTeam () |> TEAM.teamScore

    /// Optional aggregate score for second team (None if incomplete).
    let getSecondTeamScoreOption () =
        getSecondTeam () |> TEAM.teamScoreOption

    /// Second team as printable strings.
    let getSecondTeamAsStrings () =
        getSecondTeam () |> TEAM.teamAsStrings

    // -- Weakest relevant attribute per player --

    /// Return all position objects from a Team as a flat list (preserves role names).
    let private teamPositions (t: TEAM.Team) : TEAM.Position list =
        List.concat [
            [ t.SweeperKeeper; t.InvertedWingBackRight; t.InvertedWingBackLeft ]
            t.BallPlayingDefs
            [ t.WingerAttackRight; t.InvertedWingerLeft; t.BallWinningMidfielderSupport;
              t.AdvancedPlaymakerSupport; t.AdvancedForwardAttack; t.TargetManAttack ]
        ]

    /// For a TEAM.Position with an assigned Player, return the weakest relevant attribute (role, player, attr, value).
    /// Uses ROLE.getRelevantAttributesForRole to restrict attributes considered.
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
        teamPositions team
        |> List.choose weakestRelevantAttributeForPosition
        |> List.map formatWeakest

    /// Public: get weakest relevant attribute for each assigned player in the first team.
    let getFirstTeamWeakestAttributes () =
        getFirstTeam () |> getWeakestAttributesForTeam

    /// Public: get weakest relevant attribute for each assigned player in the second team.
    let getSecondTeamWeakestAttributes () =
        getSecondTeam () |> getWeakestAttributesForTeam

namespace FM21_ToolsLib

open System

module MY_CLUB =

    /// Build the first team from the currently loaded `HTML.MyPlayers`.
    let getFirstTeam () : TEAM.Team =
        TEAM.buildTeam HTML.MyPlayers

    /// Return the aggregate team score (sum of position ratings, missing ratings treated as 0.0).
    let getFirstTeamScore () : float =
        getFirstTeam () |> TEAM.teamScore

    /// Return the aggregate team score if the team is complete (all positions have ratings), otherwise None.
    let getFirstTeamScoreOption () : float option =
        getFirstTeam () |> TEAM.teamScoreOption

    /// Return the first team as printable strings: "Role: PlayerName" list.
    let getFirstTeamAsStrings () : string list =
        getFirstTeam () |> TEAM.teamAsStrings

    /// Remove players assigned in `team` from `pool` (matching by player name).
    let private removeAssignedPlayers (team: TEAM.Team) (pool: HTML.Player list) =
        let assignedNames =
            team
            |> TEAM.teamAsPositionNameOptions
            |> List.choose snd
        pool |> List.filter (fun p -> not (List.exists ((=) p.Name) assignedNames))

    /// Build the second team from the currently loaded `HTML.MyPlayers`.
    /// The second team is constructed from the remaining players after the first team selections are removed.
    let getSecondTeam () : TEAM.Team =
        let first = getFirstTeam ()
        let remainingPool = removeAssignedPlayers first HTML.MyPlayers
        TEAM.buildTeam remainingPool

    /// Aggregate score for second team.
    let getSecondTeamScore () : float =
        getSecondTeam () |> TEAM.teamScore

    /// Optional aggregate score for second team (None if incomplete).
    let getSecondTeamScoreOption () : float option =
        getSecondTeam () |> TEAM.teamScoreOption

    /// Second team as printable strings.
    let getSecondTeamAsStrings () : string list =
        getSecondTeam () |> TEAM.teamAsStrings

    // -- New: weakest relevant attribute per player in first team --

    /// Return all position objects from a Team as a flat list (preserves role names).
    let private teamPositions (t: TEAM.Team) : TEAM.Position list =
        [ t.SweeperKeeper; t.InvertedWingBackRight; t.InvertedWingBackLeft ]
        @ t.BallPlayingDefs
        @ [ t.WingerAttackRight; t.InvertedWingerLeft; t.BallWinningMidfielderSupport;
            t.AdvancedPlaymakerSupport; t.AdvancedForwardAttack; t.TargetManAttack ]

    /// For a TEAM.Position with an assigned Player, return the weakest relevant attribute (name and value).
    /// Uses ROLE.getRelevantAttributesForRole to restrict attributes considered.
    let private weakestRelevantAttributeForPosition (pos: TEAM.Position) : (string * string * string * int) option =
        match pos.Player with
        | None -> None
        | Some player ->
            let relevant = ROLE.getRelevantAttributesForRole pos.RoleName
            if List.isEmpty relevant then
                None
            else
                // All numeric attribute keys are present in player's Attributes map (parsed earlier); default to 0 if missing.
                let values = relevant |> List.map (fun key -> key, (Map.tryFind key player.Attributes |> Option.defaultValue 0))
                let (attr, value) = values |> List.minBy snd
                Some (pos.RoleName, player.Name, attr, value)

    /// Public: get weakest relevant attribute for each assigned player in the first team.
    /// Returns strings in the form: "Role: Player -> weakest: Attr (value)".
    let getFirstTeamWeakestAttributes () : string list =
        let team = getFirstTeam ()
        teamPositions team
        |> List.choose weakestRelevantAttributeForPosition
        |> List.map (fun (role, playerName, attr, value) -> sprintf "%s: %s -> weakest: %s (%d)" role playerName attr value)

    /// Public: get weakest relevant attribute for each assigned player in the second team.
    /// Mirrors `getFirstTeamWeakestAttributes` but operates on the second team built from remaining players.
    let getSecondTeamWeakestAttributes () : string list =
        let team = getSecondTeam ()
        teamPositions team
        |> List.choose weakestRelevantAttributeForPosition
        |> List.map (fun (role, playerName, attr, value) -> sprintf "%s: %s -> weakest: %s (%d)" role playerName attr value)

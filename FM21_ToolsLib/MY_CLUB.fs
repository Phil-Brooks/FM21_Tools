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

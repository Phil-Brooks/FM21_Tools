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

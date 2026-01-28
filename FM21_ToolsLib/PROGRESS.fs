namespace FM21_ToolsLib

open System
open System.IO

module PROGRESS =

    let mutable OldPlayers : RoleRatedPlayer list = []

    /// Load OldPlayers from an HTML file using HTML.parsePlayersFromHtmlContent.
    /// Returns the list assigned to OldPlayers.
    let loadOldPlayers (path: string) : RoleRatedPlayer list =
        let content = File.ReadAllText(path)
        let players = HTML.parsePlayersFromHtmlContent content
        let rated = players |> List.choose ROLE.bestRoleRatedPlayer
        OldPlayers <- rated
        rated


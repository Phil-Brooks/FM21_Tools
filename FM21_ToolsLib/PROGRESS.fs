namespace FM21_ToolsLib

open System
open System.IO

module PROGRESS =

    let mutable OldPlayers : RoleRatedPlayer list = []

    /// Load OldPlayers from an HTML file using HTML.parsePlayersFromHtmlContent.
    /// Returns the list assigned to OldPlayers.
    let loadOldPlayers (path: string) : unit =
        let content = File.ReadAllText(path)
        let players = HTML.parsePlayersFromHtmlContent content
        let rated = players |> List.choose ROLE.bestRoleRatedPlayer
        OldPlayers <- rated

    /// Calculate improvement for a single `RoleRatedPlayer` compared to the previously-loaded `OldPlayers`.
    /// Matching is performed by Name (case-insensitive) and Height (trimmed); if no previous rating exists, `Progress` is `None`.
    let progressForRoleRatedPlayer (rr: RoleRatedPlayer) : RRPlayerProgress =
        let normalize (s: string) = if isNull s then "" else s.Trim().ToUpperInvariant()
        let oldOpt =
            OldPlayers
            |> List.tryFind (fun o ->
                (normalize o.Name) = (normalize rr.Name)
                && (normalize o.Player.Height) = (normalize rr.Player.Height))
        let progress = oldOpt |> Option.map (fun o -> rr.Rating - o.Rating)
        { Progress = progress; RRPlayer = rr }


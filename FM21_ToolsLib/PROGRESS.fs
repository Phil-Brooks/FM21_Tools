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

    let progressClub() = 
        HTML.MyPlayers 
        |> List.map ROLE.bestRoleRatedPlayer
        |> List.choose id
        |> List.map progressForRoleRatedPlayer
        // sort so players with a numeric Progress come first, sorted by Progress descending;
        // entries with no previous rating (None) are placed last
        |> List.sortWith (fun a b ->
            match a.Progress, b.Progress with
            | Some pa, Some pb -> compare pb pa  // descending order
            | Some _, None -> -1                 // a (Some) before b (None)
            | None, Some _ -> 1                  // a (None) after b (Some)
            | None, None -> 0)
        |> List.map RRPPtoString
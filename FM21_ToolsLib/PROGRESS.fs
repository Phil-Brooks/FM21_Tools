namespace FM21_ToolsLib

open System
open System.IO
open System.Text.RegularExpressions

module PROGRESS =

    let mutable OldPlayers : RoleRatedPlayer list = []
    let mutable CurPlayers : RoleRatedPlayer list = []

    /// Load OldPlayers from an HTML file using HTML.parsePlayersFromHtmlContent.
    /// Returns the list assigned to OldPlayers.
    let loadOldPlayers (path: string) : unit =
        let content = File.ReadAllText(path)
        let players = HTML.parsePlayersFromHtmlContent content
        let rated = players |> List.choose ROLE.bestRoleRatedPlayer
        OldPlayers <- rated

    /// Load CurPlayers from an HTML file using HTML.parsePlayersFromHtmlContent.
    /// Returns the list assigned to CurPlayers.
    let loadCurPlayers (path: string) : unit =
        let content = File.ReadAllText(path)
        let players = HTML.parsePlayersFromHtmlContent content
        let rated = players |> List.choose ROLE.bestRoleRatedPlayer
        CurPlayers <- rated

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

    // Try to parse DoB string into a DateTime. Fallback: extract 4-digit year if present and use Jan 1 of that year.
    let private tryParseDoB (s: string) : DateTime option =
        if String.IsNullOrWhiteSpace s then None
        else
            match DateTime.TryParse(s) with
            | true, d -> Some d
            | _ ->
                let m = Regex.Match(s, @"(\d{4})")
                if m.Success then
                    match Int32.TryParse(m.Groups.[1].Value) with
                    | true, y -> Some (DateTime(y, 1, 1))
                    | _ -> None
                else None

    let private ageAt (dob: DateTime) (onDate: DateTime) : int =
        let a = onDate.Year - dob.Year
        if dob > onDate.AddYears(-a) then a - 1 else a

    /// Return top N improvements for entries in `CurPlayers` relative to `OldPlayers`.
    /// Each item is a tuple: (Name, Club, Role, Height, Rating, Improvement).
    /// Only players aged under 21 (based on `Player.DoB`) are considered.
    let topImprovementsFromCurPlayers (topN: int) : (string * string * string * string * float * float) list =
        let normalize (s: string) = if isNull s then "" else s.Trim().ToUpperInvariant()
        CurPlayers
        |> List.choose (fun rr ->
            match tryParseDoB rr.Player.DoB with
            | Some dob when ageAt dob today < 21 ->
                OldPlayers
                |> List.tryFind (fun o ->
                    (normalize o.Name) = (normalize rr.Name)
                    && (normalize o.Player.Height) = (normalize rr.Player.Height))
                |> Option.map (fun o ->
                    let improvement = rr.Rating - o.Rating
                    let club = Map.tryFind "Club" rr.Player.Extras |> Option.defaultValue ""
                    (rr.Name, club, rr.RoleName, rr.Player.Height, rr.Rating, improvement))
            | _ -> None)
        |> List.sortByDescending (fun (_, _, _, _, _, imp) -> imp)
        |> List.truncate topN

    /// Convenience: return first 30 highest improvements.
    let top30Improvements() = topImprovementsFromCurPlayers 30

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
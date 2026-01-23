namespace FM21_ToolsLib

open System

module CLUB =

    // helper to extract the "Club" extra (Extras: Map<string,string option>)
    let private getClub (p: HTML.Player) : string option =
        match Map.tryFind "Club" p.Extras with
        | Some v -> v
        | None -> None

    // helper to extract the "Based" extra (Extras: Map<string,string option>)
    let private getBased (p: HTML.Player) : string option =
        match Map.tryFind "Based" p.Extras with
        | Some v -> v
        | None -> None

    /// Return a sorted, distinct list of all club names present in the player list.
    /// Players without a club (None or empty) are ignored.
    let allClubs (players: HTML.Player list) : string list =
        players
        |> List.choose getClub
        |> List.filter (fun s -> not (String.IsNullOrWhiteSpace s))
        |> List.distinct
        |> List.sort

    /// Return a sorted, distinct list of all divisions (based) present in the player list.
    /// Players without a based value (None or empty) are ignored.
    let allDivisions (players: HTML.Player list) : string list =
        players
        |> List.choose getBased
        |> List.filter (fun s -> not (String.IsNullOrWhiteSpace s))
        |> List.distinct
        |> List.sort

    /// Return a sorted, distinct list of all clubs for the specified division (based).
    /// Players not in the given division or without a club are ignored.
    let clubsInDivision (division: string) (players: HTML.Player list) : string list =
        players
        |> List.filter (fun p ->
            match getBased p with
            | Some d when String.Equals(d, division, StringComparison.Ordinal) -> true
            | _ -> false)
        |> List.choose getClub
        |> List.filter (fun s -> not (String.IsNullOrWhiteSpace s))
        |> List.distinct
        |> List.sort

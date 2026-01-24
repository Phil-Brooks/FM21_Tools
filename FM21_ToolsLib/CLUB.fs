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


namespace FM21_ToolsLib

open System

module CLUB =

    let private getExtra (key: string) (p: HTML.Player) : string option =
        Map.tryFind key p.Extras
        |> Option.filter (fun s -> not (String.IsNullOrWhiteSpace s))

    /// Return a sorted, distinct list of all club names present in the player list.
    /// Players without a club (None or empty) are ignored.
    let allClubs () : string list =
        HTML.AllPlayers
        |> List.choose (getExtra "Club")
        |> List.distinct
        |> List.sort


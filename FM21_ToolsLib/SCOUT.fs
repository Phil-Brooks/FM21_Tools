namespace FM21_ToolsLib

open System
open System.Text.RegularExpressions

module SCOUT =

    let private normalize (s: string) =
        if isNull s then "" else s.Trim().ToUpperInvariant()

    /// Map a TEAM role name or its abbreviation to the corresponding ROLE.roleRating function.
    /// Accepts full names (e.g. "Target Man (Attack)") or abbreviations (e.g. "TMA", "SKD", "BPD1", "BPD").
    let private roleRatingFnForRoleName (roleName: string) =
        let rn = normalize roleName
        if rn.StartsWith("BPD") || rn.StartsWith("BALL PLAYING DEFENDER") then
            ROLE.roleRatingBallPlayingDefender
        else
            match rn with
            | "TARGET MAN (ATTACK)" | "TMA" -> ROLE.roleRatingTargetManAttack
            | "ADVANCED FORWARD (ATTACK)" | "AFA" -> ROLE.roleRatingAdvancedForwardAttack
            | "WINGER (ATTACK) R" | "WAR" -> ROLE.roleRatingWingerAttackRight
            | "INVERTED WINGER (L)" | "IWL" -> ROLE.roleRatingInvertedWingerSupportLeft
            | "ADVANCED PLAYMAKER (SUPPORT)" | "AP" -> ROLE.roleRatingAdvancedPlaymakerSupport
            | "BALL WINNING MIDFIELDER (SUPPORT)" | "BWM" -> ROLE.roleRatingBallWinningMidfielderSupport
            | "INVERTED WING BACK (R)" | "IWBR" -> ROLE.roleRatingInvertedWingBackSupportRight
            | "INVERTED WING BACK (L)" | "IWBL" -> ROLE.roleRatingInvertedWingBackSupportLeft
            | "SWEEPER KEEPER" | "SKD" -> ROLE.roleRatingSweeperKeeperDefend
            // fallback: function that always returns None
            | _ -> fun (_: HTML.Player) -> None

    // Parse money strings like "£185K", "£1.2M", "Free Transfer", "Not set" into integer value in whole GBP.
    let private tryParseMoney (s: string) : int64 =
        if isNull s then 0L else
        let str = s.Trim().ToUpperInvariant()
        if str = "" then 0L
        elif str.Contains("FREE") || str.Contains("NOT SET") then 0L
        else
            // Remove currency symbols and spaces, keep suffix letter if present
            let cleaned = Regex.Replace(str, "[£€$\\s]", "")
            // Determine multiplier from suffix
            let multiplier =
                if cleaned.EndsWith("M") then 1_000_000L
                elif cleaned.EndsWith("K") then 1_000L
                else 1L
            // Extract numeric portion (handles decimals like 1.2M)
            let m = Regex.Match(cleaned, @"([\d.,]+)")
            if not m.Success then 0L
            else
                let numStr = m.Groups.[1].Value.Replace(",", ".")
                match Double.TryParse(numStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture) with
                | true, v -> int64 (v * float multiplier)
                | _ -> 0L

    let private playerMarketValue (p: HTML.Player) : int64 =
        match Map.tryFind "Value" p.Extras with
        | Some v -> tryParseMoney v
        | None -> 0L

    /// Return (Name, Rating, Player) for all players in HTML.SctPlayers whose computed role rating is > threshold.
    let getSctPlayersForRoleAbove (roleName: string) (threshold: float) : (string * float * HTML.Player) list =
        let ratingFn = roleRatingFnForRoleName roleName
        HTML.SctPlayers
        |> List.choose (fun p ->
            match ratingFn p with
            | Some r when r > threshold -> Some (p.Name, r, p)
            | _ -> None)
        |> List.sortByDescending (fun (_, r, _) -> r)

    /// Return (Name, Rating, Player) for all players in HTML.SctPlayers whose computed role rating is > threshold
    /// and whose market value is <= maxValueK (expressed in thousands; e.g. 185 means £185K).
    let getSctPlayersForRoleAboveBelowValue (roleName: string) (threshold: float) (maxValueK: int) : (string * float * HTML.Player) list =
        let ratingFn = roleRatingFnForRoleName roleName
        let maxValueInGbp = int64 maxValueK * 1000L
        HTML.SctPlayers
        |> List.choose (fun p ->
            match ratingFn p with
            | Some r when r > threshold && (playerMarketValue p) <= maxValueInGbp -> Some (p.Name, r, p)
            | _ -> None)
        |> List.sortByDescending (fun (_, r, _) -> r)

    /// Convenience: return only (Name, Rating) pairs from SctPlayers above threshold for the given role.
    let getSctPlayerNamesForRoleAbove (roleName: string) (threshold: float) : (string * float) list =
        getSctPlayersForRoleAbove roleName threshold |> List.map (fun (n, r, _) -> (n, r))

    /// Convenience: return only (Name, Rating) pairs from SctPlayers above threshold for the given role and below max market value.
    /// `maxValueK` is an `int` representing thousands (e.g. `500` => £500K).
    let getSctPlayerNamesForRoleAboveBelowValue (roleName: string) (threshold: float) (maxValueK: int) : (string * float) list =
        getSctPlayersForRoleAboveBelowValue roleName threshold maxValueK |> List.map (fun (n, r, _) -> (n, r))

namespace FM21_ToolsLib

open System
open System.Text.RegularExpressions
open System.Globalization

module SCOUT =

    /// Structured result for a player rated for a role.
    type RoleRatedPlayer = { Name: string; RoleName: string; Rating: float; Player: HTML.Player }

    let private norm (s: string) = if isNull s then "" else s.Trim().ToUpperInvariant()

    /// Lightweight lookup for role name/abbreviation -> ROLE.roleRating function.
    let private roleMap =
        dict [
            "TARGET MAN (ATTACK)", ROLE.roleRatingTargetManAttack; "TMA", ROLE.roleRatingTargetManAttack
            "ADVANCED FORWARD (ATTACK)", ROLE.roleRatingAdvancedForwardAttack; "AFA", ROLE.roleRatingAdvancedForwardAttack
            "WINGER (ATTACK) R", ROLE.roleRatingWingerAttackRight; "WAR", ROLE.roleRatingWingerAttackRight
            "INVERTED WINGER (L)", ROLE.roleRatingInvertedWingerSupportLeft; "IWL", ROLE.roleRatingInvertedWingerSupportLeft
            "ADVANCED PLAYMAKER (SUPPORT)", ROLE.roleRatingAdvancedPlaymakerSupport; "AP", ROLE.roleRatingAdvancedPlaymakerSupport
            "BALL WINNING MIDFIELDER (SUPPORT)", ROLE.roleRatingBallWinningMidfielderSupport; "BWM", ROLE.roleRatingBallWinningMidfielderSupport
            "INVERTED WING BACK (R)", ROLE.roleRatingInvertedWingBackSupportRight; "IWBR", ROLE.roleRatingInvertedWingBackSupportRight
            "INVERTED WING BACK (L)", ROLE.roleRatingInvertedWingBackSupportLeft; "IWBL", ROLE.roleRatingInvertedWingBackSupportLeft
            "SWEEPER KEEPER", ROLE.roleRatingSweeperKeeperDefend; "SKD", ROLE.roleRatingSweeperKeeperDefend
        ]

    let private roleRatingFnForRoleName (roleName: string) =
        let rn = norm roleName
        // BPD prefix handled separately, otherwise try lookup, fallback to none-returning function
        if rn.StartsWith("BPD") || rn.StartsWith("BALL PLAYING DEFENDER") then ROLE.roleRatingBallPlayingDefender
        else
            match roleMap.TryGetValue(rn) with
            | true, fn -> fn
            | _ -> fun (_: HTML.Player) -> None

    /// Parse money strings like "£185K", "€1.2M", "Free Transfer" into integer GBP (whole units).
    let private parseMoney (s: string) : int64 =
        if isNull s then 0L else
        let t = s.Trim().ToUpperInvariant()
        if t = "" || t.Contains("FREE") || t.Contains("NOT SET") then 0L
        else
            let cleaned = Regex.Replace(t, "[\u00a3\u20ac$\s]", "")
            let multiplier =
                if cleaned.EndsWith("M") then 1_000_000L
                elif cleaned.EndsWith("K") then 1_000L
                else 1L
            let m = Regex.Match(cleaned, @"([\d.,]+)")
            if not m.Success then 0L
            else
                let num = m.Groups.[1].Value.Replace(",", ".")
                match Double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture) with
                | true, v -> int64 (v * float multiplier)
                | _ -> 0L

    let private playerMarketValue (p: HTML.Player) : int64 =
        Map.tryFind "Value" p.Extras |> Option.map parseMoney |> Option.defaultValue 0L

    /// Return RoleRatedPlayer list for players whose computed role rating is > threshold.
    let getSctPlayersForRoleAbove (roleName: string) (threshold: float) : RoleRatedPlayer list =
        let ratingFn = roleRatingFnForRoleName roleName
        HTML.SctPlayers
        |> List.choose (fun p ->
            match ratingFn p with
            | Some r when r > threshold -> Some { Name = p.Name; RoleName = roleName; Rating = r; Player = p }
            | _ -> None)
        |> List.sortByDescending (fun rr -> rr.Rating)

    /// Convert a single `RoleRatedPlayer` to a `(Name, Rating)` tuple.
    let roleRatedPlayerToNameRating (rr: RoleRatedPlayer) : (string * float) =
        (rr.Name, rr.Rating)

    /// Filter helper: true when market value <= provided amount (in thousands).
    let roleRatedPlayerValueBelowK (maxValueK: int) (rr: RoleRatedPlayer) : bool =
        let maxValueGbp = int64 maxValueK * 1000L
        (playerMarketValue rr.Player) <= maxValueGbp

    /// Filter helper: true when player's loan status indicates they are loan listed (or on loan).
    let roleRatedPlayerLoanListed (rr: RoleRatedPlayer) : bool =
        let ls = Map.tryFind "LoanStatus" rr.Player.Extras |> Option.defaultValue ""
        let n = norm ls
        // Treat any non-empty value containing LOAN or LIST as loan-listed/on-loan
        n <> "" && (n.Contains("LOAN") || n.Contains("LIST"))

    /// Filter helper: true when player's transfer status indicates they are transfer listed (or available for transfer).
    let roleRatedPlayerTransferListed (rr: RoleRatedPlayer) : bool =
        let ts = Map.tryFind "TransferStatus" rr.Player.Extras |> Option.defaultValue ""
        let n = norm ts
        // Treat any non-empty value containing TRANSFER or LIST as transfer-listed/available
        n <> "" && (n.Contains("TRANSFER") || n.Contains("LIST"))

    /// Try to parse a player's DoB into a DateTime. Tries common formats, falls back to extracting a 4-digit year.
    let private tryParseDoB (s: string) : DateTime option =
        if isNull s then None else
        let t = s.Trim()
        if t = "" then None else
        let mutable dt = DateTime.MinValue
        // Try general parse with invariant culture
        if DateTime.TryParse(t, CultureInfo.InvariantCulture, DateTimeStyles.None, &dt) then Some dt
        else
            // Try common explicit formats
            let formats = [| "dd/MM/yyyy"; "d/M/yyyy"; "dd-MM-yyyy"; "yyyy-MM-dd"; "d MMM yyyy"; "dd MMM yyyy" |]
            if DateTime.TryParseExact(t, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, &dt) then Some dt
            else
                // Fallback: extract a 4-digit year (1990, 2001, etc.) and produce an approximate date (Jan 1 of that year)
                let m = Regex.Match(t, @"\b(19|20)\d{2}\b")
                if m.Success then
                    match Int32.TryParse(m.Value) with
                    | true, y -> Some (DateTime(y, 1, 1))
                    | _ -> None
                else None

    /// Compute player's age in years (approximate when only year is available).
    let private playerAge (p: HTML.Player) : int option =
        match tryParseDoB p.DoB with
        | Some dob ->
            // Use fixed reference date 31 August 2020 instead of DateTime.Today
            let today = DateTime(2020, 8, 31)
            let years = today.Year - dob.Year
            let hadBirthdayThisYear = (dob.Month < today.Month) || (dob.Month = today.Month && dob.Day <= today.Day)
            let age = if hadBirthdayThisYear then years else years - 1
            Some age
        | None -> None

    /// Filter helper: true when player's age is strictly below the provided `maxAge`.
    let roleRatedPlayerAgeBelow (maxAge: int) (rr: RoleRatedPlayer) : bool =
        match playerAge rr.Player with
        | Some age -> age < maxAge
        | None -> false

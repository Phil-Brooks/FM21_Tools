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

namespace FM21_ToolsLib

open System

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

    /// Return (Name, Rating, Player) for all players in HTML.SctPlayers whose computed role rating is > threshold.
    let getSctPlayersForRoleAbove (roleName: string) (threshold: float) : (string * float * HTML.Player) list =
        let ratingFn = roleRatingFnForRoleName roleName
        HTML.SctPlayers
        |> List.choose (fun p ->
            match ratingFn p with
            | Some r when r > threshold -> Some (p.Name, r, p)
            | _ -> None)
        |> List.sortByDescending (fun (_, r, _) -> r)

    /// Convenience: return only (Name, Rating) pairs from SctPlayers above threshold for the given role.
    let getSctPlayerNamesForRoleAbove (roleName: string) (threshold: float) : (string * float) list =
        getSctPlayersForRoleAbove roleName threshold |> List.map (fun (n, r, _) -> (n, r))

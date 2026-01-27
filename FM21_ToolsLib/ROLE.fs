namespace FM21_ToolsLib

open System

module ROLE =

    // small helpers to reduce repetition
    let private toFloatOpt = Option.map float

    // Helpers adapted to current Player shape (Extras: Map<string,string>, Attributes: Map<string,int>)
    let private getExtra (p: HTML.Player) (key: string) : string option =
        Map.tryFind key p.Extras
        |> Option.filter (fun s -> not (String.IsNullOrWhiteSpace s))

    let private getAttr (p: HTML.Player) (key: string) : int option =
        Map.tryFind key p.Attributes

    let private weightedScore (weightedAttrs: (float * float option) list) : float option =
        let totalWeight, weightedSum =
            weightedAttrs
            |> List.fold (fun (tw, ws) (w, vOpt) ->
                match vOpt with
                | Some v -> (tw + w, ws + w * v)
                | None -> (tw, ws)) (0.0, 0.0)

        if totalWeight = 0.0 then None else Some (5.0 * weightedSum / totalWeight)

    let private posMatches (p: HTML.Player) (predicate: string -> bool) =
        getExtra p "Position" |> Option.exists (fun s -> predicate (s.ToUpperInvariant()))

    let private bestBy (rating: HTML.Player -> float option) (players: HTML.Player list) (topN: int) : (string * float) list =
        let sorted =
            players
            |> List.choose (fun p -> rating p |> Option.map (fun s -> (p.Name, s)))
            |> List.sortByDescending snd

        if topN <= 0 then sorted else List.truncate topN sorted

    // --- lists of relevant attributes for each role (exposed for diagnostics) ---
    let private attrsTargetManAttack = [ "Dri"; "Fin"; "Fir"; "Hea"; "Pas"; "Tec"; "Ant"; "Cmp"; "Acc"; "Agi"; "Jum"; "Pac"; "Str" ]
    let private attrsAdvancedForwardAttack = [ "Pac"; "Acc"; "Fin"; "Dri"; "Fir"; "OtB"; "Tec"; "Ant"; "Cmp"; "Agi"; "Bal"; "Sta"; "Pas" ]
    let private attrsWingerAttackRight = [ "Cro"; "Pac"; "Acc"; "Dri"; "Tec"; "Pas"; "OtB"; "Agi"; "Fla"; "Sta"; "Fin" ]
    let private attrsInvertedWingerSupportLeft = [ "Cro"; "Pas"; "Tec"; "OtB"; "Dri"; "Fla"; "Cmp"; "Ant"; "Acc"; "Pac"; "Agi"; "Sta"; "Fin" ]
    let private attrsAdvancedPlaymakerSupport = [ "Pas"; "Tec"; "OtB"; "Ant"; "Cmp"; "Fir"; "Dri"; "Fla"; "Acc"; "Pac"; "Sta" ]
    let private attrsBallWinningMidfielderSupport = [ "Tck"; "Mar"; "Agg"; "Sta"; "Wor"; "Str"; "Ant"; "Dec"; "Cmp"; "Pas"; "Pac"; "Acc"; "Tec" ]
    let private attrsBallPlayingDefender = [ "Pas"; "Tec"; "Cmp"; "Dec"; "Ant"; "Tck"; "Mar"; "Str"; "Hea"; "Jum"; "Pac"; "Acc"; "Sta"; "Agg" ]
    let private attrsInvertedWingBackSupportRight = [ "Pas"; "Tec"; "OtB"; "Cro"; "Dri"; "Pac"; "Acc"; "Sta"; "Wor"; "Cmp"; "Dec"; "Tck"; "Mar"; "Ant"; "Agi"; "Str" ]
    let private attrsInvertedWingBackSupportLeft = attrsInvertedWingBackSupportRight
    let private attrsSweeperKeeperDefend = [ "Ref"; "Han"; "Pos"; "Kic"; "Cmd"; "Thr"; "OneVOne"; "Pun"; "Com"; "Ecc"; "Aer"; "Acc"; "Pac" ]

    /// Return the list of relevant attribute keys for a TEAM position role name.
    /// Matches the RoleName strings used in TEAM.Position.RoleName (handles "Ball Playing Defender #n").
    let getRelevantAttributesForRole (roleName: string) : string list =
        if roleName.StartsWith("BPD", StringComparison.InvariantCultureIgnoreCase) then
            attrsBallPlayingDefender
        else
            match roleName with
            | "TMA" -> attrsTargetManAttack
            | "AFA" -> attrsAdvancedForwardAttack
            | "WAR" -> attrsWingerAttackRight
            | "IWL" -> attrsInvertedWingerSupportLeft
            | "AP" -> attrsAdvancedPlaymakerSupport
            | "BWM" -> attrsBallWinningMidfielderSupport
            | "IWBR" -> attrsInvertedWingBackSupportRight
            | "IWBL" -> attrsInvertedWingBackSupportLeft
            | "SKD" -> attrsSweeperKeeperDefend
            // default: no relevant attributes known
            | _ -> []

    // General role rating builder to remove repetitive code
    let private mkRoleRating (positionPredicate: string -> bool) (weightedAttrKeys: (float * string) list) : (HTML.Player -> float option) =
        fun (p: HTML.Player) ->
            if not (posMatches p positionPredicate) then None
            else
                weightedAttrKeys
                |> List.map (fun (w, key) -> (w, toFloatOpt (getAttr p key)))
                |> weightedScore

    // role-specific ratings (use mkRoleRating to keep definitions concise)

    // Target Man (Attack)
    let roleRatingTargetManAttack =
        mkRoleRating
            (fun up -> up.Contains("ST") || up.Contains("F C"))
            [
                (0.40, "Dri"); (0.60, "Fin"); (0.60, "Fir"); (0.60, "Hea"); (0.20, "Pas");
                (0.40, "Tec"); (0.40, "Ant"); (0.60, "Cmp"); (1.00, "Acc"); (0.40, "Agi");
                (0.60, "Jum"); (1.00, "Pac"); (0.60, "Str")
            ]

    let bestTargetMenAttack = bestBy roleRatingTargetManAttack
    let bestTargetMenAttackNames players topN = bestTargetMenAttack players topN |> List.map fst

    // Advanced Forward (Attack)
    let roleRatingAdvancedForwardAttack =
        mkRoleRating
            (fun up -> up.Contains("ST") || up.Contains("F C"))
            [
                (1.00, "Pac"); (1.00, "Acc"); (1.00, "Fin"); (0.80, "Dri"); (0.60, "Fir");
                (0.60, "OtB"); (0.60, "Tec"); (0.60, "Ant"); (0.60, "Cmp"); (0.40, "Agi");
                (0.40, "Bal"); (0.20, "Sta"); (0.20, "Pas")
            ]

    let bestAdvancedForwardsAttack = bestBy roleRatingAdvancedForwardAttack
    let bestAdvancedForwardsAttackNames players topN = bestAdvancedForwardsAttack players topN |> List.map fst

    // Winger (Attack) Right
    let roleRatingWingerAttackRight =
        mkRoleRating
            (fun up -> up.Contains("M") && up.Contains("R"))
            [
                (1.20, "Cro"); (1.00, "Pac"); (1.00, "Acc"); (0.80, "Dri"); (0.60, "Tec");
                (0.60, "Pas"); (0.60, "OtB"); (0.40, "Agi"); (0.40, "Fla"); (0.20, "Sta"); (0.20, "Fin")
            ]

    let bestWingersAttackRight = bestBy roleRatingWingerAttackRight
    let bestWingersAttackRightNames players topN = bestWingersAttackRight players topN |> List.map fst

    // Inverted Winger (Support) Left
    let roleRatingInvertedWingerSupportLeft =
        mkRoleRating
            (fun up -> up.Contains("M") && up.Contains("L"))
            [
                (0.40, "Cro"); (0.90, "Pas"); (0.80, "Tec"); (0.80, "OtB"); (0.80, "Dri");
                (0.60, "Fla"); (0.60, "Cmp"); (0.50, "Ant"); (0.60, "Acc"); (0.60, "Pac");
                (0.40, "Agi"); (0.20, "Sta"); (0.20, "Fin")
            ]

    let bestInvertedWingersSupportLeft = bestBy roleRatingInvertedWingerSupportLeft
    let bestInvertedWingersSupportLeftNames players topN = bestInvertedWingersSupportLeft players topN |> List.map fst

    // Advanced Playmaker (Support) MC
    let roleRatingAdvancedPlaymakerSupport =
        mkRoleRating
            (fun up -> up.Contains("M") && up.Contains("C"))
            [
                (1.20, "Pas"); (0.90, "Tec"); (0.90, "OtB"); (0.80, "Ant"); (0.80, "Cmp");
                (0.60, "Fir"); (0.60, "Dri"); (0.50, "Fla"); (0.40, "Acc"); (0.40, "Pac"); (0.30, "Sta")
            ]

    let bestAdvancedPlaymakersSupport = bestBy roleRatingAdvancedPlaymakerSupport
    let bestAdvancedPlaymakersSupportNames players topN = bestAdvancedPlaymakersSupport players topN |> List.map fst

    // Ball Winning Midfielder (Support) MC
    let roleRatingBallWinningMidfielderSupport =
        mkRoleRating
            (fun up -> up.Contains("M") && up.Contains("C"))
            [
                (1.20, "Tck"); (1.00, "Mar"); (0.80, "Agg"); (0.80, "Sta"); (0.70, "Wor");
                (0.70, "Str"); (0.60, "Ant"); (0.60, "Dec"); (0.50, "Cmp"); (0.40, "Pas");
                (0.30, "Pac"); (0.30, "Acc"); (0.20, "Tec")
            ]

    let bestBallWinningMidfieldersSupport = bestBy roleRatingBallWinningMidfielderSupport
    let bestBallWinningMidfieldersSupportNames players topN = bestBallWinningMidfieldersSupport players topN |> List.map fst

    // Ball Playing Defender (DC)
    let roleRatingBallPlayingDefender =
        mkRoleRating
            (fun up -> ((up.Contains("D") && up.Contains("C")) || up.Contains("CB")))
            [
                (1.20, "Pas"); (0.90, "Tec"); (0.90, "Cmp"); (0.80, "Dec"); (0.70, "Ant");
                (0.70, "Tck"); (0.60, "Mar"); (0.60, "Str"); (0.50, "Hea"); (0.40, "Jum");
                (0.40, "Pac"); (0.30, "Acc"); (0.30, "Sta"); (0.30, "Agg")
            ]

    let bestBallPlayingDefenders = bestBy roleRatingBallPlayingDefender
    let bestBallPlayingDefendersNames players topN = bestBallPlayingDefenders players topN |> List.map fst

    // Inverted Wing Back (Support) Right
    let roleRatingInvertedWingBackSupportRight =
        mkRoleRating
            (fun up -> ((up.Contains("D") && up.Contains("R")) || up.Contains("RB") || up.Contains("RWB")))
            [
                (1.00, "Pas"); (0.90, "Tec"); (0.80, "OtB"); (0.80, "Cro"); (0.80, "Dri");
                (0.70, "Pac"); (0.60, "Acc"); (0.60, "Sta"); (0.60, "Wor"); (0.60, "Cmp");
                (0.50, "Dec"); (0.50, "Tck"); (0.50, "Mar"); (0.40, "Ant"); (0.40, "Agi"); (0.30, "Str")
            ]

    let bestInvertedWingBacksSupportRight = bestBy roleRatingInvertedWingBackSupportRight
    let bestInvertedWingBacksSupportRightNames players topN = bestInvertedWingBacksSupportRight players topN |> List.map fst

    // Inverted Wing Back (Support) Left
    let roleRatingInvertedWingBackSupportLeft =
        mkRoleRating
            (fun up -> ((up.Contains("D") && up.Contains("L")) || up.Contains("LB") || up.Contains("LWB")))
            [
                (1.00, "Pas"); (0.90, "Tec"); (0.80, "OtB"); (0.80, "Cro"); (0.80, "Dri");
                (0.70, "Pac"); (0.60, "Acc"); (0.60, "Sta"); (0.60, "Wor"); (0.60, "Cmp");
                (0.50, "Dec"); (0.50, "Tck"); (0.50, "Mar"); (0.40, "Ant"); (0.40, "Agi"); (0.30, "Str")
            ]

    let bestInvertedWingBacksSupportLeft = bestBy roleRatingInvertedWingBackSupportLeft
    let bestInvertedWingBacksSupportLeftNames players topN = bestInvertedWingBacksSupportLeft players topN |> List.map fst

    // Sweeper Keeper (Defend)
    let roleRatingSweeperKeeperDefend =
        mkRoleRating
            (fun up -> up.Contains("GK") || up.Contains("G K") || up.Contains("GOAL"))
            [
                (1.20, "Ref"); (1.00, "Han"); (0.90, "Pos"); (0.80, "Kic"); (0.70, "Cmd");
                (0.60, "Thr"); (0.60, "OneVOne"); (0.50, "Pun"); (0.40, "Com"); (0.30, "Ecc");
                (0.30, "Aer"); (0.20, "Acc"); (0.20, "Pac")
            ]

    let bestSweeperKeepersDefend = bestBy roleRatingSweeperKeeperDefend
    let bestSweeperKeepersDefendNames players topN = bestSweeperKeepersDefend players topN |> List.map fst

    // --- New: compute all role ratings for a player and pick the best ---

    /// Mapping of role display names to rating functions
    let private allRoleRatings : (string * (HTML.Player -> float option)) list = [
        ("TMA", roleRatingTargetManAttack)
        ("AFA", roleRatingAdvancedForwardAttack)
        ("WAR", roleRatingWingerAttackRight)
        ("IWL", roleRatingInvertedWingerSupportLeft)
        ("AP", roleRatingAdvancedPlaymakerSupport)
        ("BWM", roleRatingBallWinningMidfielderSupport)
        // Ball Playing Defender may appear as "Ball Playing Defender" or "Ball Playing Defender #n" in teams;
        // keep base name here and callers can match with StartsWith if needed.
        ("BPD", roleRatingBallPlayingDefender)
        ("IWBR", roleRatingInvertedWingBackSupportRight)
        ("IWBL", roleRatingInvertedWingBackSupportLeft)
        ("SKD", roleRatingSweeperKeeperDefend)
    ]

    /// Return a sorted list of (roleName, rating) for roles that apply to the player (descending by rating).
    let roleRatingsForPlayer (p: HTML.Player) : (string * float) list =
        allRoleRatings
        |> List.choose (fun (roleName, rf) ->
            rf p |> Option.map (fun r -> (roleName, r)))
        |> List.sortByDescending snd

    /// Return the single best role for a player as (roleName, rating) option.
    let bestRoleForPlayer (p: HTML.Player) : (string * float) option =
        roleRatingsForPlayer p |> List.tryHead

    /// Convenience: return TYPES.RoleRatedPlayer option for the best role.
    let bestRoleRatedPlayer (p: HTML.Player) : TYPES.RoleRatedPlayer option =
        match bestRoleForPlayer p with
        | Some (role, rating) -> Some { Name = p.Name; RoleName = role; Rating = rating; Player = p }
        | None -> None

    /// For an optional RoleRatedPlayer with an assigned Player, return the weakest relevant attribute (roleAbbrev, player, attr, value).
    let weakestRelevantAttributeForPosition (roleAbbrev: string, posOpt: TYPES.RoleRatedPlayer option) : (string * string * string * int) option =
        posOpt
        |> Option.bind (fun r ->
            // r.Player is a concrete HTML.Player (TYPES.RoleRatedPlayer.Player is non-optional)
            match getRelevantAttributesForRole r.RoleName with
            | [] -> None
            | relevant ->
                relevant
                |> List.map (fun key -> key, (Map.tryFind key r.Player.Attributes |> Option.defaultValue 0))
                |> List.minBy snd
                |> fun (attr, value) -> Some (roleAbbrev, r.Player.Name, attr, value)
        )

    let weakestRelevantAttributeForPlayer (p: TYPES.RoleRatedPlayer) = weakestRelevantAttributeForPosition (p.RoleName, Some p)

    /// For an optional RoleRatedPlayer with an assigned Player, return the second weakest relevant attribute (roleAbbrev, player, attr, value).
    let secondWeakestRelevantAttributeForPosition (roleAbbrev: string, posOpt: TYPES.RoleRatedPlayer option) : (string * string * string * int) option =
        posOpt
        |> Option.bind (fun r ->
            match getRelevantAttributesForRole r.RoleName with
            | [] | [_] -> None
            | relevant ->
                relevant
                |> List.map (fun key -> key, (Map.tryFind key r.Player.Attributes |> Option.defaultValue 0))
                |> List.sortBy snd
                |> fun sorted ->
                    match sorted with
                    | _ :: (second :: _) -> Some (roleAbbrev, r.Player.Name, fst second, snd second)
                    | _ -> None
        )

    let secondWeakestRelevantAttributeForPlayer (p: TYPES.RoleRatedPlayer) = secondWeakestRelevantAttributeForPosition (p.RoleName, Some p)
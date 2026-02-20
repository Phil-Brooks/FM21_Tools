namespace FM21_ToolsLib

open System

module ROLE =

    // small helpers to reduce repetition
    let private toFloatOpt = Option.map float

    // Helpers adapted to current Player shape (Extras: Map<string,string>, Attributes: Map<string,int>)
    let private getExtra (p: Player) (key: string) : string option =
        Map.tryFind key p.Extras
        |> Option.filter (fun s -> not (String.IsNullOrWhiteSpace s))

    let private getAttr (p: Player) (key: string) : int option =
        Map.tryFind key p.Attributes

    let private weightedScore (weightedAttrs: (float * float option) list) : float option =
        let totalWeight, weightedSum =
            weightedAttrs
            |> List.fold (fun (tw, ws) (w, vOpt) ->
                match vOpt with
                | Some v -> (tw + w, ws + w * v)
                | None -> (tw, ws)) (0.0, 0.0)

        if totalWeight = 0.0 then None else Some (5.0 * weightedSum / totalWeight)

    let private posMatches (p: Player) (predicate: string -> bool) =
        getExtra p "Position" |> Option.exists (fun s -> predicate (s.ToUpperInvariant()))

    let private bestBy (rating: Player -> float option) (players: Player list) (topN: int) : (string * float) list =
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
    // Shadow Striker (Attack) — an attacking midfield role that looks to get into goalscoring positions
    let private attrsShadowStrikerAttack = [ "Fin"; "Dri"; "OtB"; "Ant"; "Cmp"; "Pas"; "Acc"; "Pac"; "Agi"; "Fir" ]
    let private attrsDefensiveWingerSupportLeft = [ "Pac"; "Acc"; "Tck"; "Mar"; "Cro"; "Pas"; "Tec"; "Sta"; "OtB"; "Tea"; "Wor"; "Cmp" ]
    let private attrsDefensiveWingerSupportRight = attrsDefensiveWingerSupportLeft
    let private attrsCentralMidfielderAttack = [ "Pas"; "OtB"; "Fir"; "Tec"; "Dec"; "Cmp"; "Acc"; "Pac"; "Sta"; "Ant" ]
    let private attrsBallWinningMidfielderSupport = [ "Tck"; "Mar"; "Agg"; "Sta"; "Wor"; "Str"; "Ant"; "Dec"; "Cmp"; "Pas"; "Pac"; "Acc"; "Tec" ]
    let private attrsDeepLyingPlaymakerSupport = [ "Pas"; "Cmp"; "Tec"; "Dec"; "Fir"; "Tea"; "Vis"; "Pac" ]
    let private attrsBallPlayingDefender = [ "Pas"; "Tec"; "Cmp"; "Dec"; "Ant"; "Tck"; "Mar"; "Str"; "Hea"; "Jum"; "Pac"; "Acc"; "Sta"; "Agg" ]
    let private attrsInvertedWingBackSupportRight = [ "Pas"; "Tec"; "OtB"; "Cro"; "Dri"; "Pac"; "Acc"; "Sta"; "Wor"; "Cmp"; "Dec"; "Tck"; "Mar"; "Ant"; "Agi"; "Str" ]
    let private attrsInvertedWingBackSupportLeft = attrsInvertedWingBackSupportRight
    let private attrsSweeperKeeperDefend = [ "Ref"; "Han"; "Pos"; "Kic"; "Cmd"; "Thr"; "OneVOne"; "Pun"; "Com"; "Ecc"; "Aer"; "Acc"; "Pac" ]
    let private attrsSweeperKeeperSupport = [ "Ref"; "Han"; "Pos"; "Kic"; "Cmd"; "Thr"; "OneVOne"; "Pun"; "Com"; "Ecc"; "Aer"; "Acc"; "Pac"; "TRO"; "Cmp" ]

    /// Return the list of relevant attribute keys for a TEAM position role name.
    /// Matches the RoleName strings used in TEAM.Position.RoleName (handles "Ball Playing Defender #n").
    let getRelevantAttributesForRole (roleName: string) : string list =
        if roleName.StartsWith("BPD", StringComparison.InvariantCultureIgnoreCase) then
            attrsBallPlayingDefender
        elif roleName.StartsWith("AMSS", StringComparison.InvariantCultureIgnoreCase) then
            attrsShadowStrikerAttack
        else
            match roleName with
            | "TMA" -> attrsTargetManAttack
            | "AFA" -> attrsAdvancedForwardAttack
            | "WAR" -> attrsWingerAttackRight
            | "IWL" -> attrsInvertedWingerSupportLeft
            | "ML" -> attrsDefensiveWingerSupportLeft
            | "MR" -> attrsDefensiveWingerSupportRight
            | "AP" -> attrsAdvancedPlaymakerSupport
            | "MCA" -> attrsCentralMidfielderAttack
            | "DLP" -> attrsDeepLyingPlaymakerSupport
            | "BWM" -> attrsBallWinningMidfielderSupport
            | "IWBR" -> attrsInvertedWingBackSupportRight
            | "IWBL" -> attrsInvertedWingBackSupportLeft
            | "SKD" -> attrsSweeperKeeperDefend
            | "SKS" -> attrsSweeperKeeperSupport
            // default: no relevant attributes known
            | _ -> []

    // General role rating builder to remove repetitive code
    let private mkRoleRating (positionPredicate: string -> bool) (weightedAttrKeys: (float * string) list) : (Player -> float option) =
        fun (p: Player) ->
            if not (posMatches p positionPredicate) then None
            else
                weightedAttrKeys
                |> List.map (fun (w, key) -> (w, toFloatOpt (getAttr p key)))
                |> weightedScore

    // role-specific ratings (use mkRoleRating to keep definitions concise)
      //["ST (C)"; "M/AM (R)"; "AM (RL)"; "D/WB/M/AM (L)"; "M (RL)"; "AM (RLC)";
      // "D/WB (R)"; "M/AM (RL)"; "D (C)"; "D/WB/M (R)"; "M (L)"; "WB/M/AM (R)";
      // "AM (R)"; "M (R)"; "M (LC)"; "AM (C)"; "D/WB/AM (R)"; "D/WB (L)"; "AM (L)";
      // "WB/M/AM (L)"; "D (RLC)"; "WB (R)"; "DM"; "M (C)"; "D/WB/M (L)";
      // "D/WB/M/AM (R)"; "D (LC)"; "M/AM (L)"; "D/WB (RL)"; "WB (L)"; "M/AM (C)";
      // "D (RC)"; "WB/M (L)"; "D/WB/M (RL)"; "AM (LC)"; "M/AM (LC)"; "D (R)";
      // "WB (RL)"; "M/AM (RLC)"; "D (L)"; "AM (RC)"; "D (RL)"; "M (RLC)";
      // "WB/M (R)"; "M/AM (RC)"; "D/WB/AM (L)"; "WB/AM (L)"; "M (RC)";
      // "WB/M/AM (RL)"; "D/M (R)"; "GK"; "D/AM (L)"; "D/M/AM (L)"; "D/M/AM (R)";
      // "D/AM (R)"; "D/M (L)"; "WB/M (RL)"; "WB/AM (R)"; "D/M (C)"; "D/M/AM (C)";
      // "D/WB/M/AM (RL)"; "D/AM (C)"; "D/M (RC)"]

    // Target Man (Attack)
    let roleRatingTargetManAttack =
        mkRoleRating
            (fun up -> up.Contains("ST (C)"))
            [
                (0.40, "Dri"); (0.40, "Fin"); (0.60, "Fir"); (0.60, "Hea"); (0.20, "Pas");
                (0.40, "Tec"); (0.40, "Ant"); (0.60, "Cmp"); (1.00, "Acc"); (0.40, "Agi");
                (0.60, "Jum"); (1.00, "Pac"); (0.60, "Str")
            ]

    let bestTargetMenAttack = bestBy roleRatingTargetManAttack
    let bestTargetMenAttackNames players topN = bestTargetMenAttack players topN |> List.map fst

    // Advanced Forward (Attack)
    let roleRatingAdvancedForwardAttack =
        mkRoleRating
            (fun up -> up.Contains("ST (C)"))
            [
                (1.00, "Pac"); (1.00, "Acc"); (0.50, "Fin"); (0.80, "Dri"); (0.60, "Fir");
                (0.20, "OtB"); (0.60, "Tec"); (0.60, "Ant"); (0.60, "Cmp"); (0.40, "Agi");
                (0.20, "Bal"); (0.20, "Sta"); (0.40, "Pas")
            ]

    let bestAdvancedForwardsAttack = bestBy roleRatingAdvancedForwardAttack
    let bestAdvancedForwardsAttackNames players topN = bestAdvancedForwardsAttack players topN |> List.map fst

    // Winger (Attack) Right
    let wars =  ["M/AM (R)"; "M (RL)"; "M/AM (RL)"; "D/WB/M (R)";
       "WB/M/AM (R)"; "M (R)"; "D/WB/M/AM (R)";
       "D/WB/M (RL)"; "M/AM (RLC)"; "M (RLC)"; "WB/M (R)"; "M/AM (RC)";
       "M (RC)"; "WB/M/AM (RL)"; "D/M (R)"; "D/M/AM (R)"; "WB/M (RL)";
       "D/WB/M/AM (RL)"; "D/M (RC)"]

    let roleRatingWingerAttackRight =
        mkRoleRating
            (fun up -> wars |> List.exists (fun w -> up.Contains(w.ToUpperInvariant())))
            [
                (1.20, "Cro"); (1.00, "Pac"); (1.00, "Acc"); (0.80, "Dri"); (0.60, "Tec");
                (0.60, "Pas"); (0.30, "OtB"); (0.40, "Agi"); (0.40, "Fla"); (0.20, "Sta"); (0.10, "Fin")
            ]

    let bestWingersAttackRight = bestBy roleRatingWingerAttackRight
    let bestWingersAttackRightNames players topN = bestWingersAttackRight players topN |> List.map fst

    // Inverted Winger (Support) Left
    let iwl =  ["D/WB/M/AM (L)"; "M (RL)"; "M/AM (RL)"; "M (L)";
           "M (LC)"; "WB/M/AM (L)"; "D/WB/M (L)"; "M/AM (L)"; "WB/M (L)";
           "D/WB/M (RL)"; "M/AM (LC)"; "M/AM (RLC)"; "M (RLC)";
           "WB/M/AM (RL)"; "D/M/AM (L)";
           "D/M (L)"; "WB/M (RL)"; "D/WB/M/AM (RL)"]

    
    let roleRatingInvertedWingerSupportLeft =
        mkRoleRating
            (fun up -> iwl |> List.exists (fun w -> up.Contains(w.ToUpperInvariant())))
            [
                (0.40, "Cro"); (0.90, "Pas"); (0.80, "Tec"); (0.30, "OtB"); (0.80, "Dri");
                (0.30, "Fla"); (0.60, "Cmp"); (0.50, "Ant"); (0.60, "Acc"); (0.60, "Pac");
                (0.40, "Agi"); (0.20, "Sta"); (0.30, "Fin")
            ]

    let bestInvertedWingersSupportLeft = bestBy roleRatingInvertedWingerSupportLeft
    let bestInvertedWingersSupportLeftNames players topN = bestInvertedWingersSupportLeft players topN |> List.map fst

    // Advanced Playmaker (Support) MC
    let ap =   ["M (LC)"; "M (C)"; "M/AM (C)"; "M/AM (LC)";
       "M/AM (RLC)"; "M (RLC)"; "M/AM (RC)"; "M (RC)"; "D/M (C)";
       "D/M/AM (C)"; "D/M (RC)"]
    
    let roleRatingAdvancedPlaymakerSupport =
        mkRoleRating
            (fun up -> ap |> List.exists (fun w -> up.Contains(w.ToUpperInvariant())))
            [
                (1.20, "Pas"); (0.90, "Tec"); (0.40, "OtB"); (0.80, "Ant"); (0.80, "Cmp");
                (0.60, "Fir"); (0.60, "Dri"); (0.30, "Fla"); (0.60, "Acc"); (0.60, "Pac"); (0.30, "Sta")
            ]

    let bestAdvancedPlaymakersSupport = bestBy roleRatingAdvancedPlaymakerSupport
    let bestAdvancedPlaymakersSupportNames players topN = bestAdvancedPlaymakersSupport players topN |> List.map fst

    // Shadow Striker (Attack)
    let ssa = [ "AM (RLC)"; "AM (C)"; "M/AM (C)"; "AM (LC)"; "M/AM (LC)"; "M/AM (RLC)"; "AM (RC)";
                "M/AM (RC)"; "D/M/AM (C)"; "D/AM (C)"]
    
    let roleRatingShadowStrikerAttack =
        mkRoleRating
            (fun up -> ssa |> List.exists (fun w -> up.Contains(w.ToUpperInvariant())))
            [
                (1.00, "Fin"); (0.90, "Dri"); (0.80, "OtB"); (0.60, "Ant"); (0.50, "Cmp");
                (0.30, "Pas"); (0.50, "Acc"); (0.30, "Pac"); (0.20, "Agi"); (0.40, "Fir")
            ]

    let bestShadowStrikersAttack = bestBy roleRatingShadowStrikerAttack
    let bestShadowStrikersAttackNames players topN = bestShadowStrikersAttack players topN |> List.map fst

    // Ball Winning Midfielder (Support) MC
    let bwm =   ["M (LC)"; "M (C)"; "M/AM (C)"; "M/AM (LC)";
       "M/AM (RLC)"; "M (RLC)"; "M/AM (RC)"; "M (RC)"; "D/M (C)";
       "D/M/AM (C)"; "D/M (RC)"]
    
    let roleRatingBallWinningMidfielderSupport =
        mkRoleRating
            (fun up -> bwm |> List.exists (fun w -> up.Contains(w.ToUpperInvariant())))
            [
                (1.20, "Tck"); (1.00, "Mar"); (0.30, "Agg"); (0.80, "Sta"); (0.70, "Wor");
                (0.70, "Str"); (0.60, "Ant"); (0.60, "Dec"); (0.10, "Cmp"); (0.40, "Pas");
                (0.30, "Pac"); (0.30, "Acc"); (0.20, "Tec")
            ]

    let bestBallWinningMidfieldersSupport = bestBy roleRatingBallWinningMidfielderSupport
    let bestBallWinningMidfieldersSupportNames players topN = bestBallWinningMidfieldersSupport players topN |> List.map fst

    // Central Midfielder (Attack) — M (C) variants
    let mca = ["M (LC)"; "M (C)"; "M/AM (C)"; "M/AM (LC)";
       "M/AM (RLC)"; "M (RLC)"; "M/AM (RC)"; "M (RC)"; "D/M (C)";
       "D/M/AM (C)"; "D/M (RC)"]

    let roleRatingCentralMidfielderAttack =
        mkRoleRating
            (fun up -> mca |> List.exists (fun w -> up.Contains(w.ToUpperInvariant())))
            [
                (1.20, "Pas"); (0.90, "OtB"); (0.80, "Fir"); (0.80, "Tec"); (0.80, "Dec");
                (0.60, "Cmp"); (0.60, "Acc"); (0.40, "Pac"); (0.60, "Sta"); (0.40, "Ant")
            ]

    let bestCentralMidfieldersAttack = bestBy roleRatingCentralMidfielderAttack
    let bestCentralMidfieldersAttackNames players topN = bestCentralMidfieldersAttack players topN |> List.map fst

    // Deep Lying Playmaker (Support) — typically a DM / deep central midfielder
    let dlp = [ "DM" ]

    let roleRatingDeepLyingPlaymakerSupport =
        mkRoleRating
            (fun up -> dlp |> List.exists (fun w -> up.Contains(w.ToUpperInvariant())))
            [
                (1.20, "Pas"); (0.90, "Cmp"); (0.80, "Tec"); (0.60, "Dec"); (0.60, "Fir");
                (0.40, "Tea"); (0.30, "Vis"); (0.30, "Pac")
            ]

    let bestDeepLyingPlaymakersSupport = bestBy roleRatingDeepLyingPlaymakerSupport
    let bestDeepLyingPlaymakersSupportNames players topN = bestDeepLyingPlaymakersSupport players topN |> List.map fst

    // Ball Playing Defender (DC)
    let bpd =
          ["D (C)"; "D (RLC)"; "D (LC)"; "D (RC)"; "D/M (C)"; "D/M/AM (C)"; "D/AM (C)";"D/M (RC)"]

    let roleRatingBallPlayingDefender =
        mkRoleRating
            (fun up -> bpd |> List.exists (fun w -> up.Contains(w.ToUpperInvariant())))
            [
                (1.20, "Pas"); (0.50, "Tec"); (0.20, "Cmp"); (0.80, "Dec"); (0.70, "Ant");
                (0.70, "Tck"); (0.60, "Mar"); (0.60, "Str"); (0.70, "Hea"); (0.80, "Jum");
                (0.40, "Pac"); (0.30, "Acc"); (0.30, "Sta"); (0.10, "Agg")
            ]

    let bestBallPlayingDefenders = bestBy roleRatingBallPlayingDefender
    let bestBallPlayingDefendersNames players topN = bestBallPlayingDefenders players topN |> List.map fst

    // Inverted Wing Back (Support) Right
    let iwbr =   ["D/WB (R)"; "D/WB/M (R)"; "D/WB/AM (R)"; "D (RLC)"; "D/WB/M/AM (R)";
       "D/WB (RL)"; "D (RC)"; "D/WB/M (RL)"; "D (R)"; "D (RL)"; "D/M (R)";
       "D/M/AM (R)"; "D/AM (R)"; "D/WB/M/AM (RL)"; "D/M (RC)"]

    let roleRatingInvertedWingBackSupportRight =
        mkRoleRating
            (fun up -> iwbr |> List.exists (fun w -> up.Contains(w.ToUpperInvariant())))
            [
                (1.00, "Pas"); (0.90, "Tec"); (0.10, "OtB"); (0.80, "Cro"); (0.10, "Dri");
                (0.70, "Pac"); (0.60, "Acc"); (0.60, "Sta"); (0.30, "Wor"); (0.10, "Cmp");
                (0.50, "Dec"); (0.50, "Tck"); (0.50, "Mar"); (0.40, "Ant"); (0.40, "Agi"); (0.30, "Str")
            ]

    let bestInvertedWingBacksSupportRight = bestBy roleRatingInvertedWingBackSupportRight
    let bestInvertedWingBacksSupportRightNames players topN = bestInvertedWingBacksSupportRight players topN |> List.map fst

    // Inverted Wing Back (Support) Left
    let iwbl =   ["D/WB/M/AM (L)"; "D/WB (L)"; "D (RLC)"; "D/WB/M (L)"; "D (LC)"; "D/WB (RL)";
       "D/WB/M (RL)"; "D (L)"; "D (RL)"; "D/WB/AM (L)"; "D/AM (L)"; "D/M/AM (L)";
       "D/M (L)"; "D/WB/M/AM (RL)"]

    let roleRatingInvertedWingBackSupportLeft =
        mkRoleRating
            (fun up -> iwbl |> List.exists (fun w -> up.Contains(w.ToUpperInvariant())))
            [
                (1.00, "Pas"); (0.90, "Tec"); (0.10, "OtB"); (0.80, "Cro"); (0.10, "Dri");
                (0.70, "Pac"); (0.60, "Acc"); (0.60, "Sta"); (0.30, "Wor"); (0.10, "Cmp");
                (0.50, "Dec"); (0.50, "Tck"); (0.50, "Mar"); (0.40, "Ant"); (0.40, "Agi"); (0.30, "Str")
            ]

    let bestInvertedWingBacksSupportLeft = bestBy roleRatingInvertedWingBackSupportLeft
    let bestInvertedWingBacksSupportLeftNames players topN = bestInvertedWingBacksSupportLeft players topN |> List.map fst

    // Sweeper Keeper (Defend)
    let roleRatingSweeperKeeperDefend =
        mkRoleRating
            (fun up -> up.Contains("GK"))
            [
                (1.20, "Ref"); (1.00, "Han"); (0.90, "Pos"); (0.80, "Kic"); (0.70, "Cmd");
                (0.10, "Thr"); (0.60, "OneVOne"); (0.10, "Pun"); (0.40, "Com"); (0.30, "Ecc");
                (0.30, "Aer"); (0.20, "Acc"); (0.20, "Pac")
            ]

    let bestSweeperKeepersDefend = bestBy roleRatingSweeperKeeperDefend
    let bestSweeperKeepersDefendNames players topN = bestSweeperKeepersDefend players topN |> List.map fst

    // Sweeper Keeper (Support)
    let roleRatingSweeperKeeperSupport =
        mkRoleRating
            (fun up -> up.Contains("GK"))
            [
                (1.20, "Ref"); (1.00, "Han"); (0.90, "Pos"); (0.80, "Kic"); (0.70, "Cmd");
                (0.10, "Thr"); (0.60, "OneVOne"); (0.10, "Pun"); (0.40, "Com"); (0.30, "Ecc");
                (0.30, "Aer"); (0.20, "Acc"); (0.20, "Pac"); (0.40, "TRO"); (0.40, "Cmp")
            ]

    let bestSweeperKeepersSupport = bestBy roleRatingSweeperKeeperSupport
    let bestSweeperKeepersSupportNames players topN = bestSweeperKeepersSupport players topN |> List.map fst

    // Defensive Winger (Support) Left — "ML"
    let ml =  ["D/WB/M/AM (L)"; "M (RL)"; "M/AM (RL)"; "M (L)";
           "M (LC)"; "WB/M/AM (L)"; "D/WB/M (L)"; "M/AM (L)"; "WB/M (L)";
           "D/WB/M (RL)"; "M/AM (LC)"; "M/AM (RLC)"; "M (RLC)";
           "WB/M/AM (RL)"; "D/M/AM (L)";
           "D/M (L)"; "WB/M (RL)"; "D/WB/M/AM (RL)"]

    let roleRatingDefensiveWingerSupportLeft =
        mkRoleRating
            (fun up -> ml |> List.exists (fun w -> up.Contains(w.ToUpperInvariant())))
            [
                (1.00, "Pac"); (0.90, "Acc"); (0.80, "Tck"); (0.70, "Mar"); (0.80, "Cro"); (0.70, "Pas"); (0.60, "Tec");
                (0.60, "Sta"); (0.40, "OtB"); (0.40, "Tea"); (0.20, "Wor"); (0.20, "Cmp")
            ]

    let bestDefensiveWingersSupportLeft = bestBy roleRatingDefensiveWingerSupportLeft
    let bestDefensiveWingersSupportLeftNames players topN = bestDefensiveWingersSupportLeft players topN |> List.map fst

    // Defensive Winger (Support) Right — "MR"
    let mr = [ "D/WB/M/AM (R)"; "M (RL)"; "M/AM (RL)"; "M (R)";
           "M (RC)"; "WB/M/AM (R)"; "D/WB/M (R)"; "M/AM (R)"; "WB/M (R)";
           "D/WB/M (RL)"; "M/AM (RC)"; "M/AM (RLC)"; "M (RLC)";
           "WB/M/AM (RL)"; "D/M/AM (R)";
           "D/M (R)"; "WB/M (RL)"; "D/WB/M/AM (RL)" ]

    let roleRatingDefensiveWingerSupportRight =
        mkRoleRating
            (fun up -> mr |> List.exists (fun w -> up.Contains(w.ToUpperInvariant())))
            [
                (1.00, "Pac"); (0.90, "Acc"); (0.80, "Tck"); (0.70, "Mar"); (0.80, "Cro"); (0.70, "Pas"); (0.60, "Tec");
                (0.60, "Sta"); (0.40, "OtB"); (0.40, "Tea"); (0.20, "Wor"); (0.20, "Cmp")
            ]

    let bestDefensiveWingersSupportRight = bestBy roleRatingDefensiveWingerSupportRight
    let bestDefensiveWingersSupportRightNames players topN = bestDefensiveWingersSupportRight players topN |> List.map fst

    // --- New: compute all role ratings for a player and pick the best ---

    // just ZAZ roles
    /// Mapping of role display names to rating functions
    let private ZAZRoleRatings : (string * (Player -> float option)) list = [
        ("ML", roleRatingDefensiveWingerSupportLeft)
        ("MR", roleRatingDefensiveWingerSupportRight)
        ("AMSS", roleRatingShadowStrikerAttack)
        ("MCA", roleRatingCentralMidfielderAttack)
        ("DLP", roleRatingDeepLyingPlaymakerSupport)
        ("BPD", roleRatingBallPlayingDefender)
        ("IWBR", roleRatingInvertedWingBackSupportRight)
        ("IWBL", roleRatingInvertedWingBackSupportLeft)
        ("SKS", roleRatingSweeperKeeperSupport)
    ]

    /// Return a sorted list of (roleName, rating) for roles that apply to the player (descending by rating).
    let ZAZroleRatingsForPlayer (p: Player) : (string * float) list =
        ZAZRoleRatings
        |> List.choose (fun (roleName, rf) ->
            rf p |> Option.map (fun r -> (roleName, r)))
        |> List.sortByDescending snd

    /// Mapping of role display names to rating functions
    let private allRoleRatings : (string * (Player -> float option)) list = [
        ("TMA", roleRatingTargetManAttack)
        ("AFA", roleRatingAdvancedForwardAttack)
        ("WAR", roleRatingWingerAttackRight)
        ("IWL", roleRatingInvertedWingerSupportLeft)
        ("ML", roleRatingDefensiveWingerSupportLeft)
        ("MR", roleRatingDefensiveWingerSupportRight)
        ("AP", roleRatingAdvancedPlaymakerSupport)
        ("AMSS", roleRatingShadowStrikerAttack)
        ("MCA", roleRatingCentralMidfielderAttack)
        ("DLP", roleRatingDeepLyingPlaymakerSupport)
        ("BWM", roleRatingBallWinningMidfielderSupport)
        // Ball Playing Defender may appear as "Ball Playing Defender" or "Ball Playing Defender #n" in teams;
        // keep base name here and callers can match with StartsWith if needed.
        ("BPD", roleRatingBallPlayingDefender)
        ("IWBR", roleRatingInvertedWingBackSupportRight)
        ("IWBL", roleRatingInvertedWingBackSupportLeft)
        ("SKD", roleRatingSweeperKeeperDefend)
        ("SKS", roleRatingSweeperKeeperSupport)
    ]

    /// Return a sorted list of (roleName, rating) for roles that apply to the player (descending by rating).
    let roleRatingsForPlayer (p: Player) : (string * float) list =
        allRoleRatings
        |> List.choose (fun (roleName, rf) ->
            rf p |> Option.map (fun r -> (roleName, r)))
        |> List.sortByDescending snd

    /// Return the single best role for a player as (roleName, rating) option.
    let bestRoleForPlayer (p: Player) : (string * float) option =
        roleRatingsForPlayer p |> List.tryHead

    /// Convenience: return RoleRatedPlayer option for the best role.
    let bestRoleRatedPlayer (p: Player) : RoleRatedPlayer option =
        match bestRoleForPlayer p with
        | Some (role, rating) -> Some { Name = p.Name; RoleName = role; Rating = rating; Player = p }
        | None -> None

    /// For an optional RoleRatedPlayer with an assigned Player, return the weakest relevant attribute (roleAbbrev, player, attr, value).
    let weakestRelevantAttributeForPosition (roleAbbrev: string, posOpt: RoleRatedPlayer option) : (string * string * string * int) option =
        posOpt
        |> Option.bind (fun r ->
            // r.Player is a concrete Player (RoleRatedPlayer.Player is non-optional)
            match getRelevantAttributesForRole r.RoleName with
            | [] -> None
            | relevant ->
                relevant
                |> List.map (fun key -> key, (Map.tryFind key r.Player.Attributes |> Option.defaultValue 0))
                |> List.minBy snd
                |> fun (attr, value) -> Some (roleAbbrev, r.Player.Name, attr, value)
        )

    let weakestRelevantAttributeForPlayer (p: RoleRatedPlayer) = weakestRelevantAttributeForPosition (p.RoleName, Some p)

    /// For an optional RoleRatedPlayer with an assigned Player, return the second weakest relevant attribute (roleAbbrev, player, attr, value).
    let secondWeakestRelevantAttributeForPosition (roleAbbrev: string, posOpt: RoleRatedPlayer option) : (string * string * string * int) option =
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

    let secondWeakestRelevantAttributeForPlayer (p: RoleRatedPlayer) = secondWeakestRelevantAttributeForPosition (p.RoleName, Some p)
namespace FM21_ToolsLib

open System

module ROLE =

    // small helpers to reduce repetition
    let private toFloatOpt = Option.map float

    let private weightedScore (weightedAttrs: (float * float option) list) : float option =
        let totalWeight, weightedSum =
            weightedAttrs
            |> List.fold (fun (tw, ws) (w, vOpt) ->
                match vOpt with
                | Some v -> (tw + w, ws + w * v)
                | None -> (tw, ws)) (0.0, 0.0)

        if totalWeight = 0.0 then None else Some (5.0 * weightedSum / totalWeight)

    let private posMatches (p: HTML.Player) (predicate: string -> bool) =
        p.Position |> Option.exists (fun s -> predicate (s.ToUpperInvariant()))

    let private bestBy (rating: HTML.Player -> float option) (players: HTML.Player list) (topN: int) : (string * float) list =
        let sorted =
            players
            |> List.choose (fun p -> rating p |> Option.map (fun s -> (p.Name, s)))
            |> List.sortByDescending snd

        if topN <= 0 then sorted else List.truncate topN sorted

    // Target Man (Attack)
    let roleRatingTargetManAttack (p: HTML.Player) : float option =
        let isForwardPosition =
            posMatches p (fun up -> up.Contains("ST") || up.Contains("F C"))

        if not isForwardPosition then None
        else
            let weightedAttrs : (float * float option) list = [
                (0.40, toFloatOpt p.Dri)
                (0.60, toFloatOpt p.Fin)
                (0.60, toFloatOpt p.Fir)
                (0.60, toFloatOpt p.Hea)
                (0.20, toFloatOpt p.Pas)
                (0.40, toFloatOpt p.Tec)
                (0.40, toFloatOpt p.Ant)
                (0.60, toFloatOpt p.Cmp)
                (1.00, toFloatOpt p.Acc)
                (0.40, toFloatOpt p.Agi)
                (0.60, toFloatOpt p.Jum)
                (1.00, toFloatOpt p.Pac)
                (0.60, toFloatOpt p.Str)
            ]
            weightedScore weightedAttrs

    let bestTargetMenAttack (players: HTML.Player list) (topN: int) : (string * float) list =
        bestBy roleRatingTargetManAttack players topN

    let bestTargetMenAttackNames (players: HTML.Player list) (topN: int) : string list =
        bestTargetMenAttack players topN |> List.map fst

    // Advanced Forward (Attack)
    let roleRatingAdvancedForwardAttack (p: HTML.Player) : float option =
        let isForwardPosition =
            posMatches p (fun up -> up.Contains("ST") || up.Contains("F C"))

        if not isForwardPosition then None
        else
            let weightedAttrs : (float * float option) list = [
                (1.00, toFloatOpt p.Pac)
                (1.00, toFloatOpt p.Acc)
                (1.00, toFloatOpt p.Fin)
                (0.80, toFloatOpt p.Dri)
                (0.60, toFloatOpt p.Fir)
                (0.60, toFloatOpt p.OtB)
                (0.60, toFloatOpt p.Tec)
                (0.60, toFloatOpt p.Ant)
                (0.60, toFloatOpt p.Cmp)
                (0.40, toFloatOpt p.Agi)
                (0.40, toFloatOpt p.Bal)
                (0.20, toFloatOpt p.Sta)
                (0.20, toFloatOpt p.Pas)
            ]
            weightedScore weightedAttrs

    let bestAdvancedForwardsAttack (players: HTML.Player list) (topN: int) : (string * float) list =
        bestBy roleRatingAdvancedForwardAttack players topN

    let bestAdvancedForwardsAttackNames (players: HTML.Player list) (topN: int) : string list =
        bestAdvancedForwardsAttack players topN |> List.map fst

    // Winger (Attack) Right
    let roleRatingWingerAttackRight (p: HTML.Player) : float option =
        let isRightWingPosition =
            posMatches p (fun up -> up.Contains("M") && up.Contains("R"))

        if not isRightWingPosition then None
        else
            let weightedAttrs : (float * float option) list = [
                (1.20, toFloatOpt p.Cro)
                (1.00, toFloatOpt p.Pac)
                (1.00, toFloatOpt p.Acc)
                (0.80, toFloatOpt p.Dri)
                (0.60, toFloatOpt p.Tec)
                (0.60, toFloatOpt p.Pas)
                (0.60, toFloatOpt p.OtB)
                (0.40, toFloatOpt p.OneVOne)
                (0.40, toFloatOpt p.Agi)
                (0.40, toFloatOpt p.Fla)
                (0.20, toFloatOpt p.Sta)
                (0.20, toFloatOpt p.Fin)
            ]
            weightedScore weightedAttrs

    let bestWingersAttackRight (players: HTML.Player list) (topN: int) : (string * float) list =
        bestBy roleRatingWingerAttackRight players topN

    let bestWingersAttackRightNames (players: HTML.Player list) (topN: int) : string list =
        bestWingersAttackRight players topN |> List.map fst

    // Inverted Winger (Support) Left
    let roleRatingInvertedWingerSupportLeft (p: HTML.Player) : float option =
        let isLeftWingPosition =
            posMatches p (fun up -> up.Contains("M") && up.Contains("L"))

        if not isLeftWingPosition then None
        else
            let weightedAttrs : (float * float option) list = [
                (0.40, toFloatOpt p.Cro)
                (0.90, toFloatOpt p.Pas)
                (0.80, toFloatOpt p.Tec)
                (0.80, toFloatOpt p.OtB)
                (0.80, toFloatOpt p.Dri)
                (0.60, toFloatOpt p.Fla)
                (0.50, toFloatOpt p.OneVOne)
                (0.60, toFloatOpt p.Cmp)
                (0.50, toFloatOpt p.Ant)
                (0.60, toFloatOpt p.Acc)
                (0.60, toFloatOpt p.Pac)
                (0.40, toFloatOpt p.Agi)
                (0.20, toFloatOpt p.Sta)
                (0.20, toFloatOpt p.Fin)
            ]
            weightedScore weightedAttrs

    let bestInvertedWingersSupportLeft (players: HTML.Player list) (topN: int) : (string * float) list =
        bestBy roleRatingInvertedWingerSupportLeft players topN

    let bestInvertedWingersSupportLeftNames (players: HTML.Player list) (topN: int) : string list =
        bestInvertedWingersSupportLeft players topN |> List.map fst

    // Advanced Playmaker (Support) MC
    let roleRatingAdvancedPlaymakerSupport (p: HTML.Player) : float option =
        let isCentralMidPosition =
            posMatches p (fun up -> up.Contains("M") && up.Contains("C"))

        if not isCentralMidPosition then None
        else
            let weightedAttrs : (float * float option) list = [
                (1.20, toFloatOpt p.Pas)
                (0.90, toFloatOpt p.Tec)
                (0.90, toFloatOpt p.OtB)
                (0.80, toFloatOpt p.Ant)
                (0.80, toFloatOpt p.Cmp)
                (0.60, toFloatOpt p.Fir)
                (0.60, toFloatOpt p.Dri)
                (0.50, toFloatOpt p.Fla)
                (0.40, toFloatOpt p.Acc)
                (0.40, toFloatOpt p.Pac)
                (0.30, toFloatOpt p.Sta)
            ]
            weightedScore weightedAttrs

    let bestAdvancedPlaymakersSupport (players: HTML.Player list) (topN: int) : (string * float) list =
        bestBy roleRatingAdvancedPlaymakerSupport players topN

    let bestAdvancedPlaymakersSupportNames (players: HTML.Player list) (topN: int) : string list =
        bestAdvancedPlaymakersSupport players topN |> List.map fst

    // Ball Winning Midfielder (Support) MC
    let roleRatingBallWinningMidfielderSupport (p: HTML.Player) : float option =
        let isCentralMidPosition =
            posMatches p (fun up -> up.Contains("M") && up.Contains("C"))

        if not isCentralMidPosition then None
        else
            let weightedAttrs : (float * float option) list = [
                (1.20, toFloatOpt p.Tck)
                (1.00, toFloatOpt p.Mar)
                (0.80, toFloatOpt p.Agg)
                (0.80, toFloatOpt p.Sta)
                (0.70, toFloatOpt p.Wor)
                (0.70, toFloatOpt p.Str)
                (0.60, toFloatOpt p.Ant)
                (0.60, toFloatOpt p.Dec)
                (0.50, toFloatOpt p.Cmp)
                (0.40, toFloatOpt p.Pas)
                (0.30, toFloatOpt p.Pac)
                (0.30, toFloatOpt p.Acc)
                (0.20, toFloatOpt p.Tec)
            ]
            weightedScore weightedAttrs

    let bestBallWinningMidfieldersSupport (players: HTML.Player list) (topN: int) : (string * float) list =
        bestBy roleRatingBallWinningMidfielderSupport players topN

    let bestBallWinningMidfieldersSupportNames (players: HTML.Player list) (topN: int) : string list =
        bestBallWinningMidfieldersSupport players topN |> List.map fst

    // Ball Playing Defender (DC)
    let roleRatingBallPlayingDefender (p: HTML.Player) : float option =
        let isCentralDefender =
            posMatches p (fun up -> ((up.Contains("D") && up.Contains("C")) || up.Contains("CB")))

        if not isCentralDefender then None
        else
            let weightedAttrs : (float * float option) list = [
                (1.20, toFloatOpt p.Pas)
                (0.90, toFloatOpt p.Tec)
                (0.90, toFloatOpt p.Cmp)
                (0.80, toFloatOpt p.Dec)
                (0.70, toFloatOpt p.Ant)
                (0.70, toFloatOpt p.Tck)
                (0.60, toFloatOpt p.Mar)
                (0.60, toFloatOpt p.Str)
                (0.50, toFloatOpt p.Hea)
                (0.40, toFloatOpt p.Jum)
                (0.40, toFloatOpt p.Pac)
                (0.30, toFloatOpt p.Acc)
                (0.30, toFloatOpt p.Sta)
                (0.30, toFloatOpt p.Agg)
            ]
            weightedScore weightedAttrs

    let bestBallPlayingDefenders (players: HTML.Player list) (topN: int) : (string * float) list =
        bestBy roleRatingBallPlayingDefender players topN

    let bestBallPlayingDefendersNames (players: HTML.Player list) (topN: int) : string list =
        bestBallPlayingDefenders players topN |> List.map fst

    // Inverted Wing Back (Support) Right
    let roleRatingInvertedWingBackSupportRight (p: HTML.Player) : float option =
        let isRightDefender =
            posMatches p (fun up -> ((up.Contains("D") && up.Contains("R")) || up.Contains("RB") || up.Contains("RWB")))

        if not isRightDefender then None
        else
            let weightedAttrs : (float * float option) list = [
                (1.00, toFloatOpt p.Pas)
                (0.90, toFloatOpt p.Tec)
                (0.80, toFloatOpt p.OtB)
                (0.80, toFloatOpt p.Cro)
                (0.80, toFloatOpt p.Dri)
                (0.70, toFloatOpt p.Pac)
                (0.60, toFloatOpt p.Acc)
                (0.60, toFloatOpt p.Sta)
                (0.60, toFloatOpt p.Wor)
                (0.60, toFloatOpt p.Cmp)
                (0.50, toFloatOpt p.Dec)
                (0.50, toFloatOpt p.Tck)
                (0.50, toFloatOpt p.Mar)
                (0.40, toFloatOpt p.Ant)
                (0.40, toFloatOpt p.Agi)
                (0.30, toFloatOpt p.Str)
            ]
            weightedScore weightedAttrs

    let bestInvertedWingBacksSupportRight (players: HTML.Player list) (topN: int) : (string * float) list =
        bestBy roleRatingInvertedWingBackSupportRight players topN

    let bestInvertedWingBacksSupportRightNames (players: HTML.Player list) (topN: int) : string list =
        bestInvertedWingBacksSupportRight players topN |> List.map fst

    // Inverted Wing Back (Support) Left
    let roleRatingInvertedWingBackSupportLeft (p: HTML.Player) : float option =
        let isLeftDefender =
            posMatches p (fun up -> ((up.Contains("D") && up.Contains("L")) || up.Contains("LB") || up.Contains("LWB")))

        if not isLeftDefender then None
        else
            let weightedAttrs : (float * float option) list = [
                (1.00, toFloatOpt p.Pas)
                (0.90, toFloatOpt p.Tec)
                (0.80, toFloatOpt p.OtB)
                (0.80, toFloatOpt p.Cro)
                (0.80, toFloatOpt p.Dri)
                (0.70, toFloatOpt p.Pac)
                (0.60, toFloatOpt p.Acc)
                (0.60, toFloatOpt p.Sta)
                (0.60, toFloatOpt p.Wor)
                (0.60, toFloatOpt p.Cmp)
                (0.50, toFloatOpt p.Dec)
                (0.50, toFloatOpt p.Tck)
                (0.50, toFloatOpt p.Mar)
                (0.40, toFloatOpt p.Ant)
                (0.40, toFloatOpt p.Agi)
                (0.30, toFloatOpt p.Str)
            ]
            weightedScore weightedAttrs

    let bestInvertedWingBacksSupportLeft (players: HTML.Player list) (topN: int) : (string * float) list =
        bestBy roleRatingInvertedWingBackSupportLeft players topN

    let bestInvertedWingBacksSupportLeftNames (players: HTML.Player list) (topN: int) : string list =
        bestInvertedWingBacksSupportLeft players topN |> List.map fst

    // Sweeper Keeper (Defend)
    let roleRatingSweeperKeeperDefend (p: HTML.Player) : float option =
        let isGoalkeeper =
            posMatches p (fun up -> up.Contains("GK") || up.Contains("G K") || up.Contains("GOAL"))

        if not isGoalkeeper then None
        else
            let weightedAttrs : (float * float option) list = [
                (1.20, toFloatOpt p.Ref)
                (1.00, toFloatOpt p.Han)
                (0.90, toFloatOpt p.Pos)
                (0.80, toFloatOpt p.Kic)
                (0.70, toFloatOpt p.Cmd)
                (0.60, toFloatOpt p.Thr)
                (0.60, toFloatOpt p.OneVOne)
                (0.50, toFloatOpt p.Pun)
                (0.40, toFloatOpt p.Com)
                (0.30, toFloatOpt p.Ecc)
                (0.30, toFloatOpt p.Aer)
                (0.20, toFloatOpt p.Acc)
                (0.20, toFloatOpt p.Pac)
            ]
            weightedScore weightedAttrs

    let bestSweeperKeepersDefend (players: HTML.Player list) (topN: int) : (string * float) list =
        bestBy roleRatingSweeperKeeperDefend players topN

    let bestSweeperKeepersDefendNames (players: HTML.Player list) (topN: int) : string list =
        bestSweeperKeepersDefend players topN |> List.map fst
namespace FM21_ToolsLib

open System

module ROLE =

    /// Calculate a normalized role rating for "Target Man (Attack)".
    /// Uses a weighted set of attributes; missing attributes are ignored and weights renormalized.
    let roleRatingTargetManAttack (p: HTML.Player) : float option =
        // only consider players who have a forward/striker position
        let isForwardPosition =
            p.Position
            |> Option.exists (fun s ->
                let up = s.ToUpperInvariant()
                up.Contains("ST") || up.Contains("F C"))

        if not isForwardPosition then None
        else
            // helper to convert int option -> float option
            let toFloatOpt = Option.map float

            // Weights chosen to emphasise aerial ability, strength and finishing for a target man.
            // We renormalize if some attributes are missing.
            let weightedAttrs : (float * float option) list = [
                (0.40, toFloatOpt p.Dri)   // Dribbling
                (0.60, toFloatOpt p.Fin)   // Finishing
                (0.60, toFloatOpt p.Fir)   // First touch
                (0.60, toFloatOpt p.Hea)   // Heading
                (0.20, toFloatOpt p.Pas)   // Passing
                (0.40, toFloatOpt p.Tec)   // Technique
                (0.40, toFloatOpt p.Ant)   // Anticipation
                (0.60, toFloatOpt p.Cmp)   // Composure
                (1.00, toFloatOpt p.Acc)   // Acceleration
                (0.40, toFloatOpt p.Agi)   // Agility
                (0.60, toFloatOpt p.Jum)   // Jumping reach
                (1.00, toFloatOpt p.Pac)   // Pace (acc/pace helps hold-up play into channels)
                (0.60, toFloatOpt p.Str)   // Strength
            ]

            // Keep only present attributes and compute weighted score and total weight
            let totalWeight, weightedSum =
                weightedAttrs
                |> List.fold (fun (tw, ws) (w, vOpt) ->
                    match vOpt with
                    | Some v -> (tw + w, ws + w * v)
                    | None -> (tw, ws)) (0.0, 0.0)

            if totalWeight = 0.0 then None
            else Some (5.0 * weightedSum / totalWeight)

    /// Return the best target men (attack) as a list of (Name, Score) sorted descending by score.
    /// If `topN` <= 0 all players with a score are returned; otherwise only the top `topN` are returned.
    let bestTargetMenAttack (players: HTML.Player list) (topN: int) : (string * float) list =
        let sorted =
            players
            |> List.choose (fun p -> roleRatingTargetManAttack p |> Option.map (fun s -> (p.Name, s)))
            |> List.sortByDescending snd

        if topN <= 0 then sorted else List.truncate topN sorted

    /// Return only the names of the best target men (attack), ordered by rating (highest first).
    /// `topN` follows the same semantics as `bestTargetMenAttack`.
    let bestTargetMenAttackNames (players: HTML.Player list) (topN: int) : string list =
        bestTargetMenAttack players topN |> List.map fst


    /// Calculate a normalized role rating for "Advanced Forward (Attack)".
    /// Focuses on pace, finishing, dribbling and off-the-ball movement; missing attributes are ignored and weights renormalized.
    let roleRatingAdvancedForwardAttack (p: HTML.Player) : float option =
        // only consider players who have a forward/striker position
        let isForwardPosition =
            p.Position
            |> Option.exists (fun s ->
                let up = s.ToUpperInvariant()
                up.Contains("ST") || up.Contains("F C"))

        if not isForwardPosition then None
        else
            let toFloatOpt = Option.map float

            // Weights chosen to emphasise pace, finishing, dribbling and movement
            let weightedAttrs : (float * float option) list = [
                (1.00, toFloatOpt p.Pac)   // Pace
                (1.00, toFloatOpt p.Acc)   // Acceleration
                (1.00, toFloatOpt p.Fin)   // Finishing
                (0.80, toFloatOpt p.Dri)   // Dribbling
                (0.60, toFloatOpt p.Fir)   // First touch
                (0.60, toFloatOpt p.OtB)   // Off the ball
                (0.60, toFloatOpt p.Tec)   // Technique
                (0.60, toFloatOpt p.Ant)   // Anticipation
                (0.60, toFloatOpt p.Cmp)   // Composure
                (0.40, toFloatOpt p.Agi)   // Agility
                (0.40, toFloatOpt p.Bal)   // Balance
                (0.20, toFloatOpt p.Sta)   // Stamina
                (0.20, toFloatOpt p.Pas)   // Passing (link-up play)
            ]

            let totalWeight, weightedSum =
                weightedAttrs
                |> List.fold (fun (tw, ws) (w, vOpt) ->
                    match vOpt with
                    | Some v -> (tw + w, ws + w * v)
                    | None -> (tw, ws)) (0.0, 0.0)

            if totalWeight = 0.0 then None
            else Some (5.0 * weightedSum / totalWeight)

    /// Return the best advanced forwards (attack) as a list of (Name, Score) sorted descending by score.
    /// If `topN` <= 0 all players with a score are returned; otherwise only the top `topN` are returned.
    let bestAdvancedForwardsAttack (players: HTML.Player list) (topN: int) : (string * float) list =
        let sorted =
            players
            |> List.choose (fun p -> roleRatingAdvancedForwardAttack p |> Option.map (fun s -> (p.Name, s)))
            |> List.sortByDescending snd

        if topN <= 0 then sorted else List.truncate topN sorted

    /// Return only the names of the best advanced forwards (attack), ordered by rating (highest first).
    /// `topN` follows the same semantics as `bestAdvancedForwardsAttack`.
    let bestAdvancedForwardsAttackNames (players: HTML.Player list) (topN: int) : string list =
        bestAdvancedForwardsAttack players topN |> List.map fst


    /// Calculate a normalized role rating for "Winger (Attack)" — targeted at right midfield/wing (MR/RW).
    /// Emphasises crossing, pace and dribbling; missing attributes are ignored and weights renormalized.
    let roleRatingWingerAttackRight (p: HTML.Player) : float option =
        // only consider players who have a right-sided wide midfield/wing position
        let isRightWingPosition =
            p.Position
            |> Option.exists (fun s ->
                let up = s.ToUpperInvariant()
                up.Contains("M") && up.Contains("R"))

        if not isRightWingPosition then None
        else
            let toFloatOpt = Option.map float

            // Weights chosen to prioritize crossing and pace for an attacking right winger,
            // with support from dribbling, technique and passing.
            let weightedAttrs : (float * float option) list = [
                (1.20, toFloatOpt p.Cro)      // Crossing (primary)
                (1.00, toFloatOpt p.Pac)      // Pace
                (1.00, toFloatOpt p.Acc)      // Acceleration
                (0.80, toFloatOpt p.Dri)      // Dribbling
                (0.60, toFloatOpt p.Tec)      // Technique
                (0.60, toFloatOpt p.Pas)      // Passing (crosses, cutbacks)
                (0.60, toFloatOpt p.OtB)      // Off the ball (movement into space)
                (0.40, toFloatOpt p.OneVOne)  // Ability to beat defender 1v1
                (0.40, toFloatOpt p.Agi)      // Agility
                (0.40, toFloatOpt p.Fla)      // Flair (creative play)
                (0.20, toFloatOpt p.Sta)      // Stamina (work rate up and down flank)
                (0.20, toFloatOpt p.Fin)      // Finishing (occasional cutting inside)
            ]

            let totalWeight, weightedSum =
                weightedAttrs
                |> List.fold (fun (tw, ws) (w, vOpt) ->
                    match vOpt with
                    | Some v -> (tw + w, ws + w * v)
                    | None -> (tw, ws)) (0.0, 0.0)

            if totalWeight = 0.0 then None
            else Some (5.0 * weightedSum / totalWeight)

    /// Return the best wingers (attack, right side) as a list of (Name, Score) sorted descending by score.
    /// If `topN` <= 0 all players with a score are returned; otherwise only the top `topN` are returned.
    let bestWingersAttackRight (players: HTML.Player list) (topN: int) : (string * float) list =
        let sorted =
            players
            |> List.choose (fun p -> roleRatingWingerAttackRight p |> Option.map (fun s -> (p.Name, s)))
            |> List.sortByDescending snd

        if topN <= 0 then sorted else List.truncate topN sorted

    /// Return only the names of the best wingers (attack, right side), ordered by rating (highest first).
    /// `topN` follows the same semantics as `bestWingersAttackRight`.
    let bestWingersAttackRightNames (players: HTML.Player list) (topN: int) : string list =
        bestWingersAttackRight players topN |> List.map fst


    /// Calculate a normalized role rating for "Inverted Winger (Support)" — targeted at left midfield/wing (ML/LW).
    /// Inverted wingers on the left in a support role link play and create chances by cutting inside and combining.
    /// Emphasise passing, technique, off-the-ball movement and dribbling; crossing and finishing are de-emphasised.
    let roleRatingInvertedWingerSupportLeft (p: HTML.Player) : float option =
        // only consider players who have a left-sided wide midfield/wing position
        let isLeftWingPosition =
            p.Position
            |> Option.exists (fun s ->
                let up = s.ToUpperInvariant()
                up.Contains("M") && up.Contains("L"))

        if not isLeftWingPosition then None
        else
            let toFloatOpt = Option.map float

            // Weights chosen to favour chance creation and link-up play rather than pure scoring.
            let weightedAttrs : (float * float option) list = [
                (0.40, toFloatOpt p.Cro)      // Crossing (useful but not primary for inverted support)
                (0.90, toFloatOpt p.Pas)      // Passing (key for creating chances and link-up)
                (0.80, toFloatOpt p.Tec)      // Technique
                (0.80, toFloatOpt p.OtB)      // Off the ball (movement into pockets)
                (0.80, toFloatOpt p.Dri)      // Dribbling (ability to beat and combine)
                (0.60, toFloatOpt p.Fla)      // Flair (creative play)
                (0.50, toFloatOpt p.OneVOne)  // Ability to beat defender 1v1
                (0.60, toFloatOpt p.Cmp)      // Composure (under pressure)
                (0.50, toFloatOpt p.Ant)      // Anticipation
                (0.60, toFloatOpt p.Acc)      // Acceleration
                (0.60, toFloatOpt p.Pac)      // Pace (to exploit spaces)
                (0.40, toFloatOpt p.Agi)      // Agility
                (0.20, toFloatOpt p.Sta)      // Stamina
                (0.20, toFloatOpt p.Fin)      // Finishing (support role: occasional)
            ]

            let totalWeight, weightedSum =
                weightedAttrs
                |> List.fold (fun (tw, ws) (w, vOpt) ->
                    match vOpt with
                    | Some v -> (tw + w, ws + w * v)
                    | None -> (tw, ws)) (0.0, 0.0)

            if totalWeight = 0.0 then None
            else Some (5.0 * weightedSum / totalWeight)

    /// Return the best inverted wingers (support, left side) as a list of (Name, Score) sorted descending by score.
    /// If `topN` <= 0 all players with a score are returned; otherwise only the top `topN` are returned.
    let bestInvertedWingersSupportLeft (players: HTML.Player list) (topN: int) : (string * float) list =
        let sorted =
            players
            |> List.choose (fun p -> roleRatingInvertedWingerSupportLeft p |> Option.map (fun s -> (p.Name, s)))
            |> List.sortByDescending snd

        if topN <= 0 then sorted else List.truncate topN sorted

    /// Return only the names of the best inverted wingers (support, left side), ordered by rating (highest first).
    /// `topN` follows the same semantics as `bestInvertedWingersSupportLeft`.
    let bestInvertedWingersSupportLeftNames (players: HTML.Player list) (topN: int) : string list =
        bestInvertedWingersSupportLeft players topN |> List.map fst


    /// Calculate a normalized role rating for "Advanced Playmaker (Support)" — targeted at central midfield (MC).
    /// Emphasises passing, technique, off-the-ball movement, anticipation and composure.
    let roleRatingAdvancedPlaymakerSupport (p: HTML.Player) : float option =
        // only consider players who have a central midfield position
        let isCentralMidPosition =
            p.Position
            |> Option.exists (fun s ->
                let up = s.ToUpperInvariant()
                up.Contains("M") && up.Contains("C"))

        if not isCentralMidPosition then None
        else
            let toFloatOpt = Option.map float

            // Weights chosen to prioritise chance creation and control in midfield.
            let weightedAttrs : (float * float option) list = [
                (1.20, toFloatOpt p.Pas)    // Passing (primary)
                (0.90, toFloatOpt p.Tec)    // Technique
                (0.90, toFloatOpt p.OtB)    // Off the ball (movement into pockets)
                (0.80, toFloatOpt p.Ant)    // Anticipation
                (0.80, toFloatOpt p.Cmp)    // Composure
                (0.60, toFloatOpt p.Fir)    // First touch
                (0.60, toFloatOpt p.Dri)    // Dribbling (retain ball and combine)
                (0.50, toFloatOpt p.Fla)    // Flair (creative instinct)
                (0.40, toFloatOpt p.Acc)    // Acceleration (short bursts)
                (0.40, toFloatOpt p.Pac)    // Pace (covering ground)
                (0.30, toFloatOpt p.Sta)    // Stamina (work rate)
            ]

            let totalWeight, weightedSum =
                weightedAttrs
                |> List.fold (fun (tw, ws) (w, vOpt) ->
                    match vOpt with
                    | Some v -> (tw + w, ws + w * v)
                    | None -> (tw, ws)) (0.0, 0.0)

            if totalWeight = 0.0 then None
            else Some (5.0 * weightedSum / totalWeight)

    /// Return the best advanced playmakers (support, central midfield) as a list of (Name, Score) sorted descending by score.
    /// If `topN` <= 0 all players with a score are returned; otherwise only the top `topN` are returned.
    let bestAdvancedPlaymakersSupport (players: HTML.Player list) (topN: int) : (string * float) list =
        let sorted =
            players
            |> List.choose (fun p -> roleRatingAdvancedPlaymakerSupport p |> Option.map (fun s -> (p.Name, s)))
            |> List.sortByDescending snd

        if topN <= 0 then sorted else List.truncate topN sorted

    /// Return only the names of the best advanced playmakers (support, central midfield), ordered by rating (highest first).
    /// `topN` follows the same semantics as `bestAdvancedPlaymakersSupportMC`.
    let bestAdvancedPlaymakersSupportNames (players: HTML.Player list) (topN: int) : string list =
        bestAdvancedPlaymakersSupport players topN |> List.map fst
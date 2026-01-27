namespace FM21_ToolsLib

open System
open System.Text.RegularExpressions

module TEAM =

    // Use the shared RoleRatedPlayer type instead of the local Position type.
    // Unassigned positions are represented as a `TYPES.RoleRatedPlayer option`.
    // BallPlayingDefs remains a list (may contain fewer than 2 entries if picks < 2).
    type Team = {
        SweeperKeeper: TYPES.RoleRatedPlayer option
        InvertedWingBackRight: TYPES.RoleRatedPlayer option
        InvertedWingBackLeft: TYPES.RoleRatedPlayer option
        BallPlayingDefs: TYPES.RoleRatedPlayer option list
        WingerAttackRight: TYPES.RoleRatedPlayer option
        InvertedWingerLeft: TYPES.RoleRatedPlayer option
        BallWinningMidfielderSupport: TYPES.RoleRatedPlayer option
        AdvancedPlaymakerSupport: TYPES.RoleRatedPlayer option
        AdvancedForwardAttack: TYPES.RoleRatedPlayer option
        TargetManAttack: TYPES.RoleRatedPlayer option
    }

    // mkUnassigned returns None; mkAssigned returns Some RoleRatedPlayer only when an HTML.Player is available.
    let private mkUnassigned (_role : string) : TYPES.RoleRatedPlayer option = None
    let private mkAssigned role (name:string) (rating:float) (playerOpt: HTML.Player option) : TYPES.RoleRatedPlayer option =
        match playerOpt with
        | Some p -> Some { Name = name; RoleName = role; Rating = rating; Player = p }
        | None -> None

    // pick helper: call ROLE.* fn to get top N (name * rating), attach matching Player from pool, remove selected from pool
    let private pickN (bestFn: HTML.Player list -> int -> (string * float) list) count (pool: HTML.Player list) =
        let picks = bestFn pool count
        let picksWithPlayer =
            picks |> List.map (fun (n,r) -> (n, r, pool |> List.tryFind (fun p -> p.Name = n)))
        let selectedNames = picksWithPlayer |> List.map (fun (n,_,_) -> n)
        let remaining = pool |> List.filter (fun p -> not (List.exists ((=) p.Name) selectedNames))
        picksWithPlayer, remaining

    let buildTeam (players: HTML.Player list) : Team =
        let pool0 = players

        let sk, pool1 = pickN ROLE.bestSweeperKeepersDefend 1 pool0
        let skPos = match sk |> List.tryHead with | Some (n,r,p) -> mkAssigned "Sweeper Keeper" n r p | None -> mkUnassigned "Sweeper Keeper"

        let iwbR, pool2 = pickN ROLE.bestInvertedWingBacksSupportRight 1 pool1
        let iwbRPos = match iwbR |> List.tryHead with | Some (n,r,p) -> mkAssigned "Inverted Wing Back (R)" n r p | None -> mkUnassigned "Inverted Wing Back (R)"

        let iwbL, pool3 = pickN ROLE.bestInvertedWingBacksSupportLeft 1 pool2
        let iwbLPos = match iwbL |> List.tryHead with | Some (n,r,p) -> mkAssigned "Inverted Wing Back (L)" n r p | None -> mkUnassigned "Inverted Wing Back (L)"

        let bpd, pool4 = pickN ROLE.bestBallPlayingDefenders 2 pool3
        let bpdPos =
            bpd |> List.mapi (fun i (n,r,p) -> mkAssigned (sprintf "Ball Playing Defender #%d" (i+1)) n r p)

        let wgr, pool5 = pickN ROLE.bestWingersAttackRight 1 pool4
        let wgrPos = match wgr |> List.tryHead with | Some (n,r,p) -> mkAssigned "Winger (Attack) R" n r p | None -> mkUnassigned "Winger (Attack) R"

        let iwL, pool6 = pickN ROLE.bestInvertedWingersSupportLeft 1 pool5
        let iwLPos = match iwL |> List.tryHead with | Some (n,r,p) -> mkAssigned "Inverted Winger (L)" n r p | None -> mkUnassigned "Inverted Winger (L)"

        let bwm, pool7 = pickN ROLE.bestBallWinningMidfieldersSupport 1 pool6
        let bwmPos = match bwm |> List.tryHead with | Some (n,r,p) -> mkAssigned "Ball Winning Midfielder (Support)" n r p | None -> mkUnassigned "Ball Winning Midfielder (Support)"

        let ap, pool8 = pickN ROLE.bestAdvancedPlaymakersSupport 1 pool7
        let apPos = match ap |> List.tryHead with | Some (n,r,p) -> mkAssigned "Advanced Playmaker (Support)" n r p | None -> mkUnassigned "Advanced Playmaker (Support)"

        let afa, pool9 = pickN ROLE.bestAdvancedForwardsAttack 1 pool8
        let afaPos = match afa |> List.tryHead with | Some (n,r,p) -> mkAssigned "Advanced Forward (Attack)" n r p | None -> mkUnassigned "Advanced Forward (Attack)"

        let tma, _ = pickN ROLE.bestTargetMenAttack 1 pool9
        let tmaPos = match tma |> List.tryHead with | Some (n,r,p) -> mkAssigned "Target Man (Attack)" n r p | None -> mkUnassigned "Target Man (Attack)"

        {
            SweeperKeeper = skPos
            InvertedWingBackRight = iwbRPos
            InvertedWingBackLeft = iwbLPos
            BallPlayingDefs = bpdPos
            WingerAttackRight = wgrPos
            InvertedWingerLeft = iwLPos
            BallWinningMidfielderSupport = bwmPos
            AdvancedPlaymakerSupport = apPos
            AdvancedForwardAttack = afaPos
            TargetManAttack = tmaPos
        }

    // map a RoleName to the requested abbreviation for printed output
    let private roleAbbrev (role: string) : string =
        if role.StartsWith("Ball Playing Defender", StringComparison.InvariantCultureIgnoreCase) then
            // try to extract the index suffix if present (#n)
            let m = Regex.Match(role, "#(\\d+)")
            if m.Success then sprintf "BPD%s" m.Groups.[1].Value else "BPD"
        else
            match role with
            | "Sweeper Keeper" -> "SKD"
            | "Inverted Wing Back (R)" -> "IWBR"
            | "Inverted Wing Back (L)" -> "IWBL"
            | "Winger (Attack) R" -> "WAR"
            | "Inverted Winger (L)" -> "IWL"
            | "Ball Winning Midfielder (Support)" -> "BWM"
            | "Advanced Playmaker (Support)" -> "AP"
            | "Advanced Forward (Attack)" -> "AFA"
            | "Target Man (Attack)" -> "TMA"
            | _ -> role // fallback to original if unknown

    let teamAsPositionNameOptions (t: Team) =
        let toTupleFromField canonicalRoleName (pOpt: TYPES.RoleRatedPlayer option) =
            let roleName = match pOpt with | Some r -> r.RoleName | None -> canonicalRoleName
            let playerName = pOpt |> Option.map (fun r -> r.Name)
            (roleAbbrev roleName, playerName)

        [ toTupleFromField "Sweeper Keeper" t.SweeperKeeper
          toTupleFromField "Inverted Wing Back (R)" t.InvertedWingBackRight
          toTupleFromField "Inverted Wing Back (L)" t.InvertedWingBackLeft ]
        @ (t.BallPlayingDefs |> List.map (fun pOpt ->
            let roleName = match pOpt with | Some r -> r.RoleName | None -> "Ball Playing Defender"
            let playerName = pOpt |> Option.map (fun r -> r.Name)
            (roleAbbrev roleName, playerName)))
        @ [ toTupleFromField "Winger (Attack) R" t.WingerAttackRight
            toTupleFromField "Inverted Winger (L)" t.InvertedWingerLeft
            toTupleFromField "Ball Winning Midfielder (Support)" t.BallWinningMidfielderSupport
            toTupleFromField "Advanced Playmaker (Support)" t.AdvancedPlaymakerSupport
            toTupleFromField "Advanced Forward (Attack)" t.AdvancedForwardAttack
            toTupleFromField "Target Man (Attack)" t.TargetManAttack ]

    let teamAsStrings t =
        teamAsPositionNameOptions t |> List.map (fun (r,n) -> sprintf "%s: %s" r (defaultArg n "Unassigned"))

    let teamScore t =
        let ratings =
            [ t.SweeperKeeper |> Option.map (fun r -> r.Rating)
              t.InvertedWingBackRight |> Option.map (fun r -> r.Rating)
              t.InvertedWingBackLeft |> Option.map (fun r -> r.Rating) ]
            @ (t.BallPlayingDefs |> List.map (fun pOpt -> pOpt |> Option.map (fun r -> r.Rating)))
            @ [ t.WingerAttackRight |> Option.map (fun r -> r.Rating)
                t.InvertedWingerLeft |> Option.map (fun r -> r.Rating)
                t.BallWinningMidfielderSupport |> Option.map (fun r -> r.Rating)
                t.AdvancedPlaymakerSupport |> Option.map (fun r -> r.Rating)
                t.AdvancedForwardAttack |> Option.map (fun r -> r.Rating)
                t.TargetManAttack |> Option.map (fun r -> r.Rating) ]
        ratings |> List.sumBy (fun o -> defaultArg o 0.0)

    let teamScoreOption t =
        let ratings =
            [ t.SweeperKeeper |> Option.map (fun r -> r.Rating)
              t.InvertedWingBackRight |> Option.map (fun r -> r.Rating)
              t.InvertedWingBackLeft |> Option.map (fun r -> r.Rating) ]
            @ (t.BallPlayingDefs |> List.map (fun pOpt -> pOpt |> Option.map (fun r -> r.Rating)))
            @ [ t.WingerAttackRight |> Option.map (fun r -> r.Rating)
                t.InvertedWingerLeft |> Option.map (fun r -> r.Rating)
                t.BallWinningMidfielderSupport |> Option.map (fun r -> r.Rating)
                t.AdvancedPlaymakerSupport |> Option.map (fun r -> r.Rating)
                t.AdvancedForwardAttack |> Option.map (fun r -> r.Rating)
                t.TargetManAttack |> Option.map (fun r -> r.Rating) ]
        if List.exists Option.isNone ratings then None else Some (ratings |> List.sumBy (fun o -> defaultArg o 0.0))
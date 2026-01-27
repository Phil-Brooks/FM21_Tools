namespace FM21_ToolsLib

open System
open System.Text.RegularExpressions

module TEAM =

    // Use the shared RoleRatedPlayer type instead of the local Position type.
    // Unassigned positions are represented as a `TYPES.RoleRatedPlayer option`.
    // BallPlayingDef1 and BallPlayingDef2 replace the previous BallPlayingDefs list (two separate fields).
    type Team = {
        SweeperKeeper: TYPES.RoleRatedPlayer option
        InvertedWingBackRight: TYPES.RoleRatedPlayer option
        InvertedWingBackLeft: TYPES.RoleRatedPlayer option
        BallPlayingDef1: TYPES.RoleRatedPlayer option
        BallPlayingDef2: TYPES.RoleRatedPlayer option
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
        let skPos = match sk |> List.tryHead with | Some (n,r,p) -> mkAssigned "SKD" n r p | None -> mkUnassigned "SKD"

        let iwbR, pool2 = pickN ROLE.bestInvertedWingBacksSupportRight 1 pool1
        let iwbRPos = match iwbR |> List.tryHead with | Some (n,r,p) -> mkAssigned "IWBR" n r p | None -> mkUnassigned "IWBR"

        let iwbL, pool3 = pickN ROLE.bestInvertedWingBacksSupportLeft 1 pool2
        let iwbLPos = match iwbL |> List.tryHead with | Some (n,r,p) -> mkAssigned "IWBL" n r p | None -> mkUnassigned "IWBL"

        let bpd, pool4 = pickN ROLE.bestBallPlayingDefenders 2 pool3
        let bpd1Pos = match bpd |> List.tryItem 0 with | Some (n,r,p) -> mkAssigned "BPD1" n r p | None -> mkUnassigned "BPD1"
        let bpd2Pos = match bpd |> List.tryItem 1 with | Some (n,r,p) -> mkAssigned "BPD2" n r p | None -> mkUnassigned "BPD2"

        let wgr, pool5 = pickN ROLE.bestWingersAttackRight 1 pool4
        let wgrPos = match wgr |> List.tryHead with | Some (n,r,p) -> mkAssigned "WAR" n r p | None -> mkUnassigned "WAR"

        let iwL, pool6 = pickN ROLE.bestInvertedWingersSupportLeft 1 pool5
        let iwLPos = match iwL |> List.tryHead with | Some (n,r,p) -> mkAssigned "IWL" n r p | None -> mkUnassigned "IWL"

        let bwm, pool7 = pickN ROLE.bestBallWinningMidfieldersSupport 1 pool6
        let bwmPos = match bwm |> List.tryHead with | Some (n,r,p) -> mkAssigned "BWM" n r p | None -> mkUnassigned "BWM"

        let ap, pool8 = pickN ROLE.bestAdvancedPlaymakersSupport 1 pool7
        let apPos = match ap |> List.tryHead with | Some (n,r,p) -> mkAssigned "AP" n r p | None -> mkUnassigned "AP"

        let afa, pool9 = pickN ROLE.bestAdvancedForwardsAttack 1 pool8
        let afaPos = match afa |> List.tryHead with | Some (n,r,p) -> mkAssigned "AFA" n r p | None -> mkUnassigned "AFA"

        let tma, _ = pickN ROLE.bestTargetMenAttack 1 pool9
        let tmaPos = match tma |> List.tryHead with | Some (n,r,p) -> mkAssigned "TMA" n r p | None -> mkUnassigned "TMA"

        {
            SweeperKeeper = skPos
            InvertedWingBackRight = iwbRPos
            InvertedWingBackLeft = iwbLPos
            BallPlayingDef1 = bpd1Pos
            BallPlayingDef2 = bpd2Pos
            WingerAttackRight = wgrPos
            InvertedWingerLeft = iwLPos
            BallWinningMidfielderSupport = bwmPos
            AdvancedPlaymakerSupport = apPos
            AdvancedForwardAttack = afaPos
            TargetManAttack = tmaPos
        }

    let teamAsPositionNameOptions (t: Team) =
        let toTupleFromField canonicalRoleName (pOpt: TYPES.RoleRatedPlayer option) =
            let roleName = match pOpt with | Some r -> r.RoleName | None -> canonicalRoleName
            let playerName = pOpt |> Option.map (fun r -> r.Name)
            (roleName, playerName)

        // annotate pOpt type so field lookups resolve at compile time
        let bpTuple (pOpt: TYPES.RoleRatedPlayer option) =
            let roleName = match pOpt with | Some r -> r.RoleName | None -> "BPD"
            let playerName = pOpt |> Option.map (fun r -> r.Name)
            (roleName, playerName)

        [ toTupleFromField "SKD" t.SweeperKeeper
          toTupleFromField "IWBR" t.InvertedWingBackRight
          toTupleFromField "IWBL" t.InvertedWingBackLeft ]
        @ [ bpTuple t.BallPlayingDef1
            bpTuple t.BallPlayingDef2 ]
        @ [ toTupleFromField "WAR" t.WingerAttackRight
            toTupleFromField "IWL" t.InvertedWingerLeft
            toTupleFromField "BWM" t.BallWinningMidfielderSupport
            toTupleFromField "AP" t.AdvancedPlaymakerSupport
            toTupleFromField "AFA" t.AdvancedForwardAttack
            toTupleFromField "TMA" t.TargetManAttack ]

    let teamAsStrings t =
        teamAsPositionNameOptions t |> List.map (fun (r,n) -> sprintf "%s: %s" r (defaultArg n "Unassigned"))

    let teamScore t =
        let ratings =
            [ t.SweeperKeeper |> Option.map (fun r -> r.Rating)
              t.InvertedWingBackRight |> Option.map (fun r -> r.Rating)
              t.InvertedWingBackLeft |> Option.map (fun r -> r.Rating)
              t.BallPlayingDef1 |> Option.map (fun r -> r.Rating)
              t.BallPlayingDef2 |> Option.map (fun r -> r.Rating) ]
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
              t.InvertedWingBackLeft |> Option.map (fun r -> r.Rating)
              t.BallPlayingDef1 |> Option.map (fun r -> r.Rating)
              t.BallPlayingDef2 |> Option.map (fun r -> r.Rating) ]
            @ [ t.WingerAttackRight |> Option.map (fun r -> r.Rating)
                t.InvertedWingerLeft |> Option.map (fun r -> r.Rating)
                t.BallWinningMidfielderSupport |> Option.map (fun r -> r.Rating)
                t.AdvancedPlaymakerSupport |> Option.map (fun r -> r.Rating)
                t.AdvancedForwardAttack |> Option.map (fun r -> r.Rating)
                t.TargetManAttack |> Option.map (fun r -> r.Rating) ]
        if List.exists Option.isNone ratings then None else Some (ratings |> List.sumBy (fun o -> defaultArg o 0.0))
namespace FM21_ToolsLib

open System
open System.Text.RegularExpressions

module TEAM =

    type Position = {
        RoleName: string
        PlayerName: string option
        Rating: float option
        Player: HTML.Player option
    }

    type Team = {
        SweeperKeeper: Position
        InvertedWingBackRight: Position
        InvertedWingBackLeft: Position
        BallPlayingDefs: Position list
        WingerAttackRight: Position
        InvertedWingerLeft: Position
        BallWinningMidfielderSupport: Position
        AdvancedPlaymakerSupport: Position
        AdvancedForwardAttack: Position
        TargetManAttack: Position
    }

    let private mkUnassigned role = { RoleName = role; PlayerName = None; Rating = None; Player = None }
    let private mkAssigned role (name:string) (rating:float) (playerOpt: HTML.Player option) =
        { RoleName = role; PlayerName = Some name; Rating = Some rating; Player = playerOpt }

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
        let toTuple p = (roleAbbrev p.RoleName, p.PlayerName)
        [ toTuple t.SweeperKeeper; toTuple t.InvertedWingBackRight; toTuple t.InvertedWingBackLeft ]
        @ (t.BallPlayingDefs |> List.map (fun p -> (roleAbbrev p.RoleName, p.PlayerName)))
        @ [ toTuple t.WingerAttackRight; toTuple t.InvertedWingerLeft; toTuple t.BallWinningMidfielderSupport;
            toTuple t.AdvancedPlaymakerSupport; toTuple t.AdvancedForwardAttack; toTuple t.TargetManAttack ]

    let teamAsStrings t =
        teamAsPositionNameOptions t |> List.map (fun (r,n) -> sprintf "%s: %s" r (defaultArg n "Unassigned"))

    let teamScore t =
        let ratings =
            [ t.SweeperKeeper.Rating; t.InvertedWingBackRight.Rating; t.InvertedWingBackLeft.Rating ]
            @ (t.BallPlayingDefs |> List.map (fun p -> p.Rating))
            @ [ t.WingerAttackRight.Rating; t.InvertedWingerLeft.Rating; t.BallWinningMidfielderSupport.Rating;
                t.AdvancedPlaymakerSupport.Rating; t.AdvancedForwardAttack.Rating; t.TargetManAttack.Rating ]
        ratings |> List.sumBy (fun o -> defaultArg o 0.0)

    let teamScoreOption t =
        let ratings =
            [ t.SweeperKeeper.Rating; t.InvertedWingBackRight.Rating; t.InvertedWingBackLeft.Rating ]
            @ (t.BallPlayingDefs |> List.map (fun p -> p.Rating))
            @ [ t.WingerAttackRight.Rating; t.InvertedWingerLeft.Rating; t.BallWinningMidfielderSupport.Rating;
                t.AdvancedPlaymakerSupport.Rating; t.AdvancedForwardAttack.Rating; t.TargetManAttack.Rating ]
        if List.exists Option.isNone ratings then None else Some (ratings |> List.sumBy (fun o -> defaultArg o 0.0))
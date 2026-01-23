namespace FM21_ToolsLib

open System

module TEAM =

    // A simple container for a role assignment
    type Position =
        { RoleName: string
          PlayerName: string option
          Rating: float option }

    type Team =
        { SweeperKeeper: Position
          InvertedWingBackRight: Position
          InvertedWingBackLeft: Position
          BallPlayingDefs: Position list          // expecting 2
          WingerAttackRight: Position
          InvertedWingerLeft: Position
          BallWinningMidfielderSupport: Position
          AdvancedPlaymakerSupport: Position
          AdvancedForwardAttack: Position
          TargetManAttack: Position }

    // helper to convert (name * rating) option -> Position
    let private toPosition role (candidate: (string * float) option) =
        match candidate with
        | Some (n, r) -> { RoleName = role; PlayerName = Some n; Rating = Some r }
        | None -> { RoleName = role; PlayerName = None; Rating = None }

    // remove selected player names from the remaining pool
    let private removeSelected (remaining: HTML.Player list) (selectedNames: string list) =
        remaining |> List.filter (fun p -> not (List.contains p.Name selectedNames))

    // pick N best using a ROLE.* function that returns (string * float) list
    let private pickBestN (bestFn: HTML.Player list -> int -> (string * float) list) (count: int) (remaining: HTML.Player list) =
        let picks = bestFn remaining count
        let names = picks |> List.map fst
        let remaining' = removeSelected remaining names
        picks, remaining'

    // Build a Team from a list of players. Picks greedily in a stable order and ensures uniqueness.
    let buildTeam (players: HTML.Player list) : Team =
        // start with full pool
        let remaining0 = players

        // 1 Sweeper Keeper
        let skPicks, remaining1 = pickBestN ROLE.bestSweeperKeepersDefend 1 remaining0
        let skPos = toPosition "Sweeper Keeper" (skPicks |> List.tryHead)

        // 1 Inverted Wing Back Right
        let iwbR, remaining2 = pickBestN ROLE.bestInvertedWingBacksSupportRight 1 remaining1
        let iwbRPos = toPosition "Inverted Wing Back (R)" (iwbR |> List.tryHead)

        // 1 Inverted Wing Back Left
        let iwbL, remaining3 = pickBestN ROLE.bestInvertedWingBacksSupportLeft 1 remaining2
        let iwbLPos = toPosition "Inverted Wing Back (L)" (iwbL |> List.tryHead)

        // 2 Ball Playing Defenders (CBs)
        let bpdPicks, remaining4 = pickBestN ROLE.bestBallPlayingDefenders 2 remaining3
        let bpdPositions = 
            bpdPicks 
            |> List.mapi (fun i p -> toPosition (sprintf "Ball Playing Defender #%d" (i+1)) (Some p))

        // Winger Attack Right
        let wgrPicks, remaining5 = pickBestN ROLE.bestWingersAttackRight 1 remaining4
        let wgrPos = toPosition "Winger (Attack) R" (wgrPicks |> List.tryHead)

        // Inverted Winger Left (attacking/support role)
        let iwL_Picks, remaining6 = pickBestN ROLE.bestInvertedWingersSupportLeft 1 remaining5
        let iwLPos = toPosition "Inverted Winger (L)" (iwL_Picks |> List.tryHead)

        // Ball Winning Midfielder (Support)
        let bwmPicks, remaining7 = pickBestN ROLE.bestBallWinningMidfieldersSupport 1 remaining6
        let bwmPos = toPosition "Ball Winning Midfielder (Support)" (bwmPicks |> List.tryHead)

        // Advanced Playmaker (Support)
        let apPicks, remaining8 = pickBestN ROLE.bestAdvancedPlaymakersSupport 1 remaining7
        let apPos = toPosition "Advanced Playmaker (Support)" (apPicks |> List.tryHead)

        // Advanced Forward (Attack)
        let afaPicks, remaining9 = pickBestN ROLE.bestAdvancedForwardsAttack 1 remaining8
        let afaPos = toPosition "Advanced Forward (Attack)" (afaPicks |> List.tryHead)

        // Target Man (Attack)
        let tmaPicks, _remaining10 = pickBestN ROLE.bestTargetMenAttack 1 remaining9
        let tmaPos = toPosition "Target Man (Attack)" (tmaPicks |> List.tryHead)

        { SweeperKeeper = skPos
          InvertedWingBackRight = iwbRPos
          InvertedWingBackLeft = iwbLPos
          BallPlayingDefs = bpdPositions
          WingerAttackRight = wgrPos
          InvertedWingerLeft = iwLPos
          BallWinningMidfielderSupport = bwmPos
          AdvancedPlaymakerSupport = apPos
          AdvancedForwardAttack = afaPos
          TargetManAttack = tmaPos }
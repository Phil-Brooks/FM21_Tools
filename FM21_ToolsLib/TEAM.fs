namespace FM21_ToolsLib

open System

module TEAM =

    // A simple container for a role assignment
    type Position =
        { RoleName: string
          PlayerName: string option
          Rating: float option
          // keep a reference to the Player record from HTML.fs (new Player shape)
          Player: HTML.Player option }

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

    // helper to convert (name * rating) option -> Position (no Player reference available)
    let private toPosition role (candidate: (string * float) option) =
        match candidate with
        | Some (n, r) -> { RoleName = role; PlayerName = Some n; Rating = Some r; Player = None }
        | None -> { RoleName = role; PlayerName = None; Rating = None; Player = None }

    // helper to convert (name * rating * player option) option -> Position (keeps Player reference)
    let private toPositionWithPlayer role (candidate: (string * float * HTML.Player option) option) =
        match candidate with
        | Some (n, r, pOpt) -> { RoleName = role; PlayerName = Some n; Rating = Some r; Player = pOpt }
        | None -> { RoleName = role; PlayerName = None; Rating = None; Player = None }

    // remove selected player names from the remaining pool
    let private removeSelected (remaining: HTML.Player list) (selectedNames: string list) =
        remaining |> List.filter (fun p -> not (List.contains p.Name selectedNames))

    // pick N best using a ROLE.* function that returns (string * float) list
    // returns picks annotated with the corresponding Player record (if present in the remaining pool)
    let private pickBestN (bestFn: HTML.Player list -> int -> (string * float) list) (count: int) (remaining: HTML.Player list) =
        let picks = bestFn remaining count
        // annotate each pick with the Player object from the pool (if found)
        let picksWithPlayer =
            picks
            |> List.map (fun (name, rating) ->
                let playerOpt = remaining |> List.tryFind (fun p -> p.Name = name)
                (name, rating, playerOpt))
        let names = picksWithPlayer |> List.map (fun (n,_,_) -> n)
        let remaining' = removeSelected remaining names
        picksWithPlayer, remaining'

    // Build a Team from a list of players. Picks greedily in a stable order and ensures uniqueness.
    let buildTeam (players: HTML.Player list) : Team =
        // start with full pool
        let remaining0 = players

        // 1 Sweeper Keeper
        let skPicks, remaining1 = pickBestN ROLE.bestSweeperKeepersDefend 1 remaining0
        let skPos = toPositionWithPlayer "Sweeper Keeper" (skPicks |> List.tryHead)

        // 1 Inverted Wing Back Right
        let iwbR, remaining2 = pickBestN ROLE.bestInvertedWingBacksSupportRight 1 remaining1
        let iwbRPos = toPositionWithPlayer "Inverted Wing Back (R)" (iwbR |> List.tryHead)

        // 1 Inverted Wing Back Left
        let iwbL, remaining3 = pickBestN ROLE.bestInvertedWingBacksSupportLeft 1 remaining2
        let iwbLPos = toPositionWithPlayer "Inverted Wing Back (L)" (iwbL |> List.tryHead)

        // 2 Ball Playing Defenders (CBs)
        let bpdPicks, remaining4 = pickBestN ROLE.bestBallPlayingDefenders 2 remaining3
        let bpdPositions =
            bpdPicks
            |> List.mapi (fun i (n, r, pOpt) -> toPositionWithPlayer (sprintf "Ball Playing Defender #%d" (i+1)) (Some (n, r, pOpt)))

        // Winger Attack Right
        let wgrPicks, remaining5 = pickBestN ROLE.bestWingersAttackRight 1 remaining4
        let wgrPos = toPositionWithPlayer "Winger (Attack) R" (wgrPicks |> List.tryHead)

        // Inverted Winger Left (attacking/support role)
        let iwL_Picks, remaining6 = pickBestN ROLE.bestInvertedWingersSupportLeft 1 remaining5
        let iwLPos = toPositionWithPlayer "Inverted Winger (L)" (iwL_Picks |> List.tryHead)

        // Ball Winning Midfielder (Support)
        let bwmPicks, remaining7 = pickBestN ROLE.bestBallWinningMidfieldersSupport 1 remaining6
        let bwmPos = toPositionWithPlayer "Ball Winning Midfielder (Support)" (bwmPicks |> List.tryHead)

        // Advanced Playmaker (Support)
        let apPicks, remaining8 = pickBestN ROLE.bestAdvancedPlaymakersSupport 1 remaining7
        let apPos = toPositionWithPlayer "Advanced Playmaker (Support)" (apPicks |> List.tryHead)

        // Advanced Forward (Attack)
        let afaPicks, remaining9 = pickBestN ROLE.bestAdvancedForwardsAttack 1 remaining8
        let afaPos = toPositionWithPlayer "Advanced Forward (Attack)" (afaPicks |> List.tryHead)

        // Target Man (Attack)
        let tmaPicks, _remaining10 = pickBestN ROLE.bestTargetMenAttack 1 remaining9
        let tmaPos = toPositionWithPlayer "Target Man (Attack)" (tmaPicks |> List.tryHead)

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
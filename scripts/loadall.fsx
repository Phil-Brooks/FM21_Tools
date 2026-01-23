#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let path = "../data/all.html"
let players = HTML.loadPlayers path

printfn "Loaded %d players from %s" (List.length players) path

let messi = players |> List.find (fun p -> p.Name = "Lionel Messi")

let tma =
    ROLE.roleRatingTargetManAttack messi
    |> Option.defaultWith (fun () -> failwith "No Target Man Attack rating for Lionel Messi")

let besttma = ROLE.bestTargetMenAttack players 20

let bestafa = ROLE.bestAdvancedForwardsAttack players 20

let bestwar = ROLE.bestWingersAttackRight players 20

let bestiwsl = ROLE.bestInvertedWingersSupportLeft players 20

let bestaps = ROLE.bestAdvancedPlaymakersSupport players 20

let bestbwms = ROLE.bestBallWinningMidfieldersSupport players 20

let bestbpd = ROLE.bestBallPlayingDefenders players 20

let bestiwbsr = ROLE.bestInvertedWingBacksSupportRight players 20

let bestiwbsl = ROLE.bestInvertedWingBacksSupportLeft players 20

let bestskd = ROLE.bestSweeperKeepersDefend players 20
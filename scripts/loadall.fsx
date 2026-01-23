#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let path = "../data/all.html"
let players = HTML.loadPlayers path

printfn "Loaded %d players from %s" (List.length players) path

//let messi = players |> List.find (fun p -> p.Name = "Lionel Messi")
//let tma =
//    ROLE.roleRatingTargetManAttack messi
//    |> Option.defaultWith (fun () -> failwith "No Target Man Attack rating for Lionel Messi")
//let besttma = ROLE.bestTargetMenAttack players 20
//let bestafa = ROLE.bestAdvancedForwardsAttack players 20
//let bestwar = ROLE.bestWingersAttackRight players 20
//let bestiwsl = ROLE.bestInvertedWingersSupportLeft players 20
//let bestaps = ROLE.bestAdvancedPlaymakersSupport players 20
//let bestbwms = ROLE.bestBallWinningMidfieldersSupport players 20
//let bestbpd = ROLE.bestBallPlayingDefenders players 20
//let bestiwbsr = ROLE.bestInvertedWingBacksSupportRight players 20
//let bestiwbsl = ROLE.bestInvertedWingBacksSupportLeft players 20
//let bestskd = ROLE.bestSweeperKeepersDefend players 20

let besttm = TEAM.buildTeam players
let bestnms = besttm |> TEAM.teamAsStrings
let bestscr = besttm |> TEAM.teamScore

//let clubs = CLUB.allClubs players
//let divs = CLUB.allDivisions players
//let engdivs = divs|>List.filter (fun d -> d.Contains("England"))

let div = "England (Sky Bet League Two)"
let clubs = CLUB.clubsInDivision div players
let club = clubs[0]
let clubPlayers = players|>List.filter (fun p -> p.Extras["Club"] = Some(club))
let clubbesttm = TEAM.buildTeam clubPlayers
let clubbestnms = clubbesttm |> TEAM.teamAsStrings
let clubbestscr = clubbesttm |> TEAM.teamScore

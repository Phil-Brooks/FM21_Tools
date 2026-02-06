#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let div = "England (Sky Bet Championship)"
let path = "../data/all.html"
do HTML.loadPlayers path

let mypath = "../data/my7.html"
HTML.loadMyPlayers mypath
printfn "Loaded %d players from %s" (List.length HTML.MyPlayers) path

let myteam = MY_CLUB.getFirstTeamAsStrings ()
let scr = MY_CLUB.getFirstTeamScore ()
let myteam2 = MY_CLUB.getSecondTeamAsStrings ()
let scr2 = MY_CLUB.getSecondTeamScore ()

let wka =  MY_CLUB.getFirstTeamWeakestAttributes()
let wka2 =  MY_CLUB.getSecondTeamWeakestAttributes()

let wkp = (MY_CLUB.getFirstTeamWeakestRelativeToDivision div).Value
let wkp2 = (MY_CLUB.getSecondTeamWeakestRelativeToDivision div).Value

let cmp = MY_CLUB.getFirstTeamComparisonToDivision div
let cmp2 = MY_CLUB.getSecondTeamComparisonToDivision div
let cmp3 = MY_CLUB.getThirdTeamComparisonToDivision div

//val cmp: string list =
//  ["SKD: Tim Krul -> player 69.34 vs avg 63.76 -> delta 5.58";
//   "IWBR: Max Aarons -> player 66.79 vs avg 61.74 -> delta 5.05";
//   "IWBL: Rúben Vinagre -> player 64.62 vs avg 62.29 -> delta 2.32";
//   "BPD1: Juanpe -> player 68.61 vs avg 65.04 -> delta 3.57";
//   "BPD2: Fausto Vera -> player 66.20 (no division avg)";
//   "WAR: Bruno Peres -> player 70.76 vs avg 67.57 -> delta 3.19";
//   "IWL: Thiago Almada -> player 69.18 vs avg 64.94 -> delta 4.24";
//   "BWM: Maxym Malyshev -> player 68.19 vs avg 66.19 -> delta 2.01";
//   "AP: Hatem Ben Arfa -> player 72.25 vs avg 65.49 -> delta 6.76";
//   "AFA: Teemu Pukki -> player 69.44 vs avg 64.37 -> delta 5.06";
//   "TMA: Adam Idah -> player 68.47 vs avg 63.25 -> delta 5.22";
//   "Team average: 68.53 vs Division average: 64.47 -> delta 4.30"]
//val cmp2: string list =
//  ["SKD: Jamie Cumming -> player 62.79 vs avg 63.76 -> delta -0.97";
//   "IWBR: Ashley Young -> player 61.67 vs avg 61.74 -> delta -0.08";
//   "IWBL: Kieran Gibbs -> player 63.27 vs avg 62.29 -> delta 0.98";
//   "BPD1: Christoph Zimmermann -> player 66.08 vs avg 65.04 -> delta 1.03";
//   "BPD2: Ben Gibson -> player 65.44 (no division avg)";
//   "WAR: Cristian Olivera -> player 70.15 vs avg 67.57 -> delta 2.58";
//   "IWL: Jon Toral -> player 68.66 vs avg 64.94 -> delta 3.71";
//   "BWM: Jacob Sørensen -> player 66.60 vs avg 66.19 -> delta 0.41";
//   "AP: Moritz Leitner -> player 70.92 vs avg 65.49 -> delta 5.43";
//   "AFA: Giacomo Raspadori -> player 68.10 vs avg 64.37 -> delta 3.73";
//   "TMA: Adam Hlozek -> player 66.39 vs avg 63.25 -> delta 3.14";
//   "Team average: 66.37 vs Division average: 64.47 -> delta 2.00"]
//val cmp3: string list =
//  ["IWBR: Bali Mumba -> player 55.51 vs avg 61.74 -> delta -6.23";
//   "IWBL: Lewis Shipley -> player 58.78 vs avg 62.29 -> delta -3.51";
//   "BPD1: Louis Lomas -> player 53.99 vs avg 65.04 -> delta -11.05";
//   "BPD2: Alex O'Neill -> player 52.47 (no division avg)";
//   "WAR: Onel Hernández -> player 68.94 vs avg 67.57 -> delta 1.37";
//   "IWL: Kieran Dowell -> player 68.58 vs avg 64.94 -> delta 3.64";
//   "BWM: Sol Hamilton -> player 58.19 vs avg 66.19 -> delta -7.99";
//   "AP: Marco Stiepermann -> player 67.46 vs avg 65.49 -> delta 1.97";
//   "AFA: Barrie McKay -> player 67.18 vs avg 64.37 -> delta 2.81";
//   "TMA: Jordan Hugill -> player 64.86 vs avg 63.25 -> delta 1.61";
//   "Team average: 61.60 vs Division average: 64.54 -> delta -1.93"]

#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let div = "England (Sky Bet Championship)"
let path = "../data/all.html"
do HTML.loadPlayers path

let mypath = "../data/my6.html"
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
//  ["SKD: Tim Krul -> player 67.53 vs avg 63.45 -> delta 4.08";
//   "IWBR: Max Aarons -> player 69.50 vs avg 61.59 -> delta 7.91";
//   "IWBL: Xavi Quintillà -> player 66.55 vs avg 61.95 -> delta 4.60";
//   "BPD1: Juanpe -> player 67.56 vs avg 64.41 -> delta 3.14";
//   "BPD2: Nathan Collins -> player 65.81 (no division avg)";
//   "WAR: Emiliano Buendía -> player 75.43 vs avg 67.33 -> delta 8.10";
//   "IWL: Thiago Almada -> player 71.28 vs avg 65.38 -> delta 5.91";
//   "BWM: Jacob Sørensen -> player 67.28 vs avg 66.11 -> delta 1.18";
//   "AP: Moritz Leitner -> player 71.01 vs avg 65.63 -> delta 5.38";
//   "AFA: Teemu Pukki -> player 70.50 vs avg 64.81 -> delta 5.69";
//   "TMA: Adam Idah -> player 68.38 vs avg 63.31 -> delta 5.07";
//   "Team average: 69.17 vs Division average: 64.40 -> delta 5.11"]
//val cmp2: string list =
//  ["SKD: Yoan Cardinale -> player 65.00 vs avg 63.45 -> delta 1.55";
//   "IWBR: Lukas Rupp -> player 63.95 vs avg 61.59 -> delta 2.36";
//   "IWBL: Sam Byram -> player 62.30 vs avg 61.95 -> delta 0.35";
//   "BPD1: Ben Gibson -> player 65.52 vs avg 64.41 -> delta 1.11";
//   "BPD2: Christoph Zimmermann -> player 63.66 (no division avg)";
//   "WAR: Onel Hernández -> player 68.86 vs avg 67.33 -> delta 1.53";
//   "IWL: Marco Stiepermann -> player 67.57 vs avg 65.38 -> delta 2.19";
//   "BWM: Louis Thompson -> player 65.37 vs avg 66.11 -> delta -0.74";
//   "AP: Kieran Dowell -> player 68.65 vs avg 65.63 -> delta 3.02";
//   "AFA: Josip Drmić -> player 65.00 vs avg 64.81 -> delta 0.19";
//   "TMA: Jordan Hugill -> player 65.00 vs avg 63.31 -> delta 1.69";
//   "Team average: 65.53 vs Division average: 64.40 -> delta 1.32"]
//val cmp3: string list =
//  ["IWBL: Sam McCallum -> player 55.65 vs avg 61.95 -> delta -6.30";
//   "WAR: Willem Geubbels -> player 64.71 vs avg 67.33 -> delta -2.61";
//   "IWL: Shane Hutchinson -> player 50.20 vs avg 65.38 -> delta -15.17";
//   "BWM: Sol Hamilton -> player 55.19 vs avg 66.11 -> delta -10.92";
//   "AFA: Oscar Thorn -> player 54.75 vs avg 64.81 -> delta -10.06";
//   "Team average: 56.10 vs Division average: 65.11 -> delta -9.01"]

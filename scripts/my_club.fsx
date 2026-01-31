#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let div = "England (Sky Bet Championship)"
let path = "../data/all.html"
do HTML.loadPlayers path

let mypath = "../data/my3.html"
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
//   "IWBR: Max Aarons -> player 70.85 vs avg 61.59 -> delta 9.26";
//   "IWBL: Youssouf Koné -> player 67.05 vs avg 61.95 -> delta 5.10";
//   "BPD1: Dan-Axel Zagadou -> player 70.70 vs avg 64.41 -> delta 6.28";
//   "BPD2: Nathan Collins -> player 67.09 (no division avg)";
//   "WAR: Emiliano Buendía -> player 75.86 vs avg 67.33 -> delta 8.53";
//   "IWL: Thiago Almada -> player 72.70 vs avg 65.38 -> delta 7.33";
//   "BWM: Mo Bešić -> player 72.84 vs avg 66.11 -> delta 6.73";
//   "AP: Maxime Lopez -> player 72.84 vs avg 65.63 -> delta 7.20";
//   "AFA: Pietro Pellegri -> player 68.75 vs avg 64.81 -> delta 3.94";
//   "TMA: Adam Idah -> player 68.78 vs avg 63.31 -> delta 5.47";
//   "Team average: 70.45 vs Division average: 64.40 -> delta 6.39"]
//val cmp2: string list =
//  ["SKD: Yoan Cardinale -> player 65.00 vs avg 63.45 -> delta 1.55";
//   "IWBR: Michał Karbownik -> player 65.30 vs avg 61.59 -> delta 3.71";
//   "IWBL: Xavi Quintillà -> player 66.55 vs avg 61.95 -> delta 4.60";
//   "BPD1: Luis Binks -> player 66.80 vs avg 64.41 -> delta 2.39";
//   "BPD2: Jacob Sørensen -> player 65.41 (no division avg)";
//   "WAR: Onel Hernández -> player 68.29 vs avg 67.33 -> delta 0.96";
//   "IWL: Janis Antiste -> player 67.91 vs avg 65.38 -> delta 2.53";
//   "BWM: James McCarthy -> player 70.06 vs avg 66.11 -> delta 3.96";
//   "AP: Rekeem Harper -> player 72.03 vs avg 65.63 -> delta 6.39";
//   "AFA: Leonardo Campana -> player 66.13 vs avg 64.81 -> delta 1.32";
//   "TMA: Willem Geubbels -> player 63.78 vs avg 63.31 -> delta 0.47";
//   "Team average: 67.02 vs Division average: 64.40 -> delta 2.79"]
//val cmp3: string list =
//  ["IWBR: Colin Dagba -> player 64.70 vs avg 61.59 -> delta 3.11";
//   "IWBL: Adrien Truffert -> player 62.35 vs avg 61.95 -> delta 0.40";
//   "BPD1: Ben Gibson -> player 65.35 vs avg 64.41 -> delta 0.94";
//   "BPD2: Christoph Zimmermann -> player 63.66 (no division avg)";
//   "WAR: Aidan Fitzpatrick -> player 66.14 vs avg 67.33 -> delta -1.18";
//   "IWL: Kieran Dowell -> player 67.09 vs avg 65.38 -> delta 1.72";
//   "BWM: Louis Thompson -> player 65.37 vs avg 66.11 -> delta -0.74";
//   "AP: Moritz Leitner -> player 70.61 vs avg 65.63 -> delta 4.97";
//   "AFA: Oscar Thorn -> player 56.75 vs avg 64.81 -> delta -8.06";
//   "TMA: Shane Hutchinson -> player 47.97 vs avg 63.31 -> delta -15.34";
//   "Team average: 63.00 vs Division average: 64.50 -> delta -1.58"]

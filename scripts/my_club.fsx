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
//  ["SKD: Tim Krul -> player 67.87 vs avg 63.76 -> delta 4.11";
//   "IWBR: Max Aarons -> player 69.49 vs avg 61.74 -> delta 7.74";
//   "IWBL: Xavi Quintillà -> player 68.91 vs avg 62.29 -> delta 6.62";
//   "BPD1: Dan-Axel Zagadou -> player 73.04 vs avg 65.04 -> delta 8.00";
//   "BPD2: Luis Binks -> player 68.23 (no division avg)";
//   "WAR: Emiliano Buendía -> player 76.97 vs avg 67.57 -> delta 9.40";
//   "IWL: Thiago Almada -> player 73.88 vs avg 64.94 -> delta 8.94";
//   "BWM: Mo Bešić -> player 72.36 vs avg 66.19 -> delta 6.17";
//   "AP: Rekeem Harper -> player 73.38 vs avg 65.49 -> delta 7.89";
//   "AFA: Janis Antiste -> player 72.11 vs avg 64.37 -> delta 7.74";
//   "TMA: Adam Idah -> player 68.75 vs avg 63.25 -> delta 5.50";
//   "Team average: 71.36 vs Division average: 64.47 -> delta 7.21"]
//val cmp2: string list =
//  ["SKD: Yoan Cardinale -> player 64.78 vs avg 63.76 -> delta 1.02";
//   "IWBR: Nathan Collins -> player 61.54 vs avg 61.74 -> delta -0.21";
//   "IWBL: Youssouf Koné -> player 66.60 vs avg 62.29 -> delta 4.31";
//   "BPD1: Sivert Mannsverk -> player 66.84 vs avg 65.04 -> delta 1.79";
//   "BPD2: Jacob Sørensen -> player 66.46 (no division avg)";
//   "WAR: Aidan Fitzpatrick -> player 69.77 vs avg 67.57 -> delta 2.20";
//   "IWL: Kieran Dowell -> player 67.76 vs avg 64.94 -> delta 2.82";
//   "BWM: James McCarthy -> player 68.89 vs avg 66.19 -> delta 2.70";
//   "AP: Maxime Lopez -> player 73.31 vs avg 65.49 -> delta 7.82";
//   "AFA: Pietro Pellegri -> player 66.76 vs avg 64.37 -> delta 2.39";
//   "TMA: Willem Geubbels -> player 64.58 vs avg 63.25 -> delta 1.33";
//   "Team average: 67.03 vs Division average: 64.47 -> delta 2.62"]
//val cmp3: string list =
//  ["IWBR: Sam Byram -> player 61.28 vs avg 61.74 -> delta -0.46";
//   "IWBL: Adrien Truffert -> player 65.77 vs avg 62.29 -> delta 3.48";
//   "IWL: Shane Hutchinson -> player 52.84 vs avg 64.94 -> delta -12.11";
//   "BWM: Sol Hamilton -> player 58.75 vs avg 66.19 -> delta -7.44";
//   "AP: Spencer Ginty -> player 53.17 vs avg 65.49 -> delta -12.32";
//   "AFA: Oscar Thorn -> player 59.08 vs avg 64.37 -> delta -5.29";
//   "Team average: 58.48 vs Division average: 64.17 -> delta -5.69"]

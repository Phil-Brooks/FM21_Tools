#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let div = "England (Sky Bet Championship)"
let path = "../data/all.html"
do HTML.loadPlayers path

let mypath = "../data/my1.html"
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
//  ["SKD: Tim Krul -> player 67.27 vs avg 63.45 -> delta 3.82";
//   "IWBR: Max Aarons -> player 69.50 vs avg 61.59 -> delta 7.91";
//   "IWBL: Xavi Quintillà -> player 66.55 vs avg 61.95 -> delta 4.60";
//   "BPD1: Mo Bešić -> player 66.34 vs avg 64.41 -> delta 1.92";
//   "BPD2: Nathan Collins -> player 65.99 (no division avg)";
//   "WAR: Emiliano Buendía -> player 75.43 vs avg 67.33 -> delta 8.10";
//   "IWL: Thiago Almada -> player 70.74 vs avg 65.38 -> delta 5.37";
//   "BWM: Jacob Sørensen -> player 67.28 vs avg 66.11 -> delta 1.18";
//   "AP: Moritz Leitner -> player 71.01 vs avg 65.63 -> delta 5.38";
//   "AFA: Teemu Pukki -> player 70.75 vs avg 64.81 -> delta 5.94";
//   "TMA: Adam Idah -> player 68.38 vs avg 63.31 -> delta 5.07";
//   "Team average: 69.02 vs Division average: 64.40 -> delta 4.93"]
//val cmp2: string list =
//  ["SKD: Yoan Cardinale -> player 65.00 vs avg 63.45 -> delta 1.55";
//   "IWBR: Colin Dagba -> player 64.70 vs avg 61.59 -> delta 3.11";
//   "IWBL: Michał Karbownik -> player 64.55 vs avg 61.95 -> delta 2.60";
//   "BPD1: Ben Gibson -> player 65.35 vs avg 64.41 -> delta 0.94";
//   "BPD2: Christoph Zimmermann -> player 63.66 (no division avg)";
//   "WAR: Onel Hernández -> player 68.86 vs avg 67.33 -> delta 1.53";
//   "IWL: Kieran Dowell -> player 67.50 vs avg 65.38 -> delta 2.12";
//   "BWM: Louis Thompson -> player 65.37 vs avg 66.11 -> delta -0.74";
//   "AP: Marco Stiepermann -> player 68.04 vs avg 65.63 -> delta 2.41";
//   "AFA: Janis Antiste -> player 66.88 vs avg 64.81 -> delta 2.07";
//   "TMA: Leonardo Campana -> player 67.57 vs avg 63.31 -> delta 4.26";
//   "Team average: 66.13 vs Division average: 64.40 -> delta 1.98"]
//val cmp3: string list =
//  ["IWBR: Sam Byram -> player 62.30 vs avg 61.59 -> delta 0.71";
//   "IWBL: Adrien Truffert -> player 61.30 vs avg 61.95 -> delta -0.65";
//   "BPD1: Luca Connell -> player 59.65 vs avg 64.41 -> delta -4.76";
//   "WAR: Willem Geubbels -> player 65.43 vs avg 67.33 -> delta -1.90";
//   "IWL: Shane Hutchinson -> player 50.20 vs avg 65.38 -> delta -15.17";
//   "BWM: Sol Hamilton -> player 56.54 vs avg 66.11 -> delta -9.56";
//   "AFA: Joshua Zirkzee -> player 65.63 vs avg 64.81 -> delta 0.82";
//   "TMA: Oscar Thorn -> player 57.30 vs avg 63.31 -> delta -6.01";
//   "Team average: 59.79 vs Division average: 64.36 -> delta -4.57"]

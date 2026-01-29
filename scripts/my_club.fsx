#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let div = "England (Sky Bet Championship)"
let path = "../data/all.html"
do HTML.loadPlayers path

let mypath = "../data/my4.html"
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
//  ["SKD: Tim Krul -> player 67.79 vs avg 63.45 -> delta 4.34";
//   "IWBR: Max Aarons -> player 68.25 vs avg 61.59 -> delta 6.66";
//   "IWBL: Xavi Quintillà -> player 66.55 vs avg 61.95 -> delta 4.60";
//   "BPD1: Juanpe -> player 67.56 vs avg 64.41 -> delta 3.14";
//   "BPD2: Ben Gibson -> player 65.52 (no division avg)";
//   "WAR: Emiliano Buendía -> player 75.43 vs avg 67.33 -> delta 8.10";
//   "IWL: Thiago Almada -> player 69.12 vs avg 65.38 -> delta 3.74";
//   "BWM: Alexander Tettey -> player 67.90 vs avg 66.11 -> delta 1.80";
//   "AP: Moritz Leitner -> player 71.01 vs avg 65.63 -> delta 5.38";
//   "AFA: Teemu Pukki -> player 70.50 vs avg 64.81 -> delta 5.69";
//   "TMA: Adam Idah -> player 68.65 vs avg 63.31 -> delta 5.34";
//   "Team average: 68.94 vs Division average: 64.40 -> delta 4.88"]
//val cmp2: string list =
//  ["SKD: Yoan Cardinale -> player 65.00 vs avg 63.45 -> delta 1.55";
//   "IWBR: Lukas Rupp -> player 63.95 vs avg 61.59 -> delta 2.36";
//   "IWBL: Dimitris Giannoulis -> player 64.05 vs avg 61.95 -> delta 2.10";
//   "BPD1: Jacob Sørensen -> player 65.41 vs avg 64.41 -> delta 0.99";
//   "BPD2: Christoph Zimmermann -> player 63.66 (no division avg)";
//   "WAR: Onel Hernández -> player 68.43 vs avg 67.33 -> delta 1.10";
//   "IWL: Marco Stiepermann -> player 67.57 vs avg 65.38 -> delta 2.19";
//   "BWM: Oliver Skipp -> player 67.35 vs avg 66.11 -> delta 1.24";
//   "AP: Kieran Dowell -> player 68.18 vs avg 65.63 -> delta 2.54";
//   "AFA: Fiete Arp -> player 67.63 vs avg 64.81 -> delta 2.82";
//   "TMA: Willem Geubbels -> player 65.54 vs avg 63.31 -> delta 2.23";
//   "Team average: 66.07 vs Division average: 64.40 -> delta 1.91"]
//val cmp3: string list =
//  ["SKD: Aston Oxborough -> player 51.56 vs avg 63.45 -> delta -11.89";
//   "IWBR: Sam Byram -> player 62.70 vs avg 61.59 -> delta 1.11";
//   "IWBL: Philip Heise -> player 58.25 vs avg 61.95 -> delta -3.70";
//   "BPD1: Tom Trybull -> player 60.81 vs avg 64.41 -> delta -3.60";
//   "BPD2: Timm Klose -> player 60.70 (no division avg)";
//   "WAR: Aidan Fitzpatrick -> player 64.14 vs avg 67.33 -> delta -3.18";
//   "IWL: Josip Drmić -> player 63.31 vs avg 65.38 -> delta -2.07";
//   "BWM: Louis Thompson -> player 65.37 vs avg 66.11 -> delta -0.74";
//   "AP: Danel Sinani -> player 59.12 vs avg 65.63 -> delta -6.51";
//   "AFA: Jordan Hugill -> player 62.25 vs avg 64.81 -> delta -2.56";
//   "TMA: Joe Duffy -> player 54.86 vs avg 63.31 -> delta -8.45";
//   "Team average: 60.28 vs Division average: 64.40 -> delta -4.16"]

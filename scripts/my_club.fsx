#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let div = "England (Sky Bet Championship)"
let path = "../data/all.html"
do HTML.loadPlayers path

let mypath = "../data/my2.html"
HTML.loadMyPlayers mypath
printfn "Loaded %d players from %s" (List.length HTML.MyPlayers) path

let fst = MY_CLUB.getFirstTeam ()

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
//   "IWBR: Max Aarons -> player 67.65 vs avg 61.59 -> delta 6.06";
//   "IWBL: Xavi Quintillà -> player 66.55 vs avg 61.95 -> delta 4.60";
//   "BPD1: Juanpe -> player 67.56 vs avg 64.41 -> delta 3.14";
//   "BPD2: Jacob Sørensen -> player 65.41 vs avg 62.53 -> delta 2.88";
//   "WAR: Emiliano Buendía -> player 75.43 vs avg 67.33 -> delta 8.10";
//   "IWL: Thiago Almada -> player 69.12 vs avg 65.38 -> delta 3.74";
//   "BWM: Alexander Tettey -> player 68.33 vs avg 66.11 -> delta 2.23";
//   "AP: Moritz Leitner -> player 71.01 vs avg 65.63 -> delta 5.38";
//   "AFA: Teemu Pukki -> player 70.50 vs avg 64.81 -> delta 5.69";
//   "TMA: Adam Idah -> player 66.22 vs avg 63.31 -> delta 2.91";
//   "Team average: 68.69 vs Division average: 64.23 -> delta 4.46"]
//val cmp2: string list =
//  ["SKD: Ørjan Nyland -> player 61.56 vs avg 63.45 -> delta -1.89";
//   "IWBR: Sam Byram -> player 64.50 vs avg 61.59 -> delta 2.91";
//   "IWBL: Dimitris Giannoulis -> player 64.40 vs avg 61.95 -> delta 2.45";
//   "BPD1: Ben Gibson -> player 65.35 vs avg 64.41 -> delta 0.94";
//   "BPD2: Christoph Zimmermann -> player 63.66 vs avg 62.53 -> delta 1.13";
//   "WAR: Onel Hernández -> player 68.43 vs avg 67.33 -> delta 1.10";
//   "IWL: Marco Stiepermann -> player 67.57 vs avg 65.38 -> delta 2.19";
//   "BWM: Oliver Skipp -> player 67.35 vs avg 66.11 -> delta 1.24";
//   "AP: Kieran Dowell -> player 68.18 vs avg 65.63 -> delta 2.54";
//   "AFA: Fiete Arp -> player 67.63 vs avg 64.81 -> delta 2.82";
//   "TMA: Willem Geubbels -> player 65.54 vs avg 63.31 -> delta 2.23";
//   "Team average: 65.83 vs Division average: 64.23 -> delta 1.60"]
//val cmp3: string list =
//  ["SKD: Aston Oxborough -> player 51.69 vs avg 63.45 -> delta -11.76";
//   "IWBR: Lukas Rupp -> player 64.35 vs avg 61.59 -> delta 2.76";
//   "IWBL: Philip Heise -> player 58.25 vs avg 61.95 -> delta -3.70";
//   "BPD1: Timm Klose -> player 60.87 vs avg 64.41 -> delta -3.54";
//   "BPD2: Tom Trybull -> player 60.81 vs avg 62.53 -> delta -1.72";
//   "WAR: Aidan Fitzpatrick -> player 63.14 vs avg 67.33 -> delta -4.18";
//   "IWL: Josip Drmi? -> player 62.97 vs avg 65.38 -> delta -2.40";
//   "BWM: Louis Thompson -> player 65.37 vs avg 66.11 -> delta -0.74";
//   "AP: Danel Sinani -> player 59.12 vs avg 65.63 -> delta -6.51";
//   "AFA: Jordan Hugill -> player 62.25 vs avg 64.81 -> delta -2.56";
//   "TMA: Sebastian Soto -> player 58.11 vs avg 63.31 -> delta -5.20";
//   "Team average: 60.63 vs Division average: 64.23 -> delta -3.60"]

let rrps = 
    HTML.MyPlayers|>List.map ROLE.bestRoleRatedPlayer
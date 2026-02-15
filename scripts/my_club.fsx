#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

//let div = "England (Sky Bet Championship)"
let div = "Holland (Eredivisie)"
let path = @"D:\Github\FM21_Tools\data\all.html"
do HTML.loadPlayers path

let mypath = @"D:\Github\FM21_Tools\data\my9.html"
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
//  ["SKD: Alexander Schlager -> player 69.78 vs avg 63.35 -> delta 6.43";
//   "IWBR: Lutsharel Geertruida -> player 62.88 vs avg 60.92 -> delta 1.97";
//   "IWBL: Tyrell Malacia -> player 67.69 vs avg 61.89 -> delta 5.80";
//   "BPD1: Jesus -> player 67.03 vs avg 63.66 -> delta 3.37";
//   "BPD2: Fabian Schär -> player 66.90 (no division avg)";
//   "WAR: Luis Sinisterra -> player 71.21 vs avg 69.28 -> delta 1.93";
//   "IWL: Gio-Renys Felicia -> player 67.01 vs avg 64.59 -> delta 2.43";
//   "BWM: Aliou Traoré -> player 63.89 vs avg 61.86 -> delta 2.03";
//   "AP: Ömer Beyaz -> player 69.01 vs avg 65.61 -> delta 3.40";
//   "AFA: Adam Hlozek -> player 70.35 vs avg 63.69 -> delta 6.67";
//   "TMA: Rafa Mir -> player 69.17 vs avg 61.59 -> delta 7.58";
//   "Team average: 67.72 vs Division average: 63.64 -> delta 4.16"]
//val cmp2: string list =
//  ["SKD: Lucas Cañizares -> player 62.35 vs avg 63.35 -> delta -1.00";
//   "IWBR: Mimeirhel Benita -> player 56.15 vs avg 60.92 -> delta -4.76";
//   "IWBL: Sean Roughan -> player 61.54 vs avg 61.89 -> delta -0.36";
//   "BPD1: Lukas Mai -> player 64.18 vs avg 63.66 -> delta 0.52";
//   "BPD2: Luca Palmiero -> player 57.66 (no division avg)";
//   "WAR: Cristian Olivera -> player 68.26 vs avg 69.28 -> delta -1.02";
//   "IWL: Shiloh 't Zand -> player 66.12 vs avg 64.59 -> delta 1.53";
//   "BWM: Xian Emmers -> player 57.85 vs avg 61.86 -> delta -4.02";
//   "AP: Mark Diemers -> player 67.39 vs avg 65.61 -> delta 1.78";
//   "AFA: Folarin Balogun -> player 65.49 vs avg 63.69 -> delta 1.81";
//   "TMA: Bradley Fink -> player 61.81 vs avg 61.59 -> delta 0.22";
//   "Team average: 62.62 vs Division average: 63.64 -> delta -0.53"]
//val cmp3: string list =
//  ["SKD: Tein Troost -> player 59.04 vs avg 63.35 -> delta -4.31";
//   "IWBL: Callum Doyle -> player 51.79 vs avg 61.89 -> delta -10.10";
//   "BPD1: Wouter Burger -> player 57.59 vs avg 63.66 -> delta -6.06";
//   "BPD2: Morgan Zouan -> player 54.68 (no division avg)";
//   "WAR: Christian Conteh -> player 66.06 vs avg 69.28 -> delta -3.22";
//   "IWL: Nicola Dalmonte -> player 64.70 vs avg 64.59 -> delta 0.12";
//   "BWM: Lars de Blok -> player 57.22 vs avg 61.86 -> delta -4.64";
//   "AP: Orkun Kökçü -> player 67.32 vs avg 65.61 -> delta 1.71";
//   "AFA: Peque -> player 64.23 vs avg 63.69 -> delta 0.54";
//   "TMA: Ilyas el Moussaoui -> player 59.58 vs avg 61.59 -> delta -2.01";
//   "Team average: 60.22 vs Division average: 63.95 -> delta -3.11"]

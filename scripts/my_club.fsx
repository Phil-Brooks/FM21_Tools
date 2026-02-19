#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

//let div = "England (Sky Bet Championship)"
let div = "Holland (Eredivisie)"
let path = @"D:\Github\FM21_Tools\data\all.html"
do HTML.loadPlayers path

let mypath = @"D:\Github\FM21_Tools\data\my.html"
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
//  ["SKD: Matheus -> player 70.29 vs avg 63.35 -> delta 6.94";
//   "IWBR: Mateu Morey -> player 71.99 vs avg 60.84 -> delta 11.15";
//   "IWBL: Ronaël Pierre-Gabriel -> player 67.44 vs avg 61.82 -> delta 5.62";
//   "BPD1: Krystian Bielik -> player 74.87 vs avg 63.21 -> delta 11.66";
//   "BPD2: Dan-Axel Zagadou -> player 74.24 (no division avg)";
//   "WAR: Cristian Olivera -> player 71.74 vs avg 69.28 -> delta 2.46";
//   "IWL: Matheusinho -> player 71.57 vs avg 64.59 -> delta 6.98";
//   "BWM: Santiago Ascacíbar -> player 73.68 vs avg 63.78 -> delta 9.90";
//   "AP: Hannibal -> player 74.58 vs avg 65.50 -> delta 9.07";
//   "AFA: Juanjo Echeverría -> player 74.58 vs avg 63.95 -> delta 10.63";
//   "TMA: Rafa Mir -> player 71.81 vs avg 61.60 -> delta 10.21";
//   "Team average: 72.43 vs Division average: 63.79 -> delta 8.46"]
//val cmp2: string list =
//  ["SKD: Alexander Schlager -> player 69.85 vs avg 63.35 -> delta 6.50";
//   "IWBR: José Luis Pasquali -> player 66.67 vs avg 60.84 -> delta 5.83";
//   "IWBL: Youssouf Koné -> player 66.67 vs avg 61.82 -> delta 4.85";
//   "BPD1: Jean-Clair Todibo -> player 72.34 vs avg 63.21 -> delta 9.13";
//   "BPD2: Robert Piris Da Motta -> player 60.63 (no division avg)";
//   "WAR: Gio-Renys Felicia -> player 71.67 vs avg 69.28 -> delta 2.39";
//   "IWL: Shiloh 't Zand -> player 71.34 vs avg 64.59 -> delta 6.76";
//   "BWM: Orkun Kökçü -> player 52.71 vs avg 63.78 -> delta -11.07";
//   "AP: Ömer Beyaz -> player 72.32 vs avg 65.50 -> delta 6.82";
//   "AFA: Borja Garcés -> player 73.94 vs avg 63.95 -> delta 9.99";
//   "TMA: Ljubomir Denkovski -> player 66.94 vs avg 61.60 -> delta 5.35";
//   "Team average: 67.74 vs Division average: 63.79 -> delta 4.65"]
//val cmp3: string list =
//  ["IWBL: Alberto Moreno -> player 66.09 vs avg 61.82 -> delta 4.27";
//   "WAR: Folarin Balogun -> player 66.14 vs avg 69.28 -> delta -3.14";
//   "Team average: 66.11 vs Division average: 65.55 -> delta 0.56"]

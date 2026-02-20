#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

//let div = "England (Sky Bet Championship)"
let div = "Holland (Eredivisie)"
let path = @"D:\Github\FM21_Tools\data\all.html"
do HTML.loadPlayers path

let mypath = @"D:\Github\FM21_Tools\data\my.html"
HTML.loadMyPlayers mypath
printfn "Loaded %d players from %s" (List.length HTML.MyPlayers) path

let myteam = MY_CLUB.getFirstZAZTeamAsStrings ()
let scr = MY_CLUB.getFirstZAZTeamScore ()
let myteam2 = MY_CLUB.getSecondZAZTeamAsStrings ()
let scr2 = MY_CLUB.getSecondZAZTeamScore ()

let wka =  MY_CLUB.getFirstZAZTeamWeakestAttributes()
let wka2 =  MY_CLUB.getSecondZAZTeamWeakestAttributes()

let wkp = (MY_CLUB.getFirstZAZTeamWeakestRelativeToDivision div).Value
let wkp2 = (MY_CLUB.getSecondZAZTeamWeakestRelativeToDivision div).Value

let cmp = MY_CLUB.getFirstZAZTeamComparisonToDivision div
let cmp2 = MY_CLUB.getSecondZAZTeamComparisonToDivision div
let cmp3 = MY_CLUB.getThirdZAZTeamComparisonToDivision div

//val cmp: string list =
//  ["SKS: Matheus -> player 69.47 (no division avg)";
//   "IWBR: Mateu Morey -> player 71.99 vs avg 60.84 -> delta 11.15";
//   "IWBL: Ronaël Pierre-Gabriel -> player 67.76 vs avg 61.82 -> delta 5.94";
//   "BPD1: Krystian Bielik -> player 73.92 vs avg 63.21 -> delta 10.71";
//   "BPD2: José Luis Pasquali -> player 72.97 (no division avg)";
//   "MR: Mario Weber -> player 67.12 (no division avg)";
//   "ML: Achim Schmedes -> player 67.88 (no division avg)";
//   "DLP: Billy Gilmour -> player 76.47 (no division avg)";
//   "MCA: Hannibal -> player 75.14 (no division avg)";
//   "AMSS1: Juanjo Echeverría -> player 73.00 (no division avg)";
//   "AMSS2: Shiloh 't Zand -> player 71.09 (no division avg)";
//   "Team average: 71.53 vs Division average: 61.96 -> delta 9.27"]
//val cmp2: string list =
//  ["SKS: Alexander Schlager -> player 68.49 (no division avg)";
//   "IWBR: Cristinel Vaida -> player 68.27 vs avg 60.84 -> delta 7.43";
//   "IWBL: Youssouf Koné -> player 67.37 vs avg 61.82 -> delta 5.55";
//   "BPD1: Jean-Clair Todibo -> player 72.34 vs avg 63.21 -> delta 9.13";
//   "BPD2: Issa Diop -> player 68.67 (no division avg)";
//   "MR: Matheusinho -> player 61.44 (no division avg)";
//   "ML: Alberto Moreno -> player 66.30 (no division avg)";
//   "DLP: Claudio Rizzardi -> player 76.37 (no division avg)";
//   "MCA: Orkun Kökçü -> player 70.63 (no division avg)";
//   "AMSS1: Ömer Beyaz -> player 68.64 (no division avg)";
//   "Team average: 68.85 vs Division average: 61.96 -> delta 7.37"]
//val cmp3: string list =
//  ["IWBL: Pietro Celeste -> player 61.41 vs avg 61.82 -> delta -0.41";
//   "Team average: 61.41 vs Division average: 61.82 -> delta -0.41"]

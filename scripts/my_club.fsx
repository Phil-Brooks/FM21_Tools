#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let div = "England (Sky Bet Championship)"
let path = "../data/all.html"
do HTML.loadPlayers path

let mypath = "../data/my.html"
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


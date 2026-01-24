#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let path = "../data/my.html"
HTML.loadMyPlayers path
printfn "Loaded %d players from %s" (List.length HTML.MyPlayers) path

let myteam = MY_CLUB.getFirstTeamAsStrings ()
let scr = MY_CLUB.getFirstTeamScore ()
let myteam2 = MY_CLUB.getSecondTeamAsStrings ()
let scr2 = MY_CLUB.getSecondTeamScore ()

let wka =  MY_CLUB.getFirstTeamWeakestAttributes()
let wka2 =  MY_CLUB.getSecondTeamWeakestAttributes()

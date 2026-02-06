#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let path = "../data/my3.html"
do PROGRESS.loadOldPlayers path

let mypath = "../data/my5.html"
HTML.loadMyPlayers mypath
printfn "Loaded %d players from %s" (List.length HTML.MyPlayers) path

let pclub = PROGRESS.progressClub()


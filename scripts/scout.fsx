#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let sctpath = "../data/sct4.html"
HTML.loadSctPlayers sctpath

//let path = "../data/all.html"
//do PROGRESS.loadOldPlayers path

let bestbwms = SCOUT.getBest "BWM" 74.0 3000
let tlbwms = SCOUT.getTrLst "BWM" 67.0 3000
let llbwms = SCOUT.getLnLst "BWM" 63.0 3000
let yngbwms = SCOUT.getYng "BWM" 64.0 3000 20

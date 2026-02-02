open System

#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let sctpath = "../data/sct3.html"
HTML.loadSctPlayers sctpath

//let path = "../data/all.html"
//do PROGRESS.loadOldPlayers path

SCOUT.today = DateTime(2022, 3, 31)

let yngs = SCOUT.getYng "SKD" 63 4000 20

//SKD
//IWBR
//IWBL
//BPD
//WAR
//IWL
//BWM
//AP
//AFA
//TMA


let bests = SCOUT.getBest "BWM" 67.7 17000
let lls = SCOUT.getLnLst "AFA" 62.0 13000

let tls = SCOUT.getTrLst "IWBR" 65.0 19000


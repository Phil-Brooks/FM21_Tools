open System

#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let sctpath = "../data/sct.html"
HTML.loadSctPlayers sctpath

//let path = "../data/all.html"
//do PROGRESS.loadOldPlayers path

today <- DateTime(2024, 3, 31)

let yngs = SCOUT.getYng "BPD" 70 29000 32

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


let bests = SCOUT.getBest "AFA" 62 1



let lls = SCOUT.getLnLst "AFA" 62.0 29000

let tls = SCOUT.getTrLst "BPD" 64.0 19000


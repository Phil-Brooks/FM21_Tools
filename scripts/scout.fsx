open System

#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let sctpath = "../data/sct3.html"
HTML.loadSctPlayers sctpath

//let path = "../data/all.html"
//do PROGRESS.loadOldPlayers path

today = DateTime(2023, 3, 31)

let yngs = SCOUT.getYng "AFA" 61 9000 21

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



let lls = SCOUT.getLnLst "IWBL" 64.0 23000

let tls = SCOUT.getTrLst "WAR" 69.0 19000


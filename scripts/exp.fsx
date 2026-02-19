open System

#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let sctpath = "../data/exp.html"
HTML.loadSctPlayers sctpath

//let path = "../data/all.html"
//do PROGRESS.loadOldPlayers path

today <- DateTime(2022, 3, 31)

let yngs = SCOUT.getYng "BPD" 60 29000 30

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

let tls = SCOUT.getTrLst "IWBL" 66.0 29000


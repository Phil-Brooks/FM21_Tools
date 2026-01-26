#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let sctpath = "../data/sct3.html"
HTML.loadSctPlayers sctpath

// try to replace BPD as first team has this  weak one:
//"BPD1: Jacob Sørensen -> player 65.41 vs avg 64.41 -> delta 0.99";

//try to get a loanee with BPD > 65.41
let llbpds = 
    //get all players better
    let bpds = SCOUT.getSctPlayersForRoleAbove "BPD" 65.0
    bpds |> List.filter SCOUT.roleRatedPlayerLoanListed
    //NONE

//try to get a listed player with BPD > 65.41
let tlbpds = 
    //get all players better
    let bpds = SCOUT.getSctPlayersForRoleAbove "BPD" 65.0
    bpds |> List.filter SCOUT.roleRatedPlayerTransferListed
    //only two and really DMs

//try to get a cheap player with BPD > 65.41
let chpbpds = 
    //get all players better
    let bpds = SCOUT.getSctPlayersForRoleAbove "BPD" 67.0
    bpds 
    |> List.filter (SCOUT.roleRatedPlayerValueBelowK 3000)
    |> List.filter (SCOUT.roleRatedPlayerAgeBelow 30)
    |> List.map SCOUT.rRPlayerReport
    //found 4
    // NOT INTERESTED "Marc-Oliver Kempf", "VfB Stuttgart", 67.90697674; "Paulo Díaz", "River", 67.84883721;"Jordan Torunarigha", "Hertha BSC", 67.3255814
   // OFFER 1.9m "Juanpe", "Girona", 67.55813953

//   "BWM: Alexander Tettey -> player 68.70 vs avg 66.11 -> delta 2.60"; FIX
//try to get a loanee with BWM > 69.0
let llbwms = 
    //get all players better
    let bwms = SCOUT.getSctPlayersForRoleAbove "BWM" 69.0
    bwms |> List.filter SCOUT.roleRatedPlayerLoanListed
    //NONE
//try to get a listed player with BWM > 69.0
let tlbwms = 
    //get all players better
    let bwms = SCOUT.getSctPlayersForRoleAbove "BWM" 69.0
    bwms |> List.filter SCOUT.roleRatedPlayerTransferListed
    |> List.map SCOUT.rRPlayerReport
    //only two 
    //[("James McCarthy", "Crystal Palace", 71.17283951) - too expensive;
   //("Mohamed Elneny", "Arsenal", 70.92592593)] - too expensive
//try to get a cheap player with BWM > 69.0
let chpbwms = 
    //get all players better
    let bwms = SCOUT.getSctPlayersForRoleAbove "BWM" 69.0
    bwms 
    |> List.filter (SCOUT.roleRatedPlayerValueBelowK 3000)
    |> List.filter (SCOUT.roleRatedPlayerAgeBelow 30)
    |> List.map SCOUT.rRPlayerReport
    //found 10
    //NOT INTERESTED "Jacek Góralski", "Qairat", 70.43209877;"Dominik Kohr", "Mainz 05", 70.43209877;"Robert Piris Da Motta", "Gençlerbirliği", 70.18518519;"Rani Khedira", "FC Augsburg", 69.32098765);
    // NO WP - "André Poko", "Altay", 70.37037037;"Anderson Esiti", "Göztepe", 70.0;"Saša Zdjelar", "Partizan", 69.19753086;"Iddrisu Baba", "Mallorca", 69.01234568
   // OFFER 3.5m "Alpaslan Öztürk", "Göztepe", 70.30864198;
   // OFFER 1.3m "Dan Glazer", "Maccabi Tel-Aviv", 69.9382716;

//   "TMA: Adam Idah -> player 66.22 vs avg 63.31 -> delta 2.91"; FIX
//try to get a loanee with TMA > 70.0
let lltmas = 
    //get all players better
    let tmas = SCOUT.getSctPlayersForRoleAbove "AFA" 67.0
    tmas |> List.filter SCOUT.roleRatedPlayerLoanListed
    //NONE
//try to get a listed player with TMAM > 68.0
let tltmas = 
    //get all players better
    let tmas = SCOUT.getSctPlayersForRoleAbove "AFA" 68.0
    tmas |> List.filter SCOUT.roleRatedPlayerTransferListed
    |> List.map SCOUT.rRPlayerReport
    //NONE
//try to get a cheap player with AFA > 68.0
let chptmas = 
    //get all players better
    let tmas = SCOUT.getSctPlayersForRoleAbove "AFA" 67.0
    tmas 
    |> List.filter (SCOUT.roleRatedPlayerValueBelowK 9000)
    |> List.filter (SCOUT.roleRatedPlayerAgeBelow 21)
    |> List.map SCOUT.rRPlayerReport
   
 
 //   "IWL: Todd Cantwell -> player 72.09 vs avg 65.38 -> delta 6.72";
 //try to get a loanee 
let lliwls = 
    //get all players better
    let iwls = SCOUT.getSctPlayersForRoleAbove "IWL" 69.0
    iwls |> List.filter SCOUT.roleRatedPlayerLoanListed
    //NONE
//try to get a listed player
let tliwls = 
    //get all players better
    let iwls = SCOUT.getSctPlayersForRoleAbove "IWL" 69.0
    iwls |> List.filter SCOUT.roleRatedPlayerTransferListed
    |> List.map SCOUT.rRPlayerReport
    //NONE
//try to get a cheap player 
let chpiwls = 
    //get all players better
    let iwls = SCOUT.getSctPlayersForRoleAbove "IWL" 69.0
    iwls 
    |> List.filter (SCOUT.roleRatedPlayerValueBelowK 19000)
    |> List.filter (SCOUT.roleRatedPlayerAgeBelow 23)
    |> List.map SCOUT.rRPlayerReport




//  ["SKD: Tim Krul -> player 67.79 vs avg 63.45 -> delta 4.34";
//   "IWBR: Max Aarons -> player 67.65 vs avg 61.59 -> delta 6.06";
//   "IWBL: Xavi Quintillà -> player 66.55 vs avg 61.95 -> delta 4.60";
//   "BPD1: Juanpe -> player 67.56 vs avg 64.41 -> delta 3.14";
//   "BPD2: Jacob Sørensen -> player 65.41 vs avg 62.53 -> delta 2.88";
//   "WAR: Emiliano Buendía -> player 75.43 vs avg 67.33 -> delta 8.10";
//   "IWL: Todd Cantwell -> player 72.09 vs avg 65.38 -> delta 6.72";
//   "BWM: Alexander Tettey -> player 68.33 vs avg 66.11 -> delta 2.23";
//   "AP: Moritz Leitner -> player 71.01 vs avg 65.63 -> delta 5.38";
//   "AFA: Teemu Pukki -> player 70.50 vs avg 64.81 -> delta 5.69";
//   "TMA: Adam Idah -> player 66.22 vs avg 63.31 -> delta 2.91";
//   "Team average: 68.96 vs Division average: 64.23 -> delta 4.73"]
//val cmp2: string list =
//  ["SKD: Ørjan Nyland -> player 61.56 vs avg 63.45 -> delta -1.89";
//   "IWBR: Sam Byram -> player 64.50 vs avg 61.59 -> delta 2.91";
//   "IWBL: Dimitris Giannoulis -> player 64.40 vs avg 61.95 -> delta 2.45";
//   "BPD1: Ben Gibson -> player 65.35 vs avg 64.41 -> delta 0.94";
//   "BPD2: Christoph Zimmermann -> player 63.66 vs avg 62.53 -> delta 1.13";
//   "WAR: Onel Hernández -> player 68.43 vs avg 67.33 -> delta 1.10";
//   "IWL: Marco Stiepermann -> player 67.57 vs avg 65.38 -> delta 2.19";
//   "BWM: Oliver Skipp -> player 67.35 vs avg 66.11 -> delta 1.24";
//   "AP: Kieran Dowell -> player 67.91 vs avg 65.63 -> delta 2.27";
//   "AFA: Willem Geubbels -> player 65.50 vs avg 64.81 -> delta 0.69";
//   "TMA: Jordan Hugill -> player 65.00 vs avg 63.31 -> delta 1.69";
//   "Team average: 65.57 vs Division average: 64.23 -> delta 1.34"]

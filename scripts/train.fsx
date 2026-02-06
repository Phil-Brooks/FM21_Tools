#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let div = "England (Sky Bet Championship)"
let path = "../data/all.html"
do HTML.loadPlayers path

let mypath = "../data/my8.html"
HTML.loadMyPlayers mypath
printfn "Loaded %d players from %s" (List.length HTML.MyPlayers) path

let brls = 
    HTML.MyPlayers
    |>List.map (fun p -> p.Name , ROLE.bestRoleForPlayer p )
    |>List.sortBy fst

let rrps = HTML.MyPlayers|>List.map ROLE.bestRoleRatedPlayer

let wkas = 
    rrps
    |>List.map (fun brp -> if brp.IsSome then ROLE.weakestRelevantAttributeForPlayer brp.Value else None)
    |>List.sortBy (fun o -> match o with Some (_,v,_,_) -> v | None -> "")

let wkas2 = 
    rrps
    |>List.map (fun brp -> if brp.IsSome then ROLE.secondWeakestRelevantAttributeForPlayer brp.Value else None)
    |>List.sortBy (fun o -> match o with Some (_,v,_,_) -> v | None -> "")

//val wkas: (string * string * string * int) option list =
//  [Some ("TMA", "Adam Idah", "Pas", 11);
//   Some ("IWBL", "Adrien Truffert", "Dri", 9);
//   Some ("WAR", "Aidan Fitzpatrick", "Fin", 10);
//   Some ("BPD", "Dan-Axel Zagadou", "Agg", 6);
//   Some ("AP", "Emiliano Buendía", "Sta", 13);
//   Some ("BWM", "Jacob Sørensen", "Acc", 10);
//   Some ("BWM", "James McCarthy", "Acc", 9);
//   Some ("AFA", "Janis Antiste", "Bal", 9);
//   Some ("AFA", "João Pedro", "Fin", 12);
//   Some ("AP", "Kieran Dowell", "Pac", 12);
//   Some ("IWBR", "Kieran Trippier", "Dri", 10);
//   Some ("BPD", "Luis Binks", "Acc", 7);
//   Some ("IWBR", "Max Aarons", "Cro", 10);
//   Some ("AP", "Maxime Lopez", "OtB", 13);
//   Some ("IWL", "Michael Levy", "Fin", 3); Some ("BWM", "Mo Bešić", "Acc", 11);
//   Some ("BPD", "Nathan Collins", "Pas", 12);
//   Some ("TMA", "Oscar Thorn", "Pas", 8);
//   Some ("TMA", "Pietro Pellegri", "Pas", 11);
//   Some ("AP", "Rekeem Harper", "Pac", 13);
//   Some ("SKD", "Rick Jonkers", "Ecc", 5);
//   Some ("IWBR", "Sam Byram", "Str", 9);
//   Some ("AP", "Shane Hutchinson", "Ant", 8);
//   Some ("BWM", "Sivert Mannsverk", "Cmp", 10);
//   Some ("BWM", "Sol Hamilton", "Mar", 9);
//   Some ("AP", "Spencer Ginty", "Fla", 8);
//   Some ("AP", "Thiago Almada", "Pac", 13); Some ("SKD", "Tim Krul", "Pac", 3);
//   Some ("WAR", "Willem Geubbels", "Cro", 11);
//   Some ("IWBL", "Xavi Quintillà", "Str", 9);
//   Some ("IWBL", "Youssouf Koné", "Dri", 12)]
//val wkas2: (string * string * string * int) option list =
//  [Some ("TMA", "Adam Idah", "Dri", 12);
//   Some ("IWBL", "Adrien Truffert", "Tec", 12);
//   Some ("WAR", "Aidan Fitzpatrick", "OtB", 11);
//   Some ("BPD", "Dan-Axel Zagadou", "Acc", 10);
//   Some ("AP", "Emiliano Buendía", "Acc", 14);
//   Some ("BWM", "Jacob Sørensen", "Dec", 12);
//   Some ("BWM", "James McCarthy", "Sta", 10);
//   Some ("AFA", "Janis Antiste", "Sta", 10);
//   Some ("AFA", "João Pedro", "OtB", 13);
//   Some ("AP", "Kieran Dowell", "OtB", 13);
//   Some ("IWBR", "Kieran Trippier", "Dec", 12);
//   Some ("BPD", "Luis Binks", "Pac", 8);
//   Some ("IWBR", "Max Aarons", "Pas", 12);
//   Some ("AP", "Maxime Lopez", "Fla", 13);
//   Some ("IWL", "Michael Levy", "Cmp", 5); Some ("BWM", "Mo Bešić", "Str", 12);
//   Some ("BPD", "Nathan Collins", "Tec", 12);
//   Some ("TMA", "Oscar Thorn", "Dri", 9);
//   Some ("TMA", "Pietro Pellegri", "Hea", 12);
//   Some ("AP", "Rekeem Harper", "Cmp", 14);
//   Some ("SKD", "Rick Jonkers", "Cmd", 10);
//   Some ("IWBR", "Sam Byram", "Cro", 11);
//   Some ("AP", "Shane Hutchinson", "Dri", 9);
//   Some ("BWM", "Sivert Mannsverk", "Acc", 12);
//   Some ("BWM", "Sol Hamilton", "Dec", 10);
//   Some ("AP", "Spencer Ginty", "Pac", 8);
//   Some ("AP", "Thiago Almada", "Ant", 14); Some ("SKD", "Tim Krul", "Ecc", 8);
//   Some ("WAR", "Willem Geubbels", "Pas", 11);
//   Some ("IWBL", "Xavi Quintillà", "Dri", 10);
//   Some ("IWBL", "Youssouf Koné", "Cmp", 12)]

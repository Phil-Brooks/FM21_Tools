#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

//let div = "England (Sky Bet Championship)"
//let path = "../data/all.html"
//do HTML.loadPlayers path

let mypath = "../data/my.html"
HTML.loadMyPlayers mypath
printfn "Loaded %d players from %s" (List.length HTML.MyPlayers) mypath

let rrls = 
    HTML.MyPlayers
    |>List.map (fun p -> p.Name , ROLE.ZAZroleRatingsForPlayer p )

//val rrls: (string * (string * float) list) list =
//   ("Alexander Schlager", [("SKS", 68.02631579)]);
//   ("Matheus", [("SKS", 69.21052632)])]

//   ("Ronaël Pierre-Gabriel", [("IWBR", 67.43589744); ("IWBL", 67.43589744)]);

//   ("Youssouf Koné", [("IWBL", 66.66666667)]);

//   ("Jean-Clair Todibo",  [("BPD", 72.34177215); ("DLP", 70.0); ("MCA", 65.35211268)]);
//   ("Dan-Axel Zagadou", [("BPD", 74.24050633); ("IWBL", 66.21794872)]);
//   ("José Luis Pasquali", [("BPD", 71.13924051); ("IWBR", 66.66666667); ("DLP", 66.17647059)]);

//   ("Krystian Bielik",  [("DLP", 75.98039216); ("BPD", 74.87341772); ("MCA", 68.52112676)]);

//   ("Mateu Morey", [("IWBR", 71.98717949); ("MR", 71.57534247)]);

//   ("Alberto Moreno", [("ML", 67.46575342); ("IWBL", 66.08974359)]);

//   ("Ömer Beyaz", [("MCA", 69.15492958); ("AMSS", 67.72727273)]);
//   ("Hannibal", [("MCA", 74.57746479); ("AMSS", 69.09090909)]);
//   ("Orkun Kökçü", [("MCA", 70.35211268); ("AMSS", 64.36363636); ("MR", 58.83561644)]);

//   ("Shiloh 't Zand",  [("AMSS", 70.18181818); ("MCA", 68.30985915); ("ML", 64.8630137)]); 
//   ("Matheusinho",   [("MCA", 69.57746479); ("AMSS", 69.0); ("ML", 60.47945205); ("MR", 60.47945205)]);
//   ("Juanjo Echeverría", [("AMSS", 70.81818182); ("MCA", 70.56338028); ("MR", 58.01369863)]);

//   ("Santiago Ascacíbar", [("MCA", 56.69014085); ("DLP", 55.49019608)]);
//   ("Robert Piris Da Motta", [("BPD", 60.63291139); ("MCA", 57.6056338); ("DLP", 56.66666667)]);
//   ("Borja Garcés", []);
//   TRN ("Gio-Renys Felicia", [("ML", 61.23287671); ("MR", 61.23287671)]);
//   TRN ("Rafa Mir", [("ML", 58.42465753); ("MR", 58.42465753)]);
//   TRN ("Cristian Olivera", [("MR", 64.52054795)]);
//   ("Ljubomir Denkovski", [("MCA", 66.12676056); ("AMSS", 63.54545455)]);
//   TRN ("Folarin Balogun", [("MR", 55.34246575)]);

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


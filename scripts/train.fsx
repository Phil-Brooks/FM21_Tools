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

  // ("Alexander Schlager", [("SKS", 68.48684211)]);
  // ("Matheus", [("SKS", 69.47368421)]);

  // ("Ronaël Pierre-Gabriel", [("IWBR", 67.75641026); ("IWBL", 67.75641026)]);
  // ("Cristinel Vaida", [("MCA", 72.95774648); ("IWBR", 68.26923077); ("MR", 64.65753425)]);
  
  // ("Youssouf Koné", [("IWBL", 67.05128205)]);
   // ("Alberto Moreno", [("ML", 66.30136986); ("IWBL", 65.19230769)]);
 
  // ("Mateu Morey", [("IWBR", 71.98717949); ("MR", 71.57534247)])]
  // ("Mario Weber", [("MR", 67.12328767); ("IWBR", 65.51282051)

  // ("Pietro Celeste", [("ML", 66.16438356);
  // ("Achim Schmedes", [("ML", 67.87671233)]);

  // ("Krystian Bielik", [("DLP", 75.98039216); ("BPD", 73.92405063);
  // ("José Luis Pasquali",[("BPD", 72.97468354); ("DLP", 68.23529412); ("IWBR", 68.07692308)])]
  // ("Issa Diop", [("BPD", 68.67088608)]);
  // ("Jean-Clair Todibo", "BPD", 72.34177215); ("DLP", 70.29411765)

  // ("Claudio Rizzardi", [("DLP", 76.37254902); ("MCA", 69.57746479)]);
  // ("Billy Gilmour", [("DLP", 76.47058824); ("MCA", 72.53521127); 
  
  // ("Hannibal", [("MCA", 75.14084507); ("AMSS", 68.63636364)]);
  // ("Orkun Kökçü", [("MCA", 70.63380282); ("AMSS", 65.81818182); 

  // ("Shiloh 't Zand", ("AMSS", 71.09090909); ("MCA", 68.87323944);
  // ("Juanjo Echeverría", [("AMSS", 73.0); ("MCA", 71.83098592);
  // ("Matheusinho", [("MCA", 70.14084507); ("AMSS", 69.90909091); 
  // ("Ömer Beyaz", [("MCA", 70.42253521); ("AMSS", 68.63636364)]);



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


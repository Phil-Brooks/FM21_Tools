#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let div = "England (Sky Bet Championship)"
let path = "../data/all.html"
do HTML.loadPlayers path

let mypath = "../data/my2.html"
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

//val brls: (string * (string * float) option) list =
//  [("Adam Idah", Some ("AFA", 66.375));
//   ("Aidan Fitzpatrick", Some ("WAR", 63.14285714));
//   ("Alexander Tettey", Some ("BWM", 68.33333333));
//   ("Aston Oxborough", Some ("SKD", 51.68831169));
//   ("Bali Mumba", Some ("IWBR", 54.65));
//   ("Ben Gibson", Some ("BPD", 65.34883721));
//   ("Brad Hills", Some ("BPD", 43.77906977));
//   ("Caleb Richards", Some ("IWBL", 48.25));
//   ("Christoph Zimmermann", Some ("BPD", 63.6627907));
//   ("Danel Sinani", Some ("WAR", 62.28571429));
//   ("Daniel Adshead", Some ("AP", 57.77027027));
//   ("Dimitris Giannoulis", Some ("IWBL", 64.4));
//   ("Eddie Jackson", Some ("IWBR", 44.2));
//   ("Emiliano Buendía", Some ("AP", 76.48648649));
//   ("Ethen Vaughan", Some ("IWBR", 49.25));
//   ("Fiete Arp", Some ("AFA", 67.625));
//   ("Gassan Ahadme", Some ("TMA", 53.37837838));
//   ("Harry Pitcher", Some ("TMA", 46.21621622));
//   ("Jacob Sørensen", Some ("BWM", 67.28395062));
//   ("Joe Duffy", Some ("AFA", 52.5));
//   ("Jonathan Tomkinson", Some ("BPD", 50.81395349));
//   ("Jordan Hugill", Some ("TMA", 65.0));
//   ("Jordan Thomas", Some ("IWBR", 50.95));
//   ("Josh Giurgi", Some ("WAR", 53.42857143));
//   ("Josip Drmić", Some ("AFA", 65.0)); ("Juanpe", Some ("BWM", 67.83950617));
//   ("Kieran Dowell", Some ("AP", 68.17567568));
//   ("Lewis Shipley", Some ("BPD", 56.62790698));
//   ("Louis Lomas", Some ("BPD", 51.68604651));
//   ("Louis Thompson", Some ("BWM", 65.37037037));
//   ("Lukas Rupp", Some ("AP", 65.0));
//   ("Marco Stiepermann", Some ("AP", 68.44594595));
//   ("Matt Richardson", Some ("BPD", 49.30232558));
//   ("Max Aarons", Some ("IWBR", 67.65));
//   ("Moritz Leitner", Some ("AP", 71.01351351));
//   ("Ola Okeowo", Some ("IWBR", 44.1));
//   ("Oliver Skipp", Some ("BWM", 67.34567901));
//   ("Onel Hernández", Some ("WAR", 68.42857143));
//   ("Oscar Thorn", Some ("TMA", 51.35135135));
//   ("Philip Heise", Some ("IWBL", 58.25)); ("Sam Byram", Some ("IWBR", 64.5));
//   ("Sam McCallum", Some ("IWBL", 55.65));
//   ("Saul Milovanovic", Some ("AP", 51.48648649));
//   ("Saxon Earley", Some ("AP", 48.91891892));
//   ("Sebastian Soto", Some ("AFA", 58.125));
//   ("Shae Hutchinson", Some ("AFA", 52.0));
//   ("Sol Hamilton", Some ("BWM", 50.80246914));
//   ("Teemu Pukki", Some ("AFA", 70.5));
//   ("Thiago Almada", Some ("AP", 70.13513514));
//   ("Tim Krul", Some ("SKD", 67.79220779));
//   ("Timm Klose", Some ("BPD", 60.87209302));
//   ("Tom Trybull", Some ("BWM", 61.60493827));
//   ("Willem Geubbels", Some ("WAR", 65.57142857));
//   ("William Hondermarck", Some ("AP", 55.74324324));
//   ("Xavi Quintillà", Some ("IWBL", 66.55));
//   ("Zach Dronfield", Some ("AP", 53.17567568));
//   ("Zak Brown", Some ("BPD", 43.95348837));
//   ("Ísak Snær Þorvaldsson", Some ("AP", 55.94594595));
//   ("Ørjan Nyland", Some ("SKD", 61.55844156))]

//val wkas: (string * string * string * int) option list =
//  [Some ("AFA", "Adam Idah", "Pas", 10);
//   Some ("WAR", "Aidan Fitzpatrick", "OtB", 9);
//   Some ("BWM", "Alexander Tettey", "Acc", 8);
//   Some ("SKD", "Aston Oxborough", "Ecc", 1);
//   Some ("IWBR", "Bali Mumba", "Str", 8);
//   Some ("BPD", "Ben Gibson", "Acc", 10); Some ("BPD", "Brad Hills", "Pas", 5);
//   Some ("IWBL", "Caleb Richards", "Dri", 7);
//   Some ("BPD", "Christoph Zimmermann", "Tec", 9);
//   Some ("WAR", "Danel Sinani", "Sta", 9);
//   Some ("AP", "Daniel Adshead", "OtB", 9);
//   Some ("IWBL", "Dimitris Giannoulis", "Str", 8);
//   Some ("IWBR", "Eddie Jackson", "OtB", 6);
//   Some ("AP", "Emiliano Buendía", "Sta", 13);
//   Some ("IWBR", "Ethen Vaughan", "Dri", 7);
//   Some ("AFA", "Fiete Arp", "Sta", 11);
//   Some ("TMA", "Gassan Ahadme", "Pas", 7);
//   Some ("TMA", "Harry Pitcher", "Dri", 8);
//   Some ("BWM", "Jacob Sørensen", "Acc", 10);
//   Some ("AFA", "Joe Duffy", "Dri", 9);
//   Some ("BPD", "Jonathan Tomkinson", "Tec", 7);
//   Some ("TMA", "Jordan Hugill", "Tec", 10);
//   Some ("IWBR", "Jordan Thomas", "Dri", 7);
//   Some ("WAR", "Josh Giurgi", "OtB", 8);
//   Some ("AFA", "Josip Drmić", "Ant", 11); Some ("BWM", "Juanpe", "Agg", 11);
//   Some ("AP", "Kieran Dowell", "Pac", 10);
//   Some ("BPD", "Lewis Shipley", "Ant", 9);
//   Some ("BPD", "Louis Lomas", "Dec", 8);
//   Some ("BWM", "Louis Thompson", "Dec", 10);
//   Some ("AP", "Lukas Rupp", "Fla", 10);
//   Some ("AP", "Marco Stiepermann", "Tec", 11);
//   Some ("BPD", "Matt Richardson", "Pas", 9);
//   Some ("IWBR", "Max Aarons", "Cro", 10);
//   Some ("AP", "Moritz Leitner", "OtB", 12);
//   Some ("IWBR", "Ola Okeowo", "Pas", 6);
//   Some ("BWM", "Oliver Skipp", "Mar", 11);
//   Some ("WAR", "Onel Hernández", "Fin", 9);
//   Some ("TMA", "Oscar Thorn", "Dri", 7);
//   Some ("IWBL", "Philip Heise", "Ant", 9);
//   Some ("IWBR", "Sam Byram", "Str", 10);
//   Some ("IWBL", "Sam McCallum", "OtB", 7);
//   Some ("AP", "Saul Milovanovic", "Fla", 7);
//   Some ("AP", "Saxon Earley", "Dri", 7);
//   Some ("AFA", "Sebastian Soto", "OtB", 10);
//   Some ("AFA", "Shae Hutchinson", "Bal", 8);
//   Some ("BWM", "Sol Hamilton", "Dec", 7);
//   Some ("AFA", "Teemu Pukki", "Dri", 12);
//   Some ("AP", "Thiago Almada", "Pac", 12); Some ("SKD", "Tim Krul", "Ecc", 8);
//   Some ("BPD", "Timm Klose", "Tec", 8);
//   Some ("BWM", "Tom Trybull", "Agg", 10);
//   Some ("WAR", "Willem Geubbels", "Cro", 11);
//   Some ("AP", "William Hondermarck", "OtB", 10);
//   Some ("IWBL", "Xavi Quintillà", "Str", 9);
//   Some ("AP", "Zach Dronfield", "Dri", 8);
//   Some ("BPD", "Zak Brown", "Cmp", 6);
//   Some ("AP", "Ísak Snær Þorvaldsson", "OtB", 9);
//   Some ("SKD", "Ørjan Nyland", "Pun", 8)]

//val wkas2: (string * string * string * int) option list =
//  [Some ("AFA", "Adam Idah", "Dri", 12);
//   Some ("WAR", "Aidan Fitzpatrick", "Fin", 10);
//   Some ("BWM", "Alexander Tettey", "Pac", 9);
//   Some ("SKD", "Aston Oxborough", "Pun", 3);
//   Some ("IWBR", "Bali Mumba", "Cro", 9);
//   Some ("BPD", "Ben Gibson", "Pas", 12); Some ("BPD", "Brad Hills", "Tec", 6);
//   Some ("IWBL", "Caleb Richards", "Tec", 8);
//   Some ("BPD", "Christoph Zimmermann", "Dec", 10);
//   Some ("WAR", "Danel Sinani", "Pas", 10);
//   Some ("AP", "Daniel Adshead", "Fla", 9);
//   Some ("IWBL", "Dimitris Giannoulis", "Tck", 9);
//   Some ("IWBR", "Eddie Jackson", "Pas", 7);
//   Some ("AP", "Emiliano Buendía", "Ant", 14);
//   Some ("IWBR", "Ethen Vaughan", "Tec", 8);
//   Some ("AFA", "Fiete Arp", "Pas", 11);
//   Some ("TMA", "Gassan Ahadme", "Acc", 9);
//   Some ("TMA", "Harry Pitcher", "Fir", 8);
//   Some ("BWM", "Jacob Sørensen", "Mar", 12);
//   Some ("AFA", "Joe Duffy", "OtB", 9);
//   Some ("BPD", "Jonathan Tomkinson", "Str", 8);
//   Some ("TMA", "Jordan Hugill", "Dri", 11);
//   Some ("IWBR", "Jordan Thomas", "Cro", 8);
//   Some ("WAR", "Josh Giurgi", "Fin", 8);
//   Some ("AFA", "Josip Drmić", "Pas", 11); Some ("BWM", "Juanpe", "Acc", 11);
//   Some ("AP", "Kieran Dowell", "Acc", 12);
//   Some ("BPD", "Lewis Shipley", "Agg", 9);
//   Some ("BPD", "Louis Lomas", "Agg", 8);
//   Some ("BWM", "Louis Thompson", "Cmp", 10);
//   Some ("AP", "Lukas Rupp", "Dri", 12);
//   Some ("AP", "Marco Stiepermann", "Acc", 11);
//   Some ("BPD", "Matt Richardson", "Tec", 9);
//   Some ("IWBR", "Max Aarons", "Str", 10);
//   Some ("AP", "Moritz Leitner", "Ant", 12);
//   Some ("IWBR", "Ola Okeowo", "Cro", 6);
//   Some ("BWM", "Oliver Skipp", "Dec", 11);
//   Some ("WAR", "Onel Hernández", "Tec", 12);
//   Some ("TMA", "Oscar Thorn", "Pas", 7);
//   Some ("IWBL", "Philip Heise", "Tec", 10);
//   Some ("IWBR", "Sam Byram", "Dec", 11);
//   Some ("IWBL", "Sam McCallum", "Tec", 9);
//   Some ("AP", "Saul Milovanovic", "OtB", 8);
//   Some ("AP", "Saxon Earley", "Fla", 7);
//   Some ("AFA", "Sebastian Soto", "Ant", 10);
//   Some ("AFA", "Shae Hutchinson", "Pas", 8);
//   Some ("BWM", "Sol Hamilton", "Mar", 8);
//   Some ("AFA", "Teemu Pukki", "Tec", 12);
//   Some ("AP", "Thiago Almada", "Ant", 13); Some ("SKD", "Tim Krul", "Pac", 8);
//   Some ("BPD", "Timm Klose", "Pas", 10);
//   Some ("BWM", "Tom Trybull", "Str", 10);
//   Some ("WAR", "Willem Geubbels", "Pas", 11);
//   Some ("AP", "William Hondermarck", "Cmp", 10);
//   Some ("IWBL", "Xavi Quintillà", "Dri", 10);
//   Some ("AP", "Zach Dronfield", "Tec", 10);
//   Some ("BPD", "Zak Brown", "Pas", 7);
//   Some ("AP", "Ísak Snær Þorvaldsson", "Ant", 9);
//   Some ("SKD", "Ørjan Nyland", "Ecc", 8)]

#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let sctpath = "../data/sct1.html"
HTML.loadSctPlayers sctpath

//let path = "../data/all.html"
//do PROGRESS.loadOldPlayers path

let yngs = SCOUT.getYng "BWM" 65 4000 20

//SKD
  //[("Filip Jörgensen", "Villarreal C", "190 cm", 62.20779221);
  // ("Alessandro Russo", "Entella", "193 cm", 61.55844156);
  // ("Gavin Bazunu", "Rochdale", "189 cm", 61.2987013)]
//IWBR
  //[("Tomás Esteves", "Reading", "180 cm", 64.15);
  // ("Aaron Hickey", "Bologna", "176 cm", 63.75);
  // ("Ethan Laird", "MK Dons", "177 cm", 63.05);
  // ("Issa Kaboré", "KV Mechelen", "178 cm", 61.9);
  // ("Yan Couto", "Girona", "170 cm", 61.0)]
//IWBL
  //[("Rayan Aït-Nouri", "Wolves", "180 cm", 66.25);
  // ("Dennis Cirkin", "Colchester", "183 cm", 63.8);
  // ("Aaron Hickey", "Bologna", "176 cm", 63.75);
  // ("Ethan Laird", "MK Dons", "177 cm", 63.05);
  // ("Noah Katterbach", "1. FC Köln", "180 cm", 62.8);
  // ("Arthur Zagré", "Dijon FCO", "168 cm", 62.65);
  // ("Riccardo Calafiori", "Roma", "186 cm", 62.1);
  // ("Adrien Truffert", "US Orléans", "174 cm", 61.3)]
//BPD
  //[("Luis Binks", "Montreal Impact", "188 cm", 66.22093023);
  // ("Nathan Collins", "Stoke", "196 cm", 64.70930233);
  // ("Tanguy Nianzou", "FC Bayern II", "187 cm", 64.01162791);
  // ("Gonçalo Inácio", "Sporting", "187 cm", 63.6627907);
  // ("Armel Bella-Kotchap", "VfL Bochum", "190 cm", 63.02325581)]
//WAR
  //[("Francisco Conceição", "FCP", "170 cm", 71.85714286);
  // ("Cristian Olivera", "Almería", "170 cm", 69.28571429);
  // ("Amad Diallo", "Man Utd", "174 cm", 69.14285714);
  // ("Bryan Gil", "Eibar", "175 cm", 69.14285714);
  // ("Yusuf Demir", "SK Rapid Vienna", "174 cm", 68.14285714);
  // ("Nathanaël Mbuku", "Reims", "171 cm", 67.42857143);
  // ("Xavier Amaechi", "KSC", "179 cm", 67.28571429)]
//IWL
  // ("Khvicha Kvaratskhelia", "Rubin", "183 cm", 69.39189189);
  // ("Mohamed Taabouni", "AZ", "175 cm", 66.82432432);
  // ("Jamal Musiala", "FC Bayern", "183 cm", 66.68918919);
  // ("Antonio Marin", "Lokomotiva", "182 cm", 66.55405405)]
//BWM
  // ("Marco Kana", "Anderlecht", "179 cm", 63.58024691);
  // ("Manuel Ugarte", "Famalicão", "179 cm", 63.33333333);
  // ("Angelo Stiller", "FC Bayern", "183 cm", 63.20987654)]
//AP
  //[("Reinier", "Borussia Dortmund", "185 cm", 71.48648649);
  // ("Ajdin Hasic", "Beşiktaş", "178 cm", 66.68918919);
  // ("Pierre Dwomoh", "KRC Genk", "185 cm", 66.55405405);
  // ("Lazar Samardžić", "RB Leipzig", "183 cm", 66.35135135);
  // ("Thomas Van Den Keybus", "Club Brugge", "159 cm", 66.28378378);
  // ("Manu Koné", "Toulouse FC", "185 cm", 66.28378378);
  // ("Robert Navarro", "Real San Sebastián", "178 cm", 66.21621622);
//AP2
  //[("Ömer Beyaz", "Fenerbahçe", "173 cm", 72.02702703);
  // ("Mohamed Daramy", "FC København", "181 cm", 70.54054054);
  // ("Youssoufa Moukoko", "Borussia Dortmund", "180 cm", 69.66216216);
  // ("Jamal Musiala", "FC Bayern", "183 cm", 69.66216216);
  // ("Pierre Dwomoh", "KRC Genk", "186 cm", 69.52702703);
  // ("Hannibal", "Swansea", "183 cm", 68.91891892);
  // ("Cristian Olivera", "Almería", "172 cm", 68.71621622);
  // ("Yusuf Demir", "SK Rapid Vienna", "174 cm", 68.51351351);
  // ("Thomas Van Den Keybus", "Club Brugge", "160 cm", 68.44594595);
  // ("Bryan Gil", "Sevilla", "175 cm", 68.37837838);
  // ("Alejandro Salas", "Valencia B", "182 cm", 67.97297297);
  // ("Luca Connell", "Celtic", "169 cm", 67.90540541);
  // ("Talles Magno", "VDG", "187 cm", 67.83783784);
  // ("Adil Aouchiche", "AS Saint-Etienne", "182 cm", 67.77027027);
  // ("Mouhamadou Diarra", "Strasbourg", "180 cm", 67.63513514);
  // ("Martin Palumbo", "Udinese", "184 cm", 67.63513514);
  // ("Manu Koné", "Borussia M'gladbach", "185 cm", 67.43243243);
  // ("Moisés Caicedo", "Brighton", "179 cm", 67.2972973);
  // ("Robert Navarro", "Villarreal", "178 cm", 67.22972973);
  // ("Rayan Cherki", "OL", "177 cm", 67.09459459);
  // ("Mohamed Taabouni", "AZ", "175 cm", 67.02702703);
  // ("Amadou Onana", "Hamburger SV", "192 cm", 66.75675676);
  // ("Kenzo Goudmijn", "AZ", "174 cm", 66.62162162);
  // ("Edvard Tagseth", "Rosenborg", "170 cm", 66.62162162);
  // ("Wassim Essanoussi", "VVV-Venlo", "169 cm", 66.62162162);
  // ("James McAtee", "Man City", "183 cm", 66.55405405);
  // ("Pape Matar Sarr", "FC Metz", "159 cm", 66.48648649);
  // ("Stanislav Shopov", "sc Heerenveen", "179 cm", 66.48648649);
  // ("Enrik Ostrc", "Olimpija", "187 cm", 66.41891892);
  // ("Janis Antiste", "Toulouse FC", "186 cm", 66.35135135);
  // ("Shola Shoretire", "Man Utd", "176 cm", 66.14864865);
  // ("Robert Jezek", "Slavia Prague", "182 cm", 66.01351351);


//AFA
  //[("Youssoufa Moukoko", "Borussia Dortmund", "179 cm", 71.25);
  // ("Joe Gelhardt", "Rotherham", "177 cm", 66.875);
  // ("Abdoul Karim Camara", "Hoffenheim", "169 cm", 66.75);
  // ("Liam Delap", "Man City", "185 cm", 66.625);
  // ("Lorenzo Colombo", "Cremonese", "183 cm", 66.25);
  // ("Fábio Silva", "Wolves", "185 cm", 66.125);
  // ("Adam Hlozek", "Sparta Prague", "185 cm", 66.125)]
//TMA
  // ("Janis Antiste", "Toulouse FC", "186 cm", 65.94594595);
  // ("Evann Guessand", "Lausanne", "188 cm", 65.40540541);
  // ("Danylo Sikan", "Mariupol", "185 cm", 64.86486486);
  // ("Pietro Pellegri", "AS Monaco", "188 cm", 64.59459459);





let bests = SCOUT.getBest "BWM" 67.7 17000
let tls = SCOUT.getTrLst "AP" 67.0 13000
let lls = SCOUT.getLnLst "AFA" 62.0 13000

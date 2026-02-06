#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let path = "../data/sct2.html"
do PROGRESS.loadOldPlayers path
let curpath = "../data/sct3.html"
do PROGRESS.loadCurPlayers curpath

let best = PROGRESS.top30Improvements()

//val best: (string * string * string * string * float * float) list =
//  [("Jordi", "R. Madrid B", "WAR", "175 cm", 67.12121212, 34.36480186);
//   ("Joe Williams", "Bristol City", "BWM", "178 cm", 63.88888889, 32.24331927);
//   ("Álvaro", "OM", "BPD", "182 cm", 65.06329114, 25.37974684);
//   ("Jordan Jones", "Rangers", "WAR", "173 cm", 67.8030303, 22.73260777);
//   ("Mateo", "Intercity", "AP", "181 cm", 54.78873239, 22.32037796);
//   ("Rayco", "Lanzarote", "IWL", "177 cm", 46.04477612, 18.03195561);
//   ("Moha", "Huelva", "AFA", "186 cm", 54.22535211, 15.62241094);
//   ("James Brown", "Charlton", "IWBR", "181 cm", 57.05128205, 15.61188811);
//   ("Borja", "", "AP", "185 cm", 56.83098592, 15.3158344);
//   ("Gareth Evans", "Bradford City", "AP", "183 cm", 54.71830986, 15.07042254);
//   ("Sergio Pérez", "El Ejido", "WAR", "180 cm", 62.34848485, 14.77495544);
//   ("Rubén", "Villarreal", "AP", "178 cm", 63.87323944, 14.63712833);
//   ("Alberto Alonso", "Villanueva de Gállego", "BWM", "179 cm", 57.29166667,
//    14.47476526);
//   ("Fernando", "GRE", "BWM", "183 cm", 68.68055556, 13.95833333);
//   ("Alberto", "", "BPD", "183 cm", 52.65822785, 13.54058079);
//   ("Rayco", "O. Marítima", "AP", "177 cm", 40.21126761, 12.19844709);
//   ("Víctor", "F.C. Andorra", "BPD", "185 cm", 54.81012658, 11.96290436);
//   ("Álex Pérez", "Llagostera", "AFA", "170 cm", 60.35211268, 11.95467678);
//   ("Alberto", "Albacete", "SKD", "183 cm", 49.19117647, 10.07352941);
//   ("Rayco", "Tenisca", "IWL", "182 cm", 39.40298507, 9.979908152);
//   ("Gonzalo", "Cacereño", "BWM", "175 cm", 48.81944444, 9.875782473);
//   ("Jean-Louis Perrot", "OL", "BWM", "183 cm", 51.59722222, 9.417735043);
//   ("Juanan", "HIFK", "WAR", "183 cm", 58.48484848, 8.970959596);
//   ("José Manuel", "Stadium Casablanca", "WAR", "182 cm", 44.77272727,
//    8.939393939);
//   ("Miki", "Compostela", "AFA", "180 cm", 60.07042254, 8.298270636);
//   ("Danny Rowe", "", "WAR", "186 cm", 61.28787879, 7.537878788);
//   ("Moha", "Fuenlabrada", "TMA", "185 cm", 59.16666667, 7.406103286);
//   ("Marcos", "Liverpool", "AP", "187 cm", 53.38028169, 7.203811102);
//   ("Jordi Altena", "Vitesse", "WAR", "182 cm", 52.57575758, 6.801109688);
//   ("Manu", "Coruxo", "IWBL", "176 cm", 54.80769231, 6.779523294)]

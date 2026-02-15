open System

#r "../FM21_ToolsLib/bin/Debug/net10.0/FM21_ToolsLib.dll"

open FM21_ToolsLib

let sctpath = "../data/sct7.html"
HTML.loadSctPlayers sctpath


let getRoles (pos:string) =
    let bits = pos.Split([|','|], StringSplitOptions.RemoveEmptyEntries)
    let bits1 = bits |> Array.map (fun b -> b.Trim())
    bits1|>List.ofArray
let roles = HTML.SctPlayers |> List.map (fun p -> getRoles(p.Extras["Position"])) |> List.concat |> List.distinct

let ct = roles.Length

let tma = roles|>List.filter(fun r -> r.Contains("S"))

let war = roles|>List.filter(fun r -> r.Contains("M") && r.Contains("R"))
let iwl = roles|>List.filter(fun r -> r.Contains("M") && r.Contains("L"))
let bwm = roles|>List.filter(fun r -> r.Contains("M") && r.Contains("C"))
let bpd = roles|>List.filter(fun r -> r.Contains("D") && r.Contains("C"))
let iwbr = roles|>List.filter(fun r -> r.Contains("D") && r.Contains("R"))
let iwbl = roles|>List.filter(fun r -> r.Contains("D") && r.Contains("L"))
let skd = roles|>List.filter(fun r -> r.Contains("K"))

  //["ST (C)"; "M/AM (R)"; "AM (RL)"; "D/WB/M/AM (L)"; "M (RL)"; "AM (RLC)";
  // "D/WB (R)"; "M/AM (RL)"; "D (C)"; "D/WB/M (R)"; "M (L)"; "WB/M/AM (R)";
  // "AM (R)"; "M (R)"; "M (LC)"; "AM (C)"; "D/WB/AM (R)"; "D/WB (L)"; "AM (L)";
  // "WB/M/AM (L)"; "D (RLC)"; "WB (R)"; "DM"; "M (C)"; "D/WB/M (L)";
  // "D/WB/M/AM (R)"; "D (LC)"; "M/AM (L)"; "D/WB (RL)"; "WB (L)"; "M/AM (C)";
  // "D (RC)"; "WB/M (L)"; "D/WB/M (RL)"; "AM (LC)"; "M/AM (LC)"; "D (R)";
  // "WB (RL)"; "M/AM (RLC)"; "D (L)"; "AM (RC)"; "D (RL)"; "M (RLC)";
  // "WB/M (R)"; "M/AM (RC)"; "D/WB/AM (L)"; "WB/AM (L)"; "M (RC)";
  // "WB/M/AM (RL)"; "D/M (R)"; "GK"; "D/AM (L)"; "D/M/AM (L)"; "D/M/AM (R)";
  // "D/AM (R)"; "D/M (L)"; "WB/M (RL)"; "WB/AM (R)"; "D/M (C)"; "D/M/AM (C)";
  // "D/WB/M/AM (RL)"; "D/AM (C)"; "D/M (RC)"]

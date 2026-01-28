namespace FM21_ToolsLib.Tests

open NUnit.Framework
open FM21_ToolsLib
open System

[<TestFixture>]
type MyClubTests() =

    // helper to build a Player easily with given position string and attribute list
    let mkPlayer (name: string) (position: string) (attrs: (string * int) list) =
        {
            Rec = ""
            Inf = ""
            Name = name
            DoB = ""
            Height = ""
            Extras = Map.ofList [ ("Position", position) ]
            Attributes = Map.ofList attrs
        }

    [<Test>]
    member _.``getFirst/getSecond/getThird produce distinct selections and third is empty when only two per slot`` () =
        // Build two players for each of the 11 role slots (first should pick the higher-rated, second the lower-rated)
        let sk1 = mkPlayer "SK1" "GK" [ ("Ref",20); ("Han",19); ("Pos",18); ("Kic",15) ]
        let sk2 = mkPlayer "SK2" "GK" [ ("Ref",10); ("Han",10); ("Pos",10); ("Kic",10) ]

        let iwbR1 = mkPlayer "IWB_R1" "RB" [ ("Pas",18); ("Tec",17); ("Cro",16); ("Pac",17) ]
        let iwbR2 = mkPlayer "IWB_R2" "RB" [ ("Pas",10); ("Tec",10); ("Cro",10); ("Pac",10) ]

        let iwbL1 = mkPlayer "IWB_L1" "LB" [ ("Pas",17); ("Tec",16); ("Cro",15); ("Pac",16) ]
        let iwbL2 = mkPlayer "IWB_L2" "LB" [ ("Pas",10); ("Tec",10); ("Cro",10); ("Pac",10) ]

        let bpdA1 = mkPlayer "BPD_A1" "CB" [ ("Pas",19); ("Tec",18); ("Cmp",17); ("Tck",14) ]
        let bpdA2 = mkPlayer "BPD_A2" "CB" [ ("Pas",10); ("Tec",10); ("Cmp",10); ("Tck",10) ]

        let bpdB1 = mkPlayer "BPD_B1" "CB" [ ("Pas",18); ("Tec",17); ("Cmp",16); ("Tck",15) ]
        let bpdB2 = mkPlayer "BPD_B2" "CB" [ ("Pas",10); ("Tec",10); ("Cmp",10); ("Tck",10) ]

        let wgr1 = mkPlayer "WGR1" "MR" [ ("Cro",19); ("Pac",18); ("Acc",18); ("Dri",15) ]
        let wgr2 = mkPlayer "WGR2" "MR" [ ("Cro",10); ("Pac",10); ("Acc",10); ("Dri",10) ]

        let iwl1 = mkPlayer "IWL1" "ML" [ ("Pas",18); ("Tec",17); ("Dri",16); ("OtB",15) ]
        let iwl2 = mkPlayer "IWL2" "ML" [ ("Pas",10); ("Tec",10); ("Dri",10); ("OtB",10) ]

        let bwm1 = mkPlayer "BWM1" "MC" [ ("Tck",19); ("Mar",18); ("Agg",17); ("Sta",16) ]
        let bwm2 = mkPlayer "BWM2" "MC" [ ("Tck",10); ("Mar",10); ("Agg",10); ("Sta",10) ]

        let ap1  = mkPlayer "AP1"  "MC" [ ("Pas",19); ("Tec",18); ("OtB",17); ("Cmp",17) ]
        let ap2  = mkPlayer "AP2"  "MC" [ ("Pas",10); ("Tec",10); ("OtB",10); ("Cmp",10) ]

        let afa1 = mkPlayer "AFA1" "ST" [ ("Pac",19); ("Acc",18); ("Fin",19); ("Dri",14) ]
        let afa2 = mkPlayer "AFA2" "ST" [ ("Pac",10); ("Acc",10); ("Fin",10); ("Dri",10) ]

        let tma1 = mkPlayer "TMA1" "ST" [ ("Fin",18); ("Pac",17); ("Acc",17); ("Hea",15) ]
        let tma2 = mkPlayer "TMA2" "ST" [ ("Fin",10); ("Pac",10); ("Acc",10); ("Hea",10) ]

        // Pool contains two candidates per slot
        let pool =
            [ sk1; sk2;
              iwbR1; iwbR2;
              iwbL1; iwbL2;
              bpdA1; bpdA2;
              bpdB1; bpdB2;
              wgr1; wgr2;
              iwl1; iwl2;
              bwm1; bwm2;
              ap1; ap2;
              afa1; afa2;
              tma1; tma2 ]

        // Assign into the module global used by MY_CLUB
        HTML.MyPlayers <- pool

        let first = MY_CLUB.getFirstTeam ()
        let second = MY_CLUB.getSecondTeam ()
        let third = MY_CLUB.getThirdTeam ()

        // Extract chosen player names
        let names t = TEAM.teamAsPositionNameOptions t |> List.choose snd

        let names1 = names first
        let names2 = names second
        let names3 = names third

        // Ensure no overlap between first and second selected players (each slot had two candidates)
        Assert.IsTrue(names1 |> List.forall (fun n -> not (List.contains n names2)), "Expected no player to be selected in both first and second teams")

        // Third team should have no assigned players because all candidates were used by first + second
        Assert.IsTrue(List.isEmpty names3, "Expected third team to have no assigned players in this setup")

    [<Test>]
    member _.``getFirstTeamScore equals TEAM teamScore and getFirstTeamAsStrings contains SKD line`` () =
        // Build a canonical full 11 to ensure first team is complete
        let sk = mkPlayer "SK" "GK" [ ("Ref",20); ("Han",19); ("Pos",18); ("Kic",15) ]
        let iwbR = mkPlayer "IWB_R" "RB" [ ("Pas",18); ("Tec",17); ("Cro",16); ("Pac",17) ]
        let iwbL = mkPlayer "IWB_L" "LB" [ ("Pas",17); ("Tec",16); ("Cro",15); ("Pac",16) ]
        let bpdA = mkPlayer "BPD_A" "CB" [ ("Pas",19); ("Tec",18); ("Cmp",17); ("Tck",14) ]
        let bpdB = mkPlayer "BPD_B" "CB" [ ("Pas",18); ("Tec",17); ("Cmp",16); ("Tck",15) ]
        let wgr = mkPlayer "WGR" "MR" [ ("Cro",19); ("Pac",18); ("Acc",18); ("Dri",15) ]
        let iwl = mkPlayer "IWL" "ML" [ ("Pas",18); ("Tec",17); ("Dri",16); ("OtB",15) ]
        let bwm = mkPlayer "BWM" "MC" [ ("Tck",19); ("Mar",18); ("Agg",17); ("Sta",16) ]
        let ap  = mkPlayer "AP"  "MC" [ ("Pas",10); ("Tec",19); ("OtB",18); ("Cmp",17) ]
        let afa = mkPlayer "AFA" "ST" [ ("Pac",19); ("Acc",18); ("Fin",19); ("Dri",14) ]
        let tma = mkPlayer "TMA" "ST" [ ("Fin",18); ("Pac",17); ("Acc",17); ("Hea",15) ]

        let pool = [ sk; iwbR; iwbL; bpdA; bpdB; wgr; iwl; bwm; ap; afa; tma ]
        HTML.MyPlayers <- pool

        let first = MY_CLUB.getFirstTeam ()
        let scoreFromModule = MY_CLUB.getFirstTeamScore ()
        let scoreDirect = TEAM.teamScore first

        Assert.AreEqual(scoreDirect, scoreFromModule, "getFirstTeamScore should match TEAM.teamScore for the constructed first team")

        let lines = MY_CLUB.getFirstTeamAsStrings ()
        Assert.IsTrue(lines |> List.exists (fun s -> s.StartsWith("SKD:") && s.Contains("SK")), "Expected SKD line to include SK")

    [<Test>]
    member _.``posListForTeam returns 11 entries and SKD assigned when available`` () =
        // Reuse a full pool to build a team
        let sk = mkPlayer "SK" "GK" [ ("Ref",20); ("Han",19); ("Pos",18) ]
        let iwbR = mkPlayer "IWB_R" "RB" [ ("Pas",18); ("Tec",17); ("Cro",16) ]
        let iwbL = mkPlayer "IWB_L" "LB" [ ("Pas",17); ("Tec",16); ("Cro",15) ]
        let bpdA = mkPlayer "BPD_A" "CB" [ ("Pas",19); ("Tec",18); ("Cmp",17) ]
        let bpdB = mkPlayer "BPD_B" "CB" [ ("Pas",18); ("Tec",17); ("Cmp",16) ]
        let wgr = mkPlayer "WGR" "MR" [ ("Cro",19); ("Pac",18); ("Acc",18) ]
        let iwl = mkPlayer "IWL" "ML" [ ("Pas",18); ("Tec",17); ("Dri",16) ]
        let bwm = mkPlayer "BWM" "MC" [ ("Tck",19); ("Mar",18); ("Agg",17) ]
        let ap  = mkPlayer "AP"  "MC" [ ("Pas",10); ("Tec",19); ("OtB",18) ]
        let afa = mkPlayer "AFA" "ST" [ ("Pac",19); ("Acc",18); ("Fin",19) ]
        let tma = mkPlayer "TMA" "ST" [ ("Fin",18); ("Pac",17); ("Acc",17) ]

        let pool = [ sk; iwbR; iwbL; bpdA; bpdB; wgr; iwl; bwm; ap; afa; tma ]
        let team = TEAM.buildTeam pool

        let posList = MY_CLUB.posListForTeam team
        Assert.AreEqual(11, List.length posList, "posListForTeam should return 11 position entries")

        // ensure SKD slot is assigned and names match
        match posList |> List.tryFind (fun (r,_) -> r = "SKD") with
        | Some (_, Some rr) -> Assert.AreEqual("SK", rr.Name)
        | _ -> Assert.Fail("Expected SKD to be assigned with player SK")
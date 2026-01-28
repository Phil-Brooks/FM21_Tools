namespace FM21_ToolsLib.Tests

open NUnit.Framework
open FM21_ToolsLib
open System

[<TestFixture>]
type TeamTests() =

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
    member _.``buildTeam assigns distinct ball playing defenders and sets expected roles`` () =
        // Prepare 11 players targeted to fill each slot
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
        let team = TEAM.buildTeam pool

        // Ensure both ball playing defenders are assigned and are distinct players
        match team.BallPlayingDef1, team.BallPlayingDef2 with
        | Some p1, Some p2 ->
            Assert.AreNotEqual(p1.Name, p2.Name, "Expected two different players for BallPlayingDef1 and BallPlayingDef2")
            // RoleName for BPD picks in TEAM is assigned as "BPD1"/"BPD2"
            Assert.AreEqual("BPD1", p1.RoleName)
            Assert.AreEqual("BPD2", p2.RoleName)
        | _ -> Assert.Fail("Expected both BallPlayingDef1 and BallPlayingDef2 to be assigned")

        // Check some other canonical assignments
        Assert.AreEqual(Some "SK", team.SweeperKeeper |> Option.map (fun r -> r.Name))
        Assert.AreEqual(Some "IWB_R", team.InvertedWingBackRight |> Option.map (fun r -> r.Name))
        Assert.AreEqual(Some "IWB_L", team.InvertedWingBackLeft |> Option.map (fun r -> r.Name))
        Assert.AreEqual(Some "WGR", team.WingerAttackRight |> Option.map (fun r -> r.Name))
        Assert.AreEqual(Some "AP", team.AdvancedPlaymakerSupport |> Option.map (fun r -> r.Name))

    [<Test>]
    member _.``teamAsStrings contains human readable role lines and teamScoreOption is Some when all assigned`` () =
        // Use same pool as previous test to produce fully assigned team
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

        let lines = TEAM.teamAsStrings team
        // Expect the SKD line and that it names our SK player
        Assert.IsTrue(lines |> List.exists (fun s -> s.StartsWith("SKD:") && s.Contains("SK")), "Expected SKD line to include SK")

        // teamScoreOption should be Some when all positions were assigned
        let scoreOpt = TEAM.teamScoreOption team
        Assert.IsTrue(scoreOpt.IsSome, "Expected teamScoreOption to be Some when full team assigned")
        // teamScore returns the sum even when everything is assigned
        let score = TEAM.teamScore team
        Assert.AreEqual(score, scoreOpt.Value, "teamScore and teamScoreOption.Value should match when all positions assigned")

    [<Test>]
    member _.``teamScoreOption returns None when some positions are unassigned`` () =
        // Minimal pool: only a few players so many positions remain unassigned
        let only = [ mkPlayer "SK" "GK" [ ("Ref",20); ("Han",19) ]; mkPlayer "CB" "CB" [ ("Pas",18); ("Tck",17) ] ]
        let team = TEAM.buildTeam only

        let scoreOpt = TEAM.teamScoreOption team
        Assert.IsTrue(scoreOpt.IsNone, "Expected teamScoreOption to be None when not all positions are assigned")
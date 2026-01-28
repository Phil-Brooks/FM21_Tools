namespace FM21_ToolsLib.Tests

open NUnit.Framework
open FM21_ToolsLib
open System

[<TestFixture>]
type DivisionTests() =

    // helper to build a Player easily with given division and club
    let mkPlayer (name: string) (position: string) (attrs: (string * int) list) (based: string option) (club: string option) =
        let extras =
            [ yield ("Position", position)
              match based with | Some b -> yield ("Based", b) | None -> ()
              match club with  | Some c -> yield ("Club", c) | None -> () ]
            |> Map.ofList
        {
            Rec = ""
            Inf = ""
            Name = name
            DoB = ""
            Height = ""
            Extras = extras
            Attributes = Map.ofList attrs
        }

    [<SetUp>]
    member _.Setup() =
        // reset global player lists before each test
        HTML.AllPlayers <- []

    [<Test>]
    member _.``allDivisions returns sorted distinct non-empty based values`` () =
        let p1 = mkPlayer "P1" "ST" [ ("Fin", 10) ] (Some "DivB") None
        let p2 = mkPlayer "P2" "MC" [ ("Pas", 10) ] (Some "DivA") None
        let p3 = mkPlayer "P3" "GK" [ ("Ref", 10) ] None None // no Based -> ignored
        let p4 = mkPlayer "P4" "ST" [ ("Fin", 11) ] (Some "DivB") None // duplicate Based
        HTML.AllPlayers <- [ p1; p2; p3; p4 ]

        let divs = DIVISION.allDivisions()
        Assert.AreEqual([ "DivA"; "DivB" ], divs)

    [<Test>]
    member _.``clubsInDivision returns sorted distinct clubs for a division`` () =
        let a1 = mkPlayer "A1" "ST" [ ("Fin", 12) ] (Some "D1") (Some "ClubX")
        let a2 = mkPlayer "A2" "MC" [ ("Pas", 12) ] (Some "D1") (Some "ClubA")
        let a3 = mkPlayer "A3" "GK" [ ("Ref", 12) ] (Some "D1") (Some "ClubX")
        let other = mkPlayer "O1" "ST" [ ("Fin", 12) ] (Some "D2") (Some "ClubOther")
        HTML.AllPlayers <- [ a1; a2; a3; other ]

        let clubs = DIVISION.clubsInDivision "D1"
        Assert.AreEqual([ "ClubA"; "ClubX" ], clubs)

    [<Test>]
    member _.``playersInClub returns players for given division and club`` () =
        let pA1 = mkPlayer "P-A1" "ST" [ ("Fin", 12) ] (Some "Div1") (Some "TheClub")
        let pA2 = mkPlayer "P-A2" "MC" [ ("Pas", 12) ] (Some "Div1") (Some "TheClub")
        let pB1 = mkPlayer "P-B1" "ST" [ ("Fin", 12) ] (Some "Div1") (Some "OtherClub")
        HTML.AllPlayers <- [ pA1; pA2; pB1 ]

        let players = DIVISION.playersInClub "Div1" "TheClub"
        let names = players |> List.map (fun p -> p.Name) |> Set.ofList
        Assert.IsTrue(names.Contains "P-A1")
        Assert.IsTrue(names.Contains "P-A2")
        Assert.IsFalse(names.Contains "P-B1")

    [<Test>]
    member _.``clubTeams and bestClub prefer complete teams over incomplete ones`` () =
        // GoodClub: provide 11 players (same pool as TeamTests to produce a full team)
        let sk = mkPlayer "SK" "GK" [ ("Ref",20); ("Han",19); ("Pos",18); ("Kic",15) ] (Some "DivX") (Some "GoodClub")
        let iwbR = mkPlayer "IWB_R" "RB" [ ("Pas",18); ("Tec",17); ("Cro",16); ("Pac",17) ] (Some "DivX") (Some "GoodClub")
        let iwbL = mkPlayer "IWB_L" "LB" [ ("Pas",17); ("Tec",16); ("Cro",15); ("Pac",16) ] (Some "DivX") (Some "GoodClub")
        let bpdA = mkPlayer "BPD_A" "CB" [ ("Pas",19); ("Tec",18); ("Cmp",17); ("Tck",14) ] (Some "DivX") (Some "GoodClub")
        let bpdB = mkPlayer "BPD_B" "CB" [ ("Pas",18); ("Tec",17); ("Cmp",16); ("Tck",15) ] (Some "DivX") (Some "GoodClub")
        let wgr = mkPlayer "WGR" "MR" [ ("Cro",19); ("Pac",18); ("Acc",18); ("Dri",15) ] (Some "DivX") (Some "GoodClub")
        let iwl = mkPlayer "IWL" "ML" [ ("Pas",18); ("Tec",17); ("Dri",16); ("OtB",15) ] (Some "DivX") (Some "GoodClub")
        let bwm = mkPlayer "BWM" "MC" [ ("Tck",19); ("Mar",18); ("Agg",17); ("Sta",16) ] (Some "DivX") (Some "GoodClub")
        let ap  = mkPlayer "AP"  "MC" [ ("Pas",10); ("Tec",19); ("OtB",18); ("Cmp",17) ] (Some "DivX") (Some "GoodClub")
        let afa = mkPlayer "AFA" "ST" [ ("Pac",19); ("Acc",18); ("Fin",19); ("Dri",14) ] (Some "DivX") (Some "GoodClub")
        let tma = mkPlayer "TMA" "ST" [ ("Fin",18); ("Pac",17); ("Acc",17); ("Hea",15) ] (Some "DivX") (Some "GoodClub")
        let goodPool = [ sk; iwbR; iwbL; bpdA; bpdB; wgr; iwl; bwm; ap; afa; tma ]

        // BadClub: only a single player -> incomplete team
        let single = mkPlayer "OnlyOne" "ST" [ ("Fin",10) ] (Some "DivX") (Some "BadClub")

        HTML.AllPlayers <- (goodPool |> List.map (fun p -> p)) @ [ single ]

        let teams = DIVISION.clubTeams "DivX"
        // Expect two clubs present
        let clubNames = teams |> List.map (fun (n,_,_) -> n) |> Set.ofList
        Assert.IsTrue(clubNames.Contains "GoodClub")
        Assert.IsTrue(clubNames.Contains "BadClub")

        // bestClub should pick the complete GoodClub (BadClub will have None score -> treated as -1.0)
        let (bestName, _, bestScoreOpt) = DIVISION.bestClub "DivX"
        Assert.AreEqual("GoodClub", bestName)
        Assert.IsTrue(bestScoreOpt.IsSome)

    [<Test>]
    member _.``averageRatingsByRole returns empty for empty division and Some ratings when at least one full team exists`` () =
        // Empty division
        HTML.AllPlayers <- []
        let emptyResult = DIVISION.averageRatingsByRole "NoSuchDiv"
        Assert.AreEqual(0, List.length emptyResult)

        // Now use the GoodClub full team from previous test
        let sk = mkPlayer "SK" "GK" [ ("Ref",20); ("Han",19); ("Pos",18); ("Kic",15) ] (Some "DFull") (Some "SoloClub")
        let iwbR = mkPlayer "IWB_R" "RB" [ ("Pas",18); ("Tec",17); ("Cro",16); ("Pac",17) ] (Some "DFull") (Some "SoloClub")
        let iwbL = mkPlayer "IWB_L" "LB" [ ("Pas",17); ("Tec",16); ("Cro",15); ("Pac",16) ] (Some "DFull") (Some "SoloClub")
        let bpdA = mkPlayer "BPD_A" "CB" [ ("Pas",19); ("Tec",18); ("Cmp",17); ("Tck",14) ] (Some "DFull") (Some "SoloClub")
        let bpdB = mkPlayer "BPD_B" "CB" [ ("Pas",18); ("Tec",17); ("Cmp",16); ("Tck",15) ] (Some "DFull") (Some "SoloClub")
        let wgr = mkPlayer "WGR" "MR" [ ("Cro",19); ("Pac",18); ("Acc",18); ("Dri",15) ] (Some "DFull") (Some "SoloClub")
        let iwl = mkPlayer "IWL" "ML" [ ("Pas",18); ("Tec",17); ("Dri",16); ("OtB",15) ] (Some "DFull") (Some "SoloClub")
        let bwm = mkPlayer "BWM" "MC" [ ("Tck",19); ("Mar",18); ("Agg",17); ("Sta",16) ] (Some "DFull") (Some "SoloClub")
        let ap  = mkPlayer "AP"  "MC" [ ("Pas",10); ("Tec",19); ("OtB",18); ("Cmp",17) ] (Some "DFull") (Some "SoloClub")
        let afa = mkPlayer "AFA" "ST" [ ("Pac",19); ("Acc",18); ("Fin",19); ("Dri",14) ] (Some "DFull") (Some "SoloClub")
        let tma = mkPlayer "TMA" "ST" [ ("Fin",18); ("Pac",17); ("Acc",17); ("Hea",15) ] (Some "DFull") (Some "SoloClub")
        HTML.AllPlayers <- [ sk; iwbR; iwbL; bpdA; bpdB; wgr; iwl; bwm; ap; afa; tma ]

        let averages = DIVISION.averageRatingsByRole "DFull"
        // When there is at least one full team we expect canonical 11 role entries and each should have Some average
        Assert.AreEqual(11, List.length averages)
        Assert.IsTrue(averages |> List.forall (fun (_, vOpt) -> vOpt.IsSome))
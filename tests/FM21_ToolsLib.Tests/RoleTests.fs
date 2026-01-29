namespace FM21_ToolsLib.Tests

open NUnit.Framework
open FM21_ToolsLib
open System

[<TestFixture>]
type RoleTests() =

    // helper to build a Player easily
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
    member _.``getRelevantAttributesForRole returns expected keys for known roles and empty for unknown`` () =
        let tmaAttrs = ROLE.getRelevantAttributesForRole "TMA"
        Assert.IsTrue(List.contains "Fin" tmaAttrs, "TMA should include 'Fin'")
        Assert.IsTrue(List.contains "Pac" tmaAttrs, "TMA should include 'Pac'")

        let bpdAttrs = ROLE.getRelevantAttributesForRole "BPD #1"
        Assert.IsTrue(List.contains "Pas" bpdAttrs, "BPD* should include 'Pas'")

        let unknown = ROLE.getRelevantAttributesForRole "XYZ"
        Assert.AreEqual(0, List.length unknown, "Unknown role should return empty list")

    [<Test>]
    member _.``roleRatingsForPlayer includes roles that match the player's position`` () =
        // Build a striker whose attributes make him relevant to ST-based roles
        let attrs =
            [ ("Fin", 18); ("Pac", 17); ("Acc", 16); ("Cmp", 15); ("Dri", 14); ("Fir", 12); ("Hea", 10) ]
        let p = mkPlayer "Striker" "ST" attrs

        let roles = ROLE.roleRatingsForPlayer p |> List.map fst |> Set.ofList
        Assert.IsTrue(roles.Contains "TMA", "Expected TMA to be present for an ST")
        Assert.IsTrue(roles.Contains "AFA", "Expected AFA to be present for an ST")

    [<Test>]
    member _.``bestTargetMenAttackNames orders players by Target Man rating`` () =
        // High-rated and low-rated STs
        let highAttrs = [ ("Fin", 20); ("Pac", 18); ("Acc", 18); ("Cmp", 16); ("Dri", 10); ("Fir", 12); ("Hea", 12) ]
        let lowAttrs  = [ ("Fin", 6);  ("Pac", 6);  ("Acc", 6);  ("Cmp", 6);  ("Dri", 6);  ("Fir", 6);  ("Hea", 6) ]

        let high = mkPlayer "High" "ST" highAttrs
        let low  = mkPlayer "Low"  "ST" lowAttrs

        let names = ROLE.bestTargetMenAttackNames [ low; high ] 2
        Assert.AreEqual("High", List.head names, "Expected the higher-rated Target Man to appear first")

    [<Test>]
    member _.``weakestRelevantAttributeForPlayer identifies the single weakest relevant attribute`` () =
        // Make a striker with one very low relevant stat ("Pac")
        let attrs =
            [ ("Pac", 1); ("Fin", 15); ("Acc", 14); ("Cmp", 13); ("Dri", 12); ("Fir", 11); ("Hea", 10); ("OtB", 3);
              ("Pas", 3); ("Tec", 3); ("Ant", 3); ("Agi", 3); ("Jum", 3); ("Bal", 3); ("Str", 3); ("Sta", 3) ]
        let p = mkPlayer "WeakPac" "ST" attrs

        match ROLE.bestRoleRatedPlayer p with
        | None -> Assert.Fail("Expected a best role for the test player")
        | Some rr ->
            match ROLE.weakestRelevantAttributeForPlayer rr with
            | None -> Assert.Fail("Expected a weakest relevant attribute")
            | Some (_roleAbbrev, playerName, attr, value) ->
                Assert.AreEqual("WeakPac", playerName)
                Assert.AreEqual("Pac", attr)
                Assert.AreEqual(1, value)

    [<Test>]
    member _.``getBest orders by rating desc and by market value asc and respects value filter`` () =
        // Build two players with identical high attributes (equal rating) but different market values,
        // plus one clearly lower-rated player.
        let highAttrs = [ ("Fin", 20); ("Pac", 18); ("Acc", 18); ("Cmp", 16); ("Dri", 14); ("Fir", 12); ("Hea", 12) ]
        let medAttrs  = [ ("Fin", 15); ("Pac", 14); ("Acc", 14); ("Cmp", 13); ("Dri", 12); ("Fir", 11); ("Hea", 10) ]

        let high = mkPlayer "High" "ST" highAttrs
        let cheapHigh = mkPlayer "CheapHigh" "ST" highAttrs
        let med = mkPlayer "Med" "ST" medAttrs

        // attach Value and Club extras
        let high = { high with Extras = Map.ofList [ ("Position", "ST"); ("Value", "£2.0M"); ("Club", "Big FC") ] }
        let cheapHigh = { cheapHigh with Extras = Map.ofList [ ("Position", "ST"); ("Value", "£500K"); ("Club", "Small FC") ] }
        let med = { med with Extras = Map.ofList [ ("Position", "ST"); ("Value", "£300K"); ("Club", "Mid FC") ] }

        // Ensure the HTML.SctPlayers source used by SCOUT is our controlled list
        HTML.SctPlayers <- [ high; cheapHigh; med ]

        // Use a low threshold so both high and cheapHigh are included.
        let resultsAll = SCOUT.getBest "TMA" 0.0 10000
        // Expect CheapHigh to come before High because ratings equal and market value lower
        Assert.IsTrue(List.length resultsAll >= 2, "Expected at least two results")
        let (firstName, firstClub, _, _) = List.head resultsAll
        Assert.AreEqual("CheapHigh", firstName)
        Assert.AreEqual("Small FC", firstClub)

        // Now apply a value filter that excludes the expensive "High" player (maxValueK = 1000 => £1,000,000)
        let resultsFiltered = SCOUT.getBest "TMA" 0.0 1000
        // CheapHigh (500K) and Med (300K) should remain; High (2M) should be excluded.
        let namesFiltered = resultsFiltered |> List.map (fun (n,_,_,_) -> n)
        Assert.IsTrue(List.contains "CheapHigh" namesFiltered, "CheapHigh should be included under value filter")
        Assert.IsFalse(List.contains "High" namesFiltered, "High should be excluded by value filter")

    [<Test>]
    member _.``getTrLst filters to transfer-listed players and respects value filter`` () =
        // Build two high-rated STs and one medium-rated; mark two as transfer-listed.
        let highAttrs = [ ("Fin", 20); ("Pac", 18); ("Acc", 18); ("Cmp", 16); ("Dri", 14); ("Fir", 12); ("Hea", 12) ]
        let medAttrs  = [ ("Fin", 15); ("Pac", 14); ("Acc", 14); ("Cmp", 13); ("Dri", 12); ("Fir", 11); ("Hea", 10) ]

        let high = mkPlayer "High" "ST" highAttrs
        let cheapHigh = mkPlayer "CheapHigh" "ST" highAttrs
        let med = mkPlayer "Med" "ST" medAttrs

        // attach Value, Club and TransferStatus extras
        let high = { high with Extras = Map.ofList [ ("Position", "ST"); ("Value", "£2.0M");   ("Club", "Big FC");   ("TransferStatus", "") ] }
        let cheapHigh = { cheapHigh with Extras = Map.ofList [ ("Position", "ST"); ("Value", "£500K"); ("Club", "Small FC"); ("TransferStatus", "Transfer Listed") ] }
        let med = { med with Extras = Map.ofList [ ("Position", "ST"); ("Value", "£300K");   ("Club", "Mid FC");   ("TransferStatus", "Transfer Listed") ] }

        // Use controlled source
        HTML.SctPlayers <- [ high; cheapHigh; med ]

        // No value limit: should return only the transfer-listed players (cheapHigh, med), ordered by rating then value.
        let resultsAll = SCOUT.getTrLst "TMA" 0.0 10000
        Assert.AreEqual(2, List.length resultsAll, "Expected two transfer-listed results")
        let (firstName, firstClub, _, _) = List.head resultsAll
        Assert.AreEqual("CheapHigh", firstName)
        Assert.AreEqual("Small FC", firstClub)

        // Apply value filter that excludes CheapHigh (500K) but keeps Med (300K)
        let resultsFiltered = SCOUT.getTrLst "TMA" 0.0 400
        let namesFiltered = resultsFiltered |> List.map (fun (n,_,_,_) -> n)
        Assert.IsTrue(List.contains "Med" namesFiltered, "Med (300K) should be included under value filter")
        Assert.IsFalse(List.contains "CheapHigh" namesFiltered, "CheapHigh (500K) should be excluded by value filter")


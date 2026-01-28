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
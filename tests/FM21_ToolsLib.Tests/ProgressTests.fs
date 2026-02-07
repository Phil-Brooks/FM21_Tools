namespace FM21_ToolsLib.Tests

open NUnit.Framework
open FM21_ToolsLib
open System

[<TestFixture>]
type ProgressTests() =

    // helper to build a Player easily with given position string and attribute list
    let mkPlayer (name: string) (position: string) (attrs: (string * int) list) =
        {
            Rec = ""
            Inf = ""
            Name = name
            DoB = ""
            Height = ""   // keep heights identical so matching works
            Extras = Map.ofList [ ("Position", position) ]
            Attributes = Map.ofList attrs
        }

    [<Test>]
    member _.``progressClub produces numeric progress for matched players and N/A for new players, numeric entries first`` () =
        // Old version of the player (lower rating)
        let oldP = mkPlayer "Foo" "ST" [ ("Pac",10); ("Acc",10); ("Fin",10); ("Dri",10) ]
        // New version of the same player (higher rating)
        let newP = mkPlayer "Foo" "ST" [ ("Pac",14); ("Acc",14); ("Fin",14); ("Dri",14) ]
        // A player only present in current MyPlayers (no previous rating)
        let onlyNew = mkPlayer "Bar" "MC" [ ("Pas",12); ("Tec",12); ("OtB",12); ("Cmp",12) ]

        // Build RoleRatedPlayer entries for OldPlayers (mimic loadOldPlayers behaviour)
        let oldRR =
            match ROLE.bestRoleRatedPlayer oldP with
            | Some r -> r
            | None -> Assert.Fail("oldP did not produce a RoleRatedPlayer"); Unchecked.defaultof<_>

        // Assign OldPlayers and current HTML.MyPlayers
        PROGRESS.OldPlayers <- [ oldRR ]
        HTML.MyPlayers <- [ newP; onlyNew ]

        // Run the function under test
        let results = PROGRESS.progressClub()

        // Expect at least two entries
        Assert.GreaterOrEqual(results.Length, 2, "Expected at least two result lines")

        // Find lines for Foo and Bar
        let fooLine = results |> List.find (fun s -> s.Contains("Name: Foo"))
        let barLine = results |> List.find (fun s -> s.Contains("Name: Bar"))

        // Compute expected numeric progress
        let newRR =
            match ROLE.bestRoleRatedPlayer newP with
            | Some r -> r
            | None -> Assert.Fail("newP did not produce a RoleRatedPlayer"); Unchecked.defaultof<_>

        let expectedProgress = newRR.Rating - oldRR.Rating
        let expectedProgressStr = sprintf "%.2f" expectedProgress

        // Foo should show numeric progress equal to computed difference
        Assert.IsTrue(fooLine.Contains("Progress: " + expectedProgressStr),
                      sprintf "Expected Foo progress to be %s, got: %s" expectedProgressStr fooLine)

        // Bar should show N/A (no previous rating)
        Assert.IsTrue(barLine.Contains("Progress: N/A"), sprintf "Expected Bar to show N/A progress, got: %s" barLine)

        // Ensure numeric progress entries come before N/A entries (Foo before Bar)
        let idxFoo = results |> List.findIndex (fun s -> s.Contains("Name: Foo"))
        let idxBar = results |> List.findIndex (fun s -> s.Contains("Name: Bar"))
        Assert.Less(idxFoo, idxBar, "Expected players with numeric progress to appear before entries with N/A")

    [<Test>]
    member _.``topImprovementsFromCurPlayers returns top N improvements ordered descending and includes club, role, height, rating, improvement`` () =
        // create a DoB that ensures players are under 21 at test runtime (20 years ago from today)
        let dobUnder21 = DateTime.UtcNow.AddYears(-20).ToString("yyyy-MM-dd")

        // helper that includes Club and fixed Height so matching works
        let mkPlayerWithClub (name: string) (position: string) (attrs: (string * int) list) (club: string) =
            {
                Rec = ""
                Inf = ""
                Name = name
                DoB = dobUnder21
                Height = "180"
                Extras = Map.ofList [ ("Position", position); ("Club", club) ]
                Attributes = Map.ofList attrs
            }

        // Old players
        let oldA = mkPlayerWithClub "Alice" "ST" [ ("Pac",10); ("Acc",10); ("Fin",10); ("Dri",10) ] "OldFC"
        let oldB = mkPlayerWithClub "Bob"   "ST" [ ("Pac",15); ("Acc",15); ("Fin",15); ("Dri",15) ] "OldFC"
        let oldC = mkPlayerWithClub "Charlie" "ST" [ ("Pac",12); ("Acc",12); ("Fin",12); ("Dri",12) ] "OldFC"

        // Current players (Alice big improvement, Charlie small improvement, Bob slight drop)
        let curA = mkPlayerWithClub "Alice" "ST" [ ("Pac",20); ("Acc",20); ("Fin",20); ("Dri",20) ] "NewFC"
        let curB = mkPlayerWithClub "Bob"   "ST" [ ("Pac",13); ("Acc",13); ("Fin",13); ("Dri",13) ] "NewFC"
        let curC = mkPlayerWithClub "Charlie" "ST" [ ("Pac",13); ("Acc",13); ("Fin",13); ("Dri",13) ] "NewFC"

        let toRR p =
            match ROLE.bestRoleRatedPlayer p with
            | Some r -> r
            | None -> Assert.Fail(sprintf "player %s did not produce a RoleRatedPlayer" p.Name); Unchecked.defaultof<_>

        let oldRRs = [ toRR oldA; toRR oldB; toRR oldC ]
        let curRRs = [ toRR curA; toRR curB; toRR curC ]

        // Assign to PROGRESS mutable lists used by the function under test
        PROGRESS.OldPlayers <- oldRRs
        PROGRESS.CurPlayers <- curRRs

        // Request top 2 improvements
        let top2 = PROGRESS.topImprovementsFromCurPlayers 2

        // Should return exactly 2 entries
        Assert.AreEqual(2, top2.Length)

        // Unpack first and second entries
        let (name1, club1, role1, height1, rating1, improvement1) = top2.[0]
        let (name2, club2, role2, height2, rating2, improvement2) = top2.[1]

        // First must be Alice (largest improvement)
        Assert.AreEqual("Alice", name1, "Expected top improver to be Alice")
        // Club should reflect the current player's Club value
        Assert.AreEqual("NewFC", club1, "Expected club field to be taken from current player Extras")
        // Height should match the Height we set
        Assert.AreEqual("180", height1, "Expected height to be preserved")
        // Ensure ordering by improvement descending
        Assert.Greater(improvement1, improvement2, "Expected first improvement to be larger than second")

        // Verify numeric improvement equals cur.Rating - old.Rating
        let curAliceRR = toRR curA
        let oldAliceRR = toRR oldA
        let expectedAliceImprovement = curAliceRR.Rating - oldAliceRR.Rating
        Assert.AreEqual(expectedAliceImprovement, improvement1, 1e-6, sprintf "Expected Alice improvement %f" expectedAliceImprovement)
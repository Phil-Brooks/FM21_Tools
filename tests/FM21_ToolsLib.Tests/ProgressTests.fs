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
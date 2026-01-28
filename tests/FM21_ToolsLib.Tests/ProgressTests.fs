namespace FM21_ToolsLib.Tests

open NUnit.Framework
open FM21_ToolsLib
open System
open System.IO

[<TestFixture>]
type ProgressTests() =

    // helper to build a Player easily for constructing RoleRatedPlayer
    let mkPlayer (name: string) (height: string) (extras: (string * string) list) (attrs: (string * int) list) =
        {
            Rec = ""
            Inf = ""
            Name = name
            DoB = ""
            Height = height
            Extras = Map.ofList extras
            Attributes = Map.ofList attrs
        }

    let mkRRPlayer name role rating height =
        { Name = name; RoleName = role; Rating = rating; Player = mkPlayer name height [ ("Position", "ST") ] [] }

    [<Test>]
    member _.``progressForRoleRatedPlayer returns None when there are no old players`` () =
        // Ensure OldPlayers is empty
        PROGRESS.OldPlayers <- []
        let rr = mkRRPlayer "Alice" "TMA" 12.0 "175"
        let result = PROGRESS.progressForRoleRatedPlayer rr
        Assert.IsFalse(result.Progress.IsSome, "Expected Progress to be None when no previous players are loaded")
        Assert.AreEqual(rr, result.RRPlayer)

    [<Test>]
    member _.``progressForRoleRatedPlayer matches by name case-insensitive and trimmed height`` () =
        // Old player record with extra whitespace and different case in name/height
        let old = mkRRPlayer "Bob " "TMA" 10.0 " 180 "
        PROGRESS.OldPlayers <- [ old ]

        let current = mkRRPlayer "bob" "TMA" 12.5 "180"
        let res = PROGRESS.progressForRoleRatedPlayer current

        Assert.IsTrue(res.Progress.IsSome, "Expected progress to be calculated when matching old player exists")
        Assert.AreEqual(2.5, res.Progress.Value, 1e-9, "Progress should equal current.Rating - old.Rating")
        // cleanup
        PROGRESS.OldPlayers <- []

    [<Test>]
    member _.``loadOldPlayers sets OldPlayers to empty when HTML file has no table`` () =
        // Create a temporary file with content that will not parse into players
        let tmp = Path.GetTempFileName()
        try
            File.WriteAllText(tmp, "<html><body>No table here</body></html>")
            // Should not throw; OldPlayers should become empty list
            PROGRESS.loadOldPlayers(tmp)
            Assert.AreEqual(0, List.length PROGRESS.OldPlayers, "Expected OldPlayers to be empty when HTML contains no player table")
        finally
            try File.Delete(tmp) with | _ -> ()
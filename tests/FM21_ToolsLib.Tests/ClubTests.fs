namespace FM21_ToolsLib.Tests

open NUnit.Framework
open FM21_ToolsLib

[<TestFixture>]
type ClubTests() =

    // small helper to build Player records with optional club
    let mkPlayer (name: string) (clubOpt: string option) =
        let extras =
            match clubOpt with
            | Some c -> Map.ofList [ ("Club", c) ]
            | None -> Map.empty
        {
            Rec = ""
            Inf = ""
            Name = name
            DoB = ""
            Height = ""
            Extras = extras
            Attributes = Map.empty
        }

    [<Test>]
    member _.``allClubs returns sorted distinct non-empty club names`` () =
        let original = HTML.AllPlayers
        try
            let p1 = mkPlayer "P1" (Some "Zeta")
            let p2 = mkPlayer "P2" (Some "Alpha")
            let p3 = mkPlayer "P3" (Some "Alpha")   // duplicate
            let p4 = mkPlayer "P4" (Some "")        // empty -> ignored
            let p5 = mkPlayer "P5" (Some "   ")     // whitespace -> ignored
            let p6 = mkPlayer "P6" None             // no club -> ignored

            HTML.AllPlayers <- [ p1; p2; p3; p4; p5; p6 ]

            let clubs = CLUB.allClubs()
            Assert.AreEqual([ "Alpha"; "Zeta" ], clubs)
        finally
            HTML.AllPlayers <- original

    [<Test>]
    member _.``allClubs returns empty list when no valid clubs present`` () =
        let original = HTML.AllPlayers
        try
            let p1 = mkPlayer "P1" (Some "")
            let p2 = mkPlayer "P2" (Some " ")
            let p3 = mkPlayer "P3" None

            HTML.AllPlayers <- [ p1; p2; p3 ]

            let clubs = CLUB.allClubs()
            Assert.IsEmpty(clubs)
        finally
            HTML.AllPlayers <- original
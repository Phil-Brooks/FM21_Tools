namespace FM21_ToolsLib.Tests

open NUnit.Framework
open FM21_ToolsLib
open System

[<TestFixture>]
type ScoutTests() =

    // helper to build a Player easily
    let mkPlayer (name: string) (position: string) (attrs: (string * int) list) (extras: (string * string) list) =
        {
            Rec = ""
            Inf = ""
            Name = name
            DoB = "" // placeholder; use mkPlayerWithDoB when an explicit DoB is required
            Height = ""
            // We'll construct Player manually with proper DoB when needed.
            Extras = Map.ofList (("Position", position) :: extras)
            Attributes = Map.ofList attrs
        }

    // Lightweight builder to create a Player with explicit DoB handling
    let mkPlayerWithDoB (name: string) (position: string) (attrs: (string * int) list) (extras: (string * string) list) (dob: string) =
        {
            Rec = ""
            Inf = ""
            Name = name
            DoB = dob
            Height = ""
            Extras = Map.ofList (("Position", position) :: extras)
            Attributes = Map.ofList attrs
        }

    // Helper to build RoleRatedPlayer easily
    let mkRR name role rating (player: Player) =
        { Name = name; RoleName = role; Rating = rating; Player = player }

    [<TearDown>]
    member _.TearDown() =
        // reset global SctPlayers to avoid affecting other tests
        HTML.SctPlayers <- []

    [<Test>]
    member _.``rRPlayerReport returns name club and rating`` () =
        let p = mkPlayerWithDoB "Alice" "ST" [ ("Fin", 12) ] [ ("Club", "FC Test") ] ""
        let rr = mkRR "Alice" "TMA" 7.5 p
        let (n, c, h, r) = SCOUT.rRPlayerReport rr
        Assert.AreEqual("Alice", n)
        Assert.AreEqual("FC Test", c)
        Assert.AreEqual("", h)
        Assert.AreEqual(7.5, r)

    [<Test>]
    member _.``roleRatedPlayerValueBelowK parses various money formats and compares correctly`` () =
        let p185 = mkPlayerWithDoB "P185" "ST" [] [ ("Value", "£185K") ] ""
        let rr185 = mkRR "P185" "TMA" 5.0 p185
        Assert.IsTrue(SCOUT.roleRatedPlayerValueBelowK 200 rr185) // 185K <= 200K
        Assert.IsFalse(SCOUT.roleRatedPlayerValueBelowK 100 rr185) // 185K > 100K

        let p1m2 = mkPlayerWithDoB "P1M2" "CB" [] [ ("Value", "€1.2M") ] ""
        let rr1m2 = mkRR "P1M2" "BPD1" 5.0 p1m2
        Assert.IsTrue(SCOUT.roleRatedPlayerValueBelowK 1300 rr1m2) // 1.2M == 1200K <= 1300K
        Assert.IsFalse(SCOUT.roleRatedPlayerValueBelowK 1000 rr1m2)

        let pFree = mkPlayerWithDoB "PFree" "MC" [] [ ("Value", "Free Transfer") ] ""
        let rrFree = mkRR "PFree" "AP" 5.0 pFree
        Assert.IsTrue(SCOUT.roleRatedPlayerValueBelowK 1 rrFree) // Free -> 0 <= 1K

    [<Test>]
    member _.``roleRatedPlayerLoanListed and TransferListed detect tokens correctly`` () =
        let ploan = mkPlayerWithDoB "Loaned" "ST" [] [ ("LoanStatus", "On Loan") ] ""
        let rloan = mkRR "Loaned" "TMA" 4.0 ploan
        Assert.IsTrue(SCOUT.roleRatedPlayerLoanListed rloan)

        let pnotloan = mkPlayerWithDoB "NotLoaned" "ST" [] [ ("LoanStatus", "") ] ""
        let rnotloan = mkRR "NotLoaned" "TMA" 4.0 pnotloan
        Assert.IsFalse(SCOUT.roleRatedPlayerLoanListed rnotloan)

        let ptrans = mkPlayerWithDoB "Trans" "ST" [] [ ("TransferStatus", "Transfer Listed") ] ""
        let rtrans = mkRR "Trans" "TMA" 4.0 ptrans
        Assert.IsTrue(SCOUT.roleRatedPlayerTransferListed rtrans)

        let pnottrans = mkPlayerWithDoB "NotTrans" "ST" [] [ ("TransferStatus", "Unavailable") ] ""
        let rnottrans = mkRR "NotTrans" "TMA" 4.0 pnottrans
        Assert.IsFalse(SCOUT.roleRatedPlayerTransferListed rnottrans)

    [<Test>]
    member _.``roleRatedPlayerAgeBelow uses DoB parsing and fixed reference date`` () =
        today <- DateTime(2020, 8, 31)
        // Born 2002-01-01 -> age at 2020-08-31 is 18
        let p2002 = mkPlayerWithDoB "Young" "ST" [] [] "01/01/2002"
        let rr2002 = mkRR "Young" "TMA" 5.0 p2002
        Assert.IsTrue(SCOUT.roleRatedPlayerAgeBelow 19 rr2002)
        Assert.IsFalse(SCOUT.roleRatedPlayerAgeBelow 18 rr2002)

        // Year-only fallback: "2004" -> Jan 1, 2004 -> age 16 at 2020-08-31
        let p2004 = mkPlayerWithDoB "Younger" "ST" [] [] "2004"
        let rr2004 = mkRR "Younger" "TMA" 5.0 p2004
        Assert.IsTrue(SCOUT.roleRatedPlayerAgeBelow 17 rr2004)
        Assert.IsFalse(SCOUT.roleRatedPlayerAgeBelow 16 rr2004)

    [<Test>]
    member _.``getSctPlayersForRoleAbove returns players from HTMLSctPlayers when role matches`` () =
        // Create striker that should match TMA/AFA logic (Position contains ST)
        let strikerAttrs = [ ("Fin", 18); ("Pac", 17); ("Acc", 16); ("Cmp", 15); ("Dri", 14); ("Fir", 12); ("Hea", 10) ]
        let striker = mkPlayerWithDoB "Striker" "ST" strikerAttrs [] ""
        HTML.SctPlayers <- [ striker ]

        // Use threshold 0.0 to include any rated player whose role applies
        let listed = SCOUT.getSctPlayersForRoleAbove "TMA" 0.0
        Assert.IsTrue(listed |> List.exists (fun r -> r.Name = "Striker"))

        // Ball Playing Defender mapping via "BPD" prefix
        let defenderAttrs = [ ("Pas", 18); ("Tec", 17); ("Cmp", 16); ("Tck", 15) ]
        let defender = mkPlayerWithDoB "Def" "CB" defenderAttrs [] ""
        HTML.SctPlayers <- [ defender ]
        let bpdListed = SCOUT.getSctPlayersForRoleAbove "BPD1" 0.0
        Assert.IsTrue(bpdListed |> List.exists (fun r -> r.Name = "Def"))

    [<Test>]
    member _.``getLnLst returns only loan-listed players for a role and respects value limit`` () =
        // create a striker that will be rated for TMA and is loan-listed
        let strikerAttrs = [ ("Fin", 18); ("Pac", 17); ("Acc", 16); ("Cmp", 15); ("Dri", 14); ("Fir", 12); ("Hea", 10) ]
        let loaned = mkPlayerWithDoB "LoanedStriker" "ST" strikerAttrs [ ("LoanStatus", "On Loan"); ("Value", "£50K") ] ""
        HTML.SctPlayers <- [ loaned ]

        // no value limit (maxValueK = 0) should include the loaned striker
        let listed = SCOUT.getLnLst "TMA" 0.0 0
        Assert.IsTrue(listed |> List.exists (fun (n,_,_,_) -> n = "LoanedStriker"))

        // set a value cap lower than the player's value to exclude them
        let listedUnder10K = SCOUT.getLnLst "TMA" 0.0 10
        Assert.IsFalse(listedUnder10K |> List.exists (fun (n,_,_,_) -> n = "LoanedStriker"))

        // now a non-loan-listed player should not appear
        let notLoaned = mkPlayerWithDoB "NotLoanedStriker" "ST" strikerAttrs [ ("LoanStatus", "") ] ""
        HTML.SctPlayers <- [ notLoaned ]
        let listed2 = SCOUT.getLnLst "TMA" 0.0 0
        Assert.IsFalse(listed2 |> List.exists (fun (n,_,_,_) -> n = "NotLoanedStriker"))

    [<Test>]
    member _.``getYng returns players below maxAge and respects value and age limits`` () =
        today <- DateTime(2020, 8, 31)
        // create two strikers: one young (2004 -> age 16 at ref date) and one older (1995 -> age 25)
        let strikerAttrs = [ ("Fin", 18); ("Pac", 17); ("Acc", 16); ("Cmp", 15); ("Dri", 14); ("Fir", 12); ("Hea", 10) ]
        let young = mkPlayerWithDoB "YoungStriker" "ST" strikerAttrs [ ("Value", "£50K") ] "2004"
        let old = mkPlayerWithDoB "OldStriker" "ST" strikerAttrs [ ("Value", "£30K") ] "01/01/1995"
        HTML.SctPlayers <- [ young; old ]

        // maxAge 18 should include only the younger player
        let listedUnder18 = SCOUT.getYng "TMA" 0.0 0 18
        Assert.IsTrue(listedUnder18 |> List.exists (fun (n,_,_,_) -> n = "YoungStriker"))
        Assert.IsFalse(listedUnder18 |> List.exists (fun (n,_,_,_) -> n = "OldStriker"))

        // maxAge <= 0 means no age limit -> both players appear
        let listedNoAgeLimit = SCOUT.getYng "TMA" 0.0 0 0
        Assert.IsTrue(listedNoAgeLimit |> List.exists (fun (n,_,_,_) -> n = "YoungStriker"))
        Assert.IsTrue(listedNoAgeLimit |> List.exists (fun (n,_,_,_) -> n = "OldStriker"))

        // value cap of 40K with maxAge 30 should include only the older (30K) and exclude the younger (50K)
        let listedValueCap = SCOUT.getYng "TMA" 0.0 40 30
        Assert.IsFalse(listedValueCap |> List.exists (fun (n,_,_,_) -> n = "YoungStriker"))
        Assert.IsTrue(listedValueCap |> List.exists (fun (n,_,_,_) -> n = "OldStriker"))

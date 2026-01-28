namespace FM21_ToolsLib.Tests

open NUnit.Framework
open System.IO
open FM21_ToolsLib

[<TestFixture>]
type HTMLTests() =

    [<SetUp>]
    member _.Setup () =
        // reset mutable lists before each test
        HTML.AllPlayers <- []
        HTML.MyPlayers <- []
        HTML.SctPlayers <- []

    [<Test>]
    member _.``parse table produces one player with decoded fields and parsed attributes`` () =
        // Build 58 cells (indices 0..57). First row will be header; second row is player.
        let cells = Array.init 58 (fun i -> sprintf "cell%d" i)
        cells.[0] <- "R1"                     // Rec
        cells.[1] <- "I1"                     // Inf
        cells.[2] <- "O&apos;Neill"           // Name (HTML encoded apostrophe)
        cells.[3] <- "1990-01-01"             // DoB
        cells.[4] <- "180cm"                  // Height

        // extras 5..10
        cells.[5] <- "100k"
        cells.[6] <- "OnTransfer"
        cells.[7] <- "Loan"
        cells.[8] <- "ST"
        cells.[9] <- "BasedTown"
        cells.[10] <- "MyClub"

        // numeric attributes 11..57 -> produce values 1..47 (strings)
        for i in 11 .. 57 do
            cells.[i] <- sprintf "%d" (i - 10)

        // header row (58 th cells) and one player row
        let headerTds = cells |> Array.map (fun _ -> "<th>h</th>") |> String.concat ""
        let rowTds = cells |> Array.map (fun v -> sprintf "<td>%s</td>" v) |> String.concat ""
        let html = sprintf "<html><body><table><tr>%s</tr><tr>%s</tr></table></body></html>" headerTds rowTds

        let path = Path.GetTempFileName()
        try
            File.WriteAllText(path, html)
            // call loader that writes parsed players into HTML.AllPlayers
            HTML.loadPlayers path

            Assert.AreEqual(1, List.length HTML.AllPlayers, "Expected one parsed player")
            let p = List.head HTML.AllPlayers

            Assert.AreEqual("R1", p.Rec)
            Assert.AreEqual("I1", p.Inf)
            Assert.AreEqual("O'Neill", p.Name, "HTML decoding should convert &apos; to '")
            Assert.AreEqual("180cm", p.Height)
            Assert.AreEqual("100k", p.Extras.["Value"])

            // first numeric attribute key is "Acc" -> cell at index 11 which we set to "1"
            Assert.AreEqual(1, p.Attributes.["Acc"])
            // another attribute check, e.g. "Bal" is third in numericNames -> cell value 3
            Assert.AreEqual(3, p.Attributes.["Bal"]) // sanity check: corresponds to the correct index in numeric list
        finally
            try File.Delete(path) with _ -> ()

    [<Test>]
    member _.``loadMyPlayers with no table produces empty list`` () =
        let path = Path.GetTempFileName()
        try
            File.WriteAllText(path, "this file contains no table tags")
            HTML.loadMyPlayers path
            Assert.AreEqual(0, List.length HTML.MyPlayers)
        finally
            try File.Delete(path) with _ -> ()
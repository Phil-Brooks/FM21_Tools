namespace FM21_ToolsLib

open System
open System.IO
open System.Text.RegularExpressions
open System.Net

module HTML =

    let mutable AllPlayers: Player list = []
    let mutable MyPlayers: Player list = []
    let mutable SctPlayers: Player list = []

    let private stripHtml (s: string) =
        Regex.Replace(s, "<.*?>", "", RegexOptions.Singleline)
        |> WebUtility.HtmlDecode
        |> fun s -> s.Trim()

    let private parseIntSafe (s: string) =
        let input = if isNull s then "" else s
        let cleaned = Regex.Replace(input, "[^0-9-]", "")
        match Int32.TryParse(cleaned) with
        | true, v -> v
        | _ -> 0

    // shared names used by both loaders
    let private extraNames = [ "Value"; "TransferStatus"; "LoanStatus"; "Position"; "Based"; "Club" ]
    let private numericNames = [
        "Acc"; "Agi"; "Bal"; "Jum"; "Nat"; "Pac"; "Sta"; "Str"; "Agg"; "Ant"; "Bra"; "Cmp";
        "Cnt"; "Dec"; "Det"; "Fla"; "Ldr"; "OtB"; "Pos"; "Tea"; "Vis"; "Wor"; "Cor"; "Cro";
        "Dri"; "Fin"; "Fir"; "Fre"; "Hea"; "Lon"; "LTh"; "Mar"; "Pas"; "Pen"; "Tck"; "Tec";
        "Aer"; "Cmd"; "Com"; "OneVOne"; "Han"; "Kic"; "Ecc"; "Pun"; "Ref"; "TRO"; "Thr"
    ]

    let private parsePlayersFromHtmlContent (content: string) =
        let tableM = Regex.Match(content, "(?is)<table.*?>(.*?)</table>")
        if not tableM.Success then
            []
        else
            let tableInner = tableM.Groups.[1].Value
            let trMatches = Regex.Matches(tableInner, "(?is)<tr.*?>(.*?)</tr>")
            if trMatches.Count <= 1 then
                []
            else
                let rows =
                    [ for i = 0 to trMatches.Count - 1 do
                        yield trMatches.[i].Groups.[1].Value ]
                    |> List.skip 1 // skip header

                rows
                |> List.choose (fun trHtml ->
                    let tdMatches = Regex.Matches(trHtml, "(?is)<td.*?>(.*?)</td>")
                    let cells =
                        [ for i = 0 to tdMatches.Count - 1 do
                            yield stripHtml tdMatches.[i].Groups.[1].Value ]
                    if cells.Length < 58 then
                        None
                    else
                        let cell i = cells.[i]
                        let extras =
                            extraNames
                            |> List.mapi (fun i name -> name, (cell (5 + i)))
                            |> Map.ofList
                        let attrs =
                            numericNames
                            |> List.mapi (fun i name -> name, parseIntSafe (cell (11 + i)))
                            |> Map.ofList
                        Some {
                            Rec = cell 0
                            Inf = cell 1
                            Name = cell 2
                            DoB = cell 3
                            Height = cell 4
                            Extras = extras
                            Attributes = attrs
                        }
                )

    let loadPlayers (path: string) =
        let content = File.ReadAllText(path)
        AllPlayers <- parsePlayersFromHtmlContent content

    let loadMyPlayers (path: string) =
        let content = File.ReadAllText(path)
        MyPlayers <- parsePlayersFromHtmlContent content

    let loadSctPlayers (path: string) =
        let content = File.ReadAllText(path)
        SctPlayers <- parsePlayersFromHtmlContent content


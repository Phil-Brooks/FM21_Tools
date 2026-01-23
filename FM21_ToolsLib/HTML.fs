namespace FM21_ToolsLib

open System
open System.IO
open System.Text.RegularExpressions
open System.Net

module HTML =

    /// Simplified player record:
    /// - keeps original leading string columns
    /// - groups the six inserted string columns into `Extras`
    /// - groups the many numeric attributes into `Attributes` by name
    type Player = {
        Rec: string
        Inf: string
        Name: string
        DoB: string
        Height: string
        Extras: Map<string,string option>
        Attributes: Map<string,int option>
    }

    let private tryParseInt (s: string) : int option =
        if String.IsNullOrWhiteSpace s then None
        else
            // strip non-digit characters (keeps negative sign if present)
            let trimmed = Regex.Replace(s, "[^0-9-]", "")
            match Int32.TryParse(trimmed) with
            | true, v -> Some v
            | _ -> None

    // strip HTML tags and decode entities
    let private stripHtml (s: string) : string =
        let withoutTags = Regex.Replace(s, "<.*?>", "", RegexOptions.Singleline)
        WebUtility.HtmlDecode(withoutTags).Trim()

    /// Load players from an HTML file containing the table like `data/all.html`.
    let loadPlayers (path: string) : Player list =
        let content = File.ReadAllText(path)
        // find the first <table> ... </table> (singleline + ignore case)
        let tableMatch = Regex.Match(content, "(?is)<table.*?>(.*?)</table>")
        if not tableMatch.Success then []
        else
            let tableInner = tableMatch.Groups.[1].Value
            // find all rows
            let trMatches = Regex.Matches(tableInner, "(?is)<tr.*?>(.*?)</tr>")
            // convert MatchCollection to a list of inner HTML strings for each tr
            let trList =
                [ for i = 0 to trMatches.Count - 1 do
                    yield trMatches.[i].Groups.[1].Value ]
            // first tr is header — skip it
            let extraNames = [ "Value"; "TransferStatus"; "LoanStatus"; "Position"; "Based"; "Club" ]
            let numericNames = [
                "Acc"; "Agi"; "Bal"; "Jum"; "Nat"; "Pac"; "Sta"; "Str"; "Agg"; "Ant"; "Bra"; "Cmp";
                "Cnt"; "Dec"; "Det"; "Fla"; "Ldr"; "OtB"; "Pos"; "Tea"; "Vis"; "Wor"; "Cor"; "Cro";
                "Dri"; "Fin"; "Fir"; "Fre"; "Hea"; "Lon"; "LTh"; "Mar"; "Pas"; "Pen"; "Tck"; "Tec";
                "Aer"; "Cmd"; "Com"; "OneVOne"; "Han"; "Kic"; "Ecc"; "Pun"; "Ref"; "TRO"; "Thr"
            ]

            trList
            |> List.skip 1
            |> List.choose (fun trHtml ->
                // extract td cell contents
                let tdMatches = Regex.Matches(trHtml, "(?is)<td.*?>(.*?)</td>")
                let tds =
                    [ for i = 0 to tdMatches.Count - 1 do
                        yield stripHtml(tdMatches.[i].Groups.[1].Value) ]

                // expect at least 58 columns (0..57) as before
                if tds.Length >= 58 then
                    // helpers
                    let td i = tds.[i]
                    let maybe s = if String.IsNullOrWhiteSpace s then None else Some s

                    // build extras map from indices 5..10
                    let extras =
                        extraNames
                        |> List.mapi (fun idx name -> name, maybe (td (5 + idx)))
                        |> Map.ofList

                    // build attributes map from indices 11..
                    let attributes =
                        numericNames
                        |> List.mapi (fun idx name -> name, tryParseInt (td (11 + idx)))
                        |> Map.ofList

                    Some {
                        Rec = td 0
                        Inf = td 1
                        Name = td 2
                        DoB = td 3
                        Height = td 4
                        Extras = extras
                        Attributes = attributes
                    }
                else None)


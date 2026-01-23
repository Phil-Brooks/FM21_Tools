namespace FM21_ToolsLib

open System
open System.IO
open System.Text.RegularExpressions
open System.Net

module HTML =

    /// Player record matching the columns in `data/all.html`
    type Player = {
        Rec: string
        Inf: string
        Name: string
        DoB: string
        Height: string
        Acc: int option
        Agi: int option
        Bal: int option
        Jum: int option
        Nat: int option
        Pac: int option
        Sta: int option
        Str: int option
        Agg: int option
        Ant: int option
        Bra: int option
        Cmp: int option
        Cnt: int option
        Dec: int option
        Det: int option
        Fla: int option
        Ldr: int option
        OtB: int option
        /// Position text (e.g. "ST, AMR" or "F C") — used to filter by role
        Position: string option
        Pos: int option
        Tea: int option
        Vis: int option
        Wor: int option
        Cor: int option
        Cro: int option
        Dri: int option
        Fin: int option
        Fir: int option
        Fre: int option
        Hea: int option
        Lon: int option
        LTh: int option
        Mar: int option
        Pas: int option
        Pen: int option
        Tck: int option
        Tec: int option
        Aer: int option
        Cmd: int option
        Com: int option
        OneVOne: int option
        Han: int option
        Kic: int option
        Ecc: int option
        Pun: int option
        Ref: int option
        TRO: int option
        Thr: int option
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

    /// Load players from an HTML file containing the table like `data/test.html`.
    /// Path should point to the HTML file (relative or absolute).
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
            trList
            |> List.skip 1
            |> List.choose (fun trHtml ->
                // extract td cell contents
                let tdMatches = Regex.Matches(trHtml, "(?is)<td.*?>(.*?)</td>")
                let tds =
                    [ for i = 0 to tdMatches.Count - 1 do
                        yield stripHtml(tdMatches.[i].Groups.[1].Value) ]

                // updated to expect the new number of columns (58) — six additional columns
                // (Value, Transfer Status, Loan Status, Position, Based, Club) inserted after Height
                if tds.Length >= 58 then
                    // small helpers to reduce repetition
                    let td i = tds.[i]
                    let p i = tryParseInt (td i)
                    let maybe s = if String.IsNullOrWhiteSpace s then None else Some s

                    Some {
                        Rec = td 0
                        Inf = td 1
                        Name = td 2
                        DoB = td 3
                        Height = td 4
                        Position = maybe (td 8)
                        // numeric fields (shifted by +6 between Height and Acc)
                        Acc = p 11
                        Agi = p 12
                        Bal = p 13
                        Jum = p 14
                        Nat = p 15
                        Pac = p 16
                        Sta = p 17
                        Str = p 18
                        Agg = p 19
                        Ant = p 20
                        Bra = p 21
                        Cmp = p 22
                        Cnt = p 23
                        Dec = p 24
                        Det = p 25
                        Fla = p 26
                        Ldr = p 27
                        OtB = p 28
                        Pos = p 29
                        Tea = p 30
                        Vis = p 31
                        Wor = p 32
                        Cor = p 33
                        Cro = p 34
                        Dri = p 35
                        Fin = p 36
                        Fir = p 37
                        Fre = p 38
                        Hea = p 39
                        Lon = p 40
                        LTh = p 41
                        Mar = p 42
                        Pas = p 43
                        Pen = p 44
                        Tck = p 45
                        Tec = p 46
                        Aer = p 47
                        Cmd = p 48
                        Com = p 49
                        OneVOne = p 50
                        Han = p 51
                        Kic = p 52
                        Ecc = p 53
                        Pun = p 54
                        Ref = p 55
                        TRO = p 56
                        Thr = p 57
                    }
                else None)


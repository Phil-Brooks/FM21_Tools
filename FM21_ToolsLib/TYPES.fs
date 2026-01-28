namespace FM21_ToolsLib

[<AutoOpen>]
module TYPES =
    type Player = {
        Rec: string
        Inf: string
        Name: string
        DoB: string
        Height: string
        Extras: Map<string,string>
        Attributes: Map<string,int>
    }

    /// Structured result for a player rated for a role.
    type RoleRatedPlayer = { Name: string; RoleName: string; Rating: float; Player: Player }
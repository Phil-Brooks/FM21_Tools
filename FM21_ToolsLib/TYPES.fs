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

    // Unassigned positions are represented as a `RoleRatedPlayer option`.
    type Team = {
        SweeperKeeper: RoleRatedPlayer option
        InvertedWingBackRight: RoleRatedPlayer option
        InvertedWingBackLeft: RoleRatedPlayer option
        BallPlayingDef1: RoleRatedPlayer option
        BallPlayingDef2: RoleRatedPlayer option
        WingerAttackRight: RoleRatedPlayer option
        InvertedWingerLeft: RoleRatedPlayer option
        BallWinningMidfielderSupport: RoleRatedPlayer option
        AdvancedPlaymakerSupport: RoleRatedPlayer option
        AdvancedForwardAttack: RoleRatedPlayer option
        TargetManAttack: RoleRatedPlayer option
    }

    /// Structured result for a role rated player with progress.
    type RRPlayerProgress = { Progress: float option; RRPlayer: RoleRatedPlayer }

    let RRPPtoString (rrpp: RRPlayerProgress) : string =
        let prog = if rrpp.Progress.IsSome then sprintf "%.2f" rrpp.Progress.Value else "N/A"
        "Name: " + rrpp.RRPlayer.Name + ", Role: " + rrpp.RRPlayer.RoleName + ", Rating: " + sprintf "%.2f" rrpp.RRPlayer.Rating + ", Progress: " + prog

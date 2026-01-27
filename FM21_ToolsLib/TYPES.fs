namespace FM21_ToolsLib

module TYPES =

    /// Structured result for a player rated for a role.
    type RoleRatedPlayer = { Name: string; RoleName: string; Rating: float; Player: HTML.Player }
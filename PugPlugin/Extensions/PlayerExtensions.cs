using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace PugPlugin.Extensions;

public static class PlayerExtensions
{
    public static bool IsTerrorist(this CCSPlayerController? player)
    {
        if (player == null || !player.IsValid || !player.PlayerPawn.IsValid)
        {
            return false;
        }

        var isTerrorist = player.PlayerPawn.Value.TeamNum == 2;
        return isTerrorist;
    }

    public static bool IsCounterTerrorist(this CCSPlayerController? player)
    {
        if (player == null || !player.IsValid || !player.PlayerPawn.IsValid)
        {
            return false;
        }

        var isCounterTerrorist = player.PlayerPawn.Value.TeamNum == 3;
        return isCounterTerrorist;
    }
    
    public static bool IsOnATeam(this CCSPlayerController player)
    {
        return player.IsTerrorist() || player.IsCounterTerrorist();
    }
    
    public static bool TryFindPlayerBySteamId(ulong steamId, out CCSPlayerController player)
    {
        player = null!;
        var foundPlayer = Utilities.GetPlayers().FirstOrDefault(player => player.SteamID == steamId);

        if (foundPlayer == null) return false;

        player = foundPlayer;
        return true;
    }
}
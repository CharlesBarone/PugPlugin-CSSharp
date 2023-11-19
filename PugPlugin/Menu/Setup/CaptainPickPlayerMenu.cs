using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using PugPlugin.Config;

namespace PugPlugin.Menu.Setup;

public class CaptainPickPlayerMenu : Menu<ulong>
{
    public CaptainPickPlayerMenu()
    {
        SetTitle($"{PugConfig.ChatPrefix} Pick a player for your team:");
    }

    public void SetPlayers(ulong captain1, ulong captain2)
    {
        foreach (CCSPlayerController player in Utilities.GetPlayers())
        {
            if (player.SteamID != captain1 && player.SteamID != captain2)
            {
                AddMenuItem(player.PlayerName, player.SteamID);
            }
        }
    }
}
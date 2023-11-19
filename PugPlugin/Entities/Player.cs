using CounterStrikeSharp.API.Core;

namespace PugPlugin.Entities;

public class Player
{
    public CCSPlayerController? PlayerController;
    public bool IsReady;
    public static Player Default(CCSPlayerController player)
    {
        return new Player
        {
            PlayerController = player,
            IsReady = false,
            
        };
    }
    
    
}
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using PugPlugin.Entities;
using PugPlugin.Extensions;

namespace PugPlugin.Managers;

public class PlayerManager
{
    private Dictionary<ulong, Player> Players { get; set; } = null!;
    private int _readyCount = 0;
    private int _aliveCount = 0;
    
    public void Init()
    {
        Players = new Dictionary<ulong, Player>();
    }

    public void Cleanup()
    {
        Players.Clear();
        _readyCount = 0;
    }

    public void OnPlayerConnect(EventPlayerConnectFull @event)
    {
        Players.Add(@event.Userid.SteamID, Player.Default(@event.Userid));
    }
    
    public void OnPlayerDisconnect(EventPlayerDisconnect @event)
    {
        if (Players[@event.Userid.SteamID].IsReady)
        {
            _readyCount--;
        }
        
        Players.Remove(@event.Userid.SteamID);
    }

    public bool IsServerFull()
    {
        return Players.Count == 10;
    }

    public int GetPlayerCount()
    {
        return Players.Count;
    }
    
    public int GetReadyCount()
    {
        return _readyCount;
    }

    public void ResetAliveCount()
    {
        _aliveCount = Players.Count;
    }

    public void OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        _aliveCount--;
    }

    public int GetAliveCount()
    {
        return _aliveCount;
    }
    
    public bool IsServerReady()
    {
        return _readyCount == 10;
    }

    public bool SetReady(CCSPlayerController player)
    {
        if (!Players[player.SteamID].IsReady)
        {
            Players[player.SteamID].IsReady = true;
            _readyCount++;
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public bool SetUnready(CCSPlayerController player)
    {
        if (Players[player.SteamID].IsReady)
        {
            Players[player.SteamID].IsReady = false;
            _readyCount--;
            return true;
        }
        else
        {
            return false;
        }
    }
}
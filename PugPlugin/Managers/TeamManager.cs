using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using PugPlugin.Entities;
using PugPlugin.Extensions;

namespace PugPlugin.Managers;

public class TeamManager
{
    private ulong _captain1;
    private ulong _captain2;

    private bool _team1IsTerrorist = false;
    private bool _team1CaptainPickFirstPlayer = false;
    
    private Dictionary<ulong, Player> _Team1Players { get; set; } = null!;
    private Dictionary<ulong, Player> _Team2Players { get; set; } = null!;
    
    public void Init()
    {
        _Team1Players = new Dictionary<ulong, Player>();
        _Team2Players = new Dictionary<ulong, Player>();
    }
    
    public void Cleanup()
    {
        _Team1Players.Clear();
        _Team2Players.Clear();
    }

    public void MovePlayersToTeams()
    {
        foreach (CCSPlayerController player in Utilities.GetPlayers())
        {
            if (_Team1Players.ContainsKey(player.SteamID)) //Team1
            {
                if (_team1IsTerrorist && !player.IsTerrorist())
                {
                    player.SwitchTeam(CsTeam.Terrorist);
                }
                else if (!_team1IsTerrorist && !player.IsCounterTerrorist())
                {
                    player.SwitchTeam(CsTeam.CounterTerrorist);
                }
            }
            else if(_Team2Players.ContainsKey(player.SteamID))//Team2
            {
                if (_team1IsTerrorist && !player.IsCounterTerrorist())
                {
                    player.SwitchTeam(CsTeam.CounterTerrorist);
                }
                else if (!_team1IsTerrorist && !player.IsTerrorist())
                {
                    player.SwitchTeam(CsTeam.Terrorist);
                }
            }
        }
    }

    public void SetCaptain1(ulong steamid)
    {
        _captain1 = steamid;
    }
    
    public void SetCaptain2(ulong steamid)
    {
        _captain2 = steamid;
    }
    
    public Player GetCaptain1()
    {
        return _Team1Players[_captain1];
    }
    
    public Player GetCaptain2()
    {
        return _Team2Players[_captain2];
    }

    public void SetTeam1IsTerrorist(bool team1IsTerrorist)
    {
        _team1IsTerrorist = team1IsTerrorist;
    }
    
    public bool GetTeam1CaptainPickFirstPlayer()
    {
        return _team1CaptainPickFirstPlayer;
    }
    
    public void SetTeam1CaptainPickFirstPlayer(bool team1CaptainPickFirstPlayer)
    {
        _team1CaptainPickFirstPlayer = team1CaptainPickFirstPlayer;
    }

    public void AddPlayerToTeam1(CCSPlayerController player)
    {
        _Team1Players[player.SteamID] = Player.Default(player);
    }
    
    public void AddPlayerToTeam2(CCSPlayerController player)
    {
        _Team2Players[player.SteamID] = Player.Default(player);
    }
}
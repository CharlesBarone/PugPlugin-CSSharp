using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using PugPlugin.Config;
using PugPlugin.Extensions;
using PugPlugin.Managers;
using PugPlugin.Menu.Setup;

namespace PugPlugin;

public class PugPlugin: BasePlugin
{
    public override string ModuleName => "PugPlugin";
    public override string ModuleVersion => "1.1";
    public override string ModuleAuthor => "hypnos <hyps.dev>";

    public override string ModuleDescription => "Pug server setup plugin, automates the setup of 10man servers.";

    private PlayerManager _playerManager = new();
    private GameManager _gameManager = new();
    private TeamManager _teamManager = new();
    
    //Menus
    private Captain1ChoiceMenu _captain1ChoiceMenu = new();
    private PickFirstSideMenu _pickFirstSideMenu = new();
    private CaptainPickPlayerMenu _captainPickPlayerMenu = new();
    private TestMenu _testMenu = new();
    
    public override void Load(bool hotReload)
    {
        _playerManager.Init();
        _gameManager.Init();
        _teamManager.Init();

        //Menus
        _captain1ChoiceMenu.AddCommands(AddCommand);
        _pickFirstSideMenu.AddCommands(AddCommand);
        _captainPickPlayerMenu.AddCommands(AddCommand);
        _testMenu.AddCommands(AddCommand);
        
        RegisterListener<Listeners.OnMapEnd>(() => Unload(true));
    }
    
    public override void Unload(bool hotReload)
    {
        _playerManager.Cleanup();
        _teamManager.Cleanup();
        _gameManager.Cleanup();
        
        //Menus
        _captain1ChoiceMenu.RemoveCommands(RemoveCommand);
        _pickFirstSideMenu.RemoveCommands(RemoveCommand);
        _captainPickPlayerMenu.RemoveCommands(RemoveCommand);
        _testMenu.RemoveCommands(RemoveCommand);
        _captain1ChoiceMenu.Cleanup();
        _pickFirstSideMenu.Cleanup();
        _captainPickPlayerMenu.Cleanup();
        _testMenu.Cleanup();
    }
    
    [GameEventHandler]
    public HookResult OnPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event.Userid.IsValid && @event.Userid.PlayerPawn.IsValid)
        {
            if (_playerManager.GetPlayerCount() == 0)
            {
                //Set warmup settings
                Server.ExecuteCommand("mp_warmuptime 120");
                Server.ExecuteCommand("mp_warmup_pausetimer 1");
                Server.ExecuteCommand("bot_quota 0");
                Server.ExecuteCommand("sv_auto_full_alltalk_during_warmup_half_end 1");
                Server.ExecuteCommand("mp_autokick 0");
                Server.ExecuteCommand("mp_death_drop_gun 0");
                Server.ExecuteCommand("mp_weapons_allow_typecount -1");
                Server.ExecuteCommand("ammo_grenade_limit_flashbang 2");
                Server.PrintToConsole("[PugPlugin] Set Warmup Settings, 1st player connected.");
            }

            _playerManager.OnPlayerConnect(@event);
        }

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event.Userid.IsValid && @event.Userid.PlayerPawn.IsValid && !@event.Userid.IsBot)
        {
            _playerManager.OnPlayerDisconnect(@event);
            
            //Menus
            _captain1ChoiceMenu.OnPlayerDisconnect(@event);
            _pickFirstSideMenu.OnPlayerDisconnect(@event);
            _captainPickPlayerMenu.OnPlayerDisconnect(@event);
            _testMenu.OnPlayerDisconnect(@event);
        }

        if (_gameManager.GetGameState() == 1 || _gameManager.GetGameState() == 2)
        {
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} Player count dropped below 10 during setup, resetting while server waits for more players!");
            
            //Reset back to warmup/ready up state until 10 players connect again
            _gameManager.Cleanup();
            _teamManager.Cleanup();
            
            //Menus
            _captain1ChoiceMenu.Cleanup();
            _pickFirstSideMenu.Cleanup();
            _captainPickPlayerMenu.Cleanup();
            _testMenu.Cleanup();
            
            _captainPickPlayerMenu.RemoveAllMenuItems();
            
            //If in knife round, set settings back to warmup default.
            if (_gameManager.GetGameState() == 1)
            {
                Server.ExecuteCommand("mp_timelimit 5");
                Server.ExecuteCommand("mp_ignore_round_win_conditions 0");
                Server.ExecuteCommand("sv_talk_enemy_living 0");
                Server.ExecuteCommand("sv_talk_enemy_dead 0");
                Server.ExecuteCommand("sv_deadtalk 1");
                Server.ExecuteCommand("mp_give_player_c4 1");
                Server.ExecuteCommand("mp_death_drop_gun 0");
                Server.ExecuteCommand("mp_drop_grenade_enable 1");
                Server.ExecuteCommand("mp_death_drop_taser 1");
                Server.ExecuteCommand("mp_death_drop_healthshot 0");
                Server.ExecuteCommand("mp_death_drop_c4 1");
                Server.ExecuteCommand("mp_death_drop_breachcharge 0");
                Server.ExecuteCommand("mp_maxmoney 16000");
                Server.ExecuteCommand("mp_ct_default_secondary weapon_hkp2000");
                Server.ExecuteCommand("mp_t_default_secondary weapon_glock");
                Server.ExecuteCommand("mp_friendlyfire 0");
                Server.ExecuteCommand("ammo_grenade_limit_flashbang 2");
        
                Server.ExecuteCommand("mp_warmup_start");
                Server.ExecuteCommand("mp_warmup_pausetimer 1");
            }
        }
        return HookResult.Continue;
    }
    
    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        _playerManager.ResetAliveCount();
        return HookResult.Continue;
    }
    
    [ConsoleCommand("css_r", "Set player status to ready")]
    [ConsoleCommand("css_ready", "Set player status to ready")]
    public void OnReady(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null || !player.IsOnATeam() || _gameManager.GetIsStarted()) return;

        if (_playerManager.SetReady(player))
        {
            player.PrintToChat($"{PugConfig.ChatPrefix} Set status to Ready!");
            player.PrintToConsole($"{PugConfig.ChatPrefix} Set status to Ready!");
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} {_playerManager.GetReadyCount()}/{_playerManager.GetPlayerCount()} Players Ready. {player.PlayerName} set to Ready!");
            
            if (_playerManager.IsServerFull() && _playerManager.IsServerReady())
            {
                _gameManager.StartCaptainSelectRound();
            }
        }
        else
        {
            player.PrintToChat($"{PugConfig.ChatPrefix} Status already set to Ready (!ur/!unready to Unready).");
            player.PrintToConsole($"{PugConfig.ChatPrefix} Status already set to Ready (!ur/!unready to Unready).");
        }
    }
    
    [ConsoleCommand("css_ur", "Set player status to unready")]
    [ConsoleCommand("css_uready", "Set player status to unready")]
    public void OnUnready(CCSPlayerController? player, CommandInfo command)
    {
        if (player == null || !player.IsOnATeam() || _gameManager.GetIsStarted()) return;

        if (_playerManager.SetUnready(player))
        {
            player.PrintToChat($"{PugConfig.ChatPrefix} Set status to Unready!");
            player.PrintToConsole($"{PugConfig.ChatPrefix} Set status to Unready!");
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} {_playerManager.GetReadyCount()}/{_playerManager.GetPlayerCount()} Players Ready. {player.PlayerName} set to Unready.");
        }
        else
        {
            player.PrintToChat($"{PugConfig.ChatPrefix} Status already set to Unready (!r/!ready to Ready).");
            player.PrintToConsole($"{PugConfig.ChatPrefix} Status already set to Unready (!r/!ready to Ready).");
        }
    }
    
    [GameEventHandler]
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        var attacker = @event.Attacker;
        var target = @event.Userid;
        if (!(attacker.IsOnATeam() && target.IsOnATeam())) return HookResult.Continue;

        _playerManager.OnPlayerDeath(@event, info);
        
        //Unlimited Money during Warmup gameSates
        if (_gameManager.GetGameState() == 0 || _gameManager.GetGameState() == 2)
        {
            @event.Userid.InGameMoneyServices!.Account = 16000;
        }
        
        if (_gameManager.GetGameState() == 1)
        {
            if (_playerManager.GetAliveCount() == 2)
            {
                Server.PrintToChatAll($"{PugConfig.ChatPrefix} Two remaining players will be team captains. The last remaining of the captains will choose if they get first player pick or first side pick.");
            }

            if (_playerManager.GetAliveCount() == 1)
            {
                _teamManager.SetCaptain1(attacker.SteamID);
                _teamManager.AddPlayerToTeam1(attacker);
                _teamManager.SetCaptain2(target.SteamID);
                _teamManager.AddPlayerToTeam2(target);
                _gameManager.StartSetupRound();
                Server.PrintToChatAll($"{PugConfig.ChatPrefix} Captain {_teamManager.GetCaptain1().PlayerController!.PlayerName} is now selecting 1st captain preference. (First Team or First Player)");
                _captain1ChoiceMenu.Init(_teamManager.GetCaptain1().PlayerController!, false, Captain1ChoiceMenuCallback);
            }
        }
        
        return HookResult.Continue;
    }
    
    //Menu Callbacks
    public void Captain1ChoiceMenuCallback(int result, CCSPlayerController player)
    {
        if (result == 0)
        {
            _teamManager.SetTeam1CaptainPickFirstPlayer(false);
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} Captain {_teamManager.GetCaptain1().PlayerController!.PlayerName} picked First Team choice.");
            Server.NextFrame(() => _pickFirstSideMenu.Init(player, false, PickFirstSideMenuCallback));
        }
        else
        {
            _teamManager.SetTeam1CaptainPickFirstPlayer(true);
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} Captain {_teamManager.GetCaptain1().PlayerController!.PlayerName} picked First Player choice.");
            Server.NextFrame(() => _pickFirstSideMenu.Init(_teamManager.GetCaptain2().PlayerController!, false, PickFirstSideMenuCallback));
        }
    }

    public void PickFirstSideMenuCallback(int result, CCSPlayerController player)
    {
        if (result == 0)
        {
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} Captain {player.PlayerName} picked team: Terrorist");
        }
        else
        {
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} Captain {player.PlayerName} picked team: Counter Terrorist");
        }
        
        
        //check if team 1 or 2
        if (_teamManager.GetCaptain1().PlayerController!.SteamID == player.SteamID) //team1
        {
            if (result == 0) //Captain 1 picked Terrorist
            {
                _teamManager.SetTeam1IsTerrorist(true);
            }
            else //Captain 1 picked Counter Terrorist
            {
                _teamManager.SetTeam1IsTerrorist(false);
            }
        }
        else //team2
        {
            if (result == 1) //Captain 2 picked Counter Terrorist
            {
                _teamManager.SetTeam1IsTerrorist(true);
            }
            else //Captain 2 picked Terrorist
            {
                _teamManager.SetTeam1IsTerrorist(false);
            }
        }

        
        //Set player options for CaptainPickPlayerMenu now that players are connected.
        _captainPickPlayerMenu.SetPlayers(_teamManager.GetCaptain1().PlayerController!.SteamID, _teamManager.GetCaptain2().PlayerController!.SteamID);
        
        if (_teamManager.GetTeam1CaptainPickFirstPlayer())
        {
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} Captain {_teamManager.GetCaptain1().PlayerController!.PlayerName}'s turn to pick a player!");
            Server.NextFrame(() => _captainPickPlayerMenu.Init(_teamManager.GetCaptain1().PlayerController!, false, CaptainPickPlayerMenuCallback));
        }
        else
        {
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} Captain {_teamManager.GetCaptain2().PlayerController!.PlayerName}'s turn to pick a player!");
            Server.NextFrame(() => _captainPickPlayerMenu.Init(_teamManager.GetCaptain2().PlayerController!, false, CaptainPickPlayerMenuCallback));
        }
    }

    public void CaptainPickPlayerMenuCallback(ulong steamId, CCSPlayerController menuPlayer)
    {
        if (!PlayerExtensions.TryFindPlayerBySteamId(steamId, out CCSPlayerController player))
        {
            Console.WriteLine($"{PugConfig.ChatPrefix} ERROR: CaptainPickPlayerMenu, INVALID PLAYER!");
            return;
        }
        
        _captainPickPlayerMenu.RemoveMenuItem(steamId);
        
        //Determine Team
        if (_teamManager.GetCaptain1().PlayerController!.SteamID == menuPlayer.SteamID) //Team1
        {
            _teamManager.AddPlayerToTeam1(player);
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} Captain {_teamManager.GetCaptain1().PlayerController!.PlayerName} picked {player.PlayerName}.");
            if (_captainPickPlayerMenu.GetMenuItemCount() != 0)
            {
                Server.PrintToChatAll($"{PugConfig.ChatPrefix} Captain {_teamManager.GetCaptain2().PlayerController!.PlayerName}'s turn to pick a player!");
                Server.NextFrame(() => _captainPickPlayerMenu.Init(_teamManager.GetCaptain2().PlayerController!, false, CaptainPickPlayerMenuCallback));
            }
        }
        else //Team2
        {
            _teamManager.AddPlayerToTeam2(player);
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} Captain {_teamManager.GetCaptain2().PlayerController!.PlayerName} picked {player.PlayerName}.");
            if (_captainPickPlayerMenu.GetMenuItemCount() != 0)
            {
                Server.PrintToChatAll($"{PugConfig.ChatPrefix} Captain {_teamManager.GetCaptain1().PlayerController!.PlayerName}'s turn to pick a player!");
                Server.NextFrame(() => _captainPickPlayerMenu.Init(_teamManager.GetCaptain1().PlayerController!, false, CaptainPickPlayerMenuCallback));
            }
        }
        
        //Player selection done, start game
        if (_captainPickPlayerMenu.GetMenuItemCount() == 0)
        {
            //Assign Players to correct teams
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} Player selection complete moving players to correct teams.");
            _teamManager.MovePlayersToTeams();
            
            //Start Game
            Server.PrintToChatAll($"{PugConfig.ChatPrefix} Starting Match! glhf");
            _gameManager.StartGame();
        }
    }
}
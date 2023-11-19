using CounterStrikeSharp.API;
using PugPlugin.Config;

namespace PugPlugin.Managers;

public class GameManager
{

    private Boolean _isStarted;
    private int _gameState = 0; // 0 - Warmup, 1 - captains selection, 2 - setup, 3 - game
    
    public void Init()
    {
        _isStarted = false;
    }

    public void Cleanup()
    {
        _isStarted = false;
        _gameState = 0;
    }

    public void StartGame()
    {
        Server.PrintToChatAll($"{PugConfig.ChatPrefix} Teams are selected! Starting Game!");
        
        _gameState = 3;
        Server.ExecuteCommand("bot_autodifficulty_threshold_high 0.0");
        Server.ExecuteCommand("bot_autodifficulty_threshold_low -2.0");
        Server.ExecuteCommand("bot_chatter normal");
        Server.ExecuteCommand("bot_defer_to_human_goals 1");
        Server.ExecuteCommand("bot_defer_to_human_items 1");
        Server.ExecuteCommand("bot_difficulty 2");
        Server.ExecuteCommand("bot_quota 1");
        Server.ExecuteCommand("bot_quota_mode competitive");
        Server.ExecuteCommand("cash_player_bomb_defused 300");
        Server.ExecuteCommand("cash_player_bomb_planted 300");
        Server.ExecuteCommand("cash_player_damage_hostage -30");
        Server.ExecuteCommand("cash_player_interact_with_hostage 300");
        Server.ExecuteCommand("cash_player_killed_enemy_default 300");
        Server.ExecuteCommand("cash_player_killed_enemy_factor 1");
        Server.ExecuteCommand("cash_player_killed_hostage -1000");
        Server.ExecuteCommand("cash_player_killed_teammate -300");
        Server.ExecuteCommand("cash_player_rescued_hostage 1000");
        Server.ExecuteCommand("cash_team_elimination_bomb_map 3250");
        Server.ExecuteCommand("cash_team_elimination_hostage_map_t 3000");
        Server.ExecuteCommand("cash_team_elimination_hostage_map_ct 3000");
        Server.ExecuteCommand("cash_team_hostage_alive 0");
        Server.ExecuteCommand("cash_team_hostage_interaction 600");
        Server.ExecuteCommand("cash_team_loser_bonus 1400");
        Server.ExecuteCommand("cash_team_bonus_shorthanded 1000");
        Server.ExecuteCommand("mp_starting_losses 1");
        Server.ExecuteCommand("cash_team_loser_bonus_consecutive_rounds 500");
        Server.ExecuteCommand("cash_team_planted_bomb_but_defused 800");
        Server.ExecuteCommand("cash_team_rescued_hostage 600");
        Server.ExecuteCommand("cash_team_terrorist_win_bomb 3500");
        Server.ExecuteCommand("cash_team_win_by_defusing_bomb 3500");
        Server.ExecuteCommand("cash_team_win_by_hostage_rescue 2900");
        Server.ExecuteCommand("cash_team_win_by_time_running_out_hostage 3250");
        Server.ExecuteCommand("cash_team_win_by_time_running_out_bomb 3250");
        Server.ExecuteCommand("ff_damage_reduction_bullets 0.33");
        Server.ExecuteCommand("ff_damage_reduction_grenade 0.85");
        Server.ExecuteCommand("ff_damage_reduction_grenade_self 1");
        Server.ExecuteCommand("ff_damage_reduction_other 0.4");
        Server.ExecuteCommand("mp_afterroundmoney 0");
        Server.ExecuteCommand("mp_buytime 20");
        Server.ExecuteCommand("mp_buy_anywhere 0");
        Server.ExecuteCommand("mp_buy_during_immunity 0");
        Server.ExecuteCommand("mp_death_drop_defuser 1	");
        Server.ExecuteCommand("mp_death_drop_grenade 2");
        Server.ExecuteCommand("mp_death_drop_gun 1");
        Server.ExecuteCommand("mp_fists_replace_melee 1");
        Server.ExecuteCommand("mp_defuser_allocation 0");
        Server.ExecuteCommand("mp_force_pick_time 15");
        Server.ExecuteCommand("mp_forcecamera 1");
        Server.ExecuteCommand("mp_free_armor 0		");
        Server.ExecuteCommand("mp_freezetime 15");
        Server.ExecuteCommand("mp_friendlyfire 1");
        Server.ExecuteCommand("mp_win_panel_display_time 3");
        Server.ExecuteCommand("mp_respawn_immunitytime -1");
        Server.ExecuteCommand("mp_halftime 1");
        Server.ExecuteCommand("mp_match_can_clinch 1");
        Server.ExecuteCommand("mp_maxmoney 16000");
        Server.ExecuteCommand("mp_maxrounds 24");
        Server.ExecuteCommand("mp_molotovusedelay 0");
        Server.ExecuteCommand("mp_playercashawards 1");
        Server.ExecuteCommand("mp_roundtime 1.92");
        Server.ExecuteCommand("mp_roundtime_hostage 1.92");
        Server.ExecuteCommand("mp_roundtime_defuse 1.92");
        Server.ExecuteCommand("mp_solid_teammates 1");
        Server.ExecuteCommand("mp_startmoney 800");
        Server.ExecuteCommand("mp_teamcashawards 1");
        Server.ExecuteCommand("mp_timelimit 0");
        Server.ExecuteCommand("mp_technical_timeout_per_team 1");
        Server.ExecuteCommand("mp_technical_timeout_duration_s 120");
        Server.ExecuteCommand("mp_weapons_allow_zeus 5");
        Server.ExecuteCommand("spec_freeze_panel_extended_time 0");
        Server.ExecuteCommand("spec_freeze_time 2.0");
        Server.ExecuteCommand("sv_allow_votes 1");
        Server.ExecuteCommand("sv_talk_enemy_living 0");
        Server.ExecuteCommand("sv_talk_enemy_dead 0		");
        Server.ExecuteCommand("sv_auto_full_alltalk_during_warmup_half_end 1");
        Server.ExecuteCommand("sv_deadtalk 1");
        Server.ExecuteCommand("sv_ignoregrenaderadio 0");
        Server.ExecuteCommand("sv_grenade_trajectory_time_spectator 4");
        Server.ExecuteCommand("tv_delay 105");
        Server.ExecuteCommand("mp_warmup_pausetimer 0");
        Server.ExecuteCommand("mp_halftime_pausetimer 0");
        Server.ExecuteCommand("mp_randomspawn 0");
        Server.ExecuteCommand("mp_randomspawn_los 0");
        Server.ExecuteCommand("sv_infinite_ammo 0");
        Server.ExecuteCommand("ammo_grenade_limit_flashbang 2");
        Server.ExecuteCommand("ammo_grenade_limit_total 4");
        Server.ExecuteCommand("mp_weapons_allow_map_placed 1");
        Server.ExecuteCommand("mp_weapons_glow_on_ground 0");
        Server.ExecuteCommand("mp_display_kill_assists 1");
        Server.ExecuteCommand("mp_respawn_on_death_t 0");
        Server.ExecuteCommand("mp_respawn_on_death_ct 0");
        Server.ExecuteCommand("mp_ct_default_melee weapon_knife");
        Server.ExecuteCommand("mp_ct_default_secondary weapon_hkp2000");
        Server.ExecuteCommand("mp_ct_default_primary \"\"");
        Server.ExecuteCommand("mp_t_default_melee weapon_knife");
        Server.ExecuteCommand("mp_t_default_secondary weapon_glock");
        Server.ExecuteCommand("mp_t_default_primary \"\"");
        Server.ExecuteCommand("mp_default_team_winner_no_objective -1");
        Server.ExecuteCommand("sv_occlude_players 1");
        Server.ExecuteCommand("occlusion_test_async 0");
        Server.ExecuteCommand("spec_replay_enable 0");
        Server.ExecuteCommand("sv_gameinstructor_enable 0");
        Server.ExecuteCommand("mp_weapons_allow_typecount 5");
        Server.ExecuteCommand("mp_endmatch_votenextmap 1");
        Server.ExecuteCommand("mp_endmatch_votenextmap_keepcurrent 0");
        
        Server.ExecuteCommand("mp_warmup_end");
    }
    
    public void StartSetupRound()
    {
        Server.PrintToChatAll($"{PugConfig.ChatPrefix} All players are ready, starting captain selection knife round!");
        
        //setup setup warmup, reset settings from captain round
        _gameState = 2;
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

    public void StartCaptainSelectRound()
    {
        _isStarted = true;
        Server.PrintToChatAll($"{PugConfig.ChatPrefix} All players are ready, starting captain selection knife round!");
        
        //setup captains match
        _gameState = 1;
        Server.ExecuteCommand("mp_timelimit 1");
        Server.ExecuteCommand("mp_ignore_round_win_conditions 1");
        Server.ExecuteCommand("sv_talk_enemy_living 1");
        Server.ExecuteCommand("sv_talk_enemy_dead 1");
        Server.ExecuteCommand("sv_deadtalk 1");
        Server.ExecuteCommand("mp_give_player_c4 0");
        Server.ExecuteCommand("mp_death_drop_gun 0");
        Server.ExecuteCommand("mp_drop_grenade_enable 0");
        Server.ExecuteCommand("mp_death_drop_taser 0");
        Server.ExecuteCommand("mp_death_drop_healthshot 0");
        Server.ExecuteCommand("mp_death_drop_c4 0");
        Server.ExecuteCommand("mp_death_drop_breachcharge 0");
        Server.ExecuteCommand("mp_maxmoney 0");
        Server.ExecuteCommand("mp_ct_default_secondary 0");
        Server.ExecuteCommand("mp_t_default_secondary 0");
        Server.ExecuteCommand("mp_friendlyfire 1");
        
        Server.ExecuteCommand("mp_warmup_end");
    }

    public bool GetIsStarted()
    {
        return _isStarted;
    }

    public int GetGameState()
    {
        return _gameState;
    }
}
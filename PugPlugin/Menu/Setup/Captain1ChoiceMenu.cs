using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using PugPlugin.Config;

namespace PugPlugin.Menu;

public class Captain1ChoiceMenu : AbstractMenu<int>
{
    public Captain1ChoiceMenu()
    {
        SetTitle($"{PugConfig.ChatPrefix} As first Captain you can choose one of the following preferences:");
        AddMenuItem("Pick First Side", 0);
        AddMenuItem("Pick First Teammate", 1);
    }

    protected override void OnSelectOption(CCSPlayerController player, int option)
    {
        Server.PrintToChatAll($"{PugConfig.ChatPrefix} Captain {player.PlayerName} chose to {GetOptionText(option)}.");
        _callbackAction?.Invoke(option);
    }
}
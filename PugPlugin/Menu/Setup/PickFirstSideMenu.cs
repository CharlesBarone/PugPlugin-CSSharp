using PugPlugin.Config;

namespace PugPlugin.Menu.Setup;

public class PickFirstSideMenu : Menu<int>
{
    public PickFirstSideMenu()
    {
        SetTitle($"{PugConfig.ChatPrefix} Pick first side:");
        AddMenuItem("Terrorist", 0);
        AddMenuItem("Counter Terrorist", 1);
    }
}
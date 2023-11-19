using PugPlugin.Config;

namespace PugPlugin.Menu.Setup;

public class TestMenu : Menu<int>
{
    public TestMenu()
    {
        SetTitle($"{PugConfig.ChatPrefix} TestMenu:");
        AddMenuItem("0", 0);
        AddMenuItem("1", 1);
        AddMenuItem("2", 2);
        AddMenuItem("3", 3);
        AddMenuItem("4", 4);
        AddMenuItem("5", 5);
        AddMenuItem("6", 6);
        AddMenuItem("7", 7);
        AddMenuItem("8", 8);
        AddMenuItem("9", 9);
        AddMenuItem("10", 10);
        AddMenuItem("11", 11);
        AddMenuItem("12", 12);
        AddMenuItem("13", 13);
        AddMenuItem("14", 14);
        AddMenuItem("15", 15);
        AddMenuItem("16", 16);
        AddMenuItem("17", 17);
        AddMenuItem("18", 18);
        AddMenuItem("19", 19);
        AddMenuItem("20", 20);
    }
}
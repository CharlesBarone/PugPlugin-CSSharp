using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using PugPlugin.Extensions;

namespace PugPlugin.Menu;

public abstract class Menu<T>
{
    private List<Tuple<string, T>> _menuItems;
    private List<ulong> _playersWithMenuActive;
    private string _title { get; set; }
    private int CurrentPage = 1;
    protected Action<T>? _callbackAction = null;
    
    protected Menu()
    {
        _menuItems = new List<Tuple<string, T>>();
        _playersWithMenuActive = new List<ulong>();
        _title = "";
    }
    
    protected Menu(string title)
    {
        _menuItems = new List<Tuple<string, T>>();
        _playersWithMenuActive = new List<ulong>();
        _title = title;
    }
    
    public void AddMenuItem(string text, T value)
    {
        _menuItems.Add(new Tuple<string, T>(text, value));
    }

    public string GetOptionText(int option)
    {
        return _menuItems[option - 1].Item1;
    }
    
    public T GetOptionValue(int option)
    {
        return _menuItems[option - 1].Item2;
    }

    public string GetTitle()
    {
        return _title;
    }
    
    public void SetTitle(string title)
    {
        _title = title;
    }

    public List<Tuple<string, T>> GetOptions(int pageNumber, bool showTitle, bool showExit)
    {
        int linesRemaining = 8; //cs2 can show 8 lines of chat at a time

        if (showTitle)
        {
            linesRemaining--;
        }
        
        if (showExit)
        {
            linesRemaining--;
        }

        if (pageNumber > 1) // Show Previous Option
        {
            linesRemaining--;
        }
        
        int startIndex = (pageNumber - 1) * linesRemaining;
        int endIndex = Math.Min(startIndex + linesRemaining, _menuItems.Count);
        bool nextButton = false;
        if (endIndex != _menuItems.Count) // Check if a next button is needed
        {
            linesRemaining--;
            nextButton = true;
            //Redo values to account for next button
            //startIndex = (pageNumber - 1) * linesRemaining;
            //endIndex = Math.Min(startIndex + linesRemaining, _menuItems.Count);
        }
        

        List<Tuple<string, T>> options = new List<Tuple<string, T>>();
        int i = startIndex;
        for (; i < endIndex; i++)
        {
            options.Add(new Tuple<string, T>($"[{i + 1}] {_menuItems[i].Item1}", _menuItems[i].Item2));
        }
        
        if (pageNumber > 1) // Show Previous Option
        {
            options.Add(new Tuple<string, T>($"[{i + 1}] Previous", default(T)!));
            i++;
        }

        if (nextButton)
        {
            options.Add(new Tuple<string, T>($"[{i + 1}] Next", default(T)!));
            i++;
        }
        
        return options;
    }

    public void PrintMenu(CCSPlayerController player, int pageNumber, bool showTitle, bool showExit)
    {
        if (showTitle)
        {
            player.PrintToChat(_title);
        }

        List<Tuple<string, T>> options = GetOptions(pageNumber, showTitle, showExit);

        foreach (Tuple<string, T> option in options)
        {
            player.PrintToChat(option.Item1);
        }

        if (showExit)
        {
            player.PrintToChat("[0] Exit");
            
        }
    }

    public void Init(CCSPlayerController player, bool showExit, Action<T>? callbackAction = null)
    {
        _playersWithMenuActive.Add(player.SteamID);
        
        PrintMenu(player, CurrentPage, !String.IsNullOrEmpty(_title), showExit);
        if (callbackAction != null)
        {
            _callbackAction = callbackAction;
        }
    }

    public void Cleanup()
    {
        _callbackAction = null;
        _playersWithMenuActive.Clear();
    }

    protected virtual void OnSelectOption(CCSPlayerController player, int option)
    {
        //Console.WriteLine($"AbstractMenu: {player.PlayerName} picked option {option}");
        //player?.PrintToChat($"AbstractMenu: {player.PlayerName} picked option {option}");
    }

    private void OnSlot(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null || !_playersWithMenuActive.Contains(player.SteamID)) return;
        
        string optionString = info.GetCommandString.Substring(4);
        if (int.TryParse(optionString, out int option))
        {
            _playersWithMenuActive.Remove(player.SteamID);
            OnSelectOption(player, option);
        }
    }

    public void AddCommands(Action<string, string, CommandInfo.CommandCallback> addCommand)
    {
        addCommand("css_0", "Menu Option 0", OnSlot);
        addCommand("css_1", "Menu Option 1", OnSlot);
        addCommand("css_2", "Menu Option 2", OnSlot);
        addCommand("css_3", "Menu Option 3", OnSlot);
        addCommand("css_4", "Menu Option 4", OnSlot);
        addCommand("css_5", "Menu Option 5", OnSlot);
        addCommand("css_6", "Menu Option 6", OnSlot);
        addCommand("css_7", "Menu Option 7", OnSlot);
        addCommand("css_8", "Menu Option 8", OnSlot);
        addCommand("css_9", "Menu Option 9", OnSlot);
    }
}
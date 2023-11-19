using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Entities;
using PugPlugin.Config;

namespace PugPlugin.Menu;

public abstract class Menu<T>
{
    private List<Tuple<string, T>> _menuItems;
    private List<ulong> _playersWithMenuActive;
    private string _title { get; set; }
    private Dictionary<ulong, int> _currentPage { get; set; } = null!;
    private Dictionary<ulong, bool> _isNextPresent { get; set; } = null!;
    private Dictionary<ulong, int> _nextIndex { get; set; } = null!;
    private Dictionary<ulong, bool> _isPreviousPresent { get; set; } = null!;
    private Dictionary<ulong, int> _previousIndex { get; set; } = null!;
    private Dictionary<ulong, bool> _isExitPresent { get; set; } = null!;
    private Dictionary<ulong, Stack<int>> _previousPageLastIndex { get; set; } = null!;
    
    
    protected Action<T, CCSPlayerController>? _callbackAction = null;
    
    protected Menu()
    {
        _menuItems = new List<Tuple<string, T>>();
        _playersWithMenuActive = new List<ulong>();
        _currentPage = new Dictionary<ulong, int>();
        
        //Navigation Stuff
        _isNextPresent = new Dictionary<ulong, bool>();
        _nextIndex = new Dictionary<ulong, int>();
        _isPreviousPresent = new Dictionary<ulong, bool>();
        _previousIndex = new Dictionary<ulong, int>();
        _isExitPresent = new Dictionary<ulong, bool>();
        _previousPageLastIndex = new Dictionary<ulong, Stack<int>>();
        
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

    public void RemoveMenuItem(T value)
    {
        int index = _menuItems.FindIndex(tuple => tuple.Item2!.Equals(value));
        if (index != -1)
        {
            _menuItems.RemoveAt(index);
        }
    }

    public int GetMenuItemCount()
    {
        return _menuItems.Count;
    }

    public string GetOptionText(int option)
    {
        return _menuItems[option - 1].Item1;
    }
    
    public T GetOptionValue(int option)
    {
        return _menuItems[option - 1].Item2;
    }
    
    public void OnPlayerDisconnect(EventPlayerDisconnect @event)
    {
        if (_playersWithMenuActive.Contains(@event.Userid.SteamID))
        {
            _playersWithMenuActive.Remove(@event.Userid.SteamID);
            _currentPage.Remove(@event.Userid.SteamID);
            
            //Navigation Stuff
            _isNextPresent.Remove(@event.Userid.SteamID);
            _nextIndex.Remove(@event.Userid.SteamID);
            _isPreviousPresent.Remove(@event.Userid.SteamID);
            _previousIndex.Remove(@event.Userid.SteamID);
            _isExitPresent.Remove(@event.Userid.SteamID);
            _previousPageLastIndex.Remove(@event.Userid.SteamID);
        }
    }

    public string GetTitle()
    {
        return _title;
    }
    
    public void SetTitle(string title)
    {
        _title = title;
    }

    public List<Tuple<string, T>> GetOptions(bool showTitle, bool showExit, ulong steamId, bool doPushLastIndex)
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

        if (_currentPage[steamId] > 1) // Show Previous Option
        {
            linesRemaining--;
        }
        
        int startIndex = _previousPageLastIndex[steamId].Peek();
        int endIndex = Math.Min(startIndex + linesRemaining, _menuItems.Count);
        bool nextButton = false;
        if (endIndex != _menuItems.Count) // Check if a next button is needed
        {
            linesRemaining--;
            nextButton = true;
        }
        

        List<Tuple<string, T>> options = new List<Tuple<string, T>>();
        int i = startIndex;
        int displayIndex = 0;
        for (; i < endIndex; i++)
        {
            options.Add(new Tuple<string, T>($"[{displayIndex + 1}] {_menuItems[i].Item1}", _menuItems[i].Item2));
            displayIndex++;
        }
        
        if (_currentPage[steamId] > 1) // Show Previous Option
        {
            options.Add(new Tuple<string, T>($"[{displayIndex + 1}] Previous", default(T)!));
            _isPreviousPresent[steamId] = true;
            _previousIndex[steamId] = displayIndex;
            displayIndex++;
        }
        else
        {
            _isPreviousPresent[steamId] = false;
        }

        if (nextButton)
        {
            options.Add(new Tuple<string, T>($"[{displayIndex + 1}] Next", default(T)!));
            _isNextPresent[steamId] = true;
            _nextIndex[steamId] = displayIndex;
            displayIndex++;
        }
        else
        {
            _isNextPresent[steamId] = false;
        }

        if (doPushLastIndex)
        {
            _previousPageLastIndex[steamId].Push(i - 1);
        }
        return options;
    }

    public void PrintMenu(CCSPlayerController player, bool showTitle, bool showExit, bool doPushLastIndex)
    {
        if (showTitle)
        {
            player.PrintToChat(_title);
        }

        List<Tuple<string, T>> options = GetOptions(showTitle, showExit, player.SteamID, doPushLastIndex);

        foreach (Tuple<string, T> option in options)
        {
            player.PrintToChat(option.Item1);
        }

        if (showExit)
        {
            player.PrintToChat("[0] Exit");
        }
    }

    public void Init(CCSPlayerController player, bool showExit, Action<T, CCSPlayerController> callbackAction)
    {
        //Console.WriteLine($"{GetTitle()}: Adding {player.PlayerName} => {player.SteamID}");
        _playersWithMenuActive.Add(player.SteamID);
        _currentPage.Add(player.SteamID, 1);
        
        //Navigation Stuff
        _isNextPresent.Add(player.SteamID, false);
        _nextIndex.Add(player.SteamID, 0);
        _isPreviousPresent.Add(player.SteamID, false);
        _previousIndex.Add(player.SteamID, 0);
        _isExitPresent.Add(player.SteamID, showExit);
        Stack<int> temp = new Stack<int>();
        temp.Push(0);
        _previousPageLastIndex.Add(player.SteamID, temp);
        
        PrintMenu(player, !String.IsNullOrEmpty(_title), showExit, true);
        _callbackAction = callbackAction;
    }

    public void Cleanup()
    {
        //Console.WriteLine($"Cleanup ({GetTitle()})");
        _callbackAction = null;
        _playersWithMenuActive.Clear();
        _currentPage.Clear();
        
        //Navigation Stuff
        _isNextPresent.Clear();
        _nextIndex.Clear();
        _isPreviousPresent.Clear();
        _previousIndex.Clear();
        _isExitPresent.Clear();
        _previousPageLastIndex.Clear();
    }

    protected virtual void OnSelectOption(CCSPlayerController player, int option)
    {
        int indexCache = _previousPageLastIndex[player.SteamID].Pop();
        List<Tuple<string, T>> options = GetOptions(!String.IsNullOrEmpty(_title), false, player.SteamID, false);
        _previousPageLastIndex[player.SteamID].Push(indexCache);
        if (option <= options.Count)
        {
            //Console.WriteLine("Removing player");
            _playersWithMenuActive.Remove(player.SteamID);
            
            T returnValue = options[option - 1].Item2;
            
            //Navigation Stuff
            _currentPage.Remove(player.SteamID);
            _isNextPresent.Remove(player.SteamID);
            _nextIndex.Remove(player.SteamID);
            _isPreviousPresent.Remove(player.SteamID);
            _previousIndex.Remove(player.SteamID);
            _isExitPresent.Remove(player.SteamID);
            _previousPageLastIndex.Remove(player.SteamID);
        
            //Console.WriteLine("Calling back");
            _callbackAction?.Invoke(returnValue, player);
        }
    }

    private void OnSlot(CCSPlayerController? player, CommandInfo info)
    {
        //Console.WriteLine($"BEGIN(OnSlot): {GetTitle()}");
        //Console.WriteLine($"{player?.PlayerName ?? "NULL"} => {player?.SteamID.ToString() ?? "NULL"};");
        
        if (player == null || !_playersWithMenuActive.Contains(player.SteamID)) return;
        
        //Console.WriteLine($"OnSlot PreviousIndex: {_previousIndex[player.SteamID]}");
        //Console.WriteLine($"OnSlot NextIndex: {_nextIndex[player.SteamID]}");
        
        string optionString = info.GetCommandString.Substring(4);
        if (int.TryParse(optionString, out int option))
        {
            //Console.WriteLine($"OnSlot option: {option}");
            //determine if it is exit or next or previous
            if (_isExitPresent[player.SteamID] && option == 0)
            {
                //Console.WriteLine("Removing player onexit");
                _playersWithMenuActive.Remove(player.SteamID);
                _currentPage.Remove(player.SteamID);
            
                //Navigation Stuff
                _isNextPresent.Remove(player.SteamID);
                _nextIndex.Remove(player.SteamID);
                _isPreviousPresent.Remove(player.SteamID);
                _previousIndex.Remove(player.SteamID);
                _isExitPresent.Remove(player.SteamID);
                _previousPageLastIndex.Remove(player.SteamID);

                player.PrintToChat($"{PugConfig.ChatPrefix} Menu Exited");
                
            }
            else if (_isPreviousPresent[player.SteamID] && option - 1 == _previousIndex[player.SteamID])
            {
                _currentPage[player.SteamID]--;
                _previousPageLastIndex[player.SteamID].Pop();
                _previousPageLastIndex[player.SteamID].Pop();
                PrintMenu(player, !String.IsNullOrEmpty(_title), _isExitPresent[player.SteamID], false);
            }
            else if (_isNextPresent[player.SteamID] && option - 1 == _nextIndex[player.SteamID])
            {
                _currentPage[player.SteamID]++;
                PrintMenu(player, !String.IsNullOrEmpty(_title), _isExitPresent[player.SteamID], true);
            }
            else
            {
                OnSelectOption(player, option);
            }
        }
        
        //Console.WriteLine($"END: {GetTitle()}");
    }

    public void AddCommands(Action<string, string, CommandInfo.CommandCallback> addCommand)
    {
        //Console.WriteLine($"{GetTitle()}: REGISTERING COMMANDS");
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

    public void RemoveCommands(Action<string, CommandInfo.CommandCallback> removeCommand)
    {
        //Console.WriteLine($"{GetTitle()}: REMOVING COMMANDS");
        removeCommand("css_0", OnSlot);
        removeCommand("css_1", OnSlot);
        removeCommand("css_2", OnSlot);
        removeCommand("css_3", OnSlot);
        removeCommand("css_4", OnSlot);
        removeCommand("css_5", OnSlot);
        removeCommand("css_6", OnSlot);
        removeCommand("css_7", OnSlot);
        removeCommand("css_8", OnSlot);
        removeCommand("css_9", OnSlot);
    }
}
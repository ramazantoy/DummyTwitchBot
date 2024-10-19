using Newtonsoft.Json;
using TwBot.Interfaces;

namespace TwBot.Core;

public class CommandService : ICommandService
{
    private Dictionary<string, string>? _commands = new Dictionary<string, string>();
    private const string FilePath = "commands.json";

    public void LoadCommands()
    {
        if (!File.Exists(FilePath)) return;
        
        var json = File.ReadAllText(FilePath);
        _commands = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
    }

    public void AddCommand(string command, string response)
    {
        _commands![command] = response;
        SaveCommands();
    }

    public void RemoveCommand(string command)
    {
        if (_commands == null || !_commands.ContainsKey(command)) return;
        
        _commands.Remove(command);
        SaveCommands();
    }

    public void UpdateCommand(string command, string newResponse)
    {
        if (_commands == null || !_commands.ContainsKey(command)) return;
        
        _commands[command] = newResponse;
        SaveCommands();
    }

    public string? GetCommandResponse(string command)
    {
        return _commands != null && _commands.TryGetValue(command, out var commandResponse) ? commandResponse : null;
    }

    private void SaveCommands()
    {
        var json = JsonConvert.SerializeObject(_commands, Formatting.Indented);
        File.WriteAllText(FilePath, json);
    }
}
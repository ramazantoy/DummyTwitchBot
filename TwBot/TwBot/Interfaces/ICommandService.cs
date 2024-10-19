namespace TwBot.Interfaces;

public interface  ICommandService
{
    void LoadCommands();
    void AddCommand(string command, string response);
    void RemoveCommand(string command);
    void UpdateCommand(string command, string newResponse);
    string? GetCommandResponse(string command);
}
using TwBot.Interfaces;

namespace TwBot.Core;

public class TwitchBot : ITwitchBot
{
    private readonly IrcClient _ircClient;
    private readonly ICommandService _commandService;

    public TwitchBot(IrcClient ircClient, ICommandService commandService)
    {
        _ircClient = ircClient;
        _commandService = commandService;
    }

    public void Connect()
    {
        _commandService.LoadCommands();
        Console.WriteLine("Bot connected");
    }
    
    private string? ExtractUsernameFromUsernotice(string? message)
    {
        try
        {
            var parts = message.Split(';');
            foreach (var part in parts)
            {
                if (part.StartsWith("login="))
                {
                    return part.Split('=')[1]; 
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }
        return null;
    }

public void StartListening()
{
    while (true)
    {
        var message = _ircClient.ReadMessage();
        if (string.IsNullOrEmpty(message))
        {
            continue;
        }

        // Console.WriteLine(message);
        
        string? username;
        if (message.Contains("USERNOTICE"))
        {
            if (message.Contains("msg-id=sub") || message.Contains("msg-id=follow"))
            {
                username = ExtractUsernameFromUsernotice(message);
                if (username != null)
                {
                    _ircClient.SendMessage($"Hoş geldin @{username}! Takip ettiğin için teşekkürler!");
                }
            }
        }

        if (!message.Contains("PRIVMSG")) continue;

        var index = message.IndexOf("PRIVMSG", StringComparison.Ordinal);
        if (index == -1) continue;

        var colonIndex = message.IndexOf(':', index);
        if (colonIndex == -1) continue;

        var chatMessage = message.Substring(colonIndex + 1);
        username = message.Split('!')[0].Substring(1);
        
        if (chatMessage.ToLower().Contains("selam"))
        {
            _ircClient.SendMessage($"Selam @{username}, hoş geldin!");
        }
        
        if (chatMessage.StartsWith("!"))
        {
            var splitMessage = chatMessage.Split(' ');
            var command = splitMessage[0].Substring(1);

            string? response;
            switch (command)
            {
                case "addcommand":
                    if (splitMessage.Length >= 3)
                    {
                        var newCommand = splitMessage[1];
                        response = string.Join(" ", splitMessage, 2, splitMessage.Length - 2);
                        _commandService.AddCommand(newCommand, response);
                        _ircClient.SendMessage($"New command added: !{newCommand}");
                    }
                    break;

                case "removecommand":
                    if (splitMessage.Length == 2)
                    {
                        var commandToRemove = splitMessage[1];
                        _commandService.RemoveCommand(commandToRemove);
                        _ircClient.SendMessage($"Command removed: !{commandToRemove}");
                    }
                    break;

                case "updatecommand":
                    if (splitMessage.Length >= 3)
                    {
                        var commandToUpdate = splitMessage[1];
                        var newResponse = string.Join(" ", splitMessage, 2, splitMessage.Length - 2);
                        _commandService.UpdateCommand(commandToUpdate, newResponse);
                        _ircClient.SendMessage($"Command updated: !{commandToUpdate}");
                    }
                    break;

                default:
                    response = _commandService.GetCommandResponse(command);
                    if (response != null)
                    {
                        _ircClient.SendMessage(response);
                    }
                    break;
            }
        }
    }
}

}
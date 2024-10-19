using System.Net.Sockets;

namespace TwBot.Core;

public class IrcClient
{
    private readonly TcpClient _tcpClient;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly string? _channelName;
    
    public IrcClient(string? botUsername, string? oauthToken, string? channelName)
    { 
        _channelName = channelName;
        _tcpClient = new TcpClient("irc.chat.twitch.tv", 6667);
        _reader = new StreamReader(_tcpClient.GetStream());
        _writer = new StreamWriter(_tcpClient.GetStream());

        _writer.WriteLine($"PASS {oauthToken}");
        _writer.WriteLine($"NICK {botUsername}");
        _writer.WriteLine($"JOIN #{channelName}");
        _writer.Flush();
    }

    public void SendMessage(string message)
    {
        _writer.WriteLine($"PRIVMSG #{_channelName} :{message}");
        _writer.Flush();
    }

    public string? ReadMessage()
    {
        return _tcpClient.Available > 0 ? _reader.ReadLine() : null;
    }
}
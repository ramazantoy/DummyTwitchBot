using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwBot.Core;
using TwBot.Interfaces;

namespace TwBot;

public static class Program
{
    private static void Main(string[] args)
    {
   
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            
            var twitchSettings = configuration.GetSection("TwitchSettings").Get<TwitchSettings>();
            
            var serviceProvider = new ServiceCollection()
                .AddSingleton<ICommandService, CommandService>()
                .AddSingleton<IrcClient>(provider => new IrcClient(
                    twitchSettings?.Username, 
                    twitchSettings?.OAuthToken, 
                    twitchSettings?.ChannelName))
                .AddSingleton<ITwitchBot, TwitchBot>()
                .BuildServiceProvider();

            var bot = serviceProvider.GetService<ITwitchBot>();
            bot?.Connect();
            bot?.StartListening();
        }
    
}
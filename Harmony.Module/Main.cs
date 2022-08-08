using CloudTheWolf.DSharpPlus.Scaffolding.Shared.Interfaces;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
//using DSharpPlus.Lavalink;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using Harmony.Module.Commands;


namespace Harmony.Module
{
    public class Main : IPlugin
    {
        public string Name => "Harmony Bot";
        public string Description => "Bot for LegacyRP Harmony Mechanic Shop";
        public int Version => 3;
        public static ILogger<Logger> Logger;
        public static InteractivityExtension Interactivity;
        public static DiscordClient Client;
        private static DiscordConfiguration _discordConfiguration;

        public void InitPlugin(IBot bot, ILogger<Logger> logger, DiscordConfiguration discordConfiguration, IConfigurationRoot applicationConfig)
        {
            try
            {
                Logger = logger;
                LoadConfig(applicationConfig);
                _discordConfiguration = discordConfiguration;
                 logger.LogInformation(this.Name + ": Loaded successfully!");
                 if (!Libs.OperatingSystem.IsWindows())
                     logger.LogInformation("We are NOT on Windows");
                 Interactivity = bot.Client.GetInteractivity();
                 bot.Client.Intents.AddIntent(DiscordIntents.All);
                 Client = bot.Client;
                 bot.Client.GuildDownloadCompleted += SetStatus;
                AddCommands(bot, Name);

            }
            catch (Exception e)
            {
                logger.LogCritical($"Failed to load {Name} \n {e}");
            }
        }

        private static void LoadConfig(IConfiguration applicationConfig)
        {
            Options.MySqlHost = applicationConfig.GetValue<string>("SQL:Host");
            Options.MySqlPort = applicationConfig.GetValue<int>("SQL:Port");
            Options.MySqlUsername = applicationConfig.GetValue<string>("SQL:user");
            Options.MySqlPassword = applicationConfig.GetValue<string>("SQL:pass");
            Options.MySqlDatabase = applicationConfig.GetValue<string>("SQL:name");
            Options.CompanyName = applicationConfig.GetValue<string>("CompanyName");
        }

        private static void AddCommands(IBot bot, string Name)
        {
            bot.Commands.RegisterCommands<StaffCommands>();
            Logger.LogInformation(Name + ": Registered {0}!", nameof(StaffCommands));
            
        }

        private static async Task SetStatus(DiscordClient client, GuildDownloadCompletedEventArgs args)
        {
            await client.UpdateStatusAsync(new DiscordActivity("The World Go By", (ActivityType)3));
        }
    }
}
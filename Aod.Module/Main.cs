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
using DSharpPlus.CommandsNext.Converters;
using System.Linq;

namespace Aod.Module
{
    public class Main : IPlugin
    {
        public string Name => "AOD Plugin";

        public string Description => "AOD Plugin";

        public int Version => 1;

        public static ILogger<Logger> Logger;
        public static InteractivityExtension Interactivity;
        public static DiscordClient Client;
        private static DiscordConfiguration _discordConfiguration;
        internal static IConfiguration ApplicationConfig;

        public void InitPlugin(IBot bot, ILogger<Logger> logger, DiscordConfiguration discordConfiguration, IConfigurationRoot applicationConfig)
        {
            try
            {
                Logger = logger;
                LoadConfig(applicationConfig, bot);
                _discordConfiguration = discordConfiguration;
                logger.LogInformation(this.Name + ": Loaded successfully!");
                if (!Libs.OperatingSystem.IsWindows())
                    logger.LogInformation("We are NOT on Windows");
                Interactivity = bot.Client.GetInteractivity();
                Client = bot.Client;
                Client.Intents.AddIntent(DiscordIntents.All);
                Client.GuildDownloadCompleted += Actions.GuildDownloadCompleted.SetStatus;
                Client.MessageCreated += Actions.OnMessageCreation.AddVoteReactions;
                Client.Heartbeated += Actions.Heartbeated.CheckNextChurch;
                
                AddCommands(bot, this.Name);
            }
            catch (Exception e)
            {
                logger.LogCritical($"Failed to load {this.Name} \n {e}");
            }
        }

        private static void LoadConfig(IConfiguration applicationConfig, IBot bot)
        {
            ApplicationConfig = applicationConfig;
            //TODO Add More Config at a later date
            return;
        }

        private static void AddCommands(IBot bot, string Name)
        {

            /*
             * Add Command Later 
             * bot.SlashCommandsExt.RegisterCommands<MyActions>();
             * Logger.LogInformation(Name + ": Registered {0}!", nameof(StaffActions));
             */

        }
     
    }
}
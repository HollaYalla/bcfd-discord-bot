using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using Microsoft.Extensions.Configuration;

namespace Aod.Module.Actions
{
    internal class GuildDownloadCompleted
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required GuildDownloadCompleted Event Handler")]
        internal static async Task SetStatus(DiscordClient client, GuildDownloadCompletedEventArgs args)
        {
            
            Options.VotesChannelHighTable = client.Guilds[Main.ApplicationConfig.GetValue<ulong>("Discord:GuildId")]
                .GetChannel(Main.ApplicationConfig.GetValue<ulong>("Discord:Channels:HighVote"));
            Options.VotesChannelLowTable = client.Guilds[Main.ApplicationConfig.GetValue<ulong>("Discord:GuildId")]
                .GetChannel(Main.ApplicationConfig.GetValue<ulong>("Discord:Channels:LowVote"));
            Options.ReminderChannel = client.Guilds[Main.ApplicationConfig.GetValue<ulong>("Discord:GuildId")]
                .GetChannel(Main.ApplicationConfig.GetValue<ulong>("Discord:Channels:Reminder"));
            Options.EventsChannel = client.Guilds[Main.ApplicationConfig.GetValue<ulong>("Discord:GuildId")]
                .GetChannel(Main.ApplicationConfig.GetValue<ulong>("Discord:Channels:Events"));
            await client.UpdateStatusAsync(new DiscordActivity("Lynette Stab People", ActivityType.Watching));
        }
    }
}

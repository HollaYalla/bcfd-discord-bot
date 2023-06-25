using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Aod.Module.Actions
{
    internal class Heartbeated
    {
        internal static async Task CheckNextChurch(DiscordClient sender, HeartbeatEventArgs e)
        {           
            Main.Logger.LogInformation("Heartbeat");
            if (Options.ReminderChannel == null) return;
            try
            {
                var channelMessages = await Options.ReminderChannel.GetMessagesAsync();
                var lastMessageSent = channelMessages[0];
                TimeSpan diff = DateTime.UtcNow.Subtract(lastMessageSent.CreationTimestamp.UtcDateTime);
                Main.Logger.LogInformation($"{DateTime.UtcNow.Subtract(lastMessageSent.CreationTimestamp.DateTime).TotalHours}");
                if (DateTime.UtcNow.Subtract(lastMessageSent.CreationTimestamp.DateTime).TotalHours < 23) 
                {
                    Main.Logger.LogInformation("No need to remind people");
                    return;
                }
            }
            catch
            {
                Main.Logger.LogInformation("No Messages");
            }

            var allEvents = await Main.Client.Guilds[1050419292476293181].GetEventsAsync();
            var newest = DateTime.MinValue;
            var search = DateTime.MinValue;
            var now = DateTime.UtcNow;
            DiscordScheduledGuildEvent nextEvent = null;
            foreach (var discordEvent in allEvents)
            {
                if (now.ToUniversalTime() > discordEvent.StartTime.DateTime) continue;
                if (newest == DateTime.MinValue)
                {
                    newest = discordEvent.StartTime.DateTime;
                    nextEvent = discordEvent;
                    continue;
                }
                else
                {
                    search = discordEvent.StartTime.DateTime;
                }
                if (search < newest)
                {
                    Main.Logger.LogInformation($"{search} is closer than {newest}");
                    nextEvent = discordEvent;
                }
                else
                {
                    Main.Logger.LogInformation($"{newest} is closer than {search}");
                }
            }


            if (nextEvent.StartTime.Subtract(now.ToUniversalTime()).TotalHours > 24)
            {
                Main.Logger.LogInformation("Next Event Is Over 24 Hours Away, No need to do anything yet.");
                return;
            }

            var desc = string.IsNullOrWhiteSpace(nextEvent.Description) ? "None" : nextEvent.Description;

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Title = nextEvent.Name,
                Description = "Next Church Is Almost Upon Us!",
                Color = DiscordColor.Purple

            }.AddField("Start Time", $"<t:{nextEvent.StartTime.ToUnixTimeSeconds()}:f>")
                .AddField("End Time", $"<t:{nextEvent.EndTime.Value.ToUnixTimeSeconds()}:f>", true)
                .AddField("Location", nextEvent.Metadata.Location)
                .AddField("Additional Details", desc)
                .WithImageUrl("https://cdn.discordapp.com/attachments/1054807586194595931/1074696201192099861/image.png")
                .WithFooter("Created By CloudTheWolf 🐺", "https://cdn.discordapp.com/attachments/839793938281791508/839794154149117952/pngtuber-closed-2.png");
            await Options.ReminderChannel.SendMessageAsync("<@&1071145434074075338> - ** 🔔 Event Reminder**", embedBuilder.Build());
        }
    }
}

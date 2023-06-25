using DSharpPlus.Entities;

namespace Aod.Module
{
    internal class Options
    {
        public static DiscordChannel VotesChannelHighTable { get; set; }
        public static DiscordChannel VotesChannelLowTable { get; set; }
        public static DiscordChannel ReminderChannel { get; set; }
        public static DiscordChannel EventsChannel { get; set; }
    }
}

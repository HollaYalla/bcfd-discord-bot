using DSharpPlus.Entities;

namespace BCFD.Module
{
    internal class Options
    {
        public static ulong GuildId { get; set; }

        public static string RestApiUrl { get; set; } = "https://rest.opfw.net/c3"; //Default to International

        /// <summary>
        /// OP-FW Week Zero EPOCH Timestamp
        /// </summary>
        public static int WeekZero { get; set; } = 1609113600; // 1609113600 = Monday, 28 December 2020 00:00:00

        public static string ApiKey { get; set; } // Get this from OP-FW Discord
    }
}
using DSharpPlus.Entities;

namespace Harmony.Module
{
    internal class Options
    {
        public static string MySqlHost { get; set; } = null!;

        public static int MySqlPort { get; set; }

        public static string MySqlUsername { get; set; } = null!;

        public static string MySqlPassword { get; set; } = null!;

        public static string MySqlDatabase { get; set; } = null!;

        public static string CompanyName { get; set; } = null!;

        public static DiscordRole ManagerRole { get; set; }
    }
}
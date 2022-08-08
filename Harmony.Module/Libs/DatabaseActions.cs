
using CloudTheWolf.DSharpPlus.Scaffolding.Data;
using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Newtonsoft.Json.Linq;

namespace Harmony.Module.Libs
{
    internal class DatabaseActions
    {
        public static ILogger<Logger> DbLogger;
        private static MySqlDataAccess _sda;

        internal DatabaseActions()
        {
            DbLogger = Main.Logger;
            DatabaseActions._sda = new MySqlDataAccess();
            MySqlDataAccess sda = DatabaseActions._sda;
            MySqlConnectionStringBuilder connStringBuilder = new MySqlConnectionStringBuilder
            {
                Server = Options.MySqlHost,
                Port = (uint)Options.MySqlPort,
                UserID = Options.MySqlUsername,
                Password = Options.MySqlPassword,
                Database = Options.MySqlDatabase
            };
            ILogger<Logger> dbLogger = DbLogger;
            sda.LoadConnectionString(connStringBuilder.ToString(), dbLogger);
        }

        /// <summary>
        /// Get work time for specified user
        /// </summary>
        /// <param name="name">Name of the user (Defaults to invoker)</param>
        /// <param name="currentWeek"></param>
        /// <returns></returns>
        public JArray GetUserTime(string name, bool currentWeek = true)
        {
            var startOfWeekDate = GetStartOfWeekDate(!currentWeek ? DateTime.Today.AddDays(-7.0) : DateTime.Today);
            var timeZoneInfo = OperatingSystem.IsWindows() ? 
                TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time") : 
                TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
            var startTime = timeZoneInfo.IsDaylightSavingTime(startOfWeekDate.Date) ? "00:01:00" : "00:00:00";
            var endTime = timeZoneInfo.IsDaylightSavingTime(startOfWeekDate.Date) ? "00:00:59" : "23:59:59";
            var timeWindow = !currentWeek ? $"`clockInAt` between '{startOfWeekDate:yyyy-MM-dd} {startTime}' " +
                $"and '{startOfWeekDate.AddDays(7):yyyy-MM-dd} {endTime}'" : $"`clockInAt` > " +
                $"'{startOfWeekDate:yyyy-MM-dd} {startTime}'";
            var command =
                $"SELECT SUM(`totalTime`) as Time FROM `workTime` INNER JOIN `users` on workTime.cid = users.cid WHERE {timeWindow} " +
                $"and users.name = '{name.Replace("'", "''")}';";
            
            return JArray.Parse(_sda!.Request(command, DbLogger));
        }
        /// <summary>
        /// Get total work time for specified user
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Name of the user (Defaults to invoker)</returns>
        public JArray GetUserTotalTime(string name) => JArray.Parse(_sda!.Request("SELECT SUM(`totalTime`) as Time FROM `workTime` INNER JOIN `users` on workTime.cid = users.cid WHERE users.name = '" + name.Replace("'", "''") + "';", DbLogger));

        public JArray GetAllStaff() => JArray.Parse(_sda!.Request("SELECT * FROM `users` WHERE `disabled` = 0;", DbLogger));
        /// <summary>
        /// Find the Beginning of the week for the date provided
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetStartOfWeekDate(DateTime date)
        {
            int num = (int)(date.DayOfWeek - 1);
            if (num == -1)
                num = 7;
            return date.AddDays(-num);
        }
    }
}

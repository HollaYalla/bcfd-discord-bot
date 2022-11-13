
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

            var startOfWeekDate = GetStartOfWeekDate(!currentWeek ? DateTime.Today.AddDays(-7) : DateTime.Today);
            var timeZoneInfo = OperatingSystem.IsWindows() ? 
                TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time") : 
                TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
            var startTime = timeZoneInfo.IsDaylightSavingTime(startOfWeekDate.Date) ? "00:01:00" : "00:00:00";
            var endDate = timeZoneInfo.IsDaylightSavingTime(startOfWeekDate.Date)
                ? startOfWeekDate.AddDays(7)
                : startOfWeekDate.AddDays(6);
            var endTime = timeZoneInfo.IsDaylightSavingTime(startOfWeekDate.Date) ? "00:00:59" : "23:59:59";

            DbLogger.LogInformation($"Get Time For User {name} between {startOfWeekDate:yyyy-MM-dd} {startTime} and {endDate:yyyy-MM-dd} {endTime}");

            var timeWindow = !currentWeek ? $"`clockInAt` between '{startOfWeekDate:yyyy-MM-dd} {startTime}' " +
                $"and '{endDate:yyyy-MM-dd} {endTime}'" : 
                $"`clockInAt` > '{startOfWeekDate:yyyy-MM-dd} {startTime}'";
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
                num = 6;
            return date.AddDays(-num);
        }

        /// <summary>
        /// Force a user Off Duty
        /// </summary>
        /// <param name="name"></param>
        public void ClockOutUser(string name)
        {
            DbLogger.LogInformation($"Setting {name} as Off Duty");
            _sda.Request($"UPDATE `users` SET `onDuty` = 0 where name = '{name.Replace("'","''")}'",DbLogger);
            DbLogger.LogInformation($"Update workTime table");
            _sda.Request($"UPDATE `workTime` JOIN `users` on `users`.`cid` = `workTime`.`cid` SET `workTime`.`clockOutAt` = CURRENT_TIMESTAMP WHERE `users`.`name` = '{name.Replace("'", "''")}' and `workTime`.`totalTime` = 0;", DbLogger);
        }
        /// <summary>
        /// Update the user to be in a Clocked In Status
        /// </summary>
        /// <param name="name"></param>
        /// <param name="state"></param>
        public void ClockInUser(string name, int id, string state,bool update = false)
        {
            DbLogger.LogInformation($"Setting {name} as {state}");
            _sda.Request($"UPDATE `users` SET `onDuty` = 1, workingAs = '{state}' where id = '{id}'", DbLogger);
            if(update) return;
            DbLogger.LogInformation($"Update workTime table");
            _sda.Request($"INSERT INTO `workTime`( `cid`) VALUES ('{id}')", DbLogger);
        }

        /// <summary>
        /// Debug Function to return generated SQL 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string DebugGetUserTime(string name)
        {
            var startOfWeekDate = GetStartOfWeekDate(DateTime.Today);
            var startOfLastWeekDate = GetStartOfWeekDate(DateTime.Today.AddDays(-7.0));
            var timeZoneInfo = OperatingSystem.IsWindows() ?
                TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time") :
                TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
            var startTime = timeZoneInfo.IsDaylightSavingTime(startOfWeekDate.Date) ? "00:01:00" : "00:00:00";
            var endDate = timeZoneInfo.IsDaylightSavingTime(startOfWeekDate.Date)
                ? startOfWeekDate.AddDays(7)
                : startOfWeekDate.AddDays(6);
            var endOfLastWeek = timeZoneInfo.IsDaylightSavingTime(startOfLastWeekDate.Date)
                ? startOfLastWeekDate.AddDays(7)
                : startOfLastWeekDate.AddDays(6);
            var endTime = timeZoneInfo.IsDaylightSavingTime(startOfWeekDate.Date) ? "00:00:59" : "23:59:59";
            var timeWindow = $"`clockInAt` > '{startOfWeekDate:yyyy-MM-dd} {startTime}'";
            var lastWeekTimeWindow = $"`clockInAt` between '{startOfLastWeekDate:yyyy-MM-dd} {startTime}' " +
                                     $"and '{endOfLastWeek:yyyy-MM-dd} {endTime}'";
            return
                $"SELECT SUM(`totalTime`) as Time FROM `workTime` INNER JOIN `users` on workTime.cid = users.cid WHERE {timeWindow} " +
                $"and users.name = '{name.Replace("'", "''")}';\n" +
            $"SELECT SUM(`totalTime`) as Time FROM `workTime` INNER JOIN `users` on workTime.cid = users.cid WHERE {lastWeekTimeWindow} " +
                $"and users.name = '{name.Replace("'", "''")}';\n";
        }

        public JArray GetUser(string name)
        {
            DbLogger.LogInformation($"Getting Profile Of {name}");
            var user = _sda.Request($"SELECT * FROM `users` WHERE `disabled` = 0 and name = '{name.Replace("'", "''")}';", DbLogger);
            return JArray.Parse(user);
        }
    }
}

using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Harmony.Module.Common
{
    using System.Diagnostics;
    using Harmony.Module.Libs;

    internal class StaffCommon
    {
        private static readonly DatabaseActions Da = new();
        private static readonly ILogger<Logger> Logger = Main.Logger;

        public static async Task GetUserTime(InteractionContext ctx, DiscordMember member)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (member.IsBot)
                return;
            try
            {
                var staff = Da.GetStaffMember(member);
                var message = StaffCommon.CalculateTime(member, staff[0]);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(message));
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Error: {ex.Message}\n```{ex}```"));
            }
        }

        public static async Task GetThisWeek(CommandContext ctx)
        {
            var (embed, embed2, embed3) = await GetWeekEmbed();

            await ctx.Channel.SendMessageAsync("This weeks timesheets are so far", embed);
            if (embed2.Fields.Count != 0)
            {
                await ctx.Channel.SendMessageAsync("Pt 2...", embed2);
            }
            if (embed3.Fields.Count != 0)
            {
                await ctx.Channel.SendMessageAsync("Pt 3...", embed3);
            }
        }

        public static async Task GetThisWeek(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var (embed, embed2, embed3) = await GetWeekEmbed();

            if (embed3.Fields.Count > 0)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("This weeks timesheets so far...")
                    .AddEmbeds(new List<DiscordEmbed>() { embed, embed2, embed3 }));
                return;
            }
            if (embed3.Fields.Count == 0 && embed2.Fields.Count > 0)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("This weeks timesheets so far...")
                    .AddEmbeds(new List<DiscordEmbed>() { embed, embed2 }));
                return;
            }
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("This weeks timesheets so far...")
                .AddEmbeds(new List<DiscordEmbed>() { embed }));
        }

        public static async Task GetLastWeek(CommandContext ctx)
        {
            var (embed, embed2, embed3) = await GetWeekEmbed(false);

            await ctx.Channel.SendMessageAsync("Last Weeks Timesheets Are Here", embed);
            if (embed2.Fields.Count != 0)
            {
                await ctx.Channel.SendMessageAsync("Pt 2...", embed2);
            }
            if (embed3.Fields.Count != 0)
            {
                await ctx.Channel.SendMessageAsync("Pt 3...", embed3);
            }
        }

        public static async Task GetLastWeek(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var (embed, embed2, embed3) = await GetWeekEmbed(false);

            if (embed3.Fields.Count > 0)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("Last Weeks Timesheets")
                    .AddEmbeds(new List<DiscordEmbed>() { embed, embed2, embed3 }));
                return;
            }
            if (embed3.Fields.Count == 0 && embed2.Fields.Count > 0)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                    .WithContent("Last Weeks Timesheets")
                    .AddEmbeds(new List<DiscordEmbed>() { embed, embed2 }));
                return;
            }
            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent("Last Weeks Timesheets")
                .AddEmbeds(new List<DiscordEmbed>() { embed }));
        }

        public static async Task GetTotalTime(InteractionContext ctx, DiscordMember member)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            StaffCommon.Logger.LogInformation("Getting work time for " + member.Nickname);
            try
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(CalculateTotalTime(member)));
            }
            catch (Exception ex)
            {
                Main.Logger.LogError(ex.Message);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("<@!126060427250630656>, Something went wrong\n" + ex.Message));
            }
        }

        public static string CalculateTotalTime(DiscordMember member)
        {
            JArray userTotalTime = StaffCommon.Da.GetUserTotalTime(member.Nickname);
            if (string.IsNullOrEmpty(userTotalTime[0]["Time"].ToString()))
                return member.Mention + " has worked for `00:00:00` In Total";
            var time = TimeSpan.FromSeconds(int.Parse(userTotalTime[0]["Time"].ToString())).ToString();
            var message = $"has worked for `{time.Replace(".", " Day(s) - ")}` in total";
            Logger.LogInformation($"{member.Nickname} {message}");
            return $"{member.Mention} {message}";
        }

        
        private static async Task<(DiscordEmbedBuilder embed, DiscordEmbedBuilder embed2, DiscordEmbedBuilder embed3)> GetWeekEmbed(bool currentWeek = true)
        {
            var allStaff = Da.GetAllStaff();
            var records = new List<KeyValuePair<string, Dictionary<string,object>>>();
            var prefix = currentWeek ? "This" : "Last";
            foreach (var staff in allStaff)
            {
                var data = Da.GetUserTime(staff, currentWeek);
                var repairs = Da.GetUserRepairs(staff,currentWeek);
                var impounds = Da.GetUserImpounds (staff, currentWeek);
                var time = string.IsNullOrEmpty(data[0]["Time"].ToString())
                    ? TimeSpan.FromSeconds(0.0)
                    : TimeSpan.FromSeconds(int.Parse(data[0]["Time"].ToString()));
                var values = new Dictionary<string, object>
                {
                    { "time", time }
                };
                values.Add("repairs",repairs?.Count ?? -1);
                values.Add("impounds",impounds?.Count ?? 0);
                records.Add(new KeyValuePair<string, Dictionary<string, object>>(staff["name"].ToString(), values));

            }

            var embed = new DiscordEmbedBuilder()
            {
                Title = $"{prefix} Weeks Timesheets",
                Color = DiscordColor.DarkGreen
            };
            var embed2 = new DiscordEmbedBuilder()
            {
                Title = $"{prefix} Weeks Timesheets Part 2",
                Color = DiscordColor.Green
            };
            var embed3 = new DiscordEmbedBuilder()
            {
                Title = $"{prefix} Weeks Timesheets Part 3",
                Color = DiscordColor.SapGreen
            };
            foreach (var record in records)
            {
                var inline = true;
                var stats = record.Value;
                var stat = $"🕛: {stats["time"].ToString()!.Replace(".", "d - ")}";
                if (int.Parse(stats["repairs"].ToString()) > -1)
                    stat += $"\n 🧰: {stats["repairs"]}";
                else
                    stat += "\n 🧰: --";

                stat += $"\n 🛻: {stats["impounds"]}";

                var activeEmbed = new DiscordEmbedBuilder();

                switch (embed.Fields.Count)
                {
                    case < 25:
                        activeEmbed = embed;
                        break;
                    case 25 when embed2.Fields.Count < 25:
                        activeEmbed = embed2;
                        break;
                    case 25 when embed2.Fields.Count == 25:
                        activeEmbed = embed3;
                        break;
                }

                activeEmbed.AddField(record.Key, stat,inline);

            }

            return (embed, embed2, embed3);
        }


        private static string CalculateTime(DiscordMember member, JToken memberRecord)
        {
            Logger.LogInformation("Getting work time for " + member.Nickname);
            try
            {
                JArray userTime = StaffCommon.Da.GetUserTime(memberRecord);
                if (string.IsNullOrEmpty(userTime[0]["Time"]!.ToString()))
                    return member.Mention + " has worked for `00:00:00` this week";
                string str = TimeSpan.FromSeconds(int.Parse(userTime[0]["Time"].ToString())).ToString();
                Logger.LogInformation(member.Nickname + " has worked for `" + str.Replace(".", " Day(s) - ") + "` this week");
                return member.Mention + " has worked for `" + str.Replace(".", "d ") + "` this week";
            }
            catch (Exception ex)
            {
                Logger.LogError($"{ex}");
                throw;
            }
        }
    }
}

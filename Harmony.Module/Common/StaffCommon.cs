using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Harmony.Module.Libs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Harmony.Module.Common
{
    internal class StaffCommon
    {
        private static readonly DatabaseActions Da = new();
        private static readonly ILogger<Logger> Logger = Main.Logger;

        public static async Task GetUserTime(CommandContext ctx, DiscordMember member)
        {
            if (member == null)
                member = ctx.Member;
            if (member!.IsBot)
                return;
            try
            {
                await ctx.Message.DeleteAsync();
                var message = CalculateTime(member);
                await ctx.Channel.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync($"Error: {ex.Message}\n```{ex}```");
            }
        }

        public static async Task GetUserTime(InteractionContext ctx, DiscordMember member)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            if (member.IsBot)
                return;
            try
            {
                var message = StaffCommon.CalculateTime(member);
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

        public static async Task GetTotalTime(CommandContext ctx, DiscordMember member)
        {
            if (member.IsBot)
                return;
            try
            {
                await ctx.Message.DeleteAsync();
                await ctx.Channel.SendMessageAsync(CalculateTime(member));
            }
            catch (Exception ex)
            {
                Main.Logger.LogError(ex.Message);
                await ctx.RespondAsync(ex.Message);
            }
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
            var records = new List<KeyValuePair<string, TimeSpan>>();
            var prefix = currentWeek ? "This" : "Last";
            foreach (var staff in allStaff)
            {
                var data = Da.GetUserTime(staff["name"].ToString(), currentWeek);
                var time = string.IsNullOrEmpty(data[0]["Time"].ToString())
                    ? TimeSpan.FromSeconds(0.0)
                    : TimeSpan.FromSeconds(int.Parse(data[0]["Time"].ToString()));
                records.Add(new KeyValuePair<string, TimeSpan>(staff["name"].ToString(), time));
            }

            var embed = new DiscordEmbedBuilder()
            {
                Title = $"{prefix} Weeks Timesheets",
                Color = DiscordColor.Green
            };
            var embed2 = new DiscordEmbedBuilder()
            {
                Title = $"{prefix} Weeks Timesheets Part 2",
                Color = DiscordColor.Yellow
            };
            var embed3 = new DiscordEmbedBuilder()
            {
                Title = $"{prefix} Weeks Timesheets Part 3",
                Color = DiscordColor.Red
            };
            foreach (var record in records)
            {
                var extra = "✅ ";
                TimeSpan timeSpan;
                if (record.Key != "Max Maxwell")
                {
                    timeSpan = record.Value;
                    if (timeSpan.TotalHours == 0.0)
                    {
                        extra = "❗ ";
                    }
                    else
                    {
                        timeSpan = record.Value;
                        if (timeSpan.TotalHours <= 2.0)
                        {
                            extra = "⚠️ ";
                        }
                        else
                        {
                            timeSpan = record.Value;
                            int num;
                            if (timeSpan.TotalHours > 2.0)
                            {
                                timeSpan = record.Value;
                                num = timeSpan.TotalHours < 4.0 ? 1 : 0;
                            }
                            else
                                num = 0;

                            if (num != 0)
                                extra = "\uD83D\uDFE1 ";
                        }
                    }
                }

                if (embed.Fields.Count < 25)
                    embed.AddField(extra + record.Key, record.Value.ToString().Replace(".", "d - "));
                else if (embed.Fields.Count == 25 && embed2.Fields.Count < 25)
                    embed2.AddField(extra + record.Key, record.Value.ToString().Replace(".", "d - "));
                else if (embed2.Fields.Count == 25 && embed3.Fields.Count < 25)
                    embed3.AddField(extra + record.Key, record.Value.ToString().Replace(".", "d - "));

            }

            return (embed, embed2, embed3);
        }


        private static string CalculateTime(DiscordMember member)
        {
            Logger.LogInformation("Getting work time for " + member.Nickname);
            try
            {
                JArray userTime = StaffCommon.Da.GetUserTime(member.Nickname);
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

using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Harmony.Module.Libs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Harmony.Module.Commands
{
    internal class StaffCommands : BaseCommandModule
    {
        private DatabaseActions da = new DatabaseActions();
        private ILogger<Logger> logger = Main.Logger;

        [Command("time")]
        [Aliases(new string[] { "t" })]
        [Description("Get how much time you have worked on this week")]
        public async Task Time(CommandContext ctx, DiscordMember member = null, [RemainingText] string extra = null)
        {
            if (member == null) member = ctx.Member;
            if (member!.IsBot) return;
            logger.LogInformation("Getting worktime for " + member.Nickname);
            try
            {
                var data = da.GetUserTime(member.Nickname);

                if (string.IsNullOrEmpty((data[0]["Time"]).ToString()))
                {
                    await ctx.Channel.SendMessageAsync(member.Mention + " has worked for `00:00:00` this week");
                    return;
                }
                var workTime = TimeSpan.FromSeconds(int.Parse(data[0]["Time"].ToString())).ToString();
                logger.LogInformation(member.Nickname + " has worked for `" + workTime.Replace(".", " Day(s) - ") + "` this week");
                await ctx.Channel.SendMessageAsync(member.Mention + " has worked for `" + workTime.Replace(".", "d ") + "` this week");
            }
            catch (Exception ex)
            {
                Main.Logger.LogError(ex.Message);
                await ctx.RespondAsync(ex.Message);
            }
        }

        [Command("thisWeek")]
        [Aliases(new string[] { "tw" })]
        [Description("Get times for all staff for current week")]
        public async Task ThisWeek(CommandContext ctx)
        {
            JArray allStaff = da.GetAllStaff();
            List<KeyValuePair<string, TimeSpan>> records = new List<KeyValuePair<string, TimeSpan>>();
            foreach (JToken staff in allStaff)
            {
                JArray data = da.GetUserTime((staff["name"]).ToString());
                TimeSpan time = string.IsNullOrEmpty(data[0]["Time"].ToString()) ? TimeSpan.FromSeconds(0.0) : TimeSpan.FromSeconds(int.Parse(data[0]["Time"].ToString()));
                records.Add(new KeyValuePair<string, TimeSpan>((staff["name"]).ToString(), time));
            }
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Title = "This Weeks Timesheets",
                Color = DiscordColor.Green
            };
            DiscordEmbedBuilder embed2 = new DiscordEmbedBuilder()
            {
                Title = "This Weeks Timesheets Part 2",
                Color = DiscordColor.Yellow
            };
            DiscordEmbedBuilder embed3 = new DiscordEmbedBuilder()
            {
                Title = "This Weeks Timesheets Part 3",
                Color = DiscordColor.Red
            };
            foreach (KeyValuePair<string, TimeSpan> keyValuePair in records)
            {
                KeyValuePair<string, TimeSpan> record = keyValuePair;
                string extra = "✅ ";
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
                try
                {
                    if (embed.Fields.Count < 25)
                    {
                        embed.AddField(extra + record.Key, record.Value.ToString().Replace(".", "d - "), false);
                    }
                    else if (embed.Fields.Count == 25 && embed2.Fields.Count < 25)
                    {
                        embed2.AddField(extra + record.Key, record.Value.ToString().Replace(".", "d - "), false);
                    }
                    else if (embed2.Fields.Count == 25 && embed3.Fields.Count < 25)
                    {
                        embed3.AddField(extra + record.Key, record.Value.ToString().Replace(".", "d - "), false);
                    }
                }
                catch (Exception ex)
                {
                    await ctx.Message.Channel.SendMessageAsync("<@!126060427250630656>, Something went wrong\n" + ex.Message);
                }
            }
            await ctx.Channel.SendMessageAsync("This weeks timesheets are so far", embed);
            if (embed2.Fields.Count != 0)
            {
                await ctx.Channel.SendMessageAsync("Pt 2...", embed2);
            }
            if (embed3.Fields.Count != 0)
            {
                await ctx.Channel.SendMessageAsync("Pt 3...", embed3);
            }
            await ctx.Message.DeleteAsync();
        }

        [Command("lastWeek")]
        [Aliases(new string[] { "lw" })]
        [Description("Get all activity for Last Week")]
        public async Task LastWeek(CommandContext ctx)
        {
            var allStaff = da.GetAllStaff();
            var records = new List<KeyValuePair<string, TimeSpan>>();
            foreach (var staff in allStaff)
            {
                var data = da.GetUserTime((staff["name"]).ToString(), false);
                var time = string.IsNullOrEmpty(data[0]["Time"].ToString()) ? TimeSpan.FromSeconds(0.0) : TimeSpan.FromSeconds((double)int.Parse(data[0]["Time"].ToString()));
                records.Add(new KeyValuePair<string, TimeSpan>((staff["name"]).ToString(), time));
                data = (JArray)null;
                time = new TimeSpan();
            }
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Last Weeks Timesheets",
                Color = DiscordColor.Purple
            };
            var embed2 = new DiscordEmbedBuilder()
            {
                Title = "Last Weeks Timesheets Part 2",
                Color = DiscordColor.Yellow
            };
            var embed3 = new DiscordEmbedBuilder()
            {
                Title = "Last Weeks Timesheets Part 3",
                Color = DiscordColor.Red
            };
            foreach (KeyValuePair<string, TimeSpan> keyValuePair in records)
            {
                KeyValuePair<string, TimeSpan> record = keyValuePair;
                string extra = "✅ ";
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
                try
                {
                    if (embed.Fields.Count < 25)
                    {
                        embed.AddField(extra + record.Key, record.Value.ToString().Replace(".", "d - "), false);
                    }
                    if (embed.Fields.Count == 25 && embed2.Fields.Count < 25)
                    {
                        embed2.AddField(extra + record.Key, record.Value.ToString().Replace(".", "d - "), false);
                    }
                    if (embed2.Fields.Count == 25 && embed3.Fields.Count < 25)
                    {
                        embed3.AddField(extra + record.Key, record.Value.ToString().Replace(".", "d - "), false);
                    }
                }
                catch (Exception ex)
                {
                    await ctx.Message.Channel.SendMessageAsync("<@!126060427250630656>, Something went wrong\n" + ex.Message);
                }
            }
            await ctx.Channel.SendMessageAsync("@here last weeks timesheets are here", embed);
            if (embed2.Fields.Count != 0)
            {
                await ctx.Channel.SendMessageAsync("Pt 2...", embed2);
            }
            if (embed3.Fields.Count != 0)
            {
                await ctx.Channel.SendMessageAsync("Pt 3...", embed2);
            }
            await ctx.Message.DeleteAsync();
        }

        [Command("toaltime")]
        [Aliases(new string[] { "tt" })]
        [Description("Get how much time you have worked on this week")]
        public async Task TotalTime(CommandContext ctx, DiscordMember member = null, [RemainingText] string extra = null)
        {
            if (member == null) member = ctx.Member;
            if (member.IsBot) return;
            logger.LogInformation("Getting work time for " + member.Nickname);
            try
            {
                JArray data = da.GetUserTotalTime(member.Nickname);
                if (string.IsNullOrEmpty(data[0]["Time"].ToString()))
                {
                    await ctx.Channel.SendMessageAsync(member.Mention + " has worked for `00:00:00` In Total");
                    return;
                }
                var workTime = TimeSpan.FromSeconds(int.Parse(data[0]["Time"].ToString())).ToString();
                logger.LogInformation(member.Nickname + " has worked for `" + workTime.Replace(".", " Day(s) - ") + "` in total");
                await ctx.Channel.SendMessageAsync(member.Mention + " has worked for `" + workTime.Replace(".", "d ") + "`  in total");
            }
            catch (Exception ex)
            {
                Main.Logger.LogError(ex.Message);
                await ctx.RespondAsync(ex.Message);
            }
        }

    }
}
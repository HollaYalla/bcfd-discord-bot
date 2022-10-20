using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Harmony.Module.Common;
using Harmony.Module.Libs;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Harmony.Module.Actions
{
    [SlashCommandGroup("Timesheets", "Staff Timesheets", false)]
    class StaffActions : ApplicationCommandModule
    {
        private static DatabaseActions da = new DatabaseActions();

        [SlashCommand("time", "Get time for this week", true)]
        public async Task GetTime(InteractionContext ctx, [Option("user", "User to get time for", false)] DiscordUser user = null)
        {
            var member = user == (DiscordUser)null ? ctx.Member : await ctx.Guild.GetMemberAsync(user.Id);
            await StaffCommon.GetUserTime(ctx, member);
        }

        [SlashCommand("thisweek", "Get this weeks Timesheets", true)]
        public async Task GetThisWeek(InteractionContext ctx)
        {
            await StaffCommon.GetThisWeek(ctx);
        }

        [SlashCommand("lastweek", "Get last weeks Timesheets", true)]
        public async Task GetLastWeek(InteractionContext ctx)
        {
            await StaffCommon.GetLastWeek(ctx);
        }

        [SlashCommand("totaltime", "Get Total Time For User", true)]
        public async Task GetTotalTime(InteractionContext ctx, [Option("user", "User to get time for", false)] DiscordUser user = null)
        {
            var member = user == (DiscordUser)null ? ctx.Member : await ctx.Guild.GetMemberAsync(user.Id);            
            await StaffCommon.GetTotalTime(ctx, member);        }

        [SlashCommand("forceclockout", "Force someone off duty", true)]
        public async Task ForceOffDuty(InteractionContext ctx, [Option("user", "User to force off duty", false)] DiscordUser user, [Option("reason", "Reason to force off duty", false)] string reason = "")
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var member = user == null ? await ctx.Guild.GetMemberAsync(user.Id) : ctx.Member;
            Main.Logger.LogInformation($"New Force Clock Out Request By {ctx.Member.Username}#{ctx.Member.Discriminator} in {ctx.Guild.Name} for {member.Nickname}");
            if (!ctx.Member.Roles.Contains<DiscordRole>(Options.ManagerRole))
            {
                DiscordMessage discordMessage1 = await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Oops, you don't have access to this command"));
            }
            StaffActions.da.ClockOutUser(member.Nickname);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{member.Mention} has been clocked out by {ctx.Member.Mention} with the reason: {reason}"));
            
        }
    }
}
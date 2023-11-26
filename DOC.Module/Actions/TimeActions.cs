using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DOC.Module.Common;
using DOC.Module.Libs;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DOC.Module.Actions
{
    using DOC.Module.Common;
    using System.Text.RegularExpressions;

    [SlashCommandGroup("Timesheets", "Staff Timesheets", true)]
    class TimeActions : ApplicationCommandModule
    {

        [SlashCommand("time", "Get time for this week", true)]
        public async Task GetTime(InteractionContext ctx, [Option("user", "User to get time for", false)] DiscordUser user = null, [Option("lastweek", "Get time for Last Week", true)] bool lastWeek = false)
        {
            var member = user == null ? ctx.Member.Nickname : (await ctx.Guild.GetMemberAsync(user.Id)).Nickname;
            var match = Regex.Match(member, @"\[\d+\]\s(.+)");

            if (match.Success)
            {
                member = match.Groups[1].Value;
            }

            await StaffCommon.GetUserTime(ctx, member, lastWeek);
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

    }
}
using CloudTheWolf.DSharpPlus.Scaffolding.Logging;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DOJ.Module.Common;
using DOJ.Module.Libs;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DOJ.Module.Actions
{
    [SlashCommandGroup("Timesheets", "Staff Timesheets", true)]
    class TimeActions : ApplicationCommandModule
    {

        [SlashCommand("time", "Get time for this week", true)]
        public async Task GetTime(InteractionContext ctx, [Option("user", "User to get time for", false)] string user = null)
        {
            var member = user ?? ctx.Member.Nickname;
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

    }
}
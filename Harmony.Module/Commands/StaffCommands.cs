using Azure.Identity;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Harmony.Module.Common;
using Harmony.Module.Libs;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using Logger = CloudTheWolf.DSharpPlus.Scaffolding.Logging.Logger;


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
            await ctx.RespondAsync("`.` Commands are no longer available, please use `/` Commands now. Ty");
        }
        
        [Command("thisWeek")]
        [Aliases(new string[] { "tw" })]
        [Description("Get times for all staff for current week")]
        public async Task ThisWeek(CommandContext ctx)
        {
            await ctx.RespondAsync("`.` Commands are no longer available, please use `/` Commands now. Ty");
        }

        [Command("lastWeek")]
        [Aliases(new string[] { "lw" })]
        [Description("Get all activity for Last Week")]
        public async Task LastWeek(CommandContext ctx)
        {
            await ctx.RespondAsync("`.` Commands are no longer available, please use `/` Commands now. Ty");
        }

        [Command("toaltime")]
        [Aliases("tt")]
        [Description("Get how much time you have worked on this week")]
        public async Task TotalTime(CommandContext ctx, DiscordMember member = null, [RemainingText] string extra = null)
        {
            await ctx.RespondAsync("`.` Commands are no longer available, please use `/` Commands now. Ty");
        }


        [Command("forceclockout")]
        [Aliases("fco")]
        [Description("Force Clock Out Mentioned User")]
        public async Task ForceOffDuty(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            await ctx.RespondAsync("`.` Commands are no longer available, please use `/` Commands now. Ty");
        }
    }
}
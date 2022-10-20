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
            member = member ?? ctx.Member;
            await StaffCommon.GetUserTime(ctx,member);
        }
        
        [Command("thisWeek")]
        [Aliases(new string[] { "tw" })]
        [Description("Get times for all staff for current week")]
        public async Task ThisWeek(CommandContext ctx)
        {
            Main.Logger.LogInformation($"New This Week Check Request By {ctx.Member.Username}#{ctx.Member.Discriminator} in {ctx.Guild.Name}");
            await StaffCommon.GetThisWeek(ctx);
            Main.Logger.LogInformation("Deleting Command Request");
            await ctx.Message.DeleteAsync();
        }

        [Command("lastWeek")]
        [Aliases(new string[] { "lw" })]
        [Description("Get all activity for Last Week")]
        public async Task LastWeek(CommandContext ctx)
        {
            Main.Logger.LogInformation($"New Last Week Check Request By {ctx.Member.Username}#{ctx.Member.Discriminator} in {ctx.Guild.Name}");

            Main.Logger.LogInformation("Deleting Command Request");
            await ctx.Message.DeleteAsync();
        }

        [Command("toaltime")]
        [Aliases("tt")]
        [Description("Get how much time you have worked on this week")]
        public async Task TotalTime(CommandContext ctx, DiscordMember member = null, [RemainingText] string extra = null)
        {
            StaffCommon.GetTotalTime(ctx, member);
        }


        [Command("forceclockout")]
        [Aliases("fco")]
        [Description("Force Clock Out Mentioned User")]
        public async Task ForceOffDuty(CommandContext ctx, DiscordMember member, [RemainingText] string reason = null)
        {
            Main.Logger.LogInformation($"New Force Clock Out Request By {ctx.Member.Username}#{ctx.Member.Discriminator} in {ctx.Guild.Name} for {member.Nickname}");
            if (!ctx.Member.Roles.Contains(Options.ManagerRole)) return;

            da.ClockOutUser(member.Nickname);

            await ctx.Channel.SendMessageAsync(
                $"{member.Mention} has been clocked out by {ctx.Member.Mention} with the reason: {reason}");
            await ctx.Message.DeleteAsync();
        }

        [Command("debugSql")]
        [RequireOwner()]
        [Description("Debug Time Sheet SQL Queries")]
        public async Task DebugSql(CommandContext ctx)
        {
            var message = da.DebugGetUserTime(ctx.Member.Nickname);
            await ctx.RespondAsync($"```sql\n{message}\n```");
        }
    }
}
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Aod.Module.Actions
{
    using System.Diagnostics.CodeAnalysis;

    internal class OnMessageCreation
    {
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted", Justification = "Reviewed. Suppression is OK here.")]
        internal static async Task AddVoteReactions(DiscordClient client, MessageCreateEventArgs args)
        {
            if (args.Channel != Options.VotesChannelHighTable && args.Channel != Options.VotesChannelLowTable && args.Channel != Options.EventsChannel) return;
            if (!args.Message.Content.ToString().ToLower().StartsWith("[vote]")) return;

            await args.Message.CreateReactionAsync(DiscordEmoji.FromName(client, ":white_check_mark:"));
            await args.Message.CreateReactionAsync(DiscordEmoji.FromName(client, ":x:"));
        }
    }
}

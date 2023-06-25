using System.Text;


namespace Aod.Module.Actions
{
    using System.Net.WebSockets;

    using DSharpPlus;
    using DSharpPlus.EventArgs;
    using DSharpPlus.Exceptions;

    internal class SocketErrors
    {
        public static async Task Closed(DiscordClient sender, SocketCloseEventArgs e)
        {
            Environment.Exit(500);
        }

        public static async Task Errored(DiscordClient sender, SocketErrorEventArgs e)
        {
            try
            {
                if(e.Handled) return;
                Environment.Exit(500);
            }
            catch (Exception ex)
            {
                
                Environment.Exit(500);
            }
        }

        public static async Task SocketErrored(DiscordClient sender, SocketErrorEventArgs e)
        {
            Environment.Exit(500);
        }
    }
}

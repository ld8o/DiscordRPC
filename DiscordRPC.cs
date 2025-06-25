// Coded With ❤️ By Hy4e


using System.Threading.Tasks;

namespace DiscordRichPresence
{
    public class DiscordRPC
    {
        private readonly DiscordPipeClient pipeClient;

        public bool IsConnected => pipeClient.IsConnected;

        public DiscordRPC()
        {
            pipeClient = new DiscordPipeClient();
        }

        public Task<bool> ConnectAsync(string clientId)
        {
            return pipeClient.ConnectAsync(clientId);
        }

        public void SetPresence(string details, string state, string largeImageKey, string largeImageText)
        {
            pipeClient.SendPresence(details, state, largeImageKey, largeImageText);
        }

        public void Disconnect()
        {
            pipeClient.Disconnect();
        }
    }
}

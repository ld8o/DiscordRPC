using System;
using System.IO.Pipes;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordRichPresence
{
    internal class DiscordPipeClient
    {
        private NamedPipeClientStream pipe;
        private string clientId;
        private bool connected = false;

        public bool IsConnected => connected;

        public async Task<bool> ConnectAsync(string clientId)
        {
            try
            {
                this.clientId = clientId;
                pipe = new NamedPipeClientStream(".", "discord-ipc-0", PipeDirection.InOut, PipeOptions.Asynchronous);
                await pipe.ConnectAsync(3000);

                var handshake = new
                {
                    v = 1,
                    client_id = clientId
                };

                SendFrame(0, JsonSerializer.Serialize(handshake));
                var (op, res) = ReadFrame();
                connected = true;
                return true;
            }
            catch
            {
                connected = false;
                return false;
            }
        }

        public void SendPresence(string details, string state, string largeImageKey, string largeImageText)
        {
            if (!connected) return;

            var presence = new
            {
                cmd = "SET_ACTIVITY",
                args = new
                {
                    pid = Environment.ProcessId,
                    activity = new
                    {
                        details,
                        state,
                        assets = new
                        {
                            large_image = largeImageKey,
                            large_text = largeImageText
                        },
                        timestamps = new
                        {
                            start = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        }
                    }
                },
                nonce = Guid.NewGuid().ToString()
            };

            SendFrame(1, JsonSerializer.Serialize(presence));
        }

        public void Disconnect()
        {
            connected = false;
            pipe?.Dispose();
        }

        private void SendFrame(int opCode, string json)
        {
            if (pipe == null || !pipe.IsConnected) return;

            byte[] payload = Encoding.UTF8.GetBytes(json);
            byte[] opBytes = BitConverter.GetBytes(opCode);
            byte[] lenBytes = BitConverter.GetBytes(payload.Length);

            pipe.Write(opBytes, 0, 4);
            pipe.Write(lenBytes, 0, 4);
            pipe.Write(payload, 0, payload.Length);
            pipe.Flush();
        }

        private (int op, string json) ReadFrame()
        {
            byte[] opBytes = new byte[4];
            byte[] lenBytes = new byte[4];
            pipe.Read(opBytes, 0, 4);
            pipe.Read(lenBytes, 0, 4);

            int op = BitConverter.ToInt32(opBytes, 0);
            int len = BitConverter.ToInt32(lenBytes, 0);

            byte[] payload = new byte[len];
            pipe.Read(payload, 0, len);

            string json = Encoding.UTF8.GetString(payload);
            return (op, json);
        }
    }
}


using System.Net.Sockets;

namespace ChatTalk.Server
{
    public class ClientHandler
    {
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        private readonly TCPServer _server;
        private readonly TcpClient _client;

        public string UserName { get; private set; } = "UnKnown";

        private string messageId = string.Empty;

        public ClientHandler(TcpClient client, TCPServer server)
        {
            _client = client;
            _server = server;

            var stream = _client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream) { AutoFlush = true };
        }

        public async Task SendAsync(string message)
        {
            await _writer.WriteLineAsync(message);
            Console.WriteLine($"{UserName}: {message}");
        }

        public async Task ReceiveAsync()
        {
            try
            {
                while (true)
                {
                    string? message = await _reader.ReadLineAsync();

                    if (message == null) break;

                    if (string.IsNullOrEmpty(message)) continue;

                    await this.HandleMessage(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Receive Error] {ex.Message}");
            }
            finally
            {
                _client.Close();
                Console.WriteLine($"[Client Disconnected] {UserName}");
            }
        }

        private async Task HandleMessage(string message)
        {
            Console.WriteLine($"[Receive MSG] PORT : {message}");

            var parts = message.Split("^||^");

            /*
             * 1. ID^||^userName
             *    [0]: "ID", [1]: userName 
             * 2. MSG^||^userName^||^messageId^||^message
             *    [0]: "MSG", [1]: userName, [2]: messageId, [3]: message
             */
            if (parts.Length >= 2)
            {
                switch (parts[0])
                {   
                    case "ID":
                        string userName = parts[1].Trim();
                        SetUserName(userName);
                        break;
                    case "MSG":
                        messageId = parts[2].Trim();
                        string msg = parts[3];
                        await SendMessageAsync(msg);
                        break;
                }
            }
        }

        private void SetUserName(string userName)
        {
            if (!ValidateUserName(userName)) return;
            this.UserName = userName;
        }

        private async Task SendMessageAsync(string message)
        {
            string fullMsg = $"{this.messageId}^||^{this.UserName}^||^{message}\n";
            await _server.BroadcastAsync(fullMsg);
        }

        private bool ValidateUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return false;
            // if(userName.Length > 20) return false; 글자수 체크(추후적용)
            if (userName.Contains("^||^")) return false;

            return true;
        }
    }
}

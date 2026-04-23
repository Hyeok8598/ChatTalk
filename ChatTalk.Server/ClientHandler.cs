using System.Net.Sockets;
using ChatTalk.Common.Protocol;

namespace ChatTalk.Server
{
    public class ClientHandler
    {
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        private readonly TCPServer _server;
        private readonly TcpClient _client;

        private bool _isRunning = true;
        private bool _isDisconnected = false;

        public string UserName { get; private set; } = "UnKnown";

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
                while (_isRunning)
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
                Disconnect();
                Console.WriteLine($"[Client Disconnected] {UserName}");
            }
        }

        private async Task HandleMessage(string message)
        {
            Console.WriteLine($"[Received MSG] : {message}");

            string msg, msgId, fromUsrNm, toUsrNm;

            ParsedMessage parsedMessage = MessageParser.Parse(message);
            switch (parsedMessage.Type)
            {   
                case "JOIN":
                    string userName = parsedMessage.Values[0];
                    SetUserName(userName);
                    _server.GetClientDictionary().TryAdd(UserName, this);
                    await SendClientListMessageAsync();
                    break;

                case "LEAVE":
                    _isRunning = false;
                    _server.GetClientDictionary().TryRemove(UserName, out _);
                    await SendClientListMessageAsync();
                    break;

                case "MSG":
                    msgId = parsedMessage.Values[1];
                    msg = parsedMessage.Values[2];
                    await SendMessageAsync(msgId, msg);
                    break;

                case "WHISPER":
                    fromUsrNm = parsedMessage.Values[0];
                    toUsrNm = parsedMessage.Values[1];
                    msg = parsedMessage.Values[2];
                    await SendWhisperMessageAsync(fromUsrNm, toUsrNm, message);
                    break;
            }
        }

        private void SetUserName(string userName)
        {
            if (!ValidateUserName(userName)) return;
            this.UserName = userName;
        }

        private async Task SendMessageAsync(string messageId, string message)
        {
            string fullMsg = MessageBuilder.CreateChatMessage(UserName, messageId, message);
            await _server.BroadcastAsync(fullMsg);
        }

        private async Task SendClientListMessageAsync()
        {
            string clientListMessage = MessageBuilder.CreateUsrListMessage(_server.GetClientDictionary().Keys);
            await _server.BroadcastAsync(clientListMessage);
        }

        private async Task SendWhisperMessageAsync(string fromUsrNm, string toUsrNm, string msg)
        {
            await _server.SendToClientAsync(toUsrNm, msg);
        }

        private bool ValidateUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return false;
            // if(userName.Length > 20) return false; 글자수 체크(추후적용)
            if (userName.Contains("^||^")) return false;

            return true;
        }

        private void Disconnect()
        {
            if (_isDisconnected) return;
            _isDisconnected = true;

            _reader?.Dispose();
            _writer?.Dispose();
            _client?.Close();
        }
    }
}

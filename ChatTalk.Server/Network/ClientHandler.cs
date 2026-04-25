using System.Net.Sockets;
using ChatTalk.Common.Protocol.Building;
using ChatTalk.Common.Protocol.Messages;
using ChatTalk.Common.Protocol.Parsing;

/* ===================================================================== *
 * ClientHandler
 * --------------------------------------------------------------------- *
 * 1. Fields
 * 2. Constructor
 * 3. Network (Send / Receive)
 * 4. Message Handling
 * 5. Message Sending
 * 6. User Defined Methods
 * ===================================================================== */

namespace ChatTalk.Server.Network
{
    public class ClientHandler
    {
        /* ===================================================================== *
         * 1. Fields
         * ===================================================================== */
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        private readonly TCPServer _server;
        private readonly TcpClient _client;

        private bool _isRunning = true;
        private bool _isDisconnected = false;

        public string UserName { get; private set; } = "UnKnown";

        /* ===================================================================== *
         * 2. Constructor
         * ===================================================================== */
        public ClientHandler(TcpClient client, TCPServer server)
        {
            _client = client;
            _server = server;

            var stream = _client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream) { AutoFlush = true };
        }

        /* ===================================================================== *
         * 3. Network (Send / Receive)
         * ===================================================================== */
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
                await DisconnectAsync();
                Console.WriteLine($"[Client Disconnected] {UserName}");
            }
        }

        /* ===================================================================== *
         * 4. Message Handling
         * ===================================================================== */
        private async Task HandleMessage(string message)
        {
            Console.WriteLine($"[Received MSG] : {message}");

            string msg, msgId, fromUsrNm, toUsrNm;

            ProtocolMessage protocolMessage = MessageParser.Parse(message);
            switch (protocolMessage)
            {   
                case JoinProtocolMessage join:
                    string userName = join.UserName;
                    SetUserName(userName);
                    _server.GetClientDictionary().TryAdd(UserName, this);
                    await SendJoinMessageAsync(join.UserName);
                    await SendClientListMessageAsync();
                    break;

                case LeaveProtocolMessage leave:
                    _isRunning = false;
                    await SendLeaveMessageAsync(leave.UserName);
                    await DisconnectAsync();
                    break;

                case ChatProtocolMessage chat:
                    msgId = chat.MessageId;
                    msg = chat.Content;
                    await SendMessageAsync(msgId, msg);
                    break;

                case WhisperProtocolMessage whisper:
                    fromUsrNm = whisper.FromUserName;
                    toUsrNm = whisper.ToUserName;
                    msg = whisper.Content;
                    await SendWhisperMessageAsync(fromUsrNm, toUsrNm, message);
                    break;
            }
        }

        /* ===================================================================== *
         * 5. Message Sending
         * ===================================================================== */
        private async Task SendMessageAsync(string messageId, string message)
        {
            string fullMsg = MessageBuilder.CreateChatMessage(UserName, messageId, message);
            await _server.BroadcastAsync(fullMsg);
        }

        private async Task SendJoinMessageAsync(string JoinUser)
        {
            string fullMsg = MessageBuilder.CreateJoinMessage(JoinUser);
            await _server.BroadcastAsync(fullMsg);
        }

        private async Task SendLeaveMessageAsync(string leftUser)
        {
            string fullMsg = MessageBuilder.CreateLeaveMessage(leftUser);
            await _server.BroadcastAsync(fullMsg);
        }

        private async Task SendClientListMessageAsync()
        {
            string clientListMessage = MessageBuilder.CreateUsrListMessage(_server.GetClientDictionary().Keys);
            await _server.BroadcastAsync(clientListMessage);
        }

        private async Task SendWhisperMessageAsync(string fromUsrNm, string toUsrNm, string msg)
        {
            if(!_server.GetClientDictionary().TryGetValue(toUsrNm, out ClientHandler? clientHandler))
            {
                Console.WriteLine($"[Whisper Failed] User not found: {toUsrNm}");
                return;
            }

            try
            {
                await _server.SendToClientAsync(toUsrNm, msg);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Whisper Failed] UserName: {toUsrNm}, Error:{ex.Message}");
                _server.GetClientDictionary().TryRemove(toUsrNm, out _);
            }
        }

        /* ===================================================================== *
         * 6. User Defined Methods
         * ===================================================================== */
        private bool ValidateUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return false;
            // if(userName.Length > 20) return false; 글자수 체크(추후적용)
            if (userName.Contains("^||^")) return false;

            return true;
        }

        private void SetUserName(string userName)
        {
            if (!ValidateUserName(userName)) return;
            this.UserName = userName;
        }

        private async Task DisconnectAsync()
        {
            if (_isDisconnected) return;
            _isDisconnected = true;
            _isRunning = false;

            _server.GetClientDictionary().TryRemove(UserName, out _);
            await SendClientListMessageAsync();

            _reader?.Dispose();
            _writer?.Dispose();
            _client?.Close();
        }
    }
}

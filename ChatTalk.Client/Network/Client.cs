using ChatTalk.Client.Model;
using ChatTalk.Common.Protocol.Building;
using ChatTalk.Common.Protocol.Messages;
using ChatTalk.Common.Protocol.Parsing;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;

/* ===================================================================== *
 * Client
 * --------------------------------------------------------------------- *
 * 1. Fields
 * 2. Constructor
 * 3. UI Event Handler
 * 4. User Defined Methods
 * ===================================================================== */

namespace ChatTalk.Client.Network
{
    public class Client
    {
        /* ===================================================================== *
         * 1. Fields
         * ===================================================================== */
        private TcpClient? _client;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private Stream? _stream;

        private readonly HashSet<string> _sentMessageIds = new HashSet<string>();
        private string _userName { get; set; } = string.Empty;
        public string UserName => _userName;

        public event Action<ChatMessage>? MessageReceived;
        public event Action<string[], string>? UserListReceived;
        public event Action<string>? UserStatusReceived;

        /* ===================================================================== *
         * 2. Constructor
         * ===================================================================== */

        /* ===================================================================== *
         * 3. UI Event Handler
         * ===================================================================== */
        public void HandleReceivedMessage(string receivedMessage)
        {
            ProtocolMessage protocalMessage = MessageParser.Parse(receivedMessage);

            switch (protocalMessage)
            {
                case ChatProtocolMessage chat:
                    if (_sentMessageIds.Contains(chat.MessageId)) return;

                    ChatMessage normalMsg = new ChatMessage
                    {
                        SenderName = chat.SenderName,
                        Content = chat.Content,
                        Type = MessageType.Normal,
                        Direction = MessageDirection.Received
                    };

                    MessageReceived?.Invoke(normalMsg);
                    break;

                case UsrListProtocolMessage usrList:
                    string[] users = usrList.UserListContent;
                    string userCnt = users.Length.ToString();
                    UserListReceived?.Invoke(users, userCnt);
                    break;

                case WhisperProtocolMessage whisper:

                    ChatMessage whisperMsg = new ChatMessage
                    {
                        SenderName = whisper.FromUserName,
                        Content = whisper.Content,
                        Type = MessageType.Whisper,
                        Direction = MessageDirection.Received
                    };

                    MessageReceived?.Invoke(whisperMsg);
                    break;

                case IUserStatusProtocolMessage status:
                    UserStatusReceived?.Invoke(status.StatusText);
                    break;
            }
        }

        /* ===================================================================== *
         * 4. User Defined Methods
         * ===================================================================== */
        public async Task ConnectAsync(string ip, int port)
        {
            _client = new TcpClient();

            Task connectTask  = _client.ConnectAsync(ip, port);
            Task timeoutTask  = Task.Delay(3000);
            Task completeTask = await Task.WhenAny(connectTask, timeoutTask);

            if (completeTask == timeoutTask)
            {
                throw new TimeoutException("서버에 연결할 수 없습니다.");
            }

            await connectTask;

            _stream = _client.GetStream();
            _reader = new StreamReader(_stream, Encoding.UTF8);
            _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };
        }

        public async Task SendJoinMsgAsync(string userName)
        {
            _userName = userName;
            string msg = MessageBuilder.CreateJoinMessage(_userName);
            await SendMsgAsync(msg);
        }

        public async Task SendChatMsgAsync(string message)
        {
            string messageId = Guid.NewGuid().ToString();
            _sentMessageIds.Add(messageId);
            string msg = MessageBuilder.CreateChatMessage(_userName, messageId, message);
            await SendMsgAsync(msg);
        }

        public async Task SendWhisperMsgAsync(string toUsrNm, string content)
        {
            string msg = MessageBuilder.CreateWhisperMessage(_userName, toUsrNm, content);
            await SendMsgAsync(msg);
        }

        public async Task SendLeaveMsgAsync()
        {
            string msg = MessageBuilder.CreateLeaveMessage(_userName);
            await SendMsgAsync(msg);
        }

        public async Task SendMsgAsync(string message)
        {
            if (_writer == null) return;
            if (string.IsNullOrWhiteSpace(message)) return;

            await _writer.WriteLineAsync(message);
        }

        public async Task ReceiveMsgAsync()
        {
            if (_client == null || _reader == null)
                return;

            try
            {
                while (_client.Connected)
                {
                    string? receivedMsg = await _reader.ReadLineAsync();

                    if (receivedMsg == null) break;
                    if (string.IsNullOrWhiteSpace(receivedMsg)) continue;

                    this.HandleReceivedMessage(receivedMsg);
                }
            }
            catch (ObjectDisposedException)
            {
                // 창 닫기, 연결 종료 (무시)
            }
            catch (IOException)
            {
                // 소켓 종료 발생 (정상종료)
            }
            catch (Exception ex)
            {
                MessageBox.Show($"수신 중 오류 발생: {ex.Message}");
            }
        }

        public async Task Disconnect()
        {
            await SendLeaveMsgAsync();

            /* 26.04.25 - Flush 시간 확보 */
            await Task.Delay(100);

            _reader?.Dispose();
            _writer?.Dispose();
            _stream?.Dispose();
            _client?.Close();

            _reader = null;
            _writer = null;
            _stream = null;
            _client = null;
        }
    }
}

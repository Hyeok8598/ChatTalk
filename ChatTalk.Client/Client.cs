using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Windows;
using ChatTalk.Client.Model;
using ChatTalk.Common.Protocol;

namespace ChatTalk.Client
{
    public class Client
    {
        private TcpClient? _client;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private Stream? _stream;

        private readonly HashSet<string> _sentMessageIds = new HashSet<string>();
        private string _userName { get; set; } = string.Empty;
        public string UserName => _userName;

        public event Action<ChatMessage>? MessageReceived;
        public event Action<string[], string>? UserListReceived;

        public bool IsConnected => _client != null && _client.Connected;

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
            byte[] buffer = new byte[1024];
            try
            {
                while (_client != null && _client.Connected)
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

            _reader?.Dispose();
            _writer?.Dispose();
            _stream?.Dispose();
            _client?.Close();

            _reader = null;
            _writer = null;
            _stream = null;
            _client = null;
        }

        public void HandleReceivedMessage(string parsingData) {
            ParsedMessage parsedMessage = MessageParser.Parse(parsingData);

            switch(parsedMessage.Type)
            {
                case "MSG"    :
                    string receivedMsgId = parsedMessage.Values[1];
                    if (_sentMessageIds.Contains(receivedMsgId)) return;
                    string userNm  = parsedMessage.Values[0];
                    string message = parsedMessage.Values[2];

                    ChatMessage normalMsg = new ChatMessage
                    {
                        SenderName = userNm,
                        Content = message,
                        Type = MessageType.Normal,
                        Direction = MessageDirection.Received
                    };

                    MessageReceived?.Invoke(normalMsg);
                    break;
                    
                case "USRLIST" :
                    string[] users = parsedMessage.Values[0].Split(",");
                    string userCnt = users.Length.ToString();
                    UserListReceived?.Invoke(users, userCnt);
                    break;

                case "WHISPER"  :
                    string fromUsr = parsedMessage.Values[0];
                    string msg     = parsedMessage.Values[2];

                    ChatMessage whisperMsg = new ChatMessage
                    {
                        SenderName = fromUsr,
                        Content = msg,
                        Type = MessageType.Whisper,
                        Direction = MessageDirection.Received
                    };
                    
                    MessageReceived?.Invoke(whisperMsg);
                    break;
            }
        }
    }
}

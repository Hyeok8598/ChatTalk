using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace ChatTalk.Client
{
    public class Client
    {
        private TcpClient? _client;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private Stream? _stream;

        private readonly HashSet<string> _sentMessageIds = new HashSet<string>();
        private string _userName = string.Empty;

        public event Action<string, string>? onMessageReceived;

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

        public async Task SendUserNameAsync(string userName)
        {
            _userName = userName;
            string parsingData = $"ID^||^{userName}\n";
            await SendAsync(parsingData);
        }

        public async Task SendChatAsync(string message)
        {
            string messageId = Guid.NewGuid().ToString();
            _sentMessageIds.Add(messageId);
            string parsingData = $"MSG^||^{_userName}^||^{messageId}^||^{message}\n";
            await SendAsync(parsingData);
        }

        public async Task SendAsync(string message)
        {
            if (_writer == null) return;
            if (string.IsNullOrWhiteSpace(message)) return;

            await _writer.WriteLineAsync(message);
        }

        public async Task ReceiveAsync()
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

        public void Disconnect()
        {
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
            string[]? parts = parsingData.Split("^||^");

            //if (parts.Length < 3) return;

            string receiveMsgId = parts[0];

            if (_sentMessageIds.Contains(receiveMsgId)) return;
            string userName     = parts[1];
            string message      = string.Join("^||^", parts.Skip(2));
            onMessageReceived?.Invoke(userName, message);
        }
    }
}

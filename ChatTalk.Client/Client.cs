using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace ChatTalk.Client
{
    public class Client
    {
        private TcpClient? _client;
        private NetworkStream? _stream;

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

            _stream = _client.GetStream();
        }

        public async Task SendAsync(string message)
        {
            if (_stream == null) return;

            byte[] data = Encoding.UTF8.GetBytes(message);
            await _stream.WriteAsync(data, 0, data.Length);
        }

        public async Task ReceiveAsync(Action<String> onMessageReceived)
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (_client != null && _client.Connected)
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer);

                    onMessageReceived?.Invoke(message);
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
            _stream?.Dispose();
            _client?.Dispose();

            _stream = null;
            _client = null;
        }
    }
}

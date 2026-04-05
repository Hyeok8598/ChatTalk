using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatTalk.Server
{
    public class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;

        public string UserName { get; private set; } = "UnKnown";

        public ClientHandler(TcpClient client)
        {
            var stream = client.GetStream();
            _reader = new StreamReader(stream);
            _writer = new StreamWriter(stream) { AutoFlush = true };
        }

        public async Task SendAsync(string message)
        {
            await _writer.WriteLineAsync(message);
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

                    this.HandleMessage(message);
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

        private void HandleMessage(string message)
        {
            var parts = message.Split("^||^");

            // ^||^ID^||^BLABLA
            // [0]: "", [1]: "ID", [2]: "BLABLA" 
            if (parts.Length >= 3)
            {
                switch (parts[1])
                {
                    case "ID":
                        string userName = parts[2].Trim();
                        SetUserName(userName);
                        break;
                    case "MSG":
                        string msg = parts[2];
                        break;
                }
            }
        }
        private void SetUserName(string userName)
        {
            string trimUserName = userName.Trim();
            if (!ValidateUserName(trimUserName)) return;

            this.UserName = trimUserName;
        }

        private void SendMessageAsync(string message)
        {
            string fullMsg = $"^||^{this.UserName}^||^{message}";
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

using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace ChatTalk.Server
{
	public class TCPServer
	{
		private readonly int _port;
		private TcpListener? _listener;
		protected static ConcurrentDictionary<string, ClientHandler> _clientDictionary = new();

        //public string UserName { get; private set; } = "Unknown";

		public TCPServer(int port)
		{
			_port = port;
		}

		public void Start()
		{
			this.StartListner();

			while (true)
			{
				TcpClient client = AcceptClient();

                ClientHandler clientHandler = new ClientHandler(client, this);

				_ = clientHandler.ReceiveAsync();
            }
		}

		public void StartListner()
		{
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Console.WriteLine($"[Server Start] PORT : {_port}");
        }

		public TcpClient AcceptClient()
		{
            TcpClient client = _listener.AcceptTcpClient();
            Console.WriteLine($"[Client Connect] {client.Client.RemoteEndPoint}");

			return client;
        }

		public async Task BroadcastAsync(string message)
		{
            Console.WriteLine($"[Broadcast Message] : {message}");
            foreach (var clientDictionary in _clientDictionary)
			{
                ClientHandler client = clientDictionary.Value;
                await client.SendAsync(message);
			}
		}

		public async Task SendToClientAsync(string toUsrNm, string message)
		{
            if (_clientDictionary.TryGetValue(toUsrNm, out ClientHandler? client))
            {
                await client.SendAsync(message);
            }
        }

        public ConcurrentDictionary<string, ClientHandler> GetClientDictionary()
		{
			return _clientDictionary;
		}
    }
}
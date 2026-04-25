using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

/* ===================================================================== *
 * TCPServer
 * --------------------------------------------------------------------- *
 * 1. Fields
 * 2. Constructor
 * 3. Server Lifecycle
 * 4. User Defined Methods
 * ===================================================================== */

/// <summary>
/// TCP 클라이언트 접속을 수락하고, 연결된 클라이언트에게 메시지를 전달하는 서버 클래스
/// </summary>
namespace ChatTalk.Server.Network
{
	public class TCPServer
	{
        /* ===================================================================== *
         * 1. Fields
         * ===================================================================== */
        private readonly int _port;
		private TcpListener? _listener;
		protected static ConcurrentDictionary<string, ClientHandler> _clientDictionary = new();

        /* ===================================================================== *
         * 2. Constructor
         * ===================================================================== */
        public TCPServer(int port)
		{
			_port = port;
		}

        /* ===================================================================== *
         * 3. Server Lifecycle
         * ===================================================================== */
        public void Start()
		{
			StartListner();

            if (_listener == null)
                throw new InvalidOperationException("Listener가 초기화되지 않았습니다.");

            while (true)
			{
				TcpClient client = _listener.AcceptTcpClient();
                Console.WriteLine($"[Client Connect] {client.Client.RemoteEndPoint}");
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

        /* ===================================================================== *
         * 4. User Defined Methods
         * ===================================================================== */
        public async Task BroadcastAsync(string message)
		{
            Console.WriteLine($"[Broadcast Message] : {message}");
			
			if (string.IsNullOrWhiteSpace(message)) return;
            foreach (var clientDictionary in _clientDictionary)
			{
				try
				{
					ClientHandler client = clientDictionary.Value;
					await client.SendAsync(message);
				}
				catch (Exception ex)
				{
                    Console.WriteLine($"[Broadcast Failed] : User Name:{clientDictionary.Key}, Error:{ex.Message}");
					_clientDictionary.TryRemove(clientDictionary.Key, out _);
                }
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
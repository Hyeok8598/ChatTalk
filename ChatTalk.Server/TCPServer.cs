using System;
using System.Net;
using System.Net.Sockets;

namespace ChatTalk.Server
{
	class TCPServer
	{
		private readonly int _port;
		private TcpListener _listener;
		private List<ClientHandler> _clientHandlers = new List<ClientHandler>();

        public string UserName { get; private set; } = "Unknown";

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
				ClientHandler clientHandler = new ClientHandler(client);
                _clientHandlers.Add(clientHandler);

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
    }
}
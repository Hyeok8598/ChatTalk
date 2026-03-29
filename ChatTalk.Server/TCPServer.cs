using System;
using System.Net;
using System.Net.Sockets;

namespace ChatTalk.Server
{
	class TCPServer
	{
		private readonly int _port;
		private TcpListener _listener;

		public TCPServer(int port)
		{
			_port = port;
		}

		public void Start()
		{
			_listener = new TcpListener(IPAddress.Any, _port);
			_listener.Start();
			Console.WriteLine($"[Server Start] PORT : {_port}");

			TcpClient client = _listener.AcceptTcpClient();
			Console.WriteLine($"[Client Connect] {client.Client.RemoteEndPoint}");

			NetworkStream stream = client.GetStream();
			StreamReader reader = new StreamReader(stream);
			StreamWriter writer = new StreamWriter(stream) { AutoFlush = true  };
			
			while (true)
			{
				String? message = reader.ReadLine();

				if(message == null) break;

				Console.WriteLine($"Server Receive  : {message}");
                Console.WriteLine($"Server Response : {message}");
            }
		}
	}
}
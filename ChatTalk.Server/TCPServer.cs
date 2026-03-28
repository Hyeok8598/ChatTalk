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

			while (true)
			{
				TcpClient client = _listener.AcceptTcpClient();
				Console.WriteLine($"[Client Connect] {client.Client.RemoteEndPoint}");
			}
		}
	}
}
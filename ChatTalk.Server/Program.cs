namespace ChatTalk.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            TCPServer server = new TCPServer(5000);
            server.Start();
        }
    }
}
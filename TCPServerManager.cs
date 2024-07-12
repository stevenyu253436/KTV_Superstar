namespace DualScreenDemo
{
    public static class TCPServerManager
    {
        public static void StartServer()
        {
            TCPServer server = new TCPServer();
            server.Start();
        }
    }
}
namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            string ipAddress = "192.168.0.168"; // 服务器的IP地址
            int port = 502; // 服务器的端口号

            TcpServer server = new TcpServer(ipAddress, port);
            server.Start();
        }
    }
}
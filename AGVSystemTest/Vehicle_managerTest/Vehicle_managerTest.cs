using AGVSystem.vehicle_manager;

namespace AGVSystemTest.vehicle_manager
{
    [TestClass]
    public class Vehicle_managerTest
    {
        [TestMethod]
        public void OpenServer()
        {
            string ipAddress = "192.168.0.27"; // 服务器的IP地址
            int port = 502; // 服务器的端口号

            // TcpServer server = new TcpServer(ipAddress, port);

            //Thread clientThread = new Thread(server.Start);
            //clientThread.Start();

            //// Console.ReadKey();
            //Console.WriteLine("server started ! ! ");
            TcpServer tcpServer = new TcpServer(ipAddress, port);
            tcpServer.Start();

        }
    }
}
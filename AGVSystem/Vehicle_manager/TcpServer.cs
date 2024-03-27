using System.Net.Sockets;
using System.Net;
using System.Text;

namespace AGVSystem.vehicle_manager
{
    public class AgvInfo
    {
        public int AgvId { get; set; }
        public string AgvName { get; set; }
        public string AgvType { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
    }



    class TcpServer
    {
        // TCP监听器
        private TcpListener listener;

        // 已连接的客户端列表
        private List<AgvInfo> connectedClients;


        // 构造函数，初始化服务器
        public TcpServer(string ipAddress, int port)
        {
            IPAddress localAddr = IPAddress.Parse(ipAddress);
            listener = new TcpListener(localAddr, port);
            connectedClients = new List<AgvInfo>();
        }

        // 启动服务器
        public void Start()
        {
            listener.Start();
            Console.WriteLine("Server started. Waiting for connections...");

            while (true)
            {
                // 接受客户端连接
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                // 获取客户端的IP地址和分配的ID
                string clientAddress = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                // int clientId = connectedClients.Count + 1;

                // 创建连接的客户端对象
                //AgvInfo connectedClient = new AgvInfo
                //{
                //    IpAddress = clientAddress,
                //};

                // 将连接的客户端添加到列表中
                // connectedClients.Add(connectedClient);

                // 为每个客户端连接启动一个新线程来处理
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();

            }
        }

        private void HandleClient(TcpClient client)
        {
            // TcpClient client = (TcpClient)state;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;
            AgvInfo agvinfo = new AgvInfo();

            // 如果客户端已断开连接或没有更多数据可读，则 bytesRead 将为 0，不进入循环
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received : {dataReceived}");

                string[] parts = dataReceived.Split(',');
                // string agvInfo = new AgvInfo();
                string ipAddress = parts[2];
                string attribute = parts[3];
                string value = parts[4];

                agvinfo = connectedClients.Find(agv => agv.IpAddress == ipAddress);
                if (agvinfo == null)
                {
                    agvinfo = new AgvInfo { IpAddress = ipAddress };
                    connectedClients.Add(agvinfo);
                }

                switch (attribute)
                {
                    case "AgvId":
                        agvinfo.AgvId = int.Parse(value);
                        break;
                    case "AgvName":
                        agvinfo.AgvName = value;
                        break;
                    case "AgvType":
                        agvinfo.AgvType = value;
                        break;
                    case "Port":
                        agvinfo.Port = int.Parse(value);
                        break;
                    default:
                        // 处理未知属性
                        break;
                }

                // agvinfo_ = agvinfo;
                // 显示
                //foreach (AgvInfo agvInfo in connectedClients)
                //{
                //    Console.WriteLine($"IP: {agvInfo.IpAddress}");
                //    Console.WriteLine($"   AGV ID: {agvInfo.AgvId}");
                //    Console.WriteLine($"   AGV Name: {agvInfo.AgvName}");
                //    Console.WriteLine($"   AGV Type: {agvInfo.AgvType}");
                //    Console.WriteLine($"   Port: {agvInfo.Port}");
                //    Console.WriteLine();
                //}


                // 复诵数据以确保收到（可以不复诵）
                // byte[] response = Encoding.ASCII.GetBytes("Received data: " + dataReceived);
                // stream.Write(response, 0, response.Length);
            }

            client.Close();
            Console.WriteLine($"Client disconnected.");

            // 在列表中移除断开连接的客户端
            connectedClients.Remove(agvinfo);
        }

        public void Stop()
        {
            listener.Stop();
        }
    }

    class GetTools
    {
        internal int AgvId;

        GetTools( int AgvId_ )
        {
            AgvId = AgvId_;
        }


    }


}
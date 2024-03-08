using System;
using System.Net;
using System.Net.Sockets;
namespace AGVSystem.Traffic
{
    public class AgvCommunication
    {
        public static void Agv()
        {
            string ipAddress = "192.168.233.1"; // Modbus TCP 服务器的 IP 地址
            int port = 502; // Modbus TCP 默认端口号

            using (TcpClient client = new TcpClient(ipAddress, port))
            {
                NetworkStream stream = client.GetStream();

                // Modbus TCP 请求数据，这里是一个读取保持寄存器的例子
                byte[] request = { 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x01, 0x03, 0x00, 0x00, 0x00, 0x01 };

                // 发送请求数据
                stream.Write(request, 0, request.Length);

                // 接收响应数据
                byte[] response = new byte[1024];
                int bytesRead = stream.Read(response, 0, response.Length);

                // 处理响应数据
                Console.WriteLine("Response: ");
                for (int i = 0; i < bytesRead; i++)
                {
                    Console.Write(response[i] + " ");
                }
                Console.WriteLine();
                stream.Close();
            }
        }
    }
}

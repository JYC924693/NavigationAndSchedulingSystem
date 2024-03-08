using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
namespace AGVSystem.Traffic
{
    public class ModbusTCPClient
    {
        private TcpClient client;
        private NetworkStream stream;

        /// <summary>
        /// 新建TCP对象
        /// </summary>
        /// <param name="ipAddress">ip地址</param>
        /// <param name="port">端口号</param>
        public ModbusTCPClient(string ipAddress, int port)
        {
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
        }
        public void Close()
        {
            stream.Close();
            client.Close();
        }
        /// <summary>
        /// 发送信息有反馈 发送/接收为字节数组
        /// </summary>
        /// <param name="request">请求数据</param>
        /// <returns></returns>
        public byte[] SendModbusRequest(byte[] request)
        {
            stream.Write(request, 0, request.Length);

            byte[] response = new byte[1024];
            int bytesRead = stream.Read(response, 0, response.Length);
            Console.WriteLine("Response: ");
            for (int i = 0; i < bytesRead; i++)
            {
                Console.Write(response[i] + " ");
            }
            Console.WriteLine();

            byte[] responseData = new byte[bytesRead];
            Array.Copy(response, responseData, bytesRead);

            return responseData;
        }
        /// <summary>
        /// 判断连接是否成功 发送/接收为bool类型
        /// </summary>
        /// <param name="connection">连接信号</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool IsConnectionSuccess(int connectionSignal)
        {
            byte[] request = BitConverter.GetBytes(connectionSignal);
            stream.Write(request, 0, request.Length);

            byte[] response = new byte[1024];
            int bytesRead = stream.Read(response, 0, response.Length);

            if (response == null || response.Length == 0)
            {
                throw new ArgumentException("The byte array must have at least one element.", nameof(response));
            }
            byte firstByte = response[0];
            bool value = (firstByte & 1) == 1;
            return value;
        }
        /// <summary>
        /// 通过请求信号相应获取数据
        /// </summary>
        /// <param name="requestSignal">请求信号</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public byte[] RequestDataFromRemote(int requestSignal)
        {
            byte[] request = BitConverter.GetBytes(requestSignal);
            stream.Write(request, 0, request.Length);

            byte[] response = new byte[1024];
            int bytesRead = stream.Read(response, 0, response.Length);

            if (response == null || response.Length == 0)
            {
                throw new ArgumentException("The byte array must have at least one element.", nameof(response));
            }

            byte[] responseData = new byte[bytesRead];
            Array.Copy(response, responseData, bytesRead);
            return responseData;
        }
        /// <summary>
        /// 发送数据给远端
        /// </summary>
        /// <param name="sendData">发送的数据</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool SendDataToRemote(byte[] sendData)
        {
            stream.Write(sendData, 0, sendData.Length);

            byte[] response = new byte[1024];
            int bytesRead = stream.Read(response, 0, response.Length);
            if (response == null || response.Length == 0)
            {
                throw new ArgumentException("The byte array must have at least one element.", nameof(response));
            }
            byte firstByte = response[0];
            bool value = (firstByte & 1) == 1;
            return value;
        }
        /// <summary>
        /// 将字节流数据第一位转化为bool值
        /// </summary>
        /// <param name="byteStream">字节流数组</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool byteToBool(byte[] byteStream)
        {
            if (byteStream == null || byteStream.Length == 0)
            {
                throw new ArgumentException("The byte array must have at least one element.", nameof(byteStream));
            }
            byte firstByte = byteStream[0];
            bool value = (firstByte & 1) == 1;
            return value;
        }

        
    }
}

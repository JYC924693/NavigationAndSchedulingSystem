using System.Security.Cryptography.X509Certificates;
using System;
using System.Timers;
using System.Threading;
using Timer = System.Threading.Timer;

namespace AGVSystem.Traffic
{
    public class AgvManagement
    {
        public static List<AgvInfo> onlineAgvs;
        public static List<CommunicationInfo> agvCommunication;
        private static DateTime currentTime;
        private static DateTime nextTime;
        private static Timer timer;

        public AgvManagement()
        {

        }

        /// <summary>
        /// 检查掉线和新上线AGV
        /// </summary>
        public void CheckDisconnectionAndNewLaunchedAgvs()
        {
            //1、每隔5s连接一次，检查是否离线
            currentTime = DateTime.Now;
            nextTime = currentTime.AddSeconds(5);
            //var agv = new ModbusTCPClient("192.168.233.1",502);
            //byte[] request = { 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x01, 0x03, 0x00, 0x00, 0x00, 0x01 };
            //var s = agv.SendModbusRequest(request);
            //agv.Close();
            int i = 0;
            while (i < 10000)
            {
                var agv = new ModbusTCPClient("192.168.233.1", 502);
                Thread.Sleep(10);
                agv.Close();
                //Thread.Sleep(2000);
                i++;
            }

            //2、打开端口监听 AGV上线主动连接调度 上线

        }

        /// <summary>
        /// 更新AGV信息
        /// </summary>
        private void UpdateAgvInfo(string IPAddress, int port)
        {
            //上线的所有AGV都需更新状态
            foreach (var single in onlineAgvs)
            {
                try
                {
                    var agvConnection = new ModbusTCPClient(single.IPAddress, single.Port);
                    var receiveAgvInfo = agvConnection.RequestDataFromRemote(2);
                    
                    agvConnection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Socket 异常: " + ex.Message);
                }
            }
            //1、连接AGV

            //2、发送请求更新数据
            //3、字节数组转化为对应变量
            //4、更新到AGV信息对象里

        }
        /// <summary>
        /// 加载上线AGV
        /// </summary>
        private void LoadingOnlineAgvs()
        {
            //1、从sql中加载所有AGV [ID编号：IPAdress]

            //2、遍历 与每个AGV尝试建立连接
            foreach (var single in agvCommunication)
            {
                try
                {
                    var agvConnection = new ModbusTCPClient(single.IPAddress, single.Port);
                    var connectSignal = agvConnection.IsConnectionSuccess(1);
                    if (connectSignal)
                    {
                        //3、连接成功 将AGV加入在线列表中
                        var agv = new AgvInfo(single.AgvID);
                        onlineAgvs.Add(agv);
                    }
                    agvConnection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Socket 异常: " + ex.Message);
                }
            }
        }
        /// <summary>
        /// 任务绑定AGV
        /// </summary>
        public void TaskBindingAgv()
        {

        }

        public bool AddAgv(int AgvID)
        {
            var agv = new AgvInfo(AgvID);
            onlineAgvs.Add(agv);
            return true;
        }

        public bool RemoveAgv(int AgvID)
        {
            foreach (var agv in onlineAgvs)
            {
                if (agv.AgvID == AgvID)
                {
                    onlineAgvs.Remove(agv);
                    break;
                }
            }
            return true;
        }

        public AgvInfo FindAgv(int AgvID)
        {
            var agvTemp = new AgvInfo(AgvID);
            foreach (var agv in onlineAgvs)
            {
                if (agv.AgvID == AgvID)
                {
                    agvTemp = agv;
                }
            }
            return agvTemp;
        }

        public bool ModifyAgv()
        {
            return true;
        }
    }

}

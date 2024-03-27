using HslCommunication.ModBus;
using HslCommunication;
using System.Runtime.InteropServices;

/// <summary>
/// 用于通过modbusTCP与汇川PLC进行通讯，实现连接，发送数据，断开连接的功能
/// </summary>
namespace modbus_inovance
{
    public class dotnet_plc_lib
    {
        /// <summary>
        /// 定义储存一个路径点信息的结构体
        /// codeID:    二维码编号
        /// moveMOD:   运动模式
        /// angle:     角度
        /// missionID: 任务编号
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PathPointStruct
        {
            public ushort codeID;
            public ushort moveMOD;
            public float angle;
            public ushort missionID;
        }

        //public class plc_setting
        //{
        //    public string ipAddr;
        //    public int port;
        //}

        // internal Path path = new Path();
       
        

        public class plc_connection : HslCommunication.ModBus.ModbusTcpNet
        {

            public plc_connection(string ipAddr, int port) : base(ipAddr, port) { }
            public int connect()
            {
                try
                {
                    this.ConnectServer();
                    Console.WriteLine("Connect successfully ! ! !");
                    return 1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connect failed ! ! !");
                    Console.WriteLine(ex.Message);
                    return 0;
                }
            }

            public List<PathPointStruct> converter(Path path)
            {
                List<PathPointStruct> pathPoints = new List<PathPointStruct>(path.IdSequence.Count);
                for (int i = 0; i < pathPoints.Count; i++)
                {
                    pathPoints[i].codeID = (ushort)path.IdSequence[i];
                    pathPoints[i].moveMOD = (ushort)path.MoveSequence[i];
                    pathPoints[i].angle = (float)path.AngleSequence[i];
                    pathPoints[i].missionID = (ushort)path.TaskSequence[i];
                }

                return pathPoints;
            }
            public void write(List<PathPointStruct> PathPoints,int targetAddr)
            {
                PathPointStruct structure = new PathPointStruct();

                for (int i = 0; i < PathPoints.Count; i++)
                {
                    structure.codeID = PathPoints[i].codeID;
                    structure.moveMOD = PathPoints[i].moveMOD;
                    structure.angle = PathPoints[i].angle;
                    structure.missionID = PathPoints[i].missionID;

                    byte[] res = BitConverter.GetBytes(structure.angle);
                    byte[] data = new byte[4];
                    data[0] = res[1];
                    data[1] = res[0];
                    data[2] = res[3];
                    data[3] = res[2];
                    // Console.WriteLine(data);
                    try
                    {

                        this.Write((targetAddr + i * 6).ToString(), data);   
                        this.Write((targetAddr + i * 6 + 2).ToString(), structure.codeID);
                        this.Write((targetAddr + i * 6 + 3).ToString(), structure.moveMOD);
                        this.Write((targetAddr + i * 6 + 4).ToString(), structure.missionID);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }
    }
}

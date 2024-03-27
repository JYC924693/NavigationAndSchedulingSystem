using System;
using System.Runtime.InteropServices;

namespace AGVSystem.vehicle_manager
{
    public class Path_data
    {
        public class Path
        {
            public List<int> IdSequence { get; set; }

            public List<AgvState> MoveSequence { get; set; }

            public List<double> AngleSequence { get; set; }

            public List<TaskMode> TaskSequence { get; set; }
        }

        public enum AgvState
        {
            Left = -30,
            Backwark = -1,
            Forward = 1,
            Right = 30,
        }

        public enum TaskMode
        {
            None = 0,
        }
    }
    public class ModbusClient
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
            public short moveMOD;
            public float angle;
            public ushort missionID;

            public PathPointStruct(ushort codeID, short moveMOD, float angle, ushort missionID)
            {
                this.codeID = codeID;
                this.moveMOD = moveMOD;
                this.angle = angle;
                this.missionID = missionID;
            }
        }

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

            public List<PathPointStruct> converter(Path_data.Path path)
            {
                List<PathPointStruct> pathPoints = new List<PathPointStruct>(path.IdSequence.Count);
                // pathPoints = path.IdSequence.Count;
                for (int i = 0; i < path.IdSequence.Count; i++)
                {
                    //pathPoints[i].codeID = (ushort)path.IdSequence[i];
                    //pathPoints[i].moveMOD = (ushort)path.MoveSequence[i];
                    //pathPoints[i].angle = (float)path.AngleSequence[i];
                    //pathPoints[i].missionID = (ushort)path.TaskSequence[i];
                    pathPoints.Add(new PathPointStruct((ushort)path.IdSequence[i], (short)path.MoveSequence[i], (float)path.AngleSequence[i], (ushort)path.TaskSequence[i]));
                }

                return pathPoints;
            }
            public void write(List<PathPointStruct> PathPoints, int targetAddr)
            {
                PathPointStruct structure = new PathPointStruct(0, 0, 0, 0);

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

        void StartClient(string IpAddress, int Port, Path_data.Path path, int RegisterAddress)
        {
            plc_connection connection = new plc_connection(IpAddress, Port);
            int Check = connection.connect();
            if (Check == 0) 
            {
                return;
            }

            List<PathPointStruct> PathPointList = new List<PathPointStruct>();
            PathPointList = connection.converter(path);
            connection.write(PathPointList, RegisterAddress);
        }
    }
}

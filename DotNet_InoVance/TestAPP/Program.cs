using HslCommunication.ModBus;
using HslCommunication;
using static modbus_inovance.dotnet_plc_lib;
using modbus_inovance;
using System.ComponentModel;

namespace PLCcommunication
{
    
    class Program
    {
        //modbus_inovance.Path path = new modbus_inovance.Path();

        //modbus_inovance.Path MakeData_0()
        //{
        //    path.IdSequence = new List<int> { 2314 , 2315 , 2316 , 2317 };
        //    path.MoveSequence = new List<AgvState> { AgvState.Forward , AgvState.Left, AgvState.Right , AgvState.Backwark };
        //    path.AngleSequence = new List<double> { 120.2151f, 5.14159f, 3.14159f, 359.89214f };
        //    path.TaskSequence = new List<TaskMode> { TaskMode.None, TaskMode.None, TaskMode.None, TaskMode.None };

        //    return path;
        //}

        // private ModbusTcpNet client = new ModbusTcpNet("192.168.1.88");
        static void Main(string[] args)
        {
            // dotnet_plc_lib connect = new dotnet_plc_lib();
            string ipAddr = "192.168.0.29";
            int port = 502;

            plc_connection connect = new plc_connection(ipAddr,port);

            int Check = connect.connect();
            if (Check == 0)
            {
                return;
            }

            // -----------------------------------------------------------------------------------
            /**
             * 制作一个用于测试的path
             */
            modbus_inovance.Path path = new modbus_inovance.Path();

            path.IdSequence = new List<int> { 2314, 2315, 2316, 2317 };
            path.MoveSequence = new List<AgvState> { AgvState.Forward, AgvState.Left, AgvState.Right, AgvState.Backwark };
            path.AngleSequence = new List<double> { 120.2151f, 5.14159f, 3.14159f, 359.89214f };
            path.TaskSequence = new List<TaskMode> { TaskMode.None, TaskMode.None, TaskMode.None, TaskMode.None };
            // -----------------------------------------------------------------------------------



            //PathPointStruct pathStruct0 = new PathPointStruct();
            //pathStruct0.angle = 120.2151f;
            //pathStruct0.codeID = 2314;
            //pathStruct0.moveMOD = 1;
            //pathStruct0.missionID = 12;

            //PathPointStruct pathStruct1 = new PathPointStruct();
            //pathStruct1.codeID = 2315;
            //pathStruct1.moveMOD = 2;
            //pathStruct1.angle = 5.14159F;
            //pathStruct1.missionID = 13;

            //PathPointStruct pathStruct2 = new PathPointStruct();
            //pathStruct2.codeID = 2316;
            //pathStruct2.moveMOD = 3;
            //pathStruct2.angle = 7.14159F;
            //pathStruct2.missionID = 14;
            // var pg = new Program();
            // modbus_inovance.Path PathData = pg.MakeData_0();

            // List<PathPointStruct> PathPointList = new List<PathPointStruct> { pathStruct0, pathStruct1, pathStruct2 };
            List<PathPointStruct> PathPointList = new List<PathPointStruct>();
            PathPointList = connect.converter(path);
            connect.write(PathPointList, 0000);



            //ModbusTcpNet client = new ModbusTcpNet("192.168.1.88");
            //string targetAddr = "2850";
            //ushort data = 65535;


            //try
            //{
            //    client.ConnectServer();
            //    Console.WriteLine("Connect successfully ! ! !");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Connect failed ! ! !");
            //    Console.WriteLine(ex.Message);
            //}
            //try
            //{
            //    client.Write(targetAddr, data);
            //    Console.WriteLine("write successfully ! ! !");
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine("write failed ! ! !");
            //    Console.WriteLine(ex.Message);
            //}

            //client.ConnectClose();
        }

    }
}
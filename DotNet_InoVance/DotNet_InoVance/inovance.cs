using System.Runtime.InteropServices;

namespace DotNet_InoVance
{
    public class inovance
    {
        #region //标准库
        public enum SoftElemType
        {
            ELEM_QX = 0,     //QX元件
            ELEM_MW = 1,     //MW元件
            ELEM_X = 2,      //X元件(对应QX200~QX300)
            ELEM_Y = 3,		 //Y元件(对应QX300~QX400)
        }

        [DllImport("StandardModbusApi.dll", EntryPoint = "Init_ETH_String", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Init_ETH_String(string sIpAddr, int nNetId = 0, int IpPort = 502);

        [DllImport("StandardModbusApi.dll", EntryPoint = "Exit_ETH", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Exit_ETH(int nNetId = 0);

        [DllImport("StandardModbusApi.dll", EntryPoint = "Am600_Write_Soft_Elem", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AM500_Write_Soft_Elem(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId = 0);

        [DllImport("StandardModbusApi.dll", EntryPoint = "Am600_Read_Soft_Elem", CallingConvention = CallingConvention.Cdecl)]
        public static extern int AM500_Read_Soft_Elem(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId = 0);
        public static bool AM500_Write_Soft_Elem_MB(SoftElemType eType, int nStarAddr, int nCount, byte[] pValue, int nNetld)
        {
            byte[] pReadValue = new byte[2];
            byte[] pWriteValue = new byte[2];
            int nRetRaed = AM500_Read_Soft_Elem(eType, nStarAddr, 1, pReadValue, nNetld);
            if (nRetRaed == 1)
            {
                if (nCount == 0)
                {
                    pWriteValue[0] = pValue[0];
                    pWriteValue[1] = pReadValue[1];
                    int nRetWrite = AM500_Write_Soft_Elem(eType, nStarAddr, 1, pWriteValue, nNetld);
                    if (nRetWrite == 1)
                    {
                        return true;
                    }
                }
                else
                {
                    pWriteValue[0] = pReadValue[0];
                    pWriteValue[1] = pValue[0];
                    int nRetWrite = AM500_Write_Soft_Elem(eType, nStarAddr, 1, pWriteValue, nNetld);
                    if (nRetWrite == 1)
                    {
                        return true;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        // 路径点中包含的数据类型
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PathPointStruct
        {
            public int codeID;
            public int moveMOD;
            public double angle;
            public int missionID;
        }

        public struct Adsplc
        {
            public string adsipAdrr;       //ads地址
            public int adsPort;            //ads端口号
            public uint StarAdrr;           //plc起始地址 
            public uint Datalenth;          //数据长度
        }

        // 创建对象 
        


        public class PlcConnect
        {
            Adsplc adsplc = new Adsplc();

            public bool plc_init(string ipAdrr, int adsport)
            {
                bool port = false;
                try
                {
                    port = Init_ETH_String(ipAdrr, 0, adsport);
                    Console.WriteLine("Connect successfully ! ! !");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Connect failed ! ! !");
                }
                return port;
            }

            public bool plc_cut()
            {
                bool port = false;
                try
                {
                    port = Exit_ETH(0);
                    Console.WriteLine("disconnect successfully ! ! !");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("disconnect failed ! ! !");
                }
                return port;
            }

            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AxisBasicControl_HMI
{
  public  class PlcadsStuct
   {
        //ads设备信息
        public struct Adsplc
        {
            public string adsipAdrr;       //ads地址
            public int adsPort;            //ads端口号
            public uint StarAdrr;           //plc起始地址 
            public uint Datalenth;          //数据长度
         }
   }
}

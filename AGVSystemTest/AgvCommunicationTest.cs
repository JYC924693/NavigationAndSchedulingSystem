using AGVSystem.Traffic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGVSystemTest
{
    [TestClass]
    public class AgvCommunicationTest
    {
        [TestMethod]
        public void Test1()
        {
            AgvCommunication.Agv();
        }

        [TestMethod]
        public void Test2()
        {
            var tcp = new AgvManagement();
            tcp.CheckDisconnectionAndNewLaunchedAgvs();
        }
    }
}

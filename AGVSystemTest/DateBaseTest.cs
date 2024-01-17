using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGVSystem.Models;

namespace AGVSystemTest
{
    [TestClass]
    public class DateBaseTest
    {
        private readonly string DBName = "agv_system";
        private readonly string UserName = "agv_system_reader";
        private readonly string Password = "6hJ2vT6%*63b$!^2j&of80PH37*hQN";

        [TestMethod]
        public void GetEdges()
        {
            var map = new MapConverter(DBName, UserName, Password);
            foreach (var edge in map.GetEdges())
            {
                Console.WriteLine(edge);
            }
        }

        [TestMethod]
        public void GetVertices()
        {
            var map = new MapConverter(DBName, UserName, Password);
            foreach (var vertex in map.GetVertices())
            {
                Console.WriteLine(vertex);
            }
        }
    }

}

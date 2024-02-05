using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGVSystem.Models;

namespace AGVSystemTest
{
    [TestClass]
    public class CoreModuleTest
    {
        private ConcreteMap _map;

        private const string Path = @"C:\Users\Lenovo\Documents\Code\C#\Project\NavigationAndSchedulingSystem\AGVSystemTest\CoreModuleTestData\CoreTestMap.txt";

        [TestInitialize]
        public void Initialize()
        {
            _map = MapFactory.CreateMap(Path);
        }

        [TestMethod]
        public void CoreTest()
        {
            // 生成路径
            Console.WriteLine($"Vertices: {_map.V}, Edges: {_map.E}");
            foreach (var edge in _map.Edges)
            {
                Console.WriteLine(edge);
            }
            Console.WriteLine();
            // 对地图进行增删改查

            // 对点进行增删改查
                // 添加
            _map.AddVertices([new Vertex(9), new Vertex(10)]);
                // 查 & 改
            _map.ChangeVertexId(2, 11);
                // 删
            _map.RemoveVertex(5);

            // 对边进行增删改查
                // 添加
            _map.AddEdges([new Edge(4, 8), new Edge(8, 9), new Edge(9, 10)]);
                // 查 & 改
            _map.ChangeEdgeId(6, 8, 6, 10);
                // 删
            _map.RemoveEdge(3, 4);

            Console.WriteLine($"Vertices: {_map.V}, Edges: {_map.E}");
            foreach (var edge in _map.Edges)
            {
                Console.WriteLine(edge);
            }
            // 生成路径
            

            // 下发给AGV

        }
    }
}

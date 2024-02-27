using AGVSystem.Models;
using AGVSystem.Path;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AGVSystemTest
{
    [TestClass]
    public class PathTest
    {
        public static ConcreteMap Map
        {
            get
            {
               var map = MapFactory.CreateMap("D:\\J_7Code\\self_code\\NavigationAndSchedulingSystemQBD\\AGVSystemTest\\graph.txt", MapFactory.Type.SparseGraph);
               return map;
            }
        }

        public static Dictionary<int, Tuple<int, int>> PathID
        {
            get
            {
                var p = new Dictionary<int, Tuple<int, int>>();
                p.Add(1,Tuple.Create(17,23));
                p.Add(2,Tuple.Create(17,20));
                p.Add(3,Tuple.Create(20,21));
                return p;
            }
        }

        public static Dictionary<int, List<PathPoint>> ExpectResult
        {
            get
            {

                //var r = new Dictionary<int, List<int>>();
                //var path1 = new List<int>() { 17, 19, 24, 23 };
                //r.Add(1, path1);
                //var path2 = new List<int>() { 17, 19, 24, 20 };
                //r.Add(2, path2);
                //var path3 = new List<int>() { 20, 24, 19, 17, 21 };
                //r.Add(3, path3);

                var r = new Dictionary<int, List<PathPoint>>();
                var path1 = new List<PathPoint>();
                PathPoint p1 = new PathPoint { codeID = 17, moveMOD = 1, angle = 0.0, missionID = 0, name = "17" };
                PathPoint p2 = new PathPoint { codeID = 19, moveMOD = 1, angle = 0.0, missionID = 0, name = "19" };
                PathPoint p3 = new PathPoint { codeID = 24, moveMOD = 1, angle = 0.0, missionID = 0, name = "24" };
                PathPoint p4 = new PathPoint { codeID = 23, moveMOD = 1, angle = 0.0, missionID = 0, name = "23" };
                path1.Add(p1);
                path1.Add(p2);
                path1.Add(p3);
                path1.Add(p4);
                r.Add(1, path1);

                var path2 = new List<PathPoint>();
                PathPoint p5 = new PathPoint { codeID = 17, moveMOD = 1, angle = 0.0, missionID = 0, name = "17" };
                PathPoint p6 = new PathPoint { codeID = 19, moveMOD = 1, angle = 0.0, missionID = 0, name = "19" };
                PathPoint p7 = new PathPoint { codeID = 24, moveMOD = 1, angle = 0.0, missionID = 0, name = "24" };
                PathPoint p8 = new PathPoint { codeID = 20, moveMOD = 1, angle = 0.0, missionID = 0, name = "20" };
                path2.Add(p5);
                path2.Add(p6);
                path2.Add(p7);
                path2.Add(p8);
                r.Add(2, path2);

                var path3 = new List<PathPoint>();
                PathPoint p9 = new PathPoint { codeID = 20, moveMOD = 1, angle = 0.0, missionID = 0, name = "20" };
                PathPoint p10 = new PathPoint { codeID = 24, moveMOD = 1, angle = 0.0, missionID = 0, name = "24" };
                PathPoint p11 = new PathPoint { codeID = 19, moveMOD = 1, angle = 0.0, missionID = 0, name = "19" };
                PathPoint p12 = new PathPoint { codeID = 17, moveMOD = 1, angle = 0.0, missionID = 0, name = "17" };
                PathPoint p13 = new PathPoint { codeID = 21, moveMOD = 1, angle = 0.0, missionID = 0, name = "21" };
                path3.Add(p9);
                path3.Add(p10);
                path3.Add(p11);
                path3.Add(p12);
                path3.Add(p13);
                r.Add(3, path3);
                return r;
            }
        }

        public static IEnumerable<object[]> AdditionData
        {
            get
            {
                return new[]
                { 
                    new object[] { Map, PathID, ExpectResult },
                    //new object[] { Map },
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(AdditionData))]
        //public void AddIntegers_FromDynamicDataTest(ConcreteMap map)
        public void AddIntegers_FromDynamicDataTest(ConcreteMap map,  Dictionary<int, Tuple<int, int>> pathID, Dictionary<int, List<PathPoint>> expected)
        {
            var target = new Floyd();
            MapContext.GetInstance().BuildMap(map.Vertices, map.Edges);

            var actual = target.GetAllPaths(pathID);

            foreach (var id in actual.Keys)
            {
                //foreach (var data in actual[id])
                for (int i = 0; i < actual[id].Count; i++)
                {
                    var data = actual[id][i];
                    Console.Write($"{data.codeID}\t");
                    Console.Write($"{data.moveMOD}\t");
                    Console.Write($"{data.angle}\t");
                    Console.Write($"{data.missionID}\t");
                    Console.Write($"{data.name}\t");
                    Console.Write("\n");
                    Assert.AreEqual(expected[id][i].codeID, data.codeID);
                }
                Console.Write("\n");

                //Assert.IsTrue(actual[id].SequenceEqual(expected[id]));
            }
        }

        [TestMethod]
        public void Test()
        {
            var a = new Dictionary<int, List<int>>();
            a[1] = new List<int>{1,2,3};
            a[2] = new List<int> { 2, 5, 6, 7 };
            var b = new Dictionary<int, List<int>>();
            b[1] = new List<int> { 1, 2, 3 };
            b[2] = new List<int> { 2, 5, 6, 7 };
            foreach (var id in a.Keys)
            {
                Assert.IsTrue(a[id].SequenceEqual(b[id]));
            }
        }

    }

    [TestClass]
    public class DijkstraTest
    {
        public static ConcreteMap Map
        {
            get
            {
                var map = MapFactory.CreateMap("D:\\J_7Code\\self_code\\NavigationAndSchedulingSystemQBD\\AGVSystemTest\\graph.txt", MapFactory.Type.SparseGraph);
                return map;
            }
        }

        public static int start
        {
            get
            {
                int s = 17;
                return s;
            }
        }
        public static int end
        {
            get
            {
                int e = 23;
                return e;
            }
        }

        public static List<PathPoint> ExpectResult
        {
            get
            {

                //var r = new Dictionary<int, List<int>>();
                //var path1 = new List<int>() { 17, 19, 24, 23 };
                //r.Add(1, path1);
                //var path2 = new List<int>() { 17, 19, 24, 20 };
                //r.Add(2, path2);
                //var path3 = new List<int>() { 20, 24, 19, 17, 21 };
                //r.Add(3, path3);

                
                var path1 = new List<PathPoint>();
                PathPoint p1 = new PathPoint { codeID = 17, moveMOD = 1, angle = 0.0, missionID = 0, name = "17" };
                PathPoint p2 = new PathPoint { codeID = 19, moveMOD = 1, angle = 0.0, missionID = 0, name = "19" };
                PathPoint p3 = new PathPoint { codeID = 24, moveMOD = 1, angle = 0.0, missionID = 0, name = "24" };
                PathPoint p4 = new PathPoint { codeID = 23, moveMOD = 1, angle = 0.0, missionID = 0, name = "23" };
                path1.Add(p1);
                path1.Add(p2);
                path1.Add(p3);
                path1.Add(p4);

                return path1;
            }
        }

        public static IEnumerable<object[]> AdditionData
        {
            get
            {
                return new[]
                {
                    new object[] { Map, start, end, ExpectResult },
                    //new object[] { Map },
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(AdditionData))]
        public void TestDijkstra(ConcreteMap map, int start, int end, List<PathPoint> expected)
        {
            MapContext.GetInstance().BuildMap(map.Vertices, map.Edges);
            var target = new Dijkstra();
            var actual = target.GetPath(start, end);

            //foreach (var data in actual[id])
            for (int i = 0; i < actual.Count; i++)
            {
                var data = actual[i];
                Console.Write($"{data.codeID}\t");
                Console.Write($"{data.moveMOD}\t");
                Console.Write($"{data.angle}\t");
                Console.Write($"{data.missionID}\t");
                Console.Write($"{data.name}\t");
                Console.Write("\n");
                Assert.AreEqual(expected[i].codeID, data.codeID);
                Assert.AreEqual(expected[i].moveMOD, data.moveMOD);
                Assert.AreEqual(expected[i].angle, data.angle);
                Assert.AreEqual(expected[i].missionID, data.missionID);
                Assert.AreEqual(expected[i].name, data.name);
            }
            Console.Write("\n");


        }


    }
}

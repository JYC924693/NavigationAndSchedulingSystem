using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGVSystem.Models;
using AGVSystem.Path;

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

        public static Dictionary<int, List<int>> ExpectResult
        {
            get
            {

                var r = new Dictionary<int, List<int>>();
                var path1 = new List<int>() { 17, 19, 24, 23 };
                r.Add(1, path1);
                var path2 = new List<int>() { 17, 19, 24, 20 };
                r.Add(2, path2);
                var path3 = new List<int>() { 20, 24, 19, 17, 21 };
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
        public void AddIntegers_FromDynamicDataTest(ConcreteMap map,  Dictionary<int, Tuple<int, int>> pathID, Dictionary<int, List<int>> expected)
        {
            var target = new Floyd();
            MapContext.GetInstance().BuildMap(map.Vertices, map.Edges);

            var actual = target.GetAllPaths(pathID);

            foreach (var id in actual.Keys)
            {
                foreach (var data in actual[id])
                {
                    Console.WriteLine(data);
                }
                Console.Write("\n");

                Assert.IsTrue(actual[id].SequenceEqual(expected[id]));
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
}

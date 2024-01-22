using AGVSystem.Models;

namespace AGVSystemTest
{
    [TestClass]
    public class MapTest
    {
        [TestMethod]
        public void JudgeMapEqual()
        {
            var emptyMap = MapFactory.CreateMap(MapFactory.Type.EmptyGraph);
            var zeroMap = MapFactory.CreateMap(MapFactory.ZeroGraph1, MapFactory.Type.ZeroGraph);
            var nonTrivialMap = MapFactory.CreateMap(MapFactory.NontrivialGraph1, MapFactory.Type.NontrivialGraph);
            var sparseMap = MapFactory.CreateMap(MapFactory.SparseGraph1, MapFactory.Type.SparseGraph);

            var maps = new List<ConcreteMap> { emptyMap, zeroMap, nonTrivialMap, sparseMap };

            for (var i = 0; i < maps.Count; i++)
            {
                for (var j = 0; j < maps.Count; j++)
                {
                    if (i != j)
                    {
                        Assert.IsFalse(maps[i].AreIsomorphic(maps[j]), "创建或判断地图方法有误，两个相同的地图不应相等！");
                    }
                    else
                    {
                        Assert.IsTrue(maps[i].AreIsomorphic(maps[j]), "创建或判断地图方法有误，两个相同的地图并不相等！");
                    }
                }
            }
        }
        [TestMethod]
        public void GetVertexes()
        {
            var zeroMap = MapFactory.CreateMap(MapFactory.ZeroGraph1, MapFactory.Type.ZeroGraph);
            var SubNontrivialZero1_WithDifferentEdgesGraph = MapFactory.CreateMap(MapFactory.SubZeroGraph1, MapFactory.Type.ZeroGraph);

            foreach (var vertex in SubNontrivialZero1_WithDifferentEdgesGraph.Vertices)
            {
                Assert.IsTrue(vertex == zeroMap.GetVertex(vertex.ID), "获取顶点方法有误！");
            }

            var subZeroMap2 = MapFactory.CreateMap(MapFactory.ZeroGraph2, MapFactory.Type.ZeroGraph);
            foreach (var vertex in subZeroMap2.Vertices)
            {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                {
                    zeroMap.GetVertex(vertex.ID);
                }, "获取顶点方法有误！");
            }
        }
        [TestMethod]
        public void GetAdjacencyVertexes()
        {
            var zeroMap = MapFactory.CreateMap(MapFactory.ZeroGraph1, MapFactory.Type.ZeroGraph);
            var subZeroMap = MapFactory.CreateMap(MapFactory.SubZeroGraph1, MapFactory.Type.ZeroGraph);
            foreach (var vertex in subZeroMap.Vertices)
            {
                Assert.AreEqual(0, zeroMap.GetAdjacencyVertices(vertex).Count, "获取邻接点方法有误！");
            }

            var subZeroMap2 = MapFactory.CreateMap(MapFactory.SubZeroGraph2, MapFactory.Type.ZeroGraph);
            foreach (var vertex in subZeroMap2.Vertices)
            {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                {
                    zeroMap.GetAdjacencyVertices(vertex);
                }, "获取邻接点方法有误！");
            }

            var nontrivialMap = MapFactory.CreateMap(MapFactory.NontrivialGraph1, MapFactory.Type.NontrivialGraph);
            var subNontrivialMap = MapFactory.CreateMap(MapFactory.SubNontrivial1_ZeroGraph1, MapFactory.Type.ZeroGraph);
            var subNontrivialMap2 = MapFactory.CreateMap(MapFactory.SubNontrivial1_ZeroGraph2, MapFactory.Type.ZeroGraph);
            var resultMap = MapFactory.CreateMap(MapFactory.Type.EmptyGraph);
            foreach (var vertex2 in subNontrivialMap.Vertices.SelectMany(vertex => nontrivialMap.GetAdjacencyVertices(vertex)))
            {
                resultMap.AddVertex(vertex2);
            }

            Assert.IsTrue(subNontrivialMap2.AreIsomorphic(resultMap), "获取邻接点方法有误！");
        }
        [TestMethod]
        public void AddVertexes()
        {
            var zeroMap = MapFactory.CreateMap(MapFactory.ZeroGraph1, MapFactory.Type.ZeroGraph);
            var zeroMap2 = MapFactory.CreateMap(MapFactory.ZeroGraph2, MapFactory.Type.ZeroGraph);
            var zero1And2UnionMap = MapFactory.CreateMap(MapFactory.Zero1And2UnionGraph, MapFactory.Type.ZeroGraph);

            foreach (var vertex in zeroMap2.Vertices)
            {
                zeroMap.AddVertex(vertex);
            }

            Assert.IsTrue(zero1And2UnionMap.AreIsomorphic(zeroMap), "添加顶点方法有误！");

            var subZeroMap = MapFactory.CreateMap(MapFactory.SubZeroGraph1, MapFactory.Type.ZeroGraph);
            foreach (var vertex in subZeroMap.Vertices)
            {
                Assert.IsFalse(zeroMap.AddVertex(vertex), "添加顶点方法有误！");
            }
        }
        [TestMethod]
        public void InsertVertexes()
        {
            var sparseMap = MapFactory.CreateMap(MapFactory.SparseGraph1, MapFactory.Type.SparseGraph);
            var subSparseNontrivialMap =
                MapFactory.CreateMap(MapFactory.SubSparse1_NontrivialGraph1, MapFactory.Type.NontrivialGraph);
            var subZeroMap = MapFactory.CreateMap(MapFactory.SubZeroGraph1, MapFactory.Type.ZeroGraph);
            var resultMap = MapFactory.CreateMap(MapFactory.SparseAndSubZeroGraph1, MapFactory.Type.SparseGraph);


            List<Edge> edges;
            List<Vertex> vertexes;
            var testData = new List<((string, MapFactory.Type), (string, MapFactory.Type))>
            {
                ((MapFactory.SubSparse1_NontrivialGraph1, MapFactory.Type.NontrivialGraph),(MapFactory.SubSparse1_ZeroGraph1, MapFactory.Type.ZeroGraph)),
                ((MapFactory.SubSparse2_NontrivialGraph1, MapFactory.Type.NontrivialGraph),(MapFactory.SubZeroGraph1, MapFactory.Type.ZeroGraph)),
                ((MapFactory.SubSparse2_NontrivialGraph1, MapFactory.Type.NontrivialGraph),(MapFactory.SubSparse1_ZeroGraph1, MapFactory.Type.ZeroGraph)),
            };
            foreach (var (eItem, vItem) in testData)
            {
                edges = MapFactory.CreateMap(eItem.Item1, eItem.Item2).Edges;
                vertexes = MapFactory.CreateMap(vItem.Item1, vItem.Item2).Vertices;
                JudgeInsertVertexes(vertexes, edges);
            }

            edges = subSparseNontrivialMap.Edges;
            vertexes = subZeroMap.Vertices;
            for (var i = 0; i < vertexes.Count; i++)
            {
                sparseMap.InsertVertex(vertexes[i], edges[i]);
            }

            Assert.IsTrue(resultMap.AreIsomorphic(sparseMap), "插入顶点方法有误");

            return;

            void JudgeInsertVertexes(IReadOnlyList<Vertex> vertexes, IReadOnlyList<Edge> edges)
            {
                for (var i = 0; i < vertexes.Count; i++)
                {
                    Assert.IsFalse(sparseMap.InsertVertex(vertexes[i], edges[i]), "插入顶点方法有误");

                    //var r = sparseMap.InsertVertex(vertexes[i], edges[i]);
                    //if (r)
                    //{
                    //    Console.WriteLine($"v: {vertexes[i].ID}\te: {edges[i].From.ID} --- {edges[i].To.ID}");
                    //}
                    //Assert.IsFalse(r, "插入顶点方法有误");
                }
            }
        }
        [TestMethod]
        public void RemoveVertexes()
        {
            var sparseMap = MapFactory.CreateMap(MapFactory.SparseGraph1, MapFactory.Type.SparseGraph);
            var subSparseZeroMap = MapFactory.CreateMap(MapFactory.SubSparse1_ZeroGraph1, MapFactory.Type.ZeroGraph);
            var subSparseNontrivialMap =
                MapFactory.CreateMap(MapFactory.SubSparse1_NontrivialGraph1, MapFactory.Type.NontrivialGraph);
            var subZeroMap = MapFactory.CreateMap(MapFactory.SubZeroGraph2, MapFactory.Type.ZeroGraph);

            foreach (var vertex in subZeroMap.Vertices)
            {
                Assert.IsFalse(sparseMap.RemoveVertex(vertex), "移除顶点方法有误！");
            }

            foreach (var vertex in subSparseZeroMap.Vertices)
            {
                sparseMap.RemoveVertex(vertex);
            }
            Assert.IsTrue(subSparseNontrivialMap.AreIsomorphic(sparseMap), "移除顶点方法有误！");

            var emptyMap = MapFactory.CreateMap(MapFactory.Type.EmptyGraph);
            foreach (var vertex in subSparseNontrivialMap.Vertices)
            {
                sparseMap.RemoveVertex(vertex);
            }
            Assert.IsTrue(emptyMap.AreIsomorphic(sparseMap), "移除顶点方法有误！");
        }
        [TestMethod]
        public void GetAdjacencyEdges()
        {
            var zeroMap = MapFactory.CreateMap(MapFactory.ZeroGraph1, MapFactory.Type.ZeroGraph);
            var subZeroMap = MapFactory.CreateMap(MapFactory.SubZeroGraph1, MapFactory.Type.ZeroGraph);

            foreach (var vertex in subZeroMap.Vertices)
            {
                Assert.AreEqual(0, zeroMap.GetAdjacencyVertices(vertex.ID).Count, "获取邻接点方法有误！");
            }

            var subZeroMap2 = MapFactory.CreateMap(MapFactory.SubZeroGraph2, MapFactory.Type.ZeroGraph);
            foreach (var vertex in subZeroMap2.Vertices)
            {
                Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                {
                    zeroMap.GetAdjacencyEdges(vertex);
                }, "获取邻接点方法有误！");
            }
        }
        [TestMethod]
        public void AddEdges()
        {
            var sparseMap = MapFactory.CreateMap(MapFactory.SparseGraph1, MapFactory.Type.SparseGraph);
            var subSparseNontrivialMap =
                MapFactory.CreateMap(MapFactory.SubSparse1_NontrivialGraph1, MapFactory.Type.NontrivialGraph);

            foreach (var edge in subSparseNontrivialMap.Edges)
            {
                Assert.IsFalse(sparseMap.AddEdge(edge), "添加边方法有误！");
            }

            var subSparseNontrivialMap2 = MapFactory.CreateMap(MapFactory.SubSparse2_NontrivialGraph1, MapFactory.Type.NontrivialGraph);
            foreach (var edge in subSparseNontrivialMap2.Edges)
            {
                Assert.IsFalse(sparseMap.AddEdge(edge), "添加边方法有误！");
            }

            var nontrivialMap = MapFactory.CreateMap(MapFactory.PartSparseVertexesAndDifferentEdgeGraph,
                MapFactory.Type.NontrivialGraph);
            foreach (var edge in nontrivialMap.Edges)
            {
                Assert.IsTrue(sparseMap.AddEdge(edge), "添加边方法有误！");
            }
        }
        [TestMethod]
        public void DeleteEdges()
        {
            var sparseMap = MapFactory.CreateMap(MapFactory.SparseGraph1, MapFactory.Type.SparseGraph);
            var subSparseNontrivialMap =
                MapFactory.CreateMap(MapFactory.SubSparse2_NontrivialGraph1, MapFactory.Type.NontrivialGraph);

            foreach (var edge in subSparseNontrivialMap.Edges)
            {
                Assert.IsFalse(sparseMap.RemoveEdge(edge), "添加边方法有误！");
            }

            var nontrivialVertexesMap = MapFactory.CreateMap(MapFactory.SubNontrivialZero1_WithDifferentEdgesGraph,
                MapFactory.Type.NontrivialGraph);
            foreach (var edge in nontrivialVertexesMap.Edges)
            {
                Assert.IsFalse(sparseMap.RemoveEdge(edge), "添加边方法有误！");
            }

            var subSparseNontrivialMap2 =
                MapFactory.CreateMap(MapFactory.SubSparse1_NontrivialGraph1, MapFactory.Type.NontrivialGraph);
            var subSparseZeroMap = MapFactory.CreateMap(MapFactory.Sparse1_ZeroGraph1, MapFactory.Type.ZeroGraph);
            foreach (var edge in subSparseNontrivialMap2.Edges)
            {
                sparseMap.RemoveEdge(edge);
            }
            Assert.IsTrue(subSparseZeroMap.AreIsomorphic(sparseMap), "添加边方法有误！");
        }
        [TestMethod]
        public void DeleteMap()
        {
            var emptyMap = MapFactory.CreateMap(MapFactory.Type.EmptyGraph);
            var nontrivialMap = MapFactory.CreateMap(MapFactory.NontrivialGraph1, MapFactory.Type.NontrivialGraph);
            var sparseMap = MapFactory.CreateMap(MapFactory.SparseGraph1, MapFactory.Type.SparseGraph);

            var maps = new List<ConcreteMap> { nontrivialMap, sparseMap }
            ;
            foreach (var map in maps)
            {
                map.Clear();
                Assert.AreEqual(emptyMap.E, map.E, "清除地图方法有误！");
                Assert.AreEqual(emptyMap.V, map.V, "清除地图方法有误！");
                Assert.IsTrue(emptyMap.AreIsomorphic(map), "清除地图方法有误！");
            }
        }

        //[TestMethod]
        //public void Temp()
        //{
        //    var map = MapFactory.CreateMap(@"C:\Users\Lenovo\Desktop\新建 文本文档.txt", MapFactory.Type.SparseGraph);

        //    Console.WriteLine("vertexes: ");
        //    foreach (var v in map.Vertices)
        //    {
        //        Console.WriteLine(v);
        //    }

        //    Console.WriteLine("\nedges: ");
        //    foreach (var e in map.Edges)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}
    }

    public class MapFactory
    {
        private readonly Graph _map = new();
        private static readonly Random Rand = new();
        private const int MaxRange = (int)1e9;

        public static readonly string ZeroGraph1 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\ZeroGraph1.txt";
        public static readonly string SubZeroGraph1 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\SubZeroGraph1.txt";

        public static readonly string ZeroGraph2 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\ZeroGraph2.txt";
        public static readonly string SubZeroGraph2 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\SubZeroGraph2.txt";

        public static readonly string Zero1And2UnionGraph = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\Zero1And2UnionGraph.txt";

        public static readonly string NontrivialGraph1 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\NontrivialGraph1.txt";
        public static readonly string SubNontrivial1_ZeroGraph1 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\SubNontrivial1_ZeroGraph1.txt";
        public static readonly string SubNontrivial1_ZeroGraph2 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\SubNontrivial1_ZeroGraph2.txt";

        public static readonly string NontrivialGraph2 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\NontrivialGraph2.txt";

        public static readonly string SubNontrivialZero1_WithDifferentEdgesGraph = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\SubNontrivialZero1_WithDifferentEdgesGraph.txt";

        public static readonly string SparseGraph1 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\SparseGraph1.txt";
        public static readonly string Sparse1_ZeroGraph1 =
            @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\Sparse1_ZeroGraph1.txt";
        public static readonly string SubSparse1_NontrivialGraph1 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\SubSparse1_NontrivialGraph1.txt";
        public static readonly string SubSparse1_ZeroGraph1 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\SubSparse1_ZeroGraph1.txt";

        public static readonly string SparseGraph2 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\SparseGraph2.txt";
        public static readonly string SubSparse2_NontrivialGraph1 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\SubSparse2_NontrivialGraph1.txt";
        public static readonly string SubSparse2_ZeroGraph1 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\SubSparse2_ZeroGraph1.txt";

        public static readonly string SparseAndSubZeroGraph1 = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\SparseAndSubZeroGraph1.txt";

        public static readonly string PartSparseVertexesAndDifferentEdgeGraph = @"C:\Users\Lenovo\Documents\Code\C#\Project\AGVSystemTest\MapTestData\PartSparseVertexesAndDifferentEdgeGraph.txt";

        public enum Type
        {
            EmptyGraph,
            ZeroGraph, // 仅由点构成的图
            NontrivialGraph,// 无孤立顶点
            TrivialGraph, // 仅由一个点构成的图
            SparseGraph, // 由带边的顶点和孤立点构成的图
        }

        //public MapFactory(Type mapType)
        //{

        //}

        //public MapFactory(string filePath)
        //{

        //}

        public ConcreteMap GetInstance()
        {
            return new Graph(_map);
        }

        public static ConcreteMap CreateMap(Type mapType = Type.ZeroGraph, int numOfVertexes = 5, int numOfEdges = 0)
        {
            ConcreteMap map;

            switch (mapType)
            {
                case Type.ZeroGraph:
                    var vertexes = new List<Vertex>(numOfVertexes);
                    for (var i = 0; i < numOfVertexes; i++)
                    {
                        vertexes[i] = new Vertex(Rand.Next(MaxRange));
                    }
                    map = new Graph(vertexes);
                    break;
                case Type.TrivialGraph:
                    map = new Graph([new Vertex(Rand.Next(MaxRange))]);
                    break;
                case Type.NontrivialGraph:
                case Type.SparseGraph:
                    throw new NotImplementedException();
                case Type.EmptyGraph:
                default:
                    map = new Graph();
                    break;
            }

            return map;
        }

        public static ConcreteMap CreateMap(string filePath, Type mapType = Type.NontrivialGraph)
        {
            ConcreteMap map;

            switch (mapType)
            {
                case Type.TrivialGraph:
                case Type.ZeroGraph:
                    var vertexes = ReadVertexesFromFile(filePath);
                    map = new Graph(vertexes);
                    break;
                case Type.NontrivialGraph:
                    var edges = ReadEdgesFromFile(filePath);
                    map = new Graph(edges);
                    break;
                case Type.SparseGraph:
                    map = ReadMapFromFile(filePath);
                    break;
                case Type.EmptyGraph:
                default:
                    map = new Graph();
                    break;
            }

            return map;
        }

        private static List<Vertex> ReadVertexesFromFile(string filePath)
        {
            var vertexes = new List<Vertex>();
            var sr = new StreamReader(filePath);
            while (sr.ReadLine() is { } strVertexId)
            {
                var intVertexId = int.Parse(strVertexId);
                vertexes.Add(new Vertex(intVertexId));
            }

            return vertexes;
        }

        private static List<Edge> ReadEdgesFromFile(string filePath)
        {
            var edges = new List<Edge>();
            var sr = new StreamReader(filePath);
            while (sr.ReadLine() is { } line)
            {
                var parts = line.Split(' ');
                var startVertexId = int.Parse(parts[0]);
                var endVertexId = int.Parse(parts[1]);

                edges.Add(new Edge(startVertexId, endVertexId));
            }

            return edges;
        }

        private static ConcreteMap ReadMapFromFile(string filePath)
        {
            var map = new Graph();
            var sr = new StreamReader(filePath);

            var parts = sr.ReadLine().Split(' ');
            var numOfVertexes = int.Parse(parts[0]);
            var numOfEdges = int.Parse(parts[1]);
            var vertexes = new List<Vertex>(numOfVertexes);
            var edges = new List<Edge>(numOfEdges);

            while (sr.ReadLine() is { } line)
            {
                if (numOfVertexes > 0)
                {
                    vertexes.Add(new Vertex(int.Parse(line)));
                    numOfVertexes--;
                }
                else
                {
                    parts = line.Split(' ');
                    var startVertexId = int.Parse(parts[0]);
                    var endVertexId = int.Parse(parts[1]);
                    edges.Add(new Edge(startVertexId, endVertexId));
                }
            }

            map.BuildMap(vertexes, edges);

            return map;
        }
    }
}
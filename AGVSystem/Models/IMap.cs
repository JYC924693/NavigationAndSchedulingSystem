namespace AGVSystem.Models
{
    public static class MapContext
    {
        private static readonly ConcreteMap Map = new Graph();

        public static ConcreteMap GetInstance()
        {
            return Map;
        }
    }

    public interface IMap
    {
        public void BuildMap();
        public Vertex GetVertex(int id);
        public List<Vertex> GetAdjacencyVertices(int id);
        public bool AddVertex(int id);
        public bool InsertVertex(int id);
        public bool RemoveVertex(int id);
        public List<Edge> GetAdjacencyEdges(int id);
        public bool AddEdge(Edge v);
        public bool DeleteEdge(Edge v);
        public void Clear();
    }

    public abstract class ConcreteMap
    {
        public abstract int V { get; }
        public abstract int E { get; }

        public abstract List<Vertex> Vertices { get; }
        public abstract List<Edge> Edges { get; }

        public abstract bool AreIsomorphic(ConcreteMap map);
        public abstract void BuildMap(List<Vertex> vertices, List<Edge> edges);
        public virtual void BuildMap(List<Vertex> vertices) => BuildMap(vertices, []);
        public abstract void BuildMap(List<Edge> edges);
        public abstract Vertex GetVertex(int id);
        public abstract List<Vertex> GetAdjacencyVertices(int id);
        public virtual List<Vertex> GetAdjacencyVertices(Vertex vertex) => GetAdjacencyVertices(vertex.ID);
        public virtual bool AddVertex(int id) => AddVertex(new Vertex(id));
        public abstract bool AddVertex(Vertex vertex);
        public virtual List<Vertex> AddVertexVertices(List<Vertex> vertexes) => vertexes.Where(vertex => !AddVertex(vertex)).ToList();
        public virtual List<Vertex> AddVertexVertices(List<int> vertexes) => vertexes.Where(vertexId => !AddVertex(vertexId)).Select(vertexId => new Vertex(vertexId)).ToList();
        public virtual bool InsertVertex(int id, Edge edge) => InsertVertex(new Vertex(id), edge);
        public abstract bool InsertVertex(Vertex vertex, Edge edge);
        public abstract bool RemoveVertex(int id);
        public virtual bool RemoveVertex(Vertex vertex) => RemoveVertex(vertex.ID);
        public abstract Edge GetEdge(int from, int to);
        public abstract List<Edge> GetAdjacencyEdges(int id);
        public virtual List<Edge> GetAdjacencyEdges(Vertex vertex) => GetAdjacencyEdges(vertex.ID);
        public abstract bool AddEdge(Edge v);
        public virtual List<Edge> AddVertexEdges(List<Edge> edges) => edges.Where(edge => !AddEdge(edge)).ToList();
        public abstract bool DeleteEdge(Edge v);
        public abstract void Clear();
    }

    public class Graph : ConcreteMap
    {
        private readonly Dictionary<int, HashSet<int>> _adjacencyList = [];
        private readonly Dictionary<int, Vertex> _verticesDictionary = [];
        /// <summary>
        /// 边集有序存储，To的数值一定大于等于From
        /// </summary>
        private readonly Dictionary<(int From, int To), Edge> _edgesDictionary = [];


        public override int V => _verticesDictionary.Count;

        public override int E => _edgesDictionary.Count;

        public override List<Vertex> Vertices => _verticesDictionary.Select(item => new Vertex(item.Value)).ToList();

        public override List<Edge> Edges => _edgesDictionary.Values.ToList();

        public Graph() { }

        public Graph(Graph map)
        {
            _verticesDictionary = [];
            foreach (var (vertexId, vertex) in map._verticesDictionary)
            {
                _verticesDictionary[vertexId] = new Vertex(vertex);
            }

            _adjacencyList = [];
            foreach (var (vertexId, vertexes) in map._adjacencyList)
            {
                _adjacencyList[vertexId] = vertexes.Select(vId => vId).ToHashSet();
            }

            _edgesDictionary = [];
            foreach (var (edgeIndex, edge) in map._edgesDictionary)
            {
                _edgesDictionary[edgeIndex] = new Edge(edge);
            }
        }

        public Graph(List<Vertex> vertices) : this()
        {
            BuildMap(vertices);
        }

        public Graph(List<Vertex> vertices, List<Edge> edges) : this()
        {
            BuildMap(vertices, edges);
        }

        public Graph(List<Edge> edges) : this()
        {
            BuildMap(edges);
        }

        public static bool operator ==(Graph left, Graph right)
        {
            return left.AreIsomorphic(right);
        }

        public static bool operator !=(Graph left, Graph right)
        {
            return !(left == right);
        }

        public static Graph operator +(Graph left, Vertex right)
        {
            var map = new Graph(left);
            map.AddVertex(right);
            return map;
        }

        public static Graph operator +(Graph left, Edge right)
        {
            var map = new Graph(left);
            map.AddEdge(right);
            return map;
        }

        /// <summary>
        /// 当两个地图所有节点名和边相同即为同一地图
        /// </summary>
        /// <param name="concreteMap"></param>
        /// <returns></returns>
        public override bool AreIsomorphic(ConcreteMap concreteMap)
        {
            var map = concreteMap as Graph;
            var isEqual = true;

            // 判断顶点数和边数是否相等
            if (_verticesDictionary.Count != map?._verticesDictionary.Count ||
                _adjacencyList.Count != map._adjacencyList.Count) return !isEqual;

            // 获取图的度数序列
            var degrees1 = GetDegrees(_adjacencyList);
            var degrees2 = GetDegrees(map._adjacencyList);

            // 对度数序列进行排序
            degrees1.Sort();
            degrees2.Sort();

            // 比较排序后的度数序列
            isEqual = !degrees1.Where((t, i) => t != degrees2[i]).Any();

            //// 判断每个顶点ID是否相同
            //foreach (var item in _verticesDictionary)
            //{
            //    if (map._verticesDictionary.TryGetValue(item.Key, out var vertex))
            //    {
            //        if(item.Value == vertex) continue;
            //    }

            //    isEqual = false;
            //    break;
            //}

            return isEqual;
        }

        private static List<int> GetDegrees(Dictionary<int, HashSet<int>> graph)
        {
            return graph.Values.Select(neighbors => neighbors.Count).ToList();
        }

        public override void BuildMap(List<Vertex> vertices, List<Edge> edges)
        {
            Clear();

            foreach (var vertex in vertices)
            {
                _verticesDictionary[vertex.ID] = new Vertex(vertex.ID);
                _adjacencyList[vertex.ID] = [];
            }

            foreach (var edge in edges)
            {
                _adjacencyList[edge.From.ID].Add(edge.To.ID);
                _adjacencyList[edge.To.ID].Add(edge.From.ID);

                AddEdgeToDict(edge);
            }
        }

        private void AddEdgeToDict(Edge e)
        {
            if (e.From.ID < e.To.ID)
            {
                _edgesDictionary[(e.From.ID, e.To.ID)] = new Edge(e);
            }
            else
            {
                _edgesDictionary[(e.To.ID, e.From.ID)] = new Edge(e);
                _edgesDictionary[(e.To.ID, e.From.ID)].Reverse();
            }
        }

        public override void BuildMap(List<Edge> edges)
        {
            Clear();

            foreach (var edge in edges)
            {
                if (!_adjacencyList.TryAdd(edge.From.ID, [edge.To.ID]))
                {
                    _adjacencyList[edge.From.ID].Add(edge.To.ID);
                }

                if (!_adjacencyList.TryAdd(edge.To.ID, [edge.From.ID]))
                {
                    _adjacencyList[edge.To.ID].Add(edge.From.ID);
                }

                AddEdgeToDict(edge);
            }

            foreach (var vertexId in _adjacencyList.Keys)
            {
                _verticesDictionary[vertexId] = new Vertex(vertexId);
            }
        }

        public override Vertex GetVertex(int id)
        {
            if (!_verticesDictionary.TryGetValue(id, out var vertex))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return vertex;
        }

        public override List<Vertex> GetAdjacencyVertices(int id)
        {
            if (!_verticesDictionary.ContainsKey(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return _adjacencyList[id].Select(vertexId => _verticesDictionary[vertexId]).ToList();
        }

        public override bool AddVertex(Vertex vertex)
        {
            var isAdded = false;
            if (!_verticesDictionary.TryAdd(vertex.ID, vertex)) return isAdded;

            _adjacencyList[vertex.ID] = [];

            isAdded = true;

            return isAdded;
        }

        public override bool InsertVertex(Vertex vertex, Edge edge)
        {
            var isInserted = false;
            var hasVertex = _verticesDictionary.ContainsKey(vertex.ID);
            var hasEdge = JudgeTheExistenceOfEdge(edge);
            if ((hasVertex && hasEdge) | hasVertex | !hasEdge) return isInserted;

            _verticesDictionary[vertex.ID] = vertex;
            _adjacencyList[vertex.ID] = [edge.From.ID, edge.To.ID];

            ReplaceAdjacencyVertex(edge.From.ID, edge.To.ID, vertex.ID);
            var newEdge = new Edge(edge)
            {
                To =
                {
                    ID = vertex.ID
                }
            };
            AddEdgeToDict(newEdge);

            ReplaceAdjacencyVertex(edge.To.ID, edge.From.ID, vertex.ID);
            var newEdge2 = new Edge(edge)
            {
                From =
                {
                    ID = vertex.ID
                }
            };
            AddEdgeToDict(newEdge2);

            RemoveEdgeFromDict(edge);

            isInserted = true;

            return isInserted;
        }

        private void ReplaceAdjacencyVertex(int vertexId, int vertexToBeReplaced, int replacementVertex)
        {
            _adjacencyList[vertexId].Remove(vertexToBeReplaced);
            _adjacencyList[vertexId].Add(replacementVertex);
        }

        public override bool RemoveVertex(int id)
        {
            var isDel = false;
            if (!_verticesDictionary.ContainsKey(id)) return isDel;

            foreach (var vertexId in _adjacencyList[id])
            {
                _adjacencyList[vertexId].Remove(id);
                RemoveEdgeFromDict(id, vertexId);
            }

            _adjacencyList.Remove(id);
            _verticesDictionary.Remove(id);

            isDel = true;

            return isDel;
        }

        public override Edge GetEdge(int from, int to)
        {
            return _edgesDictionary[from < to  ? (from, to) : (to, from)];
        }

        public override List<Edge> GetAdjacencyEdges(int id)
        {
            if (!_verticesDictionary.ContainsKey(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return _adjacencyList[id].Select(to => _edgesDictionary[id < to ? (id,to) : (to, id)]).ToList();
        }

        private bool JudgeTheExistenceOfEdge(Edge e)
        {
            return _edgesDictionary.ContainsKey(e.From.ID < e.To.ID ? (e.From.ID, e.To.ID) : (e.To.ID, e.From.ID));
        }

        public override bool AddEdge(Edge e)
        {
            var isAdded = false;
            var hasEdge = JudgeTheExistenceOfEdge(e);
            var hasVertex = _verticesDictionary.ContainsKey(e.From.ID) && _verticesDictionary.ContainsKey(e.To.ID);
            if (!hasVertex || hasEdge)
                return isAdded;

            _adjacencyList[e.From.ID].Add(e.To.ID);
            _adjacencyList[e.To.ID].Add(e.From.ID);
            AddEdgeToDict(e);

            isAdded = true;

            return isAdded;
        }

        public override bool DeleteEdge(Edge e)
        {
            var isDel = false;

            if (!JudgeTheExistenceOfEdge(e)) return isDel;

            _adjacencyList[e.From.ID].Remove(e.To.ID);
            _adjacencyList[e.To.ID].Remove(e.From.ID);
            RemoveEdgeFromDict(e);

            isDel = true;

            return isDel;
        }

        private void RemoveEdgeFromDict(Edge e)
        {
            _edgesDictionary.Remove(e.From.ID < e.To.ID ? (e.From.ID, e.To.ID) : (e.To.ID, e.From.ID));
        }

        private void RemoveEdgeFromDict(int from, int to)
        {
            _edgesDictionary.Remove(from < to ? (from, to) : (to, from));
        }

        public override void Clear()
        {
            _verticesDictionary.Clear();
            _adjacencyList.Clear();
            _edgesDictionary.Clear();
        }
    }

    //public class Digraph : ConcreteMap
    //{

    //}


    //public class MapConverter(ConcreteMap map)
    //{
    //    private ConcreteMap _map = map;

    //}

    public class Vertex(int id)
    {
        public int ID { get; set; } = id;

        public Vertex(Vertex v) : this(v.ID)
        {

        }

        public static bool operator ==(Vertex v1, Vertex v2)
        {
            return v1.ID == v2.ID;
        }

        public static bool operator !=(Vertex v1, Vertex v2)
        {
            return !(v1 == v2);
        }
    }

    public class Edge
    {
        public Vertex From { get; set; }
        public Vertex To { get; set; }
        public double Weight { get; set; }

        public static bool operator ==(Edge e1, Edge e2)
        {
            return e1.From == e2.From && e1.To == e2.To && e1.Weight.Equals(e2.Weight);
        }

        public static bool operator !=(Edge e1, Edge e2)
        {
            return !(e1 == e2);
        }

        public Edge(int from, int to, double weight = 1d)
        {
            From = new Vertex(from);
            To = new Vertex(to);
            Weight = weight;
        }

        public Edge(Vertex from, Vertex to, double weight = 1d)
        {
            From = new Vertex(from);
            To = new Vertex(to);
            Weight = weight;
        }

        public Edge(Edge e)
        {
            From = new Vertex(e.From);
            To = new Vertex(e.To);
            Weight = e.Weight;
        }

        public void Reverse()
        {
            (From, To) = (To, From);
        }
    }

    public class UndirectedEdge : Edge
    {
        public UndirectedEdge(Vertex from, Vertex to, double weight) : base(from, to, weight)
        {
            PartialOrdering();
        }

        public UndirectedEdge(int from, int to, double weight) : base(from, to, weight)
        {
            PartialOrdering();
        }

        public UndirectedEdge(Edge e) : base(e)
        {
            PartialOrdering();
        }

        private void PartialOrdering()
        {
            if (From.ID > To.ID)
            {
                Reverse();
            }
        }
    }

    class QRCode
    {
        int ID { get; set; }
    }
}

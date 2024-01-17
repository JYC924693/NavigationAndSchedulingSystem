using static System.Net.WebRequestMethods;

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
        public int V { get; }
        public int E { get; }
        public List<Vertex> Vertices { get; }
        public List<Edge> Edges { get; }
        public void BuildMap();
        public Vertex GetVertex(int id);
        public List<Vertex> GetAdjacencyVertices(int id);
        public bool AddVertex(int id);
        public bool InsertVertex(int id);
        public bool RemoveVertex(int id);
        public List<Edge> GetAdjacencyEdges(int id);
        public bool AddEdge(Edge v);
        public bool RemoveEdge(Edge v);
        public void Clear();
    }

    public abstract class ConcreteMap
    {
        /// <summary>
        /// 图的顶点数
        /// </summary>
        public abstract int V { get; }
        /// <summary>
        /// 图的边数
        /// </summary>
        public abstract int E { get; }

        /// <summary>
        /// 图的顶点集
        /// </summary>
        public abstract List<Vertex> Vertices { get; }
        /// <summary>
        /// 图的边集
        /// </summary>
        public abstract List<Edge> Edges { get; }

        /// <summary>
        /// 判断两个地图是否同构
        /// </summary>
        /// <param name="map">需要对比的另一个地图</param>
        /// <returns>两个图的顶点数相等且每个顶点的入度和出度都相等，则图<see href="https://zh.wikipedia.org/wiki/%E5%9B%BE%E5%90%8C%E6%9E%84">同构</see>，返回<see langword="true"/>；否则返回<see langword="fasle"/></returns>
        public abstract bool AreIsomorphic(ConcreteMap map);
        /// <summary>
        /// 通过顶点和边构建一个图
        /// </summary>
        /// <param name="vertices">待构建图的顶点集合</param>
        /// <param name="edges">待构建图的边集合</param>
        public abstract void BuildMap(List<Vertex> vertices, List<Edge> edges);
        /// <summary>
        /// 通过顶点构建一个零图（无边的图）
        /// </summary>
        /// <param name="vertices">待构建图的顶点集合</param>
        public void BuildMap(List<Vertex> vertices) => BuildMap(vertices, []);
        /// <summary>
        /// 通过顶点构建一个连通图（两点都是连通的图）
        /// </summary>
        /// <param name="edges">待构建图的边集合</param>
        public abstract void BuildMap(List<Edge> edges);
        /// <summary>
        /// 获取指定ID的节点数据，点不存在则抛出异常<see cref="ArgumentOutOfRangeException"/>
        /// </summary>
        /// <param name="id">待获取节点的ID</param>
        /// <returns>对应ID的顶点</returns>
        /// <exception cref="ArgumentOutOfRangeException">当点不在图中时抛出异常</exception>
        public abstract Vertex GetVertex(int id);
        /// <param name="id">顶点ID</param>
        /// <inheritdoc cref="ContainVertex(Vertex)"/>
        public bool ContainVertex(int id) => ContainVertex(new Vertex(id));
        /// <summary>
        /// 判断此顶点是否在图中
        /// </summary>
        /// <param name="v">顶点</param>
        /// <returns>当顶点在图中，则返回<see langword="true"/>；否则返回<see langword="false"/></returns>
        public abstract bool ContainVertex(Vertex v);
        /// <param name="id">待获取邻接点集合的ID</param>
        /// <inheritdoc cref="GetAdjacencyVertices(Vertex)"/>
        public List<Vertex> GetAdjacencyVertices(int id) => GetAdjacencyVertices(new Vertex(id));
        /// <summary>
        /// 获取指定顶点的邻接点集合
        /// </summary>
        /// <param name="vertex">待获取邻接点集合的顶点</param>
        /// <returns>对应顶点的邻接点集合</returns>
        public abstract List<Vertex> GetAdjacencyVertices(Vertex vertex);
        /// <param name="id">顶点ID</param>
        /// <inheritdoc cref="AddVertex(Vertex)"/>
        public bool AddVertex(int id) => AddVertex(new Vertex(id));
        /// <summary>
        /// 添加顶点
        /// </summary>
        /// <param name="vertex">顶点</param>
        /// <returns>图中不存在该点则添加成功，返回<see langword="true"/>；否则返回<see langword="false"/></returns>
        public bool AddVertex(Vertex vertex) => AddVertices([vertex]).Count == 0;
        /// <summary>
        /// 添加顶点集合
        /// </summary>
        /// <param name="vertexes">顶点集合</param>
        /// <returns>图中不存在该点则添加成功，返回添加失败的点集</returns>
        public abstract List<Vertex> AddVertices(List<Vertex> vertexes);
        /// <param name="vertexes">顶点ID集合</param>
        /// <inheritdoc cref="AddVertices(System.Collections.Generic.List{AGVSystem.Models.Vertex})"/>
        public List<Vertex> AddVertices(List<int> vertexes) => AddVertices(vertexes.Select(id => new Vertex(id)).ToList());
        public bool InsertVertex(int id, Edge edge) => InsertVertex(new Vertex(id), edge);
        public abstract bool InsertVertex(Vertex vertex, Edge edge);
        /// <param name="id">待移除顶点ID</param>
        /// <inheritdoc cref="RemoveVertex(Vertex)"/>
        public bool RemoveVertex(int id) => RemoveVertex(new Vertex(id));
        /// <summary>
        /// 将该顶点及其相邻边移除
        /// </summary>
        /// <param name="vertex">待移除顶点</param>
        /// <returns>图中存在该点则移除成功，返回<see langword="true"/>；否则为<see langword="false"/></returns>
        public bool RemoveVertex(Vertex vertex) => RemoveVertices([vertex]).Count == 0;
        /// <summary>
        /// 移除顶点的集合及其相邻边的集合
        /// </summary>
        /// <param name="vertexes">待移除顶点集合</param>
        /// <returns>当图中不存在该点则该点移除失败，返回移除失败的点集</returns>
        public abstract List<Vertex> RemoveVertices(List<Vertex> vertexes);
        /// <param name="from">边的起始点ID</param>
        /// <param name="to">边的终点ID</param>
        /// <param name="edge">获取的边</param>
        /// <inheritdoc cref="TryGetEdge(in int, in int, in double, out Edge?)"/>
        public bool TryGetEdge(in int from, in int to, out Edge? edge) => TryGetEdge(from, to, 1, out edge);
        /// <summary>
        /// 尝试获取一条边，存在则将边赋值给参数<paramref name="edge"/>，不存在则<paramref name="edge"/>为<see langword="null"/>
        /// </summary>
        /// <param name="from">边的起始点ID</param>
        /// <param name="to">边的终点ID</param>
        /// <param name="weight">权重</param>
        /// <param name="edge">获取的边</param>
        /// <returns>存在边时返回<see langword="true"/>；否则返回<see langword="false"/></returns>
        public bool TryGetEdge(in int from, in int to, in double weight, out Edge? edge)
        {
            var isExist = ContainEdge(from, to, weight);
            edge = null;
            if (isExist)
            {
                edge = GetEdge(from, to);
            }

            return isExist;
        }
        /// <summary>
        /// 获取一条边
        /// </summary>
        /// <param name="from">边的起始点ID</param>
        /// <param name="to">边的终点ID</param>
        /// <param name="weight">边的权重， 默认为1</param>
        /// <returns>返回对应的边</returns>
        public abstract Edge GetEdge(int from, int to, double weight = 1);
        /// <param name="from">边的起始点ID</param>
        /// <param name="to">边的终点ID</param>
        /// <param name="weight">边的权重， 默认为1</param>
        /// <inheritdoc cref="ContainEdge(Edge)"/>
        public bool ContainEdge(int from, int to, double weight = 1) => ContainEdge(new Edge(from, to, weight));
        /// <summary>
        /// 判断一条边是否存在图里
        /// </summary>
        /// <param name="e">需要判断的边</param>
        /// <returns>图中存在边则返回<see langword="true"/>；否则返回<see langword="false"/></returns>
        public abstract bool ContainEdge(Edge e);
        /// <summary>
        /// 获取指定点的邻接边集
        /// </summary>
        /// <param name="id">指定点的ID</param>
        /// <returns>返回图中边含有该点的所有边</returns>
        public List<Edge> GetAdjacencyEdges(int id) => GetAdjacencyEdges(new Vertex(id));
        /// <summary>
        /// 获取指定点的邻接边集
        /// </summary>
        /// <param name="vertex">指定点</param>
        /// <returns>返回图中边含有该点的所有边</returns>
        public abstract List<Edge> GetAdjacencyEdges(Vertex vertex);
        /// <param name="from">边的起始点ID</param>
        /// <param name="to">边的终点ID</param>
        /// <param name="weight">边的权重， 默认为1</param>
        /// <inheritdoc cref="AddEdge(Edge)"/>
        public bool AddEdge(int from, int to, double weight = 1) => AddEdge(new Edge(from, to, weight));
        /// <summary>
        /// 向图中存在的点添加边
        /// </summary>
        /// <param name="e">待添加的边</param>
        /// <returns>图中存在边的两个点，不存在该边则添加成功，返回<see langword="true"/>；否则返回<see langword="false"/></returns>
        public bool AddEdge(Edge e) => AddEdges([e]).Count == 0;
        /// <summary>
        /// 向图中存在的点添加边集
        /// </summary>
        /// <param name="edges">待添加的边集</param>
        /// <returns>存在边的两个点，不存在该边则添加成功，返回的是未添加成功的边集</returns>
        public abstract List<Edge> AddEdges(List<Edge> edges);
        /// <param name="from">边的起始点ID</param>
        /// <param name="to">边的终点ID</param>
        /// <param name="weight">边的权重， 默认为1</param>
        /// <inheritdoc cref="RemoveEdge(Edge)"/>
        public bool RemoveEdge(int from, int to, double weight = 1) => RemoveEdge(new Edge(from, to, weight));
        /// <summary>
        /// 移除指定边
        /// </summary>
        /// <param name="e">待移除的边</param>
        /// <returns>边存在则删除成功，返回<see langword="true"/>；否则返回<see langword="false"/></returns>
        public bool RemoveEdge(Edge e) => RemoveEdges([e]).Count == 0;
        /// <summary>
        /// 移除指定的边集
        /// </summary>
        /// <param name="edges">待移除的边集</param>
        /// <returns>删除存在的边，返回的是未成功删除的边集</returns>
        public abstract List<Edge> RemoveEdges(List<Edge> edges);
        /// <summary>
        /// 清空图的节点和边
        /// </summary>
        public abstract void Clear();
        /// <summary>
        /// 获取指定节点的入度或出度
        /// </summary>
        /// <param name="id">节点ID</param>
        /// <param name="isInDegree">出入度，<see langword="true"/>为入度，<see langword="false"/>为出度</param>
        /// <returns>该节点的入度或出度</returns>
        public abstract int GetDegree(int id, bool isInDegree = true);
        /// <summary>
        /// 获取图中每个顶点的出入度
        /// </summary>
        /// <returns>每个顶点出入度的集合</returns>
        public abstract Dictionary<int, (int InDegree, int OutDegree)> GetAllDegrees();
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
            return !left.AreIsomorphic(right);
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

        public override bool AreIsomorphic(ConcreteMap concreteMap)
        {
            var map = concreteMap as Graph;
            var isEqual = true;

            // 判断顶点数和边数是否相等
            if (_verticesDictionary.Count != map?._verticesDictionary.Count ||
                _adjacencyList.Count != map._adjacencyList.Count) return !isEqual;

            // 获取图的度数序列
            var degrees1 = GetDegrees();
            var degrees2 = map.GetDegrees();

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

        private List<int> GetDegrees()
        {
            return _adjacencyList.Values.Select(adjacencices => adjacencices.Count).ToList();
        }

        public override int GetDegree(int id, bool isInDegree = true)
        {
            if (!_verticesDictionary.ContainsKey(id))
                throw new ArgumentOutOfRangeException(nameof(id));

            return _adjacencyList[id].Count;
        }

        public override Dictionary<int, (int InDegree, int OutDegree)> GetAllDegrees()
        {
            return _adjacencyList.Select(item => (item.Key ,(item.Value.Count, item.Value.Count))).ToDictionary();
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

        public override bool ContainVertex(Vertex v)
        {
            return _verticesDictionary.ContainsKey(v.ID);
        }

        public override List<Vertex> GetAdjacencyVertices(Vertex v)
        {
            if (!_verticesDictionary.ContainsKey(v.ID))
            {
                throw new ArgumentOutOfRangeException(nameof(v));
            }

            return _adjacencyList[v.ID].Select(vertexId => _verticesDictionary[vertexId]).ToList();
        }

        public override List<Vertex> AddVertices(List<Vertex> vertexes)
        {
            return vertexes.Where(vertex => !_verticesDictionary.TryAdd(vertex.ID, vertex)).ToList();
        }

        public override bool InsertVertex(Vertex vertex, Edge edge)
        {
            var isInserted = false;
            var hasVertex = _verticesDictionary.ContainsKey(vertex.ID);
            var hasEdge = ContainEdge(edge);
            if ((hasVertex && hasEdge) | hasVertex | !hasEdge) return isInserted;

            _verticesDictionary[vertex.ID] = vertex;
            _adjacencyList[vertex.ID] = [edge.From.ID, edge.To.ID];

            ReplaceAdjacencyVertex(edge.From.ID, edge.To.ID, vertex.ID);
            var newEdge = new Edge(edge)
            {
                To = vertex
            };
            AddEdgeToDict(newEdge);

            ReplaceAdjacencyVertex(edge.To.ID, edge.From.ID, vertex.ID);
            var newEdge2 = new Edge(edge)
            {
                From = vertex
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

        public override List<Vertex> RemoveVertices(List<Vertex> vertexes)
        {
            var nonexistVertex = new List<Vertex>();

            foreach (var vertex in vertexes)
            {
                var id = vertex.ID;
                if (!_verticesDictionary.ContainsKey(id))
                {
                    nonexistVertex.Add(vertex);
                    continue;
                }

                foreach (var adjacencyId in _adjacencyList[id])
                {
                    _adjacencyList[adjacencyId].Remove(id);
                    RemoveEdgeFromDict(id, adjacencyId);
                }

                _adjacencyList.Remove(id);
                _verticesDictionary.Remove(id);
            }

            return nonexistVertex;
        }

        public override Edge GetEdge(int from, int to, double weight)
        {
            return _edgesDictionary[from < to  ? (from, to) : (to, from)];
        }

        public override List<Edge> GetAdjacencyEdges(Vertex v)
        {
            var id = v.ID;
            if (!_verticesDictionary.ContainsKey(id))
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            return _adjacencyList[id].Select(to => _edgesDictionary[id < to ? (id,to) : (to, id)]).ToList();
        }

        public override bool ContainEdge(Edge e)
        {
            return _edgesDictionary.ContainsKey(e.From.ID < e.To.ID ? (e.From.ID, e.To.ID) : (e.To.ID, e.From.ID));
        }

        public override List<Edge> AddEdges(List<Edge> edges)
        {
            var existEdges = new List<Edge>();

            foreach (var e in edges)
            {
                var hasEdge = ContainEdge(e);
                var hasVertex = _verticesDictionary.ContainsKey(e.From.ID) && _verticesDictionary.ContainsKey(e.To.ID);
                if (!hasVertex || hasEdge)
                {
                    existEdges.Add(e);
                    continue;
                }

                _adjacencyList[e.From.ID].Add(e.To.ID);
                _adjacencyList[e.To.ID].Add(e.From.ID);
                AddEdgeToDict(e);
            }

            return existEdges;
        }

        public override List<Edge> RemoveEdges(List<Edge> edges)
        {
            var nonExistEdges = new List<Edge>();

            foreach (var e in edges)
            {
                if (!ContainEdge(e))
                {
                    nonExistEdges.Add(e);
                    continue;
                }

                _adjacencyList[e.From.ID].Remove(e.To.ID);
                _adjacencyList[e.To.ID].Remove(e.From.ID);
                RemoveEdgeFromDict(e);
            }

            return nonExistEdges;
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

    public class Vertex(int id, QRCode qr)
    {
        public int ID { get; set; } = id;
        public QRCode QR { get; set; } = qr;

        public Vertex(Vertex v) : this(v.ID, v.QR) { }

        public Vertex(int id) : this(id, new QRCode()) { }

        public static bool operator ==(Vertex v1, Vertex v2)
        {
            return v1.ID == v2.ID;
        }

        public static bool operator !=(Vertex v1, Vertex v2)
        {
            return !(v1 == v2);
        }

        public override string ToString()
        {
            return $"{id}:\n\tqr: {qr}";
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

        public override string ToString()
        {
            return $"{From.ID} -> {To.ID}: {Weight}";
        }
    }

    public class QRCode
    {
        public QRCode()
        {

        }

        public QRCode(int id, QRState state)
        {
            ID = id;
            State = state;
        }

        public int ID { get; set; } = 0;
        public QRState State { get; set; } = QRState.SyntropyUnlock;

        public override string ToString()
        {
            return $"id: {ID}, state: {State}";
        }
    }

    public enum QRState
    {
        SyntropyLock = 0,
        SyntropyUnlock,
        SyntropyLeft,
        SyntropyForward,
        SyntropyRight,
        SubtendLock,
        SubtendUnlock,
        SubtendLeft,
        SubtendForward,
        SubtendRight,
    }
}

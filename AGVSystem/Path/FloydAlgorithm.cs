using AGVSystem.Models;

namespace AGVSystem.Path
{
    public class Floyd
    {
        private const int Inf = int.MaxValue;

        private List<List<double>> _distance = []; //最短路径距离矩阵
        private List<List<int>> _path = []; //最近邻矩阵
        private List<int> signal_path_ = [];

        //新建的变量
        private readonly ConcreteMap map = MapContext.GetInstance();

        private List<Vertex> GetVertices()
        {
            return map.Vertices;


        }

        private int GetVerticesNum()
        {
            var vertices_num = map.V;
            return vertices_num;
        }

        private List<Vertex> GetAdjacencyVertices(int id)
        {
            return map.GetAdjacencyVertices(id);
        }

        private List<Edge> GetAdjacencyEdges(int id)
        {
            return map.GetAdjacencyEdges(id);
        }

        private List<PathPoint> AddModeAngleMissionInPath(List<int> path)
        {
            var pathTemp = new List<PathPoint>();
            if (path.Count == 0)
            {
                try
                {
                    throw new Exception("Path is empty.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Caught  exception: " + ex.Message);
                    return new List<PathPoint>();
                }
            }

            var verticesTemp = GetVertices();
            for (int i = 0; i < path.Count; i++)
            {
                var index = path[i];
                path[i] = verticesTemp[index].ID;
                PathPoint pp = new PathPoint
                    { codeID = path[i], moveMOD = 1, angle = 0.0, missionID = 0, name = path[i].ToString() };
                pathTemp.Add(pp);
            }

            return pathTemp;
        }

        public void FloydRun()
        {
            _distance.Clear();
            _path.Clear();
            var vertices_nums = GetVerticesNum();
            var vertices = GetVertices();
            
            for (var i = 0; i < vertices_nums; i++)
            {
                _distance.Add(new List<double>(vertices_nums));
                _path.Add(new List<int>(vertices_nums));
                for (var j = 0;j< vertices_nums; j++)
                {
                    _distance[i].Add(Inf);
                    _path[i].Add(-1);
                }
            }

            // 填充初始距离矩阵和最近邻矩阵
            for (int i = 0; i < vertices_nums; ++i)
            {
                int id = vertices[i].ID; //获取该序号下二维码id
                List<Edge>  adjacency_edges = GetAdjacencyEdges(id); //通过该二维码id获取对应邻接边
                List<Vertex> adjacency_vertices = GetAdjacencyVertices(id); //通过该二维码id获取对应邻接顶点

                foreach(Vertex vert in adjacency_vertices)
                {
                    int qrId = vert.ID;
                    int index = -1;
                    foreach (var ver in vertices)
                    {
                        if (ver.ID == vert.ID)
                        {
                            index = vertices.IndexOf(ver);
                        }
                    }

                    //if(map.TryGetEdge(id, vert.ID, out var e_cur))
                    //{
                    //    _distance[i][index] = e_cur.Weight;
                    //    _path[i][index] = i;
                    //}
                    Edge e_cur = map.GetEdge(id, vert.ID);
                    _distance[i][index] = e_cur.Weight;
                    _path[i][index] = i;

                }
            }

            // 执行Floyd算法
            for (int k = 0; k < vertices_nums; ++k)
            {
                for (int i = 0; i < vertices_nums; ++i)
                {
                    for (int j = 0; j < vertices_nums; ++j)
                    {
                        if (_distance[i][k] != Inf && _distance[k][j] != Inf &&
                            _distance[i][k] + _distance[k][j] < _distance[i][j])
                        {
                            _distance[i][j] = _distance[i][k] + _distance[k][j];
                            _path[i][j] = k; // 更新最近邻矩阵
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paths_id">所有路径的起点、终点</param>
        /// <returns>所有路径</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Dictionary<int, List<PathPoint>> GetAllPaths(Dictionary<int, Tuple<int, int>> paths_id)
        {
            FloydRun();
            Dictionary<int, List<PathPoint>> all_paths  = new Dictionary<int, List<PathPoint>>();
            if(paths_id == null || paths_id.Count == 0)
            {
                throw new ArgumentNullException(nameof(paths_id), "the paths_id is null or empty.");
            }
            foreach (KeyValuePair<int, Tuple<int, int>> path_id in paths_id)
            {
                var num = path_id.Key;
                var (pathStart, pathEnd) = path_id.Value;

                var vertexTemp = GetVertices();
                var pathStartIndex = -1; 
                var pathEndIndex = -1;
                foreach (var vert in vertexTemp)
                {
                    if (pathStart == vert.ID)
                    {
                        pathStartIndex = vertexTemp.IndexOf(vert);
                    }
                    if (pathEnd == vert.ID)
                    {
                        pathEndIndex = vertexTemp.IndexOf(vert);
                    }
                }
                SignalPath(pathStartIndex, pathEndIndex);
                List<int> path = signal_path_.Select(item=>item).ToList();
                var pathPoint = AddModeAngleMissionInPath(path);
                all_paths[num] = pathPoint;                                            
                signal_path_.Clear();
            }
            return all_paths;
        }

        public List<int> GetSignalPath(int start, int end)
        {
            SignalPath(start, end);
            var path = signal_path_.Select(item => item).ToList();
            return path;
        }

        public void SignalPath(int start, int end)
        {
            var vertices_nums = GetVerticesNum();
            if ((start > vertices_nums) || (end > vertices_nums) ||(start < 0) || (end < 0))
            {
                signal_path_ = new List<int>();
                return;
            }
            var intermediateK = _path[start][end];
            if (intermediateK == -1) // 表示两点之间没有路(-1应该作为设置参数)
            {
                signal_path_ = new List<int>();
                return;
            }
            else if (intermediateK == start) // 表示前节点等于出发点找到两者最短路径
            {
                if (signal_path_.Count == 0)
                {
                    signal_path_.Add(start);
                    signal_path_.Add(end);
                }
                else
                {
                    signal_path_.Add(end);
                }
            }
            else // 如果没找到 要从中间分开成两段 两段分别找
            {
                SignalPath(start, intermediateK);
                SignalPath(intermediateK, end);
            }

        }

        public void print_path()
        {
            foreach (var item in signal_path_)
            {
                Console.Write(item);
                Console.Write(" ");
            }
            Console.Write("\n");
        }
        // 打印矩阵
        private static void PrintMatrix(int[,] matrix, int vertices)
        {
            for (int i = 0; i < vertices; ++i)
            {
                for (int j = 0; j < vertices; ++j)
                {
                    if (matrix[i, j] == Inf)
                        Console.Write("Inf\t");
                    else
                        Console.Write($"{matrix[i, j]}\t");
                }
                Console.WriteLine();
            }
        }
    }
}

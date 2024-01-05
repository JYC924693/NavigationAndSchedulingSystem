using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PathAlgorithm
{
    internal class FloydAlgorithm
    {
        private static int INF = int.MaxValue;
        public static int[,] distance_; // 最短路径距离矩阵
        public static int[,] path_; // 最近邻矩阵
        //public List<List<int>> distance_ = [];
        //public List<List<int>> path_ = [];
        public Dictionary<int, List<int>> all_paths_ = [];
        public static List<int> signal_path_ = [];
        public Dictionary<int, Tuple<int, int>> paths_id_ = [];

        public static void Floyd(Graph graph)
        {
            distance_ = new int[graph.Vertices, graph.Vertices];
            path_ = new int[graph.Vertices, graph.Vertices];
            // 初始化距离矩阵和最近邻矩阵
            for (int i = 0; i < graph.Vertices; i++)
            {
                for (int j = 0; j < graph.Vertices; j++)
                {
                    distance_[i, j] = INF;
                    // 初始情况下，最近邻矩阵为-1表示无直接连接的顶点
                    path_[i, j] = -1; 
                }
            }

            // 填充初始距离矩阵和最近邻矩阵
            for (int i = 0; i < graph.Vertices; ++i)
            {
                foreach (var neighbor in graph.AdjList[i])
                {
                    distance_[i, neighbor.Vertex] = neighbor.Weight;
                    path_[i, neighbor.Vertex] = i; // 初始情况下，最近邻矩阵为直接相连的顶点
                }
            }

            // 执行Floyd算法
            for (int k = 0; k < graph.Vertices; ++k)
            {
                for (int i = 0; i < graph.Vertices; ++i)
                {
                    for (int j = 0; j < graph.Vertices; ++j)
                    {
                        if (distance_[i, k] != INF && distance_[k, j] != INF &&
                            distance_[i, k] + distance_[k, j] < distance_[i, j])
                        {
                            distance_[i, j] = distance_[i, k] + distance_[k, j];
                            path_[i, j] = k; // 更新最近邻矩阵
                        }
                    }
                }
            }
            // 打印最短路径矩阵
            //Console.WriteLine("最短路径矩阵:");
            //PrintMatrix(distance_, graph.Vertices);

            // 打印最近邻矩阵
            //Console.WriteLine("\n最近邻矩阵:");
            //PrintMatrix(path_, graph.Vertices);
        }

        public static void GetAllPaths(Dictionary<int, Tuple<int, int>> paths_id)
        {
            bool isEmpty = path_.Length == 0;
            if (isEmpty)
            {
                return;
            }
            foreach(KeyValuePair<int, Tuple<int, int>> path_id in paths_id)
            {
                int num = path_id.Key;
                Tuple<int, int> start_end_pair = path_id.Value;
                int path_start = start_end_pair.Item1;
                int path_end = start_end_pair.Item2;

                GetPath(path_start, path_end);
                print_path();
                signal_path_.Clear();   
            }
        }

        public static void GetPath(int start, int end)
        {
            int intermediate_k = path_[start, end];
            if (intermediate_k == -1) // 表示两点之间没有路(-1应该作为设置参数)
            {
                List<int> no_path = new List<int>();
                //all_paths_.Add(num, no_path);
                signal_path_ = no_path;
            }
            else if (intermediate_k == start) // 表示前节点等于出发点找到两者最短路径
            {
                //List<int> path_i = new List<int>();
                //path_i.Add(path_start);
                //path_i.Add(path_end);
                //all_paths_.Add(num, path_i);
                if (signal_path_.Count() == 0)
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
                GetPath(start, intermediate_k);
                GetPath(intermediate_k, end);
            }

        }

        public static void print_path()
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
                    if (matrix[i, j] == INF)
                        Console.Write("INF\t");
                    else
                        Console.Write($"{matrix[i, j]}\t");
                }
                Console.WriteLine();
            }
        }
    }
}

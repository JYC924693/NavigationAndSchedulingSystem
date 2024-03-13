using AGVSystem.Models;

namespace AGVSystem.Path
{
    public class Dijkstra
    {
        private const int Inf = int.MaxValue;
        private readonly ConcreteMap _map = MapContext.GetInstance();

        public Dijkstra() { }

        private List<Vertex> GetVertices()
        {
            return _map.Vertices;
        }

        private int GetVerticesNum()
        {
            var verticesNum = _map.V;
            return verticesNum;
        }


        private List<Vertex> GetAdjacencyVertices(int id)
        {
            return _map.GetAdjacencyVertices(id);
        }

        private List<Edge> GetAdjacencyEdges(int id)
        {
            return _map.GetAdjacencyEdges(id);
        }

        private int GetIndexFromVertices(int id)
        {
            int index = -1;
            var verticesTemp = GetVertices();
            foreach (var vert in verticesTemp)
            {
                if (vert.ID == id)
                {
                    index = verticesTemp.IndexOf(vert);
                    break;
                }
            }

            //if (index == -1)
            //{
            //    try
            //    {
            //        throw new Exception("Index cannot be -1.");
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine("Caught  exception: " + ex.Message);
            //    }
            //}
            return index;
        }

        private int GetVertIDFormIndex(int index)
        {
            var verticesTemp = GetVertices();
            int vertID = verticesTemp[index].ID;
            return vertID;
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

            for (int i = 0; i < path.Count; i++)
            {
                var index = path[i];
                //path[i] = verticesTemp[index].ID;
                PathPoint pp = new PathPoint
                    { codeID = index, moveMOD = 1, angle = 0.0, missionID = 0, name = index.ToString() };
                pathTemp.Add(pp);
            }

            return pathTemp;
        }

        public List<int> DijkstraRun(int start, int end)
        {
            var verticesCount = GetVerticesNum();
            var vertices = GetVertices();

            var distances = new List<double>(verticesCount);// 最短距离
            var previous = new List<int>(verticesCount);// 前驱数组
            var visited = new List<bool>(verticesCount);// 遍历记录
            var startIndex = GetIndexFromVertices(start);
            var endIndex = GetIndexFromVertices(end);

            for (var i = 0; i < verticesCount; i++)
            {
                distances.Add(Inf);
                previous.Add(-1);
                visited.Add(false);
            }

            foreach (var vert in vertices)
            {
                if (vert.ID == start)
                {
                    var index = vertices.IndexOf(vert);
                    distances[index] = 0;
                    break;
                }
            }

            for (var i = 0; i < verticesCount-1; i++)
            {
                var minDistanceVertex = GetMinDistance(distances, visited);//数组索引
                visited[minDistanceVertex] = true;//遍历标识
                //遍历邻接点
                var id = vertices[minDistanceVertex].ID;//
                var adjacencyEdges = GetAdjacencyEdges(id);
                var adjacencyVertices = GetAdjacencyVertices(id);
                foreach (var vert in adjacencyVertices)
                {
                    int index = -1;
                    foreach (var ver in vertices)
                    {
                        if (ver.ID == vert.ID)
                        { 
                            index = vertices.IndexOf(ver);
                            break;
                        }
                    }

                    if (_map.TryGetEdge(id, vert.ID, out var curEdge))
                    {
                        var newDistance = distances[minDistanceVertex] + curEdge.Weight;
                        if (newDistance < distances[index])
                        {
                            distances[index] = newDistance;
                            previous[index] = vertices[minDistanceVertex].ID;
                        }
                    }
                }
            }
            ShortestPath(startIndex, endIndex, previous, out var path);
            return path;
        }

        private int GetMinDistance(List<double> distanceDoubles, List<bool> visitedBools)
        {
            double minDistance = Inf;
            var minDistanceVertex = -1;

            for (var i = 0; i < distanceDoubles.Count; i++)
            {
                if (!visitedBools[i] && distanceDoubles[i] <= minDistance)
                {
                    minDistance = distanceDoubles[i];
                    minDistanceVertex = i;
                }
            }
            return minDistanceVertex;
        }

        private void ShortestPath(int start, int end, List<int> previous, out List<int> path)
        {
            path = new List<int>();
            int currentVertex = end;
            while (currentVertex != -1)
            {
                int verticeID = GetVertIDFormIndex(currentVertex);
                path.Insert(0, verticeID);
                int currentVertexTemp = previous[currentVertex];
                currentVertex = GetIndexFromVertices(currentVertexTemp);
            }
        }

        public List<PathPoint> GetPath(int start, int end)
        {

            var path = DijkstraRun(start, end);
            var pathPoint = AddModeAngleMissionInPath(path);

            return pathPoint;
        }
    }
}

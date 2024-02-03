using AGVSystem.Models;

namespace AGVSystem.Path
{
    public class Dijkstra
    {
        private const int Inf = int.MaxValue;
        private readonly ConcreteMap _map = MapContext.GetInstance();

        public Dijkstra() { }

        private int GetVerticesNum()
        {
            var verticesNum = _map.V;
            return verticesNum;
        }

        private List<Vertex> GetVertices()
        {
            return _map.Vertices;
        }

        private List<Vertex> GetAdjacencyVertices(int id)
        {
            return _map.GetAdjacencyVertices(id);
        }

        private List<Edge> GetAdjacencyEdges(int id)
        {
            return _map.GetAdjacencyEdges(id);
        }

        public List<int> DijkstraRun(int start, int end)
        {
            var verticesCount = GetVerticesNum();
            var vertices = GetVertices();

            var distances = new List<double>(verticesCount);
            var previous = new List<int>(verticesCount);
            var visited = new List<bool>(verticesCount);

            for (var i = 0; i < verticesCount; i++)
            {
                distances.Add(Inf);
                previous.Add(-1);
                visited.Add(false);
            }

            distances[start] = 0;

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
                    int index = vertices.IndexOf(vert);

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
            ShortestPath(start, end, previous, out var path);
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
                path.Insert(0, currentVertex);
                currentVertex = previous[currentVertex];
            }
        }
    }
}

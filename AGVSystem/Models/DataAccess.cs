#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using MySql.Data.MySqlClient;
using System.Data;
using MySql.Data.Types;
using System.Xml.Linq;
using System.Text.Json;

namespace AGVSystem.Models
{
    public class DataAccess
    {
        //public QrTable CreateQrTable()
        //{

        //}
        //public PathTable CreateAPthTable()
        //{

        //}
    }

    public static class MySqlConnectionInstance
    {
        private static readonly MySqlConnection Connection;

        static MySqlConnectionInstance()
        {
            var builder = new MySqlConnectionStringBuilder
            {
                UserID = "agv_system_owner",
                Password = "%6eueh5D3^ntHM$&V&0*DLj$#@2s$F",
                Server = "localhost",
                Database = "agv_system"
            };

            Connection = new MySqlConnection(builder.ConnectionString);
            //if (!Connection.Ping())
            //{
            //    throw new MySqlConversionException("Ping测试失败！");
            //}
        }

        public static MySqlConnection GetInstance()
        {
            return Connection;
        }
    }

    public abstract class QrTable
    {
        public abstract List<Vertex> GetVertices();
        public abstract Vertex GetVertex(int id);
        public abstract List<Edge> GetEdges();
        public abstract double GetWeight(int from, int to);
        public abstract bool AddVertex(Vertex vertex);
        public abstract bool ChangeVertex(int id, Vertex newVertex);
        public abstract bool ChangeVertexState(int id, QRState state);
        public abstract bool ChangeVertexId(int id, int newId);
        public abstract bool ChangeEdge(int from, int to, Edge newEdge);
        public abstract bool ChangeEdgeId(int from, int to, int newFrom, int newTo);
        public abstract bool ChangeWeight(int from, int to, int weight);
        public abstract bool RemoveVertex(int id);
        public abstract bool RemoveEdge(int from, int to);
    }

    public class MySqlQrTable : QrTable
    {
        private MySqlConnection _connection;
        private Dictionary<int, HashSet<int>> _adjacencyList;

        public MySqlQrTable()
        {
            _connection = MySqlConnectionInstance.GetInstance();

            _adjacencyList = [];
            
            foreach (var id in GetAllIds())
            {
                _adjacencyList.Add(id, []);
            }

            foreach (var (from, to) in GetAllEdges())
            {
                _adjacencyList[from].Add(to);
                _adjacencyList[to].Add(from);
            }
        }

        private List<int> GetAllIds()
        {
            var cmd = new MySqlCommand("SELECT id FROM qrcodes_vw", _connection);

            _connection.Open();

            var ids = new List<int>();
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                ids.Add(reader.GetInt32("id"));
            }

            _connection.Close();

            return ids;
        }

        private List<(int From, int To)> GetAllEdges()
        {
            var cmd = new MySqlCommand("SELECT head, tail FROM edges", _connection);

            _connection.Open();

            var edges = new List<(int From, int To)>();
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                edges.Add((reader.GetInt32("head"), reader.GetInt32("tail")));
            }

            _connection.Close();

            return edges;
        }

        public override List<Vertex> GetVertices()
        {
            var vertices = new List<Vertex>();

            var cmd = new MySqlCommand("SELECT * FROM qrcodes_vw", _connection);

            _connection.Open();

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var id = reader.GetInt32("id");
                var state = (QRState)reader.GetInt32("state_id");
                var qr = new QRCode(id, state);
                vertices.Add(new Vertex(id, qr));
            }

            _connection.Close();

            return vertices;
        }

        public override Vertex GetVertex(int id)
        {
            if (!_adjacencyList.ContainsKey(id))
                throw new ArgumentOutOfRangeException(nameof(id));

            var vertex = new Vertex(id);
            
            var cmd = new MySqlCommand("SELECT * FROM qrcodes_vw WHERE id = @id", _connection);
            cmd.Parameters.AddWithValue("@id", id);

            _connection.Open();

            var reader = cmd.ExecuteReader();
            var state = (QRState)reader.GetInt32("state_id");
            vertex.QR = new QRCode(id, state);

            _connection.Close();

            return vertex;
        }

        public override List<Edge> GetEdges()
        {
            var edges = new List<Edge>();

            var cmd = new MySqlCommand("SELECT * FROM edges", _connection);
            

            _connection.Open();

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var from = reader.GetInt32("head");
                var to = reader.GetInt32("tail");
                var weight = reader.GetDouble("weight");
                edges.Add(new Edge(from, to, weight));
            }

            _connection.Close();

            return edges;
        }

        private bool ContainEdge(int from, int to)
        {
            return _adjacencyList.ContainsKey(from) && _adjacencyList.ContainsKey(to) &&
                   _adjacencyList[from].Contains(to);
        }

        public override double GetWeight(int from, int to)
        {
            if (!ContainEdge(from, to))
                throw new ArgumentOutOfRangeException();

            var cmd = new MySqlCommand("SELECT weight FROM edges WHERE head = @head And tail = @tail", _connection);
            cmd.Parameters.AddWithValue("@head", from);
            cmd.Parameters.AddWithValue("@tail", to);

            _connection.Open();

            var reader = cmd.ExecuteReader();
            var weight = reader.GetDouble("weight");

            _connection.Close();

            return weight;
        }

        public override bool AddVertex(Vertex vertex)
        {
            if (!_adjacencyList.ContainsKey(vertex.ID))
                throw new ArgumentOutOfRangeException(nameof(vertex));

            var cmd = new MySqlCommand("INSERT INTO qrcodes (id, state) VALUES (@id, @state)", _connection);
            cmd.Parameters.AddWithValue("@id", vertex.ID);
            cmd.Parameters.AddWithValue("@state", (int)vertex.QR.State);
            
            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            _adjacencyList.Add(vertex.ID, []);

            return rowsAffected > 0;
        }

        private void ReplaceId(int id, int newId)
        {
            _adjacencyList.Add(newId, _adjacencyList[id]);
            _adjacencyList.Remove(id);

            foreach (var adj in _adjacencyList[newId])
            {
                _adjacencyList[adj].Remove(id);
                _adjacencyList[adj].Add(newId);
            }
        }

        public override bool ChangeVertex(int id, Vertex newVertex)
        {
            if(!_adjacencyList.ContainsKey(id))
                throw new ArgumentOutOfRangeException(nameof(id));

            var cmd = new MySqlCommand("UPDATE qrcodes SET id = @newId, state = @newState WHERE id = @id", _connection);
            cmd.Parameters.AddWithValue("@newId", newVertex.ID);
            cmd.Parameters.AddWithValue("@newState", (int)newVertex.QR.State);
            cmd.Parameters.AddWithValue("@id", id);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            if (id != newVertex.ID)
            {
                ReplaceId(id, newVertex.ID);
            }

            return rowsAffected > 0;
        }

        public override bool ChangeVertexState(int id, QRState state)
        {
            if (!_adjacencyList.ContainsKey(id))
                throw new ArgumentOutOfRangeException(nameof(id));

            var cmd = new MySqlCommand("UPDATE qrcodes SET state = @newState WHERE id = @id", _connection);
            cmd.Parameters.AddWithValue("@newState", (int)state);
            cmd.Parameters.AddWithValue("@id", id);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            return rowsAffected > 0;
        }

        public override bool ChangeVertexId(int id, int newId)
        {
            if (!_adjacencyList.ContainsKey(id))
                throw new ArgumentOutOfRangeException(nameof(id));

            var cmd = new MySqlCommand("UPDATE qrcodes SET id = @newId WHERE id = @id", _connection);
            cmd.Parameters.AddWithValue("@newId", newId);
            cmd.Parameters.AddWithValue("@id", id);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            ReplaceId(id, newId);

            return rowsAffected > 0;
        }

        private void ReplaceEdge(int from, int to, int newFrom, int newTo)
        {
            if (from != newFrom)
            {
                _adjacencyList[from].Remove(to);
                _adjacencyList[newFrom].Add(newTo);
            }

            if (to != newTo)
            {
                _adjacencyList[to].Remove(from);
                _adjacencyList[newTo].Add(newFrom);
            }
        }

        public override bool ChangeEdge(int from, int to, Edge newEdge)
        {
            if (!ContainEdge(from, to))
                throw new ArgumentOutOfRangeException();

            var cmd = new MySqlCommand("UPDATE edges SET weight = @newWeight WHERE head = @head AND tail = @tail", _connection);
            cmd.Parameters.AddWithValue("@newWeight", newEdge.Weight);
            cmd.Parameters.AddWithValue("@head", from);
            cmd.Parameters.AddWithValue("@tail", to);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            ReplaceEdge(from, to, newEdge.From.ID, newEdge.To.ID);

            return rowsAffected > 0;
        }

        public override bool ChangeEdgeId(int from, int to, int newFrom, int newTo)
        {
            if (!ContainEdge(from, to))
                throw new ArgumentOutOfRangeException();

            var cmd = new MySqlCommand("UPDATE edges SET head = @newFrom, tail = @newTo WHERE head = @head AND tail = @tail", _connection);
            cmd.Parameters.AddWithValue("@newFrom", newFrom);
            cmd.Parameters.AddWithValue("@newTo", newTo);
            cmd.Parameters.AddWithValue("@head", from);
            cmd.Parameters.AddWithValue("@tail", to);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            ReplaceEdge(from, to, newFrom, newTo);

            return rowsAffected > 0;
        }

        public override bool ChangeWeight(int from, int to, int weight)
        {
            if (!ContainEdge(from, to))
                throw new ArgumentOutOfRangeException();

            var cmd = new MySqlCommand("UPDATE edges SET weight = @newWeight WHERE head = @head AND tail = @tail", _connection);
            cmd.Parameters.AddWithValue("@newWeight", weight);
            cmd.Parameters.AddWithValue("@head", from);
            cmd.Parameters.AddWithValue("@tail", to);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            return rowsAffected > 0;
        }

        private void RemoveId(int id)
        {
            foreach (var adj in _adjacencyList[id])
            {
                _adjacencyList[adj].Remove(id);
            }

            _adjacencyList.Remove(id);
        }

        public override bool RemoveVertex(int id)
        {
            if (!_adjacencyList.ContainsKey(id))
                throw new ArgumentOutOfRangeException(nameof(id));

            var cmd = new MySqlCommand("DELETE FROM qrcodes WHERE id = @id", _connection);
            cmd.Parameters.AddWithValue("@id", id);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            RemoveId(id);

            return rowsAffected > 0;
        }

        private void RemoveAdjacency(int from, int to)
        {
            _adjacencyList[from].Remove(to);
            _adjacencyList[to].Remove(from);
        }

        public override bool RemoveEdge(int from, int to)
        {
            if (!ContainEdge(from, to))
                throw new ArgumentOutOfRangeException();

            var cmd = new MySqlCommand("DELETE FROM edges WHERE head = @head AND tail = @tail", _connection);
            cmd.Parameters.AddWithValue("@head", from);
            cmd.Parameters.AddWithValue("@tail", to);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            RemoveAdjacency(from, to);

            return rowsAffected > 0;
        }

    }

    public abstract class PathTable
    {
        public abstract Path GetPath(string name);
        public abstract bool AddPath(Path path);
        public abstract bool ChangePath(string name, Path newPath);
        public abstract bool ChangeIdSequence(string name, List<int> newIdSequence);
        public abstract bool ChangeAgvStateSequence(string name, List<AgvState> newAgvState);
        public abstract bool ChangeAngleSequence(string name, List<double> newAngleSequence);
        public abstract bool ChangeTaskModeSequence(string name, List<TaskMode> newTaskModeSequence);
        public abstract bool ChangeTaskName(string name, string newName);
        public abstract bool RemovePath(string name);
    }

    public class MySqlPathTable : PathTable
    {
        private MySqlConnection _connection;
        private List<string> _pathNames;

        public MySqlPathTable()
        {
            _connection = MySqlConnectionInstance.GetInstance();

            _pathNames = new List<string>();
            var cmd = new MySqlCommand("SELECT task_name FROM paths");

            _connection.Open();

            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                _pathNames.Add(reader.GetString("task_name"));
            }

            _connection.Close();
        }
        public override Path GetPath(string name)
        {
            if (!_pathNames.Contains(name))
                throw new ArgumentOutOfRangeException(nameof(name));

            var cmd = new MySqlCommand("SELECT * FROM paths WHERE task_name = @task_name", _connection);
            cmd.Parameters.AddWithValue("@task_name", name);

            _connection.Open();
            var reader = cmd.ExecuteReader();
            Path path = null;
            if (reader.Read())
            {
                path = new Path
                {
                    Name = reader["name"].ToString(),
                    IdSequence = JsonSerializer.Deserialize<List<int>>(reader["id_sequence"].ToString()),
                    MoveSequence = JsonSerializer.Deserialize<List<AgvState>>(reader["agv_state_sequence"].ToString()),
                    AngleSequence = JsonSerializer.Deserialize<List<double>>(reader["angle_sequence"].ToString()),
                    TaskSequence = JsonSerializer.Deserialize<List<TaskMode>>(reader["task_mode_sequence"].ToString())
                };
            }
            _connection.Close();

            return path;
        }

        public override bool AddPath(Path path)
        {
            if (_pathNames.Contains(path.Name))
            {
                return false;
            }

            var cmd = new MySqlCommand("INSERT INTO paths (id_sequence, angle_sequence, mode_sequence, task_sequence, task_name) VALUES (@id_sequence, @angle_sequence, @mode_sequence, @task_sequence, @task_name)", _connection);
            cmd.Parameters.AddWithValue("@id_sequence", JsonSerializer.Serialize(path.IdSequence));
            cmd.Parameters.AddWithValue("@angle_sequence", path.AngleSequence);
            cmd.Parameters.AddWithValue("@mode_sequence", path.MoveSequence);
            cmd.Parameters.AddWithValue("@task_sequence", path.TaskSequence);
            cmd.Parameters.AddWithValue("@task_name", path.Name);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            _pathNames.Add(path.Name);

            return rowsAffected > 0;
        }

        public override bool ChangePath(string name, Path newPath)
        {
            if (!_pathNames.Contains(name))
            {
                return false;
            }

            var cmd = new MySqlCommand("UPDATE paths SET id_sequence = @id_sequence, angle_sequence = @angle_sequence, mode_sequence = @mode_sequence, task_sequence = @task_sequence, task_name = @new_task_name WHERE task_name = @task_name", _connection);
            cmd.Parameters.AddWithValue("@id_sequence", JsonSerializer.Serialize(newPath.IdSequence));
            cmd.Parameters.AddWithValue("@mode_sequence", JsonSerializer.Serialize(newPath.MoveSequence));
            cmd.Parameters.AddWithValue("@angle_sequence", JsonSerializer.Serialize(newPath.AngleSequence));
            cmd.Parameters.AddWithValue("@task_sequence", JsonSerializer.Serialize(newPath.TaskSequence));
            cmd.Parameters.AddWithValue("@new_task_name", name);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            if (name != newPath.Name)
            {
                _pathNames.Remove(name);
                _pathNames.Add(newPath.Name);
            }

            return rowsAffected > 0;
        }

        public override bool ChangeIdSequence(string name, List<int> newIdSequence)
        {
            if (!_pathNames.Contains(name))
            {
                return false;
            }

            var cmd = new MySqlCommand("UPDATE paths SET id_sequence = @newIdSequence WHERE task_name = @task_name", _connection);
            cmd.Parameters.AddWithValue("@newIdSequence", JsonSerializer.Serialize(newIdSequence));
            cmd.Parameters.AddWithValue("@task_name", name);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            return rowsAffected > 0;
        }

        public override bool ChangeAgvStateSequence(string name, List<AgvState> newAgvState)
        {
            if (!_pathNames.Contains(name))
            {
                return false;
            }

            var cmd = new MySqlCommand("UPDATE paths SET mode_sequence = @mode_sequence WHERE task_name = @task_name", _connection);
            cmd.Parameters.AddWithValue("@mode_sequence", JsonSerializer.Serialize(newAgvState));
            cmd.Parameters.AddWithValue("@task_name", name);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            return rowsAffected > 0;
        }

        public override bool ChangeAngleSequence(string name, List<double> newAngleSequence)
        {
            if (!_pathNames.Contains(name))
            {
                return false;
            }

            var cmd = new MySqlCommand("UPDATE paths SET angle_sequence = @angle_sequence WHERE task_name = @task_name", _connection);
            cmd.Parameters.AddWithValue("@angle_sequence", JsonSerializer.Serialize(newAngleSequence));
            cmd.Parameters.AddWithValue("@task_name", name);

            _connection.Open();
            var rowsAffected = cmd.ExecuteNonQuery();
            _connection.Close();

            return rowsAffected > 0;
        }

        public override bool ChangeTaskModeSequence(string name, List<TaskMode> newTaskModeSequence)
        {
            if (!_pathNames.Contains(name))
            {
                return false;
            }

            var cmd = new MySqlCommand("UPDATE paths SET task_sequence = @task_sequence WHERE task_name = @task_name", _connection);
            cmd.Parameters.AddWithValue("@task_sequence", JsonSerializer.Serialize(newTaskModeSequence));
            cmd.Parameters.AddWithValue("@task_name", name);

            _connection.Open();
            var rowsAffected = cmd.ExecuteNonQuery();
            _connection.Close();

            return rowsAffected > 0;
        }

        public override bool ChangeTaskName(string name, string newName)
        {
            if (!_pathNames.Contains(name) || name == newName)
            {
                return false;
            }

            var cmd = new MySqlCommand("UPDATE paths SET task_name = @new_task_name WHERE task_name = @task_name", _connection);
            cmd.Parameters.AddWithValue("@new_task_name", newName);
            cmd.Parameters.AddWithValue("@task_name", name);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            _pathNames.Remove(name);
            _pathNames.Add(newName);

            return rowsAffected > 0;
        }

        public override bool RemovePath(string name)
        {
            if (!_pathNames.Contains(name))
            {
                return false;
            }

            var cmd = new MySqlCommand("DELETE FROM paths WHERE task_name = @task_name", _connection);
            cmd.Parameters.AddWithValue("@task_name", name);

            _connection.Open();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            _pathNames.Remove(name);

            return rowsAffected > 0;
        }
    }
    
    public class Path
    {
        public List<int> IdSequence { get; set; }
        public List<AgvState> MoveSequence { get; set; }
        public List<double> AngleSequence { get; set; }
        public List<TaskMode> TaskSequence { get; set; }
        public string Name { get; set; }

        public static bool operator ==(Path left, Path right)
        {
            var isEqual = left.Name == right.Name;

            if (isEqual)
            {
                isEqual = left.IdSequence.SequenceEqual(right.IdSequence);
            }
            
            if (isEqual)
            {
                isEqual = left.MoveSequence.SequenceEqual(right.MoveSequence);
            }

            if (isEqual)
            {
                isEqual = left.AngleSequence.SequenceEqual(right.AngleSequence);
            }

            if (isEqual)
            {
                isEqual = left.TaskSequence.SequenceEqual(right.TaskSequence);
            }

            return isEqual;
        }

        public static bool operator !=(Path left, Path right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{{\n\tIdSequence:\t\t{JsonSerializer.Serialize(IdSequence)},\n\tMoveSequence:\t{JsonSerializer.Serialize(MoveSequence)},\n\tAngleSequence:\t{JsonSerializer.Serialize(AngleSequence)},\n\tTaskSequence:\t{JsonSerializer.Serialize(TaskSequence)},\n\tName:\t\t\t\"{Name}\"\n}}";
        }
    }
}

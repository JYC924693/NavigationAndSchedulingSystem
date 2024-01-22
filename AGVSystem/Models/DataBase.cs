using MySql.Data.MySqlClient;
using System.Data;
using MySql.Data.Types;

namespace AGVSystem.Models
{
    public class DataBase
    {
    }

    public class MapConverter
    {
        private MySqlConnection _connection;
        public MapConverter(MySqlConnectionStringBuilder builder)
        {
            _connection = new MySqlConnection(builder.ConnectionString);
        }

        public MapConverter(string dbname, string name, string password)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                UserID = name,
                Password = password,
                Server = "localhost",
                Database = dbname
            };

            _connection = new MySqlConnection(builder.ConnectionString);
            if (!_connection.Ping())
            {
                throw new MySqlConversionException("Ping测试失败！");
            }
        }
        public List<Edge> GetEdges()
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

        public List<Vertex> GetVertices()
        {
            var vertices = new List<Vertex>();

            var cmd = new MySqlCommand("SELECT * FROM qrcodes_vw", _connection);

            _connection.Open();
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var id =reader.GetInt32("id");
                var state = (QRState)reader.GetInt32("state_id");
                var qr = new QRCode(id, state);
                vertices.Add(new Vertex(id, qr));
            }

            _connection.Close();

            return vertices;
        }

        public void AddVertex(Vertex v)
        {

        }

        public void RemoveVertex(Vertex v)
        {

        }
    }

    public abstract class QrTable
    {
        public abstract List<Vertex> GetVertices();
        public abstract List<Edge> GetEdges();
        public abstract bool AddQrCode(QRCode qr);
        public abstract QRCode ChangeQrCode(int id, QRCode newQr);
        public abstract QRState ChangeQrState(int id, QRState state);
        public abstract int ChangeQrId(int id, int newId);
        public abstract Edge ChangeEdge(int head, int tail, Edge newEdge);
        public abstract int ChangeEdgeId(int head, int tail, int newHead, int newTail);
        public abstract int ChangeWeight(int head, int tail, int weight);
        public abstract bool RemoveVertex(int id);
        public abstract bool RemoveEdge(int head, int tail);
    }

    public abstract class PathTable
    {
        public abstract Path GetPath(string name);
        public abstract bool AddPath(string name);
        public abstract Path ChangePath(string name, Path newPath);
        public abstract List<int> ChangeIdSequence(string name, List<int> newIdSequence);
        public abstract List<AgvState> ChangeAgvStateSequence(string name, List<AgvState> newAgvState);
        public abstract List<double> ChangeAngleSequence(string name, List<double> newAngleSequence);
        public abstract List<TaskMode> ChangeTaskModeSequence(string name, List<TaskMode> newTaskModeSequence);
        public abstract string ChangeTaskName(string name, string newName);
        public abstract bool RemovePath(string name);
    }

    public class Path
    {
        public List<int> IdSequence;
        public List<AgvState> MoveSequence;
        public List<double> AngleSequence;
        public List<TaskMode> TaskSequence;
        public string Name;
    }
}

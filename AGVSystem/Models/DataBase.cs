using MySql.Data.MySqlClient;
using System.Data;

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
    }
}

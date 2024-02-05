#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AGVSystem.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;
using Path = AGVSystem.Models.Path;

namespace AGVSystemTest
{
    [TestClass]
    public class QrTableTest
    {
        private QrTable _qrTable;

        [TestInitialize]
        public void Initialize()
        {
            // 初始化QrTable实现类对象，例如：
            _qrTable = new MySqlQrTable();
        }

        [TestMethod]
        public void GetEdges_JudgeAllEdges_ReturnsALLEdges(List<Edge> expected)
        {
            var edges = _qrTable.GetEdges();

            var actual = expected.Count == edges.Count && expected.SequenceEqual(edges);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void GetVertices_JudgeAllVertexes_ReturnsAllVertexes(List<Vertex> expected)
        {
            var vertexes = _qrTable.GetVertices();

            var actual = expected.Count == vertexes.Count && expected.SequenceEqual(vertexes);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void GetVertex_CompareVertex_ReturnsAVertex(int id, Vertex expected)
        {
            var vertex = _qrTable.GetVertex(id);

            Assert.AreEqual(expected, vertex);
        }

        [TestMethod]
        public void GetWeight_CompareWeight_ReturnsWeight(int from, int to, double expected)
        {
            var weight = _qrTable.GetWeight(from, to);

            Assert.AreEqual(expected, weight);
        }

        [TestMethod]
        public void AddVertex_CompareVertex_ReturnsTrue(Vertex vertex)
        {
            var result = _qrTable.AddVertex(vertex);
            if (result)
            {
                var actual = _qrTable.GetVertex(vertex.ID);
                result = actual == vertex;
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ChangeVertex_CompareReplacedVertex_ReturnsTrue(int id, Vertex newVertex)
        {
            var result = _qrTable.ChangeVertex(id, newVertex);
            if (result)
            {
                var actual = _qrTable.GetVertex(newVertex.ID);
                result = actual == newVertex;
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ChangeVertexState_CompareChangedState_ReturnsTrue(int id, QRState state)
        {
            var result = _qrTable.ChangeVertexState(id, state);
            if (result)
            {
                var actual = _qrTable.GetVertex(id).QR.State;
                result = actual == state;
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ChangeVertexId_CompareChangedId_ReturnsTrue(int id, int newId)
        {
            _qrTable.ChangeVertexId(id, newId);

            _qrTable.GetVertex(newId);
        }

        [TestMethod]
        public void ChangeEdge_CompareChangedEdge_ReturnsTrue(int head, int tail, Edge newEdge)
        {
            var result = _qrTable.ChangeEdge(head, tail, newEdge);
            if (result)
            {
                var weight = _qrTable.GetWeight(head, head);
                result = weight.Equals(newEdge.Weight);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ChangeEdgeId_TryGetChangedEdge_ReturnWeight(int head, int tail, int newHead, int newTail)
        {
            _qrTable.ChangeEdgeId(head, tail, newHead, newTail);

            _qrTable.GetWeight(newHead, newTail);
        }

        [TestMethod]
        public void ChangeWeight_CompareChangeWeight_ReturnsWeight(int head, int tail, int newWeight)
        {
            var result = _qrTable.ChangeWeight(head, tail, newWeight);
            if (result)
            {
                var actual = _qrTable.GetWeight(head, tail);
                result = actual == newWeight;
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RemoveVertex_TryGetRemovedVertex_ReturnException(int id)
        {
            _qrTable.RemoveVertex(id);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => _qrTable.GetVertex(id));
        }

        [TestMethod]
        public void RemoveEdge_TryGetRemovedEdge_ReturnException(int head, int tail)
        {
            _qrTable.RemoveEdge(head, tail);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => _qrTable.GetWeight(head, tail));
        }

        [TestMethod]
        public void AllOperation()
        {
            
        }
    }

    [TestClass]
    public class PathTableTest
    {
        private PathTable _path;

        [TestInitialize]
        public void Initialize()
        {
            _path = new MySqlPathTable();
        }

        [TestMethod]
        public void GetPath_ComparePath_ReturnsPath(string name, Path expected)
        {
            var path = _path.GetPath(name);

            Assert.AreEqual(expected, path);
        }

        [TestMethod]
        public void AddPath_TryGetAddedPathName_ReturnsPath(Path path)
        {
            var result = _path.AddPath(path);

            if (result)
            {
                _path.GetPath(path.Name);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ChangePath_CompareChangedPath_ReturnsTrue(string name, Path expected)
        {
            var result = _path.ChangePath(name, expected);

            if (result)
            {
                var actual = _path.GetPath(name);
                result = actual == expected;
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ChangeIdSequence_CompareChangedIdSequence_ReturnsTrue(string name, List<int> expected)
        {
            var result = _path.ChangeIdSequence(name, expected);

            if (result)
            {
                var actual = _path.GetPath(name).IdSequence;
                result = expected.SequenceEqual(actual);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ChangeAvgStateSequence_CompareChangedAgvStateSequence_ReturnsTrue(string name, List<AgvState> expected)
        {
            var result = _path.ChangeAgvStateSequence(name, expected);

            if (result)
            {
                var actual = _path.GetPath(name).MoveSequence;
                result = expected.SequenceEqual(actual);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ChangeAngleSequence_CompareChangedAngleSequence_ReturnsTrue(string name, List<double> expected)
        {
            var result = _path.ChangeAngleSequence(name, expected);

            if (result)
            {
                var actual = _path.GetPath(name).AngleSequence;
                result = expected.SequenceEqual(actual);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ChangeTaskModeSequence_CompareChangedTaskModeSequence_ReturnsTrue(string name, List<TaskMode> expected)
        {
            var result = _path.ChangeTaskModeSequence(name, expected);

            if (result)
            {
                var actual = _path.GetPath(name).TaskSequence;
                result = expected.SequenceEqual(actual);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ChangeTaskName_TryGetChangedPath_ReturnsTrue(string name, string expected)
        {
            var result = _path.ChangeTaskName(name, expected);

            if (result)
            {
                _path.GetPath(name);
            }

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RemovePath_TryGetRemovedPath_ReturnsException(string name)
        {
            _path.RemovePath(name);

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => _path.GetPath(name));
        }
    }

    public class DataBaseFactory
    {
        public static List<Edge> GetEdgeFromFile(string path)
        {
            var edges = new List<Edge>();

            var content = File.ReadAllLines(path);

            return edges;
        }

        public static List<Vertex> GetVertexFromFile(string path)
        {
            var vertices = new List<Vertex>();


            return vertices;
        }

        public static List<Path> GetPathFromFile(string path)
        {
            var pathList = new List<Path>();

            return pathList;
        }
    }

    public class Example : IEquatable<Example>
    {
        public int Member;
        public static bool operator ==(Example left, Example right)
        {
            return left.Member == right.Member;
        }

        public static bool operator !=(Example left, Example right)
        {
            return left.Member != right.Member;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return GetType() != obj?.GetType() || Equals(obj);
        }

        public bool Equals(Example? other)
        {
            return other is not null && (ReferenceEquals(this, other) || other == this);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Member);
        }
    }
}

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGVSystem.Models;

namespace AGVSystemTest
{
    [TestClass]
    public class DateBaseTest
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
        public void Test2()
        {
            var lst1 = new List<int> { 1, 2 };
            var lst2 = new List<int> { 1, 2 };

            Assert.IsTrue(lst1.SequenceEqual(lst2));
        }
    }
}

using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Qorpent.Graphs.Dot;

namespace Qorpent.Serialization.Tests {
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class GraphTests {
        /// <summary>
        ///     #AP-281
        /// </summary>
        [Test]
        public void CalculateEdgeInWeight() {
            var graph = new Graph();
            var nodes = new[] {new Node(), new Node(), new Node(), new Node(), new Node(), new Node() };
            nodes[0].Code = "1";
            nodes[1].Code = "2";
            nodes[2].Code = "3";
            nodes[3].Code = "4";
            nodes[4].Code = "5";
            nodes[5].Code = "6";
         
            foreach (var node in nodes) {
                graph.AddNode(node);
            }

            var edge1to2 = Edge.Create(nodes[0].Code, nodes[1].Code);
            var edge2to3 = Edge.Create(nodes[1].Code, nodes[2].Code);
            var edge6to3 = Edge.Create(nodes[5].Code, nodes[2].Code);
            var edge4to5 = Edge.Create(nodes[3].Code, nodes[4].Code);
            var edge5to3 = Edge.Create(nodes[4].Code, nodes[2].Code);

            graph.AddEdge(edge1to2);
            graph.AddEdge(edge2to3);
            graph.AddEdge(edge6to3);
            graph.AddEdge(edge4to5);
            graph.AddEdge(edge5to3);

            var ws = graph.CalculateWeight();
            var zeros = ws.Count(_ => _.Value == 0);
            var fives = ws.Count(_ => _.Value == 5);
            var ones = ws.Count(_ => _.Value == 1);

            Assert.AreEqual(7, ws.Sum(_ => _.Value));
            Assert.AreEqual(3, zeros);
            Assert.AreEqual(2, ones);
            Assert.AreEqual(1, fives);


            foreach (var w in ws) {
                Debug.Print(w.Key + " = " + w.Value);
            }
        }
        /// <summary>
        ///     #AP-281
        /// </summary>
        [Test]
        public void CalculateEdgeInWeightWithRecycles() {
            var graph = new Graph();
            var nodes = new[] { new Node(), new Node(), new Node(), new Node(), new Node(), new Node() };
            nodes[0].Code = "1";
            nodes[1].Code = "2";
            nodes[2].Code = "3";
            nodes[3].Code = "4";
            nodes[4].Code = "5";
            nodes[5].Code = "6";

            foreach (var node in nodes) {
                graph.AddNode(node);
            }

            var edge3to1 = Edge.Create(nodes[2].Code, nodes[0].Code);
            var edge1to2 = Edge.Create(nodes[0].Code, nodes[1].Code);
            var edge2to3 = Edge.Create(nodes[1].Code, nodes[2].Code);
            var edge6to3 = Edge.Create(nodes[5].Code, nodes[2].Code);
            var edge4to5 = Edge.Create(nodes[3].Code, nodes[4].Code);
            var edge5to3 = Edge.Create(nodes[4].Code, nodes[2].Code);

            graph.AddEdge(edge3to1);
            graph.AddEdge(edge1to2);
            graph.AddEdge(edge2to3);
            graph.AddEdge(edge6to3);
            graph.AddEdge(edge4to5);
            graph.AddEdge(edge5to3);
            
            var ws = graph.CalculateWeight();
            Assert.AreEqual(3, ws.Count(_ => _.Value == 1));
            Assert.AreEqual(1, ws.Count(_ => _.Value == 3));
            Assert.AreEqual(6, ws.Sum(_ => _.Value));

            foreach (var w in graph.CalculateWeight()) {
                Debug.Print(w.Key + " = " + w.Value);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Graphs.Dot;
using Qorpent.Graphs.Dot.Types;

namespace Qorpent.Serialization.Tests.Dot
{
    /// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class TuningTest
	{
		[Test]
		public void EdgeDirTest() {
			var e = new Edge();
			e.ArrowTail = ArrowType.Diamond;
			Assert.False(e.Attributes.ContainsKey(DotConstants.DirAttribute));
			e.AutoTune();
			Assert.True(e.Attributes.ContainsKey(DotConstants.DirAttribute));
			Assert.AreEqual(DirType.Both,e.Dir);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void LTailOrHeadEdgeCauseCompundGraph(bool head) {
			var g = new Graph();
			Assert.False(g.Attributes.ContainsKey(DotConstants.CompoundAttribute));
			var e = new Edge();
			if (head) {
				e.Lhead = "x";
			}
			else {
				e.Ltail = "x";
			}
			g.Edges.Add(e);
			Assert.False(g.Attributes.ContainsKey(DotConstants.CompoundAttribute));
			g.AutoTune();
			Assert.True(g.Attributes.ContainsKey(DotConstants.CompoundAttribute));
			Assert.True(g.Compound);
		}
		
        [Test]
        public void PushNodesToClusters() {
            var g = new Graph();
            var c = new SubGraph {Code = "cluster_0"};
            var n = new Node {Code = "x", SubgraphCode = "cluster_0"};
            g.Nodes.Add(n);
            g.SubGraphs.Add(c);
            g.AutoTune();
            Assert.True(c.Nodes.Contains(n));
            Assert.True(n.Parent==c);
            Console.WriteLine(GraphRender.Render(g));
        }

        [Test]
        public void AutoGenerateSubgraph()
        {
            var g = new Graph();
            var n = new Node { Code = "x", SubgraphCode = "cluster_0" };
            g.Nodes.Add(n);
            g.AutoTune();
            Assert.False(g.Nodes.Contains(n));
            Assert.True(n.Parent != g);
            Console.WriteLine(GraphRender.Render(g));
        }

        [Test]
        public void CorrectClusterCodes()
        {
            var g = new Graph();
            var n = new Node { Code = "x", SubgraphCode = "0" };
            var n2 = new Node { Code = "y", SubgraphCode = "cluster_0" };
            g.Nodes.Add(n);
            g.Nodes.Add(n2);
            g.AutoTune();
            Assert.False(g.Nodes.Contains(n));
            Assert.True(n.Parent != g);
            Assert.AreEqual("cluster_0",n.Parent.Code);
            Assert.AreEqual(n2.Parent.Code,n.Parent.Code);
            Console.WriteLine(GraphRender.Render(g));
        }
	}
}

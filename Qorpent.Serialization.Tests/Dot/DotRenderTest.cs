using System;
using NUnit.Framework;
using Qorpent.Graphs;
using Qorpent.Graphs.Dot;
using Qorpent.Graphs.Dot.Types;

namespace Qorpent.Serialization.Tests.Dot {
    [TestFixture]
    public class DotRenderTest
    {
        string render(Graph g,GraphOptions o=null) {
            var result = GraphRender.Create(g,o).GenerateGraphScript();
            Console.WriteLine(result);
            Console.WriteLine(result.Replace("\"","\"\""));
            return result;
        }
        [Test]
        public void EmptyGraph() {
            var g = new Graph {Code = "test"};
            var r = render(g);
            Assert.AreEqual(@"digraph test {
}
",r);
        }
        [Test]
        public void SimpleGraphNoNodes()
        {
            var g = new Graph { Code = "test" };
            g += Edge.Create("a", "b");            
            var r = render(g);
            Assert.AreEqual(@"digraph test {
	a -> b;
}
", r);

        }

      

      

        [Test]
        public void SimpleGraphNodes()
        {
            var g = new Graph { Code = "test" };
            g += Edge.Create("1a", "1b");
            g += Node.Create("1a");
            g += Node.Create("1b");
            var r = render(g);
            Assert.AreEqual(@"digraph test {
	_0x0031a [label=""1a"";];
	_0x0031b [label=""1b"";];
	_0x0031a -> _0x0031b;
}
", r);
        }

        [Test]
        public void ShapedAndColorizedGraph()
        {
            var g = new Graph { Code = "test" };
            Node na;
            Node nb;
            Edge e;
            g += (e=Edge.Create("a", "b"));
            g += (na=Node.Create("a"));
            g += (nb=Node.Create("b"));

            na.Shape = NodeShapeType.Box;
            na.Style = NodeStyleType.Bold | NodeStyleType.Dashed | NodeStyleType.Striped;
            na.FillColor = Color.Red + 0.3 + Color.Blue+0.2+Color.Green;

            nb.Shape= NodeShapeType.Mcircle;
            nb.Style = NodeStyleType.Filled;
            nb.FillColor = (ColorList)Color.Red + Color.Blue;


            e.ArrowHead = ArrowType.LCurve;
            e.ArrowHead+=ArrowType.ROBox;
            e.ArrowTail = ArrowType.OBox;
            
            var r = render(g);
            Assert.AreEqual(@"digraph test {
	a [label=a;shape=box;color=""bold,dashed,striped"";fillcolor=""red;0.3:blue;0.2:green"";];
	b [label=b;shape=Mcircle;color=filled;fillcolor=""red:blue"";];
	a -> b [arrowhead=lcurverobox;arrowtail=obox;dir=both;];
}
", r);
        }
    }
}
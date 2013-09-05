using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Dot;

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
		
	}
}

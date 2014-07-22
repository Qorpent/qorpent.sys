using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Model;
using Qorpent.Utils.Extensions;

namespace Qorpent.Core.Tests.Model
{
	/// <summary>
	/// Tests of <see cref="HierarchyExtensions"/>
	/// </summary>
	[TestFixture]
	public class HierarchyExtensionTest
	{
		private Th _root;
		private Th _child;
		private Th _child2;

		private  class Th : Hierarchy<Th>{};

		/// <summary>
		/// tests that can evaluate default path
		/// </summary>
		[Test]
		public void CanEvaluatePath() {
			var root = new Th {Code = "root"};
			var child = new Th {Code = "child", Parent = root};
			var child2 = new Th {Code = "child2", Parent = child};
			Assert.AreEqual("/root/",root.Path);
			Assert.AreEqual("/root/child/",child.Path);
			Assert.AreEqual("/root/child/child2/",child2.Path);
		}

		/// <summary>
		/// tests that can assign parent over hierarchy down
		/// </summary>
		[Test]
		public void CanNormalizeParent() {
			BuildSimpleHierarchy();
			_root.NormalizeParentInHierarchy();
			Assert.AreEqual(_child,_child2.Parent);
			Assert.AreEqual(_root, _child.Parent);
		}

		/// <summary>
		/// tests ability to enumerate down of hierarchy
		/// </summary>
		[Test]
		public void CanEnumerateAll() {
			BuildSimpleHierarchy();
			var all = _root.GetSelfAndDescendantsFromHierarchy().ToArray();
			Assert.AreEqual(3,all.Length);
		}
		[Test]
		public void CanEnumerateAllComplex() {
			BuildComplexHierarchy();
			var all = _root.GetAllHierarchy(true).ToArray();
			Assert.AreEqual(8, all.Length);
		}
		/// <summary>
		/// tests that cannot reassign invalidly setted parents
		/// </summary>
		[Test]
		public void CannotProceedBrokenHierarchyInNormalizationOnParentAlreadySeted() {
			BuildSimpleHierarchy();
			_child2.Parent = _root; //break of hierarchy
			Assert.Throws<Exception>(_root.NormalizeParentInHierarchy);
		}
		private void BuildComplexHierarchy() {
			_root = new Th {Code = "m111"};
			var c11 = new Th {Code = "m111_1"};
			var c12 = new Th {Code = "m111_2"};
			_root.Children.Add(c11);
			_root.Children.Add(c12);
			var c111 = new Th {Code = "m111_1_1"};
			var c112 = new Th {Code = "m111_1_2"};
			c11.Children.Add(c111);
			c11.Children.Add(c112);
			var c121 = new Th {Code = "m111_2_1"};
			c12.Children.Add(c121);
			var c1211 = new Th {Code = "m111_2_1_1"};
			c121.Children.Add(c1211);
			var c12111 = new Th {Code = "m111_2_1_1"};
			c1211.Children.Add(c12111);
		}
		private void BuildSimpleHierarchy() {
			_root = new Th {Code = "root"};
			_child = new Th {Code = "child"};
			_child2 = new Th {Code = "child2"};
			_root.Children.Add(_child);
			_child.Children.Add(_child2);
		}

		/// <summary>
		/// tests that cannot reassign invalidly setted parents
		/// </summary>
		[Test]
		public void CannotProceedBrokenHierarchyInNormalizationOnInvalidCode()
		{
			BuildSimpleHierarchy();
			_child2.ParentCode = "root";
			Assert.Throws<Exception>(_root.NormalizeParentInHierarchy);
		}

		/// <summary>
		/// tests that normalization can repare parent references if code matches
		/// </summary>
		[Test]
		public void CanReplacePreviousAssignedParentIfCodeMatches() {
			BuildSimpleHierarchy();
			_child2.Parent = new Th {Code = "child"};
			_root.NormalizeParentInHierarchy();
			Assert.AreEqual(_child,_child2.Parent);
		}

		/// <summary>
		/// Tests that can build hierarchy from scratch with no any conflicts in it (simple variant)
		/// </summary>
		[Test]
		public void CanBuildHierarchyFromScratchSimple() {
			var r1 = new Th {Code = "r1"};
			var r2 = new Th { Code = "r2" };
			var r3 = new Th { Code = "r3" };
			var c1 = new Th { Code = "c1",ParentCode = "r1"};
			var c2 = new Th { Code = "c2",ParentCode = "r2"};
			var c3 = new Th { Code = "c3" ,ParentCode = "r3"};
			var c11 = new Th { Code = "c11", ParentCode = "c1" };
			var c12 = new Th { Code = "c12", ParentCode = "c2" };
			var c13 = new Th { Code = "c13", ParentCode = "c3" };
			var roots = new[] {r1, r2, r3, c1, c2, c3, c11, c12, c13}.BuildHierarchy().ToArray();
			Assert.AreEqual(3,roots.Length);
			Assert.AreEqual(c1,c11.Parent);
			Assert.AreEqual(c2, c12.Parent);
			Assert.AreEqual(c3, c13.Parent);
			Assert.AreEqual("/r1/c1/c11/",c11.Path);
			Assert.AreEqual("/r2/c2/c12/", c12.Path);
			Assert.AreEqual("/r3/c3/c13/", c13.Path);
		}
		/// <summary>
		/// Tests that can build hierarchy from scratch with some merging issues
		/// </summary>
		[Test]
		public void CanBuildHierarchyFromScratchMerged()
		{
			var r1 = new Th { Code = "r1" };
			r1.Children.Add(new Th{Code="c1",Children = {new Th{Code="c4"}}}); //directly
			var c1 = new Th { Code = "c1", ParentCode = "r1" };
			var c11 = new Th { Code = "c11", ParentCode = "c1" };
			var roots = new[] { r1,  c1,  c11}.BuildHierarchy().ToArray();
			Assert.AreEqual(1,roots.Length);
			var allinh = roots.First().GetSelfAndDescendantsFromHierarchy().ToArray();
			Assert.AreEqual(4,allinh.Length);
			var c4 = allinh.First(_ => _.Code == "c4");
			Assert.AreEqual("/r1/c1/c4/",c4.Path);
			Assert.AreEqual("/r1/c1/c11/",c11.Path);
			Assert.AreSame(c11.Parent,c4.Parent);

		}
	}
}

using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Qorpent.IO.DirtyVersion;
using Qorpent.IO.DirtyVersion.Mapping;
using Qorpent.IO.DirtyVersion.Storage;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Tests.DirtyVersion {
	[TestFixture]
	public class MapperTest {
		private string dir;
		private Mapper mapper;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			dir = Path.Combine(Path.GetTempPath(), "MapperTest");
			if (Directory.Exists(dir))
			{
				try {
					Directory.Delete(dir, true);
				}
				catch {
					Thread.Sleep(200);
					Directory.Delete(dir, true);
				}
			}
			Directory.CreateDirectory(dir);
			mapper = new Mapper(dir,hashsize:7);
		}

		[Test]
		public void CanWriteInitial() {
			using (var o = mapper.Open("CanWriteInitial"))
			{
				o.Commit("a");
				o.Commit();
			}
			using (var o2 = mapper.Open("CanWriteInitial"))
			{
				var mi = o2.MappingInfo;
				var c = mi.Resolve("a");
				Assert.NotNull(c);
				Assert.AreEqual(mi.Head,c.Hash);
				Assert.AreEqual(CommitSourceType.Initial,c.SourceType);
			}

		}

		[Test]
		public void CanUpdate()
		{
			using (var o = mapper.Open("CanUpdate"))
			{
				var c1 = o.Commit("a");
				var c2 = o.Commit("b", "a");
				var c3 = o.Commit("c", "a");
				var c4 = o.Commit("d", "c");
				Assert.AreEqual(HeadState.Merged, c1.HeadState);
				Assert.AreEqual(HeadState.IsHead, c2.HeadState);
				Assert.AreEqual(HeadState.NonMerged, c3.HeadState);
				Assert.AreEqual(HeadState.NonMergedHead, c4.HeadState);
				var c5 = o.Commit("d", "b");
				Assert.AreEqual(HeadState.Merged, c1.HeadState);
				Assert.AreEqual(HeadState.Merged, c2.HeadState);
				Assert.AreEqual(HeadState.Merged, c3.HeadState);
				Assert.AreEqual(HeadState.IsHead, c4.HeadState);
				Assert.AreEqual(c5,c4);
				o.Commit();
			}
			using (var o = mapper.Open("CanUpdate")) {
				var c1 = o.MappingInfo.Resolve("a");
				var c2 = o.MappingInfo.Resolve("b");
				var c3 = o.MappingInfo.Resolve("c");
				var c4 = o.MappingInfo.Resolve("d");
				Assert.AreEqual(HeadState.Merged, c1.HeadState);
				Assert.AreEqual(HeadState.Merged, c2.HeadState);
				Assert.AreEqual(HeadState.Merged, c3.HeadState);
				Assert.AreEqual(HeadState.IsHead, c4.HeadState);
			}
		}

		[Test]
		public void CanUpdateDirectHead()
		{
			using (var o = mapper.Open("CanUpdateDirectHead"))
			{
				var c1 = o.Commit("a");
				var c2 = o.Commit("b", "a");
				var c3 = o.Commit("c",CommitHeadBehavior.Direct, "a");
				var c4 = o.Commit("d", "c");
				Assert.AreEqual(HeadState.Merged, c1.HeadState);
				Assert.AreEqual(HeadState.NonMergedHead, c2.HeadState);
				Assert.AreEqual(HeadState.Merged, c3.HeadState);
				Assert.AreEqual(HeadState.IsHead, c4.HeadState);
			}
		
		}

		[Test]
		public void CanUpdateOverrideHead()
		{
			using (var o = mapper.Open("CanUpdateOverrideHead"))
			{
				var c1 = o.Commit("a");
				var c2 = o.Commit("b", "a");
				var c3 = o.Commit("c", "a");
				var c4 = o.Commit("d", CommitHeadBehavior.Override, "c");
				Assert.AreEqual(HeadState.Merged, c1.HeadState);
				Assert.AreEqual(HeadState.Merged, c2.HeadState);
				Assert.AreEqual(HeadState.Merged, c3.HeadState);
				Assert.AreEqual(HeadState.IsHead, c4.HeadState);
				Assert.True(c4.Sources.Contains("b"));
			}
		}


		[Test]
		public void CanDeleteNoHead()
		{
			using (var o = mapper.Open("CanDeleteNoHead"))
			{
				var c1 = o.Commit("a");
				var c2 = o.Commit("b", "a");
				var c3 = o.Commit("c", "a");
				var c4 = o.Commit("d", "c");
				var c5 = o.Commit("d", "b");
				var c6 = o.Commit("e", "d");
				Assert.AreEqual(2, c5.Sources.Count);
				Assert.False(c5.Sources.Contains("a"));
				Assert.True(c5.Sources.Contains("b"));
				o.Delete("b");
				Assert.AreEqual(2, c5.Sources.Count);
				Assert.True(c5.Sources.Contains("a"));
				Assert.False(c5.Sources.Contains("b"));
				Assert.Null(o.MappingInfo.Resolve("b"));
				o.Commit();
			}
			using (var o = mapper.Open("CanDeleteNoHead"))
			{
				Assert.Null(o.MappingInfo.Resolve("b"));
				Assert.NotNull(o.MappingInfo.Resolve("e"));
			}
		}

		[Test]
		[ExpectedException]
		public void CanDenyHeadDeletion()
		{
			using (var o = mapper.Open("CanDenyHeadDeletion"))
			{
				var c1 = o.Commit("a");
				var c2 = o.Commit("b", "a");
				var c3 = o.Commit("c", "b");
				o.Delete(c3.Hash);
			}
			
		}

		[Test]
		public void CanDeleteHeadWithDetach()
		{
			using (var o = mapper.Open("CanUpdate"))
			{
				var c1 = o.Commit("a");
				var c2 = o.Commit("b", "a");
				var c3 = o.Commit("c", "b");
				o.Delete(c3.Hash,DeleteHeadBehavior.Detach);
				Assert.AreEqual(Const.DETACHEDHEAD,o.MappingInfo.Head);
			}

		}

		[Test]
		[ExpectedException]
		public void CanDenyHeadDeletionWithMerge()
		{
			using (var o = mapper.Open("CanDenyHeadDeletionWithMerge"))
			{
				var c1 = o.Commit("a");
				var c2 = o.Commit("b", "a");
				var c3 = o.Commit("c", "b");
				var c4 = o.Commit("d", "c", "a", "b");
				o.Delete(c4.Hash, DeleteHeadBehavior.AllowSingleDenyMerge);
				Assert.AreEqual(Const.DETACHEDHEAD, o.MappingInfo.Head);
			}

		}

		[Test]
		public void CanDeleteSingleWithPropogationAndDetachMerges()
		{
			using (var o = mapper.Open("CanDeleteSingleWithPropogationAndDetachMerges"))
			{
				var c1 = o.Commit("a");
				var c2 = o.Commit("b", "a");
				var c3 = o.Commit("c", "b");
				var c4 = o.Commit("d", "c", "a", "b");
				var c5 = o.Commit("e", "d");
				Assert.AreEqual(c5.Hash, o.MappingInfo.Head);
				o.Delete(c5.Hash, DeleteHeadBehavior.AllowSingleDetachMerge);
				Assert.AreEqual(c4.Hash, o.MappingInfo.Head);
				o.Delete(c4.Hash, DeleteHeadBehavior.AllowSingleDetachMerge);
				Assert.AreEqual(Const.DETACHEDHEAD, o.MappingInfo.Head);
			}

		}

	}
}
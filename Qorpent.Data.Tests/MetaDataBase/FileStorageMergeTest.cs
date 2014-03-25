using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Data.MetaDataBase;

namespace Qorpent.Data.Tests.MetaDataBase{
	[TestFixture]
	public class FileStorageMergeTest{
		[Test]
		public void SimpleMerge(){
			var i1 = new InMemoryMetaFileRegistry();
			var i2 = new InMemoryMetaFileRegistry();
			i1.Register(new MetaFileDescriptor{
				Code = "x",
				Content = "a",
				Revision = "1",
				RevisionTime = new DateTime(1979, 1, 1)
			});

			i2.Register(new MetaFileDescriptor
			{
				Code = "x",
				Content = "b",
				Revision = "2",
				RevisionTime = new DateTime(1980, 1, 1)
			});
			new MetaFileRegistryMerger().Merge(i1,i2);
			Assert.AreEqual("b",i1.GetCurrent("x").Content);
			Assert.AreEqual(2,i1.GetRevisions("x").Count());
		}

		[Test]
		public void SimpleMergeAddFile()
		{
			var i1 = new InMemoryMetaFileRegistry();
			var i2 = new InMemoryMetaFileRegistry();
			i1.Register(new MetaFileDescriptor
			{
				Code = "x",
				Content = "a",
				Revision = "1",
				RevisionTime = new DateTime(1979, 1, 1)
			});

			i2.Register(new MetaFileDescriptor
			{
				Code = "y",
				Content = "b",
				Revision = "2",
				RevisionTime = new DateTime(1980, 1, 1)
			});
			new MetaFileRegistryMerger().Merge(i1, i2);
			Assert.AreEqual("b", i1.GetCurrent("y").Content);
		}

		[Test]
		public void SimpleMergeIgnoreOlder()
		{
			var i1 = new InMemoryMetaFileRegistry();
			var i2 = new InMemoryMetaFileRegistry();
			i1.Register(new MetaFileDescriptor
			{
				Code = "x",
				Content = "a",
				Revision = "1",
				RevisionTime = new DateTime(1979, 1, 1)
			});

			i2.Register(new MetaFileDescriptor
			{
				Code = "y",
				Content = "b",
				Revision = "2",
				RevisionTime = new DateTime(1978, 1, 1)
			});
			new MetaFileRegistryMerger().Merge(i1, i2);
			Assert.AreEqual("a", i1.GetCurrent("x").Content);
			Assert.AreEqual(1, i1.GetRevisions("x").Count());
		}

		[Test]
		public void SimpleMergeIgnoreSameRevisions()
		{
			var i1 = new InMemoryMetaFileRegistry();
			var i2 = new InMemoryMetaFileRegistry();
			i1.Register(new MetaFileDescriptor
			{
				Code = "x",
				Content = "a",
				Revision = "1",
				RevisionTime = new DateTime(1979, 1, 1)
			});

			i2.Register(new MetaFileDescriptor
			{
				Code = "y",
				Content = "b",
				Revision = "1",
				RevisionTime = new DateTime(1980, 1, 1)
			});
			new MetaFileRegistryMerger().Merge(i1, i2);
			Assert.AreEqual("a", i1.GetCurrent("x").Content);
			Assert.AreEqual(1, i1.GetRevisions("x").Count());
		}


		[TestCase(MergeFlags.FullHistory,4)]
		[TestCase(MergeFlags.FullLateHistory,3)]
		[TestCase(MergeFlags.Snapshot,2)]
		public void StrategyMergeTest(MergeFlags flags, int expectedHistCount)
		{
			var i1 = new InMemoryMetaFileRegistry();
			var i2 = new InMemoryMetaFileRegistry();
			i1.Register(new MetaFileDescriptor
			{
				Code = "x",
				Content = "a",
				Revision = "1",
				RevisionTime = new DateTime(1979, 1, 1)
			});

			i2.Register(new MetaFileDescriptor
			{
				Code = "x",
				Content = "b",
				Revision = "2",
				RevisionTime = new DateTime(1978, 1, 1)
			});
			i2.Register(new MetaFileDescriptor
			{
				Code = "x",
				Content = "d",
				Revision = "4",
				RevisionTime = new DateTime(1981, 1, 1)
			});
			i2.Register(new MetaFileDescriptor
			{
				Code = "x",
				Content = "c",
				Revision = "3",
				RevisionTime = new DateTime(1980, 1, 1)
			});

			new MetaFileRegistryMerger().Merge(i1, i2, flags);
			Assert.AreEqual("d", i1.GetCurrent("x").Content);
			Assert.AreEqual(expectedHistCount, i1.GetRevisions("x").Count());
		}

	
	}
}
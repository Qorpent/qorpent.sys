using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
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
			var result = mapper.Write(new MapRecord {Name = "test1", NewDataHash = new Hasher(7).GetHash("hello!")});
			var xml = mapper.GetFullInfo(result.Name);
			Assert.AreEqual(result.NewDataHash, xml.Attr("head"));
			Assert.AreEqual("init", xml.Element(result.NewDataHash).Attr("src"));
		}

		[Test]
		public void CanUpdate()
		{
			var result = mapper.Write(new MapRecord { Name = "test", NewDataHash = new Hasher(7).GetHash("hello!") });
			var result2 = mapper.Write(new MapRecord { Name = "test", NewDataHash = new Hasher(7).GetHash("hello2!")
				,SourceDataHashes = new[]{result.NewDataHash}});
			
			Assert.AreEqual(result2.NewDataHash, mapper.GetFullInfo(result.Name).Attr("head"));
			var result3 = mapper.Write(new MapRecord
			{
				Name = "test",
				NewDataHash = new Hasher(7).GetHash("hello3!")
				,
				SourceDataHashes = new[] { result.NewDataHash }
			});
			Assert.AreEqual(result2.NewDataHash, mapper.GetFullInfo(result.Name).Attr("head"));
			var result4 = mapper.Write(new MapRecord
			{
				Name = "test",
				NewDataHash = new Hasher(7).GetHash("hello4!")
				,
				Commiter="fst",
				SourceDataHashes = new[] { result3.NewDataHash }
			});
			Assert.AreEqual(result2.NewDataHash, mapper.GetFullInfo(result.Name).Attr("head"));
			var result5 = mapper.Write(new MapRecord
			{
				Name = "test",
				NewDataHash = new Hasher(7).GetHash("hello4!")
				,
				Commiter = "sec",
				SourceDataHashes = new[] { result2.NewDataHash }
			});
			var xml = mapper.GetFullInfo(result.Name);
			Assert.AreEqual(result5.NewDataHash, xml.Attr("head"));
			Assert.AreEqual(result.NewDataHash, xml.Element(result2.NewDataHash).Attr("src"));
			Assert.True(result2.IsHead);
			Assert.False(result3.IsHead);
			Assert.False(result4.IsHead);
			Assert.True(result5.IsHead);
		}

		[Test]
		public void CanDelete()
		{
			var result = mapper.Write(new MapRecord { Name = "CanDelete", NewDataHash = new Hasher(7).GetHash("hello!") });
			var result2 = mapper.Write(new MapRecord
			{
				Name = "CanDelete",
				NewDataHash = new Hasher(7).GetHash("hello2!")
				,
				SourceDataHashes = new[] { result.NewDataHash }
			});
			var result3 = mapper.Write(new MapRecord
			{
				Name = "CanDelete",
				NewDataHash = new Hasher(7).GetHash("hello3!")
				,
				SourceDataHashes = new[] { result.NewDataHash }
			});
			mapper.Write(new MapRecord
			{
				Name = "CanDelete",
				NewDataHash = new Hasher(7).GetHash("hello4!")
				,
				Commiter = "fst",
				SourceDataHashes = new[] { result3.NewDataHash }
			});
			var result5 =mapper.Write(new MapRecord
			{
				Name = "CanDelete",
				NewDataHash = new Hasher(7).GetHash("hello4!")
				,
				Commiter = "sec",
				SourceDataHashes = new[] { result2.NewDataHash }
			});
			var xml = mapper.GetFullInfo(result.Name);
			var last = xml.Element(result5.NewDataHash);
			Assert.AreEqual("merged",last.Attr("src"));
			Assert.AreEqual(3,last.Elements().Count());
			Console.WriteLine(xml);
			mapper.Delete(result3);
			xml = mapper.GetFullInfo(result.Name);
			last = xml.Element(result5.NewDataHash);
			Assert.AreEqual(result2.NewDataHash, last.Attr("src"));
			Assert.AreEqual(1, last.Elements().Count());
			Console.WriteLine(xml);
		}


		[Test]
		public void CanFindNonMergedAndWorkWithHead()
		{
			var result = mapper.Write(new MapRecord { Name = "CanFindNonMerged", NewDataHash = new Hasher(7).GetHash("hello!") });
			var result2 = mapper.Write(new MapRecord
			{
				Name = "CanFindNonMerged",
				NewDataHash = new Hasher(7).GetHash("hello2!")
			});
			var result3 = mapper.Write(new MapRecord
			{
				Name = "CanFindNonMerged",
				NewDataHash = new Hasher(7).GetHash("hello3!")
				,
				SourceDataHashes = new[] { result2.NewDataHash }
			});
			var result4 = mapper.Write(new MapRecord
			{
				Name = "CanFindNonMerged",
				NewDataHash = new Hasher(7).GetHash("hello4!")
				,
				Commiter = "sec",
				SourceDataHashes = new[] { result3.NewDataHash }
			});
			var head = mapper.GetHead("CanFindNonMerged");
			Assert.AreEqual(result.NewDataHash, head);
			var nonmerged = mapper.GetNotMerged("CanFindNonMerged").ToArray();
			Assert.AreEqual(3,nonmerged.Length);
			nonmerged = mapper.GetNotMerged("CanFindNonMerged",true).ToArray();
			Assert.AreEqual(1, nonmerged.Length);
			Assert.AreEqual(result4.NewDataHash, nonmerged[0]);

			mapper.MoveHead(result3);
			head = mapper.GetHead("CanFindNonMerged");
			Assert.AreEqual(result3.NewDataHash, head);
			nonmerged = mapper.GetNotMerged("CanFindNonMerged", true).ToArray();
			Assert.AreEqual(2, nonmerged.Length);
			Assert.AreEqual(result.NewDataHash, nonmerged[0]);
			Assert.AreEqual(result4.NewDataHash, nonmerged[1]);

			mapper.MoveHead(result4);
			head = mapper.GetHead("CanFindNonMerged");
			Assert.AreEqual(result4.NewDataHash, head);
			nonmerged = mapper.GetNotMerged("CanFindNonMerged", true).ToArray();
			Assert.AreEqual(1, nonmerged.Length);
			Assert.AreEqual(result.NewDataHash, nonmerged[0]);
		}


	}
}
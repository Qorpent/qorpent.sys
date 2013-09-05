using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Qorpent.IO.DirtyVersion;
using Qorpent.IO.DirtyVersion.Mapping;

namespace Qorpent.IO.Tests.DirtyVersion {
	[TestFixture]
	public class VersionedStorageTest
	{
		private string dir;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			dir = Path.Combine(Path.GetTempPath(), "VersionedStorageTest");
			if (Directory.Exists(dir))
			{
				try
				{
					Directory.Delete(dir, true);
				}
				catch
				{
					Thread.Sleep(200);
					Directory.Delete(dir, true);
				}
			}
			Directory.CreateDirectory(dir);
		}

		[Test]
		public void CanSimplyWriteAndRead_Without_OverHeat() {
			
			var s = new DirtyVersionStorage(dir);
			const string FILENAME = "hello world!";
			const string CONTENT1 = "hello world!";
			const string CONTENT2 = "hello world - 222 !!!";

			//взяли и с любым именем сохранили нечто и спокойно прочитали
			var c1 = s.Save(FILENAME, CONTENT1);
			Assert.AreEqual(CONTENT1,s.ReadString(FILENAME));

			//взяли и сохранили новую версию
			var c2 = s.Save(FILENAME, CONTENT2);
			Assert.AreNotEqual(c1.Hash,c2.Hash);
			Assert.True(c2.HeadState == HeadState.IsHead);
			Assert.AreEqual(CONTENT2, s.ReadString(FILENAME));

			//но доступна и старая
			Assert.AreEqual(CONTENT1, s.ReadString(FILENAME,c1.Hash));

			//взяли и нативным апи пофиксили историю
			using (var o = s.GetMapper().Open(FILENAME))
			{
				o.MoveHead(c1.Hash);
				o.Commit();
			}
			Assert.AreEqual(CONTENT1, s.ReadString(FILENAME));
			Assert.AreEqual(CONTENT2, s.ReadString(FILENAME,c2.Hash));

			

			var c3  = s.Save(FILENAME, "new", c2.Hash);
			var c4 = s.Save(FILENAME, "new1", c2.Hash);
			var c5 = s.Save(FILENAME, "new2", c2.Hash);
			//взяли и нативным апи пофиксили историю
			using (var o = s.GetMapper().Open(FILENAME)) {
				o.Commit(c5.Hash, CommitHeadBehavior.Override, c1.Hash, c3.Hash);
				o.Commit();
			}
			//и это все еще можно получить и сконвертить в XML
			var xmlinfo = s.ExplainAsXml(FILENAME);
			Console.WriteLine(xmlinfo);
		}
	}
}
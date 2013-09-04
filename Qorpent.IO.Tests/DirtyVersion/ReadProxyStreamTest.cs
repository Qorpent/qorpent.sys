using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Qorpent.IO.DirtyVersion;
using Qorpent.IO.DirtyVersion.Storage;

namespace Qorpent.IO.Tests.DirtyVersion
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class CopyOnReadStreamTest
	{
		[Test]
		public void CanRedirectOnHashing() {
			var hasher= MD5.Create();
			var frommem = new MemoryStream(Encoding.ASCII.GetBytes("Hello!"));
			var tomem = new MemoryStream();
			var proxy = new CopyOnReadStream(frommem, tomem);
			hasher.ComputeHash(proxy);
			tomem.Position = 0;
			var res = new StreamReader(tomem).ReadToEnd();
			Assert.AreEqual("Hello!",res);
		}
	}

	[TestFixture]
	public class HashedDirectoryTest {
		private string dir;
		private HashedDirectory hashdir;

		[TestFixtureSetUp]
		public void FixtureSetup() {
			dir = Path.Combine(Path.GetTempPath(), "HashedDirectoryTest");
			if (Directory.Exists(dir)) {
				Directory.Delete(dir,true);
			}
			Directory.CreateDirectory(dir);
			hashdir = new HashedDirectory(dir);
		}

		[TestCase("very bad \\ name!!!","hello",true)]
		[TestCase("very bad \\ name!!!","hello",false)]
		[TestCase("/usual/long/path/name.txt","hello2",false)]
		[TestCase("/usual/long/path/name.txt","hello2",true)]
		public void CanWriteAndRead(string file,string content,bool compressed) {
			hashdir = new HashedDirectory(dir+"/compress_"+compressed,compressed);
			for (var i = 0; i <= 10; i++) {
				content = content + content;
			}
			var record = hashdir.Write(file,content);
			WriteDirToConsole();
			Assert.NotNull(record);
			Assert.AreNotEqual(record.NameHash,record.DataHash);
			Assert.AreEqual(Const.HashSize,record.NameHash.Length);
			Assert.AreEqual(Const.HashSize, record.DataHash.Length);
			Assert.AreEqual(DateTime.Now.Year,record.LastWriteTime.Year);

			var res = new StreamReader(hashdir.Open(record)).ReadToEnd();
			Assert.AreEqual(content,res);
			res = new StreamReader(hashdir.Open(file)).ReadToEnd();
			Assert.AreEqual(content, res);
		}
		[Test]
		public void CanFindLast() {
			var f1 = hashdir.Write("CanFindLast", "1");
			Thread.Sleep(200);
			var f2 = hashdir.Write("CanFindLast", "2");
			Thread.Sleep(200);
			var f3 = hashdir.Write("CanFindLast", "3");
			Thread.Sleep(200);
			var f4 = hashdir.Write("CanFindLast", "4");
			Thread.Sleep(200);
			var f5 = hashdir.Write("CanFindLast", "5");

			Assert.AreEqual(f1.DataHash,hashdir.FindLast(f2).DataHash);
			Assert.AreEqual(f2.DataHash,hashdir.FindLast(f3).DataHash);
			Assert.AreEqual(f3.DataHash,hashdir.FindLast(f4).DataHash);
			Assert.AreEqual(f4.DataHash,hashdir.FindLast(f5).DataHash);
			Assert.AreEqual(f5.DataHash, hashdir.FindLast("CanFindLast").DataHash);
		}
		[Test]
		public void CanFindFirst()
		{
			var f1 = hashdir.Write("CanFindLast", "1");
			Thread.Sleep(200);
			var f2 = hashdir.Write("CanFindLast", "2");
			Thread.Sleep(200);
			var f3 = hashdir.Write("CanFindLast", "3");
			Thread.Sleep(200);
			var f4 = hashdir.Write("CanFindLast", "4");
			Thread.Sleep(200);
			var f5 = hashdir.Write("CanFindLast", "5");

			Assert.AreEqual(f2.DataHash, hashdir.FindFirst(f1).DataHash);
			Assert.AreEqual(f3.DataHash, hashdir.FindFirst(f2).DataHash);
			Assert.AreEqual(f4.DataHash, hashdir.FindFirst(f3).DataHash);
			Assert.AreEqual(f5.DataHash, hashdir.FindFirst(f4).DataHash);
		
			Assert.AreEqual(f1.DataHash, hashdir.FindFirst("CanFindLast").DataHash);
		}


		private void WriteDirToConsole() {
			foreach (var e in Directory.GetFileSystemEntries(dir).OrderBy(_ => _)) {
				Console.WriteLine(e);
			}
		}
	}
}

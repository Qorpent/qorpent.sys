using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using NUnit.Framework;
using Qorpent.Utils.Git;
using Qorpent.Utils.IO;
namespace Qorpent.Utils.Tests.IO
{
	[TestFixture]
	public class FileDescriptorExTest
	{
		private string dir;
		private string file;

		[SetUp]
		public void Setup() {
			dir = FileSystemHelper.ResetTemporaryDirectory();
			file = Path.Combine(dir, "test.txt");
			File.WriteAllText(file,@"/*!
opts x=1 y=2
*/
data");
		}

		[Test]
		public void LocalFileTest() {
			var desc = new FileDescriptorEx {FullName = file};
			Assert.AreEqual(File.GetLastWriteTimeUtc(file),desc.Version);
			var hash = Convert.ToBase64String(MD5.Create().ComputeHash(File.ReadAllBytes(file)));
			Assert.AreEqual(hash,desc.Hash);
			Assert.False(desc.IsGitBased);
			Assert.Null(desc.CommitInfo);
		}

		[Test]
		public void GitFileTest()
		{
			var desc = new FileDescriptorEx { FullName = file };
			GitHelper.Init(dir);
			GitHelper.CommitAll(dir);
			var commit = GitHelper.GetCommit(dir);
			Assert.AreEqual(commit.GlobalRevisionTime, desc.Version);
		   Assert.AreEqual(commit.Hash, desc.Hash);
			Assert.True(desc.IsGitBased);
			Assert.NotNull(desc.CommitInfo);
		}

		[Test]
		public void ReadHeader() {
			var desc = new FileDescriptorEx { FullName = file };
			var header = desc.Header.ToString().Replace("\"", "'");
			Console.WriteLine(header);
			Assert.AreEqual(@"<opts _file='code.bxl' _line='2' x='1' y='2' />", header);
		}

	    [Test]
	    public void CanUseExplicitHash() {
	        File.WriteAllText(file,"--!options hash=XXX\r\ndata");
	        var desc = new FileDescriptorEx {FullName = file};
            Assert.AreEqual("XXX",desc.Hash);
	    }

		[Test]
		public void CanReadAssemblyVersionAndAttributes() {
			var assembly = Assembly.GetExecutingAssembly();
			var path = assembly.GetName().CodeBase.Replace("file:///", "");
			var desc = new FileDescriptorEx {FullName = path};
			Assert.True(desc.IsAssembly);
			Console.WriteLine(desc.Hash);
			Console.WriteLine(desc.Version);
			Console.WriteLine(desc.Header);
		}
	}
}

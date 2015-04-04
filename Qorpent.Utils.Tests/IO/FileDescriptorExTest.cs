using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using NUnit.Framework;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Git;
using Qorpent.Utils.IO;
namespace Qorpent.Utils.Tests.IO
{
	[TestFixture]
	public class FileDescriptorExTest
	{
		private string dir;
		private string file;
		private string file2;
	    private string file2_1;
	    private string dir2;

	    [SetUp]
		public void Setup() {
			dir = FileSystemHelper.ResetTemporaryDirectory();
            dir2 = FileSystemHelper.ResetTemporaryDirectory("FileDescriptorExTest2");
			file = Path.Combine(dir, "test.txt");
			file2_1 = Path.Combine(dir2, "test2.txt");
			file2 = Path.Combine(dir, "test2.txt");
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

	    [Test]
	    public void FileLevelGitHash() {
            var desc = new FileDescriptorEx { FullName = file};
            GitHelper.Init(dir);
            GitHelper.CommitAll(dir);
            desc.Refresh();
            Assert.AreEqual(desc.Hash, GitHelper.GetCommit(dir).Hash);
            File.WriteAllText(file2, "sample");
            GitHelper.CommitAll(dir);
            Assert.AreNotEqual(desc.Hash, GitHelper.GetCommit(dir).Hash);
            desc.Refresh();
            Assert.AreNotEqual(desc.Hash, GitHelper.GetCommit(dir).Hash);
	    }

	    [Test]
	    public void RepositoryLevelHash() {
            var desc = new FileDescriptorEx { FullName = file,UseRepositoryCommit = true};
            GitHelper.Init(dir);
            GitHelper.CommitAll(dir);
            desc.Refresh();
            Assert.AreEqual(desc.Hash,GitHelper.GetCommit(dir).Hash);
            File.WriteAllText(file2,"sample");
            GitHelper.CommitAll(dir);
            Assert.AreNotEqual(desc.Hash, GitHelper.GetCommit(dir).Hash);
            desc.Refresh();
            Assert.AreEqual(desc.Hash, GitHelper.GetCommit(dir).Hash);
	    }

	    [Test]
	    public void MultiRepositoryDependency() {
	        File.WriteAllText(file2_1,"test");
	        GitHelper.Init(dir);
	        GitHelper.CommitAll(dir);
            GitHelper.Init(dir2);
	        GitHelper.CommitAll(dir2);
	        var h1 = GitHelper.GetCommit(dir).Hash;
	        var h2 = GitHelper.GetCommit(dir2).Hash;
	        var h3 = GitHelper.GetCommit(EnvironmentInfo.ResolvePath("@repos@/qorpent.sys")).Hash;
            Assert.NotNull(h1);
            Assert.NotNull(h2);
            Assert.NotNull(h3);
	        var h = (h1 + h2 + h3).GetMd5();
            var desc = new FileDescriptorEx { FullName = file, RepositoryDependencies = new[] { "../FileDescriptorExTest2", "@repos@/qorpent.sys" } };
            Assert.AreEqual(h,desc.Hash);
	    }

        [Test]
        public void HeaderMultiRepositoryDependency()
        {
            File.WriteAllText(file, @"/*!
opts 
    repodependency ../FileDescriptorExTest2
    repodependency @repos@/qorpent.sys
*/
data");
            File.WriteAllText(file2_1, "test");
            GitHelper.Init(dir);
            GitHelper.CommitAll(dir);
            GitHelper.Init(dir2);
            GitHelper.CommitAll(dir2);
            var h1 = GitHelper.GetCommit(dir).Hash;
            var h2 = GitHelper.GetCommit(dir2).Hash;
            var h3 = GitHelper.GetCommit(EnvironmentInfo.ResolvePath("@repos@/qorpent.sys")).Hash;
            Assert.NotNull(h1);
            Assert.NotNull(h2);
            Assert.NotNull(h3);
            var h = (h1 + h2 + h3).GetMd5();
            var desc = new FileDescriptorEx { FullName = file };
            Assert.AreEqual(h, desc.Hash);
        }

        [Test]
        public void HeaderRepositoryLevelHash()
        {
            File.WriteAllText(file, @"/*!
opts userepositorycommit=true
*/
data");
            var desc = new FileDescriptorEx { FullName = file};
            GitHelper.Init(dir);
            GitHelper.CommitAll(dir);
            desc.Refresh();
            Assert.AreEqual(desc.Hash, GitHelper.GetCommit(dir).Hash);
            File.WriteAllText(file2, "sample");
            GitHelper.CommitAll(dir);
            Assert.AreNotEqual(desc.Hash, GitHelper.GetCommit(dir).Hash);
            desc.Refresh();
            Assert.AreEqual(desc.Hash, GitHelper.GetCommit(dir).Hash);
        }
	}
}

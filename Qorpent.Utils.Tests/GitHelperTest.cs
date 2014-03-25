using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Qorpent.Utils.Tests
{
	[TestFixture]
	public class GitHelperTest{
		private string dirname = "GitHelperTest";
		[SetUp]
		public void Setup(){
			FileSystemHelper.KillDirectory(dirname);
		}

		[Test]
		public void CanInitDir(){
			var githelper = new GitHelper{DirectoryName = dirname};
			githelper.Connect();
			Assert.True(Directory.Exists(dirname));
			Assert.True(Directory.Exists(dirname+"\\.git"));
		}


		[Test]
		public void CanInitDirWithQorpentSys()
		{
			var githelper = new GitHelper { DirectoryName = dirname,RemoteUrl = "g:/repos/qorpent.kernel"};
			githelper.Connect();
			Assert.True(Directory.Exists(dirname+"\\default-content"));
			
		}
		[Test]
		public void CanGetContent()
		{
			var githelper = new GitHelper { DirectoryName = dirname, RemoteUrl = "g:/repos/qorpent.kernel" };
			githelper.Connect();
			var content = githelper.GetContent("LICENSE");
			StringAssert.StartsWith("Copyright 2007-2014",content);
			content = githelper.GetContent("LICENSE", "fd4f64e");
			StringAssert.StartsWith("Copyright 2007-2012", content);
		}

		[Test]
		public void CanRestoreFile(){
			var githelper = new GitHelper{DirectoryName = dirname,AuthorName =Applications.Application.Current.Principal.CurrentUser.Identity.Name};
			githelper.Connect();
			var initial = githelper.GetCommitId();
			var currentFile = githelper.GetContent("x");
			Assert.Null(currentFile);
			githelper.WriteAndCommit("x","a","is a");
			var first = githelper.GetCommitId();
			Assert.AreNotEqual(initial,first);
			githelper.WriteAndCommit("x", "b", "is b");
			var second = githelper.GetCommitId();
			Assert.AreNotEqual(initial, second);
			var content = githelper.ReadFile("x");
			Assert.AreEqual("b",content);
			githelper.RestoreSingleFile("x",first);
			content = githelper.ReadFile("x");
			Assert.AreEqual("a",content);
		}

		[Test]
		public void CanGetFileList()
		{
			var githelper = new GitHelper { DirectoryName = dirname, AuthorName = Applications.Application.Current.Principal.CurrentUser.Identity.Name };
			githelper.Connect();
			var initial = githelper.GetCommitId();
			var initialList = githelper.GetFileList();
			Assert.False(initialList.Any(_=>_=="x"));
			githelper.WriteAndCommit("x", "a", "is a");
			var first = githelper.GetCommitId();
			Assert.True(githelper.GetFileList().Any(_ => _ == "x"));
			githelper.WriteAndCommit("x z", "b", "is b");
			Assert.True(githelper.GetFileList().Any(_ => _ == "x z"));
			Assert.False(githelper.GetFileList(first).Any(_ => _ == "x z"));
			Assert.True(githelper.GetFileList(first).Any(_ => _ == "x"));
			Assert.False(githelper.GetFileList(initial).Any(_ => _ == "x"));
		}

		[Test]
		public void CanGetFileListUnicode()
		{
			var githelper = new GitHelper { DirectoryName = dirname, AuthorName = Applications.Application.Current.Principal.CurrentUser.Identity.Name };
			githelper.Connect();
			var initial = githelper.GetCommitId();
			var initialList = githelper.GetFileList();
			Assert.False(initialList.Any(_ => _ == "абв"));
			githelper.WriteAndCommit("абв", "a", "is a");
			var first = githelper.GetCommitId();
			Assert.True(githelper.GetFileList().Any(_ => _ == "абв"));
			Assert.AreEqual("a", githelper.GetContent("абв"));
		}

		[Test]
		public void CanGetCommitInfo(){
			var now = DateTime.Now;
			var githelper = new GitHelper { DirectoryName = dirname, AuthorName = "test" };
			githelper.Connect();
			githelper.WriteAndCommit("x","a","message");
			var info = githelper.GetCommitInfo();
			Assert.AreEqual(githelper.AuthorName,info.Author);
			Assert.AreEqual(githelper.AuthorName+"@auto."+Environment.MachineName+".com",info.AuthorEmail);
			Assert.AreEqual(githelper.GetCommitId(),info.Hash);
			Assert.AreEqual(githelper.GetCommitId().Substring(0,7),info.ShortHash);
			Assert.AreEqual(info.GlobalRevisionTime.ToLocalTime(),info.LocalRevisionTime);
			Assert.AreEqual("message",info.Comment);
			var now2 = DateTime.Now;
			Assert.True(info.LocalRevisionTime>=now.AddSeconds(-1) && info.LocalRevisionTime<=now2.AddSeconds(1));
		}



		[Test]
		public void CanGetUnicodeCommitInfo()
		{
			var githelper = new GitHelper { DirectoryName = dirname, AuthorName = "test" };
			githelper.Connect();
			githelper.WriteAndCommit("x", "a", "it is сообщение");
			var info = githelper.GetCommitInfo();
			Assert.AreEqual("it is сообщение", info.Comment);
			
		}


		[Test]
		public void CanGetFileHistory(){
			var githelper = new GitHelper { DirectoryName = dirname, AuthorName = "test" }.Connect();
			githelper.WriteAndCommit("x", "a", "1");
			var first = githelper.GetCommitId();
			githelper.WriteAndCommit("x", "b", "2");
			var second = githelper.GetCommitId();
			var hist = githelper.GetHistory("x");
			Assert.AreEqual(2,hist.Length);
			Assert.AreEqual(second,hist[0].Hash);
			Assert.AreEqual(first,hist[1].Hash);
		}
	}
}

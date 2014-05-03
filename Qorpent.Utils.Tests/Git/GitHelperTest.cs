using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Qorpent.Utils.Git;

namespace Qorpent.Utils.Tests {
    [TestFixture]
	public class GitHelperTest{
		private string dirname = "GitHelperTest";
        /// <summary>
        ///     Установка фикстуры
        /// </summary>
        [TestFixtureSetUp]
        public void FixtureSetUp() {
            GitFuxtureInitializer.SetUpFixture();
        }
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
		public void CanReadTags(){
			var githelper = new GitHelper { DirectoryName = dirname }.Connect();
			githelper.WriteAndCommit("a", "b");
			githelper.SetTag("a");
			githelper.WriteAndCommit("c", "d");
			githelper.SetTag("b");
			githelper.WriteAndCommit("e", "f");
			githelper.SetTag("c");
			var tags = githelper.GetTags();
			Assert.AreEqual(3,tags.Length);
			CollectionAssert.AreEquivalent(tags.Select(_=>_.Name),new[]{"a","b","c"});
		}
		
	    [Test]
	    public void MergeCanBeTested(){
			var githelper = new GitHelper { DirectoryName = dirname }.Connect();
		    githelper.ExecuteCommand("branch", "x");
		    githelper.WriteAndCommit("x.txt", "xxx");
		    Assert.True(githelper.IsMerged("x", "HEAD"));
		    githelper.Checkout("x");
			githelper.WriteAndCommit("y.txt", "yyy");
		    githelper.Checkout(githelper.Branch);
			Assert.False(githelper.IsMerged("x", "HEAD"));
	    }

		[Test]
		public void MergeCanUseCache()
		{
			var githelper = new GitHelper { DirectoryName = dirname }.Connect();
			var c1 = githelper.GetCommitId();
			githelper.ExecuteCommand("branch", "x");
			githelper.WriteAndCommit("x.txt", "xxx");
			ConsoleApplicationHandler.Calls = 0;
			Assert.True(githelper.IsMerged(c1, "HEAD"));
			Assert.True(githelper.IsMerged(c1, "HEAD"));
			Assert.AreEqual(4,ConsoleApplicationHandler.Calls);
			ConsoleApplicationHandler.Calls = 0;
			Assert.True(githelper.IsMerged(c1, "HEAD",true));
			Assert.True(githelper.IsMerged(c1, "HEAD", true));
			Assert.AreEqual(2, ConsoleApplicationHandler.Calls);
		}


		[Test]
		[Explicit]
		public void CanInitDirWithQorpentSys()
		{
			var githelper = new GitHelper { DirectoryName = dirname,RemoteUrl = "g:/repos/qorpent.kernel"};
			githelper.Connect();
			Assert.True(Directory.Exists(dirname+"\\default-content"));
			
		}

		[Test]
		[Explicit]
		public void CanInitDirWithQorpentSysClone()
		{
			var githelper = new GitHelper { DirectoryName = dirname, AuthorName="comdiv",Password = "aqsw123e",RemoteUrl = "https://assoi-git.ugmk.com:8601/qorpent.kernel.git" };
			githelper.Connect();
			Assert.True(Directory.Exists(dirname + "\\default-content"));

		}

		[Test]
		public void CanCheckRemoteConnection()
		{
			var githelper = new GitHelper { RemoteUrl = "https://assoi-git.ugmk.com:8601/qorpent.kernel.git" };
			Assert.True(githelper.IsRemoteAccessible());
			githelper.RemoteUrl = "https://non-existed:9899/qorpent.kernel.git";
			Assert.False(githelper.IsRemoteAccessible());
		}


		[Test]
		public void CanGetChangedList()
		{
			var githelper = new GitHelper { DirectoryName = dirname };
			githelper.Connect();
			githelper.WriteFile("x","1");
			githelper.WriteFile("y","1");
			var changed = githelper.GetChangedFilesList();
			Assert.AreEqual(2,changed.Length);
			Assert.True(changed.Any(_=>_.FileName=="x"));
			Assert.True(changed.Any(_=>_.FileName=="y"));
			var ver1 = githelper.CommitAllChanges("1");
			changed = githelper.GetChangedFilesList();
			Assert.AreEqual(0, changed.Length);
			changed = githelper.GetChangedFilesList(toref:"HEAD");
			Assert.AreEqual(2, changed.Length);
			var ver2 = githelper.WriteAndCommit("x", "2", "2");
			changed = githelper.GetChangedFilesList(toref: "HEAD");
			Assert.AreEqual(1, changed.Length);
			var ver3 = githelper.WriteAndCommit("y", "2", "3");
			changed = githelper.GetChangedFilesList(toref: "HEAD");
			Assert.AreEqual(1, changed.Length);
			changed = githelper.GetChangedFilesList(fromref:ver1,toref: "HEAD");
			Assert.AreEqual(2, changed.Length);
		}

		[Test]
		[Explicit]
		public void CanGetContent() {
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
        [Explicit("На 144'ом валится, для TC помечаю эксплицитом")]
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
		public void CanGetDistance(){
			var githelper = new GitHelper { DirectoryName = dirname, AuthorName = "test" };
			githelper.Connect();
			var c1 = githelper.WriteAndCommit("x", "a", "message");
			var c2 = githelper.WriteAndCommit("x", "b", "message");
			var dist1 = githelper.GetDistance(c1, c2);
			Assert.AreEqual(0,dist1.Behind);
			Assert.AreEqual(1,dist1.Forward);
			Assert.True(dist1.IsForwardable);
			var dist2 = githelper.GetDistance(c2, c1);
			Assert.AreEqual(1, dist2.Behind);
			Assert.AreEqual(0, dist2.Forward);
			Assert.True(dist2.IsUpdateable);
			githelper.Checkout(c1);
			var c3 = githelper.WriteAndCommit("x", "c", "message");
			var dist3 = githelper.GetDistance(c2, c3);
			Assert.AreEqual(1, dist3.Behind);
			Assert.AreEqual(1, dist3.Forward);
			Assert.False(dist3.IsUpdateable);
			Assert.False(dist3.IsForwardable);
		}

		[Test]
		[Explicit]
		public void CanGetNullDistanceOnUnknown()
		{
			var githelper = new GitHelper { DirectoryName = dirname, AuthorName = "test" };
			githelper.Connect();
			
			var dist1 = githelper.GetDistance("x","y");
			Assert.Null(dist1);
		}

		[Test]
		public void CanGetNonExistedCommitAsNull()
		{
			var githelper = new GitHelper { DirectoryName = dirname, AuthorName = "test" };
			githelper.Connect();
			var info = githelper.GetCommitInfo("nonexisted");
			Assert.Null(info);
		}

		[Test]
		[Explicit]
		public void InvalidCommitMessage(){
			var gh = new GitHelper{DirectoryName = @"C:\z3projects\assoi\comdiv\work\local"}.Connect();
			var inf = gh.GetCommitInfo();
			Assert.AreEqual("Привет!",inf.Comment);
		}


		[Test]
		[Explicit("Fail on team city server cannot reproduce now")]
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

using NUnit.Framework;
using Qorpent.Data.MetaDataBase;
using Qorpent.Utils;

namespace Qorpent.Data.Tests.MetaDataBase{
	[TestFixture]
	public class GitBasedStorageTest{
		private string dirname;
		private GitHelper git;
		private GitBasedMetaFileRegistry gitmd;

		[SetUp]
		public void SetUp(){
			dirname = "GitBasedStorageTest";
			git = new GitHelper{DirectoryName = dirname, AuthorName = "test"};
			gitmd = new GitBasedMetaFileRegistry{DirectoryName = dirname, AuthorName = "test"};
		}

		[Test]
		public void CanWriteFiles(){
			var result = gitmd.Register(new MetaFileDescriptor{Code = "x", Comment = "1", Content = "a"});
			var result2 = gitmd.Register(new MetaFileDescriptor { Code = "x", Comment = "2", Content = "b" });
			Assert.AreNotEqual(result.Revision,result2.Revision);
			Assert.AreEqual("a",git.GetContent("x",result.Revision));
			Assert.AreEqual("b", git.GetContent("x", result2.Revision));
		}

		[Test]
		public void CanCheckout()
		{
			var result = gitmd.Register(new MetaFileDescriptor { Code = "x", Comment = "1", Content = "a" });
			var result2 = gitmd.Register(new MetaFileDescriptor { Code = "x", Comment = "2", Content = "b" });
			var restore = gitmd.Checkout(result.Code,result.Revision);
			Assert.AreEqual("a",restore.Content);
			Assert.AreEqual(restore.Revision,result.Revision);
			Assert.AreNotEqual(restore.Revision, result2.Revision);
			Assert.AreEqual("a",gitmd.GetCurrent("x").Content);
			Assert.AreNotEqual(result.Revision, git.GetCommitInfo().ShortHash);
		}
	}
}
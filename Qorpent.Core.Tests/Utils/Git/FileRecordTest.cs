using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Utils.Git;

namespace Qorpent.Utils.Tests.Git
{
	[TestFixture]
	public class FileRecordTest
	{
		[Test]
		public void CanParseSimple(){
			var result = new FileRecord("?? x");
			Assert.AreEqual(FileState.Untracked,result.FirstState);
			Assert.AreEqual(FileState.Untracked,result.SecondState);
			Assert.AreEqual("x",result.FileName);
			Assert.AreEqual("",result.NewFileName);
			Assert.False(result.IsConflict);
		}

		[Test]
		public void CanParseHaskel(){
			var result = new FileRecord("A  \"a\\320\\247 - b\"");
			Assert.AreEqual(FileState.Added,result.FirstState);
			Assert.AreEqual(FileState.NotModified,result.SecondState);
			Assert.AreEqual("aЧ - b",result.FileName);
			Assert.AreEqual("",result.NewFileName);
			Assert.False(result.IsConflict);
		}

		[Test]
		public void NewModifiedFile()
		{
			var result = new FileRecord(" M test2");
			Assert.AreEqual(FileState.NotModified, result.FirstState);
			Assert.AreEqual(FileState.Modified, result.SecondState);
			Assert.AreEqual("test2", result.FileName);
			
		}

		[Test]
		public void CanParseWithWS(){
			var result = new FileRecord("?? a b");
			Assert.AreEqual(FileState.Untracked, result.FirstState);
			Assert.AreEqual(FileState.Untracked, result.SecondState);
			Assert.AreEqual("a b", result.FileName);
			Assert.AreEqual("", result.NewFileName);
			Assert.False(result.IsConflict);

			result = new FileRecord("A  \"a b\"");
			Assert.AreEqual(FileState.Added, result.FirstState);
			Assert.AreEqual(FileState.NotModified, result.SecondState);
			Assert.AreEqual("a b", result.FileName);
			Assert.AreEqual("", result.NewFileName);
			Assert.False(result.IsConflict);
		}

		[Test]
		public void CanParseComplex(){
			var result = new FileRecord("DU  \"a\\320\\247 - b\" -> \"a\\320\\247 - b2\"");
			Assert.AreEqual(FileState.Deleted, result.FirstState);
			Assert.AreEqual(FileState.Updated, result.SecondState);
			Assert.AreEqual("aЧ - b", result.FileName);
			Assert.AreEqual("aЧ - b2", result.NewFileName);
			Assert.True(result.IsConflict);
		}
	}
}

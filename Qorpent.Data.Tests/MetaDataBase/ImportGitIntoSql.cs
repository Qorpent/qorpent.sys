using NUnit.Framework;
using Qorpent.Data.MetaDataBase;
using Qorpent.Utils;

namespace Qorpent.Data.Tests.MetaDataBase{
	[TestFixture]
	public class ImportGitIntoSql{
		private const string code = "ImportGitIntoSql";
		private const string connection =
			"Data Source=(local);Initial Catalog=z3;Integrated Security=True;Application Name=ecohome";



		[Test]
		[Explicit]
		public void ImportQorpentKernelToSql2(){
			FileSystemHelper.KillDirectory(code);
			var sqlmd = new SqlBasedMetaFileRegistry { ConnectionString = connection };
			var giter = new GitBasedMetaFileRegistry{DirectoryName = code, RemoteUrl = "g:/repos/qorpent.sys"};
			new MetaFileRegistryMerger{ExcludeRegex=@"\.dll$",Debug = true}.Merge(sqlmd,giter);
		}

		[Test]
		[Explicit]
		public void ImportQorpentKernelToSql2Snapshot()
		{
			FileSystemHelper.KillDirectory(code);
			var sqlmd = new SqlBasedMetaFileRegistry { ConnectionString = connection };
			var giter = new GitBasedMetaFileRegistry { DirectoryName = code, RemoteUrl = "g:/repos/qorpent.sys" };
			new MetaFileRegistryMerger { ExcludeRegex = @"\.dll$"}.Merge(sqlmd, giter,MergeFlags.Snapshot);
		}
	}
}
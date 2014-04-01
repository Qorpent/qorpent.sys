using System;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using Qorpent.Data.MetaDataBase;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Git;

namespace Qorpent.Data.Tests.MetaDataBase{
	[TestFixture]
	public class MetaDataSyncServiceRealDbTest{
		private const string dirname = "MetaDataSyncServiceRealDbTest";
		const string connection = "Data Source=(local);Initial Catalog=z3;Integrated Security=True;Application Name=ecohome";
		[SetUp]
		public void Setup(){
			InDbNQ("create schema test");
			InDbNQ("drop table test.mds2");
			InDbNQ("drop table test.mds");
			InDbNQ("drop sequence test.mds_SEQ");
			InDbNQ("drop sequence test.mds_SEQ2");
			InDbNQ("create sequence test.mds_SEQ start with 10",true);
			InDbNQ("create sequence test.mds_SEQ2 start with 10",true);
			InDbNQ(
				@"create table test.mds (id int not null primary key default (next value for test.mds_SEQ), code nvarchar(255) not null default '', name nvarchar(255) not null default '', tag nvarchar(255) not null default '',
parent int not null foreign key references test.mds (id) default 0, active bit not null default 1, version datetime not null default (CURRENT_TIMESTAMP))",true);
			InDbNQ(
				@"insert test.mds (Id,Code) values (0,'/')",true);
			InDbNQ(
				@"create table test.mds2 (id int not null primary key default (next value for test.mds_SEQ2), code nvarchar(255) not null default '', name nvarchar(255) not null default '', tag nvarchar(255) not null default '',
parent int not null foreign key references test.mds (id) default 0, mds int not null foreign key references test.mds (id) default 0, active bit not null default 1, version datetime not null  default (CURRENT_TIMESTAMP))", true);
			InDbNQ(
				@"insert test.mds2 (Id,Code) values (0,'/')",true);
		}

		[Test]
		public void SetupWell(){
			InDbNQ("select * from test.mds select * from test.mds2", true);
		}

		bool InDbNQ(string query, bool throwError =false){
			return InDb(c =>{
				try{
					c.ExecuteNonQuery(query);
					return true;
				}
				catch{
					if (throwError) throw;
					return false;
				}
				
			});
		}

		T InDb<T>(Func<IDbConnection, T> func){
			using (var c = new SqlConnection(connection)){
				c.Open();
				return func(c);
			}
		}

		[Test]
		public void NativeGitNewModelTest(){
			var gh = new GitHelper{DirectoryName = dirname}.Connect();
			gh.WriteAndCommit("mdf1", "test.mds x y");
			gh.WriteAndCommit("mdf2", "test.mds a b");
			var sync = new MetaDataSyncService{SqlConnection = connection,IncludeRegex = "mdf"};
			sync.TargetStorage = new InMemoryMetaFileRegistry();
			sync.SourceStorage = new GitBasedMetaFileRegistry{DirectoryName = dirname,};
			sync.Synchronize();
			var cnt = InDb(c => c.ExecuteScalar<int>("select count(id) from test.mds where id!=0"));
			Assert.AreEqual(2,cnt);
		}


		[Test]
		public void UpgradeTest()
		{
			var gh = new GitHelper { DirectoryName = dirname }.Connect();
			gh.WriteAndCommit("mdf1", "test.mds x y");
			gh.WriteAndCommit("mdf2", "test.mds a b");
			var sync = new MetaDataSyncService { SqlConnection = connection, IncludeRegex = "mdf" };
			sync.TargetStorage = new InMemoryMetaFileRegistry();
			sync.SourceStorage = new GitBasedMetaFileRegistry { DirectoryName = dirname, };
			sync.Synchronize();
			var cnt = InDb(c => c.ExecuteScalar<int>("select count(id) from test.mds where id!=0"));
			Assert.AreEqual(2, cnt);
			gh.WriteAndCommit("mdf1", "test.mds x y1");
			gh.WriteAndCommit("mdf2", "test.mds a b1");
			sync.Synchronize();
			Console.WriteLine(sync.LastSql);
			cnt = InDb(c => c.ExecuteScalar<int>("select count(id) from test.mds where name in('y1','b1')"));
			Assert.AreEqual(2, cnt);
		}


	}
}
using System;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Scaffolding.Model.SqlWriters;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm
{
	[TestFixture]
	public class WritersTest
	{
		[TestCase("test", ScriptMode.Create, SqlDialect.SqlServer, @"if (SCHEMA_ID('test') is null) exec sp_executesql N'CREATE SCHEMA test';")]
		[TestCase("test", ScriptMode.Create, SqlDialect.PostGres, @"CREATE SCHEMA IF NOT EXISTS test;")]
		[TestCase("test", ScriptMode.Drop, SqlDialect.SqlServer, @"DROP SCHEMA test;")]
		[TestCase("test", ScriptMode.Drop, SqlDialect.PostGres, @"DROP SCHEMA test;")]
		public void SchemaWriter(string schemaname, ScriptMode mode, SqlDialect dialect, string test){
			var schema = new Schema{Name = schemaname};
			var writer = new SchemaWriter(schema){Mode = mode, Dialect = dialect,NoComment=true,NoDelimiter=true};
			Assert.AreEqual(test,writer.ToString().Trim());
		}

		[TestCase("test", 2, 20, false, true,"exec __ensurefg @n='TEST', @filecount=2, @filesize=20, @withidx=0, @isdefault=1")]
		[TestCase("test", 2, 20, false, false,"exec __ensurefg @n='TEST', @filecount=2, @filesize=20, @withidx=0, @isdefault=0")]
		[TestCase("test", 2, 20, true, true,"exec __ensurefg @n='TEST', @filecount=2, @filesize=20, @withidx=1, @isdefault=1")]
		[TestCase("test", 2, 20, true, false,"exec __ensurefg @n='TEST', @filecount=2, @filesize=20, @withidx=1, @isdefault=0")]
		public void FileGroupWriter(string name, int count,int size, bool withidx, bool isdefault,string test){
			var fg = new FileGroup{Name = name, FileCount = count, FileSize = size, WithIndex = withidx, IsDefault = isdefault};
			var writer = new FileGroupWriter(fg){NoComment = true, NoDelimiter = true,Mode = ScriptMode.Create,Dialect = SqlDialect.SqlServer};
			Assert.AreEqual(test,writer.ToString().Trim());
		}

		[TestCase("x", 5, 20, SqlDialect.SqlServer, ScriptMode.Create, "CREATE SEQUENCE dbo.x_SEQ AS int START WITH 0 INCREMENT BY 10;")]
		[TestCase("x", 5, 20, SqlDialect.SqlServer, ScriptMode.Drop, "DROP SEQUENCE dbo.x_SEQ;")]
		[TestCase("x", 5, 20, SqlDialect.PostGres, ScriptMode.Create, "CREATE SEQUENCE dbo.x_SEQ INCREMENT BY 10 START WITH 0;")]
		[TestCase("x", 5, 20, SqlDialect.PostGres, ScriptMode.Drop, "DROP SEQUENCE dbo.x_SEQ;")]
		public void SequenceWriter(string name, int start, int step, SqlDialect dialect,ScriptMode mode, string test){
			var pt = new PersistentClass{Name = name, Schema = "dbo"};
			var sq = new Sequence();
			sq.Setup(null,pt,null,null);
			var wr = new SequenceWriter(sq) { NoComment = true, NoDelimiter = true, Mode = mode, Dialect = dialect }; ;
			Assert.AreEqual(test,wr.ToString().Trim());
		}

		private const string SimpleModel = @"
class table prototype=dbtable abstract
table master
	string Code 'Код' unique idx=20
table slave
	datetime Version idx=30
	ref master idx=40
";
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void TableWriterWithPkUniqueAndForeignKeySqlServer(){
			var model = PersistentModel.Compile(SimpleModel);
			var master = model["master"];
			var mwr = new TableWriter(master){
				Dialect = SqlDialect.SqlServer,
				NoDelimiter = true,
				NoComment = true,
				Mode = ScriptMode.Create
			};
			var scr = mwr.ToString();
			Console.WriteLine(scr);
			Assert.AreEqual(@"CREATE TABLE dbo.master (
	Id int NOT NULL CONSTRAINT dbo_master_Id_PK PRIMARY KEY DEFAULT (NEXT VALUE FOR dbo.master_SEQ),
	Code nvarchar(255) NOT NULL CONSTRAINT dbo_master_Code_UNQ UNIQUE DEFAULT ''
) ON SECONDARY
".Trim(), scr.Trim());
			var slave = model["slave"];
			var swr = new TableWriter(slave)
			{
				Dialect = SqlDialect.SqlServer,
				NoDelimiter = true,
				NoComment = true,
				Mode = ScriptMode.Create
			};
			scr = swr.ToString();
			Console.WriteLine(scr);
			Assert.AreEqual(@"CREATE TABLE dbo.slave (
	Id int NOT NULL CONSTRAINT dbo_slave_Id_PK PRIMARY KEY DEFAULT (NEXT VALUE FOR dbo.slave_SEQ),
	Version datetime NOT NULL DEFAULT 0,
	master int NOT NULL CONSTRAINT dbo_slave_master_master_Id_FK FOREIGN KEY REFERENCES dbo.master (Id) DEFAULT 0
) ON SECONDARY".Trim(), scr.Trim());
		}
	}
}

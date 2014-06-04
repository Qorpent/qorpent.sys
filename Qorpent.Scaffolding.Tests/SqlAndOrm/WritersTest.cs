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

		[TestCase("x", 5, 20, SqlDialect.SqlServer, ScriptMode.Create, "CREATE SEQUENCE dbo.x_SEQ AS int START WITH 10 INCREMENT BY 10;")]
		[TestCase("x", 5, 20, SqlDialect.SqlServer, ScriptMode.Drop, "DROP SEQUENCE dbo.x_SEQ;")]
		[TestCase("x", 5, 20, SqlDialect.PostGres, ScriptMode.Create, "CREATE SEQUENCE dbo.x_SEQ INCREMENT BY 10 START WITH 10;")]
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
	datetime Version  'Версия'  idx=30
	ref master 'Главный объект' idx=40
";

		private const string CircularModel = @"
class table prototype=dbtable abstract
table master
	string Code 'Код' unique idx=20
	ref slave  'Младший объект' idx=30
table slave
	datetime Version 'Версия' idx=30
	ref master 'Главный объект' idx=40
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
	Code nvarchar(255) NOT NULL CONSTRAINT dbo_master_Code_UNQ UNIQUE DEFAULT '',
	Id int NOT NULL CONSTRAINT dbo_master_Id_PK PRIMARY KEY DEFAULT (NEXT VALUE FOR dbo.master_SEQ)
) ON SECONDARY;
EXECUTE sp_addextendedproperty N'MS_Description', 'Код', N'SCHEMA', N'dbo', N'TABLE', N'master', N'COLUMN', 'Code';

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
	master int NOT NULL CONSTRAINT dbo_slave_master_master_Id_FK FOREIGN KEY REFERENCES dbo.master (Id) DEFAULT 0,
	Version datetime NOT NULL DEFAULT 0,
	Id int NOT NULL CONSTRAINT dbo_slave_Id_PK PRIMARY KEY DEFAULT (NEXT VALUE FOR dbo.slave_SEQ)
) ON SECONDARY;
EXECUTE sp_addextendedproperty N'MS_Description', 'Главный объект', N'SCHEMA', N'dbo', N'TABLE', N'slave', N'COLUMN', 'master';
EXECUTE sp_addextendedproperty N'MS_Description', 'Версия', N'SCHEMA', N'dbo', N'TABLE', N'slave', N'COLUMN', 'Version';".Trim(), scr.Trim());
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void TableWriterWithPkUniqueAndForeignKeyPostgresql()
		{
			var model = PersistentModel.Compile(SimpleModel);
			var master = model["master"];
			var mwr = new TableWriter(master)
			{
				Dialect = SqlDialect.PostGres,
				NoDelimiter = true,
				NoComment = true,
				Mode = ScriptMode.Create
			};
			var scr = mwr.ToString();
			Console.WriteLine(scr);
			Assert.AreEqual(@"CREATE TABLE dbo.master (
	Code varchar(255) NOT NULL CONSTRAINT dbo_master_Code_UNQ UNIQUE DEFAULT '',
	Id int NOT NULL CONSTRAINT dbo_master_Id_PK PRIMARY KEY DEFAULT (nextval('dbo.master_SEQ'))
) TABLESPACE SECONDARY;
COMMENT ON COLUMN dbo.master.Code IS 'Код';
".Trim(), scr.Trim());
			var slave = model["slave"];
			var swr = new TableWriter(slave)
			{
				Dialect = SqlDialect.PostGres,
				NoDelimiter = true,
				NoComment = true,
				Mode = ScriptMode.Create
			};
			scr = swr.ToString();
			Console.WriteLine(scr);
			Assert.AreEqual(@"CREATE TABLE dbo.slave (
	master int NOT NULL CONSTRAINT dbo_slave_master_master_Id_FK FOREIGN KEY REFERENCES dbo.master (Id) DEFERRABLE DEFAULT 0,
	Version datetime NOT NULL DEFAULT 0,
	Id int NOT NULL CONSTRAINT dbo_slave_Id_PK PRIMARY KEY DEFAULT (nextval('dbo.slave_SEQ'))
) TABLESPACE SECONDARY;
COMMENT ON COLUMN dbo.slave.master IS 'Главный объект';
COMMENT ON COLUMN dbo.slave.Version IS 'Версия';".Trim(), scr.Trim());
		}


		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void CircularPreventFkes()
		{
			var model = PersistentModel.Compile(CircularModel);
			var master = model["master"];
			var mwr = new TableWriter(master)
			{
				Dialect = SqlDialect.SqlServer,
				NoDelimiter = true,
				NoComment = true,
				Mode = ScriptMode.Create
			};
			var scr = mwr.ToString();
			Console.WriteLine(scr);
			Assert.AreEqual(@"CREATE TABLE dbo.master (
	slave int NOT NULL DEFAULT 0,
	Code nvarchar(255) NOT NULL CONSTRAINT dbo_master_Code_UNQ UNIQUE DEFAULT '',
	Id int NOT NULL CONSTRAINT dbo_master_Id_PK PRIMARY KEY DEFAULT (NEXT VALUE FOR dbo.master_SEQ)
) ON SECONDARY;
EXECUTE sp_addextendedproperty N'MS_Description', 'Младший объект', N'SCHEMA', N'dbo', N'TABLE', N'master', N'COLUMN', 'slave';
EXECUTE sp_addextendedproperty N'MS_Description', 'Код', N'SCHEMA', N'dbo', N'TABLE', N'master', N'COLUMN', 'Code';
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
	master int NOT NULL DEFAULT 0,
	Version datetime NOT NULL DEFAULT 0,
	Id int NOT NULL CONSTRAINT dbo_slave_Id_PK PRIMARY KEY DEFAULT (NEXT VALUE FOR dbo.slave_SEQ)
) ON SECONDARY;
EXECUTE sp_addextendedproperty N'MS_Description', 'Главный объект', N'SCHEMA', N'dbo', N'TABLE', N'slave', N'COLUMN', 'master';
EXECUTE sp_addextendedproperty N'MS_Description', 'Версия', N'SCHEMA', N'dbo', N'TABLE', N'slave', N'COLUMN', 'Version';".Trim(), scr.Trim());
		}

		[TestCase(SqlDialect.SqlServer, ScriptMode.Create, "ALTER TABLE dbo.slave ADD CONSTRAINT dbo_slave_master_master_Id_FK FOREIGN KEY REFERENCES dbo.master (Id);")]
		[TestCase(SqlDialect.PostGres, ScriptMode.Create, "ALTER TABLE dbo.slave ADD CONSTRAINT dbo_slave_master_master_Id_FK FOREIGN KEY REFERENCES dbo.master (Id) DEFERABLE;")]
		[TestCase(SqlDialect.SqlServer, ScriptMode.Drop, "ALTER TABLE dbo.slave DROP CONSTRAINT dbo_slave_master_master_Id_FK;")]
		public void LateFKGenerator(SqlDialect dialect, ScriptMode mode,string test){
			var model = PersistentModel.Compile(CircularModel);
			var cref = model["slave"]["master"];
			var crefwr = new LateForeignKeyWriter(cref){NoDelimiter = true, NoComment = true, Dialect = dialect,Mode = mode};
			var result = crefwr.ToString().Trim();
			Console.WriteLine(result);
			Assert.AreEqual(test, result);
		}

		[Test]
		public void TriggerTest(){
			var trigger = new SqlTrigger{Insert = true, TableName = "dbo.x", Name = "x_insert", Body="print 1;"};
			var writer = new SqlTriggerWriter(trigger){Dialect = SqlDialect.SqlServer,Mode = ScriptMode.Create,NoComment = true};
			var res = writer.ToString();
			Console.WriteLine(res);
			Assert.AreEqual(@"IF OBJECT_ID('dbo.x_insert') IS NOT NULL DROP TRIGGER dbo.x_insert;
GO
CREATE TRIGGER dbo.x_insert ON dbo.x FOR INSERT AS BEGIN
print 1;
END;
GO".Trim(), writer.ToString().Trim());

			trigger.Update = true;
			trigger.Delete = true;
			trigger.Before = true;
			writer = new SqlTriggerWriter(trigger) { Dialect = SqlDialect.SqlServer, Mode = ScriptMode.Create, NoComment = true };
			res = writer.ToString();
			Console.WriteLine(res);
			Assert.AreEqual(@"IF OBJECT_ID('dbo.x_insert') IS NOT NULL DROP TRIGGER dbo.x_insert;
GO
CREATE TRIGGER dbo.x_insert ON dbo.x INSTEAD OF INSERT,UPDATE,DELETE AS BEGIN
print 1;
END;
GO".Trim(), writer.ToString().Trim());
		}
	}
}

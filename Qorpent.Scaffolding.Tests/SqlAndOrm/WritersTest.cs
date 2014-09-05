using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Scaffolding.Model.SqlWriters;
using Qorpent.Serialization;

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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="schemaname"></param>
		/// <param name="mode"></param>
		/// <param name="dialect"></param>
		/// <param name="test"></param>
		[Test]
		public void SchemaWriterWithComments_COVER_PROPOSE()
		{
			var schema = new Schema { Name = "x"};
			var writer = new SchemaWriter(schema) { Mode = ScriptMode.Create, Dialect = SqlDialect.SqlServer, Comment = "Multi line\r\ncomment" };
			var result = writer.ToString().Trim();
			var dig = writer.GetDigest().Trim();
			Console.WriteLine(result);
			Assert.AreEqual(@"-- begin command SchemaWriter
-- Multi line
-- comment
if (SCHEMA_ID('x') is null) exec sp_executesql N'CREATE SCHEMA x';
GO", result);
			Assert.AreEqual(@"Schema x (C,S,R, Multi line; comment)", dig);
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

		[TestCase("x", 5, 20, SqlDialect.SqlServer, ScriptMode.Create, "CREATE SEQUENCE \"dbo\".\"x_seq\" AS int START WITH 10 INCREMENT BY 10;")]
		[TestCase("x", 5, 20, SqlDialect.SqlServer, ScriptMode.Drop, "DROP SEQUENCE \"dbo\".\"x_seq\";")]
		[TestCase("x", 5, 20, SqlDialect.PostGres, ScriptMode.Create, "CREATE SEQUENCE \"dbo\".\"x_seq\" INCREMENT BY 10 START WITH 10;")]
		[TestCase("x", 5, 20, SqlDialect.PostGres, ScriptMode.Drop, "DROP SEQUENCE \"dbo\".\"x_seq\";")]
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
table Master
	string Code 'Код' unique idx=20
	ref Slave  'Младший объект' idx=30
table Slave
	datetime Version 'Версия' idx=30
	ref master 'Главный объект' idx=40
";

		[TestCase("order")]
		[TestCase("index")]
		public void CanGenerateTableWithSystemName(string name){
			var code = @"
class " + name + @" prototype=dbtable
	ref " + name + @"
";
			var model = PersistentModel.Compile(code);
			var t = model[name];
			var mwr = new TableWriter(t)
			{
				Dialect = SqlDialect.SqlServer,
				NoDelimiter = true,
				NoComment = true,
				Mode = ScriptMode.Create
			};
			var scr = mwr.ToString();
			Console.WriteLine(scr);
			//Console.WriteLine("------------------  for copy-paste --------------------------");
			//Console.WriteLine(scr.Replace("\"","\"\"").Replace("\""+name+"\"","\"{0}\"").Replace("_"+name+"_","_{0}_"));
			Assert.AreEqual(string.Format(@"CREATE TABLE ""dbo"".""{0}"" (
	""id"" int NOT NULL CONSTRAINT dbo_{0}_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""{0}_seq""),
	""{0}"" int NOT NULL CONSTRAINT dbo_{0}_{0}_{0}_id_fk FOREIGN KEY REFERENCES ""dbo"".""{0}"" (""id"") DEFAULT 0
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""{0}"" where ""id""=0)  INSERT INTO ""dbo"".""{0}"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""{0}"" where ""id""=-1)  INSERT INTO ""dbo"".""{0}"" (""id"", ""{0}"") VALUES (-1, -1);
".Trim(),name), scr.Trim());
		}
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
			Console.WriteLine(scr.Replace("\"", "\"\""));
			Assert.AreEqual(@"CREATE TABLE ""dbo"".""master"" (
	""id"" int NOT NULL CONSTRAINT dbo_master_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""master_seq""),
	""code"" nvarchar(255) NOT NULL CONSTRAINT dbo_master_code_unq UNIQUE DEFAULT ''
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""master"" where ""id""=0)  INSERT INTO ""dbo"".""master"" (""id"", ""code"") VALUES (0, '/');
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""master"" where ""id""=-1)  INSERT INTO ""dbo"".""master"" (""id"", ""code"") VALUES (-1, 'ERR');
EXECUTE sp_addextendedproperty N'MS_Description', 'Код', N'SCHEMA', N'dbo', N'TABLE', N'master', N'COLUMN', 'code';
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
			Console.WriteLine(scr.Replace("\"","\"\""));
			Assert.AreEqual(@"
CREATE TABLE ""dbo"".""slave"" (
	""id"" int NOT NULL CONSTRAINT dbo_slave_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""slave_seq""),
	""version"" datetime NOT NULL DEFAULT ('19000101'),
	""master"" int NOT NULL CONSTRAINT dbo_slave_master_master_id_fk FOREIGN KEY REFERENCES ""dbo"".""master"" (""id"") DEFAULT 0
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""slave"" where ""id""=0)  INSERT INTO ""dbo"".""slave"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""slave"" where ""id""=-1)  INSERT INTO ""dbo"".""slave"" (""id"", ""master"") VALUES (-1, -1);
EXECUTE sp_addextendedproperty N'MS_Description', 'Версия', N'SCHEMA', N'dbo', N'TABLE', N'slave', N'COLUMN', 'version';
EXECUTE sp_addextendedproperty N'MS_Description', 'Главный объект', N'SCHEMA', N'dbo', N'TABLE', N'slave', N'COLUMN', 'master';
".Trim(), scr.Trim());
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
			Console.WriteLine(scr.Replace("\"","\"\""));
			Assert.AreEqual(@"
CREATE TABLE ""dbo"".""master"" (
	""id"" int NOT NULL CONSTRAINT dbo_master_id_pk PRIMARY KEY DEFAULT (nextval('""dbo"".""master_seq""')),
	""code"" varchar(255) NOT NULL CONSTRAINT dbo_master_code_unq UNIQUE DEFAULT ''
) TABLESPACE SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""master"" where ""id""=0)  INSERT INTO ""dbo"".""master"" (""id"", ""code"") VALUES (0, '/');
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""master"" where ""id""=-1)  INSERT INTO ""dbo"".""master"" (""id"", ""code"") VALUES (-1, 'ERR');
COMMENT ON COLUMN ""dbo"".""master"".""code"" IS 'Код';
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
			Console.WriteLine(scr.Replace("\"", "\"\""));
			Assert.AreEqual(@"CREATE TABLE ""dbo"".""slave"" (
	""id"" int NOT NULL CONSTRAINT dbo_slave_id_pk PRIMARY KEY DEFAULT (nextval('""dbo"".""slave_seq""')),
	""version"" timestamp NOT NULL DEFAULT ('19000101'),
	""master"" int NOT NULL CONSTRAINT dbo_slave_master_master_id_fk REFERENCES ""dbo"".""master"" (""id"") DEFERRABLE DEFAULT 0
) TABLESPACE SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""slave"" where ""id""=0)  INSERT INTO ""dbo"".""slave"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""slave"" where ""id""=-1)  INSERT INTO ""dbo"".""slave"" (""id"", ""master"") VALUES (-1, -1);
COMMENT ON COLUMN ""dbo"".""slave"".""version"" IS 'Версия';
COMMENT ON COLUMN ""dbo"".""slave"".""master"" IS 'Главный объект';".Trim(), scr.Trim());
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
			Console.WriteLine(scr.Replace("\"","\"\""));
			Assert.AreEqual(@"
CREATE TABLE ""dbo"".""master"" (
	""id"" int NOT NULL CONSTRAINT dbo_master_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""master_seq""),
	""code"" nvarchar(255) NOT NULL CONSTRAINT dbo_master_code_unq UNIQUE DEFAULT '',
	""slave"" int NOT NULL DEFAULT 0
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""master"" where ""id""=0)  INSERT INTO ""dbo"".""master"" (""id"", ""code"") VALUES (0, '/');
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""master"" where ""id""=-1)  INSERT INTO ""dbo"".""master"" (""id"", ""code"", ""slave"") VALUES (-1, 'ERR', -1);
EXECUTE sp_addextendedproperty N'MS_Description', 'Код', N'SCHEMA', N'dbo', N'TABLE', N'master', N'COLUMN', 'code';
EXECUTE sp_addextendedproperty N'MS_Description', 'Младший объект', N'SCHEMA', N'dbo', N'TABLE', N'master', N'COLUMN', 'slave';
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
			Console.WriteLine(scr.Replace("\"", "\"\""));
			Assert.AreEqual(@"
CREATE TABLE ""dbo"".""slave"" (
	""id"" int NOT NULL CONSTRAINT dbo_slave_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""slave_seq""),
	""version"" datetime NOT NULL DEFAULT ('19000101'),
	""master"" int NOT NULL DEFAULT 0
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""slave"" where ""id""=0)  INSERT INTO ""dbo"".""slave"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""slave"" where ""id""=-1)  INSERT INTO ""dbo"".""slave"" (""id"", ""master"") VALUES (-1, -1);
EXECUTE sp_addextendedproperty N'MS_Description', 'Версия', N'SCHEMA', N'dbo', N'TABLE', N'slave', N'COLUMN', 'version';
EXECUTE sp_addextendedproperty N'MS_Description', 'Главный объект', N'SCHEMA', N'dbo', N'TABLE', N'slave', N'COLUMN', 'master';
".Trim(), scr.Trim());
		}

		[TestCase(SqlDialect.SqlServer, ScriptMode.Create, "ALTER TABLE \"dbo\".\"slave\" ADD CONSTRAINT dbo_slave_master_master_id_fk FOREIGN KEY (\"master\") REFERENCES \"dbo\".\"master\" (\"id\");")]
		[TestCase(SqlDialect.PostGres, ScriptMode.Create, "ALTER TABLE \"dbo\".\"slave\" ADD CONSTRAINT dbo_slave_master_master_id_fk FOREIGN KEY (\"master\") REFERENCES \"dbo\".\"master\" (\"id\") DEFERABLE;")]
		[TestCase(SqlDialect.SqlServer, ScriptMode.Drop, "ALTER TABLE \"dbo\".\"slave\" DROP CONSTRAINT dbo_slave_master_master_id_fk;")]
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
			var trigger = new SqlTrigger{Insert = true, TableName = "dbo.x".SqlQuoteName(), Name = "OnInsert", Body="print 1;"};
			var writer = new SqlTriggerWriter(trigger){Dialect = SqlDialect.SqlServer,Mode = ScriptMode.Create,NoComment = true};
			var res = writer.ToString();
			Console.WriteLine(res);
			Assert.AreEqual(@"IF OBJECT_ID('""dbo"".""xOnInsert""') IS NOT NULL DROP TRIGGER ""dbo"".""xOnInsert"";
GO
CREATE TRIGGER ""dbo"".""xOnInsert"" ON ""dbo"".""x"" FOR INSERT AS BEGIN
print 1;
END;
GO".Trim(), writer.ToString().Trim());

			trigger.Update = true;
			trigger.Delete = true;
			trigger.Before = true;
			writer = new SqlTriggerWriter(trigger) { Dialect = SqlDialect.SqlServer, Mode = ScriptMode.Create, NoComment = true };
			res = writer.ToString();
			Console.WriteLine(res);
			Assert.AreEqual(@"IF OBJECT_ID('""dbo"".""xOnInsert""') IS NOT NULL DROP TRIGGER ""dbo"".""xOnInsert"";
GO
CREATE TRIGGER ""dbo"".""xOnInsert"" ON ""dbo"".""x"" INSTEAD OF INSERT,UPDATE,DELETE AS BEGIN
print 1;
END;
GO".Trim(), writer.ToString().Trim());
		}

		[Test]
		public void BaseViewTest(){
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	string Code
	view Full 
		selffields
");
			var view = model["a"].SqlObjects.OfType<SqlView>().First();
			var writer = new SqlViewWriter(view){NoComment = true};
			var res = writer.ToString();
			Console.WriteLine(res.Replace("\"","\"\""));
			Assert.AreEqual(@"IF OBJECT_ID('""dbo"".""aFull""') IS NOT NULL DROP VIEW ""dbo"".""aFull"";
GO
CREATE VIEW ""dbo"".""aFull"" AS SELECT 
""id"", --
""code"", --

1 as __TERMINAL FROM ""dbo"".""a""".Trim(), res.Trim());
		}


		[Test]
		public void SqlFunctionTest(){
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	int GetValue @s-int=1 @v=int : (
		return (select @s*id+@v from @this)
	)");
			var f = model["a"].SqlObjects.OfType<SqlFunction>().First();
			var writer = new SqlFunctionWriter(f){Dialect = SqlDialect.SqlServer,NoComment = true};
			var res = writer.ToString();
			Console.WriteLine(res.Replace("\"", "\"\""));
			Assert.AreEqual(@"IF OBJECT_ID('""dbo"".""aGetValue""') IS NOT NULL DROP FUNCTION ""dbo"".""aGetValue"";
GO
CREATE FUNCTION ""dbo"".""aGetValue"" ( @s int = '1',@v int = null  ) RETURNS int AS BEGIN
return (select @s*id+@v from ""dbo"".""a"")
END;
GO".Trim(), res.Trim());
		}


		[Test]
		public void SqlFunctionTestWithFuncReference()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	int GetValue @s-int=1 @v=int : (
		exec @this.proc
		return (select @s*id+@v + @this.GetValue(@s,@v) from @this where @this.Id=0)
	)");
			var f = model["a"].SqlObjects.OfType<SqlFunction>().First();
			var writer = new SqlFunctionWriter(f) { Dialect = SqlDialect.SqlServer, NoComment = true };
			var res = writer.ToString();
			Console.WriteLine(res.Replace("\"", "\"\""));
			Assert.AreEqual(@"IF OBJECT_ID('""dbo"".""aGetValue""') IS NOT NULL DROP FUNCTION ""dbo"".""aGetValue"";
GO
CREATE FUNCTION ""dbo"".""aGetValue"" ( @s int = '1',@v int = null  ) RETURNS int AS BEGIN
exec ""dbo"".""aproc""
		return (select @s*id+@v + ""dbo"".""aGetValue""(@s,@v) from ""dbo"".""a"" where ""dbo"".""a"".Id=0)
END;
GO".Trim(), res.Trim());
		}


		

		[Test]
		public void SqlFunctionVoidTest()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	void GetValue @s-int=1 @v=int : (
		select @s*id+@v from @this
	)");
			var f = model["a"].SqlObjects.OfType<SqlFunction>().First();
			var writer = new SqlFunctionWriter(f) { Dialect = SqlDialect.SqlServer,NoComment = true};
			var res = writer.ToString();
			Console.WriteLine(res.Replace("\"", "\"\""));
			Assert.AreEqual(@"IF OBJECT_ID('""dbo"".""aGetValue""') IS NOT NULL DROP PROCEDURE ""dbo"".""aGetValue"";
GO
CREATE PROCEDURE ""dbo"".""aGetValue"" @s int = '1',@v int = null  AS BEGIN
select @s*id+@v from ""dbo"".""a""
END;
GO".Trim(), res.Trim());
		}

		[Test]
		public void RefViewTest()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	string Code
	string Name
class b prototype=dbtable
	string Code
	ref a
	view Full
		selffields
		reffields
			by a
			use Code Name
");
			var view = model["b"].SqlObjects.OfType<SqlView>().First();
			var writer = new SqlViewWriter(view){NoComment = true};
			var res = writer.ToString();
			Console.WriteLine(res.Replace("\"", "\"\""));
			Assert.AreEqual(@"IF OBJECT_ID('""dbo"".""bFull""') IS NOT NULL DROP VIEW ""dbo"".""bFull"";
GO
CREATE VIEW ""dbo"".""bFull"" AS SELECT 
""id"", --
""code"", --
""a"", --
(select x.""code"" from ""dbo"".""a"" x where x.""id"" = ""dbo"".""b"".""a"") as aCode,
(select x.""name"" from ""dbo"".""a"" x where x.""id"" = ""dbo"".""b"".""a"") as aName,

1 as __TERMINAL FROM ""dbo"".""b""
".Trim(), res.Trim());
		}

		[Test]
		public void RefFreeViewTest()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	string Code
	string Name
class b prototype=dbtable
	string Code
	ref a
	view Full
		selffields
		reffields free
			by a, nosuch
			use Code Name NoSuch
");
			var view = model["b"].SqlObjects.OfType<SqlView>().First();
			var writer = new SqlViewWriter(view) { NoComment = true };
			var res = writer.ToString();
			Console.WriteLine(res.Replace("\"", "\"\""));
			Assert.AreEqual(@"IF OBJECT_ID('""dbo"".""bFull""') IS NOT NULL DROP VIEW ""dbo"".""bFull"";
GO
CREATE VIEW ""dbo"".""bFull"" AS SELECT 
""id"", --
""code"", --
""a"", --
(select x.""code"" from ""dbo"".""a"" x where x.""id"" = ""dbo"".""b"".""a"") as aCode,
(select x.""name"" from ""dbo"".""a"" x where x.""id"" = ""dbo"".""b"".""a"") as aName,

1 as __TERMINAL FROM ""dbo"".""b""
".Trim(), res.Trim());
		}
	}
}

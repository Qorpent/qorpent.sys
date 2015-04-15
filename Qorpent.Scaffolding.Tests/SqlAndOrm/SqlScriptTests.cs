using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Qorpent.Data;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.SqlObjects;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm{
	[TestFixture]
	public class SqlScriptTests{
		string GetDigest(string code,ScriptMode mode =ScriptMode.Create, SqlDialect dialect = SqlDialect.SqlServer){
			var model = PersistentModel.Compile(code);
			if(!model.IsValid)Assert.Fail("invalid model");
			var sb = new StringBuilder();
			foreach (var w in model.GetWriters(dialect,mode)){
				var str = w.GetDigest();
				if (!string.IsNullOrWhiteSpace(str)){
					sb.AppendLine(str);	
				}
				
			}
			var result = sb.ToString();
			Console.WriteLine(result.Replace("\"","\"\""));
			return result.Trim();
		}
		string GetScript(string code, ScriptMode mode = ScriptMode.Create, SqlDialect dialect = SqlDialect.SqlServer, bool print = true)
		{
			var model = PersistentModel.Compile(code);
			if (!model.IsValid) Assert.Fail("invalid model");
			var sb = new StringBuilder();
			foreach (var w in model.GetWriters(dialect, mode)){
				w.NoComment = true;
				var str = w.ToString();
				if (!string.IsNullOrWhiteSpace(str))
				{
					sb.AppendLine(str);
				}

			}
			var result = sb.ToString();
			if (print){
				Console.WriteLine(result.Replace("\"", "\"\""));
			}
			return result.Trim();
		}
		[Test]
		public void NoDefaultRowsSupports() {
			var script = GetScript("class a prototype=dbtable nodefaultrows");
			Console.WriteLine(script);
			Assert.AreEqual(@"SET NOCOUNT ON
GO
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
GO
 CREATE PROCEDURE __ensurefg @n nvarchar(255),@filecount int = 1, @filesize int = 100, @withidx bit = 0, @isdefault bit = 0 AS begin declare @q nvarchar(max) set @filesize = isnull(@filesize,100) if @filesize <=3 set @filesize =3 set @filecount = ISNULL(@filecount,1) if @filecount < 1 set @filecount= 1 set @withidx = isnull(@withidx,0) set @isdefault = isnull(@isdefault,0) set @q = 'ALTER DATABASE '+DB_NAME()+' ADD FILEGROUP '+@n BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH declare @basepath nvarchar(255) set @basepath = reverse((Select top 1 filename from sys.sysfiles)) set @basepath = REVERSE( RIGHT( @basepath, len(@basepath)-CHARINDEX('\',@basepath)+1)) declare @c int set @c = @filecount while @c >= 1 begin BEGIN TRY set @q='ALTER DATABASE '+DB_NAME()+' ADD FILE ( NAME = N'''+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+''', FILENAME = N'''+ @basepath+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+'.ndf'' , SIZE = '+cast(@filesize as nvarchar(255))+'MB , FILEGROWTH = 5% ) TO FILEGROUP ['+@n+']' exec sp_executesql @q END TRY BEGIN CATCH END CATCH set @c = @c - 1 end IF @isdefault = 1 BEGIN set @q='ALTER DATABASE '+DB_NAME()+' MODIFY FILEGROUP '+@n+' DEFAULT ' BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH end IF @withidx = 1 BEGIN set @n = @n +'IDX' exec __ensurefg @n, @filecount,@filesize,0,0 END end 
GO

GO

exec __ensurefg @n='SECONDARY', @filecount=1, @filesize=10, @withidx=0, @isdefault=1
GO

begin try
CREATE SEQUENCE ""dbo"".""a_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

CREATE TABLE ""dbo"".""a"" (
	""id"" int NOT NULL CONSTRAINT dbo_a_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""a_seq"")
) ON SECONDARY;

GO


IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg

GO", script);
		}
		[Test]
		public void BasicSqlServerScriptDigest(){
			var digest = GetDigest("class a prototype=dbtable");
			Assert.AreEqual(@"
Script sys:support_for_filegroups_begin (C,S,R)
FileGroup SECONDARY (C,S,R)
Sequence ""dbo"".""a_seq"" (C,S,O)
Table ""dbo"".""a"" (Id) (C,S,R)
TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" (C,S,R)
Script sys:support_for_filegroups_end (C,S,R)
".Trim(), digest);
		}

		[Test]
		public void PartitionedDigest()
		{
			var digest = GetDigest(@"
class table prototype=dbtable abstract
table a
	partitioned with=selector start=2
	int selector
table b
	partitioned with=ver start='19900101'
	datetime ver
");
			Assert.AreEqual(@"
Script sys:support_for_filegroups_begin (C,S,R)
FileGroup SECONDARY (C,S,R)
Sequence ""dbo"".""a_seq"" (C,S,O)
Sequence ""dbo"".""b_seq"" (C,S,O)
PARTDEF dbo_a_PARTITION (C,S,O)
PARTDEF dbo_b_PARTITION (C,S,O)
Table ""dbo"".""a"" (Id, selector) (C,S,R)
Table ""dbo"".""b"" (Id, ver) (C,S,R)
PROCEDURE ""dbo"".""aalignpartitions"" (C,S,R)
PROCEDURE ""dbo"".""balignpartitions"" (C,S,R)
TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" (C,S,R)
TRIGGER ""dbo"".""bPreventDeletionOfSystemDefinedRows"" (C,S,R)
Script sys:support_for_filegroups_end (C,S,R)

".Trim(), digest);
		}

		[Test]
		public void CanUsePersistentAttribute()
		{
			var digest = GetDigest(@"
class table persistent=1 abstract
table a
	partitioned with=selector start=2
	int selector
table b
	partitioned with=ver start='19900101'
	datetime ver
");
			Assert.AreEqual(@"
Script sys:support_for_filegroups_begin (C,S,R)
FileGroup SECONDARY (C,S,R)
Sequence ""dbo"".""a_seq"" (C,S,O)
Sequence ""dbo"".""b_seq"" (C,S,O)
PARTDEF dbo_a_PARTITION (C,S,O)
PARTDEF dbo_b_PARTITION (C,S,O)
Table ""dbo"".""a"" (Id, selector) (C,S,R)
Table ""dbo"".""b"" (Id, ver) (C,S,R)
PROCEDURE ""dbo"".""aalignpartitions"" (C,S,R)
PROCEDURE ""dbo"".""balignpartitions"" (C,S,R)
TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" (C,S,R)
TRIGGER ""dbo"".""bPreventDeletionOfSystemDefinedRows"" (C,S,R)
Script sys:support_for_filegroups_end (C,S,R)
".Trim(), digest);
		}

		[Test]
		public void PartitionedScript()
		{
			var digest = GetScript(@"
class table prototype=dbtable abstract
table a
	partitioned with=selector start=2
	int selector
table b
	partitioned with=ver start='19900101'
	datetime ver
");
			Assert.AreEqual(@"

SET NOCOUNT ON
GO
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
GO
 CREATE PROCEDURE __ensurefg @n nvarchar(255),@filecount int = 1, @filesize int = 100, @withidx bit = 0, @isdefault bit = 0 AS begin declare @q nvarchar(max) set @filesize = isnull(@filesize,100) if @filesize <=3 set @filesize =3 set @filecount = ISNULL(@filecount,1) if @filecount < 1 set @filecount= 1 set @withidx = isnull(@withidx,0) set @isdefault = isnull(@isdefault,0) set @q = 'ALTER DATABASE '+DB_NAME()+' ADD FILEGROUP '+@n BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH declare @basepath nvarchar(255) set @basepath = reverse((Select top 1 filename from sys.sysfiles)) set @basepath = REVERSE( RIGHT( @basepath, len(@basepath)-CHARINDEX('\',@basepath)+1)) declare @c int set @c = @filecount while @c >= 1 begin BEGIN TRY set @q='ALTER DATABASE '+DB_NAME()+' ADD FILE ( NAME = N'''+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+''', FILENAME = N'''+ @basepath+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+'.ndf'' , SIZE = '+cast(@filesize as nvarchar(255))+'MB , FILEGROWTH = 5% ) TO FILEGROUP ['+@n+']' exec sp_executesql @q END TRY BEGIN CATCH END CATCH set @c = @c - 1 end IF @isdefault = 1 BEGIN set @q='ALTER DATABASE '+DB_NAME()+' MODIFY FILEGROUP '+@n+' DEFAULT ' BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH end IF @withidx = 1 BEGIN set @n = @n +'IDX' exec __ensurefg @n, @filecount,@filesize,0,0 END end 
GO

GO

exec __ensurefg @n='SECONDARY', @filecount=1, @filesize=10, @withidx=0, @isdefault=1
GO

begin try
CREATE SEQUENCE ""dbo"".""a_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

begin try
CREATE SEQUENCE ""dbo"".""b_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

begin try

CREATE PARTITION FUNCTION dbo_a_PARTITIONFunc (int)
AS RANGE LEFT FOR VALUES ('2')
CREATE PARTITION SCHEME dbo_a_PARTITION AS PARTITION dbo_a_PARTITIONFunc ALL TO (SECONDARY)

end try begin catch print ERROR_MESSAGE() end catch
GO

begin try

CREATE PARTITION FUNCTION dbo_b_PARTITIONFunc (datetime)
AS RANGE LEFT FOR VALUES ('19900101')
CREATE PARTITION SCHEME dbo_b_PARTITION AS PARTITION dbo_b_PARTITIONFunc ALL TO (SECONDARY)

end try begin catch print ERROR_MESSAGE() end catch
GO

CREATE TABLE ""dbo"".""a"" (
	""id"" int NOT NULL CONSTRAINT dbo_a_id_pk PRIMARY KEY NONCLUSTERED ON SECONDARY DEFAULT (NEXT VALUE FOR ""dbo"".""a_seq""),
	""selector"" int NOT NULL DEFAULT 0
) ON dbo_a_PARTITION ( selector);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=0)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=-1)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (-1);

GO

CREATE TABLE ""dbo"".""b"" (
	""id"" int NOT NULL CONSTRAINT dbo_b_id_pk PRIMARY KEY NONCLUSTERED ON SECONDARY DEFAULT (NEXT VALUE FOR ""dbo"".""b_seq""),
	""ver"" datetime NOT NULL DEFAULT ('19000101')
) ON dbo_b_PARTITION ( ver);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""b"" where ""id""=0)  INSERT INTO ""dbo"".""b"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""b"" where ""id""=-1)  INSERT INTO ""dbo"".""b"" (""id"") VALUES (-1);

GO

IF OBJECT_ID('""dbo"".""aalignpartitions""') IS NOT NULL DROP PROCEDURE ""dbo"".""aalignpartitions"";
GO
CREATE PROCEDURE ""dbo"".""aalignpartitions""  AS BEGIN
declare @fullparts table ( num int, limit int)
while ( 1 = 1 ) begin
	delete @fullparts
	insert @fullparts (num) 
		select partition_number from sys.dm_db_partition_stats where object_id = OBJECT_ID('""dbo"".""a""') and index_id =0 and row_count > 1000000
	if (select count (*) from @fullparts ) = 0  break;
	update @fullparts set limit = lq.selector from (
		select part, min(selector) as selector from (
		select  part, selector,sum(cnt) over (order by selector) as sum from (
		select  $PARTITION.dbo_a_PARTITIONFunc(selector) as part,selector,count(*) as cnt from ""dbo"".""a""
		where $PARTITION.dbo_a_PARTITIONFunc(selector) in (select num from @fullparts)
		group by selector,$PARTITION.dbo_a_PARTITIONFunc(selector)
		) as t 
		)as c where c.sum >=900000 group by part 
	) as lq join @fullparts f2 on f2.num = lq.part
	declare @q nvarchar(max) set @q = ''
	if 'int' = 'datetime' 
		select @q = @q + '
			ALTER PARTITION SCHEME dbo_a_PARTITION NEXT USED SECONDARY
			ALTER PARTITION FUNCTION dbo_a_PARTITIONFunc() SPLIT RANGE ('''+convert(varchar(10),limit,112)+''')
		' from @fullparts
	else 
			select @q = @q + '
			ALTER PARTITION SCHEME dbo_a_PARTITION NEXT USED SECONDARY
			ALTER PARTITION FUNCTION dbo_a_PARTITIONFunc() SPLIT RANGE ('+cast(limit as varchar(20))+')
		' from @fullparts
	print @q
	exec sp_executesql @q
end
END;
GO

IF OBJECT_ID('""dbo"".""balignpartitions""') IS NOT NULL DROP PROCEDURE ""dbo"".""balignpartitions"";
GO
CREATE PROCEDURE ""dbo"".""balignpartitions""  AS BEGIN
declare @fullparts table ( num int, limit datetime)
while ( 1 = 1 ) begin
	delete @fullparts
	insert @fullparts (num) 
		select partition_number from sys.dm_db_partition_stats where object_id = OBJECT_ID('""dbo"".""b""') and index_id =0 and row_count > 1000000
	if (select count (*) from @fullparts ) = 0  break;
	update @fullparts set limit = lq.ver from (
		select part, min(ver) as ver from (
		select  part, ver,sum(cnt) over (order by ver) as sum from (
		select  $PARTITION.dbo_b_PARTITIONFunc(ver) as part,ver,count(*) as cnt from ""dbo"".""b""
		where $PARTITION.dbo_b_PARTITIONFunc(ver) in (select num from @fullparts)
		group by ver,$PARTITION.dbo_b_PARTITIONFunc(ver)
		) as t 
		)as c where c.sum >=900000 group by part 
	) as lq join @fullparts f2 on f2.num = lq.part
	declare @q nvarchar(max) set @q = ''
	if 'datetime' = 'datetime' 
		select @q = @q + '
			ALTER PARTITION SCHEME dbo_b_PARTITION NEXT USED SECONDARY
			ALTER PARTITION FUNCTION dbo_b_PARTITIONFunc() SPLIT RANGE ('''+convert(varchar(10),limit,112)+''')
		' from @fullparts
	else 
			select @q = @q + '
			ALTER PARTITION SCHEME dbo_b_PARTITION NEXT USED SECONDARY
			ALTER PARTITION FUNCTION dbo_b_PARTITIONFunc() SPLIT RANGE ('+cast(limit as varchar(20))+')
		' from @fullparts
	print @q
	exec sp_executesql @q
end
END;
GO

IF OBJECT_ID('""dbo"".""aPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"";
GO
CREATE TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" ON ""dbo"".""a"" INSTEAD OF DELETE AS BEGIN
delete ""dbo"".""a"" from deleted d join ""dbo"".""a"" on ""dbo"".""a"".id = d.id where ""dbo"".""a"".id not in (0,-1)
END;
GO

IF OBJECT_ID('""dbo"".""bPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""bPreventDeletionOfSystemDefinedRows"";
GO
CREATE TRIGGER ""dbo"".""bPreventDeletionOfSystemDefinedRows"" ON ""dbo"".""b"" INSTEAD OF DELETE AS BEGIN
delete ""dbo"".""b"" from deleted d join ""dbo"".""b"" on ""dbo"".""b"".id = d.id where ""dbo"".""b"".id not in (0,-1)
END;
GO


IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg

GO
".Trim(), digest);
		}
		[Test]
		public void CyclycSequenceTest() {
			var s = GetScript(@"class a prototype=dbtable cyclicid");
			Console.WriteLine(s);
			Assert.IsTrue(s.Contains(@"CREATE SEQUENCE ""dbo"".""a_seq"" AS int START WITH 10 INCREMENT BY 10 CYCLE;"));
		}
		[Test]
		public void PartitionedDropScript()
		{
			var digest = GetScript(@"
class table prototype=dbtable abstract
table a
	partitioned with=selector start=2
	int selector
table b
	partitioned with=ver start='19900101'
	datetime ver
",ScriptMode.Drop);
			Assert.AreEqual(@"
IF OBJECT_ID('""dbo"".""bPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""bPreventDeletionOfSystemDefinedRows"";
GO

GO

IF OBJECT_ID('""dbo"".""aPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"";
GO

GO

IF OBJECT_ID('""dbo"".""balignpartitions""') IS NOT NULL DROP PROCEDURE ""dbo"".""balignpartitions"";
GO

GO

IF OBJECT_ID('""dbo"".""aalignpartitions""') IS NOT NULL DROP PROCEDURE ""dbo"".""aalignpartitions"";
GO

GO

DROP TABLE ""dbo"".""b"";
GO

DROP TABLE ""dbo"".""a"";
GO

begin try

DROP PARTITION SCHEME dbo_b_PARTITION 
DROP PARTITION FUNCTION dbo_b_PARTITIONFunc

end try begin catch print ERROR_MESSAGE() end catch
GO

begin try

DROP PARTITION SCHEME dbo_a_PARTITION 
DROP PARTITION FUNCTION dbo_a_PARTITIONFunc

end try begin catch print ERROR_MESSAGE() end catch
GO

begin try
DROP SEQUENCE ""dbo"".""b_seq"";
end try begin catch print ERROR_MESSAGE() end catch
GO

begin try
DROP SEQUENCE ""dbo"".""a_seq"";
end try begin catch print ERROR_MESSAGE() end catch
GO

".Trim(), digest);
		}

		[Test]
		public void PartitionedDigestPG()
		{
			var digest = GetDigest(@"
class table prototype=dbtable abstract
table a
	partitioned with=selector start=2
	int selector
table b
	partitioned with=ver start='19900101'
	datetime ver
",dialect:SqlDialect.PostGres);
			Assert.AreEqual(@"
Script sys:psql_start (C,P,R)
Sequence ""dbo"".""a_seq"" (C,P,O)
Sequence ""dbo"".""b_seq"" (C,P,O)
Table ""dbo"".""a"" (Id, selector) (C,P,R)
Table ""dbo"".""b"" (Id, ver) (C,P,R)
Script sys:psql_end (C,P,R)
".Trim(), digest);
		}


		[Test]
		public void CircularLinksGenerationOrder()
		{
			var digest = GetDigest(@"
class table prototype=dbtable abstract
table a
	ref b
table b
	ref a
");
			Assert.AreEqual(@"
Script sys:support_for_filegroups_begin (C,S,R)
FileGroup SECONDARY (C,S,R)
Sequence ""dbo"".""b_seq"" (C,S,O)
Sequence ""dbo"".""a_seq"" (C,S,O)
Table ""dbo"".""b"" (Id, a) (C,S,R)
Table ""dbo"".""a"" (Id, b) (C,S,R)
FK dbo_b_a_a_id_fk (C,S,R)
FK dbo_a_b_b_id_fk (C,S,R)
TRIGGER ""dbo"".""bPreventDeletionOfSystemDefinedRows"" (C,S,R)
TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" (C,S,R)
Script sys:support_for_filegroups_end (C,S,R)
".Trim(), digest);
		}

		[Test]
		public void BasicSqlServerScript()
		{
			var digest = GetScript("class a prototype=dbtable");
			Assert.AreEqual(@"

SET NOCOUNT ON
GO
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
GO
 CREATE PROCEDURE __ensurefg @n nvarchar(255),@filecount int = 1, @filesize int = 100, @withidx bit = 0, @isdefault bit = 0 AS begin declare @q nvarchar(max) set @filesize = isnull(@filesize,100) if @filesize <=3 set @filesize =3 set @filecount = ISNULL(@filecount,1) if @filecount < 1 set @filecount= 1 set @withidx = isnull(@withidx,0) set @isdefault = isnull(@isdefault,0) set @q = 'ALTER DATABASE '+DB_NAME()+' ADD FILEGROUP '+@n BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH declare @basepath nvarchar(255) set @basepath = reverse((Select top 1 filename from sys.sysfiles)) set @basepath = REVERSE( RIGHT( @basepath, len(@basepath)-CHARINDEX('\',@basepath)+1)) declare @c int set @c = @filecount while @c >= 1 begin BEGIN TRY set @q='ALTER DATABASE '+DB_NAME()+' ADD FILE ( NAME = N'''+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+''', FILENAME = N'''+ @basepath+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+'.ndf'' , SIZE = '+cast(@filesize as nvarchar(255))+'MB , FILEGROWTH = 5% ) TO FILEGROUP ['+@n+']' exec sp_executesql @q END TRY BEGIN CATCH END CATCH set @c = @c - 1 end IF @isdefault = 1 BEGIN set @q='ALTER DATABASE '+DB_NAME()+' MODIFY FILEGROUP '+@n+' DEFAULT ' BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH end IF @withidx = 1 BEGIN set @n = @n +'IDX' exec __ensurefg @n, @filecount,@filesize,0,0 END end 
GO

GO

exec __ensurefg @n='SECONDARY', @filecount=1, @filesize=10, @withidx=0, @isdefault=1
GO

begin try
CREATE SEQUENCE ""dbo"".""a_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

CREATE TABLE ""dbo"".""a"" (
	""id"" int NOT NULL CONSTRAINT dbo_a_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""a_seq"")
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=0)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=-1)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (-1);

GO

IF OBJECT_ID('""dbo"".""aPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"";
GO
CREATE TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" ON ""dbo"".""a"" INSTEAD OF DELETE AS BEGIN
delete ""dbo"".""a"" from deleted d join ""dbo"".""a"" on ""dbo"".""a"".id = d.id where ""dbo"".""a"".id not in (0,-1)
END;
GO


IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg

GO

".Trim(), digest);
		}

		[Test]
		public void BasicPostgrsqlServerScript()
		{
			var digest = GetScript("class a prototype=dbtable",dialect:SqlDialect.PostGres);
			Assert.AreEqual(@"
DROP FUNCTION IF EXISTS ___script();
CREATE FUNCTION ___script () returns int as $$ BEGIN
CREATE SCHEMA IF NOT EXISTS dbo ; --mssql matching


BEGIN
CREATE SEQUENCE ""dbo"".""a_seq"" INCREMENT BY 10 START WITH 10;
EXCEPTION WHEN OTHERS THEN raise notice '% %', SQLERRM, SQLSTATE; END;

CREATE TABLE ""dbo"".""a"" (
	""id"" int NOT NULL CONSTRAINT dbo_a_id_pk PRIMARY KEY DEFAULT (nextval('""dbo"".""a_seq""'))
) TABLESPACE SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=0)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=-1)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (-1);



RETURN 1;
EXCEPTION
	WHEN OTHERS THEN BEGIN 
		raise notice '% %', SQLERRM, SQLSTATE;
		RETURN 0;
	END;
END;
$$ LANGUAGE plpgsql;
SELECT ___script();
DROP FUNCTION IF EXISTS ___script();

".Trim(), digest);
		}


		[Test]
		public void BasicSqlServerDropScript()
		{
			var digest = GetScript("class a prototype=dbtable",ScriptMode.Drop);
			Assert.AreEqual(@"
IF OBJECT_ID('""dbo"".""aPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"";
GO

GO

DROP TABLE ""dbo"".""a"";
GO

begin try
DROP SEQUENCE ""dbo"".""a_seq"";
end try begin catch print ERROR_MESSAGE() end catch
GO


".Trim(), digest);
		}

		private string simpleTrigger = @"
class a prototype=dbtable
	trigger '_x' insert : (
		print 25;
	)
";

		private string sampleProc = @"
class a prototype=dbtable
	void _test @i-int-out=2 @x=string : (
		print 1;
	)
	int _test2 @i=bool : (
		return 2;
	)
	function _test3 @i=int returns=bool : (
		return 1;
	)
";
		private string simpleTriggerExternalBody = @"
class a prototype=dbtable
	trigger '_x' insert externalbody=simpleTriggerExternalBody.sql
";
		private string simpleTriggerExternalFull = @"
class a prototype=dbtable
	trigger '_x' insert external=simpleTriggerExternalFull.sql
";

		[Test]
		public void TableWithTriggerDigest(){
			var digest = GetDigest(simpleTrigger);
			Assert.AreEqual(@"Script sys:support_for_filegroups_begin (C,S,R)
FileGroup SECONDARY (C,S,R)
Sequence ""dbo"".""a_seq"" (C,S,O)
Table ""dbo"".""a"" (Id) (C,S,R)
TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" (C,S,R)
TRIGGER ""dbo"".""a_x"" (C,S,R)
Script sys:support_for_filegroups_end (C,S,R)".Trim(), digest.Trim());
		}

		[Test]
		public void TableWithFunctionsDigest()
		{
			var digest = GetDigest(sampleProc);
			Assert.AreEqual(@"Script sys:support_for_filegroups_begin (C,S,R)
FileGroup SECONDARY (C,S,R)
Sequence ""dbo"".""a_seq"" (C,S,O)
Table ""dbo"".""a"" (Id) (C,S,R)
PROCEDURE ""dbo"".""a_test"" (C,S,R)
FUNCTION ""dbo"".""a_test2"" (C,S,R)
FUNCTION ""dbo"".""a_test3"" (C,S,R)
TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" (C,S,R)
Script sys:support_for_filegroups_end (C,S,R)".Trim(), digest.Trim());
		}

		[Test]
		public void TableWithFunctionsScript()
		{
			var digest = GetScript(sampleProc);
			Assert.AreEqual(@"

SET NOCOUNT ON
GO
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
GO
 CREATE PROCEDURE __ensurefg @n nvarchar(255),@filecount int = 1, @filesize int = 100, @withidx bit = 0, @isdefault bit = 0 AS begin declare @q nvarchar(max) set @filesize = isnull(@filesize,100) if @filesize <=3 set @filesize =3 set @filecount = ISNULL(@filecount,1) if @filecount < 1 set @filecount= 1 set @withidx = isnull(@withidx,0) set @isdefault = isnull(@isdefault,0) set @q = 'ALTER DATABASE '+DB_NAME()+' ADD FILEGROUP '+@n BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH declare @basepath nvarchar(255) set @basepath = reverse((Select top 1 filename from sys.sysfiles)) set @basepath = REVERSE( RIGHT( @basepath, len(@basepath)-CHARINDEX('\',@basepath)+1)) declare @c int set @c = @filecount while @c >= 1 begin BEGIN TRY set @q='ALTER DATABASE '+DB_NAME()+' ADD FILE ( NAME = N'''+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+''', FILENAME = N'''+ @basepath+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+'.ndf'' , SIZE = '+cast(@filesize as nvarchar(255))+'MB , FILEGROWTH = 5% ) TO FILEGROUP ['+@n+']' exec sp_executesql @q END TRY BEGIN CATCH END CATCH set @c = @c - 1 end IF @isdefault = 1 BEGIN set @q='ALTER DATABASE '+DB_NAME()+' MODIFY FILEGROUP '+@n+' DEFAULT ' BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH end IF @withidx = 1 BEGIN set @n = @n +'IDX' exec __ensurefg @n, @filecount,@filesize,0,0 END end 
GO

GO

exec __ensurefg @n='SECONDARY', @filecount=1, @filesize=10, @withidx=0, @isdefault=1
GO

begin try
CREATE SEQUENCE ""dbo"".""a_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

CREATE TABLE ""dbo"".""a"" (
	""id"" int NOT NULL CONSTRAINT dbo_a_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""a_seq"")
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=0)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=-1)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (-1);

GO

IF OBJECT_ID('""dbo"".""a_test""') IS NOT NULL DROP PROCEDURE ""dbo"".""a_test"";
GO
CREATE PROCEDURE ""dbo"".""a_test"" @i int = '2' OUTPUT,@x nvarchar(255) = null  AS BEGIN
print 1;
END;
GO

IF OBJECT_ID('""dbo"".""a_test2""') IS NOT NULL DROP FUNCTION ""dbo"".""a_test2"";
GO
CREATE FUNCTION ""dbo"".""a_test2"" ( @i bit = null  ) RETURNS int AS BEGIN
return 2;
END;
GO

IF OBJECT_ID('""dbo"".""a_test3""') IS NOT NULL DROP FUNCTION ""dbo"".""a_test3"";
GO
CREATE FUNCTION ""dbo"".""a_test3"" ( @i int = null  ) RETURNS bit AS BEGIN
return 1;
END;
GO

IF OBJECT_ID('""dbo"".""aPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"";
GO
CREATE TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" ON ""dbo"".""a"" INSTEAD OF DELETE AS BEGIN
delete ""dbo"".""a"" from deleted d join ""dbo"".""a"" on ""dbo"".""a"".id = d.id where ""dbo"".""a"".id not in (0,-1)
END;
GO


IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg

GO
".Trim(), digest.Trim());
		}

		
		[Test]
		public void TableWithTriggerScript()
		{
			var digest = GetScript(simpleTrigger);
			Assert.AreEqual(@"

SET NOCOUNT ON
GO
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
GO
 CREATE PROCEDURE __ensurefg @n nvarchar(255),@filecount int = 1, @filesize int = 100, @withidx bit = 0, @isdefault bit = 0 AS begin declare @q nvarchar(max) set @filesize = isnull(@filesize,100) if @filesize <=3 set @filesize =3 set @filecount = ISNULL(@filecount,1) if @filecount < 1 set @filecount= 1 set @withidx = isnull(@withidx,0) set @isdefault = isnull(@isdefault,0) set @q = 'ALTER DATABASE '+DB_NAME()+' ADD FILEGROUP '+@n BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH declare @basepath nvarchar(255) set @basepath = reverse((Select top 1 filename from sys.sysfiles)) set @basepath = REVERSE( RIGHT( @basepath, len(@basepath)-CHARINDEX('\',@basepath)+1)) declare @c int set @c = @filecount while @c >= 1 begin BEGIN TRY set @q='ALTER DATABASE '+DB_NAME()+' ADD FILE ( NAME = N'''+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+''', FILENAME = N'''+ @basepath+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+'.ndf'' , SIZE = '+cast(@filesize as nvarchar(255))+'MB , FILEGROWTH = 5% ) TO FILEGROUP ['+@n+']' exec sp_executesql @q END TRY BEGIN CATCH END CATCH set @c = @c - 1 end IF @isdefault = 1 BEGIN set @q='ALTER DATABASE '+DB_NAME()+' MODIFY FILEGROUP '+@n+' DEFAULT ' BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH end IF @withidx = 1 BEGIN set @n = @n +'IDX' exec __ensurefg @n, @filecount,@filesize,0,0 END end 
GO

GO

exec __ensurefg @n='SECONDARY', @filecount=1, @filesize=10, @withidx=0, @isdefault=1
GO

begin try
CREATE SEQUENCE ""dbo"".""a_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

CREATE TABLE ""dbo"".""a"" (
	""id"" int NOT NULL CONSTRAINT dbo_a_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""a_seq"")
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=0)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=-1)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (-1);

GO

IF OBJECT_ID('""dbo"".""aPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"";
GO
CREATE TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" ON ""dbo"".""a"" INSTEAD OF DELETE AS BEGIN
delete ""dbo"".""a"" from deleted d join ""dbo"".""a"" on ""dbo"".""a"".id = d.id where ""dbo"".""a"".id not in (0,-1)
END;
GO

IF OBJECT_ID('""dbo"".""a_x""') IS NOT NULL DROP TRIGGER ""dbo"".""a_x"";
GO
CREATE TRIGGER ""dbo"".""a_x"" ON ""dbo"".""a"" FOR INSERT AS BEGIN
print 25;
END;
GO


IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg

GO".Trim(), digest.Trim());
		}

		[Test]
		public void DetectsTrigger(){
			var model = PersistentModel.Compile(simpleTrigger);
			Assert.NotNull(model["a"].SqlObjects.OfType<SqlTrigger>().FirstOrDefault());
		}


		[Test]
		public void TriggerWithExternalBody(){
			File.WriteAllText("simpleTriggerExternalBody.sql", "print 45;");
			var digest = GetScript(simpleTriggerExternalBody);
			Assert.AreEqual(@"

SET NOCOUNT ON
GO
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
GO
 CREATE PROCEDURE __ensurefg @n nvarchar(255),@filecount int = 1, @filesize int = 100, @withidx bit = 0, @isdefault bit = 0 AS begin declare @q nvarchar(max) set @filesize = isnull(@filesize,100) if @filesize <=3 set @filesize =3 set @filecount = ISNULL(@filecount,1) if @filecount < 1 set @filecount= 1 set @withidx = isnull(@withidx,0) set @isdefault = isnull(@isdefault,0) set @q = 'ALTER DATABASE '+DB_NAME()+' ADD FILEGROUP '+@n BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH declare @basepath nvarchar(255) set @basepath = reverse((Select top 1 filename from sys.sysfiles)) set @basepath = REVERSE( RIGHT( @basepath, len(@basepath)-CHARINDEX('\',@basepath)+1)) declare @c int set @c = @filecount while @c >= 1 begin BEGIN TRY set @q='ALTER DATABASE '+DB_NAME()+' ADD FILE ( NAME = N'''+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+''', FILENAME = N'''+ @basepath+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+'.ndf'' , SIZE = '+cast(@filesize as nvarchar(255))+'MB , FILEGROWTH = 5% ) TO FILEGROUP ['+@n+']' exec sp_executesql @q END TRY BEGIN CATCH END CATCH set @c = @c - 1 end IF @isdefault = 1 BEGIN set @q='ALTER DATABASE '+DB_NAME()+' MODIFY FILEGROUP '+@n+' DEFAULT ' BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH end IF @withidx = 1 BEGIN set @n = @n +'IDX' exec __ensurefg @n, @filecount,@filesize,0,0 END end 
GO

GO

exec __ensurefg @n='SECONDARY', @filecount=1, @filesize=10, @withidx=0, @isdefault=1
GO

begin try
CREATE SEQUENCE ""dbo"".""a_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

CREATE TABLE ""dbo"".""a"" (
	""id"" int NOT NULL CONSTRAINT dbo_a_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""a_seq"")
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=0)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=-1)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (-1);

GO

IF OBJECT_ID('""dbo"".""aPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"";
GO
CREATE TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" ON ""dbo"".""a"" INSTEAD OF DELETE AS BEGIN
delete ""dbo"".""a"" from deleted d join ""dbo"".""a"" on ""dbo"".""a"".id = d.id where ""dbo"".""a"".id not in (0,-1)
END;
GO

IF OBJECT_ID('""dbo"".""a_x""') IS NOT NULL DROP TRIGGER ""dbo"".""a_x"";
GO
CREATE TRIGGER ""dbo"".""a_x"" ON ""dbo"".""a"" FOR INSERT AS BEGIN
print 45;
END;
GO


IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg

GO

".Trim(), digest.Trim());
		}

		[Test]
		public void TriggerWithExternalFull()
		{
			File.WriteAllText("simpleTriggerExternalFull.sql", "create trigger dbo.a_xxx on dbo.a for delete as begin print 15; end;");
			var digest = GetScript(simpleTriggerExternalFull);
			Assert.AreEqual(@"
SET NOCOUNT ON
GO
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
GO
 CREATE PROCEDURE __ensurefg @n nvarchar(255),@filecount int = 1, @filesize int = 100, @withidx bit = 0, @isdefault bit = 0 AS begin declare @q nvarchar(max) set @filesize = isnull(@filesize,100) if @filesize <=3 set @filesize =3 set @filecount = ISNULL(@filecount,1) if @filecount < 1 set @filecount= 1 set @withidx = isnull(@withidx,0) set @isdefault = isnull(@isdefault,0) set @q = 'ALTER DATABASE '+DB_NAME()+' ADD FILEGROUP '+@n BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH declare @basepath nvarchar(255) set @basepath = reverse((Select top 1 filename from sys.sysfiles)) set @basepath = REVERSE( RIGHT( @basepath, len(@basepath)-CHARINDEX('\',@basepath)+1)) declare @c int set @c = @filecount while @c >= 1 begin BEGIN TRY set @q='ALTER DATABASE '+DB_NAME()+' ADD FILE ( NAME = N'''+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+''', FILENAME = N'''+ @basepath+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+'.ndf'' , SIZE = '+cast(@filesize as nvarchar(255))+'MB , FILEGROWTH = 5% ) TO FILEGROUP ['+@n+']' exec sp_executesql @q END TRY BEGIN CATCH END CATCH set @c = @c - 1 end IF @isdefault = 1 BEGIN set @q='ALTER DATABASE '+DB_NAME()+' MODIFY FILEGROUP '+@n+' DEFAULT ' BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH end IF @withidx = 1 BEGIN set @n = @n +'IDX' exec __ensurefg @n, @filecount,@filesize,0,0 END end 
GO

GO

exec __ensurefg @n='SECONDARY', @filecount=1, @filesize=10, @withidx=0, @isdefault=1
GO

begin try
CREATE SEQUENCE ""dbo"".""a_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

CREATE TABLE ""dbo"".""a"" (
	""id"" int NOT NULL CONSTRAINT dbo_a_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""a_seq"")
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=0)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=-1)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (-1);

GO

IF OBJECT_ID('""dbo"".""aPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"";
GO
CREATE TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" ON ""dbo"".""a"" INSTEAD OF DELETE AS BEGIN
delete ""dbo"".""a"" from deleted d join ""dbo"".""a"" on ""dbo"".""a"".id = d.id where ""dbo"".""a"".id not in (0,-1)
END;
GO

IF OBJECT_ID('""dbo"".""a_x""') IS NOT NULL DROP TRIGGER ""dbo"".""a_x"";
GO
create trigger dbo.a_xxx on dbo.a for delete as begin print 15; end;
GO


IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg

GO".Trim(), digest.Trim());
		}


		private string sampleView = @"
class a prototype=dbtable
	string code
	string name
a b prototype=dbtable
	ref a
	view _v : (
		--PARENT_FIELD_SET--
		--PARENT_REF_SET FOR (a) WITH (code,name) --
	)
";

		[Test]
		public void TableWithViewDigest()
		{
			var digest = GetDigest(sampleView);
			Assert.AreEqual(@"Script sys:support_for_filegroups_begin (C,S,R)
FileGroup SECONDARY (C,S,R)
Sequence ""dbo"".""a_seq"" (C,S,O)
Sequence ""dbo"".""b_seq"" (C,S,O)
Table ""dbo"".""a"" (Id, code, name) (C,S,R)
Table ""dbo"".""b"" (Id, a, code, name) (C,S,R)
VIEW ""dbo"".""b_v"" (C,S,R)
TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" (C,S,R)
TRIGGER ""dbo"".""bPreventDeletionOfSystemDefinedRows"" (C,S,R)
Script sys:support_for_filegroups_end (C,S,R)".Trim(), digest.Trim());
		}

		[Test]
		[Repeat(1000)]
		[Category("NOTC")]
		public void Q229_B533_TableWithViewScript_Stability(){
			var digest = GetScript(sampleView,print:false);
			Assert.AreEqual(@"

SET NOCOUNT ON
GO
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
GO
 CREATE PROCEDURE __ensurefg @n nvarchar(255),@filecount int = 1, @filesize int = 100, @withidx bit = 0, @isdefault bit = 0 AS begin declare @q nvarchar(max) set @filesize = isnull(@filesize,100) if @filesize <=3 set @filesize =3 set @filecount = ISNULL(@filecount,1) if @filecount < 1 set @filecount= 1 set @withidx = isnull(@withidx,0) set @isdefault = isnull(@isdefault,0) set @q = 'ALTER DATABASE '+DB_NAME()+' ADD FILEGROUP '+@n BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH declare @basepath nvarchar(255) set @basepath = reverse((Select top 1 filename from sys.sysfiles)) set @basepath = REVERSE( RIGHT( @basepath, len(@basepath)-CHARINDEX('\',@basepath)+1)) declare @c int set @c = @filecount while @c >= 1 begin BEGIN TRY set @q='ALTER DATABASE '+DB_NAME()+' ADD FILE ( NAME = N'''+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+''', FILENAME = N'''+ @basepath+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+'.ndf'' , SIZE = '+cast(@filesize as nvarchar(255))+'MB , FILEGROWTH = 5% ) TO FILEGROUP ['+@n+']' exec sp_executesql @q END TRY BEGIN CATCH END CATCH set @c = @c - 1 end IF @isdefault = 1 BEGIN set @q='ALTER DATABASE '+DB_NAME()+' MODIFY FILEGROUP '+@n+' DEFAULT ' BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH end IF @withidx = 1 BEGIN set @n = @n +'IDX' exec __ensurefg @n, @filecount,@filesize,0,0 END end 
GO

GO

exec __ensurefg @n='SECONDARY', @filecount=1, @filesize=10, @withidx=0, @isdefault=1
GO

begin try
CREATE SEQUENCE ""dbo"".""a_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

begin try
CREATE SEQUENCE ""dbo"".""b_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

CREATE TABLE ""dbo"".""a"" (
	""id"" int NOT NULL CONSTRAINT dbo_a_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""a_seq""),
	""code"" nvarchar(255) NOT NULL DEFAULT '',
	""name"" nvarchar(255) NOT NULL DEFAULT ''
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=0)  INSERT INTO ""dbo"".""a"" (""id"", ""code"", ""name"") VALUES (0, '/', 'NULL/ROOT');
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=-1)  INSERT INTO ""dbo"".""a"" (""id"", ""code"", ""name"") VALUES (-1, 'ERR', 'ERROR/LOST');

GO

CREATE TABLE ""dbo"".""b"" (
	""id"" int NOT NULL CONSTRAINT dbo_b_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""b_seq""),
	""a"" int NOT NULL CONSTRAINT dbo_b_a_a_id_fk FOREIGN KEY REFERENCES ""dbo"".""a"" (""id"") DEFAULT 0,
	""code"" nvarchar(255) NOT NULL DEFAULT '',
	""name"" nvarchar(255) NOT NULL DEFAULT ''
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""b"" where ""id""=0)  INSERT INTO ""dbo"".""b"" (""id"", ""code"", ""name"") VALUES (0, '/', 'NULL/ROOT');
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""b"" where ""id""=-1)  INSERT INTO ""dbo"".""b"" (""id"", ""code"", ""name"", ""a"") VALUES (-1, 'ERR', 'ERROR/LOST', -1);

GO

IF OBJECT_ID('""dbo"".""b_v""') IS NOT NULL DROP VIEW ""dbo"".""b_v"";
GO
CREATE VIEW ""dbo"".""b_v"" AS SELECT
""id"", --
""a"", --
""code"", --
""name"", --

		(select x.""code"" from ""dbo"".""a"" x where x.""id"" = ""dbo"".""b"".""a"") as acode,
(select x.""name"" from ""dbo"".""a"" x where x.""id"" = ""dbo"".""b"".""a"") as aname,
1 as __TERMINAL FROM ""dbo"".""b""


GO

IF OBJECT_ID('""dbo"".""aPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"";
GO
CREATE TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" ON ""dbo"".""a"" INSTEAD OF DELETE AS BEGIN
delete ""dbo"".""a"" from deleted d join ""dbo"".""a"" on ""dbo"".""a"".id = d.id where ""dbo"".""a"".id not in (0,-1)
END;
GO

IF OBJECT_ID('""dbo"".""bPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""bPreventDeletionOfSystemDefinedRows"";
GO
CREATE TRIGGER ""dbo"".""bPreventDeletionOfSystemDefinedRows"" ON ""dbo"".""b"" INSTEAD OF DELETE AS BEGIN
delete ""dbo"".""b"" from deleted d join ""dbo"".""b"" on ""dbo"".""b"".id = d.id where ""dbo"".""b"".id not in (0,-1)
END;
GO


IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg

GO


".Trim(), digest.Trim());
		}

		[Test]
		public void TableWithViewScript()
		{
			var digest = GetScript(sampleView);
			Assert.AreEqual(@"

SET NOCOUNT ON
GO
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
GO
 CREATE PROCEDURE __ensurefg @n nvarchar(255),@filecount int = 1, @filesize int = 100, @withidx bit = 0, @isdefault bit = 0 AS begin declare @q nvarchar(max) set @filesize = isnull(@filesize,100) if @filesize <=3 set @filesize =3 set @filecount = ISNULL(@filecount,1) if @filecount < 1 set @filecount= 1 set @withidx = isnull(@withidx,0) set @isdefault = isnull(@isdefault,0) set @q = 'ALTER DATABASE '+DB_NAME()+' ADD FILEGROUP '+@n BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH declare @basepath nvarchar(255) set @basepath = reverse((Select top 1 filename from sys.sysfiles)) set @basepath = REVERSE( RIGHT( @basepath, len(@basepath)-CHARINDEX('\',@basepath)+1)) declare @c int set @c = @filecount while @c >= 1 begin BEGIN TRY set @q='ALTER DATABASE '+DB_NAME()+' ADD FILE ( NAME = N'''+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+''', FILENAME = N'''+ @basepath+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+'.ndf'' , SIZE = '+cast(@filesize as nvarchar(255))+'MB , FILEGROWTH = 5% ) TO FILEGROUP ['+@n+']' exec sp_executesql @q END TRY BEGIN CATCH END CATCH set @c = @c - 1 end IF @isdefault = 1 BEGIN set @q='ALTER DATABASE '+DB_NAME()+' MODIFY FILEGROUP '+@n+' DEFAULT ' BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH end IF @withidx = 1 BEGIN set @n = @n +'IDX' exec __ensurefg @n, @filecount,@filesize,0,0 END end 
GO

GO

exec __ensurefg @n='SECONDARY', @filecount=1, @filesize=10, @withidx=0, @isdefault=1
GO

begin try
CREATE SEQUENCE ""dbo"".""a_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

begin try
CREATE SEQUENCE ""dbo"".""b_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

CREATE TABLE ""dbo"".""a"" (
	""id"" int NOT NULL CONSTRAINT dbo_a_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""a_seq""),
	""code"" nvarchar(255) NOT NULL DEFAULT '',
	""name"" nvarchar(255) NOT NULL DEFAULT ''
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=0)  INSERT INTO ""dbo"".""a"" (""id"", ""code"", ""name"") VALUES (0, '/', 'NULL/ROOT');
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=-1)  INSERT INTO ""dbo"".""a"" (""id"", ""code"", ""name"") VALUES (-1, 'ERR', 'ERROR/LOST');

GO

CREATE TABLE ""dbo"".""b"" (
	""id"" int NOT NULL CONSTRAINT dbo_b_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""b_seq""),
	""a"" int NOT NULL CONSTRAINT dbo_b_a_a_id_fk FOREIGN KEY REFERENCES ""dbo"".""a"" (""id"") DEFAULT 0,
	""code"" nvarchar(255) NOT NULL DEFAULT '',
	""name"" nvarchar(255) NOT NULL DEFAULT ''
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""b"" where ""id""=0)  INSERT INTO ""dbo"".""b"" (""id"", ""code"", ""name"") VALUES (0, '/', 'NULL/ROOT');
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""b"" where ""id""=-1)  INSERT INTO ""dbo"".""b"" (""id"", ""code"", ""name"", ""a"") VALUES (-1, 'ERR', 'ERROR/LOST', -1);

GO

IF OBJECT_ID('""dbo"".""b_v""') IS NOT NULL DROP VIEW ""dbo"".""b_v"";
GO
CREATE VIEW ""dbo"".""b_v"" AS SELECT
""id"", --
""a"", --
""code"", --
""name"", --

		(select x.""code"" from ""dbo"".""a"" x where x.""id"" = ""dbo"".""b"".""a"") as acode,
(select x.""name"" from ""dbo"".""a"" x where x.""id"" = ""dbo"".""b"".""a"") as aname,
1 as __TERMINAL FROM ""dbo"".""b""


GO

IF OBJECT_ID('""dbo"".""aPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"";
GO
CREATE TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" ON ""dbo"".""a"" INSTEAD OF DELETE AS BEGIN
delete ""dbo"".""a"" from deleted d join ""dbo"".""a"" on ""dbo"".""a"".id = d.id where ""dbo"".""a"".id not in (0,-1)
END;
GO

IF OBJECT_ID('""dbo"".""bPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""bPreventDeletionOfSystemDefinedRows"";
GO
CREATE TRIGGER ""dbo"".""bPreventDeletionOfSystemDefinedRows"" ON ""dbo"".""b"" INSTEAD OF DELETE AS BEGIN
delete ""dbo"".""b"" from deleted d join ""dbo"".""b"" on ""dbo"".""b"".id = d.id where ""dbo"".""b"".id not in (0,-1)
END;
GO


IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg

GO


".Trim(), digest.Trim());
		}


		[Test]
		public void CanFilterGenerationTypesWithInclude()
		{
			var model = PersistentModel.Compile(new GenerationOptions { IncludeSqlObjectTypes = SqlObjectType.Function },@"
class a prototype=dbtable 
	int Get1 : (
		return 1
	)
");
			var script = model.GetScript(SqlDialect.SqlServer, ScriptMode.Create);
			Console.WriteLine(script.Replace("\"","\"\""));
			Assert.AreEqual(@"
-- begin command SqlFunctionWriter
IF OBJECT_ID('""dbo"".""aGet1""') IS NOT NULL DROP FUNCTION ""dbo"".""aGet1"";
GO
CREATE FUNCTION ""dbo"".""aGet1"" (  ) RETURNS int AS BEGIN
return 1
END;
GO
".Trim(),script.Trim());
		}

		[Test]
		public void CanFilterGenerationTypesWithExclude()
		{
			var model = PersistentModel.Compile(new GenerationOptions { ExcludeSqlObjectTypes = SqlObjectType.Function }, @"
class a prototype=dbtable 
	int Get1 : (
		return 1
	)
");
			var script = model.GetScript(SqlDialect.SqlServer, ScriptMode.Create);
			Console.WriteLine(script.Replace("\"", "\"\""));
			Assert.AreEqual(@"
-- begin command ScriptWriter

SET NOCOUNT ON
GO
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
GO
 CREATE PROCEDURE __ensurefg @n nvarchar(255),@filecount int = 1, @filesize int = 100, @withidx bit = 0, @isdefault bit = 0 AS begin declare @q nvarchar(max) set @filesize = isnull(@filesize,100) if @filesize <=3 set @filesize =3 set @filecount = ISNULL(@filecount,1) if @filecount < 1 set @filecount= 1 set @withidx = isnull(@withidx,0) set @isdefault = isnull(@isdefault,0) set @q = 'ALTER DATABASE '+DB_NAME()+' ADD FILEGROUP '+@n BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH declare @basepath nvarchar(255) set @basepath = reverse((Select top 1 filename from sys.sysfiles)) set @basepath = REVERSE( RIGHT( @basepath, len(@basepath)-CHARINDEX('\',@basepath)+1)) declare @c int set @c = @filecount while @c >= 1 begin BEGIN TRY set @q='ALTER DATABASE '+DB_NAME()+' ADD FILE ( NAME = N'''+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+''', FILENAME = N'''+ @basepath+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+'.ndf'' , SIZE = '+cast(@filesize as nvarchar(255))+'MB , FILEGROWTH = 5% ) TO FILEGROUP ['+@n+']' exec sp_executesql @q END TRY BEGIN CATCH END CATCH set @c = @c - 1 end IF @isdefault = 1 BEGIN set @q='ALTER DATABASE '+DB_NAME()+' MODIFY FILEGROUP '+@n+' DEFAULT ' BEGIN TRY exec sp_executesql @q END TRY BEGIN CATCH END CATCH end IF @withidx = 1 BEGIN set @n = @n +'IDX' exec __ensurefg @n, @filecount,@filesize,0,0 END end 
GO

GO

-- begin command FileGroupWriter
exec __ensurefg @n='SECONDARY', @filecount=1, @filesize=10, @withidx=0, @isdefault=1
GO

-- begin command SequenceWriter
begin try
CREATE SEQUENCE ""dbo"".""a_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

-- begin command TableWriter
CREATE TABLE ""dbo"".""a"" (
	""id"" int NOT NULL CONSTRAINT dbo_a_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""a_seq"")
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=0)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (0);
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=-1)  INSERT INTO ""dbo"".""a"" (""id"") VALUES (-1);

GO

-- begin command SqlTriggerWriter
IF OBJECT_ID('""dbo"".""aPreventDeletionOfSystemDefinedRows""') IS NOT NULL DROP TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"";
GO
CREATE TRIGGER ""dbo"".""aPreventDeletionOfSystemDefinedRows"" ON ""dbo"".""a"" INSTEAD OF DELETE AS BEGIN
delete ""dbo"".""a"" from deleted d join ""dbo"".""a"" on ""dbo"".""a"".id = d.id where ""dbo"".""a"".id not in (0,-1)
END;
GO

-- begin command ScriptWriter

IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg

GO
".Trim(), script.Trim());
		}
	}

	
	
}
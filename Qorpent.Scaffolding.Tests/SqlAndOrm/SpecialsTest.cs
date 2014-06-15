using System;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm{
	[TestFixture]
	public class SpecialsTest{
		[Test]
		public void NoSqlFilterApplyed(){
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	string Code
	string Code2 nosql
	
class b prototype=dbtable
	string Code
	string Code2 nosql
	ref a
	view Full
		selffields
		reffields
			ref a
			to Code
			to Code2
");
			var script = model.GetScript(SqlDialect.SqlServer, ScriptMode.Create);
			Console.WriteLine(script.Replace("\"","\"\""));
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

-- begin command SequenceWriter
begin try
CREATE SEQUENCE ""dbo"".""b_seq"" AS int START WITH 10 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

-- begin command TableWriter
CREATE TABLE ""dbo"".""a"" (
	""id"" int NOT NULL CONSTRAINT dbo_a_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""a_seq""),
	""code"" nvarchar(255) NOT NULL DEFAULT ''
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=0)  INSERT ""dbo"".""a"" (""id"", ""code"") VALUES (0, '/');
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""a"" where ""id""=-1)  INSERT ""dbo"".""a"" (""id"", ""code"") VALUES (-1, 'ERR');

GO

-- begin command TableWriter
CREATE TABLE ""dbo"".""b"" (
	""id"" int NOT NULL CONSTRAINT dbo_b_id_pk PRIMARY KEY DEFAULT (NEXT VALUE FOR ""dbo"".""b_seq""),
	""a"" int NOT NULL CONSTRAINT dbo_b_a_a_id_fk FOREIGN KEY REFERENCES ""dbo"".""a"" (""id"") DEFAULT 0,
	""code"" nvarchar(255) NOT NULL DEFAULT ''
) ON SECONDARY;
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""b"" where ""id""=0)  INSERT ""dbo"".""b"" (""id"", ""code"") VALUES (0, '/');
IF NOT EXISTS (SELECT TOP 1 * FROM ""dbo"".""b"" where ""id""=-1)  INSERT ""dbo"".""b"" (""id"", ""code"") VALUES (-1, 'ERR');

GO

-- begin command SqlViewWriter
IF OBJECT_ID('""dbo"".""bFull""') IS NOT NULL DROP VIEW ""dbo"".""bFull"";
GO
CREATE VIEW ""dbo"".""bFull"" AS SELECT 
""id"", --
""code"", --
""a"", --
(select x.""code"" from ""dbo"".""a"" x where x.""id"" = ""dbo"".""b"".""a"") as aCode,

1 as __TERMINAL FROM ""dbo"".""b""


GO

-- begin command ScriptWriter

IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg

GO
".Trim(),script.Trim());
		}
	}
}
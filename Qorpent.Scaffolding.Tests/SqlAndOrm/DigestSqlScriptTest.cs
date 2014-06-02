using System;
using System.Text;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm{
	[TestFixture]
	public class DigestSqlScriptTest2{
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
			Console.WriteLine(result);
			return result.Trim();
		}
		string GetScript(string code, ScriptMode mode = ScriptMode.Create, SqlDialect dialect = SqlDialect.SqlServer)
		{
			var model = PersistentModel.Compile(code);
			if (!model.IsValid) Assert.Fail("invalid model");
			var sb = new StringBuilder();
			foreach (var w in model.GetWriters(dialect, mode))
			{
				var str = w.ToString();
				if (!string.IsNullOrWhiteSpace(str))
				{
					sb.AppendLine(str);
				}

			}
			var result = sb.ToString();
			Console.WriteLine(result);
			return result.Trim();
		}
		[Test]
		public void BasicSqlServerScriptDigest(){
			var digest = GetDigest("class a prototype=dbtable");
			Assert.AreEqual(@"
Script sys:support_for_filegroups_begin (C,S,R)
FileGroup SECONDARY (C,S,R)
Sequence dbo.a_SEQ (C,S,O)
Table dbo.a (C,S,R)
Script sys:support_for_filegroups_end (C,S,R)

".Trim(), digest);
		}


		[Test]
		public void BasicSqlServerScript()
		{
			var digest = GetScript("class a prototype=dbtable");
			Assert.AreEqual(@"
-- begin command ScriptWriter

SET NOCOUNT ON
GO
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
GO
CREATE PROCEDURE __ensurefg @n nvarchar(255),@filecount int = 1, @filesize int = 100, @withidx bit = 0, @isdefault bit = 0 AS begin
	declare @q nvarchar(max) 
	set @filesize = isnull(@filesize,100)
	if @filesize <=3 set @filesize  =3
	set @filecount = ISNULL(@filecount,1)
	if @filecount < 1 set @filecount= 1
	set @withidx = isnull(@withidx,0)
	set @isdefault = isnull(@isdefault,0)
	set @q = 'ALTER DATABASE '+DB_NAME()+' ADD FILEGROUP '+@n
	BEGIN TRY
		exec sp_executesql @q
	END TRY
	BEGIN CATCH
	END CATCH
	declare @basepath nvarchar(255) set @basepath = reverse((Select top 1 filename from sys.sysfiles))
	set @basepath = REVERSE( RIGHT( @basepath, len(@basepath)-CHARINDEX('\',@basepath)+1))

	declare @c int set @c = @filecount 
	while @c >= 1 begin
		BEGIN TRY
			set @q='ALTER DATABASE '+DB_NAME()+' ADD FILE ( NAME = N'''+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+''', FILENAME = N'''+
				@basepath+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+'.ndf'' , SIZE = '+cast(@filesize as nvarchar(255))+'MB , FILEGROWTH = 5% ) TO FILEGROUP ['+@n+']'
			exec sp_executesql @q
		END TRY
		BEGIN CATCH
		END CATCH
		set @c = @c - 1
	end

	IF @isdefault = 1 BEGIN
		set @q='ALTER DATABASE '+DB_NAME()+' MODIFY FILEGROUP '+@n+' DEFAULT '
		BEGIN TRY
			exec sp_executesql @q
		END TRY 
		BEGIN CATCH
		END CATCH
	end

	IF @withidx = 1 BEGIN
		set @n = @n +'IDX'
		exec __ensurefg @n, @filecount,@filesize,0,0
	END
end
GO

GO

-- begin command FileGroupWriter
exec __ensurefg @n='SECONDARY', @filecount=1, @filesize=10, @withidx=0, @isdefault=1
GO

-- begin command SequenceWriter
begin try
CREATE SEQUENCE dbo.a_SEQ AS int START WITH 0 INCREMENT BY 10;
end try begin catch print ERROR_MESSAGE() end catch
GO

-- begin command TableWriter
CREATE TABLE dbo.a (
	Id int NOT NULL CONSTRAINT dbo_a_Id_PK PRIMARY KEY DEFAULT (NEXT VALUE FOR dbo.a_SEQ)
) ON SECONDARY

GO

-- begin command ScriptWriter

IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg

GO
".Trim(), digest);
		}
	}
}
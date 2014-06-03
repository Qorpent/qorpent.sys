using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Scaffolding.Model
{
	/// <summary>
	/// Скрипты по умолчанию
	/// </summary>
	public static class DefaultScripts{
		/// <summary>
		/// Начало скрипта SQL
		/// </summary>
		public static readonly string SqlServerCreatePeramble = @"
SET NOCOUNT ON
GO
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
GO
" + @"
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

".Replace("\r", " ").Replace("\n", " ").Replace("\t", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ") + @"
GO
";
		/// <summary>
		/// Финиширующий скрипт для SQL-server
		/// </summary>
		public const string SqlServerCreateFinisher = @"
IF OBJECT_ID('__ensurefg') IS NOT NULL DROP PROC __ensurefg
";
		/// <summary>
		/// Преамбуда POSTGRESQL скрипта
		/// </summary>
		public const string PostgresqlPeramble = @"
DROP FUNCTION IF EXISTS ___script();
CREATE FUNCTION ___script () returns int as $$ BEGIN
CREATE SCHEMA IF NOT EXISTS dbo ; --mssql matching
";
		/// <summary>
		/// Финиширующий скрипт для SQL-server
		/// </summary>
		public const string PostgresqlFinisher = @"
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
";
	}
}

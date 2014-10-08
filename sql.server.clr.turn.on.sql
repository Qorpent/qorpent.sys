use [master]
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_clr_turn_on]
    @database nvarchar(255)
AS
BEGIN
    declare @q nvarchar(max)
    set @q = N'ALTER DATABASE [' + @database+' ] SET TRUSTWORTHY ON'
    exec sp_executesql @q
    set @q = N'ALTER AUTHORIZATION ON database::[' + @database +'] TO "sa"'
    exec sp_executesql @q
    exec sp_configure 'show advanced options', 1
    exec sp_executesql N'RECONFIGURE'
    exec sp_configure 'clr enabled', 1
    exec sp_executesql N'RECONFIGURE'
END
GO
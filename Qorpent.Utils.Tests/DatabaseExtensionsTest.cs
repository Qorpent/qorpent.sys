using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class DatabaseExtensionsTest
	{

		[TestCase("UNISELECT x.y | a=~, b=~, c=23, d='u'", DatabaseEngineType.MySql, "CALL ALL `x_y` ( ?a, ?b, 23, 'u' )")]
		[TestCase("UNISELECT x.y | a=~, b=~, c=23, d='u'", DatabaseEngineType.Oracle, "SELECT * FROM x.\"y\" ( a => :a, b => :b, c => 23, d => 'u' )")]
		[TestCase("UNISELECT x.y | a=~, b=~, c=23, d='u'", DatabaseEngineType.Postgres, "SELECT * FROM x.\"y\" ( a := :a, b := :b, c := 23, d := 'u' )")]
		[TestCase("UNISELECT x.y | a=~, b=~, c=23, d='u'", DatabaseEngineType.SqlServer, "EXEC x.[y] @a = @a, @b = @b, @c = 23, @d = 'u'")]
		[TestCase("select * from s.[mytable] where x= ~y", DatabaseEngineType.MySql, "select * from `s_mytable` where x= ?y")]
		[TestCase("select * from s.[mytable] where x= ~y", DatabaseEngineType.Oracle, "select * from s.\"mytable\" where x= :y")]
		[TestCase("select * from s.[mytable] where x= ~y", DatabaseEngineType.Postgres, "select * from s.\"mytable\" where x= :y")]
		[TestCase("select * from s.[mytable] where x= ~y", DatabaseEngineType.SqlServer, "select * from s.[mytable] where x= @y")]
		[TestCase("UNISELECT x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.MySql, "CALL ALL `x_y` ( ?x, ?z, 23, 'u' )")]
		[TestCase("UNISELECT x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.Oracle, "SELECT * FROM x.\"y\" ( a => :x, b => :z, c => 23, d => 'u' )")]
		[TestCase("UNISELECT x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.Postgres, "SELECT * FROM x.\"y\" ( a := :x, b := :z, c := 23, d := 'u' )")]
		[TestCase("UNISELECT x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.SqlServer, "EXEC x.[y] @a = @x, @b = @z, @c = 23, @d = 'u'")]
		[TestCase("UNICALL x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.MySql, "CALL `x_y` ( ?x, ?z, 23, 'u' )")]
		[TestCase("UNICALL x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.Oracle, "SELECT x.\"y\" ( a => :x, b => :z, c => 23, d => 'u' )")]
		[TestCase("UNICALL x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.Postgres, "SELECT x.\"y\" ( a := :x, b := :z, c := 23, d := 'u' )")]
		[TestCase("UNICALL x.y | a=~x, b=~z, c=23, d='u'",DatabaseEngineType.SqlServer,"EXEC x.[y] @a = @x, @b = @z, @c = 23, @d = 'u'")]
		public void UnifiedSqlRewritingTest(string src, DatabaseEngineType dbtype, string result) {
			Assert.AreEqual(result,DbExtensions.RewriteSql(src,dbtype));
		}
	}

	[TestFixture]
	public class UniSqlTest {

		[TestCase(SqlCommandType.Select, DatabaseEngineType.MySql, "CALL ALL `x_y` ( ?x, ?b, 23, 'u' )")]
		[TestCase(SqlCommandType.Select, DatabaseEngineType.Oracle, "SELECT * FROM \"x\".\"y\" ( a => :x, b => :b, c => 23, d => 'u' )")]
		[TestCase(SqlCommandType.Select, DatabaseEngineType.Postgres, "SELECT * FROM \"x\".\"y\" ( a := :x, b := :b, c := 23, d := 'u' )")]
		[TestCase(SqlCommandType.Select, DatabaseEngineType.SqlServer, "EXEC [x].[y] @a = @x, @b = @b, @c = 23, @d = 'u'")]
		[TestCase(SqlCommandType.Call, DatabaseEngineType.MySql, "CALL `x_y` ( ?x, ?b, 23, 'u' )")]
		[TestCase(SqlCommandType.Call, DatabaseEngineType.Oracle, "SELECT \"x\".\"y\" ( a => :x, b => :b, c => 23, d => 'u' )")]
		[TestCase(SqlCommandType.Call, DatabaseEngineType.Postgres, "SELECT \"x\".\"y\" ( a := :x, b := :b, c := 23, d := 'u' )")]
		[TestCase(SqlCommandType.Call, DatabaseEngineType.SqlServer, "EXEC [x].[y] @a = @x, @b = @b, @c = 23, @d = 'u'")]
		public void CanCreateValidQueryOnAnyDatabaseCall(SqlCommandType calltype, DatabaseEngineType dbtype, string result) {
			var q = new UniSqlQuery(
				"x","y",calltype,new{a="~x",b="~",c=23,d="u"}
			);
			Assert.AreEqual(result,q.PrepareQueryText(dbtype));
		}
	}
}

﻿using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests
{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class DatabaseExtensionsTest
	{

		[TestCase("select * from s.[mytable] where x= ~y", DatabaseEngineType.MySql, "select * from s.`mytable` where x= ?y")]
		[TestCase("select * from s.[mytable] where x= ~y", DatabaseEngineType.Oracle, "select * from s.\"mytable\" where x= :y")]
		[TestCase("select * from s.[mytable] where x= ~y", DatabaseEngineType.Postgres, "select * from s.\"mytable\" where x= :y")]
		[TestCase("select * from s.[mytable] where x= ~y", DatabaseEngineType.SqlServer, "select * from s.[mytable] where x= @y")]
		[TestCase("UNISELECT x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.MySql, "CALL ALL x.`y` ( ?x, ?z, 23, 'u' )")]
		[TestCase("UNISELECT x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.Oracle, "SELECT * FROM x.\"y\" ( a => :x, b => :z, c => 23, d => 'u' )")]
		[TestCase("UNISELECT x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.Postgres, "SELECT * FROM x.\"y\" ( a := :x, b := :z, c := 23, d := 'u' )")]
		[TestCase("UNISELECT x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.SqlServer, "EXEC x.[y] @a = @x, @b = @z, @c = 23, @d = 'u'")]
		[TestCase("UNICALL x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.MySql, "SELECT x.`y` ( ?x, ?z, 23, 'u' )")]
		[TestCase("UNICALL x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.Oracle, "SELECT x.\"y\" ( a => :x, b => :z, c => 23, d => 'u' )")]
		[TestCase("UNICALL x.y | a=~x, b=~z, c=23, d='u'", DatabaseEngineType.Postgres, "SELECT x.\"y\" ( a := :x, b := :z, c := 23, d := 'u' )")]
		[TestCase("UNICALL x.y | a=~x, b=~z, c=23, d='u'",DatabaseEngineType.SqlServer,"EXEC x.[y] @a = @x, @b = @z, @c = 23, @d = 'u'")]
		public void UnifiedSqlRewritingTest(string src, DatabaseEngineType dbtype, string result) {
			Assert.AreEqual(result,DbExtensions.RewriteSql(src,dbtype));
		}
	}
}

using System;
using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests {
	/// <summary>
	/// </summary>
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
				"x", "y", new { a = "~x", b = "~", c = 23, d = "u" }, calltype
				);
			Assert.AreEqual(result,q.PrepareQueryText(dbtype));
		}
		[Test]
		public void CanSupplyValidParameters() {
			var q = new UniSqlQuery(
				"x", "y", new { a = "~x", b = "~", c = 23, d = "u" }, SqlCommandType.Call
				);

			var con = new SqlConnection();

			var cmd = q.PrepareCommand(con, new {x = 1});

			Assert.AreEqual(cmd.CommandText,q.PrepareQueryText(DatabaseEngineType.SqlServer));

			Assert.AreEqual(2,cmd.Parameters.Count);
			Assert.AreEqual(1,((IDbDataParameter)cmd.Parameters["x"]).Value);
			Assert.AreEqual(DBNull.Value, ((IDbDataParameter)cmd.Parameters["b"]).Value);
		}
	}
}
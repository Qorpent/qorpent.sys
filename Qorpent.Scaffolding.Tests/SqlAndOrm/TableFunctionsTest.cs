using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.SqlWriters;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm{
	[TestFixture]
	public class TableFunctionsTest{
		private const string SimplestTableFunction = @"
class A persistent
	string X
class B persistent 
	A* GetA @id=^B sql-method=cacheable : (
		insert @result (id) select id from A
	)
";
		[Test]
		public void CanParseTableFunction(){
			var model = PersistentModel.Compile(SimplestTableFunction);
			Assert.True(model.IsValid,"Модель должна быть валидна");
			var b = model["B"];
			Assert.NotNull(b,"А где B?");
			var a = model["A"];
			Assert.NotNull(b, "А где A?");
			var geta = b.SqlObjects.FirstOrDefault(_ => _.Name == "GetA") as SqlFunction;
			Assert.NotNull(geta,"Функция должна быть прочитана и инициализирована");
			var dt = geta.ReturnType;
			Assert.True(dt.IsTable, "Тип данных должен быть табличным");
			Assert.AreEqual(a,dt.TargetType,"Тип возвращаемой таблицы должен быть A");
			var arg = geta.Arguments["id"];
			Assert.NotNull(arg,"Должен был считать аргумент @id");
			Assert.True(arg.DataType.IsIdRef,"Тип аргумента должен быть ссылкой на Id");
			Assert.AreEqual(b,arg.DataType.TargetType,"Тип ссылки по ID должен быть B");
		}

		/// <summary>
		/// Проверяем генератор SQL
		/// </summary>
		[Test]
		public void SqlGenerationTest(){
			var model = PersistentModel.Compile(SimplestTableFunction);
			var func = model["B"][SqlObjectType.Function, "GetA"];
			var sqlfuncwriter = SqlCommandWriter.Create(func);
			sqlfuncwriter.Dialect = SqlDialect.SqlServer;
			var code = sqlfuncwriter.ToString();
			Console.WriteLine(code);
			Console.WriteLine(code.Replace("\"","\"\""));
			Assert.AreEqual(@"-- begin command SqlFunctionWriter
IF OBJECT_ID('""dbo"".""bGetA""') IS NOT NULL DROP FUNCTION ""dbo"".""bGetA"";
GO
CREATE FUNCTION ""dbo"".""bGetA"" ( @id int = null  )
RETURNS @result TABLE (""id"" int, ""x"" nvarchar(255)) AS BEGIN
insert @result (id) select id from A
RETURN;
END;
GO".Trim().LfOnly(), code.Trim().LfOnly());
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void AdapterGotGeneratedSqlMethod(){
			
		}
	}
}
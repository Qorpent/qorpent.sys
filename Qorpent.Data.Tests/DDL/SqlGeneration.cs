using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.Data.BSharpDDL;
using Qorpent.Uson;

namespace Qorpent.Data.Tests.DDL
{
	/// <summary>
	/// Генерация SQL
	/// </summary>
	[TestFixture]
	public class SqlGeneration
	{
		
		[TestCase("int Id","Id int NOT NULL DEFAULT 0")]
		[TestCase("string Code","Code nvarchar(255) NOT NULL DEFAULT ''")]
		[TestCase("shortstring Code","Code nvarchar(20) NOT NULL DEFAULT ''")]
		public void BxlFieldToSql(string bxl, string sql){
			var xml = new BxlParser().Parse(bxl).Elements().First();
			var types = DbDataType.GetDefaultTypes();
			var type = types[xml.Name.LocalName];
			var fld = DbField.CreateField(null, type, xml);
			Assert.AreEqual(sql,fld.GetSql());
		}
		[Test]
		public void DictionaryTest(){
			var code = @"
require data
using dict=Qorpent.Db.DataDictionary
dict Category 'дерево категорий' schema=x	
";

			var bs = BSharpCompiler.CreateDefault();
			var src = new BxlParser().Parse(code);
			var result = bs.Compile(new[] { src });
			var objects = result.Get(BSharpContextDataType.Working).SelectMany(_ => DbObject.Create(_, result));
			Console.WriteLine(DbObject.GetSql(objects));
		}

		[TestCase(DbGenerationMode.Script | DbGenerationMode.Safe)]
		[TestCase(DbGenerationMode.Script)]
		public void CreateTableFromBSharp(DbGenerationMode mode){
			var code = @"
require data
using table=Qorpent.Db.Table
table mytable abstract
	schema=x
mytable first 'Просто табличка для примера'
	string Code unique
	int SecondId ref=second.Id
mytable second 'Вторая табличка для примера'
	string Code unique
	
";

			var bs = BSharpCompiler.CreateDefault();
			var src = new BxlParser().Parse(code);
			var result = bs.Compile(new[]{src});
			var objects = result.Get(BSharpContextDataType.Working).SelectMany(_=>DbObject.Create(_,result));
			Console.WriteLine(DbObject.GetSql(objects,mode));
			
			
		}
	}
}

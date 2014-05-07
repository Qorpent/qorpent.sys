using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.Data.DataDiff;

namespace Qorpent.Data.Tests.DataDiff
{
	[TestFixture]
	public class BasicDiffTest
	{

		[Test]
		public void CanUpdateTree(){
			var diff = GetDiffString(
@"
x table=A tree
	a x n
		b y n2
		b y2 n3
",
@"
x table=A tree
	a x n
		b y3 n4
	b y2 n3
");
			Assert.AreEqual(
@"Update table : A
Sources : 2
Definitions : 2
	Id: 0; Code: y2; set_parent: ; 
	Id: 0; Code: y3; name: n4; set_parent: a-code-x; 
", diff);
		}


		[Test]
		public void CanUpdateTreeSql()
		{
			var diff = GetSqlString(
@"
x table=A tree
	a x n
		b y n2
		b y2 n3
",
@"
x table=A tree
	a x n
		b y3 n4
	b y2 n3
");
			Assert.AreEqual(
@"Update table : A
Sources : 2
Definitions : 2
	Id: 0; Code: y2; set_parent: ; 
	Id: 0; Code: y3; name: n4; set_parent: a-code-x; 
", diff);
		}

		[Test]
		public void CreateNewElement()
		{
			var diff = GetDiffString(
@"
x table=A
",
@"
x table=A
	i x n2
");
			Assert.AreEqual(
@"Update table : A
Sources : 1
Definitions : 1
	Id: 0; Code: x; name: n2; 
", diff);
		}

		[Test]
		public void CreateNewElementSql()
		{
			var diff = GetSqlString(
@"
x table=A
",
@"
x table=A
	i x n2
");
			Assert.AreEqual(
@"Update table : A
Sources : 1
Definitions : 1
	Id: 0; Code: x; name: n2; 
", diff);
		}

		[Test]
		public void SingleFieldUpdate(){
			var diff = GetDiffString(
@"
x table=A
	i x n
",
@"
x table=A
	i x n2
");
			Assert.AreEqual(
@"Update table : A
Sources : 1
Definitions : 1
	Id: 0; Code: x; name: n2; 
",diff);
		}

		[Test]
		public void SingleFieldUpdateSql()
		{
			var diff = GetSqlString(
@"
x table=A
	i x n
",
@"
x table=A
	i x n2
");
			Assert.AreEqual(
@"Update table : A
Sources : 1
Definitions : 1
	Id: 0; Code: x; name: n2; 
", diff);
		}

		[Test]
		public void MultipleFieldUpdate()
		{
			var diff = GetDiffString(
@"
x table=A
	i x n
",
@"
x table=A
	i x n2 x=1 y=2
");
			Assert.AreEqual(
@"Update table : A
Sources : 1
Definitions : 1
	Id: 0; Code: x; name: n2; x: 1; y: 2; 
", diff);
		}


		[Test]
		public void MultipleFieldUpdatedSql()
		{
			var diff = GetSqlString(
@"
x table=A
	i x n
",
@"
x table=A
	i x n2 x=1 y=2
");
			Assert.AreEqual(
@"Update table : A
Sources : 1
Definitions : 1
	Id: 0; Code: x; name: n2; x: 1; y: 2; 
", diff);
		}

		[Test]
		public void WithSettedAttributesUpdate()
		{
			var diff = GetDiffString(
@"
x table=A
	i x n	
",
@"
x table=A
	i x x=1 y=2 update-id=23 update-code=x2
");
			Assert.AreEqual(
@"Update table : A
Sources : 1
Definitions : 1
	Id: 0; Code: x; set_code: x2; set_id: 23; x: 1; y: 2; 
", diff);
		}

		[Test]
		public void WithSettedAttributesUpdateSql()
		{
			var diff = GetSqlString(
@"
x table=A
	i x n	
",
@"
x table=A
	i x x=1 y=2 update-id=23 update-code=x2
");
			Assert.AreEqual(
@"Update table : A
Sources : 1
Definitions : 1
	Id: 0; Code: x; set_code: x2; set_id: 23; x: 1; y: 2; 
", diff);
		}

		[Test]
		public void MultitableWithDynamic()
		{
			var diff = GetDiffString(
@"
x
	i x n	
	j y n
",
@"
x
	i x n2
	j y n2
");
			Assert.AreEqual(
@"Update table : i
Sources : 1
Definitions : 1
	Id: 0; Code: x; name: n2; 

-----
Update table : j
Sources : 1
Definitions : 1
	Id: 0; Code: y; name: n2; 
", diff);
		}

		[Test]
		public void MultitableWithDynamicSql()
		{
			var diff = GetSqlString(
@"
x
	i x n	
	j y n
",
@"
x
	i x n2
	j y n2
");
			Assert.AreEqual(
@"Update table : i
Sources : 1
Definitions : 1
	Id: 0; Code: x; name: n2; 

-----
Update table : j
Sources : 1
Definitions : 1
	Id: 0; Code: y; name: n2; 
", diff);
		}

		private string GetDiffString(string basis, string updated){
			var result = GetDiff(basis, updated);
			var str = string.Join("\r\n-----\r\n",result.OrderBy(_ => _.TableName));
			Console.WriteLine(str);
			return str;
		}

		private string GetSqlString(string basis, string updated){
			var tables = GetDiff(basis, updated);
			var sw = new StringWriter();
			var ctx = new TableDiffGeneratorContext{Tables = tables, SqlOutput = sw};
			new SqlDiffGenerator(ctx).GenerateScript();
			Console.WriteLine(sw.ToString());
			return sw.ToString();
		}

		private static IEnumerable<DataDiffTable> GetDiff(string basis, string updated){
			
			var bx = basis.StartsWith("<") ? XElement.Parse(basis) : new BxlParser().Parse(basis).Elements().First();
			var up = basis.StartsWith("<") ? XElement.Parse(updated) : new BxlParser().Parse(updated).Elements().First();
			var diff = new DiffPair{Base = bx, Updated = up};
			var context = new TableDiffGeneratorContext{DiffPairs =new[]{ diff}};
			new DataTableDiffGenerator(context).GetTableDiff();
			return context.Tables;
		}
	}
}

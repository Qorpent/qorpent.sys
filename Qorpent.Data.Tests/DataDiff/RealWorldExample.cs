using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Data.DataDiff;

namespace Qorpent.Data.Tests.DataDiff
{
	[TestFixture]
	[Explicit]
	public class RealWorldExample
	{
		/// <summary>
		/// 
		/// </summary>
		[Test]
		[Explicit]
		public void TryGenerateDiffPairsFromAssoiHead(){
			var ctx = new TableDiffGeneratorContext{
				RootDirectory = @"c:\z3projects\assoi\DataDiff",
				GitUrl = @"c:\z3projects\assoi\publish",
				ProjectDirectory = @"Draft/db/migration",
				OutputDirectory = @"Draft/db/migration/.output",
				GitBranch = @"assoi_publish"
			};
			new DiffPairGenerator(ctx).Generate();
			Assert.Greater(ctx.DiffPairs.Count(), 5);
		}

		[Test]
		[Explicit]
		public void TryGenerateDiffPairsBetweenKnownRevisions()
		{
			var ctx = new TableDiffGeneratorContext
			{
				RootDirectory = @"c:\z3projects\assoi\DataDiff",
				GitUrl = @"c:\z3projects\assoi\publish",
				ProjectDirectory = @"Draft/db/migration",
				OutputDirectory = @"Draft/db/migration/.output",
				GitBranch = @"assoi_publish",
				GitBaseRevision = "9dbe04b",
				GitUpdateRevision = "03cd073"
			};
			new DiffPairGenerator(ctx).Generate();
			Assert.AreEqual(0,ctx.DiffPairs.Count()); //там не было значимых изменений
		}
		[Test]
		[Explicit]
		public void WellKnownChanges(){
			var sw = new StringWriter();
			var ctx = new TableDiffGeneratorContext
			{
				RootDirectory = @"c:\z3projects\assoi\DataDiff",
				GitUrl = @"c:\z3projects\assoi\publish",
				ProjectDirectory = @"Draft/db/migration",
				OutputDirectory = @"Draft/db/migration/.output",
				GitBranch = @"assoi_publish",
				GitBaseRevision = "9dbe04b",
				GitUpdateRevision = "8e5e349",
				SqlOutput = sw
			};
			new DiffPairGenerator(ctx).Generate();
			Assert.AreEqual(1,ctx.DiffPairs.Count()); //там должен поменяться один файл
			new DataTableDiffGenerator(ctx).Generate();
			new SqlDiffGenerator(ctx).Generate();

			Console.WriteLine(sw.ToString());

			Assert.AreEqual(@"BEGIN TRAN
	 --table for storing check constraints problems
	declare @c table (t nvarchar(255), c nvarchar(255), w nvarchar(255))
BEGIN TRY
--switch off foreign keys
	alter table z3.Industry nocheck constraint all
	--z3.Industry base insert and update
	declare @t1 table ( id int, code nvarchar(255), _exists bit default 0 , set_code nvarchar(max))
		insert @t1 (id,code,set_code) values
			('17','ЦМ', 'CVM'),
			('20','ЧМ', 'BLM'),
			('21','УП', 'CLP'),
			('22','СО', 'BLD'),
			('23','МАШ', 'MSH'),
			('24','ЦМО', 'CVO'),
			('25','КАБ', 'CBL'),
			('26','СХ', 'AGR'),
			('27','СМИ', 'SMI'),
			('28','Связь', 'SVZ'),
			('29','ФИН', 'FIN'),
			('30','Наука', 'SNC'),
			('31','ЭН', 'NRG'),
			('33','ТР', 'TRN'),
			('34','МЕД', 'MED'),
			('35','СМ', 'SMT')
			update @t1 set id = this.id, code=this.code, _exists =1 from z3.Industry this join @t1 temp on (temp.code = this.code or temp.id=this.id)
			insert z3.Industry (id,code) select id,isnull(code,id) from @t1 where _exists = 0 and id is not null
			insert z3.Industry (code) select code from @t1 where _exists = 0 and code is not null and id is null
			update @t1 set id = this.id, code=this.code, _exists =1 from z3.Industry this join @t1 temp on (temp.code = this.code or temp.id=this.id)
	--z3.Industry fk_binding
	--z3.Industry main update
		update z3.Industry set code = isnull(x.set_code,z3.Industry.code)from @t1 x join z3.Industry on x.id = z3.Industry.id 
	-- return of foreign keys contraints
	alter table z3.Industry check constraint all
	insert @c (t,c,w) exec sp_executesql N'DBCC CHECKCONSTRAINTS (''z3.Industry'')'
	-- this code allows late control over foreighn keys
		if ((select count(*) from @c)!=0) begin
		select * from @c;
		throw 51012 , 'patch violates constraints', 1;
	end else commit
END TRY
BEGIN CATCH
	if (@@TRANCOUNT!=0) rollback;
	throw
END CATCH
",sw.ToString());
		}
	}
}

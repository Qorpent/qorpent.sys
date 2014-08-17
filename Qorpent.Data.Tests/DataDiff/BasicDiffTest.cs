using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.Data.DataDiff;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.Tests.DataDiff
{
	[TestFixture]
	public class BasicDiffTest
	{

		[Test]
		public void CanUpdateTree(){
			var diff = GetDiffString(
@"
x table=test tree
	a x n
		b y n2
		b y2 n3
",
@"
x table=test tree
	a x n
		b y3 n4
	b y2 n3
");
			Assert.AreEqual(
@"Update table : test
Sources : 3
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
x table=test tree
	a x n
		b y n2
		b y2 n3
",
@"
x table=test tree
	a x n
		b y3 n4
	b y2 n3
");
			Assert.AreEqual(
@"-- START OF SCRIPT: MAIN
BEGIN TRAN
	 --table for storing check constraints problems
	declare @c table (t nvarchar(255), c nvarchar(255), w nvarchar(255))
BEGIN TRY
--switch off foreign keys
	alter table test nocheck constraint all
	--test base insert and update
	declare @t1 table ( id int, code nvarchar(255), _exists bit default 0 , name nvarchar(max), parent int, parent_code nvarchar(255))
		insert @t1 (id, code,name,parent ,parent_code) values
			(null,'y2', null, 0, null),
			(null,'y3', 'n4', null, 'x')
			update @t1 set id = this.id, code=this.code, _exists =1 from test this join @t1 temp on (temp.code = this.code or temp.id=this.id)
			insert test (id,code) select id,isnull(code,id) from @t1 where _exists = 0 and id is not null
			insert test (code) select code from @t1 where _exists = 0 and code is not null and id is null
			update @t1 set id = this.id, code=this.code, _exists =1 from test this join @t1 temp on (temp.code = this.code or temp.id=this.id)
	--test fk_binding
		update @t1 set parent = isnull((select id from test where test.code = parent_code ),-1) where parent is null and parent_code is not null 
	--test main update
		update test set name = isnull(x.name,test.name), parent = isnull(x.parent,test.parent)from @t1 x join test on x.id = test.id 
	-- return of foreign keys contraints
	alter table test check constraint all
	insert @c (t,c,w) exec sp_executesql N'DBCC CHECKCONSTRAINTS (''test'')'
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
-- END OF SCRIPT: MAIN
", diff);
		}

		[Test]
		public void CreateNewElement()
		{
			var diff = GetDiffString(
@"
x table=test
",
@"
x table=test
	i x n2
");
			Assert.AreEqual(
@"Update table : test
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
x table=test
",
@"
x table=test
	i x n2
");
			Assert.AreEqual(
@"-- START OF SCRIPT: MAIN
BEGIN TRAN
	 --table for storing check constraints problems
	declare @c table (t nvarchar(255), c nvarchar(255), w nvarchar(255))
BEGIN TRY
--switch off foreign keys
	alter table test nocheck constraint all
	--test base insert and update
	declare @t1 table ( id int, code nvarchar(255), _exists bit default 0 , name nvarchar(max))
		insert @t1 (id, code,name) values
			(null,'x', 'n2')
			update @t1 set id = this.id, code=this.code, _exists =1 from test this join @t1 temp on (temp.code = this.code or temp.id=this.id)
			insert test (id,code) select id,isnull(code,id) from @t1 where _exists = 0 and id is not null
			insert test (code) select code from @t1 where _exists = 0 and code is not null and id is null
			update @t1 set id = this.id, code=this.code, _exists =1 from test this join @t1 temp on (temp.code = this.code or temp.id=this.id)
	--test fk_binding
	--test main update
		update test set name = isnull(x.name,test.name)from @t1 x join test on x.id = test.id 
	-- return of foreign keys contraints
	alter table test check constraint all
	insert @c (t,c,w) exec sp_executesql N'DBCC CHECKCONSTRAINTS (''test'')'
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
-- END OF SCRIPT: MAIN
".Trim().LfOnly(), diff.Trim().LfOnly());
		}

		[Test]
		public void SingleFieldUpdate(){
			var diff = GetDiffString(
@"
x table=test
	i x n
",
@"
x table=test
	i x n2
");
			Assert.AreEqual(
@"Update table : test
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
x table=test
	i x n
",
@"
x table=test
	i x n2
");
			Assert.AreEqual(
@"-- START OF SCRIPT: MAIN
BEGIN TRAN
	 --table for storing check constraints problems
	declare @c table (t nvarchar(255), c nvarchar(255), w nvarchar(255))
BEGIN TRY
--switch off foreign keys
	alter table test nocheck constraint all
	--test base insert and update
	declare @t1 table ( id int, code nvarchar(255), _exists bit default 0 , name nvarchar(max))
		insert @t1 (id, code,name) values
			(null,'x', 'n2')
			update @t1 set id = this.id, code=this.code, _exists =1 from test this join @t1 temp on (temp.code = this.code or temp.id=this.id)
			insert test (id,code) select id,isnull(code,id) from @t1 where _exists = 0 and id is not null
			insert test (code) select code from @t1 where _exists = 0 and code is not null and id is null
			update @t1 set id = this.id, code=this.code, _exists =1 from test this join @t1 temp on (temp.code = this.code or temp.id=this.id)
	--test fk_binding
	--test main update
		update test set name = isnull(x.name,test.name)from @t1 x join test on x.id = test.id 
	-- return of foreign keys contraints
	alter table test check constraint all
	insert @c (t,c,w) exec sp_executesql N'DBCC CHECKCONSTRAINTS (''test'')'
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
-- END OF SCRIPT: MAIN
", diff);
		}

		[Test]
		public void MultipleFieldUpdate()
		{
			var diff = GetDiffString(
@"
x table=test
	i x n
",
@"
x table=test
	i x n2 x=1 y=2
");
			Assert.AreEqual(
@"Update table : test
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
x table=test
	i x n
",
@"
x table=test
	i x n2 x=1 y=2
");
			Assert.AreEqual(
@"-- START OF SCRIPT: MAIN
BEGIN TRAN
	 --table for storing check constraints problems
	declare @c table (t nvarchar(255), c nvarchar(255), w nvarchar(255))
BEGIN TRY
--switch off foreign keys
	alter table test nocheck constraint all
	--test base insert and update
	declare @t1 table ( id int, code nvarchar(255), _exists bit default 0 , name nvarchar(max), x nvarchar(max), y nvarchar(max))
		insert @t1 (id, code,name,x,y) values
			(null,'x', 'n2', '1', '2')
			update @t1 set id = this.id, code=this.code, _exists =1 from test this join @t1 temp on (temp.code = this.code or temp.id=this.id)
			insert test (id,code) select id,isnull(code,id) from @t1 where _exists = 0 and id is not null
			insert test (code) select code from @t1 where _exists = 0 and code is not null and id is null
			update @t1 set id = this.id, code=this.code, _exists =1 from test this join @t1 temp on (temp.code = this.code or temp.id=this.id)
	--test fk_binding
	--test main update
		update test set name = isnull(x.name,test.name), x = isnull(x.x,test.x), y = isnull(x.y,test.y)from @t1 x join test on x.id = test.id 
	-- return of foreign keys contraints
	alter table test check constraint all
	insert @c (t,c,w) exec sp_executesql N'DBCC CHECKCONSTRAINTS (''test'')'
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
-- END OF SCRIPT: MAIN
".Trim().LfOnly(), diff.Trim().LfOnly());
		}

		[Test]
		public void WithSettedAttributesUpdate()
		{
			var diff = GetDiffString(
@"
x table=test
	i x n	
",
@"
x table=test
	i x x=1 y=2 update-id=23 update-code=x2
",emptychange:false);
			Assert.AreEqual(
@"Update table : test
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
x table=test
	i x n	
",
@"
x table=test
	i x x=1 y=2 update-id=23 update-code=x2
",emptyaschange:false);
			Assert.AreEqual(
@"-- START OF SCRIPT: MAIN
BEGIN TRAN
	 --table for storing check constraints problems
	declare @c table (t nvarchar(255), c nvarchar(255), w nvarchar(255))
BEGIN TRY
--switch off foreign keys
	alter table test nocheck constraint all
	--test base insert and update
	declare @t1 table ( id int, code nvarchar(255), _exists bit default 0 , set_code nvarchar(max), set_id nvarchar(max), x nvarchar(max), y nvarchar(max))
		insert @t1 (id, code,set_code,set_id,x,y) values
			(null,'x', 'x2', '23', '1', '2')
			update @t1 set id = this.id, code=this.code, _exists =1 from test this join @t1 temp on (temp.code = this.code or temp.id=this.id)
			insert test (id,code) select id,isnull(code,id) from @t1 where _exists = 0 and id is not null
			insert test (code) select code from @t1 where _exists = 0 and code is not null and id is null
			update @t1 set id = this.id, code=this.code, _exists =1 from test this join @t1 temp on (temp.code = this.code or temp.id=this.id)
	--test fk_binding
	--test main update
		update test set code = isnull(x.set_code,test.code), id = isnull(x.set_id,test.id), x = isnull(x.x,test.x), y = isnull(x.y,test.y)from @t1 x join test on x.id = test.id 
	-- return of foreign keys contraints
	alter table test check constraint all
	insert @c (t,c,w) exec sp_executesql N'DBCC CHECKCONSTRAINTS (''test'')'
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
-- END OF SCRIPT: MAIN
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
@"-- START OF SCRIPT: MAIN
BEGIN TRAN
	 --table for storing check constraints problems
	declare @c table (t nvarchar(255), c nvarchar(255), w nvarchar(255))
BEGIN TRY
--switch off foreign keys
	alter table i nocheck constraint all
	alter table j nocheck constraint all
	--i base insert and update
	declare @t1 table ( id int, code nvarchar(255), _exists bit default 0 , name nvarchar(max))
		insert @t1 (id, code,name) values
			(null,'x', 'n2')
			update @t1 set id = this.id, code=this.code, _exists =1 from i this join @t1 temp on (temp.code = this.code or temp.id=this.id)
			insert i (id,code) select id,isnull(code,id) from @t1 where _exists = 0 and id is not null
			insert i (code) select code from @t1 where _exists = 0 and code is not null and id is null
			update @t1 set id = this.id, code=this.code, _exists =1 from i this join @t1 temp on (temp.code = this.code or temp.id=this.id)
	--j base insert and update
	declare @t2 table ( id int, code nvarchar(255), _exists bit default 0 , name nvarchar(max))
		insert @t2 (id, code,name) values
			(null,'y', 'n2')
			update @t2 set id = this.id, code=this.code, _exists =1 from j this join @t2 temp on (temp.code = this.code or temp.id=this.id)
			insert j (id,code) select id,isnull(code,id) from @t2 where _exists = 0 and id is not null
			insert j (code) select code from @t2 where _exists = 0 and code is not null and id is null
			update @t2 set id = this.id, code=this.code, _exists =1 from j this join @t2 temp on (temp.code = this.code or temp.id=this.id)
	--i fk_binding
	--j fk_binding
	--i main update
		update i set name = isnull(x.name,i.name)from @t1 x join i on x.id = i.id 
	--j main update
		update j set name = isnull(x.name,j.name)from @t2 x join j on x.id = j.id 
	-- return of foreign keys contraints
	alter table i check constraint all
	alter table j check constraint all
	insert @c (t,c,w) exec sp_executesql N'DBCC CHECKCONSTRAINTS (''i'')'
	insert @c (t,c,w) exec sp_executesql N'DBCC CHECKCONSTRAINTS (''j'')'
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
-- END OF SCRIPT: MAIN
".Trim().LfOnly(), diff.Trim().LfOnly());
		}

		private string GetDiffString(string basis, string updated, bool emptychange = true){
			var result = GetDiff(basis, updated,emptychange);
			var str = string.Join("\r\n-----\r\n",result.OrderBy(_ => _.TableName));
			Console.WriteLine(str);
			return str;
		}

		private string GetSqlString(string basis, string updated, bool emptyaschange = true){
			var tables = GetDiff(basis, updated,emptyaschange);
			var sw = new StringWriter();
			var ctx = new TableDiffGeneratorContext{Tables = tables, SqlOutput = sw,EmptyAttributesAsUpdates =emptyaschange};
			new SqlDiffGenerator(ctx).Generate();
			Console.WriteLine(sw.ToString());
			return sw.ToString();
		}

		private static IEnumerable<DataDiffTable> GetDiff(string basis, string updated, bool emptyaschange = true){
			
			var bx = basis.StartsWith("<") ? XElement.Parse(basis) : new BxlParser().Parse(basis).Elements().First();
			var up = basis.StartsWith("<") ? XElement.Parse(updated) : new BxlParser().Parse(updated).Elements().First();
			var diff = new DiffPair{Base = bx, Updated = up};
			var context = new TableDiffGeneratorContext{DiffPairs =new[]{ diff},EmptyAttributesAsUpdates = emptyaschange};
			new DataTableDiffGenerator(context).Generate();
			return context.Tables;
		}
	}
}

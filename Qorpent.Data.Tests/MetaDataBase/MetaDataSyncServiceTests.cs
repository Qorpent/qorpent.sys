using System;
using NUnit.Framework;
using Qorpent.Data.MetaDataBase;

namespace Qorpent.Data.Tests.MetaDataBase{
	[TestFixture]
	public class MetaDataSyncServiceTests
	{
		private MetaDataSyncService sync;

		[SetUp]
		public void Setup(){
			sync = new MetaDataSyncService();
			sync.TargetStorage = new InMemoryMetaFileRegistry();
			sync.SourceStorage = new InMemoryMetaFileRegistry();
			
		}

		[Test]
		public void SimpleInsertSingleRow(){
			sync.SourceStorage.Register(new MetaFileDescriptor(){Code="file1", Content = @"
x.t id=1 code1  name1 
"
			});
			var sql = sync.GetUpdateScript();
			Console.WriteLine(sql);
			StringAssert.Contains(@"if exists (select top 1 id from x.t where Id = 1) 
update x.t set Code = 'code1', Name = 'name1', Active = 1, Version = (getdate()) where Id = 1
else insert x.t (Id, Code, Name, Active, Version) values (1, 'code1', 'name1', 1, (getdate()))
", sql);

		}


		[Test]
		public void CanJoinUpdatesInSingleFile()
		{
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "file1",
				Content = @"
x.t id=1 code1  name1 
x.t code1 ShortName=x
"
			});
			var sql = sync.GetUpdateScript();
			Console.WriteLine(sql);
			StringAssert.Contains(@"if exists (select top 1 id from x.t where Id = 1) 
update x.t set Code = 'code1', Name = 'name1', ShortName = 'x', Active = 1, Version = (getdate()) where Id = 1
else insert x.t (Id, Code, Name, ShortName, Active, Version) values (1, 'code1', 'name1', 'x', 1, (getdate()))", sql);

		}

		[Test]
		public void CanJoinUpdatesInDistinctFiles()
		{
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "file1",
				Content = @"
x.t id=1 code1  name1 
"
			});

			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "file2",
				Content = @"
x.t code1 ShortName=x
"
			});
			var sql = sync.GetUpdateScript();
			Console.WriteLine(sql);
			StringAssert.Contains(@"if exists (select top 1 id from x.t where Id = 1) 
update x.t set Code = 'code1', Name = 'name1', ShortName = 'x', Active = 1, Version = (getdate()) where Id = 1
else insert x.t (Id, Code, Name, ShortName, Active, Version) values (1, 'code1', 'name1', 'x', 1, (getdate()))", sql);

		}
		[Test]
		[Explicit]
		public void RealZ3ColTable(){
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
z3.Col id=55 Б1 'Основная старая колонка'
z3.Col Pd 'Приход'
z3.Col Kol 'Количество' Currency='NAT'
z3.Currency NAT
"
			});
			var sql = sync.GetUpdateScript();
			Console.WriteLine(sql);

			sync.SourceStorage = new InMemoryMetaFileRegistry();
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
data schema=z3
	col id=55 Б1 'Основная старая колонка'
	Col Pd 'Приход'
	col Kol 'Количество' Currency='NAT'
	currency NAT
"
			});
			var sql2 = sync.GetUpdateScript();
			Console.WriteLine(sql2);

			Assert.AreEqual(sql,sql2);

		}


		[Test]
		public void ChangeIdsWork()
		{


			sync.SourceStorage = new InMemoryMetaFileRegistry();
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
data change-ids=true
	col id=55 'x'
"
			});
			var sql2 = sync.GetUpdateScript();
			Console.WriteLine(sql2);
			StringAssert.Contains(@"if exists (select top 1 id from dbo.col where Code = 'x') 
update dbo.col set Id = 55, Active = 1, Version = (getdate()) where Code = 'x'
else insert dbo.col (Id, Code, Active, Version) values (55, 'x', 1, (getdate()))",sql2);


			sync.SourceStorage = new InMemoryMetaFileRegistry();
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
data
	col id=55 'x'
"
			});
			sql2 = sync.GetUpdateScript();
			Console.WriteLine(sql2);
			StringAssert.Contains(@"if exists (select top 1 id from dbo.col where Id = 55) 
update dbo.col set Code = 'x', Active = 1, Version = (getdate()) where Id = 55
else insert dbo.col (Id, Code, Active, Version) values (55, 'x', 1, (getdate()))", sql2);
		}
		[Test]
		public void CanSupplyHierarchy(){
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
data tree=1
	a x
		a y
			a z"
			});
			var sql = sync.GetUpdateScript();
			Console.WriteLine(sql);
			StringAssert.Contains(@"update dbo.a set Parent = (select Id from dbo.a where Code = 'y') where Code = 'z'", sql);
		}

		[Test]
		[Ignore("not well defined behavior")]
		public void CanBlockDoubleIds()
		{
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
a id=2 x
a id=2 y"
			});
			Assert.Throws<Exception>(()=> sync.GetUpdateScript());
		}

		[Test]
		[Ignore("not well defined behavior")]
		public void CanBlockDoubleCodes()
		{
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
a id=2 x
a id=3 x"
			});
			Assert.Throws<Exception>(() => sync.GetUpdateScript());
		}

		

		[Test]
		public void CanSupplyHierarchyIgnoreNames()
		{
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
data tree=1 schema=dbo table=a
	b x
		c y
			d z"
			});
			var sql = sync.GetUpdateScript();
			Console.WriteLine(sql);
			StringAssert.Contains(@"update dbo.a set Parent = (select Id from dbo.a where Code = 'y') where Code = 'z'", sql);
		}

		[Test]
		public void CanUpdateHierarchy()
		{
			sync.TargetStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
data tree=1
	a x
		a y
			a z", RevisionTime = new DateTime(1901,1,1)
			});
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
data tree=1
	a x
		a y
		a z"
			});
			var sql = sync.GetUpdateScript();
			Console.WriteLine(sql);
			StringAssert.Contains(@"update dbo.a set Parent = (select Id from dbo.a where Code = 'x') where Code = 'z'", sql);
		}


		[Test]
		[Explicit]
		public void RealZ3ObjWithParentTable()
		{
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
data schema=z3 ref-z3-obj-parent=z3.obj
	obj KUEM4 'b' parent = KUEM3
	obj KUEM2 'a' parent = 352
	obj KUEM3 'b' parent = KUEM
	obj id=352 KUEM 'ОАО ""Уралэлектромедь""'
	col id=55 Б1 'Основная старая колонка'
	Col Pd 'Приход'
	col Kol 'Количество' Currency='NAT'
	currency NAT
"
			});
			var sql = sync.GetUpdateScript();
			Console.WriteLine(sql);
		}


		[Test]
		[Explicit]
		public void Z3ObjHierarchy()
		{
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
data schema=z3 tree=1 
	obj id=352 KUEM 'ОАО ""Уралэлектромедь""'	
		obj KUEM2 'a'
		obj KUEM3 'b'
			obj KUEM4 'b'
	col id=55 Б1 'Основная старая колонка'
	Col Pd 'Приход'
	col Kol 'Количество' Currency='NAT'
	currency NAT
"
			});
			var sql = sync.GetUpdateScript();
			Console.WriteLine(sql);
		}

		[Test]
		[Explicit]
		public void RealZ3ObjWithParentTable2()
		{
			sync.TargetStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
data schema=z3
	obj KUEM4 'b' parent = KUEM3
	obj KUEM2 'a' parent = 352
	obj KUEM3 'b' parent = KUEM
	obj id=352 KUEM 'ОАО ""Уралэлектромедь""'
	col id=55 Б1 'Основная старая колонка'
	Col Pd 'Приход'
	col Kol 'Количество' Currency='NAT'
	currency NAT
", RevisionTime = new DateTime(1901,1,1)
			});
			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "Columns",
				Content = @"
data schema=z3
	obj KUEM4 'b' parent = KUEM3
	obj KUEM2 'Полиметаллы' parent = 352
	obj KUEM3 'b' parent = KUEM
	obj id=352 KUEM 'ОАО ""Уралэлектромедь""' ShortName=""УЭМ""
	col id=55 Б1 'Основная старая колонка'
	Col Pd 'Приход'
	col Kol 'Количество новое' Currency='NAT'
	currency NAT
",
				RevisionTime = new DateTime(1901, 1, 2)
			});
			var sql = sync.GetUpdateScript();
			Console.WriteLine(sql);
		}

		[Test]
		public void CanMakeValidDelta()
		{
			sync.TargetStorage.Register(new MetaFileDescriptor()
			{
				Code = "file1",
				Content = @"
x.t id=1 code1  name11 
", RevisionTime = new DateTime(1901,1,1)
			});

			sync.TargetStorage.Register(new MetaFileDescriptor()
			{
				Code = "file2",
				Content = @"
x.t code1 ShortName=x
",
				RevisionTime = new DateTime(1901, 1, 2)
			});

			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "file1",
				Content = @"
x.t id=1 code1  name14 idx=23
",
				RevisionTime = new DateTime(1901, 1, 3)
			});

			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "file2",
				Content = @"
x.t code1 ShortName=x FullName=xxx
",
				RevisionTime = new DateTime(1901, 1, 4)
			});
			var sql = sync.GetUpdateScript();
			Console.WriteLine(sql);
			StringAssert.Contains(@"if exists (select top 1 id from x.t where Id = 1) 
update x.t set Code = 'code1', idx = 23, Name = 'name14', FullName = 'xxx', Active = 1, Version = (getdate()) where Id = 1
else insert x.t (Id, Code, idx, Name, FullName, Active, Version) values (1, 'code1', 23, 'name14', 'xxx', 1, (getdate()))", sql);

		}

		[Test]
		public void CanMakeValidDeltaWithLessAttributes()
		{
			sync.TargetStorage.Register(new MetaFileDescriptor()
			{
				Code = "file1",
				Content = @"
x.t id=1 code1  name11 
",
				RevisionTime = new DateTime(1901, 1, 1)
			});

			sync.TargetStorage.Register(new MetaFileDescriptor()
			{
				Code = "file2",
				Content = @"
x.t code1 ShortName=x
",
				RevisionTime = new DateTime(1901, 1, 2)
			});

			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "file1",
				Content = @"
x.t id=1 code1  name11 
",
				RevisionTime = new DateTime(1901, 1, 3)
			});

			sync.SourceStorage.Register(new MetaFileDescriptor()
			{
				Code = "file2",
				Content = @"
x.t code1 ShortName=x FullName=xxx
",
				RevisionTime = new DateTime(1901, 1, 4)
			});
			var sql = sync.GetUpdateScript();
			Console.WriteLine(sql);
			StringAssert.Contains(@"if exists (select top 1 id from x.t where Code = 'code1') 
update x.t set FullName = 'xxx', Active = 1, Version = (getdate()) where Code = 'code1'
else insert x.t (Code, FullName, Active, Version) values ('code1', 'xxx', 1, (getdate()))
declare @c table (t nvarchar(255), c nvarchar(255), w nvarchar(255))", sql);

		}
	}
}
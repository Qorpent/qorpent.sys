using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Data.MetaDataBase;
using Qorpent.Utils.XDiff;

namespace Qorpent.Data.Tests.MetaDataBase
{
	[TestFixture]
	public class MetaFileProcessorTest
	{


		[Test]
		public void CanGenerateInsertScript(){
			var r = new DatabaseUpdateRecord();
			r.DiffItem = new XDiffItem();
			r.Schema = "x";
			r.TargetTable = "t";
			r.DiffItem.Action = XDiffAction.CreateElement;
			r.DiffItem.NewestElement = XElement.Parse("<item id='1' code='aaa' name='bbb'  tag.x='2' SomeField='x'/>");
			var sql = new DefaultMetaFileProcessor().GetSql(new[]{r}, false).Replace("\"","'");
			Console.WriteLine(sql);
			StringAssert.Contains(@"if exists (select top 1 id from x.t where Id = 1) 
update x.t set Code = 'aaa', Name = 'bbb', SomeField = 'x', Tag = '/x:2/', Active = 1, Version = (getdate()) where Id = 1
else insert x.t (Id, Code, Name, SomeField, Tag, Active, Version) values (1, 'aaa', 'bbb', 'x', '/x:2/', 1, (getdate()))", sql);
		}

		[Test]
		public void CanGenerateUpdateScript()
		{
			var r = new DatabaseUpdateRecord();
			r.DiffItem = new XDiffItem();
			r.Schema = "x";
			r.TargetTable = "t";
			r.DiffItem.Action = XDiffAction.ChangeAttribute;
			r.DiffItem.BasisElement = XElement.Parse("<item id='1' code='aaa' name='bbb'  tag.x='2' SomeField='x'/>");
			r.DiffItem.NewestAttribute =new XAttribute("name","a+");
			var sql = new DefaultMetaFileProcessor().GetSql(new[] { r }).Replace("\"", "'");
			Console.WriteLine(sql);
			StringAssert.Contains(@"update x.t set Name = 'a+', Acitve = 1, Version = (getdate())  where Id = 1", sql);
		}

		[Test]
		public void CanGenerateUpdateScriptWithCode()
		{
			var r = new DatabaseUpdateRecord();
			r.DiffItem = new XDiffItem();
			r.Schema = "x";
			r.TargetTable = "t";
			r.DiffItem.Action = XDiffAction.ChangeAttribute;
			r.DiffItem.BasisElement = XElement.Parse("<item code=\"aa'a\" name='bbb'  tag.x='2' SomeField='x'/>");
			r.DiffItem.NewestAttribute = new XAttribute("name", "a+");
			var sql = new DefaultMetaFileProcessor().GetSql(new[] { r }).Replace("\"", "'");
			Console.WriteLine(sql);
			StringAssert.Contains(@"update x.t set Name = 'a+', Acitve = 1, Version = (getdate())  where Code = 'aa''a'", sql);
		}

		[Test]
		public void CanGroupDeltasOnSameObject(){
			var u1 = new DatabaseUpdateRecord{TargetTable = "a", TargetId = 1, TargetCode = "b"};
			var u2 = new DatabaseUpdateRecord{TargetTable = "a", TargetCode = "b"};
			var u3 = new DatabaseUpdateRecord{TargetTable = "b", TargetId = 2};
			var u4 = new DatabaseUpdateRecord{TargetTable = "b", TargetId = 2, TargetCode = "c"};
			var objects = DatabaseUpdateRecord.Group(new[]{u1, u2, u3, u4});
			Assert.AreEqual(2,objects.Count());
			CollectionAssert.AreEquivalent(objects.Keys,new[]{
				new ObjectKey{Code = "b",Id=1,Table = "a"},
				new ObjectKey{Code = "c",Id=2,Table = "b"}
			});
		}


		[Test]
		public void CanGenerateUpdateCodeScriptWithCode()
		{
			var r = new DatabaseUpdateRecord();
			r.DiffItem = new XDiffItem();
			r.Schema = "x";
			r.TargetTable = "t";
			r.DiffItem.Action = XDiffAction.ChangeAttribute;
			r.DiffItem.BasisElement = XElement.Parse("<item code=\"aa'a\" name='bbb' />");
			r.DiffItem.NewestAttribute = new XAttribute("code", "22");
			var sql = new DefaultMetaFileProcessor().GetSql(new[] { r }, false).Replace("\"", "'");
			Console.WriteLine(sql);
			StringAssert.Contains(@"update x.t set Code = '22', Acitve = 1, Version = (getdate())  where Code = 'aa''a'", sql);
		}


		[Test]
		public void CanGenerateDeleteScript()
		{
			var r = new DatabaseUpdateRecord();
			r.DiffItem = new XDiffItem();
			r.Schema = "x";
			r.TargetTable = "t";
			r.DiffItem.Action = XDiffAction.DeleteElement;
			r.DiffItem.BasisElement = XElement.Parse("<item id='1' code=\"aa'a\" name='bbb'  tag.x='2' SomeField='x'/>");
			r.DiffItem.NewestAttribute = new XAttribute("name", "a+");
			var sql = new DefaultMetaFileProcessor().GetSql(new[] { r }, false).Replace("\"", "'");
			Console.WriteLine(sql);
			StringAssert.Contains(@"delete x.t where Id = 1", sql);
		}
		[Test]
		public void CanGenerateDeleteScriptWithCode()
		{
			var r = new DatabaseUpdateRecord();
			r.DiffItem = new XDiffItem();
			r.Schema = "x";
			r.TargetTable = "t";
			r.DiffItem.Action = XDiffAction.DeleteElement;
			r.DiffItem.BasisElement = XElement.Parse("<item code=\"aaa\" name='bbb'  tag.x='2' SomeField='x'/>");
			r.DiffItem.NewestAttribute = new XAttribute("name", "a+");
			var sql = new DefaultMetaFileProcessor().GetSql(new[] { r }, false).Replace("\"", "'");
			Console.WriteLine(sql);
			StringAssert.Contains(@"delete x.t where Code = 'aaa'", sql);
		}

		[Test]
		public void CanBuildNormalSqlFromDiff(){
			
		}
	}
}

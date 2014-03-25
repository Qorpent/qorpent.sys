using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;
using Qorpent.Data.MetaDataBase;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.Tests.MetaDataBase
{
	[TestFixture]
	public class SqlFileStorageTest{
		private SqlBasedMetaFileRegistry sqlmd;

		private void InDb(Action<IDbConnection> action){
			using (var c = new SqlConnection(connection)){
				c.Open();
				action(c);
			}
		}

		private const string code = "SqlFileStorageTest";
		private const string connection =
			"Data Source=(local);Initial Catalog=z3;Integrated Security=True;Application Name=ecohome";
		[SetUp]
		public void SetUp(){
			InDb(c => c.ExecuteNonQuery("exec qptmds.MDFileDelete @code=@code",new{code}));
			InDb(c => c.ExecuteNonQuery("exec qptmds.MDFileDelete @code=@code",new{code=code+"1"}));
			InDb(c => c.ExecuteNonQuery("exec qptmds.MDFileDelete @code=@code",new{code=code+"2"}));
			sqlmd = new SqlBasedMetaFileRegistry{ConnectionString = connection};

		}

		[Test]
		public void CanSave(){
			sqlmd.Register(new MetaFileDescriptor{Code = code, Content = "hw", Name = "hello"});
			var current = sqlmd.GetCurrent(code);
			Assert.AreEqual(code,current.Code);
			Assert.AreEqual("hello",current.Name);
			Assert.AreEqual("hw",current.Content);
			Assert.AreEqual("ZcKj13EnwV0Gjex+AOUGSQ==", current.Hash);
		}

		[Test]
		public void CanGetCodes(){

			sqlmd.Register(new MetaFileDescriptor { Code = code, Content = "hw", Name = "hello", Revision = "1" });
			sqlmd.Register(new MetaFileDescriptor { Code = code+"1", Content = "hw", Name = "hello", Revision = "1" });
			sqlmd.Register(new MetaFileDescriptor { Code = code+"2", Content = "hw", Name = "hello", Revision = "1" });

			var codes = sqlmd.GetCodes(code);
			Assert.AreEqual(3,codes.Count());
		}


		[Test]
		public void CanCheckExisted()
		{

			sqlmd.Register(new MetaFileDescriptor { Code = code, Content = "hw", Name = "hello", Revision = "1" });
			Assert.True(sqlmd.Exists(code));
			Assert.False(sqlmd.Exists(code+"1"));
		}


		[Test]
		public void CanCheckout()
		{
			sqlmd.Register(new MetaFileDescriptor { Code = code, Content = "hw", Name = "hello", Revision = "1"});
			sqlmd.Register(new MetaFileDescriptor { Code = code, Content = "hw2", Name = "hello", Revision = "2"});
			sqlmd.Register(new MetaFileDescriptor { Code = code, Content = "hw3", Name = "hello", Revision = "3"});
			var current = sqlmd.GetCurrent(code);
			Assert.AreEqual("hw3", current.Content);
			sqlmd.Checkout(code, "2");
			current = sqlmd.GetCurrent(code);
			Assert.AreEqual("hw2", current.Content);
		}

		[Test]
		public void CanGetRevision()
		{
			sqlmd.Register(new MetaFileDescriptor { Code = code, Content = "hw", Name = "hello", Revision = "1" });
			sqlmd.Register(new MetaFileDescriptor { Code = code, Content = "hw2", Name = "hello", Revision = "2" });
			sqlmd.Register(new MetaFileDescriptor { Code = code, Content = "hw3", Name = "hello", Revision = "3" });
			var current = sqlmd.GetByRevision(code,"2");
			Assert.AreEqual("hw2", current.Content);
		}



		[Test]
		public void CanGetRevisions()
		{
			sqlmd.Register(new MetaFileDescriptor { Code = code, Content = "hw", Name = "hello", Revision = "1" });
			sqlmd.Register(new MetaFileDescriptor { Code = code, Content = "hw2", Name = "hello", Revision = "2" });
			sqlmd.Register(new MetaFileDescriptor { Code = code, Content = "hw3", Name = "hello", Revision = "3" });
			var revisions = sqlmd.GetRevisions(code).ToArray();
			Assert.AreEqual(3,revisions.Length);
		}
	}
}

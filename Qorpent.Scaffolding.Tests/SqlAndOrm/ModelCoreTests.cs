using System;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm
{



	/// <summary>
	/// Тесты  для проверки работоспособности модели
	/// </summary>
	[TestFixture]
	public class ModelCoreTests
	{


		[Test]
		public void BasicTableParseWithDefaults(){
			var model =PersistentModel.Compile(@"class X prototype=dbtable");
			Assert.True(model.IsValid);
			Assert.True(model.Classes.ContainsKey("dbo.x"),"must contains key for table as full sql name with default dbo schema and in lower case");
			var cls = model.Classes["dbo.x"];
			Assert.AreEqual("X",cls.Name);
			Assert.AreEqual("X",cls.FullCodeName);
			Assert.AreEqual("dbo.X",cls.FullSqlName);
			Assert.AreEqual("dbo",cls.Schema);
			Assert.NotNull(cls.PrimaryKey,"all tables must implicitly include primary key");
			var pk = cls.PrimaryKey;
			Assert.AreEqual("Id",pk.Name);
			Assert.True(pk.IsPrimaryKey);
			Assert.True(pk.IsAutoIncrement);
			Assert.AreEqual("int",pk.DataType.CSharpDataType);
			Assert.True(cls.Fields.ContainsKey("id"),"field must be regestered in cls with lowercase");
		}


		[Test]
		public void CanApplySchemaAndNamespace()
		{
			var model = PersistentModel.Compile(@"class A.X prototype=dbtable schema=test");
			Assert.True(model.IsValid);
			Assert.True(model.Classes.ContainsKey("test.x"), "must apply test schema");
			var cls = model.Classes["test.x"];
			Assert.AreEqual("X", cls.Name);
			Assert.AreEqual("A.X", cls.FullCodeName);
			Assert.AreEqual("test.X", cls.FullSqlName);
			Assert.AreEqual("test", cls.Schema);
		}

		[Test]
		public void ApplyPrimaryKeyToGivenIdFieldIfNoOtherPk(){
			var model = PersistentModel.Compile(@"
class X prototype=dbtable
	long Id");
			Assert.True(model.IsValid);
			var fld = model["dbo.x"]["id"];
			Assert.AreEqual("long",fld.DataType.CSharpDataType);
			Assert.True(fld.IsPrimaryKey);
			Assert.True(fld.IsAutoIncrement);
		}

		[Test]
		public void CanSetReferenceToCustomPk(){
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table X
	string myid primarykey=1
table Y
	ref X
");
			Assert.True(model.IsValid);
			Assert.False(model["x"].Fields.ContainsKey("id"));
			Assert.AreEqual("myid",model["y"]["x"].ReferenceField);
			Assert.AreEqual("/",model["y"]["x"].DefaultSqlValue.Value);
		}

		[Test]
		public void CanSetReferenceToNonCustomPk()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table X
	string Code
table Y
	ref X to=X.Code
");
			Assert.True(model.IsValid);
			Assert.True(model["x"].Fields.ContainsKey("id"));
			Assert.AreEqual("Code", model["y"]["x"].ReferenceField);
			Assert.AreEqual("/", model["y"]["x"].DefaultSqlValue.Value);
		}


		[Test]
		public void SimpleReferenceSupport()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table X
table Y
	ref X
");
			Assert.True(model.IsValid);
			var x = model["dbo.x"];
			var y = model["dbo.y"];
			Console.WriteLine(x.Rank);
			Console.WriteLine(y.Rank);
			Assert.Greater(x.Rank,y.Rank,"ранг таблицы X должен быть больше, так как от нее зависит  Y");
			var reference = y["x"];
			Assert.True(reference.IsReference);
			Assert.AreEqual(x,reference.ReferenceClass);
			Assert.AreEqual(x["id"].DataType,reference.DataType);
			Assert.True(x.ReverseFields.ContainsKey(reference.ReverseCollectionName));
			Assert.AreEqual("Ys",reference.ReverseCollectionName);
			Assert.AreEqual(x.ReverseFields[reference.ReverseCollectionName],reference);
			Assert.False(reference.GetIsCircular());
			Assert.False(reference.IsReverese,"по умолчанию бэк коллекций не указывается");
		}

		[Test]
		public void ReferenceWithCustomName()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table X
table Y
	ref MyX to=X
");
			Assert.True(model.IsValid);
			var x = model["x"];
			var y = model["y"];
			var reference = y["myx"];
			Assert.AreEqual(x["id"].DataType, reference.DataType);
			Assert.True(x.ReverseFields.ContainsKey(reference.ReverseCollectionName));
			Assert.AreEqual("YsByMyX", reference.ReverseCollectionName);
			Assert.AreEqual(x.ReverseFields[reference.ReverseCollectionName], reference);
			Assert.False(reference.IsReverese, "по умолчанию бэк коллекций не указывается");
		}

		[Test]
		public void ReferenceWithCustomNameAndCustomReverseName()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table X
table Y
	ref MyX to=X reverse=TheXs
");
			Assert.True(model.IsValid);
			var x = model["x"];
			var y = model["y"];
			var reference = y["myx"];
			Assert.AreEqual(x["id"].DataType, reference.DataType);
			Assert.True(x.ReverseFields.ContainsKey(reference.ReverseCollectionName));
			Assert.AreEqual("TheXs", reference.ReverseCollectionName);
			Assert.AreEqual(x.ReverseFields[reference.ReverseCollectionName], reference);
			Assert.True(reference.IsReverese, "явно указан атрибут реверса");
		}

		[Test]
		public void ReferenceWithReverseAndCommonReverseName()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table X
table Y
	ref MyX to=X reverse
");
			Assert.True(model.IsValid);
			var x = model["dbo.x"];
			var y = model["dbo.y"];
			var reference = y["myx"];
			Assert.AreEqual(x["id"].DataType, reference.DataType);
			Assert.True(x.ReverseFields.ContainsKey(reference.ReverseCollectionName));
			Assert.AreEqual("YsByMyX", reference.ReverseCollectionName);
			Assert.AreEqual(x.ReverseFields[reference.ReverseCollectionName], reference);
			Assert.True(reference.IsReverese, "явно указан атрибут реверса");
		}

		[Test]
		public void CanCheckCirculars()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table A
	ref A # will not be circular
	ref C # will be circular
	ref D # will not be circular
table B
	ref A #will be circular
table C
	ref D # will no be circular
	ref B # will be circular
table D
	
");
			Assert.True(model.IsValid);
			Assert.False(model["a"]["a"].GetIsCircular(),"(-)a->a (self links are not treats as circular for model)");
			Assert.True(model["a"]["c"].GetIsCircular(),"(+)a->c");
			Assert.False(model["a"]["d"].GetIsCircular(),"(-)a->c");
			Assert.True(model["b"]["a"].GetIsCircular(), "(+)b->a");
			Assert.False(model["c"]["d"].GetIsCircular(), "(-)c->d");
			Assert.True(model["c"]["b"].GetIsCircular(), "(+)c->b");
			//проверяем, что ранги сформированы максимально адекватно исходя из алгоритма обхода
			Assert.AreEqual(4,model["d"].Rank);
			Assert.AreEqual(4,model["b"].Rank);
			Assert.AreEqual(3, model["c"].Rank);
			Assert.AreEqual(1, model["a"].Rank);

		}

		[Test]
		public void HierarchyDefinitionSampleWithAutoLoadParentAndLazyChildren(){
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table category
	ref Parent to=category reverse=Children auto reverse-lazy reverse-auto
");		
			Assert.True(model.IsValid);
			var category = model["category"];
			var cparent = category["parent"];
			Assert.IsTrue(cparent.IsReference);
			Assert.AreEqual(category,cparent.ReferenceClass);
			Assert.True(category.ReverseFields.ContainsKey(cparent.ReverseCollectionName));
			Assert.True(cparent.IsReverese);
			Assert.AreEqual("Children",cparent.ReverseCollectionName);
			Assert.True(cparent.IsAutoLoadByDefault);
			Assert.True(cparent.IsAutoLoadReverseByDefault);
			Assert.True(cparent.IsLazyLoadReverseByDefault);
			Assert.False(cparent.IsLazyLoadByDefault);
		}

		[Test]
		public void CanDetectNonValidTableRef(){
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table a
	ref b
");
			Assert.False(model.IsValid);
		}
		[Test]
		public void CanDetectNonValidTableFieldRef()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table a
table b
	ref a to=a.x
");
			Assert.False(model.IsValid);
		}


		

		

		
	}
}

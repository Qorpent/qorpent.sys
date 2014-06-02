using System.Linq;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.SqlObjects;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm{
	[TestFixture]
	public class FileAllocationTest{
		[Test]
		public void CanApplyDefaultFileGroup()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table a
");
			Assert.AreEqual("SECONDARY", model["a"].AllocationInfo.FileGroupName);
		}

		[Test]
		public void CanApplyFileGroup()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table a filegroup=TEST
");
			Assert.AreEqual("TEST", model["a"].AllocationInfo.FileGroupName);
		}

		[Test]
		public void CanApplyFileGroupWithAttr()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
class X.TEST
table a filegroup=^X.TEST
");
			Assert.AreEqual("TEST", model["a"].AllocationInfo.FileGroupName);
			Assert.AreEqual(model.DatabaseSqlObjects[0], model["a"].AllocationInfo.FileGroup);
		}

		[Test]
		public void CanApplyFileGroupWithAttrToCustomName()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
class X.TEST sqlname=T2
table a filegroup=^X.TEST
");
			Assert.AreEqual("T2", model["a"].AllocationInfo.FileGroupName);
			Assert.AreEqual(model.DatabaseSqlObjects[0], model["a"].AllocationInfo.FileGroup);
		}

		[Test]
		public void CanSetupPartitioning()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table a
	partitioned with=x start=2
	int X
");
			var t = model["a"];
			Assert.AreEqual("SECONDARY", t.AllocationInfo.FileGroupName);
			Assert.True(t.AllocationInfo.Partitioned);
			Assert.AreEqual("x", t.AllocationInfo.PartitionFieldName);
			Assert.AreEqual(t["x"], t.AllocationInfo.PartitionField);
			Assert.AreEqual(2, t.AllocationInfo.PartitioningStart);
		}
		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void AutoGenerateFileGroup(){
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table a filegroup=test	
");
			var a = model["a"].AllocationInfo;
			Assert.AreEqual("TEST",a.FileGroupName);
			Assert.NotNull(a.FileGroup);
			Assert.True(model.DatabaseSqlObjects.Contains(a.FileGroup));
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void BestPracticeFileGroupSecondaryAlwaysExistsAndMappedAsDefault()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table a 
");
			var fg = model.DatabaseSqlObjects.OfType<FileGroup>().FirstOrDefault(_ => _.Name == "SECONDARY");
			Assert.NotNull(fg);
			Assert.True(fg.IsDefault);
		}


		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void NotMappedPartitionedFieldIsError()
		{
			var model = PersistentModel.Compile(@"
class table prototype=dbtable abstract
table a 
	partitioned with=X
");
			Assert.False(model.IsValid);
		}
	}
}
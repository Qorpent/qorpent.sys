using System;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm {
	public class DataTypeTests {
		[TestCase(true, Description = "Проверяет корректность работы с System.DateTime с разрешением имени по typeof(DateTime).FullName")]
		[TestCase(false, Description = "Проверяет корректность работы с System.DateTime с разрешением имени по typeof(DateTime).Name")]
		public void IsCorrectWorkWithDateTime(bool resolveByFullName) {
			var dateTimeType = typeof (DateTime);
			var dataType = new DataType {CSharpDataType = resolveByFullName ? dateTimeType.FullName : dateTimeType.Name};
			Assert.IsTrue(dataType.IsDateTime);
		}
	}
}

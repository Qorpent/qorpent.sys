using System.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using Qorpent.Data;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests {
	/// <summary>
	///		Набор тестов для хранилища документов на базе B#
	/// </summary>
	[TestFixture(Description = "Набор тестов для хранилища документов на базе B#")]
	public class BSharpDocumentStorageTests {
		private const string SimpleSet = @"
class a
	v = 1
class b
	v = 2
";
		private const string NsSet = @"
namespace y
	class a
		v = 1
namespace z
	class b
		v = 2		
";
		[Test]
		public void IsCorrectGettingSimpleClass() {
			var storage = new BSharpDocumentStorage();
			storage.SetContext(SimpleSet);
			var cls = storage.ExecuteQuery("a");
			Assert.NotNull(cls);
			Assert.NotNull(cls.Attribute("v"));
			Assert.AreEqual("1", cls.Attribute("v").Value);
		}
		[Test]
		public void IsCorrectGettingTwoClassesInSingleResult() {
			var storage = new BSharpDocumentStorage();
			storage.SetContext(SimpleSet);
			var cls = storage.ExecuteQuery("");
			Assert.NotNull(cls);
			Assert.IsTrue(cls.HasElements);
			Assert.AreEqual(2, cls.Elements().Count());
			Assert.NotNull(cls.Attribute(BSharpDocumentStorage.IsComplexResultAttribute));
			Assert.IsTrue(cls.Attribute(BSharpDocumentStorage.IsComplexResultAttribute).Value.ToBool());
			var a = cls.XPathSelectElement("/*[@code = 'a']");
			var b = cls.XPathSelectElement("/*[@code = 'b']");
			Assert.NotNull(a);
			Assert.NotNull(b);
			Assert.NotNull(a.Attribute("v"));
			Assert.NotNull(b.Attribute("v"));
			Assert.AreEqual("1", a.Attribute("v").Value);
			Assert.AreEqual("2", b.Attribute("v").Value);
		}
		[Test]
		public void IsCorrectGettingClassIsNamespaceWithoutNamespaceInQueryOptions() {
			var storage = new BSharpDocumentStorage();
			storage.SetContext(NsSet);
			var cls = storage.ExecuteQuery("a");
			Assert.NotNull(cls);
			Assert.NotNull(cls.Attribute("v"));
			Assert.AreEqual("1", cls.Attribute("v").Value);
		}
		[Test]
		public void IsCorrectGettingClassInNamespaceWithNamespaceInQueryOptions() {
			var storage = new BSharpDocumentStorage();
			storage.SetContext(NsSet);
			var cls = storage.ExecuteQuery("a", new DocumentStorageOptions {Collection = "y"});
			Assert.NotNull(cls);
			Assert.NotNull(cls.Attribute("v"));
			Assert.AreEqual("1", cls.Attribute("v").Value);
		}
		[Test]
		public void IsCorrectGettingClassInNamespaceWithNamespaceInQuery() {
			var storage = new BSharpDocumentStorage();
			storage.SetContext(NsSet);
			var cls = storage.ExecuteQuery("y.a");
			Assert.NotNull(cls);
			Assert.NotNull(cls.Attribute("v"));
			Assert.AreEqual("1", cls.Attribute("v").Value);
		}
	}
}

using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.BSharp.Schema;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp.Schema {
	[TestFixture]
	public class ElementRuleTest {
		[Test]
		public void AutoStrictModeAttributeFilter() {
			var r = new ElementRule();
			r.AttributeRules.Add(new AttributeRule {Code = "a", Type = RuleType.Allow, Action = RuleActionType.Rename,Value="c"});
			var e = XElement.Parse("<x a='3' b='2'/>");
			r.Apply(e);
			Assert.Null(e.Attribute("b"));
			Assert.Null(e.Attribute("a"));
			Assert.NotNull(e.Attribute("c"));
			Assert.AreEqual("3",e.Attr("c"));
		}
		[Test]
		public void NonStrictMode() {
			var r = new ElementRule();
			r.AttributeRules.Add(new AttributeRule { Code = "a", Type = RuleType.Deny });
			var e = XElement.Parse("<x a='1' b='2'/>");
			r.Apply(e);
			Assert.NotNull(e.Attribute("b"));
			Assert.Null(e.Attribute("a"));
		}
	}
}
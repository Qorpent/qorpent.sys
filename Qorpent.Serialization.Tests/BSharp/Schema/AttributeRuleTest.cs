using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.BSharp.Schema;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp.Schema
{
	[TestFixture]
	public class AttributeRuleTest
	{
		[Test]
		public void CanRemoveAttributeWithAction() {
			var rule = new AttributeRule {Action = RuleActionType.Remove, Code="a", Type = RuleType.Allow};
			var e = XElement.Parse("<a a='1' b='2'/>");
			var n = rule.Apply(e);
			Assert.Null(n);
			Assert.Null(e.Attribute("a"));
			Assert.NotNull(e.Attribute("b"));
		}

		[Test]
		public void CanSkipRequiredAttibute()
		{
			var rule = new AttributeRule { Action = RuleActionType.None, Code = "a", Type = RuleType.Require };
			var e = XElement.Parse("<a a='1' b='2'/>");
			var n = rule.Apply(e);
			Assert.Null(n);
			Assert.NotNull(e.Attribute("a"));
			Assert.AreEqual("1",e.Attr("a"));
			Assert.NotNull(e.Attribute("b"));
		}


		[Test]
		public void WarnObsoleteAttibute()
		{
			var rule = new AttributeRule { Action = RuleActionType.None, Code = "a", Type = RuleType.Obsolete };
			var e = XElement.Parse("<a a='1' b='2'/>");
			var n = rule.Apply(e);
			Assert.NotNull(n);
			Assert.AreEqual(ErrorLevel.Warning,n.Level);
			Assert.NotNull(e.Attribute("a"));
			Assert.AreEqual("1", e.Attr("a"));
			Assert.NotNull(e.Attribute("b"));
		}
		[Test]
		public void CanCreateRequiredAttibute()
		{
			var rule = new AttributeRule { Action = RuleActionType.None, Code = "a", Type = RuleType.Require ,Value = "def"};
			var e = XElement.Parse("<a  b='2'/>");
			var n = rule.Apply(e);
			Assert.NotNull(n);
			Assert.AreEqual(ErrorLevel.Hint,n.Level);
			Assert.NotNull(e.Attribute("a"));
			Assert.AreEqual("def", e.Attr("a"));
			Assert.NotNull(e.Attribute("b"));
		}

		[Test]
		public void CanRemoveAttributeWithDenyType()
		{
			var rule = new AttributeRule { Action = RuleActionType.None, Code = "a", Type = RuleType.Deny };
			var e = XElement.Parse("<a a='1' b='2'/>");
			var n = rule.Apply(e);
			Assert.NotNull(n);
			Assert.AreEqual(ErrorLevel.Error,n.Level);
			Assert.Null(e.Attribute("a"));
			Assert.NotNull(e.Attribute("b"));
		}
	}
}

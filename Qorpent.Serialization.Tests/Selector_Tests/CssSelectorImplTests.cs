using NUnit.Framework;
using Qorpent.Selector.Implementations;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.Serialization.Tests.Selector_Tests {
    [TestFixture]
    public class CssSelectorImplTests {
        private const string _case1 = @"<root>
    <div class="" first second"">TRUE</div>
    <div class=""first"">NOT</div>
    <div class=""second"">NOT</div>
</root>";
        private const string _case2 = @"<root>
    <div class=""first"">TRUE</div>
    <div class=""first"">NOT</div>
</root>";
        [TestCase(_case1, "div.first", new[] { @"<div class="" first second"">TRUE</div>", @"<div class=""first"">NOT</div>" }, 2)]
        [TestCase(_case1, "div.first.second", new[] {@"<div class="" first second"">TRUE</div>"}, 1)]
        [TestCase(_case2, "div.first:first-child", new[] { @"<div class=""first"">TRUE</div>" }, 1)]
        public void CanSelect(string source, string clause, IEnumerable<string> result, int count) {
            var cssSelector = new CssSelectorImpl();
            var selected = cssSelector.Select(XElement.Parse(source), clause);
            Assert.AreEqual(count, selected.Count());

            foreach (var i in selected.Select(_ => _.ToString())) {
                Assert.IsTrue(result.Contains(i));
            }
        }
    }
}

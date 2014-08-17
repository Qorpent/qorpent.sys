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
    <div class=""first"">BUZZ</div>
    <div class=""first"">NOT</div>
</root>";
		private const string _case3 = @"
<div tag=""tr"" class=""qtag-tr qlevel-10 qpos-1"">
	<div class=""news qtag-td qlevel-11 qpos-0"" tag=""td"">
		<span class=""small_gray 1"">15 Август 2014,18:00</span>
		<div tag=""table"" class=""qtag-table qlevel-12 qpos-0"">
			<div tag=""tr"" class=""qtag-tr qlevel-13 qpos-0"">		
				<div tag=""td"" class=""qtag-td qlevel-14 qpos-0"">
					<span class=""small_gray 2"">15 Август 2014,18:00</span>
				</div>
			</div>
		</div>
	</div>
</div>
";

        [TestCase(_case1, "div.first", new[] { @"<div class="" first second"">TRUE</div>", @"<div class=""first"">NOT</div>" }, 2)]
        [TestCase(_case1, "div.first.second", new[] {@"<div class="" first second"">TRUE</div>"}, 1)]
		[TestCase(_case1, ".first.second", new[] { @"<div class="" first second"">TRUE</div>" }, 1)]
		[TestCase(_case3, ".news span.small_gray", new[] { @"<span class=""small_gray 1"">15 Август 2014,18:00</span>", @"<span class=""small_gray 2"">15 Август 2014,18:00</span>" }, 2)]
		[TestCase(_case3, ".news+span.small_gray", new[] { @"<span class=""small_gray 1"">15 Август 2014,18:00</span>" }, 1)]
        [TestCase(_case2, "div.first:first-child", new[] { @"<div class=""first"">TRUE</div>" }, 1)]
        [TestCase(_case2, "div.first:last-child", new[] { @"<div class=""first"">NOT</div>" }, 1)]
        [TestCase(_case2, "div.first:nth-child(2)", new[] { @"<div class=""first"">BUZZ</div>" }, 1)]
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

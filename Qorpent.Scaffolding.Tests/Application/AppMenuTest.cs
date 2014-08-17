using System;
using NUnit.Framework;
using Qorpent.Scaffolding.Application;

namespace Qorpent.Scaffolding.Tests.Application {
    [TestFixture]
	[Category("NOTC")]
    public class AppMenuTest : AppModelBasicTestsBase {
        [Test]
        public void CanParseNestedElements() {
            var model = TestModel(@"
class menu menu_name prototype=ui-menu type=inline
    MenuItem a an
        MenuItem b  bn
    MenuItem c cn
        MenuItem d dn
");
            var menu = model.Resolve<AppMenu>("menu");
            Assert.NotNull(menu);
            Assert.AreEqual(AppMenuType.Inline,menu.Type);
            Assert.AreEqual("menu_name",menu.Name);
            Assert.AreEqual(2,menu.Items.Count);
            var a = menu.Items["a"];
            var c = menu.Items["c"];
            var b = a.Items["b"];
            var d = c.Items["d"];
            Assert.AreEqual(null,a.Parent);
            Assert.AreEqual(null,c.Parent);
            Assert.AreEqual(a,b.Parent);
            Assert.AreEqual(c,d.Parent);
            foreach(var _ in new[]{a,b,c,d}){
                Assert.AreEqual(AppMenuItemType.Default,_.Type);
                Assert.AreEqual(_.Code+"n",_.Name);
                Assert.AreEqual(menu,_.Menu);
            }
        }

        [Test]
        public void CanResolveItems() {
            var model = TestModel(@"
class menu menu_name prototype=ui-menu type=inline
    MenuItem a an
        MenuItem b  bn
            MenuItem c cn
                MenuItem d dn
");
            var menu = model.Resolve<AppMenu>("menu");
            foreach (var mcode in new[] { "a", "b", "c", "d" }) {
                var item = menu.GetItem(mcode);
                Assert.NotNull(item);
            }
        }
    }
}
using NUnit.Framework;
using Qorpent.Scaffolding.Application;

namespace Qorpent.Scaffolding.Tests.Application {
    [TestFixture]
    public class AppLayoutTest : AppModelBasicTestsBase {
        [Test]
        public void CanParseNestedElements() {
            var model = TestModel(@"
class layout layout_name prototype=ui-layout
    Layout a an
        Layout b bn
            Widget c cn
        Widget d dn 
    Layout e en
        Widget f fn
");
            var layout = model.Resolve<AppLayout>("layout");
            Assert.NotNull(layout);
            Assert.True(layout.Root);
            Assert.AreEqual("layout_name", layout.Name);
            Assert.AreEqual(2, layout.Layouts.Count);
            Assert.AreEqual(1, layout.Layouts["a"].Layouts.Count);
            Assert.AreEqual(1, layout.Layouts["a"].Widgets.Count);
            var a = layout.Layouts["a"];
            var b = a.Layouts["b"];
            var c = b.Widgets["c"];
            var d = a.Widgets["d"];
            var e = layout.Layouts["e"];
            Assert.AreEqual(layout, a.Parent);
            Assert.AreEqual(layout, e.Parent);
            Assert.AreEqual(a, b.Parent);
            Assert.AreEqual(a, d.Layout);
        }

        [Test]
        public void CanParseViewsInWidgets() {
            var model = TestModel(@"
class layout layout_name prototype=ui-layout
    Layout a an
        Widget b bn
            view c cn
");
            var layout = model.Resolve<AppLayout>("layout");
            var a = layout.Layouts["a"];
            var b = a.Widgets["b"];
            Assert.NotNull(b.Views);
            Assert.AreEqual(1, b.Views.Count);
        }

        [Test]
        public void CanParseWidgetController() {
            var model = TestModel(@"
class controller prototype=ui-controller
class layout layout_name prototype=ui-layout
    Layout a an
        Widget ^controller bn
");
            var controller = model.Resolve<AppController>("controller");
            var layout = model.Resolve<AppLayout>("layout");
            Assert.AreEqual(controller, layout.Layouts["a"].Widgets["controller"].Controller);
        }
    }
}
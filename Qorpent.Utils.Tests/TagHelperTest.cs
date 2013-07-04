using System.Collections.Generic;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests {
    [TestFixture]
    public class TagHelperTest {
        [Test]
        public void QPT79_Cannot_Parse_Escaped_Doubles_And_Slashes() {
            var tag = "/a:x~`/";
            var val = TagHelper.Value(tag, "a");
            Assert.AreEqual("x:/",val);
        }
        [Test]
        public void QPT79_Does_Not_Apply_Escapes_On_Set_Value() {
            var tag = TagHelper.SetValue("", "a", "x:/");
            Assert.AreEqual("/a:x~`/", tag);
        }

        [Test]
        public void QPT79_Does_Not_Apply_Escapes_On_Dictionary_Generate() {
            var tag = TagHelper.ToString(new Dictionary<string, string> {{"a", "x:/"}});
            Assert.AreEqual("/a:x~`/", tag);
        }
    }
}
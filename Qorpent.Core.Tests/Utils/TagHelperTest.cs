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

        [TestCase("/x:123//y:abc//y1:def/","/x:*/",true)]
        [TestCase("/x:123//y:abc//y1:def/","/x:1/",false)]
        [TestCase("/x:123//y:abc//y1:def/","/x:*^1/",true)]
        [TestCase("/x:123//y:abc//y1:def/","/x:123/",true)]
        [TestCase("/x:123//y:abc//y1:def/", "/y:*/", true)]
        [TestCase("/x:123//y:abc//y1:def/", "/*^y:*/", true)]
        [TestCase("/x:123//y:abc//y1:def/", "/y:abc/", true)]
        [TestCase("/x:123//y:abc//y1:def/", "/y1:def/", true)]
        [TestCase("/x:123//y:abc//y1:def/", "/*^y:def/", true)]
        [TestCase("/x:123//y:abc//y1:def/", "/y:def/", false)]
        [TestCase("/x:123//y:abc//y1:def/", "/y:abc//z:1/", false)]
        [TestCase("/x:123//y:abc//y1:def/", "/y:abc//!z:*/", true)]
        [TestCase("/x:123//y:abc//y1:def//z:1/", "/y:abc//!z:*/", false)]
        [TestCase("/x:123//y:abc//y1:def//z:1/", "/y:abc//!z:1/", false)]
        [TestCase("/x:123//y:abc//y1:def//z:1/", "/y:abc//!z:2/", false)]
        [TestCase("/x:123//y:abc//y1:def//z:2/", "/y:abc//z:!1/", true)]
        [TestCase("/x:123//y:abc//y1:def//z:2/", "/y:abc//z:!2/", false)]
        public void Q346_Match_Test(string src, string mask, bool result) {
            Assert.AreEqual(result,TagHelper.Match(src,mask));
        }
    }
}
using System;
using System.Data.SqlClient;
using System.IO;
using NUnit.Framework;
using Qorpent.Core.Tests.BSharp.Runtime;
using Qorpent.Utils.Extensions;

namespace Qorpent.Core.Tests.Experiments {
    [TestFixture]
    public class JsonWriterTests {
        private TextWriter sw;
        private JsonWriter jw;

        [SetUp]
        public void Setup() {
            this.sw = new StringWriter();
            this.jw = new JsonWriter(this.sw);
        }

        [TestCase(null,"null")]
        [TestCase(1,"1")]
        [TestCase(1.1,"1.1")]
        [TestCase(999999999999999L, "\"999999999999999\"")]
        [TestCase(true,"true")]
        [TestCase(false,"false")]
        [TestCase("test","\"test\"")]
        [TestCase("\"test\"", "\"\\\"test\\\"\"")]
        [TestCase("\"te\tst\"", "\"\\\"te\\tst\\\"\"")]
        public void CanWriteValue(object obj, string result) {
            jw.Write(obj);
            jw.Flush();
            Console.WriteLine(sw.ToString());
            Assert.AreEqual(result,sw.ToString());
        }

        [Test]
        public void CanWriteObject() {
            jw.OpenObject();
            jw.Write("a","b");
            jw.Write("b",new{c="d"});
            jw.Flush();
            jw.Close();
            var result = sw.ToString().Simplify(SimplifyOptions.SingleQuotes);
            Assert.AreEqual("{'a':'b','b':{'c':'d'}}", result);
        }

        [Test]
        public void CanWriteArray()
        {
            jw.OpenArray();
            jw.Write(1);
            jw.Write(new { c = "d" });
            jw.Write(new[]{1,2});
            jw.Flush();
            jw.Close();
            var result = sw.ToString().Simplify(SimplifyOptions.SingleQuotes);
            Assert.AreEqual("[1,{'c':'d'},[1,2]]", result);
        }
    }
}
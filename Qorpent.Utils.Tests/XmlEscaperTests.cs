using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Utils.Extensions;


namespace Qorpent.Utils.Tests
{
    [TestFixture]
    public class XmlEscaperTests
    {
        [Test]
        public void EscapeTest()
        {
            String bxl = "~";
            String xml = "__TILD__";
            Assert.AreEqual(XmlEscaper.Escape(bxl), xml);
        }

        [Test]
        public void UnescapeTest()
        {
            String xml = "__PERC__";
            String bxl = "%";
            Assert.AreEqual(XmlEscaper.Unescape(xml), bxl);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Serialization;


namespace Qorpent.Core.Tests.Serialization
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

        [Test]
        public void EscapeAllTest()
        {
            String bxl = "test~qwerty!<>#+-";
            String xml = "test__TILD__qwerty__EXC____LT____GT__#__PLUS____MINUS__";
            Assert.AreEqual(XmlEscaper.EscapeAll(bxl), xml);
        }

        [Test]
        public void UnescapeAllTest()
        {
            String xml = "test__TILD__qwerty__EXC____LT____GT__#__PLUS____MINUS__";
            String bxl = "test~qwerty!<>#+-";
            Assert.AreEqual(XmlEscaper.UnescapeAll(xml), bxl);
        }
    }
}
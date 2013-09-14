using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests
{
    /// <summary>
    /// Проверяет работоспособность XHtml5 XML Writer
    /// </summary>
    [TestFixture]
    public class XHtml5XmlWriterTests
    {
        [Test]
        public void WroteElementsCorrectlyClosed() {
            var sw = new StringWriter();
            var xw = new XHtml5XmlWriter(sw);
            xw.GenerateHtmlWrapper = false;
            xw.WriteStartElement("meta");
            xw.WriteEndElement();
            xw.WriteStartElement("p"); 
            xw.WriteEndElement();
            xw.WriteStartElement("img");
            xw.WriteEndElement();
            xw.WriteStartElement("img");
            xw.WriteString("test");
            xw.WriteEndElement();
            var res = sw.ToString();
            Console.WriteLine(res);
            Assert.AreEqual("<meta /><p></p><img /><img>test</img>",res);
        }

        [Test]
        public void CanWrapXElementConent()
        {
            var sw = new StringWriter();
            var xw = new XHtml5XmlWriter(sw);
            var xe = new XElement("result", new XAttribute("code","1"),"test");
            xe.WriteTo(xw);
            var res = sw.ToString();
            Console.WriteLine(res);
            Assert.AreEqual(@"<!DOCTYPE html><html><head><meta name=""generator"" value=""XHtml5XmlWriter"" /></head><body><result code=""1"">test</result><style type=""text/css"">result {border:solid 1px black; padding:1px; margin:1px;}result:before {content: ""[result:"" attr(code) ""] ""}</style></body></html>", res);
        }
        [Test]
        public void CanPreventAutoStyling()
        {
            var sw = new StringWriter();
            var xw = new XHtml5XmlWriter(sw);
            xw.GenerateStylesForNonHtmlElements = false;
            var xe = new XElement("result", new XAttribute("code", "1"), "test");
            xe.WriteTo(xw);
            var res = sw.ToString();
            Console.WriteLine(res);
            Assert.AreEqual(@"<!DOCTYPE html><html><head><meta name=""generator"" value=""XHtml5XmlWriter"" /></head><body><result code=""1"">test</result></body></html>", res);
        }

        [Test]
        public void CanWrapXElementConentWithHtmlDefinition()
        {
            var sw = new StringWriter();
            var xw = new XHtml5XmlWriter(sw);
            var xe = new XElement("result", new XAttribute("code", "1"), "test");
            var be = new XElement("body", xe);
            var he = new XElement("html",be);
            he.WriteTo(xw);
            var res = sw.ToString();
            Console.WriteLine(res);
            Assert.AreEqual(@"<!DOCTYPE html><html><body><result code=""1"">test</result><style type=""text/css"">result {border:solid 1px black; padding:1px; margin:1px;}result:before {content: ""[result:"" attr(code) ""] ""}</style></body></html>", res);
        }
    }
}

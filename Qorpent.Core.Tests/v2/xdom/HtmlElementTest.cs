using System;
using System.Xml.Linq;
using NUnit.Framework;
using qorpent.v2.xdom;
using Qorpent.Utils.Extensions;

namespace Qorpent.Core.Tests.v2.xdom
{
    [TestFixture]
    public class HtmlElementTest
    {
        [Test]
        public void InitialStructure() {
            var html = new HtmlElement();
            Assert.NotNull(html.Head);
            Assert.NotNull(html.Body);
            var res = html.ToDocument().ToString().Simplify(SimplifyOptions.SingleQuotes);
            Console.WriteLine(res);
            Assert.AreEqual(@" < !DOCTYPE html >
<html>
  <head>
    <meta charset='utf-8' />
  </head>
  <body />
</html>", res);
        }

        [Test]
        public void CanDispatchItems()
        {
            var html = new HtmlElement(new MetaElement(new XAttribute("x","y")), "text", new XElement("div", "a"),
                
                new XElement("HeAd",new StyleElement(".c{}")),
                new XAttribute("zzz", "val"),
                ScriptElement.RequireJs());
            var res = html.ToDocument().ToString().Simplify(SimplifyOptions.SingleQuotes);
            Console.WriteLine(res);
            Assert.AreEqual(@"<!DOCTYPE html >
<html zzz='val'>
  <head>
    <meta charset='utf-8' />
    <meta x='y' />
    <style type='text/css'>.c{}</style>
    <script type='text/javascript' data-main='scripts/main.js' src='scripts/require.js'></script>
  </head>
  <body>text<div>a</div></body>
</html>", res);
        }
    }
}

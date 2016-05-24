using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.BSharp.Builder.Tasks.xslt;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Builder.Tests
{
    public class TestProject : BSharpProject {
        public TestProject() {
            this.RootDirectory = "/root";
            this.MainOutputDirectory = "/out";
            this.CompileFolder = "/compile";
        
            var context = new BSharpContext();
            context.Working.Add(new BSharpClass
            {
                Name = "x",
                Prototype = "a",
                Compiled = new XElement("class", new XAttribute("attr", "x"))
            });
            context.Working.Add(new BSharpClass
            {
                Name = "y",
                Prototype = "a",
                Compiled = new XElement("class", new XAttribute("attr", "y"))
            });
            context.Working.Add(new BSharpClass
            {
                Name = "z",
                Prototype = "b",
                Compiled = new XElement("class", new XAttribute("attr", "y"))
            });
            context.BuildIndexes();
            this.Context = context;
        }
    }

    [TestFixture]
    public class BSharpXmlResolverTest {
        private readonly BSharpProject _project = new TestProject();

        [Test]
        public void ReadClassXml() {
            var resolver = new BSharpXmlResolver(_project);
            var xml = XElement.Load((Stream) resolver.GetEntity(new Uri("classes://a"), "", typeof (Stream)));
            string _xml;
            Console.WriteLine(_xml = xml.ToString().Simplify(SimplifyOptions.SingleQuotes));
            Assert.AreEqual(@"<classes>
  <class attr='x' />
  <class attr='y' />
</classes>", _xml);
        }
    }

    [TestFixture]
    public class ProjectUrlResolverTest {
        private readonly BSharpProject _project = new TestProject();
        

        [TestCase("project://x/a.xslt","/root/x/a.xslt")]
        [TestCase("output://x/a.xslt","/out/x/a.xslt")]
        [TestCase("compile://x/a.xslt","/compile/x/a.xslt")]
        public void ResolvesFilePath(string url, string path) {
            var resolver = new ProjectUriResolver(_project);
            Assert.AreEqual(path,resolver.GetPath(new Uri(url)).Replace("c:",""));
        }

        [TestCase("class://x","x")]
        [TestCase("class://y","y")]
        [TestCase("class://z","z")]
        [TestCase("class://w","")]
        [TestCase("class://?a","x")]
        [TestCase("classes://a","x,y")]
        [TestCase("classes://b","z")]
        [TestCase("classes://?a","x,y")]
        [TestCase("classes://?attr:y","y,z")]
        public void ResolveClasses(string url, string names) {
            var resolver = new ProjectUriResolver(_project);
            var classlist = string.Join(",", resolver.GetClasses(new Uri(url)).Select(_ => _.Name).OrderBy(_ => _));
            Assert.AreEqual(names, classlist);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Host.Server;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Tests
{
    /// <summary>
    /// Can load from webfarm dir
    /// </summary>
    [TestFixture]
    public class ServerParametersTest
    {
        [Test]
        public void CanReadConfigFromWebFarmRoot() {
            var dir = Path.Combine(Path.GetTempPath(), "ServerParametersTest");
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir,"a.hostconf"),@"
class x
    y=23
");
            var parameters = new ServerParameters().Initialize("x","-fr",dir);
            Assert.AreEqual("23",parameters.Definition.Attr("y"));
        }
    }
}

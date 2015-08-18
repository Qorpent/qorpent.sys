using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Config;
using Qorpent.IO;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Core.Tests.Config
{
    [TestFixture]
    public class ConfigLoaderTest
    {
        private string dir;

        [SetUp]
        public void Setup() {
            dir = FileSystemHelper.ResetTemporaryDirectory();
            opts = new ConfigurationOptions {
                FileSet = new FileSet(dir,"*.bxls")
            };
            writefile("simple_config", simple_config);
            loader = new ConfigurationLoader(opts);
        }

        void writefile(string name, string data) {
            if (string.IsNullOrWhiteSpace(Path.GetExtension(name))) {
                name += ".bxls";
            }
            File.WriteAllText(Path.Combine(dir,name),data);
        }

        private string simple_config = @"
class A
class B
class C if='APP_A || !APP'
";
        private ConfigurationOptions opts;
        private ConfigurationLoader loader;

        [Test]
        public void CanLoadSimpleNonNamed() {
           
            var config = loader.Load();
            var context = config.GetContext();
            Assert.NotNull(context.Get("A"));
            Assert.NotNull(context.Get("B"));
            Assert.NotNull(context.Get("C"));
            Assert.Null(config.GetConfig());
        }

        [Test]
        public void CanLoadNamedA() {
            opts.Name = "A";
            var config = loader.Load();
            var context = config.GetContext();
            Assert.NotNull(context.Get("A"));
            Assert.NotNull(context.Get("B"));
            Assert.NotNull(context.Get("C"));
            Assert.NotNull(config.GetConfig());
            Assert.AreEqual("A",config.GetConfig().Attr("code"));
        }

        [Test]
        public void CanLoadNamedB()
        {
            opts.Name = "B";
            var config = loader.Load();
            var context = config.GetContext();
            Assert.NotNull(context.Get("A"));
            Assert.NotNull(context.Get("B"));
            Assert.Null(context.Get("C"));
            Assert.NotNull(config.GetConfig());
            Assert.AreEqual("B", config.GetConfig().Attr("code"));
        }
    }
}

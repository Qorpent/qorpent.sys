using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp
{
    [TestFixture]
    public class BscHelperTest
    {
        private string dir;
        private string bxls;
        private string bsproj;
        private string mybxls;

        [SetUp]
        public void setup() {
            dir = FileSystemHelper.ResetTemporaryDirectory();
            bxls = Path.Combine(dir, "test.bxls");
            mybxls = Path.Combine(dir, "test.mybxls");
            bsproj = Path.Combine(dir, "test.bsproj");
        }

        [Test]
        public void CanCompileClass() {
            File.WriteAllText(bxls,@"
class A prototype=x
    hello world
");
            var ctx = BscHelper.Execute(dir);
            Assert.NotNull(ctx);
            Assert.AreEqual(0,ctx.GetErrors().Count());
            var cls = ctx["A"];
            Assert.NotNull(cls);
            Assert.AreEqual("x",cls.Prototype);
            Assert.AreEqual(@"<class code=""A"" prototype=""x"" fullcode=""A"">
  <hello code=""world"" />
</class>".LfOnly(),cls.Compiled.ToString().LfOnly());
        }

        [Test]
        public void CanCompileProject() {
            File.WriteAllText(mybxls, @"
class B prototype=x
    hello world
");
            File.WriteAllText(bsproj, @"
class myproj
    InputExtensions mybxls
");
            var ctx = BscHelper.Execute(dir, "myproj");
            Assert.AreEqual(0,ctx.GetErrors().Count());
            Assert.AreEqual(1,ctx.Get(BSharpContextDataType.Working).Count());
            var cls = ctx["B"];
            Assert.NotNull(cls);
            Assert.AreEqual("x", cls.Prototype);
            Assert.AreEqual(@"<class code=""B"" prototype=""x"" fullcode=""B"">
  <hello code=""world"" />
</class>".LfOnly(), cls.Compiled.ToString().LfOnly()); 
        }

        [Test]
        public void CanCatchError()
        {
            File.WriteAllText(bxls, @"

my A prototype=x
    hello world

");
            var ctx = BscHelper.Execute(dir);
            Assert.NotNull(ctx);
            Assert.AreEqual(1, ctx.GetErrors().Count());
            var error = ctx.GetErrors().First();
            Assert.AreEqual(BSharpErrorType.OrphanClass, error.Type);
            Assert.AreEqual("test.bxls", Path.GetFileName(error.LexInfo.File));
            Assert.AreEqual(3,error.LexInfo.Line);
            Console.WriteLine(error.Message);
            Assert.AreEqual(@"OrphanClass:SourceIndexing В коде обнаружен участок, похожий на класс, но который нельзя связать ни с одной из имеющихся базовых классов или ключевым словом class (A,)", error.Message);
        }


        [Test]
        public void CanBeUsedAsBasisForPersistentModel() {
            File.WriteAllText(bxls, @"
require data
TableBase mytable schema=test
    import IEntity
");
            var ctx = BscHelper.Execute(dir);
            Assert.AreEqual(0,ctx.GetErrors().Count());
            var pm = new PersistentModel();
            pm.Setup(ctx);
            var table = pm["test.mytable"];
            Assert.NotNull(table);
            Assert.True(table.Fields["id"].DataType.Code == "long");
        }

        [Test]
        public void BugInResolveAll() {
            File.WriteAllText(bxls, @"
class A prototype=xxx
");
            Thread.Sleep(100);
            var ctx = BscHelper.Execute(dir);
            var find = ctx.ResolveAll("xxx");
            Assert.AreEqual(1,find.Count());
        }
    }
}

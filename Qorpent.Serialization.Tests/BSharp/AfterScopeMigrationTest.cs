using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.BSharp;

namespace Qorpent.Serialization.Tests.BSharp
{
    [TestFixture]
    public class AfterScopeMigrationTest
    {
        [Test]
        public void BUG_CannotInheritTwoStatics() {

            var code = @"
class a static
a b	static
	";
            var result = BSharpCompiler.Compile(code);
            foreach (var error in result.GetErrors()) {
                Console.WriteLine(error.ToLogString());
            }
            Assert.AreEqual(0,result.GetErrors().Count());

        }
    }
}

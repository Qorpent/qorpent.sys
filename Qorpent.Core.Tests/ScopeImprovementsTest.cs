using System.Net;
using NUnit.Framework;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.Core.Tests {
    [TestFixture]
    public class ScopeImprovementsTest {
        private Scope scope;

        [SetUp]
        public void setup() {
            scope = new Scope(
                new { a = 1, b = 1, x = 1 },
                new Scope(
                    new { a = 2, c = 2, z = 3 },
                    new Scope(
                        new { a = 3, b = 3, d = 3 }
                        )

                    )
                );
        }

        [Test]
        public void RequiredLevelPathOperator() {
     
            Assert.AreEqual(2, scope["^a"]);
            Assert.AreEqual(3, scope[".^a"]);
            Assert.AreEqual(3, scope["^.a"]);
            Assert.AreEqual(null, scope["^x"]);
            scope["^z"] = 4;
            Assert.AreEqual(4, scope["^z"]);

        }

        [Test]
        public void DisableLastReturnOnSkips()
        {
            Assert.AreEqual(3,scope[".....a"]);
            scope.Options.LastOnSkipOverflow = false;
            Assert.Null(scope[".....a"]);

        }

        [Test]
        public void SimplificationSupport() {
            Assert.AreEqual(null, scope["--A"]);
            scope.Options.KeySimplification = SimplifyOptions.Full;
            Assert.AreEqual(1,scope["--A"]);
        }
    }
}
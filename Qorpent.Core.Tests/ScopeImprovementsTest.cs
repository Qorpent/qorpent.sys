using System.Linq;
using System.Net;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Core.Tests {
    [TestFixture]
    public class ScopeImprovementsTest {
        private Scope scope;

        [SetUp]
        public void setup() {
            scope = new Scope(
                new { a = 1, b = 1, x = 1, notrealnull = (object)null, realnull=(object)Scope.Null,ex=Scope.Append("1"),ex2=Scope.Append("1",",","^ex") },
                new Scope(
                    new { a = 2, c = 2, z = 3, notrealnull = "nrn", realnull = "rn", ex=Scope.Append("2")},
                    new Scope(
                        new { a = 3, b = 3, d = 3, ex=Scope.Append(3) }
                        )

                    )
                );
        }


        [Test]
        public void SupportDotedNamesForJson()
        {
            var cfg = new Scope();
            cfg.Set("a", new {b=1});
            var cfg2 = new Scope(cfg);
            cfg2.Set("a", new {b=3});

            Assert.AreEqual(3, cfg2.Get("a.b"));
            Assert.AreEqual(1, cfg2.Get(".a.b"));
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
        public void MultipleSources() {
            var scopeA = new Scope(new {a = 1, b = 1});
            var scopeB = new Scope(new {a = 2, c = 2});
            var scope = new Scope(new {a = 3, b = 3, c = 3}, scopeA, scopeB);
            Assert.AreEqual(1,scope[".a"]);
            Assert.AreEqual(1,scope[".b"]);
            Assert.AreEqual(2,scope[".c"]);
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

        [Test]
        public void NullSupport() {
            Assert.AreEqual("nrn",scope["notrealnull"]);
            Assert.Null(scope["realnull"]);
        }

        [Test]
        public void AppendSupport() {
            Assert.AreEqual("3 2 1",scope["ex"]);
            Assert.AreEqual("3 2,1",scope["ex2"]);
        }

        class MyNames : IScopeBound {
            public object Get(IScope scope, string key, ScopeOptions options) {
                return string.Join(", ", scope.GetKeys().OrderBy(_=>_));
            }
        }
        [Test]
        public void CustomIScopeBound() {
            scope["mynames"] = new MyNames();
            Assert.AreEqual("a, b, c, d, ex, ex2, mynames, notrealnull, realnull, x, z", scope["mynames"]);
        }
    }
}
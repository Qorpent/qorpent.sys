using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.LogicalExpressions;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.Core.Tests.Utils.LogicalExpressions
{
    [TestFixture]
    public class LogicalExpressionsTest   
    {
        ILogicalExpressionEvaluator le = new LogicalExpressionEvaluator();
        [Test]
        public void SingleConstant() {
            Assert.True(le.Eval("my",new{my=true}));
            Assert.False(le.Eval("my2",new{my=true}));
            Assert.False(le.Eval("my",new{my=false}));
        }

        [Test]
        public void LiteralsWithUnderscore_Q515() {
            
            Assert.True(le.Eval("my_1",new {my_1=true}));
        }

        [Test]
        public void NoSpace() {
            Assert.True(le.Eval("my_1='x'", new { my_1 = "x" }));
        }

        [Test]
        public void DoubleSyntax_Q517() {
            Assert.True(le.Eval("my_1 == 'x'", new { my_1 = "x" }));
            Assert.True(le.Eval("my_1&& my_1", new { my_1 = "x" }));
            Assert.True(le.Eval("my_1 ||my_1", new { my_1 = "x" }));
        }

        [Test]
        public void CompareStringFirst() {
            Assert.True(le.Eval("my_1 == 'x'", new { my_1 = "x" }));
            Assert.False(le.Eval("my_1 == 'y'", new { my_1 = "x" }));
            Assert.True(le.Eval("'x' == my_1", new { my_1 = "x" }));
            Assert.False(le.Eval("'y' == my_1", new { my_1 = "x" }));
        }

        [Test]
        public void NotSign() {
            Assert.False(le.Eval("!my", new {my = true}));
        }

        [Test]
        public void Eq() {
            Assert.True(le.Eval("my = 'x'",new{my = "x"}));
            Assert.False(le.Eval("my != 'x'",new{my = "x"}));
            Assert.True(le.Eval("my != 'y'",new{my = "x"}));
        }

        [Test]
        public void EqNum()
        {
            Assert.True(le.Eval("my = 10", new { my = 10 }));
            Assert.True(le.Eval("my = 10.2", new { my = 10.2 }));
            Assert.False(le.Eval("my != 10", new { my = 10 }));
            Assert.True(le.Eval("my != 9", new { my = "10" }));
            Assert.True(le.Eval("my = 10", new { my = "10" }));
        }


        [Test]
        public void CompareNum()
        {
            Assert.True(le.Eval("my > 10", new { my = 11 }));
            Assert.True(le.Eval("my >= 10", new { my = 11 }));
            Assert.True(le.Eval("my >= 11", new { my = 11 }));
            Assert.False(le.Eval("my > 15", new { my = 12 }));
            Assert.False(le.Eval("my > 12", new { my = 12 }));
            Assert.False(le.Eval("my < 12", new { my = 12 }));
            Assert.True(le.Eval("my <= 12", new { my = 12 }));
            Assert.True(le.Eval("my < 15", new { my = "12" }));
            Assert.False(le.Eval("my < 15", new { my = 16 }));
        }

        [Test]
        public void OrGroupsAndAnd() {
            var obj = new {a = true, b = false, c = true};
            Assert.True(le.Eval("(a | b) & (b | c)",obj));
            Assert.False(le.Eval("(a & b) | (b & c)",obj));
        }

        [Test]
        public void SafeParsing()
        {
            Assert.False(le.Eval("! (my )", new { my = true }));
            Assert.True(le.Eval("!(! (my ))", new { my = true }));
        }

        [TestCase("X ~ 'a'", false)]
        [TestCase("X~'a'", false)]
        [TestCase("X~ 'b'", true)]
        [TestCase("X ~'b'", true)]
        [TestCase("'bbb' ~ 'b'", true)]
        [TestCase("'b' ~ 'bbb'", false)]
        [TestCase("'bbbbbb' ~ X", true)]
        [TestCase("'bb' ~ X", false)]
        public void RegexSupportQ523(string cond, bool result) {
            Assert.AreEqual(result,le.Eval(cond,new {X="bbb"}));
        }
    }
}

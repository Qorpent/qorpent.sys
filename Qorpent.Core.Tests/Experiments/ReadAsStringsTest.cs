using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Bridge.Tests.Utils
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class ReadAsStringsTest
    {

        [TestCase("aa,bb,cc")]
        [TestCase("aa,bb;cc")]
        [TestCase("aa/bb;cc")]
        [TestCase(" aa/ bb; cc")]
        [TestCase(" aa / bb ; cc  ")]
        [TestCase(" aa //// bb ; cc,,,,,  ")]
        public void ReadAsStringsNotInternalWS(string variant) {
            var result = StringUtils.ReadAsStrings(variant);
            Assert.AreEqual(3,result.Length);
            Assert.AreEqual("aa",result[0]);
            Assert.AreEqual("bb",result[1]);
            Assert.AreEqual("cc",result[2]);
        }
        [TestCase("a a,b  b,c   c")]
        [TestCase("a a,b  b;c   c")]
        [TestCase("a a/b  b;c   c")]
        [TestCase(" a a/ b  b; c   c")]
        [TestCase(" a a / b  b ; c   c  ")]
        [TestCase(" a a //// b  b ; c   c,,,,,  ")]
        public void ReadAsStringsInternalWS(string variant)
        {
            var result = StringUtils.ReadAsStrings(variant);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("a a", result[0]);
            Assert.AreEqual("b  b", result[1]);
            Assert.AreEqual("c   c", result[2]);
        }

        [Test]
        [Explicit]
        public void Timestamp() {
            var str = "";
            for (var i = 0; i < 1000; i++) {
                str += " ,, " + i.ToString().Replace("7",";").Replace("8","   ");
            }
            Console.WriteLine("String prepared");
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 10000; i++) {
                var x = StringUtils.ReadAsStrings(str);
            }
            sw.Stop();
            Console.WriteLine("Bridge:");
            Console.WriteLine(sw.Elapsed);
            sw = Stopwatch.StartNew();
            for (var i = 0; i < 10000; i++) {
                var x = str.SmartSplit();
            }
            sw.Stop();
            Console.WriteLine("OldSys:");
            Console.WriteLine(sw.Elapsed);
        }
    }
}

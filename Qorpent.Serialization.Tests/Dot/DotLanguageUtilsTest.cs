using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Dot;

namespace Qorpent.Serialization.Tests.Dot
{
    [TestFixture]
    public class DotLanguageUtilsTest
    {
        [TestCase("aРус", "aРус")]
        [TestCase("a1", "a1")]
        [TestCase("_a1", "_a1")]
        [TestCase("1a", "_0x0031a")]
        [TestCase("a,b", "a_0x002Cb")]
        [TestCase("a,b>c", "a_0x002Cb_0x003Ec")]
        public void CodeEscaping(string code, string result) {
            Assert.AreEqual(result,DotLanguageUtils.EscapeCode(code));
        }

        [TestCase("aРус", "aРус")]
        [TestCase("a1", "a1")]
        [TestCase("_a1", "_a1")]
        [TestCase("1a", "_0x0031a")]
        [TestCase("a,b", "a_0x002Cb")]
        [TestCase("a,b>c", "a_0x002Cb_0x003Ec")]
        public void CodeUnEscaping(string result, string code)
        {
            Assert.AreEqual(result, DotLanguageUtils.UnEscapeCode(code));
        }
    }
}

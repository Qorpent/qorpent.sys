using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.logon.services;

namespace qorpent.v2.security.Tests.logon.services
{
    [TestFixture]
    public class PasswordPolicyTest
    {
        [TestCase("",0)]
        [TestCase(null,0)]
        [TestCase("aaaaaaaa",0)]
        [TestCase("aAaaaaaa",0)]
        [TestCase("aAaaaaa1",0)]
        [TestCase("aAaebcd1",1)]
        [TestCase("aAaebcd_",1)]
        [TestCase("cde3$RFV",1)]
        [TestCase("cde3$RFVBGT%",2)]
        public void LeveledCheck(string password, int expectedRate) {
            var policy = new PasswordPolicy(password);
            Console.WriteLine(policy);
            Assert.AreEqual(expectedRate,policy.Rate);
        }
    }
}

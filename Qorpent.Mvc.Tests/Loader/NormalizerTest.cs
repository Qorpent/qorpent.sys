using System;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Qorpent.Mvc.Loader;

namespace Qorpent.Mvc.Tests.Loader
{
    [TestFixture]
    public class NormalizerTest
    {
        [TestCase(2, LoadLevel.Admin, @"^aa\:aag\.js,aau\.js,aaa\.js,;au\:auu\.js,aua\.js,;ag\:agu\.js,;$")] // по зависимостям не будет резолюции
        [TestCase(2, LoadLevel.Auth, @"^aa\:aag\.js,aau\.js,;au\:auu\.js,;ag\:agu\.js,;$")] // по зависимостям не будет резолюции
        [TestCase(2, LoadLevel.Guest, @"^$")] // по зависимостям не будет резолюции
        [TestCase(1,LoadLevel.Guest, @"^ag\:agg\.js,;$")]
        [TestCase(1,LoadLevel.Auth, @"^ag\:agg\.js,agu.js,;au:aug.js,auu.js,;$")]
        [TestCase(1,LoadLevel.Admin, @"^ag\:agg\.js,agu\.js,aga\.js,;au\:aug\.js,auu.js,aua.js,;aa\:aag\.js,aau\.js,aaa\.js,;$")]
        public void Valid_Level_Split(int setnumber,LoadLevel level, string contains) {
            var raw = PackageGenerator.Get(setnumber);
            var result = new LoadPackageSet(raw);
            var subset = result[level];
            var str = Stringify(subset);
            Console.WriteLine(str);
            Assert.True(Regex.IsMatch(str,contains));
        }

        private string Stringify(LoadPackage[] subset) {
            var sb = new StringBuilder();
            foreach (var package in subset) {
                sb.Append(package.Code);
                sb.Append(":");
                foreach (var i in package.Items) {
                    sb.Append(i.Value);
                    sb.Append(",");
                }
                sb.Append(";");
            }
            return sb.ToString();
        }
    }
}

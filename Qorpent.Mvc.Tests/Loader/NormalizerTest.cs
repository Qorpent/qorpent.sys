using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Qorpent.Mvc.Loader;

namespace Qorpent.Mvc.Tests.Loader
{
    [TestFixture]
    public class NormalizerTest
    {
        public IEnumerable<LoadPackage> GetDependentPackages()
        {
            yield return new LoadPackage
            {
                Code = "ag",
                Level = LoadLevel.Guest,
                Items = {
                    new LoadItem {Value = "agu.js"},
                },
                Dependency= {
                    {"au",null},
                    {"aa",null}
                }
            };
            yield return new LoadPackage
            {
                Code = "au",
                Level = LoadLevel.Guest,
                Items = {
                    new LoadItem {Value = "auu.js", Level = LoadLevel.Auth},
                    new LoadItem {Value = "aua.js", Level = LoadLevel.Admin},
                },
                Dependency = {
                    {"aa",null}
                }
            };
            yield return new LoadPackage
            {
                Code = "aa",
                Level = LoadLevel.Auth,
                Items = {
                    new LoadItem {Value = "aag.js", Level = LoadLevel.Guest},
                    new LoadItem {Value = "aau.js", Level = LoadLevel.Auth},
                    new LoadItem {Value = "aaa.js", Level = LoadLevel.Admin},
                }
            };
        }


        public IEnumerable<LoadPackage> GetLeveledPackages() {
            yield return new LoadPackage {
                Code = "ag",
                Level = LoadLevel.Guest,
                Items = {
                    new LoadItem {Value = "agg.js", Level = LoadLevel.Guest},
                    new LoadItem {Value = "agu.js", Level = LoadLevel.Auth},
                    new LoadItem {Value = "aga.js", Level = LoadLevel.Admin},
                }
            };
            yield return new LoadPackage
            {
                Code = "au",
                Level = LoadLevel.Auth,
                Items = {
                    new LoadItem {Value = "aug.js", Level = LoadLevel.Guest},
                    new LoadItem {Value = "auu.js", Level = LoadLevel.Auth},
                    new LoadItem {Value = "aua.js", Level = LoadLevel.Admin},
                }
            };
            yield return new LoadPackage
            {
                Code = "aa",
                Level = LoadLevel.Admin,
                Items = {
                    new LoadItem {Value = "aag.js", Level = LoadLevel.Guest},
                    new LoadItem {Value = "aau.js", Level = LoadLevel.Auth},
                    new LoadItem {Value = "aaa.js", Level = LoadLevel.Admin},
                }
            };
        }
        [TestCase(2, LoadLevel.Admin, @"^aa\:aag\.js,aau\.js,aaa\.js,;au\:auu\.js,aua\.js,;ag\:agu\.js,;$")] // по зависимостям не будет резолюции
        [TestCase(2, LoadLevel.Auth, @"^aa\:aag\.js,aau\.js,;au\:auu\.js,;ag\:agu\.js,;$")] // по зависимостям не будет резолюции
        [TestCase(2, LoadLevel.Guest, @"^$")] // по зависимостям не будет резолюции
        [TestCase(1,LoadLevel.Guest, @"^ag\:agg\.js,;$")]
        [TestCase(1,LoadLevel.Auth, @"^ag\:agg\.js,agu.js,;au:aug.js,auu.js,;$")]
        [TestCase(1,LoadLevel.Admin, @"^ag\:agg\.js,agu\.js,aga\.js,;au\:aug\.js,auu.js,aua.js,;aa\:aag\.js,aau\.js,aaa\.js,;$")]
        public void Valid_Level_Split(int setnumber,LoadLevel level, string contains) {
            var raw = 1==setnumber?GetLeveledPackages():GetDependentPackages();
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

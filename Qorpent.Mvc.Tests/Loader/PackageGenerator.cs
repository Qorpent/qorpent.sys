using System.Collections.Generic;
using Qorpent.Mvc.Loader;

namespace Qorpent.Mvc.Tests.Loader {
    public static class PackageGenerator {
        public static IEnumerable<LoadPackage> GetDependentPackages()
        {
            yield return new LoadPackage
            {
                Code = "ag",
                Level = LoadLevel.Guest,
                Items = {
                    new LoadItem {Value = "agu.js"},
                },
                Dependency = {"au","aa"}
            };
            yield return new LoadPackage
            {
                Code = "au",
                Level = LoadLevel.Guest,
                Items = {
                    new LoadItem {Value = "auu.js", Level = LoadLevel.Auth},
                    new LoadItem {Value = "aua.js", Level = LoadLevel.Admin},
                },
                Dependency = {"aa"}
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


        public static IEnumerable<LoadPackage> GetLeveledPackages()
        {
            yield return new LoadPackage
            {
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


        public static IEnumerable<LoadPackage> GetFullContentPackages()
        {
            yield return new LoadPackage
            {
                Code = "ag",
                Level = LoadLevel.Guest,
                Items = {
                    new LoadItem {Value = "agg.js", Level = LoadLevel.Guest},
                    new LoadItem {Value = "agu.css", Level = LoadLevel.Auth},
                    new LoadItem {Value = "aga.html", Level = LoadLevel.Admin},
                }
            };
            yield return new LoadPackage
            {
                Code = "au",
                Level = LoadLevel.Auth,
                Items = {
                    new LoadItem {Value = "aug.css", Level = LoadLevel.Guest},
                    new LoadItem {Value = "auu.html", Level = LoadLevel.Auth},
                    new LoadItem {Value = "aua.js", Level = LoadLevel.Admin},
                }
            };
            yield return new LoadPackage
            {
                Code = "aa",
                Level = LoadLevel.Admin,
                Items = {
                    new LoadItem {Value = "aag.html", Level = LoadLevel.Guest},
                    new LoadItem {Value = "aau.js", Level = LoadLevel.Auth},
                    new LoadItem {Value = "aaa.css", Level = LoadLevel.Admin},
                }
            };
        }

        public static IEnumerable<LoadPackage> Get(int setnumber) {
            if (1 == setnumber) return GetLeveledPackages();
            if (2 == setnumber) return GetDependentPackages();
            return GetFullContentPackages();

        }
    }
}
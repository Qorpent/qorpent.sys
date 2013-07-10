using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Mvc.Loader {
    /// <summary>
    /// Нормализованный и подчищенный набор пакетов
    /// </summary>
    public class LoadPackageSet : IComparer<LoadPackage> {
        /// <summary>
        /// def ctor
        /// </summary>
        /// <param name="packages"></param>
        public LoadPackageSet(IEnumerable<LoadPackage> packages) {
            Normalize(packages);
        }

        private void Normalize(IEnumerable<LoadPackage> packages) {
            var pkgs = packages.ToArray();
            GuestSet = Normalize(pkgs, LoadLevel.Guest);
            AuthSet = Normalize(pkgs, LoadLevel.Auth);
            AdminSet = Normalize(pkgs, LoadLevel.Admin);
        }

        private LoadPackage[] Normalize(LoadPackage[] packages, LoadLevel level) {
            var pkgsdict = packages.Where(_=>_.Level<=level).ToDictionary(_ => _.Code, _ => _);
            var resultpkg = new Dictionary<string, LoadPackage>();
            foreach (var package in pkgsdict) {
                resultpkg[package.Key] = package.Value.Clone();
            }
            foreach (var rp in resultpkg.ToArray()) {
                foreach (var item in rp.Value.Items.ToArray()) {
                    if (item.Level > level) {
                        rp.Value.Items.Remove(item);
                    }
                }
                if (rp.Value.Items.Count == 0) resultpkg.Remove(rp.Key);
            }
            foreach (var rp in resultpkg.ToArray()) {
                bool remove = false;
                foreach (var dep in rp.Value.Dependency.ToArray())
                {
                    if (resultpkg.ContainsKey(dep.Key)) {
                        rp.Value.Dependency[dep.Key] = resultpkg[dep.Key];
                    }
                    else {
                        remove = true;
                        break;
                    }
                    
                }
                if (remove) {
                    resultpkg.Remove(rp.Key);

                }

            }
            return resultpkg.Values.OrderBy(_=>_,this).ToArray();
        }

        private LoadPackage[] GuestSet;
        private LoadPackage[] AuthSet;
        private LoadPackage[] AdminSet;

        /// <summary>
        /// Получить набор по уровню
        /// </summary>
        /// <param name="level"></param>
        public LoadPackage[] this[LoadLevel level] {
            get { 
                if (level == LoadLevel.Guest) return GuestSet;
                if (level == LoadLevel.Auth) return AuthSet;
                if (level == LoadLevel.Admin) return AdminSet;
                throw new Exception("unknown level");
            }
        }
        /// <summary>
        /// Сравнивает порядок пакетов
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(LoadPackage x, LoadPackage y) {
            if (y.Dependency.ContainsKey(x.Code)) return -1;
            if (x.Dependency.ContainsKey(y.Code)) return 1;
            if (x.Level != y.Level) return x.Level.CompareTo(y.Level);
            return System.String.Compare(x.Code, y.Code, StringComparison.Ordinal);
        }
    }
}
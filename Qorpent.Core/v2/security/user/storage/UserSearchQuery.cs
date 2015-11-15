using System.Linq;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.user.storage {
    public class UserSearchQuery {
        public bool Groups { get; set; } = true;
        public bool Users { get; set; } = true;
        public string Domain { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }

        public bool IsMatch(IUser user) {
            if (user.IsGroup) {
                if (!Groups) return false;
            }
            else {
                if (!Users) return false;
                if (!string.IsNullOrWhiteSpace(Domain)) {
                    if (Domain != user.Domain) return false;
                }
            }
            if (!CheckString(user.Name, Name)) return false;
            if (!CheckString(user.Login, Login)) return false;

            return true;
        }

        private bool CheckString(string tname, string name) {
            if (string.IsNullOrWhiteSpace(name)) return true;
            tname = tname.ToLowerInvariant();
            name = name.ToLowerInvariant();
            var namewords = tname.ToLowerInvariant().SmartSplit(false, true, ' ', ',', ';', '@', '/', '\\');
            if (Name.EndsWith("*")) {
                if (!namewords.Any(_ => _.StartsWith(name.Substring(0, Name.Length - 1)))) {
                    return false;
                }
            }
            else {
                if (!namewords.Contains(name)) {
                    return false;
                }
            }
            return true;
        }
    }
}
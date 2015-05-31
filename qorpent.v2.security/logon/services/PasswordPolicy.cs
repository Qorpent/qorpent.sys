using System.Linq;
using Qorpent.Experiments;

namespace qorpent.v2.security.logon.services {
    public class PasswordPolicy : IPasswordPolicy {
        public PasswordPolicy(string password) {
            password = password ?? "";
            Size = password.Length;
            Distinct = password.Distinct().Count();
            HasUpper = password.Any(char.IsUpper);
            HasLower = password.Any(char.IsLower);
            HasDigit = password.Any(char.IsDigit);
            HasSign = password.Any(_ => !char.IsLetter(_) && !char.IsDigit(_));
        }

        public int Size { get; set; }
        public int Distinct { get; set; }
        public bool HasUpper { get; set; }
        public bool HasDigit { get; set; }
        public bool HasLower { get; set; }
        public bool HasSign { get; set; }

        public int DifferentRate {
            get {
                var result = 0;
                if (HasUpper) {
                    result++;
                }
                if (HasLower) {
                    result++;
                }
                if (HasDigit) {
                    result++;
                }
                if (HasSign) {
                    result++;
                }
                return result;
            }
        }

        public bool SizeOk {
            get { return Size >= 8; }
        }

        public bool SizeGood {
            get { return Size >= 12; }
        }

        public bool DistinctOk {
            get { return (Distinct/(double) Size) > 0.8; }
        }

        public bool DistinctGood {
            get { return Distinct == Size; }
        }

        public bool DifferenceOk {
            get { return DifferentRate >= 3; }
        }

        public bool DifferenceGood {
            get { return DifferentRate >= 4; }
        }

        public int Rate {
            get {
                if (Good) {
                    return 2;
                }
                if (Ok) {
                    return 1;
                }
                return 0;
            }
        }

        public bool Ok {
            get { return SizeOk && DistinctOk && DifferenceOk; }
        }

        public bool Good {
            get { return SizeGood && DistinctGood && DifferenceGood; }
        }

        public override string ToString() {
            return this.stringify();
        }
    }
}
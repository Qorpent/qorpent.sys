using Qorpent.Utils.Extensions;

namespace Qorpent {
    public class ScopeOptions {
        public bool LastOnSkipOverflow = true;
        public bool UseInheritance = true;
        public int MaxLevel = -1;
        public int SkipLevels;
        public int ResultCount;
        public bool TreatFirstDotAsLevelUp = true;
        public SimplifyOptions KeySimplification = SimplifyOptions.None;

        public int SkipResults;

        public ScopeOptions LevelUp(int resultCount) {
            var result = Copy();
            if (result.SkipLevels > 0) {
                result.SkipLevels--;
            }

            if (result.MaxLevel > 0) {
                result.MaxLevel--;
                if (MaxLevel == 0) {
                    result.UseInheritance = false;
                }
            }
            result.Level++;
           
            result.ResultCount = resultCount;

            return result;
        }

        public int Level;

        public ScopeOptions Copy() {
            return (ScopeOptions) MemberwiseClone();
        }
    }
}
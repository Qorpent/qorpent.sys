using System.Text.RegularExpressions;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.LogicalExpressions;

namespace Qorpent.Utils.LogicalExpressions {
    public class RegexTestNode : LogicalExpressionNode {
        public string First { get; set; }
        public bool FirstIsLiteral { get; set; }
        public string Second { get; set; }
        public bool SecondIsLiteral { get; set; }
        protected override bool InternalEval(ILogicTermSource source) {
            var text = FirstIsLiteral ? source.Value(First) : First;
            var regex = SecondIsLiteral ? source.Value(Second) : Second;
            return Regex.IsMatch(text, regex);

        }
    }
}
using System.Security.Principal;

namespace qorpent.v2.security.authorization {
    public interface IRoleExpressionEvaluator {
        bool Evaluate(IRoleResolver resolver, IIdentity identity, string roleexpression);
    }
}
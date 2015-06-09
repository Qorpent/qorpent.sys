using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Qorpent.IoC;
using Qorpent.LogicalExpressions;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;

namespace qorpent.v2.security.authorization {
    /// <summary>
    /// 
    /// </summary>
    [ContainerComponent(Lifestyle.Singleton,"sys.sec.roleeval",ServiceType=typeof(IRoleExpressionEvaluator))]
    public class RoleExpressionEvaluator : IRoleExpressionEvaluator {
        readonly LogicalExpressionEvaluator _e = new LogicalExpressionEvaluator();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="identity"></param>
        /// <param name="roleexpression"></param>
        /// <returns></returns>
        public bool Evaluate(IRoleResolver resolver, IIdentity identity, string roleexpression) {
            if (-1!=roleexpression.IndexOf(',')) {
                return EvaluateOrList(resolver,identity, roleexpression.SmartSplit(false, true, ','));
            }
            if (-1 != roleexpression.IndexOf('+')) {
                return EvaluateAndList(resolver,identity, roleexpression.SmartSplit(false, true, '+'));
            }
            if (-1 != roleexpression.IndexOfAny(new[] {' ', '&', '|', '(', ')'})) {
                return EvaluateFormula(resolver,identity, roleexpression);
            }
            if (roleexpression[0] == '!' || roleexpression[0] == '-') {
                return !resolver.IsInRole(identity, roleexpression.Substring(1));
            }
            return resolver.IsInRole(identity,roleexpression);
        }

        class RoleTermSource : ILogicTermSource {
            private readonly IRoleExpressionEvaluator _eval;
            private readonly IRoleResolver _resolver;
            private readonly IIdentity _identity;

            public RoleTermSource(IRoleExpressionEvaluator eval, IRoleResolver resolver, IIdentity identity) {
                _eval = eval;
                _resolver = resolver;
                _identity = identity;
            }

            public bool Get(string name) {
                return _eval.Evaluate(_resolver, _identity, name);
            }

            public bool Equal(string name, string value, bool isNumber = false) {
                return Get(name).Equals(value.ToBool());
            }

            public string Value(string name) {
                return Get(name).ToStr();
            }

            static readonly IDictionary<string,string> Empty  = new Dictionary<string, string>(); 

            public IDictionary<string, string> GetAll() {
                return Empty;
            }
        }

        private bool EvaluateFormula(IRoleResolver resolver,IIdentity identity, string roleexpression) {
            return _e.Eval(roleexpression, new RoleTermSource(this, resolver, identity));
        }

        private bool EvaluateAndList(IRoleResolver resolver,IIdentity identity, IList<string> roles) {
            return roles.All(role => Evaluate(resolver,identity, role));
        }

        private bool EvaluateOrList(IRoleResolver resolver, IIdentity identity, IList<string> roles)
        {
            return roles.Any(role => Evaluate(resolver,identity, role));
        }
    }
}
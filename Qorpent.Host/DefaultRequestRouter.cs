using System.Collections.Generic;
using System.Linq;
using Qorpent.IO.Http;

namespace Qorpent.Host
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultRequestRouter : ServiceBase, IRequestRouter
    {
        readonly IList<RouteRule> _rules = new List<RouteRule>(); 

        public void Register(RouteRule rule)
        {
            _rules.Add(rule);
        }

        public void Remove(RouteRule rule)
        {
            _rules.Remove(rule);
        }

        public IEnumerable<RouteRule> GetRules()
        {
            return _rules.ToArray();
        }

        public void Route(WebContext context)
        {
            foreach (var rule in _rules)
            {
                if (rule.IsMatch(context))
                {
                    rule.Rewrite(context);
                }
            }
        }
    }
}
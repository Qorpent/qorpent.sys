using System.Collections.Generic;
using Qorpent.IO.Http;

namespace Qorpent.Host
{
   

    public interface IRequestRouter
    {
        void Register(RouteRule rule);
        void Remove(RouteRule rule);
        IEnumerable<RouteRule> GetRules(); 
        void Route(WebContext context);
    }
}
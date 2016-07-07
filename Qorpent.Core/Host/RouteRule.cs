using Qorpent.IO.Http;

namespace Qorpent.Host
{
    public abstract class RouteRule
    {
        public abstract bool IsMatch(WebContext context);
        public abstract void Rewrite(WebContext context);
    }
}
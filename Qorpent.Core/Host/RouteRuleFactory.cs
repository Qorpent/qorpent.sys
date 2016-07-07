namespace Qorpent.Host
{
    public static class RouteRuleFactory
    {
        public static RouteRule RewriteUri(string pattern, string replace=null)
        {
            return new RewriteUriRouteRule(pattern, replace);
        }
    }
}
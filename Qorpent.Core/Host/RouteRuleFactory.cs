namespace Qorpent.Host
{
    public static class RouteRuleFactory
    {
        public static RouteRule RewriteUri(string pattern, string replace=null)
        {
            return new RewriteUriRouteRule(pattern, replace);
        }

        public static IHostServer Route(this IHostServer server, string pattern, string replace = null)
        {
            server.Router.Register(RewriteUri(pattern,replace));
            return server;
        }
    }

    
}
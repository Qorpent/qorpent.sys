using System;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.IO.Http;

namespace Qorpent.Host
{
    public class RewriteUriRouteRule : RouteRule
    {
        public RewriteUriRouteRule(string pattern, string replace)
        {
            this.Pattern = pattern;
            this.Replace = replace;
            var _regex = ConstructRegex(this.Pattern);
            this.Regex = new Regex(_regex);
            if (string.IsNullOrWhiteSpace(this.Replace))
            {
                this.Replace = ConstructReplace(this.Pattern);
            }
            
        }

        private string ConstructReplace(string pattern)
        {
            pattern = pattern.Replace("[", "(");
            pattern = pattern.Replace("]", ")?");
            return Regex.Replace(pattern, @"\(?/?:([\d\w_]+)(\)\?)?", "");
        }

        private string ConstructRegex(string pattern)
        {
            pattern = pattern.Replace("[", "(");
            pattern = pattern.Replace("]", ")?");
            return Regex.Replace(pattern, @":([\d\w_]+)", @"(?<$1>[^/&]+?)")+@"(\?[\s\S]*)?$";
        }

        public string Replace { get; set; }

        public string Pattern { get; set; }

        public Regex Regex { get; set; }

        public override bool IsMatch(WebContext context)
        {
            return Regex.IsMatch(context.Request.Uri.ToString());
        }

        public override void Rewrite(WebContext context)
        {
            var url = context.Request.Uri.ToString();
            var oldquery = context.Request.Uri.Query;
            var newurl = Regex.Replace(url, Replace);
            var newuri = new Uri(newurl);
            var newquery = newuri.Query;
            var trgdict = RequestParameters.ParseQuery(newquery);
           
            var match = Regex.Match(url);
            var names = Regex.GetGroupNames();
            foreach (var name in names)
            {
                if (!trgdict.ContainsKey(name))
                {
                    trgdict[name] = match.Groups[name].Value;
                }
            }
            var srcdict = RequestParameters.ParseQuery(oldquery);
            foreach (var pair in srcdict)
            {
                if (!trgdict.ContainsKey(pair.Key))
                {
                    trgdict[pair.Key] = pair.Value;
                }
            }
            var asterindex = newurl.IndexOf("?");
            var path = newurl;
            if (asterindex!=-1)
            {
                path = newurl.Substring(0, asterindex);
            }
            if (path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }
            var query = "?" +
                        string.Join("&",
                            trgdict.Select(_ => Uri.EscapeDataString(_.Key) + "=" + Uri.EscapeDataString(_.Value)));
            newuri = new Uri(path+query);
            context.Request.Uri = newuri;
        }
    }
}
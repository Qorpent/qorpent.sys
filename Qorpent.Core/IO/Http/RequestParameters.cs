using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Http
{
	/// <summary>
	/// Набор параметров запроса
	/// </summary>
	public class RequestParameters
	{
		/// <summary>
		/// 
		/// </summary>
		public RequestParameters()
		{
			Query =new Dictionary<string, string>();
			Form = new Dictionary<string, string>();
			Files =new Dictionary<string, PostFile>();
            
		}
		/// <summary>
		/// 
		/// </summary>
		public string PostData { get; set; }
        public string QueryData { get; set; }

        public object QueryJson { get; set; }
        public object FormJson { get; set; }

	    public object Json {
	        get { return FormJson ?? QueryJson; }
	    }

		/// <summary>
		/// GET параметры
		/// </summary>
		public IDictionary<string, string> Query { get; private set; } 
		/// <summary>
		/// Form -данные
		/// </summary>
		public IDictionary<string, string> Form { get; private set; } 

		/// <summary>
		/// Form -данные
		/// </summary>
		public IDictionary<string, PostFile> Files { get; private set; } 

		/// <summary>
		/// Получить простой строковый параметр по имени
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string Get(string name) {
		    return
		        Get(name, FormJson as IDictionary<string, object>, false)
		        ??
                Get(name, QueryJson as IDictionary<string, object>, false)
                ??
		        Get(name, Form, false)
		        ??
		        Get(name, Query, false)
		        ??
		        Get(name, FormJson as IDictionary<string, object>, true)
		        ??
                 Get(name, QueryJson as IDictionary<string, object>, true)
                ??
		        Get(name, Form, true)
		        ??
		        Get(name, Query, true)
		        ??
		        String.Empty;

		}

	    private string Get<V>(string name, IDictionary<string, V> src, bool ignorecase) {
	        if (null == src) return null;
	        if (src.ContainsKey(name)) return src[name].ToStr() ?? String.Empty;
	        if (ignorecase) {
	            foreach (var key in src.Keys) {
	                if (key.ToLowerInvariant() == name.ToLowerInvariant()) {
	                    return src[key].ToStr() ?? String.Empty;
	                }
	            }
	        }
	        return null;
	    }
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IDictionary<string, string> GetParameters()
		{
			var result = new Dictionary<string, string>();
			foreach (var p in Query)
			{
				result[p.Key] = p.Value;
			}
			foreach (var p in Form)
			{
				result[p.Key] = p.Value;
			}
		    var dictionary   = QueryJson as IDictionary<string, object>;
		    if (dictionary != null) {
		        foreach (var p in dictionary)
		        {
		            result[p.Key] = p.Value.ToStr();
		        }
		    }
            dictionary = FormJson as IDictionary<string, object>;
            if (dictionary != null)
            {
                foreach (var p in dictionary)
                {
                    result[p.Key] = p.Value.ToStr();
                }
            }
		    if (!String.IsNullOrWhiteSpace(PostData))
			{
				result["__postdata"] = PostData;
			}
			return result;
		}

	    public static RequestParameters Create(IHttpRequestDescriptor request) {
	        return Create(new WebContext {Request = request});

	    }
	    /// <summary>
	    /// 
	    /// </summary>
	    /// <returns></returns>
	    public static RequestParameters Create(WebContext context) {
	        var result = new RequestParameters { QueryData = Unescape(context.Uri.Query, true) };
            if (IsJson(result.QueryData))
            {
                result.QueryJson = Experiments.Json.Parse(result.QueryData);
            }
            if (IsDictionary(result.QueryData))
            {
                PrepareDictionaryData(result.Query, result.QueryData, false);
            }
            if (context.IsPost)
            {
                if (context.IsMultipartForm)
                {
                    ReadMultipartForm(result, context);
                }
                else {
                    var str = context.ReadRequestString();
                    str = Unescape(str, true);
                    result.PostData = str;
                    if (IsJson(str))
                    {
                        result.FormJson = Experiments.Json.Parse(result.PostData);
                    }
                    else if (IsDictionary(str))
                    {
                        PrepareDictionaryData(result.Form, str, context.InContentType != null && context.InContentType.Contains("application/x-www-form-urlencoded"));
                    }

                }
            }
            return result;
	    }

	    private static string Unescape(string q, bool pluses) {
	        q = q ?? "";
	        if (q.StartsWith("?")) {
	            q = q.Substring(1);
	        }
	        if (pluses) {
	            q = q.Replace("+", " ");
	        }
	        q = Uri.UnescapeDataString(q);
	        return q.Trim();
	    }

	    private static bool IsDictionary(string queryData) {
	        if (queryData.Contains("=")) return true;
	        return false;
	    }

	    private static bool IsJson(string queryData) {
	        if (queryData.StartsWith("{") && queryData.EndsWith("}")) return true;
	        if (queryData.StartsWith("[") && queryData.EndsWith("]")) return true;
	        if (queryData.StartsWith("\"") && queryData.EndsWith("\"")) return true;
	        if (queryData == "true" || queryData == "false" || queryData == "null") return true;
	        if (Regex.IsMatch(queryData, @"^-?\d+(\.\d+)?$")) return true;
	        return false;
	    }

	    private static void ReadMultipartForm(RequestParameters result,WebContext context)
	    {

	        var mainbufferStream = new MemoryStream();
            context.ReadRequest(mainbufferStream);
	        var mrcontext = new MiltipartReadContext( mainbufferStream.GetBuffer(), context.Request.ContentType,context.Request.Encoding,result);
            mrcontext.Read();
	    }

	    private static void PrepareDictionaryData(IDictionary<string,string> target, string query, bool isqueryString)
	    {
	        if(String.IsNullOrWhiteSpace(query))return;



	        var querydata = query.Split('&');
	        foreach (var queryItem in querydata)
	        {
	            var parts = queryItem.Split('=');
	            var name = parts[0];
	            var value = parts[1];
               
				
	            if (target.ContainsKey(name))
	            {
	                target[name] += "," + value;
	            }
	            else
	            {
	                target[name] = value;
	            }
	        }
	    }
        /// <summary>
        /// Shortcut for singleparameter as default
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
	    public string GetOrFullText(string id) {
	        var dictbased = Get(id);
	        if (string.IsNullOrWhiteSpace(dictbased)) {
	            if (!string.IsNullOrWhiteSpace(PostData)) return PostData;
	            if (!string.IsNullOrWhiteSpace(QueryData)) return QueryData;
	        }
            return string.Empty;
        }
	}
}
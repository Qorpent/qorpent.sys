using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.Experiments;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Http
{
    public interface IWebContextBound {
        void SetWebContext(WebContext context,IScope scope);
    }

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
        public object GetObject (string name) {
            if (null == Json) return Get(name);
            var result = Json.get(name);
            if (null == result && name.ToLowerInvariant() != name) {
                result = Json.get(name.ToLowerInvariant());
            }
            return result;
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

	    public IDictionary<string, object> AsJson() {
	        var result = new Dictionary<string,object>();
            result.extend(FormJson);
            result.extend(QueryJson);
            foreach (var p in Query)
            {
                result[p.Key] = Uri.UnescapeDataString(p.Value);
            }
            foreach (var p in Form)
            {
                result[p.Key] = p.Value;
            }
            return result;
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
	        if (null != context.PreparedParameters) return context.PreparedParameters;
	        lock (context.RequestParametersSync) {
                if (null != context.PreparedParameters) return context.PreparedParameters;

	            var result = new RequestParameters {QueryData = context.Uri.Query.StartsWith("?")? context.Uri.Query.Substring(1):context.Uri.Query};
	            var unescaped = Unescape(result.QueryData, true);
	            if (IsJson(unescaped)) {
	                result.QueryJson = Experiments.Json.Parse(unescaped);
	            }
	            else if (IsDictionary(result.QueryData)) {
	                PrepareDictionaryData(result.Query, result.QueryData, true);
	            }
	            if (context.IsPost) {
	                if (context.IsMultipartForm) {
	                    ReadMultipartForm(result, context);
	                }
	                else {
	                    var str = context.ReadRequestString().Trim();	                  
	                    result.PostData = str;
	                    if (IsJson(str)) {
                            unescaped = Unescape(str, false);
	                        result.FormJson = Experiments.Json.Parse(unescaped);
	                    }
	                    else if (IsDictionary(str)) {
	                        PrepareDictionaryData(result.Form, str,
	                            string.IsNullOrWhiteSpace(context.InContentType) ||
	                            context.InContentType.Contains("application/x-www-form-urlencoded"));
	                    }

	                }
	            }
	            context.PreparedParameters = result;
	            return result;
	        }
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

	    private static void PrepareDictionaryData(IDictionary<string, string> target, string query, bool unescape)
	    {
	        if(String.IsNullOrWhiteSpace(query))return;
	        if (query.StartsWith("?")) {
	            query = query.Substring(1);
	        }


	        var querydata = query.Split('&');
	        foreach (var queryItem in querydata)
	        {
	            var parts = queryItem.Split('=');
	            var name = parts[0];
				var value = string.Join("=", parts.Skip(1).ToArray());
	            if (unescape) {
	                value = Unescape(value, true);
	            }
				
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
	        if (String.IsNullOrWhiteSpace(dictbased)) {
	            if (!String.IsNullOrWhiteSpace(PostData)) return PostData;
	            if (!String.IsNullOrWhiteSpace(QueryData)) return QueryData;
	        }
            return String.Empty;
        }

	    public  IDictionary<string, object> ReadDict(string name) {
	        IDictionary<string, object> _result = null;
	        var __custom = GetObject(name);
	        if (__custom is string) {
	            var _custom = __custom as string;
	            if (!String.IsNullOrWhiteSpace(_custom)) {
	                if (_custom.StartsWith("{")) {
	                    _result = (IDictionary<string, object>) Experiments.Json.Parse(_custom);
	                }
	                else {
	                    _result = TagHelper.Parse(_custom).ToDictionary(_ => _.Key, _ => (object) _.Value);
	                }
	            }
	        }
	        else if (__custom is IDictionary<string, object>) {
	            _result = (IDictionary<string, object>) __custom;
	        }
	        return _result ?? new Dictionary<string, object>();
	    }

        public object[] ReadArray(string name)
        {
            object[] _result = null;
            var __custom = GetObject(name);
            if (__custom is string)
            {
                var _custom = __custom as string;
                if (!String.IsNullOrWhiteSpace(_custom))
                {
                    _result = _custom.SmartSplit(false, true, ',', '|', ' ').Select(_=>(object)_).ToArray();
                }
            }
            else if (__custom is object[])
            {
                _result = (object[])__custom;
            }
            return _result ?? new object[]{};
        }

	    public static IDictionary<string,string> ParseQuery(string newquery)
	    {
          
            var result = new Dictionary<string,string>();
	        if (!string.IsNullOrWhiteSpace(newquery))
	        {
	            PrepareDictionaryData(result, newquery, true);
	        }
	        return result;
	    }
	}
}
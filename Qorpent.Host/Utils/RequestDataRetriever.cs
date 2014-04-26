using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Qorpent.Host.Utils
{
	/// <summary>
	/// Утилиты для работы с HTTP Listener
	/// </summary>
	public class RequestDataRetriever
	{
		private readonly Encoding _contentEncoding;
		private readonly long _contentLength;
		private readonly Uri _url;
		private readonly Stream _inputStream;
		private readonly string _httpMethod;
		private readonly string _contentType;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		public RequestDataRetriever(HttpListenerRequest request):this(request.ContentType,request.ContentEncoding,request.ContentLength64,request.Url,request.InputStream, request.HttpMethod)
		{
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="contentType"></param>
		/// <param name="contentEncoding"></param>
		/// <param name="contentLength64"></param>
		/// <param name="url"></param>
		/// <param name="inputStream"></param>
		/// <param name="httpMethod"></param>
		public RequestDataRetriever(string contentType, Encoding contentEncoding, long contentLength64, Uri url, Stream inputStream, string httpMethod)
		{
			_contentType = contentType;
			_contentEncoding = contentEncoding;
			_contentLength = contentLength64;
			_url = url;
			_inputStream = inputStream;
			_httpMethod = httpMethod;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public RequestParameterSet GetRequestData()
		{
			var result = new RequestParameterSet();
			PrepareDictionaryData(result.Query, _url.Query,true);
			if (_httpMethod.ToUpper() == "POST")
			{
				if (null!=_contentType && _contentType.Contains("multipart/form-data"))
				{
					ReadMultipartForm(result);
				}
				else
				{
					var buffer = new byte[_contentLength];
					_inputStream.Read(buffer, 0, (int)_contentLength);
					var str = _contentEncoding.GetString(buffer);
					result.PostData = str;
					try
					{
						PrepareDictionaryData(result.Form, str,false);
					}
					catch
					{
						
					}
				}
			}
			return result;
		}

		private void ReadMultipartForm(RequestParameterSet result)
		{

			var mainbufferStream = new MemoryStream();
			_inputStream.CopyTo(mainbufferStream);
			var context = new MiltipartReadContext( mainbufferStream.GetBuffer(), _contentType,_contentEncoding,result);
			context.Read();
		}

		private void PrepareDictionaryData(IDictionary<string,string> target, string query, bool isqueryString)
		{
			if(string.IsNullOrWhiteSpace(query))return;
			if (query.StartsWith("?"))
			{
				query = query.Substring(1);
			}
            if (string.IsNullOrWhiteSpace(query)) return;
			var querydata = query.Split('&');
			foreach (var queryItem in querydata)
			{
				var parts = queryItem.Split('=');
				var name = Uri.UnescapeDataString(parts[0]);
				var value = Uri.UnescapeDataString(parts[1]);
				if (isqueryString){
					value = value.Replace("+", " ");
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
	}
}
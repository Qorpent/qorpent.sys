using System;
using System.Globalization;
using System.Net;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Utils{
	/// <summary>
	/// Расширения для удобной работы с HttpListener
	/// </summary>
	public static class HttpListenerContextExtensions{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static DateTime GetIfModifiedSince(this HttpListenerContext context){
			return GetIfModifiedSince(context.Request);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static DateTime GetIfModifiedSince(this HttpListenerRequest request){
				var header = request.Headers["If-Modified-Since"];
				if (header.IsNotEmpty()){
					var result =
						DateTime.ParseExact(header, "R", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToLocalTime();
					return new DateTime(result.Year, result.Month, result.Day,
					                    result.Hour, result.Minute, result.Second);
				}
				else{
					return new DateTime(1900,1,1);
				}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static string GetIfNoneMatch(this HttpListenerContext context)
		{
			return GetIfNoneMatch(context.Request);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public static string GetIfNoneMatch(this HttpListenerRequest request)
		{
			var header = request.Headers["If-None-Match"];
			return header ?? "";
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="etag"></param>
		/// <returns></returns>
		public static void SetETag(this HttpListenerContext context,string etag)
		{
			SetETag(context.Response,etag);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		/// <param name="etag"></param>
		/// <returns></returns>
		public static void SetETag(this HttpListenerResponse response, string etag)
		{
			response.Headers["Etag"] = etag ?? "";
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public static DateTime SetLastModified(this HttpListenerContext context, DateTime time)
		{
			return SetLastModified(context.Response, time);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="response"></param>
		/// <param name="time"></param>
		/// <returns></returns>
		public static DateTime SetLastModified(this HttpListenerResponse response, DateTime time)
		{
			var v = time;
			if (v.Year < 1900)
			{
				v = new DateTime(1900, 1, 1);
			}


			v = new DateTime(v.Year, v.Month, v.Day, v.Hour, v.Minute, v.Second);
		response.Headers["Last-Modified"] = v.ToUniversalTime().ToString("R",
																									  CultureInfo.InvariantCulture);
			return v;

		}
	}
}
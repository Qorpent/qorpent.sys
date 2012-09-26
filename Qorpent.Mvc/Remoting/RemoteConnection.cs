﻿#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : RemoteConnection.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Remoting {
	/// <summary>
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, Name = "remote.mvc.connection")]
	public class RemoteConnection : ConnectionBase {
		/// <summary>
		/// 	Generates HTTP web request ro query
		/// </summary>
		/// <param name="query"> </param>
		/// <returns> </returns>
		protected WebRequest CreateWebRequest(MvcQuery query) {
			var resulturl = url + "/" + query.Action.Replace(".", "/") + "." + query.RenderName.ToLower() + ".qweb";
			if (null != query.Parameters) {
				var sb = new StringBuilder();
				sb.Append("?");
				if (query.Parameters is IDictionary<string, object>) {
					foreach (var o in ((IDictionary<string, object>) query.Parameters)) {
						sb.Append("&");
						sb.Append(Uri.EscapeUriString(o.Key));
						sb.Append("=");
						sb.Append(Uri.EscapeUriString(o.Value.ToStr()));
					}
				}
				else if (query.Parameters is IDictionary<string, string>) {
					foreach (var o in ((IDictionary<string, string>) query.Parameters)) {
						sb.Append("&");
						sb.Append(Uri.EscapeUriString(o.Key));
						sb.Append("=");
						sb.Append(Uri.EscapeUriString(o.Value));
					}
				}
				else {
					var props = query.Parameters.GetType().GetProperties();
					foreach (var p in props) {
						sb.Append("&");
						sb.Append(Uri.EscapeUriString(p.Name));
						sb.Append("=");
						sb.Append(Uri.EscapeUriString(p.GetValue(query.Parameters, null).ToStr()));
					}
				}
				resulturl += sb.ToString();
			}
			var result = WebRequest.Create(resulturl);
			foreach (var header in query.Headers) {
				result.Headers.Add(header.Key, header.Value);
			}
			return result;
		}

		/// <summary>
		/// 	implement for real calling to mvc
		/// </summary>
		/// <param name="query"> </param>
		/// <returns> </returns>
		protected override MvcResult internalCall(MvcQuery query) {
			var u = url;
			if (!u.EndsWith("/")) {
				u += "/";
			}
			var req = CreateWebRequest(query);
			if (null != credentials) {
				req.Credentials = credentials;
			}
			else {
				req.UseDefaultCredentials = true;
			}
			var result = new MvcResult();


			try {
				var resp = (HttpWebResponse) req.GetResponse();
				result.LastModified = resp.LastModified;
				var header = resp.Headers["ETag"];
				if (header != null) {
					result.ETag = header;
				}
				result.Status = (int) resp.StatusCode;
				try {
					using (var r = resp.GetResponseStream()) {
						result.Content = new StreamReader(r).ReadToEnd();
					}
				}
				catch (Exception ex) {
					result.Error = ex;
				}
			}
			catch (WebException ex) {
				var errorResponse = (HttpWebResponse) ex.Response;
				result.Status = (int) errorResponse.StatusCode;
				try {
					using (var resst = errorResponse.GetResponseStream()) {
						var sr = new StreamReader(resst);
						result.ErrorString = sr.ReadToEnd();
					}
				}
				catch (Exception ex2) {
					result.Error = ex2;
				}
			}
			catch (Exception ex) {
				result.Error = ex;
			}
			return result;
		}
	}
}
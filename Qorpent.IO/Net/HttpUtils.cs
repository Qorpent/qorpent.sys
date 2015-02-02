// Copyright 2007-2014  Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Created : 2014-09-02

using System;
using System.Collections.Generic;
using System.Net;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Net{
	/// <summary>
	///     Утилиты парсинга HTTP
	/// </summary>
	public static class HttpUtils{
		/// <summary>
		///     Преобразует хидер куки в набор объектов куки
		/// </summary>
		/// <param name="headerValue"></param>
		/// <returns></returns>
		public static IEnumerable<Cookie> ParseCookies(string headerValue){
			Cookie current = null;
			foreach (var c in headerValue.SmartSplit(false, true, ';')){
				if (null == current){
					current = new Cookie();
				}
				if (c == ","){
					yield return current;
					current = null;
					continue;
				}
				var part = c.SmartSplit(false, true, '=');
				if (part[0].ToLowerInvariant() == "expires"){
					current.Expires = DateTime.Parse(part[1]);
				}
				else if (part[0].ToLowerInvariant() == "path"){
					current.Path = part[1];
				}
				else if (part[0].ToLowerInvariant() == "domain") {
				    current.Domain = part[1];
				}
				else if(part[0].ToLowerInvariant()=="httponly") {
				    current.HttpOnly = true;
				} else if (part[0].ToLowerInvariant() == "secure") {
				    current.Secure = true;
				}
				else{
					current.Name = part[0];
				    if (part.Count == 1) {
				        current.Value = "";
				    }
				    else {
				        current.Value = part[1];
				    }
				}
			}
			yield return current;
		}
		/// <summary>
		/// Проверяет соответствие куки адресу
		/// </summary>
		/// <param name="cookie"></param>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static bool IsCookieMatch(Cookie cookie, Uri uri){
			if (cookie.Expired) return false;
			var path = uri.AbsolutePath;
			var host = uri.Host;
			if (!String.IsNullOrWhiteSpace(cookie.Domain)){
				if (cookie.Domain.StartsWith(".")){
					if (!host.EndsWith(cookie.Domain)) return false;
				}
				else{
					if (cookie.Domain != host) return false;
				}
			}
			if (!String.IsNullOrWhiteSpace(cookie.Path) && cookie.Path != "/"){
				if (!path.StartsWith(cookie.Path)) return false;
			}
			return true;
		}
	}
}
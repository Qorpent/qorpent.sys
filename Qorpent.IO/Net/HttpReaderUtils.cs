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
	public static class HttpReaderUtils{
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
				else if (part[0].ToLowerInvariant() == "domain"){
					current.Domain = part[1];
				}
				else{
					current.Name = part[0];
					current.Value = part[1];
				}
			}
			yield return current;
		}
	}
}
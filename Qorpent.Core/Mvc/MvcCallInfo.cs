#region LICENSE

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
// Original file : MvcCallInfo.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Encapsulate info about MVC call
	/// </summary>
	public class MvcCallInfo {
		/// <summary>
		/// 	Calling Url
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// 	Action Name
		/// </summary>
		public string ActionName { get; set; }

		/// <summary>
		/// 	Render Name
		/// </summary>
		public string RenderName { get; set; }

		/// <summary>
		/// </summary>
		public IDictionary<string, string> Parameters { get; set; }

		/// <summary>
		/// 	Возвращает объект <see cref="T:System.String" />, который представляет текущий объект <see cref="T:System.Object" />.
		/// </summary>
		/// <returns> Объект <see cref="T:System.String" /> , представляющий текущий объект <see cref="T:System.Object" /> . </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString() {
			return ActionName + "." + RenderName + ":" + Url;
		}


		/// <summary>
		/// 	Calculates action name from given uri
		/// </summary>
		/// <param name="uri"> </param>
		/// <returns> </returns>
		public static string GetActionName(Uri uri) {
			if (null == uri) {
				return "qweb.none";
			}
			var result =
				Regex.Match(uri.ToString(), @"https?\://[^/]+/[^/]+/((\w+/)?\w+?)(\.\w+)?\.qweb", RegexOptions.Compiled).Groups[1].
					Value;
			if (string.IsNullOrEmpty(result)) {
				return "qweb.none";
			}
			return result.Replace("/", ".").ToLowerInvariant();
		}

		/// <summary>
		/// 	Calculates render name from given uri
		/// </summary>
		/// <param name="uri"> </param>
		/// <returns> </returns>
		public static string GetRenderName(Uri uri) {
			if (null == uri) {
				return "xml";
			}
			var result = Regex.Match(uri.ToString(), @"/\w+\.(\w+)\.qweb", RegexOptions.Compiled).Groups[1].Value;
			if (string.IsNullOrEmpty(result)) {
				return "xml";
			}
			return result.ToLowerInvariant();
		}
	}
}
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
// Original file : DslCSharpXsltExtensions.cs
// Project: Qorpent.Dsl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Text.RegularExpressions;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	functions, usefull for C# generation from XSLT
	/// </summary>
	public class DslCSharpXsltExtensions {
		/// <summary>
		/// 	safely converts given string to cs literal (@ prefix if it's alpha-alphanumeric) (with @_ prefix and dots replaced in other cases)
		/// </summary>
		/// <param name="src"> </param>
		/// <returns> </returns>
		/// <exception cref="QorpentException"></exception>
		public string literal(string src) {
			var res = "@" + src;
			if (Regex.IsMatch(res, @"^@[\w_][\w\d_]*$")) {
				return res;
			}
			if (!Regex.IsMatch(src, @"^@[\w_]")) {
				res = "@_" + src;
			}
			res = res.Replace(".", "_");
			if (Regex.IsMatch(res, @"^@[\w_][\w\d_]*$")) {
				return res;
			}
			throw new QorpentException("cannot convert given string '" + src + "' to literal");
		}

		/// <summary>
		/// 	returns usual escaped CS str
		/// </summary>
		/// <param name="src"> </param>
		/// <returns> </returns>
		public string str(string src) {
			var res = src;
			res = res.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t").Replace("\"", "\\\"");
			return "\"" + res + "\"";
		}

		/// <summary>
		/// 	returns usual escaped CS verbatim str
		/// </summary>
		/// <param name="src"> </param>
		/// <returns> </returns>
		public string vstr(string src) {
			var res = src;
			res = res.Replace("\"", "\"\"");
			return "@\"" + res + "\"";
		}
	}
}
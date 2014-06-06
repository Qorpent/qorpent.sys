#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
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
// PROJECT ORIGIN: Qorpent.Utils/StringHelper.cs
#endregion
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
	/// <summary>
	/// 	Contains main string-relative extensions such IsEmpty, IsNotEmpty and so on...
	/// </summary>
	public class StringHelper {
		/// <summary>
		/// 	simple wrapper over IsNullOrWhiteSpace with compliance to 3.5
		/// </summary>
		/// <param name="stringToTest"> string to test if it's empty </param>
		/// <returns> </returns>
		public bool IsEmpty(string stringToTest) {
#if !SQL2008
			return string.IsNullOrWhiteSpace(stringToTest);
#else
			if (string.IsNullOrEmpty(stringToTest)) return true;
			return string.IsNullOrEmpty(stringToTest.Trim());
#endif
		}

		/// <summary>
		/// 	simple negative wrapper over IsNullOrWhiteSpace with compliance to 3.5
		/// </summary>
		/// <param name="stringToTest"> string to test if it's not empty </param>
		/// <returns> </returns>
		public bool IsNotEmpty(string stringToTest) {
#if !SQL2008
			return !string.IsNullOrWhiteSpace(stringToTest);
#else
			return !IsEmpty(stringToTest);
#endif
		}


		/// <summary>
		/// 	Concatenates any given IEnumerable to joined string, null-ignorance
		/// </summary>
		/// <param name="objects"> any set of objects of any type </param>
		/// <param name="delimiter"> delimiter between strings </param>
		/// <param name="empties"> true - empties included in result </param>
		/// <param name="nullstring"> null replacer (null by default) </param>
		/// <param name="useTrailDelimiters"> if true - trail delimiters will be generated </param>
		/// <returns> </returns>
		public string ConcatString(IEnumerable objects, string delimiter, bool empties = true,
		                           string nullstring = null,
		                           bool useTrailDelimiters = false) {
			if (null == objects) {
				return string.Empty;
			}
			if (null == delimiter) {
				delimiter = string.Empty;
			}
			var result = new StringBuilder();
			if (useTrailDelimiters) {
				result.Append(delimiter);
			}
			var first = true;
			foreach (var obj in objects) {
				var val = obj.ToStr();
				if (val.IsEmpty() && !empties) {
					continue;
				}
				if (null == obj && null != nullstring) {
					val = nullstring;
				}
				if (!first) {
					result.Append(delimiter);
				}
				if (first) {
					first = false;
				}
				result.Append(val);
			}
			if (useTrailDelimiters) {
				result.Append(delimiter);
			}
			return result.ToString();
		}

		///<summary>
		///	Converts string into list
		///</summary>
		///<param name="str"> source string </param>
		///<param name="empty"> true - include empties (false by default) </param>
		///<param name="trim"> true - trim before adding to list (true by default) </param>
		///<param name="splitters"> array of delimiter chars </param>
		///<returns> </returns>
		public IList<string> SmartSplit(string str, bool empty = false, bool trim = true, params char[] splitters) {
			if (null == splitters || 0 == splitters.Length) {
				splitters = QorpentConst.DefaultSplitters;
			}
			if (IsEmpty(str)) {
				return new List<string>();
			}
			var res = (IEnumerable<string>) str.Split(splitters);
			if (!empty) {
				res = res.Where(IsNotEmpty);
			}
			if (trim) {
				res = res.Select(s => s.Trim());
			}
			return res.ToList();
		}
	}
}
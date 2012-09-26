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
// Original file : StringExtensions.cs
// Project: Qorpent.Utils
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections;
using System.Collections.Generic;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// 	C# extensions for StringHelper
	/// </summary>
	public static class StringExtensions {
		private static StringHelper _helper = new StringHelper();

		/// <summary>
		/// 	Default domain-wide stringhelper
		/// </summary>
		public static StringHelper DefaultHelper {
			get { return _helper; }
			set { _helper = value; }
		}

		/// <summary>
		/// 	Extension-like wrapper over string.IsNullOrWhitespace
		/// </summary>
		/// <param name="stringToTest"> string to test if it's empty </param>
		/// <returns> </returns>
		public static bool IsEmpty(this string stringToTest) {
			return _helper.IsEmpty(stringToTest);
		}

		/// <summary>
		/// 	Extension-like wrapper over negation string.IsNullOrWhitespace
		/// </summary>
		/// <param name="stringToTest"> string to test if it's empty </param>
		/// <returns> </returns>
		public static bool IsNotEmpty(this string stringToTest) {
			return _helper.IsNotEmpty(stringToTest);
		}

		/// <summary>
		/// 	Concatenates any given IEnumerable to joined string, null-ignorance
		/// </summary>
		/// <param name="objects"> any set of objects of any type </param>
		/// <param name="delimiter"> delimiter between strings </param>
		/// <param name="nullstring"> string representation for null items </param>
		/// <param name="useTrailDelimiters"> if true - trail delimiters will be generated </param>
		/// <param name="empties"> true - empty strings are included in list </param>
		/// <returns> </returns>
		public static string ConcatString(this IEnumerable objects, string delimiter = "", bool empties = true,
		                                  string nullstring = null, bool useTrailDelimiters = false) {
			return _helper.ConcatString(objects, delimiter, empties, nullstring, useTrailDelimiters);
		}

		///<summary>
		///	Converts string into list
		///</summary>
		///<param name="str"> source string </param>
		///<param name="empty"> true - include empties (false by default) </param>
		///<param name="trim"> true - trim before adding to list (true by default) </param>
		///<param name="splitters"> array of delimiter chars </param>
		///<returns> </returns>
		public static IList<string> SmartSplit(this string str, bool empty = false, bool trim = true, params char[] splitters) {
			return _helper.SmartSplit(str, empty, trim, splitters);
		}
	}
}
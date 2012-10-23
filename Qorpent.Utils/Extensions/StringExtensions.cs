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

using System;
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
		/// \r\n and \r приводятся к \n
		/// </summary>
		/// <param name="stringToFixCrLf"></param>
		/// <returns></returns>
		public static string LfOnly(this string stringToFixCrLf) {
			var result = stringToFixCrLf.Replace("\r", "\n");
			result = result.Replace("\n\n", "\n");
			return result;
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


		/// <summary>
		/// Для строк формата SCESCE (S!=E) или SCSCS (S==E) возвращает True если элемент есть в списке
		/// </summary>
		/// <param name="str"></param>
		/// <param name="value"> </param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="ignorecase"> </param>
		/// <returns></returns>
		public static bool ListContains(this string str, string value, string start = "/", string end= "/", bool ignorecase= true) {
			if(str.IsEmpty()) return false;
			if(value.IsEmpty()) return false;
			if(start.IsEmpty())throw new QorpentException("start cannot be empty");
			if (end.IsEmpty()) throw new QorpentException("end cannot be empty");
			if (value.Contains(end)||value.Contains(start)) throw new QorpentException(string.Format("cannot apply ListContains if find contains start or end: {0},{1},{2} ",value,start,end));
			var realsearch = start + value + end;
			var wheresearch = str;
			if(ignorecase) {
				realsearch = realsearch.ToUpperInvariant();
				wheresearch = wheresearch.ToUpperInvariant();
			}
			return wheresearch.Contains(realsearch);
		}

		/// <summary>
		/// Для строк формата SCESCE (S!=E) или SCSCS (S==E) дописывает элемент списка при отсутствии
		/// </summary>
		/// <param name="str"></param>
		/// <param name="value"> </param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="ignorecase"> </param>
		/// <returns></returns>
		public static string ListAppend(this string str, string value, string start = "/", string end = "/", bool ignorecase = true)
		{
			if (start.IsEmpty()) throw new QorpentException("start cannot be empty");
			if (end.IsEmpty()) throw new QorpentException("end cannot be empty");
			if (value.Contains(end) || value.Contains(start)) throw new QorpentException(string.Format("cannot apply ListAppend if find contains start or end: {0},{1},{2} ", value, start, end));
			if (value.IsEmpty()||ListContains(str,value,start,end,ignorecase)) return str;

			var appendablevalue = start + value + end;
			
			if (str.IsEmpty()) return appendablevalue;

			var newvalue = str;
			
			newvalue += appendablevalue;


			newvalue = newvalue.Replace(start + start, start);
			newvalue = newvalue.Replace(end + end, end);

			return newvalue;
		}


		/// <summary>
		/// Для строк формата SCESCE (S!=E) или SCSCS (S==E) убирает элемент из списка при наличии
		/// </summary>
		/// <param name="str"></param>
		/// <param name="value"> </param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="ignorecase"> </param>
		/// <returns></returns>
		public static string ListRemove(this string str, string value, string start = "/", string end = "/", bool ignorecase = true)
		{
			if (start.IsEmpty()) throw new QorpentException("start cannot be empty");
			if (end.IsEmpty()) throw new QorpentException("end cannot be empty");
			if (value.Contains(end) || value.Contains(start)) throw new QorpentException(string.Format("cannot apply ListRemove if find contains start or end: {0},{1},{2} ", value, start, end));
			if (value.IsEmpty() || str.IsEmpty() || !ListContains(str, value, start, end, ignorecase)) return str;

			var removevalue = start + value + end;
			var newvalue = str;
			var index = newvalue.IndexOf(removevalue,
			                             ignorecase
				                             ? StringComparison.InvariantCultureIgnoreCase
				                             : StringComparison.InvariantCulture);
			while (index!=-1) {
				newvalue = newvalue.Substring(0, index ) + start + end + newvalue.Substring(index + removevalue.Length);
				index = newvalue.IndexOf(removevalue,
										 ignorecase
											 ? StringComparison.InvariantCultureIgnoreCase
											 : StringComparison.InvariantCulture);
			}

			newvalue = newvalue.Replace(end + start + end, end);
			newvalue = newvalue.Replace(start + start, start);
			newvalue = newvalue.Replace(end + end, end);
			if(newvalue==start+end||newvalue==start||newvalue==end) {
				newvalue = "";
			}
			return newvalue;
		}

	}
}
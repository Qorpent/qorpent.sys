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
// PROJECT ORIGIN: Qorpent.Utils/StringExtensions.cs
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
		/// Замещает символы, не совместимые с именем файла
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToSafeFileName(this string str){
			var sb = new StringBuilder();
			foreach (var c in str){
				if (c == '/' || c == '\\' || c == ':' || c == '?' || c == '*' || c == '|' ||c=='<'||c=='>'){
					sb.Append('_');
				}else if (c == '"'){
					sb.Append('\'');
				}
				else{
					sb.Append(c);
				}
			}
			return sb.ToString();
		}
		/// <summary>
		/// Замещает символы, не совместимые с путем
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToSafePath(this string str){
			var sb = new StringBuilder();
			int cnt = 0;
			bool wasletter = false;
			foreach (var c in str)
			{
				if (c == '?' || c == '*' || c == '|' || c == '<' || c == '>')
				{
					sb.Append('_');
				}else if (c == ':'){
					if (wasletter && cnt == 1){
						sb.Append(':');
					}
					else{
						sb.Append('_');
					}
				}
				else if (c == '"')
				{
					sb.Append('\'');
				}
				else
				{
					sb.Append(c);
					wasletter = true;
				}
				cnt++;
			}
			return sb.ToString().Replace("//","/");
		}

        /// <summary>
        ///     Проверяет, что исходная строка имеет хотя бы одно вхождение StartsWith
        /// </summary>
        /// <param name="source"></param>
        /// <param name="strings"></param>
        /// <returns></returns>
	    public static bool StartsWith(this string source, IEnumerable<string> strings) {
	        return strings.Any(source.StartsWith);
	    }
	    /// <summary>
	    /// Конвертирует исходный текст в сущности XML
	    /// </summary>
	    /// <param name="src"></param>
	    /// <param name="escapeQuots"></param>
	    /// <param name="xws"></param>
	    /// <param name="altprefix"></param>
	    /// <param name="cws"></param>
	    /// <returns></returns>
	    public static string GetUnicodeSafeXmlString(this string src, bool escapeQuots = false, bool cws = false, bool xws=false, string altprefix = null) {
			var builder = new StringBuilder();
			foreach (char c in src)
			{
				
				if (c > 127)
				{
					builder.Append(altprefix ?? "&#");
					builder.Append((int)c);
					builder.Append(";");
				}
				else
				{
					if (escapeQuots)
					{
						if (c == '"') {
							builder.Append("&quot;");
						}else if (c == '\'') {
							builder.Append("&apos;");
						}
						else if (c == '&')
						{
							builder.Append("&amp;");
						}
						else if (c == '<')
						{
							builder.Append("&lt;");
						}
						else if (c == '>') {
							builder.Append("&gt;");
						}
                        else if ((cws||xws) &&(c == '\r' || c=='\n')) {
                            if (cws) {
                                if (c == '\r') builder.Append("\\\r");
                                else builder.Append("\\\n");
                            }
                            else {
                                if (c == '\r') builder.Append("&#0D;");
                                else builder.Append("&#0A;");
                            }
                        }
						else {
							builder.Append(c);
						}
					}
					else {

						builder.Append(c);
					}
				}
			}
			return builder.ToString();
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
			result = result.Replace("\n\n", "\n");
			result = result.Replace("\n\n", "\n");
			result = result.Replace("&#xD;", "&#xA;");
			result = result.Replace("&#xA;&#xA;", "&#xA;");
			result = result.Replace("&#xA;&#xA;", "&#xA;");
			result = result.Replace("&#xA;&#xA;", "&#xA;");
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

		/// <summary>
		/// Выполняет Regex.IsMatch относительно переданной строки, null-safe
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <param name="compiled"></param>
		/// <returns></returns>
		public static bool Like (this string str, string pattern, bool compiled = false) {
			if(str.IsEmpty()) return false;
			if(pattern.IsEmpty()) return false;
			return Regex.IsMatch(str, pattern, compiled ? RegexOptions.Compiled : RegexOptions.None);
		}


		/// <summary>
		/// Простой шоткат для Regex.Replace в виде расширения
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <param name="substitute"></param>
		/// <returns></returns>
		public static string RegexReplace(this string str, string pattern, string substitute)
		{
			return RegexReplace(str, pattern, substitute,
						   RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		}

		/// <summary>
		/// Простой шоткат для Regex.Replace в виде расширения
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <param name="substitute"></param>
		/// <param name="options"> </param>
		/// <returns></returns>
		public static string RegexReplace(this string str, string pattern, string substitute, RegexOptions options)
		{
			if (null == str || null == pattern || null == substitute)
			{
				return String.Empty;
			}
			return Regex.Replace(str, pattern, substitute, options);
		}

		/// <summary>
		/// Простой шоткат для Regex.Replace в виде расширения
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <param name="evaluator"> </param>
		/// <returns></returns>
		public static string RegexReplace(this string str, string pattern, MatchEvaluator evaluator)
		{
			return RegexReplace(str, pattern, evaluator,
						   RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		}

		/// <summary>
		/// Простой шоткат для Regex.Replace в виде расширения
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <param name="evaluator"> </param>
		/// <param name="options"> </param>
		/// <returns></returns>
		public static string RegexReplace(this string str, string pattern, MatchEvaluator evaluator, RegexOptions options)
		{
			if (null == str || null == pattern || null == evaluator)
			{
				return String.Empty;
			}
			return Regex.Replace(str, pattern, evaluator, options);
		}

		/// <summary>
		/// Оболочка над Regex.Match с возвратом найденного значения, игнорируется регистр и пробельные символы
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public static string RegexFind(this string str, string pattern)
		{
			return RegexFind(str, pattern,
						RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		}

		/// <summary>
		/// Оболочка над Regex.Match с возвратом найденного значения, игнорируется регистр и пробельные символы
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <param name="options"> </param>
		/// <returns></returns>
		public static string RegexFind(this string str, string pattern, RegexOptions options)
		{
			if (null == str || null == pattern)
			{
				return String.Empty;
			}
			var m = Regex.Match(str, pattern, options);
			if (m.Success)
			{
				return m.Value;
			}
			return String.Empty;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <param name="groupname"></param>
		/// <returns></returns>
		public static string RegexFind(this string str, string pattern, string groupname)
		{
			return RegexFind(str, pattern, groupname, RegexOptions.Compiled);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <param name="groupname"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static string RegexFind(this string str, string pattern, string groupname, RegexOptions options)
		{
			if (null == str || null == pattern)
			{
				return String.Empty;
			}
			var m = Regex.Match(str, pattern, options);
			if (m.Success)
			{
				return m.Groups[groupname].Value;
			}
			return String.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <param name="groupidx"></param>
		/// <returns></returns>
		public static string RegexFind(this string str, string pattern, int groupidx)
		{
			return RegexFind(str, pattern, groupidx, RegexOptions.Compiled);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <param name="groupidx"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static string RegexFind(this string str, string pattern, int groupidx, RegexOptions options)
		{
			if (null == str || null == pattern)
			{
				return String.Empty;
			}
			var m = Regex.Match(str, pattern, options);
			if (m.Success)
			{
				return m.Groups[groupidx].Value;
			}
			return String.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public static IEnumerable<Match> RegexFindAll(this string str, string pattern)
		{
			return RegexFindAll(str, pattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <param name="pattern"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static IEnumerable<Match> RegexFindAll(this string str, string pattern, RegexOptions options)
		{
			if (str.IsEmpty())
			{
				yield break;
			}
			if (pattern.IsEmpty())
			{
				yield break;
			}
			foreach (Match m in Regex.Matches(str, pattern, options))
			{
				yield return m;
			}
		}

		/// <summary>
		/// Выполняет подстановки в строке (без учета регистра)
		/// </summary>
		/// <param name="str">Исходная строка</param>
		/// <param name="from">Массив заменяемых строк</param>
		/// <param name="to">Массив строк - замен, null означает пропуск</param>
		/// <returns>Строка с выполненным</returns>
		public static string Translate(this string str, IEnumerable<string> from, IEnumerable<string> to)
		{
			return Translate(str, from, to, true);
		}

		/// <summary>
		/// Выполняет подстановки в строке
		/// </summary>
		/// <param name="str">Исходная строка</param>
		/// <param name="from">Массив заменяемых строк</param>
		/// <param name="to">Массив строк - замен, null означает пропуск</param>
		/// <param name="ignoreCase"> </param>
		/// <returns>Строка с выполненным</returns>
		public static string Translate(this string str, IEnumerable<string> from, IEnumerable<string> to,
									   bool ignoreCase)
		{
			if (String.IsNullOrEmpty(str))
			{
				return String.Empty;
			}
			if (null == from || null == to)
			{
				return str;
			}
			var f = from.ToList();
			var t = to.ToList();


			var result = str;

			for (var i = 0; i < Math.Min(f.Count(), t.Count()); i++)
			{
				var _f = f[i];
				if (String.IsNullOrEmpty(_f))
				{
					continue;
				}
				var _t = t[i];
				if (null == _t)
				{
					continue;
				}
				if (ignoreCase)
				{
					result = result.RegexReplace(_f, _t);
				}
				else
				{
					result = result.RegexReplace(_f, _t, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
				}
			}

			return result;
		}

	}
}
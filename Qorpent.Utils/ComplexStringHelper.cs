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
// Original file : ComplexStringHelper.cs
// Project: Qorpent.Utils
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
	/// <summary>
	/// 	Universal parser for lists and dictionaries
	/// </summary>
	public sealed class ComplexStringHelper {


		private const string TagLikePrototype = @"{0}(?<name>{4})({1}(?<value>[\s\S]*?))?{2}";
		private const string CsLikePrototype = @"(^|{3})(?<name>{4})({1}(?<value>[\s\S]+?))?({3}|$)";

		private Regex ParseRegex {
			get {
				if (null == _parseregex) {
					string regexstr = ItemDelimiter.IsNotEmpty() ? CsLikePrototype : TagLikePrototype;
					regexstr = string.Format(regexstr, ItemPrefix, ValueDelimiter, ItemSuffix, ItemDelimiter, @"[\s\S]+?");
					_parseregex = new Regex(regexstr, RegexOptions.Compiled);
				}
				return _parseregex;
			}
		}

		/// <summary>
		/// 	Префикс элемента строки
		/// </summary>
		public string ItemPrefix { get; set; }

		/// <summary>
		/// 	Суффкс элемента
		/// </summary>
		public string ItemSuffix { get; set; }

		/// <summary>
		/// 	Разделитель элементов
		/// </summary>
		public string ItemDelimiter { get; set; }

		/// <summary>
		/// 	Разделитель имя-значение
		/// </summary>
		public string ValueDelimiter { get; set; }

		/// <summary>
		/// 	Значение при отсутствии элемента
		/// </summary>
		public string NotExistedValue { get; set; }

		/// <summary>
		/// 	Значение при пустом элементе
		/// </summary>
		public string EmptyValue { get; set; }

		/// <summary>
		/// 	Строка - экран для  разделителя
		/// </summary>
		public string ItemDelimiterSubstitution { get; set; }

		/// <summary>
		/// 	Строка - экран для префикса разделителя
		/// </summary>
		public string ItemPrefixSubstitution { get; set; }

		/// <summary>
		/// 	Строка - экран для суффикса разделителя
		/// </summary>
		public string ItemSuffixSubstitution { get; set; }

		/// <summary>
		/// 	Строка - экран разделителя имя-значение
		/// </summary>
		public string ValueDelimiterSubstitution { get; set; }

		

		private string Escape(string str) {
			if (str.IsEmpty()) {
				return "";
			}
			var preformatedString = str;
			if(!string.IsNullOrEmpty(ItemDelimiter)) {
				preformatedString = preformatedString.Replace(ItemDelimiter, ItemDelimiterSubstitution);
			}
			if (!string.IsNullOrEmpty(ItemPrefix))
			{
				preformatedString = preformatedString.Replace(ItemPrefix, ItemPrefixSubstitution);
			}
			if (!string.IsNullOrEmpty(ItemSuffix))
			{
				preformatedString = preformatedString.Replace(ItemSuffix, ItemSuffixSubstitution);
			}
			if (!string.IsNullOrEmpty(ValueDelimiter))
			{
				preformatedString = preformatedString.Replace(ValueDelimiter, ValueDelimiterSubstitution);
			}
			return preformatedString;
		}

		private string UnEscape(string str) {
			if (str.IsEmpty()) {
				return "";
			}
			var preformatedString = str;
			if(ItemDelimiterSubstitution.IsNotEmpty()) {
				preformatedString = preformatedString.Replace(ItemDelimiterSubstitution, ItemDelimiter);
			}
			if(ItemPrefixSubstitution.IsNotEmpty()) {
				preformatedString = preformatedString.Replace(ItemPrefixSubstitution, ItemPrefix);
			}
			if(ItemSuffixSubstitution.IsNotEmpty()) {
				preformatedString = preformatedString.Replace(ItemSuffixSubstitution, ItemSuffix);
			}
			if(ValueDelimiterSubstitution.IsNotEmpty()) {
				preformatedString = preformatedString.Replace(ValueDelimiterSubstitution, ValueDelimiter);
			}
			return preformatedString;
		}

		/// <summary>
		/// 	Парсит входную строку как комплексную
		/// </summary>
		/// <param name="complexstring"> </param>
		/// <returns> </returns>
		public IDictionary<string, string> Parse(string complexstring) {
			var result = new Dictionary<string, string>();
			if (complexstring.IsEmpty()) {
				return result;
			}
			if(OptimizedParser || !string.IsNullOrEmpty(ItemDelimiter)) {
				ParseCS(complexstring, result);
			}
			else {
				ParseTags(complexstring, result);
			}
			return result;
		}

		private void ParseTags(string complexstring, Dictionary<string, string> result) {
			var regex = ParseRegex;
			regex.Replace(complexstring, m =>
				{
					var name = UnEscape(m.Groups["name"].Value).Trim();
					var val = UnEscape(m.Groups["value"].Value).Trim();
					if (val.IsEmpty()) {
						val = EmptyValue;
					}
					result[name] = val;
					return "";
				});
		}

		private void ParseCS(string complexstring, Dictionary<string, string> result) {
			char splitter = OptimizedParser ? ItemPrefix[0] : ItemDelimiter[0];
			var items = complexstring.SmartSplit(false, true, splitter);
			foreach (var item in items) {
				var pair = item.SmartSplit(false, true, ValueDelimiter[0]);
				var name = UnEscape(pair[0]);
				var val = "";
				if (pair.Count > 1) {
					val = UnEscape(pair[1]);
				}
				if (val.IsEmpty()) {
					val = EmptyValue;
				}
				result[name] = val;
			}
		}

		/// <summary>
		/// 	Возвращает значение из комплексной строки
		/// </summary>
		/// <param name="complexstring"> </param>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public string GetValue(string complexstring, string name) {
			var dict = Parse(complexstring);
			if (dict.ContainsKey(name)) {
				return dict[name];
			}
			return NotExistedValue;
		}

		/// <summary>
		/// 	Формирует строку
		/// </summary>
		/// <param name="dict"> </param>
		/// <returns> </returns>
		public string Generate(IDictionary<string, string> dict) {
			if (null == dict) {
				return string.Empty;
			}
			if (!string.IsNullOrEmpty(ItemDelimiter)) {
				return GenerateCsLike(dict);
			}
			return GenerateTagLike(dict);
		}

		private string GenerateTagLike(IEnumerable<KeyValuePair<string, string>> dict) {
			var result = new StringBuilder();
			foreach (var i in dict) {
				var name = Escape(i.Key);
				var val = Escape(i.Value);
				result.Append(ItemPrefix);
				result.Append(name);
				result.Append(ValueDelimiter);
				result.Append(val);
				result.Append(ItemSuffix);
				result.Append(" ");
			}
			return result.ToString().Trim();
		}

		private string GenerateCsLike(IEnumerable<KeyValuePair<string, string>> dict) {
			var result = new StringBuilder();
			var idx = 0;
			foreach (var i in dict) {
				var name = Escape(i.Key);
				var val = Escape(i.Value);
				if (0 != idx) {
					result.Append(ItemDelimiter);
				}
				result.Append(name);
				if (val.IsNotEmpty()) {
					result.Append(ValueDelimiter);
					result.Append(val);
				}
				idx++;
			}
			return result.ToString();
		}

		/// <summary>
		/// 	Создает парсер строк вида /x:val/ /y/ /z:~\~/
		/// </summary>
		/// <returns> </returns>
		public static ComplexStringHelper CreateTagParser() {
			return new ComplexStringHelper
				{
					ItemPrefix = "/",
					ItemSuffix = "/",
					ItemDelimiter = "",
					ValueDelimiter = ":",
					NotExistedValue = "",
					EmptyValue = "",
					ItemPrefixSubstitution = "`",
					ItemSuffixSubstitution = "`",
					ValueDelimiterSubstitution = "~",
					OptimizedParser = true,
				};
		}

		/// <summary>
		/// Двух-разделительный парсер обрабатывается как сплитовой
		/// </summary>
		public bool OptimizedParser  { get; set; }

		/// <summary>
		/// 	Создает парсер строк вида <see cref="ComplexStringExtension" />
		/// </summary>
		/// <returns> </returns>
		public static ComplexStringHelper CreateComplexStringParser() {
			return new ComplexStringHelper
				{
					ItemPrefix = "",
					ItemSuffix = "",
					ItemDelimiter = "|",
					ValueDelimiter = ":",
					NotExistedValue = "",
					EmptyValue = "1",
					ItemDelimiterSubstitution = "`",
					ValueDelimiterSubstitution = "~"
				};
		}

		/// <summary>
		/// 	Создает парсер строк вида <see cref="ComplexStringExtension" />
		/// </summary>
		/// <returns> </returns>
		public static ComplexStringHelper CreateWSComplexStringParser()
		{
			return new ComplexStringHelper
			{
				ItemPrefix = "",
				ItemSuffix = "",
				ItemDelimiter = " ",
				ValueDelimiter = ":",
				NotExistedValue = "",
				EmptyValue = "1",
				ItemDelimiterSubstitution = "`",
				ValueDelimiterSubstitution = "~"
			};
		}

		/// <summary>
		/// 	Автоматически определяет тип комплексной строки
		/// </summary>
		/// <param name="complexString"> </param>
		/// <returns> </returns>
		/// <exception cref="QorpentException"></exception>
		public static ComplexStringHelper AutoDetect(string complexString) {
			if (complexString.Trim().StartsWith("/")) {
				return CreateTagParser();
			}
			if(complexString.Contains("|")) {
				return CreateComplexStringParser();
			}
			return CreateWSComplexStringParser();
		}

		private Regex _parseregex;
	}
}
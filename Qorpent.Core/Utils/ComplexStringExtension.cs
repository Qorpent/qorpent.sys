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
// PROJECT ORIGIN: Qorpent.Utils/ComplexStringExtension.cs
#endregion
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
	/// <summary>
	/// 	parses string in form NAME1|NAME2:VAL2|NAME3|NAME4... into dictionary and perform some update operations on such strings
	/// </summary>
	public static class ComplexStringExtension {
		private const string Regexmask = @"(^|\|){0}(:[^\|]+)?(\||$)";
	
		
		
		/// <summary>
		/// 	updates/inserts specified name into string
		/// </summary>
		/// <param name="target"> </param>
		/// <param name="name"> </param>
		/// <param name="value"> </param>
		/// <returns> </returns>
		public static string Set(string target, string name, string value = null) {
			if (value.IsEmpty() && IsComplex(name)) {
				return Set(target, Parse(name));
			}
			if (name.IsEmpty()) {
				return target;
			}
			var val = value.IsEmpty() ? name : string.Format("{0}:{1}", name, value);
			if (target.IsEmpty()) {
				return val;
			}
			var regex = new Regex(string.Format(Regexmask, name));
			var result = target;
			if (regex.IsMatch(target)) {
				result = regex.Replace(target, "|" + val + "|");
			}
			else {
				result += "|" + val;
			}
			return Cleanup(result);
		}

		private static string Cleanup(string result) {
			result = result.Replace("||", "|");
			if (result.StartsWith("|")) {
				result = result.Substring(1);
			}
			if (result.EndsWith("|")) {
				result = result.Substring(0, result.Length - 1);
			}
			return result;
		}

		/// <summary>
		/// 	Set dictionary into string
		/// </summary>
		/// <param name="target"> </param>
		/// <param name="data"> </param>
		/// <returns> </returns>
		public static string Set(string target, IEnumerable<KeyValuePair<string, string>> data) {
			return data.Aggregate(target, (current, p) => Set(current, p.Key, p.Value));
		}

		/// <summary>
		/// 	remove given value from string
		/// </summary>
		/// <param name="target"> </param>
		/// <param name="nameorvalue"> </param>
		/// <returns> </returns>
		public static string Remove(string target, string nameorvalue) {
			if (IsComplex(nameorvalue)) {
				return Remove(target, Parse(nameorvalue));
			}
			if (target.IsEmpty()) {
				return string.Empty;
			}
			var result = Regex.Replace(target, string.Format(Regexmask, nameorvalue), "$1");
			return Cleanup(result);
		}

		/// <summary>
		/// 	remove given dictionary from string
		/// </summary>
		/// <param name="target"> </param>
		/// <param name="data"> </param>
		/// <returns> </returns>
		public static string Remove(string target, IEnumerable<KeyValuePair<string, string>> data) {
			return data.Aggregate(target, (current, p) => Remove(current, p.Key));
		}


		private static bool IsComplex(string str) {
			if (str.IsEmpty()) {
				return false;
			}
			return -1 != str.IndexOfAny(new[] {'|', ':'});
		}

		/// <summary>
		/// 	parses complex string into dictionary
		/// </summary>
		/// <param name="str"> </param>
		/// <param name="useTrueInstedOfEmpty"> </param>
		/// <param name="partdelimiter"> </param>
		/// <param name="valuedelimiter"> </param>
		/// <returns> </returns>
		public static IDictionary<string, string> Parse(string str, bool useTrueInstedOfEmpty = false,
		                                                char partdelimiter = '|', char valuedelimiter = ':') {
			var result = new Dictionary<string, string>();
			if (str.IsNotEmpty()) {
				var items = str.SmartSplit(false, true, partdelimiter);
				foreach (var item in items) {
					var pair = item.SmartSplit(false, true, valuedelimiter);
					var name = pair[0];
					var val = useTrueInstedOfEmpty ? "true" : "";
					if (pair.Count > 1) {
						val = pair[1];
					}
					result[name] = val;
				}
			}
			return result;
		}

	    public static string SetList(string first, string second, char delimiter = ';', char valdelimiter=':') {
	        var list1 = Parse(first, partdelimiter: delimiter,valuedelimiter:valdelimiter);
	        var list2 = Parse(second, partdelimiter: delimiter, valuedelimiter: valdelimiter);
	        foreach (var p  in list2 ) {
	            list1[p.Key] = p.Value;
	        }
	        return string.Join(delimiter.ToString(),
	            list1.Select(_ => string.IsNullOrWhiteSpace(_.Value) ? _.Key : (_.Key + valdelimiter + _.Value.ToStr())));
	    }
	}
}
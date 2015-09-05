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
// PROJECT ORIGIN: Qorpent.Utils/TagHelper.cs
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.Model;

namespace Qorpent.Utils.Extensions{
    /// <summary>
    /// Устаревший хелпер для работы с тегами, оставленный для совместимости
    /// </summary>
    public static class TagHelper{
        /// <summary>
        /// Конвертирует словарь в таг-строку
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static string ToString(IDictionary<string,string> tags){
            var result = new StringBuilder();
            foreach (var tag in tags){
                result.Append(" /" + tag.Key + ":" + Escape(tag.Value) + "/");
            }
            return result.ToString().Trim();
        }
        /// <summary>
        /// Конвертирует словарь в таг-строку
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static string ToString(IDictionary<string, object> tags)
        {
            var result = new StringBuilder();
            foreach (var tag in tags)
            {
                result.Append(" /" + tag.Key + ":" + Escape(tag.Value.ToStr()) + "/");
            }
            return result.ToString().Trim();
        }
        /// <summary>
        /// Производит экранизаци
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string Escape(string val) {
            return val.Replace(":", "~").Replace("/", "`");
        }

        /// <summary>
        /// Производит де-экранизацию значения
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string UnEscape(string val) {
            return val.Replace("~", ":").Replace("`", "/");
        }
        /// <summary>
        /// Производит сопоставление тега с маской
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="tagmask"></param>
        /// <returns></returns>
        public static bool Match(string tag, string tagmask) {
            var srcdict = Parse(tag);
            var maskdict = Parse(tagmask);
            
            foreach (var md in maskdict) {
                var key = md.Key;
                var testval = md.Value;
                var negateKey = key.StartsWith("!");
                key = negateKey ? key.Substring(1):key;
                var negateValue = testval.StartsWith("!");
                testval = negateValue ? testval.Substring(1) : testval;
                var found = false;
                if (key.StartsWith("*")) {
                    found = MatchWithRegexKey(srcdict, key.Substring(1),testval,negateKey,negateValue);
                }
                else {
                    if (negateKey) {
                        found = !srcdict.ContainsKey(key);
                    }
                    else {
                        if (srcdict.ContainsKey(key)) {
                            var val = srcdict[key];
                            found = TestValue(val, testval, negateValue);
                        }
                    }
                }
                if(!found)return false;
            }
            return true;
        }

        private static bool TestValue(string val, string testval, bool negate) {
            bool valtested;
            if (val == "*") {
                valtested = true;
            }
            else if (testval.StartsWith("*")) {
                valtested = Regex.IsMatch(val, testval.Substring(1));
            }
            else {
                valtested = val == testval;
            }
            return negate?!valtested:valtested;
        }

        private static bool MatchWithRegexKey(IEnumerable<KeyValuePair<string, string>> srcdict, string key,string value, bool negatekey,bool negatevalue) {
            var found = false;
            if (negatekey) {
                found = srcdict.All(_ => !Regex.IsMatch(_.Key, key));
            }
            else {
                foreach (var pair in srcdict) {
                    if (Regex.IsMatch(pair.Key, key)) {
                        found = TestValue(pair.Value, value, negatevalue);
                    }
                    if (found) break;
                }
            }

            return found;
        }

        /// <summary>
		/// Убирает таг из строки
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="tag"></param>
		/// <returns></returns>
        public static string RemoveTag(string tags, string tag)
        {

            var trg = Parse(tags);
            if (trg.ContainsKey(tag))
            {
                trg.Remove(tag);
            }
            return ToString(trg);
        }

		/// <summary>
		/// Устанавливает таг в строке
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="tagname"></param>
		/// <param name="value"></param>
		/// <returns></returns>
        public static string SetValue(string tags, string tagname, string value)
        {
            var dict = Parse(tags);
            if (null == value)
            {
                if (dict.ContainsKey(tagname))
                {
                    dict.Remove(tagname);
                }
            }
            else
            {
                dict[tagname] = value;
            }
            return ToString(dict);
        }
		/// <summary>
		/// Объединяет две таг строки
		/// </summary>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string Merge(string target, string source) {
			return ToString(Merge(Parse(target), Parse(source)));
		}
		/// <summary>
		/// Объединяет два словаря
		/// </summary>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <returns></returns>
        public static IDictionary<string, string> Merge(IDictionary<string, string> target, IDictionary<string, string> source)
        {
            foreach (var val in source){
                if(val.Value==null || val.Value=="DELETE"){
                    target.Remove(val.Key);
                }
                else target[val.Key] = val.Value;
            }
            return target;
        }
		/// <summary>
		/// Проверяет, есть ли у строки заданный тег
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="tagname"></param>
		/// <returns></returns>
        public static bool Has(string tags, string tagname) {
            var dict = Parse(tags);
            return dict.ContainsKey(tagname);
        }
        /// <summary>
        /// Возвращает значение тага
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="tagname"></param>
        /// <returns></returns>
        public static string Value (string tags, string tagname){
            var dict = Parse(tags);
			if(dict.ContainsKey(tagname)) return UnEscape(dict[tagname]);
	        return "";
        }
		/// <summary>
		/// Парсит строку тегов в словарь
		/// </summary>
		/// <param name="tags"></param>
		/// <returns></returns>
        public static IDictionary<string ,string > Parse (string tags){
            var result = new Dictionary<string, string>();
            if (tags.IsEmpty()) return result;
            if (tags.Contains("/")){
				Regex.Replace(tags,@"/([^:/]+):([^:/]*)/", m => {
                                                   result[m.Groups[1].Value] = m.Groups[2].Value;
                                                   return m.Value;
                                               },RegexOptions.Compiled);
            }else{
                foreach (var s in tags.SmartSplit(false, true, ' '))
                {
                    if (s.Contains(":"))
                    {
                        result[s.Split(':')[0]] = UnEscape(s.Split(':')[1]);
                    }
                    else
                    {
                        result[s] = "";
                    }
                }
            }
            return result;
        }

        public static string TagGet(this IWithTag tagged, string name) {
            return TagHelper.Value(tagged.Tag, name);
        }
    }
}
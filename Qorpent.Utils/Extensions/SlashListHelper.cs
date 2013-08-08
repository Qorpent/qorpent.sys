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
// PROJECT ORIGIN: Qorpent.Utils/SlashListHelper.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.Extensions{
    /// <summary>
    /// Устаревший вспомогательный класс для разобра строк из Zeta
    /// </summary>
    [Obsolete("zeta compatible")]
    public static class SlashListHelper{
		/// <summary>
		/// Преобразует строку в перечисление элементов
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
        public static IEnumerable<string> ReadList(string src){
            if(null==src) return new string[]{};
            return src.SmartSplit(false, true, '/',';').Distinct().ToArray();
        }
		private static string getDelimiter(string src) {
			if (src.Contains(";")) return ";";
			return "/";
		}
		/// <summary>
		/// Устанавливает элемент в строке
		/// </summary>
		/// <param name="src"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
        public static string SetMark(string src, string mark){
            if(src==null) src = "";
        	var delimiter = getDelimiter(src);
            RemoveMark(src, mark);
            src += delimiter + mark + delimiter;
            src = src.Replace(delimiter+delimiter, delimiter);
            return src;
        }
		/// <summary>
		/// Убирает элемент из строки
		/// </summary>
		/// <param name="src"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
        public static string RemoveMark(string src, string  mark){
            if(string.IsNullOrWhiteSpace(src)) return "";
        	var delimiter = getDelimiter(src);
            var result= src.Replace(delimiter + mark + delimiter, delimiter).Replace(delimiter+delimiter, delimiter);
			if (result == "/") {
				result = "";
			}
			return result;
		}
		/// <summary>
		/// Проверяет наличие элемента в строек
		/// </summary>
		/// <param name="src"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
        public static bool HasMark(string src, string  mark){
            if(string.IsNullOrWhiteSpace(src)||string.IsNullOrWhiteSpace(mark)) return false;
        	var delimiter = getDelimiter(src);
            return src.Contains(delimiter + mark + delimiter);
        }
		/// <summary>
		/// Конвертирует список в строку с конкатенацией
		/// </summary>
		/// <param name="strings"></param>
		/// <returns></returns>
        public static string ToString(IEnumerable<string> strings){
            if(null==strings||strings.Count()==0) return "";
            return ("/" + strings.Where(x=>!string.IsNullOrWhiteSpace(x)).Distinct().ConcatString("/") + "/").Replace("//", "/");
        }
    }
}
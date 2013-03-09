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
            return src.Replace(delimiter + mark + delimiter, delimiter).Replace(delimiter+delimiter, delimiter);
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
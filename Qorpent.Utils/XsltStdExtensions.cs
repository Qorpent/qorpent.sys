using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Qorpent.Utils
{
    /// <summary>
    /// Класс со стандартными утилитами для XSLT
    /// </summary>
    public class XsltStdExtensions
    {
        /// <summary>
        /// Проверяет соответствие значения регулярному выражению
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool rx_ismatch(string input, string pattern)
        {
            return Regex.IsMatch(input, pattern, RegexOptions.Compiled);
        }

        /// <summary>
        /// Возвращает результат выполнения регулярного выражения относительно входной строки
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public string rx_match(string input, string pattern) {
            var result = Regex.Match(input, pattern,RegexOptions.Compiled);
            if (result.Success) return result.Value;
            return "";
        }

        /// <summary>
        /// Возвращает результат замены строки по регулярному выражению
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public string rx_replace(string input, string pattern, string replace) {
            return Regex.Replace(input, pattern, replace, RegexOptions.Compiled);
        }
        /// <summary>
        /// Возвращает результат выполнения регулярного выражения с возвратом конкретной группы
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pattern"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public string rx_match_g(string input, string pattern, string group)
        {
            var result = Regex.Match(input, pattern, RegexOptions.Compiled);
            if (result.Success) return result.Groups[group].Value;
            return "";
        }
    }
}

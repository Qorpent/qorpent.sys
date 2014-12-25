using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.Utils.Extensions;

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
        /// <summary>
        /// Возвращает форматированный диапазон даты-времени
        /// </summary>
        /// <param name="baseDateTime"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public string dt_range(string baseDateTime, string range) {
            var resultRange = baseDateTime.ToDate().CalculateRange(range);
            return string.Format("{0} - {1}", resultRange.Start.ToString("dd.MM.yyyy HH:mm:ss"),
                resultRange.Finish.ToString("dd.MM.yyyy HH:mm:ss"));
        }
        /// <summary>
        /// выбор уникальных значений
        /// </summary>
        /// <param name="iter"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public XPathNodeIterator distinct_values(XPathNodeIterator iter, string xpath) {
            var values = iter.OfType<XPathNavigator>().Select(_ => _.Evaluate(xpath).ToStr()).Distinct();
            var xe = new XElement("x");
            xe.Add(values.Select(_=>new XElement("v",_)));
            var nav = xe.CreateNavigator();
            return nav.Select("//v");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Dot
{
    /// <summary>
    /// Языковые утилиты DOT
    /// </summary>
    public static class DotLanguageUtils
    {

        
 

        /// <summary>
        /// Код нулл-узла
        /// </summary>
        public const string NULLCODE = "NULL";

        /// <summary>
        /// Префикс ненормального символа в коде
        /// </summary>
        public const string NONLITERALSUBSTPREFIX = "_0x";

        /// <summary>
        /// Формат представления UNICODE
        /// </summary>
        public const string HEXUNICODE = "X4";
        /// <summary>
        /// Размер записи о символе
        /// </summary>
        public const int HEXSIZE = 4;
        /// <summary>
        /// Символ пустой строки
        /// </summary>
        public const string EMPTYSTRING = "\"\"";
       /// <summary>
       /// ФОрмат представления чисел
       /// </summary>
        public const string NUMBERFORMAT = "0.####";

        /// <summary>
        /// Приводит код узла к нормальной форме
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EscapeCode(string code) {
            if (IsLiteral(code)) return code;
            if (String.IsNullOrWhiteSpace(code)) return NULLCODE;
            var fst = code[0];
            var needprefix =!( Char.IsLetter(fst)||('_'==fst) );
           
            var sb = new StringBuilder();
            if (needprefix) {
                sb.Append(NONLITERALSUBSTPREFIX);
                sb.Append(((int) code[0]).ToString(HEXUNICODE));
            }
            else {
                sb.Append(code[0]);
            }
            foreach (var c in code.Skip(1)) {
                if (IsLiteral(c)) {
                    sb.Append(c);
                }
                else {
                    sb.Append(NONLITERALSUBSTPREFIX);
                    sb.Append(((int) c).ToString(HEXUNICODE));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string UnEscapeCode(string code) {
            if (-1 == code.IndexOf(NONLITERALSUBSTPREFIX)) {
                return code;
            }
            var sb = new StringBuilder();
            int curidx = 0;
            while (true) {
                var idx = code.IndexOf(NONLITERALSUBSTPREFIX, curidx);
                if (-1 == idx) {
                    sb.Append(code.Substring(curidx));
                    break;
                }
                else {
                    if (idx > curidx) {
                        sb.Append(code.Substring(curidx, idx - curidx));
                    }
                    idx += NONLITERALSUBSTPREFIX.Length;
                    var hex = code.Substring(idx, HEXSIZE);
                    curidx = idx+HEXSIZE;
                    sb.Append((char)short.Parse(hex, NumberStyles.HexNumber));
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Проверяет что символ - литерал
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsLiteral(char c) {
            return Char.IsLetterOrDigit(c) || (c == '_');
        }
        /// <summary>
        /// Проверка строки на то что она литерал
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsLiteral(string s) {
            if (string.IsNullOrWhiteSpace(s)) return false;
            if (!char.IsLetter(s[0])) return false;
            return s.All(IsLiteral);
        }
        /// <summary>
        /// Готовит атрибут к рендерингу
        /// </summary>
        /// <param name="attrname"></param>
        /// <param name="attrvalue"></param>
        /// <returns></returns>
        public static string GetAttributeString(string attrname, object attrvalue) {
            if (null == attrvalue) return EMPTYSTRING;
            if (attrvalue is int) return attrvalue.ToString();
            if (attrvalue is decimal) return ((decimal) attrvalue).ToString(NUMBERFORMAT, CultureInfo.InvariantCulture);
            if (attrvalue is double) return ((double)attrvalue).ToString(NUMBERFORMAT, CultureInfo.InvariantCulture);
            if (attrvalue is bool) return ((bool) attrvalue).ToString().ToLower();
            if (attrvalue is NodeStyleType) {
                return ((NodeStyleType) attrvalue).GetAttributeString();
            }
            if (attrvalue is NodeShapeType) {
                return ((NodeShapeType) attrvalue).GetAttributeString();
            }
        }
    }
}

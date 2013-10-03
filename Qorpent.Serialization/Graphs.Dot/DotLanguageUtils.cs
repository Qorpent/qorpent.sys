using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Qorpent.Graphs.Dot.Types;
using Qorpent.Utils.Extensions;

namespace Qorpent.Graphs.Dot
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
        /// Кавычка
        /// </summary>
        public const string QUOT = "\"";
        /// <summary>
        /// Признак атрибута с URL
        /// </summary>
        public const string ATTRWITHURL = "URL";
        /// <summary>
        /// Признак атрибута с HREF
        /// </summary>
        public const string ATTRWITHHREF = "href";
        /// <summary>
        /// Открывашка элемента с таблицей
        /// </summary>
        public const string OPENTABLE = "<";
        /// <summary>
        /// Закрывашка элемента с таблицей
        /// </summary>
        public const string CLOSETABLE = ">";

        /// <summary>
        /// Приводит код узла к нормальной форме
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EscapeCode(string code) {
        //    if (code == "node") return "__esc_node";
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
        /// Приводит код узла к нормальной форме
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EscapeScript(string code)
        {
   
            var sb = new StringBuilder();
            foreach (var c in code)
            {
                if (c<=127)
                {
                    sb.Append(c);
                }
                else
                {
                   sb.Append("&#x"+((int)c).ToString(HEXUNICODE)+";");
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
           // if (code == "__esc_node") return "node";
            if (-1 == code.IndexOf(NONLITERALSUBSTPREFIX, StringComparison.Ordinal)) {
                return code;
            }
            var sb = new StringBuilder();
            var curidx = 0;
            while (true) {
                var idx = code.IndexOf(NONLITERALSUBSTPREFIX, curidx, StringComparison.Ordinal);
                if (-1 == idx) {
                    sb.Append(code.Substring(curidx));
                    break;
                }
                if (idx > curidx) {
                    sb.Append(code.Substring(curidx, idx - curidx));
                }
                idx += NONLITERALSUBSTPREFIX.Length;
                var hex = code.Substring(idx, HEXSIZE);
                curidx = idx+HEXSIZE;
                sb.Append((char)short.Parse(hex, NumberStyles.HexNumber));
            }
            return sb.ToString();
        }
        /// <summary>
        /// Проверяет что символ - литерал
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsLiteral(char c) {
            return (Char.IsLetterOrDigit(c) || (c == '_')) && c<=127;
        }
        /// <summary>
        /// Проверка строки на то что она литерал
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsLiteral(string s) {
            if (string.IsNullOrWhiteSpace(s)) return false;
            var fst = s[0];
            if (!(fst=='_'||char.IsLetter(s[0]))) return false;
            return s.All(IsLiteral);
        }
        /// <summary>
        /// Конвертирует переданную строку в код кластера
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetClusterCode(string code) {
            if (!code.StartsWith("cluster"))
            {
                code = "cluster_" + code;
            }
            return EscapeCode(code);

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
            if (attrvalue is string) {
                return GetStringAttributeValue(attrname,(string) attrvalue);
            }
            if (attrvalue is NodeStyleType) {
                return ((NodeStyleType) attrvalue).GetAttributeString();
            }
            if (attrvalue is NodeShapeType) {
                return ((NodeShapeType) attrvalue).GetAttributeString();
            }
            if (attrvalue is RankDirType) {
                return ((RankDirType) attrvalue).ToString();
            }
            
            if (attrvalue.GetType().IsEnum) {
                return attrvalue.ToString().ToLower();
            }
            if (attrvalue is ColorAttribute) {
                var ca = attrvalue as ColorAttribute;
                if (ca.Mode == ColorAttributeType.Multiple || !IsLiteral(ca.ToString())) {
                    return QUOT + ca + QUOT;
                }
                return ca.ToString();
            }
            return attrvalue.ToString();
        }
        
        private static string GetStringAttributeValue(string attrname, string attrvalue) {
            if (attrname == "label" && attrvalue.StartsWith("<TABLE")) {
                return OPENTABLE + attrvalue.GetUnicodeSafeXmlString()+CLOSETABLE;
            }
            if (string.IsNullOrEmpty(attrvalue)) return EMPTYSTRING;
            if (attrvalue == "node") return QUOT + attrvalue + QUOT;
            var safe = attrvalue.GetUnicodeSafeXmlString(escapeQuots:true);
            //в случае литерала и отсутствия признаков изменений без кавычек
            if (IsLiteral(safe) && safe==attrvalue) {
                return safe;
            }
            if (attrname.Contains(ATTRWITHURL) || attrname.Contains(ATTRWITHHREF)) {
                safe = Uri.EscapeUriString(safe).Replace("+", Uri.HexEscape('+'));
            }
            return QUOT + safe + QUOT;
        }
       
        
    }
}

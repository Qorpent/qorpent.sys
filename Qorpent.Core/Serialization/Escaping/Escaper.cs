using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization
{
    /// <summary>
    /// Univarsal character escaper
    /// </summary>
    public static class Escaper{
	    /// <summary>
	    /// Ключевые слова C#
	    /// </summary>
	    public static readonly string[] CSharpKeywords = new[]{
		    "abstract", "add", "as", "ascending",
		    "async", "await", "base", "bool",
		    "break", "by", "byte", "case",
		    "catch", "char", "checked", "class",
		    "const", "continue", "decimal", "default",
		    "delegate", "descending", "do", "double",
		    "dynamic", "else", "enum", "equals","event",
		    "explicit", "extern", "false", "finally",
		    "fixed", "float", "for", "foreach",
		    "from", "get", "global", "goto",
		    "group", "if", "implicit", "in",
		    "int", "interface", "internal", "into",
		    "is", "join", "let", "lock",
		    "long", "namespace", "new", "null",
		    "object", "on", "operator", "orderby",
		    "out", "override", "params", "partial",
		    "private", "protected", "public", "readonly",
		    "ref", "remove", "return", "sbyte",
		    "sealed", "select", "set", "short",
		    "sizeof", "stackalloc", "static", "string",
		    "struct", "switch", "this", "throw",
		    "true", "try", "typeof", "uint",
		    "ulong", "unchecked", "unsafe", "ushort",
		    "using", "value", "var", "virtual",
		    "void", "volatile", "where", "while",
		    "yield"
	    };

	    
		/// <summary>
		/// Экранирует ключевые слова C#
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string EscapeCSharpKeyword(string str){
			if (-1 != Array.IndexOf(CSharpKeywords, str)){
				return "@" + str;
			}
			return str;
		}

        private static IDictionary<char, string> _translitMap = new Dictionary<char, string> {

            {'а', "a"},
            {'б', "b"},
            {'в', "v"},
            {'г', "g"},
            {'д', "d"},
            {'е', "e"},
            {'ё', "e"},
            {'ж', "zh"},
            {'з', "z"},
            {'и', "i"},
            {'й', "y"},
            {'к', "k"},
            {'л', "l"},
            {'м', "m"},
            {'н', "n"},
            {'о', "o"},
            {'п', "p"},
            {'р', "r"},
            {'с', "s"},
            {'т', "t"},
            {'у', "u"},
            {'ф', "f"},
            {'х', "h"},
            {'ц', "ts"},
            {'ч', "ch"},
            {'ш', "sh"},
            {'щ', "sh"},
            {'ъ', ""},
            {'ы', "i"},
            {'ь', ""},
            {'э', "e"},
            {'ю', "yu"},
            {'я', "ya"},

        };
        /// <summary>
        /// Make rus-eng translit
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Translit(string str) {
            if (string.IsNullOrWhiteSpace(str)) {
                return string.Empty;
            }
            var sb = new StringBuilder();
            foreach (var c in str) {
                var ic = char.ToLowerInvariant(c);
                if (_translitMap.ContainsKey(ic)) {
                    var oc = _translitMap[ic];
                    if (ic != c) {
                        if (oc.Length == 1) {
                            oc = oc.ToUpperInvariant();
                        }
                        else {
                            oc = oc.Substring(0, 1).ToUpperInvariant() + oc.Substring(1);
                        }
                    }
                    sb.Append(oc);
                }
                else {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Escape russian organization name to transliterated system name
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string OrganizationSysName(string s) {
            if (string.IsNullOrWhiteSpace(s)) {
                return string.Empty;
            }
            s = Regex.Replace(s, @"^\s*(ООО)|(ОАО)|(ПО)|(ТС)|(ЗАО)|(ИП)\s+", "");
            s = Regex.Replace(s, @"[\""'\-]", "");
            s = s.Trim().ToLowerInvariant();
            s = Regex.Replace(s, @"\s+", " ");
            var words = s.SmartSplit(false, true, ' ');
            if (words.Count <= 2) {
                s = string.Join("_", words);
            }
            else {
                s = words[0] + "_" + string.Join("", words.Skip(1).Select(_ => _.Substring(0, 1)));
            }
            s = Translit(s);
            return s;
        }

        /// <summary>
        /// Escape all symbols for given type
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static String Escape(this String str, EscapingType type)
        {
            if (type == EscapingType.BxlSinglelineString || type == EscapingType.BxlMultilineString)
            {
                if (string.IsNullOrEmpty(str))
                {
                    return "\"\"";
                }
                if (type == EscapingType.BxlSinglelineString)
                {
                    return ToBxlSingleLineString(str);
                }
                if (type == EscapingType.BxlMultilineString)
                {
                    return ToBxlMultiLineString(str);
                }
            }else if (type == EscapingType.BxlStringOrLiteral) {
				if (string.IsNullOrEmpty(str))
				{
					return "\"\"";
				}
				if (str.IsLiteral(EscapingType.BxlLiteral ) && !str.Contains("\\") && !str.Contains("#")) {
					return str;
				}
				
				return ToBxlSingleLineString(str);
				
            }else if(type==EscapingType.JsonValue)
	        {
				//идентично
			        return ToBxlSingleLineString(str,false);
		        
	        }
			if (string.IsNullOrEmpty(str))
			{
				return string.Empty;
			}

            IEscapeProvider d = EscapingDataFactory.Get(type);
            var sb = new StringBuilder(str.Length);

			if (!IsLiteral(str[0],type,true)) {
				sb.Append(EscapeFirst(str[0], d));
			}
			else {
				sb.Append(str[0]);
			}
            
            for (int i = 1; i < str.Length; i++)
	            if (!IsLiteral(str[i],type)) {
		            sb.Append(EscapeCommon(str[i], d));
	            }
	            else {
		            sb.Append(str[i]);
	            }

	        return sb.ToString();
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToSqlString( this object str ){
			return str.ToStr().Replace("'", "''");
		}

	    /// <summary>
	    /// Заключает строку в кавычки
	    /// </summary>
	    /// <param name="s"></param>
	    /// <param name="lowercase">Приводить к нижнему регистру</param>
	    /// <returns></returns>
	    public static string SqlQuoteName(this string s,bool lowercase = true){
			if (string.IsNullOrWhiteSpace(s)){
				return "\"\"";
			}
			if (-1 == s.IndexOfAny(new[] { '"', '.' }))
			{
				return "\"" + s.ToLowerInvariant() + "\"";
			}
			var sb = new StringBuilder();
			sb.Append('"');
			for (var i = 0; i < s.Length; i++){
				var c = s[i];
				if (c == '.'){
					sb.Append('"');
					sb.Append('.');
					sb.Append('"');
				}else  if (c == '"'){
					continue;
				}
				else{
					if (lowercase){
						sb.Append(char.ToLower(c));
					}
					else{
						sb.Append(c);
					}
					
				}
			}
			sb.Append('"');
			return sb.ToString();
		}
		

        private static string ToBxlMultiLineString(string str) {
            if (-1 == str.IndexOf('\r') && -1 == str.IndexOf('\n')) {
                return ToBxlSingleLineString(str);
            }
            return "\"\"\"" + str + "\"\"\"";
        }

        private static string ToBxlSingleLineString(string str, bool enquote = true) {
            if (-1 != str.IndexOf('\\')) {
                str = str.Replace("\\", "\\\\");
            }
            if (-1 != str.IndexOf('\r')) {
                str = str.Replace("\r", "\\r");
            }
            if (-1 != str.IndexOf('\n'))
            {
                str = str.Replace("\n", "\\n");
            }
            if (-1 != str.IndexOf('\t'))
            {
                str = str.Replace("\t", "\\t");
            }
            if (-1 != str.IndexOf('\'')) {
                str = str.Replace("'", "\\\'");
            }
			if (-1 != str.IndexOf('\"'))
			{
				str = str.Replace("\"", "\\\"");
			}
	        if (enquote) str = "'" + str + "'";
            return str;
        }

        /// <summary>
        /// Unescape all symbols (auto type)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static String Unescape(this String str)
        {



            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="type"></param>
        /// <param name="first"></param>
        /// <returns></returns>
        public static bool IsLiteral(this char c, EscapingType type, bool first = false)
        {
            IEscapeProvider d = EscapingDataFactory.Get(type);
            return !(first && d.GetFirst().ContainsKey(c)
                    || d.GetCommon().ContainsKey(c)
                    || d.NeedEscapeUnicode(c));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsLiteral(this String str, EscapingType type)
        {
            IEscapeProvider d = EscapingDataFactory.Get(type);

            if (d.GetFirst().ContainsKey(str[0]))
                return false;
            foreach (char c in str)
                if (d.GetCommon().ContainsKey(c))
                    return false;
            return true;
        }

        private static String EscapeFirst(char c, IEscapeProvider d)
        {
            String r;
            if (d.GetFirst().TryGetValue(c, out r))
                return r;
            return EscapeCommon(c, d);
        }

        private static String EscapeCommon(char c, IEscapeProvider d)
        {
            String r;
            if (d.GetCommon().TryGetValue(c, out r))
                return r;
            return EscapeUnicode(c, d);
        }

        private static String EscapeUnicode(char c, IEscapeProvider d)
        {
            // escaping not defined
            if (d.GetUnicodePattern() == null)
                return c.ToString();

            if (!d.NeedEscapeUnicode(c))
                return c.ToString();

            return d.GetUnicodePattern().Replace(" ", ((int)c).ToString("X4"));
        }


        /// <summary>
        /// Unscape all symbols for given type
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static String Unescape(this String str, EscapingType type)
        {
            IEscapeProvider d = EscapingDataFactory.Get(type);
            StringBuilder res = new StringBuilder(str.Length);

            int offset = 0;
            foreach (var i in d.GetFirst())
            {
                if (str.StartsWith(i.Value))
                {
                    res.Append(i.Key);
                    offset = i.Value.Length;
                    break;
                }
            }

            for (int i = offset; i < str.Length; i++)
            {
                String s = CheckSuffix(str, offset, i, d);
                if (s != null)
                {
                    res.Append(s);
                    offset = i + 1;
                }
            }

            return res + str.Substring(offset);
        }

        private static String CheckSuffix(String str, int start, int end, IEscapeProvider d)
        {
            String s;
            // the length of escaping code not constant so check all variants
            foreach (int j in d.GetUnescape().KeysLength)
            {
                if (j > end - start + 1)
                    continue;

                s = str.Substring(end - j + 1, j);
                s = UnescapeEntity(s, d);
                if (s != null)
                    return str.Substring(start, end - start - j + 1) + s;
            }

            // check unicode
            if (d.GetUnicodePattern() == null)
                return null;

            int p = d.GetUnicodePattern().Length + 3;
            if (p > end - start + 1)
                return null;

            s = str.Substring(end - p + 1, p);
            s = UnescapeUnicode(s, d);
            if (s != null)
                return str.Substring(start, end - start - p + 1) + s;

            return null;
        }

        private static String UnescapeEntity(this String s, IEscapeProvider d)
        {
            char r;
            if (d.GetUnescape().TryGetValue(s, out r))
                return r.ToString();

            return null;
        }

        private static String UnescapeUnicode(this String s, IEscapeProvider d)
        {
            String p = d.GetUnicodePattern();
            if (p == null)
                return null;
            if (p.Length + 3 == s.Length)
            {
                String hex = s.Substring(p.IndexOf(' '), 4);
                ushort value;
                if (ushort.TryParse(hex, NumberStyles.HexNumber, null, out value))
                    return ((char)value).ToString();
            }

            return null;
        }

        /// <summary>
        /// Возвращает строку в PascalCase
        /// </summary>
        /// <param name="s"></param>
        /// <param name="escapeWhiteSpaces"></param>
        /// <returns></returns>
        public static string PascalCase(string s, bool escapeWhiteSpaces = true) {
            if (string.IsNullOrEmpty(s)) return "";
            if (escapeWhiteSpaces) {
                if (s.Contains(" ")) {
                    var res = new StringBuilder();
                    foreach (var n in s.Split(' ')) {
                        res.Append(PascalCase(n));
                    }
                    return res.ToString();
                }
            }
            return (s[0].ToString().ToUpper() + s.Substring(1)).Replace(" ", "_");
        }
    }
}

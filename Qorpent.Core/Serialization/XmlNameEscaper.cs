using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Qorpent.Serialization
{
    /// <summary>
    /// преобразовывает спецсимволы из xml в bxl и обратно
    /// </summary>
    public static class XmlNameEscaper
    {
        private static readonly String _digits = "0123456789";

        private static readonly Dictionary<char, string> _xml_common = new Dictionary<char, string>()
            {
                {'+', "__PLUS__"},
                {'?', "__ASK__"},
                {'!', "__EXC__"},
                {'~', "__TILD__"},
                {'@', "__AT__"},
                {'*', "__STAR__"},
                {'$', "__USD__"},
                {'^', "__UP__"},
                {'&', "__AMP__"},
                {'/', "__DIV__"},
                {':', "__DBL__"},
                {'%', "__PERC__"},
                {'(', "__LBRACE__"},
                {')', "__RBRACE__"},
                {'[', "__LINDEX__"},
                {']', "__RINDEX__"},
                {'{', "__LBLOCK__"},
                {'}', "__RBLOCK__"},
                {'|', "__VLINE__"},
                {';', "__PERIOD__"},
                {'<', "__LT__"},
                {'>', "__GT__"},
            };

        private static readonly Dictionary<char, string> _xml_first = new Dictionary<char, string>()
            {
                {'-', "__MINUS__"},
                {'.', "__DOT__"}
            };

        private static readonly Dictionary<string, char> _bxl = new Dictionary<string, char>()
            {
                {"__PLUS__", '+'},
                {"__MINUS__", '-'},
                {"__ASK__", '?'},
                {"__EXC__", '!'},
                {"__TILD__", '~'},
                {"__AT__", '@'},
                {"__STAR__", '*'},
                {"__USD__", '$'},
                {"__UP__", '^'},
                {"__AMP__", '&'},
                {"__DIV__", '/'},
                {"__DBL__", ':'},
                {"__PERC__", '%'},
                {"__LBRACE__", '('},
                {"__RBRACE__", ')'},
                {"__LINDEX__", '['},
                {"__RINDEX__", ']'},
                {"__LBLOCK__", '{'},
                {"__RBLOCK__", '}'},
                {"__VLINE__", '|'},
                {"__PERIOD__", ';'},
                {"__LT__", '<'},
                {"__GT__", '>'},
                {"__DOT__", '.'}
            };


        /// <summary>
        /// Converts specials symbols to equivalent string (e.g. "+" => "__PLUS__")
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static String EscapeEntity(this char c)
        {
            String r;
            if (_xml_first.TryGetValue(c, out r))
                return r;          
            return EscapeCommon(c);
        }

        private static String EscapeCommon(this char c)
        {
            String r;
            if (_xml_common.TryGetValue(c, out r))
                return r;
            return EscapeUnicode(c);
        }

        private static String EscapeFirst(this char c)
        {
            if (_digits.Contains(c))
                return "_" + c;

            return EscapeEntity(c);
        }

        private static String EscapeUnicode(this char c)
        {
            // standard ASCII
            if (c < 128)
                return c.ToString();
            // Russian
            if (c >= 0x0410 && c <= 0x044f || c == 0x0401 || c == 0x451)
                return c.ToString();

            return "__0x" + ((int)c).ToString("X4") + "__";
        }



        /// <summary>
        /// Convert String representation to special symbols (e.g. "__PLUS__" => "+")
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static String UnescapeEntity(this String c)
        {
            String r = AnotherUnescapeEntity(c);
            return r ?? c;
        }

        private static String AnotherUnescapeEntity(this String c)
        {
            char r;
            if (_bxl.TryGetValue(c, out r))
                return r.ToString();

            if (c.Length == 10)
            {
                String hex = c.Substring(4, 4);
                ushort value;
                if (ushort.TryParse(hex, NumberStyles.HexNumber, null, out value))
                    return ((char) value).ToString();
            }

            return null;
        }

        /// <summary>
        /// EscapeEntity all symbols
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static String EscapeXmlName(this String txt)
        {
            StringBuilder sb = new StringBuilder(txt.Length);
            
            sb.Append(EscapeFirst(txt[0]));
            for (int i = 1; i < txt.Length; i++)
                sb.Append(EscapeCommon(txt[i]));

            return sb.ToString();
        }

        /// <summary>
        /// UnescapeEntity all symbols
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static String UnescapeXmlName(this String txt)
        {
            StringBuilder res = new StringBuilder(txt.Length);
            StringBuilder buf = new StringBuilder(txt.Length);

            if (txt[0] == '_' && _digits.Contains(txt[1]))
                res.Append(txt[1]);

            // if first digit is detected starts scaning string from third symbol
            for (int i = 0 + res.Length * 2; i < txt.Length; i++)
            {
                buf.Append(txt[i]);
                res.Append(CheckSuffix(buf));
            }

            res.Append(buf);
            return res.ToString();
        }

        /// <summary>
        /// Check if c is literal (not special symbol) 
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsLiteral(char c)
        {
            return !_xml_common.ContainsKey(c) && !_xml_first.ContainsKey(c);
        }

        /// <summary>
        /// Checks the given buffer for escape codes at the end
        /// </summary>
        /// <param name="buf"></param>
        /// <returns>whole buffer content with replaced escape code at the end or "" if the code not correct </returns>
        private static String CheckSuffix(StringBuilder buf)
        {
            int n = buf.Length;
            if (n < 3) return "";
            n--;
            if (buf[n] == '_' && buf[n - 1] == '_')
            {
                n -= 2;
                for (int i = n - 1; i >= 0; i--)
                {
                    if (buf[i] == '_' && buf[i + 1] == '_')
                    {
                        String suffix = buf.ToString(i, n - i + 3);
                        String b = AnotherUnescapeEntity(suffix);
                        if (b == null)
                            return "";

                        String prefix = buf.ToString(0, i);
                        buf.Clear();
                        return prefix + b;
                    }
                }
            }
            return "";
        }
    }
}

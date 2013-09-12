using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Serialization
{
    /// <summary>
    /// преобразовывает спецсимволы из xml в bxl и обратно
    /// </summary>
    public class XmlEscaper
    {
        private static readonly Dictionary<String, String> _xml = new Dictionary<string, string>()
            {
                {"+", "__PLUS__"},
                {"-", "__MINUS__"},
                {"?", "__ASK__"},
                {"!", "__EXC__"},
                {"~", "__TILD__"},
                {"@", "__AT__"},
                {"*", "__STAR__"},
                {"$", "__USD__"},
                {"^", "__UP__"},
                {"&", "__AMP__"},
                {"/", "__DIV__"},
                {":", "__DBL__"},
                {"%", "__PERC__"},
                {"(", "__LBRACE__"},
                {"},", "__RBRACE__"},
                {"[", "__LINDEX__"},
                {"]", "__RINDEX__"},
                {"{", "__LBLOCK__"},
                {"}", "__RBLOCK__"},
                {"|", "__VLINE__"},
                {";", "__PERIOD__"},
                {"<", "__LT__"},
                {">", "__GT__"},
            };

        private static readonly Dictionary<String, String> _bxl = new Dictionary<string, string>()
            {
                {"__PLUS__", "+"},
                {"__MINUS__", "-"},
                {"__ASK__", "?"},
                {"__EXC__", "!"},
                {"__TILD__", "~"},
                {"__AT__", "@"},
                {"__STAR__", "*"},
                {"__USD__", "$"},
                {"__UP__", "^"},
                {"__AMP__", "&"},
                {"__DIV__", "/"},
                {"__DBL__", ":"},
                {"__PERC__", "%"},
                {"__LBRACE__", "("},
                {"__RBRACE__", "},"},
                {"__LINDEX__", "["},
                {"__RINDEX__", "]"},
                {"__LBLOCK__", "{"},
                {"__RBLOCK__", "}"},
                {"__VLINE__", "|"},
                {"__PERIOD__", ";"},
                {"__LT__", "<"},
                {"__GT__", ">"}
            };

        /// <summary>
        /// Converts specials symbols to equivalent string (e.g. "+" => "__PLUS__")
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static String Escape(String c)
        {
            String r = c;
            if (_xml.TryGetValue(c, out r))
                return r;
            return c;
        }

        /// <summary>
        /// Convert String representation to special symbols (e.g. "__PLUS__" => "+")
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static String Unescape(String c)
        {
            String r = c;
            if (_bxl.TryGetValue(c, out r))
                return r;
            return c;
        }

        /// <summary>
        /// Escape all symbols
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static String EscapeAll(String txt)
        {
            foreach (String c in _xml.Keys)
            {
                txt = txt.Replace(c, _xml[c]);
            }
            return txt;
        }

        /// <summary>
        /// Unescape all symbols
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static String UnescapeAll(String txt)
        {
            foreach (String c in _bxl.Keys)
            {
                txt = txt.Replace(c, _bxl[c]);
            }
            return txt;
        }
    }
}

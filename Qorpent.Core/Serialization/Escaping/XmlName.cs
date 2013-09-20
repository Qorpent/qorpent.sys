using System.Collections.Generic;

namespace Qorpent.Serialization
{
    /// <summary>
    /// <see cref="EscapingType.XmlName" />
    /// </summary>
    class XmlName : IData
    {
        private static readonly Dictionary<char, string> _common = new Dictionary<char, string>()
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

                {'=', "__EQ__"},
                {',', "__COMMA__"},
                {'"', "__QUOT__"},
                {'\'', "__APOS__"},
                {' ', "__SPACE__"},
                {'\t', "__TAB__"},
                {'\n', "__NLINE"},
            };

        private static readonly Dictionary<char, string> _first = new Dictionary<char, string>()
            {
                {'-', "__MINUS__"},
                {'.', "__DOT__"},
                {'0', "_0"},
                {'1', "_1"},
                {'2', "_2"},
                {'3', "_3"},
                {'4', "_4"},
                {'5', "_5"},
                {'6', "_6"},
                {'7', "_7"},
                {'8', "_8"},
                {'9', "_9"},
            };

        private static readonly OptimizedEscapeDictionary _unescape = new OptimizedEscapeDictionary()
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
                {"__DOT__", '.'},

                {"__EQ__", '='},
                {"__COMMA__", ','},
                {"__QUOT__", '"'},
                {"__APOS__", '\''},
                {"__SPACE__", ' '},
                {"__TAB__", '\t'},
                {"__NLINE", '\n'},
            };

        public Dictionary<char, string> GetFirst()
        {
            return _first;
        }

        public Dictionary<char, string> GetCommon()
        {
            return _common;
        }

        public OptimizedEscapeDictionary GetUnescape()
        {
            return _unescape;
        }

        public string GetUnicodePattern()
        {
            return "__0x __";
        }

        public bool NeedEscapeUnicode(char c)
        {
            // standard ASCII exclude control characters
            if (c >= 32 && c <= 127)
                return false;
            // Russian
            if (c >= 0x0410 && c <= 0x044f || c == 0x0401 || c == 0x0451)
                return false;

            return true;
        }
    }
}

using System.Collections.Generic;

namespace Qorpent.Serialization
{
    /// <summary>
    /// <see cref="EscapingType.BxlLiteral" />
    /// </summary>
    class BxlLiteral : IData
    {
        protected static readonly Dictionary<char, string> _common = new Dictionary<char, string>()
            {
                {'=', "__EQ__"},
                {',', "__COMMA__"},
                {':', "__DBL__"},
                {'"', "__QUOT__"},
                {'\'', "__APOS__"},
                {' ', "__SPACE__"},
                {'\t', "__TAB__"},
                {'\n', "__NLINE"},
            };

        protected static readonly Dictionary<char, string> _first = new Dictionary<char, string>();

        protected static readonly OptimizedEscapeDictionary _unescape = new OptimizedEscapeDictionary()
            {
                {"__EQ__", '='},
                {"__COMMA__", ','},
                {"__DBL__", ':'},
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
            return null;
        }
    }
}

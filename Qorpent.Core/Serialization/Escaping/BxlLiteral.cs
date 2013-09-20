using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Serialization.Escaping
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

        protected static readonly MyDictionary _unescape = new MyDictionary()
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

        public MyDictionary GetUnescape()
        {
            return _unescape;
        }

        public string GetUnicodePattern()
        {
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Serialization.Escaping
{
    /// <summary>
    /// <see cref="EscapingType.JsonLiteral" />
    /// </summary>
    class JsonLiteral : IData
    {
        private static readonly Dictionary<char, string> _common = new Dictionary<char, string>()
            {
                {'\\', "__BSLASH__"},
                {'"', "__QUOT__"},
            };

        private static readonly Dictionary<char, string> _first = new Dictionary<char, string>();

        private static readonly MyDictionary _unescape = new MyDictionary()
            {
                {"__BSLASH__", '\\'},
                {"__QUOT__", '"'},
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
            return "\\u ";
        }

        public bool NeedEscapeRussian()
        {
            return false;
        }
    }
}

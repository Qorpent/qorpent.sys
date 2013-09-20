using System.Collections.Generic;

namespace Qorpent.Serialization
{
    /// <summary>
    /// <see cref="EscapingType.XmlAttribute"/>
    /// </summary>
    class XmlAttribute : IData
    {
        protected static readonly Dictionary<char, string> _common = new Dictionary<char, string>()
            {
                {'&', "&amp;"},
                {'<', "&lt;"},
                {'>', "&gt;"},
                {'"', "&quot;"},
                {'\'', "&apos;"},
            };

        protected static readonly Dictionary<char, string> _first = new Dictionary<char, string>(); //empty

        protected static readonly OptimizedEscapeDictionary _unescape = new OptimizedEscapeDictionary()
            {
                {"&amp;", '&'},
                {"&lt;", '<'},
                {"&gt;", '>'},
                {"&quot;", '"'},
                {"&apos;", '\''},
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
            return "&#x ;";
        }
    }
}

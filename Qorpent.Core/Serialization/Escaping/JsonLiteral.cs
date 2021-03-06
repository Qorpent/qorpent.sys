﻿using System.Collections.Generic;

namespace Qorpent.Serialization.Escaping
{
    /// <summary>
    /// <see cref="JsonLiteral" />
    /// </summary>
    class JsonLiteral : IEscapeProvider
    {
        private static readonly Dictionary<char, string> _common = new Dictionary<char, string>()
            {
                {'\\', "__BSLASH__"},
                {'"', "__QUOT__"},
                {',', "__COMMA__"},
                {' ',"__SP__" },
                {'-',"__MINUS__" },
                 {'.',"__DOT__" },
            };

        private static readonly Dictionary<char, string> _first = new Dictionary<char, string>();

        private static readonly OptimizedEscapeDictionary _unescape = new OptimizedEscapeDictionary()
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

        public OptimizedEscapeDictionary GetUnescape()
        {
            return _unescape;
        }

        public string GetUnicodePattern()
        {
            return "\\u ";
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

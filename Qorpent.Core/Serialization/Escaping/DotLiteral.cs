using System;
using System.Collections.Generic;

namespace Qorpent.Serialization
{
    /// <summary>
    /// <see cref="DotLiteral"/>
    /// </summary>
    class DotLiteral : IEscapeProvider
    {
        protected static readonly String _uni = "_0x ";
        protected static readonly Dictionary<char, string> _common = new Dictionary<char, string>();

        protected static readonly Dictionary<char, string> _first = new Dictionary<char, string>()
            {
                {'0', _uni.Replace(" ", ((int) '0').ToString("X4"))},
                {'1', _uni.Replace(" ", ((int) '1').ToString("X4"))},
                {'2', _uni.Replace(" ", ((int) '2').ToString("X4"))},
                {'3', _uni.Replace(" ", ((int) '3').ToString("X4"))},
                {'4', _uni.Replace(" ", ((int) '4').ToString("X4"))},
                {'5', _uni.Replace(" ", ((int) '5').ToString("X4"))},
                {'6', _uni.Replace(" ", ((int) '6').ToString("X4"))},
                {'7', _uni.Replace(" ", ((int) '7').ToString("X4"))},
                {'8', _uni.Replace(" ", ((int) '8').ToString("X4"))},
                {'9', _uni.Replace(" ", ((int) '9').ToString("X4"))},
            };
        protected static readonly OptimizedEscapeDictionary _unescape = new OptimizedEscapeDictionary();

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
            return _uni;
        }

        public bool NeedEscapeUnicode(char c)
        {
            return !(Char.IsLetterOrDigit(c) || (c == '_'));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Escaping
{
    /// <summary>
    /// Univarsal character escaper
    /// </summary>
    public static class Escaper
    {
        /// <summary>
        /// Escape all symbols for given type
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static String Escape(this String str, EscapingType type)
        {
            IData d = EscapingDataFactory.Get(type);
            StringBuilder sb = new StringBuilder(str.Length);

            sb.Append(EscapeFirst(str[0], d));
            for (int i = 1; i < str.Length; i++)
                sb.Append(EscapeCommon(str[i], d));

            return sb.ToString();
        }

        /// <summary>
        /// Unescape all symbols (auto type)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static String Unescape(this String str)
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
        public static bool IsLiteral(char c, EscapingType type, bool first = false)
        {
            IData d = EscapingDataFactory.Get(type);
            return !(first && d.GetFirst().ContainsKey(c)
                    || d.GetCommon().ContainsKey(c));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsLiteral(String str, EscapingType type)
        {
            IData d = EscapingDataFactory.Get(type);

            if (d.GetFirst().ContainsKey(str[0]))
                return false;
            foreach (char c in str)
                if (d.GetCommon().ContainsKey(c))
                    return false;
            return true;
        }

        private static String EscapeFirst(char c, IData d)
        {
            String r;
            if (d.GetFirst().TryGetValue(c, out r))
                return r;
            return EscapeCommon(c, d);
        }

        private static String EscapeCommon(char c, IData d)
        {
            String r;
            if (d.GetCommon().TryGetValue(c, out r))
                return r;
            return EscapeUnicode(c, d);
        }

        private static String EscapeUnicode(char c, IData d)
        {
            // escaping not defined
            if (d.GetUnicodePattern() == null)
                return c.ToString();

            // standard ASCII
            if (c < 128)
                return c.ToString();
            // Russian
            if (c >= 0x0410 && c <= 0x044f || c == 0x0401 || c == 0x0451)
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
            IData d = EscapingDataFactory.Get(type);
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

        private static String CheckSuffix(String str, int start, int end, IData d)
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

        private static String UnescapeEntity(this String s, IData d)
        {
            char r;
            if (d.GetUnescape().TryGetValue(s, out r))
                return r.ToString();

            return null;
        }

        private static String UnescapeUnicode(this String s, IData d)
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
    }
}

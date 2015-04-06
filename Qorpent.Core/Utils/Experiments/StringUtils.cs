using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Qorpent.Utils {



    /// <summary>
    /// String-awared utils
    /// </summary>
    public static class StringUtils {
        private static readonly string[] EmptyStrings = {};
        private static readonly char[] DefaultSplitters = {',', ';', '/'};

        /// <summary>
        /// Parses string as array of strings
        /// </summary>
        /// <param name="source"></param>
        /// <param name="splitters"></param>
        /// <param name="trim"></param>
        /// <returns></returns>
        public static unsafe string[] ReadAsStrings(string source, char[] splitters = null, bool trim = true)
        {
            if (string.IsNullOrWhiteSpace(source)) return EmptyStrings;
            splitters = splitters ?? DefaultSplitters;
            if (0 == splitters.Length) return new[] { source };
            var list = new List<string>();
            var i = 0;
            var size = source.Length;
            fixed (char* s = source) {
                while (i<size) {
                    var next = source.IndexOfAny(splitters,i);
                    if (-1 == next) {
                        next = size;
                    }
                    var end = next-1;
                    if (trim) {
                        bool ltrimmed = false;
                        bool rtrimmed = false;
                        while (true) {
                            if (i > end) break;
                            if (!ltrimmed) {
                                var l = *(s + i);
                                if (!char.IsWhiteSpace(l)) {
                                    ltrimmed = true;
                                }
                                else {
                                    i += 1;
                                }
                            }
                            if (!rtrimmed) {
                                var r = *(s + end);
                                if (!char.IsWhiteSpace(r)) {
                                    rtrimmed = true;
                                }
                                else {
                                    end -= 1;
                                }
                            }
                            if (ltrimmed && rtrimmed) {
                                break;
                            }
                        }
                    }
                    if (i <= end) {
                       list.Add(new string(s,i,end-i+1));
                    }
                    i = next + 1;
                }
            }
            var result = new string[list.Count];
            list.CopyTo(result, 0);
            return result;
        }

    }
} ;
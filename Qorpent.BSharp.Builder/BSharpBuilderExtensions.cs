using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.BSharp.Builder;

namespace Qorpent.Integration.BSharp.Builder {
    /// <summary>
    /// 
    /// </summary>
    public static class BSharpBuilderExtensions {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBxlsprojPath(this String str) {
            var position = str.LastIndexOf(
                BSharpBuilderDefaults.DefaultBSharpProjectExtension,
                StringComparison.Ordinal
            );

            if (position != -1) {
                if (position + 9 == str.Length) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBxlsExtension(this String str) {
            if (str.IsBxlsprojPath()) {
                return true;
            }

            if (str.Contains(BSharpBuilderDefaults.DefaultInputExtension)) {
                return true;
            }

            return false;
        }
        /// <summary>
        ///     Склонировать список
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone"></param>
        /// <returns></returns>
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}
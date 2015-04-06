using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Utils {
    /// <summary>
    ///     Набор расширений для работы со списками и перечислениями
    /// </summary>
    public static class ListUtils {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T TryGetValue<T>(this IList<T> list, int index) {
            if (index > list.Count - 1) {
                return new List<T>().FirstOrDefault();
            } else {
                return list[index];
            }
        }
    }
}
